# JSON-RPC

在 `Trivial.Tasks` [命名空间](../)中。

<!-- Start -->

你可以托管一个 JSON-RPC 服务来处理函数。

## 路由

你可以创建一个 JSON-RPC 处理程序路由来处理操作并返回结果。

```csharp
internal class Increasement() : JsonRpcRequestHandler("Increase a number.")
{
    public override async Task<BaseJsonRpcResponseObject> ProcessAsync(JsonRpcRequestObject request, JsonRpcRequestRoute route, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        if (request is not JsonRpcRequestObject<JsonObjectNode> r)
            return request.ToErrorResponse(JsonRpcConstants.InvalidRequest);
        var json = r.Parameter;
        if (!json.TryGetInt32Value("value", out var i))
            return request.ToErrorResponse(JsonRpcConstants.InvalidRequest);
        return request.ToSuccessResponse(new JsonObjectNode()
        {
            { "value", i + (json.TryGetInt32Value("delta") ?? 1) }
        });
    }
} 
```

```csharp
var service = new JsonRpcRequestRoute();
service.Register("increase", new Increasement);
```

```csharp
var req = new JsonRpcRequestObject<JsonObjectNode>("increase", new()
{
    { "value", 100 }
});
var resp = await service.ProcessAsync(req); // resp.Parameter.GetValue<int>("value") == 101
```
