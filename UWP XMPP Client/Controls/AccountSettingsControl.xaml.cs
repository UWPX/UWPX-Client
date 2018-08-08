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
        private ConnectionError lastConnectionError;

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
            this.lastConnectionError = null;
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
            await UiUtils.showDialogAsyncQueue(dialog);
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
                        Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showConnectionState(client, client.getConnetionState(), client.getLastConnectionError())).AsTask();
                    }
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsEnabled = true).AsTask();
                });
            }
        }

        private void replaceAccount(XMPPAccount newAccount)
        {
            XMPPAccount oldAccount = Account;

            Task.Run(() => AccountDBManager.INSTANCE.replaceAccount(oldAccount, newAccount));

            Account = newAccount;
        }

        private async Task<bool> saveAccountAsync()
        {
            if (await account_acc.isAccountVaildAsync())
            {
                XMPPAccount newAccount = account_acc.getAccount();
                if (newAccount == null)
                {
                    await showErrorDialogAsync(Localisation.getLocalizedString("invalid_jabber_id_text"));
                }
                else
                {
                    replaceAccount(newAccount);
                }
                return newAccount != null;
            }
            return false;
        }

        private void updateSecurityButton(XMPPClient client, ConnectionState state)
        {
            if (client != null && state == ConnectionState.CONNECTED)
            {
                if (client.getXMPPAccount().CONNECTION_INFO.tlsConnected)
                {
                    accountSecurityStatus_tbx.Foreground = new SolidColorBrush(Colors.Green);
                    accountSecurityStatus_tbx.Text = "\uE72E";
                }
                else
                {
                    accountSecurityStatus_tbx.Foreground = new SolidColorBrush(Colors.Red);
                    accountSecurityStatus_tbx.Text = "\uE785";
                }
            }
            else
            {
                accountSecurityStatus_tbx.Foreground = UiUtils.getPresenceBrush(Presence.Unavailable);
                accountSecurityStatus_tbx.Text = "\uE785";
            }
        }

        private void showConnectionState(XMPPClient client, ConnectionState state, object param)
        {
            updateSecurityButton(client, state);

            error_tblck.Visibility = Visibility.Collapsed;
            lastConnectionError = null;

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
            if (param is ConnectionError connectionError)
            {
                lastConnectionError = connectionError;

                switch (connectionError.ERROR_CODE)
                {
                    case ConnectionErrorCode.UNKNOWN:
                        error_tblck.Text = connectionError.ERROR_MESSAGE ?? "";
                        break;

                    case ConnectionErrorCode.SOCKET_ERROR:
                        error_tblck.Text = connectionError.SOCKET_ERROR.ToString();
                        break;

                    default:
                        error_tblck.Text = connectionError.ERROR_CODE.ToString();
                        break;
                }

                error_tblck.Visibility = Visibility.Visible;
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
            await UiUtils.showDialogAsyncQueue(dialog);
            XMPPAccount account = Account;
            Task t = Task.Run(async () =>
            {
                if (dialog.deleteAccount)
                {
                    await ConnectionHandler.INSTANCE.removeAccountAsync(account.getIdAndDomain());

                    AccountDBManager.INSTANCE.deleteAccount(account, true, true);

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
                IsEnabled = false;
                Account.disabled = !disableAccount_tggls.IsOn;
                XMPPAccount newAccount = Account;
                Task.Run(() =>
                {
                    AccountDBManager.INSTANCE.setAccountDisabled(newAccount);
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsEnabled = true).AsTask();
                });
            }
        }

        private async void Client_ConnectionStateChanged(XMPPClient client, XMPP_API.Classes.Network.Events.ConnectionStateChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showConnectionState(client, args.newState, args.param));
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

        private async void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (error_tblck.Visibility == Visibility.Visible && lastConnectionError != null)
            {
                ConnectionErrorDialog dialog = new ConnectionErrorDialog(lastConnectionError);
                await dialog.ShowAsync();
            }
        }

        private async void showConnectionInfo_btn_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                ConnectionInfoDialog dialog = new ConnectionInfoDialog()
                {
                    Cert = client.getXMPPAccount().CONNECTION_INFO.socketInfo?.ServerCertificate,
                    ConnectionInfo = client.getXMPPAccount().CONNECTION_INFO,
                    ParserStats = client.getMessageParserStats()
                };
                await UiUtils.showDialogAsyncQueue(dialog);
            }
        }

        private async void accountSecurityStatus_tbx_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            string tlsState = "";
            if (client != null && client.getConnetionState() == ConnectionState.CONNECTED)
            {
                if (client.getXMPPAccount().CONNECTION_INFO.tlsConnected)
                {
                    tlsState = "connected";
                }
                else
                {
                    tlsState = "disconnected";
                }
            }
            else
            {
                tlsState = "client not connected";
            }
            TextDialog dialog = new TextDialog("TLS state: " + tlsState, "TLS state:");
            await dialog.ShowAsync();
        }

        #endregion
    }
}
