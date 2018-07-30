using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Pages.SettingsPages;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class AddChatDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool addToRoster;
        public bool requestSubscription;
        public string jabberId;
        public bool cancled;
        public XMPPClient client;
        private ObservableCollection<string> accounts;
        private List<XMPPClient> clients;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/09/2017 Created [Fabian Sauter]
        /// </history>
        public AddChatDialog()
        {
            this.InitializeComponent();
            loadAccounts();
            this.cancled = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void loadAccounts()
        {
            accounts = new ObservableCollection<string>();
            clients = ConnectionHandler.INSTANCE.getClients();
            foreach (XMPPClient c in clients)
            {
                accounts.Add(c.getXMPPAccount().getIdAndDomain());
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private async Task addChatAsync()
        {
            if (account_cbx.SelectedIndex < 0 || account_cbx.SelectedIndex >= clients.Count)
            {
                TextDialog dialog = new TextDialog()
                {
                    Title = "Error",
                    Text = "Please select a valid account!"
                };
                await UiUtils.showDialogAsyncQueue(dialog);
            }
            else if (((string)account_cbx.SelectedItem).Equals(jabberId_tbx.Text))
            {
                TextDialog dialog = new TextDialog()
                {
                    Title = "Error",
                    Text = "You can't start a chat with your self!"
                };
                await UiUtils.showDialogAsyncQueue(dialog);
            }
            else if (Utils.isBareJid(jabberId_tbx.Text))
            {
                jabberId = jabberId_tbx.Text;
                client = clients[account_cbx.SelectedIndex];
                if (ChatDBManager.INSTANCE.doesChatExist(ChatTable.generateId(jabberId, client.getXMPPAccount().getIdAndDomain())))
                {
                    TextDialog dialog = new TextDialog()
                    {
                        Title = "Error",
                        Text = "Chat does already exist!"
                    };
                    await UiUtils.showDialogAsyncQueue(dialog);
                }
                else
                {
                    addToRoster = (bool)roster_cbx.IsChecked;
                    requestSubscription = (bool)subscription_cbx.IsChecked;
                    cancled = false;
                    Hide();
                }
            }
            else
            {
                TextDialog dialog = new TextDialog()
                {
                    Title = "Error",
                    Text = "Invalid JabberID!"
                };
                await UiUtils.showDialogAsyncQueue(dialog);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void add_btn_Click(object sender, RoutedEventArgs e)
        {
            await addChatAsync();
        }

        private void cancle_btn_Click(object sender, RoutedEventArgs e)
        {
            cancled = true;
            Hide();
        }

        private void jabberId_tbx_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Space)
            {
                e.Handled = true;
            }
        }

        private void account_cbx_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (account_cbx.Items.Count > 0)
            {
                account_cbx.SelectedIndex = 0;
            }
        }

        private void addAccount_tblck_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Hide();
            (Window.Current.Content as Frame).Navigate(typeof(AccountSettingsPage));

        }

        private void jabberId_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            int selectionStart = jabberId_tbx.SelectionStart;
            jabberId_tbx.Text = jabberId_tbx.Text.ToLower();
            jabberId_tbx.SelectionStart = selectionStart;
            jabberId_tbx.SelectionLength = 0;
            jabberId_tbx.BorderBrush = new SolidColorBrush(Utils.isBareJid(jabberId_tbx.Text) ? Colors.Green : Colors.Red);
        }

        private async void jabberId_tbx_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await addChatAsync();
            }
        }

        #endregion
    }
}
