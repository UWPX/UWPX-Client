using System;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class InitialStartDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly InitialStartDialogContext VIEW_MODEL = new InitialStartDialogContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public InitialStartDialog()
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
        private void GoToState1()
        {
            state1_scroll.Visibility = Visibility.Visible;
            state2_scroll.Visibility = Visibility.Collapsed;
            next_btn.Visibility = Visibility.Visible;
            previous_btn.Visibility = Visibility.Collapsed;
            skip_btn.Visibility = Visibility.Visible;
            finish_btn.Visibility = Visibility.Collapsed;
        }

        private void GoToState2()
        {
            state1_scroll.Visibility = Visibility.Collapsed;
            state2_scroll.Visibility = Visibility.Visible;
            next_btn.Visibility = Visibility.Collapsed;
            previous_btn.Visibility = Visibility.Visible;
            skip_btn.Visibility = Visibility.Collapsed;
            finish_btn.Visibility = Visibility.Visible;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void OnMarkdownLinkClicked(object sender, LinkClickedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(e.Link));
        }

        private void OnSkipClicked(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            Hide();
        }

        private void OnPreviousClicked(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            GoToState1();
        }

        private void OnNextClicked(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            GoToState2();
        }

        private void OnFinishClicked(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            Hide();
        }

        private async void OnMoreInformationClicked(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(Localisation.GetLocalizedString("PrivacyPolicyCrashReportingUrl")));
        }

        #endregion
    }
}
