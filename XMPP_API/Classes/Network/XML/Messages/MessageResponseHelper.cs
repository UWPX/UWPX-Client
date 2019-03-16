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

        private readonly OnMessageHandler ON_MESSAGE;
        private readonly OnTimeoutHandler ON_TIMEOUT;

        private readonly SemaphoreSlim METHOD_SEMA;
        private readonly SemaphoreSlim TIMER_SEMA;
        private bool disposed;

        /// <summary>
        /// The default timeout is 5000 ms = 5 sec.
        /// </summary>
        public TimeSpan timeout;

        private ThreadPoolTimer timer;

        public const int TIMEOUT_5_SEC = 5;

        public bool matchId;
        public string sendId;

        public delegate bool OnMessageHandler(MessageResponseHelper<T> helper, T msg);
        public delegate void OnTimeoutHandler(MessageResponseHelper<T> helper);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MessageResponseHelper(IMessageSender messageSender, OnMessageHandler onMessage, OnTimeoutHandler onTimeout)
        {
            this.MESSAGE_SENDER = messageSender;
            this.ON_MESSAGE = onMessage;
            this.ON_TIMEOUT = onTimeout;
            this.timeout = TimeSpan.FromSeconds(TIMEOUT_5_SEC);
            this.matchId = true;
            this.sendId = null;
            this.METHOD_SEMA = new SemaphoreSlim(1, 1);
            this.TIMER_SEMA = new SemaphoreSlim(1, 1);
            this.disposed = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public IMessageSender getMessageSender()
        {
            return MESSAGE_SENDER;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public Task start(AbstractMessage msg)
        {
            return sendAndWaitAsync(msg);
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
                MESSAGE_SENDER.NewValidMessage -= MESSAGE_SENDER_NewValidMessage;
                MESSAGE_SENDER.NewValidMessage += MESSAGE_SENDER_NewValidMessage;
            }

            TIMER_SEMA.Wait();
            bool success = await MESSAGE_SENDER.sendAsync(msg).ConfigureAwait(false);

            if (ON_TIMEOUT != null)
            {
                if (!success)
                {
                    ON_TIMEOUT(this);
                }

                startTimer();
            }
            if(disposed)
            {
                return;
            }
            TIMER_SEMA.Release();
        }

        private void startTimer()
        {
            if(timer != null)
            {
                throw new InvalidOperationException("Can not start timer - timer not null!");
            }
            timer = ThreadPoolTimer.CreateTimer(onTimeout, timeout);
        }

        private void onTimeout(ThreadPoolTimer timer)
        {
            if (timer is null || disposed)
            {
                return;
            }
            METHOD_SEMA.Wait();
            if (disposed)
            {
                return;
            }
            try
            {
                MESSAGE_SENDER.NewValidMessage -= MESSAGE_SENDER_NewValidMessage;
                ON_TIMEOUT?.Invoke(this);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (!disposed)
                {
                    METHOD_SEMA.Release();
                }
            }
        }

        private void stopTimer()
        {
            MESSAGE_SENDER.NewValidMessage -= MESSAGE_SENDER_NewValidMessage;
            if(timer != null)
            {
                timer.Cancel();
                timer = null;
            }
        }

        public void Dispose()
        {
            disposed = true;
            stop();
            METHOD_SEMA.Dispose();
            TIMER_SEMA.Dispose();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void MESSAGE_SENDER_NewValidMessage(IMessageSender sender, Events.NewValidMessageEventArgs args)
        {
            if(disposed)
            {
                return;
            }

            if (args.MESSAGE is T)
            {
                if (matchId && !Equals(sendId, args.MESSAGE.ID))
                {
                    return;
                }

                try
                {
                    METHOD_SEMA.Wait();
                    if ((ON_MESSAGE is null || ON_MESSAGE(this, args.MESSAGE as T)) && !disposed)
                    {
                        // Prevent the case that a result is already available although the timer hasn't started yet:
                        TIMER_SEMA.Wait();
                        if (disposed)
                        {
                            return;
                        }
                        stopTimer();
                        TIMER_SEMA.Release();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (!disposed)
                    {
                        METHOD_SEMA.Release();
                    }
                }
            }
        }

        #endregion
    }
}
