using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class MessageMessage : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly string MESSAGE;
        private readonly string TYPE;
        // Already shown as a toast:
        private bool toasted;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public MessageMessage(string from, string to, string message) : this(from, to, message, "chat")
        {
        }

        public MessageMessage(string from, string to, string message, string type) : base(from, to)
        {
            this.MESSAGE = message;
            this.TYPE = type;
            this.toasted = false;
        }

        public MessageMessage(XmlNode node) : base(node.Attributes["from"]?.Value, node.Attributes["to"]?.Value, node.Attributes["id"]?.Value)
        {
            if (!node.HasChildNodes)
            {
                MESSAGE = "invalid message: " + node.ToString();
                TYPE = "error";
                return;
            }

            XmlAttribute typeAttribute = XMLUtils.getAttribute(node, "type");
            if(typeAttribute != null)
            {
                TYPE = typeAttribute.Value;
                switch (TYPE)
                {
                    case "error":
                        XmlNode error = XMLUtils.getChildNode(node, "error");
                        if(error != null)
                        {
                            XmlNode text = XMLUtils.getChildNode(error, "text");
                            if(text != null)
                            {
                                MESSAGE = text.InnerText;
                            }
                            else
                            {
                                MESSAGE = error.InnerXml;
                            }
                        }
                        else
                        {
                            MESSAGE = node.InnerXml;
                        }
                        return;
                }
            }

            XmlNode body = XMLUtils.getChildNode(node, "body");
            if (body != null)
            {
                MESSAGE = body.InnerText;
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public string getMessage()
        {
            return MESSAGE;
        }

        public string getType()
        {
            return TYPE;
        }

        public bool getToasted()
        {
            return toasted;
        }

        public void setToasted()
        {
            toasted = true;
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
            node.Add(new XAttribute("type", TYPE));

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
