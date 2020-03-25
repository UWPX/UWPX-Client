using System;

namespace UWPX_UI_Context.Classes.Events
{
    public class MucConfigurationErrorEventArgs: EventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly string ERROR_TEXT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MucConfigurationErrorEventArgs(string errorText)
        {
            ERROR_TEXT = errorText;
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
