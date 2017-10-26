using Data_Manager.Classes.DBEntries;
using Data_Manager.Classes.Events;
using System;
using System.Collections.Generic;
using XMPP_API.Classes;

namespace Data_Manager.Classes.Managers
{
    public class ChatManager : AbstractManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly ChatManager INSTANCE = new ChatManager();

        public delegate void ChatChangedHandler(ChatManager handler, ChatChangedEventArgs args);
        public delegate void NewChatMessageHandler(ChatManager handler, NewChatMessageEventArgs args);

        public event ChatChangedHandler ChatChanged;
        public event NewChatMessageHandler NewChatMessage;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatManager()
        {
            //dropTables();
            //createTables();
            resetPresence();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool doesChatExist(XMPPClient client, string chatId)
        {
            List<ChatEntry> chats = dB.Query<ChatEntry>("SELECT * FROM ChatEntry WHERE userAccountId LIKE ? AND id LIKE ?", client.getSeverConnectionConfiguration().getIdAndDomain(), chatId);
            return chats.Count > 0;
        }

        public IList<ChatMessageEntry> getAllChatMessagesForChat(ChatEntry chat)
        {
            return dB.Query<ChatMessageEntry>("SELECT * FROM ChatMessageEntry WHERE chatId LIKE ? ORDER BY date ASC", chat.id + '%');
        }

        public string getLastChatMessageForChat(ChatEntry chat)
        {
            IList<ChatMessageEntry> list = getAllChatMessagesForChat(chat);
            if (list.Count <= 0)
            {
                return "";
            }
            return list[list.Count - 1].message;
        }

        public ChatEntry getChatEntry(string id, string userAccountId)
        {
            IList<ChatEntry> list = dB.Query<ChatEntry>("SELECT * FROM ChatEntry WHERE id LIKE ? AND userAccountId LIKE ?", id, userAccountId);
            if (list.Count < 1)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        public void setChatMessageEntry(ChatMessageEntry message, bool triggerNewChatMessage)
        {
            dB.InsertOrReplace(message);
            if (triggerNewChatMessage)
            {
                NewChatMessage?.Invoke(this, new NewChatMessageEventArgs(message));
            }
        }

        public void setChatEntry(ChatEntry chat, bool triggerChatChanged)
        {
            dB.InsertOrReplace(chat);
            if (triggerChatChanged)
            {
                onChatChanged(chat, false);
            }
        }

        public void setLastActivity(string chatId, DateTime date)
        {
            foreach (ChatEntry c in dB.Query<ChatEntry>("SELECT * FROM ChatEntry WHERE id LIKE ?", chatId))
            {
                if(c.lastActive == null || c.lastActive.CompareTo(date) < 0)
                {
                    c.lastActive = date;
                    setChatEntry(c, true);
                }
            }
        }

        public List<ChatEntry> getAllChatsForClient(XMPPClient c)
        {
            return dB.Query<ChatEntry>("SELECT * FROM ChatEntry WHERE userAccountId LIKE ?", c.getSeverConnectionConfiguration().getIdAndDomain());
        }

        public void setAllNotInRoster(string userAccountId)
        {
            dB.Query<ChatEntry>("UPDATE ChatEntry SET inRoster = 0 WHERE userAccountId LIKE ?", userAccountId);
            foreach (ChatEntry c in dB.Query<ChatEntry>("SELECT * FROM ChatEntry WHERE userAccountId LIKE ?", userAccountId))
            {
                onChatChanged(c, false);
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void removeChatEntry(ChatEntry chat)
        {
            dB.Delete(chat);
            onChatChanged(chat, true);
        }

        public void deleteChat(ChatEntry chat, bool deleteMessages)
        {
            dB.Query<UserAccountEntry>("DELETE FROM ChatMessageEntry WHERE chatId LIKE ?", chat.id);
            removeChatEntry(chat);

        }

        #endregion

        #region --Misc Methods (Private)--
        private void onChatChanged(ChatEntry chat, bool removed)
        {
            if(chat == null)
            {
                return;
            }
            ChatChanged?.Invoke(this, new ChatChangedEventArgs(chat, removed));
        }

        private void resetPresence()
        {
            dB.Execute("UPDATE ChatEntry SET presence = 0;");
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<ChatMessageEntry>();
            dB.CreateTable<ChatEntry>();
        }

        protected override void dropTables()
        {
            dB.DropTable<ChatMessageEntry>();
            dB.DropTable<ChatEntry>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
