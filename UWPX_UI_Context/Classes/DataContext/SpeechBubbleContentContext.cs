using Logging;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext
{
    public class SpeechBubbleContentContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SpeechBubbleContentDataTemplate MODEL = new SpeechBubbleContentDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue is ChatMessageDataTemplate oldChatMessage)
            {
                oldChatMessage.PropertyChanged += ChatMessage_PropertyChanged;
            }

            if (args.NewValue is ChatMessageDataTemplate newChatMessage)
            {
                newChatMessage.PropertyChanged += ChatMessage_PropertyChanged;
                MODEL.UpdateView(newChatMessage.Chat, newChatMessage.Message);
            }
        }

        public void ResendMessage(ChatDetailsControlContext chatDetailsContext)
        {
            if (chatDetailsContext is null)
            {
                Logger.Error("Failed to resend message - chatDetailsContext is null!");
            }
            else
            {
                chatDetailsContext.MODEL.MessageText = MODEL.Text;
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void ChatMessage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ChatMessageDataTemplate chatMessage)
            {
                MODEL.UpdateView(chatMessage.Chat, chatMessage.Message);
            }
        }

        #endregion
    }
}
