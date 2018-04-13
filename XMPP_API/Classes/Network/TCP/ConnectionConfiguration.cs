using System.Collections.Generic;
using Windows.Security.Cryptography.Certificates;

namespace XMPP_API.Classes.Network.TCP
{
    public class ConnectionConfiguration
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public TLSConnectionMode tlsMode;
        public readonly List<ChainValidationResult> IGNORED_CERTIFICATE_ERRORS;

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
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (obj is ConnectionConfiguration c)
            {
                return c.tlsMode == tlsMode &&
                    c.IGNORED_CERTIFICATE_ERRORS.Equals(IGNORED_CERTIFICATE_ERRORS);
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
