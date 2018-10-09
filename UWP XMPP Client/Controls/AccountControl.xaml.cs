using System;
using System.Linq;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Dialogs;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class AccountControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public XMPPAccount Account
        {
            get { return (XMPPAccount)GetValue(AccountProperty); }
            set
            {
                SetValue(AccountProperty, value);
                if (value != null && jabberId_tbx != null)
                {
                    jabberId_tbx.Text = value.getIdAndDomain();
                }
            }
        }
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register("Account", typeof(XMPPAccount), typeof(AccountControl), null);
        
        public bool JIDReadOnly
        {
            get { return (bool)GetValue(JIDReadOnlyProperty); }
            set { SetValue(JIDReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty JIDReadOnlyProperty = DependencyProperty.Register("JIDReadOnly", typeof(bool), typeof(AccountControl), new PropertyMetadata(false));
        
        public delegate void AccountAcceptedEventHandler(AccountControl sender, EventArgs args);
        public event AccountAcceptedEventHandler AccountAccepted;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/04/2018 Created [Fabian Sauter]
        /// </history>
        public AccountControl()
        {
            this.Account = new XMPPAccount(new XMPPUser(null, null), null, 5222);
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public XMPPAccount getAccount()
        {
            XMPPUser user = getUser();
            if (user != null)
            {
                Account.user = user;
                Account.serverAddress = serverAddress_tbx.Text.ToLowerInvariant();
                int.TryParse(serverPort_tbx.Text, out Account.port);
                Account.presencePriorety = (int)presencePriorety_slider.Value;
                Account.color = color_tbx.Text;
                Account.presence = Presence.Online;
                Account.status = "";

                return Account;
            }
            return null;
        }

        public XMPPUser getUser()
        {
            int index = jabberId_tbx.Text.IndexOf('@');
            if (index > 0)
            {
                string userId = jabberId_tbx.Text.ToLowerInvariant().Substring(0, index);
                string domain = jabberId_tbx.Text.ToLowerInvariant().Substring(index + 1);
                return new XMPPUser(userId, password_pwb.Password, domain, resource_tbx.Text);
            }
            return null;
        }

        public async Task<bool> isAccountVaildAsync()
        {
            if (!Utils.isBareJid(jabberId_tbx.Text))
            {
                await showErrorDialogAsync(Localisation.getLocalizedString("invalid_jabber_id_text"));
                return false;
            }
            if (string.IsNullOrEmpty(resource_tbx.Text))
            {
                await showErrorDialogAsync(Localisation.getLocalizedString("invalid_resource_text"));
                return false;
            }
            if (string.IsNullOrEmpty(serverAddress_tbx.Text))
            {
                await showErrorDialogAsync(Localisation.getLocalizedString("invalid_server_text"));
                return false;
            }
            if (string.IsNullOrEmpty(serverPort_tbx.Text) || !int.TryParse(serverPort_tbx.Text, out int x))
            {
                await showErrorDialogAsync(Localisation.getLocalizedString("invalid_port_text"));
                return false;
            }
            return true;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void updateColor(string color)
        {
            if (UiUtils.isHexColor(color))
            {
                color_tbx.Header = "Hex color:";
                color_rcta.Fill = UiUtils.convertHexColorToBrush(color);
                color_rcta.Visibility = Visibility.Visible;
            }
            else
            {
                color_tbx.Header = "Hex color (invalid):";
                color_rcta.Fill = new SolidColorBrush(Colors.Transparent);
                color_rcta.Visibility = Visibility.Collapsed;
            }
        }

        private void showDeviceName()
        {
            var deviceInfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
            resource_tbx.Text = deviceInfo.FriendlyName ?? "";
        }

        private async Task showErrorDialogAsync(string text)
        {
            TextDialog dialog = new TextDialog(text, Localisation.getLocalizedString("error_text"));
            await UiUtils.showDialogAsyncQueue(dialog);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (advancedOptions_stckpnl.Visibility == Visibility.Visible)
            {
                advancedOptions_stckpnl.Visibility = Visibility.Collapsed;
                advancedOptionsStatus_tblk.Text = "Show advanced options";
                advancedOptionsStatusArrow_tblk.Text = "\uE0AB";
            }
            else
            {
                advancedOptions_stckpnl.Visibility = Visibility.Visible;
                advancedOptionsStatus_tblk.Text = "Hide advanced options";
                advancedOptionsStatusArrow_tblk.Text = "\uE1FD";
            }
        }

        private void password_pwb_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(resource_tbx.Text))
            {
                resource_tbx.Focus(FocusState.Keyboard);
            }
            else if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AccountAccepted?.Invoke(this, new EventArgs());
            }
        }

        private void color_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateColor(color_tbx.Text);
        }

        private void randomColor_btn_Click(object sender, RoutedEventArgs e)
        {
            color_tbx.Text = UiUtils.getRandomColor();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            color_tbx.Text = UiUtils.getRandomColor();
            showDeviceName();
        }

        private void jabberId_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (jabberId_tbx.Text.Contains('@'))
            {
                serverAddress_tbx.Text = jabberId_tbx.Text.Substring(jabberId_tbx.Text.IndexOf('@') + 1);
            }
        }

        private void resource_tbx_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AccountAccepted?.Invoke(this, new EventArgs());
            }
        }

        private void serverPort_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Account != null)
            {
                int.TryParse(serverPort_tbx.Text, out Account.port);
            }
        }

        private async void changeCertificateRequirements_btn_Click(object sender, RoutedEventArgs e)
        {
            ChangeCertificateRequirementsDialog dialog = new ChangeCertificateRequirementsDialog(Account);
            await UiUtils.showDialogAsyncQueue(dialog);
        }

        private async void more_btn_Click(object sender, RoutedEventArgs e)
        {
            MoreAccountOptionsDialog dialog = new MoreAccountOptionsDialog(Account.connectionConfiguration);
            await UiUtils.showDialogAsyncQueue(dialog);
        }

        #endregion
    }
}
