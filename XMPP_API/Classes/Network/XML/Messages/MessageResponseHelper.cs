using System;
using System.Threading;
using System.Threading.Tasks;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class MessageResponseHelper : IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XMPPClient CLIENT;

        private readonly Func<AbstractMessage, bool> ON_MESSAGE;
        private readonly Action ON_TIMEOUT;

        /// <summary>
        /// The default timeout is 5000 ms = 5 sec.
        /// </summary>
        public int timeout;

        private Timer timer;

        public const int TIMEOUT_5_SEC = 5000;

        private bool stoped;

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
            this.timeout = TIMEOUT_5_SEC;
            this.stoped = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void start(AbstractMessage msg)
        {
            Task t = sendAndWaitAsync(msg);
        }

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
            stoped = false;
            timer = new Timer((o) =>
            {
                if (!stoped)
                {
                    ON_TIMEOUT();
                }
                stopTimer();
            }, null, timeout, Timeout.Infinite);
        }

        private void stopTimer()
        {
            stoped = true;
            CLIENT.NewValidMessage -= Client_NewValidMessage;
            timer?.Dispose();
        }

        public void Dispose()
        {
            stopTimer();
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
