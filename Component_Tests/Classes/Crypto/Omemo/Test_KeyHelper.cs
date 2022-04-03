using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Omemo.Classes;
using Omemo.Classes.Keys;
using Org.BouncyCastle.Math.EC.Rfc8032;
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

                byte[] signature = new byte[Ed25519.SignatureSize];
                Ed25519.Sign(identityKeyPair.privKey.key, 0, data, 0, data.Length, signature, 0);
                byte[] sigRef = SharedUtils.HexStringToByteArray("6dd355667fae4eb43c6e0ab92e870edb2de0a88cae12dbd8591507f584fe4912babff497f1b8edf9567d2483d54ddc6459bea7855281b7a246a609e3001a4e08");
                string sigRefBase64 = Convert.ToBase64String(sigRef);
                string sigBase64 = Convert.ToBase64String(signature);
                Assert.AreEqual(sigBase64, sigRefBase64);
            }
        }
    }
}
