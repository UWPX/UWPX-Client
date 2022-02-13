using System;
using Windows.Graphics.Imaging;

namespace UWPX_UI.Classes.Events
{
    public class ImageEditDoneEventArgs: EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// True in case editing was successful.
        /// </summary>
        public readonly bool SUCCESS;
        /// <summary>
        /// Only valid in case <see cref="SUCCESS"/> is true.
        /// </summary>
        public readonly SoftwareBitmap IMAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Called when editing an image was successful.
        /// </summary>
        /// <param name="image">The resulting image.</param>
        public ImageEditDoneEventArgs(SoftwareBitmap image)
        {
            IMAGE = image;
            SUCCESS = true;
        }

        /// <summary>
        /// Called when editing an image got canceled or failed.
        /// </summary>
        public ImageEditDoneEventArgs()
        {
            SUCCESS = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


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
