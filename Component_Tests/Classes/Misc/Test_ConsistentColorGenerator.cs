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
        public void Test_ConsistentColorGeneratorNoCorrection_1()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("Romeo", false, false);
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.865));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.000));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.686));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorNoCorrection_2()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("😺", false, false);
            Assert.IsTrue(color.R == 222);
            Assert.IsTrue(color.G == 0);
            Assert.IsTrue(color.B == 167);
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorNoCorrection_3()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("council", false, false);
            Assert.IsTrue(color.R == 233);
            Assert.IsTrue(color.G == 0);
            Assert.IsTrue(color.B == 100);
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorNoCorrection_4()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("juliet@capulet.lit", false, false);
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.0));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.515));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.573));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorRedGreenCorrection_1()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("Romeo", true, false);
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.865));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.000));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.686));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorRedGreenCorrection_2()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("😺", true, false);
            Assert.IsTrue(color.R == 222);
            Assert.IsTrue(color.G == 0);
            Assert.IsTrue(color.B == 167);
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorRedGreenCorrection_3()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("council", true, false);
            Assert.IsTrue(color.R == 233);
            Assert.IsTrue(color.G == 0);
            Assert.IsTrue(color.B == 100);
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorRedGreenCorrection_4()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("juliet@capulet.lit", true, false);
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.742));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.359));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.0));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorBlueCorrection_1()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("Romeo", false, true);
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.0));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.535));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.350));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorBlueCorrection_2()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("😺", false, true);
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.0));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.533));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.373));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorBlueCorrection_3()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("council", false, true);
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.0));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.524));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.485));
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_ConsistentColorGeneratorBlueCorrection_4()
        {
            Color color = ConsistentColorGenerator.GenForegroundColor("juliet@capulet.lit", false, true);
            Assert.IsTrue(color.R == (byte)(byte.MaxValue * 0.742));
            Assert.IsTrue(color.G == (byte)(byte.MaxValue * 0.359));
            Assert.IsTrue(color.B == (byte)(byte.MaxValue * 0.0));
        }
    }
}
