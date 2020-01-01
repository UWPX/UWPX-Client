using Shared.Classes;
using UWPX_UI_Context.Classes.DataContext.Dialogs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Dialogs
{
    public sealed partial class ConfirmDialog: ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string MarkdownText
        {
            get => (string)GetValue(MarkdownTextProperty);
            set => SetValue(MarkdownTextProperty, value);
        }
        public static readonly DependencyProperty MarkdownTextProperty = DependencyProperty.Register(nameof(MarkdownText), typeof(string), typeof(ConfirmDialog), new PropertyMetadata(""));

        public string PositiveText
        {
            get => (string)GetValue(PositiveTextProperty);
            set => SetValue(PositiveTextProperty, value);
        }
        public static readonly DependencyProperty PositiveTextProperty = DependencyProperty.Register(nameof(PositiveText), typeof(string), typeof(ConfirmDialog), new PropertyMetadata("Yes"));

        public string NegativeText
        {
            get => (string)GetValue(NegativeTextProperty);
            set => SetValue(NegativeTextProperty, value);
        }
        public static readonly DependencyProperty NegativeTextProperty = DependencyProperty.Register(nameof(NegativeText), typeof(string), typeof(ConfirmDialog), new PropertyMetadata("No"));

        public readonly ConfirmDialogContext VIEW_MODEL = new ConfirmDialogContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ConfirmDialog(string title, string markdownText)
        {
            Title = title;
            MarkdownText = markdownText;
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
        private void positive_btn_Click(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            VIEW_MODEL.OnPositive();
            Hide();
        }

        private void negative_btn_Click(Controls.IconButtonControl sender, RoutedEventArgs args)
        {
            VIEW_MODEL.OnNegative();
            Hide();
        }

        private async void Text_mrkdwn_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            await VIEW_MODEL.OnLinkClickedAsync(e.Link).ConfAwaitFalse();
        }

        #endregion
    }
}
