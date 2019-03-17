using Microsoft.Toolkit.Uwp.UI.Animations;
using System.Collections.ObjectModel;
using System.Text;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Pages;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Pages.Settings
{
    public sealed partial class SettingsPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SettingsPageContext VIEW_MODEL = new SettingsPageContext();
        private ObservableCollection<SettingsPageButtonDataTemplate> SETTINGS_PAGES = new ObservableCollection<SettingsPageButtonDataTemplate>()
        {
            new SettingsPageButtonDataTemplate {Glyph = "\xE13D", Name = "Accounts", Description = "Manage Accounts", NavTarget = typeof(AccountsSettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\xE771", Name = "Personalize", Description = "Background, Theme", NavTarget = typeof(PersonalizeSettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\xE12B", Name = "Data", Description = "Mobile Data, Wifi", NavTarget = typeof(DataSettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\xE15F", Name = "Chat", Description = "Availability, Media, OMEMO", NavTarget = typeof(ChatSettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\xE71D", Name = "Background Tasks", Description = "Manage Tasks", NavTarget = typeof(BackgroundTaskSettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\uE72E", Name = "Security", Description = "Passwords", NavTarget = typeof(SettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\uEB52", Name = "Donate", Description = "Keep The Project Running", NavTarget = typeof(DonateSettingsPage)},
            new SettingsPageButtonDataTemplate {Glyph = "\xE713", Name = "Misc", Description = "Everything Else", NavTarget = typeof(MiscSettingsPage)},
        };

        private FrameworkElement LastPopUpElement = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SettingsPage()
        {
            this.InitializeComponent();
            LoadAppVersion();
            VIEW_MODEL.MODEL.PropertyChanged += MODEL_PropertyChanged;
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
                        }
                        else
                        {
                            debugSettings_notification.Show("Debug settings disabled.", 5000);
                        }
                        break;
                }
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
        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SettingsPageButtonDataTemplate page)
            {
                UiUtils.NavigateToPage(page.NavTarget);
            }
        }

        private void SettingsSelectionControl_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (DeviceFamilyHelper.IsMouseInteractionMode() && sender is FrameworkElement settingsSelection)
            {
                LastPopUpElement = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(settingsSelection) as FrameworkElement) as FrameworkElement;
                Canvas.SetZIndex(LastPopUpElement, 10);
                LastPopUpElement.Scale(scaleX: 1.05f, scaleY: 1.05f, centerX: (float)LastPopUpElement.Width / 2, centerY: (float)LastPopUpElement.Height / 2, easingType: EasingType.Sine).Start();
            }
        }

        private void SettingsSelectionControl_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!(LastPopUpElement is null))
            {
                Canvas.SetZIndex(LastPopUpElement, 0);
                LastPopUpElement.Scale(centerX: (float)LastPopUpElement.Width / 2, centerY: (float)LastPopUpElement.Height / 2, easingType: EasingType.Sine).Start();
                LastPopUpElement = null;
            }
        }

        private void Version_tbx_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            VIEW_MODEL.OnVersionTextTapped();
        }

        #endregion
    }
}
