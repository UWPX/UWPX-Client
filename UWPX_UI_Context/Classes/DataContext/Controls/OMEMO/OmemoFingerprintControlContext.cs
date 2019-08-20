using libsignal.ecc;
using Windows.UI.Xaml;
using XMPP_API.Classes.Crypto;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO
{
    public sealed class OmemoFingerprintControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoFingerprintControlDataTemplate MODEL = new OmemoFingerprintControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ECPublicKey identityPubKey)
            {
                MODEL.UpdateView(identityPubKey);
            }
            else
            {
                MODEL.UpdateView(null);
            }
        }

        public void CopyFingerprintToClipboard()
        {
            UiUtils.SetClipboardText(CryptoUtils.generateOmemoFingerprint(MODEL.Fingerprint));
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
