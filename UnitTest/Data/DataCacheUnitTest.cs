using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
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
                { "opq", 17 },
                { "hijklmn", 17 },
                { "rst", 20 }
            };
            cache.Expiration = TimeSpan.FromMilliseconds(1600);
            Assert.AreEqual(4, cache.Count());
            Assert.AreEqual(12, cache["abcdefg"]);
            Assert.AreEqual(17, cache["opq"]);
            Assert.AreEqual(17, cache["hijklmn"]);
            Assert.AreEqual(20, cache["rst"]);
            Assert.IsFalse(cache.Contains("xyz"));
            Assert.IsTrue(cache.Any());
            var info = cache.GetInfo("rst");
            Assert.AreEqual(20, info.Value);
            Assert.IsTrue(cache.Contains(info));

            cache.MaxCount = 3;
            cache.RemoveExpired();
            Assert.AreEqual(3, cache.Count());
            Assert.AreEqual(17, cache["opq"]);
            Assert.AreEqual(20, cache["rst"]);

            cache.MaxCount = 4;
            var i = 0;
            cache.Register(async id =>
            {
                await Task.Delay(30);
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
                    var no = cache.GetAsync("*#06#");
                    var j = await cache.GetInfoAsync("1234567890");
                    Assert.IsNotNull(j);
                    Assert.IsTrue(j.Value < 7);
                    Assert.IsTrue(await no < 7);
                }

                tasks.Add(task());
            }

            tasks.Add(Task.Delay(100));
            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(cache.Contains("*#06#"));
            Assert.IsTrue(cache["*#06#"] < 7);
            //if (cache.Count() == 3)
            //{
            //    Console.WriteLine($"{info.UpdateDate} {DateTime.Now} {cache.IsExpired(info)}");
            //    foreach (var item in cache)
            //    {
            //        Console.WriteLine(item.Id);
            //    }
            //}

            Assert.AreEqual(4, cache.Count());
            Assert.AreEqual(4, cache.CacheCount);
            Assert.AreEqual(99, cache["rst"]);

            cache.RemoveAll(ele => ele.Id == "1234567890");
            Assert.IsFalse(cache.Contains("1234567890"));
            Assert.IsFalse(cache.Contains("xyz"));
            Assert.IsTrue(cache.Contains("rst"));
            Assert.AreEqual(3, cache.Count());
            cache.Clear();
            Assert.IsFalse(cache.Any());
            Assert.AreEqual(0, cache.Count());

            cache.Expiration = TimeSpan.FromMilliseconds(30);
            cache.Add("abcdefg", 17);
            Assert.IsNull(cache.GetInfo("*#06#"));
            Assert.AreEqual(17, cache["abcdefg"]);
            Thread.Sleep(50);
            Assert.IsFalse(cache.Contains("abcdefg"));
            Assert.IsNotNull(cache.Contains("*#06#"));
        }

        /// <summary>
        /// Tests query data.
        /// </summary>
        [TestMethod]
        public void TestNsCollection()
        {
            var cache = new NamespacedDataCacheCollection<int>
            {
                { null, "abcdefg", 12 },
                { "opq", "abcdefg", 17 },
                { null, "hijklmn", 17 },
                { "opq", "hijklmn", 20 }
            };
            cache.Expiration = TimeSpan.FromMilliseconds(1600);
            Assert.AreEqual(4, cache.Count());
            Assert.AreEqual(12, cache[null, "abcdefg"]);
            Assert.AreEqual(17, cache["opq", "abcdefg"]);
            Assert.AreEqual(17, cache[null, "hijklmn"]);
            Assert.AreEqual(20, cache["opq", "hijklmn"]);
            Assert.IsFalse(cache.Contains(null, "opq"));
            Assert.IsTrue(cache.Any());

            cache.MaxCount = 3;
            cache.RemoveExpired();
            Assert.AreEqual(0, cache.Count("none"));
            Assert.AreEqual(2, cache.Count("opq"));
            Assert.AreEqual(17, cache["opq", "abcdefg"]);
            Assert.AreEqual(20, cache["opq", "hijklmn"]);

            cache.MaxCount = 4;
            var i = 0;
            cache.Register("xyz", async id =>
            {
                await Task.Delay(30);
                i++;
                return i;
            });

            Parallel.For(0, 100, i =>
            {
                for (var j = 0; j < 100; j++)
                {
                    cache[null, "rst"] = j;
                    Assert.IsTrue(cache[null, "rst"] >= 0);
                    cache[null, "uvw"] = j;
                    Assert.IsTrue(cache[null, "uvw"] >= 0);
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

            tasks.Add(Task.Delay(200));
            Task.WaitAll(tasks.ToArray());
            Assert.IsTrue(cache.Contains("xyz", "*#06#"));
            Assert.IsTrue(cache["xyz", "*#06#"] < 7);
            Assert.AreEqual(4, cache.Count());
            Assert.AreEqual(4, cache.CacheCount);
            Assert.AreEqual(99, cache[null, "rst"]);

            cache.RemoveAll(ele => ele.Id == "1234567890");
            Assert.IsFalse(cache.Contains("xyz", "1234567890"));
            Assert.IsFalse(cache.Contains(null, "*#06#"));
            Assert.IsTrue(cache.Contains(null, "rst"));
            Assert.AreEqual(3, cache.Count());
            cache.Clear();
            Assert.IsFalse(cache.Any());
            Assert.AreEqual(0, cache.Count());

            cache.Expiration = TimeSpan.FromMilliseconds(30);
            cache.Add(null, "abcdefg", 17);
            Assert.IsNull(cache.GetInfo("xyz", "*#06#"));
            Assert.AreEqual(17, cache[null, "abcdefg"]);
            Thread.Sleep(50);
            Assert.IsFalse(cache.Contains(null, "abcdefg"));
            Assert.IsNotNull(cache.Contains("xyz", "*#06#"));
        }
    }
}
