using libsignal;
using libsignal.state;
using System.Collections.Generic;
using XMPP_API.Classes.Network.XML.DBManager;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal
{
    public class OmemoSignedPreKeyStore : SignedPreKeyStore
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
        public OmemoSignedPreKeyStore(XMPPAccount account)
        {
            this.ACCOUNT = account;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool ContainsSignedPreKey(uint signedPreKeyId)
        {
            return SignalKeyDBManager.INSTANCE.containsSignedPreKey(signedPreKeyId, ACCOUNT.getIdAndDomain());
        }

        public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
        {
            SignedPreKeyRecord signedPreKeyRecord = SignalKeyDBManager.INSTANCE.getSignedPreKey(signedPreKeyId, ACCOUNT.getIdAndDomain());
            if (signedPreKeyRecord == null)
            {
                throw new InvalidKeyIdException("No such key: " + signedPreKeyId);
            }
            return signedPreKeyRecord;
        }

        public List<SignedPreKeyRecord> LoadSignedPreKeys()
        {
            return SignalKeyDBManager.INSTANCE.getAllSignedPreKeys(ACCOUNT.getIdAndDomain());
        }

        public void RemoveSignedPreKey(uint signedPreKeyId)
        {
            SignalKeyDBManager.INSTANCE.deleteSignedPreKey(signedPreKeyId, ACCOUNT.getIdAndDomain());
        }

        public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord signedPreKey)
        {
            SignalKeyDBManager.INSTANCE.setSignedPreKey(signedPreKeyId, signedPreKey, ACCOUNT.getIdAndDomain());
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
