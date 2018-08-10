using libsignal;
using libsignal.ecc;
using libsignal.state;
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
        public IdentityKey PUBLIC_IDENTITY_KEY { get; private set; }
        public ECPublicKey PUBLIC_SIGNED_PRE_KEY { get; private set; }
        public readonly IList<Tuple<uint, ECPublicKey>> PUBLIC_PRE_KEYS;
        public byte[] SIGNED_PRE_KEY_SIGNATURE { get; private set; }
        public uint SIGNED_PRE_KEY_ID { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 06/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoBundleInformation() : this(null, null, 0, null, new List<Tuple<uint, ECPublicKey>>())
        {

        }

        public OmemoBundleInformation(IdentityKey publicIdentityKey, ECPublicKey publicSignedPreKey, uint signedPreKeyId, byte[] signedPreKeySignature, IList<Tuple<uint, ECPublicKey>> publicPreKeys)
        {
            this.id = null;
            this.PUBLIC_IDENTITY_KEY = publicIdentityKey;
            this.PUBLIC_SIGNED_PRE_KEY = publicSignedPreKey;
            this.PUBLIC_PRE_KEYS = publicPreKeys;
            this.SIGNED_PRE_KEY_SIGNATURE = signedPreKeySignature;
            this.SIGNED_PRE_KEY_ID = signedPreKeyId;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public PreKeyBundle getRandomPreKey(uint deviceId)
        {
            if (PUBLIC_PRE_KEYS.Count <= 0)
            {
                return null;
            }
            Random r = new Random();
            Tuple<uint, ECPublicKey> publicPreKey = PUBLIC_PRE_KEYS[r.Next(0, PUBLIC_PRE_KEYS.Count)];

            return new PreKeyBundle(publicPreKey.Item1, deviceId, publicPreKey.Item1, publicPreKey.Item2, SIGNED_PRE_KEY_ID, PUBLIC_SIGNED_PRE_KEY, SIGNED_PRE_KEY_SIGNATURE, PUBLIC_IDENTITY_KEY);
        }

        protected override XElement getContent(XNamespace ns)
        {
            XNamespace ns1 = Consts.XML_XEP_0384_NAMESPACE;
            XElement bundleNode = new XElement(ns1 + "bundle");

            XElement sigPreKeyNode = new XElement(ns1 + "signedPreKeyPublic");
            sigPreKeyNode.Add(new XAttribute("signedPreKeyId", SIGNED_PRE_KEY_ID));
            sigPreKeyNode.Value = Convert.ToBase64String(PUBLIC_SIGNED_PRE_KEY.serialize());
            bundleNode.Add(sigPreKeyNode);

            XElement sigPreKeySigNode = new XElement(ns1 + "signedPreKeySignature")
            {
                Value = Convert.ToBase64String(SIGNED_PRE_KEY_SIGNATURE)
            };
            bundleNode.Add(sigPreKeySigNode);

            XElement identKeyNode = new XElement(ns1 + "identityKey")
            {
                Value = Convert.ToBase64String(PUBLIC_IDENTITY_KEY.serialize())
            };
            bundleNode.Add(identKeyNode);

            if (PUBLIC_PRE_KEYS.Count < 20)
            {
                throw new InvalidOperationException("Failed to convert " + nameof(OmemoBundleInformation) + " to an " + nameof(XElement) + "! Less then 20 PreKeys given.");
            }

            XElement preKeysNode = new XElement(ns1 + "prekeys");
            XElement preKeyNode;
            foreach (Tuple<uint, ECPublicKey> key in PUBLIC_PRE_KEYS)
            {
                preKeyNode = new XElement(ns1 + "preKeyPublic")
                {
                    Value = Convert.ToBase64String(key.Item2.serialize())
                };
                preKeyNode.Add(new XAttribute("preKeyId", key.Item1));
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
                    XmlNode bundleNode = XMLUtils.getChildNode(itemNode, "bundle", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
                    if (bundleNode != null)
                    {
                        foreach (XmlNode n in bundleNode.ChildNodes)
                        {
                            switch (n.Name)
                            {
                                case "signedPreKeyPublic":
                                    byte[] pubSignedPreKey = Convert.FromBase64String(n.InnerText);
                                    PUBLIC_SIGNED_PRE_KEY = Curve.decodePoint(pubSignedPreKey, 0);

                                    uint.TryParse(n.Attributes["signedPreKeyId"]?.Value, out uint sigId);
                                    SIGNED_PRE_KEY_ID = sigId;
                                    break;

                                case "signedPreKeySignature":
                                    SIGNED_PRE_KEY_SIGNATURE = Convert.FromBase64String(n.InnerText);
                                    break;

                                case "identityKey":
                                    byte[] identPubKey = Convert.FromBase64String(n.InnerText);
                                    PUBLIC_IDENTITY_KEY = new IdentityKey(identPubKey, 0);
                                    break;

                                case "prekeys":
                                    foreach (XmlNode n1 in n.ChildNodes)
                                    {
                                        switch (n1.Name)
                                        {
                                            case "preKeyPublic":
                                                if (uint.TryParse(n1.Attributes["preKeyId"]?.Value, out uint preKeyId))
                                                {
                                                    byte[] pubPreKey = Convert.FromBase64String(n.InnerText);
                                                    PUBLIC_PRE_KEYS.Add(new Tuple<uint, ECPublicKey>(preKeyId, Curve.decodePoint(pubPreKey, 0)));
                                                }
                                                else
                                                {
                                                    throw new InvalidMessageException("Invalid message. Unable to parse preKeyId");
                                                }
                                                break;
                                        }
                                    }
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
