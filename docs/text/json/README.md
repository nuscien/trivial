# JSON

Includes writable JSON DOM and lots of commonly used JSON converters.

In `Trivial.Text` [namespace](./text) of `Trivial.dll` [library](../../).

## JSON DOM

You can create a writable JSON DOM including JSON object `JsonObjectNode` and JSON array `JsonArrayNode`.

```csharp
var json = new JsonObjectNode
{
    { "prop-a", 1234 }, // Add property with name and value
    { "prop-b", "opq" }, // Add another property
    { "prop-c", true },
    { "prop-d", new JsonArrayNode { 5678, "rst" } }
};
```

You can also get or set the property of JSON object as you want.

```csharp
// Get the count of properties.
var num = json.Count; // 4
num = json.Keys.Count; // 4

// Get a property in different way.
var num = json.GetInt32Value("prop-a"); // 1234
num = json.GetValue<int>("prop-a"); // 1234
num = json.TryGetInt32Value("prop-a") ?? 0; // 1234

// Get a property and convert to another type.
var numStr = json.GetStringValue("prop-a"); // "1234"
numStr = json.GetValue<string>("prop-a"); // "1234"
numStr = json.TryGetStringValue("prop-a"); // "1234"

// Test if exists.
var has = json.ContainsKey("prop-a"); // true
has = json.ContainsKey("non-exist"); // false
var kind = json.GetValueKind("prop-a"); // JsonValueKind.Number

// Add a property.
json.SetValue("prop-e", "uvw");
numStr = json.GetValue<string>("prop-e"); // "uvw"

// Override a property.
json.SetValue("prop-a", 5678);
num = json.GetValue<int>("prop-a"); // 5678

// Remove a property
json.Remove("prop-e");
has = json.ContainsKey("prop-e"); // false
```

At last, you can write to an instance of `System.Text.Json.Utf8JsonWriter` by `WriteTo` member method or get the JSON format string by `ToString` member method.

```csharp
var jsonStr = json.ToString(IndentStyles.Compact); // "{ \"prop-a\": 5678, … }"
```

## JSON access in thread-safe

For scenarios of concurrency, you can enable thread-safe mode.

```csharp
json.EnableThreadSafeMode();
```

## JSON switch

If the JSON node is in different value kinds in different cases, you can

- call `Switch` extension method to get the instance to configure routes to handle these cases, or
- call `SwitchValue` method for a specific JSON object property.

These methods return the `JsonSwitchContext` instance of the specific JSON node or property value.
The instance has `Case`, `Config`, `Default` and other related methods
used to configure predicate and callback handlers of routes.

```csharp
json.SwitchValue("prop-a")
    .Case<string>(s => { /* not matched */ })
    .Case<int>(i => { /* matched and i == 5678 */ })
    .Default(() => { /* not matched */ });
json.SwitchValue("prop-b")
    .Case("uvw", () => { /* not matched */ })
    .Case("opq", () => { /* matched */ })
    .Default(() => { /* not matched */ });
json.SwitchValue("prop-c")
    .Case("uvw", () => { /* not matched */ })
    .Default(node => { /* matched and node == JsonBooleanNode.True */ });
json.SwitchValue("prop-d")
    .Case(node => node is JsonArrayNode arr && arr.Length == 2, node => { /* matched and node is the JsonArrayNode */ })
     .Default(node => { /* not matched */ });
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