using libsignal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoDevices : AbstractPubSubItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly HashSet<uint> IDS;

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
            this.IDS = new HashSet<uint>();
            this.id = "current";
        }

        public OmemoDevices(string id)
        {
            this.IDS = new HashSet<uint>();
            this.id = id;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getContent(XNamespace ns)
        {
            XNamespace ns1 = Consts.XML_XEP_0384_NAMESPACE;
            XElement listNode = new XElement(ns1 + "list");
            foreach (uint i in IDS)
            {
                XElement device = new XElement(ns1 + "device");
                device.Add(new XAttribute("id", i));
                listNode.Add(device);
            }
            return listNode;
        }

        public uint getRandomDeviceId()
        {
            if (IDS.Count <= 0)
            {
                throw new InvalidOperationException("Failed to get random device id! Device count = " + IDS.Count);
            }
            Random r = new Random();
            return IDS.ToArray()[r.Next(0, IDS.Count)];
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
                    this.id = itemNode.Attributes["id"]?.Value;
                    XmlNode listNode = XMLUtils.getChildNode(itemNode, "list", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
                    if (listNode != null)
                    {
                        foreach (XmlNode device in listNode.ChildNodes)
                        {
                            if (string.Equals(device.Name, "device") && device.Attributes["id"] != null)
                            {
                                if (uint.TryParse(device.Attributes["id"].Value, out uint d) && d != 0)
                                {
                                    IDS.Add(d);
                                }
                            }
                        }
                    }
                }
            }
        }

        public IList<SignalProtocolAddress> toSignalProtocolAddressList(string name)
        {
            List<SignalProtocolAddress> ret = new List<SignalProtocolAddress>();
            foreach (var d in IDS)
            {
                ret.Add(new SignalProtocolAddress(name, d));
            }
            return ret;
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
