using libsignal;
using libsignal.state;
using XMPP_API.Classes.Network.XML.DBManager;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal
{
    public class OmemoPreKeyStore : PreKeyStore
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
        /// 08/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoPreKeyStore(XMPPAccount account)
        {
            this.ACCOUNT = account;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool ContainsPreKey(uint preKeyId)
        {
            return SignalKeyDBManager.INSTANCE.containsPreKeyRecord(preKeyId, ACCOUNT.getIdAndDomain());
        }

        public PreKeyRecord LoadPreKey(uint preKeyId)
        {
            PreKeyRecord preKeyRecord = SignalKeyDBManager.INSTANCE.getPreKeyRecord(preKeyId, ACCOUNT.getIdAndDomain());
            if (preKeyRecord == null)
            {
                throw new InvalidKeyIdException("No such key: " + preKeyId);
            }
            return preKeyRecord;
        }

        public void RemovePreKey(uint preKeyId)
        {
            SignalKeyDBManager.INSTANCE.deletePreKey(preKeyId, ACCOUNT.getIdAndDomain());
        }

        public void StorePreKey(uint preKeyId, PreKeyRecord preKey)
        {
            SignalKeyDBManager.INSTANCE.setPreKey(preKeyId, preKey, ACCOUNT.getIdAndDomain());
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
