using System;

namespace UWPX_UI_Context.Classes.Events
{
    public enum PreviewError
    {
        /// <summary>
        /// The user denied the access to the camera.
        /// </summary>
        ACCESS_DENIED,
        /// <summary>
        /// The camera is in use by an other app.
        /// </summary>
        ACCESS_DENIED_OTHER_APP,

        /// <summary>
        /// MediaFrame creation result is null.
        /// </summary>
        MEDIA_FRAME_IS_NULL,
        /// <summary>
        /// No camera available.
        /// </summary>
        MEDIA_FRAME_NO_CAMERA,
        /// <summary>
        /// An error occurred during the MediaFrame creation.
        /// </summary>
        MEDIA_FRAME_CREATION_FAILED,
    };

    public class PreviewFailedEventArgs: EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly PreviewError ERROR;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PreviewFailedEventArgs(PreviewError error)
        {
            ERROR = error;
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
