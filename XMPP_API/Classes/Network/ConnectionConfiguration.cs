using System.Collections.Generic;
using Windows.Security.Cryptography.Certificates;
using XMPP_API.Classes.Network.TCP;

namespace XMPP_API.Classes.Network
{
    public class ConnectionConfiguration
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public TLSConnectionMode tlsMode;
        public readonly List<ChainValidationResult> IGNORED_CERTIFICATE_ERRORS;
        public bool disableStreamManagement;
        public bool disableMessageCarbons;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 13/04/2018 Created [Fabian Sauter]
        /// </history>
        public ConnectionConfiguration()
        {
            this.tlsMode = TLSConnectionMode.FORCE;
            this.IGNORED_CERTIFICATE_ERRORS = new List<ChainValidationResult>();
            this.disableStreamManagement = false;
            this.disableMessageCarbons = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override bool Equals(object obj)
        {
            if (obj is ConnectionConfiguration c && c != null)
            {
                return c.tlsMode == tlsMode && c.IGNORED_CERTIFICATE_ERRORS.Equals(IGNORED_CERTIFICATE_ERRORS) && disableStreamManagement == c.disableStreamManagement && disableMessageCarbons == c.disableMessageCarbons;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
