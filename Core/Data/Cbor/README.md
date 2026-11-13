# CBOR

In `Trivial.Data` [namespace](../).

<!-- Start -->

Concise Binary Object Representation.

## CBOR writer

Class `SimpleCborStreamWriter` writes CBOR directly to a writable `Stream`. It does not read, seek, flush, or close the stream.

> This class is obsolete and will be removed in a future version. Use the `CborWriter` class in .NET latest version (12 or higher) instead.

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

- Arrays and objects use CBOR indefinite-length containers, so their sizes need not be known
  in advance. Objects are maps with UTF-8 text keys. This is not deterministic/canonical CBOR.
- Parameterless `StartArray()` / `StartObject()` write the root or an array element;
  overloads accepting a key write an object property. End methods must match the innermost container.
- `AppendItem` also supports a scalar root. Each writer writes exactly one root value.
- Property and item overloads support `string`, `byte[]`, `bool`, `long`, `ulong`, `float`,
  `double`, `decimal`, `DateTime`, and `DateTimeOffset`. Smaller integer types convert implicitly.
  Null strings/byte arrays encode CBOR null; use the explicit null methods for untyped nulls.
- Dates use tag 0 with invariant RFC 3339 text. `DateTime` is normalized to UTC;
  `DateTimeKind.Unspecified` is interpreted as UTC. `DateTimeOffset` preserves its offset.
- Decimal values use tag 4 with an exact exponent and mantissa, using bignum tags 2/3 as needed.
  Floating-point values retain their 32/64-bit width and support NaN and infinity.
- Invalid container operations, null keys, and invalid UTF-16 text throw before writing bytes.
  Stream errors propagate; discard the writer after an I/O failure because output may be partial.
  The writer is not thread-safe. Finish every container and flush/dispose the stream as needed.
