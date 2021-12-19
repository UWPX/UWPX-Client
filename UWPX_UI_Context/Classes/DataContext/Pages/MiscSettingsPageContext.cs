using System;
using System.Threading.Tasks;
using Logging;
using Manager.Classes;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using Windows.Foundation;
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
        public IAsyncOperation<bool> ShowAnalyticsCrashesMoreInformationAsync()
        {
            return UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("PrivacyPolicyCrashReportingUrl")));
        }

        public IAsyncOperation<bool> ShowLicenceAsync()
        {
            return UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("LicenceUrl")));
        }

        public IAsyncOperation<bool> ShowPrivacyPolicy()
        {
            return UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("PrivacyPolicyUrl")));
        }

        public IAsyncOperation<bool> ViewOnGithubAsync()
        {
            return UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("GitHubUrl")));
        }

        public IAsyncOperation<bool> ViewCreditsAsync()
        {
            return UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("CreditsUrl")));
        }

        public IAsyncOperation<bool> ReportBugAsync()
        {
            return UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("BugReportUrl")));
        }

        public IAsyncOperation<bool> GiveFeedbackAsync()
        {
            return UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("FeedbackUrl")));
        }

        public IAsyncOperation<bool> OpenAppDataFolderAsync()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            return Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        public Task OpenImageCacheFolderAsync()
        {
            return ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.OpenCacheFolderAsync();
        }

        public Task ExportLogsAsync()
        {
            return Logger.ExportLogsAsync();
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
