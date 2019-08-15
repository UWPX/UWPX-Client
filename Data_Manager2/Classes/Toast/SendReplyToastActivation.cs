using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using XMPP_API.Classes;

namespace Data_Manager2.Classes.Toast
{
    public class SendReplyToastActivation : AbstractToastActivation
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string TYPE = "SEND_REPLY";
        public const string CHAT_MESSAGE_QUERY = "MSG_ID";
        public const string CHAT_QUERY = "CHAT_ID";
        public readonly string CHAT_ID;
        public readonly string CHAT_MESSAGE_ID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SendReplyToastActivation(string chatId, string msgId)
        {
            CHAT_ID = chatId;
            CHAT_MESSAGE_ID = msgId;
            IS_VALID = !string.IsNullOrEmpty(CHAT_ID) && !string.IsNullOrEmpty(CHAT_MESSAGE_ID);
        }

        public SendReplyToastActivation(Uri uri)
        {
            WwwFormUrlDecoder query = UriUtils.parseUriQuery(uri);

            CHAT_ID = query.Where(x => string.Equals(x.Name, CHAT_QUERY)).Select(x => x.Value).FirstOrDefault();
            CHAT_MESSAGE_ID = query.Where(x => string.Equals(x.Name, CHAT_MESSAGE_QUERY)).Select(x => x.Value).FirstOrDefault();
            IS_VALID = !string.IsNullOrEmpty(CHAT_ID) && !string.IsNullOrEmpty(CHAT_MESSAGE_ID);
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
