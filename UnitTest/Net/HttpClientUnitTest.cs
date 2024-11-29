using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Collection;
using Trivial.Data;
using Trivial.Reflection;
using Trivial.Security;
using Trivial.Tasks;
using Trivial.Text;

namespace Trivial.Net;

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
        Assert.AreEqual(JsonValues.JsonMIME, mime);

        var oauth = new OAuthClient("someone", "secret", null);
        oauth.Scope.Add("test");
        oauth.Scope.Add("another");
        Assert.AreEqual("test another", oauth.ScopeString);
        oauth.ScopeString = "query";
        Assert.AreEqual(1, oauth.Scope.Count);
        var client = oauth.Create<JsonModel>();
    }

    /// <summary>
    /// Tests server-sent event.
    /// </summary>
    [TestMethod]
    public void TestServerSentEvent()
    {
        var sse = new List<ServerSentEventInfo>
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
        sse = ServerSentEventInfo.Parse(s).ToList();
        Assert.AreEqual(3, sse.Count);
        using var stream = new MemoryStream();
        sse.WriteTo(stream);
        stream.Seek(0, SeekOrigin.Begin);
        sse = ServerSentEventInfo.Parse(stream).ToList();
        stream.Close();
        Assert.AreEqual(3, sse.Count);
        Assert.AreEqual("1", sse[0].Id);
        var json = sse[2].GetJsonData();
        Assert.IsNotNull(json);
        Assert.AreEqual("中文测试", json.GetStringValue("message"));
    }
}
