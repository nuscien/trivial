# [Trivial.Chemistry](../docs/chemistry)

This library includes the basic chemistry models.

## Import

Just add following namespace to your code file to use.

```csharp
using Trivial.Chemistry;
```

## Element

You can get the element from periodic table by its symbol or atomic numbers.

```csharp
var oxygen = ChemicalElement.Get(8);
var gold = ChemicalElement.Get("Au");
var carbon = ChemicalElement.C;
```

## Isotope

And you can create an isotope from an element.

```csharp
var hydrogen = ChemicalElement.Get(1);
var diplogen = hydrogen.Isotope(2);
```

## Molecular formula

You can parse a molecular formula.

```csharp
var carbonicAcid = MolecularFormula.Parse("H2CO3");
```

Or create a molecular formula by merging elements and other molecular formulas.

```csharp
var sulfuricAcid = ChemicalElement.H * 2 + ChemicalElement.S + ChemicalElement.O * 4;
var ethanol = MolecularFormula.Parse("CH3") + MolecularFormula.Parse("CH2") + MolecularFormula.Parse("OH");
var iron = (MolecularFormula)ChemicalElement.Fe;
```
