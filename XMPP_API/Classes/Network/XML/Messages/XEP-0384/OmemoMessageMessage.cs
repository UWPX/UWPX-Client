using libsignal;
using libsignal.protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML.Messages.XEP_0334;

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

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Encrypts the content of MESSAGE with the given SessionCipher and saves the result in BASE_64_PAYLOAD.
        /// Sets ENCRYPTED to true.
        /// </summary>
        /// <param name="cipher">The SessionCipher for encrypting the content of MESSAGE.</param>
        public void encrypt(SessionCipher cipher, uint sourceDeviceId, uint preKeyRemoteDeviceId, IList<uint> remoteDeviceIds)
        {
            SOURCE_DEVICE_ID = sourceDeviceId;

            // 1. Generate a new AES-128 GCM key/iv pair:
            Aes128Gcm aes128Gcm = new Aes128Gcm()
            {
                data = Encoding.Unicode.GetBytes(MESSAGE)
            };
            aes128Gcm.generateKey();

            // 2. Encrypt the message using this Aes128Gcm instance:
            aes128Gcm.encrypt();
            BASE_64_PAYLOAD = Convert.ToBase64String(aes128Gcm.encryptedData);

            byte[] iv = aes128Gcm.cipherParameters.GetIV();
            BASE_64_IV = Convert.ToBase64String(iv);

            // 3. Concatenate the AES:
            byte[] keyiv = new byte[iv.Length + aes128Gcm.key.Length];
            Buffer.BlockCopy(aes128Gcm.key, 0, keyiv, 0, aes128Gcm.key.Length);
            Buffer.BlockCopy(iv, 0, keyiv, aes128Gcm.key.Length, iv.Length);

            // 4. Encrypt the key/iv pair with libsignal for each deviceId:
            CiphertextMessage ciphertextMessage = cipher.encrypt(keyiv);
            foreach (uint deviceId in remoteDeviceIds)
            {
                KEYS = new List<OmemoKey>()
                {
                    new OmemoKey(deviceId, deviceId == preKeyRemoteDeviceId, Convert.ToBase64String(ciphertextMessage.serialize()))
                };
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

            addMPHints(msgNode, null, new List<MessageProcessingHint>() { MessageProcessingHint.STORE });

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
