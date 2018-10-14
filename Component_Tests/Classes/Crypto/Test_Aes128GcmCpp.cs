using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using XMPP_API.Classes.Crypto;

namespace Component_Tests.Classes.Crypto
{
    [TestClass]
    public class Test_Aes128GcmCpp
    {
        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Aes128GcmCpp_Enc_Dec_1()
        {
            for (int i = 0; i < 100; i++)
            {
                Aes128GcmCpp aesEnc = new Aes128GcmCpp();
                aesEnc.generateKey();
                aesEnc.generateIv();
                Random r = new Random();
                byte[] data = new byte[200];
                r.NextBytes(data);
                byte[] dataEnc = aesEnc.encrypt(data);

                Aes128GcmCpp aesDec = new Aes128GcmCpp
                {
                    authTag = aesEnc.authTag,
                    iv = aesEnc.iv,
                    key = aesEnc.key,
                };
                byte[] dataDec = aesDec.decrypt(dataEnc);
                Assert.IsTrue(data.SequenceEqual(dataDec));
            }
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Aes128GcmCpp_Enc_Dec_2()
        {
            // Test vectors from: https://git.openssl.org/gitweb/?p=openssl.git;a=commitdiff;h=6b7b34330beaf7ff062d6a06b7733f069d77feaa
            Aes128GcmCpp aesEnc = new Aes128GcmCpp
            {
                key = CryptoUtils.hexStringToByteArray("00000000000000000000000000000000"),
                iv = CryptoUtils.hexStringToByteArray("000000000000000000000000"),
            };

            byte[] data = CryptoUtils.hexStringToByteArray("00000000000000000000000000000000");

            byte[] refCiphertext = CryptoUtils.hexStringToByteArray("0388dace60b6a392f328c2b971b2fe78");
            byte[] refAuthTag = CryptoUtils.hexStringToByteArray("ab6e47d42cec13bdf53a67b21257bddf");
            byte[] encData = aesEnc.encrypt(data);

            Assert.IsTrue(encData.SequenceEqual(refCiphertext));
            Assert.IsTrue(aesEnc.authTag.SequenceEqual(refAuthTag));
        }

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Aes128GcmCpp_Enc_Dec_3()
        {
            // Test vectors from: https://git.openssl.org/gitweb/?p=openssl.git;a=commitdiff;h=6b7b34330beaf7ff062d6a06b7733f069d77feaa
            Aes128GcmCpp aesEnc = new Aes128GcmCpp
            {
                key = CryptoUtils.hexStringToByteArray("feffe9928665731c6d6a8f9467308308"),
                iv = CryptoUtils.hexStringToByteArray("cafebabefacedbaddecaf888"),
            };

            byte[] data = CryptoUtils.hexStringToByteArray("d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b391aafd255");

            byte[] refCiphertext = CryptoUtils.hexStringToByteArray("42831ec2217774244b7221b784d0d49ce3aa212f2c02a4e035c17e2329aca12e21d514b25466931c7d8f6a5aac84aa051ba30b396a0aac973d58e091473f5985");
            byte[] refAuthTag = CryptoUtils.hexStringToByteArray("4d5c2af327cd64a62cf35abd2ba6fab4");
            byte[] encData = aesEnc.encrypt(data);

            Assert.IsTrue(encData.SequenceEqual(refCiphertext));
            Assert.IsTrue(aesEnc.authTag.SequenceEqual(refAuthTag));
        }
    }
}
