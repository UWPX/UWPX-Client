using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Logging;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Omemo.Classes.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0082;
using XMPP_API.Classes.Network.XML.Messages.XEP_0420;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoEncryptedMessage: EncryptedMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The current state of the message. True in case the massage has been encrypted.
        /// </summary>
        public bool ENCRYPTED { get; private set; } = false;
        /// <summary>
        /// The device ID of the sender.
        /// </summary>
        public uint SID { get; private set; }
        public List<OmemoKeys> keys;
        public string BASE_64_PAYLOAD { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoEncryptedMessage(string from, string to, string message, string type, bool reciptRequested) : base(from, to, message, type, reciptRequested)
        {
            includeBody = false;
        }

        public OmemoEncryptedMessage(XmlNode node, CarbonCopyType ccType) : base(node, ccType)
        {
            XmlNode encryptedNode = XMLUtils.getChildNode(node, "encrypted", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
            Debug.Assert(!(encryptedNode is null));
            XmlNode headerNode = XMLUtils.getChildNode(encryptedNode, "header");
            Debug.Assert(!(headerNode is null));
            SID = uint.Parse(headerNode.Attributes["sid"]?.Value);
            foreach (XmlNode keysNode in headerNode.ChildNodes)
            {
                if (string.Equals(keysNode.Name, "keys"))
                {
                    keys.Add(new OmemoKeys(keysNode));
                }
            }

            // Can be null for empty messages:
            XmlNode payloadNode = XMLUtils.getChildNode(encryptedNode, "payload");
            if (payloadNode is null)
            {
                MESSAGE = null;
            }
            else
            {
                BASE_64_PAYLOAD = payloadNode.Value;
            }
            ENCRYPTED = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            if (!ENCRYPTED)
            {
                throw new InvalidOperationException("Message not encrypted! Call encrypt(...) first.");
            }

            XElement msgNode = base.toXElement();

            XNamespace ns = Consts.XML_XEP_0384_NAMESPACE;
            XElement encryptedNode = new XElement(ns + "encrypted");

            // Header:
            XElement headerNode = new XElement("header");
            headerNode.Add(new XAttribute("sid", SID));
            foreach (OmemoKeys key in keys)
            {
                headerNode.Add(key.toXElement(ns));
            }
            encryptedNode.Add(headerNode);

            // Payload:
            if (!string.IsNullOrEmpty(BASE_64_PAYLOAD))
            {

                encryptedNode.Add(new XElement("payload", BASE_64_PAYLOAD));
            }

            return msgNode;
        }

        /// <summary>
        /// Encrypts the message and prepares everything for sending it.
        /// </summary>
        /// <param name="sid">The device ID of the sender.</param>
        /// <param name="senderIdentityKey">The identity key pair of the sender.</param>
        /// <param name="storage">An instance of the <see cref="IOmemoStorage"/> interface.</param>
        /// <param name="devices">A collection of <see cref="OmemoDeviceGroup"/>s the message should be encrypted for.</param>
        public void encrypt(uint sid, IdentityKeyPair senderIdentityKey, IOmemoStorage storage, List<OmemoDeviceGroup> devices)
        {
            SID = sid;
            XElement contentNode = generateContent();
            byte[] contentData = Encoding.UTF8.GetBytes(contentNode.ToString());

            // Encrypt message:
            DoubleRachet rachet = new DoubleRachet(senderIdentityKey, storage);
            Tuple<byte[], byte[]> encrypted = rachet.EncryptMessasge(contentData);
            BASE_64_PAYLOAD = Convert.ToBase64String(encrypted.Item1);

            // Encrypt key || HMAC for devices:
            keys = rachet.EncryptKeyHmacForDevices(encrypted.Item2, devices).Select(x => new OmemoKeys(x.Item1, x.Item2.Select(x => new OmemoKey(x)).ToList())).ToList();

            ENCRYPTED = true;
        }

        /// <summary>
        /// Tries to decrypt the message and returns true on success.
        /// </summary>
        /// <param name="receiverIdentityKey">The receives <see cref="IdentityKeyPair"/>.</param>
        /// <param name="storage">An instance of the <see cref="IOmemoStorage"/> interface.</param>
        /// <returns>True in case the message has been decrypted successfully.</returns>
        public bool decrypt(OmemoProtocolAddress receiverAddress, IdentityKeyPair receiverIdentityKey, SignedPreKey receiverSignedPreKey, PreKey receiverPreKey, IOmemoStorage storage)
        {
            bool result = true;
            // Check if the message has been encrypted for us:
            OmemoKeys omemoKeys = keys.Where(k => string.Equals(k.BARE_JID, receiverAddress.BARE_JID)).FirstOrDefault();
            if (keys is null)
            {
                Logger.Warn("Failed to decrypt message. Not encrypted for JID.");
                return false;
            }
            OmemoKey key = omemoKeys.KEYS?.Where(k => k.DEVICE_ID == receiverAddress.DEVICE_ID).FirstOrDefault();
            if (key is null)
            {
                Logger.Warn("Failed to decrypt message. Not encrypted for device.");
                return false;
            }

            // Content can be null in case we have a pure keys exchange message:
            if (!string.IsNullOrEmpty(BASE_64_PAYLOAD))
            {
                OmemoProtocolAddress senderAddress = new OmemoProtocolAddress(Utils.getBareJidFromFullJid(FROM), SID);
                byte[] contentEnc = Convert.FromBase64String(BASE_64_PAYLOAD);
                byte[] contentDec = decryptContent(contentEnc, senderAddress, receiverAddress, receiverIdentityKey, receiverSignedPreKey, receiverPreKey, storage);
                string contentStr = Encoding.UTF8.GetString(contentDec);
                XmlNode contentNode = getContentNode(contentStr);
                result = parseContentNode(contentNode);
            }

            ENCRYPTED = false;
            return result;
        }

        #endregion

        #region --Misc Methods (Private)--
        private XElement generateContent()
        {
            XNamespace ns = Consts.XML_XEP_0420_NAMESPACE;
            XElement contentNode = new XElement(ns + "content");

            // Payload:
            XElement payloadNode = new XElement("payload");
            XNamespace bodyNs = Consts.XML_CLIENT;
            payloadNode.Add(new XElement(bodyNs + "body", MESSAGE));
            contentNode.Add(payloadNode);

            // Padding
            contentNode.Add(generatePaddingNode(1, 200, ns));

            // From:
            XElement fromNode = new XElement("from");
            fromNode.Add(new XAttribute("jid", FROM));
            contentNode.Add(fromNode);

            // To:
            XElement toNode = new XElement("to");
            toNode.Add(new XAttribute("jid", TO));
            contentNode.Add(toNode);

            // Time:
            timeStamp = DateTime.Now;
            XElement timeNode = new XElement("time");
            timeNode.Add(new XAttribute("stamp", DateTimeHelper.ToString(timeStamp)));
            contentNode.Add(timeNode);

            return contentNode;
        }

        private byte[] decryptContent(byte[] content, OmemoProtocolAddress senderAddress, OmemoProtocolAddress receiverAddress, IdentityKeyPair receiverIdentityKey, SignedPreKey receiverSignedPreKey, PreKey receiverPreKey, IOmemoStorage storage)
        {
            OmemoAuthenticatedMessage msg = prepareSession(senderAddress, receiverAddress, receiverIdentityKey, receiverSignedPreKey, receiverPreKey, storage);
            DoubleRachet rachet = new DoubleRachet(receiverIdentityKey, storage);
            OmemoSession session = storage.LoadSession(senderAddress);
            if (session is null)
            {
                throw new InvalidOperationException("Failed to decrypt. No session found.");
            }
            return rachet.DecryptMessage(msg, session, content);
        }

        private OmemoAuthenticatedMessage prepareSession(OmemoProtocolAddress senderAddress, OmemoProtocolAddress receiverAddress, IdentityKeyPair receiverIdentityKey, SignedPreKey receiverSignedPreKey, PreKey receiverPreKey, IOmemoStorage storage)
        {
            OmemoKey key = getOmemoKeyForAddress(receiverAddress);
            if (key is null)
            {
                throw new InvalidOperationException("Unable to decrypt message content. No key for device found.");
            }

            byte[] data = Convert.FromBase64String(key.BASE64_PAYLOAD);
            if (key.KEY_EXCHANGE)
            {
                OmemoKeyExchangeMessage msg = new OmemoKeyExchangeMessage(data);
                storage.StoreSession(senderAddress, new OmemoSession(receiverIdentityKey, receiverSignedPreKey, receiverPreKey, msg));
                return msg.MESSAGE;
            }
            else
            {
                return new OmemoAuthenticatedMessage(data);
            }
        }

        private OmemoKey getOmemoKeyForAddress(OmemoProtocolAddress receiverAddress)
        {
            return keys.Where(k => string.Equals(k.BARE_JID, receiverAddress.BARE_JID)).FirstOrDefault()?.KEYS.Where(k => k.DEVICE_ID == receiverAddress.DEVICE_ID).FirstOrDefault();
        }

        private XmlNode getContentNode(string contentNodeStr)
        {
            List<XmlNode> nodes = MessageParser2.parseToXmlNodes(contentNodeStr);
            foreach (XmlNode n in nodes)
            {
                if (string.Equals(n.Name, "content") && string.Equals(n.Attributes[Consts.XML_XMLNS]?.Value, Consts.XML_XEP_0420_NAMESPACE))
                {
                    return n;
                }
            }
            return null;
        }

        private bool parseContentNode(XmlNode contentNode)
        {
            // Validate the message:
            XmlNode fromNode = XMLUtils.getChildNode(contentNode, "from");
            if (!(fromNode is null))
            {
                refFrom = fromNode.Attributes["jid"]?.Value;
                if (!string.Equals(refFrom, FROM))
                {
                    Logger.Error("Failed to parse OMEMO message. Content 'from' does not match: " + refFrom + " != " + FROM);
                    return false;
                }
            }
            XmlNode toNode = XMLUtils.getChildNode(contentNode, "to");
            if (toNode is null)
            {
                Logger.Error("Failed to parse OMEMO message. Content does not contain a 'to' node for validation, which is mandatory.");
                return false;
            }
            refTo = toNode.Attributes["jid"]?.Value;
            if (!string.Equals(refTo, TO))
            {
                Logger.Error("Failed to parse OMEMO message. Content 'to' does not match: " + refTo + " != " + TO);
                return false;
            }
            XmlNode timeNode = XMLUtils.getChildNode(contentNode, "time");
            if (!(timeNode is null))
            {
                timeStamp = DateTimeHelper.Parse(timeNode.Attributes["stamp"]?.Value);
                if (!ValidateTimeStamp())
                {
                    return false;
                }
            }


            // Load the payload:
            XmlNode payloadNode = XMLUtils.getChildNode(contentNode, "payload");
            if (payloadNode is null)
            {
                return false;
            }
            XmlNode bodyNode = XMLUtils.getChildNode(payloadNode, "body");
            MESSAGE = bodyNode.InnerText;
            return true;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
