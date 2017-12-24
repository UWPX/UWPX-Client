using SQLite.Net.Attributes;
using System;
using XMPP_API.Classes;

namespace Data_Manager2.Classes.DBTables
{
    public class ChatTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        // A hash value from userAccountId and
        [PrimaryKey]
        public string id { get; set; }
        // userId@domain or chatId@domain
        [NotNull]
        public string chatJabberId { get; set; }
        [NotNull]
        public string userAccountId { get; set; }
        // Last new message
        public DateTime lastActive { get; set; }
        // Chat muted yes or no
        public bool muted { get; set; }
        public string subscription { get; set; }
        // A part of your personal roster
        public bool inRoster { get; set; }
        // Subscription request status
        public string ask { get; set; }
        // Status text
        public string status { get; set; }
        // online, dnd, xa, ...
        public Presence presence { get; set; }
        // The state of the chat (XEP-0083) - only interesting dunging app runtime
        [Ignore]
        public string chatState { get; set; }

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
        public static string generateId(string jabberID, string userAccountID)
        {
            return jabberID + userAccountID;
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
