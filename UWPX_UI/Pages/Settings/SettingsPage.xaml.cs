﻿using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Shared.Classes;
using UWPX_UI.Controls.Settings;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class SettingsPage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SettingsPageContext VIEW_MODEL = new SettingsPageContext();
        private readonly ObservableCollection<SettingsPageButtonDataTemplate> SETTINGS_PAGES = new ObservableCollection<SettingsPageButtonDataTemplate>
        {
            new SettingsPageButtonDataTemplate {Glyph = "\xE13D", Name = "Accounts", Description = "Manage Accounts, Push", NavTarget = typeof(AccountsSettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\xE771", Name = "Personalize", Description = "Background, Theme", NavTarget = typeof(PersonalizeSettingsPage)},
            // new SettingsPageButtonDataTemplate {Glyph = "\xE12B", Name = "Data", Description = "Mobile Data, Wifi", NavTarget = typeof(DataSettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\xE15F", Name = "Chat", Description = "Availability, Media, OMEMO", NavTarget = typeof(ChatSettingsPage)},
            // new SettingsPageButtonDataTemplate {Glyph = "\xE71D", Name = "Background Tasks", Description = "Manage Tasks", NavTarget = typeof(BackgroundTaskSettingsPage)},
            // new SettingsPageButtonDataTemplate {Glyph = "\uE72E", Name = "Security", Description = "Passwords", NavTarget = typeof(SecuritySettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\uEB52", Name = "Donate", Description = "Keep The Project Running", NavTarget = typeof(DonateSettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\xE713", Name = "Misc", Description = "Everything Else", NavTarget = typeof(MiscSettingsPage)},
        };

        private readonly SettingsPageButtonDataTemplate DEBUG_SETTINGS = new SettingsPageButtonDataTemplate { Glyph = "\uEBE8", Name = "Debug", Description = "Debug Test Features", NavTarget = typeof(DebugSettingsPage) };

        private FrameworkElement LastPopUpElement = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SettingsPage()
        {
            InitializeComponent();
            UiUtils.ApplyBackground(this);
            LoadAppVersion();
            VIEW_MODEL.MODEL.PropertyChanged += MODEL_PropertyChanged;

            if (VIEW_MODEL.MODEL.DebugSettingsEnabled)
            {
                SETTINGS_PAGES.Add(DEBUG_SETTINGS);
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadAppVersion()
        {
            name_run.Text = Package.Current.DisplayName;
            PackageVersion version = Package.Current.Id.Version;
            StringBuilder sb = new StringBuilder("v.");
            sb.Append(version.Major);
            sb.Append('.');
            sb.Append(version.Minor);
            sb.Append('.');
            sb.Append(version.Build);
            sb.Append('.');
            sb.Append(version.Revision);
            version_run.Text = sb.ToString();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void SettingsSelectionControl_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!(DeviceFamilyHelper.IsMouseInteractionMode() && sender is SettingsSelectionLargeControl settingsSelection))
            {
                return;
            }

            LastPopUpElement = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(settingsSelection) as FrameworkElement) as FrameworkElement;
            Canvas.SetZIndex(LastPopUpElement, 10);
            AnimationBuilder.Create().Scale(to: new Vector2(1.05f), easingType: EasingType.Sine).Start(LastPopUpElement);
        }

        private void SettingsSelectionControl_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (LastPopUpElement is null)
            {
                return;
            }

            Canvas.SetZIndex(LastPopUpElement, 0);
            AnimationBuilder.Create().Scale(to: new Vector2(1.0f), easingType: EasingType.Sine).Start(LastPopUpElement);
            LastPopUpElement = null;
        }

        private void Version_tbx_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            VIEW_MODEL.OnVersionTextTapped();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedTo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedFrom();
        }

        private void MODEL_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is SettingsPageDataTemplate settingsPageDataTemplate)
            {
                switch (e.PropertyName)
                {
                    case nameof(settingsPageDataTemplate.DebugSettingsEnabled):
                        if (settingsPageDataTemplate.DebugSettingsEnabled)
                        {
                            debugSettings_notification.Show("Debug settings enabled.", 5000);
                            if (!SETTINGS_PAGES.Contains(DEBUG_SETTINGS))
                            {
                                SETTINGS_PAGES.Add(DEBUG_SETTINGS);
                            }
                        }
                        else
                        {
                            debugSettings_notification.Show("Debug settings disabled.", 5000);
                            SETTINGS_PAGES.Remove(DEBUG_SETTINGS);
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
