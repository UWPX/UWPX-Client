using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Pages.SettingsPages
{
    public sealed partial class DonateSettingsPage : Page
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
        /// 15/04/2018 Created [Fabian Sauter]
        /// </history>
        public DonateSettingsPage()
        {
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
        private async Task requestPuracheAsync(string featureName)
        {
            Exception ex = await BuyContentHelper.requestPuracheAsync(featureName);

            TextDialog dialog;
            if (ex != null)
            {
                dialog = new TextDialog("Failed to place order:\n" + ex.Message, "An error occurred!");
            }
            else
            {
                dialog = new TextDialog("Thanks for supporting the development!", "Success!");
            }

            await dialog.ShowAsync();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void buySupportTier1_btn_Click(object sender, RoutedEventArgs e)
        {
            await requestPuracheAsync(BuyContentHelper.SUPPORT_TIER_1);
        }

        private void buySupportTier2_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buySupportTier3_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}
