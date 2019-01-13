using UWPX_UI.Dialogs;
using UWPX_UI.Extensions;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class MiscSettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly MiscSettingsPageContext VIEW_MODEL = new MiscSettingsPageContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MiscSettingsPage()
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


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void MoreInformation_hlb_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await VIEW_MODEL.ShowAnalyticsCrashesMoreInformationAsync();
        }

        private async void OpenAppDataFolder_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.OpenAppDataFolderAsync();
        }

        private async void DeleteLogs_btn_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDialog dialog = new ConfirmDialog("Delete logs:", "Do you really want to **delete** all logs?");
            await UiUtils.ShowDialogAsync(dialog);
            await VIEW_MODEL.DeleteLogsAsync(dialog.VIEW_MODEL);
        }

        private async void ExportLogs_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ExportLogsAsync();
        }

        private async void ClearImageCache_btn_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDialog dialog = new ConfirmDialog("Clear image cache:", "Do you really want to **delete** all cached images?");
            await UiUtils.ShowDialogAsync(dialog);
            await VIEW_MODEL.ClearImageCacheAsync(dialog.VIEW_MODEL);
        }

        private async void OpenImageCahceFolder_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.OpenImageCacheFolderAsync();
        }

        private async void Credits_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ViewCreditsAsync();
        }

        private async void PrivacyPolicy_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ShowPrivacyPolicy();
        }

        private async void License_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ShowLicenceAsync();
        }

        private async void Feedback_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.GiveFeedbackAsync();
        }

        private async void ReportBug_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ReportBugAsync();
        }

        private async void ViewOnGitHub_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.ViewOnGithubAsync();
        }

        private void Main_nview_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is Microsoft.UI.Xaml.Controls.NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "Logs":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, logs_scp, false);
                        break;

                    case "Cache":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, cache_scp, false);
                        break;

                    case "Analytics":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, analytics_scp, false);
                        break;

                    case "Misc":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, misc_scp, false);
                        break;

                    case "About":
                        ScrollViewerExtensions.ScrollIntoViewVertically(main_scv, about_scp, false);
                        break;
                }
            }
        }

        private void Main_nview_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (object item in main_nview.MenuItems)
            {
                if (item is Microsoft.UI.Xaml.Controls.NavigationViewItem navItem && string.Equals((string)navItem.Tag, "Logs"))
                {
                    main_nview.SelectedItem = item;
                    break;
                }
            }
        }

        private async void ClearCache_btn_Click(object sender, RoutedEventArgs e)
        {
            ClearCacheDialog dialog = new ClearCacheDialog();
            await UiUtils.ShowDialogAsync(dialog);
        }

        #endregion
    }
}
