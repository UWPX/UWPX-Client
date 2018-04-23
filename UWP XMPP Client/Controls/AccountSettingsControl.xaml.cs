using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using XMPP_API.Classes.Network;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using UWP_XMPP_Client.Dialogs;
using Data_Manager2.Classes.DBTables;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class AccountSettingsControl : UserControl
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
        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register("Account", typeof(XMPPAccount), typeof(AccountSettingsControl), null);

        private XMPPClient client;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/09/2017 Created [Fabian Sauter]
        /// </history>
        public AccountSettingsControl()
        {
            this.client = null;
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
        private async Task showErrorDialogAsync(string text)
        {
            TextDialog dialog = new TextDialog(text, Localisation.getLocalizedString("error_text"));
            await dialog.ShowAsync();
        }

        private void showAccount()
        {
            if (Account != null)
            {
                IsEnabled = false;
                if (client != null)
                {
                    client.ConnectionStateChanged -= Client_ConnectionStateChanged;
                }
                string accountId = Account.getIdAndDomain();
                disableAccount_tggls.IsOn = !Account.disabled;
                Task.Run(() =>
                {
                    client = ConnectionHandler.INSTANCE.getClient(accountId);

                    if (client != null)
                    {
                        client.ConnectionStateChanged -= Client_ConnectionStateChanged;
                        client.ConnectionStateChanged += Client_ConnectionStateChanged;
                        client.getXMPPAccount().CONNECTION_INFO.PropertyChanged -= CONNECTION_INFO_PropertyChanged;
                        client.getXMPPAccount().CONNECTION_INFO.PropertyChanged += CONNECTION_INFO_PropertyChanged;
                        Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showConnectionState(client, client.getConnetionState(), client.getLastErrorMessage())).AsTask();
                    }
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsEnabled = true).AsTask();
                });
            }
        }

        private void replaceAccount(XMPPAccount account)
        {
            if (!IsEnabled || account.Equals(Account))
            {
                return;
            }

            XMPPAccount oldAccount = Account;

            Task.Run(() => AccountDBManager.INSTANCE.replaceAccount(oldAccount, account));

            Account = account;
        }

        private async Task<bool> saveAccountAsync()
        {
            if (await account_acc.isAccountVaildAsync())
            {
                XMPPAccount acc = account_acc.getAccount();
                if (acc == null)
                {
                    await showErrorDialogAsync(Localisation.getLocalizedString("invalid_jabber_id_text"));
                }
                else
                {
                    replaceAccount(acc);
                }
                return acc != null;
            }
            return false;
        }

        private void updateSecurityButton(XMPPClient client, ConnectionState state)
        {
            if (client != null && state == ConnectionState.CONNECTED)
            {
                if (client.getXMPPAccount().CONNECTION_INFO.tlsConnected)
                {
                    showSecurity_btn.Foreground = new SolidColorBrush(Colors.Green);
                    showSecurity_btn.Content = "\uE72E";
                }
                else
                {
                    showSecurity_btn.Foreground = new SolidColorBrush(Colors.Red);
                    showSecurity_btn.Content = "\uE785";
                }

                showSecurity_btn.IsEnabled = true;
            }
            else
            {
                showSecurity_btn.IsEnabled = false;
                showSecurity_btn.Foreground = new SolidColorBrush(Colors.Red);
                showSecurity_btn.Content = "\uE785";
            }
        }

        private void showConnectionState(XMPPClient client, ConnectionState state, object param)
        {
            updateSecurityButton(client, state);

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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showConnectionState(client, args.newState, args.param));
        }

        private void showSecurity_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void CONNECTION_INFO_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "tlsConnected":
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => updateSecurityButton(client, client.getConnetionState()));
                    break;
            }
        }

        #endregion
    }
}
