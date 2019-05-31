using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Toast;
using Logging;
using Shared.Classes;
using UWPX_UI_Context.Classes.Collections.Toolkit;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public class ChatMessageListControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly IncrementalLoadingCollection<ChatMessageDataTemplate> CHAT_MESSAGES;

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
        private CancellationToken loadChatMessagesCancelToken = default;
        private Task loadChatMessagesTask = null;
        private bool loaded = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageListControlDataTemplate()
        {
            CHAT_MESSAGES = new IncrementalLoadingCollection<ChatMessageDataTemplate>()
            {
                OnStartLoading = () => IsLoading = true,
                OnEndLoading = () => IsLoading = false,
                GetPagedItemsFuncAsync = GetPagedItemsFuncAsync
            };
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public async Task<IEnumerable<ChatMessageDataTemplate>> GetPagedItemsFuncAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            List<ChatMessageDataTemplate> msgs = new List<ChatMessageDataTemplate>();

            if (!(Chat is null) && !loaded)
            {
                loaded = true;
                return await LoadChatMessagesAsync(pageIndex, pageSize, cancellationToken);
            }
            return msgs;
        }

        private void SetChatProperty(ChatDataTemplate value)
        {
            if (SetProperty(ref _Chat, value, nameof(Chat)) && !(value is null))
            {
                loaded = false;
                // Chat changed load chat messages:
                CHAT_MESSAGES.RefreshAsync();
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

        #endregion

        #region --Misc Methods (Private)--
        private async Task<List<ChatMessageDataTemplate>> LoadChatMessagesAsync(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                loadChatMessagesCancelToken = cancellationToken;

                if (!(loadChatMessagesTask is null))
                {
                    await loadChatMessagesTask;
                }

                List<ChatMessageDataTemplate> msgs = new List<ChatMessageDataTemplate>();
                loadChatMessagesTask = Task.Run(() =>
                {
                    IList<ChatMessageTable> list = ChatDBManager.INSTANCE.getAllChatMessagesForChat(Chat.Chat.id);
                    for (int i = 0; i < list.Count && !loadChatMessagesCancelToken.IsCancellationRequested; i++)
                    {
                        msgs.Add(new ChatMessageDataTemplate
                        {
                            Message = list[i],
                            Chat = Chat.Chat,
                            MUC = Chat.MucInfo
                        });
                    }
                });

                await loadChatMessagesTask;

                if (!loadChatMessagesCancelToken.IsCancellationRequested)
                {
                    ToastHelper.removeToastGroup(Chat.Chat.id);
                }
                return msgs;
            });
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
