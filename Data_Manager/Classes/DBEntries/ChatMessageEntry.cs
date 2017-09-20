using SQLite.Net.Attributes;
using System;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;

namespace Data_Manager.Classes.DBEntries
{
    public class ChatMessageEntry
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        [PrimaryKey]
        public string id { get; set; }
        public string chatId { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public string fromUser { get; set; }
        public DateTime date { get; set; }
        public byte state { get; set; }

        public static readonly byte SENDING = 0;
        public static readonly byte SEND = 1;
        public static readonly byte RECEIVED = 2;
        public static readonly byte READ = 3;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 29/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatMessageEntry()
        {

        }

        public ChatMessageEntry(MessageMessage msg, ChatEntry chat)
        {
            if(msg.getType() != null && msg.getType().Equals("error"))
            {
                id = msg.getId() + '_' + chat.id + "_error";
            }
            else
            {
                id = msg.getId() + '_' + chat.id;
            }
            chatId = chat.id;
            message = msg.getMessage();
            type = msg.getType();
            fromUser = Utils.removeResourceFromJabberid(msg.getFrom());
            date = DateTime.Now;
            state = ChatMessageEntry.RECEIVED;
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
