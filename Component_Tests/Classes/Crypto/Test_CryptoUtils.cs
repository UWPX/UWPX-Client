using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Omemo.Classes.Keys;
using Shared.Classes;
using Windows.Security.Cryptography.Core;
using XMPP_API.Classes.Crypto;

namespace Component_Tests.Classes.Crypto
{
    [TestClass]
    public class Test_CryptoUtils
    {
        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_Pbkdf2Sha_1()
        {
            byte[] saltBytes = SharedUtils.HexStringToByteArray("4125c247e43ab1e93c6dff76");
            byte[] saltedPassword = CryptoUtils.pbkdf2Sha("pencil".Normalize(), saltBytes, 4096, HashAlgorithmName.SHA1, 20);
            byte[] saltedPasswordRef = SharedUtils.HexStringToByteArray("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");
            Assert.IsTrue(saltedPassword.SequenceEqual(saltedPasswordRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HmacSha1_1()
        {
            byte[] saltedPassword = SharedUtils.HexStringToByteArray("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");
            string authMessage = "Client Key";
            byte[] clientKey = CryptoUtils.hmacSha1(authMessage, saltedPassword);
            byte[] clientKeyRef = SharedUtils.HexStringToByteArray("e234c47bf6c36696dd6d852b99aaa2ba26555728");
            Assert.IsTrue(clientKey.SequenceEqual(clientKeyRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HmacSha1_2()
        {
            byte[] storedKey = SharedUtils.HexStringToByteArray("e9d94660c39d65c38fbad91c358f14da0eef2bd6");
            string authMessage = "n=user,r=fyko+d2lbbFgONRv9qkxdawL,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096,c=biws,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j";
            byte[] clientSignature = CryptoUtils.hmacSha1(authMessage, storedKey);
            byte[] clientSignatureRef = SharedUtils.HexStringToByteArray("5d7138c486b0bfabdf49e3e2da8bd6e5c79db613");
            Assert.IsTrue(clientSignature.SequenceEqual(clientSignatureRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HmacSha1_3()
        {
            byte[] saltedPassword = SharedUtils.HexStringToByteArray("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");
            string authMessage = "Server Key";
            byte[] serverKey = CryptoUtils.hmacSha1(authMessage, saltedPassword);
            byte[] serverKeyRef = SharedUtils.HexStringToByteArray("0fe09258b3ac852ba502cc62ba903eaacdbf7d31");
            Assert.IsTrue(serverKey.SequenceEqual(serverKeyRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HmacSha1_4()
        {
            byte[] serverKey = SharedUtils.HexStringToByteArray("0fe09258b3ac852ba502cc62ba903eaacdbf7d31");
            string authMessage = "n=user,r=fyko+d2lbbFgONRv9qkxdawL,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096,c=biws,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j";
            byte[] serverSignature = CryptoUtils.hmacSha1(authMessage, serverKey);
            byte[] serverSignatureRef = SharedUtils.HexStringToByteArray("ae617da6a57c4bbb2e0286568dae1d251905b0a4");
            Assert.IsTrue(serverSignature.SequenceEqual(serverSignatureRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HmacSha1_5()
        {
            // Values vectors from: https://tools.ietf.org/html/rfc2202
            byte[] key = SharedUtils.HexStringToByteArray("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b");
            byte[] data = Encoding.UTF8.GetBytes("Hi There");
            byte[] digest = CryptoUtils.hmacSha1(data, key);
            byte[] digestRef = SharedUtils.HexStringToByteArray("b617318655057264e28bc0b6fb378c8ef146be00");
            Assert.IsTrue(digest.SequenceEqual(digestRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_Xor()
        {
            byte[] clientKey = SharedUtils.HexStringToByteArray("e234c47bf6c36696dd6d852b99aaa2ba26555728");
            byte[] clientSignature = SharedUtils.HexStringToByteArray("5d7138c486b0bfabdf49e3e2da8bd6e5c79db613");

            byte[] clientProof = CryptoUtils.xor(clientKey, clientSignature);

            byte[] clientProofRef = SharedUtils.HexStringToByteArray("bf45fcbf7073d93d022466c94321745fe1c8e13b");
            Assert.IsTrue(clientProof.SequenceEqual(clientProofRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_hash()
        {
            byte[] clientKey = SharedUtils.HexStringToByteArray("e234c47bf6c36696dd6d852b99aaa2ba26555728");
            byte[] storedKey = CryptoUtils.hash(clientKey, HashAlgorithmNames.Sha1);

            byte[] storedKeyRef = SharedUtils.HexStringToByteArray("e9d94660c39d65c38fbad91c358f14da0eef2bd6");
            Assert.IsTrue(storedKey.SequenceEqual(storedKeyRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_ByteArrayHexString()
        {
            string input = "e234c47bf6c36696dd6d852b99aaa2ba26555728";
            byte[] a = SharedUtils.HexStringToByteArray(input);
            string output = CryptoUtils.byteArrayToHexString(a);
            Assert.IsTrue(input.Equals(output));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HexToByteArray()
        {
            string saltStringBase64Ref = "QSXCR+Q6sek8bf92";
            string saltStringHexRef = "4125c247e43ab1e93c6dff76";

            byte[] salt = Convert.FromBase64String(saltStringBase64Ref);
            byte[] saltRef = SharedUtils.HexStringToByteArray(saltStringHexRef);
            Assert.IsTrue(salt.SequenceEqual(saltRef));

            string saltStringHex = CryptoUtils.byteArrayToHexString(salt);
            Assert.IsTrue(saltStringHex.Equals(saltStringHexRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_GenOmemoFingerprint()
        {
            string publicIdentKeySerializedHex = "3b6a27bcceb6a42d62a3a8d02a6f0d73653215771de243a63ac048a18b59da29";
            byte[] publicIdentKeySerialized = SharedUtils.HexStringToByteArray(publicIdentKeySerializedHex);
            ECPubKeyModel identKeyPair = new ECPubKeyModel(publicIdentKeySerialized);

            string outputRef = "3b6a27bc ceb6a42d 62a3a8d0 2a6f0d73 65321577 1de243a6 3ac048a1 8b59da29";
            string output = CryptoUtils.generateOmemoFingerprint(identKeyPair.key);

            Assert.AreEqual(outputRef, output);
        }
    }
}
