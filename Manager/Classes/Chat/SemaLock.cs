using System;
using System.Diagnostics;
using System.Threading;

namespace Manager.Classes.Chat
{
    public class SemaLock: IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly SemaphoreSlim SEMA;
        private bool disposed = false;
        private bool isWaiting = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        internal SemaLock(SemaphoreSlim sema)
        {
            SEMA = sema;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Dispose()
        {
            if (!disposed)
            {
                Debug.Assert(isWaiting);
                disposed = true;
                SEMA.Release();
            }
        }

        internal void Wait()
        {
            Debug.Assert(!isWaiting);
            isWaiting = true;
            SEMA.Wait();
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
