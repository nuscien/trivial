# JSON

Includes writable JSON DOM and lots of commonly used JSON converters.

In `Trivial.Text` [namespace](./text) of `Trivial.dll` [library](../../).

## JSON DOM

You can create a writable JSON DOM including JSON object `JsonObjectNode` and JSON array `JsonArrayNode`.

```csharp
var json = new JsonObjectNode
{
    { "prop-a", 1234 },
    { "prop-b", "opq" },
    { "prop-c", true },
    { "prop-d", new JsonArrayNode { 5678, "rst" } }
};
```

You can also get or set the property of JSON object as you want.

```csharp
var num = json.GetInt32Value("prop-a"); // 1234
var numStr = json.GetStringValue("prop-a"); // "1234"

json.SetValue("prop-a", 5678);
num = json.GetInt32Value("prop-a"); // 5678
```

For scenarios of concurrency, you can enable thread-safe mode.

```csharp
json.EnableThreadSafeMode();
```

At last, you can write to an instance of `System.Text.Json.Utf8JsonWriter` by `WriteTo` member method or get the JSON format string by `ToString` member method.

```csharp
var jsonStr = json.ToString(IndentStyles.Compact);
```

## JSON converters

Includes a lot of useful JSON converters so that you can use `System.Text.Json.Serialization.JsonConvertAttribute` attribute to use for member properties of the model.

Following are date time related.

| Converter | .NET type | JSON value kind (serialize/deserialize) | Additional JSON value kind (deserialize only) | Nullable |
| ----------------- | ---------- | ---------- | ---------- | --- |
| `JsonJavaScriptTicksConverter` | `DateTime` | JavaScript ticks `number` | Date JSON `string` | × |
| `JsonJavaScriptTicksConverter.NullableConverter` | `DateTime?` | JavaScript ticks `number` | Date JSON `string` | √ |
| `JsonJavaScriptTicksConverter.FallbackConverter` | `DateTime` | Date JSON `string` |JavaScript ticks `number` | × |
| `JsonJavaScriptTicksConverter.FallbackNullableConverter` | `DateTime?` | Date JSON `string` |JavaScript ticks `number` | √ |
| `JsonUnixTimestampConverter` | `DateTime` | Unix timestamp `number` | Date JSON `string` | × |
| `JsonUnixTimestampConverter.NullableConverter` | `DateTime?` | Unix timestamp `number` | Date JSON `string` | √ |
| `JsonUnixTimestampConverter.FallbackConverter` | `DateTime` | Date JSON `string` | Unix timestamp `number` | × |
| `JsonUnixTimestampConverter.FallbackNullableConverter` | `DateTime?` | Date JSON `string` | Unix timestamp `number` | √ |

Following are number related.

| Converter | .NET type | JSON value kind (serialize/deserialize) | Additional JSON value kind (deserialize only) | Nullable |
| ----------------- | ---------- | ---------- | ---------- | --- |
| `JsonNumberConverter` | number types | `number` | number `string` | √ |
| `JsonNumberConverter.NumberStringConverter` | number types | number `string` | `number` | √ |
| `JsonNumberConverter.StrictConverter` | number types | `number` | number `string` | × |

Following are string collection related.

| Converter | .NET type | JSON value kind | Nullable |
| ----------------- | ---------- | ---------- | --- |
| `JsonStringListConverter` | common class of `IEnumerable<string>` | `string` or `string[]` | √ |
| `JsonStringListConverter.WhiteSpaceSeparatedConverter` | common class of `IEnumerable<string>` | `string` or `string[]` | √ |
| `JsonStringListConverter.CommaSeparatedConverter` | common class of `IEnumerable<string>` | `string` or `string[]` | √ |
| `JsonStringListConverter.SemicolonSeparatedConverter` | common class of `IEnumerable<string>` | `string` or `string[]` | √ |
| `JsonStringListConverter.VerticalBarSeparatedConverter` | common class of `IEnumerable<string>` | `string` or `string[]` | √ |

Following are others.

| Converter | .NET type | JSON value kind | Nullable |
| ----------------- | ---------- | ---------- | --- |
| `JsonObjectNodeConverter` | `JsonObjectNode` or `JsonArrayNode` | `object` or `array` | √ |

For example.

```csharp
public class Model
{
    [JsonConverter(typeof(JsonNumberConverter))
    public int Number { get; set; }

    [JsonPropertyName("creation")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))
    public DateTime CreationTime { get; set; }

    [JsonConverter(typeof(JsonObjectNodeConverter)]
    public JsonObject Properties { get; set; }
}
```

Now you can deserialize following JSON.

```json
{
    "number": "1234",
    "creation": 1577628663614,
    "properties": {
        "items": [ 5, 6, 7, "a", "b", "c" ]
        "b": true
    }
}
```