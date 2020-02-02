using libsignal.ecc;
using UWPX_UI_Context.Classes.DataTemplates.Controls.OMEMO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.OMEMO
{
    public sealed partial class OmemoFingerprintControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ECPublicKey IdentityPubKey
        {
            get => (ECPublicKey)GetValue(IdentityPubKeyProperty);
            set => SetValue(IdentityPubKeyProperty, value);
        }
        public static readonly DependencyProperty IdentityPubKeyProperty = DependencyProperty.Register(nameof(IdentityPubKey), typeof(ECPublicKey), typeof(OmemoFingerprintControl), new PropertyMetadata(null, OnIdentityPubKeyChanged));

        public Visibility CopyButtonVisibility
        {
            get => (Visibility)GetValue(CopyButtonVisibilityProperty);
            set => SetValue(CopyButtonVisibilityProperty, value);
        }
        public static readonly DependencyProperty CopyButtonVisibilityProperty = DependencyProperty.Register(nameof(CopyButtonVisibility), typeof(Visibility), typeof(OmemoFingerprintControl), new PropertyMetadata(Visibility.Visible));

        public readonly OmemoFingerprintControlContext VIEW_MODEL = new OmemoFingerprintControlContext();
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoFingerprintControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnIdentityPubKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is OmemoFingerprintControl omemoFingerprintControl)
            {
                omemoFingerprintControl.UpdateView(e);
            }
        }

        private void CopyFingerprint_btn_Click(object sender, RoutedEventArgs e)
        {
            VIEW_MODEL.CopyFingerprintToClipboard();
        }

        #endregion
    }
}
