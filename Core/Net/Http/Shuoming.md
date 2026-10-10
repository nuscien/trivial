# HTTP 扩展和 JSON 支持

在 `Trivial.Net` [命名空间](../) 中。

## HTTP 客户端扩展

现在你可以直接从 `System.Net.Http.HttpContent` 写入特定文件或序列化为特定类型，通过以下扩展方法。

- `WriteFileAsync` 写入特定文件并支持进度报告。
- `SerializeJsonAsync` 通过 JSON 序列化为特定类型。
- `SerializeXmlAsync` 通过 XML 序列化为特定类型。
- `SerializeAsync` 通过给定的序列化程序序列化为特定类型。

## JSON HTTP 客户端

许多 Web 响应主体基于 JSON 格式，因此我们提供了一种简单的方法来反序列化它们。你可以使用响应模型的类型初始化 `JsonHttpClient` 类的实例。以下是一个示例。

```csharp
// 假设你有一个具有属性 Name 和 Description 的类 NameAndDescription。
const url = "https://github.com/compositejs/datasense/raw/master/package.json";
var webClient = new JsonHttpClient<NameAndDescription>();
var packageInfo = await webClient.SendAsync(HttpMethod.Get, url);

// packageInfo.Name == "datasense"
```

你也可以将泛型参数类型设置为 `JsonObjectNode`，以便获取 DOM。

```csharp
// 假设你有一个具有属性 Name 和 Description 的类 NameAndDescription。
var webClient2 = new JsonHttpClient<JsonObjectNode>();
var json = await webClient2.SendAsync(HttpMethod.Get, url);

// json.TryGetValue<string>("Name") == "datasense"
```

## Server-sent event

`JsonHttpClient` 类还支持服务器发送事件（SSE），通过将泛型参数类型设置为 `IAsyncEnumerable<ServerSentEventInfo>`。

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

对于服务器端（ASP.NET）推送数据，你可以使用 `Trivial.Web` 包中的 `ToActionResult` 扩展方法。

```csharp
public IActionResult Streaming() // 该方法在一个控制器中
{
    IAsyncEnumerable<ServerSentEventInfo> sse = GetStreamingData();
    return sse.ToActionResult();
}
```

## 常用类型

以下类型是 `JsonHttpClient<T>` 泛型参数类型的常用类型。

| HTTP 响应内容的描述 | 泛型参数类型 `T` |
| -------------------- | ---------- |
| `application/json` (JSON 对象) | `JsonObjectNode` |
| JSON 数组 | `JsonArrayNode` |
| `application/jsonl` (JSON 对象行) | `IAsyncEnumerable<JsonObjectNode>` |
| `text/event-stream` (SSE) | `IAsyncEnumerable<ServerSentEventInfo>` |
| `application/x-www-form-urlencoded` | `QueryData` |
| 文本 | `string` |
| 多行文本 | `IAsyncEnumerable<string>` |

对于 JSON DOM 情况，你也可以将 `T` 设置为以下类型。

- JSON 对象: `Newtonsoft.Json.Linq.JObject` 或 `System.Text.Json.JsonDocument`。
- JSON 数组: `Newtonsoft.Json.Linq.JArray`。
