using System;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
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
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("PrivacyPolicyCrashReportingUrl")));
        }

        public async Task ShowLicenceAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("LicenceUrl")));
        }

        public async Task ShowPrivacyPolicy()
        {
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("PrivacyPolicyUrl")));
        }

        public async Task ViewOnGithubAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("GitHubUrl")));
        }

        public async Task ViewCreditsAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("CreditsUrl")));
        }

        public async Task ReportBugAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("BugReportUrl")));
        }

        public async Task GiveFeedbackAsync()
        {
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("FeedbackUrl")));
        }

        public async Task OpenAppDataFolderAsync()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        public async Task OpenImageCacheFolderAsync()
        {
            await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.OpenCacheFolderAsync();
        }

        public async Task ExportLogsAsync()
        {
            await Logger.ExportLogsAsync();
        }

        public async Task DeleteLogsAsync(ConfirmDialogContext viewModel)
        {
            if (viewModel.MODEL.Confirmed)
            {
                await Logger.DeleteLogsAsync();
            }
        }

        public async Task ClearImageCacheAsync(ConfirmDialogContext viewModel)
        {
            if (viewModel.MODEL.Confirmed)
            {
                await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.ClearCacheAsync();
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
