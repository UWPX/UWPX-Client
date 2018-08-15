using org.whispersystems.libsignal.fingerprint;
using System.Text;
using System.Text.RegularExpressions;
using UWP_XMPP_Client.Classes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls.Omemo
{
    public sealed partial class OmemoFingerprintControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Fingerprint MyFingerprint
        {
            get { return (Fingerprint)GetValue(MyFingerprintProperty); }
            set
            {
                SetValue(MyFingerprintProperty, value);
                showFingerprint();
            }
        }
        public static readonly DependencyProperty MyFingerprintProperty = DependencyProperty.Register(nameof(MyFingerprint), typeof(Fingerprint), typeof(OmemoFingerprintControl), new PropertyMetadata(null));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoFingerprintControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showFingerprint()
        {
            if (MyFingerprint != null)
            {
                string displayFingerprint = MyFingerprint.getDisplayableFingerprint().getDisplayText();
                fingerprint_tbx.Text = Regex.Replace(displayFingerprint, ".{4}", "$0 ");
                fingerprintQRCode_qrcc.QRCodeText = Encoding.ASCII.GetString(MyFingerprint.getScannableFingerprint().getSerialized());
                cpyFingerprint_btn.IsEnabled = true;
            }
            else
            {
                fingerprint_tbx.Text = "Error! No fingerprint available.";
                fingerprintQRCode_qrcc.QRCodeText = null;
                cpyFingerprint_btn.IsEnabled = false;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void cpyFingerprint_btn_Click(object sender, RoutedEventArgs e)
        {
            if (MyFingerprint != null)
            {
                UiUtils.addTextToClipboard(MyFingerprint.getDisplayableFingerprint().getDisplayText());
            }
        }

        #endregion
    }
}
