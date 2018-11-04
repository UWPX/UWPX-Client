using System.Collections.Generic;
using Thread_Save_Components.Classes.SQLite;
using XMPP_API.Classes.Network.XML.DBEntries;
using XMPP_API.Classes.Network.XML.Messages;

namespace XMPP_API.Classes.Network.XML.DBManager
{
    class MessageCacheDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly MessageCacheDBManager INSTANCE = new MessageCacheDBManager();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/03/2018 Created [Fabian Sauter]
        /// </history>
        public MessageCacheDBManager()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void addMessage(string accountId, AbstractMessage msg)
        {
            MessageCacheTable mT = new MessageCacheTable
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

        public List<MessageCacheTable> getAllForAccount(string accountId)
        {
            return dB.Query<MessageCacheTable>(true, "SELECT * FROM " + DBTableConsts.MESSAGE_CACHE_TABLE + " WHERE accountId = ?", accountId);
        }

        public void removeEntry(MessageCacheTable entry)
        {
            dB.Delete(entry);
        }

        protected override void createTables()
        {
            dB.CreateTable<MessageCacheTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<MessageCacheTable>();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
