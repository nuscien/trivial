# Set

In `Trivial.Maths` [namespace](../).

<!-- Start -->

This library contains a lot of models for sets.

## Interval

- `NullableValueSimpleInterval<T>`.
- `StructValueSimpleInterval<T>`.

```csharp
var set = new StructValueSimpleInterval<int>(20, 100, false, true); // => [20, 100)
```
