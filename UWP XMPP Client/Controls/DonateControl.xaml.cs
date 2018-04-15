using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Dialogs;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Controls
{
    public sealed partial class DonateControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public StoreProduct Product
        {
            get { return (StoreProduct)GetValue(ProductProperty); }
            set { SetValue(ProductProperty, value); }
        }
        public static readonly DependencyProperty ProductProperty = DependencyProperty.Register("Product", typeof(StoreProduct), typeof(DonateControl), null);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/04/2018 Created [Fabian Sauter]
        /// </history>
        public DonateControl()
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
        private void requestPurchase(string featureName)
        {
            Task.Run(async () =>
            {
                Exception ex = await BuyContentHelper.requestPuracheAsync(featureName);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
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
                    donate_prgr.Visibility = Visibility.Collapsed;
                    donate_btn.IsEnabled = true;
                });
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void donate_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Product != null)
            {
                donate_btn.IsEnabled = false;
                donate_prgr.Visibility = Visibility.Visible;
                requestPurchase(Product.StoreId);
            }
        }

        #endregion
    }
}
