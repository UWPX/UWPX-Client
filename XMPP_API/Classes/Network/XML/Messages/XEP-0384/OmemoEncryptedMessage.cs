using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Logging;
using Omemo.Classes;
using Omemo.Classes.Exceptions;
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
        /// <summary>
        /// True in case the message ha no content (body).
        /// Only valid when <see cref="ENCRYPTED"/> == false/>.
        /// </summary>
        public bool IS_PURE_KEY_EXCHANGE_MESSAGE { get; private set; } = false;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoEncryptedMessage(string from, string to, string message, string type, bool reciptRequested) : base(from, to, message, type, reciptRequested)
        {
            includeBody = false;
            IS_PURE_KEY_EXCHANGE_MESSAGE = string.IsNullOrEmpty(message);
        }

        public OmemoEncryptedMessage(XmlNode node, CarbonCopyType ccType) : base(node, ccType)
        {
            XmlNode encryptedNode = XMLUtils.getChildNode(node, "encrypted", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
            Debug.Assert(!(encryptedNode is null));
            XmlNode headerNode = XMLUtils.getChildNode(encryptedNode, "header");
            Debug.Assert(!(headerNode is null));
            SID = uint.Parse(headerNode.Attributes["sid"]?.Value);
            keys = new List<OmemoKeys>();
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
                IS_PURE_KEY_EXCHANGE_MESSAGE = true;
            }
            else
            {
                BASE_64_PAYLOAD = payloadNode.InnerText;
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

            // Fallback for clients that do not support OMEMO:
            XNamespace bodyNs = Consts.XML_CLIENT;
            msgNode.Add(new XElement(bodyNs + "body", "I sent you an OMEMO encrypted message but your client doesn’t seem to support that. Find more information on: https://conversations.im/omemo"));

            XNamespace ns = Consts.XML_XEP_0384_NAMESPACE;
            XElement encryptedNode = new XElement(ns + "encrypted");
            msgNode.Add(encryptedNode);

            // Header:
            XElement headerNode = new XElement(ns + "header");
            headerNode.Add(new XAttribute("sid", SID));
            foreach (OmemoKeys key in keys)
            {
                headerNode.Add(key.toXElement(ns));
            }
            encryptedNode.Add(headerNode);

            // Payload:
            if (!IS_PURE_KEY_EXCHANGE_MESSAGE)
            {
                encryptedNode.Add(new XElement(ns + "payload", BASE_64_PAYLOAD));
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
        public void encrypt(uint sid, IdentityKeyPairModel senderIdentityKey, IOmemoStorage storage, List<OmemoDeviceGroup> devices)
        {
            try
            {
                SID = sid;
                if (IS_PURE_KEY_EXCHANGE_MESSAGE)
                {
                    // Encrypt 32 zero bytes as dummy data for devices:
                    DoubleRachet rachet = new DoubleRachet(senderIdentityKey);
                    keys = rachet.EncryptKeyHmacForDevices(new byte[32], devices).Select(x => new OmemoKeys(x.Item1, x.Item2.Select(x => new OmemoKey(x)).ToList())).ToList();
                }
                else
                {
                    XElement contentNode = generateEnvelope();
                    byte[] contentData = Encoding.UTF8.GetBytes(contentNode.ToString());

                    // Encrypt message:
                    DoubleRachet rachet = new DoubleRachet(senderIdentityKey);
                    Tuple<byte[], byte[]> encrypted = rachet.EncryptMessasge(contentData);
                    BASE_64_PAYLOAD = Convert.ToBase64String(encrypted.Item1);

                    // Encrypt key || HMAC for devices:
                    keys = rachet.EncryptKeyHmacForDevices(encrypted.Item2, devices).Select(x => new OmemoKeys(x.Item1, x.Item2.Select(x => new OmemoKey(x)).ToList())).ToList();
                }

                // Store all sessions:
                foreach (OmemoDeviceGroup group in devices)
                {
                    foreach (KeyValuePair<uint, OmemoSessionModel> device in group.SESSIONS)
                    {
                        // Only store sessions in case encryption for this device was successful:
                        if (!(keys.Where(k => string.Equals(k.BARE_JID, group.BARE_JID)).FirstOrDefault()?.KEYS.Where(k => k.DEVICE_ID == device.Key).FirstOrDefault() is null))
                        {
                            storage.StoreSession(new OmemoProtocolAddress(group.BARE_JID, device.Key), device.Value);
                        }
                    }
                }

                ENCRYPTED = true;
            }
            catch (Exception e)
            {
                if (e is OmemoException)
                {
                    throw e;
                }
                throw new OmemoException("Failed to encrypt.", e);
            }
        }

        /// <summary>
        /// Tries to decrypt the message and returns true on success.
        /// </summary>
        /// <param name="decryptCtx">The <see cref="OmemoDecryptionContext"/> containing all necessary information for decrypting the message.</param>
        /// <param name="storage">An instance of the <see cref="IOmemoStorage"/> interface.</param>
        public void decrypt(OmemoDecryptionContext decryptCtx)
        {
            try
            {
                // Check if the message has been encrypted for us:
                OmemoKeys omemoKeys = keys.Where(k => string.Equals(k.BARE_JID, decryptCtx.RECEIVER_ADDRESS.BARE_JID)).FirstOrDefault();
                if (keys is null)
                {
                    throw new NotForDeviceException("Failed to decrypt message. Not encrypted for JID.");
                }
                decryptCtx.key = omemoKeys.KEYS?.Where(k => k.DEVICE_ID == decryptCtx.RECEIVER_ADDRESS.DEVICE_ID).FirstOrDefault();
                if (decryptCtx.key is null)
                {
                    throw new NotForDeviceException("Failed to decrypt message. Not encrypted for device.");
                }
                decryptCtx.senderAddress = new OmemoProtocolAddress(Utils.getBareJidFromFullJid(FROM), SID);

                // Check if the PreKey and SignedPreKey are still available in case the massage is a key exchange message:
                byte[] data = Convert.FromBase64String(decryptCtx.key.BASE64_PAYLOAD);
                if (decryptCtx.key.KEY_EXCHANGE)
                {
                    decryptCtx.keyExchangeMsg = new OmemoKeyExchangeMessage(data);

                    // Check if there already exists a session. In case yes, compare the ephemeral public key stored in the Double Ratchet:
                    OmemoSessionModel oldSession = decryptCtx.STORAGE.LoadSession(decryptCtx.senderAddress);
                    if (!(oldSession is null) && oldSession.ek.Equals(decryptCtx.keyExchangeMsg.EK))
                    {
                        // Process only the auth message: 
                        decryptCtx.authMsg = decryptCtx.keyExchangeMsg.MESSAGE;
                        decryptCtx.keyExchange = false;
                        Logger.Info($"Received an {nameof(OmemoKeyExchangeMessage)} for a session from '{decryptCtx.senderAddress}' that already exists. This could be because we have not yet responded with an own message to the sender.");
                    }
                    else
                    {
                        decryptCtx.keyExchange = true;
                        decryptCtx.authMsg = decryptCtx.keyExchangeMsg.MESSAGE;
                        if (decryptCtx.keyExchangeMsg.SPK_ID != decryptCtx.RECEIVER_SIGNED_PRE_KEY.preKey.keyId)
                        {
                            throw new NotForDeviceException($"Failed to decrypt message. Signed PreKey with id {decryptCtx.keyExchangeMsg.SPK_ID} not available any more.");
                        }

                        decryptCtx.usedPreKey = decryptCtx.RECEIVER_PRE_KEYS.Where(k => k.keyId == decryptCtx.keyExchangeMsg.PK_ID).FirstOrDefault();
                        if (decryptCtx.usedPreKey is null)
                        {
                            throw new NotForDeviceException($"Failed to decrypt message. PreKey with id {decryptCtx.keyExchangeMsg.PK_ID} not available any more.");
                        }
                    }
                }
                else
                {
                    decryptCtx.authMsg = new OmemoAuthenticatedMessage(data);
                }

                // Content can be null in case we have a pure keys exchange message:
                byte[] contentEnc = Convert.FromBase64String(BASE_64_PAYLOAD);

                // Check the senders fingerprint and handle new devices:
                ValidateSender(decryptCtx);

                if (!IS_PURE_KEY_EXCHANGE_MESSAGE)
                {
                    byte[] contentDec = decryptContent(contentEnc, decryptCtx);
                    string contentStr = Encoding.UTF8.GetString(contentDec);
                    XmlNode envelopeNode = getEnvelopeNode(contentStr);
                    parseEnvelopeNode(envelopeNode);
                }

                // In case nothing went wrong, store the session:
                decryptCtx.STORAGE.StoreSession(decryptCtx.senderAddress, decryptCtx.session);
                ENCRYPTED = false;
            }
            catch (Exception e)
            {
                if (e is OmemoException)
                {
                    throw e;
                }
                throw new OmemoException("Failed to decrypt.", e);
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private XElement generateEnvelope()
        {
            XNamespace ns = Consts.XML_XEP_0420_NAMESPACE;
            XElement envelopeNode = new XElement(ns + "envelope");

            // Content:
            XElement contentNode = new XElement("content");
            XNamespace bodyNs = Consts.XML_CLIENT;
            contentNode.Add(new XElement(bodyNs + "body", MESSAGE));
            envelopeNode.Add(contentNode);

            // Padding
            envelopeNode.Add(generatePaddingNode(1, 200, ns));

            // From:
            XElement fromNode = new XElement("from");
            fromNode.Add(new XAttribute("jid", FROM));
            envelopeNode.Add(fromNode);

            // To:
            XElement toNode = new XElement("to");
            toNode.Add(new XAttribute("jid", TO));
            envelopeNode.Add(toNode);

            // Time:
            timeStamp = DateTime.Now;
            XElement timeNode = new XElement("time");
            timeNode.Add(new XAttribute("stamp", DateTimeHelper.ToString(timeStamp)));
            envelopeNode.Add(timeNode);

            return envelopeNode;
        }

        private byte[] decryptContent(byte[] content, OmemoDecryptionContext decryptCtx)
        {
            if (decryptCtx.keyExchange)
            {
                decryptCtx.session = new OmemoSessionModel(decryptCtx.RECEIVER_IDENTITY_KEY, decryptCtx.RECEIVER_SIGNED_PRE_KEY, decryptCtx.usedPreKey, decryptCtx.keyExchangeMsg);
            }
            else
            {
                decryptCtx.session = decryptCtx.STORAGE.LoadSession(decryptCtx.senderAddress);
                if (decryptCtx.session is null)
                {
                    throw new InvalidOperationException("Failed to decrypt. No session found.");
                }
                Logger.Debug("Loaded OMEMO session for decrypting: " + decryptCtx.session.ToString());
            }
            DoubleRachet rachet = new DoubleRachet(decryptCtx.RECEIVER_IDENTITY_KEY);
            return rachet.DecryptMessage(decryptCtx.authMsg, decryptCtx.session, content);
        }

        private XmlNode getEnvelopeNode(string contentNodeStr)
        {
            List<XmlNode> nodes = MessageParser2.parseToXmlNodes(contentNodeStr);
            foreach (XmlNode n in nodes)
            {
                if (string.Equals(n.Name, "envelope") && string.Equals(n.Attributes[Consts.XML_XMLNS]?.Value, Consts.XML_XEP_0420_NAMESPACE))
                {
                    return n;
                }
            }
            return null;
        }

        private void parseEnvelopeNode(XmlNode envelopeNode)
        {
            // Validate the message:
            XmlNode fromNode = XMLUtils.getChildNode(envelopeNode, "from");
            if (!(fromNode is null))
            {
                refFrom = fromNode.Attributes["jid"]?.Value;
                if (!string.Equals(refFrom, FROM))
                {
                    throw new OmemoException("Failed to parse OMEMO message. Content 'from' does not match: " + refFrom + " != " + FROM);
                }
            }
            XmlNode toNode = XMLUtils.getChildNode(envelopeNode, "to");
            if (toNode is null)
            {
                throw new OmemoException("Failed to parse OMEMO message. Content does not contain a 'to' node for validation, which is mandatory.");
            }
            refTo = toNode.Attributes["jid"]?.Value;
            if (!string.Equals(refTo, Utils.getBareJidFromFullJid(TO)))
            {
                throw new OmemoException("Failed to parse OMEMO message. Content 'to' does not match: " + refTo + " != " + TO);
            }
            XmlNode timeNode = XMLUtils.getChildNode(envelopeNode, "time");
            if (!(timeNode is null))
            {
                timeStamp = DateTimeHelper.Parse(timeNode.Attributes["stamp"]?.Value);
                if (!ValidateTimeStamp())
                {
                    throw new OmemoException("Failed to parse OMEMO message. Invalid time stamp: " + timeNode.Attributes["stamp"]?.Value);
                }
            }

            // Load the payload:
            XmlNode contentNode = XMLUtils.getChildNode(envelopeNode, "content");
            if (!(contentNode is null))
            {
                XmlNode bodyNode = XMLUtils.getChildNode(contentNode, "body");
                MESSAGE = bodyNode.InnerText;
            }
        }

        private void ValidateSender(OmemoDecryptionContext decryptCtx)
        {
            OmemoFingerprint fingerprint = decryptCtx.STORAGE.LoadFingerprint(decryptCtx.senderAddress);
            // New device:
            if (fingerprint is null)
            {
                List<Tuple<OmemoProtocolAddress, string>> devices = decryptCtx.STORAGE.LoadDevices(decryptCtx.senderAddress.BARE_JID);
                devices.Add(new Tuple<OmemoProtocolAddress, string>(decryptCtx.senderAddress, null));
                decryptCtx.STORAGE.StoreDevices(devices, decryptCtx.senderAddress.BARE_JID);
                fingerprint = new OmemoFingerprint(decryptCtx.keyExchangeMsg.IK, decryptCtx.senderAddress, DateTime.Now, false);
            }
            else
            {
                fingerprint.lastSeen = DateTime.Now;
            }
            decryptCtx.STORAGE.StoreFingerprint(fingerprint);


            if (decryptCtx.TRUSTED_KEYS_ONLY && !fingerprint.trusted)
            {
                throw new OmemoException("Failed to decrypt OMEMO message since we do not trust the sender.");
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
