using System;
using System.Xml;

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
            if (XMLUtils.getChildNode(node, "active", Consts.XML_XMLNS, Consts.XML_XEP_0085_NAMESPACE) != null)
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
        public override string toXmlString()
        {
            throw new NotImplementedException();
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
