﻿using UWPX_UI.Controls.Chat.SpeechBubbles.Content;
using UWPX_UI_Context.Classes.DataContext;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Chat.SpeechBubbles
{
    public sealed partial class SpeechBubbleDownControl : UserControl
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public ChatMessageDataTemplate ChatMessage
        {
            get { return (ChatMessageDataTemplate)GetValue(ChatMessageProperty); }
            set { SetValue(ChatMessageProperty, value); }
        }
        public static readonly DependencyProperty ChatMessageProperty = DependencyProperty.Register(nameof(ChatMessage), typeof(ChatMessageDataTemplate), typeof(SpeechBubbleDownControl), new PropertyMetadata(null, OnChatMessageChanged));

        private readonly SpeechBubbleContentContext VIEW_MODEL = new SpeechBubbleContentContext();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SpeechBubbleDownControl()
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
            if (sender is SpeechBubbleDownControl speechBubble)
            {
                if (content_cp.ContentTemplateRoot is IShowFlyoutSpeechBubbleContent flyoutSpeechBubbleContent)
                {
                    flyoutSpeechBubbleContent.ShowFlyout(speechBubble);
                }
            }
        }

        private static void OnChatMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SpeechBubbleDownControl speechBubble)
            {
                speechBubble.UpdateView(e);
            }
        }

        #endregion
    }
}