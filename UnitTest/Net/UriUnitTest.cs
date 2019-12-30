﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Net;
using Trivial.Text;

namespace Trivial.UnitTest.Net
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
            Assert.AreEqual(query.Replace("%20", "+").Replace("\\", "%5c"), q.ToString());
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
            Assert.AreEqual(2, uri.Query.Count);
            Assert.AreEqual("cd", uri.Query["b"]);
            Assert.AreEqual("e", uri.Query[1].Key);
            Assert.AreEqual("fg", uri.Query[1].Value);
            Assert.AreEqual("#hijklmn", uri.Hash);
            Assert.AreEqual(url, uri.ToString());
        }
    }
}
