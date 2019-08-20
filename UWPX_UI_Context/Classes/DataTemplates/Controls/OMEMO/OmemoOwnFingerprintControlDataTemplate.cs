using libsignal;
using libsignal.ecc;
using Shared.Classes;
using XMPP_API.Classes.Crypto;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO
{
    public class OmemoOwnFingerprintControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ECPublicKey _IdentPubKey;
        public ECPublicKey IdentPubKey
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
        public void UpdateView(AccountDataTemplate account)
        {
            if (!(account is null))
            {
                IdentityKey key = account.Account.omemoIdentityKeyPair?.getPublicKey();
                IdentPubKey = key.getPublicKey();
                QrCodeFingerprint = !(key is null) ? CryptoUtils.generateOmemoQrCodeFingerprint(key, account.Account) : null;
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
