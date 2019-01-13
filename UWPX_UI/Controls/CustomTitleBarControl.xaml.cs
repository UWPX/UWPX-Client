﻿using UWPX_UI.Controls.Toolkit.MasterDetailsView;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPX_UI.Controls
{
    public sealed partial class CustomTitleBarControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Frame Frame
        {
            get { return (Frame)GetValue(FrameProperty); }
            set { SetValue(FrameProperty, value); }
        }
        public static readonly DependencyProperty FrameProperty = DependencyProperty.Register(nameof(Frame), typeof(Frame), typeof(CustomTitleBarControl), new PropertyMetadata(null));

        public MasterDetailsView MasterDetailsView
        {
            get { return (MasterDetailsView)GetValue(MasterDetailsViewProperty); }
            set { SetValue(MasterDetailsViewProperty, value); }
        }
        public static readonly DependencyProperty MasterDetailsViewProperty = DependencyProperty.Register(nameof(MasterDetailsView), typeof(MasterDetailsView), typeof(CustomTitleBarControl), new PropertyMetadata(null));

        public Visibility BackRequestButtonVisability
        {
            get { return (Visibility)GetValue(BackRequestButtonVisabilityProperty); }
            set { SetValue(BackRequestButtonVisabilityProperty, value); }
        }
        public static readonly DependencyProperty BackRequestButtonVisabilityProperty = DependencyProperty.Register(nameof(BackRequestButtonVisability), typeof(Visibility), typeof(CustomTitleBarControl), new PropertyMetadata(Visibility.Visible));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public CustomTitleBarControl()
        {
            this.InitializeComponent();
            if (!UiUtils.IsRunningOnDesktopDevice())
            {
                this.Visibility = Visibility.Collapsed;
                return;
            }
            InitTitleBar();
            SetupKeyboardAccelerators();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void InitTitleBar()
        {
            if (UiUtils.IsApplicationViewApiAvailable() && !UiUtils.IsRunningOnMobileDevice())
            {
                // Set XAML element as a draggable region.
                CoreApplicationViewTitleBar titleBar = CoreApplication.GetCurrentView().TitleBar;
                UpdateTitleBarLayout(titleBar);
                Window.Current.SetTitleBar(titleBar_grid);
                titleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
            }
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar titleBar)
        {
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            leftPaddingColumn.Width = new GridLength(titleBar.SystemOverlayLeftInset);
            rightPaddingColumn.Width = new GridLength(titleBar.SystemOverlayRightInset);
            titleBar_grid.Margin = new Thickness(0, -1, 0, 0);

            // Update title bar control size as needed to account for system size changes.
            titleBar_grid.Height = titleBar.Height;
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
                args.Handled = true;
                UiUtils.OnGoBackRequested(Frame);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void BackRequest_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!(MasterDetailsView is null) && MasterDetailsView.ViewState == MasterDetailsViewState.Details)
            {
                MasterDetailsView.SelectedItem = null;
            }
            else
            {
                UiUtils.OnGoBackRequested(Frame);
            }
        }

        #endregion
    }
}
