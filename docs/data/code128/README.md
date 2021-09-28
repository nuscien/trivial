# Code 128

Utility for Code 128 and GS1-128.

In `Trivial.Data` [namespace](../) of `Trivial.Mime.dll` [library](../../).

## Code 128

You can create a Code 128 instance by passing a collection with any of following information.

- Boolean values that white represented as false and black represented as true.
- Symbol values.

The checksum (the symbol before `Stop`) can be ignored.

After creating the instance of Code 128, you can get the areas information that white represented as 0 and black represented as 1.

```csharp
// Create Code 128 without checksum.
var code128 = Code128.CreateB(new byte[] { 43, 73, 78, 71, 67, 69, 65, 78 });
Console.WriteLine(ean.ToString()); // -> Kingcean
Console.WriteLine(ean.ToBarcodeString());

// Create GS1-128 without checksum.
code128 = Code128.CreateC(new byte[] { 102, 42, 18, 40, 20, 50, 101, 16 });
Console.WriteLine(ean.ToString()); // -> [FNC1]4218402050
Console.WriteLine(code128.GetAiData().First());	// -> 42184020500
```
