# Chemical element

Chemical element, periodic table and isotope.

In `Trivial.Chemistry` [namespace](../) of `Trivial.Chemistry.dll` [library](../../).

## Elements

Get an element from periodic table by its symbol or atomic numbers.

```csharp
var oxygen = ChemicalElement.Get(8);
var gold = ChemicalElement.Get("Au");
var carbon = ChemicalElement.C;
```

## Isotope

Create an isotope from an element.

```csharp
var hydrogen = ChemicalElement.Get(1);
var diplogen = hydrogen.Isotope(2);
```
