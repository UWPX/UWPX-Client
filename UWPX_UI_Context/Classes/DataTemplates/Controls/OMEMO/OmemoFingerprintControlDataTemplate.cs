using libsignal.ecc;
using Shared.Classes;
using XMPP_API.Classes.Crypto;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO
{
    public sealed class OmemoFingerprintControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private byte[] _Fingerprint;
        public byte[] Fingerprint
        {
            get => _Fingerprint;
            set => SetProperty(ref _Fingerprint, value);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ECPublicKey identityPubKey)
        {
            Fingerprint = !(identityPubKey is null) ? CryptoUtils.getRawFromECPublicKey(identityPubKey) : null;
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
