using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMPP_API.Classes.Crypto;

namespace Component_Tests.Classes.Crypto
{
    [TestClass]
    internal class Test_Aes128GcmCpp
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

        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_Aes128GcmCpp_Enc_Dec_Long_1()
        {
            Aes128GcmCpp aesEnc = new Aes128GcmCpp
            {
                key = CryptoUtils.hexStringToByteArray("feffe9928665731c6d6a8f9467308308"),
                iv = CryptoUtils.hexStringToByteArray("cafebabefacedbaddecaf888"),
            };

            byte[] data = new byte[1024];

            byte[] refCiphertext = CryptoUtils.hexStringToByteArray("9bb22ce7d9f372c1ee2b28722b25f206650d887c3936533a1b8d4e1ea39d2b5c3de91827c10e9a4f5240647ee5221f20aac9e6ccc0074ac0873b9ba85d908bd0dda867c4cef2b1f1b8a9ff49ca208b6116d8f4f1c3c9273f23d7581cc373e461f01e5cbfa52f40f2500c07c41f377171c1bdf4b560668370848aaa36061a9758fed6fdb1e83ba41f237936ab466025c0c0b6dc862245883a92d3f8d43b39a62b726d4616ada9e4b101479e81340226bf81bc7bc1a5c9372973ecbe829aff5720a4b304fe9a91a9aeafc4c4a1af5eaeb22e92be237a73b3174be2e5388099a53ab09d7ef18916daa6f7817d3345432a93b1903ad99767f610c691102a0a25a46c21610189f099a5ad44890c8c188325e881627a5b6c0c911b7ce3b3f079b7e8cd8e189b28c8158805633cdc448b78d4e4880b37f2faca7e34c4be2ff3c4bd00fdc0395cdc02eb5e727152559e4ac5c3d688b12ea339ffa46222a9ef2bf1d948bdd0c35db154b550cf88990d9d4391178999e106a4e4fc48e272d6b0b43959f6782f6850e533571e01122ae61ebc6fd34953c6a847b0d1fafde7e93821ea8bf98db5dc2b13aa9f1c5046682573e500c57058dfd21ec5c36cf627e8ffc8667fe774480c0f4b7b7843d336836f1ab5ac043f423e32ab5daea7d385e2668bc333b4e2aa3b7232ff62bcd61ec28af04842fd4642abb9900c416d6443132202bc59f9331ce3d98e9f56e59d0c43455c0653ec6de357e1ab3fde6145ea1caa313c3c005ddfa542359325a276018b4992d15799b3e53bffb4a7e2d4cdf0ded91f49072fdeb5a4383f6e6b3a6091172c5bb392151079ca31c0280bbeb191307f5ed753ee15113b73b1df4bb566cf5d6175ae83229ca55fc6bd0662b28848d7cbd9c987df339f193833640375523972bc8666b39f85d99bec505e8bb397cf08ea694f03b0883d60a1408b735d2fb0259ca8a3cf75eb6b6340600a83edd9aa2c20b6a2827e194d7a4e3b09d7267e739d9b89d00ead14c6888df05b72932ee8793ffaca64c9dd62ad7addfaff73ab1dd1dd7ff5bf006d4b25bf82f193449f20e04bb485a4bdfdf7b5fee4a812404bd6c87408275e41a5e09d24e29c3364016c6eec2e37fc316091b44886b73a888fc06eea87ee1c5bbdc82fc4f0cf303af81b5452c41f7ee8a2eb2c30c9d09a09735678109ae64ccc002b93f182cc858a08c4a144d5afabf1b7d4e47a232963719df669b50b3002f020e404cd7141c596e7804e7da133bf6a9030d584fb6f1e0b69137b1f9c0440f18e2dafab746ec3f976ebe07d426f54db3258aebdff22687fda41c7516162f533a316ef519706a76424b0884e2f8f8979cb305561bcf0b5f38ebaa74e8ea6c6692599ace5e1a190e073838952417b2a2434cf90f1d66b11d90b00a391f42c02d1261e19ab80d744fd1b402aa3a3d6f61492");
            byte[] refAuthTag = CryptoUtils.hexStringToByteArray("5f4226cd3dbf20fdfbbd1947f6da4e82");
            byte[] encData = aesEnc.encrypt(data);
            string s = CryptoUtils.byteArrayToHexString(aesEnc.authTag);

            Assert.IsTrue(encData.SequenceEqual(refCiphertext));
            Assert.IsTrue(aesEnc.authTag.SequenceEqual(refAuthTag));
        }
    }
}
