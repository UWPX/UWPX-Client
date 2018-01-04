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
using UWP_XMPP_Client.DataTemplates;
using System.Threading;
using Windows.UI.Core;
using XMPP_API.Classes.Network.XML.Messages.XEP_0030;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Pages;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class AddMUCContentDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool cancled;
        private ObservableCollection<string> accounts;
        private ObservableCollection<string> servers;
        private List<XMPPClient> clients;

        private bool showingAddMUC;

        //private ObservableCollection<MUCRoomsDataTemplate> rooms;
        private string discoId;
        private Timer timer;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/01/2018 Created [Fabian Sauter]
        /// </history>
        public AddMUCContentDialog()
        {
            this.cancled = true;
            this.discoId = null;
            this.timer = null;
            //this.rooms = new ObservableCollection<MUCRoomsDataTemplate>();
            this.accounts = new ObservableCollection<string>();
            this.servers = new ObservableCollection<string>();
            InitializeComponent();
            showAddMUC();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
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

        private void loadServers()
        {
            servers.Clear();
            foreach (DiscoFeatureTable f in DiscoManager.INSTANCE.getAllMUCServers())
            {
                servers.Add(f.fromServer);
            }
        }

        private void showAddMUC()
        {
            showingAddMUC = true;
            Title = "Add MUC";
            loadAccounts();
            loadServers();
            browseRooms_grid.Visibility = Visibility.Collapsed;
            addMUC_stckpnl.Visibility = Visibility.Visible;
            add_btn.IsEnabled = true;
        }

        private void showBrowseRooms(string server, XMPPClient client)
        {
            stopTimer();
            discoId = null;
            showingAddMUC = false;
            Title = "Browse rooms";
            bindEvents(client);
            sendDiscoInfo(server, client);
            addMUC_stckpnl.Visibility = Visibility.Collapsed;
            browseRooms_grid.Visibility = Visibility.Visible;
            add_btn.IsEnabled = false;
        }

        private void startTimer()
        {
            timer = new Timer(async (obj) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(null));
            }, null, 5000, Timeout.Infinite);
        }

        private void bindEvents(XMPPClient client)
        {
            if (client != null)
            {
                client.NewDiscoResponseMessage += Client_NewDiscoResponseMessage;
            }
        }

        private void stopTimer()
        {
            timer?.Dispose();
        }

        private void showResultDisco(DiscoResponseMessage disco)
        {
            stopTimer();
            if (disco == null || disco.ITEMS == null || disco.ITEMS.Count <= 0)
            {
                showNoneFound();
                return;
            }
            //rooms.Clear();
            foreach (DiscoItem i in disco.ITEMS)
            {
                /*rooms.Add(new MUCRoomsDataTemplate()
                {
                    jid = i.JID ?? "",
                    name = i.NAME ?? ""
                });*/
            }
            loadingRooms_stckpnl.Visibility = Visibility.Collapsed;
            noneFound_stckl.Visibility = Visibility.Collapsed;
            rooms_itmc.Visibility = Visibility.Visible;
        }

        private void showNoneFound()
        {
            rooms_itmc.Visibility = Visibility.Collapsed;
            loadingRooms_stckpnl.Visibility = Visibility.Collapsed;
            noneFound_stckl.Visibility = Visibility.Visible;
        }

        private void sendDiscoInfo(string server, XMPPClient client)
        {
            loadingRooms_stckpnl.Visibility = Visibility.Visible;
            noneFound_stckl.Visibility = Visibility.Collapsed;
            rooms_itmc.Visibility = Visibility.Collapsed;
            if (discoId == null && client != null)
            {
                discoId = "";
                Task<string> t = client.createDiscoAsync(server, DiscoType.ITEMS);
                Task.Factory.StartNew(async () => discoId = await t);
                startTimer();
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cancle_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!showingAddMUC)
            {
                showAddMUC();
                return;
            }
            cancled = true;
            Hide();
        }

        private void account_cbx_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (account_cbx.Items.Count > 0)
            {
                account_cbx.SelectedIndex = 0;
            }
        }

        private void server_cbx_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (server_cbx.Items.Count > 0)
            {
                server_cbx.SelectedIndex = 0;
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
            var selectionStart = roomName_tbx.SelectionStart;
            roomName_tbx.Text = roomName_tbx.Text.ToLower();
            roomName_tbx.SelectionStart = selectionStart;
            roomName_tbx.SelectionLength = 0;
        }

        private void server_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            browse_btn.IsEnabled = server_cbx.SelectedIndex >= 0;
        }

        private void browse_btn_Click(object sender, RoutedEventArgs e)
        {
            object o = server_cbx.SelectedItem;
            if(o is string && account_cbx.SelectedIndex >= 0)
            {
                (Window.Current.Content as Frame).Navigate(typeof(BrowseMUCRoomsPage), new BrowseMUCNavigationParameter(o as string, clients[account_cbx.SelectedIndex]));
                Hide();
            }
        }

        private async void Client_NewDiscoResponseMessage(XMPPClient client, XMPP_API.Classes.Network.Events.NewDiscoResponseMessageEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => showResultDisco(args.DISCO));
        }

        private void refresh_btn_Click(object sender, RoutedEventArgs e)
        {
            object o = server_cbx.SelectedItem;
            if (o is string && account_cbx.SelectedIndex >= 0)
            {
                showBrowseRooms(o as string, clients[account_cbx.SelectedIndex]);
            }
        }
        #endregion
    }
}
