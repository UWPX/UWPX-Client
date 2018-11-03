using libsignal.state;
using System.Collections.Generic;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal
{
    public interface ISignalKeyDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        List<SignedPreKeyRecord> getAllSignedPreKeys(string accountId);

        List<PreKeyRecord> getAllPreKeys(string accountId);

        SignedPreKeyRecord getSignedPreKey(uint signedPreKeyId, string accountId);

        void setPreKey(uint preKeyId, PreKeyRecord preKey, string accountId);

        void setSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord signedPreKey, string accountId);
        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        void deleteSignedPreKey(uint signedPreKeyId, string accountId);

        void deleteAllForAccount(string accountId);
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
