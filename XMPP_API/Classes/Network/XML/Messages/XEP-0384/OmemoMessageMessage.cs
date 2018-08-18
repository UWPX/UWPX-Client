using libsignal;
using libsignal.protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML.Messages.XEP_0334;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoMessageMessage : MessageMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public uint SOURCE_DEVICE_ID { get; private set; }

        public IList<OmemoKey> KEYS { get; private set; }

        public string BASE_64_IV { get; private set; }
        public string BASE_64_PAYLOAD { get; private set; }
        public bool ENCRYPTED { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 08/08/2018 Created [Fabian Sauter]
        /// </history>
        public OmemoMessageMessage(string from, string to, string message, string type, bool reciptRequested) : base(from, to, message, type, reciptRequested)
        {
            this.includeBody = false;
            this.KEYS = new List<OmemoKey>();
        }

        public OmemoMessageMessage(XmlNode node, CarbonCopyType ccType) : base(node, ccType)
        {
            this.KEYS = new List<OmemoKey>();
            XmlNode encryptedNode = XMLUtils.getChildNode(node, "encrypted", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
            if (encryptedNode != null)
            {
                XmlNode headerNode = XMLUtils.getChildNode(encryptedNode, "header");
                if (headerNode != null)
                {
                    foreach (XmlNode n in headerNode.ChildNodes)
                    {
                        switch (n.Name)
                        {
                            case "key":
                                KEYS.Add(new OmemoKey(n));
                                break;

                            case "iv":
                                this.BASE_64_IV = n.InnerText;
                                break;
                        }
                    }
                }

                XmlNode payloadNode = XMLUtils.getChildNode(encryptedNode, "header");
                if (payloadNode != null)
                {
                    this.BASE_64_PAYLOAD = payloadNode.InnerText;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool hasDeviceIdKey(uint deviceId)
        {
            foreach (OmemoKey key in KEYS)
            {
                if (key.REMOTE_DEVICE_ID == deviceId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Encrypts the content of MESSAGE with the given SessionCipher and saves the result in BASE_64_PAYLOAD.
        /// Sets ENCRYPTED to true.
        /// </summary>
        /// <param name="cipher">The SessionCipher for encrypting the content of MESSAGE.</param>

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Encrypts the content of MESSAGE with the given SessionCipher and saves the result in BASE_64_PAYLOAD.
        /// </summary>
        /// <param name="omemoSession">A storage object containing all SessionCipher for the target OMEMO devices.</param>
        /// <param name="sourceDeviceId">The sender OMEMO device id.</param>
        public void encrypt(OmemoSession omemoSession, uint sourceDeviceId)
        {
            SOURCE_DEVICE_ID = sourceDeviceId;

            // 1. Generate a new AES-128 GCM key/iv:
            Aes128Gcm aes128Gcm = new Aes128Gcm();
            aes128Gcm.generateKey();
            aes128Gcm.generateIv();

            // 2. Encrypt the message using the Aes128Gcm instance:
            byte[] encryptedData = aes128Gcm.encrypt(Encoding.Unicode.GetBytes(MESSAGE));
            BASE_64_PAYLOAD = Convert.ToBase64String(encryptedData);
            BASE_64_IV = Convert.ToBase64String(aes128Gcm.iv);

            // 3. Concatenate key and authentication tag:
            byte[] keyAuthTag = new byte[aes128Gcm.authTag.Length + aes128Gcm.key.Length];
            Buffer.BlockCopy(aes128Gcm.key, 0, keyAuthTag, 0, aes128Gcm.key.Length);
            Buffer.BlockCopy(aes128Gcm.authTag, 0, keyAuthTag, aes128Gcm.key.Length, aes128Gcm.authTag.Length);

            // 4. Encrypt the key/authTag pair with libsignal for each deviceId:
            KEYS = new List<OmemoKey>();
            CiphertextMessage ciphertextMessage;
            foreach (KeyValuePair<uint, SessionCipher> pair in omemoSession.DEVICE_SESSIONS)
            {
                ciphertextMessage = pair.Value.encrypt(keyAuthTag);
                // Create a new OmemoKey object with the target device id, whether it's the first time the session got established and the encrypted key:
                OmemoKey key = new OmemoKey(pair.Key, ciphertextMessage is PreKeySignalMessage, Convert.ToBase64String(ciphertextMessage.serialize()));
                KEYS.Add(key);
            }
            ENCRYPTED = true;
        }

        /// <summary>
        /// Decrypts the content of BASE_64_PAYLOAD with the given SessionCipher and saves the result in MESSAGE.
        /// Sets ENCRYPTED to false.
        /// </summary>
        /// <param name="cipher">The SessionCipher for decrypting the content of BASE_64_PAYLOAD.</param>
        public void decrypt(SessionCipher cipher)
        {
            ENCRYPTED = false;
        }

        public override XElement toXElement()
        {
            if (!ENCRYPTED)
            {
                throw new InvalidOperationException("Message not encrypted! Call encrypt(...) first.");
            }

            XElement msgNode = base.toXElement();

            XNamespace ns = Consts.XML_XEP_0384_NAMESPACE;
            XElement encNode = new XElement(ns + "encrypted");

            encNode.Add(new XElement(ns + "payload")
            {
                Value = BASE_64_PAYLOAD
            });
            XElement headerNode = new XElement(ns + "header");
            headerNode.Add(new XAttribute("sid", SOURCE_DEVICE_ID));

            foreach (OmemoKey key in KEYS)
            {
                headerNode.Add(key.toXElement(ns));
            }

            headerNode.Add(new XElement(ns + "iv")
            {
                Value = BASE_64_IV
            });
            encNode.Add(headerNode);
            msgNode.Add(encNode);

            addMPHints(msgNode, new List<MessageProcessingHint>() { MessageProcessingHint.STORE });

            return msgNode;
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
