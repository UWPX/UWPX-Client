using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Threading.Tasks;
using Thread_Save_Components.Classes.Collections;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

// https://xmpp.org/extensions/xep-0045.html
// 7.2.2 Basic MUC Protocol
namespace Data_Manager2.Classes
{
    public class MUCJoinHelper : ITimedEntry
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The timeout for receiving a response message in ms.
        /// Default = 5000ms
        /// </summary>
        public int messageTimeout = 5000;

        /// <summary>
        /// The Full JID of the room e.g. 'coven@chat.shakespeare.lit'.
        /// </summary>
        public readonly ChatTable MUC;
        public readonly MUCChatInfoTable INFO;
        public readonly XMPPClient CLIENT;

        private TSTimedList<MessageResponseHelper> messageResponseHelpers;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/01/2018 Created [Fabian Sauter]
        /// </history>
        public MUCJoinHelper(XMPPClient client, ChatTable muc, MUCChatInfoTable info)
        {
            this.CLIENT = client;
            this.MUC = muc;
            this.INFO = info;
            this.messageResponseHelpers = new TSTimedList<MessageResponseHelper>
            {
                itemTimeoutInMs = messageTimeout * 2
            };
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task requestReservedNicksAsync()
        {
            DiscoReservedRoomNicknamesMessages msg = new DiscoReservedRoomNicknamesMessages(CLIENT.getXMPPAccount().getIdDomainAndResource(), MUC.chatJabberId);
            await CLIENT.sendMessageAsync(msg, false);
            MessageResponseHelper helper = new MessageResponseHelper(CLIENT, null, null)
            {
                timeout = messageTimeout
            };
        }

        public async Task enterRoomAsync()
        {
            INFO.enterState = MUCEnterState.ENTERING;
            saveMUCInfo();
            JoinRoomRequestMessage msg = new JoinRoomRequestMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), MUC.chatJabberId, INFO.nickname, INFO.password);
            await sendMessageAsync(msg);
        }

        public bool canGetRemoved()
        {
            switch (INFO.enterState)
            {
                case MUCEnterState.ENTERING:
                    return false;

                case MUCEnterState.ENTERD:
                case MUCEnterState.ERROR:
                case MUCEnterState.DISCONNECTED:
                default:
                    return true;
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task sendMessageAsync(AbstractMessage msg)
        {
            CLIENT.NewValidMessage -= CLIENT_NewValidMessage;
            CLIENT.NewValidMessage += CLIENT_NewValidMessage;
            await CLIENT.sendMessageAsync(msg, false);
        }

        private void processMessage(AbstractMessage msg)
        {
            if (msg is DiscoReservedRoomNicknamesResponseMessages)
            {
                DiscoReservedRoomNicknamesResponseMessages result = msg as DiscoReservedRoomNicknamesResponseMessages;
            }
            else if (msg is IQMessage)
            {
                IQMessage iq = msg as IQMessage;
            }
        }

        private void saveMUCInfo()
        {
            ChatManager.INSTANCE.setMUCChatInfo(INFO, false, true);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CLIENT_NewValidMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewValidMessageEventArgs args)
        {
            switch (INFO.enterState)
            {
                case MUCEnterState.ENTERING:
                    if (args.getMessage() is MUCMemberPresenceMessage)
                    {
                        MUCMemberPresenceMessage msg = args.getMessage() as MUCMemberPresenceMessage;

                        // Evaluate status codes:
                        foreach (MUCPresenceStatusCode statusCode in msg.STATUS_CODES)
                        {
                            switch (statusCode)
                            {
                                case MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE:
                                    CLIENT.NewValidMessage -= CLIENT_NewValidMessage;
                                    INFO.enterState = MUCEnterState.ENTERD;
                                    saveMUCInfo();
                                    break;
                            }
                        }

                        // Save member:
                        MUCMemberTable member = new MUCMemberTable()
                        {
                            id = MUCMemberTable.generateId(MUC.id, msg.NICKNAME),
                            nickname = msg.NICKNAME,
                            chatId = MUC.id,
                            affiliation = msg.AFFILIATION,
                            role = msg.ROLE
                        };

                        // Cancel event:
                        args.Cancel = true;
                    }
                    break;
            }
        }

        #endregion
    }
}
