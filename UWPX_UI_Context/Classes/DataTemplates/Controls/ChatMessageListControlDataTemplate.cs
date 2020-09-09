using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Logging;
using Shared.Classes;
using Shared.Classes.Collections;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class ChatMessageListControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly CustomObservableCollection<ChatMessageDataTemplate> CHAT_MESSAGES;
        public bool hasMoreMessages
        {
            get;
            private set;
        }

        private ChatDataTemplate _Chat;
        public ChatDataTemplate Chat
        {
            get => _Chat;
            set => SetChatProperty(value);
        }

        private bool _IsDummy;
        public bool IsDummy
        {
            get => _IsDummy;
            set => SetProperty(ref _IsDummy, value);
        }

        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        private readonly SemaphoreSlim CHAT_MESSAGES_SEMA = new SemaphoreSlim(1);
        private CancellationTokenSource loadMoreMessagesToken = null;
        private Task<List<ChatMessageDataTemplate>> loadMoreMessagesTask = null;
        private readonly SemaphoreSlim LOAD_MORE_MESSAGES_SEMA = new SemaphoreSlim(1);

        private const int MAX_MESSAGES_PER_REQUEST = 10;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageListControlDataTemplate()
        {
            CHAT_MESSAGES = new CustomObservableCollection<ChatMessageDataTemplate>(true);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        private async void SetChatProperty(ChatDataTemplate value)
        {
            if (SetProperty(ref _Chat, value, nameof(Chat)) && !(value is null))
            {
                // Chat changed load chat messages:
                CHAT_MESSAGES.Clear();
                await LoadMoreMessagesAsync();
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(ChatDataTemplate oldChat, ChatDataTemplate newChat)
        {
            Chat = newChat;
        }

        public async Task OnNewChatMessageAsync(ChatMessageTable msg, ChatTable chat, MUCChatInfoTable muc)
        {
            await CHAT_MESSAGES_SEMA.WaitAsync();
            await SharedUtils.CallDispatcherAsync(() =>
            {
                CHAT_MESSAGES.Add(new ChatMessageDataTemplate
                {
                    Chat = chat,
                    Message = msg,
                    MUC = muc
                });
            });
            CHAT_MESSAGES_SEMA.Release();
        }

        public async Task OnChatMessageChangedAsync(ChatMessageTable msg, bool removed)
        {
            await CHAT_MESSAGES_SEMA.WaitAsync();
            for (int i = 0; i < CHAT_MESSAGES.Count; i++)
            {
                if (string.Equals(CHAT_MESSAGES[i].Message.id, msg.id))
                {
                    if (removed)
                    {
                        CHAT_MESSAGES.RemoveAt(i);
                    }
                    else
                    {
                        CHAT_MESSAGES[i].Message = msg;
                    }
                    CHAT_MESSAGES_SEMA.Release();
                    return;
                }
            }
            CHAT_MESSAGES_SEMA.Release();
            Logger.Warn("OnChatMessageChanged failed - no chat message with id: " + msg.id + " for chat: " + msg.chatId);
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
                loadMoreMessagesToken = new CancellationTokenSource();

                loadMoreMessagesTask = Task.Run(() =>
                {
                    List<ChatMessageDataTemplate> tmpMsgs = new List<ChatMessageDataTemplate>();
                    IList<ChatMessageTable> list = ChatDBManager.INSTANCE.getNextNChatMessages(Chat.Chat.id, GetLastMessageId(), MAX_MESSAGES_PER_REQUEST + 1); // Load one item more than we use laster to determin if there are more items available
                    for (int i = 0; i < list.Count && i < MAX_MESSAGES_PER_REQUEST && !loadMoreMessagesToken.IsCancellationRequested; i++)
                    {
                        tmpMsgs.Add(new ChatMessageDataTemplate
                        {
                            Message = list[i],
                            Chat = Chat.Chat,
                            MUC = Chat.MucInfo
                        });
                    }
                    hasMoreMessages = list.Count > MAX_MESSAGES_PER_REQUEST;
                    return tmpMsgs;
                });

                List<ChatMessageDataTemplate> msgs = await loadMoreMessagesTask;
                if (!loadMoreMessagesToken.IsCancellationRequested && msgs.Count > 0)
                {
                    ToastHelper.removeToastGroup(Chat.Chat.id);

                    await CHAT_MESSAGES_SEMA.WaitAsync();
                    CHAT_MESSAGES.InsertRange(0, msgs);
                    CHAT_MESSAGES_SEMA.Release();
                }

                LOAD_MORE_MESSAGES_SEMA.Release();
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private string GetLastMessageId()
        {
            return CHAT_MESSAGES.Count > 0 ? CHAT_MESSAGES[0].Message.id : null;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
