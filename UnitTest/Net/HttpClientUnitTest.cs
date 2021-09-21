using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Security;
using Trivial.Text;

namespace Trivial.Net
{
    /// <summary>
    /// HTTP URI unit test.
    /// </summary>
    [TestClass]
    public class HttpClientUnitTest
    {
        /// <summary>
        /// Tests query data.
        /// </summary>
        [TestMethod]
        public void TestHttpClientExtensions()
        {
            using var content = HttpClientExtensions.CreateJsonContent((byte)0);
            Assert.IsNotNull(content);
            var mime = content.Headers.ContentType.ToString().YieldSplit(new List<string> { " " }).FirstOrDefault().YieldSplit(";", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            Assert.AreEqual(Web.WebFormat.JsonMIME, mime);

            var oauth = new OAuthClient("someone", "secret", null);
            oauth.Scope.Add("test");
            oauth.Scope.Add("another");
            Assert.AreEqual("test another", oauth.ScopeString);
            oauth.ScopeString = "query";
            var oneSec = TimeSpan.FromSeconds(1);
            oauth.Timeout = oneSec;
            Assert.AreEqual(1, oauth.Scope.Count);
            var client = oauth.Create<JsonModel>();
            Assert.AreEqual(oneSec, client.Timeout);
        }
    }
}
