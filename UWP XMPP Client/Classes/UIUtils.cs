using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UWP_XMPP_Client.DataTemplates;
using UWP_XMPP_Client.Dialogs;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using XMPP_API.Classes;

namespace UWP_XMPP_Client.Classes
{
    static class UiUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static TaskCompletionSource<ContentDialog> contentDialogShowRequest;

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

        public static string getRandomMaterialColor()
        {
            Random r = new Random();
            switch (r.Next(17))
            {
                case 0:
                    return "#4CAF50";
                case 1:
                    return "#8BC34A";
                case 2:
                    return "#CDDC39";
                case 3:
                    return "#F44336";
                case 4:
                    return "#E91E63";
                case 5:
                    return "#9C27B0";
                case 6:
                    return "#673AB7";
                case 7:
                    return "#3F51B5";
                case 8:
                    return "#2196F3";
                case 9:
                    return "#03A9F4";
                case 10:
                    return "#00BCD4";
                case 11:
                    return "#009688";
                case 12:
                    return "#FFEB3B";
                case 13:
                    return "#FFC107";
                case 14:
                    return "#FF9800";
                case 15:
                    return "#FF5722";
                default:
                    return "#607D8B";
            }
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

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static bool isHexColor(string color)
        {
            Regex reg = new Regex("#[0-9a-fA-F]{6}");
            return color != null && reg.Match(color).Success;
        }

        public static SolidColorBrush convertHexColorToBrush(string color)
        {
            color = color.Replace("#", string.Empty);
            var r = (byte)Convert.ToUInt32(color.Substring(0, 2), 16);
            var g = (byte)Convert.ToUInt32(color.Substring(2, 2), 16);
            var b = (byte)Convert.ToUInt32(color.Substring(4, 2), 16);
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
