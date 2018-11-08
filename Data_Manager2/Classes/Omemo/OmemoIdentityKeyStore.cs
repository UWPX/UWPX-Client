using Data_Manager2.Classes.DBManager.Omemo;
using libsignal;
using libsignal.state;
using XMPP_API.Classes.Network;

namespace Data_Manager2.Classes.Omemo
{
    public class OmemoIdentityKeyStore : IdentityKeyStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly XMPPAccount ACCOUNT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/11/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoIdentityKeyStore(XMPPAccount account)
        {
            this.ACCOUNT = account;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public IdentityKeyPair GetIdentityKeyPair()
        {
            return ACCOUNT.omemoIdentityKeyPair;
        }

        public uint GetLocalRegistrationId()
        {
            return ACCOUNT.omemoDeviceId;
        }

        public bool IsTrustedIdentity(string name, IdentityKey identityKey)
        {
            // XEP-0384 (OMEMO Encryption) recommends to disable trust management provided by the signal library:
            // Source: https://xmpp.org/extensions/xep-0384.html#impl
            return true;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool SaveIdentity(string name, IdentityKey identityKey)
        {
            bool contains = OmemoSignalKeyDBManager.INSTANCE.containsIdentityKey(name, ACCOUNT.getIdAndDomain());
            OmemoSignalKeyDBManager.INSTANCE.setIdentityKey(name, identityKey, ACCOUNT.getIdAndDomain());
            return contains;
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
