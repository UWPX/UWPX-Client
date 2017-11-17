using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMPP_API.Classes;

namespace Data_Manager.Classes.DBEntries
{
    class ChatEntry2
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        // A hash value from userAccountId and
        [PrimaryKey]
        public string id { get; set; }
        // userId@domain or chatId@domain
        public string chatJabberId { get; set; }
        public string userAccountId { get; set; }
        // Chat name
        public string name { get; set; }
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

        public event EventHandler ChatChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/11/2017 Created [Fabian Sauter]
        /// </history>
        public ChatEntry2()
        {

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
