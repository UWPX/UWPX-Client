using libsignal;
using libsignal.state;
using System.Collections.Generic;

namespace Component_Tests.Classes.Crypto.Libsignal
{
    class InMemoryIdentityKeyStore : IdentityKeyStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Dictionary<string, IdentityKey> IDENTITY_KEYS;

        private readonly IdentityKeyPair CLIENT_IDENT_KEY_PAIR;
        private readonly uint CLIENT_REG_ID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/11/2018 Created [Fabian Sauter]
        /// </history>
        public InMemoryIdentityKeyStore(IdentityKeyPair clientIdentKeyPair, uint clientRegId)
        {
            this.CLIENT_IDENT_KEY_PAIR = clientIdentKeyPair;
            this.CLIENT_REG_ID = clientRegId;
            this.IDENTITY_KEYS = new Dictionary<string, IdentityKey>();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public IdentityKeyPair GetIdentityKeyPair()
        {
            return CLIENT_IDENT_KEY_PAIR;
        }

        public uint GetLocalRegistrationId()
        {
            return CLIENT_REG_ID;
        }

        public bool IsTrustedIdentity(string name, IdentityKey identityKey)
        {
            return true;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool SaveIdentity(string name, IdentityKey identityKey)
        {
            bool contains = IDENTITY_KEYS.ContainsKey(name);
            IDENTITY_KEYS[name] = identityKey;
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
