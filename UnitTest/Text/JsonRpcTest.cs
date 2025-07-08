using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.IO;
using Trivial.Maths;
using Trivial.Tasks;
using Trivial.Web;

namespace Trivial.Text;

/// <summary>
/// JSON unit test.
/// </summary>
[TestClass]
public class JsonRpcUnitTest
{
    internal class TestHandler : JsonRpcRequestHandler
    {
        protected override Task<BaseJsonRpcResponseObject> ProcessAsync(JsonRpcRequestObject request, JsonRpcRequestRoute route, CancellationToken cancellationToken = default)
        {
            if (request is JsonRpcRequestObject<string> s)
                return Task.FromResult<BaseJsonRpcResponseObject>(request.ToSuccessResponse(s.Parameter));
            if (request is JsonRpcRequestObject<int> i)
                return Task.FromResult<BaseJsonRpcResponseObject>(request.ToSuccessResponse(i.Parameter));
            return Task.FromResult<BaseJsonRpcResponseObject>(request.ToErrorResponse(JsonRpcConstants.InvalidRequest, "Invalid request type."));
        }
    }

    /// <summary>
    /// Tests writable JSON DOM.
    /// </summary>
    [TestMethod]
    public async Task TestJsonRpcAsync()
    {
        var route = new JsonRpcRequestRoute();
        route.Register("test", new TestHandler());
        var req1 = new JsonRpcRequestObject<string>("test", "Hello, world!");
        var s = JsonSerializer.Serialize<JsonRpcRequestObject>(req1);
        var resp1 = await route.ProcessAsync(s) as SuccessJsonRpcResponseObject<string>;
        Assert.IsNotNull(resp1);
        s = JsonSerializer.Serialize<BaseJsonRpcResponseObject>(resp1);
        resp1 = JsonSerializer.Deserialize<BaseJsonRpcResponseObject>(s) as SuccessJsonRpcResponseObject<string>;
        Assert.IsNotNull(resp1);
        Assert.AreEqual("Hello, world!", resp1.Result);
        var req2 = new JsonRpcRequestObject<int>("test", 100);
        var resp2 = await route.ProcessAsync(req2) as SuccessJsonRpcResponseObject<int>;
        Assert.IsNotNull(resp2);
        Assert.AreEqual(100, resp2.Result);
        var req3 = new JsonRpcRequestObject<bool>("test", false);
        var resp3 = await route.ProcessAsync(req3) as ErrorJsonRpcResponseObject;
        Assert.IsNotNull(resp1);
        Assert.AreEqual(JsonRpcConstants.InvalidRequest, resp3.Code);
        Assert.IsFalse(string.IsNullOrEmpty(resp3.Message));
    }
}
