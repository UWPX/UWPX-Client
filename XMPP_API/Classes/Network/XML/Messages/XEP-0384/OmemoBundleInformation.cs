﻿using System;
using System.Xml;
using System.Xml.Linq;
using Omemo.Classes.Keys;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoBundleInformation: AbstractPubSubItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public Bundle bundle { get; private set; }
        public uint deviceId { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoBundleInformation() : this(null, 0) { }

        public OmemoBundleInformation(Bundle bundle, uint deviceId)
        {
            this.bundle = bundle;
            this.deviceId = deviceId;
            id = deviceId.ToString();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getContent(XNamespace ns)
        {
            XNamespace ns1 = Consts.XML_XEP_0384_NAMESPACE;
            XElement bundleNode = new XElement(ns1 + "bundle");

            XElement sigPreKeyNode = new XElement(ns1 + "spk");
            sigPreKeyNode.Add(new XAttribute("id", bundle.signedPreKeyId));
            sigPreKeyNode.Value = bundle.signedPreKey.ToBase64String();
            bundleNode.Add(sigPreKeyNode);

            XElement sigPreKeySigNode = new XElement(ns1 + "spks")
            {
                Value = Convert.ToBase64String(bundle.preKeySignature)
            };
            bundleNode.Add(sigPreKeySigNode);

            XElement identKeyNode = new XElement(ns1 + "ik")
            {
                Value = bundle.identityKey.ToBase64String()
            };
            bundleNode.Add(identKeyNode);

            XElement preKeysNode = new XElement(ns1 + "prekeys");
            XElement preKeyNode;
            foreach (PreKeyModel preKey in bundle.preKeys)
            {
                preKeyNode = new XElement(ns1 + "pk")
                {
                    Value = preKey.pubKey.ToBase64String()
                };
                preKeyNode.Add(new XAttribute("id", preKey.keyId));
                preKeysNode.Add(preKeyNode);
            }
            bundleNode.Add(preKeysNode);
            return bundleNode;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void loadBundleInformation(XmlNode node)
        {
            foreach (XmlNode itemNode in node.ChildNodes)
            {
                if (string.Equals(itemNode.Name, "item"))
                {
                    if (!uint.TryParse(itemNode.Attributes["id"].Value, out uint id) || id == 0)
                    {
                        continue;
                    }
                    deviceId = id;
                    XmlNode bundleNode = XMLUtils.getChildNode(itemNode, "bundle", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
                    if (bundleNode != null)
                    {
                        bundle = new Bundle();
                        foreach (XmlNode n in bundleNode.ChildNodes)
                        {
                            switch (n.Name)
                            {
                                case "spk":
                                    bundle.signedPreKey = ECPubKeyModel.FromBase64String(n.InnerText);

                                    uint.TryParse(n.Attributes["id"]?.Value, out uint sigId);
                                    bundle.signedPreKeyId = sigId;
                                    break;

                                case "spks":
                                    bundle.preKeySignature = Convert.FromBase64String(n.InnerText);
                                    break;

                                case "ik":
                                    bundle.identityKey = ECPubKeyModel.FromBase64String(n.InnerText);
                                    break;

                                case "prekeys":
                                    foreach (XmlNode n1 in n.ChildNodes)
                                    {
                                        switch (n1.Name)
                                        {
                                            case "pk":
                                                if (uint.TryParse(n1.Attributes["id"]?.Value, out uint preKeyId))
                                                {
                                                    byte[] pubPreKey = Convert.FromBase64String(n1.InnerText);
                                                    bundle.preKeys.Add(new PreKeyModel(null, ECPubKeyModel.FromBase64String(n1.InnerText), preKeyId));
                                                }
                                                else
                                                {
                                                    throw new InvalidOperationException("Invalid message. Unable to parse preKeyId");
                                                }
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                        return;
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
