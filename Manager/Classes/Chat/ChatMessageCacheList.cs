using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Manager.Classes.Toast;
using Shared.Classes.Collections;
using Shared.Classes.Network;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0059;
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

        private CancellationTokenSource loadMoreMessagesToken = null;
        private Task<List<ChatMessageDataTemplate>> loadMoreMessagesTask = null;
        private readonly SemaphoreSlim LOAD_MESSAGES_SEMA = new SemaphoreSlim(1);

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
        public async Task SetChatAsync(ChatDataTemplate chat)
        {
            if (this.chat == chat)
            {
                return;
            }
            this.chat = chat;
            await LoadChatMessagesAsync();
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
            if (!(loadMoreMessagesTask is null))
            {
                if (!(loadMoreMessagesToken is null) && !loadMoreMessagesToken.IsCancellationRequested)
                {
                    loadMoreMessagesToken.Cancel();
                }
                await loadMoreMessagesTask;
            }

            await LOAD_MESSAGES_SEMA.WaitAsync();
            IsLoading = true;
            loadMoreMessagesToken = new CancellationTokenSource();

            loadMoreMessagesTask = LoadMoreMessagesInTaskAsync();

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
            chat.UpdateUnreadCount();
            LOAD_MESSAGES_SEMA.Release();
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadChatMessagesAsync()
        {
            // Cancel all outstanding loading operations:
            if (!(loadMoreMessagesToken is null) && !loadMoreMessagesToken.IsCancellationRequested)
            {
                loadMoreMessagesToken.Cancel();
            }
            await LOAD_MESSAGES_SEMA.WaitAsync();

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

                // Check for downloads and update the images in case:
                foreach (ChatMessageDataTemplate msg in msgs)
                {
                    if (msg.Message.isImage)
                    {
                        AbstractDownloadableObject result = await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.FindAsync((i) => i.id == msg.Message.image.id);
                        if (!(result is null))
                        {
                            msg.Message.image = (ChatMessageImageModel)result;
                        }
                    }
                }

                AddRange(msgs);
                HasMoreMessages = true;
                loadedAllLocalMessages = false;
                mamRequested = false;
                chat.UpdateUnreadCount();
            }
            else
            {
                HasMoreMessages = false;
                loadedAllLocalMessages = true;
                mamRequested = true;
            }
            LOAD_MESSAGES_SEMA.Release();
            IsLoading = false;
        }

        private async Task<List<ChatMessageDataTemplate>> LoadMoreMessagesInTaskAsync()
        {
            List<ChatMessageDataTemplate> tmpMsgs;
            if (loadedAllLocalMessages)
            {
                if (!Settings.GetSettingBoolean(SettingsConsts.DISABLE_MAM) && !mamRequested)
                {
                    tmpMsgs = await LoadMoreMamMessagesAsync();
                    HasMoreMessages = tmpMsgs.Count > 0;
                    return tmpMsgs;
                }
                else
                {
                    HasMoreMessages = false;
                    mamRequested = true;
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
            Logger.Info("Requesting MAM messages for \"" + chat.Chat.bareJid + "\".");
            QueryFilter filter = GenerateMamQueryFilter();
            // For MUCs ask the MUC server and for everything else ask our own server for messages:
            string target = chat.Chat.chatType == ChatType.CHAT ? chat.Client.xmppClient.getXMPPAccount().getBareJid() : chat.Chat.bareJid;
            // Request only 30 messages at the time:
            Set rsm = new Set
            {
                max = 30
            };
            MessageResponseHelperResult<MamResult> result = await chat.Client.xmppClient.GENERAL_COMMAND_HELPER.requestMamAsync(filter, rsm, target);
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                Logger.Info("Found " + result.RESULT.COUNT + " MAM messages for \"" + chat.Chat.bareJid + "\".");
                Logger.Debug("MAM result: " + result.RESULT.ToString());

                mamRequested = result.RESULT.COMPLETE;

                List<ChatMessageDataTemplate> msgs = new List<ChatMessageDataTemplate>();
                if (result.RESULT.RESULTS.Count > 0)
                {
                    using (MainDbContext ctx = new MainDbContext())
                    {
                        foreach (QueryArchiveResultMessage message in result.RESULT.RESULTS)
                        {
                            foreach (AbstractMessage abstractMessage in message.CONTENT)
                            {
                                if (abstractMessage is MessageMessage msg)
                                {
                                    ChatMessageDataTemplate tmp = new ChatMessageDataTemplate(new ChatMessageModel(msg, chat.Chat), chat.Chat);

                                    // Set the image path and file name:
                                    if (tmp.Message.isImage)
                                    {
                                        await DataCache.PrepareImageModelPathAndNameAsync(tmp.Message.image);
                                    }

                                    ctx.Add(tmp.Message);
                                    msgs.Add(tmp);
                                }
                                else
                                {
                                    Logger.Warn("MAM contained message of type: " + abstractMessage.GetType());
                                }
                            }
                        }
                    }
                }
                return msgs;
            }
            else
            {
                Logger.Error("Failed to load more MAM messages for \"" + chat.Chat.bareJid + "\". Failed with: " + result.STATE);
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

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
