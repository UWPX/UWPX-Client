using System;
using System.Numerics;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Shared.Classes;
using UWPX_UI.Controls.Settings;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Pages;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace UWPX_UI.Pages
{
    public sealed partial class RegisterPage: Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SettingsPageButtonDataTemplate LOGIN_DATA_TEMPLATE = new SettingsPageButtonDataTemplate { Glyph = "\uEBDB", Name = "Login", Description = "Use an existing account.", NavTarget = typeof(AddAccountPage) };
        public readonly SettingsPageButtonDataTemplate REGISTER_DATA_TEMPLATE = new SettingsPageButtonDataTemplate { Glyph = "\uE8FA", Name = "Register", Description = "Get a new account.", NavTarget = null };

        private FrameworkElement LastPopUpElement = null;

        public readonly RegisterPageContext VIEW_MODEL = new RegisterPageContext();

        /// <summary>
        /// Where should we navigate the frame to once we finished?
        /// </summary>
        private Type doneTargetPage = null;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public RegisterPage()
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
        private void UpdateViewState(string state)
        {
            VisualStateManager.GoToState(this, state, true);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnSelectionButtonPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!(DeviceFamilyHelper.IsMouseInteractionMode() && sender is SettingsSelectionLargeControl settingsSelection))
            {
                return;
            }

            LastPopUpElement = settingsSelection;
            Canvas.SetZIndex(LastPopUpElement, 10);
            AnimationBuilder.Create().Scale(to: new Vector2(1.05f), easingType: EasingType.Sine).Start(LastPopUpElement);
        }

        private void OnSelectionButtonPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (LastPopUpElement is null)
            {
                return;
            }

            Canvas.SetZIndex(LastPopUpElement, 0);
            AnimationBuilder.Create().Scale(to: new Vector2(1.0f), easingType: EasingType.Sine).Start(LastPopUpElement);
            LastPopUpElement = null;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedFrom();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            titleBar.OnPageNavigatedTo();

            if (e.Parameter is Type type)
            {
                doneTargetPage = type;
            }
            titleBar.OnPageNavigatedTo();
        }

        private void OnRegisterClicked(SettingsSelectionLargeControl sender, RoutedEventArgs args)
        {
            UpdateViewState(State_2.Name);
        }

        private void OnLoginClicked(object sender, RoutedEventArgs e)
        {
            UiUtils.NavigateToPage(typeof(AddAccountPage), doneTargetPage);
            // Make sure we remove the last entry from the back stack to prevent navigation back to this page:
            UiUtils.RemoveLastBackStackEntry();
        }

        private async void OnInfoLinkClicked(object sender, LinkClickedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(e.Link));
        }

        #endregion
    }
}
