using Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Services.Store;

namespace UWP_XMPP_Client.Classes
{
    class BuyContentHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string SUPPORT_TIER_1 = "SupportTier1";
        private LicenseInformation licenseInformation;
        public static readonly BuyContentHelper INSTANCE = new BuyContentHelper();

        public const string CONSUMABLE = "Consumable";
        public const string UNMANAGED_CONSUMABLE = "UnmanagedConsumable";
        public const string APPLICATION = "Application";
        public const string GAME = "Game";
        public const string DURABLE = "Durable";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 15/04/2018 Created [Fabian Sauter]
        /// </history>
        public BuyContentHelper()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static async Task<List<StoreProduct>> requestConsumablesAsync()
        {
            List<StoreProduct> products = new List<StoreProduct>();

            string[] productKinds = { UNMANAGED_CONSUMABLE };
            List<String> filterList = new List<string>(productKinds);

            try
            {
                StoreProductQueryResult queryResult = await StoreContext.GetDefault().GetAssociatedStoreProductsAsync(filterList);
                if (queryResult != null)
                {
                    products.AddRange(queryResult.Products.Values);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Error during requesting consumable products.", e);
            }

            return products;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static async Task<ProductPurchaseStatus> requestPuracheAsync(string featureName)
        {
            try
            {
                PurchaseResults results;
#if DEBUG
                results = await CurrentAppSimulator.RequestProductPurchaseAsync(featureName);
#else
                results = await CurrentApp.RequestProductPurchaseAsync(featureName);
#endif
                return results.Status;
            }
            catch (Exception)
            {
                return ProductPurchaseStatus.NotFulfilled;
            }
        }

        public void init()
        {
#if DEBUG
            licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            licenseInformation = CurrentApp.LicenseInformation;
#endif
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
