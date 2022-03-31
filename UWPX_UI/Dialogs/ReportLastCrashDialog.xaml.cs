using System;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UWPX_UI.Controls;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class ReportLastCrashDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ReportLastCrashDialogContext VIEW_MODEL = new ReportLastCrashDialogContext();
        private readonly string DETAILS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ReportLastCrashDialog(string details)
        {
            DETAILS = details;
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
        private void OnCancelClicked(IconButtonControl sender, RoutedEventArgs args)
        {
            Hide();
        }

        private void OnSendClicked(IconButtonControl sender, RoutedEventArgs args)
        {
            AppCenterHelper.ReportCrashDetails(DETAILS, VIEW_MODEL.MODEL.Report);
            Hide();
        }

        private async void OnMarkdownLinkClicked(object sender, LinkClickedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(e.Link));
        }

        private void OnCopyCrashDetailsClicked(IconButtonControl sender, RoutedEventArgs args)
        {
            UiUtils.SetClipboardText(DETAILS);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            report_tbx.Focus(FocusState.Programmatic);
        }

        #endregion
    }
}
