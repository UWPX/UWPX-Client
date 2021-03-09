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
            if (args.NewValue is ChatMessageDataTemplate message)
            {
                MODEL.Message = message;
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
                chatDetailsContext.MODEL.MessageText = MODEL.Message.Message.message;
            }
        }

        public void SetFromUserAsClipboardText()
        {
            UiUtils.SetClipboardText(MODEL.Message.Message.fromBareJid);
        }

        public void SetMessageAsClipboardText()
        {
            UiUtils.SetClipboardText(MODEL.Message.Message.message);
        }

        public void SetDateAsClipboardText(IValueConverter converter)
        {
            UiUtils.SetClipboardText((string)converter.Convert(MODEL.Message.Message.date, typeof(string), null, null));
        }

        public async Task MarkAsSpamAsync()
        {
            await Task.Run(() =>
            {
                using (MainDbContext ctx = new MainDbContext())
                {
                    ctx.Add(new SpamMessageModel(MODEL.Message.Message.message));
                }
                Logger.Info("Marked message as spam: " + MODEL.Message.Message.message);
            });
        }

        public void DeleteMessage()
        {
            DataCache.INSTANCE.RemoveChatMessage(MODEL.Message);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
