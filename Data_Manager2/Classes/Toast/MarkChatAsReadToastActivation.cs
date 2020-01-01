using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using XMPP_API.Classes.XmppUri;

namespace Data_Manager2.Classes.Toast
{
    public class MarkChatAsReadToastActivation: AbstractToastActivation
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string TYPE = "MARK_CHAT_AS_READ";
        public const string CHAT_QUERY = "CHAT_ID";
        public readonly string CHAT_ID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MarkChatAsReadToastActivation(string chatId, bool received)
        {
            CHAT_ID = chatId;
            IS_VALID = !string.IsNullOrEmpty(CHAT_ID);
        }

        public MarkChatAsReadToastActivation(Uri uri)
        {
            WwwFormUrlDecoder query = UriUtils.parseUriQuery(uri);
            if (query is null)
            {
                IS_VALID = false;
                return;
            }

            CHAT_ID = query.Where(x => string.Equals(x.Name, CHAT_QUERY)).Select(x => x.Value).FirstOrDefault();
            IS_VALID = !string.IsNullOrEmpty(CHAT_ID);
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
                { CHAT_QUERY, CHAT_ID }
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
