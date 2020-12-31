using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0082;

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
        public MessageMessage(string from, string to, string message, string type, bool reciptRequested) : this(from, to, message, type, null, reciptRequested) { }

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

        public MessageMessage(XmlNode node, CarbonCopyType ccType) : base(node.Attributes["from"]?.Value, node.Attributes["to"]?.Value, loadMessageId(node))
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
                            MESSAGE = text != null ? text.InnerText : error.InnerXml;
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
                parseDelay(delayNode);
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
        protected static string loadMessageId(XmlNode node)
        {
            // Check for a 'XEP-0359: Unique and Stable Stanza IDs' ID:
            XmlNode stanzaIdNode = XMLUtils.getChildNode(node, "stanza-id", Consts.XML_XMLNS, Consts.XML_XEP_0359_NAMESPACE);
            if (!(stanzaIdNode is null))
            {
                string sId = stanzaIdNode.Attributes["id"]?.Value;
                if (!(sId is null))
                {
                    return sId;
                }
            }
            // Fall back to an 'origin-id' node:
            XmlNode originIdNode = XMLUtils.getChildNode(node, "origin-id", Consts.XML_XMLNS, Consts.XML_XEP_0359_NAMESPACE);
            if (!(originIdNode is null))
            {
                string oId = originIdNode.Attributes["id"]?.Value;
                if (!(oId is null))
                {
                    return oId;
                }
            }

            // Check for a MAM-ID in the archived node:
            XmlNode archivedNode = XMLUtils.getChildNode(node, "archived", Consts.XML_XMLNS, Consts.XML_XEP_0313_TMP_NAMESPACE);
            if (!(archivedNode is null))
            {
                string mamId = archivedNode.Attributes["id"]?.Value;
                if (!(mamId is null))
                {
                    return mamId;
                }
            }

            // Check for the default message ID attribute:
            string id = node.Attributes["id"]?.Value;
            if (!(id is null))
            {
                return id;
            }

            // Fall back to a new message ID:
            return getRandomId();
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

            if (TYPE != null)
            {
                msgNode.Add(new XAttribute("type", TYPE));
            }

            if (includeBody && MESSAGE != null)
            {
                msgNode.Add(new XElement("body", MESSAGE));
            }

            // XEP-0359 (Unique and Stable Stanza IDs):
            Debug.Assert(ID != null);
            XNamespace sid_ns = Consts.XML_XEP_0359_NAMESPACE;
            XElement originId = new XElement(sid_ns + "origin-id");
            originId.Add(new XAttribute("id", ID));
            msgNode.Add(originId);

            // XEP-0203 (Delayed Delivery):
            if (delay != DateTime.MinValue)
            {
                XNamespace ns = Consts.XML_XEP_0203_NAMESPACE;
                XElement delayNode = new XElement(ns + "delay");
                delayNode.Add(new XAttribute("stamp", DateTimeHelper.ToString(DateTime.Now)));
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

        /// <summary>
        /// Parses a XEP-0203 (Delayed Delivery).
        /// </summary>
        public void parseDelay(XmlNode delayNode)
        {
            Debug.Assert(!(delayNode is null));
            XmlAttribute stamp = XMLUtils.getAttribute(delayNode, "stamp");
            if (stamp != null)
            {
                delay = DateTimeHelper.Parse(stamp.Value);
            }
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
