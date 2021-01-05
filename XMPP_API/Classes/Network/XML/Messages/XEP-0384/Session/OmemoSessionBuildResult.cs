namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Session
{
    public class OmemoSessionBuildResult
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly bool SUCCESS;
        public readonly OmemoSessionBuildError ERROR;
        public readonly OmemoDeviceSession SESSION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        internal OmemoSessionBuildResult(OmemoDeviceSession session)
        {
            SESSION = session;
            SUCCESS = true;
        }

        internal OmemoSessionBuildResult(OmemoSessionBuildError error)
        {
            ERROR = error;
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
