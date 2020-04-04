# I/O

Provide the helper functions and extension functions for stream.

In `Trivial.IO` [namespace](../) of `Trivial.dll` [library](../../).

## Copy

You can copy a stream to another one with progress reporting supports.

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

## Separate

You can separate a big stream.

```csharp
const SIZE_PER_STREAM = 1024;
using (var a = new FileStream("a.png", FileMode.Open))
{
    var streamCollection = a.Separate(SIZE_PER_STREAM);
}
```
