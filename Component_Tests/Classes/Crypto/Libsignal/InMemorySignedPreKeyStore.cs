using System.Collections.Generic;
using System.Linq;
using libsignal.state;

namespace Component_Tests.Classes.Crypto.Libsignal
{
    internal class InMemorySignedPreKeyStore: SignedPreKeyStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Dictionary<uint, SignedPreKeyRecord> SIGNED_PRE_KEYS = new Dictionary<uint, SignedPreKeyRecord>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool ContainsSignedPreKey(uint signedPreKeyId)
        {
            return SIGNED_PRE_KEYS.ContainsKey(signedPreKeyId);
        }

        public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
        {
            return SIGNED_PRE_KEYS[signedPreKeyId];
        }

        public List<SignedPreKeyRecord> LoadSignedPreKeys()
        {
            return SIGNED_PRE_KEYS.Values.ToList();
        }

        public void RemoveSignedPreKey(uint signedPreKeyId)
        {
            SIGNED_PRE_KEYS.Remove(signedPreKeyId);
        }

        public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
        {
            SIGNED_PRE_KEYS[signedPreKeyId] = record;
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
