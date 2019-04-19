using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Logging;
using Microsoft.Toolkit.Uwp.UI.Helpers;
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

        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    return rootElement.RequestedTheme;
                }

                return ElementTheme.Default;
            }
            set
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                    SetupWindow(Application.Current);
                }
                Settings.setSetting(SettingsConsts.APP_REQUESTED_THEME, value.ToString());
            }
        }
        public static ThemeListener themeListener;

        public static bool IsWindowActivated
        {
            get;
            private set;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Returns the resource for the given key from the <see cref="Application.Current.Resources"/> for the current theme specified in <see cref="RootTheme"/>./>
        /// </summary>
        /// <param name="key">The key for the resource you want to retrieve.</param>
        public static T GetThemeResource<T>(string key)
        {
            return GetThemeResource<T>(key, Application.Current.Resources);
        }

        /// <summary>
        /// Returns the resource for the given key from the given <see cref="ResourceDictionary"/> for the current theme specified in <see cref="RootTheme"/>./>
        /// </summary>
        /// <param name="resources">The <see cref="ResourceDictionary"/> you want to get the resource from.</param>
        /// <param name="key">The key for the resource you want to retrieve.</param>
        public static T GetThemeResource<T>(string key, ResourceDictionary resources)
        {
            if (resources.ThemeDictionaries.ContainsKey(RootTheme.ToString()))
            {
                ResourceDictionary themeResources = resources.ThemeDictionaries[RootTheme.ToString()] as ResourceDictionary;
                if (themeResources.ContainsKey(key))
                {
                    return (T)themeResources[key];
                }
            }
            return (T)resources[key];
        }

        /// <summary>
        /// Sets the given resource for all themes.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <param name="value">The value of the resource.</param>
        public static void SetResourceAll(string key, object value)
        {
            SetResourceAll(key, value, Application.Current.Resources);
        }

        /// <summary>
        /// Sets the given resource for all themes.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <param name="value">The value of the resource.</param>
        /// <param name="resources">The <see cref="ResourceDictionary"/> to set the resource in.</param>
        public static void SetResourceAll(string key, object value, ResourceDictionary resources)
        {
            // Remove all theme resources:
            foreach (ElementTheme theme in Enum.GetValues(typeof(ElementTheme)))
            {
                if (resources.ThemeDictionaries.ContainsKey(theme.ToString()))
                {
                    resources.ThemeDictionaries.Remove(key);
                }
            }

            // Set the default key and value:
            resources[key] = value;
        }

        /// <summary>
        /// Sets the given resource for the given theme.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <param name="value">The value of the resource.</param>
        /// <param name="theme">The <see cref="ElementTheme"/> for which the resource should be set.</param>
        public static void SetThemeResource(string key, object value, ElementTheme theme)
        {
            SetThemeResource(key, value, theme, Application.Current.Resources);
        }

        /// <summary>
        /// Sets the given resource for the given theme.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <param name="value">The value of the resource.</param>
        /// <param name="theme">The <see cref="ElementTheme"/> for which the resource should be set.</param>
        /// <param name="resources">The <see cref="ResourceDictionary"/> to set the resource in.</param>
        public static void SetThemeResource(string key, object value, ElementTheme theme, ResourceDictionary resources)
        {
            if (!resources.ThemeDictionaries.ContainsKey(theme.ToString()))
            {
                resources.ThemeDictionaries[theme.ToString()] = new ResourceDictionary();
            }

            ResourceDictionary themeResources = resources.ThemeDictionaries[theme.ToString()] as ResourceDictionary;
            themeResources[key] = value;
        }

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
                    return GetThemeResource<SolidColorBrush>("PresenceOnlineBrush");

                case Presence.Chat:
                    return GetThemeResource<SolidColorBrush>("PresenceChatBrush");

                case Presence.Away:
                    return GetThemeResource<SolidColorBrush>("PresenceAwayBrush");

                case Presence.Xa:
                    return GetThemeResource<SolidColorBrush>("PresenceXaBrush");

                case Presence.Dnd:
                    return GetThemeResource<SolidColorBrush>("PresenceDndBrush");

                default:
                    return GetThemeResource<SolidColorBrush>("PresenceUnavailableBrush");

            }
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

        public static void RemoveLastBackStackEntry()
        {
            if (Window.Current.Content is Frame frame && frame.BackStackDepth > 0)
            {
                frame.BackStack.RemoveAt(frame.BackStack.Count - 1);
            }
        }

        public static bool NavigateToPage(Type pageType)
        {
            return NavigateToPage(pageType, null);
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
                    return false;
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
            ElementTheme theme = ElementTheme.Dark;

            // Set the default theme to dark:
            if (!Settings.getSettingBoolean(SettingsConsts.INITIALLY_STARTED))
            {
                theme = ElementTheme.Dark;
            }
            else
            {
                string themeString = Settings.getSettingString(SettingsConsts.APP_REQUESTED_THEME);
                if (themeString != null)
                {
                    Enum.TryParse(themeString, out theme);
                }
            }
            RootTheme = theme;
            return RootTheme;
        }

        public static void SetupThemeListener()
        {
            if (themeListener is null)
            {
                themeListener = new ThemeListener();
                themeListener.ThemeChanged += ThemeListener_ThemeChanged;
            }
        }

        public static void SetupWindow(Application application)
        {
            // PC, Mobile:
            if (IsApplicationViewApiAvailable())
            {
                ApplicationView appView = ApplicationView.GetForCurrentView();

                // Dye title:
                Brush windowBrush = GetThemeResource<Brush>("AppBackgroundAcrylicWindowBrush", application.Resources);
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
                appView.TitleBar.ButtonInactiveForegroundColor = GetThemeResource<Color>("SystemListLowColor", application.Resources);
                appView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                appView.TitleBar.ButtonForegroundColor = GetThemeResource<Color>("SystemBaseHighColor", application.Resources);

                // Extend window:
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            }

            // Mobile:
            if (IsStatusBarApiAvailable())
            {

                StatusBar statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundColor = GetThemeResource<SolidColorBrush>("AppBackgroundAcrylicElementBrush", application.Resources).Color;
                    statusBar.BackgroundOpacity = 1.0d;
                }
            }
        }

        /// <summary>
        /// Sets up the listener for the current window activated state.
        /// </summary>
        public static void SetupWindowActivatedListener()
        {
            IsWindowActivated = true;
            Window.Current.Activated -= Current_Activated;
            Window.Current.Activated += Current_Activated;
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
            byte r = (byte)Convert.ToUInt32(hexString.Substring(0, 2), 16);
            byte g = (byte)Convert.ToUInt32(hexString.Substring(2, 2), 16);
            byte b = (byte)Convert.ToUInt32(hexString.Substring(4, 2), 16);
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
        /// Returns a random RGB color as hex string in HTML notation.
        /// e.g. #FFFFFF
        /// </summary>
        public static string GenRandomHexColor()
        {
            return string.Format("#{0:X6}", RANDOM.Next(0x1000000));
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
            SetResourceAll("ButtonRevealStyle", GetThemeResource<object>("DefaultButtonStyle"));
            SetResourceAll("AcceptButtonRevealStyle", GetThemeResource<object>("AcceptButtonStyle"));
            SetResourceAll("DeclineButtonRevealStyle", GetThemeResource<object>("DeclineButtonStyle"));
            SetResourceAll("SaveButtonRevealStyle", GetThemeResource<object>("SaveButtonStyle"));

            // Brushes:
            // Dark:
            SetThemeResource("AppBackgroundAcrylicWindowBrush", new SolidColorBrush(new Color()
            {
                A = 0xFF,
                R = 0x24,
                G = 0x24,
                B = 0x24,
            }), ElementTheme.Dark);
            SetThemeResource("AppBackgroundAcrylicElementBrush", new SolidColorBrush(new Color()
            {
                A = 0xFF,
                R = 0x2D,
                G = 0x2D,
                B = 0x2D,
            }), ElementTheme.Dark);

            // Light:
            SetThemeResource("AppBackgroundAcrylicWindowBrush", new SolidColorBrush(new Color()
            {
                A = 0xFF,
                R = 0xD6,
                G = 0xD6,
                B = 0xD6,
            }), ElementTheme.Dark);
            SetThemeResource("AppBackgroundAcrylicElementBrush", new SolidColorBrush(new Color()
            {
                A = 0xFF,
                R = 0xCD,
                G = 0xCD,
                B = 0xCD,
            }), ElementTheme.Light);
        }

        /// <summary>
        /// Hides the StatusBar on Windows Mobile devices.
        /// </summary>
        public static async Task HideStatusBarAsync()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().HideAsync();
            }
        }

        /// <summary>
        /// Shows the StatusBar on Windows Mobile devices.
        /// </summary>
        public static async Task ShowStatusBarAsync()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().ShowAsync();
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void ThemeListener_ThemeChanged(ThemeListener sender)
        {
            SetupWindow(Application.Current);
        }

        private static void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            IsWindowActivated = e.WindowActivationState != CoreWindowActivationState.Deactivated;
        }

        #endregion
    }
}
