using System.Xml;
using System.Xml.Linq;

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
        #region --Constructors--
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
            s += new XElement("username", from).ToString();
            s += "</query>";
            return s;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string toXmlString()
        {
            XElement node = new XElement("iq");
            node.Add(new XAttribute("type", TYPE));
            node.Add(new XAttribute("id", ID));
            if (TO != null)
            {
                node.Add(new XAttribute("to", TO));
            }
            if (FROM != null)
            {
                node.Add(new XAttribute("from", FROM));
            }
            if (QUERY != null)
            {
                node.Add(XElement.Parse(QUERY));
            }
            return node.ToString();
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
