using System;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.Classes;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Pages.SettingsPages
{
    public sealed partial class DonateSettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ObservableCollection<StoreProduct> PRODUCTS;

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
            this.PRODUCTS = new ObservableCollection<StoreProduct>();
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void donateLP_btn_Click(object sender, RoutedEventArgs e)
        {
            await UiUtils.launchUriAsync(new Uri("http://liberapay.uwpx.org"));
        }

        private async void donatePP_btn_Click(object sender, RoutedEventArgs e)
        {
            await UiUtils.launchUriAsync(new Uri("http://paypal.uwpx.org"));
        }

        private async void sendMail_link_Click(object sender, RoutedEventArgs e)
        {
            await UiUtils.launchUriAsync(new Uri("mailto:support@uwpx.org?subject=[Donation] Bank detail"));
        }

        #endregion
    }
}
