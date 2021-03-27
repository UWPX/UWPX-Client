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
            IdentityKeyPairModel aliceIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKeyModel> alicePreKeys = KeyHelper.GeneratePreKeys(0, 1);
            SignedPreKeyModel aliceSignedPreKey = KeyHelper.GenerateSignedPreKey(0, aliceIdentKey.privKey);
            Bundle aliceBundle = new Bundle()
            {
                identityKey = aliceIdentKey.pubKey,
                preKeys = alicePreKeys.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList(),
                preKeySignature = aliceSignedPreKey.signature,
                signedPreKey = aliceSignedPreKey.preKey.pubKey,
                signedPreKeyId = aliceSignedPreKey.preKey.keyId
            };
            InMemmoryOmemoStorage aliceStorage = new InMemmoryOmemoStorage();
            DoubleRachet aliceRachet = new DoubleRachet(aliceIdentKey);

            // Generate Bobs keys:
            IdentityKeyPairModel bobIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKeyModel> bobPreKeys = KeyHelper.GeneratePreKeys(0, 1);
            SignedPreKeyModel bobSignedPreKey = KeyHelper.GenerateSignedPreKey(0, bobIdentKey.privKey);
            Bundle bobBundle = new Bundle()
            {
                identityKey = bobIdentKey.pubKey,
                preKeys = bobPreKeys.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList(),
                preKeySignature = bobSignedPreKey.signature,
                signedPreKey = bobSignedPreKey.preKey.pubKey,
                signedPreKeyId = bobSignedPreKey.preKey.keyId
            };
            InMemmoryOmemoStorage bobStorage = new InMemmoryOmemoStorage();
            DoubleRachet bobRachet = new DoubleRachet(bobIdentKey);

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
            bobDeviceGroup.SESSIONS[BOB_ADDRESS.DEVICE_ID] = new OmemoSessionModel(bobBundle, 0, aliceIdentKey);
            bobDevices.Add(bobDeviceGroup);
            omemoEncryptedMessage.encrypt(ALICE_ADDRESS.DEVICE_ID, aliceIdentKey, aliceStorage, bobDevices);
            Assert.IsTrue(omemoEncryptedMessage.ENCRYPTED);

            // Decrypt:
            // Throws an exception in case something goes wrong:
            OmemoDecryptionContext decryptCtx = new OmemoDecryptionContext(BOB_ADDRESS, bobIdentKey, bobSignedPreKey, bobPreKeys, false, bobStorage);
            omemoEncryptedMessage.decrypt(decryptCtx);
            Assert.AreEqual(msg, omemoEncryptedMessage.MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage.IS_PURE_KEY_EXCHANGE_MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage.ENCRYPTED);
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Double_Rachet_Multiple_Sending()
        {
            // Generate Alices keys:
            IdentityKeyPairModel aliceIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKeyModel> alicePreKeys = KeyHelper.GeneratePreKeys(0, 1);
            SignedPreKeyModel aliceSignedPreKey = KeyHelper.GenerateSignedPreKey(0, aliceIdentKey.privKey);
            Bundle aliceBundle = new Bundle()
            {
                identityKey = aliceIdentKey.pubKey,
                preKeys = alicePreKeys.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList(),
                preKeySignature = aliceSignedPreKey.signature,
                signedPreKey = aliceSignedPreKey.preKey.pubKey,
                signedPreKeyId = aliceSignedPreKey.preKey.keyId
            };
            InMemmoryOmemoStorage aliceStorage = new InMemmoryOmemoStorage();
            DoubleRachet aliceRachet = new DoubleRachet(aliceIdentKey);

            // Generate Bobs keys:
            IdentityKeyPairModel bobIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKeyModel> bobPreKeys = KeyHelper.GeneratePreKeys(0, 1);
            SignedPreKeyModel bobSignedPreKey = KeyHelper.GenerateSignedPreKey(0, bobIdentKey.privKey);
            Bundle bobBundle = new Bundle()
            {
                identityKey = bobIdentKey.pubKey,
                preKeys = bobPreKeys.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList(),
                preKeySignature = bobSignedPreKey.signature,
                signedPreKey = bobSignedPreKey.preKey.pubKey,
                signedPreKeyId = bobSignedPreKey.preKey.keyId
            };
            InMemmoryOmemoStorage bobStorage = new InMemmoryOmemoStorage();
            DoubleRachet bobRachet = new DoubleRachet(bobIdentKey);

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

            // Encrypt Message 1:
            string msg1 = "Hello OMEMO";
            OmemoEncryptedMessage omemoEncryptedMessage = new OmemoEncryptedMessage(ALICE_ADDRESS.BARE_JID, BOB_ADDRESS.BARE_JID, msg1, MessageMessage.TYPE_CHAT, false);
            List<OmemoDeviceGroup> bobDevices = new List<OmemoDeviceGroup>();
            OmemoDeviceGroup bobDeviceGroup = new OmemoDeviceGroup(BOB_ADDRESS.BARE_JID);
            bobDeviceGroup.SESSIONS[BOB_ADDRESS.DEVICE_ID] = new OmemoSessionModel(bobBundle, 0, aliceIdentKey);
            bobDevices.Add(bobDeviceGroup);
            omemoEncryptedMessage.encrypt(ALICE_ADDRESS.DEVICE_ID, aliceIdentKey, aliceStorage, bobDevices);
            Assert.IsTrue(omemoEncryptedMessage.ENCRYPTED);

            // Decrypt Message 2:
            // Throws an exception in case something goes wrong:
            OmemoDecryptionContext decryptCtx1 = new OmemoDecryptionContext(BOB_ADDRESS, bobIdentKey, bobSignedPreKey, bobPreKeys, false, bobStorage);
            omemoEncryptedMessage.decrypt(decryptCtx1);
            Assert.AreEqual(msg1, omemoEncryptedMessage.MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage.IS_PURE_KEY_EXCHANGE_MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage.ENCRYPTED);

            // Encrypt Message 2:
            string msg2 = "Hello OMEMO 2";
            OmemoEncryptedMessage omemoEncryptedMessage2 = new OmemoEncryptedMessage(ALICE_ADDRESS.BARE_JID, BOB_ADDRESS.BARE_JID, msg2, MessageMessage.TYPE_CHAT, false);
            omemoEncryptedMessage2.encrypt(ALICE_ADDRESS.DEVICE_ID, aliceIdentKey, aliceStorage, bobDevices);
            Assert.IsTrue(omemoEncryptedMessage2.ENCRYPTED);

            // Decrypt Message 2:
            // Throws an exception in case something goes wrong:
            OmemoDecryptionContext decryptCtx2 = new OmemoDecryptionContext(BOB_ADDRESS, bobIdentKey, bobSignedPreKey, bobPreKeys, false, bobStorage);
            omemoEncryptedMessage2.decrypt(decryptCtx2);
            Assert.AreEqual(msg2, omemoEncryptedMessage2.MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage2.IS_PURE_KEY_EXCHANGE_MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage2.ENCRYPTED);
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Omemo_Send_Receive()
        {
            // Generate Alices keys:
            IdentityKeyPairModel aliceIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKeyModel> alicePreKeys = KeyHelper.GeneratePreKeys(0, 1);
            SignedPreKeyModel aliceSignedPreKey = KeyHelper.GenerateSignedPreKey(0, aliceIdentKey.privKey);
            Bundle aliceBundle = new Bundle()
            {
                identityKey = aliceIdentKey.pubKey,
                preKeys = alicePreKeys.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList(),
                preKeySignature = aliceSignedPreKey.signature,
                signedPreKey = aliceSignedPreKey.preKey.pubKey,
                signedPreKeyId = aliceSignedPreKey.preKey.keyId
            };
            InMemmoryOmemoStorage aliceStorage = new InMemmoryOmemoStorage();
            DoubleRachet aliceRachet = new DoubleRachet(aliceIdentKey);

            // Generate Bobs keys:
            IdentityKeyPairModel bobIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKeyModel> bobPreKeys = KeyHelper.GeneratePreKeys(0, 1);
            SignedPreKeyModel bobSignedPreKey = KeyHelper.GenerateSignedPreKey(0, bobIdentKey.privKey);
            Bundle bobBundle = new Bundle()
            {
                identityKey = bobIdentKey.pubKey,
                preKeys = bobPreKeys.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList(),
                preKeySignature = bobSignedPreKey.signature,
                signedPreKey = bobSignedPreKey.preKey.pubKey,
                signedPreKeyId = bobSignedPreKey.preKey.keyId
            };
            InMemmoryOmemoStorage bobStorage = new InMemmoryOmemoStorage();
            DoubleRachet bobRachet = new DoubleRachet(bobIdentKey);

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
            aliceStorage.StoreFingerprint(new OmemoFingerprint(bobBundle.identityKey, BOB_ADDRESS));

            // Encrypt Message 1:
            string msg1 = "Hello OMEMO";
            OmemoEncryptedMessage omemoEncryptedMessage = new OmemoEncryptedMessage(ALICE_ADDRESS.BARE_JID, BOB_ADDRESS.BARE_JID, msg1, MessageMessage.TYPE_CHAT, false);
            List<OmemoDeviceGroup> bobDevices = new List<OmemoDeviceGroup>();
            OmemoDeviceGroup bobDeviceGroup = new OmemoDeviceGroup(BOB_ADDRESS.BARE_JID);
            bobDeviceGroup.SESSIONS[BOB_ADDRESS.DEVICE_ID] = new OmemoSessionModel(bobBundle, 0, aliceIdentKey);
            bobDevices.Add(bobDeviceGroup);
            omemoEncryptedMessage.encrypt(ALICE_ADDRESS.DEVICE_ID, aliceIdentKey, aliceStorage, bobDevices);
            Assert.IsTrue(omemoEncryptedMessage.ENCRYPTED);
            Assert.IsNotNull(aliceStorage.LoadFingerprint(BOB_ADDRESS));
            Assert.IsNotNull(aliceStorage.LoadSession(BOB_ADDRESS));

            // Decrypt Message 1:
            // Throws an exception in case something goes wrong:
            OmemoDecryptionContext decryptCtx1 = new OmemoDecryptionContext(BOB_ADDRESS, bobIdentKey, bobSignedPreKey, bobPreKeys, false, bobStorage);
            omemoEncryptedMessage.decrypt(decryptCtx1);
            Assert.AreEqual(msg1, omemoEncryptedMessage.MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage.IS_PURE_KEY_EXCHANGE_MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage.ENCRYPTED);
            Assert.IsNotNull(bobStorage.LoadFingerprint(ALICE_ADDRESS));
            Assert.IsNotNull(bobStorage.LoadSession(ALICE_ADDRESS));

            // Encrypt Message 2:
            string msg2 = "Hello OMEMO 2";
            OmemoEncryptedMessage omemoEncryptedMessage2 = new OmemoEncryptedMessage(BOB_ADDRESS.BARE_JID, ALICE_ADDRESS.BARE_JID, msg2, MessageMessage.TYPE_CHAT, false);
            List<OmemoDeviceGroup> aliceDevices = new List<OmemoDeviceGroup>();
            OmemoDeviceGroup aliceDeviceGroup = new OmemoDeviceGroup(ALICE_ADDRESS.BARE_JID);
            aliceDeviceGroup.SESSIONS[ALICE_ADDRESS.DEVICE_ID] = bobStorage.LoadSession(ALICE_ADDRESS);
            aliceDevices.Add(aliceDeviceGroup);
            omemoEncryptedMessage2.encrypt(BOB_ADDRESS.DEVICE_ID, bobIdentKey, bobStorage, aliceDevices);
            Assert.IsTrue(omemoEncryptedMessage2.ENCRYPTED);

            // Decrypt Message 2:
            // Throws an exception in case something goes wrong:
            OmemoDecryptionContext decryptCtx2 = new OmemoDecryptionContext(ALICE_ADDRESS, aliceIdentKey, aliceSignedPreKey, alicePreKeys, false, aliceStorage);
            omemoEncryptedMessage2.decrypt(decryptCtx2);
            Assert.AreEqual(msg2, omemoEncryptedMessage2.MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage2.IS_PURE_KEY_EXCHANGE_MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage2.ENCRYPTED);
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Omemo_Send_Receive_Send()
        {
            // Generate Alices keys:
            IdentityKeyPairModel aliceIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKeyModel> alicePreKeys = KeyHelper.GeneratePreKeys(0, 1);
            SignedPreKeyModel aliceSignedPreKey = KeyHelper.GenerateSignedPreKey(0, aliceIdentKey.privKey);
            Bundle aliceBundle = new Bundle()
            {
                identityKey = aliceIdentKey.pubKey,
                preKeys = alicePreKeys.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList(),
                preKeySignature = aliceSignedPreKey.signature,
                signedPreKey = aliceSignedPreKey.preKey.pubKey,
                signedPreKeyId = aliceSignedPreKey.preKey.keyId
            };
            InMemmoryOmemoStorage aliceStorage = new InMemmoryOmemoStorage();
            DoubleRachet aliceRachet = new DoubleRachet(aliceIdentKey);

            // Generate Bobs keys:
            IdentityKeyPairModel bobIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKeyModel> bobPreKeys = KeyHelper.GeneratePreKeys(0, 1);
            SignedPreKeyModel bobSignedPreKey = KeyHelper.GenerateSignedPreKey(0, bobIdentKey.privKey);
            Bundle bobBundle = new Bundle()
            {
                identityKey = bobIdentKey.pubKey,
                preKeys = bobPreKeys.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList(),
                preKeySignature = bobSignedPreKey.signature,
                signedPreKey = bobSignedPreKey.preKey.pubKey,
                signedPreKeyId = bobSignedPreKey.preKey.keyId
            };
            InMemmoryOmemoStorage bobStorage = new InMemmoryOmemoStorage();
            DoubleRachet bobRachet = new DoubleRachet(bobIdentKey);

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
            aliceStorage.StoreFingerprint(new OmemoFingerprint(bobBundle.identityKey, BOB_ADDRESS));

            // Encrypt Message 1:
            string msg1 = "Hello OMEMO";
            OmemoEncryptedMessage omemoEncryptedMessage = new OmemoEncryptedMessage(ALICE_ADDRESS.BARE_JID, BOB_ADDRESS.BARE_JID, msg1, MessageMessage.TYPE_CHAT, false);
            List<OmemoDeviceGroup> bobDevices = new List<OmemoDeviceGroup>();
            OmemoDeviceGroup bobDeviceGroup = new OmemoDeviceGroup(BOB_ADDRESS.BARE_JID);
            bobDeviceGroup.SESSIONS[BOB_ADDRESS.DEVICE_ID] = new OmemoSessionModel(bobBundle, 0, aliceIdentKey);
            bobDevices.Add(bobDeviceGroup);
            omemoEncryptedMessage.encrypt(ALICE_ADDRESS.DEVICE_ID, aliceIdentKey, aliceStorage, bobDevices);
            Assert.IsTrue(omemoEncryptedMessage.ENCRYPTED);
            Assert.IsNotNull(aliceStorage.LoadFingerprint(BOB_ADDRESS));
            Assert.IsNotNull(aliceStorage.LoadSession(BOB_ADDRESS));

            // Decrypt Message 1:
            // Throws an exception in case something goes wrong:
            OmemoDecryptionContext decryptCtx1 = new OmemoDecryptionContext(BOB_ADDRESS, bobIdentKey, bobSignedPreKey, bobPreKeys, false, bobStorage);
            omemoEncryptedMessage.decrypt(decryptCtx1);
            Assert.AreEqual(msg1, omemoEncryptedMessage.MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage.IS_PURE_KEY_EXCHANGE_MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage.ENCRYPTED);
            Assert.IsNotNull(bobStorage.LoadFingerprint(ALICE_ADDRESS));
            Assert.IsNotNull(bobStorage.LoadSession(ALICE_ADDRESS));

            // Encrypt Message 2:
            string msg2 = "Hello OMEMO 2";
            OmemoEncryptedMessage omemoEncryptedMessage2 = new OmemoEncryptedMessage(BOB_ADDRESS.BARE_JID, ALICE_ADDRESS.BARE_JID, msg2, MessageMessage.TYPE_CHAT, false);
            List<OmemoDeviceGroup> aliceDevices = new List<OmemoDeviceGroup>();
            OmemoDeviceGroup aliceDeviceGroup = new OmemoDeviceGroup(ALICE_ADDRESS.BARE_JID);
            aliceDeviceGroup.SESSIONS[ALICE_ADDRESS.DEVICE_ID] = bobStorage.LoadSession(ALICE_ADDRESS);
            aliceDevices.Add(aliceDeviceGroup);
            omemoEncryptedMessage2.encrypt(BOB_ADDRESS.DEVICE_ID, bobIdentKey, bobStorage, aliceDevices);
            Assert.IsTrue(omemoEncryptedMessage2.ENCRYPTED);

            // Decrypt Message 2:
            // Throws an exception in case something goes wrong:
            OmemoDecryptionContext decryptCtx2 = new OmemoDecryptionContext(ALICE_ADDRESS, aliceIdentKey, aliceSignedPreKey, alicePreKeys, false, aliceStorage);
            omemoEncryptedMessage2.decrypt(decryptCtx2);
            Assert.AreEqual(msg2, omemoEncryptedMessage2.MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage2.IS_PURE_KEY_EXCHANGE_MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage2.ENCRYPTED);

            // Encrypt Message 3:
            string msg3 = "Hello OMEMO 3";
            OmemoEncryptedMessage omemoEncryptedMessage3 = new OmemoEncryptedMessage(ALICE_ADDRESS.BARE_JID, BOB_ADDRESS.BARE_JID, msg3, MessageMessage.TYPE_CHAT, false);
            bobDeviceGroup.SESSIONS[BOB_ADDRESS.DEVICE_ID] = aliceStorage.LoadSession(BOB_ADDRESS);
            omemoEncryptedMessage3.encrypt(ALICE_ADDRESS.DEVICE_ID, aliceIdentKey, aliceStorage, bobDevices);
            Assert.IsFalse(omemoEncryptedMessage3.IS_PURE_KEY_EXCHANGE_MESSAGE);
            Assert.IsTrue(omemoEncryptedMessage3.ENCRYPTED);
            Assert.IsNotNull(aliceStorage.LoadFingerprint(BOB_ADDRESS));
            Assert.IsNotNull(aliceStorage.LoadSession(BOB_ADDRESS));

            // Decrypt Message 3:
            // Throws an exception in case something goes wrong:
            OmemoDecryptionContext decryptCtx3 = new OmemoDecryptionContext(BOB_ADDRESS, bobIdentKey, bobSignedPreKey, bobPreKeys, false, bobStorage);
            omemoEncryptedMessage3.decrypt(decryptCtx3);
            Assert.AreEqual(msg3, omemoEncryptedMessage3.MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage3.IS_PURE_KEY_EXCHANGE_MESSAGE);
            Assert.IsFalse(omemoEncryptedMessage3.ENCRYPTED);
            Assert.IsNotNull(bobStorage.LoadFingerprint(ALICE_ADDRESS));
            Assert.IsNotNull(bobStorage.LoadSession(ALICE_ADDRESS));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_OmemoBundleInformation()
        {
            IdentityKeyPairModel bobIdentKey = KeyHelper.GenerateIdentityKeyPair();
            List<PreKeyModel> bobPreKeys = KeyHelper.GeneratePreKeys(0, 100);
            SignedPreKeyModel bobSignedPreKey = KeyHelper.GenerateSignedPreKey(0, bobIdentKey.privKey);
            Bundle bobBundle = new Bundle()
            {
                identityKey = bobIdentKey.pubKey,
                preKeys = bobPreKeys.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList(),
                preKeySignature = bobSignedPreKey.signature,
                signedPreKey = bobSignedPreKey.preKey.pubKey,
                signedPreKeyId = bobSignedPreKey.preKey.keyId
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
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.bundle.preKeys.SequenceEqual(bobPreKeys.Select(key => new PreKeyModel(null, key.pubKey, key.keyId)).ToList()));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.bundle.preKeySignature.SequenceEqual(bobSignedPreKey.signature));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.bundle.signedPreKey.Equals(bobSignedPreKey.preKey.pubKey));
            Assert.IsTrue(bundleInfoMsg.BUNDLE_INFO.bundle.signedPreKeyId == bobSignedPreKey.preKey.keyId);
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
            Assert.IsTrue(devicesMsg.DEVICES.DEVICES.First().label.Equals("Gajim on Ubuntu Linux"));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_OmemoMessage()
        {
            Random rand = new Random();
            byte[] sessionKey = new byte[32];
            rand.NextBytes(sessionKey);

            IdentityKeyPairModel identityKey = KeyHelper.GenerateIdentityKeyPair();
            SignedPreKeyModel signedPreKey = KeyHelper.GenerateSignedPreKey(0, identityKey.privKey);
            OmemoSessionModel refOmemoSession = new OmemoSessionModel(new Bundle()
            {
                identityKey = identityKey.pubKey,
                preKeys = KeyHelper.GeneratePreKeys(0, 10),
                preKeySignature = signedPreKey.signature,
                signedPreKey = signedPreKey.preKey.pubKey,
                signedPreKeyId = signedPreKey.preKey.keyId

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

            IdentityKeyPairModel identityKey = KeyHelper.GenerateIdentityKeyPair();
            SignedPreKeyModel signedPreKey = KeyHelper.GenerateSignedPreKey(0, identityKey.privKey);
            OmemoSessionModel refOmemoSession = new OmemoSessionModel(new Bundle()
            {
                identityKey = identityKey.pubKey,
                preKeys = KeyHelper.GeneratePreKeys(0, 10),
                preKeySignature = signedPreKey.signature,
                signedPreKey = signedPreKey.preKey.pubKey,
                signedPreKeyId = signedPreKey.preKey.keyId

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

            IdentityKeyPairModel identityKey = KeyHelper.GenerateIdentityKeyPair();
            SignedPreKeyModel signedPreKey = KeyHelper.GenerateSignedPreKey(0, identityKey.privKey);
            OmemoSessionModel refOmemoSession = new OmemoSessionModel(new Bundle()
            {
                identityKey = identityKey.pubKey,
                preKeys = KeyHelper.GeneratePreKeys(0, 10),
                preKeySignature = signedPreKey.signature,
                signedPreKey = signedPreKey.preKey.pubKey,
                signedPreKeyId = signedPreKey.preKey.keyId

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
