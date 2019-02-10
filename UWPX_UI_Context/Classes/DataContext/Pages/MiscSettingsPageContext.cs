using Data_Manager2.Classes.DBManager;
using Logging;
using System;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using Windows.Storage;

namespace UWPX_UI_Context.Classes.DataContext.Pages
{
    public class MiscSettingsPageContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly MiscSettingsPageDataTemplate MODEL = new MiscSettingsPageDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task ShowAnalyticsCrashesMoreInformationAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri("https://github.com/UWPX/UWPX-Client/blob/master/PRIVACY_POLICY.md#crash-reporting"));
        }

        public async Task ShowLicenceAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri("https://github.com/UWPX/UWPX-Client/blob/master/LICENSE"));
        }

        public async Task ShowPrivacyPolicy()
        {
            await UiUtils.LaunchUriAsync(new Uri("https://github.com/UWPX/UWPX-Client/blob/master/PRIVACY_POLICY.md"));
        }

        public async Task ViewOnGithubAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri("http://git.uwpx.org"));
        }

        public async Task ViewCreditsAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri("https://uwpx.org/about/"));
        }

        public async Task ReportBugAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri("https://github.com/UWPX/UWPX-Client/issues"));
        }

        public async Task GiveFeedbackAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri("https://github.com/UWPX/UWPX-Client/issues"));
        }

        public async Task OpenAppDataFolderAsync()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        public async Task OpenImageCacheFolderAsync()
        {
            StorageFolder folder = await ImageDBManager.INSTANCE.getCachedImagesFolderAsync();
            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        public async Task ExportLogsAsync()
        {
            await Logger.exportLogsAsync();
        }

        public async Task DeleteLogsAsync(ConfirmDialogContext viewModel)
        {
            await Logger.deleteLogsAsync();
        }

        public async Task ClearImageCacheAsync(ConfirmDialogContext viewModel)
        {
            await ImageDBManager.INSTANCE.deleteImageCacheAsync();
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
