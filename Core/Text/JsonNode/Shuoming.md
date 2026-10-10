# JSON

包含可写的 JSON 节点和许多常用的 JSON 转换器。

位于 `Trivial.Text` [命名空间](../) 中。

## JSON 节点

你可以创建一个可写的 JSON 节点，包括 JSON 对象 `JsonObjectNode` 和 JSON 数组 `JsonArrayNode`。

```csharp
var json = new JsonObjectNode
{
    { "prop-a", 1234 }, // 添加具有名称和值的属性
    { "prop-b", "opq" }, // 添加另一个属性
    { "prop-c", true },
    { "prop-d", new JsonArrayNode { 5678, "rst" } }
};
```

你也可以根据需要获取或设置 JSON 对象的属性。

```csharp
// 获取属性的数量。
var num = json.Count; // 4
num = json.Keys.Count; // 4

// 以不同的方式获取属性。
var num = json.GetInt32Value("prop-a"); // 1234
num = json.GetValue<int>("prop-a"); // 1234
num = json.TryGetInt32Value("prop-a") ?? 0; // 1234

// 获取属性并转换为另一种类型。
var numStr = json.GetStringValue("prop-a"); // "1234"
numStr = json.GetValue<string>("prop-a"); // "1234"
numStr = json.TryGetStringValue("prop-a"); // "1234"

// 测试是否存在。
var has = json.ContainsKey("prop-a"); // true
has = json.ContainsKey("non-exist"); // false
var kind = json.GetValueKind("prop-a"); // JsonValueKind.Number

// 添加属性。
json.SetValue("prop-e", "uvw");
numStr = json.GetValue<string>("prop-e"); // "uvw"

// 覆盖属性。
json.SetValue("prop-a", 5678);
num = json.GetValue<int>("prop-a"); // 5678

// 删除属性
json.Remove("prop-e");
has = json.ContainsKey("prop-e"); // false
```

## 序列化

你可以通过 `WriteTo` 成员方法将 JSON 写入 `System.Text.Json.Utf8JsonWriter` 的实例，或者通过 `ToString` 成员方法获取 JSON 格式的字符串。

```csharp
var jsonStr = json.ToString(IndentStyles.Compact); // "{ \"prop-a\": 5678, … }"
```

调用 `JsonObjectNode.Parse` 将 JSON 格式的字符串或其 UTF-8 流反序列化为节点对象。

```csharp
json = JsonObjectNode.Parse(jsonStr);
```

如果你有一个类需要从 JSON 对象节点反序列化，请调用其 `Deserialize` 成员方法。
或者调用 `ConvertFrom` 静态方法将类转换回 JSON 对象节点。

你也可以获取 YAML 字符串。

```csharp
var yaml = json.ToYamlString();
```

## 线程安全模式

对于并发场景，你可以启用线程安全模式。

```csharp
json.EnableThreadSafeMode();
```

## JSON 值路由

如果 JSON 节点在不同情况下具有不同的值类型，你可以

- 调用 `Switch` 扩展方法获取实例以配置路由来处理这些情况，或者
- 调用 `SwitchValue` 方法处理特定的 JSON 对象属性。

这些方法返回特定 JSON 节点或属性值的 `JsonSwitchContext` 实例。
该实例具有 `Case`、`Config`、`Default` 和其他相关方法
用于配置路由的谓词和回调处理程序。

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

## JSON 转化器

包含许多有用的 JSON 转化器，以便你可以使用 `System.Text.Json.Serialization.JsonConvertAttribute` 特性来为模型的成员属性使用。

以下是日期时间相关的。

| 转化器 | .NET 类型 | JSON 值类型 (序列化/反序列化) | 额外的 JSON 值类型 (仅反序列化) |
| ----------------- | ---------- | ---------- | ---------- |
| `JsonJavaScriptTicksConverter` | `DateTime` | JavaScript ticks `number` | Date JSON `string` |
| `JsonJavaScriptTicksConverter.FallbackConverter` | `DateTime` | Date JSON `string` | JavaScript ticks `number` |
| `JsonJavaScriptTicksConverter.StringConverter` | `DateTime` | JavaScript ticks in `string` | JavaScript ticks `number` |
| `JsonUnixTimestampConverter` | `DateTime` | Unix timestamp `number` | Date JSON `string` |
| `JsonUnixTimestampConverter.FallbackConverter` | `DateTime` | Date JSON `string` | Unix timestamp `number` |
| `JsonUnixTimestampConverter.StringConverter` | `DateTime` | Unix timestamp in `string` | Unix timestamp `number` |

以下是数字相关的。

| 转化器 | .NET 类型 | JSON 值类型 (序列化/反序列化) | 额外的 JSON 值类型 (仅反序列化) |
| ----------------- | ---------- | ---------- | ---------- |
| `JsonNumberConverter` | 数字类型 | `number` | 数字 `string` |
| `JsonNumberConverter.NumberStringConverter` | 数字类型 | 数字 `string` | `number` |
| `JsonNumberConverter.StrictConverter` | 数字类型 | `number` | 数字 `string` |

以下是字符串集合相关的。

| 转化器 | .NET 类型 | JSON 值类型 |
| ----------------- | ---------- | ---------- |
| `JsonStringListConverter` | `IEnumerable<string>` 的通用类 | `string` 或 `string[]` |
| `JsonStringListConverter.WhiteSpaceSeparatedConverter` | `IEnumerable<string>` 的通用类 | `string` 或 `string[]` |
| `JsonStringListConverter.CommaSeparatedConverter` | `IEnumerable<string>` 的通用类 | `string` 或 `string[]` |
| `JsonStringListConverter.SemicolonSeparatedConverter` | `IEnumerable<string>` 的通用类 | `string` 或 `string[]` |
| `JsonStringListConverter.VerticalBarSeparatedConverter` | `IEnumerable<string>` 的通用类 | `string` 或 `string[]` |

以下是其他的。

| 转化器 | .NET 类型 | JSON 值类型 |
| ----------------- | ---------- | ---------- |
| `JsonObjectNodeConverter` | `JsonObjectNode` 或 `JsonArrayNode` | `object` 或 `array` |

例如。

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

现在你可以反序列化以下 JSON。

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
