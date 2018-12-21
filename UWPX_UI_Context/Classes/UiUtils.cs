using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes;

namespace UWPX_UI_Context.Classes
{
    public static class UiUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static TaskCompletionSource<ContentDialog> contentDialogShowRequest;
        private static readonly Regex HEX_COLOR_REGEX = new Regex("^#[0-9a-fA-F]{6}$");
        public static readonly char[] TRIM_CHARS = { ' ', '\t', '\n', '\r' };

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static SolidColorBrush GetPresenceBrush(Presence presence)
        {
            switch (presence)
            {
                case Presence.Online:
                    return (SolidColorBrush)Application.Current.Resources["PresenceOnlineBrush"];

                case Presence.Chat:
                    return (SolidColorBrush)Application.Current.Resources["PresenceChatBrush"];

                case Presence.Away:
                    return (SolidColorBrush)Application.Current.Resources["PresenceAwayBrush"];

                case Presence.Xa:
                    return (SolidColorBrush)Application.Current.Resources["PresenceXaBrush"];

                case Presence.Dnd:
                    return (SolidColorBrush)Application.Current.Resources["PresenceDndBrush"];

                default:
                    return (SolidColorBrush)Application.Current.Resources["PresenceUnavailableBrush"];

            }
        }

        public static bool IsDarkThemeActive()
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Dark;
        }

        /// <summary>
        /// Checks whether the current device is a Windows Mobile device.
        /// </summary>
        public static bool IsRunningOnMobileDevice()
        {
            return AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Mobile");
        }

        /// <summary>
        /// Checks whether the current device is a Windows 10 Desktop device.
        /// </summary>
        public static bool IsRunningOnDesktopDevice()
        {
            return AnalyticsInfo.VersionInfo.DeviceFamily.Equals("Windows.Desktop");
        }

        public static bool IsApplicationViewApiAvailable()
        {
            return ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView");
        }

        public static bool IsStatusBarApiAvailable()
        {
            return ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
        }

        public static bool IsHexColor(string color)
        {
            return color != null && HEX_COLOR_REGEX.Match(color).Success;
        }

        public static void SetClipboardText(string text)
        {
            DataPackage package = new DataPackage();
            package.SetText(text ?? "");
            Clipboard.SetContent(package);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Shows the given content dialog on the screen, if no other dialog is shown right now.
        /// If there is an other one being shown, the dialog will be shown afterwards.
        /// </summary>
        /// <param name="dialog">The dialog that should get shown.</param>
        public static async Task<ContentDialogResult> ShowDialogAsync(ContentDialog dialog)
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

        /// <summary>
        /// Launches the default application for the given Uri.
        /// </summary>
        /// <param name="url">The Uri that defines the application that should get launched.</param>
        /// <returns>Returns true on success.</returns>
        public static async Task<bool> LaunchUriAsync(Uri url)
        {
            return await Windows.System.Launcher.LaunchUriAsync(url);
        }

        public static void SetupWindow(Application application)
        {
            // PC:
            if (IsApplicationViewApiAvailable())
            {
                ApplicationView appView = ApplicationView.GetForCurrentView();

                // Dye title:
                appView.TitleBar.BackgroundColor = ((Microsoft.UI.Xaml.Media.AcrylicBrush)application.Resources["SystemControlAcrylicWindowBrush"]).TintColor;

                //Dye title bar buttons:
                bool isDarkTheme = IsDarkThemeActive();
                appView.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                appView.TitleBar.ButtonInactiveForegroundColor = (isDarkTheme) ? Colors.White : Colors.Black;
                appView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                appView.TitleBar.ButtonForegroundColor = (isDarkTheme) ? Colors.White : Colors.Black;

                // Extend window:
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            }

            // Mobile:
            if (IsStatusBarApiAvailable())
            {

                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundColor = ((Microsoft.UI.Xaml.Media.AcrylicBrush)application.Resources["SystemControlAcrylicWindowBrush"]).TintColor;
                    statusBar.BackgroundOpacity = 1;
                }
            }
        }

        /// <summary>
        /// Converts the given Color to a hex string e.g. #012345.
        /// </summary>
        /// <param name="c">The Color that should get converted.</param>
        /// <returns>A hex string representing the given Color object.</returns>
        public static string ColorToHexString(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        /// <summary>
        /// Converts the given hex string into a Color object.
        /// </summary>
        /// <param name="hexString">The hex color string e.g. #012345.</param>
        /// <returns>Returns the Color object representing the given hex color.</returns>
        public static Color HexStringToColor(string hexString)
        {
            hexString = hexString.Replace("#", string.Empty);
            byte r = (byte)(Convert.ToUInt32(hexString.Substring(0, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hexString.Substring(2, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hexString.Substring(4, 2), 16));
            return Color.FromArgb(255, r, g, b);
        }

        /// <summary>
        /// Converts the given hex string into a SolidColorBrush object.
        /// </summary>
        /// <param name="hexString">The hex color string e.g. #012345.</param>
        /// <returns>Returns the SolidColorBrush object representing the given hex color.</returns>
        public static SolidColorBrush HexStringToBrush(string hexString)
        {
            return new SolidColorBrush(HexStringToColor(hexString));
        }

        /// <summary>
        /// Calls the UI thread dispatcher and executes the given callback on it.
        /// </summary>
        /// <param name="callback">The callback that should be executed in the UI thread.</param>
        public static async Task CallDispatcherAsync(DispatchedHandler callback)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, callback);
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
