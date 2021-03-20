using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Omemo.Classes;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoXmlDevices: AbstractPubSubItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly HashSet<OmemoXmlDevice> DEVICES;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoXmlDevices() : this("current") { }

        public OmemoXmlDevices(string id)
        {
            DEVICES = new HashSet<OmemoXmlDevice>();
            this.id = id;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getContent(XNamespace ns)
        {
            XNamespace ns1 = Consts.XML_XEP_0384_NAMESPACE;
            XElement devicesNode = new XElement(ns1 + "devices");
            foreach (OmemoXmlDevice d in DEVICES)
            {
                devicesNode.Add(d.toXElement(ns1));
            }
            return devicesNode;
        }

        /// <summary>
        /// Required for test only.
        /// </summary>
        public uint getRandomDeviceId()
        {
            if (DEVICES.Count <= 0)
            {
                throw new InvalidOperationException("Failed to get random device id! Device count = " + DEVICES.Count);
            }
            Random r = new Random();
            return DEVICES.ToArray()[r.Next(0, DEVICES.Count)].ID;
        }

        /// <summary>
        /// Sets the item id to "current".
        /// <para/>
        /// Reference: https://xmpp.org/extensions/xep-0384.html#usecases-announcing
        /// </summary>
        public void setId()
        {
            id = "current";
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void loadDevices(XmlNode node)
        {
            setId();
            foreach (XmlNode itemNode in node.ChildNodes)
            {
                if (string.Equals(itemNode.Name, "item"))
                {
                    if (string.Equals(itemNode.Attributes["id"]?.Value, "current"))
                    {
                        XmlNode listNode = XMLUtils.getChildNode(itemNode, "devices", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
                        if (listNode != null)
                        {
                            foreach (XmlNode device in listNode.ChildNodes)
                            {
                                if (string.Equals(device.Name, "device") && device.Attributes["id"] != null)
                                {
                                    DEVICES.Add(new OmemoXmlDevice(device));
                                }
                            }
                        }
                        return;
                    }
                }
            }
        }

        public List<OmemoProtocolAddress> toOmemoProtocolAddress(string bareJid)
        {
            return DEVICES.Select(d => new OmemoProtocolAddress(bareJid, d.ID)).ToList();
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
