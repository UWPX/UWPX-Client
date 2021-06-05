using System;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Shared.Classes.AppCenter;
using UWPX_UI.Controls;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class ReportCrashDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ReportCrashDialogContext VIEW_MODEL = new ReportCrashDialogContext();

        public string MarkdownText
        {
            get => (string)GetValue(MarkdownTextProperty);
            set => SetValue(MarkdownTextProperty, value);
        }
        public static readonly DependencyProperty MarkdownTextProperty = DependencyProperty.Register(nameof(MarkdownText), typeof(string), typeof(ReportCrashDialog), new PropertyMetadata("No information provided..."));

        public string Comment
        {
            get => (string)GetValue(CommentProperty);
            set => SetValue(CommentProperty, value);
        }
        public static readonly DependencyProperty CommentProperty = DependencyProperty.Register(nameof(Comment), typeof(string), typeof(ReportCrashDialog), new PropertyMetadata(""));

        public bool AlwaysReport
        {
            get => (bool)GetValue(AlwaysReportProperty);
            set => SetValue(AlwaysReportProperty, value);
        }
        public static readonly DependencyProperty AlwaysReportProperty = DependencyProperty.Register(nameof(AlwaysReport), typeof(bool), typeof(ReportCrashDialog), new PropertyMetadata(false));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ReportCrashDialog(TrackErrorEventArgs args)
        {
            InitializeComponent();
            MarkdownText = args.ToMarkdown();
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
        private void Positive_btn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            VIEW_MODEL.MODEL.Report = true;
            Hide();
        }

        private void Negative_btn_Click(IconButtonControl sender, RoutedEventArgs args)
        {
            VIEW_MODEL.MODEL.Report = false;
            Hide();
        }

        private async void Text_mrkdwn_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            await UiUtils.LaunchUriAsync(new Uri(e.Link));
        }

        #endregion
    }
}
