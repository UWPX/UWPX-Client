using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Shared.Classes;
using Shared.Classes.Collections;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;

namespace Manager.Classes.Chat
{
    public class DataCache: AbstractDataTemplate
    {
        //--------------------------------------------------------Instance:-----------------------------------------------------------------\\
        #region --Instance--
        /// <summary>
        /// Represents an instance of the <see cref="DataCache"/> aiming to keep the UI and Db in sync.
        /// </summary>
        public static readonly DataCache INSTANCE = new DataCache();
        #endregion
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// True in case a valid cache with all chats is stored in <see cref="CHATS"/>.
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// A collection of all chats that should be displayed.
        /// </summary>
        public readonly SaveObservableChatDictionaryList CHATS = new SaveObservableChatDictionaryList();

        /// <summary>
        /// All currently displayed chat messages.
        /// </summary>
        public readonly CustomObservableCollection<ChatMessageModel> CHAT_MESSAGES = new CustomObservableCollection<ChatMessageModel>(false);

        private ChatDataTemplate _SelectedChat;
        /// <summary>
        /// The currently selected chat displaying all chat messages in <see cref="CHAT_MESSAGES"/> for.
        /// </summary>
        public ChatDataTemplate SelectedChat
        {
            get => _SelectedChat;
            set => SetProperty(ref _SelectedChat, value);
        }

        private readonly SemaphoreSlim CHATS_SEMA = new SemaphoreSlim(1);
        private readonly SemaphoreSlim CHATS_MESSAGES_SEMA = new SemaphoreSlim(1);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public ChatModel GetChat(int chatId)
        {
            if (initialized)
            {
                CHATS_SEMA.Wait();
                ChatModel chat = null;
                if (CHATS.Contains(chatId))
                {
                    chat = CHATS[chatId].Chat;
                }
                CHATS_SEMA.Release();
                return chat;
            }

            using (MainDbContext ctx = new MainDbContext())
            {
                return ctx.Chats.Where(c => c.id == chatId).FirstOrDefault();
            }
        }

        public ChatModel GetChat(string accountBareJid, string chatBareJid)
        {
            if (initialized)
            {
                CHATS_SEMA.Wait();
                ChatModel chat = CHATS.Where(c => string.Equals(c.Chat.accountBareJid, accountBareJid) && string.Equals(c.Chat.bareJid, chatBareJid)).FirstOrDefault()?.Chat;
                CHATS_SEMA.Release();
                return chat;
            }

            using (MainDbContext ctx = new MainDbContext())
            {
                return ctx.Chats.Where(c => string.Equals(c.accountBareJid, accountBareJid) && string.Equals(c.bareJid, chatBareJid)).FirstOrDefault();
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task InitAsync()
        {
            Debug.Assert(!initialized);
            initialized = true;
            await LoadChatsAsync();
        }

        public void MarkAllChatMessagesAsRead(int chatId)
        {
            // Update the cache:
            if (initialized)
            {
                CHATS_SEMA.Wait();
                if (!(SelectedChat is null) && SelectedChat.Chat.id == chatId)
                {
                    CHATS_MESSAGES_SEMA.Wait();
                    foreach (ChatMessageModel msg in CHAT_MESSAGES)
                    {
                        if (msg.state == MessageState.UNREAD)
                        {
                            msg.state = MessageState.READ;
                        }
                    }
                    CHATS_MESSAGES_SEMA.Release();
                }
                CHATS_SEMA.Release();
            }

            // Update the DB:
            using (MainDbContext ctx = new MainDbContext())
            {
                foreach (ChatMessageModel msg in ctx.ChatMessages.Where(msg => msg.chat.id == chatId && msg.state == MessageState.UNREAD))
                {
                    msg.state = MessageState.READ;
                    ctx.Update(msg);
                }
            }
        }

        public void MarkChatMessageAsRead(int chatId, int msgId)
        {
            // Update the cache:
            if (initialized)
            {
                CHATS_SEMA.Wait();
                if (!(SelectedChat is null) && SelectedChat.Chat.id == chatId)
                {
                    CHATS_MESSAGES_SEMA.Wait();
                    foreach (ChatMessageModel msg in CHAT_MESSAGES.Where(msg => msg.id == msgId && msg.state == MessageState.UNREAD))
                    {
                        msg.state = MessageState.READ;
                    }
                    CHATS_MESSAGES_SEMA.Release();
                }
                CHATS_SEMA.Release();
            }

            // Update the DB:
            using (MainDbContext ctx = new MainDbContext())
            {
                foreach (ChatMessageModel msg in ctx.ChatMessages.Where(msg => msg.id == msgId && msg.state == MessageState.UNREAD))
                {
                    msg.state = MessageState.READ;
                    ctx.Update(msg);
                }
            }
        }

        /// <summary>
        /// Adds the given new chat message  to the DB and stores an updated version of the given chat in the DB.
        /// </summary>
        public void AddChatMessage(ChatMessageModel msg, ChatModel chat)
        {
            // Update the cache:
            if (initialized)
            {
                CHATS_SEMA.Wait();
                if (!(SelectedChat is null) && SelectedChat.Chat.id == chat.id)
                {
                    CHATS_MESSAGES_SEMA.Wait();
                    CHAT_MESSAGES.Add(msg);
                    CHATS_MESSAGES_SEMA.Release();
                }
                CHATS_SEMA.Release();
            }

            // Update the DB:
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Add(msg);
                ctx.Update(chat);
            }
        }

        public void DeleteChat(ChatModel chat, bool keepChatMessages, bool removeFromRoster)
        {
            // Update the cache:
            if (initialized)
            {
                CHATS_SEMA.Wait();
                if (!(SelectedChat is null) && SelectedChat.Chat.id == chat.id)
                {
                    SelectedChat = null;
                }
                CHATS.RemoveId(chat.id);
                CHATS_SEMA.Release();
            }

            // Update the DB:
            using (MainDbContext ctx = new MainDbContext())
            {
                if (!keepChatMessages)
                {
                    ctx.Remove(ctx.ChatMessages.Where(msg => msg.chat.id == chat.id));
                    Logger.Info("Deleted chat messages for: " + chat.bareJid);
                }

                if (chat.chatType == ChatType.MUC || removeFromRoster)
                {
                    ctx.Remove(chat);
                    Logger.Info("Deleted chat: " + chat.bareJid);
                }
                else
                {
                    chat.isChatActive = false;
                    chat.Save();
                    Logger.Info("Marked chat as not active: " + chat.bareJid);
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadChatsAsync()
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                await CHATS_SEMA.WaitAsync();
                CHATS.Clear();
                CHATS.AddRange(ctx.Chats.Select(c => LoadChat(c, ctx)), true);
                CHATS_SEMA.Release();
            }
        }

        private ChatDataTemplate LoadChat(ChatModel chat, MainDbContext ctx)
        {
            Client client = ConnectionHandler.INSTANCE.GetClient(chat.accountBareJid);
            ChatMessageModel lastMsg = ctx.ChatMessages.Where(m => m.chat.id == chat.id).OrderByDescending(m => m.date).FirstOrDefault();
            return new ChatDataTemplate(chat, client, lastMsg, null);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
