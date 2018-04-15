using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace UWP_XMPP_Client.Classes
{
    class BuyContentHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string SUPPORT_TIER_1 = "SupportTier1";

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


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static async Task<Exception> requestPuracheAsync(string featureName)
        {
            try
            {
#if DEBUG
                await CurrentAppSimulator.RequestProductPurchaseAsync(featureName);
#else
                await CurrentApp.RequestProductPurchaseAsync(featureName);
#endif
                return null;
            }
            catch (Exception e)
            {
                return e;
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
