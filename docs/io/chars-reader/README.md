# Characters reader

A text reader to read character collection or stream collection.

In `Trivial.IO` [namespace](./io) of `Trivial.dll` [library](../../).

## Characters reader

Sometimes, you will get a number of stream, e.g. load buffer from online resource or local files, then you want to merge them and read text. You can use `CharsReader` to do so.

```csharp
var streams = /* IEnumerable<Stream> */;

// Create the text reader.
var reader = new CharsReader(streams);

// Read line one by one.
while (true)
{
    var line = reader.ReadLine();
    if (line == null) break;
    Console.WriteLine(line);
}
```

And you can call the static method `ReadChars` to get characters or static method `ReadLines` to get lines from the stream collection directly.
