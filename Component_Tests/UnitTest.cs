using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA1;

namespace Component_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestDateTimeParserHelper1()
        {
            DateTimeParserHelper helper = new DateTimeParserHelper();
            DateTime referenceDateTime = new DateTime(1969, 07, 21, 02, 56, 15).AddHours((int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours);
            Assert.IsTrue(helper.parse("1969-07-21T02:56:15Z").CompareTo(referenceDateTime) == 0);
        }

        [TestMethod]
        public void TestDateTimeParserHelper2()
        {
            DateTimeParserHelper helper = new DateTimeParserHelper();
            DateTime referenceDateTime = new DateTime(1969, 07, 21, 02, 56, 15).AddHours((int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours);
            Assert.IsTrue(helper.parse("1969-07-20T21:56:15-05:00").CompareTo(referenceDateTime) == 0);
        }

        [TestMethod]
        public void TestDateTimeParserHelper3()
        {
            DateTimeParserHelper helper = new DateTimeParserHelper();
            Assert.IsTrue(helper.parse("").CompareTo(DateTime.MinValue) == 0);
            Assert.IsTrue(helper.parse(null).CompareTo(DateTime.MinValue) == 0);
            Assert.IsTrue(helper.parse("sadwesdbnjfksd").CompareTo(DateTime.MinValue) == 0);
        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism1()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            Assert.IsTrue(string.Equals("biwsbj11c2VyLHI9ZnlrbytkMmxiYkZnT05Sdjlxa3hkYXdM", msg.VALUE));
        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism2()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> msgs = parser.parseMessages("<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj02ZDQ0MmI1ZDllNTFhNzQwZjM2OWUzZGNlY2YzMTc4ZWMxMmIzOTg1YmJkNGE4ZTZmODE0YjQyMmFiNzY2NTczLHM9UVNYQ1IrUTZzZWs4YmY5MixpPTQwOTY=</challenge>");
            Assert.IsTrue(msgs.Count == 1);
        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism3()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> msgs = parser.parseMessages("<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj02ZDQ0MmI1ZDllNTFhNzQwZjM2OWUzZGNlY2YzMTc4ZWMxMmIzOTg1YmJkNGE4ZTZmODE0YjQyMmFiNzY2NTczLHM9UVNYQ1IrUTZzZWs4YmY5MixpPTQwOTY=</challenge>");
            Assert.IsTrue(msgs[0] is ScramSHA1ChallengeMessage);

        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism4()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> msgs = parser.parseMessages("<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj02ZDQ0MmI1ZDllNTFhNzQwZjM2OWUzZGNlY2YzMTc4ZWMxMmIzOTg1YmJkNGE4ZTZmODE0YjQyMmFiNzY2NTczLHM9UVNYQ1IrUTZzZWs4YmY5MixpPTQwOTY=</challenge>");
            ScramSHA1ChallengeMessage challenge = (ScramSHA1ChallengeMessage)msgs[0];
            AbstractMessage resp = mechanism.generateResponse(challenge);
            Assert.IsTrue(resp is ScramSha1ChallengeSolutionMessage);
        }

        [TestMethod]
        public void TestScramSHA1SASLMechanism5()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil");
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> msgs = parser.parseMessages("<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj02ZDQ0MmI1ZDllNTFhNzQwZjM2OWUzZGNlY2YzMTc4ZWMxMmIzOTg1YmJkNGE4ZTZmODE0YjQyMmFiNzY2NTczLHM9UVNYQ1IrUTZzZWs4YmY5MixpPTQwOTY=</challenge>");
            ScramSHA1ChallengeMessage challenge = (ScramSHA1ChallengeMessage)msgs[0];
            AbstractMessage resp = mechanism.generateResponse(challenge);
            ScramSha1ChallengeSolutionMessage respMsg = (ScramSha1ChallengeSolutionMessage)resp;
            Assert.IsTrue(string.Equals("bj11c2VyLHI9ZnlrbytkMmxiYkZnT05Sdjlxa3hkYXdMLHI9ZnlrbytkMmxiYkZnT05Sdjlxa3hkYXdMM3JmY05IWUpZMVpWdldWczdqLHM9UVNYQ1IrUTZzZWs4YmY5MixpPTQwOTYsYz1iaXdzLHI9ZnlrbytkMmxiYkZnT05Sdjlxa3hkYXdMM3JmY05IWUpZMVpWdldWczdq", respMsg.SOLUTION));

        }
    }
}
