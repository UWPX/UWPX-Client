using System;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages
{
    public class MessageMessage: AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string MESSAGE { get; protected set; }
        public readonly string TYPE;
        public readonly string FROM_NICK;
        public readonly bool RECIPT_REQUESTED;
        public readonly CarbonCopyType CC_TYPE;
        public DateTime delay { get; protected set; }
        // The unique DB id of the message. Only required for send messages:
        public string chatMessageId;
        protected bool includeBody;

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
        public MessageMessage(string from, string to, string message, string type, bool reciptRequested) : this(from, to, message, type, null, reciptRequested)
        {
        }

        public MessageMessage(string from, string to, string message, string type, string from_nick, bool reciptRequested) : base(from, to)
        {
            MESSAGE = message;
            TYPE = type;
            cacheUntilSend = true;
            delay = DateTime.MinValue;
            FROM_NICK = from_nick;
            RECIPT_REQUESTED = reciptRequested;
            CC_TYPE = CarbonCopyType.NONE;
            includeBody = true;
        }

        public MessageMessage(XmlNode node, string type) : this(node, CarbonCopyType.NONE)
        {
            TYPE = type;
            chatMessageId = null;
        }

        public MessageMessage(XmlNode node, CarbonCopyType ccType) : base(node.Attributes["from"]?.Value, node.Attributes["to"]?.Value, (node.Attributes["id"]?.Value) ?? getRandomId())
        {
            CC_TYPE = ccType;
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
                        FROM_NICK = Utils.getJidResourcePart(FROM);
                        break;
                }
            }

            XmlNode body = XMLUtils.getChildNode(node, "body");
            if (body != null)
            {
                MESSAGE = body.InnerText;
            }

            // XEP-0203 (Delayed Delivery):
            XmlNode delayNode = XMLUtils.getChildNode(node, "delay", Consts.XML_XMLNS, Consts.XML_XEP_0203_NAMESPACE);
            if (delayNode != null)
            {
                XmlAttribute stamp = XMLUtils.getAttribute(delayNode, "stamp");
                if (stamp != null)
                {
                    DateTimeParserHelper parserHelper = new DateTimeParserHelper();
                    delay = parserHelper.parse(stamp.Value);
                }
            }
            else
            {
                delay = DateTime.Now;
            }

            // XEP-0184 (Message Delivery Receipts):
            XmlNode requestNode = XMLUtils.getChildNode(node, "request", Consts.XML_XMLNS, Consts.XML_XEP_0184_NAMESPACE);
            RECIPT_REQUESTED = requestNode != null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public DateTime getDelay()
        {
            return delay;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            XElement msgNode = new XElement("message");

            if (FROM != null)
            {
                msgNode.Add(new XAttribute("from", FROM));
            }

            if (TO != null)
            {
                msgNode.Add(new XAttribute("to", TO));
            }

            if (ID != null)
            {
                msgNode.Add(new XAttribute("id", ID));
            }

            if (TYPE != null)
            {
                msgNode.Add(new XAttribute("type", TYPE));
            }

            if (includeBody && MESSAGE != null)
            {
                msgNode.Add(new XElement("body", MESSAGE));
            }

            // XEP-0203 (Delayed Delivery):
            if (delay != DateTime.MinValue)
            {
                XNamespace ns = Consts.XML_XEP_0203_NAMESPACE;
                XElement delayNode = new XElement(ns + "delay");
                DateTimeParserHelper parserHelper = new DateTimeParserHelper();
                delayNode.Add(new XAttribute("stamp", parserHelper.toString(DateTime.Now)));
                delayNode.Add(new XAttribute("from", FROM));
                delayNode.Add("Offline Storage");
                msgNode.Add(delay);
            }

            // XEP-0184 (Message Delivery Receipts):
            if (RECIPT_REQUESTED)
            {
                XNamespace ns = Consts.XML_XEP_0184_NAMESPACE;
                XElement requestNode = new XElement(ns + "request");
                msgNode.Add(requestNode);
            }

            return msgNode;
        }

        public void addDelay()
        {
            delay = DateTime.Now;
        }

        public void addDelay(DateTime date)
        {
            delay = date;
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
