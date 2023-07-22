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
    public class CollectionExtensionsTest
    {
        /// <summary>
        /// Test the string collection.
        /// </summary>
        [TestMethod]
        public void TestStrings()
        {
            var now = DateTime.Now;
            var offset = DateTimeOffset.Now;
            var guid = Guid.NewGuid();

            var arr = new[] { 1, 2, 3 }.ToStringArray();
            Assert.AreEqual(2.ToString(), arr[1]);
            arr = new[] { 4L, 5L }.ToStringArray();
            Assert.AreEqual(4.ToString(), arr[0]);
            arr = new[] { 6f, 7f }.ToStringArray();
            Assert.AreEqual(7.ToString(), arr[1]);
            arr = new[] { 8d, 9d }.ToStringArray();
            Assert.AreEqual(8.ToString(), arr[0]);
            arr = new[] { (decimal)0 }.ToStringArray();
            Assert.AreEqual(0.ToString(), arr[0]);
            arr = new[] { TimeSpan.Zero }.ToStringArray();
            Assert.AreEqual(TimeSpan.Zero.ToString(), arr[0]);
            arr = new[] { now }.ToStringArray();
            Assert.AreEqual(now.ToString(), arr[0]);
            arr = new[] { offset }.ToStringArray();
            Assert.AreEqual(offset.ToString(), arr[0]);
            arr = new[] { guid }.ToStringArray();
            Assert.AreEqual(guid.ToString(), arr[0]);
        }
    }
}
