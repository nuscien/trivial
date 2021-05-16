# Chemical element

Chemical element, periodic table and isotope.

In `Trivial.Chemistry` [namespace](../) of `Trivial.Chemistry.dll` [library](../../).

## Element

Get element from periodic table by its symbol or atomic numbers.

```csharp
var oxygen = ChemicalElement.Get(8);
var gold = ChemicalElement.Get("Au");
var carbon = ChemicalElement.C;
```

Or list a set of element.

```csharp
var some = ChemicalElement.Where(ele => ele.AtomicNumber < 20);
```

## Isotope

Create an isotope from an element.

```csharp
var diplogen = ChemicalElement.H.Isotope(2);
```

Or list all for a specific element.

```csharp
var allCarbonIsotopes = ChemicalElement.C.Isotopes();
```
