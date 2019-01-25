using Data_Manager2.Classes;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
        private static readonly Random RANDOM = new Random();

        public const string DEVICE_FAMILY_DESKTOP = "";
        public const string DEVICE_FAMILY_MOBILE = "";
        public const string DEVICE_FAMILY_XBOX = "";
        public const string DEVICE_FAMILY_IOT = "";
        public const string DEVICE_FAMILY_IOT_HEADLESS = "";
        public const string DEVICE_FAMILY_HOLOLENS = "";
        public const string DEVICE_FAMILY_TEAM = "";

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static List<KeyboardAccelerator> GetGoBackKeyboardAccelerators()
        {
            return new List<KeyboardAccelerator>
            {
                new KeyboardAccelerator
                {
                    Key = VirtualKey.Back
                },
                new KeyboardAccelerator
                {
                    Key = VirtualKey.Left
                },
                new KeyboardAccelerator
                {
                    Key = VirtualKey.GoBack
                }
            };
        }

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

        public static bool NavigateToPage(Type pageType)
        {
            return NavigateToPage(pageType, null);
        }

        public static bool OnGoBackRequested(Frame frame)
        {
            if (frame is null)
            {
                Logger.Error("Failed to execute back request - frame is null!");
                return false;
            }
            else
            {
                if (frame.CanGoBack)
                {
                    frame.GoBack();
                    return true;
                }
                return false;
            }

        }

        public static bool NavigateToPage(Type pageType, object parameter)
        {
            if (pageType is null)
            {
                Logger.Error("Failed to navigate to given page type - type is null!");
                return false;
            }
            if (Window.Current.Content is Frame frame)
            {
                if (frame.Content is null || frame.Content.GetType() != pageType)
                {
                    return frame.Navigate(pageType, parameter);
                }
                else
                {
                    Logger.Warn("No need to navigate to page " + pageType.ToString() + " - already on it.");
                    return true;
                }
            }
            else
            {
                Logger.Error("Failed to navigate to " + pageType.ToString() + " - Window.Current.Content is not of type Frame!");
                return false;
            }
        }

        public static bool IsDarkThemeActive()
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Dark;
        }

        public static bool IsApplicationViewApiAvailable()
        {
            return ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView");
        }

        public static bool IsStatusBarApiAvailable()
        {
            return ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
        }

        /// <summary>
        /// The KeyboardAccelerator class got introduced with v10.0.16299.0.
        /// Source: https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.input.keyboardaccelerator
        /// </summary>
        /// <returns></returns>
        public static bool IsKeyboardAcceleratorApiAvailable()
        {
            return ApiInformation.IsTypePresent("Windows.UI.Xaml.Input.KeyboardAccelerator");
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

        public static CoreVirtualKeyStates GetVirtualKeyState(VirtualKey key)
        {
            return CoreWindow.GetForCurrentThread().GetKeyState(key);
        }

        public static bool IsVirtualKeyDown(VirtualKey key)
        {
            return GetVirtualKeyState(key) != CoreVirtualKeyStates.None;
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

            while (!(contentDialogShowRequest is null))
            {
                await contentDialogShowRequest.Task;
            }

            contentDialogShowRequest = new TaskCompletionSource<ContentDialog>();
            ContentDialogResult result = await dialog.ShowAsync();
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
            return await Launcher.LaunchUriAsync(url);
        }

        public static ElementTheme LoadRequestedTheme()
        {
            string themeString = Settings.getSettingString(SettingsConsts.APP_REQUESTED_THEME);
            ElementTheme theme = ElementTheme.Dark;
            if (themeString != null)
            {
                Enum.TryParse(themeString, out theme);
            }
            return theme;
        }

        public static void SetupWindow(Application application)
        {
            // PC, Mobile:
            if (IsApplicationViewApiAvailable())
            {
                ApplicationView appView = ApplicationView.GetForCurrentView();

                // Dye title:
                Brush windowBrush = (Brush)application.Resources["AppBackgroundAcrylicWindowBrush"];
                if (windowBrush is Microsoft.UI.Xaml.Media.AcrylicBrush acrylicWindowBrush)
                {
                    appView.TitleBar.BackgroundColor = acrylicWindowBrush.TintColor;
                }
                else
                {
                    appView.TitleBar.BackgroundColor = ((SolidColorBrush)windowBrush).Color;
                }

                //Dye title bar buttons:
                appView.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                appView.TitleBar.ButtonInactiveForegroundColor = (Color)Application.Current.Resources["SystemListLowColor"];
                appView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                appView.TitleBar.ButtonForegroundColor = (Color)Application.Current.Resources["SystemBaseHighColor"];

                // Extend window:
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            }

            // Mobile:
            if (IsStatusBarApiAvailable())
            {

                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundColor = ((SolidColorBrush)application.Resources["AppBackgroundAcrylicElementBrush"]).Color;
                    statusBar.BackgroundOpacity = 1.0d;
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
        /// Generates a random bare JID.
        /// e.g. 'chat.shakespeare.lit'
        /// </summary>
        /// <returns>A random bare JID string.</returns>
        public static string GenRandomBareJid()
        {
            StringBuilder sb = new StringBuilder(GenRandomString(RANDOM.Next(4, 10)));
            sb.Append('@');
            sb.Append(GenRandomString(RANDOM.Next(4, 6)));
            sb.Append('.');
            sb.Append(GenRandomString(RANDOM.Next(2, 4)));
            return sb.ToString();
        }

        /// <summary>
        /// Generates a random string and returns it.
        /// Based on: https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
        /// </summary>
        /// <param name="length">The length of the string that should be generated.</param>
        /// <returns>A random string.</returns>
        private static string GenRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[RANDOM.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Overrides the default resources like "ButtonRevealStyle" with more performant versions
        /// to increase the UI performance on low end devices like phones.
        /// </summary>
        public static void OverrideResources()
        {
            // Styles:
            Application.Current.Resources["ButtonRevealStyle"] = Application.Current.Resources["DefaultButtonStyle"];

            // Brushes:
            if (IsDarkThemeActive())
            {
                Application.Current.Resources["AppBackgroundAcrylicWindowBrush"] = new SolidColorBrush(new Color()
                {
                    A = 0xFF,
                    R = 0x24,
                    G = 0x24,
                    B = 0x24,
                });
                Application.Current.Resources["AppBackgroundAcrylicElementBrush"] = new SolidColorBrush(new Color()
                {
                    A = 0xFF,
                    R = 0x2D,
                    G = 0x2D,
                    B = 0x2D,
                });
            }
            else
            {
                Application.Current.Resources["AppBackgroundAcrylicWindowBrush"] = new SolidColorBrush(new Color()
                {
                    A = 0xFF,
                    R = 0xD6,
                    G = 0xD6,
                    B = 0xD6,
                });
                Application.Current.Resources["AppBackgroundAcrylicElementBrush"] = new SolidColorBrush(new Color()
                {
                    A = 0xFF,
                    R = 0xCD,
                    G = 0xCD,
                    B = 0xCD,
                });
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
