using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.Events;
using Shared.Classes.SQLite;
using SQLite;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0249;

namespace Data_Manager2.Classes.DBManager
{
    public class ChatDBManager: AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ChatDBManager INSTANCE = new ChatDBManager();

        public delegate void NewChatMessageHandler(ChatDBManager handler, NewChatMessageEventArgs args);
        public delegate void ChatChangedHandler(ChatDBManager handler, ChatChangedEventArgs args);
        public delegate void ChatMessageChangedHandler(ChatDBManager handler, ChatMessageChangedEventArgs args);

        public event NewChatMessageHandler NewChatMessage;
        public event ChatChangedHandler ChatChanged;
        public event ChatMessageChangedHandler ChatMessageChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/11/2017 Created [Fabian Sauter]
        /// </history>
        public ChatDBManager()
        {
            resetPresences();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void setMessageAsDeliverd(string id, bool triggerMessageChanged)
        {
            dB.Execute("UPDATE " + DBTableConsts.CHAT_MESSAGE_TABLE + " SET state = ? WHERE id = ?", MessageState.DELIVERED, id);
            if (triggerMessageChanged)
            {
                ChatMessageTable msg = getChatMessageById(id);
                if (msg != null)
                {
                    ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(msg, false));
                }
            }
        }

        public void setChatTableValue(string id, object idValue, string name, object value)
        {
            dB.Execute("UPDATE " + DBTableConsts.CHAT_TABLE + " SET " + name + "= ? WHERE " + id + "= ?", value, idValue);
        }

        public bool doesChatExist(string id)
        {
            return getChat(id) != null;
        }

