using Microsoft.VisualStudio.TestTools.UnitTesting;
using Omemo.Classes.Keys;
using Shared.Classes;

namespace Component_Tests.Classes.Crypto.Omemo
{
    [TestClass]
    public class Test_Xeddsa25519
    {
        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Signing()
        {
            ECPrivKeyModel priv = new ECPrivKeyModel(SharedUtils.HexStringToByteArray("1498b5467a63dffa2dc9d9e069caf075d16fc33fdd4c3b01bfadae6433767d93"));
            ECPubKeyModel pub = new ECPubKeyModel(SharedUtils.HexStringToByteArray("b7a3c12dc0c8c748ab07525b701122b88bd78f600c76342d27f25e5f92444cde"));
            IdentityKeyPairModel identityKeyPair = new IdentityKeyPairModel(priv, pub);
            // Xeddsa25519.Sign(identityKeyPair.privKey, pub new );
        }
    }
}
