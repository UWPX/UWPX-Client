using libsignal;
using libsignal.ecc;
using libsignal.protocol;
using libsignal.state;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XMPP_API.Classes.Crypto;

namespace Component_Tests.Classes.Crypto.Libsignal
{
    [TestClass]
    public class Test_Libsignal
    {
        private readonly SignalProtocolAddress ALICE_ADDRESS = new SignalProtocolAddress("alice@example.com", 1);
        private readonly SignalProtocolAddress BOB_ADDRESS = new SignalProtocolAddress("bob@example.com", 1);

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Libsignal_Enc_Dec_1()
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

            // Alice builds a session to Bob:
            SessionBuilder sessionBuilder = new SessionBuilder(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, BOB_ADDRESS);
            PreKeyBundle bobPreKey = new PreKeyBundle(BOB_ADDRESS.getDeviceId(), BOB_ADDRESS.getDeviceId(), bobPreKeys[0].getId(), bobPreKeys[0].getKeyPair().getPublicKey(), bobSignedPreKey.getId(), bobSignedPreKey.getKeyPair().getPublicKey(), bobSignedPreKey.getSignature(), bobIdentKey.getPublicKey());
            sessionBuilder.process(bobPreKey);

            // Check if session exists:
            Assert.IsTrue(aliceSessionStore.ContainsSession(BOB_ADDRESS));
            Assert.IsTrue(aliceSessionStore.LoadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);

            // Alice sends a message:
            string aliceOrigMsg = "$(rm -rvf .)";
            SessionCipher aliceSessionCipher = new SessionCipher(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, BOB_ADDRESS);
            CiphertextMessage aliceOutMsg = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(aliceOrigMsg));

            // Check if successfully encrypted:
            Assert.IsTrue(aliceOutMsg.getType() == CiphertextMessage.PREKEY_TYPE);

            // Bob receives the message:
            PreKeySignalMessage bobInMsg = new PreKeySignalMessage(aliceOutMsg.serialize());
            SessionCipher bobSessionCipher = new SessionCipher(bobSessionStore, bobPreKeyStore, bobSignedPreKeyStore, bobIdentStore, ALICE_ADDRESS);
            byte[] bobData = bobSessionCipher.decrypt(bobInMsg);
            string bobRecMsg = Encoding.UTF8.GetString(bobData);

            // Check if successfully send:
            Assert.AreEqual(aliceOrigMsg, bobRecMsg);
            Assert.IsTrue(bobSessionStore.ContainsSession(ALICE_ADDRESS));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Libsignal_Enc_Dec_2()
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

            // Alice builds a session to Bob:
            SessionBuilder sessionBuilder = new SessionBuilder(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, BOB_ADDRESS);
            PreKeyBundle bobPreKey = new PreKeyBundle(BOB_ADDRESS.getDeviceId(), BOB_ADDRESS.getDeviceId(), bobPreKeys[0].getId(), bobPreKeys[0].getKeyPair().getPublicKey(), bobSignedPreKey.getId(), bobSignedPreKey.getKeyPair().getPublicKey(), bobSignedPreKey.getSignature(), bobIdentKey.getPublicKey());
            sessionBuilder.process(bobPreKey);

            // Check if session exists:
            Assert.IsTrue(aliceSessionStore.ContainsSession(BOB_ADDRESS));
            Assert.IsTrue(aliceSessionStore.LoadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);

            // Alice sends a message:
            string aliceOrigMsg = "$(rm -rvf .)";
            SessionCipher aliceSessionCipher = new SessionCipher(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, BOB_ADDRESS);
            CiphertextMessage aliceOutMsg = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(aliceOrigMsg));

            // Check if successfully encrypted:
            Assert.IsTrue(aliceOutMsg.getType() == CiphertextMessage.PREKEY_TYPE);

            // Bob receives the message:
            PreKeySignalMessage bobInMsg = new PreKeySignalMessage(aliceOutMsg.serialize());
            SessionCipher bobSessionCipher = new SessionCipher(bobSessionStore, bobPreKeyStore, bobSignedPreKeyStore, bobIdentStore, ALICE_ADDRESS);
            byte[] bobData = bobSessionCipher.decrypt(bobInMsg);
            string bobRecMsg = Encoding.UTF8.GetString(bobData);

            // Check if successfully send:
            Assert.AreEqual(aliceOrigMsg, bobRecMsg);
            Assert.IsTrue(bobSessionStore.ContainsSession(ALICE_ADDRESS));

            //---------------------------Connection/App get restarted:---------------------------

            // Bob answers:
            string bobOrigMsg = ":(){ :|:& };:";
            // Simulate a chat break:
            bobSessionCipher = new SessionCipher(bobSessionStore, bobPreKeyStore, bobSignedPreKeyStore, bobIdentStore, ALICE_ADDRESS);
            CiphertextMessage bobOutMsg = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes(bobOrigMsg));

            // Alice receives the message:
            aliceSessionCipher = new SessionCipher(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, BOB_ADDRESS);
            SignalMessage aliceInMsg = new SignalMessage(bobOutMsg.serialize());
            byte[] aliceData = aliceSessionCipher.decrypt(aliceInMsg);
            string aliceRecMsg = Encoding.UTF8.GetString(aliceData);

            // Check if successfully send:
            Assert.AreEqual(bobOrigMsg, aliceRecMsg);
            Assert.IsTrue(bobSessionStore.ContainsSession(ALICE_ADDRESS));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Libsignal_Pre_key_Serialize_Deserialize_1()
        {
            IList<PreKeyRecord> alicePreKeys = CryptoUtils.generateOmemoPreKeys();
            byte[] pubKey = alicePreKeys[0].getKeyPair().getPublicKey().serialize();

            ECPublicKey key = Curve.decodePoint(pubKey, 0);
            Assert.IsTrue(key.serialize().SequenceEqual(pubKey));
        }
    }
}
