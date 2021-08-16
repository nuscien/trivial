using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Text
{
    /// <summary>
    /// CSV unit test.
    /// </summary>
    [TestClass]
    public class CsvUnitTest
    {
        /// <summary>
        /// Tests CSV parser.
        /// </summary>
        [TestMethod]
        public void TestCsvParser()
        {
            var text = "ab,cd,\"efg,\t\",56789,!!!\nhijk,l,mn,43210";
            var parser = new CsvParser(text);
            var col = parser.ToList();

            Assert.AreEqual(2, col.Count);
            Assert.AreEqual(5, col[0].Count);
            Assert.AreEqual("ab", col[0][0]);
            Assert.AreEqual("cd", col[0][1]);
            Assert.AreEqual("efg,\t", col[0][2]);
            Assert.AreEqual("56789", col[0][3]);
            Assert.AreEqual("!!!", col[0][4]);
            Assert.AreEqual(4, col[1].Count);
            Assert.AreEqual("hijk", col[1][0]);
            Assert.AreEqual("l", col[1][1]);
            Assert.AreEqual("mn", col[1][2]);
            Assert.AreEqual("43210", col[1][3]);

            var models = parser.ConvertTo<JsonModel>(new[] { "A", "B", "C", "Num" }).ToList();
            Assert.AreEqual(2, models.Count);
            Assert.AreEqual("ab", models[0].A);
            Assert.AreEqual("cd", models[0].B);
            Assert.AreEqual("efg,\t", models[0].C);
            Assert.AreEqual(56789, models[0].Num);
            Assert.AreEqual("hijk", models[1].A);
            Assert.AreEqual("l", models[1].B);
            Assert.AreEqual("mn", models[1].C);
            Assert.AreEqual(43210, models[1].Num);

            var jsons = parser.ConvertTo<JsonObjectNode>(new[] { "A", "B", "C", "Num" }).ToList();
            Assert.AreEqual(2, jsons.Count);
            Assert.AreEqual("ab", jsons[0].TryGetStringValue("A"));
            Assert.AreEqual("cd", jsons[0].TryGetStringValue("B"));
            Assert.AreEqual("efg,\t", jsons[0].TryGetStringValue("C"));
            Assert.AreEqual(56789, jsons[0].TryGetInt32Value("Num"));
            Assert.AreEqual("hijk", jsons[1].TryGetStringValue("A"));
            Assert.AreEqual("l", jsons[1].TryGetStringValue("B"));
            Assert.AreEqual("mn", jsons[1].TryGetStringValue("C"));
            Assert.AreEqual(43210, jsons[1].TryGetInt32Value("Num"));
        }

        /// <summary>
        /// Tests TSV parser.
        /// </summary>
        [TestMethod]
        public void TestTsvParser()
        {
            var text = "ab\tcd\t\"efg,\t\"\t56789\t!!!\nhijk\tl\tmn\t43210";
            var parser = new TsvParser(text);
            var col = parser.ToList();

            Assert.AreEqual(2, col.Count);
            Assert.AreEqual(5, col[0].Count);
            Assert.AreEqual("ab", col[0][0]);
            Assert.AreEqual("cd", col[0][1]);
            Assert.AreEqual("efg,\t", col[0][2]);
            Assert.AreEqual("56789", col[0][3]);
            Assert.AreEqual("!!!", col[0][4]);
            Assert.AreEqual(4, col[1].Count);
            Assert.AreEqual("hijk", col[1][0]);
            Assert.AreEqual("l", col[1][1]);
            Assert.AreEqual("mn", col[1][2]);
            Assert.AreEqual("43210", col[1][3]);

            var models = parser.ConvertTo<JsonModel>(new[] { "A", "B", "C", "Num" }).ToList();
            Assert.AreEqual(2, models.Count);
            Assert.AreEqual("ab", models[0].A);
            Assert.AreEqual("cd", models[0].B);
            Assert.AreEqual("efg,\t", models[0].C);
            Assert.AreEqual(56789, models[0].Num);
            Assert.AreEqual("hijk", models[1].A);
            Assert.AreEqual("l", models[1].B);
            Assert.AreEqual("mn", models[1].C);
            Assert.AreEqual(43210, models[1].Num);

            var jsons = parser.ConvertTo<JsonObjectNode>(new[] { "A", "B", "C", "Num" }).ToList();
            Assert.AreEqual(2, jsons.Count);
            Assert.AreEqual("ab", jsons[0].TryGetStringValue("A"));
            Assert.AreEqual("cd", jsons[0].TryGetStringValue("B"));
            Assert.AreEqual("efg,\t", jsons[0].TryGetStringValue("C"));
            Assert.AreEqual(56789, jsons[0].TryGetInt32Value("Num"));
            Assert.AreEqual("hijk", jsons[1].TryGetStringValue("A"));
            Assert.AreEqual("l", jsons[1].TryGetStringValue("B"));
            Assert.AreEqual("mn", jsons[1].TryGetStringValue("C"));
            Assert.AreEqual(43210, jsons[1].TryGetInt32Value("Num"));
        }

        /// <summary>
        /// Tests W3C Extended Log parser.
        /// </summary>
        [TestMethod]
        public void TestExtendedLogParser()
        {
            var text = "#Version: 1.0\n#Date: 12-Jan-1996 00:00:00\n#Fields: time cs-method cs-uri\n00:34:23 GET /foo/bar.html\n12:21:16 GET /foo/bar.html\n12:45:52 GET \"/foo/bar.html\"\n12:57:34 GET /foo/bar.html";
            var parser = new ExtendedLogParser(text);
            var col = parser.ToList();
            Assert.AreEqual(3, parser.Headers.Count);
            Assert.AreEqual("cs-method", parser.Headers[1]);
            Assert.AreEqual(4, col.Count);
            Assert.AreEqual(3, col[0].Count);
            Assert.AreEqual("12:21:16", col[1][0]);
            Assert.AreEqual("/foo/bar.html", col[2][2]);
            Assert.AreEqual("GET", col[3][1]);
            Assert.AreEqual("1.0", parser.GetDirectiveValue("Version"));
            parser.TryGetDirectiveValue("Date", out DateTime dt);
            Assert.AreEqual(1996, dt.Year);
            Assert.AreEqual(1, dt.Month);

            Assert.AreEqual("2345", SubRangeString(2, 6));
            Assert.AreEqual("012345", SubRangeString(0, 6));
            Assert.AreEqual("234567", SubRangeString(2, 2, true));
            Assert.AreEqual("012345678", SubRangeString(0, 1, true));
        }

        internal static string SubRangeString(int start, int end, bool reverseEnd = false)
        {
            var s = "0123456789";
#if NETOLDVER
            return s.Substring(start, reverseEnd ? (s.Length - end - start) : (end - start));
#else
            return reverseEnd ? s[start..^end] : s[start..end];
#endif
        }
    }
}
