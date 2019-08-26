using System;
using System.Threading;
using Windows.Graphics.Imaging;

namespace UWPX_UI_Context.Classes.Events
{
    public class FrameArrivedEventArgs: EventArgs, IDisposable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private SoftwareBitmap softwareBitmap;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetSoftwareBitmap(ref SoftwareBitmap softwareBitmap)
        {
            // Swap the process frame to qrCodeBitmap and dispose the unused image:
            softwareBitmap = Interlocked.Exchange(ref this.softwareBitmap, softwareBitmap);
            softwareBitmap?.Dispose();
        }

        public void GetSoftwareBitmap(ref SoftwareBitmap softwareBitmap)
        {
            // Swap the process frame to qrCodeBitmap and dispose the unused image:
            this.softwareBitmap = Interlocked.Exchange(ref softwareBitmap, this.softwareBitmap);
            this.softwareBitmap?.Dispose();
            this.softwareBitmap = null;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Dispose()
        {
            softwareBitmap?.Dispose();
            softwareBitmap = null;
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
