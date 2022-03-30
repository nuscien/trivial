using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Chemistry;

/// <summary>
/// The helpers and extensions of chemistry.
/// </summary>
public static class ChemistryExtensions
{
    /// <summary>
    /// Gets or sets the max atomic number of periodic table; or -1 for unlimited.
    /// But this will not prevent creating a customized chemical element nor getting an existed from periodic table.
    /// </summary>
    public static int MaxAtomicNumber { get; set; } = -1;

    /// <summary>
    /// Adds molecular formula into a list.
    /// </summary>
    /// <param name="formulas">The molecular fomula collection.</param>
    /// <param name="value">A molecular formula instance to add.</param>
    /// <param name="count">The count of the value.</param>
    /// <param name="another">Another optional molecular formula instance to add.</param>
    /// <param name="countForAnother">The count of another.</param>
    /// <param name="other">The optional third molecular formula instance to add</param>
    /// <param name="countForLast">The count of the third molecular formula instance to add.</param>
    public static void Add(this ICollection<MolecularFormula> formulas, MolecularFormula value, int count, MolecularFormula another = null, int countForAnother = 1, MolecularFormula other = null, int countForLast = 1)
    {
        if (formulas is null) return;
        if (value != null)
        {
            for (var i = 0; i < count; i++)
            {
                formulas.Add(value);
            }
        }

        if (another != null)
        {
            for (var i = 0; i < countForAnother; i++)
            {
                formulas.Add(another);
            }
        }

        if (other != null)
        {
            for (var i = 0; i < countForLast; i++)
            {
                formulas.Add(other);
            }
        }
    }

    /// <summary>
    /// Adds molecular formula into a list.
    /// </summary>
    /// <param name="formulas">The molecular fomula collection.</param>
    /// <param name="value">A chemical element to add.</param>
    /// <param name="count">The count of the value.</param>
    public static void Add(this ICollection<MolecularFormula> formulas, ChemicalElement value, int count = 1)
    {
        if (formulas is null) return;
        if (value != null && count > 0) formulas.Add(new MolecularFormula(value, count));
    }

    /// <summary>
    /// Gets a value indicating whether an element is radioelement.
    /// </summary>
    /// <param name="element">The element to test</param>
    /// <returns>true if it is a radioelement; otherwise, false.</returns>
    public static bool IsRadioelement(this ChemicalElement element)
    {
        if (element is null) return false;
        return element.AtomicNumber >= 84 || element.AtomicNumber == 61 || element.AtomicNumber == 43;
    }

    /// <summary>
    /// Gets the electron configuration.
    /// </summary>
    /// <param name="element">The element to test</param>
    /// <returns>A string that represents the electron configuation of the specific chemical element; or null, if not supported.</returns>
    public static string GetElectronConfigurationString(ChemicalElement element)
    {
        if (element is null || element.Period < 1 || element.Period > 7) return null;
        if (element.AtomicNumber > 105 && element.AtomicNumber < 113) return null;
        return element.Group switch
        {
            1 => element.Period.ToString("g") + "s¹",
            2 => element.Period.ToString("g") + "s²",
            3 => element.IndexInPeriod switch
            {
                2 => $"{element.Period - 1}d¹{element.Period}s²",
                3 => element.AtomicNumber == 58 ? $"{element.Period - 2}f¹{element.Period - 1}d¹{element.Period}s²" : $"{element.Period - 1}d²{element.Period}s²",
                4 => element.AtomicNumber == 59 ? $"{element.Period - 2}f³{element.Period}s²" : $"{element.Period - 2}f²{element.Period - 1}d¹{element.Period}s²",
                5 => element.AtomicNumber == 60 ? $"{element.Period - 2}f⁴{element.Period}s²" : $"{element.Period - 2}f³{element.Period - 1}d¹{element.Period}s²",
                6 => element.AtomicNumber == 61 ? $"{element.Period - 2}f⁵{element.Period}s²" : $"{element.Period - 2}f⁴{element.Period - 1}d¹{element.Period}s²",
                7 => $"{element.Period - 2}f⁶{element.Period}s²",
                8 => $"{element.Period - 2}f⁷{element.Period}s²",
                9 => $"{element.Period - 2}f⁷{element.Period - 1}d¹{element.Period}s²",
                10 => $"{element.Period - 2}f⁹{element.Period}s²",
                11 => $"{element.Period - 2}f¹⁰{element.Period}s²",
                12 => $"{element.Period - 2}f¹¹{element.Period}s²",
                13 => $"{element.Period - 2}f¹²{element.Period}s²",
                14 => $"{element.Period - 2}f¹³{element.Period}s²",
                15 => $"{element.Period - 2}f¹⁴{element.Period}s²",
                16 => $"{element.Period - 2}f¹⁴{element.Period - 1}d¹{element.Period}s²",
                _ => null
            },
            4 => $"{element.Period - 1}d²{element.Period}s¹",
            5 => element.AtomicNumber == 41 ? $"{element.Period - 1}d⁴{element.Period}s¹" : $"{element.Period - 1}d³{element.Period}s²",
            6 => element.AtomicNumber == 74 ? $"{element.Period - 1}d⁴{element.Period}s²" : $"{element.Period - 1}d⁵{element.Period}s¹",
            7 => $"{element.Period - 1}d⁵{element.Period}s²",
            8 => element.AtomicNumber == 44 ? $"{element.Period - 1}d⁷{element.Period}s¹" : $"{element.Period - 1}d⁶{element.Period}s²",
            9 => element.AtomicNumber == 45 ? $"{element.Period - 1}d⁸{element.Period}s¹" : $"{element.Period - 1}d⁷{element.Period}s²",
            10 => element.AtomicNumber == 28 ? $"{element.Period - 1}d⁸{element.Period}s²" : (element.AtomicNumber == 46 ? $"{element.Period - 1}d¹⁰{element.Period}s¹" : $"{element.Period - 1}d⁹{element.Period}s¹"),
            11 => $"{element.Period - 1}d¹⁰{element.Period}s¹",
            12 => $"{element.Period - 1}d¹⁰{element.Period}s²",
            13 => $"{element.Period}s²{element.Period}p¹",
            14 => $"{element.Period}s²{element.Period}p²",
            15 => $"{element.Period}s²{element.Period}p³",
            16 => $"{element.Period}s²{element.Period}p⁴",
            17 => $"{element.Period}s²{element.Period}p⁵",
            18 => $"{element.Period}s²{element.Period}p⁶",
            _ => null
        };
    }
}
