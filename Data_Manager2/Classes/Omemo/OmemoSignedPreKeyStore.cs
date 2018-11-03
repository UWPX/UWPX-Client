using Data_Manager2.Classes.DBManager.Omemo;
using libsignal;
using libsignal.state;
using System.Collections.Generic;
using XMPP_API.Classes.Network;

namespace Data_Manager2.Classes.Omemo
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
        /// 03/11/2018 Created [Fabian Sauter]
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
            return OmemoSignalKeyDBManager.INSTANCE.containsSignedPreKey(signedPreKeyId, ACCOUNT.getIdAndDomain());
        }

        public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
        {
            SignedPreKeyRecord signedPreKeyRecord = OmemoSignalKeyDBManager.INSTANCE.getSignedPreKey(signedPreKeyId, ACCOUNT.getIdAndDomain());
            if (signedPreKeyRecord == null)
            {
                throw new InvalidKeyIdException("No such key: " + signedPreKeyId);
            }
            return signedPreKeyRecord;
        }

        public List<SignedPreKeyRecord> LoadSignedPreKeys()
        {
            return OmemoSignalKeyDBManager.INSTANCE.getAllSignedPreKeys(ACCOUNT.getIdAndDomain());
        }

        public void RemoveSignedPreKey(uint signedPreKeyId)
        {
            OmemoSignalKeyDBManager.INSTANCE.deleteSignedPreKey(signedPreKeyId, ACCOUNT.getIdAndDomain());
        }

        public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord signedPreKey)
        {
            OmemoSignalKeyDBManager.INSTANCE.setSignedPreKey(signedPreKeyId, signedPreKey, ACCOUNT.getIdAndDomain());
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
