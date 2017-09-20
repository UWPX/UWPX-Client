using Data_Manager.Classes;
using Data_Manager.Classes.Managers;
using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Pages.SettingsPages;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class AccountControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ServerConnectionConfiguration Account
        {
            get { return (ServerConnectionConfiguration)GetValue(AccountProperty); }
            set
            {
                SetValue(AccountProperty, value);
                showAccount();
            }
        }
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register("Account", typeof(ServerConnectionConfiguration), typeof(AccountControl), null);

        private readonly AccountSettingsPage SETTINGS_PAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/09/2017 Created [Fabian Sauter]
        /// </history>
        public AccountControl(AccountSettingsPage settingsPage)
        {
            this.SETTINGS_PAGE = settingsPage;
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public ServerConnectionConfiguration getAccount()
        {
            return new ServerConnectionConfiguration(getUser(), serverAddress_tbx.Text, int.Parse(serverPort_tbx.Text))
            {
                presencePriorety = (int)presencePriorety_slider.Value,
                disabled = !disableAccount_tggls.IsOn
            };
        }

        public XMPPUser getUser()
        {
            string userId = jabberId_tbx.Text.ToLower().Substring(0, jabberId_tbx.Text.IndexOf('@'));
            string domain = jabberId_tbx.Text.ToLower().Substring(jabberId_tbx.Text.IndexOf('@') + 1);
            return new XMPPUser(userId, password_pwb.Password, domain, resource_tbx.Text);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void showAccount()
        {
            if(Account != null)
            {
                if(Account.user.name != null)
                {
                    primaryInfo_tblck.Text = Account.user.name ?? "";
                    secondaryInfo_tblck.Text = Account.getIdAndDomain() ?? "";
                }
                else
                {
                    primaryInfo_tblck.Text = Account.user.userId ?? "";
                    secondaryInfo_tblck.Text = Account.user.domain ?? "";
                }
                jabberId_tbx.Text = Account.user.getIdAndDomain() ?? "";
                password_pwb.Password = Account.user.userPassword ?? "";
                resource_tbx.Text = Account.user.resource ?? "";
                serverAddress_tbx.Text = Account.serverAddress ?? "";
                serverPort_tbx.Text = Account.port.ToString();
                presencePriorety_slider.Value = Account.presencePriorety;
                disableAccount_tggls.IsOn = !Account.disabled;
                XMPPClient client = ConnectionHandler.INSTANCE.getClientForAccount(Account);
                if(client != null)
                {
                    showConnectionState(client);
                    client.ConnectionStateChanged += Client_ConnectionStateChanged;
                }
            }
        }

        private void replaceAccount(ServerConnectionConfiguration account)
        {
            if(account.Equals(Account))
            {
                return;
            }
            ServerConnectionConfiguration oldAccount = Account;
            Account = account;
            UserManager.INSTANCE.replaceAccount(oldAccount, Account);
            ConnectionHandler.INSTANCE.reloadAllAccounts();
            SETTINGS_PAGE.loadAccounts();
        }

        private async Task<bool> saveAccountAsync()
        {
            if (await areEntriesValidAsync())
            {
                replaceAccount(getAccount());
                return true;
            }
            return false;
        }

        private async Task<bool> areEntriesValidAsync()
        {
            if (!jabberId_tbx.Text.Contains("@") || jabberId_tbx.Text.Length <= 3)
            {
                MessageDialog messageDialog = new MessageDialog(Localisation.getLocalizedString("invalid_jabber_id_text"), Localisation.getLocalizedString("error_text"));
                await messageDialog.ShowAsync();
                return false;
            }
            if (resource_tbx.Text == "")
            {
                MessageDialog messageDialog = new MessageDialog(Localisation.getLocalizedString("invalid_resource_text"), Localisation.getLocalizedString("error_text"));
                await messageDialog.ShowAsync();
                return false;
            }
            if (serverAddress_tbx.Text == "")
            {
                MessageDialog messageDialog = new MessageDialog(Localisation.getLocalizedString("invalid_server_text"), Localisation.getLocalizedString("error_text"));
                await messageDialog.ShowAsync();
                return false;
            }
            if (serverPort_tbx.Text == "")
            {
                MessageDialog messageDialog = new MessageDialog(Localisation.getLocalizedString("invalid_port_text"), Localisation.getLocalizedString("error_text"));
                await messageDialog.ShowAsync();
                return false;
            }
            return true;
        }

        private async Task<bool> shouldDeleteAsync()
        {
            MessageDialog dialog = new MessageDialog("Do you really want to delet this account?");
            dialog.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "No", Id = 1 });
            IUICommand command = await dialog.ShowAsync();
            return ((int)command.Id) == 0;
        }

        private void showConnectionState(XMPPClient client)
        {
            if(client == null)
            {
                return;
            }
            switch (client.getConnetionState())
            {
                case ConnectionState.DISCONNECTED:
                    image_aciwp.Presence = Presence.Unavailable;
                    break;
                case ConnectionState.CONNECTING:
                    image_aciwp.Presence = Presence.Chat;
                    break;
                case ConnectionState.CONNECTED:
                    image_aciwp.Presence = Presence.Online;
                    break;
                case ConnectionState.DISCONNECTING:
                    image_aciwp.Presence = Presence.Xa;
                    break;
                case ConnectionState.ERROR:
                    image_aciwp.Presence = Presence.Dnd;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (accountOptions_stckp.Visibility == Visibility.Collapsed)
            {
                accountOptions_stckp.Visibility = Visibility.Visible;
                edit_btn.Content = "\uE105";
            }
            else
            {
                if(await saveAccountAsync())
                {
                    accountOptions_stckp.Visibility = Visibility.Collapsed;
                    edit_btn.Content = "\uE1C2";
                }
            }
        }

        private async void deleteAccount_btn_Click(object sender, RoutedEventArgs e)
        {
            if(!await shouldDeleteAsync())
            {
                return;
            }
            UserManager.INSTANCE.deleteAccount(Account);
            SETTINGS_PAGE.loadAccounts();
            ConnectionHandler.INSTANCE.reloadAllAccounts();
        }

        private void jabberId_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (jabberId_tbx.Text.Contains("@"))
            {
                serverAddress_tbx.Text = jabberId_tbx.Text.Substring(jabberId_tbx.Text.IndexOf('@') + 1);
            }
        }

        private void disableAccount_tggls_Toggled(object sender, RoutedEventArgs e)
        {
            if(Account != null)
            {
                replaceAccount(getAccount());
            }
        }

        private async void Client_ConnectionStateChanged(XMPPClient client, ConnectionState state)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                showConnectionState(client);
            });
        }

        #endregion
    }
}
