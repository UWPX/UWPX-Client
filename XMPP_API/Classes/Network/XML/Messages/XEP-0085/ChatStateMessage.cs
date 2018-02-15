using System;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0085
{
    public class ChatStateMessage : AbstractAddressableMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly ChatState STATE;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 09/11/2017 Created [Fabian Sauter]
        /// </history>
        public ChatStateMessage(XmlNode node) : base(node.Attributes["from"]?.Value, node.Attributes["to"]?.Value, node.Attributes["id"]?.Value)
        {
            if(XMLUtils.getChildNode(node, "error") != null)
            {
                STATE = ChatState.UNKNOWN;
            }
            else if (XMLUtils.getChildNode(node, "active", Consts.XML_XMLNS, Consts.XML_XEP_0085_NAMESPACE) != null)
            {
                STATE = ChatState.ACTIVE;
            }
            else if (XMLUtils.getChildNode(node, "composing", Consts.XML_XMLNS, Consts.XML_XEP_0085_NAMESPACE) != null)
            {
                STATE = ChatState.COMPOSING;
            }
            else if (XMLUtils.getChildNode(node, "paused", Consts.XML_XMLNS, Consts.XML_XEP_0085_NAMESPACE) != null)
            {
                STATE = ChatState.PAUSED;
            }
            else if (XMLUtils.getChildNode(node, "inactive", Consts.XML_XMLNS, Consts.XML_XEP_0085_NAMESPACE) != null)
            {
                STATE = ChatState.INACTIVE;
            }
            else if (XMLUtils.getChildNode(node, "gone", Consts.XML_XMLNS, Consts.XML_XEP_0085_NAMESPACE) != null)
            {
                STATE = ChatState.GONE;
            }
            else
            {
                STATE = ChatState.UNKNOWN;
            }
        }

        public ChatStateMessage(string to, string from, ChatState state) :base(from, to, getRandomId())
        {
            this.STATE = state;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public ChatState getState()
        {
            return STATE;
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
            node.Add(new XAttribute("type", "chat"));

            XElement sNode;
            XNamespace ns = Consts.XML_XEP_0085_NAMESPACE;
            switch (STATE)
            {
                case ChatState.ACTIVE:
                    sNode = new XElement(ns + "active");
                    break;
                case ChatState.COMPOSING:
                    sNode = new XElement(ns + "composing");
                    break;
                case ChatState.PAUSED:
                    sNode = new XElement(ns + "paused");
                    break;
                case ChatState.INACTIVE:
                    sNode = new XElement(ns + "inactive");
                    break;
                case ChatState.GONE:
                    sNode = new XElement(ns + "gone");
                    break;
                case ChatState.UNKNOWN:
                default:
                    throw new Exception("Invalid chat state: " + STATE);
            }
            node.Add(sNode);
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
