using libsignal;
using libsignal.state;
using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.DBManager;

// https://github.com/signalapp/libsignal-protocol-java/blob/master/java/src/main/java/org/whispersystems/libsignal/state/IdentityKeyStore.java
namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal
{
    public class OmemoIdentityKeyStore : IdentityKeyStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Dictionary<string, IdentityKey> IDENTITY_KEYS;
        private readonly XMPPAccount ACCOUNT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoIdentityKeyStore(XMPPAccount account)
        {
            this.ACCOUNT = account;
            this.IDENTITY_KEYS = new Dictionary<string, IdentityKey>();
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
            bool contains = SignalKeyDBManager.INSTANCE.containsIdentityKey(name, ACCOUNT.getIdAndDomain());
            SignalKeyDBManager.INSTANCE.setIdentityKey(name, identityKey, ACCOUNT.getIdAndDomain());
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
