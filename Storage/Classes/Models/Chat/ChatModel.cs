using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Storage.Classes.Models.Omemo;
using XMPP_API.Classes;

namespace Storage.Classes.Models.Chat
{
    public class ChatModel: AbstractChatModel, IComparable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [Key]
        public int id { get; set; }
        /// <summary>
        /// The bare JID of the chat/room e.g. 'coven@chat.shakespeare.lit'.
        /// </summary>
        [Required]
        public string bareJid { get; set; }
        /// <summary>
        /// The bare JID of the account, the chat belongs to.
        /// </summary>
        [Required]
        public string accountBareJid { get; set; }
        /// <summary>
        /// Whether there are chat messages for the chat available/the chat has been started.
        /// </summary>
        public bool isChatActive { get; set; }
        /// <summary>
        /// The <see cref="DateTime"/> we had the last activity in this chat, like received/send a chat message.
        /// </summary>
        [Required]
        public DateTime lastActive { get; set; }
        /// <summary>
        /// Has the chat been muted by the user.
        /// </summary>
        [Required]
        public bool muted { get; set; }
        // Presence subscription e.g. 'both' or 'from'
        public string subscription { get; set; }
        // A part of your personal roster/roster-subscription-list
        /// <summary>
        /// Is the chat part of the roster for one-to-one chats, or a part of the users bookmarks for MUCs.
        /// </summary>
        [Required]
        public bool inRoster { get; set; }
        /// <summary>
        /// True in case a presence subscription request to the targets presence is pending.
        /// </summary>
        [Required]
        public bool subscriptionRequested { get; set; }
        /// <summary>
        /// The status message for the chat.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The current chat presence e.g. online, dnd, xa, ...
        /// </summary>
        [Required]
        public Presence presence { get; set; }
        /// <summary>
        /// The state of the chat (XEP-0085). Only interesting during runtime.
        /// </summary>
        [NotMapped]
        public string chatState { get; set; }
        /// <summary>
        /// The type of the chat e.g. MUC/MIX/...
        /// </summary>
        [Required]
        public ChatType chatType { get; set; }
        /// <summary>
        /// Information about the state of OMEMO for this chat.
        /// </summary>
        [Required]
        public OmemoChatInformationModel omemo { get; set; }
        /// <summary>
        /// The optional MUC information in case <see cref="chatType"/> == <seealso cref="ChatType.MUC"/>.
        /// </summary>
        public MucInfoModel muc { get; set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatModel(string chatBareJid, Account.AccountModel account)
        {
            bareJid = chatBareJid;
            accountBareJid = account.bareJid;
            isChatActive = false;
            lastActive = DateTime.Now;
            muted = false;
            subscription = null;
            inRoster = false;
            subscriptionRequested = false;
            status = null;
            presence = Presence.Unavailable;
            chatType = ChatType.CHAT;
            omemo = new OmemoChatInformationModel();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public int CompareTo(object obj)
        {
            return obj is ChatModel c ? lastActive.CompareTo(c.lastActive) : -1;
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
