using libsignal;
using libsignal.protocol;
using libsignal.state;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using XMPP_API.Classes.Crypto;

namespace Component_Tests.Classes.Crypto.Libsignal
{
    [TestClass]
    public class Test_Libsignal
    {
        SignalProtocolAddress aliceAddress = new SignalProtocolAddress("alice@example.com", 1);
        SignalProtocolAddress bobAddress = new SignalProtocolAddress("bob@example.com", 1);

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Libsignal_Enc_Dec_1()
        {
            // Generate Alices keys:
            IdentityKeyPair aliceIdentKey = CryptoUtils.generateOmemoIdentityKeyPair();
            uint aliceRegId = CryptoUtils.generateOmemoDeviceId();
            IList<PreKeyRecord> alicePreKeys = CryptoUtils.generateOmemoPreKeys();
            SignedPreKeyRecord aliceSignedPreKey = CryptoUtils.generateOmemoSignedPreKey(aliceIdentKey);

            // Create Alices stores:
            InMemoryIdentityKeyStore aliceIdentStore = new InMemoryIdentityKeyStore(aliceIdentKey, aliceRegId);
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
            uint bobRegId = CryptoUtils.generateOmemoDeviceId();
            IList<PreKeyRecord> bobPreKeys = CryptoUtils.generateOmemoPreKeys();
            SignedPreKeyRecord bobSignedPreKey = CryptoUtils.generateOmemoSignedPreKey(bobIdentKey);

            // Create Bobs stores:
            InMemoryIdentityKeyStore bobIdentStore = new InMemoryIdentityKeyStore(bobIdentKey, bobRegId);
            InMemoryPreKeyStore bobPreKeyStore = new InMemoryPreKeyStore();
            foreach (PreKeyRecord key in bobPreKeys)
            {
                bobPreKeyStore.StorePreKey(key.getId(), key);
            }
            InMemorySignedPreKeyStore bobSignedPreKeyStore = new InMemorySignedPreKeyStore();
            bobSignedPreKeyStore.StoreSignedPreKey(bobSignedPreKey.getId(), bobSignedPreKey);
            InMemorySessionStore bobSessionStore = new InMemorySessionStore();

            // Alice builds a session to Bob:
            SessionBuilder sessionBuilder = new SessionBuilder(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, bobAddress);
            PreKeyBundle bobPreKey = new PreKeyBundle(bobRegId, bobAddress.getDeviceId(), bobPreKeys[0].getId(), bobPreKeys[0].getKeyPair().getPublicKey(), bobSignedPreKey.getId(), bobSignedPreKey.getKeyPair().getPublicKey(), bobSignedPreKey.getSignature(), bobIdentKey.getPublicKey());
            sessionBuilder.process(bobPreKey);

            // Check if session exists:
            Assert.IsTrue(aliceSessionStore.ContainsSession(bobAddress));
            Assert.IsTrue(aliceSessionStore.LoadSession(bobAddress).getSessionState().getSessionVersion() == 3);

            // Alice sends a message:
            string aliceOrigMsg = "$(rm -rvf .)";
            SessionCipher aliceSessionCipher = new SessionCipher(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, bobAddress);
            CiphertextMessage aliceOutMsg = aliceSessionCipher.encrypt(Encoding.Unicode.GetBytes(aliceOrigMsg));

            // Check if successfully encrypted:
            Assert.IsTrue(aliceOutMsg.getType() == CiphertextMessage.PREKEY_TYPE);

            // Bob receives the message:
            PreKeySignalMessage bobInMsg = new PreKeySignalMessage(aliceOutMsg.serialize());
            SessionCipher bobSessionCipher = new SessionCipher(bobSessionStore, bobPreKeyStore, bobSignedPreKeyStore, bobIdentStore, aliceAddress);
            byte[] bobData = bobSessionCipher.decrypt(bobInMsg);
            string bobRecMsg = Encoding.Unicode.GetString(bobData);

            // Check if successfully send:
            Assert.AreEqual(aliceOrigMsg, bobRecMsg);
            Assert.IsTrue(bobSessionStore.ContainsSession(aliceAddress));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Libsignal_Enc_Dec_2()
        {
            // Generate Alices keys:
            IdentityKeyPair aliceIdentKey = CryptoUtils.generateOmemoIdentityKeyPair();
            uint aliceRegId = CryptoUtils.generateOmemoDeviceId();
            IList<PreKeyRecord> alicePreKeys = CryptoUtils.generateOmemoPreKeys();
            SignedPreKeyRecord aliceSignedPreKey = CryptoUtils.generateOmemoSignedPreKey(aliceIdentKey);

            // Create Alices stores:
            InMemoryIdentityKeyStore aliceIdentStore = new InMemoryIdentityKeyStore(aliceIdentKey, aliceRegId);
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
            uint bobRegId = CryptoUtils.generateOmemoDeviceId();
            IList<PreKeyRecord> bobPreKeys = CryptoUtils.generateOmemoPreKeys();
            SignedPreKeyRecord bobSignedPreKey = CryptoUtils.generateOmemoSignedPreKey(bobIdentKey);

            // Create Bobs stores:
            InMemoryIdentityKeyStore bobIdentStore = new InMemoryIdentityKeyStore(bobIdentKey, bobRegId);
            InMemoryPreKeyStore bobPreKeyStore = new InMemoryPreKeyStore();
            foreach (PreKeyRecord key in bobPreKeys)
            {
                bobPreKeyStore.StorePreKey(key.getId(), key);
            }
            InMemorySignedPreKeyStore bobSignedPreKeyStore = new InMemorySignedPreKeyStore();
            bobSignedPreKeyStore.StoreSignedPreKey(bobSignedPreKey.getId(), bobSignedPreKey);
            InMemorySessionStore bobSessionStore = new InMemorySessionStore();

            // Alice builds a session to Bob:
            SessionBuilder sessionBuilder = new SessionBuilder(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, bobAddress);
            PreKeyBundle bobPreKey = new PreKeyBundle(bobRegId, bobAddress.getDeviceId(), bobPreKeys[0].getId(), bobPreKeys[0].getKeyPair().getPublicKey(), bobSignedPreKey.getId(), bobSignedPreKey.getKeyPair().getPublicKey(), bobSignedPreKey.getSignature(), bobIdentKey.getPublicKey());
            sessionBuilder.process(bobPreKey);

            // Check if session exists:
            Assert.IsTrue(aliceSessionStore.ContainsSession(bobAddress));
            Assert.IsTrue(aliceSessionStore.LoadSession(bobAddress).getSessionState().getSessionVersion() == 3);

            // Alice sends a message:
            string aliceOrigMsg = "$(rm -rvf .)";
            SessionCipher aliceSessionCipher = new SessionCipher(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, bobAddress);
            CiphertextMessage aliceOutMsg = aliceSessionCipher.encrypt(Encoding.Unicode.GetBytes(aliceOrigMsg));

            // Check if successfully encrypted:
            Assert.IsTrue(aliceOutMsg.getType() == CiphertextMessage.PREKEY_TYPE);

            // Bob receives the message:
            PreKeySignalMessage bobInMsg = new PreKeySignalMessage(aliceOutMsg.serialize());
            SessionCipher bobSessionCipher = new SessionCipher(bobSessionStore, bobPreKeyStore, bobSignedPreKeyStore, bobIdentStore, aliceAddress);
            byte[] bobData = bobSessionCipher.decrypt(bobInMsg);
            string bobRecMsg = Encoding.Unicode.GetString(bobData);

            // Check if successfully send:
            Assert.AreEqual(aliceOrigMsg, bobRecMsg);
            Assert.IsTrue(bobSessionStore.ContainsSession(aliceAddress));

            //---------------------------Connection/App get restarted:---------------------------

            // Bob answers:
            string bobOrigMsg = ":(){ :|:& };:";
            // Simulate a chat break:
            bobSessionCipher = new SessionCipher(bobSessionStore, bobPreKeyStore, bobSignedPreKeyStore, bobIdentStore, aliceAddress);
            CiphertextMessage bobOutMsg = bobSessionCipher.encrypt(Encoding.Unicode.GetBytes(bobOrigMsg));

            // Alice receives the message:
            aliceSessionCipher = new SessionCipher(aliceSessionStore, alicePreKeyStore, aliceSignedPreKeyStore, aliceIdentStore, bobAddress);
            SignalMessage aliceInMsg = new SignalMessage(bobOutMsg.serialize());
            byte[] aliceData = aliceSessionCipher.decrypt(aliceInMsg);
            string aliceRecMsg = Encoding.Unicode.GetString(aliceData);

            // Check if successfully send:
            Assert.AreEqual(bobOrigMsg, aliceRecMsg);
            Assert.IsTrue(bobSessionStore.ContainsSession(aliceAddress));
        }
    }
}
