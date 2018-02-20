using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System;
using System.Threading.Tasks;
using Thread_Save_Components.Classes.Collections;
using XMPP_API.Classes;
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

            MUCDBManager.INSTANCE.resetMUCState(client.getXMPPAccount().getIdAndDomain(), true);

            if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_AUTO_JOIN_MUC))
            {
                enterAllMUCs(client);
            }
        }

        public void onClientDisconnected(XMPPClient client)
        {

        }

        public void onClientDisconnecting(XMPPClient client)
        {
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

            if (info.password != null)
            {
                await helper.enterRoomAsync();
            }
            else
            {
                await helper.enterRoomAsync();
            }
        }

        public async Task leaveRoomAsync(XMPPClient client, ChatTable muc, MUCChatInfoTable info)
        {
            MUCDBManager.INSTANCE.setMUCState(info.chatId, MUCState.DISCONNECTING, true);
            string from = client.getXMPPAccount().getIdDomainAndResource();
            string to = muc.chatJabberId + '/' + info.nickname;
            LeaveRoomMessage msg = new LeaveRoomMessage(from, to);
            await client.sendMessageAsync(msg, false);
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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void C_NewMUCMemberPresenceMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewMUCMemberPresenceMessageEventArgs args)
        {
            MUCMemberPresenceMessage msg = args.mucMemberPresenceMessage;
            string room = Utils.getBareJidFromFullJid(msg.getFrom());
            if (room == null)
            {
                return;
            }
            string chatId = ChatTable.generateId(room, client.getXMPPAccount().getIdAndDomain());

            MUCMemberTable member = MUCDBManager.INSTANCE.getMUCMember(chatId, msg.NICKNAME);
            if (member == null)
            {
                member = new MUCMemberTable()
                {
                    id = MUCMemberTable.generateId(chatId, msg.NICKNAME),
                    nickname = msg.NICKNAME,
                    chatId = chatId
                };
            }

            member.affiliation = msg.AFFILIATION;
            member.role = msg.ROLE;

            foreach (MUCPresenceStatusCode s in msg.STATUS_CODES)
            {
                // Is self reference message:
                if (s == MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE)
                {
                    MUCDBManager.INSTANCE.setMUCState(chatId, MUCState.DISCONNECTED, true);
                }
            }

            // If the type equals 'unavailable', a user left the room:
            MUCDBManager.INSTANCE.setMUCMember(member, Equals(msg.getType(), "unavailable"));
        }

        #endregion
    }
}
