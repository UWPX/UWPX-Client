using System;
using System.Threading;
using System.Threading.Tasks;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class MessageResponseHelper<T> : IDisposable where T : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly IMessageSender MESSAGE_SENDER;
        private bool cacheIfNotConnected;

        private readonly Func<MessageResponseHelper<T>, T, bool> ON_MESSAGE;
        private readonly Action<MessageResponseHelper<T>> ON_TIMEOUT;

        private readonly SemaphoreSlim SEMA;
        private bool disposed;

        /// <summary>
        /// The default timeout is 5000 ms = 5 sec.
        /// </summary>
        public TimeSpan timeout;

        private Timer timer;

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
        public MessageResponseHelper(IMessageSender messageSender, Func<MessageResponseHelper<T>, T, bool> onMessage, Action<MessageResponseHelper<T>> onTimeout) : this(messageSender, onMessage, onTimeout, false) { }

        public MessageResponseHelper(IMessageSender messageSender, Func<MessageResponseHelper<T>, T, bool> onMessage, Action<MessageResponseHelper<T>> onTimeout, bool cacheIfNotConnected)
        {
            this.MESSAGE_SENDER = messageSender;
            this.ON_MESSAGE = onMessage;
            this.ON_TIMEOUT = onTimeout;
            this.cacheIfNotConnected = cacheIfNotConnected;
            this.timeout = TimeSpan.FromSeconds(TIMEOUT_5_SEC);
            this.matchId = true;
            this.sendId = null;
            this.SEMA = new SemaphoreSlim(1, 1);
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

            bool success = await MESSAGE_SENDER.sendAsync(msg, cacheIfNotConnected).ConfigureAwait(false);

            if (ON_TIMEOUT != null)
            {
                if (!success)
                {
                    ON_TIMEOUT(this);
                }

                startTimer();
            }
        }

        private void startTimer()
        {
            timer = new Timer(onTimeout, this, timeout, TimeSpan.FromMilliseconds(-1));
        }

        private void onTimeout(object state)
        {
            if (!(state is MessageResponseHelper<T> helper) || helper.disposed || helper.timer is null)
            {
                return;
            }
            helper.SEMA.Wait();
            if (helper.disposed)
            {
                return;
            }
            try
            {
                helper.MESSAGE_SENDER.NewValidMessage -= MESSAGE_SENDER_NewValidMessage;
                helper.ON_TIMEOUT?.Invoke(this);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (!helper.disposed)
                {
                    helper.SEMA.Release();
                }
            }
        }

        private void stopTimer()
        {
            MESSAGE_SENDER.NewValidMessage -= MESSAGE_SENDER_NewValidMessage;
            timer?.Dispose();
            timer = null;
        }

        public void Dispose()
        {
            disposed = true;
            stop();
            SEMA.Dispose();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void MESSAGE_SENDER_NewValidMessage(IMessageSender sender, Events.NewValidMessageEventArgs args)
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
                    if (ON_MESSAGE == null || ON_MESSAGE(this, args.MESSAGE as T))
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
                    if (!disposed)
                    {
                        SEMA.Release();
                    }
                }
            }
        }

        #endregion
    }
}
