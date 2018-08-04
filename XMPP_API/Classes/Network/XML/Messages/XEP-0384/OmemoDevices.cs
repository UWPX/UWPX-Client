using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoDevices : AbstractPubSubItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly List<string> DEVICES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 04/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoDevices()
        {
            this.DEVICES = new List<string>();
            this.id = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getContent(XNamespace ns)
        {
            XNamespace ns1 = Consts.XML_XEP_0384_NAMESPACE;
            XElement listNode = new XElement(ns1 + "list");
            foreach (string s in DEVICES)
            {
                XElement device = new XElement(ns1 + "device");
                device.Add(new XAttribute("id", s));
                listNode.Add(device);
            }
            return listNode;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void loadDevices(XmlNode node)
        {
            foreach (XmlNode itemNode in node.ChildNodes)
            {
                if (string.Equals(itemNode.Name, "item"))
                {
                    XmlNode listNode = XMLUtils.getChildNode(itemNode, "list", Consts.XML_XMLNS, Consts.XML_XEP_0384_DEVICE_LIST_NODE);
                    if (listNode != null)
                    {
                        foreach (XmlNode device in listNode.ChildNodes)
                        {
                            if (string.Equals(device.Name, "device") && device.Attributes["id"] != null)
                            {
                                DEVICES.Add(device.Attributes["id"].Value);
                            }
                        }
                    }
                }
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
