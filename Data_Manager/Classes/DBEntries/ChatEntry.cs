﻿using Logging;
using SQLite.Net.Attributes;
using System;
using XMPP_API.Classes;

namespace Data_Manager.Classes.DBEntries
{
    public class ChatEntry
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
        // The state of the chat (XEP-0083)
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
        /// 30/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatEntry()
        {
            this.chatState = "";
        }

        public ChatEntry(string chatJabberId, string userAccountId)
        {
            this.id = generateId(chatJabberId, userAccountId);
            this.chatJabberId = chatJabberId;
            this.userAccountId = userAccountId;
            this.name = null;
            this.lastActive = DateTime.Now;
            this.muted = false;
            this.chatState = "";
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void update(ChatEntry chat)
        {
            this.ask = chat.ask;
            this.inRoster = chat.inRoster;
            this.lastActive = chat.lastActive;
            this.muted = chat.muted;
            this.name = chat.name;
            this.presence = chat.presence;
            this.status = chat.status;
            this.subscription = chat.subscription;

            ChatChanged?.Invoke(this, new EventArgs());
        }

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
