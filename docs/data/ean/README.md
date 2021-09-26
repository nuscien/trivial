# International Article Number

Utility for barcode symbology and numbering system EAN.

In `Trivial.Data` [namespace](../) of `Trivial.Mime.dll` [library](../../).

## EAN

You can parse an array or a string with following information to an EAN-13 or EAN-8 code.

- Boolean values that white represented as false and black represented as true.
- Areas that white represented as 0 and black represented as 1.
- Digits.

The last digit (checksum) can be ignored.

```csharp
var ean = InternationalArticleNumber.Create("400399415548");
Console.WriteLine(ean.ToString()); // -> 4003994155486

ean = InternationalArticleNumber.Create("1010111011011110101100010011001010101000010100111010000101000100101");
Console.WriteLine(ean.ToString()); // -> 73513537
```
