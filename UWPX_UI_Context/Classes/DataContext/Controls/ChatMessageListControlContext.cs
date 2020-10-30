using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
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

        private CancellationTokenSource loadMoreMessagesToken = null;
        private Task<List<ChatMessageDataTemplate>> loadMoreMessagesTask = null;
        private readonly SemaphoreSlim LOAD_MORE_MESSAGES_SEMA = new SemaphoreSlim(1);

        private const int MAX_MESSAGES_PER_REQUEST = 15;

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

        public Task LoadMoreMessagesAsync()
        {
            return Task.Run(async () =>
            {
                if (!(loadMoreMessagesTask is null))
                {
                    if (!(loadMoreMessagesToken is null) && !loadMoreMessagesToken.IsCancellationRequested)
                    {
                        loadMoreMessagesToken.Cancel();
                    }
                    await loadMoreMessagesTask;
                }

                await LOAD_MORE_MESSAGES_SEMA.WaitAsync();
                MODEL.IsLoading = true;
                loadMoreMessagesToken = new CancellationTokenSource();

                loadMoreMessagesTask = Task.Run(() =>
                {
                    List<ChatMessageDataTemplate> tmpMsgs = new List<ChatMessageDataTemplate>();
                    IList<ChatMessageTable> list = ChatDBManager.INSTANCE.getNextNChatMessages(MODEL.Chat.Chat.id, MODEL.GetLastMessageId(), MAX_MESSAGES_PER_REQUEST + 1); // Load one item more than we use laster to determine if there are more items available
                    for (int i = 0; i < list.Count && i < MAX_MESSAGES_PER_REQUEST && !loadMoreMessagesToken.IsCancellationRequested; i++)
                    {
                        tmpMsgs.Add(new ChatMessageDataTemplate
                        {
                            Message = list[i],
                            Chat = MODEL.Chat.Chat,
                            MUC = MODEL.Chat.MucInfo
                        });
                    }
                    MODEL.HasMoreMessages = list.Count > MAX_MESSAGES_PER_REQUEST;
                    return tmpMsgs;
                });

                List<ChatMessageDataTemplate> msgs = await loadMoreMessagesTask;
                if (!loadMoreMessagesToken.IsCancellationRequested && msgs.Count > 0)
                {
                    ToastHelper.removeToastGroup(MODEL.Chat.Chat.id);

                    await MODEL.InsertChatMessagesAsync(msgs);
                }
                MODEL.IsLoading = false;
                LOAD_MORE_MESSAGES_SEMA.Release();
            });
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
