﻿using System;
using Logging;
using Shared.Classes.AppCenter;
using UWPX_UI.Dialogs;
using UWPX_UI.Extensions;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class MiscSettingsPage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly MiscSettingsPageContext VIEW_MODEL = new MiscSettingsPageContext();
        private string requestedSection;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MiscSettingsPage()
        {
            InitializeComponent();
            UiUtils.ApplyBackground(this);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void ScrollToSection(string section)
        {
            switch (section)
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

        private void SelectMenuItem(string section)
        {
            foreach (object item in main_nview.MenuItems)
            {
                if (item is Microsoft.UI.Xaml.Controls.NavigationViewItem navItem && string.Equals((string)navItem.Tag, section))
                {
                    main_nview.SelectedItem = item;
                    break;
                }
            }
        }

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
            await logsFolder_fsc.RecalculateFolderSizeAsync();
        }

        private async void ExportLogs_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await VIEW_MODEL.ExportLogsAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to export logs:", ex);
                AppCenterCrashHelper.INSTANCE.TrackError(ex, "Failed to export logs!\nIf you click on cancel, please consider reporting this crash here:\n\n[GitHub#125](https://github.com/UWPX/UWPX-Client/issues/125)");
            }
        }

        private async void ClearImageCache_btn_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDialog dialog = new ConfirmDialog("Clear image cache:", "Do you really want to **delete** all cached images?");
            await UiUtils.ShowDialogAsync(dialog);
            await VIEW_MODEL.ClearImageCacheAsync(dialog.VIEW_MODEL);
            await imageCacheFolder_fsc.RecalculateFolderSizeAsync();
        }

        private async void OpenImageCahceFolder_btn_Click(object sender, RoutedEventArgs e)
        {
            await VIEW_MODEL.OpenImageCacheFolderAsync();
        }

        private void Credits_btn_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(CreditsPage));
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
            if (args.SelectedItem is Microsoft.UI.Xaml.Controls.NavigationViewItem item && item.Tag is string s)
            {
                ScrollToSection(s);
            }
        }

        private async void ClearCache_btn_Click(object sender, RoutedEventArgs e)
        {
            ClearCacheDialog dialog = new ClearCacheDialog();
            await UiUtils.ShowDialogAsync(dialog);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string s)
            {
                requestedSection = s;
            }
            titleBar.OnPageNavigatedTo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedFrom();
        }

        private void Main_nview_Loaded(object sender, RoutedEventArgs e)
        {
            if (requestedSection is null)
            {
                SelectMenuItem("Logs");
            }
            else
            {
                SelectMenuItem(requestedSection);
            }
        }

        #endregion
    }
}
