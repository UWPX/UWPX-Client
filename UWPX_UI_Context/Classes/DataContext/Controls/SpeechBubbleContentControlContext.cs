using Data_Manager2.Classes.DBManager;
using Logging;
using System;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public class SpeechBubbleContentControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SpeechBubbleContentControlDataTemplate MODEL = new SpeechBubbleContentControlDataTemplate();
        public ChatMessageDataTemplate ChatMessageModel = null;

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
            if (!(ChatMessageModel is null))
            {
                ChatMessageModel.PropertyChanged -= ChatMessage_PropertyChanged;
            }

            if (args.NewValue is ChatMessageDataTemplate newChatMessage)
            {
                ChatMessageModel = newChatMessage;
                ChatMessageModel.PropertyChanged += ChatMessage_PropertyChanged;
                MODEL.UpdateView(ChatMessageModel.Chat, ChatMessageModel.Message);
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

        public void SetFromUserAsClipboardText()
        {
            UiUtils.SetClipboardText(ChatMessageModel.Message.fromUser);
        }

        public void SetMessageAsClipboardText()
        {
            UiUtils.SetClipboardText(MODEL.Text);
        }

        public void SetDateAsClipboardText(IValueConverter converter)
        {
            UiUtils.SetClipboardText((string)converter.Convert(MODEL.Date, typeof(string), null, null));
        }

        public async Task MarkAsSpamAsync()
        {
            await Task.Run(() =>
            {
                SpamDBManager.INSTANCE.addSpamMessage(ChatMessageModel.Message.message, DateTime.Now);
                Logger.Info("Marked message as spam: " + ChatMessageModel.Message.message);
            });
        }

        public async Task DeleteMessageAsync()
        {
            await Task.Run(() => ChatDBManager.INSTANCE.deleteChatMessageAsync(ChatMessageModel.Message, true));
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
