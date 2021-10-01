# Code 128

Utility for Code 128 and GS1-128.

In `Trivial.Data` [namespace](../) of `Trivial.Mime.dll` [library](../../).

## Code 128

Code 128 is a high-density linear barcode symbology defined in ISO/IEC 15417:2007.
It is used for alphanumeric or numeric-only barcodes.

You can create a Code 128 instance by passing a collection with any of following information.

- Boolean values that white represented as false and black represented as true.
- Symbol values.

The checksum (the symbol before `Stop`) can be ignored. And you can also parse a string to create the instance.

After creating the instance of Code 128, you can get the areas information that white represented as 0 and black represented as 1.

```csharp
// Create by symbol values without checksum.
var code128 = Code128.CreateB(new byte[] { 43, 73, 78, 71, 67, 69, 65, 78 });
Console.WriteLine(code128.ToString()); // -> Kingcean
Console.WriteLine(code128.ToString(Code128.Formats.Hex)); // -> 682b494e474345414e406a
Console.WriteLine(code128.ToBarcodeString());

// Parse a string.
code128 = Code128.CreateA("Kingcean");
Console.WriteLine(code128.ToString()); // -> Kingcean
```

## GS1-128

GS1-128 is an application standard of the GS1 implementation using the Code 128 barcode specification.

You can create a GS1-128 code as following.

```csharp
// Create by an application identifier and its data value.
var ean128 = Code128.CreateGs1(421, "84020500");
Console.WriteLine(ean128.ToString()); // -> [FNC1]4218402050
Console.WriteLine(ean128.GetAiData().First()); // -> 42184020500

// Equivalent usage as following.
ean128 = Code128.CreateC(new byte[] { 102, 42, 18, 40, 20, 50, 101, 16 });
Console.WriteLine(ean128.ToString(Code128.Formats.Text)); // -> 42184020500
```

## Combination

You can add 2 or more Code 128 instances to generate a new one.

```csharp
// By operator plus (+)
var a = Code128.CreateB("Good ") + Code128.CreateB("morning!");
Console.WriteLine(a.ToString()); // -> Good morning!
```

We also provide a way to combine a set of Code 128 instances.

```csharp
var b = Code128.Join(new List<Code128>
{
    Code128.CreateB("How "),
    Code128.CreateB("are ")
    Code128.CreateB("you?")
});
Console.WriteLine(b.ToString()); // -> How are you?
```

## SVG

Following is a sample to convert Code 128 (and GS1-128) to SVG.

```csharp
public static string ToSvgString(Code128 code, int height, byte r, byte g, byte b)
    => $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\"><g><path d=\"{code.ToPathString(height)}\" stroke=\"#{r:x2)}{g:x2}{b:x2}\"></path></g></svg>";
```

Usage.

```csharp
// var code128 = Code128.CreateA("Kingcean");
var svg = ToSvgString(code128, 40, 0x33, 0x33, 0x33);
```
