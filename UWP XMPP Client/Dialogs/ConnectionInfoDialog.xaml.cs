using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network.TCP;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class ConnectionInfoDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Certificate Cert
        {
            get { return (Certificate)GetValue(CertProperty); }
            set { SetValue(CertProperty, value); }
        }
        public static readonly DependencyProperty CertProperty = DependencyProperty.Register("Cert", typeof(Certificate), typeof(ConnectionInfoDialog), null);

        public ConnectionInformation ConnectionInfo
        {
            get { return (ConnectionInformation)GetValue(ConnectionInfoProperty); }
            set
            {
                SetValue(ConnectionInfoProperty, value);
                showConnectionInfo();
            }
        }
        public static readonly DependencyProperty ConnectionInfoProperty = DependencyProperty.Register("ConnectionInfo", typeof(ConnectionInformation), typeof(ConnectionInfoDialog), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 30/05/2018 Created [Fabian Sauter]
        /// </history>
        public ConnectionInfoDialog()
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
        private void showConnectionInfo()
        {
            tlsConnected_run.Text = "";
            tlsDisconnected_run.Text = "Disconnected";

            if (ConnectionInfo != null)
            {
                if (ConnectionInfo.tlsConnected)
                {
                    tlsConnected_run.Text = "Connected";
                    tlsDisconnected_run.Text = "";
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        #endregion
    }
}
