# [Trivial.Chemistry](../docs/chemistry)

This library includes the basic chemistry models.

## Import

Just add following namespace to your code file to use.

```csharp
using Trivial.Chemistry;
```

## Element

You can get the element from periodic table by its symbol or atomic number.

```csharp
var oxygen = ChemicalElement.Get(8);
var gold = ChemicalElement.Get("Au");
```

And you can create an isotope from an element.

```csharp
var hydrogen = ChemicalElement.Get(1);
var diplogen = hydrogen.Isotope(2);
```

## Molecular formula

You can parse a molecular formula.

```csharp
var m = MolecularFormula.Parse("H2CO3");
```
