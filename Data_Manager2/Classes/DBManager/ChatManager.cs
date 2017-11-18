using Data_Manager.Classes;
using Data_Manager.Classes.Events;
using Data_Manager2.Classes.DBTables;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMPP_API.Classes;

namespace Data_Manager2.Classes.DBManager
{
    public class ChatManager : AbstractManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ChatManager INSTANCE = new ChatManager();

        public delegate void NewChatMessageHandler(ChatManager handler, NewChatMessageEventArgs args);
        public delegate void ChatChangedHandler(ChatManager handler, ChatChangedEventArgs args);

        public event NewChatMessageHandler NewChatMessage;
        public event ChatChangedHandler ChatChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/11/2017 Created [Fabian Sauter]
        /// </history>
        public ChatManager()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool doesChatExist(string id)
        {
            return getChat(id) != null;
        }

        public IList<ChatMessageTable> getAllChatMessagesForChat(ChatTable chat)
        {
            return dB.Query<ChatMessageTable>("SELECT * FROM ChatMessageTable WHERE chatId LIKE ? ORDER BY date ASC;", chat.id);
        }

        public ChatTable getChat(string id)
        {
            IList<ChatTable> list = dB.Query<ChatTable>("SELECT * FROM ChatTable WHERE id LIKE ?;", id);
            if (list.Count < 1)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        public void setChat(ChatTable chat, bool delete, bool triggerChatChanged)
        {
            if (chat != null)
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
                    ChatChanged?.Invoke(this, new ChatChangedEventArgs(chat, delete));
                }
            }
        }

        private List<ChatMessageTable> getAllUnreadMessages(ChatTable chat)
        {
            return dB.Query<ChatMessageTable>("SELECT * FROM ChatTable WHERE state == ? AND fromUser NOT LIKE ?;", MessageState.UNREAD, chat.userAccountId);
        }

        public List<ChatTable> getAllChatsForClient(string userAccountId)
        {
            return dB.Query<ChatTable>("SELECT * FROM ChatTable WHERE userAccountId LIKE ?;", userAccountId);
        }

        public void setAllNotInRoster(string userAccountId)
        {
            Parallel.ForEach(getAllChatsForClient(userAccountId), (c) =>
            {
                c.inRoster = false;
                update(c);
                ChatChanged?.Invoke(this, new ChatChangedEventArgs(c, false));
            });
        }

        public ChatMessageTable getLastChatMessageForChat(ChatTable chat)
        {
            IList<ChatMessageTable> list = getAllChatMessagesForChat(chat);
            if (list.Count <= 0)
            {
                return null;
            }
            return list[list.Count - 1];
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void deleteAllChatMessagesForAccount(ChatTable chat)
        {
            dB.Execute("DELETE FROM ChatMessageTable WHERE chatId LIKE ?;", chat.id);
        }

        public void markAllMessagesAsRead(ChatTable chat)
        {
            Parallel.ForEach(getAllUnreadMessages(chat), (msg) =>
            {
                msg.state = MessageState.READ;
                msg.onChanged();
            });
        }

        public void setChatMessageEntry(ChatMessageTable message, bool triggerNewChatMessage)
        {
            update(message);
            if (triggerNewChatMessage)
            {
                NewChatMessage?.Invoke(this, new NewChatMessageEventArgs(message));
            }
        }

        public void resetPresence(string userAccountId)
        {
            Parallel.ForEach(getAllChatsForClient(userAccountId), (c) =>
            {
                c.presence = Presence.Unavailable;
                update(c);
                ChatChanged?.Invoke(this, new ChatChangedEventArgs(c, false));
            });
        }


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<ChatTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<ChatTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
