using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSec.Cryptography;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Shared.Classes;

namespace Component_Tests.Classes.Crypto.Omemo
{
    [TestClass]
    public class Test_KeyHelper
    {
        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_PreKeyGeneration()
        {
            for (uint count = 0; count < 250; count++)
            {
                List<PreKeyModel> keys = KeyHelper.GeneratePreKeys(1, count);
                Assert.AreEqual(keys.Count, (int)count);
            }

            // Rollover:
            for (uint count = 0; count < 10; count++)
            {
                List<PreKeyModel> keys = KeyHelper.GeneratePreKeys(0x7FFFFFFF, count);
                Assert.AreEqual(keys.Count, (int)count);
            }

            for (uint count = 0; count < 10; count++)
            {
                List<PreKeyModel> keys = KeyHelper.GeneratePreKeys(0x7FFFFFFF - count, 100);
                Assert.AreEqual(keys.Count, 100);
            }

            // Key IDs are unique:
            List<uint> ids = new List<uint>();
            List<PreKeyModel> keys2 = KeyHelper.GeneratePreKeys(0x7FFFFFFF - 100, 1000);
            for (int i = 0; i < keys2.Count; i++)
            {
                Assert.IsFalse(ids.Contains(keys2[i].keyId));
                ids.Add(keys2[i].keyId);
            }
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_SignPreKey()
        {
            ECPrivKeyModel priv = new ECPrivKeyModel(SharedUtils.HexStringToByteArray("1498b5467a63dffa2dc9d9e069caf075d16fc33fdd4c3b01bfadae6433767d93"));
            ECPubKeyModel pub = new ECPubKeyModel(SharedUtils.HexStringToByteArray("b7a3c12dc0c8c748ab07525b701122b88bd78f600c76342d27f25e5f92444cde"));
            IdentityKeyPairModel identityKeyPair = new IdentityKeyPairModel(priv, pub);

            for (uint id = 1; id < 250; id++)
            {
                byte[] data = Encoding.ASCII.GetBytes("Message for Ed25519 signing");
                byte[] signature = SignatureAlgorithm.Ed25519.Sign(Key.Import(SignatureAlgorithm.Ed25519, identityKeyPair.privKey.key, KeyBlobFormat.RawPrivateKey), data);
                byte[] sigRef = SharedUtils.HexStringToByteArray("6dd355667fae4eb43c6e0ab92e870edb2de0a88cae12dbd8591507f584fe4912babff497f1b8edf9567d2483d54ddc6459bea7855281b7a246a609e3001a4e08");
                string sigRefBase64 = Convert.ToBase64String(sigRef);
                string sigBase64 = Convert.ToBase64String(signature);
                Assert.AreEqual(sigBase64, sigRefBase64);
            }
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Signing()
        {
            ECPrivKeyModel identityPriv = new ECPrivKeyModel(SharedUtils.HexStringToByteArray("1498b5467a63dffa2dc9d9e069caf075d16fc33fdd4c3b01bfadae6433767d93"));
            ECPubKeyModel identityPub = new ECPubKeyModel(SharedUtils.HexStringToByteArray("05b7a3c12dc0c8c748ab07525b701122b88bd78f600c76342d27f25e5f92444cde"));

            ECPrivKeyModel prePriv = new ECPrivKeyModel(SharedUtils.HexStringToByteArray("181c0ed79c361f2d773f3aa8d5934569395a1c1b4a8514d140a7dcde92688579"));
            ECPubKeyModel prePub = new ECPubKeyModel(SharedUtils.HexStringToByteArray("05b30aad2471f7186bdb34951747cf81a67245144260e20ffe5bf7748202d6572c"));
            PreKeyModel preKey = new PreKeyModel(prePriv, prePub, 2);

            byte[] sig = KeyHelper.SignPreKey(preKey, identityPriv);
            string sig16 = SharedUtils.ToHexString(sig);
            Assert.IsTrue(KeyHelper.VerifySignature(identityPub, prePub, sig));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_VerifySignature()
        {
            ECPubKeyModel identityPub = new ECPubKeyModel(SharedUtils.HexStringToByteArray("05b7a3c12dc0c8c748ab07525b701122b88bd78f600c76342d27f25e5f92444cde"));
            ECPubKeyModel prePub = new ECPubKeyModel(SharedUtils.HexStringToByteArray("05b30aad2471f7186bdb34951747cf81a67245144260e20ffe5bf7748202d6572c"));
            byte[][] sigs = {
                SharedUtils.HexStringToByteArray("8de8513ec136ef909a594f6369c4a91dc7bd8a5a2e05b9ec6ceedb5c22316e105be4d22b855ce551fad31257ca0a22709b7ffd2d157afab0b9d01b4261c03b08"),
                    SharedUtils.HexStringToByteArray("8b5944b2e3ec089c5864b4d4f503836117fde77235b4587c0dee69c73157251295e6831675a9bed67e5cd33c0c4d3278920efdc8fc7ada975506153f7acaf809")
            };

            foreach (byte[] sig in sigs)
            {
                Assert.IsTrue(KeyHelper.VerifySignature(identityPub, prePub, sig));
            }
        }
    }
}
