﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Logging;
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
                if (d.IS_VALID)
                {
                    devicesNode.Add(d.toXElement(ns1));
                }
                else
                {
                    Logger.Warn("Tried to send an invalid OMEMO device: " + d.ToString());
                }
            }
            return devicesNode;
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
                            foreach (XmlNode deviceNode in listNode.ChildNodes)
                            {
                                if (string.Equals(deviceNode.Name, "device") && deviceNode.Attributes["id"] != null)
                                {
                                    OmemoXmlDevice device = new OmemoXmlDevice(deviceNode);
                                    if (device.IS_VALID)
                                    {
                                        DEVICES.Add(device);
                                    }
                                    else
                                    {
                                        Logger.Warn("Received an invalid OMEMO device: " + device.ToString());
                                    }
                                }
                            }
                        }
                        return;
                    }
                }
            }
        }

        public List<Tuple<OmemoProtocolAddress, string>> toOmemoProtocolAddress(string bareJid)
        {
            return DEVICES.Select(d => new Tuple<OmemoProtocolAddress, string>(new OmemoProtocolAddress(bareJid, d.ID), d.label)).ToList();
        }

        /// <summary>
        /// Required for test only.
        /// </summary>
        public uint pickRandomDeviceId()
        {
            if (DEVICES.Count <= 0)
            {
                throw new InvalidOperationException("Failed to get random device id! Device count = " + DEVICES.Count);
            }
            Random r = new Random();
            return DEVICES.ToArray()[r.Next(0, DEVICES.Count)].ID;
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
