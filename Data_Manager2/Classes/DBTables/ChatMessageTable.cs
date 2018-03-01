using Data_Manager.Classes;
using SQLite.Net.Attributes;
using System;
using System.Text.RegularExpressions;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;

namespace Data_Manager2.Classes.DBTables
{
    [Table(DBTableConsts.CHAT_MESSAGE_TABLE)]
    public class ChatMessageTable
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        // Random message id
        [PrimaryKey]
        public string id { get; set; }
        // the chat id e.g. alice@jabber.org_bob@jaber.de
        public string chatId { get; set; }
        // error, chat, groupchat, ....
        public string type { get; set; }
        // The actual chat message
        public string message { get; set; }
        // Which user has send the message (useful for group chats e.g MUC or MIX)
        public string fromUser { get; set; }
        // The message date
        public DateTime date { get; set; }
        // send, read, sending, ...
        public MessageState state { get; set; }
        // Does the message is a link to an image
        public bool isImage { get; set; }

        private static readonly Regex IMAGE_URL_REGEX = new Regex(@"http[s]?:\/\/(([^\/:\.[:space:]]+(\.[^\/:\.[:space:]]+)*)|([0-9](\.[0-9]{3})))(:[0-9]+)?((\/[^?#[:space:]]+)(\?[^#[:space:]]+)?(\#.+)?)?\.(?:jpe?g|gif|png)$");

        public event EventHandler ChatMessageChanged;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 18/11/2017 Created [Fabian Sauter]
        /// </history>
        public ChatMessageTable()
        {
        }

        public ChatMessageTable(MessageMessage msg, ChatTable chat)
        {
            switch (msg.getType())
            {
                case MessageMessage.TYPE_ERROR:
                    this.id = generateErrorMessageId(msg.getId(), chat.id);
                    this.fromUser = msg.getFrom();
                    break;

                case MessageMessage.TYPE_GROUPCHAT:
                    this.id = generateId(msg.getId(), chat.id);
                    this.fromUser = msg.getFromNick();
                    break;

                default:
                    this.id = generateId(msg.getId(), chat.id);
                    this.fromUser = Utils.getBareJidFromFullJid(msg.getFrom());
                    break;
            }
            this.chatId = chat.id;
            this.type = msg.getType();
            this.message = msg.getMessage();
            this.date = msg.getDelay();
            if (this.date == null || this.date.Equals(DateTime.MinValue))
            {
                this.date = DateTime.Now;
            }
            this.state = MessageState.UNREAD;
            this.isImage = isMessageAnImageUrl(msg.getMessage());
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Triggers the ChatMessageChanged event.
        /// </summary>
        public void onChanged()
        {
            ChatMessageChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Test if the given string is an image url.
        /// </summary>
        /// <param name="msg">The message that should get checked.</param>
        /// <returns>Is image url?</returns>
        private bool isMessageAnImageUrl(string msg)
        {
            return msg != null && IMAGE_URL_REGEX.IsMatch(msg.ToLower());
        }

        /// <summary>
        /// Generates the message id based on the given msg id and chat id.
        /// </summary>
        /// <param name="msgId">The message id.</param>
        /// <param name="chatId">The id of the chat, this message is for.</param>
        public static string generateId(string msgId, string chatId)
        {
            return msgId + '_' + chatId;
        }

        /// <summary>
        /// Generates the message id based on the given msg id and chat id.
        /// </summary>
        /// <param name="msgId">The message id.</param>
        /// <param name="chatId">The id of the chat, this message is for.</param>
        public static string generateErrorMessageId(string msgId, string chatId)
        {
            return msgId + '_' + chatId + "_error";
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
