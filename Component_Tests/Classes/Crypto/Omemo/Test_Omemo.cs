using Component_Tests.Classes.Crypto.Libsignal;
using libsignal;
using libsignal.ecc;
using libsignal.protocol;
using libsignal.state;
using libsignal.util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XMPP_API.Classes;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384.Signal.Session;

namespace Component_Tests.Classes.Crypto.Omemo
{
    [TestClass]
    public class Test_Omemo
    {
        private readonly SignalProtocolAddress ALICE_ADDRESS = new SignalProtocolAddress("alice@example.com", 1);
        private readonly SignalProtocolAddress BOB_ADDRESS = new SignalProtocolAddress("bob@example.com", 1);

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Omemo_Enc_Dec_1()
        {
            // Generate Alices keys:
            IdentityKeyPair aliceIdentKey = CryptoUtils.generateOmemoIdentityKeyPair();
            IList<PreKeyRecord> alicePreKeys = CryptoUtils.generateOmemoPreKeys();
            SignedPreKeyRecord aliceSignedPreKey = CryptoUtils.generateOmemoSignedPreKey(aliceIdentKey);

            // Create Alices stores:
            InMemoryIdentityKeyStore aliceIdentStore = new InMemoryIdentityKeyStore(aliceIdentKey, ALICE_ADDRESS.getDeviceId());
            InMemoryPreKeyStore alicePreKeyStore = new InMemoryPreKeyStore();
            foreach (PreKeyRecord key in alicePreKeys)
            {
                alicePreKeyStore.StorePreKey(key.getId(), key);
            }
            InMemorySignedPreKeyStore aliceSignedPreKeyStore = new InMemorySignedPreKeyStore();
            aliceSignedPreKeyStore.StoreSignedPreKey(aliceSignedPreKey.getId(), aliceSignedPreKey);
            InMemorySessionStore aliceSessionStore = new InMemorySessionStore();

            // Generate Bobs keys:
            IdentityKeyPair bobIdentKey = CryptoUtils.generateOmemoIdentityKeyPair();
            IList<PreKeyRecord> bobPreKeys = CryptoUtils.generateOmemoPreKeys();
            SignedPreKeyRecord bobSignedPreKey = CryptoUtils.generateOmemoSignedPreKey(bobIdentKey);

            // Create Bobs stores:
            InMemoryIdentityKeyStore bobIdentStore = new InMemoryIdentityKeyStore(bobIdentKey, BOB_ADDRESS.getDeviceId());
            InMemoryPreKeyStore bobPreKeyStore = new InMemoryPreKeyStore();
            foreach (PreKeyRecord key in bobPreKeys)
            {
                bobPreKeyStore.StorePreKey(key.getId(), key);
            }
            InMemorySignedPreKeyStore bobSignedPreKeyStore = new InMemorySignedPreKeyStore();
            bobSignedPreKeyStore.StoreSignedPreKey(bobSignedPreKey.getId(), bobSignedPreKey);
            InMemorySessionStore bobSessionStore = new InMemorySessionStore();


            //-----------------OMEOMO Session Building:-----------------
            MessageParser2 parser = new MessageParser2();

            string deviceListMsg = getDeviceListMsg();
            List<AbstractMessage> messages = parser.parseMessages(ref deviceListMsg);
            Assert.IsTrue(messages.Count == 1);
            Assert.IsTrue(messages[0] is OmemoDeviceListResultMessage);
            OmemoDeviceListResultMessage devList = messages[0] as OmemoDeviceListResultMessage;

            uint selectedBobDeviceId = devList.DEVICES.getRandomDeviceId();
            Assert.IsTrue(selectedBobDeviceId == BOB_ADDRESS.getDeviceId());

            // Alice builds a session to Bob:
            string bundleInfoMsg = getBundleInfoMsg(bobIdentKey, bobSignedPreKey, bobPreKeys);
            messages = parser.parseMessages(ref bundleInfoMsg);
            Assert.IsTrue(messages.Count == 1);
            Assert.IsTrue(messages[0] is OmemoBundleInformationResultMessage);
            OmemoBundleInformationResultMessage bundleInfo = messages[0] as OmemoBundleInformationResultMessage;
            Assert.IsTrue(bundleInfo.DEVICE_ID == BOB_ADDRESS.getDeviceId());

            SessionBuilder sessionBuilder = new SessionBuilder(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, BOB_ADDRESS);
            PreKeyBundle bobPreKey = bundleInfo.BUNDLE_INFO.getRandomPreKey(bundleInfo.DEVICE_ID);
            sessionBuilder.process(bobPreKey);

            // Check if session exists:
            Assert.IsTrue(aliceSessionStore.ContainsSession(BOB_ADDRESS));
            Assert.IsTrue(aliceSessionStore.LoadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);

            // Alice sends a message:
            string aliceOrigMsg = "$(rm -rvf .)";

            // 1. Generate a new AES-128 GCM key/iv:
            Aes128GcmCpp aes128Gcm = new Aes128GcmCpp();
            aes128Gcm.generateKey();
            aes128Gcm.generateIv();

            // 2. Encrypt the message using the Aes128Gcm instance:
            byte[] encryptedData = aes128Gcm.encrypt(Encoding.UTF8.GetBytes(aliceOrigMsg));
            string base64Payload = Convert.ToBase64String(encryptedData);
            string base64IV = Convert.ToBase64String(aes128Gcm.iv);

            // 3. Concatenate key and authentication tag:
            byte[] keyAuthTag = new byte[aes128Gcm.authTag.Length + aes128Gcm.key.Length];
            Buffer.BlockCopy(aes128Gcm.key, 0, keyAuthTag, 0, aes128Gcm.key.Length);
            Buffer.BlockCopy(aes128Gcm.authTag, 0, keyAuthTag, aes128Gcm.key.Length, aes128Gcm.authTag.Length);

            SessionCipher aliceSessionCipher = new SessionCipher(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, BOB_ADDRESS);
            CiphertextMessage ciphertextMessage = aliceSessionCipher.encrypt(keyAuthTag);
            OmemoKey omemoKey = new OmemoKey(BOB_ADDRESS.getDeviceId(), ciphertextMessage is PreKeySignalMessage, Convert.ToBase64String(ciphertextMessage.serialize()));
            Assert.IsTrue(string.Equals(Convert.ToBase64String(ciphertextMessage.serialize()), omemoKey.BASE_64_KEY));

            //-------------------Decrypt Again:-------------------

            // 2. Load the cipher:
            SessionCipher cipher = new SessionCipher(bobSessionStore, bobPreKeyStore, bobSignedPreKeyStore, bobIdentStore, ALICE_ADDRESS);
            byte[] encryptedKeyAuthTag = Convert.FromBase64String(omemoKey.BASE_64_KEY);
            byte[] decryptedKeyAuthTag = null;
            if (omemoKey.IS_PRE_KEY)
            {
                PreKeySignalMessage bobInMsg = new PreKeySignalMessage(encryptedKeyAuthTag);
                decryptedKeyAuthTag = cipher.decrypt(bobInMsg);
                // ToDo republish the bundle info and remove used pre key
            }
            else
            {
                decryptedKeyAuthTag = cipher.decrypt(new SignalMessage(encryptedKeyAuthTag));
            }

            // 3. Check if the cipher got loaded successfully:
            Assert.IsTrue(decryptedKeyAuthTag != null);

            // 4. Decrypt the payload:
            byte[] aesIv = Convert.FromBase64String(base64IV);
            byte[] aesKey = new byte[16];
            byte[] aesAuthTag = new byte[decryptedKeyAuthTag.Length - aesKey.Length];
            Buffer.BlockCopy(decryptedKeyAuthTag, 0, aesKey, 0, aesKey.Length);
            Buffer.BlockCopy(decryptedKeyAuthTag, aesKey.Length, aesAuthTag, 0, aesAuthTag.Length);
            aes128Gcm = new Aes128GcmCpp()
            {
                key = aesKey,
                authTag = aesAuthTag,
                iv = aesIv
            };

            encryptedData = Convert.FromBase64String(base64Payload);
            byte[] bobData = aes128Gcm.decrypt(encryptedData);

            // 5. Convert decrypted data to Unicode string:
            string bobRecMsg = Encoding.UTF8.GetString(bobData);

            // Check if successfully send:
            Assert.AreEqual(aliceOrigMsg, bobRecMsg);
            Assert.IsTrue(bobSessionStore.ContainsSession(ALICE_ADDRESS));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public async void Test_Omemo_Enc_Dec_2()
        {
            // Generate Alices keys:
            IdentityKeyPair aliceIdentKey = CryptoUtils.generateOmemoIdentityKeyPair();
            IList<PreKeyRecord> alicePreKeys = CryptoUtils.generateOmemoPreKeys();
            SignedPreKeyRecord aliceSignedPreKey = CryptoUtils.generateOmemoSignedPreKey(aliceIdentKey);

            // Create Alices stores:
            InMemoryIdentityKeyStore aliceIdentStore = new InMemoryIdentityKeyStore(aliceIdentKey, ALICE_ADDRESS.getDeviceId());
            InMemoryPreKeyStore alicePreKeyStore = new InMemoryPreKeyStore();
            foreach (PreKeyRecord key in alicePreKeys)
            {
                alicePreKeyStore.StorePreKey(key.getId(), key);
            }
            InMemorySignedPreKeyStore aliceSignedPreKeyStore = new InMemorySignedPreKeyStore();
            aliceSignedPreKeyStore.StoreSignedPreKey(aliceSignedPreKey.getId(), aliceSignedPreKey);
            InMemorySessionStore aliceSessionStore = new InMemorySessionStore();

            // Generate Bobs keys:
            IdentityKeyPair bobIdentKey = CryptoUtils.generateOmemoIdentityKeyPair();
            IList<PreKeyRecord> bobPreKeys = CryptoUtils.generateOmemoPreKeys();
            SignedPreKeyRecord bobSignedPreKey = CryptoUtils.generateOmemoSignedPreKey(bobIdentKey);

            // Create Bobs stores:
            InMemoryIdentityKeyStore bobIdentStore = new InMemoryIdentityKeyStore(bobIdentKey, BOB_ADDRESS.getDeviceId());
            InMemoryPreKeyStore bobPreKeyStore = new InMemoryPreKeyStore();
            foreach (PreKeyRecord key in bobPreKeys)
            {
                bobPreKeyStore.StorePreKey(key.getId(), key);
            }
            InMemorySignedPreKeyStore bobSignedPreKeyStore = new InMemorySignedPreKeyStore();
            bobSignedPreKeyStore.StoreSignedPreKey(bobSignedPreKey.getId(), bobSignedPreKey);
            InMemorySessionStore bobSessionStore = new InMemorySessionStore();


            //-----------------OMEOMO Session Building:-----------------
            MessageParser2 parser = new MessageParser2();

            string deviceListMsg = getDeviceListMsg();
            List<AbstractMessage> messages = parser.parseMessages(ref deviceListMsg);
            Assert.IsTrue(messages.Count == 1);
            Assert.IsTrue(messages[0] is OmemoDeviceListResultMessage);
            OmemoDeviceListResultMessage devList = messages[0] as OmemoDeviceListResultMessage;

            uint selectedBobDeviceId = devList.DEVICES.getRandomDeviceId();
            Assert.IsTrue(selectedBobDeviceId == BOB_ADDRESS.getDeviceId());

            // Alice builds a session to Bob:
            string bundleInfoMsg = getBundleInfoMsg(bobIdentKey, bobSignedPreKey, bobPreKeys);
            messages = parser.parseMessages(ref bundleInfoMsg);
            Assert.IsTrue(messages.Count == 1);
            Assert.IsTrue(messages[0] is OmemoBundleInformationResultMessage);
            OmemoBundleInformationResultMessage bundleInfo = messages[0] as OmemoBundleInformationResultMessage;
            Assert.IsTrue(bundleInfo.DEVICE_ID == BOB_ADDRESS.getDeviceId());

            SessionBuilder sessionBuilder = new SessionBuilder(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, BOB_ADDRESS);
            PreKeyBundle bobPreKey = bundleInfo.BUNDLE_INFO.getRandomPreKey(bundleInfo.DEVICE_ID);
            sessionBuilder.process(bobPreKey);

            // Check if session exists:
            Assert.IsTrue(aliceSessionStore.ContainsSession(BOB_ADDRESS));
            Assert.IsTrue(aliceSessionStore.LoadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);

            // Alice sends a message:
            string aliceOrigMsg = "$(rm -rvf .)";
            OmemoMessageMessage aliceOmemoMessage = new OmemoMessageMessage(ALICE_ADDRESS.getName() + "/SOME_RESOURCE", BOB_ADDRESS.getName(), aliceOrigMsg, MessageMessage.TYPE_CHAT, true);
            Assert.IsFalse(aliceOmemoMessage.ENCRYPTED);

            OmemoSession omemoSession = new OmemoSession(BOB_ADDRESS.getName());
            SessionCipher aliceSessionCipher = new SessionCipher(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, BOB_ADDRESS);
            omemoSession.DEVICE_SESSIONS_REMOTE.Add(BOB_ADDRESS.getDeviceId(), aliceSessionCipher);

            // Alice encrypts the message:
            aliceOmemoMessage.encrypt(omemoSession, ALICE_ADDRESS.getDeviceId());
            Assert.IsTrue(aliceOmemoMessage.ENCRYPTED);

            string aliceOmemoMsgText = aliceOmemoMessage.toXmlString();

            // Bob receives the message from Alice:
            messages = parser.parseMessages(ref aliceOmemoMsgText);
            Assert.IsTrue(messages.Count == 1);
            Assert.IsTrue(messages[0] is OmemoMessageMessage);
            OmemoMessageMessage bobOmemoMessage = messages[0] as OmemoMessageMessage;
            Assert.IsTrue(bobOmemoMessage.ENCRYPTED);
            Assert.AreEqual(bobOmemoMessage.SOURCE_DEVICE_ID, aliceOmemoMessage.SOURCE_DEVICE_ID);
            Assert.AreEqual(bobOmemoMessage.BASE_64_IV, aliceOmemoMessage.BASE_64_IV);
            Assert.AreEqual(bobOmemoMessage.BASE_64_PAYLOAD, aliceOmemoMessage.BASE_64_PAYLOAD);

            // Bob decrypts the message:
            SignalProtocolAddress aliceAddress = new SignalProtocolAddress(Utils.getBareJidFromFullJid(bobOmemoMessage.getFrom()), bobOmemoMessage.SOURCE_DEVICE_ID);
            SessionCipher bobSessionCipher = new SessionCipher(bobSessionStore, bobPreKeyStore, bobSignedPreKeyStore, bobIdentStore, aliceAddress);
            await bobOmemoMessage.decryptAsync(bobSessionCipher, BOB_ADDRESS.getDeviceId(), null);
            Assert.IsFalse(bobOmemoMessage.ENCRYPTED);
            Assert.AreEqual(aliceOrigMsg, bobOmemoMessage.MESSAGE);
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_OmemoBundleInformation_1()
        {
            IdentityKeyPair aliceIdentKey = CryptoUtils.generateOmemoIdentityKeyPair();
            IList<PreKeyRecord> alicePreKeys = KeyHelper.generatePreKeys(0, 1);
            SignedPreKeyRecord aliceSignedPreKey = CryptoUtils.generateOmemoSignedPreKey(aliceIdentKey);
            string bundleInfo = getBundleInfoMsg(aliceIdentKey, aliceSignedPreKey, alicePreKeys);

            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> messages = parser.parseMessages(ref bundleInfo);

            // Check if message parsed successfully:
            Assert.IsTrue(messages.Count == 1);
            Assert.IsInstanceOfType(messages[0], typeof(OmemoBundleInformationResultMessage));

            OmemoBundleInformationResultMessage bundleInfoMsg = messages[0] as OmemoBundleInformationResultMessage;

            // Check if keys match:
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.PUBLIC_IDENTITY_KEY.serialize().SequenceEqual(aliceIdentKey.getPublicKey().serialize()));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.PUBLIC_SIGNED_PRE_KEY.serialize().SequenceEqual(aliceSignedPreKey.getKeyPair().getPublicKey().serialize()));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.SIGNED_PRE_KEY_ID == aliceSignedPreKey.getId());
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.SIGNED_PRE_KEY_SIGNATURE.SequenceEqual(aliceSignedPreKey.getSignature()));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.PUBLIC_PRE_KEYS.Count == 1);
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.PUBLIC_PRE_KEYS[0].Item2.serialize().SequenceEqual(alicePreKeys[0].getKeyPair().getPublicKey().serialize()));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_OmemoBundleInformation_2()
        {
            IdentityKeyPair aliceIdentKey = CryptoUtils.generateOmemoIdentityKeyPair();
            IList<PreKeyRecord> alicePreKeys = CryptoUtils.generateOmemoPreKeys();
            SignedPreKeyRecord aliceSignedPreKey = CryptoUtils.generateOmemoSignedPreKey(aliceIdentKey);
            string bundleInfo = getBundleInfoMsg(aliceIdentKey, aliceSignedPreKey, alicePreKeys);

            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> messages = parser.parseMessages(ref bundleInfo);

            // Check if message parsed successfully:
            Assert.IsTrue(messages.Count == 1);
            Assert.IsInstanceOfType(messages[0], typeof(OmemoBundleInformationResultMessage));

            OmemoBundleInformationResultMessage bundleInfoMsg = messages[0] as OmemoBundleInformationResultMessage;

            // Check if keys match:
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.PUBLIC_IDENTITY_KEY.serialize().SequenceEqual(aliceIdentKey.getPublicKey().serialize()));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.PUBLIC_SIGNED_PRE_KEY.serialize().SequenceEqual(aliceSignedPreKey.getKeyPair().getPublicKey().serialize()));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.SIGNED_PRE_KEY_ID == aliceSignedPreKey.getId());
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.SIGNED_PRE_KEY_SIGNATURE.SequenceEqual(aliceSignedPreKey.getSignature()));

            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.PUBLIC_PRE_KEYS.Count == alicePreKeys.Count);
            foreach (PreKeyRecord key in alicePreKeys)
            {
                IEnumerable<Tuple<uint, ECPublicKey>> matches = bundleInfoMsg.BUNDLE_INFO.PUBLIC_PRE_KEYS.Where(x => x.Item1 == key.getId());
                Assert.IsTrue(matches.Count() == 1);
                byte[] a = matches.First().Item2.serialize();
                byte[] b = key.getKeyPair().getPublicKey().serialize();
                Assert.IsTrue(a.SequenceEqual(b));
            }
        }

        public string getDeviceListMsg()
        {
            StringBuilder sb = new StringBuilder("<iq xml:lang='en' to='");
            sb.Append(ALICE_ADDRESS.getName());
            sb.Append("/SOME_RESOURCE' from='");
            sb.Append(BOB_ADDRESS.getName());
            sb.Append("' type='result' id='150b3d7c-31cf-4217-b568-d4e9a19e1979'><pubsub xmlns='http://jabber.org/protocol/pubsub'><items node='eu.siacs.conversations.axolotl.devicelist'><item id='605CF956CF732'><list xmlns='eu.siacs.conversations.axolotl'>");
            sb.Append("<device id='");
            sb.Append(BOB_ADDRESS.getDeviceId());
            sb.Append("'/>");
            sb.Append("</list></item></items></pubsub></iq>");

            return sb.ToString();
        }

        public string getBundleInfoMsg(IdentityKeyPair identKey, SignedPreKeyRecord signedPreKey, IList<PreKeyRecord> preKeys)
        {
            IList<Tuple<uint, ECPublicKey>> publicPreKeys = new List<Tuple<uint, ECPublicKey>>();
            foreach (PreKeyRecord pk in preKeys)
            {
                publicPreKeys.Add(new Tuple<uint, ECPublicKey>(pk.getId(), pk.getKeyPair().getPublicKey()));
            }
            OmemoBundleInformation bundleInfo = new OmemoBundleInformation(identKey.getPublicKey(), signedPreKey.getKeyPair().getPublicKey(), signedPreKey.getId(), signedPreKey.getSignature(), publicPreKeys);

            StringBuilder sb = new StringBuilder("<iq xml:lang='en' to='");
            sb.Append(ALICE_ADDRESS.getName());
            sb.Append("/SOME_RESOURCE' from='");
            sb.Append(BOB_ADDRESS.getName());
            sb.Append("' type='result' id='83d5aa79-484b-4b76-83db-703f5cf60b57'><pubsub xmlns='http://jabber.org/protocol/pubsub'><items node='eu.siacs.conversations.axolotl.bundles:");
            sb.Append(BOB_ADDRESS.getDeviceId());
            sb.Append("'>");
            sb.Append(bundleInfo.toXElement(Consts.XML_XEP_0384_BUNDLE_INFO_NODE + BOB_ADDRESS.getDeviceId()));
            sb.Append("</items></pubsub></iq>");

            return sb.ToString();
        }
    }
}
