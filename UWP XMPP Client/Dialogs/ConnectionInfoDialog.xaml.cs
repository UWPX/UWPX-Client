using System;
using System.Collections.Generic;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

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
                    value.PropertyChanged -= Value_PropertyChanged;
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

        public XMPPClient Client
        {
            get { return (XMPPClient)GetValue(ClientProperty); }
            set
            {
                SetValue(ClientProperty, value);
                showClient();
            }
        }
        public static readonly DependencyProperty ClientProperty = DependencyProperty.Register(nameof(Client), typeof(XMPPClient), typeof(ConnectionInfoDialog), new PropertyMetadata(null));

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

        private void showClient()
        {
            omemoFingerprint_ofc.MyFingerprint = Client?.getXMPPAccount().getOmemoFingerprint();
            if (Client != null)
            {
                if (!Client.getXMPPAccount().hasOmemoKeys())
                {
                    omemoError_itbx.Text = "OMEMO keys are corrupted. Please remove and add your account again!";
                    omemoError_itbx.Visibility = Visibility.Visible;
                }
                else
                {
                    omemoError_itbx.Visibility = Visibility.Collapsed;
                }

                OmemoDevices devices = Client.getOmemoHelper().DEVICES;
                if (devices != null)
                {
                    omemoDevices_odc.setDevices(devices.DEVICES);
                    omemoDevicesInfo_tbx.Visibility = Visibility.Collapsed;
                }
                else
                {
                    omemoDevices_odc.setDevices(new List<uint>());
                    omemoDevicesInfo_tbx.Visibility = Visibility.Visible;
                }

                uint deviceId = Client.getXMPPAccount().omemoDeviceId;
                if (deviceId != 0)
                {
                    omemoDeviceId_run.Text = deviceId.ToString();
                }
                else
                {
                    omemoDeviceId_run.Text = "-";
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

        private void resetOmemoDevices_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Client != null)
            {
                resetOmemoDevices_btn.IsEnabled = false;
                resetOmemoDevices_pgr.Visibility = Visibility.Visible;
                Client.getOmemoHelper().resetDeviceListStateless(onResetDeviceListResult);
            }
        }

        private void refreshOmemoDevices_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Client != null)
            {
                refreshOmemoDevices_btn.IsEnabled = false;
                refreshOmemoDevices_pgr.Visibility = Visibility.Visible;
                Client.getOmemoHelper().requestDeviceListStateless(onDeviceListResult);
            }
        }

        private async void onResetDeviceListResult(bool success)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                resetOmemoDevices_pgr.Visibility = Visibility.Collapsed;
                resetOmemoDevices_btn.IsEnabled = true;
            });
        }

        private async void onDeviceListResult(bool success, OmemoDevices devices)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (success)
                {
                    omemoDevices_odc.setDevices(devices.DEVICES);
                    refreshOmemoDevices_pgr.Visibility = Visibility.Collapsed;
                    refreshOmemoDevices_btn.IsEnabled = true;
                }
            });
        }

        private void ContentDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ConnectionInfo != null)
            {
                ConnectionInfo.PropertyChanged -= Value_PropertyChanged;
            }
        }

        #endregion
    }
}
