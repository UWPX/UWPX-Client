using UWPX_UI_Context.Classes.DataContext;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat.SpeechBubbles
{
    public sealed partial class SpeechBubbleErrorControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatMessageDataTemplate ChatMessage
        {
            get { return (ChatMessageDataTemplate)GetValue(ChatMessageProperty); }
            set { SetValue(ChatMessageProperty, value); }
        }
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register(nameof(ChatMessage), typeof(ChatMessageDataTemplate), typeof(SpeechBubbleErrorControl), new PropertyMetadata(null, OnChatMessageChanged));

        private readonly SpeechBubbleContentContext VIEW_MODEL = new SpeechBubbleContentContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleErrorControl()
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
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnChatMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpeechBubbleErrorControl speechBubble)
            {
                speechBubble.UpdateView(e);
            }
        }

        #endregion
    }
}
