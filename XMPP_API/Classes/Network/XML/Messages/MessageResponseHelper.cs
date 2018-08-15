using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class MessageResponseHelper<T> : IDisposable where T : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly IMessageSender MESSAGE_SENDER;

        private readonly Func<T, bool> ON_MESSAGE;
        private readonly Action ON_TIMEOUT;

        private readonly SemaphoreSlim SEMA;

        /// <summary>
        /// The default timeout is 5000 ms = 5 sec.
        /// </summary>
        public TimeSpan timeout;

        private ThreadPoolTimer timer;

        public const int TIMEOUT_5_SEC = 5;

        public bool matchId;
        public string sendId;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/01/2018 Created [Fabian Sauter]
        /// </history>
        public MessageResponseHelper(IMessageSender messageSender, Func<T, bool> onMessage, Action onTimeout)
        {
            this.MESSAGE_SENDER = messageSender;
            this.ON_MESSAGE = onMessage;
            this.ON_TIMEOUT = onTimeout;
            this.timeout = TimeSpan.FromSeconds(TIMEOUT_5_SEC);
            this.matchId = true;
            this.sendId = null;
            this.SEMA = new SemaphoreSlim(1, 1);
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

        public void stop()
        {
            stopTimer();
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task sendAndWaitAsync(AbstractMessage msg)
        {
            sendId = msg.ID;

            if (ON_MESSAGE != null)
            {
                MESSAGE_SENDER.NewValidMessage -= Client_NewValidMessage;
                MESSAGE_SENDER.NewValidMessage += Client_NewValidMessage;
            }

            await MESSAGE_SENDER.sendAsync(msg);

            if (ON_TIMEOUT != null)
            {
                startTimer();
            }
        }

        private void startTimer()
        {
            timer = ThreadPoolTimer.CreateTimer((source) => onTimeout(), timeout);
        }

        private void onTimeout()
        {
            SEMA.Wait();
            try
            {
                MESSAGE_SENDER.NewValidMessage -= Client_NewValidMessage;
                ON_TIMEOUT();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SEMA.Release();
            }
        }

        private void stopTimer()
        {
            MESSAGE_SENDER.NewValidMessage -= Client_NewValidMessage;
            timer?.Cancel();
            timer = null;
        }

        public void Dispose()
        {
            stop();
            SEMA.Dispose();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Client_NewValidMessage(IMessageSender sender, Events.NewValidMessageEventArgs args)
        {
            if (args.MESSAGE is T)
            {
                if (matchId && !Equals(sendId, args.MESSAGE.ID))
                {
                    return;
                }

                try
                {
                    SEMA.Wait();
                    if (ON_MESSAGE(args.MESSAGE as T))
                    {
                        stopTimer();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    SEMA.Release();
                }
            }
        }

        #endregion
    }
}
