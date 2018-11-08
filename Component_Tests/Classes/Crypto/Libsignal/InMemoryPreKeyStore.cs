using libsignal.state;
using System.Collections.Generic;

namespace Component_Tests.Classes.Crypto.Libsignal
{
    class InMemoryPreKeyStore : PreKeyStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Dictionary<uint, PreKeyRecord> PRE_KEYS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/11/2018 Created [Fabian Sauter]
        /// </history>
        public InMemoryPreKeyStore()
        {
            this.PRE_KEYS = new Dictionary<uint, PreKeyRecord>();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool ContainsPreKey(uint preKeyId)
        {
            return PRE_KEYS.ContainsKey(preKeyId);
        }

        public PreKeyRecord LoadPreKey(uint preKeyId)
        {
            return PRE_KEYS[preKeyId];
        }

        public void RemovePreKey(uint preKeyId)
        {
            PRE_KEYS.Remove(preKeyId);
        }

        public void StorePreKey(uint preKeyId, PreKeyRecord record)
        {
            PRE_KEYS[preKeyId] = record;
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
