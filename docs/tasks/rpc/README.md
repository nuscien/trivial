# JSON-RPC

You can host a JSON-RPC service to handle the functions.

In `Trivial.Tasks` [namespace](../) of `Trivial.dll` library.

## Route

You can create a JSON-RPC handler route to process the actions and return results.

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
