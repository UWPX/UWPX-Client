using libsignal;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public interface IOmemoFingerprintStore
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        bool IsFingerprintTrusted(OmemoFingerprint fingerprint);

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        bool StoreFingerprint(OmemoFingerprint fingerprint);

        OmemoFingerprint LoadFingerprint(SignalProtocolAddress address);

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
