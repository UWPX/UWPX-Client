using Data_Manager2.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.Classes.Events;
using UWP_XMPP_Client.Pages.SettingsPages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class AccountSelectionControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ObservableCollection<string> ACCOUNTS;
        private readonly List<XMPPClient> CLIENTS;

        public delegate void AddAccountClickedHandler(AccountSelectionControl sender, AddAccountClickedEventArgs args);
        public delegate void AccountSelectionChangedHandler(AccountSelectionControl sender, AccountSelectionChangedEventArgs args);
        public event AddAccountClickedHandler AddAccountClicked;
        public event AccountSelectionChangedHandler AccountSelectionChanged;
        private string userAccountId;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 07/07/2018 Created [Fabian Sauter]
        /// </history>
        public AccountSelectionControl()
        {
            this.ACCOUNTS = new ObservableCollection<string>();
            this.CLIENTS = new List<XMPPClient>();
            this.userAccountId = null;
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public XMPPClient getSelectedAccount()
        {
            if (account_cbx.SelectedIndex >= 0 && account_cbx.SelectedIndex < CLIENTS.Count)
            {
                return CLIENTS[account_cbx.SelectedIndex];
            }
            return null;
        }

        public void setSelectedAccount(string userAccountId)
        {
            this.userAccountId = userAccountId;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void showErrorMessage(string msg)
        {
            info_itbx.Visibility = Visibility.Collapsed;
            error_itbx.Text = msg;
            error_itbx.Visibility = Visibility.Visible;
        }

        public void showInfoMessage(string msg)
        {
            error_itbx.Visibility = Visibility.Collapsed;
            info_itbx.Text = msg;
            info_itbx.Visibility = Visibility.Visible;
        }

        public void hideErrorMessage()
        {
            error_itbx.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void loadAccounts()
        {
            CLIENTS.Clear();
            CLIENTS.AddRange(ConnectionHandler.INSTANCE.getClients());

            ACCOUNTS.Clear();
            int foundConnected = -1;
            for (int i = 0; i < CLIENTS.Count; i++)
            {
                ACCOUNTS.Add(CLIENTS[i].getXMPPAccount().getIdAndDomain());
                if (userAccountId != null && string.Equals(userAccountId, CLIENTS[i].getXMPPAccount().getIdAndDomain()))
                {
                    foundConnected = i;
                }
                else if (foundConnected < 0 && CLIENTS[i].isConnected())
                {
                    foundConnected = i;
                }
            }

            if (foundConnected < 0)
            {
                foundConnected = 0;
            }
            account_cbx.SelectedIndex = foundConnected;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void account_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (account_cbx.SelectedIndex >= 0 && account_cbx.SelectedIndex < CLIENTS.Count)
            {
                if (!CLIENTS[account_cbx.SelectedIndex].isConnected())
                {
                    showErrorMessage("Account not connected!");
                    return;
                }
                else
                {
                    hideErrorMessage();
                }
                AccountSelectionChanged?.Invoke(this, new AccountSelectionChangedEventArgs(CLIENTS[account_cbx.SelectedIndex]));
            }
            else
            {
                hideErrorMessage();
                AccountSelectionChanged?.Invoke(this, new AccountSelectionChangedEventArgs(null));
            }
        }

        private void addAccount_link_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(AccountSettingsPage));
            AddAccountClicked?.Invoke(this, new AddAccountClickedEventArgs());
        }

        private void account_cbx_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (account_cbx.SelectedIndex < 0 && account_cbx.Items.Count > 0)
            {
                account_cbx.SelectedIndex = 0;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadAccounts();
        }

        #endregion
    }
}
