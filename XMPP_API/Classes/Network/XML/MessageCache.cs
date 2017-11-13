using Logging;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using XMPP_API.Classes.Network.XML.DBEntries;
using XMPP_API.Classes.Network.XML.Messages;

namespace XMPP_API.Classes.Network.XML
{
    class MessageCache
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "messages.db");
        protected static SQLiteConnection dB = new SQLiteConnection(new SQLitePlatformWinRT(), DB_PATH);

        public static readonly bool RESET_DB_ON_STARTUP = false;
        public static readonly MessageCache INSTANCE = new MessageCache();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Initializes the object and creates all required tables for this object.
        /// </summary>
        /// <history>
        /// 26/09/2017 Created [Fabian Sauter]
        /// </history>
        public MessageCache()
        {
            if (RESET_DB_ON_STARTUP)
            {
                dropTables();
            }
            createTables();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void addMessage(string accountId, AbstractMessage msg)
        {
            if(msg is IQMessage)
            {
                dB.Insert(new MessageEntry()
                {
                    accountId = accountId,
                    message = msg.toXmlString(),
                    iQMessageId = msg.getId()
                });
            }
            else if (msg is MessageMessage)
            {
                MessageMessage message = msg as MessageMessage;
                message.addDelay();
                dB.Insert(new MessageEntry()
                {
                    accountId = accountId,
                    message = message.toXmlString(),
                    iQMessageId = null
                });
            }
            else
            {
                dB.Insert(new MessageEntry()
                {
                    accountId = accountId,
                    message = msg.toXmlString(),
                    iQMessageId = null
                });
            }
        }

        public List<MessageEntry> getAllForAccount(string accountId)
        {
            return dB.Query<MessageEntry>("SELECT * FROM MessageEntry WHERE accountId LIKE ?", accountId);
        }

        public void removeEntry(MessageEntry entry)
        {
            dB.Delete<MessageEntry>(entry.id);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        /// <summary>
        /// Deletes the whole db and recreates an empty one.
        /// Only for testing use resetDB() instead!
        /// </summary>
        protected void deleteDB()
        {
            try
            {
                dB.Close();
                File.Delete(DB_PATH);
            }
            catch (Exception e)
            {
                Logger.Error("Unable to close or delete the messages DB", e);
            }
            dB = new SQLiteConnection(new SQLitePlatformWinRT(), DB_PATH);
        }

        /// <summary>
        /// Drops every table in the db
        /// </summary>
        protected void dropTables()
        {
            dB.DropTable<MessageEntry>();
        }

        /// <summary>
        /// Creates all required tables.
        /// </summary>
        protected void createTables()
        {
            dB.CreateTable<MessageEntry>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
