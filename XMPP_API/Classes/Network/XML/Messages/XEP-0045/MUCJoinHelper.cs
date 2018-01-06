using System;
using System.Threading;
using System.Threading.Tasks;
using Thread_Save_Components.Classes.Collections;

// https://xmpp.org/extensions/xep-0045.html
// 7.2.2 Basic MUC Protocol
namespace XMPP_API.Classes.Network.XML.Messages.XEP_0045
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
        public MUCJoinState state;

        private Timer timer;
        private TSTimedList<string> messageIdCache;
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
            this.messageIdCache = new TSTimedList<string>();
            this.state = MUCJoinState.NOT_STARTED;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task requestReservedNicksAsync()
        {
            if(state == MUCJoinState.NOT_STARTED)
            {
                state = MUCJoinState.SEND_REQUESTING_RESERVED_NICKS;
                DiscoReservedRoomNicknamesMessages msg = new DiscoReservedRoomNicknamesMessages(CLIENT.getXMPPAccount().getIdDomainAndResource(), ROOM_JID);
                await CLIENT.sendMessageAsync(msg, false);
                startTimer();
            }
        }

        public async Task enterRoomAsync(string nick)
        {
            if(state == MUCJoinState.RECEIVED_RESERVED_NICKS)
            {
                state = MUCJoinState.SEND_ENTER_ROOM;
                EnterRoomRequestMessage msg = new EnterRoomRequestMessage(CLIENT.getXMPPAccount().getIdDomainAndResource(), ROOM_JID, nick);
                await sendMessageAsync(msg);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void stopTimer()
        {
            timer?.Dispose();
        }

        private void startTimer()
        {

        }

        private async Task sendMessageAsync(AbstractMessage msg)
        {
            CLIENT.NewValidMessage -= CLIENT_NewValidMessage;
            CLIENT.NewValidMessage += CLIENT_NewValidMessage;
            await CLIENT.sendMessageAsync(msg, false);
            startTimer();
        }

        private void processMessage(AbstractMessage msg)
        {
            switch (state)
            {
                case MUCJoinState.SEND_REQUESTING_RESERVED_NICKS:
                    if(msg is DiscoReservedRoomNicknamesResponseMessages)
                    {
                        state = MUCJoinState.RECEIVED_RESERVED_NICKS;
                        DiscoReservedRoomNicknamesResponseMessages result = msg as DiscoReservedRoomNicknamesResponseMessages;
                    }
                    break;
                case MUCJoinState.SEND_ENTER_ROOM:
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void CLIENT_NewValidMessage(XMPPClient client, Events.NewValidMessageEventArgs args)
        {
            if (messageIdCache.getTimed(args.getMessage().getId()) != null)
            {
                // Process the received message in a new task:
                Task.Factory.StartNew(() => processMessage(args.getMessage()));
                // If the messageIdCache is empty, we can unsubscribe from the NewValidMessage event.
                if (messageIdCache.isEmpty())
                {
                    CLIENT.NewValidMessage -= CLIENT_NewValidMessage;
                }
            }
        }

        #endregion
    }
}
