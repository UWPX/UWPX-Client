using Microsoft.Toolkit.Uwp.UI.Extensions;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace UWPX_UI.Controls.Chat.SpeechBubbles.Content
{
    public sealed partial class SpeechBubbleTextContentControl : UserControl, IShowFlyoutSpeechBubbleContent
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public SpeechBubbleContentContext SpeechBubbleContentViewModel
        {
            get { return (SpeechBubbleContentContext)GetValue(SpeechBubbleContentViewModelProperty); }
            set { SetValue(SpeechBubbleContentViewModelProperty, value); }
        }
        public static readonly DependencyProperty SpeechBubbleContentViewModelProperty = DependencyProperty.Register(nameof(SpeechBubbleContentViewModel), typeof(SpeechBubbleContentContext), typeof(SpeechBubbleTextContentControl), new PropertyMetadata(null));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleTextContentControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void ShowFlyout(FrameworkElement sender)
        {
            if (Resources["options_mfo"] is MenuFlyout flyout)
            {
                flyout.ShowAt(sender);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            SpeechBubbleContentViewModel.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
        private static void OnChatMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpeechBubbleTextContentControl speechBubbleTextContent)
            {
                speechBubbleTextContent.UpdateView(e);
            }
        }

        private void CopyMessage_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(SpeechBubbleContentViewModel.MODEL.Text);
        }

        private void CopyNickname_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(SpeechBubbleContentViewModel.MODEL.NicknameText);
        }

        private void CopyDate_mfi_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Resources["ChatDateTimeStringValueConverter"] is IValueConverter converter)
            {
                UiUtils.SetClipboardText((string)converter.Convert(SpeechBubbleContentViewModel.MODEL.Date, typeof(string), null, null));
            }
        }

        private void ResendMsg_mfi_Click(object sender, RoutedEventArgs e)
        {
            ChatDetailsControl chatDetails = VisualTree.FindAscendant<ChatDetailsControl>(this);
            SpeechBubbleContentViewModel.ResendMessage(chatDetails?.VIEW_MODEL);
        }
    }
}
