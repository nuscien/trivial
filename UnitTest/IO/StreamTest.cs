using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.IO;

namespace Trivial.UnitTest.IO
{
    [TestClass]
    public class StreamTest
    {
        [TestMethod]
        public void TestStream()
        {
            var str = "abcdefghijklmnopqrstuvwxyz";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            var streamArr = stream.Separate(10).ToList();
            Assert.IsTrue(streamArr.Count > 0);
            var reader = new CharsReader(streamArr, Encoding.UTF8);
            Assert.AreEqual(str, reader.ReadToEnd());
            foreach (var s in streamArr)
            {
                s.Dispose();
            }
        }
    }
}
