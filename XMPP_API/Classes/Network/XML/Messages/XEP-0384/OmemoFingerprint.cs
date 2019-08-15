using libsignal.ecc;
using XMPP_API.Classes.Crypto;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoFingerprint
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool trusted;
        public ECPublicKey publicKey;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public byte[] getByteArrayFingerprint()
        {
            return CryptoUtils.getRawFromECPublicKey(publicKey);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
