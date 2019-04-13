using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using XMPP_API.Classes;

namespace Component_Tests.Classes.Misc
{
    [TestClass]
    public class Test_UriUtils
    {
        [TestCategory("Misc")]
        [TestMethod]
        public void Test_BuildUri_1()
        {
            Dictionary<string, string> queryPairs = new Dictionary<string, string>
            {
                {"type", "CHAT"},
                {"msgId", "juliet@example.com_SOME_MESSAGE_ID"},
            };
            Uri result = UriUtils.buildUri("juliet@example.com", queryPairs);

            Assert.IsTrue(result.Scheme.Equals(UriUtils.URI_SCHEME));
            Assert.IsTrue(UriUtils.getBareJidFromUri(result).Equals("juliet@example.com"));

            WwwFormUrlDecoder decoder = UriUtils.parseUriQuery(result);
            Assert.AreEqual(decoder.GetFirstValueByName("type"), "CHAT");
            Assert.AreEqual(decoder.GetFirstValueByName("msgId"), "juliet@example.com_SOME_MESSAGE_ID");
        }
    }
}
