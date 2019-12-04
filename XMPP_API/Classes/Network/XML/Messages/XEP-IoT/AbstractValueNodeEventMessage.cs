using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_IoT
{
    public abstract class AbstractValueNodeEventMessage: AbstractPubSubEventMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly IoTValue VALUES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AbstractValueNodeEventMessage(XmlNode node, string nodeName) : base(node)
        {
            XmlNode eventNode = XMLUtils.getChildNode(node, "event", Consts.XML_XMLNS, Consts.XML_XEP_0060_NAMESPACE_EVENT);
            if (!(eventNode is null))
            {
                XmlNode itemsNode = XMLUtils.getChildNode(eventNode, "items", "node", nodeName);
                if (!(itemsNode is null))
                {
                    XmlNode itemNode = XMLUtils.getChildNode(itemsNode, "item");
                    if (!(itemsNode is null))
                    {
                        XmlNode valNode = XMLUtils.getChildNode(itemNode, "val", Consts.XML_XMLNS, Consts.XML_XEP_IOT_NAMESPACE);
                        if (!(valNode is null))
                        {
                            VALUES = new IoTValue(itemNode.Attributes["id"]?.Value, valNode);
                        }
                    }
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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
