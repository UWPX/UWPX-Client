using Manager.Classes;
using Omemo.Classes.Keys;
using Shared.Classes;
using XMPP_API.Classes.Crypto;

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
        public void UpdateView(Client client)
        {
            if (client is not null)
            {
                IdentPubKey = client.dbAccount.omemoInfo.identityKey.pubKey;
                QrCodeFingerprint = CryptoUtils.generateOmemoQrCodeFingerprint(IdentPubKey, client.xmppClient.getXMPPAccount());
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
