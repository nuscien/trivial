using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
            cache.Expiration = TimeSpan.FromMilliseconds(1000);
            Assert.AreEqual(4, cache.Count);
            Assert.AreEqual(12, cache["abcdefg"]);
            Assert.AreEqual(17, cache["opq", "abcdefg"]);
            Assert.AreEqual(17, cache["hijklmn"]);
            Assert.AreEqual(20, cache["opq", "hijklmn"]);
            Assert.IsFalse(cache.Contains("opq"));

            cache.MaxCount = 3;
            cache.RemoveExpired();
            Assert.AreEqual(3, cache.Count);
            Assert.AreEqual(17, cache["opq", "abcdefg"]);
            Assert.AreEqual(20, cache["opq", "hijklmn"]);

            cache.MaxCount = 4;
            var i = 0;
            cache.Register("xyz", async id =>
            {
                await Task.Delay(40);
                i++;
                return i;
            });

            Parallel.For(0, 100, i =>
            {
                for (var j = 0; j < 100; j++)
                {
                    cache["rst"] = j;
                    Assert.IsTrue(cache["rst"] >= 0);
                    cache["uvw"] = j;
                    Assert.IsTrue(cache["uvw"] >= 0);
                }
            });

            var tasks = new List<Task>();
            for (var k = 0; k < 100; k++)
            {
                async Task task()
                {
                    var no = cache.GetAsync("xyz", "*#06#");
                    var j = await cache.GetInfoAsync("xyz", "1234567890");
                    Assert.IsNotNull(j);
                    Assert.IsTrue(j.Value < 7);
                    Assert.IsTrue(await no < 7);
                }

                tasks.Add(task());
            }

            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(cache["xyz", "*#06#"] < 7);
            Assert.AreEqual(4, cache.Count);
            Assert.AreEqual(99, cache["rst"]);

            cache.RemoveAll(ele => ele.Id == "1234567890");
            Assert.AreEqual(3, cache.Count);
            cache.Clear();
            Assert.AreEqual(0, cache.Count);
        }
    }
}
