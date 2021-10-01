# International Article Number

Utility for barcode symbology and numbering system EAN.

In `Trivial.Data` [namespace](../) of `Trivial.Mime.dll` [library](../../).

## EAN

You can parse an array or a string with any of following information to an EAN with 2-, 5-, 8- or 13-code.

- Boolean values that white represented as false and black represented as true.
- Areas that white represented as 0 and black represented as 1.
- Digits.

The checksum (last digit of EAN-8 and EAN-13) can be ignored.

After creating the instance of EAN, you can get the areas information that white represented as 0 and black represented as 1.

```csharp
// Create by parsing an EAN-13.
var ean = InternationalArticleNumber.Create("5901234123457");
Console.WriteLine(ean.ToString()); // -> 5901234123457
Console.WriteLine(ean.ToBarcodeString());

// Create by parsing an EAN-13 without checksum.
ean = InternationalArticleNumber.Create("400399415548");
Console.WriteLine(ean.ToString()); // -> 4003994155486
Console.WriteLine(ean.ToBarcodeString());

// Create by parsing an ISBN.
ean = InternationalArticleNumber.Create("978-0-306-40615-7");
Console.WriteLine(ean.ToString()); // -> 9780306406157
Console.WriteLine(ean.ToBarcodeString());

// Create by parsing areas that white represented as 0 and black represented as 1.
ean = InternationalArticleNumber.Create("1010111011011110101100010011001010101000010100111010000101000100101");
Console.WriteLine(ean.ToString()); // -> 73513537
Console.WriteLine(ean.ToBarcodeString());
```

You can also call as following instead of `Console.WriteLine` to write the barcode to standard output stream (terminal).

```charp
ean.ToBarcode(Trivial.CommandLine.StyleConsole.Default);
```

## SVG

Following is a sample to convert EAN to SVG.

```csharp
public static string ToSvgString(InternationalArticleNumber code, int height, byte r, byte g, byte b)
    => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\"><g><path d=\"{code.ToPathString(height)}\" stroke=\"#{r:x2)}{g:x2}{b:x2}\"></path></g></svg>";
```

Usage.

```csharp
// var ean = InternationalArticleNumber.Create("4003994155486");
var svg = ToSvgString(ean, 40, 0x33, 0x33, 0x33);
```
