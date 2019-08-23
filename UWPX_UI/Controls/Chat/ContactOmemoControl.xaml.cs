using Data_Manager2.Classes.DBTables;
using UWPX_UI.Dialogs;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Controls.Chat;
using UWPX_UI_Context.Classes.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWPX_UI.Controls.Chat
{
    public sealed partial class ContactOmemoControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatTable Chat
        {
            get { return (ChatTable)GetValue(ChatProperty); }
            set { SetValue(ChatProperty, value); }
        }
        public static readonly DependencyProperty ChatProperty = DependencyProperty.Register(nameof(Chat), typeof(ChatTable), typeof(ContactOmemoControl), new PropertyMetadata(null, OnChatChanged));

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register(nameof(Client), typeof(XMPPClient), typeof(ContactOmemoControl), new PropertyMetadata(null, OnClientChanged));

        public readonly ContactOmemoControlContext VIEW_MODEL = new ContactOmemoControlContext();
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ContactOmemoControl()
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
        private static void OnChatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContactOmemoControl contactInfoControl)
            {
                contactInfoControl.UpdateView(e);
            }
        }

        private static void OnClientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContactOmemoControl contactInfoControl)
            {
                contactInfoControl.UpdateView(e);
            }
        }

        private async void ScanQrCode_mfo_Click(object sender, RoutedEventArgs e)
        {
            QrCodeScannerDialog dialog = new QrCodeScannerDialog();
            await UiUtils.ShowDialogAsync(dialog);
        }

        private async void ShowOwnFingerprint_mfo_Click(object sender, RoutedEventArgs e)
        {
            OmemoOwnFingerprintDialog dialog = new OmemoOwnFingerprintDialog()
            {
                Account = Client.getXMPPAccount()
            };
            await UiUtils.ShowDialogAsync(dialog);
        }

        private void OmemoTrustFingerprintControl_OmemoFingerprintTrustChanged(object sender, OmemoFingerprintTrustChangedEventArgs args)
        {
            VIEW_MODEL.OnFingerprintTrustedChanged(args.FINGERPRINT);
        }
        #endregion
    }
}
