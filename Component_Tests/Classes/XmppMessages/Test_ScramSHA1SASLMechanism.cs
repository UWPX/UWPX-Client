using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA1;

namespace Component_Tests.Classes.XmppMessages
{
    // Examples from: https://wiki.xmpp.org/web/SASLandSCRAM-SHA-1 section "Test vectors"
    [TestClass]
    public class Test_ScramSHA1SASLMechanism
    {
        [TestCategory("XMPP_Messages")]
        [TestMethod]
        public void Test_ScramSHA1SASLMechanism_1()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL", null);
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            Assert.IsTrue(string.Equals("biwsbj11c2VyLHI9ZnlrbytkMmxiYkZnT05Sdjlxa3hkYXdM", msg.VALUE));
        }

        [TestCategory("XMPP_Messages")]
        [TestMethod]
        public void Test_ScramSHA1SASLMechanism_2()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL", null);
            mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            Assert.IsTrue(msgs.Count == 1);
        }

        [TestCategory("XMPP_Messages")]
        [TestMethod]
        public void Test_ScramSHA1SASLMechanism_3()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL", null);
            mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            Assert.IsTrue(msgs[0] is ScramSHA1ChallengeMessage);
        }

        [TestCategory("XMPP_Messages")]
        [TestMethod]
        public void Test_ScramSHA1SASLMechanism_4()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL", null);
            mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            ScramSHA1ChallengeMessage challenge = (ScramSHA1ChallengeMessage)msgs[0];
            AbstractMessage resp = mechanism.generateResponse(challenge);
            Assert.IsTrue(resp is ScramSha1ChallengeSolutionMessage);
        }

        [TestCategory("XMPP_Messages")]
        [TestMethod]
        public void Test_ScramSHA1SASLMechanism_5()
        {
            ScramSHA1SASLMechanism mechanism = new ScramSHA1SASLMechanism("user", "pencil", "fyko+d2lbbFgONRv9qkxdawL", null);
            mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1meWtvK2QybGJiRmdPTlJ2OXFreGRhd0wzcmZjTkhZSlkxWlZ2V1ZzN2oscz1RU1hDUitRNnNlazhiZjkyLGk9NDA5Ng==</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            ScramSHA1ChallengeMessage challenge = (ScramSHA1ChallengeMessage)msgs[0];
            AbstractMessage resp = mechanism.generateResponse(challenge);
            ScramSha1ChallengeSolutionMessage respMsg = (ScramSha1ChallengeSolutionMessage)resp;
            Assert.IsTrue(string.Equals("Yz1iaXdzLHI9ZnlrbytkMmxiYkZnT05Sdjlxa3hkYXdMM3JmY05IWUpZMVpWdldWczdqLHA9djBYOHYzQnoyVDBDSkdiSlF5RjBYK0hJNFRzPQ==".ToLower(), respMsg.SOLUTION.ToLower()));
        }
    }
}
