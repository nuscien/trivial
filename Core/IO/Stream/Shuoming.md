# I/O

在 `Trivial.IO` [命名空间](../)中。

<!-- Start -->

提供流的辅助函数和扩展函数。

## 复制

可以将一个流复制到另一个流，并支持进度报告。

```csharp
using (var a = new FileStream("a.png", FileMode.Open))
{
    var size = a.Length;
    using (var b = new FileStream("b.png", FileMode.Create))
    {
        var progress = new Progress<long>(bytesCopied =>
        {
            Console.WriteLine("{0:F2}%", bytesCopied * 100.0 / size);
        });
        await a.CopyToAsync(b, progress);
    }
}
```

## 分割

可以分割一个大的流成多个小流。

```csharp
const SIZE_PER_STREAM = 1024;
using (var a = new FileStream("a.png", FileMode.Open))
{
    var streamCollection = a.Separate(SIZE_PER_STREAM);
}
```
