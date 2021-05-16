# [Trivial.Chemistry](../docs/chemistry)

This library includes the basic chemistry models.

## Import

Just add following namespace to your code file to use.

```csharp
using Trivial.Chemistry;
```

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

Test conservation of mass.

```csharp
// 2 Na + 2 H₂O = 2 NaOH + H₂
Console.WriteLine(MolecularFormula.ConservationOfMass(
    new List<MolecularFormula>
    {
        { (MolecularFormula)ChemicalElement.Na, 2 },
        { ChemicalElement.H * 2 + ChemicalElement.O, 2 }
    },
    new List<MolecularFormula>
    {
        { ChemicalElement.Na + ChemicalElement.H + ChemicalElement.O, 2 },
        ChemicalElement.H * 2
    }
)); // -> True
```
