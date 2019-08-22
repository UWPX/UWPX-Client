using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using libsignal;
using libsignal.ecc;
using libsignal.protocol;
using Logging;
using Strilanc.Value;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML.Messages.XEP_0334;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoMessageMessage: MessageMessage
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
            includeBody = false;
            KEYS = new List<OmemoKey>();
            ENCRYPTED = false;
        }

        public OmemoMessageMessage(XmlNode node, CarbonCopyType ccType) : base(node, ccType)
        {
            KEYS = new List<OmemoKey>();
            XmlNode encryptedNode = XMLUtils.getChildNode(node, "encrypted", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
            if (encryptedNode != null)
            {
                ENCRYPTED = true;
                XmlNode headerNode = XMLUtils.getChildNode(encryptedNode, "header");
                if (headerNode != null)
                {
                    if (uint.TryParse(headerNode.Attributes["sid"]?.Value, out uint sid))
                    {
                        SOURCE_DEVICE_ID = sid;
                    }

                    foreach (XmlNode n in headerNode.ChildNodes)
                    {
                        switch (n.Name)
                        {
                            case "key":
                                KEYS.Add(new OmemoKey(n));
                                break;

                            case "iv":
                                BASE_64_IV = n.InnerText;
                                break;

                            default:
                                break;
                        }
                    }
                }

                XmlNode payloadNode = XMLUtils.getChildNode(encryptedNode, "payload");
                if (payloadNode != null)
                {
                    BASE_64_PAYLOAD = payloadNode.InnerText;
                }
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public OmemoKey getOmemoKey(uint deviceId)
        {
            foreach (OmemoKey key in KEYS)
            {
                if (key.REMOTE_DEVICE_ID == deviceId)
                {
                    return key;
                }
            }
            return null;
        }

        /// <summary>
        /// Validates if the given identity public key should be trusted.
        /// </summary>
        /// <param name="address">The signal protocol address corresponding to the given public identity key.</param>
        /// <param name="publicKey">The public identity key we want to validate.</param>
        /// <param name="omemoStore">The OMEMO store that keeps all OMEMO related keys.</param>
        /// <returns>True if we trust else false.</returns>
        private Task<bool> isFingerprintTrustedAsync(SignalProtocolAddress address, ECPublicKey publicKey, IOmemoStore omemoStore)
        {
            return Task.Run(() =>
            {
                OmemoFingerprint fingerprint = omemoStore.LoadFingerprint(address);
                if (!(fingerprint is null))
                {
                    if (!fingerprint.checkIdentityKey(publicKey))
                    {
                        Logger.Warn("Received not OMEMO encrypted message with a not matching public identity key from: " + address.ToString());
                        return false;
                    }
                    fingerprint.lastSeen = DateTime.Now;
                }
                else
                {
                    // Create a new fingerprint and store it:
                    fingerprint = new OmemoFingerprint(publicKey, address, DateTime.Now, false);
                }
                omemoStore.StoreFingerprint(fingerprint);
                return omemoStore.IsFingerprintTrusted(fingerprint);
            });
        }

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
            Aes128GcmCpp aes128Gcm = new Aes128GcmCpp();
            aes128Gcm.generateKey();
            aes128Gcm.generateIv();

            // 2. Encrypt the message using the Aes128Gcm instance:
            byte[] encryptedData = aes128Gcm.encrypt(Encoding.UTF8.GetBytes(MESSAGE));
            BASE_64_PAYLOAD = Convert.ToBase64String(encryptedData);
            BASE_64_IV = Convert.ToBase64String(aes128Gcm.iv);

            // 3. Concatenate key and authentication tag:
            byte[] keyAuthTag = new byte[aes128Gcm.authTag.Length + aes128Gcm.key.Length];
            Buffer.BlockCopy(aes128Gcm.key, 0, keyAuthTag, 0, aes128Gcm.key.Length);
            Buffer.BlockCopy(aes128Gcm.authTag, 0, keyAuthTag, aes128Gcm.key.Length, aes128Gcm.authTag.Length);

            // 4. Encrypt the key/authTag pair with libsignal for each deviceId:
            KEYS = new List<OmemoKey>();
            Logger.Debug("[OmemoMessageMessage]: Source device id: " + SOURCE_DEVICE_ID);
            Logger.Debug("[OmemoMessageMessage]: Encrypting for remote devices.");
            encryptForDevices(omemoSession.DEVICE_SESSIONS_REMOTE, keyAuthTag);
            Logger.Debug("[OmemoMessageMessage]: Encrypting for own devices.");
            encryptForDevices(omemoSession.DEVICE_SESSIONS_OWN, keyAuthTag);

            ENCRYPTED = true;
        }

        /// <summary>
        /// Decrypts the content of BASE_64_PAYLOAD. Loads the SessionCipher from the given OmemoHelper object and saves the result in MESSAGE.
        /// Sets ENCRYPTED to false.
        /// </summary>
        /// <param name="helper">The current OmemoHelper object of the current account.</param>
        /// <param name="localeDeciceId">The local device id.</param>
        /// <returns>True on success.</returns>
        public async Task<bool> decryptAsync(OmemoHelper helper, uint localeDeciceId)
        {
            SignalProtocolAddress remoteAddress = new SignalProtocolAddress(Utils.getBareJidFromFullJid(FROM), SOURCE_DEVICE_ID);
            return await decryptAsync(helper.loadCipher(remoteAddress), remoteAddress, localeDeciceId, helper);
        }

        /// <summary>
        /// Decrypts the content of BASE_64_PAYLOAD with the given SessionCipher and saves the result in MESSAGE.
        /// Sets ENCRYPTED to false.
        /// </summary>
        /// <param name="cipher">The SessionCipher for decrypting the content of BASE_64_PAYLOAD.</param>
        /// <param name="senderAddress">The senders signal protocol address.</param>
        /// <param name="localeDeciceId">The local device id.</param>
        /// <param name="helper">The current OmemoHelper object of the current account. If null, won't remove used PreKey.</param>
        /// <returns>True on success.</returns>
        public async Task<bool> decryptAsync(SessionCipher cipher, SignalProtocolAddress senderAddress, uint localeDeciceId, OmemoHelper helper)
        {
            try
            {
                // 1. Check if the message contains a key for the local device:
                OmemoKey key = getOmemoKey(localeDeciceId);
                if (key is null)
                {
                    Logger.Info("Discarded received OMEMO message - doesn't contain device id!");
                    return false;
                }

                // 2. Load the cipher:
                byte[] encryptedKeyAuthTag = Convert.FromBase64String(key.BASE_64_KEY);
                byte[] decryptedKeyAuthTag = null;
                if (key.IS_PRE_KEY)
                {
                    PreKeySignalMessage preKeySignalMessage = new PreKeySignalMessage(encryptedKeyAuthTag);

                    // 3. a) Validate if we trust the fingerprint of the sender:
                    if (!await isFingerprintTrustedAsync(senderAddress, preKeySignalMessage.getSignalMessage().getSenderRatchetKey(), helper.OMEMO_STORE))
                    {
                        Logger.Info("Discarded received OMEMO message - fingerprint not trusted!");
                        return false;
                    }

                    decryptedKeyAuthTag = cipher.decrypt(preKeySignalMessage);
                    if (!(helper is null))
                    {
                        May<uint> preKey = preKeySignalMessage.getPreKeyId();
                        if (preKey.HasValue)
                        {
                            Logger.Info("Removing used PreKey.");
                            await helper.removePreKeyAndRepublishAsync(preKey.ForceGetValue());
                        }
                        else
                        {
                            Logger.Error("Failed to get value from PreKeySignalMessage.");
                        }
                    }
                }
                else
                {
                    SignalMessage signalMessage = new SignalMessage(encryptedKeyAuthTag);
                    // 3. b) Validate if we trust the fingerprint of the sender:
                    if (!await isFingerprintTrustedAsync(senderAddress, signalMessage.getSenderRatchetKey(), helper.OMEMO_STORE))
                    {
                        Logger.Info("Discarded received OMEMO message - fingerprint not trusted!");
                        return false;
                    }
                    decryptedKeyAuthTag = cipher.decrypt(signalMessage);
                }

                // 4. Check if the cipher got loaded successfully:
                if (decryptedKeyAuthTag is null)
                {
                    Logger.Info("Discarded received OMEMO message - failed to decrypt keyAuthTag is null!");
                    return false;
                }

                // 5. Decrypt the payload:
                byte[] aesIv = Convert.FromBase64String(BASE_64_IV);
                byte[] aesKey = new byte[16];
                byte[] aesAuthTag = new byte[decryptedKeyAuthTag.Length - aesKey.Length];
                Buffer.BlockCopy(decryptedKeyAuthTag, 0, aesKey, 0, aesKey.Length);
                Buffer.BlockCopy(decryptedKeyAuthTag, aesKey.Length, aesAuthTag, 0, aesAuthTag.Length);
                Aes128GcmCpp aes128Gcm = new Aes128GcmCpp()
                {
                    key = aesKey,
                    authTag = aesAuthTag,
                    iv = aesIv
                };

                byte[] encryptedData = Convert.FromBase64String(BASE_64_PAYLOAD);
                byte[] decryptedData = aes128Gcm.decrypt(encryptedData);

                // 6. Convert decrypted data to an Unicode string:
                MESSAGE = Encoding.UTF8.GetString(decryptedData);

                ENCRYPTED = false;
                return true;
            }
            catch (Exception e)
            {
                Logger.Info("Discarded received OMEMO message - failed to decrypt with: " + e.Message);
            }
            return false;
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
        private void encryptForDevices(Dictionary<uint, SessionCipher> devices, byte[] keyAuthTag)
        {
            CiphertextMessage ciphertextMessage;
            foreach (KeyValuePair<uint, SessionCipher> pair in devices)
            {
                Logger.Debug("[OmemoMessageMessage]: Encrypting for deviceId: " + pair.Key);
                ciphertextMessage = pair.Value.encrypt(keyAuthTag);
                // Create a new OmemoKey object with the target device id, whether it's the first time the session got established and the encrypted key:
                OmemoKey key = new OmemoKey(pair.Key, ciphertextMessage is PreKeySignalMessage, Convert.ToBase64String(ciphertextMessage.serialize()));
                KEYS.Add(key);
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
