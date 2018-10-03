using System;
using System.Collections.ObjectModel;
using Data_Manager2.Classes;
using UWP_XMPP_Client.DataTemplates;
using UWP_XMPP_Client.Dialogs;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Pages.SettingsPages
{
    public sealed partial class SecuritySettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ObservableCollection<PasswordCredentialTemplate> PASSWORDS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 05/22/2018 Created [Fabian Sauter]
        /// </history>
        public SecuritySettingsPage()
        {
            this.PASSWORDS = new ObservableCollection<PasswordCredentialTemplate>();
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += AbstractBackRequestPage_BackRequested;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        public void loadPasswords()
        {
            PASSWORDS.Clear();
            foreach (PasswordCredential c in Vault.getAll())
            {
                PASSWORDS.Add(new PasswordCredentialTemplate
                {
                    Credential = c,
                    securitySettingsPage = this
                });
            }
        }

        private void clearPwVault()
        {
            Vault.deleteAllVaults();
            loadPasswords();
        }

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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadPasswords();
        }

        private async void clearPwVault_btn_Click(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            ConfirmDialog dialog = new ConfirmDialog()
            {
                Title = "Clear password vault:",
                Text = "Do you really want to clear the password vault.\nAll passwords will be lost!"
            };

            await dialog.ShowAsync();

            if (dialog.confirmed)
            {
                clearPwVault();
            }
        }

        private void passwords_grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (passwords_itmsc.Visibility == Visibility.Visible)
            {
                passwords_itmsc.Visibility = Visibility.Collapsed;
                passwordsStatus_tblk.Text = "Show passwords";
                passwordsStatusArrow_tblk.Text = "\uE0AB";
            }
            else
            {
                passwords_itmsc.Visibility = Visibility.Visible;
                passwordsStatus_tblk.Text = "Hide passwords";
                passwordsStatusArrow_tblk.Text = "\uE1FD";
            }
        }

        #endregion
    }
}
