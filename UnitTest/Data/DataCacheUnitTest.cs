using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Text;

namespace Trivial.Data
{
    /// <summary>
    /// HTTP URI unit test.
    /// </summary>
    [TestClass]
    public class DataCacheUnitTest
    {
        /// <summary>
        /// Tests query data.
        /// </summary>
        [TestMethod]
        public void TestCollection()
        {
            var cache = new DataCacheCollection<int>
            {
                { "abcdefg", 12 },
                { "opq", "abcdefg", 17 },
                { "hijklmn", 17 },
                { "opq", "hijklmn", 20 }
            };
            Assert.AreEqual(4, cache.Count);
            Assert.AreEqual(12, cache[0].Value);
            Assert.AreEqual(12, cache["abcdefg"]);
            Assert.AreEqual(17, cache["opq", "abcdefg"]);
            Assert.AreEqual(17, cache["hijklmn"]);
            Assert.AreEqual(20, cache["opq", "hijklmn"]);
            Assert.IsFalse(cache.Contains("opq"));

            cache.MaxCount = 3;
            cache.RemoveExpired();
            Assert.AreEqual(3, cache.Count);
            Assert.AreEqual(17, cache[0].Value);
            Assert.AreEqual(20, cache[2].Value);
        }
    }
}
