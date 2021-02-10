using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Shared.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Account;
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
        public readonly ChatMessageCacheList CHAT_MESSAGES = new ChatMessageCacheList();

        private ChatDataTemplate _SelectedChat;
        /// <summary>
        /// The currently selected chat displaying all chat messages in <see cref="CHAT_MESSAGES"/> for.
        /// </summary>
        public ChatDataTemplate SelectedChat
        {
            get => _SelectedChat;
            set => SetSelectedChatProperty(value);
        }

        private readonly SemaphoreSlim CHATS_SEMA = new SemaphoreSlim(1);
        private readonly SemaphoreSlim CHATS_MESSAGES_SEMA = new SemaphoreSlim(1);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public void SetSelectedChatProperty(ChatDataTemplate value)
        {
            if (SetProperty(ref _SelectedChat, value, nameof(SelectedChat)))
            {
                CHATS_MESSAGES_SEMA.Wait();
                CHAT_MESSAGES.SetChat(value);
                CHATS_MESSAGES_SEMA.Release();
            }
        }

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
                return ctx.Chats.Where(c => c.id == chatId).Include(ctx.GetIncludePaths(typeof(ChatModel))).FirstOrDefault();
            }
        }

        public ChatMessageModel GetChatMessage(int chatId, string stableId)
        {
            if (initialized)
            {
                CHATS_SEMA.Wait();
                if (!(SelectedChat is null) && SelectedChat.Chat.id == chatId)
                {
                    ChatMessageModel msg;
                    CHATS_MESSAGES_SEMA.Wait();
                    msg = CHAT_MESSAGES.Where(m => string.Equals(m.Message.stableId, stableId)).FirstOrDefault().Message;
                    CHATS_MESSAGES_SEMA.Release();
                    // In case the message is null try loading it from the DB since it might not be shown currently:
                    if (!(msg is null))
                    {
                        CHATS_SEMA.Release();
                        return msg;
                    }
                }
                CHATS_SEMA.Release();
            }

            using (MainDbContext ctx = new MainDbContext())
            {
                return ctx.ChatMessages.Where(m => m.chatId == chatId && string.Equals(m.stableId, stableId)).Include(ctx.GetIncludePaths(typeof(ChatMessageModel))).FirstOrDefault();
            }
        }

        public ChatMessageModel GetChatMessage(int msgDbId)
        {
            if (initialized)
            {
                CHATS_SEMA.Wait();
                if (!(SelectedChat is null))
                {
                    ChatMessageModel msg;
                    CHATS_MESSAGES_SEMA.Wait();
                    msg = CHAT_MESSAGES.Where(m => m.Message.id == msgDbId).FirstOrDefault().Message;
                    CHATS_MESSAGES_SEMA.Release();
                    // In case the message is null try loading it from the DB since it might not be shown currently:
                    if (!(msg is null))
                    {
                        CHATS_SEMA.Release();
                        return msg;
                    }
                }
                CHATS_SEMA.Release();
            }

            using (MainDbContext ctx = new MainDbContext())
            {
                return ctx.ChatMessages.Where(m => m.id == msgDbId).Include(ctx.GetIncludePaths(typeof(ChatMessageModel))).FirstOrDefault();
            }
        }

        public IEnumerable<MucInfoModel> GetMucs(string accountBareJid)
        {
            if (initialized)
            {
                CHATS_SEMA.Wait();
                IEnumerable<MucInfoModel> mucs = CHATS.Where(c => string.Equals(c.Chat.bareJid, accountBareJid)).Select(c => c.Chat.muc);
                CHATS_SEMA.Release();
                return mucs;
            }

            using (MainDbContext ctx = new MainDbContext())
            {
                return ctx.Chats.Where(c => c.chatType == ChatType.MUC && string.Equals(c.accountBareJid, accountBareJid)).Select(c => c.muc).Include(ctx.GetIncludePaths(typeof(MucInfoModel)));
            }
        }

        public IEnumerable<ChatModel> GetChats(string accountBareJid)
        {
            if (initialized)
            {
                CHATS_SEMA.Wait();
                IEnumerable<ChatModel> chats = CHATS.Where(c => string.Equals(c.Chat.bareJid, accountBareJid)).Select(c => c.Chat);
                CHATS_SEMA.Release();
                return chats;
            }

            using (MainDbContext ctx = new MainDbContext())
            {
                return ctx.Chats.Where(c => c.chatType == ChatType.MUC && string.Equals(c.accountBareJid, accountBareJid)).Include(ctx.GetIncludePaths(typeof(ChatModel)));
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
                return ctx.Chats.Where(c => string.Equals(c.accountBareJid, accountBareJid) && string.Equals(c.bareJid, chatBareJid)).Include(ctx.GetIncludePaths(typeof(ChatModel))).FirstOrDefault();
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
                    foreach (ChatMessageDataTemplate msg in CHAT_MESSAGES)
                    {
                        if (msg.Message.state == MessageState.UNREAD)
                        {
                            msg.Message.state = MessageState.READ;
                        }
                    }
                    CHATS_MESSAGES_SEMA.Release();
                }
                CHATS_SEMA.Release();
            }

            // Update the DB:
            using (MainDbContext ctx = new MainDbContext())
            {
                foreach (ChatMessageModel msg in ctx.ChatMessages.Where(msg => msg.chatId == chatId && msg.state == MessageState.UNREAD))
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
                    foreach (ChatMessageDataTemplate msg in CHAT_MESSAGES.Where(msg => msg.Message.id == msgId && msg.Message.state == MessageState.UNREAD))
                    {
                        msg.Message.state = MessageState.READ;
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
                    CHAT_MESSAGES.Add(new ChatMessageDataTemplate(msg, chat));
                    CHATS_MESSAGES_SEMA.Release();
                }
                CHATS_SEMA.Release();
            }

            // Update the DB:
            using (MainDbContext ctx = new MainDbContext())
            {
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
                    ctx.Remove(ctx.ChatMessages.Where(msg => msg.chatId == chat.id));
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

        public void RemoveChatMessage(ChatMessageDataTemplate msg)
        {
            // Update the cache:
            if (initialized)
            {
                CHATS_SEMA.Wait();
                if (!(SelectedChat is null) && SelectedChat.Chat.id == msg.Chat.id)
                {
                    CHATS_MESSAGES_SEMA.Wait();
                    CHAT_MESSAGES.Remove(msg);
                    CHATS_MESSAGES_SEMA.Release();
                }
                CHATS_SEMA.Release();
            }

            // Update the DB:
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Remove(msg.Message);
            }
        }

        public void AddChat(ChatModel chat, Client client)
        {
            // Update the DB:
            using (MainDbContext ctx = new MainDbContext())
            {
                if (!(chat.muc is null))
                {
                    MucInfoModel muc = chat.muc;
                    chat.muc = null;
                    ctx.Add(muc);
                    //ctx.SaveChanges();
                    ctx.SaveChanges();
                    chat.muc = muc;
                    ctx.Update(chat);
                }
                else
                {
                    ctx.Add(chat);
                }
            }

            // Update the cache:
            if (initialized)
            {
                CHATS_SEMA.Wait();
                CHATS.Add(new ChatDataTemplate(chat, client, null, null));
                CHATS_SEMA.Release();
            }
        }

        public async Task DeleteAccountAsync(AccountModel account)
        {
            await Task.Run(async () =>
            {
                CHATS_SEMA.Wait();
                CHATS_MESSAGES_SEMA.Wait();
                try
                {
                    Logger.Info($"Deleting account '{account.bareJid}'...");
                    Logger.Info($"Making sure the account '{account.bareJid}' is disconnected before removing...");
                    try
                    {
                        await ConnectionHandler.INSTANCE.RemoveAccountAsync(account.bareJid);
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Failed to disconnect account '{account.bareJid}' for deletion.", e);
                        return;
                    }
                    Logger.Info($"Account '{account.bareJid}' disconnected.");

                    using (MainDbContext ctx = new MainDbContext())
                    {
                        Logger.Info($"Deleting chats and chat messages for '{account.bareJid}'...");
                        foreach (ChatModel chat in ctx.Chats.Where(c => string.Equals(c.accountBareJid, account.bareJid)))
                        {
                            ctx.RemoveRange(ctx.ChatMessages.Where(c => c.chatId == chat.id));
                            ctx.Remove(chat);
                        }
                        Logger.Info($"Chats and chat messages for '{account.bareJid}' deleted.");
                        ctx.Remove(account);
                    }
                    Logger.Info($"Account '{account.bareJid}' deleted.");
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to delete account '{account.bareJid}'.", e);
                }
                CHATS_MESSAGES_SEMA.Release();
                CHATS_SEMA.Release();
                await LoadChatsAsync();
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadChatsAsync()
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                await CHATS_SEMA.WaitAsync();
                CHATS.Clear();
                SelectedChat = null;
                IEnumerable<ChatModel> chats = ctx.Chats.Include(ctx.GetIncludePaths(typeof(ChatModel)));
                CHATS.AddRange(chats.Select(c => LoadChat(c, ctx)), true);
                CHATS_SEMA.Release();
            }
        }

        private static ChatDataTemplate LoadChat(ChatModel chat, MainDbContext ctx)
        {
            Client client = ConnectionHandler.INSTANCE.GetClient(chat.accountBareJid);
            ChatMessageModel lastMsg = ctx.ChatMessages.Where(m => m.chatId == chat.id).OrderByDescending(m => m.date).FirstOrDefault();
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
