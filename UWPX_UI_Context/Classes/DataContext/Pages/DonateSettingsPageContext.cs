using System;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates.Pages;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public sealed class DonateSettingsPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DonateSettingsPageDataTemplate MODEL = new DonateSettingsPageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public Task DonateViaLiberapayAsync()
        {
            return UiUtils.LaunchUriAsync(new Uri("http://liberapay.uwpx.org"));
        }

        public Task DonateViaPayPalAsync()
        {
            return UiUtils.LaunchUriAsync(new Uri("http://paypal.uwpx.org"));
        }

        public Task SendBankDetailsMailAsync()
        {
            return UiUtils.LaunchUriAsync(new Uri("mailto:support@uwpx.org?subject=[Donation]%20Bank%20details"));
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
