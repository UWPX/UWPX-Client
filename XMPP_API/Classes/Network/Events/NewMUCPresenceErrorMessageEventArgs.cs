using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace XMPP_API.Classes.Network.Events
{
    public class NewMUCPresenceErrorMessageEventArgs
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MUCPresenceErrorMessage mucPresenceErrorMessage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public NewMUCPresenceErrorMessageEventArgs(MUCPresenceErrorMessage mucPresenceErrorMessage)
        {
            this.mucPresenceErrorMessage = mucPresenceErrorMessage;
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
