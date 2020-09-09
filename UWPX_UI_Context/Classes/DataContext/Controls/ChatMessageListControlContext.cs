using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.Events;
using Data_Manager2.Classes.Toast;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public class ChatMessageListControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatMessageListControlDataTemplate MODEL = new ChatMessageListControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatDataTemplate oldChat, ChatDataTemplate newChat)
        {
            if (!(oldChat is null))
            {
                oldChat.PropertyChanged -= Chat_PropertyChanged;
                oldChat.ChatMessageChanged -= Chat_ChatMessageChanged;
                oldChat.NewChatMessage -= Chat_NewChatMessage;
            }

            if (!(newChat is null))
            {
                newChat.PropertyChanged += Chat_PropertyChanged;
                newChat.ChatMessageChanged += Chat_ChatMessageChanged;
                newChat.NewChatMessage += Chat_NewChatMessage;
            }

            MODEL.UpdateView(oldChat, newChat);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private void Chat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ChatDataTemplate chat)
            {
                UpdateView(MODEL.Chat, chat);
            }
        }

        private async void Chat_NewChatMessage(ChatDataTemplate chat, NewChatMessageEventArgs args)
        {
            if (!MODEL.IsDummy)
            {
                await MODEL.OnNewChatMessageAsync(args.MESSAGE, chat.Chat, chat.MucInfo);
                if (args.MESSAGE.state == MessageState.UNREAD)
                {
                    // Mark message as read and update the badge notification count:
                    await Task.Run(() =>
                    {
                        ChatDBManager.INSTANCE.markMessageAsRead(args.MESSAGE);
                        ToastHelper.UpdateBadgeNumber();
                    });
                }
            }
        }

        private async void Chat_ChatMessageChanged(ChatDataTemplate chat, ChatMessageChangedEventArgs args)
        {
            if (!MODEL.IsDummy)
            {
                await MODEL.OnChatMessageChangedAsync(args.MESSAGE, args.REMOVED);
            }
        }

        #endregion
    }
}
