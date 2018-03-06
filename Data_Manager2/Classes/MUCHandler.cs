using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Logging;
using System;
using System.Threading.Tasks;
using Thread_Save_Components.Classes.Collections;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048_1_0;

namespace Data_Manager2.Classes
{
    public class MUCHandler
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly MUCHandler INSTANCE = new MUCHandler();
        private TSTimedList<MUCJoinHelper> timedList;
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
            timedList = new TSTimedList<MUCJoinHelper>
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
            timedList.addTimed(helper);

            await helper.enterRoomAsync();
        }

        public async Task leaveRoomAsync(XMPPClient client, ChatTable muc, MUCChatInfoTable info)
        {
            stopMUCJoinHelper(muc);

            MUCDBManager.INSTANCE.setMUCState(info.chatId, MUCState.DISCONNECTING, true);
            await sendMUCLeaveMessageAsync(client, muc, info);
            MUCDBManager.INSTANCE.setMUCState(info.chatId, MUCState.DISCONNECTED, true);
        }

        /// <summary>
        /// Creates a new Task and sends all bookmarks to the server.
        /// </summary>
        /// <param name="client">The XMPPClient which bookmarks should get updated.</param>
        /// <param name="cI">The ConferenceItem that should get updated.</param>
        /// <returns>Returns the Task created by this call.</returns>
        public Task updateBookmarks(XMPPClient client, ConferenceItem cI)
        {
            return client.setBookmarkAsync(cI);
        }

        #endregion

        #region --Misc Methods (Private)--
        private void stopMUCJoinHelper(ChatTable muc)
        {
            foreach (MUCJoinHelper h in timedList.getEntries())
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
            await client.sendMessageAsync(msg, false);
        }

        private void enterAllMUCs(XMPPClient client)
        {
            Task.Run(async () =>
            {
                foreach (ChatTable muc in ChatDBManager.INSTANCE.getAllMUCs(client.getXMPPAccount().getIdAndDomain()))
                {
                    MUCChatInfoTable info = MUCDBManager.INSTANCE.getMUCInfo(muc.id);
                    if (info == null)
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
                        MUCDBManager.INSTANCE.setMUCState(info.chatId, MUCState.ERROR, true);

                        // Add an error chat message:
                        ChatMessageTable msg = new ChatMessageTable()
                        {
                            id = ChatMessageTable.generateErrorMessageId(errorMessage.getId() ?? AbstractMessage.getRandomId(), muc.id),
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
                string room = Utils.getBareJidFromFullJid(msg.getFrom());
                if (room == null)
                {
                    return;
                }
                string chatId = ChatTable.generateId(room, client.getXMPPAccount().getIdAndDomain());

                MUCMemberTable member = MUCDBManager.INSTANCE.getMUCMember(chatId, msg.FROM_NICKNAME);
                if (member == null)
                {
                    member = new MUCMemberTable()
                    {
                        id = MUCMemberTable.generateId(chatId, msg.FROM_NICKNAME),
                        nickname = msg.FROM_NICKNAME,
                        chatId = chatId
                    };
                }

                member.affiliation = msg.AFFILIATION;
                member.role = msg.ROLE;

                bool isUnavailable = Equals(msg.TYPE, "unavailable");
                if (isUnavailable)
                {
                    if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE))
                    {
                        // Nickname got changed by user or room:
                        if (msg.STATUS_CODES.Contains(MUCPresenceStatusCode.MEMBER_NICK_CHANGED) || msg.STATUS_CODES.Contains(MUCPresenceStatusCode.ROOM_NICK_CHANGED))
                        {
                            MUCDBManager.INSTANCE.setNickname(chatId, msg.NICKNAME, true);
                            return;
                        }
                        else
                        {
                            MUCDBManager.INSTANCE.setMUCState(chatId, MUCState.DISCONNECTED, true);
                        }
                    }
                }

                // If the type equals 'unavailable', a user left the room:
                MUCDBManager.INSTANCE.setMUCMember(member, isUnavailable, true);
            });
        }

        private async void Client_NewValidMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewValidMessageEventArgs args)
        {
            if (args.getMessage() is MUCErrorMessage)
            {
                await onMUCErrorMessageAsync(client, args.getMessage() as MUCErrorMessage);
            }
        }

        #endregion
    }
}
