using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
        private void loadInAppPuraches()
        {
            inAppPuraches_lstv.Visibility = Visibility.Collapsed;
            loadingInAppPuraches_stckp.Visibility = Visibility.Visible;
            noInAppPurachesAvailable_tbx.Visibility = Visibility.Collapsed;

            Task.Run(async () =>
            {
                List<StoreProduct> products = await BuyContentHelper.requestConsumablesAsync();

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    PRODUCTS.Clear();

                    foreach (StoreProduct p in products)
                    {
                        PRODUCTS.Add(p);
                    }

                    loadingInAppPuraches_stckp.Visibility = Visibility.Collapsed;
                    if (PRODUCTS.Count <= 0)
                    {
                        inAppPuraches_lstv.Visibility = Visibility.Collapsed;
                        noInAppPurachesAvailable_tbx.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        inAppPuraches_lstv.Visibility = Visibility.Visible;
                        noInAppPurachesAvailable_tbx.Visibility = Visibility.Collapsed;
                    }
                });
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadInAppPuraches();
        }

        private async void donateLP_btn_Click(object sender, RoutedEventArgs e)
        {
            await UiUtils.launchBrowserAsync(new Uri("https://liberapay.com/~46531/donate"));
        }

        #endregion
    }
}
