# [Trivial.Mime](https://trivial.kingcean.net/web/mime)

Commonly used MIME content types and its file extension part mapping.
Also provide parsers and barcode information generators of Code 128 and EAN-13.

## Import

Add following namespace to your code file to use.

```csharp
using Trivial.Data;
using Trivial.Web;
```

## Content types

Following are the groups of commonly used content type.

- `MimeConstants.Images`
- `MimeConstants.Audio`
- `MimeConstants.Videos`
- `MimeConstants.Documents`
- `MimeConstants.Web`
- `MimeConstants.Text`
- `MimeConstants.Packages`
- `MimeConstants.Multipart`

## File extension mapping

Get MIME by a specific file extension name.

```csharp
var av1 = MimeConstants.GetByFileExtension(".av1");
var pptx = MimeConstants.GetByFileExtension(".pptx");
```

## EAN

Create 2-, 5-, 8- and 13-bit International Article Number to get its barcode information.

```csharp
// Parse an EAN-13.
var ean = InternationalArticleNumber.Create("5901234123457");
Console.WriteLine(ean.ToBarcodeString());

// Parse an EAN-13 without checksum.
ean = InternationalArticleNumber.Create("400399415548");
Console.WriteLine(ean.ToString()); // -> 4003994155486
Console.WriteLine(ean.ToPathString()); // Can be used as data of path element of SVG or WPF.

// Parsing an ISBN.
ean = InternationalArticleNumber.Create("978-0-306-40615-7");
Console.WriteLine(ean.ToString()); // -> 9780306406157

// Parsing barcode areas that white represented as 0 and black represented as 1.
ean = InternationalArticleNumber.Create("1010111011011110101100010011001010101000010100111010000101000100101");
Console.WriteLine(ean.ToString()); // -> 73513537
```

## Code 128

Create code 128 to get its barcode information.

```csharp
// Create by symbol values without checksum.
var code128 = Code128.CreateB(new byte[] { 43, 73, 78, 71, 67, 69, 65, 78 });
Console.WriteLine(code128.ToString()); // -> Kingcean
Console.WriteLine(code128.ToString(Code128.Formats.Hex)); // -> 682b494e474345414e406a
Console.WriteLine(code128.ToBarcodeString());

// Parse a string.
code128 = Code128.CreateA("Trivial libraries");
Console.WriteLine(code128.ToPathString()); // Can be used as data of path element of SVG or WPF.
```

And for GS1-128.

```csharp
// Create by an application identifier and its data value.
var ean128 = Code128.CreateGs1(421, "84020500");
Console.WriteLine(ean128.GetAiData().First()); // -> 42184020500
```
