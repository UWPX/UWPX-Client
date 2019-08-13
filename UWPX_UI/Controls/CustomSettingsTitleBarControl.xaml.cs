using System;
using Shared.Classes;
using UWPX_UI.Pages;
using UWPX_UI.Pages.Settings;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPX_UI.Controls
{
    public sealed partial class CustomSettingsTitleBarControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Frame Frame
        {
            get => (Frame)GetValue(FrameProperty);
            set => SetValue(FrameProperty, value);
        }
        public static readonly DependencyProperty FrameProperty = DependencyProperty.Register(nameof(Frame), typeof(Frame), typeof(CustomSettingsTitleBarControl), new PropertyMetadata(null));

        public string Glyph
        {
            get => (string)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(CustomSettingsTitleBarControl), new PropertyMetadata("\uE9CE"));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(CustomSettingsTitleBarControl), new PropertyMetadata("Text"));

        public Visibility BackRequestButtonVisibility
        {
            get => (Visibility)GetValue(BackRequestButtonVisibilityProperty);
            set => SetValue(BackRequestButtonVisibilityProperty, value);
        }
        public static readonly DependencyProperty BackRequestButtonVisibilityProperty = DependencyProperty.Register(nameof(BackRequestButtonVisibility), typeof(Visibility), typeof(CustomSettingsTitleBarControl), new PropertyMetadata(Visibility.Visible));

        public Type NavigationFallbackPage
        {
            get => (Type)GetValue(NavigationFallbackPageProperty);
            set => SetValue(NavigationFallbackPageProperty, value);
        }
        public static readonly DependencyProperty NavigationFallbackPageProperty = DependencyProperty.Register(nameof(NavigationFallbackPage), typeof(Type), typeof(CustomSettingsTitleBarControl), new PropertyMetadata(typeof(ChatPage)));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(CustomSettingsTitleBarControl), new PropertyMetadata(false, OnIsActiveChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CustomSettingsTitleBarControl()
        {
            InitializeComponent();
            SetupTitleBar();
            SetupKeyboardAccelerators();

            SystemNavigationManager.GetForCurrentView().BackRequested += CustomTitleBarControl_BackRequested;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void OnPageNavigatedTo()
        {
            IsActive = true;
        }

        public void OnPageNavigatedFrom()
        {
            IsActive = false;
        }

        #endregion

        #region --Misc Methods (Private)--
        private void SetupTitleBar()
        {
            if (UiUtils.IsApplicationViewApiAvailable())
            {
                if (!DeviceFamilyHelper.ShouldShowBackButton())
                {
                    BackRequestButtonVisibility = Visibility.Collapsed;
                }

                UpdateTitleBarLayout();
            }
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout()
        {
            if (!DeviceFamilyHelper.IsRunningOnDesktopDevice())
            {
                return;
            }

            // Set XAML element as a draggable region.
            CoreApplicationViewTitleBar titleBar = CoreApplication.GetCurrentView().TitleBar;
            UpdateTitleBarLayout(titleBar);
            Window.Current.SetTitleBar(titleBar_grid);
            titleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar titleBar)
        {
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            leftPaddingColumn.Width = new GridLength(titleBar.SystemOverlayLeftInset);
            rightPaddingColumn.Width = new GridLength(titleBar.SystemOverlayRightInset);
            titleBar_grid.Margin = new Thickness(0, -1, 0, 0);

            // Update title bar control size as needed to account for system size changes.
            // Do not update the Height since it will be always 0 - bug
            // Reference: https://wpdev.uservoice.com/forums/110705-universal-windows-platform/suggestions/15657012-titlebar-extendviewintotitlebar-in-anniv-update-b
            // titleBar_grid.Height = titleBar.Height;
        }

        private void SetupKeyboardAccelerators()
        {
            if (UiUtils.IsKeyboardAcceleratorApiAvailable())
            {
                foreach (KeyboardAccelerator accelerator in UiUtils.GetGoBackKeyboardAccelerators())
                {
                    accelerator.Invoked += Accelerator_Invoked;
                    KeyboardAccelerators.Add(accelerator);
                }
            }
        }

        private void Accelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (!args.Handled)
            {
                args.Handled = OnGoBackRequested();
            }
        }

        private bool OnGoBackRequested()
        {
            if (!IsActive)
            {
                return false;
            }

            if (UiUtils.OnGoBackRequested(Frame))
            {
                return true;
            }

            if (!(NavigationFallbackPage is null))
            {
                bool b = UiUtils.NavigateToPage(NavigationFallbackPage);
                UiUtils.RemoveLastBackStackEntry();
                return b;
            }

            return false;
        }

        private void OnIsActiveChanged(bool newValue)
        {
            if (newValue)
            {
                UpdateTitleBarLayout();
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void BackRequest_btn_Click(object sender, RoutedEventArgs e)
        {
            OnGoBackRequested();
        }

        private void CustomTitleBarControl_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = OnGoBackRequested();
            }
        }

        private void GoToOverview_btn_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(SettingsPage));
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomSettingsTitleBarControl titleBarControl)
            {
                titleBarControl.OnIsActiveChanged(e.NewValue is bool b && b);
            }
        }

        #endregion
    }
}
