using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class IQMessage : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string SET = "set";
        public const string GET = "get";
        public const string RESULT = "result";
        public const string ERROR = "error";

        protected readonly string TYPE;
        protected readonly string QUERY;
        protected readonly XmlNode ANSWER;

        protected readonly XElement QUERY_NODE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/08/2017 Created [Fabian Sauter]
        /// </history>
        public IQMessage(string from, string to, string type, string id, XElement query) : base(from, to, id)
        {
            this.TYPE = type;
            this.QUERY_NODE = query;
        }

        public IQMessage(XmlNode answer) : base(answer?.Attributes["from"]?.Value, answer?.Attributes["to"]?.Value, answer.Attributes["id"]?.Value)
        {
            this.TYPE = answer.Attributes["type"]?.Value;
            this.ANSWER = answer;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getMessageType()
        {
            return TYPE;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
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
            if (QUERY_NODE != null)
            {
                node.Add(QUERY_NODE);
            }
            return node;
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
