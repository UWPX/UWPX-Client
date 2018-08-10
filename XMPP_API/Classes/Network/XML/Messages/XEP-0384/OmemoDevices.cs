using System;
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
        public readonly List<uint> DEVICES;

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
            this.DEVICES = new List<uint>();
            this.id = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getContent(XNamespace ns)
        {
            XNamespace ns1 = Consts.XML_XEP_0384_NAMESPACE;
            XElement listNode = new XElement(ns1 + "list");
            foreach (Int32 d in DEVICES)
            {
                XElement device = new XElement(ns1 + "device");
                device.Add(new XAttribute("id", d));
                listNode.Add(device);
            }
            return listNode;
        }

        public uint getRandomDeviceId()
        {
            if (DEVICES.Count <= 0)
            {
                throw new InvalidOperationException("Failed to get random device id! Device count = " + DEVICES.Count);
            }
            Random r = new Random();
            return DEVICES[r.Next(0, DEVICES.Count)];
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
                    XmlNode listNode = XMLUtils.getChildNode(itemNode, "list", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
                    if (listNode != null)
                    {
                        foreach (XmlNode device in listNode.ChildNodes)
                        {
                            if (string.Equals(device.Name, "device") && device.Attributes["id"] != null)
                            {
                                if (uint.TryParse(device.Attributes["id"].Value, out uint d))
                                {
                                    DEVICES.Add(d);
                                }
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
