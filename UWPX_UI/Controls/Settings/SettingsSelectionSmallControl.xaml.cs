﻿using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Settings
{
    public sealed partial class SettingsSelectionSmallControl: UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public SettingsPageButtonDataTemplate Model
        {
            get => (SettingsPageButtonDataTemplate)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(nameof(Model), typeof(SettingsPageButtonDataTemplate), typeof(SettingsSelectionSmallControl), new PropertyMetadata(new SettingsPageButtonDataTemplate
        {
            Description = "Description",
            Glyph = "\uE9CE",
            Name = "Name",
            NavTarget = null
        }));

        public bool EnableNavigationOnClick
        {
            get => (bool)GetValue(EnableNavigationOnClickProperty);
            set => SetValue(EnableNavigationOnClickProperty, value);
        }
        public static readonly DependencyProperty EnableNavigationOnClickProperty = DependencyProperty.Register(nameof(EnableNavigationOnClick), typeof(bool), typeof(SettingsSelectionSmallControl), new PropertyMetadata(true));

        public delegate void ClickHandler(SettingsSelectionSmallControl sender, RoutedEventArgs args);
        public event ClickHandler Click;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SettingsSelectionSmallControl()
        {
            InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
            if (EnableNavigationOnClick)
            {
                UiUtils.NavigateToPage(Model?.NavTarget);
            }
        }

        #endregion
    }
}
