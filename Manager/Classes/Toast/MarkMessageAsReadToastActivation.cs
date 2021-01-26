using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using XMPP_API.Classes.XmppUri;

namespace Manager.Classes.Toast
{
    public class MarkMessageAsReadToastActivation: AbstractToastActivation
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string TYPE = "MARK_MESSAGE_AS_READ";
        public const string CHAT_MESSAGE_QUERY = "MSG_ID";
        public readonly int CHAT_MESSAGE_ID;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public MarkMessageAsReadToastActivation(int msgId)
        {
            CHAT_MESSAGE_ID = msgId;
            IS_VALID = CHAT_MESSAGE_ID != 0;
        }

        public MarkMessageAsReadToastActivation(Uri uri)
        {
            WwwFormUrlDecoder query = UriUtils.parseUriQuery(uri);
            if (query is null)
            {
                IS_VALID = false;
                return;
            }

            string idStr = query.Where(x => string.Equals(x.Name, CHAT_MESSAGE_QUERY)).Select(x => x.Value).FirstOrDefault();
            IS_VALID = int.TryParse(idStr, out CHAT_MESSAGE_ID) && CHAT_MESSAGE_ID != 0;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string Generate()
        {
            Dictionary<string, string> queryPairs = new Dictionary<string, string>
            {
                { TYPE_QUERY, TYPE },
                { CHAT_MESSAGE_QUERY, CHAT_MESSAGE_ID.ToString() }
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
