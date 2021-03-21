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
        private readonly string NAME;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public DebugSemaphoreSlim(int initialCount, int maxCount) : this(initialCount, maxCount, "-") { }

        public DebugSemaphoreSlim(int initialCount) : this(initialCount, "-") { }

        public DebugSemaphoreSlim(int initialCount, int maxCount, string name) : base(initialCount, maxCount)
        {
            NAME = name;
        }

        public DebugSemaphoreSlim(int initialCount, string name) : base(initialCount)
        {
            NAME = name;
        }

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
            Logger.Trace('[' + NAME + ']' + " Semaphore 'Wait()' from: " + Environment.StackTrace);
            Logger.Debug('[' + NAME + ']' + " 'Wait()' PRE CurrentCount: " + CurrentCount);
            base.Wait();
            Logger.Debug('[' + NAME + ']' + " 'Wait()' POST CurrentCount: " + CurrentCount);
        }

        public new async Task WaitAsync()
        {
            Logger.Trace('[' + NAME + ']' + " Semaphore 'WaitAsync()' from: " + Environment.StackTrace);
            Logger.Debug('[' + NAME + ']' + " 'WaitAsync()' PRE CurrentCount: " + CurrentCount);
            await base.WaitAsync();
            Logger.Debug('[' + NAME + ']' + " 'WaitAsync()' POST CurrentCount: " + CurrentCount);
        }

        public new void Release()
        {
            Logger.Trace('[' + NAME + ']' + " Semaphore 'Release()' from: " + Environment.StackTrace);
            Logger.Debug('[' + NAME + ']' + " 'Release()' PRE CurrentCount: " + CurrentCount);
            base.Release();
            Logger.Debug('[' + NAME + ']' + " 'Release()' POST CurrentCount: " + CurrentCount);
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
