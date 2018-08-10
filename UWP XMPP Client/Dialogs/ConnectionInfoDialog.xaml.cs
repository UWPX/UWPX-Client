using System;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML;

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
        public static readonly DependencyProperty CertProperty = DependencyProperty.Register(nameof(Cert), typeof(Certificate), typeof(ConnectionInfoDialog), null);

        public ConnectionInformation ConnectionInfo
        {
            get { return (ConnectionInformation)GetValue(ConnectionInfoProperty); }
            set
            {
                SetValue(ConnectionInfoProperty, value);
                if (ConnectionInfo != null)
                {
                    ConnectionInfo.PropertyChanged -= Value_PropertyChanged;
                }
                if (value != null)
                {
                    value.PropertyChanged += Value_PropertyChanged;
                }
                showConnectionInfo();
            }
        }

        public static readonly DependencyProperty ConnectionInfoProperty = DependencyProperty.Register(nameof(ConnectionInfo), typeof(ConnectionInformation), typeof(ConnectionInfoDialog), null);

        public MessageParserStats ParserStats
        {
            get { return (MessageParserStats)GetValue(ParserStatsProperty); }
            set { SetValue(ParserStatsProperty, value); }
        }
        public static readonly DependencyProperty ParserStatsProperty = DependencyProperty.Register(nameof(ParserStats), typeof(MessageParserStats), typeof(ConnectionInfoDialog), null);

        public OmemoHelperState OmemoState
        {
            get { return (OmemoHelperState)GetValue(OmemoStateProperty); }
            set
            {
                SetValue(OmemoStateProperty, value);
                showOmemoState();
            }
        }
        public static readonly DependencyProperty OmemoStateProperty = DependencyProperty.Register(nameof(OmemoState), typeof(OmemoHelperState), typeof(ConnectionInfoDialog), new PropertyMetadata(OmemoHelperState.DISABLED));

        public XMPPAccount Account
        {
            get { return (XMPPAccount)GetValue(AccountProperty); }
            set
            {
                SetValue(AccountProperty, value);
                showAccount();
            }
        }
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(XMPPAccount), typeof(ConnectionInfoDialog), new PropertyMetadata(null));

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
            if (ConnectionInfo != null)
            {
                showTlsConnected();
                showMsgCarbonsState();
                showOmemoState();
            }
        }

        private void showTlsConnected()
        {
            if (ConnectionInfo.tlsConnected)
            {
                tlsConnected_run.Text = "Connected";
                tlsDisconnected_run.Text = "";
            }
            else
            {
                tlsConnected_run.Text = "";
                tlsDisconnected_run.Text = "Disconnected";
            }
        }

        private void showOmemoState()
        {
            switch (OmemoState)
            {
                case OmemoHelperState.REQUESTING_DEVICE_LIST:
                    omemoDisabled_run.Text = "";
                    omemoEnabled_run.Text = "";
                    omemoWip_run.Text = "Requested device list";
                    break;

                case OmemoHelperState.UPDATING_DEVICE_LIST:
                    omemoDisabled_run.Text = "";
                    omemoEnabled_run.Text = "";
                    omemoWip_run.Text = "Updating device list";
                    break;

                case OmemoHelperState.ANNOUNCING_BUNDLE_INFO:
                    omemoDisabled_run.Text = "";
                    omemoEnabled_run.Text = "";
                    omemoWip_run.Text = "Announcing bundle info";
                    break;

                case OmemoHelperState.ENABLED:
                    omemoDisabled_run.Text = "";
                    omemoEnabled_run.Text = "Enabled";
                    omemoWip_run.Text = "";
                    break;

                case OmemoHelperState.DISABLED:
                    omemoDisabled_run.Text = "Disabled";
                    omemoEnabled_run.Text = "";
                    omemoWip_run.Text = "";
                    break;

                default:
                    omemoDisabled_run.Text = "Error - view logs";
                    omemoEnabled_run.Text = "";
                    omemoWip_run.Text = "";
                    break;
            }
        }

        private void showMsgCarbonsState()
        {
            switch (ConnectionInfo.msgCarbonsState)
            {
                case MessageCarbonsState.DISABLED:
                    carbonsDisabled_run.Text = "Disabled";
                    carbonsEnabled_run.Text = "";
                    carbonsReqested_run.Text = "";
                    break;

                case MessageCarbonsState.UNAVAILABLE:
                    carbonsDisabled_run.Text = "Unavailable";
                    carbonsEnabled_run.Text = "";
                    carbonsReqested_run.Text = "";
                    break;

                case MessageCarbonsState.REQUESTED:
                    carbonsDisabled_run.Text = "";
                    carbonsEnabled_run.Text = "";
                    carbonsReqested_run.Text = "Requested";
                    break;

                case MessageCarbonsState.ENABLED:
                    carbonsDisabled_run.Text = "";
                    carbonsEnabled_run.Text = "Enabled";
                    carbonsReqested_run.Text = "";
                    break;

                case MessageCarbonsState.ERROR:
                    carbonsDisabled_run.Text = "Error";
                    carbonsEnabled_run.Text = "";
                    carbonsReqested_run.Text = "";
                    break;
            }
        }

        private void showAccount()
        {
            omemoFingerprint_ofc.MyFingerprint = Account?.getOmemoFingerprint();
            if (Account != null)
            {
                if (!Account.hasOmemoKeys())
                {
                    omemoError_itbx.Text = "OMEMO keys are corrupted. Please remove and add your account again!";
                    omemoError_itbx.Visibility = Visibility.Visible;
                }
                else
                {
                    omemoError_itbx.Visibility = Visibility.Collapsed;
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

        private async void Value_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (ConnectionInfo != null)
                {
                    switch (e.PropertyName)
                    {
                        case "msgCarbonsState":
                            showMsgCarbonsState();
                            break;

                        case "tlsConnected":
                            showTlsConnected();
                            break;
                    }
                }
            });
        }

        #endregion
    }
}
