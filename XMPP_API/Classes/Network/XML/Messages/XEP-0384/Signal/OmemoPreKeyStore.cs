using libsignal;
using libsignal.state;
using XMPP_API.Classes.Network.XML.DBManager;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal
{
    public class OmemoPreKeyStore : PreKeyStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoPreKeyStore()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public bool ContainsPreKey(uint preKeyId)
        {
            return SignalKeyDBManager.INSTANCE.containsPreKeyRecord(preKeyId);
        }

        public PreKeyRecord LoadPreKey(uint preKeyId)
        {
            PreKeyRecord preKeyRecord = SignalKeyDBManager.INSTANCE.getPreKeyRecord(preKeyId);
            if (preKeyRecord == null)
            {
                throw new InvalidKeyIdException("No such key: " + preKeyId);
            }
            return preKeyRecord;
        }

        public void RemovePreKey(uint preKeyId)
        {
            SignalKeyDBManager.INSTANCE.deletePreKeyRecord(preKeyId);
        }

        public void StorePreKey(uint preKeyId, PreKeyRecord preKey)
        {
            SignalKeyDBManager.INSTANCE.setPreKey(preKeyId, preKey);
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
