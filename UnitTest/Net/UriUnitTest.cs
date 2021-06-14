using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Text;

namespace Trivial.Net
{
    /// <summary>
    /// HTTP URI unit test.
    /// </summary>
    [TestClass]
    public class UriUnitTest
    {
        /// <summary>
        /// Tests query data.
        /// </summary>
        [TestMethod]
        public void TestQueryData()
        {
            var json = @"{
    ""o"": [""abcdefg"", true, { ""uvw"": null }],
    ""p"": ""hijk\\\""lmn\"""", // rst
    ""q"": 1234567
}";
            var q = QueryData.Parse(json);
            Assert.AreEqual(3, q.Count);
            Assert.AreEqual(q[0].Value, q["o"]);
            Assert.AreEqual(@"hijk\""lmn""", q[1].Value);
            Assert.AreEqual(q[1].Value, q["p"]);
            Assert.AreEqual("1234567", q[2].Value);
            Assert.AreEqual(q[2].Value, q["q"]);

            var query = "a=bcd\\efg+%20&hij=klmn&o=12345&p=67890&q=";
            q = QueryData.Parse(query);
            Assert.AreEqual(5, q.Count);
            Assert.AreEqual("bcd\\efg  ", q["a"]);
            Assert.AreEqual(q["a"], q[0].Value);
            Assert.AreEqual("klmn", q["hij"]);
            Assert.AreEqual(q["hij"], q[1].Value);
            Assert.AreEqual("12345", q[2].Value);
            Assert.AreEqual(q[2].Value, q["o"]);
            Assert.AreEqual("67890", q[3].Value);
            Assert.AreEqual(q[3].Value, q["p"]);
            Assert.AreEqual(string.Empty, q[4].Value);
            Assert.AreEqual(q[4].Value, q["q"]);
#if NETFRAMEWORK
            var backslash = "%5C";
#else
            var backslash = "%5c";
#endif
            Assert.AreEqual(query.Replace("%20", "+").Replace("\\", backslash), q.ToString());
        }

        /// <summary>
        /// Tests HTTP URI.
        /// </summary>
        [TestMethod]
        public void TestHttpUri()
        {
            var url = "https://kingcean.net/test/a?b=cd&e=fg#hijklmn";
            var uri = (HttpUri)new Uri(url);
            Assert.AreEqual(true, uri.IsSecure);
            Assert.AreEqual("kingcean.net", uri.Host);
            Assert.AreEqual("/test/a", uri.Path);
            Assert.AreEqual(2, uri.PathItems.Count);
            Assert.AreEqual(2, uri.Query.Count);
            Assert.AreEqual("cd", uri.Query["b"]);
            Assert.AreEqual("e", uri.Query[1].Key);
            Assert.AreEqual("fg", uri.Query[1].Value);
            Assert.AreEqual("#hijklmn", uri.Hash);
            Assert.AreEqual(url, uri.ToString());
            uri = HttpUri.Parse(url);
            Assert.AreEqual(url, uri.ToString());

            url = "https://user:password@kingcean.net:12345/test/a?b=cd&e=fg#hijklmn";
            uri = HttpUri.Parse(url);
            Assert.AreEqual(true, uri.IsSecure);
            Assert.AreEqual("kingcean.net", uri.Host);
            Assert.AreEqual("user:password", uri.AccountInfo);
            Assert.AreEqual(12345, uri.Port);
            Assert.AreEqual("/test/a", uri.Path);
            Assert.AreEqual(2, uri.PathItems.Count);
            Assert.AreEqual(2, uri.Query.Count);
            Assert.AreEqual("cd", uri.Query["b"]);
            Assert.AreEqual("e", uri.Query[1].Key);
            Assert.AreEqual("fg", uri.Query[1].Value);
            Assert.AreEqual("#hijklmn", uri.Hash);
            Assert.AreEqual(url, uri.ToString());
            uri = HttpUri.Parse(url);
            Assert.AreEqual(url, uri.ToString());

            url = "trivial://kingcean.net/test/a?b=cd&e=fg#hijklmn";
            var link = AppDeepLinkUri.Parse(url);
            Assert.AreEqual("trivial", link.Protocal);
            Assert.AreEqual("kingcean.net", link.Host);
            Assert.AreEqual("/test/a", link.Path);
            Assert.AreEqual(2, link.PathItems.Count);
            Assert.AreEqual(2, link.Query.Count);
            Assert.AreEqual("cd", link.Query["b"]);
            Assert.AreEqual("e", link.Query[1].Key);
            Assert.AreEqual("fg", link.Query[1].Value);
            Assert.AreEqual("#hijklmn", link.Hash);
            Assert.AreEqual(url, link.ToString());
            link = AppDeepLinkUri.Parse(url);
            Assert.AreEqual(url, link.ToString());
        }
    }
}
