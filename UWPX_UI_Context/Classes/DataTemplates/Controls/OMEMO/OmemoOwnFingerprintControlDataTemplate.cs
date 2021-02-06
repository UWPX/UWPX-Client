using Omemo.Classes.Keys;
using Shared.Classes;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO
{
    public class OmemoOwnFingerprintControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ECPubKeyModel _IdentPubKey;
        public ECPubKeyModel IdentPubKey
        {
            get => _IdentPubKey;
            set => SetProperty(ref _IdentPubKey, value);
        }
        private string _QrCodeFingerprint;
        public string QrCodeFingerprint
        {
            get => _QrCodeFingerprint;
            set => SetProperty(ref _QrCodeFingerprint, value);
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
        public void UpdateView(XMPPAccount account)
        {
            if (!(account is null))
            {
                IdentPubKey = account.omemoIdentityKey.pubKey;
                QrCodeFingerprint = CryptoUtils.generateOmemoQrCodeFingerprint(IdentPubKey, account);
                return;
            }
            IdentPubKey = null;
            QrCodeFingerprint = null;
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
