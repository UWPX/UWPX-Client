using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UWP_XMPP_Client.DataTemplates;
using UWP_XMPP_Client.Dialogs;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;
using Windows.UI.ViewManagement;

namespace UWP_XMPP_Client.Classes
{
    static class UiUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static TaskCompletionSource<ContentDialog> contentDialogShowRequest;
        private static readonly Regex HEX_COLOR_REGEX = new Regex("#[0-9a-fA-F]{6}");
        private static readonly Random HEX_COLOR_RANDOM = new Random();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static bool isDarkThemeActive()
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Dark;
        }

        public static async Task<ContentDialogResult> showDialogAsyncQueue(ContentDialog dialog)
        {
            // Make sure it gets invoked by the UI thread:
            if (!Window.Current.Dispatcher.HasThreadAccess)
            {
                throw new InvalidOperationException("This method can only be invoked from UI thread.");
            }

            while (contentDialogShowRequest != null)
            {
                await contentDialogShowRequest.Task;
            }

            contentDialogShowRequest = new TaskCompletionSource<ContentDialog>();
            var result = await dialog.ShowAsync();
            contentDialogShowRequest.SetResult(dialog);
            contentDialogShowRequest = null;

            return result;
        }

        public static void setBackgroundImage(ImageEx imgControl)
        {
            BackgroundImageTemplate img = BackgroundImageCache.selectedImage;
            if (img == null || img.imagePath == null)
            {
                imgControl.Source = null;
                imgControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                imgControl.Source = new BitmapImage(new Uri(img.imagePath));
                imgControl.Visibility = Visibility.Visible;
            }
        }

        public static string getRandomColor()
        {
            return string.Format("#{0:X6}", HEX_COLOR_RANDOM.Next(0x1000000));
        }

        public static SolidColorBrush getPresenceBrush(Presence presence)
        {
            switch (presence)
            {
                case Presence.Online:
                    return (SolidColorBrush)Application.Current.Resources["PresenceOnline"];

                case Presence.Chat:
                    return (SolidColorBrush)Application.Current.Resources["PresenceChat"];

                case Presence.Away:
                    return (SolidColorBrush)Application.Current.Resources["PresenceAway"];

                case Presence.Xa:
                    return (SolidColorBrush)Application.Current.Resources["PresenceXa"];

                case Presence.Dnd:
                    return (SolidColorBrush)Application.Current.Resources["PresenceDnd"];

                default:
                    return (SolidColorBrush)Application.Current.Resources["PresenceUnavailable"];

            }
        }

        public static bool isHexColor(string color)
        {
            return color != null && HEX_COLOR_REGEX.Match(color).Success;
        }

        /// <summary>
        /// Checks whether the current device is a Windows Mobile device.
        /// </summary>
        public static bool isRunningOnMobileDevice()
        {
            return AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile");
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Hides the StatusBar on Windows Mobile devices asynchronously.
        /// </summary>
        public static async Task hideStatusBarAsync()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().HideAsync();
            }
        }

        /// <summary>
        /// Shows the StatusBar on Windows Mobile devices asynchronously.
        /// </summary>
        public static async Task showStatusBarAsync()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().ShowAsync();
            }
        }

        public static SolidColorBrush convertHexColorToBrush(string color)
        {
            color = color.Replace("#", string.Empty);
            byte r = Convert.ToByte(color.Substring(0, 2), 16);
            byte g = Convert.ToByte(color.Substring(2, 2), 16);
            byte b = Convert.ToByte(color.Substring(4, 2), 16);
            return new SolidColorBrush(Color.FromArgb(255, r, g, b));
        }

        /// <summary>
        /// Launches the default application for the given Uri.
        /// </summary>
        /// <param name="url">The Uri that defines the application that should get launched.</param>
        /// <returns>Returns true on success.</returns>
        public static async Task<bool> launchUriAsync(Uri url)
        {
            return await Windows.System.Launcher.LaunchUriAsync(url);
        }

        public static async Task showInitialStartDialogAsync()
        {
            if (!Settings.getSettingBoolean(SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA))
            {
                InitialStartDialog dialog = new InitialStartDialog();
                await showDialogAsyncQueue(dialog);
                Settings.setSetting(SettingsConsts.HIDE_INITIAL_START_DIALOG_ALPHA, !dialog.showOnStartup);
            }
        }

        public static async Task showWhatsNewDialog()
        {
            if (!Settings.getSettingBoolean(SettingsConsts.HIDE_WHATS_NEW_DIALOG))
            {
                WhatsNewDialog dialog = new WhatsNewDialog();
                await showDialogAsyncQueue(dialog);
                Settings.setSetting(SettingsConsts.HIDE_WHATS_NEW_DIALOG, !dialog.showOnStartup);
            }
        }

        public static void addTextToClipboard(string text)
        {
            DataPackage package = new DataPackage();
            package.SetText(text);
            Clipboard.SetContent(package);
        }

        /// <summary>
        /// Manages the Windows Mobile StatusBar asynchronously.
        /// </summary>
        public static async Task onPageNavigatedFromAsync()
        {
            if (isRunningOnMobileDevice())
            {
                await showStatusBarAsync();
            }
        }

        /// <summary>
        /// Manages the Windows Mobile StatusBar asynchronously.
        /// </summary>
        public static async Task onPageSizeChangedAsync(SizeChangedEventArgs e)
        {
            if (isRunningOnMobileDevice())
            {
                if (e.NewSize.Height < e.NewSize.Width)
                {
                    await hideStatusBarAsync();
                }
                else
                {
                    await showStatusBarAsync();
                }
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
