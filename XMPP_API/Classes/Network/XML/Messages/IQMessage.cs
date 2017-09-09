using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class IQMessage : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly string SET = "set";
        public static readonly string GET = "get";
        public static readonly string RESULT = "result";
        public static readonly string ERROR = "error";

        protected readonly string TYPE;
        protected readonly string QUERY;
        protected readonly XmlNode ANSWER;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/08/2017 Created [Fabian Sauter]
        /// </history>
        public IQMessage(string from, string to, string type, string id, string query) : base(from, to, id)
        {
            this.TYPE = type;
            this.QUERY = query;
        }

        public IQMessage(XmlNode answer) : base(answer?.Attributes["from"]?.Value, answer?.Attributes["to"]?.Value, answer.Attributes["id"]?.Value)
        {
            this.TYPE = answer.Attributes["type"]?.Value;
            this.ANSWER = answer;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static string getLoginQuery(string from)
        {
            string s = "<query xmlns='jabber:iq:auth'>";
            s += "<username>" + from + "</username>";
            s += "</query>";
            return s;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string toXmlString()
        {
            string s = "<iq type='" + TYPE + "' id='" + ID;
            if(TO != null)
            {
                s += "' to='" + TO;
            }
            if (FROM != null)
            {
                s += "' from='" + FROM;
            }
            s += "'>";
            if (QUERY != null)
            {
                s += QUERY;
            }
            s += "</iq>";
            return s;
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
