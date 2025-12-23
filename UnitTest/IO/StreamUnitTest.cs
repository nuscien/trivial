using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.IO
{
    /// <summary>
    /// Stream unit test.
    /// </summary>
    [TestClass]
    public class StreamUnitTest
    {
        /// <summary>
        /// Test stream utilities.
        /// </summary>
        [TestMethod]
        public void TestStream()
        {
            var str = "abcdefghijklmnopqrstuvwxyz 中文";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            var streamArr = stream.Separate(10).ToList();
            Assert.IsNotEmpty(streamArr);
            var reader = new CharsReader(streamArr, Encoding.UTF8);
            Assert.AreEqual(str, reader.ReadToEnd());
            foreach (var s in streamArr)
            {
                s.Dispose();
            }

            reader.Close();

            str = "abcdefg\rhijklmn\nopq rst\r\nuvw xyz";
            using var stream2 = CharsReader.ToStream(str);
            var lines = CharsReader.ReadLines(stream2, Encoding.UTF8).ToList();
            Assert.HasCount(4, lines);

            reader = new CharsReader(str);
            Assert.AreEqual('a', reader.Peek());
            Assert.AreEqual('a', reader.Read());
            var arr = new char[] { ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
            reader.Read(arr, 1, 4);
            Assert.AreEqual('b', arr[1]);
            reader.ReadLine();
            reader.Dispose();
        }
    }
}
