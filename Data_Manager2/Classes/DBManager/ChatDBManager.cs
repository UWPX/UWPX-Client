using Data_Manager2.Classes.Events;
using Data_Manager2.Classes.DBTables;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMPP_API.Classes;
using Thread_Save_Components.Classes.SQLite;

namespace Data_Manager2.Classes.DBManager
{
    public class ChatDBManager : AbstractDBManager
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
                    ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(msg));
                }
            }
        }

        public void setChatTableValue(string id, object IdValue, string name, object value)
        {
            dB.Execute("UPDATE " + DBTableConsts.CHAT_TABLE + " SET " + name + "= ? WHERE " + id + "= ?", value, IdValue);
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
            update(invite);
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
                update(chat);
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

        public List<ChatTable> getAllChatsForClient(string userAccountId, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return dB.Query<ChatTable>(true, "SELECT * FROM " + DBTableConsts.CHAT_TABLE + " WHERE userAccountId = ?;", userAccountId);
            }
            else
            {
                SQLiteCommand cmd = dB.CreateCommand("SELECT * FROM " + DBTableConsts.CHAT_TABLE
                + " WHERE userAccountId = @USER_ACCOUNT_ID AND chatJabberId LIKE @FILTER;");
                cmd.Bind("@USER_ACCOUNT_ID", userAccountId);
                cmd.Bind("@FILTER", '%' + filter + '%');
                return dB.ExecuteCommand<ChatTable>(true, cmd);
            }
        }

        public void setAllNotInRoster(string userAccountId)
        {
            Parallel.ForEach(getAllChatsForClient(userAccountId, null), (c) =>
            {
                c.inRoster = false;
                update(c);
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
                ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(msg));
            });
        }

        public void deleteAllChatMessagesForChat(string chatId)
        {
            dB.Execute("DELETE FROM " + DBTableConsts.CHAT_MESSAGE_TABLE + " WHERE chatId = ?;", chatId);
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
            update(msg);
            msg.onChanged();
            ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(msg));
        }

        public void setChatMessage(ChatMessageTable message, bool triggerNewChatMessage, bool triggerMessageChanged)
        {
            update(message);
            if (triggerNewChatMessage)
            {
                NewChatMessage?.Invoke(this, new NewChatMessageEventArgs(message));
                if (message.isImage)
                {
                    cacheImage(message);
                }
            }
            if (triggerMessageChanged)
            {
                ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(message));
            }
        }

        public void resetPresence(string userAccountId)
        {
            foreach (ChatTable c in getAllChatsForClient(userAccountId, null))
            {
                if (c.chatType == ChatType.CHAT)
                {
                    c.presence = Presence.Unavailable;
                    update(c);
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
            ImageDBManager.INSTANCE.downloadImage(msg);
        }

        private void resetPresences()
        {
            dB.Execute("UPDATE " + DBTableConsts.CHAT_TABLE + " SET presence = ? WHERE chatType = ?;", Presence.Unavailable, ChatType.CHAT);
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<ChatTable>();
            dB.CreateTable<ChatMessageTable>();
            dB.CreateTable<MUCDirectInvitationTable>();
        }

        protected override void dropTables()
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
