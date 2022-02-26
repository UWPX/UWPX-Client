using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Omemo.Classes;
using Omemo.Classes.Keys;

namespace Component_Tests.Classes.Crypto.Omemo
{
    [TestClass]
    public class Test_KeyHelper
    {
        [TestCategory("Crypto")]
        [TestMethod]
        public void Test_PreKeyGeneration()
        {
            for (uint count = 0; count < 250; count++)
            {
                List<PreKeyModel> keys = KeyHelper.GeneratePreKeys(1, count);
                Assert.AreEqual(keys.Count, (int)count);
            }

            // Rollover:
            for (uint count = 0; count < 10; count++)
            {
                List<PreKeyModel> keys = KeyHelper.GeneratePreKeys(0x7FFFFFFF, count);
                Assert.AreEqual(keys.Count, (int)count);
            }

            for (uint count = 0; count < 10; count++)
            {
                List<PreKeyModel> keys = KeyHelper.GeneratePreKeys(0x7FFFFFFF - count, 100);
                Assert.AreEqual(keys.Count, 100);
            }

            // Key IDs are unique:
            List<uint> ids = new List<uint>();
            List<PreKeyModel> keys2 = KeyHelper.GeneratePreKeys(0x7FFFFFFF - 100, 1000);
            for (int i = 0; i < keys2.Count; i++)
            {
                Assert.IsFalse(ids.Contains(keys2[i].keyId));
                ids.Add(keys2[i].keyId);
            }
        }
    }
}
