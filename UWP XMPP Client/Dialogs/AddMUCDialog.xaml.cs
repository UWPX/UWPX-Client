using System;
using Data_Manager2.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.Pages.SettingsPages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBManager;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Pages;
using System.Threading.Tasks;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class AddMUCDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool cancled;
        private ObservableCollection<string> accounts;
        private List<DiscoFeatureTable> serverList;
        private ObservableCollection<string> servers;
        private List<XMPPClient> clients;

        private string requestedServer;
        private string requestedAccount;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/01/2018 Created [Fabian Sauter]
        /// </history>
        public AddMUCDialog() : this(null)
        {
        }

        public AddMUCDialog(string roomJid, string password, string userAccountId) : this(roomJid)
        {
            this.enablePassword_cbx.IsChecked = true;
            this.password_pwb.Password = password;
            this.password_pwb.Visibility = Visibility.Visible;
            this.requestedAccount = userAccountId;
        }

        public AddMUCDialog(string roomJid)
        {
            this.cancled = true;
            this.requestedAccount = null;
            this.accounts = new ObservableCollection<string>();
            this.serverList = new List<DiscoFeatureTable>();
            this.servers = new ObservableCollection<string>();
            InitializeComponent();
            loadAccounts();
            loadServers();

            if (roomJid != null)
            {
                requestedServer = Utils.getDomainFromBareJid(roomJid);
                roomName_tbx.Text = Utils.getUserFromBareJid(roomJid) ?? "";
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Loads all accounts.
        /// </summary>
        public void loadAccounts()
        {
            clients = ConnectionHandler.INSTANCE.getClients();
            if (clients != null)
            {
                accounts.Clear();
                foreach (XMPPClient c in clients)
                {
                    accounts.Add(c.getXMPPAccount().getIdAndDomain());
                }
            }
        }

        /// <summary>
        /// Loads all servers from the DB.
        /// </summary>
        private void loadServers()
        {
            serverList = DiscoDBManager.INSTANCE.getAllMUCServers();

            showServersFiltered(null);
        }

        /// <summary>
        /// Updates the auto suggest list based on the given filter text.
        /// </summary>
        /// <param name="filter">The filter for showing servers.</param>
        private void showServersFiltered(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                servers.Clear();
                foreach (DiscoFeatureTable f in serverList)
                {
                    servers.Add(f.fromServer);
                }
            }
            else
            {
                servers.Clear();
                foreach (DiscoFeatureTable f in serverList)
                {
                    if (f.fromServer.Contains(filter))
                    {
                        servers.Add(f.fromServer);
                    }
                }
            }
        }

        /// <summary>
        /// Tries to add the room.
        /// </summary>
        /// <returns>Returns true if the account got added.</returns>
        private bool addRoom()
        {
            if (checkUserInputsAndWarn())
            {
                XMPPClient c = clients[account_cbx.SelectedIndex];

                string roomJid = roomName_tbx.Text + '@' + server_asbx.Text.ToLower();

                ChatTable muc = new ChatTable()
                {
                    id = ChatTable.generateId(roomJid, c.getXMPPAccount().getIdAndDomain()),
                    ask = null,
                    chatJabberId = roomJid,
                    userAccountId = c.getXMPPAccount().getIdAndDomain(),
                    chatType = ChatType.MUC,
                    inRoster = (bool)bookmark_cbx.IsChecked,
                    muted = false,
                    lastActive = DateTime.Now,
                    subscription = "none"
                };
                ChatDBManager.INSTANCE.setChat(muc, false, true);

                MUCChatInfoTable info = new MUCChatInfoTable()
                {
                    chatId = muc.id,
                    subject = null,
                    state = MUCState.DISCONNECTED,
                    name = null,
                    password = null,
                    nickname = nick_tbx.Text,
                    autoEnterRoom = (bool)autoJoin_cbx.IsChecked
                };
                if ((bool)enablePassword_cbx.IsChecked)
                {
                    info.password = password_pwb.Password;
                }
                MUCDBManager.INSTANCE.setMUCChatInfo(info, false, true);

                if (info.autoEnterRoom)
                {
                    Task t = MUCHandler.INSTANCE.enterMUCAsync(c, muc, info);
                }

                if ((bool)bookmark_cbx.IsChecked)
                {
                    Task t = c.setBookmarkAsync(info.toConferenceItem(muc));
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the user input is valid.
        /// </summary>
        /// <returns>Returns whether the user input is valid.</returns>
        private bool checkUserInputsAndWarn()
        {
            if (account_cbx.SelectedIndex < 0)
            {
                showErrorTextBlock("No account selected!");
                return false;
            }

            if (!clients[account_cbx.SelectedIndex].isConnected())
            {
                showErrorTextBlock("Account is not connected!");
                accountNotConnected_tblck.Visibility = Visibility.Visible;
                return false;
            }

            if (string.IsNullOrWhiteSpace(nick_tbx.Text))
            {
                showErrorTextBlock("No nickname given!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(roomName_tbx.Text))
            {
                showErrorTextBlock("No room name given!");
                return false;
            }

            if (!Utils.isValidServerAddress(server_asbx.Text))
            {
                showErrorTextBlock("Invalid server!");
                return false;
            }

            string roomJid = roomName_tbx.Text + '@' + server_asbx.Text;
            XMPPClient c = clients[account_cbx.SelectedIndex];
            if (ChatDBManager.INSTANCE.doesChatExist(ChatTable.generateId(roomJid, c.getXMPPAccount().getIdAndDomain())))
            {
                showErrorTextBlock("You already joined this room!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Shows the error text block with the given text.
        /// </summary>
        /// <param name="msg">The text that should get shown in the error text block.</param>
        private void showErrorTextBlock(string msg)
        {
            error_tbx.Text = msg;
            error_tbx.Visibility = Visibility.Visible;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void account_cbx_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (account_cbx.Items.Count > 0)
            {
                if (requestedAccount != null)
                {
                    for (int i = 0; i < servers.Count; i++)
                    {
                        if (accounts[i].Equals(requestedAccount))
                        {
                            account_cbx.SelectedIndex = i;
                            return;
                        }
                    }
                }
                account_cbx.SelectedIndex = 0;
            }
        }

        private void addAccount_tblck_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Hide();
            (Window.Current.Content as Frame).Navigate(typeof(AccountSettingsPage));
        }

        private void enablePassword_cbx_Checked(object sender, RoutedEventArgs e)
        {
            password_pwb.Visibility = Visibility.Visible;
        }

        private void enablePassword_cbx_Unchecked(object sender, RoutedEventArgs e)
        {
            password_pwb.Visibility = Visibility.Collapsed;
        }

        private void roomName_tbx_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Space)
            {
                e.Handled = true;
            }
        }

        private void roomName_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            int selectionStart = roomName_tbx.SelectionStart;
            roomName_tbx.Text = roomName_tbx.Text.ToLower();
            roomName_tbx.SelectionStart = selectionStart;
            roomName_tbx.SelectionLength = 0;
        }

        private void browse_btn_Click(object sender, RoutedEventArgs e)
        {
            // Check if the current server address is valid:
            if (!Utils.isValidServerAddress(server_asbx.Text))
            {
                showErrorTextBlock("Invalid server!");
            }

            // Navigate to browse MUC page:
            (Window.Current.Content as Frame).Navigate(typeof(BrowseMUCRoomsPage), new BrowseMUCNavigationParameter(server_asbx.Text, clients[account_cbx.SelectedIndex]));
            Hide();
        }

        private void account_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (account_cbx.SelectedIndex >= 0)
            {
                nick_tbx.Text = Utils.getUserFromBareJid(accounts[account_cbx.SelectedIndex]);
                accountNotConnected_tblck.Visibility = clients[account_cbx.SelectedIndex].isConnected() ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            cancled = true;
            Hide();
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            if (addRoom())
            {
                cancled = false;
                Hide();
            }
        }

        private void server_asbx_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            showServersFiltered(server_asbx.Text);
            browse_btn.IsEnabled = Utils.isValidServerAddress(server_asbx.Text);
        }

        #endregion
    }
}
