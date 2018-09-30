using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMPP_API.Classes;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA1;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace Component_Tests
{
    [TestClass]
    public class UnitTest1
    {
        //-------------------------------------------------------------------------------------DateTimeParserHelper:-------------------------------------------------------------------------------------
        #region
        [TestMethod]
        public void TestDateTimeParserHelper_1()
        {
            DateTimeParserHelper helper = new DateTimeParserHelper();
            DateTime referenceDateTime = new DateTime(1969, 07, 21, 02, 56, 15).AddHours((int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours);
            Assert.IsTrue(helper.parse("1969-07-21T02:56:15Z").CompareTo(referenceDateTime) == 0);
        }

        [TestMethod]
        public void TestDateTimeParserHelper_2()
        {
            DateTimeParserHelper helper = new DateTimeParserHelper();
            DateTime referenceDateTime = new DateTime(1969, 07, 21, 02, 56, 15).AddHours((int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours);
            Assert.IsTrue(helper.parse("1969-07-20T21:56:15-05:00").CompareTo(referenceDateTime) == 0);
        }

        [TestMethod]
        public void TestDateTimeParserHelper_3()
        {
            DateTimeParserHelper helper = new DateTimeParserHelper();
            Assert.IsTrue(helper.parse("").CompareTo(DateTime.MinValue) == 0);
            Assert.IsTrue(helper.parse(null).CompareTo(DateTime.MinValue) == 0);
            Assert.IsTrue(helper.parse("sadwesdbnjfksd").CompareTo(DateTime.MinValue) == 0);
        }
        #endregion

        //-------------------------------------------------------------------------------------ScramSHA1SASLMechanism:-------------------------------------------------------------------------------------
        #region
        [TestMethod]
        public void TestScramSHA1SASLMechanism_1()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            Assert.IsTrue(string.Equals("biwsbj11c2VyLHI9ZnlrbytkMmxiYkZnT05Sdjlxa3hkYXdM", msg.VALUE));
        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism_2()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            Assert.IsTrue(msgs.Count == 1);
        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism_3()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            Assert.IsTrue(msgs[0] is ScramSHA1ChallengeMessage);

        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism_4()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            ScramSHA1ChallengeMessage challenge = (ScramSHA1ChallengeMessage)msgs[0];
            AbstractMessage resp = mechanism.generateResponse(challenge);
            Assert.IsTrue(resp is ScramSha1ChallengeSolutionMessage);
        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism_5()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            ScramSHA1ChallengeMessage challenge = (ScramSHA1ChallengeMessage)msgs[0];
            AbstractMessage resp = mechanism.generateResponse(challenge);
            ScramSha1ChallengeSolutionMessage respMsg = (ScramSha1ChallengeSolutionMessage)resp;
            Assert.IsTrue(string.Equals("Yz1iaXdzLHI9ZnlrbytkMmxiYkZnT05Sdjlxa3hkYXdMM3JmY05IWUpZMVpWdldWczdqLHA9djBYOHYzQnoyVDBDSkdiSlF5RjBYK0hJNFRzPQ==".ToLower(), respMsg.SOLUTION.ToLower()));

        }
        #endregion

        //-------------------------------------------------------------------------------------CryptoUtils:------------------------------------------------------------------------------------------------
        #region
        [TestMethod]
        public void TestPbkdf2Sha1()
        {
            byte[] saltBytes = CryptoUtils.hexStringToByteArray("4125c247e43ab1e93c6dff76");
            byte[] saltedPassword = CryptoUtils.Pbkdf2Sha1("pencil".Normalize(), saltBytes, 4096);
            byte[] saltedPasswordRef = CryptoUtils.hexStringToByteArray("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");
            Assert.IsTrue(saltedPassword.SequenceEqual(saltedPasswordRef));
        }

        [TestMethod]
        public void TestHmacSha1_1()
        {
            byte[] saltedPassword = CryptoUtils.hexStringToByteArray("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");
            string authMessage = "Client Key";
            byte[] clientKey = CryptoUtils.HmacSha1(authMessage, saltedPassword);
            byte[] clientKeyRef = CryptoUtils.hexStringToByteArray("e234c47bf6c36696dd6d852b99aaa2ba26555728");
            Assert.IsTrue(clientKey.SequenceEqual(clientKeyRef));
        }

        [TestMethod]
        public void TestHmacSha1_2()
        {
            byte[] storedKey = CryptoUtils.hexStringToByteArray("e9d94660c39d65c38fbad91c358f14da0eef2bd6");
            string authMessage = "n=user,r=fyko+d2lbbFgONRv9qkxdawL,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096,c=biws,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j";
            byte[] clientSignature = CryptoUtils.HmacSha1(authMessage, storedKey);
            byte[] clientSignatureRef = CryptoUtils.hexStringToByteArray("5d7138c486b0bfabdf49e3e2da8bd6e5c79db613");
            Assert.IsTrue(clientSignature.SequenceEqual(clientSignatureRef));
        }

        [TestMethod]
        public void TestHmacSha1_3()
        {
            byte[] saltedPassword = CryptoUtils.hexStringToByteArray("1d96ee3a529b5a5f9e47c01f229a2cb8a6e15f7d");
            string authMessage = "Server Key";
            byte[] serverKey = CryptoUtils.HmacSha1(authMessage, saltedPassword);
            byte[] serverKeyRef = CryptoUtils.hexStringToByteArray("0fe09258b3ac852ba502cc62ba903eaacdbf7d31");
            Assert.IsTrue(serverKey.SequenceEqual(serverKeyRef));
        }

        [TestMethod]
        public void TestHmacSha1_4()
        {
            byte[] serverKey = CryptoUtils.hexStringToByteArray("0fe09258b3ac852ba502cc62ba903eaacdbf7d31");
            string authMessage = "n=user,r=fyko+d2lbbFgONRv9qkxdawL,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096,c=biws,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j";
            byte[] serverSignature = CryptoUtils.HmacSha1(authMessage, serverKey);
            byte[] serverSignatureRef = CryptoUtils.hexStringToByteArray("ae617da6a57c4bbb2e0286568dae1d251905b0a4");
            Assert.IsTrue(serverSignature.SequenceEqual(serverSignatureRef));
        }

        [TestMethod]
        // Values from: https://tools.ietf.org/html/rfc2202
        public void TestHmacSha1_5()
        {
            byte[] key = CryptoUtils.hexStringToByteArray("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b");
            byte[] data = Encoding.ASCII.GetBytes("Hi There");
            byte[] digest = CryptoUtils.HmacSha1(data, key);
            byte[] digestRef = CryptoUtils.hexStringToByteArray("b617318655057264e28bc0b6fb378c8ef146be00");
            Assert.IsTrue(digest.SequenceEqual(digestRef));
        }

        [TestMethod]
        public void TestXor()
        {
            byte[] clientKey = CryptoUtils.hexStringToByteArray("e234c47bf6c36696dd6d852b99aaa2ba26555728");
            byte[] clientSignature = CryptoUtils.hexStringToByteArray("5d7138c486b0bfabdf49e3e2da8bd6e5c79db613");

            byte[] clientProof = CryptoUtils.xor(clientKey, clientSignature);

            byte[] clientProofRef = CryptoUtils.hexStringToByteArray("bf45fcbf7073d93d022466c94321745fe1c8e13b");
            Assert.IsTrue(clientProof.SequenceEqual(clientProofRef));
        }

        [TestMethod]
        public void TestSha1()
        {
            byte[] clientKey = CryptoUtils.hexStringToByteArray("e234c47bf6c36696dd6d852b99aaa2ba26555728");
            byte[] storedKey = CryptoUtils.SHA_1(clientKey);

            byte[] storedKeyRef = CryptoUtils.hexStringToByteArray("e9d94660c39d65c38fbad91c358f14da0eef2bd6");
            Assert.IsTrue(storedKey.SequenceEqual(storedKeyRef));
        }

        [TestMethod]
        public void TestByteArrayHexString()
        {
            string input = "e234c47bf6c36696dd6d852b99aaa2ba26555728";
            byte[] a = CryptoUtils.hexStringToByteArray(input);
            string output = CryptoUtils.byteArrayToHexString(a);
            Assert.IsTrue(input.Equals(output));
        }

        [TestMethod]
        public void TestHexToByteArray()
        {
            string saltStringBase64Ref = "QSXCR+Q6sek8bf92";
            string saltStringHexRef = "4125c247e43ab1e93c6dff76";

            byte[] salt = Convert.FromBase64String(saltStringBase64Ref);
            byte[] saltRef = CryptoUtils.hexStringToByteArray(saltStringHexRef);
            Assert.IsTrue(salt.SequenceEqual(saltRef));

            string saltStringHex = CryptoUtils.byteArrayToHexString(salt);
            Assert.IsTrue(saltStringHex.Equals(saltStringHexRef));
        }

        [TestMethod]
        public void TestAes128GcmCpp_1()
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

                Aes128GcmCpp aesDec = new Aes128GcmCpp()
                {
                    authTag = aesEnc.authTag,
                    iv = aesEnc.iv,
                    key = aesEnc.key,
                };
                byte[] dataDec = aesDec.decrypt(dataEnc);
                Assert.IsTrue(data.SequenceEqual(dataDec));
            }
        }

        [TestMethod]
        public void TestAes128GcmCpp_2()
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

        [TestMethod]
        public void TestAes128GcmCpp_3()
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
        #endregion

        //-------------------------------------------------------------------------------------MessageParser2:---------------------------------------------------------------------------------------------
        #region
        [TestMethod]
        public void TestMessageParser2_1()
        {
            string msg = "<iq xml:lang='en' to='uwptest@404.city/FABIAN-TOWER-PC' from='fabi@xmpp.uwpx.org' type='error' id='134077900-349929748-1523671119-224987985-1457976454'><pubsub xmlns='http://jabber.org/protocol/pubsub'><items node='eu.siacs.conversations.axolotl.devicelist'/></pubsub><error code='405' type='cancel'><closed-node xmlns='http://jabber.org/protocol/pubsub#errors'/><not-allowed xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/></error></iq>";
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> messages = parser.parseMessages(ref msg);
        }

        [TestMethod]
        public void TestMessageParser2Performance_1()
        {
            string msg = "<iq xml:lang='en' to='uwptest@404.city/FABIAN-TOWER-PC' from='fabi@xmpp.uwpx.org' type='error' id='134077900-349929748-1523671119-224987985-1457976454'><pubsub xmlns='http://jabber.org/protocol/pubsub'><items node='eu.siacs.conversations.axolotl.devicelist'/></pubsub><error code='405' type='cancel'><closed-node xmlns='http://jabber.org/protocol/pubsub#errors'/><not-allowed xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/></error></iq>";
            MessageParser2 parser = new MessageParser2();
            Stopwatch watch = new Stopwatch();
            long sum = 0;
            for (int e = 0; e < 100; e++)
            {
                watch.Start();
                for (int i = 0; i < 1000; i++)
                {
                    parser.parseMessages(ref msg);
                }
                watch.Stop();
                sum += watch.ElapsedTicks;
            }
            sum /= 100;
            Logging.Logger.Info("Message Parser average parse time: " + sum);
        }
        #endregion

        //-------------------------------------------------------------------------------------OmemoMessageMessage:----------------------------------------------------------------------------------------
        #region
        [TestMethod]
        public void TestOmemoMessageMessage_1()
        {
            OmemoMessageMessage msg = new OmemoMessageMessage("from", "to", "TestMessage", "chat", true);
        }
        #endregion

        //-------------------------------------------------------------------------------------Performance:------------------------------------------------------------------------------------------------
        #region
        #endregion
    }
}
