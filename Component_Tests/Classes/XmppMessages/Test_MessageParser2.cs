using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XMPP_API.Classes.Network.XML;
using XMPP_API.Classes.Network.XML.Messages;

namespace Component_Tests.Classes.XmppMessages
{
    [TestClass]
    public class Test_MessageParser2
    {
        [TestMethod]
        public void Test_MessageParser2_1()
        {
            string msg = "<iq xml:lang='en' to='uwptest@404.city/FABIAN-TOWER-PC' from='fabi@xmpp.uwpx.org' type='error' id='134077900-349929748-1523671119-224987985-1457976454'><pubsub xmlns='http://jabber.org/protocol/pubsub'><items node='eu.siacs.conversations.axolotl.devicelist'/></pubsub><error code='405' type='cancel'><closed-node xmlns='http://jabber.org/protocol/pubsub#errors'/><not-allowed xmlns='urn:ietf:params:xml:ns:xmpp-stanzas'/></error></iq>";
            MessageParser2 parser = new MessageParser2();
            List<AbstractMessage> messages = parser.parseMessages(ref msg);
            Assert.IsTrue(messages.Any((x) => x is IQErrorMessage));
        }

        [TestMethod]
        public void Test_MessageParser2_Performance_1()
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
            Logging.Logger.Info("[UNIT_TEST] Message Parser average parse time: " + sum);
        }
    }
}
