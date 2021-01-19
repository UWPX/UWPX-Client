using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Omemo.Classes.Messages;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Component_Tests.Classes.Crypto.Omemo
{
    [TestClass]
    public class Test_Omemo
    {
        private readonly OmemoProtocolAddress ALICE_ADDRESS = new OmemoProtocolAddress("alice@example.com", 1);
        private readonly OmemoProtocolAddress BOB_ADDRESS = new OmemoProtocolAddress("bob@example.com", 1);

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Double_Rachet()
        {
            // Generate Alices keys:
            IdentityKeyPair aliceIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKey> alicePreKeys = KeyHelper.GeneratePreKeys(0, 1);
            SignedPreKey aliceSignedPreKey = KeyHelper.GenerateSignedPreKey(0, aliceIdentKey.privKey);
            Bundle aliceBundle = new Bundle()
            {
                identityKey = aliceIdentKey.pubKey,
                preKeys = alicePreKeys.Select(key => new PreKey(null, key.pubKey, key.id)).ToList(),
                preKeySignature = aliceSignedPreKey.signature,
                signedPreKey = aliceSignedPreKey.preKey.pubKey,
                signedPreKeyId = aliceSignedPreKey.preKey.id
            };
            InMemmoryOmemoStorage aliceStorage = new InMemmoryOmemoStorage();
            DoubleRachet aliceRachet = new DoubleRachet(aliceIdentKey, aliceStorage);

            // Generate Bobs keys:
            IdentityKeyPair bobIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKey> bobPreKeys = KeyHelper.GeneratePreKeys(0, 1);
            SignedPreKey bobSignedPreKey = KeyHelper.GenerateSignedPreKey(0, bobIdentKey.privKey);
            Bundle bobBundle = new Bundle()
            {
                identityKey = bobIdentKey.pubKey,
                preKeys = bobPreKeys.Select(key => new PreKey(null, key.pubKey, key.id)).ToList(),
                preKeySignature = bobSignedPreKey.signature,
                signedPreKey = bobSignedPreKey.preKey.pubKey,
                signedPreKeyId = bobSignedPreKey.preKey.id
            };
            InMemmoryOmemoStorage bobStorage = new InMemmoryOmemoStorage();
            DoubleRachet bobRachet = new DoubleRachet(bobIdentKey, bobStorage);

            //-----------------OMEOMO Session Building:-----------------
            MessageParser2 parser = new MessageParser2();

            string deviceListMsg = GetBobsDeviceListMsg();
            List<AbstractMessage> messages = parser.parseMessages(ref deviceListMsg);
            Assert.IsTrue(messages.Count == 1);
            Assert.IsTrue(messages[0] is OmemoDeviceListResultMessage);
            OmemoDeviceListResultMessage devList = messages[0] as OmemoDeviceListResultMessage;

            uint selectedBobDeviceId = devList.DEVICES.getRandomDeviceId();
            Assert.IsTrue(selectedBobDeviceId == BOB_ADDRESS.DEVICE_ID);

            // Alice builds a session to Bob:
            string bundleInfoMsg = GetBobsBundleInfoMsg(bobBundle);
            messages = parser.parseMessages(ref bundleInfoMsg);
            Assert.IsTrue(messages.Count == 1);
            Assert.IsTrue(messages[0] is OmemoBundleInformationResultMessage);
            OmemoBundleInformationResultMessage bundleInfo = messages[0] as OmemoBundleInformationResultMessage;
            Assert.IsTrue(bundleInfo.BUNDLE_INFO.deviceId == BOB_ADDRESS.DEVICE_ID);

            // Encrypt:
            string msg = "Hello OMEMO";
            OmemoEncryptedMessage omemoEncryptedMessage = new OmemoEncryptedMessage(ALICE_ADDRESS.BARE_JID, BOB_ADDRESS.BARE_JID, msg, MessageMessage.TYPE_CHAT, false);
            List<OmemoDeviceGroup> bobDevices = new List<OmemoDeviceGroup>();
            OmemoDeviceGroup bobDeviceGroup = new OmemoDeviceGroup(BOB_ADDRESS.BARE_JID);
            bobDeviceGroup.SESSIONS[BOB_ADDRESS.DEVICE_ID] = new OmemoSession(bobBundle, 0, aliceIdentKey);
            bobDevices.Add(bobDeviceGroup);
            omemoEncryptedMessage.encrypt(ALICE_ADDRESS.DEVICE_ID, aliceIdentKey, aliceStorage, bobDevices);

            // Decrypt:
            Assert.IsTrue(omemoEncryptedMessage.decrypt(BOB_ADDRESS, bobIdentKey, bobSignedPreKey, bobPreKeys[0], bobStorage));
            Assert.AreEqual(msg, omemoEncryptedMessage.MESSAGE);
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_OmemoBundleInformation()
        {
            IdentityKeyPair bobIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKey> bobPreKeys = KeyHelper.GeneratePreKeys(0, 100);
            SignedPreKey bobSignedPreKey = KeyHelper.GenerateSignedPreKey(0, bobIdentKey.privKey);
            Bundle bobBundle = new Bundle()
            {
                identityKey = bobIdentKey.pubKey,
                preKeys = bobPreKeys.Select(key => new PreKey(null, key.pubKey, key.id)).ToList(),
                preKeySignature = bobSignedPreKey.signature,
                signedPreKey = bobSignedPreKey.preKey.pubKey,
                signedPreKeyId = bobSignedPreKey.preKey.id
            };
            string bundleInfo = GetBobsBundleInfoMsg(bobBundle);

            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> messages = parser.parseMessages(ref bundleInfo);

            // Check if message parsed successfully:
            Assert.IsTrue(messages.Count == 1);
            Assert.IsInstanceOfType(messages[0], typeof(OmemoBundleInformationResultMessage));

            OmemoBundleInformationResultMessage bundleInfoMsg = messages[0] as OmemoBundleInformationResultMessage;

            // Check if keys match:
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.bundle.identityKey.Equals(bobIdentKey.pubKey));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.bundle.preKeys.SequenceEqual(bobPreKeys.Select(key => new PreKey(null, key.pubKey, key.id)).ToList()));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.bundle.preKeySignature.SequenceEqual(bobSignedPreKey.signature));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.bundle.signedPreKey.Equals(bobSignedPreKey.preKey.pubKey));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.bundle.signedPreKeyId == bobSignedPreKey.preKey.id);
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_OmemoDevices()
        {
            string msg = GetBobsDeviceListMsg();

            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> messages = parser.parseMessages(ref msg);

            // Check if message parsed successfully:
            Assert.IsTrue(messages.Count == 1);
            Assert.IsInstanceOfType(messages[0], typeof(OmemoDeviceListResultMessage));

            OmemoDeviceListResultMessage devicesMsg = messages[0] as OmemoDeviceListResultMessage;

            // Check if keys match:
            Assert.IsTrue(devicesMsg.DEVICES.DEVICES.Count == 1);
            Assert.IsTrue(devicesMsg.DEVICES.DEVICES.First().IS_VALID);
            Assert.IsTrue(devicesMsg.DEVICES.DEVICES.First().ID == BOB_ADDRESS.DEVICE_ID);
            Assert.IsTrue(devicesMsg.DEVICES.DEVICES.First().LABEL.Equals("Gajim on Ubuntu Linux"));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_OmemoMessage()
        {
            Random rand = new Random();
            byte[] sessionKey = new byte[32];
            rand.NextBytes(sessionKey);

            IdentityKeyPair identityKey = KeyHelper.GenerateIdentityKeyPair();
            SignedPreKey signedPreKey = KeyHelper.GenerateSignedPreKey(0, identityKey.privKey);
            OmemoSession refOmemoSession = new OmemoSession(new Bundle()
            {
                identityKey = identityKey.pubKey,
                preKeys = KeyHelper.GeneratePreKeys(0, 10),
                preKeySignature = signedPreKey.signature,
                signedPreKey = signedPreKey.preKey.pubKey,
                signedPreKeyId = signedPreKey.preKey.id

            }, 0, KeyHelper.GenerateIdentityKeyPair())
            {
                ek = KeyHelper.GenerateKeyPair().pubKey
            };
            OmemoMessage refOmemoMessage = new OmemoMessage(refOmemoSession);

            byte[] data = refOmemoMessage.ToByteArray();
            Assert.IsTrue(data.Length > 0);

            OmemoMessage omemoMessage = new OmemoMessage(data);
            Assert.IsTrue(omemoMessage.Equals(refOmemoMessage));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_OmemoAuthenticatedMessage()
        {
            Random rand = new Random();
            byte[] hmac = new byte[16];
            rand.NextBytes(hmac);
            byte[] sessionKey = new byte[32];
            rand.NextBytes(sessionKey);

            IdentityKeyPair identityKey = KeyHelper.GenerateIdentityKeyPair();
            SignedPreKey signedPreKey = KeyHelper.GenerateSignedPreKey(0, identityKey.privKey);
            OmemoSession refOmemoSession = new OmemoSession(new Bundle()
            {
                identityKey = identityKey.pubKey,
                preKeys = KeyHelper.GeneratePreKeys(0, 10),
                preKeySignature = signedPreKey.signature,
                signedPreKey = signedPreKey.preKey.pubKey,
                signedPreKeyId = signedPreKey.preKey.id

            }, 0, KeyHelper.GenerateIdentityKeyPair())
            {
                ek = KeyHelper.GenerateKeyPair().pubKey
            };
            OmemoMessage refOmemoMessage = new OmemoMessage(refOmemoSession);
            OmemoAuthenticatedMessage refOmemoAuthenticatedMessage = new OmemoAuthenticatedMessage(new byte[16], refOmemoMessage.ToByteArray());

            byte[] data = refOmemoAuthenticatedMessage.ToByteArray();
            Assert.IsTrue(data.Length > 0);

            OmemoAuthenticatedMessage omemoAuthenticatedMessage = new OmemoAuthenticatedMessage(data);
            Assert.IsTrue(omemoAuthenticatedMessage.Equals(refOmemoAuthenticatedMessage));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_OmemoKeyExchangeMessage()
        {
            Random rand = new Random();
            byte[] hmac = new byte[16];
            rand.NextBytes(hmac);
            byte[] sessionKey = new byte[32];
            rand.NextBytes(sessionKey);

            IdentityKeyPair identityKey = KeyHelper.GenerateIdentityKeyPair();
            SignedPreKey signedPreKey = KeyHelper.GenerateSignedPreKey(0, identityKey.privKey);
            OmemoSession refOmemoSession = new OmemoSession(new Bundle()
            {
                identityKey = identityKey.pubKey,
                preKeys = KeyHelper.GeneratePreKeys(0, 10),
                preKeySignature = signedPreKey.signature,
                signedPreKey = signedPreKey.preKey.pubKey,
                signedPreKeyId = signedPreKey.preKey.id

            }, 0, KeyHelper.GenerateIdentityKeyPair())
            {
                ek = KeyHelper.GenerateKeyPair().pubKey
            };
            OmemoMessage refOmemoMessage = new OmemoMessage(refOmemoSession);
            OmemoAuthenticatedMessage refOmemoAuthenticatedMessage = new OmemoAuthenticatedMessage(new byte[16], refOmemoMessage.ToByteArray());
            OmemoKeyExchangeMessage refOmemoKeyExchangeMessage = new OmemoKeyExchangeMessage((uint)rand.Next(), (uint)rand.Next(), KeyHelper.GenerateKeyPair().pubKey, KeyHelper.GenerateKeyPair().pubKey, refOmemoAuthenticatedMessage);

            byte[] data = refOmemoKeyExchangeMessage.ToByteArray();
            Assert.IsTrue(data.Length > 0);

            OmemoKeyExchangeMessage omemoKeyExchangeMessage = new OmemoKeyExchangeMessage(data);
            Assert.IsTrue(omemoKeyExchangeMessage.Equals(refOmemoKeyExchangeMessage));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_AesCbc()
        {
            byte[] key = KeyHelper.GenerateSymetricKey();
            byte[] iv = CryptoUtils.NextBytesSecureRandom(16);
            byte[] plainTextRef = CryptoUtils.NextBytesSecureRandom(1024);
            byte[] ciperText = CryptoUtils.Aes256CbcEncrypt(key, iv, plainTextRef);
            byte[] plainText = CryptoUtils.Aes256CbcDecrypt(key, iv, ciperText);
            Assert.IsTrue(plainText.SequenceEqual(plainTextRef));
        }

        public string GetBobsDeviceListMsg()
        {
            StringBuilder sb = new StringBuilder("<iq xml:lang='en' to='");
            sb.Append(ALICE_ADDRESS.BARE_JID);
            sb.Append("/SOME_RESOURCE' from='");
            sb.Append(BOB_ADDRESS.BARE_JID);
            sb.Append("' type='result' id='150b3d7c-31cf-4217-b568-d4e9a19e1979'><pubsub xmlns='http://jabber.org/protocol/pubsub'><items node='" + XMPP_API.Classes.Consts.XML_XEP_0384_DEVICE_LIST_NODE + "'><item id='current'><devices xmlns='" + XMPP_API.Classes.Consts.XML_XEP_0384_NAMESPACE + "'>");
            sb.Append("<device id='");
            sb.Append(BOB_ADDRESS.DEVICE_ID);
            sb.Append("' label='Gajim on Ubuntu Linux'/>");
            sb.Append("</devices></item></items></pubsub></iq>");
            return sb.ToString();
        }

        public string GetBobsBundleInfoMsg(Bundle bundle)
        {
            OmemoBundleInformation bundleInfo = new OmemoBundleInformation(bundle, BOB_ADDRESS.DEVICE_ID);
            StringBuilder sb = new StringBuilder("<iq xml:lang='en' to='");
            sb.Append(ALICE_ADDRESS.BARE_JID);
            sb.Append("/SOME_RESOURCE' from='");
            sb.Append(BOB_ADDRESS.BARE_JID);
            sb.Append("' type='result' id='83d5aa79-484b-4b76-83db-703f5cf60b57'><pubsub xmlns='http://jabber.org/protocol/pubsub'><items node='" + XMPP_API.Classes.Consts.XML_XEP_0384_BUNDLES_NODE + "'>");
            sb.Append(bundleInfo.toXElement(XMPP_API.Classes.Consts.XML_XEP_0384_BUNDLES_NODE));
            sb.Append("</items></pubsub></iq>");
            return sb.ToString();
        }
    }
}
