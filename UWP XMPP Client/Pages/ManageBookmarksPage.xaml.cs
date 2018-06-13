using Data_Manager2.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.DataTemplates;
using UWP_XMPP_Client.Pages.SettingsPages;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class ManageBookmarksPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ObservableCollection<string> accounts;
        private List<XMPPClient> clients;

        private MessageResponseHelper<IQMessage> messageResponseHelper;
        private CustomObservableCollection<MUCRoomTemplate> rooms;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 13/06/2018 Created [Fabian Sauter]
        /// </history>
        public ManageBookmarksPage()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += BrowseMUCRoomsPage_BackRequested;
            this.accounts = new ObservableCollection<string>();
            this.clients = null;
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
        /// <summary>
        /// Loads all accounts.
        /// </summary>
        private void loadAccounts()
        {
            clients = ConnectionHandler.INSTANCE.getClients();
            if (clients != null)
            {
                accounts.Clear();
                bool foundConnected = false;
                for (int i = 0; i < clients.Count; i++)
                {
                    accounts.Add(clients[i].getXMPPAccount().getIdAndDomain());
                    if (!foundConnected && clients[i].isConnected())
                    {
                        account_cbx.SelectedIndex = i;
                        foundConnected = true;
                    }
                }
            }
        }

        private void showErrorMessage(string msg)
        {
            info_itbx.Visibility = Visibility.Collapsed;
            error_itbx.Text = msg;
            error_itbx.Visibility = Visibility.Visible;
        }

        private void showInfoMessage(string msg)
        {
            error_itbx.Visibility = Visibility.Collapsed;
            info_itbx.Text = msg;
            info_itbx.Visibility = Visibility.Visible;
        }

        private void hideErrorMessage()
        {
            error_itbx.Visibility = Visibility.Collapsed;
        }

        private void requestBookmarks(XMPPClient c)
        {
            if (account_cbx.SelectedIndex >= 0 && account_cbx.SelectedIndex < clients.Count)
            {
                // main_grid.Visibility = Visibility.Collapsed;
                // loading_grid.Visibility = Visibility.Visible;
                noneFound_notification.Dismiss();

                //messageResponseHelper = Client.MUC_COMMAND_HELPER.requestRooms(Server, onMessage, onTimeout);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void account_cbx_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (account_cbx.SelectedIndex < 0 && account_cbx.Items.Count > 0)
            {
                account_cbx.SelectedIndex = 0;
            }
        }

        private void addAccount_tblck_Tapped(object sender, TappedRoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(AccountSettingsPage));
        }

        private void account_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (account_cbx.SelectedIndex >= 0 && account_cbx.SelectedIndex < clients.Count)
            {
                XMPPClient c = clients[account_cbx.SelectedIndex];
                if (!c.isConnected())
                {
                    refresh_btn.IsEnabled = false;
                    showErrorMessage("Account not connected!");
                }
                else
                {
                    requestBookmarks(c);
                    hideErrorMessage();
                }
            }
            else
            {
                refresh_btn.IsEnabled = false;
                hideErrorMessage();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UiUtils.setBackgroundImage(backgroundImage_img);
            loadAccounts();
        }

        private void BrowseMUCRoomsPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void refresh_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
