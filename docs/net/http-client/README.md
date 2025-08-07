# HTTP client

HTTP client extensions and JSON HTTP client.

In `Trivial.Net` [namespace](./net) of `Trivial.dll` [library](../../).

## HTTP client extensions

Now you can write to a specific file or serialize as a specific type directly from a `System.Net.Http.HttpContent` by following extension method now.

- `WriteFileAsync` Write to a specific file with progress report supports.
- `SerializeJsonAsync` Serialize as a specific type by JSON.
- `SerializeXmlAsync` Serialize as a specific type by XML.
- `SerializeAsync` Serialize as a specific type by a given serializer.

## JSON HTTP client

Lots of web response body is based on JSON format so we provide an easy way to deserialize them. You can initialize an instance of `JsonHttpClient` class with a type of response model. Following is an example.

```csharp
// Suppose you have a class NameAndDescription with properties Name and Description.
const url = "https://github.com/compositejs/datasense/raw/master/package.json";
var webClient = new JsonHttpClient<NameAndDescription>();
var packageInfo = await webClient.SendAsync(HttpMethod.Get, url);

// packageInfo.Name == "datasense"
```

You can also set the generic argument type as `JsonObjectNode` so that you can get the DOM.

```csharp
// Suppose you have a class NameAndDescription with properties Name and Description.
var webClient2 = new JsonHttpClient<JsonObjectNode>();
var json = await webClient2.SendAsync(HttpMethod.Get, url);

// json.TryGetValue<string>("Name") == "datasense"
```

## Server-sent event

The class `JsonHttpClient` also support server-sent event (SSE) by setting
the generic argument type as `IAsyncEnumerable<ServerSentEventInfo>`.

```csharp
var http = new JsonHttpClient<IAsyncEnumerable<ServerSentEventInfo>>();
var sse = http.GetAsync(A-URL-TO-STREAM-MESSAGE);
await foreach (var item in sse)
{
    if (item.EventName != "message") continue;
    var data = item.GetJsonData();
    Process(data);  // or get item.DataString (original string in data field).
}
```

For server-side (ASP.NET) to push data,
you may use `ToActionResult` extension method in `Trivial.Web` package.

```csharp
public IActionResult Streaming() // This method is in a controller
{
    IAsyncEnumerable<ServerSentEventInfo> sse = GetStreamingData();
    return sse.ToActionResult();
}
```

## Common-used types

The following types are the common-used for the generic argument type of `JsonHttpClient<T>`.

| Description of HTTP response content | Generic argument type `T` |
| -------------------- | ---------- |
| `application/json` (JSON object) | `JsonObjectNode` |
| JSON array | `JsonArrayNode` |
| `application/jsonl` (JSON object lines) | `IAsyncEnumerable<JsonObjectNode>` |
| `text/event-stream` (SSE) | `IAsyncEnumerable<ServerSentEventInfo>` |
| `application/x-www-form-urlencoded` | `QueryData` |
| Text | `string` |
| Text in multiple lines | `IAsyncEnumerable<string>` |

For JSON DOM cases, you can also set `T` as following type.

- JSON object: `Newtonsoft.Json.Linq.JObject` or `System.Text.Json.JsonDocument`.
- JSON array: `Newtonsoft.Json.Linq.JArray`.
