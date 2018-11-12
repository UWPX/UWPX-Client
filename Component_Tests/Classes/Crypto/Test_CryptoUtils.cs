using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
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
            byte[] saltBytes = CryptoUtils.hexStringToByteArray("4125c247e43ab1e93c6dff76");
            byte[] saltedPassword = CryptoUtils.Pbkdf2Sha1("pencil".Normalize(), saltBytes, 4096);
            byte[] saltedPasswordRef = CryptoUtils.hexStringToByteArray("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");
            Assert.IsTrue(saltedPassword.SequenceEqual(saltedPasswordRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HmacSha1_1()
        {
            byte[] saltedPassword = CryptoUtils.hexStringToByteArray("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");
            string authMessage = "Client Key";
            byte[] clientKey = CryptoUtils.HmacSha1(authMessage, saltedPassword);
            byte[] clientKeyRef = CryptoUtils.hexStringToByteArray("e234c47bf6c36696dd6d852b99aaa2ba26555728");
            Assert.IsTrue(clientKey.SequenceEqual(clientKeyRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HmacSha1_2()
        {
            byte[] storedKey = CryptoUtils.hexStringToByteArray("e9d94660c39d65c38fbad91c358f14da0eef2bd6");
            string authMessage = "n=user,r=fyko+d2lbbFgONRv9qkxdawL,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096,c=biws,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j";
            byte[] clientSignature = CryptoUtils.HmacSha1(authMessage, storedKey);
            byte[] clientSignatureRef = CryptoUtils.hexStringToByteArray("5d7138c486b0bfabdf49e3e2da8bd6e5c79db613");
            Assert.IsTrue(clientSignature.SequenceEqual(clientSignatureRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HmacSha1_3()
        {
            byte[] saltedPassword = CryptoUtils.hexStringToByteArray("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");
            string authMessage = "Server Key";
            byte[] serverKey = CryptoUtils.HmacSha1(authMessage, saltedPassword);
            byte[] serverKeyRef = CryptoUtils.hexStringToByteArray("0fe09258b3ac852ba502cc62ba903eaacdbf7d31");
            Assert.IsTrue(serverKey.SequenceEqual(serverKeyRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HmacSha1_4()
        {
            byte[] serverKey = CryptoUtils.hexStringToByteArray("0fe09258b3ac852ba502cc62ba903eaacdbf7d31");
            string authMessage = "n=user,r=fyko+d2lbbFgONRv9qkxdawL,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096,c=biws,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j";
            byte[] serverSignature = CryptoUtils.HmacSha1(authMessage, serverKey);
            byte[] serverSignatureRef = CryptoUtils.hexStringToByteArray("ae617da6a57c4bbb2e0286568dae1d251905b0a4");
            Assert.IsTrue(serverSignature.SequenceEqual(serverSignatureRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_HmacSha1_5()
        {
            // Values vectors from: https://tools.ietf.org/html/rfc2202
            byte[] key = CryptoUtils.hexStringToByteArray("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b");
            byte[] data = Encoding.UTF8.GetBytes("Hi There");
            byte[] digest = CryptoUtils.HmacSha1(data, key);
            byte[] digestRef = CryptoUtils.hexStringToByteArray("b617318655057264e28bc0b6fb378c8ef146be00");
            Assert.IsTrue(digest.SequenceEqual(digestRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_Xor()
        {
            byte[] clientKey = CryptoUtils.hexStringToByteArray("e234c47bf6c36696dd6d852b99aaa2ba26555728");
            byte[] clientSignature = CryptoUtils.hexStringToByteArray("5d7138c486b0bfabdf49e3e2da8bd6e5c79db613");

            byte[] clientProof = CryptoUtils.xor(clientKey, clientSignature);

            byte[] clientProofRef = CryptoUtils.hexStringToByteArray("bf45fcbf7073d93d022466c94321745fe1c8e13b");
            Assert.IsTrue(clientProof.SequenceEqual(clientProofRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_Sha1()
        {
            byte[] clientKey = CryptoUtils.hexStringToByteArray("e234c47bf6c36696dd6d852b99aaa2ba26555728");
            byte[] storedKey = CryptoUtils.SHA_1(clientKey);

            byte[] storedKeyRef = CryptoUtils.hexStringToByteArray("e9d94660c39d65c38fbad91c358f14da0eef2bd6");
            Assert.IsTrue(storedKey.SequenceEqual(storedKeyRef));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_CryptoUtils_ByteArrayHexString()
        {
            string input = "e234c47bf6c36696dd6d852b99aaa2ba26555728";
            byte[] a = CryptoUtils.hexStringToByteArray(input);
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
            byte[] saltRef = CryptoUtils.hexStringToByteArray(saltStringHexRef);
            Assert.IsTrue(salt.SequenceEqual(saltRef));

            string saltStringHex = CryptoUtils.byteArrayToHexString(salt);
            Assert.IsTrue(saltStringHex.Equals(saltStringHexRef));
        }
    }
}