        public IList<ChatMessageTable> getAllChatMessagesForChat(string chatId)
        {
            return dB.Query<ChatMessageTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_MESSAGE_TABLE + " WHERE chatId = ? ORDER BY date ASC;", chatId);
        }

        public ChatTable getChat(string chatId)
        {
            IList<ChatTable> list = dB.Query<ChatTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_TABLE + " WHERE id = ?;", chatId);
            if (list.Count < 1)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        public void setMUCDirectInvitation(MUCDirectInvitationTable invite)
        {
            dB.InsertOrReplace(invite);
        }

        public void setMUCDirectInvitationState(string chatMessageId, MUCDirectInvitationState state)
        {
            dB.Execute("UPDATE " + DBTableConsts.MUC_DIRECT_INVITATION_TABLE + " SET state = ? WHERE chatMessageId = ?;", state, chatMessageId);
        }

        public MUCDirectInvitationTable getMUCDirectInvitation(string chatMessageId)
        {
            IList<MUCDirectInvitationTable> list = dB.Query<MUCDirectInvitationTable>(true, "SELECT * FROM " + DBTableConsts.MUC_DIRECT_INVITATION_TABLE + " WHERE chatMessageId = ?;", chatMessageId);
            if (list.Count < 1)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        public List<ChatTable> getAllMUCs(string userAccountId)
        {
            return dB.Query<ChatTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_TABLE + " WHERE userAccountId = ? AND chatType = ?;", userAccountId, ChatType.MUC);
        }

        public bool doesMUCExist(string id)
        {
            IList<ChatTable> list = dB.Query<ChatTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_TABLE + " WHERE id = ? AND chatType = ?;", id, ChatType.MUC);
            return list.Count > 0;
        }

        public bool doesOutstandingMUCInviteExist(string chatId, string roomJid)
        {
            IList<ChatMessageTable> messages = dB.Query<ChatMessageTable>(true, "SELECT c.* "
                + "FROM " + DBTableConsts.CHAT_MESSAGE_TABLE + " c JOIN " + DBTableConsts.MUC_DIRECT_INVITATION_TABLE + " i ON c.id = i.chatMessageId "
                + "WHERE c.chatId = ? AND i.state = ?;", chatId, MUCDirectInvitationState.REQUESTED);
            return messages.Count > 0;
        }

        public void setChat(ChatTable chat, bool delete, bool triggerChatChanged)
        {
            if (delete)
            {
                dB.Delete(chat);
            }
            else
            {
                dB.InsertOrReplace(chat);
            }

            if (triggerChatChanged)
            {
                onChatChanged(chat, delete);
            }
        }

        private List<ChatMessageTable> getAllUnreadMessages(string chatId)
        {
            return dB.Query<ChatMessageTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_MESSAGE_TABLE + " WHERE chatId = ? AND state = ?;", chatId, MessageState.UNREAD);
        }

        public List<ChatTable> getAllChatsForClient(string userAccountId)
        {
            return dB.Query<ChatTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_TABLE + " WHERE userAccountId = ?;", userAccountId);
        }

        public List<ChatTable> getNotStartedChatsForClient(string userAccountId, ChatType chatType)
        {
            return dB.Query<ChatTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_TABLE + " WHERE userAccountId = ? AND isChatActive = ? AND chatType = ?;", userAccountId, false, chatType);
        }

        public void setAllNotInRoster(string userAccountId)
        {
            Parallel.ForEach(getAllChatsForClient(userAccountId), (c) =>
            {
                c.inRoster = false;
                dB.InsertOrReplace(c);
                onChatChanged(c, false);
            });
        }

        public ChatMessageTable getChatMessageById(string messageId)
        {
            List<ChatMessageTable> list = dB.Query<ChatMessageTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_MESSAGE_TABLE + " WHERE id = ?;", messageId);
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public ChatMessageTable getLastChatMessageForChat(string chatId)
        {
            IList<ChatMessageTable> list = getAllChatMessagesForChat(chatId);
            if (list.Count <= 0)
            {
                return null;
            }
            return list[list.Count - 1];
        }

        public int getUnreadCount(string chatId)
        {
            return dB.Query<ChatMessageTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_MESSAGE_TABLE + " WHERE chatId = ? AND state = ?;", chatId, MessageState.UNREAD).Count;
        }

        public int getUnreadCount()
        {
            return dB.Query<ChatMessageTable>(true, "SELECT m.* "
                + "FROM " + DBTableConsts.ACCOUNT_TABLE + " a JOIN " + DBTableConsts.CHAT_TABLE + " c ON a.id = c.userAccountId "
                + "JOIN " + DBTableConsts.CHAT_MESSAGE_TABLE + " m ON m.chatId = c.id "
                + "WHERE m.state = ?;", MessageState.UNREAD).Count;
        }

        /// <summary>
        /// Returns all chat messages for the given userAccountId and state ordered by their chatId.
        /// </summary>
        public IList<ChatMessageTable> getChatMessages(string userAccountId, MessageState state)
        {
            return dB.Query<ChatMessageTable>(true, "SELECT m.* "
                + "FROM " + DBTableConsts.CHAT_MESSAGE_TABLE + " m JOIN " + DBTableConsts.CHAT_TABLE + " c ON m.chatId = c.id "
                + "WHERE c.userAccountId = ? AND m.state = ?"
                + "ORDER BY m.chatId ASC;", userAccountId, state);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public List<ChatTable> findUsers(string text)
        {
            SQLiteCommand cmd = dB.CreateCommand("SELECT DISTINCT chatJabberId FROM " + DBTableConsts.CHAT_TABLE + " WHERE chatJabberId LIKE @TEXT AND chatType = @CHAT_TYPE;");
            cmd.Bind("@CHAT_TYPE", ChatType.CHAT);
            cmd.Bind("@TEXT", '%' + text + '%');
            return dB.ExecuteCommand<ChatTable>(true, cmd);
        }

        public void updateChatMessageState(string msgId, MessageState state)
        {
            dB.Execute("UPDATE " + DBTableConsts.CHAT_MESSAGE_TABLE + " SET state = ? WHERE id = ?", state, msgId);
            List<ChatMessageTable> list = dB.Query<ChatMessageTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_MESSAGE_TABLE + " WHERE id = ?;", msgId);
            Parallel.ForEach(list, (msg) =>
            {
                ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(msg, false));
            });
        }

        public async Task deleteAllChatMessagesForChatAsync(string chatId)
        {
            List<ChatMessageTable> list = dB.Query<ChatMessageTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_MESSAGE_TABLE + " WHERE chatId = ?;", chatId);
            foreach (ChatMessageTable msg in list)
            {
                await deleteChatMessageAsync(msg, false);
            }
        }

        public async Task deleteAllChatMessagesForAccountAsync(string userAccountId)
        {
            List<ChatMessageTable> list = dB.Query<ChatMessageTable>(true, "SELECT m.* FROM " + DBTableConsts.CHAT_MESSAGE_TABLE + " m JOIN " + DBTableConsts.CHAT_TABLE + " c ON m.chatId = c.id WHERE c.userAccountId = ?;", userAccountId);
            foreach (ChatMessageTable msg in list)
            {
                await deleteChatMessageAsync(msg, false);
            }
        }

        public void deleteAllChatsForAccount(string userAccountId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.CHAT_TABLE + " WHERE userAccountId = ?;", userAccountId);
        }

        public void markAllMessagesAsRead(string chatId)
        {
            List<ChatMessageTable> list = getAllUnreadMessages(chatId);
            if (list.Count > 0)
            {
                Parallel.ForEach(list, (msg) => markMessageAsRead(msg));
            }
        }

        public void markMessageAsRead(string id)
        {
            ChatMessageTable msg = getChatMessageById(id);
            if (msg != null)
            {
                markMessageAsRead(msg);
            }
        }

        public void markMessageAsRead(ChatMessageTable msg)
        {
            msg.state = MessageState.READ;
            dB.InsertOrReplace(msg);
            msg.onChanged();
            ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(msg, false));
        }

        public void setChatMessage(ChatMessageTable message, bool triggerNewChatMessage, bool triggerMessageChanged)
        {
            dB.InsertOrReplace(message);
            if (triggerNewChatMessage)
            {
                NewChatMessage?.Invoke(this, new NewChatMessageEventArgs(message));
                if (message.isImage && !Settings.getSettingBoolean(SettingsConsts.DISABLE_IMAGE_AUTO_DOWNLOAD))
                {
                    cacheImage(message);
                }
            }
            if (triggerMessageChanged)
            {
                ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(message, false));
            }
        }

        public async Task deleteChatMessageAsync(ChatMessageTable message, bool triggerMessageChanged)
        {
            if (message.isImage)
            {
                await ImageDBManager.INSTANCE.deleteImageAsync(message);
            }

            if (string.Equals(message.type, DirectMUCInvitationMessage.TYPE_MUC_DIRECT_INVITATION))
            {
                deleteMucDirectInvite(message);
            }

            dB.Delete(message);
            if (triggerMessageChanged)
            {
                ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(message, true));
            }
        }

        public void deleteMucDirectInvite(ChatMessageTable message)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.MUC_DIRECT_INVITATION_TABLE + " WHERE chatMessageId = ?;", message.id);
        }

        public void resetPresence(string userAccountId)
        {
            foreach (ChatTable c in getAllChatsForClient(userAccountId))
            {
                if (c.chatType == ChatType.CHAT)
                {
                    c.presence = Presence.Unavailable;
                    dB.InsertOrReplace(c);
                    onChatChanged(c, false);
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void onChatChanged(ChatTable chat, bool deleted)
        {
            if (chat != null)
            {
                ChatChanged?.Invoke(this, new ChatChangedEventArgs(chat, deleted));
            }
        }

        private void onChatChanged(string chatId)
        {
            onChatChanged(getChat(chatId), false);
        }

        private void cacheImage(ChatMessageTable msg)
        {
            if (!Settings.getSettingBoolean(SettingsConsts.DISABLE_IMAGE_AUTO_DOWNLOAD))
            {
                Task.Run(async () => await ConnectionHandler.INSTANCE.IMAGE_DOWNLOAD_HANDLER.DownloadImageAsync(msg));
            }
        }

        private void resetPresences()
        {
            dB.Execute("UPDATE " + DBTableConsts.CHAT_TABLE + " SET presence = ? WHERE chatType = ?;", Presence.Unavailable, ChatType.CHAT);
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void CreateTables()
        {
            dB.CreateTable<ChatTable>();
            dB.CreateTable<ChatMessageTable>();
            dB.CreateTable<MUCDirectInvitationTable>();
        }

        protected override void DropTables()
        {
            dB.DropTable<ChatTable>();
            dB.DropTable<ChatMessageTable>();
            dB.DropTable<MUCDirectInvitationTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
