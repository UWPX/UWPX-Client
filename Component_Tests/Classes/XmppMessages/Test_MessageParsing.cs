using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace Component_Tests.Classes.XmppMessages
{
    [TestClass]
    public class Test_MessageParsing
    {
        [TestMethod]
        public void Test_Parsing_MUCPresenceErrorMessage_1()
        {
            string msg = "<presence from='coven@chat.shakespeare.lit' id='273hs51g' to='hag66@shakespeare.lit/pda' type='error'> <error by='coven@chat.shakespeare.lit' type='modify'> <jid-malformed xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/> </error> </presence>";
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> messages = parser.parseMessages(ref msg);
            Assert.IsTrue(messages.Count == 1);
            Assert.IsTrue(messages[0] is MUCPresenceErrorMessage);
            MUCPresenceErrorMessage errorMsg = messages[0] as MUCPresenceErrorMessage;
            Assert.AreEqual(errorMsg.getTo(), "hag66@shakespeare.lit/pda");
            Assert.AreEqual(errorMsg.getFrom(), "coven@chat.shakespeare.lit");
            Assert.AreEqual(errorMsg.TYPE, "error");
            Assert.IsNotNull(errorMsg.ERROR);
            Assert.AreEqual(errorMsg.ERROR.BY, "coven@chat.shakespeare.lit");
            Assert.IsTrue(string.IsNullOrEmpty(errorMsg.ERROR.ERROR_MESSAGE));
            Assert.AreEqual(errorMsg.ERROR.ERROR_NAME, ErrorName.JID_MALFORMED);
            Assert.AreEqual(errorMsg.ERROR.ERROR_TYPE, ErrorType.MODIFY);
        }

        [TestMethod]
        public void Test_Parsing_MUCPresenceErrorMessage_2()
        {
            string msg = "<presence xmlns:stream='http://etherx.jabber.org/streams' to='hag66@shakespeare.lit' from='shakespeare.lit' type='error'> <error code='404' type='cancel'> <remote-server-not-found xmlns='urn:ietf:params:xml:ns:xmpp-stanzas' /> </error> </presence>";
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> messages = parser.parseMessages(ref msg);
            Assert.IsTrue(messages.Count == 1);
            Assert.IsTrue(messages[0] is MUCPresenceErrorMessage);
            MUCPresenceErrorMessage errorMsg = messages[0] as MUCPresenceErrorMessage;
            Assert.AreEqual(errorMsg.getTo(), "hag66@shakespeare.lit");
            Assert.AreEqual(errorMsg.getFrom(), "shakespeare.lit");
            Assert.AreEqual(errorMsg.TYPE, "error");
            Assert.IsNotNull(errorMsg.ERROR);
            Assert.IsTrue(string.IsNullOrEmpty(errorMsg.ERROR.BY));
            Assert.IsTrue(string.IsNullOrEmpty(errorMsg.ERROR.ERROR_MESSAGE));
            Assert.AreEqual(errorMsg.ERROR.ERROR_NAME, ErrorName.REMOTE_SERVER_NOT_FOUND);
            Assert.AreEqual(errorMsg.ERROR.ERROR_TYPE, ErrorType.CANCEL);
        }

    }
}
