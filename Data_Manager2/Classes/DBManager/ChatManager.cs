﻿using Data_Manager.Classes;
using Data_Manager.Classes.Events;
using Data_Manager2.Classes.DBTables;
using System;
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
        public delegate void ChatMessageChangedHandler(ChatManager handler, ChatMessageChangedEventArgs args);

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
        public ChatManager()
        {
            resetPresences();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool doesChatExist(string id)
        {
            return getChat(id) != null;
        }

        public MUCChatInfoTable getMUCInfo(ChatTable chat)
        {
            IList<MUCChatInfoTable> list = dB.Query<MUCChatInfoTable>("SELECT * FROM MUCChatInfoTable WHERE chatId = ?;", chat.id);
            if(list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public IList<ChatMessageTable> getAllChatMessagesForChat(ChatTable chat)
        {
            return dB.Query<ChatMessageTable>("SELECT * FROM ChatMessageTable WHERE chatId = ? ORDER BY date ASC;", chat.id);
        }

        public ChatTable getChat(string id)
        {
            IList<ChatTable> list = dB.Query<ChatTable>("SELECT * FROM ChatTable WHERE id = ? AND id = (chatJabberId || userAccountId);", id);
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

        public void setMUCChatInfo(MUCChatInfoTable info, bool delete, bool triggerMUCChanged)
        {
            if(info != null)
            {
                if (delete)
                {
                    dB.Delete(info);
                }
                else
                {
                    update(info);
                }
                if (triggerMUCChanged)
                {
                    // TODO add MUC changed event
                }
            }
        }

        private List<ChatMessageTable> getAllUnreadMessages(ChatTable chat)
        {
            return dB.Query<ChatMessageTable>("SELECT * FROM ChatMessageTable WHERE chatId = ? AND state = ? AND fromUser != ?;", chat.id, MessageState.UNREAD, chat.userAccountId);
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

        public ChatMessageTable getChatMessageById(string messageId)
        {
            List<ChatMessageTable> list = dB.Query<ChatMessageTable>("SELECT * FROM ChatMessageTable WHERE id = ?;", messageId);
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
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
        public void updateChatMessageState(string msgId, MessageState state)
        {
            dB.Execute("UPDATE ChatMessageTable SET state = ? WHERE id = ?", state, msgId);
            List<ChatMessageTable> list = dB.Query<ChatMessageTable>("SELECT * FROM ChatMessageTable WHERE id = ?;", msgId);
            Parallel.ForEach(list, (msg) =>
            {
                ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(msg));
            });
        }

        public void deleteAllChatMessagesForAccount(ChatTable chat)
        {
            dB.Execute("DELETE FROM ChatMessageTable WHERE chatId = ?;", chat.id);
        }

        public void markAllMessagesAsRead(ChatTable chat)
        {
            List<ChatMessageTable> list = getAllUnreadMessages(chat);
            if (list.Count > 0)
            {
                Parallel.ForEach(list, (msg) =>
                {
                    msg.state = MessageState.READ;
                    update(msg);
                    msg.onChanged();
                    ChatMessageChanged?.Invoke(this, new ChatMessageChangedEventArgs(msg));
                });
            }
        }

        public void setChatMessageEntry(ChatMessageTable message, bool triggerNewChatMessage, bool triggerMessageChanged)
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
            Parallel.ForEach(getAllChatsForClient(userAccountId), (c) =>
            {
                if(c.chatType == ChatType.CHAT)
                {
                    c.presence = Presence.Unavailable;
                    update(c);
                    ChatChanged?.Invoke(this, new ChatChangedEventArgs(c, false));
                }
            });
        }

        #endregion

        #region --Misc Methods (Private)--
        private void cacheImage(ChatMessageTable msg)
        {
            ImageManager.INSTANCE.downloadImage(msg);
        }

        private void resetPresences()
        {
            dB.Execute("UPDATE ChatTable SET presence = ? WHERE chatType = ?;", Presence.Unavailable, ChatType.CHAT);
        }

        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<ChatTable>();
            dB.CreateTable<ChatMessageTable>();
            dB.CreateTable<MUCChatInfoTable>();
            dB.CreateTable<MUCMemberTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<ChatTable>();
            dB.DropTable<ChatMessageTable>();
            dB.DropTable<MUCChatInfoTable>();
            dB.DropTable<MUCMemberTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
