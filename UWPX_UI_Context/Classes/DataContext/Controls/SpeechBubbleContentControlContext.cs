using System.ComponentModel;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Chat;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
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
                ChatMessageModel.Message.PropertyChanged -= OnChatMessagePropertyChanged;
            }

            if (args.NewValue is ChatMessageDataTemplate newChatMessage)
            {
                ChatMessageModel = newChatMessage;
                ChatMessageModel.Message.PropertyChanged += OnChatMessagePropertyChanged;
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
            UiUtils.SetClipboardText(ChatMessageModel.Message.fromBareJid);
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
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Add(new SpamMessageModel(ChatMessageModel.Message.message));
                }
                Logger.Info("Marked message as spam: " + ChatMessageModel.Message.message);
            });
        }

        public void DeleteMessage()
        {
            DataCache.INSTANCE.RemoveChatMessage(ChatMessageModel);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void OnChatMessagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ChatMessageModel)
            {
                MODEL.UpdateView(ChatMessageModel.Chat, ChatMessageModel.Message);
            }
        }

        #endregion
    }
}
