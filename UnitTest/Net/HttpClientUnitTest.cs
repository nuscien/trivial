using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Collection;
using Trivial.Security;
using Trivial.Tasks;
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
        /// Tests HTTP client extensions.
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

        /// <summary>
        /// Tests server-sent event.
        /// </summary>
        [TestMethod]
        public void TestServerSentEvent()
        {
            var sse = new List<ServerSentEventRecord>
            {
                new(new JsonObjectNode
                {
                    { "name", "test" },
                    { "message", "abcdefg" }
                }, new Dictionary<string, string>
                {
                    { "id", "1" },
                    { "event", "message" }
                }),
                new(new JsonObjectNode
                {
                    { "name", "another" },
                    { "message", "hijklmn" }
                }.ToString(IndentStyles.Compact).Replace("\r", string.Empty), new Dictionary<string, string>
                {
                    { "id", "2" }
                }),
                new(new JsonObjectNode
                {
                    { "name", "chs" },
                    { "message", "中文测试" },
                    { "opq", "rst uvw xyz" }
                }, new Dictionary<string, string>
                {
                    { "id", "3" },
                    { "event", "finish" }
                })
            };
            var s = sse.ToResponseString(true);
            sse = ServerSentEventRecord.Parse(s).ToList();
            Assert.AreEqual(3, sse.Count);
            using var stream = new MemoryStream();
            sse.ToResponseString(stream);
            stream.Seek(0, SeekOrigin.Begin);
            sse = ServerSentEventRecord.Parse(stream).ToList();
            Assert.AreEqual(3, sse.Count);
            Assert.AreEqual("1", sse[0].Id);
            var json = sse[2].GetJsonData();
            Assert.IsNotNull(json);
            Assert.AreEqual("中文测试", json.GetStringValue("message"));
        }
    }
}
