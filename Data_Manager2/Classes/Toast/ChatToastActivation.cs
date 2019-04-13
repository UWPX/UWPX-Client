using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using XMPP_API.Classes;

namespace Data_Manager2.Classes.Toast
{
    public class ChatToastActivation : AbstractToastActivation
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string TYPE = "CHAT";
        public const string CHAT_MESSAGE_QUERY = "MSG_ID";
        public const string CHAT_QUERY = "CHAT_ID";
        public readonly string CHAT_ID;
        public readonly string CHAT_MESSAGE_ID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatToastActivation(string chatId, string msgId)
        {
            this.CHAT_ID = chatId;
            this.CHAT_MESSAGE_ID = msgId;
            this.IS_VALID = !string.IsNullOrEmpty(this.CHAT_ID) && !string.IsNullOrEmpty(this.CHAT_MESSAGE_ID);
        }

        public ChatToastActivation(Uri uri)
        {
            WwwFormUrlDecoder query = UriUtils.parseUriQuery(uri);

            this.CHAT_ID = query.Where(x => string.Equals(x.Name, CHAT_QUERY)).Select(x => x.Value).FirstOrDefault();
            this.CHAT_MESSAGE_ID = query.Where(x => string.Equals(x.Name, CHAT_MESSAGE_QUERY)).Select(x => x.Value).FirstOrDefault();
            this.IS_VALID = !string.IsNullOrEmpty(this.CHAT_ID) && !string.IsNullOrEmpty(this.CHAT_MESSAGE_ID);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string generate()
        {
            Dictionary<string, string> queryPairs = new Dictionary<string, string>
            {
                { TYPE_QUERY, TYPE },
                { CHAT_QUERY, CHAT_ID },
                { CHAT_MESSAGE_QUERY, CHAT_MESSAGE_ID }
            };
            return UriUtils.buildUri(queryPairs).ToString();
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
