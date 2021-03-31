using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Collection
{
    /// <summary>
    /// The unit test of synchronized list.
    /// </summary>
    [TestClass]
    public class SynchronizedListTest
    {
        /// <summary>
        /// Test the basic actions of synchronized list.
        /// </summary>
        [TestMethod]
        public void TestListActions()
        {
            var list = new SynchronizedList<string>();
            Parallel.For(0, 100, i =>
            {
                list.Add(i.ToString("g"));
                list.Add((i + 100).ToString("g"));
                list.Remove((i + 300).ToString("g"));
                Assert.IsFalse(string.IsNullOrEmpty(list[0]));
                list.AddRange(new[] { (i + 200).ToString("g"), (i + 300).ToString("g"), (i + 400).ToString("g") });
                list.Remove((i + 200).ToString("g"));
            });

            list.RemoveAll(ele => ele.Length == 3 && ele.StartsWith("4"));
            Assert.AreEqual(300, list.Count);

            var list2 = ListExtensions.ToSynchronizedList(list, new object());
            Assert.AreEqual(300, list2.Count);
            list2.Add("abcdefg");

            list = new SynchronizedList<string>(list2);
            Assert.AreEqual(301, list.Count);
            Assert.AreEqual("abcdefg", list.Last());
            list.RemoveAt(300);
            Assert.AreEqual(300, list.Count);
        }
    }
}
