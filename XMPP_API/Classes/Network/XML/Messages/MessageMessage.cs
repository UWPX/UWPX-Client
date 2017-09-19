using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class MessageMessage : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly string MESSAGE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Construktoren--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public MessageMessage(string from, string to, string message) : base(from, to)
        {
            this.MESSAGE = message;
        }

        public MessageMessage(XmlNode node) : base(node.Attributes["from"]?.Value, node.Attributes["to"]?.Value, node.Attributes["id"]?.Value)
        {
            if(!node.HasChildNodes)
            {
                return;
            }
            foreach (XmlNode n in node.ChildNodes)
            {
                if(n.Name.Equals("body"))
                {
                    MESSAGE = n.InnerText;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getMessage()
        {
            return MESSAGE;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override string toXmlString()
        {
            XElement node = new XElement("message");
            node.Add(new XAttribute("from", FROM));
            node.Add(new XAttribute("to", TO));
            node.Add(new XAttribute("id", ID));
            node.Add(new XAttribute("type", "chat"));

            node.Add(new XElement("body", MESSAGE));
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
