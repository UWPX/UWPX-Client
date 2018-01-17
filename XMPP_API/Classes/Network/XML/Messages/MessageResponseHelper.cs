using System;
using System.Threading;
using System.Threading.Tasks;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class MessageResponseHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XMPPClient CLIENT;

        private Func<AbstractMessage, bool> ON_MESSAGE;
        private Action ON_TIMEOUT;

        public int timeout;

        private Timer timer;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/01/2018 Created [Fabian Sauter]
        /// </history>
        public MessageResponseHelper(XMPPClient client, Func<AbstractMessage, bool> onMessage, Action onTimeout)
        {
            this.CLIENT = client;
            this.ON_MESSAGE = onMessage;
            this.ON_TIMEOUT = onTimeout;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private async Task sendAndWaitAsync(AbstractMessage msg)
        {
            CLIENT.NewValidMessage -= Client_NewValidMessage;
            CLIENT.NewValidMessage += Client_NewValidMessage;

            await CLIENT.sendMessageAsync(msg, false);
            statTimer();
        }

        private void statTimer()
        {
            timer = new Timer((o) =>
            {
                ON_TIMEOUT();
                stopTimer();
            }, null, timeout, Timeout.Infinite);
        }

        private void stopTimer()
        {
            CLIENT.NewValidMessage -= Client_NewValidMessage;
            timer?.Dispose();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Client_NewValidMessage(XMPPClient client, Events.NewValidMessageEventArgs args)
        {
            if (ON_MESSAGE(args.getMessage()))
            {
                stopTimer();
            }
        }

        #endregion
    }
}
