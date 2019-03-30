using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI;
using XMPP_API.Classes.Network.XML.Messages.XEP_0392;

namespace Component_Tests.Classes.Misc
{
    [TestClass]
    public class Test_ConsistentColorGenerator
    {
        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGenerator_1()
        {
            Color color = ConsistentColorGenerator.GenColor("Romeo");
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.865));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.000));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.686));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGenerator_2()
        {
            Color color = ConsistentColorGenerator.GenColor("😺");
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.872));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.000));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.659));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGenerator_3()
        {
            Color color = ConsistentColorGenerator.GenColor("council");
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.918));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.000));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.394));
        }
    }
}
