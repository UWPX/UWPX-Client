using Data_Manager2.Classes.DBManager;
using Logging;
using System;
using System.Threading.Tasks;
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
            showLogSize();
            showImageChacheSize();
        }
        #endregion
    }
}
