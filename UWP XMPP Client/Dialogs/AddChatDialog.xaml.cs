using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            this.cancled = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private bool checkUserInput()
        {
            client = accountSelection_asc.getSelectedAccount();
            if (client == null)
            {
                accountSelection_asc.showErrorMessage("No account selected!");
                return false;
            }
            if (client.getXMPPAccount().getIdAndDomain().Equals(jabberId_tbx.Text))
            {
                accountSelection_asc.showErrorMessage("You can't start a chat with your self!");
                return false;
            }
            if (!Utils.isBareJid(jabberId_tbx.Text))
            {
                accountSelection_asc.showErrorMessage("Invalid JabberID!");
                return false;
            }
            jabberId = jabberId_tbx.Text;
            if (ChatDBManager.INSTANCE.doesChatExist(ChatTable.generateId(jabberId, client.getXMPPAccount().getIdAndDomain())))
            {
                accountSelection_asc.showErrorMessage("Chat does already exist!");
                return false;
            }
            return true;
        }

        private void addChat()
        {
            add_btn.IsEnabled = false;
            add_pgr.Visibility = Visibility.Visible;
            if (checkUserInput())
            {
                addToRoster = (bool)roster_cbx.IsChecked;
                requestSubscription = (bool)subscription_cbx.IsChecked;
                cancled = false;
                Hide();
            }
            add_pgr.Visibility = Visibility.Collapsed;
            add_btn.IsEnabled = true;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            addChat();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
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

        private void jabberId_tbx_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            accountSelection_asc.hideErrorMessage();
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                addChat();
            }
        }

        #endregion
    }
}
