using System;
using Data_Manager2.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBManager;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Pages;
using System.Threading.Tasks;
using XMPP_API.Classes.Network.XML.Messages.XEP_0048;
using XMPP_API.Classes.Network.XML.Messages;
using Windows.UI.Core;
using Logging;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class AddMUCDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool cancled;
        private List<DiscoFeatureTable> serverList;
        private readonly ObservableCollection<string> SERVERS;

        MessageResponseHelper<IQMessage> setBookmarkHelper;

        private string requestedServer;

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
            this.accountSelection_asc.setSelectedAccount(userAccountId);
            this.password_pwb.Visibility = Visibility.Visible;
        }

        public AddMUCDialog(string roomJid)
        {
            this.setBookmarkHelper = null;
            this.cancled = true;
            this.serverList = new List<DiscoFeatureTable>();
            this.SERVERS = new ObservableCollection<string>();
            InitializeComponent();

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
                SERVERS.Clear();
                foreach (DiscoFeatureTable f in serverList)
                {
                    SERVERS.Add(f.fromServer);
                }
            }
            else
            {
                SERVERS.Clear();
                foreach (DiscoFeatureTable f in serverList)
                {
                    if (f.fromServer.Contains(filter))
                    {
                        SERVERS.Add(f.fromServer);
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
            if (checkUserInputAndWarn())
            {
                XMPPClient client = accountSelection_asc.getSelectedAccount();

                string roomJid = roomName_tbx.Text + '@' + server_asbx.Text.ToLowerInvariant();

                ChatTable muc = new ChatTable
                {
                    id = ChatTable.generateId(roomJid, client.getXMPPAccount().getIdAndDomain()),
                    ask = null,
                    chatJabberId = roomJid,
                    userAccountId = client.getXMPPAccount().getIdAndDomain(),
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
                    Task t = MUCHandler.INSTANCE.enterMUCAsync(client, muc, info);
                }

                if ((bool)bookmark_cbx.IsChecked)
                {
                    List<ConferenceItem> conferenceItems = MUCDBManager.INSTANCE.getXEP0048ConferenceItemsForAccount(client.getXMPPAccount().getIdAndDomain());
                    setBookmarkHelper = client.PUB_SUB_COMMAND_HELPER.setBookmars_xep_0048(conferenceItems, onMessage, onTimeout);
                }

                return true;
            }
            return false;
        }

        private bool onMessage(IQMessage msg)
        {
            if (msg is IQErrorMessage errMsg)
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Logger.Warn("Failed to set bookmarks: " + errMsg.ToString());
                    accountSelection_asc.showErrorMessage("Failed to add bookmark - server error!");
                    add_pgr.Visibility = Visibility.Collapsed;
                    add_btn.IsEnabled = true;
                }).AsTask();
                return true;
            }
            if (string.Equals(msg.TYPE, IQMessage.RESULT))
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Hide()).AsTask();
                return true;
            }
            return false;
        }

        private void onTimeout()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                accountSelection_asc.showErrorMessage("Failed to add bookmark - server timeout!");
                add_pgr.Visibility = Visibility.Collapsed;
                add_btn.IsEnabled = true;
            }).AsTask();
        }

        /// <summary>
        /// Checks if the user input is valid.
        /// </summary>
        /// <returns>Returns whether the user input is valid.</returns>
        private bool checkUserInputAndWarn()
        {
            XMPPClient client = accountSelection_asc.getSelectedAccount();
            if (client == null)
            {
                accountSelection_asc.showErrorMessage("No account selected!");
                return false;
            }

            if (!client.isConnected())
            {
                accountSelection_asc.showErrorMessage("Account not connected!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(nick_tbx.Text))
            {
                accountSelection_asc.showErrorMessage("No nickname given!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(roomName_tbx.Text))
            {
                accountSelection_asc.showErrorMessage("No room name given!");
                return false;
            }

            if (!Utils.isValidServerAddress(server_asbx.Text))
            {
                accountSelection_asc.showErrorMessage("Invalid server!");
                return false;
            }

            string roomJid = roomName_tbx.Text + '@' + server_asbx.Text;
            if (ChatDBManager.INSTANCE.doesChatExist(ChatTable.generateId(roomJid, client.getXMPPAccount().getIdAndDomain())))
            {
                accountSelection_asc.showErrorMessage("Chat already exists!");
                return false;
            }

            accountSelection_asc.hideErrorMessage();
            return true;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void accountSelection_asc_AddAccountClicked(object sender, object args)
        {
            if(setBookmarkHelper != null)
            {
                setBookmarkHelper.Dispose();
                setBookmarkHelper = null;
            }
            Hide();
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
            roomName_tbx.Text = roomName_tbx.Text.ToLowerInvariant();
            roomName_tbx.SelectionStart = selectionStart;
            roomName_tbx.SelectionLength = 0;
        }

        private void browse_btn_Click(object sender, RoutedEventArgs e)
        {
            // Check if the current server address is valid:
            if (!Utils.isValidServerAddress(server_asbx.Text))
            {
                accountSelection_asc.showErrorMessage("Invalid server!");
            }

            // Navigate to browse MUC page:
            (Window.Current.Content as Frame).Navigate(typeof(BrowseMUCRoomsPage), new BrowseMUCNavigationParameter(server_asbx.Text, accountSelection_asc.getSelectedAccount()));
            Hide();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            if (setBookmarkHelper != null)
            {
                setBookmarkHelper.Dispose();
                setBookmarkHelper = null;
            }
            cancled = true;
            Hide();
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            add_pgr.Visibility = Visibility.Visible;
            add_btn.IsEnabled = false;
            if (addRoom())
            {
                cancled = false;
                if (setBookmarkHelper == null)
                {
                    Hide();
                }
            }
            else
            {
                add_pgr.Visibility = Visibility.Collapsed;
                add_btn.IsEnabled = true;
            }
        }

        private void server_asbx_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            showServersFiltered(server_asbx.Text);
            browse_btn.IsEnabled = Utils.isValidServerAddress(server_asbx.Text);
        }

        private void AccountSelectionControl_AccountSelectionChanged(Controls.AccountSelectionControl sender, Classes.Events.AccountSelectionChangedEventArgs args)
        {
            if (args.CLIENT != null)
            {
                nick_tbx.Text = args.CLIENT.getXMPPAccount().user.userId;
            }
            else
            {
                nick_tbx.Text = "";
            }
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            loadServers();
        }

        #endregion
    }
}
