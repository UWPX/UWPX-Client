using System.Xml;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoDeviceListEventMessage : AbstractPubSubEventMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoDevices DEVICES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoDeviceListEventMessage(XmlNode node) : base(node)
        {
            DEVICES = new OmemoDevices();
            XmlNode eventNode = XMLUtils.getChildNode(node, "event", Consts.XML_XMLNS, Consts.XML_XEP_0060_NAMESPACE_EVENT);
            if (eventNode != null)
            {
                XmlNode itemsNode = XMLUtils.getChildNode(eventNode, "items", "node", Consts.XML_XEP_0384_DEVICE_LIST_NODE);
                if (itemsNode != null)
                {
                    DEVICES.loadDevices(itemsNode);
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
