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

And you can also deserialize following types for JSON values accessing directly.

- `System.Text.Json.JsonDocument`
- `Newtonsoft.Json.Linq.JObject`
- `Newtonsoft.Json.Linq.JArray`
- `Trivial.Text.JsonObject`
- `Trivial.Text.JsonArray`

```csharp
// Suppose you have a class NameAndDescription with properties Name and Description.
var webClient2 = new JsonHttpClient<JObject>();
var json = await webClient2.SendAsync(HttpMethod.Get, url);

// packageInfo["Name"] == "datasense"
```
