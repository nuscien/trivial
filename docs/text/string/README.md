# String Utilities

Provides some helper functions and extension functions for text.

In `Trivial.Text` [namespace](./text) of `Trivial.dll` [library](../../).

## String case

You can convert a string to a specific case.

- Uppercase.
- Lowercase.
- Uppercase only for the first letter.
- Lowercase only for the first letter.

```csharp
var a = " how are you?".ToSpecificCase(Cases.FirstLetterUpper);
// a -> " How are you?"
```

## Break lines

You can break lines for a specific length.

```csharp
var lines = StringExtensions.BreakLines("A very long text...", 70);
```

## Yield split

You can yield split a string as `IEnumerable<string>` output.

```csharp
var words = "Some words here to split".YieldSplit(' ');
```