using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using Thread_Save_Components.Classes.SQLite;
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
        protected static TSSQLiteConnection dB = new TSSQLiteConnection(DB_PATH);

        public const bool RESET_DB_ON_STARTUP = false;
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
            MessageTable mT = new MessageTable()
            {
                accountId = accountId,
                messageId = msg.ID,
        };
            if (msg is MessageMessage)
            {
                MessageMessage message = msg as MessageMessage;
                message.addDelay();
                mT.message = message.toXmlString();
                mT.isChatMessage = true;
                mT.chatMessageId = message.chatMessageId;
            }
            else
            {
                mT.message = msg.toXmlString();
                mT.isChatMessage = false;
            }
            dB.InsertOrReplace(mT);
        }

        public List<MessageTable> getAllForAccount(string accountId)
        {
            return dB.Query<MessageTable>(true, "SELECT * FROM " + DBTableConsts.MESSAGE_TABLE + " WHERE accountId = ?", accountId);
        }

        public void removeEntry(MessageTable entry)
        {
            dB.Delete(entry);
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
            dB = new TSSQLiteConnection(DB_PATH);
        }

        /// <summary>
        /// Drops every table in the db
        /// </summary>
        protected void dropTables()
        {
            dB.DropTable<MessageTable>();
        }

        /// <summary>
        /// Creates all required tables.
        /// </summary>
        protected void createTables()
        {
            dB.CreateTable<MessageTable>();
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
