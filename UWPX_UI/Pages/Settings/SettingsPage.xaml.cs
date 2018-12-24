using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;
using System.Text;
using UWPX_UI_Context.Classes.DataTemplates;
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
        private ObservableCollection<SettingsPageDataTemplate> SETTINGS_PAGES = new ObservableCollection<SettingsPageDataTemplate>()
        {
            new SettingsPageDataTemplate {Glyph = "\xE13D", Name = "Accounts", Description = "Manage Accounts", Page = typeof(SettingsPage)},
            new SettingsPageDataTemplate {Glyph = "\xE771", Name = "Personalize", Description = "Background, Color", Page = typeof(SettingsPage)},
            new SettingsPageDataTemplate {Glyph = "\xE12B", Name = "Data", Description = "Mobile Data, Wifi", Page = typeof(SettingsPage)},
            new SettingsPageDataTemplate {Glyph = "\xE15F", Name = "Chat", Description = "Availability", Page = typeof(SettingsPage)},
            new SettingsPageDataTemplate {Glyph = "\xE71D", Name = "Background Tasks", Description = "Manage Tasks", Page = typeof(SettingsPage)},
            new SettingsPageDataTemplate {Glyph = "\uE72E", Name = "Security", Description = "Certificates, Password Vault", Page = typeof(SettingsPage)},
            new SettingsPageDataTemplate {Glyph = "\uEB52", Name = "Donate", Description = "PayPal, Liberapay", Page = typeof(SettingsPage)},
            new SettingsPageDataTemplate {Glyph = "\xE713", Name = "Misc", Description = "Everything Else", Page = typeof(SettingsPage)},
        };

        private FrameworkElement LastPopUpElement = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SettingsPage()
        {
            this.InitializeComponent();
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

        private void NavigateToPage(Type pageType)
        {
            (Window.Current.Content as Frame).Navigate(pageType);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is SettingsPageDataTemplate page)
            {
                NavigateToPage(page.Page);
            }
        }

        private void AbstractBackRequestPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame is null)
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void DropShadowPanel_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is DropShadowPanel dropShadow)
            {
                LastPopUpElement = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(dropShadow) as FrameworkElement) as FrameworkElement;
                Canvas.SetZIndex(LastPopUpElement, 10);
                LastPopUpElement.Scale(scaleX: 1.05f, scaleY: 1.05f, centerX: (float)LastPopUpElement.Width / 2, centerY: (float)LastPopUpElement.Height / 2, easingType: EasingType.Sine).Start();
            }
        }

        private void DropShadowPanel_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!(LastPopUpElement is null))
            {
                DropShadowPanel DropShadow = sender as DropShadowPanel;
                Canvas.SetZIndex(LastPopUpElement, 0);
                LastPopUpElement.Scale(centerX: (float)LastPopUpElement.Width / 2, centerY: (float)LastPopUpElement.Height / 2, easingType: EasingType.Sine).Start();
            }
        }

        #endregion
    }
}
