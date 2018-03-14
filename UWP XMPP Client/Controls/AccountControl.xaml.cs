using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using UWP_XMPP_Client.Dialogs;
using Data_Manager2.Classes.DBTables;

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
                showAccount();
            }
        }
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register("Account", typeof(XMPPAccount), typeof(AccountControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/09/2017 Created [Fabian Sauter]
        /// </history>
        public AccountControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public XMPPAccount getAccount()
        {
            return new XMPPAccount(getUser(), serverAddress_tbx.Text.ToLower(), int.Parse(serverPort_tbx.Text))
            {
                presencePriorety = (int)presencePriorety_slider.Value,
                disabled = !disableAccount_tggls.IsOn,
                color = color_tbx.Text
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
            if (Account != null)
            {
                IsEnabled = false;
                primaryInfo_tblck.Text = Account.user.userId ?? "";
                secondaryInfo_tblck.Text = Account.user.domain ?? "";
                jabberId_tbx.Text = Account.user.getIdAndDomain() ?? "";
                password_pwb.Password = Account.user.userPassword ?? "";
                resource_tbx.Text = Account.user.resource ?? "";
                serverAddress_tbx.Text = Account.serverAddress ?? "";
                serverPort_tbx.Text = Account.port.ToString();
                presencePriorety_slider.Value = Account.presencePriorety;
                disableAccount_tggls.IsOn = !Account.disabled;
                color_tbx.Text = Account.color ?? "";
                updateColor(color_tbx.Text);
                XMPPClient client = ConnectionHandler.INSTANCE.getClient(Account.getIdAndDomain());
                if (client != null)
                {
                    showConnectionState(client.getConnetionState(), client.getLastErrorMessage());
                    client.ConnectionStateChanged += Client_ConnectionStateChanged;
                }
                IsEnabled = true;
            }
        }

        private void replaceAccount(XMPPAccount account)
        {
            if (!IsEnabled || account.Equals(Account))
            {
                return;
            }
            XMPPAccount oldAccount = Account;
            Account = account;
            AccountDBManager.INSTANCE.replaceAccount(oldAccount, Account);
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

        private void showConnectionState(ConnectionState state, object param)
        {
            error_tblck.Visibility = Visibility.Collapsed;
            switch (state)
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
                    image_aciwp.Presence = Presence.Chat;
                    break;
                case ConnectionState.ERROR:
                    image_aciwp.Presence = Presence.Dnd;
                    showError(param);
                    break;
                default:
                    break;
            }
        }

        private void showError(object param)
        {
            if (param is string)
            {
                error_tblck.Visibility = Visibility.Visible;
                error_tblck.Text = param.ToString();
            }
        }

        private void updateColor(string color)
        {
            if (UiUtils.isHexColor(color))
            {
                color_tbx.Header = "Hex color:";
                color_rcta.Fill = UiUtils.convertHexColorToBrush(color);
            }
            else
            {
                color_tbx.Header = "Hex color (invalid):";
                color_rcta.Fill = new SolidColorBrush(Colors.Transparent);
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
                if (await saveAccountAsync())
                {
                    accountOptions_stckp.Visibility = Visibility.Collapsed;
                    edit_btn.Content = "\uE1C2";
                }
            }
        }

        private async void deleteAccount_btn_Click(object sender, RoutedEventArgs e)
        {
            DeleteAccountDialog dialog = new DeleteAccountDialog();
            await dialog.ShowAsync();
            XMPPAccount account = Account;
            Task t = Task.Run(async () =>
            {
                if (dialog.deleteAccount)
                {
                    await ConnectionHandler.INSTANCE.removeAccountAsync(account.getIdAndDomain());

                    AccountDBManager.INSTANCE.deleteAccount(account, true);

                    if (!dialog.keepChatMessages)
                    {
                        foreach (ChatTable chat in ChatDBManager.INSTANCE.getAllChatsForClient(account.getIdAndDomain(), null))
                        {
                            ChatDBManager.INSTANCE.deleteAllChatMessagesForChat(chat.id);
                        }
                    }

                    if (!dialog.keepChats)
                    {
                        ChatDBManager.INSTANCE.deleteAllChatsForAccount(account.getIdAndDomain());
                    }
                }
            });
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
            if (Account != null && Account.disabled == disableAccount_tggls.IsOn)
            {
                Account.disabled = !disableAccount_tggls.IsOn;
                XMPPAccount newAccount = Account.clone();
                Task.Run(() => AccountDBManager.INSTANCE.setAccountDisabled(newAccount, newAccount.disabled));
            }
        }

        private async void Client_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.Events.ConnectionStateChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showConnectionState(args.newState, args.param));
        }

        private void color_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateColor(color_tbx.Text);
        }

        private void randomColor_btn_Click(object sender, RoutedEventArgs e)
        {
            color_tbx.Text = UiUtils.getRandomMaterialColor();
        }
        #endregion
    }
}
