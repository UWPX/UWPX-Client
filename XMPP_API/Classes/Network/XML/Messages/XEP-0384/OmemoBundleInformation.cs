using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0060;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoBundleInformation : AbstractPubSubItem
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string BASE_64_IDENTITY_KEY { get; private set; }
        public string BASE_64_SIGNED_PRE_KEY { get; private set; }
        public string BASE_64_SIGNED_PRE_KEY_SIGNATURE { get; private set; }
        public readonly List<string> BASE_64_PRE_KEYS;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoBundleInformation() : this(null, null, null, new List<string>())
        {
        }

        public OmemoBundleInformation(string base64IdentityKey, string base64SignedPreKey, string base64SignedPreKeySignature, List<string> base64PreKeys)
        {
            this.BASE_64_IDENTITY_KEY = base64IdentityKey;
            this.BASE_64_SIGNED_PRE_KEY = base64SignedPreKey;
            this.BASE_64_SIGNED_PRE_KEY_SIGNATURE = base64SignedPreKeySignature;
            this.BASE_64_PRE_KEYS = base64PreKeys;
            this.id = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        protected override XElement getContent(XNamespace ns)
        {
            XNamespace ns1 = Consts.XML_XEP_0384_NAMESPACE;
            XElement bundleNode = new XElement(ns1 + "bundle");

            XElement sigPreKeyNode = new XElement(ns1 + "signedPreKeyPublic");
            sigPreKeyNode.Add(new XAttribute("signedPreKeyId", "1"));
            sigPreKeyNode.Value = BASE_64_SIGNED_PRE_KEY;
            bundleNode.Add(sigPreKeyNode);

            XElement sigPreKeySigNode = new XElement(ns1 + "signedPreKeySignature")
            {
                Value = BASE_64_SIGNED_PRE_KEY_SIGNATURE
            };
            bundleNode.Add(sigPreKeySigNode);

            XElement identKeyNode = new XElement(ns1 + "identityKey")
            {
                Value = BASE_64_IDENTITY_KEY
            };
            bundleNode.Add(identKeyNode);

            if (BASE_64_PRE_KEYS.Count < 20)
            {
                throw new InvalidOperationException("Failed to convert " + nameof(OmemoBundleInformation) + " to an " + nameof(XElement) + "! Less then 20 PreKeys given.");
            }

            XElement preKeyNode;
            for (int i = 0; i < BASE_64_PRE_KEYS.Count; i++)
            {
                preKeyNode = new XElement(ns1 + "preKeyPublic")
                {
                    Value = BASE_64_PRE_KEYS[i]
                };
                preKeyNode.Add(new XAttribute("preKeyId", i));
                bundleNode.Add(preKeyNode);
            }

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
                    XmlNode bundleNode = XMLUtils.getChildNode(itemNode, "bundle", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
                    if (bundleNode != null)
                    {
                        foreach (XmlNode n in bundleNode.ChildNodes)
                        {
                            switch (n.Name)
                            {
                                case "signedPreKeyPublic":
                                    BASE_64_SIGNED_PRE_KEY = n.InnerText;
                                    break;

                                case "signedPreKeySignature":
                                    BASE_64_SIGNED_PRE_KEY_SIGNATURE = n.InnerText;
                                    break;

                                case "identityKey":
                                    BASE_64_IDENTITY_KEY = n.InnerText;
                                    break;

                                case "preKeyPublic":
                                    BASE_64_PRE_KEYS.Add(n.InnerText);
                                    break;
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
