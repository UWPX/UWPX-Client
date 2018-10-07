using SQLite;
using System;
using XMPP_API.Classes;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.CHAT_TABLE)]
    public class ChatTable : IComparable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // Generated in generateId()
        public string id { get; set; }
        [NotNull]
        // The bare JID of the chat/room e.g. 'coven@chat.shakespeare.lit'
        public string chatJabberId { get; set; }
        [NotNull]
        // The user account id (bare JID) e.g. 'wiccarocks@shakespeare.lit'
        public string userAccountId { get; set; }
        // Last new message
        public DateTime lastActive { get; set; }
        // Chat muted yes or no
        public bool muted { get; set; }
        // Presence subscription e.g. 'both' or 'from'
        public string subscription { get; set; }
        // A part of your personal roster/rooster-subscription-list
        public bool inRoster { get; set; }
        // Subscription request status
        public string ask { get; set; }
        // Status text
        public string status { get; set; }
        // online, dnd, xa, ...
        public Presence presence { get; set; }
        [Ignore]
        // The state of the chat (XEP-0083) - only interesting during runtime
        public string chatState { get; set; }
        [NotNull]
        // The type of the chat e.g. MUC/MIX/...
        public ChatType chatType { get; set; }
        // Whether XEP-0384 (OMEMO Encryption) is enable for the chat
        public bool omemoEnabled { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        public ChatTable()
        {
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static string generateId(string chatJabberId, string userAccountId)
        {
            return chatJabberId + '_' + userAccountId;
        }

        public int CompareTo(object obj)
        {
            if (obj is ChatTable c)
            {
                return lastActive.CompareTo(c.lastActive);
            }
            return -1;
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
