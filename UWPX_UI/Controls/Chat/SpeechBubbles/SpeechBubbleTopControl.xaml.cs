using UWPX_UI.Controls.Chat.SpeechBubbles.Content;
using UWPX_UI_Context.Classes.DataContext.Controls;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat.SpeechBubbles
{
    public sealed partial class SpeechBubbleTopControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatMessageDataTemplate ChatMessage
        {
            get { return (ChatMessageDataTemplate)GetValue(ChatMessageProperty); }
            set { SetValue(ChatMessageProperty, value); }
        }
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register(nameof(ChatMessage), typeof(ChatMessageDataTemplate), typeof(SpeechBubbleTopControl), new PropertyMetadata(null, OnChatMessageChanged));

        private readonly SpeechBubbleContentControlContext VIEW_MODEL = new SpeechBubbleContentControlContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleTopControl()
        {
            this.InitializeComponent();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            VIEW_MODEL.UpdateView(e);
            content_cp.Content = null; // Force a reevaluation of the content
            content_cp.Content = VIEW_MODEL;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void UserControl_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            if (sender is SpeechBubbleTopControl speechBubble)
            {
                if (content_cp.ContentTemplateRoot is IShowFlyoutSpeechBubbleContent flyoutSpeechBubbleContent)
                {
                    flyoutSpeechBubbleContent.ShowFlyout(speechBubble);
                }
            }
        }

        private static void OnChatMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpeechBubbleTopControl speechBubble)
            {
                speechBubble.UpdateView(e);
            }
        }

        #endregion
    }
}
