using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Features.SASL.SHA256;

namespace Component_Tests.Classes.XmppMessages
{
    // Examples from: https://datatracker.ietf.org/doc/html/rfc7677#section-3 section "Test vectors"
    [TestClass]
    public class Test_ScramSHA256SASLMechanism
    {
        [TestCategory("XMPP_Messages")]
        [TestMethod]
        public void Test_ScramSHA256SASLMechanism_1()
        {
            ScramSHA256SASLMechanism mechanism = new ScramSHA256SASLMechanism("user", "pencil", "rOprNGfwEbeRWgbNEkqO", null);
            SelectSASLMechanismMessage msg = mechanism.getSelectSASLMechanismMessage();
            Assert.IsTrue(string.Equals("biwsbj11c2VyLHI9ck9wck5HZndFYmVSV2diTkVrcU8=", msg.VALUE));
        }

        [TestCategory("XMPP_Messages")]
        [TestMethod]
        public void Test_ScramSHA256SASLMechanism_2()
        {
            ScramSHA256SASLMechanism mechanism = new ScramSHA256SASLMechanism("user", "pencil", "rOprNGfwEbeRWgbNEkqO", null);
            mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1yT3ByTkdmd0ViZVJXZ2JORWtxTyVodllEcFdVYTJSYVRDQWZ1eEZJbGopaE5sRiRrMCxzPVcyMlphSjBTTlk3c29Fc1VFamI2Z1E9PSxpPTQwOTY=</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            Assert.IsTrue(msgs.Count == 1);
        }

        [TestCategory("XMPP_Messages")]
        [TestMethod]
        public void Test_ScramSHA256SASLMechanism_3()
        {
            ScramSHA256SASLMechanism mechanism = new ScramSHA256SASLMechanism("user", "pencil", "rOprNGfwEbeRWgbNEkqO", null);
            mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1yT3ByTkdmd0ViZVJXZ2JORWtxTyVodllEcFdVYTJSYVRDQWZ1eEZJbGopaE5sRiRrMCxzPVcyMlphSjBTTlk3c29Fc1VFamI2Z1E9PSxpPTQwOTY=</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            Assert.IsTrue(msgs[0] is ScramSHA1ChallengeMessage);
        }

        [TestCategory("XMPP_Messages")]
        [TestMethod]
        public void Test_ScramSHA256SASLMechanism_4()
        {
            ScramSHA256SASLMechanism mechanism = new ScramSHA256SASLMechanism("user", "pencil", "rOprNGfwEbeRWgbNEkqO", null);
            mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1yT3ByTkdmd0ViZVJXZ2JORWtxTyVodllEcFdVYTJSYVRDQWZ1eEZJbGopaE5sRiRrMCxzPVcyMlphSjBTTlk3c29Fc1VFamI2Z1E9PSxpPTQwOTY=</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            ScramSHA1ChallengeMessage challenge = (ScramSHA1ChallengeMessage)msgs[0];
            AbstractMessage resp = mechanism.generateResponse(challenge);
            Assert.IsTrue(resp is ScramSha1ChallengeSolutionMessage);
        }

        [TestCategory("XMPP_Messages")]
        [TestMethod]
        public void Test_ScramSHA256SASLMechanism_5()
        {
            ScramSHA256SASLMechanism mechanism = new ScramSHA256SASLMechanism("user", "pencil", "rOprNGfwEbeRWgbNEkqO", null);
            mechanism.getSelectSASLMechanismMessage();
            MessageParser2 parser = new MessageParser2();
            string s = "<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>cj1yT3ByTkdmd0ViZVJXZ2JORWtxTyVodllEcFdVYTJSYVRDQWZ1eEZJbGopaE5sRiRrMCxzPVcyMlphSjBTTlk3c29Fc1VFamI2Z1E9PSxpPTQwOTY=</challenge>";
            List<AbstractMessage> msgs = parser.parseMessages(ref s);
            ScramSHA1ChallengeMessage challenge = (ScramSHA1ChallengeMessage)msgs[0];
            AbstractMessage resp = mechanism.generateResponse(challenge);
            ScramSha1ChallengeSolutionMessage respMsg = (ScramSha1ChallengeSolutionMessage)resp;
            Assert.IsTrue(string.Equals("Yz1iaXdzLHI9ck9wck5HZndFYmVSV2diTkVrcU8laHZZRHBXVWEyUmFUQ0FmdXhGSWxqKWhObEYkazAscD1kSHpiWmFwV0lrNGpVaE4rVXRlOXl0YWc5empmTUhnc3FtbWl6N0FuZFZRPQ==".ToLower(), respMsg.SOLUTION.ToLower()));
        }
    }
}
