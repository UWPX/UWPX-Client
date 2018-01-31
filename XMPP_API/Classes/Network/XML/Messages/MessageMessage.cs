using System;
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
        private readonly DateTime DELAY;
        private readonly string FROM_NICK;
        private DateTime delay;
        // Already shown as a toast:
        private bool toasted;

        public const string TYPE_CHAT = "chat";
        public const string TYPE_GROUPCHAT = "groupchat";
        public const string TYPE_ERROR = "error";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 17/08/2017 Created [Fabian Sauter]
        /// </history>
        public MessageMessage(string from, string to, string message) : this(from, to, message, TYPE_CHAT)
        {
        }

        public MessageMessage(string from, string to, string message, string type) : this(from, to, message, type, null)
        {
        }

        public MessageMessage(string from, string to, string message, string type, string from_nick) : base(from, to)
        {
            this.MESSAGE = message;
            this.TYPE = type;
            this.toasted = false;
            this.cacheUntilSend = true;
            this.delay = DateTime.MinValue;
            this.FROM_NICK = from_nick;
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
            if (typeAttribute != null)
            {
                TYPE = typeAttribute.Value;
                switch (TYPE)
                {
                    case TYPE_ERROR:
                        XmlNode error = XMLUtils.getChildNode(node, "error");
                        if (error != null)
                        {
                            XmlNode text = XMLUtils.getChildNode(error, "text");
                            if (text != null)
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

                    case TYPE_GROUPCHAT:
                        FROM_NICK = Utils.getResourceFromFullJid(FROM);
                        break;
                }
            }

            XmlNode body = XMLUtils.getChildNode(node, "body");
            if (body != null)
            {
                MESSAGE = body.InnerText;
            }

            XmlNode delay = XMLUtils.getChildNode(node, "delay", "xmlns", "urn:xmpp:delay");
            if (delay != null)
            {
                XmlAttribute stamp = XMLUtils.getAttribute(delay, "stamp");
                if (stamp != null)
                {
                    DateTimeParserHelper parserHelper = new DateTimeParserHelper();
                    DELAY = parserHelper.parse(stamp.Value);
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

        public DateTime getDelay()
        {
            return DELAY;
        }

        public string getFromNick()
        {
            return FROM_NICK;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XElement node = new XElement("message");
            node.Add(new XAttribute("from", FROM));
            node.Add(new XAttribute("to", TO));
            node.Add(new XAttribute("id", ID));
            node.Add(new XAttribute("type", TYPE));

            node.Add(new XElement("body", MESSAGE));
            if (delay != DateTime.MinValue)
            {
                XElement delayNode = new XElement("delay");
                DateTimeParserHelper parserHelper = new DateTimeParserHelper();
                delayNode.Add(new XAttribute("stamp", parserHelper.toString(DateTime.Now)));
                delayNode.Add(new XAttribute("from", FROM));
                delayNode.Add("Offline Storage");
                delayNode.Add(new XAttribute("xmlns", "urn:xmpp:delay"));
                node.Add(delay);
            }
            return node;
        }

        public void addDelay()
        {
            delay = DateTime.Now;
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
