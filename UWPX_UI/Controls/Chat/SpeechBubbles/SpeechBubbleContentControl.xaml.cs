using Microsoft.Toolkit.Uwp.UI.Extensions;
using UWPX_UI_Context.Classes;
using UWPX_UI_Context.Classes.DataContext;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace UWPX_UI.Controls.Chat.SpeechBubbles
{
    public sealed partial class SpeechBubbleContentControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatMessageDataTemplate ChatMessage
        {
            get { return (ChatMessageDataTemplate)GetValue(ChatMessageProperty); }
            set { SetValue(ChatMessageProperty, value); }
        }
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register(nameof(ChatMessage), typeof(ChatMessageDataTemplate), typeof(SpeechBubbleContentControl), new PropertyMetadata(null, OnChatMessageChanged));

        private readonly SpeechBubbleContentContext VIEW_MODEL = new SpeechBubbleContentContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleContentControl()
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
            VIEW_MODEL.UpdateView(e);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnChatMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpeechBubbleContentControl speechBubble)
            {
                speechBubble.UpdateView(e);
            }
        }

        private void CopyMessage_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.Text);
        }

        private void CopyNickname_mfi_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.SetClipboardText(VIEW_MODEL.MODEL.NicknameText);
        }

        private void CopyDate_mfi_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Resources["ChatDateTimeStringValueConverter"] is IValueConverter converter)
            {
                UiUtils.SetClipboardText((string)converter.Convert(VIEW_MODEL.MODEL.Date, typeof(string), null, null));
            }
        }

        private void ResendMsg_mfi_Click(object sender, RoutedEventArgs e)
        {
            ChatDetailsControl chatDetails = VisualTree.FindAscendant<ChatDetailsControl>(this);
            VIEW_MODEL.ResendMessage(chatDetails?.VIEW_MODEL);
        }

        #endregion
    }
}
