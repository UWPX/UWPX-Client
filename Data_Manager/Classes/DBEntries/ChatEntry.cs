using SQLite.Net.Attributes;
using System;

namespace Data_Manager.Classes.DBEntries
{
    public class ChatEntry
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        // userId@domain or chatId@domain
        public string id { get; set; }
        public string userAccountId { get; set; }
        // Chatname
        public string name { get; set; }
        // Last new message
        public DateTime lastActive { get; set; }
        // Chat muted yes or no
        public bool muted { get; set; }
        public string subscription { get; set; }
        // A part of your personal roster
        public bool inRooster { get; set; }
        // Subscription request status
        public string ask { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 30/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatEntry()
        {

        }

        public ChatEntry(string id, string userAccountId)
        {
            this.id = id;
            this.userAccountId = userAccountId;
            this.name = null;
            this.lastActive = DateTime.Now;
            this.muted = false;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
