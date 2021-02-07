using System;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using Storage.Classes;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWPX_UI_Context.Classes
{
    public static class ThemeUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get => _RootTheme;
            set
            {
                _RootTheme = value;
                Settings.SetSetting(SettingsConsts.APP_REQUESTED_THEME, value.ToString());

                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                    UiUtils.SetupWindow(Application.Current);
                }
            }
        }
        private static ElementTheme _RootTheme;
        public static ThemeListener ThemeListener
        {
            get;
            private set;
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static ApplicationTheme GetActualTheme(ElementTheme theme)
        {
            switch (theme)
            {
                case ElementTheme.Light:
                    return ApplicationTheme.Light;

                case ElementTheme.Dark:
                    return ApplicationTheme.Dark;

                default:
                    return Application.Current.RequestedTheme;
            }
        }

        public static ApplicationTheme GetActualTheme()
        {
            return GetActualTheme(RootTheme);
        }

        public static ElementTheme GetElementTheme(ApplicationTheme theme)
        {
            return theme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
        }

        /// <summary>
        /// Returns the resource for the given key from the <see cref="Application.Current.Resources"/> for the current theme specified in <see cref="ActualTheme"/>./>
        /// </summary>
        /// <param name="key">The key for the resource you want to retrieve.</param>
        public static T GetThemeResource<T>(string key)
        {
            return GetThemeResource<T>(key, Application.Current.Resources);
        }

        /// <summary>
        /// Returns the resource for the given key from the given <see cref="ResourceDictionary"/> for the current theme specified in <see cref="ActualTheme"/>./>
        /// </summary>
        /// <param name="resources">The <see cref="ResourceDictionary"/> you want to get the resource from.</param>
        /// <param name="key">The key for the resource you want to retrieve.</param>
        public static T GetThemeResource<T>(string key, ResourceDictionary resources)
        {
            ApplicationTheme actualTheme = GetActualTheme();
            if (resources.ThemeDictionaries.ContainsKey(actualTheme.ToString()))
            {
                ResourceDictionary themeResources = resources.ThemeDictionaries[actualTheme.ToString()] as ResourceDictionary;
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
            foreach (ApplicationTheme theme in Enum.GetValues(typeof(ApplicationTheme)))
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
            ApplicationTheme actualTheme = GetActualTheme(theme);
            if (!resources.ThemeDictionaries.ContainsKey(actualTheme.ToString()))
            {
                resources.ThemeDictionaries[actualTheme.ToString()] = new ResourceDictionary();
            }

            ResourceDictionary themeResources = resources.ThemeDictionaries[actualTheme.ToString()] as ResourceDictionary;
            themeResources[key] = value;
        }

        /// <summary>
        /// Overrides the default resources like "ButtonRevealStyle" with more performant versions
        /// to increase the UI performance on low end devices like phones.
        /// </summary>
        public static void OverrideThemeResources()
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
            }), ElementTheme.Light);
            SetThemeResource("AppBackgroundAcrylicElementBrush", new SolidColorBrush(new Color()
            {
                A = 0xFF,
                R = 0xCD,
                G = 0xCD,
                B = 0xCD,
            }), ElementTheme.Light);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static ElementTheme LoadRequestedTheme()
        {
            ElementTheme theme = ElementTheme.Default;

            string themeString = Settings.GetSettingString(SettingsConsts.APP_REQUESTED_THEME);
            if (themeString != null)
            {
                Enum.TryParse(themeString, out theme);
            }
            RootTheme = theme;
            return RootTheme;
        }

        public static void SetupThemeListener()
        {
            if (ThemeListener is null)
            {
                ThemeListener = new ThemeListener();
                ThemeListener.ThemeChanged += ThemeListener_ThemeChanged;
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
            if (RootTheme == ElementTheme.Default)
            {
                RootTheme = GetElementTheme(sender.CurrentTheme);
            }
            UiUtils.SetupWindow(Application.Current);
        }

        #endregion
    }
}
