using System;
using Windows.Graphics.Imaging;
using Windows.Media.Capture.Frames;

namespace UWPX_UI_Context.Classes.Events
{
    public class FrameArrivedEventArgs: EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private MediaFrameReader frameReader;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public FrameArrivedEventArgs(MediaFrameReader frameReader)
        {
            this.frameReader = frameReader;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void GetSoftwareBitmap(ref SoftwareBitmap softwareBitmap)
        {
            MediaFrameReference frameRef = frameReader.TryAcquireLatestFrame();
            VideoMediaFrame frame = frameRef?.VideoMediaFrame;
            softwareBitmap = frame?.SoftwareBitmap;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
