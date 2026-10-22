# 字符读取器

在 `Trivial.IO` [命名空间](./io)中。

<!-- Start -->

提供用于读取字符集合或流集合的文本读取器。

## 字符读取器

有时，你会得到多个流，例如从在线资源或本地文件加载缓冲区，然后你想将它们合并并读取文本。你可以使用 `CharsReader` 来实现。

```csharp
var streams = /* IEnumerable<Stream> */;

// 创建文本读取器。
var reader = new CharsReader(streams);

// Read line one by one.
while (true)
{
    var line = reader.ReadLine();
    if (line == null) break;
    Console.WriteLine(line);
}
```

你也可以直接调用静态方法 `ReadChars` 来获取字符，或调用静态方法 `ReadLines` 来获取行。
