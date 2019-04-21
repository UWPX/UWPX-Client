using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMPP_API.Classes;

namespace Component_Tests.Classes.Misc
{
    /// <summary>
    /// Examples from: https://tools.ietf.org/html/rfc7622#section-3.5
    /// </summary>
    [TestClass]
    public class Test_JidParser
    {
        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsBareJid_1()
        {
            string s = "juliet@example.com";
            Assert.IsTrue(Utils.isBareJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsBareJid_2()
        {
            string s = "foo\\20bar@example.com";
            Assert.IsTrue(Utils.isBareJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsBareJid_3()
        {
            string s = "fussball@example.com";
            Assert.IsTrue(Utils.isBareJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsBareJid_4()
        {
            string s = "fu&#xDF;ball@example.com";
            Assert.IsTrue(Utils.isBareJid(s)); // TODO: Fix RFC 7622 encoding
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsBareJid_5()
        {
            string s = "&#x3C0;@example.com";
            Assert.IsTrue(Utils.isBareJid(s)); // TODO: Fix RFC 7622 encoding
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsBareJid_6()
        {
            string s = "\"juliet\"@example.com";
            Assert.IsFalse(Utils.isBareJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsBareJid_7()
        {
            string s = "foo bar@example.com";
            Assert.IsFalse(Utils.isBareJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsBareJid_8()
        {
            string s = "juliet@";
            Assert.IsFalse(Utils.isBareJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsFullJid_1()
        {
            string s = "juliet@example.com/foo";
            Assert.IsTrue(Utils.isFullJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsFullJid_2()
        {
            string s = "juliet@example.com/foo bar";
            Assert.IsTrue(Utils.isFullJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsFullJid_3()
        {
            string s = "juliet@example.com/foo@bar";
            Assert.IsTrue(Utils.isFullJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsFullJid_4()
        {
            string s = "&#x3A3;@example.com/foo";
            Assert.IsTrue(Utils.isFullJid(s)); // TODO: Fix RFC 7622 encoding
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsFullJid_5()
        {
            string s = "&#x3C3;@example.com/foo";
            Assert.IsTrue(Utils.isFullJid(s)); // TODO: Fix RFC 7622 encoding
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsFullJid_6()
        {
            string s = "&#x3C2;@example.com/foo";
            //Assert.IsTrue(Utils.isFullJid(s));  // TODO: Fix RFC 7622 encoding
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsFullJid_7()
        {
            string s = "king@example.com/&#x265A";
            Assert.IsTrue(Utils.isFullJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsFullJid_8()
        {
            string s = "a@a.example.com/b@example.net";
            Assert.IsTrue(Utils.isFullJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsFullJid_9()
        {
            string s = "juliet@example.com/ foo";
            Assert.IsFalse(Utils.isFullJid(s));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_IsFullJid_10()
        {
            string s = "@example.com/";
            Assert.IsFalse(Utils.isFullJid(s));
        }
    }
}
