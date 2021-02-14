using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Toast;
using Shared.Classes.Collections;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0313;

namespace Manager.Classes.Chat
{
    public class ChatMessageCacheList: CustomObservableCollection<ChatMessageDataTemplate>
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ChatDataTemplate chat;

        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => SetProperty(ref _IsLoading, value);
        }

        private bool _HasMoreMessages;
        public bool HasMoreMessages
        {
            get => _HasMoreMessages;
            set => SetProperty(ref _HasMoreMessages, value);
        }

        private int _UnreadCount;
        public int UnreadCount
        {
            get => _UnreadCount;
            set => SetProperty(ref _UnreadCount, value);
        }

        private CancellationTokenSource loadMoreMessagesToken = null;
        private Task<List<ChatMessageDataTemplate>> loadMoreMessagesTask = null;
        private readonly SemaphoreSlim LOAD_MORE_MESSAGES_SEMA = new SemaphoreSlim(1);

        private bool mamRequested = false;
        private bool loadedAllLocalMessages = false;
        private const int MAX_MESSAGES_PER_REQUEST = 15;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageCacheList() : base(false)
        {
            invokeInUiThread = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetChat(ChatDataTemplate chat)
        {
            if (this.chat == chat)
            {
                return;
            }
            this.chat = chat;
            LoadChatMessages();
        }

        private string GetLastMessageStableId()
        {
            return GetLastMessage()?.Message?.stableId;
        }

        private ChatMessageDataTemplate GetLastMessage()
        {
            return Count > 0 ? this[0] : null;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Adds the given <paramref name="msg"/> to the list sorted by the message date.
        /// </summary>
        /// <param name="msg">The chat message that should be added sorted by its message date.</param>
        public void InsertSorted(ChatMessageDataTemplate msg)
        {
            if (Count <= 0)
            {
                Add(msg);
                return;
            }
            if (this[Count - 1].CompareTo(msg) <= 0)
            {
                Add(msg);
                return;
            }
            if (this[0].CompareTo(msg) >= 0)
            {
                Insert(0, msg);
                return;
            }
            int index = this.ToList().BinarySearch(msg);
            if (index < 0)
            {
                index = ~index;
            }

            Insert(index, msg);
        }

        public async Task LoadMoreMessagesAsync()
        {
            await Task.Run(async () =>
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
                IsLoading = true;
                loadMoreMessagesToken = new CancellationTokenSource();

                loadMoreMessagesTask = Task.Run(async () =>
                {
                    return await LoadMoreMessagesInTaskAsync();
                });

                List<ChatMessageDataTemplate> msgs = await loadMoreMessagesTask;
                if (!loadMoreMessagesToken.IsCancellationRequested && msgs.Count > 0)
                {
                    ToastHelper.RemoveToastGroup(chat.Chat.id.ToString());
                    foreach (ChatMessageDataTemplate msg in msgs)
                    {
                        InsertSorted(msg);
                    }
                }
                IsLoading = false;
                UpdateUnreadCount();
                LOAD_MORE_MESSAGES_SEMA.Release();
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private void LoadChatMessages()
        {
            // Cancel all outstanding loading operations:
            if (!(loadMoreMessagesToken is null) && !loadMoreMessagesToken.IsCancellationRequested)
            {
                loadMoreMessagesToken.Cancel();
            }

            // Load the initial bunch of messages and reset all properties:
            IsLoading = true;
            Clear();
            if (!(chat is null))
            {
                IEnumerable<ChatMessageDataTemplate> msgs;
                using (MainDbContext ctx = new MainDbContext())
                {
                    msgs = ctx.GetNextNChatMessages(chat.Chat, MAX_MESSAGES_PER_REQUEST).Select(msg => new ChatMessageDataTemplate(msg, chat.Chat));
                }
                AddRange(msgs);
                HasMoreMessages = true;
                loadedAllLocalMessages = false;
                mamRequested = false;
                UpdateUnreadCount();
            }
            else
            {
                HasMoreMessages = false;
                loadedAllLocalMessages = true;
                mamRequested = true;
            }
            UpdateUnreadCount();
            IsLoading = false;
        }

        private async Task<List<ChatMessageDataTemplate>> LoadMoreMessagesInTaskAsync()
        {
            List<ChatMessageDataTemplate> tmpMsgs;
            if (loadedAllLocalMessages)
            {
                if (!mamRequested)
                {
                    tmpMsgs = await LoadMoreMamMessagesAsync();
                    HasMoreMessages = tmpMsgs.Count > 0;
                    return tmpMsgs;
                }
                else
                {
                    tmpMsgs = new List<ChatMessageDataTemplate>();
                }
            }
            else
            {
                tmpMsgs = LoadMoreLocalMessages();
                loadedAllLocalMessages = tmpMsgs.Count < MAX_MESSAGES_PER_REQUEST;
                if (tmpMsgs.Count <= 0)
                {
                    return await LoadMoreMessagesInTaskAsync();
                }
            }

            HasMoreMessages = (!loadedAllLocalMessages) || (chat.Chat.chatType == ChatType.MUC && !mamRequested);
            return tmpMsgs;
        }

        private async Task<List<ChatMessageDataTemplate>> LoadMoreMamMessagesAsync()
        {
            Logger.Info("Requesting MAM messages for \"" + chat.Chat.id + "\".");
            QueryFilter filter = GenerateMamQueryFilter();
            // For MUCs ask the MUC server and for everything else ask our own server for messages:
            string target = chat.Chat.chatType == ChatType.CHAT ? chat.Client.xmppClient.getXMPPAccount().getBareJid() : chat.Chat.bareJid;
            MessageResponseHelperResult<MamResult> result = await chat.Client.xmppClient.GENERAL_COMMAND_HELPER.requestMamAsync(filter, target);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                Logger.Info("Found " + result.RESULT.COUNT + " MAM messages for \"" + chat.Chat.id + "\".");
                Logger.Debug("MAM result: " + result.RESULT.ToString());

                mamRequested = result.RESULT.COMPLETE;

                List<ChatMessageDataTemplate> msgs = new List<ChatMessageDataTemplate>();
                using (MainDbContext ctx = new MainDbContext())
                {
                    foreach (QueryArchiveResultMessage msg in result.RESULT.RESULTS)
                    {
                        ChatMessageDataTemplate tmp = new ChatMessageDataTemplate(new ChatMessageModel(msg.MESSAGE, chat.Chat), chat.Chat);
                        ctx.Add(tmp.Message);
                        msgs.Add(tmp);
                    }
                }
                return msgs;
            }
            else
            {
                Logger.Error("Failed to load more MAM messages for \"" + chat.Chat.id + "\". Failed with: " + result.STATE);
                return new List<ChatMessageDataTemplate>();
            }
        }

        private List<ChatMessageDataTemplate> LoadMoreLocalMessages()
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                return ctx.GetNextNChatMessages(chat.Chat, GetLastMessage()?.Message, MAX_MESSAGES_PER_REQUEST).Select(msg => new ChatMessageDataTemplate(msg, chat.Chat)).ToList();
            }
        }

        private QueryFilter GenerateMamQueryFilter()
        {
            QueryFilter filter = new QueryFilter();
            string lastStableId = GetLastMessageStableId();
            if (Count > 0 && !string.IsNullOrEmpty(lastStableId))
            {
                if (chat.Client.xmppClient.connection.DISCO_HELPER.HasFeature(Consts.XML_XEP_0313_EXTENDED_NAMESPACE, chat.Client.dbAccount.bareJid))
                {
                    // Only extended MAM supports setting the 'after-id' property.
                    // Reference: https://xmpp.org/extensions/xep-0313.html#support
                    filter.BeforeId(lastStableId);
                }
                else
                {
                    filter.End(this[0].Message.date);
                }
            }
            filter.With(chat.Chat.bareJid);
            return filter;
        }

        private void UpdateUnreadCount()
        {
            if (chat is null)
            {
                UnreadCount = 0;
                return;
            }
            using (MainDbContext ctx = new MainDbContext())
            {
                UnreadCount = ctx.GetUnreadMessageCount(chat.Chat.id);
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
