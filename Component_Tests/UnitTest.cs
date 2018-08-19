using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMPP_API.Classes;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA1;

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
            List<AbstractMessage> msgs = parser.parseMessages("<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>");
            Assert.IsTrue(msgs.Count == 1);
        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism_3()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> msgs = parser.parseMessages("<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>");
            Assert.IsTrue(msgs[0] is ScramSHA1ChallengeMessage);

        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism_4()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> msgs = parser.parseMessages("<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>");
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
            List<AbstractMessage> msgs = parser.parseMessages("<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>");
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
        public void TestAes128Gcm_1()
        {
            for (int i = 0; i < 100; i++)
            {
                Aes128Gcm aesEnc = new Aes128Gcm();
                aesEnc.generateKey();
                aesEnc.generateIv();
                Random r = new Random();
                byte[] data = new byte[200];
                r.NextBytes(data);
                byte[] dataEnc = aesEnc.encrypt(data);

                Aes128Gcm aesDec = new Aes128Gcm()
                {
                    authTag = aesEnc.authTag,
                    iv = aesEnc.iv,
                    key = aesEnc.key
                };
                Assert.IsTrue(data.SequenceEqual(aesDec.decrypt(dataEnc)));
            }
        }

        [TestMethod]
        public void TestAes128Gcm_2()
        {
            Aes128Gcm aesEnc = new Aes128Gcm
            {
                key = CryptoUtils.hexStringToByteArray("AD7A2BD03EAC835A6F620FDCB506B345"),
                iv = CryptoUtils.hexStringToByteArray("12153524C0895E81B2C28465"),
                authTag = CryptoUtils.hexStringToByteArray("12153524C0895E81B2C28465")
            };

            byte[] data = CryptoUtils.hexStringToByteArray("D609B1F056637A0D46DF998D88E5222AB2C2846512153524C0895E8108000F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F30313233340001");

            string refDataEnc = "b8AggfYXMdlpIO+RJFuDt9vXcBYiz8J9axw=";
            string encData = CryptoUtils.byteArrayToHexString(aesEnc.encrypt(data));

            Assert.IsTrue(encData.Equals(refDataEnc));
        }
        #endregion

        //-------------------------------------------------------------------------------------MessageParser2:---------------------------------------------------------------------------------------------
        #region
        [TestMethod]
        public void TestMessageParser2_1()
        {
            string msg = "<iq xml:lang='en' to='uwptest@404.city/FABIAN-TOWER-PC' from='fabi@xmpp.uwpx.org' type='error' id='134077900-349929748-1523671119-224987985-1457976454'><pubsub xmlns='http://jabber.org/protocol/pubsub'><items node='eu.siacs.conversations.axolotl.devicelist'/></pubsub><error code='405' type='cancel'><closed-node xmlns='http://jabber.org/protocol/pubsub#errors'/><not-allowed xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/></error></iq>";
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> messages = parser.parseMessages(msg);
        }
        #endregion
    }
}
