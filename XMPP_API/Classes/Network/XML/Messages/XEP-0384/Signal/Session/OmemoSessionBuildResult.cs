using libsignal;
using System;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session
{
    public class OmemoSessionBuildResult
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly bool SUCCESS;
        public readonly OmemoSessionBuildError ERROR;
        public readonly Tuple<SignalProtocolAddress, SessionBuilder> SESSION;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 10/08/2018 Created [Fabian Sauter]
        /// </history>
        internal OmemoSessionBuildResult(Tuple<SignalProtocolAddress, SessionBuilder> session)
        {
            this.SESSION = session;
            this.SUCCESS = true;
        }

        internal OmemoSessionBuildResult(OmemoSessionBuildError error)
        {
            this.ERROR = error;
            this.SUCCESS = false;
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
