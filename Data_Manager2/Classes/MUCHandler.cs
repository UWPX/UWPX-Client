using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using Shared.Classes.Collections;
using System;
using System.Threading.Tasks;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace Data_Manager2.Classes
{
    public class MUCHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string TYPE_CHAT_INFO = "chat_info";

        public static readonly MUCHandler INSTANCE = new MUCHandler();
        private readonly TSTimedList<MUCJoinHelper> TIMED_LIST;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/01/2018 Created [Fabian Sauter]
        /// </history>
        public MUCHandler()
        {
            TIMED_LIST = new TSTimedList<MUCJoinHelper>
            {
                itemTimeoutInMs = (int)TimeSpan.FromSeconds(20).TotalMilliseconds
            };
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void onClientConnected(XMPPClient client)
        {
            client.NewMUCMemberPresenceMessage -= C_NewMUCMemberPresenceMessage;
            client.NewMUCMemberPresenceMessage += C_NewMUCMemberPresenceMessage;
            client.NewValidMessage -= Client_NewValidMessage;
            client.NewValidMessage += Client_NewValidMessage;

            MUCDBManager.INSTANCE.resetMUCState(client.getXMPPAccount().getIdAndDomain(), true);

            if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_AUTO_JOIN_MUC))
            {
                Logger.Info("Entering all MUC rooms for '" + client.getXMPPAccount().getIdAndDomain() + '\'');
                enterAllMUCs(client);
            }
        }

        public void onClientDisconnected(XMPPClient client)
        {

        }

        public void onClientDisconnecting(XMPPClient client)
        {
            client.NewMUCMemberPresenceMessage -= C_NewMUCMemberPresenceMessage;
            client.NewValidMessage -= Client_NewValidMessage;
            MUCDBManager.INSTANCE.resetMUCState(client.getXMPPAccount().getIdAndDomain(), true);
        }

        public void onMUCRoomSubjectMessage(MUCRoomSubjectMessage mucRoomSubject)
        {
            string to = Utils.getBareJidFromFullJid(mucRoomSubject.getTo());
            string from = Utils.getBareJidFromFullJid(mucRoomSubject.getFrom());
            string id = ChatTable.generateId(from, to);

            MUCDBManager.INSTANCE.setMUCSubject(id, mucRoomSubject.SUBJECT, true);
        }

        public async Task enterMUCAsync(XMPPClient client, ChatTable muc, MUCChatInfoTable info)
        {
            MUCJoinHelper helper = new MUCJoinHelper(client, muc, info);
            TIMED_LIST.addTimed(helper);

            await helper.enterRoomAsync();
        }

        public async Task leaveRoomAsync(XMPPClient client, ChatTable muc, MUCChatInfoTable info)
        {
            stopMUCJoinHelper(muc);

            MUCDBManager.INSTANCE.setMUCState(info.chatId, MUCState.DISCONNECTING, true);
            await sendMUCLeaveMessageAsync(client, muc, info);
            MUCDBManager.INSTANCE.setMUCState(info.chatId, MUCState.DISCONNECTED, true);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void stopMUCJoinHelper(ChatTable muc)
        {
            foreach (MUCJoinHelper h in TIMED_LIST.getEntries())
            {
                if (Equals(h.MUC.id, muc.id))
                {
                    h.Dispose();
                }
            }
        }

        private async Task sendMUCLeaveMessageAsync(XMPPClient client, ChatTable muc, MUCChatInfoTable info)
        {
            string from = client.getXMPPAccount().getIdDomainAndResource();
            string to = muc.chatJabberId + '/' + info.nickname;
            LeaveRoomMessage msg = new LeaveRoomMessage(from, to);
            await client.sendAsync(msg, false);
        }

        private void enterAllMUCs(XMPPClient client)
        {
            Task.Run(async () =>
            {
                foreach (ChatTable muc in ChatDBManager.INSTANCE.getAllMUCs(client.getXMPPAccount().getIdAndDomain()))
                {
                    MUCChatInfoTable info = MUCDBManager.INSTANCE.getMUCInfo(muc.id);
                    if (info is null)
                    {
                        info = new MUCChatInfoTable()
                        {
                            chatId = muc.id,
                            state = MUCState.DISCONNECTED,
                            nickname = muc.userAccountId,
                            autoEnterRoom = true
                        };
                        MUCDBManager.INSTANCE.setMUCChatInfo(info, false, true);
                    }
                    if (info.autoEnterRoom)
                    {
                        await enterMUCAsync(client, muc, info);
                    }
                }
            });
        }

        private async Task onMUCErrorMessageAsync(XMPPClient client, MUCErrorMessage errorMessage)
        {
            string room = Utils.getBareJidFromFullJid(errorMessage.getFrom());
            if (room != null)
            {
                string chatId = ChatTable.generateId(room, client.getXMPPAccount().getIdAndDomain());
                ChatTable muc = ChatDBManager.INSTANCE.getChat(chatId);
                if (muc != null)
                {
                    MUCChatInfoTable info = MUCDBManager.INSTANCE.getMUCInfo(chatId);
                    if (info != null)
                    {
                        Logger.Error("Received an error message for MUC: " + muc.chatJabberId + "\n" + errorMessage.ERROR_MESSAGE);

                        stopMUCJoinHelper(muc);

                        if (info.state != MUCState.DISCONNECTED)
                        {
                            await sendMUCLeaveMessageAsync(client, muc, info);
                        }

                        switch (errorMessage.ERROR_CODE)
                        {
                            // No access - user got baned:
                            case 403:
                                MUCDBManager.INSTANCE.setMUCState(info.chatId, MUCState.BANED, true);
                                addChatInfoMessage(info.chatId, room, "Unable to join room!\nYou are baned from this room.");
                                return;

                            default:
                                MUCDBManager.INSTANCE.setMUCState(info.chatId, MUCState.ERROR, true);
                                break;
                        }

                        // Add an error chat message:
                        ChatMessageTable msg = new ChatMessageTable()
                        {
                            id = ChatMessageTable.generateErrorMessageId(errorMessage.ID ?? AbstractMessage.getRandomId(), muc.id),
                            chatId = muc.id,
                            date = DateTime.Now,
                            fromUser = errorMessage.getFrom(),
                            isImage = false,
                            message = "Code: " + errorMessage.ERROR_CODE + "\nType: " + errorMessage.ERROR_TYPE + "\nMessage:\n" + errorMessage.ERROR_MESSAGE,
                            state = MessageState.UNREAD,
                            type = MessageMessage.TYPE_ERROR
                        };
                        ChatDBManager.INSTANCE.setChatMessage(msg, true, false);
                    }
                }
            }
        }

        private void addOccupantKickedMessage(string chatId, string roomJid, string nickname)
        {
            string msg = nickname + " got kicked from the room.";
            addChatInfoMessage(chatId, roomJid, msg);
        }

        private void addOccupantBanedMessage(string chatId, string roomJid, string nickname)
        {
            string msg = nickname + " got baned from the room.";
            addChatInfoMessage(chatId, roomJid, msg);
        }

        private void addChatInfoMessage(string chatId, string fromUser, string message)
        {
            ChatMessageTable msg = new ChatMessageTable
            {
                id = ChatMessageTable.generateId(AbstractMessage.getRandomId(), chatId),
                chatId = chatId,
                date = DateTime.Now,
                fromUser = fromUser,
                isImage = false,
                message = message,
                state = MessageState.UNREAD,
                type = TYPE_CHAT_INFO
            };
            ChatDBManager.INSTANCE.setChatMessage(msg, true, false);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void C_NewMUCMemberPresenceMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewMUCMemberPresenceMessageEventArgs args)
        {
            Task.Run(() =>
            {
                MUCMemberPresenceMessage msg = args.mucMemberPresenceMessage;
                string roomJid = Utils.getBareJidFromFullJid(msg.getFrom());
                if (roomJid is null)
                {
                    return;
                }
                string chatId = ChatTable.generateId(roomJid, client.getXMPPAccount().getIdAndDomain());

                MUCOccupantTable member = MUCDBManager.INSTANCE.getMUCOccupant(chatId, msg.FROM_NICKNAME);
                if (member is null)
                {
                    member = new MUCOccupantTable
                    {
                        id = MUCOccupantTable.generateId(chatId, msg.FROM_NICKNAME),
                        nickname = msg.FROM_NICKNAME,
                        chatId = chatId
                    };
                }

                member.affiliation = msg.AFFILIATION;
                member.role = msg.ROLE;
                member.jid = msg.JID;

                bool isUnavailable = Equals(msg.TYPE, "unavailable");
                if (isUnavailable)
                {
                    if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE))
                    {
                        // Nickname got changed by user or room:
                        if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_NICK_CHANGED) || msg.STATUS_CODES.Contains(MUCPresenceStatusCode.ROOM_NICK_CHANGED))
                        {
                            // Update MUC info:
                            MUCDBManager.INSTANCE.setMUCInfoNickname(chatId, msg.NICKNAME, true);

                            // Add new member:
                            MUCDBManager.INSTANCE.setMUCOccupant(new MUCOccupantTable
                            {
                                id = MUCOccupantTable.generateId(chatId, msg.NICKNAME),
                                nickname = msg.NICKNAME,
                                chatId = member.chatId,
                                affiliation = member.affiliation,
                                role = member.role,
                                jid = member.jid,
                            }, false, true);
                        }
                        // Occupant got kicked:
                        else if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_GOT_KICKED))
                        {
                            // Update MUC state:
                            MUCDBManager.INSTANCE.setMUCState(chatId, MUCState.KICKED, true);
                        }
                        else if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_GOT_BANED))
                        {
                            // Update MUC state:
                            MUCDBManager.INSTANCE.setMUCState(chatId, MUCState.BANED, true);
                        }
                        else
                        {
                            // Update MUC state:
                            MUCDBManager.INSTANCE.setMUCState(chatId, MUCState.DISCONNECTED, true);
                        }
                    }


                    if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_GOT_KICKED))
                    {
                        // Add kicked chat message:
                        addOccupantKickedMessage(chatId, roomJid, member.nickname);
                    }

                    if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_GOT_BANED))
                    {
                        // Add baned chat message:
                        addOccupantBanedMessage(chatId, roomJid, member.nickname);
                    }
                }

                // If the type equals 'unavailable', a user left the room:
                MUCDBManager.INSTANCE.setMUCOccupant(member, isUnavailable, true);
            });
        }

        private async void Client_NewValidMessage(IMessageSender sender, XMPP_API.Classes.Network.Events.NewValidMessageEventArgs args)
        {
            if (args.MESSAGE is MUCErrorMessage)
            {
                await onMUCErrorMessageAsync((XMPPClient)sender, args.MESSAGE as MUCErrorMessage);
            }
        }

        #endregion
    }
}
