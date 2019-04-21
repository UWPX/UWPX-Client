using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMPP_API.Classes;

namespace Component_Tests.Classes.Misc
{
    [TestClass]
    public class Test_DateTimeParserHelper
    {
        [TestCategory("Misc")]
        [TestMethod]
        public void Test_DateTimeParserHelper_1()
        {
            DateTimeParserHelper helper = new DateTimeParserHelper();
            DateTime referenceDateTime = new DateTime(1969, 07, 21, 02, 56, 15).AddHours((int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours);
            Assert.IsTrue(helper.parse("1969-07-21T02:56:15Z").CompareTo(referenceDateTime) == 0);
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_DateTimeParserHelper_2()
        {
            DateTimeParserHelper helper = new DateTimeParserHelper();
            DateTime referenceDateTime = new DateTime(1969, 07, 21, 02, 56, 15).AddHours((int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours);
            Assert.IsTrue(helper.parse("1969-07-20T21:56:15-05:00").CompareTo(referenceDateTime) == 0);
        }

        [TestCategory("Misc")]
        [TestMethod]
        public void Test_DateTimeParserHelper_3()
        {
            DateTimeParserHelper helper = new DateTimeParserHelper();
            Assert.IsTrue(helper.parse("").CompareTo(DateTime.MinValue) == 0);
            Assert.IsTrue(helper.parse(null).CompareTo(DateTime.MinValue) == 0);
            Assert.IsTrue(helper.parse("sadwesdbnjfksd").CompareTo(DateTime.MinValue) == 0);
        }
    }
}
