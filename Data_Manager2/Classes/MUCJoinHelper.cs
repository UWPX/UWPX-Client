using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Threading.Tasks;
using Thread_Save_Components.Classes.Collections;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

// https://xmpp.org/extensions/xep-0045.html
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

        private TSTimedList<MessageResponseHelper<AbstractAddressableMessage>> messageResponseHelpers;

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
            this.messageResponseHelpers = new TSTimedList<MessageResponseHelper<AbstractAddressableMessage>>
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
        }

        public async Task enterRoomAsync()
        {
            // Update MUC info:
            INFO.state = MUCState.ENTERING;
            saveMUCEnterState();

            // Clear MUC members:
            MUCDBManager.INSTANCE.deleteAllMUCMembersforChat(MUC.id);

            // Create message:
            JoinRoomRequestMessage msg = new JoinRoomRequestMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), MUC.chatJabberId, INFO.nickname, INFO.password);

            // Subscribe to events for receiving answers:
            CLIENT.NewMUCMemberPresenceMessage -= CLIENT_NewMUCMemberPresenceMessage;
            CLIENT.NewMUCMemberPresenceMessage += CLIENT_NewMUCMemberPresenceMessage;

            // Send message:
            await CLIENT.sendMessageAsync(msg, false);
        }

        public bool canGetRemoved()
        {
            switch (INFO.state)
            {
                case MUCState.ENTERING:
                    return false;

                default:
                    return true;
            }
        }

        #endregion

        #region --Misc Methods (Private)--
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

        private void saveMUCEnterState()
        {
            MUCDBManager.INSTANCE.setMUCState(INFO.chatId, INFO.state, true);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CLIENT_NewMUCMemberPresenceMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewMUCMemberPresenceMessageEventArgs args)
        {
            string roomJId = Utils.getBareJidFromFullJid(args.mucMemberPresenceMessage.getFrom());
            if (!Equals(roomJId, MUC.chatJabberId))
            {
                return;
            }

            switch (INFO.state)
            {
                case MUCState.ENTERING:
                    // Evaluate status codes:
                    foreach (MUCPresenceStatusCode statusCode in args.mucMemberPresenceMessage.STATUS_CODES)
                    {
                        switch (statusCode)
                        {
                            case MUCPresenceStatusCode.PRESENCE_SELFE_REFERENCE:
                                // Remove event subscription:
                                CLIENT.NewMUCMemberPresenceMessage -= CLIENT_NewMUCMemberPresenceMessage;

                                // Update MUC info:
                                INFO.state = MUCState.ENTERD;
                                saveMUCEnterState();
                                break;
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}
