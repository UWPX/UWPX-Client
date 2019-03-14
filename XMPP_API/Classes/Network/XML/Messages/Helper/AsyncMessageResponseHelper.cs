using System;
using System.Threading;
using System.Threading.Tasks;

namespace XMPP_API.Classes.Network.XML.Messages.Helper
{
    public class AsyncMessageResponseHelper<T> : IDisposable where T : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Predicate<T> IS_VALID_ANSWER;

        private readonly IMessageSender MESSAGE_SENDER;
        private readonly bool CACHE_IF_NOT_CONNECTED;
        private readonly SemaphoreSlim METHOD_SEMA = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Default timeout for requests is 5 seconds.
        /// </summary>
        public readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(5.0);
        private Task timeoutTask;
        private TaskCompletionSource<MessageResponseHelperResult<T>> completionSource;

        public bool matchId = true;
        public string sendId;
        private bool done;

        private bool disposed;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AsyncMessageResponseHelper(IMessageSender messageSender, Predicate<T> isValidAnswer) : this(messageSender, isValidAnswer, false) { }

        public AsyncMessageResponseHelper(IMessageSender messageSender, Predicate<T> isValidAnswer, bool cacheIfNotConnected)
        {
            this.MESSAGE_SENDER = messageSender;
            this.IS_VALID_ANSWER = isValidAnswer;
            this.CACHE_IF_NOT_CONNECTED = cacheIfNotConnected;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Dispose()
        {
            stop();
            disposed = true;
        }

        public async Task<MessageResponseHelperResult<T>> startAsync(AbstractMessage msg)
        {
            done = false;
            sendId = msg.ID;
            completionSource = new TaskCompletionSource<MessageResponseHelperResult<T>>();

            MESSAGE_SENDER.NewValidMessage += MESSAGE_SENDER_NewValidMessage;

            bool success = await MESSAGE_SENDER.sendAsync(msg, CACHE_IF_NOT_CONNECTED);
            if (!success)
            {
                if (CACHE_IF_NOT_CONNECTED)
                {
                    return new MessageResponseHelperResult<T>(MessageResponseHelperResultState.WILL_SEND_LATER);
                }
                return new MessageResponseHelperResult<T>(MessageResponseHelperResultState.SEND_FAILED);
            }

            if (disposed)
            {
                return new MessageResponseHelperResult<T>(MessageResponseHelperResultState.DISPOSED);
            }

            return await waitForCompletionAsync();
        }

        public void stop()
        {
            if (!disposed)
            {
                done = true;
                if (!(completionSource is null) && !completionSource.Task.IsCanceled && !completionSource.Task.IsCompleted && !completionSource.Task.IsFaulted)
                {
                    completionSource.SetResult(new MessageResponseHelperResult<T>(MessageResponseHelperResultState.TIMEOUT));
                }

                if (!(MESSAGE_SENDER is null))
                {
                    MESSAGE_SENDER.NewValidMessage -= MESSAGE_SENDER_NewValidMessage;
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task<MessageResponseHelperResult<T>> waitForCompletionAsync()
        {
            timeoutTask = Task.Delay(TIMEOUT);

            // Wait for completion:
            Task resultTask = await Task.WhenAny(new Task[] { completionSource.Task, timeoutTask });
            MessageResponseHelperResult<T> result = null;

            // Evaluate and return result:
            if (resultTask == completionSource.Task)
            {
                if (completionSource.Task.IsCompleted)
                {
                    result = completionSource.Task.Result;
                }
                else
                {
                    result = new MessageResponseHelperResult<T>(MessageResponseHelperResultState.ERROR);
                }
            }
            else
            {
                result = new MessageResponseHelperResult<T>(MessageResponseHelperResultState.TIMEOUT);
            }

            stop();
            return result;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void MESSAGE_SENDER_NewValidMessage(IMessageSender sender, Events.NewValidMessageEventArgs args)
        {
            await METHOD_SEMA.WaitAsync();
            if (disposed || done)
            {
                METHOD_SEMA.Release();
                return;
            }

            if (args.MESSAGE is T msg)
            {
                if (matchId && !string.Equals(sendId, msg.ID))
                {
                    METHOD_SEMA.Release();
                    return;
                }

                if (IS_VALID_ANSWER(msg))
                {
                    completionSource.TrySetResult(new MessageResponseHelperResult<T>(MessageResponseHelperResultState.SUCCESS, msg));
                    done = true;
                }
            }
            METHOD_SEMA.Release();
        }

        #endregion
    }
}
