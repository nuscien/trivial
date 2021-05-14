# Molecular formula

Molecular formula.

In `Trivial.Chemistry` [namespace](../) of `Trivial.Chemistry.dll` [library](../../).

## Molecular formula

Parse a molecular formula.

```csharp
var carbonicAcid = MolecularFormula.Parse("H2CO3");
```

Or create a molecular formula by merging elements and other molecular formulas.

```csharp
var sulfuricAcid = ChemicalElement.H * 2 + ChemicalElement.S + ChemicalElement.O * 4;
var ethanol = MolecularFormula.Parse("CH3") + MolecularFormula.Parse("CH2") + MolecularFormula.Parse("OH");
var iron = (MolecularFormula)ChemicalElement.Fe;
```
