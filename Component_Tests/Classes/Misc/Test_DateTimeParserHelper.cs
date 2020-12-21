using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMPP_API.Classes.Network.XML.Messages.XEP_0082;

namespace Component_Tests.Classes.Misc
{
    [TestClass]
    public class Test_DateTimeParserHelper
    {
        [TestCategory("Misc")]
        [TestMethod]
        public void Test_DateTimeParserHelper_1()
        {
            DateTime referenceDateTime = new DateTime(1969, 07, 21, 02, 56, 15).AddHours((int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours);
            Assert.IsTrue(DateTimeHelper.Parse("1969-07-21T02:56:15Z").CompareTo(referenceDateTime) == 0);
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_DateTimeParserHelper_2()
        {
            DateTime referenceDateTime = new DateTime(1969, 07, 21, 02, 56, 15).AddHours((int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours);
            Assert.IsTrue(DateTimeHelper.Parse("1969-07-20T21:56:15-05:00").CompareTo(referenceDateTime) == 0);
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_DateTimeParserHelper_3()
        {
            Assert.IsTrue(DateTimeHelper.Parse("").CompareTo(DateTime.MinValue) == 0);
            Assert.IsTrue(DateTimeHelper.Parse(null).CompareTo(DateTime.MinValue) == 0);
            Assert.IsTrue(DateTimeHelper.Parse("sadwesdbnjfksd").CompareTo(DateTime.MinValue) == 0);
        }
    }
}
