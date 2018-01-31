using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Logging;
using System;
using System.Threading.Tasks;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Dialogs;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP_XMPP_Client.Pages.SettingsPages
{
    public sealed partial class MiscSettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/09/2017 Created [Fabian Sauter]
        /// </history>
        public MiscSettingsPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += AbstractBackRequestPage_BackRequested;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Shows the size of the "imageCache" folder.
        /// </summary>
        private void showImageChacheSize()
        {
            imageChacheSize_tblck.Text = "calculating...";
            Task.Factory.StartNew(async () => {
                long size = await ImageManager.INSTANCE.getCachedImagesFolderSizeAsync();
                string text = "~ ";
                if (size >= 1024)
                {
                    size /= 1024;
                    text += size + " MB";
                }
                else
                {
                    text += size + " KB";
                }
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => imageChacheSize_tblck.Text = text);
            });
        }

        /// <summary>
        /// Shows the size of the "Logs" folder.
        /// </summary>
        private void showLogSize()
        {
            logSize_tblck.Text = "calculating...";
            Task.Factory.StartNew(async () => {
                long size = await Logger.getLogFolderSizeAsync();
                string text = "~ ";
                if (size >= 1024)
                {
                    size /= 1024;
                    if(size >= 1024)
                    {
                        size /= 1024;
                        text += size + " GB";
                    }
                    else
                    {
                        text += size + " MB";
                    }
                }
                else
                {
                    text += size + " KB";
                }
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => logSize_tblck.Text = text);
            });
        }

        private void loadSettings()
        {
            showLogSize();
            showImageChacheSize();

            showInitialStartDialog_tgls.IsOn = !Settings.getSettingBoolean(SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA);
            showWhatsNewDialog_tgls.IsOn = !Settings.getSettingBoolean(SettingsConsts.HIDE_WHATS_NEW_DIALOG_ALPHA_2);
            disableCrashReporting_tgls.IsOn = Settings.getSettingBoolean(SettingsConsts.DISABLE_CRASH_REPORTING);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void AbstractBackRequestPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private async void clearImagesCache_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("Do you really want to clear the image cache?");
            dialog.Commands.Add(new UICommand { Label = "No", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Yes", Id = 1 });
            IUICommand command = await dialog.ShowAsync();
            if ((int)command.Id == 1)
            {
                await ImageManager.INSTANCE.deleteImageCacheAsync();
            }
            showImageChacheSize();
        }

        private async void openLogFolder_btn_Click(object sender, RoutedEventArgs e)
        {
            await Logger.openLogFolderAsync();
        }

        private async void exportLogs_btn_Click(object sender, RoutedEventArgs e)
        {
            await Logger.exportLogs();
        }

        private async void deleteLogs_btn_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("Do you really want to delete all logs?");
            dialog.Commands.Add(new UICommand { Label = "No", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "Yes", Id = 1 });
            IUICommand command = await dialog.ShowAsync();
            if ((int)command.Id == 1)
            {
                await Logger.deleteLogsAsync();
            }
            showLogSize();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadSettings();
        }

        private void showInitialStartDialog_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.setSetting(SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA, !showInitialStartDialog_tgls.IsOn);
        }

        private void showWhatsNewDialog_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.setSetting(SettingsConsts.HIDE_WHATS_NEW_DIALOG_ALPHA_2, !showWhatsNewDialog_tgls.IsOn);
        }

        private void disableCrashReporting_tgls_Toggled(object sender, RoutedEventArgs e)
        {
            Settings.setSetting(SettingsConsts.DISABLE_CRASH_REPORTING, disableCrashReporting_tgls.IsOn);
        }

        private async void moreInformation_tblck_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await UiUtils.launchBrowserAsync(new Uri("https://github.com/UWPX/UWPX-Client/blob/master/PRIVACY_POLICY.md"));
        }

        private async void contributeGithub_stckp_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await UiUtils.launchBrowserAsync(new Uri("https://github.com/UWPX/UWPX-Client"));
        }

        private async void reportBug_stckp_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await UiUtils.launchBrowserAsync(new Uri("https://github.com/UWPX/UWPX-Client/issues"));
        }

        private async void feedback_stckp_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await UiUtils.launchBrowserAsync(new Uri("https://www.microsoft.com/store/apps/9NW16X9JB5WV"));
        }

        private async void license_stckp_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await UiUtils.launchBrowserAsync(new Uri("https://github.com/UWPX/UWPX-Client/blob/master/LICENSE"));
        }

        private async void privacyPolicy_stckp_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await UiUtils.launchBrowserAsync(new Uri("https://github.com/UWPX/UWPX-Client/blob/master/PRIVACY_POLICY.md"));
        }

        private async void clearCache_btn_Click(object sender, RoutedEventArgs e)
        {
            ClearCacheDialog dialog = new ClearCacheDialog();
            await dialog.ShowAsync();
        }

        #endregion
    }
}
