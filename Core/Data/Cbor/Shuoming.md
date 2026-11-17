# CBOR

在 `Trivial.Data` [命名空间](../)中。

<!-- Start -->

简明二进制对象表示（Concise Binary Object Representation）。

## CBOR 写入器

`SimpleCborStreamWriter` 类直接将 CBOR 写入可写的 `Stream`。它不读取、查找、刷新或关闭流。

> 此类已过时，将在未来版本中删除。请改用 .NET 最新版本（12 或更高版本）中的 `CborWriter` 类。

```csharp
using var stream = new MemoryStream();
var writer = new SimpleCborStreamWriter(stream);
writer.StartObject();
writer.AppendProperty("name", "sample");
writer.AppendProperty("created", DateTime.UtcNow);
writer.AppendNullProperty("optional");
writer.StartArray("items");
writer.AppendItem(42);
writer.AppendNullItem();
writer.StartObject();
writer.AppendProperty("enabled", true);
writer.EndObject();
writer.EndArray();
writer.EndObject();
byte[] cbor = stream.ToArray();
```

- 数组和对象使用 CBOR 不定长容器，因此不需要预先知道它们的大小。对象是具有 UTF-8 文本键的映射。这不是确定性/规范的 CBOR。
- 无参数的 `StartArray()` / `StartObject()` 写入根或数组元素；接受键的重载写入对象属性。结束方法必须与最内层的容器匹配。
- `AppendItem` 也支持标量根。每个写入器恰好写入一个根值。
- 属性和项的重载支持 `string`、`byte[]`、`bool`、`long`、`ulong`、`float`、
  `double`、`decimal`、`DateTime` 和 `DateTimeOffset`。较小的整数类型会隐式转换。
  空字符串/字节数组编码为 CBOR null；对于无类型的 null，请使用显式的 null 方法。
- 日期使用带有不变 RFC 3339 文本的标签 0。`DateTime` 被规范化为 UTC；
  `DateTimeKind.Unspecified` 被解释为 UTC。`DateTimeOffset` 保留其偏移量。
- 十进制值使用标签 4，具有精确的指数和尾数，根据需要使用大数标签 2/3。
  浮点值保留其 32/64 位宽度，并支持 NaN 和无穷大。
- 无效的容器操作、空键和无效的 UTF-16 文本在写入字节之前会抛出异常。
  流错误会传播；在 I/O 失败后丢弃写入器，因为输出可能是部分的。
  写入器不是线程安全的。完成每个容器并根据需要刷新/处理流。