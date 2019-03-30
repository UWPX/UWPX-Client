using libsignal;
using Shared.Classes;
using XMPP_API.Classes.Crypto;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class OmemoFingerprintControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private byte[] _Fingerprint;
        public byte[] Fingerprint
        {
            get { return _Fingerprint; }
            set { SetProperty(ref _Fingerprint, value); }
        }
        private string _QrCodeFingerprint;
        public string QrCodeFingerprint
        {
            get { return _QrCodeFingerprint; }
            set { SetProperty(ref _QrCodeFingerprint, value); }
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
                if (!(key is null))
                {
                    Fingerprint = CryptoUtils.getRawFromECPublicKey(key.getPublicKey());
                    QrCodeFingerprint = CryptoUtils.generateOmemoQrCodeFingerprint(key, account.Account);
                }
                else
                {
                    Fingerprint = null;
                    QrCodeFingerprint = null;
                }
                return;
            }
            Fingerprint = null;
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
