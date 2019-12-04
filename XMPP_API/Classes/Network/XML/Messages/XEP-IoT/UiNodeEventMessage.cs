using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0004;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_IoT
{
    public class UiNodeEventMessage: AbstractPubSubEventMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly DataForm FORM;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public UiNodeEventMessage(XmlNode node) : base(node)
        {
            XmlNode eventNode = XMLUtils.getChildNode(node, "event", Consts.XML_XMLNS, Consts.XML_XEP_0060_NAMESPACE_EVENT);
            if (!(eventNode is null))
            {
                XmlNode itemsNode = XMLUtils.getChildNode(eventNode, "items", "node", IoTConsts.NODE_NAME_UI);
                if (!(itemsNode is null))
                {
                    XmlNode itemNode = XMLUtils.getChildNode(itemsNode, "item");
                    if (!(itemNode is null))
                    {
                        XmlNode xNode = XMLUtils.getChildNode(itemNode, "x", Consts.XML_XMLNS, Consts.XML_XEP_0004_NAMESPACE);
                        if (!(xNode is null))
                        {
                            FORM = new DataForm(xNode);
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
