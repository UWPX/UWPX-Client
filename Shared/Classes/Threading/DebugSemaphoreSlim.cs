using System;
using System.Threading;
using System.Threading.Tasks;
using Logging;

namespace Shared.Classes.Threading
{
    /// <summary>
    /// Class for debugging semaphores.
    /// Prints the stack trace every time the <see cref="SemaphoreSlim.CurrentCount"/> is changed.
    /// </summary>
    public class DebugSemaphoreSlim: SemaphoreSlim
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DebugSemaphoreSlim(int initialCount, int maxCount) : base(initialCount, maxCount) { }

        public DebugSemaphoreSlim(int initialCount) : base(initialCount) { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void WaitCount(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Wait();
            }
        }

        public new void Wait()
        {
            Logger.Debug("Semaphore 'Wait()' from: " + Environment.StackTrace);
            Logger.Debug("'Wait()' PRE CurrentCount: " + CurrentCount);
            base.Wait();
            Logger.Debug("'Wait()' POST CurrentCount: " + CurrentCount);
        }

        public new async Task WaitAsync()
        {
            Logger.Debug("Semaphore 'WaitAsync()' from: " + Environment.StackTrace);
            Logger.Debug("'WaitAsync()' PRE CurrentCount: " + CurrentCount);
            await base.WaitAsync();
            Logger.Debug("'WaitAsync()' POST CurrentCount: " + CurrentCount);
        }

        public new void Release()
        {
            Logger.Debug("Semaphore 'Release()' from: " + Environment.StackTrace);
            Logger.Debug("'Release()' PRE CurrentCount: " + CurrentCount);
            base.Release();
            Logger.Debug("'Release()' POST CurrentCount: " + CurrentCount);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
