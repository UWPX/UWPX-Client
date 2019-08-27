using System;
using Shared.Classes;
using UWPX_UI.Controls.Toolkit.MasterDetailsView;
using UWPX_UI.Pages;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPX_UI.Controls
{
    public sealed partial class CustomTitleBarControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Frame Frame
        {
            get => (Frame)GetValue(FrameProperty);
            set => SetValue(FrameProperty, value);
        }
        public static readonly DependencyProperty FrameProperty = DependencyProperty.Register(nameof(Frame), typeof(Frame), typeof(CustomTitleBarControl), new PropertyMetadata(null, OnFrameChanged));

        public MasterDetailsView MasterDetailsView
        {
            get => (MasterDetailsView)GetValue(MasterDetailsViewProperty);
            set => SetValue(MasterDetailsViewProperty, value);
        }
        public static readonly DependencyProperty MasterDetailsViewProperty = DependencyProperty.Register(nameof(MasterDetailsView), typeof(MasterDetailsView), typeof(CustomTitleBarControl), new PropertyMetadata(null));

        public Visibility BackRequestButtonVisibility
        {
            get => (Visibility)GetValue(BackRequestButtonVisibilityProperty);
            set => SetValue(BackRequestButtonVisibilityProperty, value);
        }
        public static readonly DependencyProperty BackRequestButtonVisibilityProperty = DependencyProperty.Register(nameof(BackRequestButtonVisibility), typeof(Visibility), typeof(CustomTitleBarControl), new PropertyMetadata(Visibility.Visible));

        public Type NavigationFallbackPage
        {
            get => (Type)GetValue(NavigationFallbackPageProperty);
            set => SetValue(NavigationFallbackPageProperty, value);
        }
        public static readonly DependencyProperty NavigationFallbackPageProperty = DependencyProperty.Register(nameof(NavigationFallbackPage), typeof(Type), typeof(CustomTitleBarControl), new PropertyMetadata(typeof(ChatPage)));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(CustomTitleBarControl), new PropertyMetadata(false, OnIsActiveChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CustomTitleBarControl()
        {
            InitializeComponent();
            if (!DeviceFamilyHelper.IsRunningOnDesktopDevice())
            {
                Visibility = Visibility.Collapsed;
            }
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

        public bool OnGoBackRequested()
        {
            if (!IsActive)
            {
                return false;
            }

            if (!(MasterDetailsView is null) && MasterDetailsView.ViewState == MasterDetailsViewState.Details)
            {
                MasterDetailsView.ClearSelectedItem();
                return true;
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
            return true;
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

        private void OnIsActiveChanged(bool newValue)
        {
            if (newValue)
            {
                UpdateTitleBarLayout();
            }
        }

        private void Frame_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(Frame).Properties.IsXButton1Pressed)
            {
                e.Handled = OnGoBackRequested();
            }
        }

        private void OnFrameChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is Frame oldFrame)
            {
                oldFrame.PointerPressed -= Frame_PointerPressed;
            }
            if (e.NewValue is Frame newFrame)
            {
                newFrame.PointerPressed += Frame_PointerPressed;
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

        private void Accelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (!args.Handled)
            {
                args.Handled = OnGoBackRequested();
            }
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomTitleBarControl titleBarControl)
            {
                titleBarControl.OnIsActiveChanged(e.NewValue is bool b && b);
            }
        }

        private static void OnFrameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomTitleBarControl titleBarControl)
            {
                titleBarControl.OnFrameChanged(e);
            }
        }

        #endregion
    }
}
