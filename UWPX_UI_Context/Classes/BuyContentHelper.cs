using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Windows.ApplicationModel.Store;
using Windows.Services.Store;

namespace UWPX_UI_Context.Classes
{
    public static class BuyContentHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string SUPPORT_TIER_1 = "SupportTier1";
#if DEBUG
        private static LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
        private static LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
#endif

        public const string CONSUMABLE = "Consumable";
        public const string UNMANAGED_CONSUMABLE = "UnmanagedConsumable";
        public const string APPLICATION = "Application";
        public const string GAME = "Game";
        public const string DURABLE = "Durable";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static async Task<List<StoreProduct>> RequestConsumablesAsync()
        {
            List<StoreProduct> products = new List<StoreProduct>();

            string[] productKinds = { UNMANAGED_CONSUMABLE };
            List<string> filterList = new List<string>(productKinds);

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

        public static async Task<ProductPurchaseStatus> RequestPuracheAsync(string featureName)
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
