using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UWP_XMPP_Client.DataTemplates;
using UWP_XMPP_Client.Pages.SettingsPages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class ChangeAccountPresenceDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ObservableCollection<string> accounts;
        private List<XMPPClient> clients;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 19/03/2018 Created [Fabian Sauter]
        /// </history>
        public ChangeAccountPresenceDialog()
        {
            this.accounts = new ObservableCollection<string>();
            this.clients = null;
            this.InitializeComponent();

            loadAccounts();
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
                foreach (XMPPClient c in clients)
                {
                    accounts.Add(c.getXMPPAccount().getIdAndDomain());
                }
            }
        }

        private void savePresence()
        {
            hideErrorMessage();
            if (account_cbx.SelectedIndex < 0)
            {
                showErrorMessage("No account selected!");
                return;
            }
            else if (account_cbx.SelectedIndex >= clients.Count)
            {
                showErrorMessage("Invalid account!");
                return;
            }

            XMPPClient c = clients[account_cbx.SelectedIndex];
            if (!c.isConnected())
            {
                showErrorMessage("Account not connected!");
                return;
            }

            save_btn.IsEnabled = false;

            if (presence_cbx.SelectedIndex < 0)
            {
                showErrorMessage("No presence selected!");
                return;
            }

            if (presence_cbx.SelectedItem is PresenceTemplate)
            {
                PresenceTemplate templateItem = presence_cbx.SelectedItem as PresenceTemplate;
                string status = string.IsNullOrEmpty(status_tbx.Text) ? null : status_tbx.Text;

                // Save presence and status:
                c.getXMPPAccount().presence = templateItem.presence;
                c.getXMPPAccount().status = status;

                AccountDBManager.INSTANCE.setAccount(c.getXMPPAccount(), false);

                // Send the updated presence and status to the server:
                Task t = c.setPreseceAsync(templateItem.presence, status);
            }
            else
            {
                showErrorMessage("Invalid presence!");
            }
            save_btn.IsEnabled = true;
        }

        private void showErrorMessage(string msg)
        {
            error_itbx.Text = msg;
            error_itbx.Visibility = Visibility.Visible;
        }

        private void hideErrorMessage()
        {
            error_itbx.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }

        private void account_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (account_cbx.SelectedIndex >= 0 && account_cbx.SelectedIndex < clients.Count)
            {
                XMPPClient c = clients[account_cbx.SelectedIndex];
                if (!c.isConnected())
                {
                    presence_cbx.IsEnabled = false;
                    save_btn.IsEnabled = false;
                    showErrorMessage("Account not connected!");
                    return;
                }
                presence_cbx.IsEnabled = true;

                Presence accountPresence = c.getXMPPAccount().presence;
                for (int i = 0; i < presence_cbx.Items.Count; i++)
                {
                    if (presence_cbx.Items[i] is PresenceTemplate)
                    {
                        if ((presence_cbx.Items[i] as PresenceTemplate).presence == accountPresence)
                        {
                            presence_cbx.SelectedIndex = i;
                            break;
                        }
                    }
                }
                status_tbx.Text = c.getXMPPAccount().status ?? "";
            }
            else
            {
                presence_cbx.IsEnabled = false;
                save_btn.IsEnabled = false;
            }
        }

        private void addAccount_tblck_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Hide();
            (Window.Current.Content as Frame).Navigate(typeof(AccountSettingsPage));
        }

        private void presence_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            save_btn.IsEnabled = presence_cbx.SelectedIndex >= 0;
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            savePresence();
        }

        private void account_cbx_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (account_cbx.SelectedIndex < 0 && account_cbx.Items.Count > 0)
            {
                account_cbx.SelectedIndex = 0;
            }
        }

        #endregion
    }
}
