using Data_Manager.Classes;
using Data_Manager.Classes.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP_XMPP_Client.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XMPP_API.Classes.Network;

namespace UWP_XMPP_Client.Pages.SettingsPages
{
    public sealed partial class AccountSettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/09/2017 Created [Fabian Sauter]
        /// </history>
        public AccountSettingsPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += AbstractBackRequestPage_BackRequested;
            loadAccounts();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void loadAccounts()
        {
            accounts_stckp.Children.Clear();
            foreach (XMPPAccount account in UserManager.INSTANCE.getAccounts())
            {
                accounts_stckp.Children.Add(new AccountControl(this) { Account = account });
            }
            if(accounts_stckp.Children.Count > 0)
            {
                reloadAccounts_btn.Visibility = Visibility.Visible;
            }
            else
            {
                reloadAccounts_btn.Visibility = Visibility.Collapsed;
            }
            reloadAccounts_btn.IsEnabled = true;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void AbstractBackRequestPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
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

        private void addAccount_btn_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(AddAccountPage));
        }

        private void reloadAccounts_btn_Click(object sender, RoutedEventArgs e)
        {
            reloadAccounts_btn.IsEnabled = false;
            ConnectionHandler.INSTANCE.reloadAllAccounts();
            loadAccounts();
        }

        #endregion
    }
}
