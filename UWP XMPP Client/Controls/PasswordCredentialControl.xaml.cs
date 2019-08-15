using Data_Manager2.Classes;
using System;
using UWP_XMPP_Client.Dialogs;
using UWP_XMPP_Client.Pages.SettingsPages;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class PasswordCredentialControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public PasswordCredential Credential
        {
            get { return (PasswordCredential)GetValue(CredentialProperty); }
            set { SetValue(CredentialProperty, value); }
        }
        public static readonly DependencyProperty CredentialProperty = DependencyProperty.Register("Credential", typeof(PasswordCredential), typeof(PasswordCredentialControl), null);

        public SecuritySettingsPage securitySettingsPage
        {
            get { return (SecuritySettingsPage)GetValue(securitySettingsPageProperty); }
            set { SetValue(securitySettingsPageProperty, value); }
        }
        public static readonly DependencyProperty securitySettingsPageProperty = DependencyProperty.Register("securitySettingsPage", typeof(SecuritySettingsPage), typeof(PasswordCredentialControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/05/2018 Created [Fabian Sauter]
        /// </history>
        public PasswordCredentialControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void deleteEntry_ibtn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            ConfirmDialog dialog = new ConfirmDialog()
            {
                Title = "Delete entry:",
                Text = "Do you really want to delete the password for:\n" + (Credential?.UserName) ?? "-"
            };

            await dialog.ShowAsync();

            if (dialog.confirmed)
            {
                Vault.deletePassword(Credential);
                if (securitySettingsPage != null)
                {
                    securitySettingsPage.loadPasswords();
                }
            }
        }

        #endregion
    }
}
