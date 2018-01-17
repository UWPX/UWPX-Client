using System;
using System.Threading;
using System.Threading.Tasks;
using Thread_Save_Components.Classes.Collections;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

// https://xmpp.org/extensions/xep-0045.html
// 7.2.2 Basic MUC Protocol
namespace Data_Manager2.Classes
{
    public class MUCJoinHelper
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
        public readonly string ROOM_JID;
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
        public MUCJoinHelper(XMPPClient client, string roomJid)
        {
            this.ROOM_JID = roomJid;
            this.CLIENT = client;
            this.messageResponseHelpers = new TSTimedList<MessageResponseHelper>();
            this.messageResponseHelpers.itemTimeoutInMs = messageTimeout * 2;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task requestReservedNicksAsync()
        {
            DiscoReservedRoomNicknamesMessages msg = new DiscoReservedRoomNicknamesMessages(CLIENT.getXMPPAccount().getIdDomainAndResource(), ROOM_JID);
            await CLIENT.sendMessageAsync(msg, false);
            MessageResponseHelper helper = new MessageResponseHelper(CLIENT, null, null)
            {
                timeout = messageTimeout
            };
        }

        public async Task enterRoomAsync(string nick, string roomPassword)
        {
            JoinRoomRequestMessage msg = new JoinRoomRequestMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), ROOM_JID, nick);
            await sendMessageAsync(msg);
        }

        public async Task enterRoomAsync(string nick)
        {
            await enterRoomAsync(nick, null);
        }

        public void addChatToList()
        {

        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task sendMessageAsync(AbstractMessage msg)
        {
            CLIENT.NewValidMessage -= CLIENT_NewValidMessage;
            CLIENT.NewValidMessage += CLIENT_NewValidMessage;
            await CLIENT.sendMessageAsync(msg, false);
            //startTimer();
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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CLIENT_NewValidMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewValidMessageEventArgs args)
        {
            /*if (messageIdCache.getTimed(args.getMessage().getId()) != null)
            {
                // Process the received message in a new task:
                Task.Factory.StartNew(() => processMessage(args.getMessage()));
                // If the messageIdCache is empty, we can unsubscribe from the NewValidMessage event.
                if (messageIdCache.isEmpty())
                {
                    CLIENT.NewValidMessage -= CLIENT_NewValidMessage;
                }
            }*/
        }

        #endregion
    }
}
