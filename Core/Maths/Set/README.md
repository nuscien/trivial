# Set

This library contains a lot of models for sets.

In `Trivial.Maths` [namespace](../).

## Interval

- `NullableValueSimpleInterval<T>`.
- `StructValueSimpleInterval<T>`.

```csharp
var set = new StructValueSimpleInterval<int>(20, 100, false, true); // => [20, 100)
```
