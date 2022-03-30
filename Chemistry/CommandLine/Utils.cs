using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Chemistry;
using Trivial.Collection;

namespace Trivial.CommandLine;

/// <summary>
/// The chemistry command line.
/// </summary>
public static class ChemistryCommandLine
{
    /// <summary>
    /// Writes the specified chemical element, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="element">A chemicial element.</param>
    /// <param name="kind">The kind to represent the information of chemical element.</param>
    public static void WriteLine(ChemicalElement element, ChemicalElementRepresentationKinds kind)
        => WriteLine(StyleConsole.Default, new ChemicalElementConsoleStyle(), element, kind);

    /// <summary>
    /// Writes the specified chemical element, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="console">The console instance.</param>
    /// <param name="element">A chemicial element.</param>
    /// <param name="kind">The kind to represent the information of chemical element.</param>
    public static void WriteLine(this StyleConsole console, ChemicalElement element, ChemicalElementRepresentationKinds kind)
        => WriteLine(console, new ChemicalElementConsoleStyle(), element, kind);

    /// <summary>
    /// Writes the specified chemical element, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="console">The console instance.</param>
    /// <param name="style">The style.</param>
    /// <param name="element">A chemicial element.</param>
    /// <param name="kind">The kind to represent the information of chemical element.</param>
    public static void WriteLine(this StyleConsole console, ChemicalElementConsoleStyle style, ChemicalElement element, ChemicalElementRepresentationKinds kind)
    {
        var col = kind switch
        {
            ChemicalElementRepresentationKinds.Details => ToConsoleText(style, element, false),
            ChemicalElementRepresentationKinds.DetailsAndIsotopes => ToConsoleText(style, element, true),
            _ => ToSimpleConsoleText(style, element, false),
        };
        (console ?? StyleConsole.Default).Write(col);
    }

    /// <summary>
    /// Writes the specified chemistry periodic table, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="console">The console instance.</param>
    /// <param name="style">The style.</param>
    public static void WriteTable(StyleConsole console, ChemicalElementConsoleStyle style)
    {
        var col = new List<ConsoleText>();
        if (style == null) style = new();
        var symbolStyle = new ConsoleTextStyle
        {
            ForegroundConsoleColor = style.SymbolConsoleColor,
            ForegroundRgbColor = style.SymbolRgbColor,
            BackgroundConsoleColor = style.BackgroundConsoleColor,
            BackgroundRgbColor = style.BackgroundRgbColor
        };
        var numberStyle = new ConsoleTextStyle
        {
            ForegroundConsoleColor = style.AtomicNumberConsoleColor,
            ForegroundRgbColor = style.AtomicNumberRgbColor,
            BackgroundConsoleColor = style.BackgroundConsoleColor,
            BackgroundRgbColor = style.BackgroundRgbColor
        };
        var punctuationStyle = new ConsoleTextStyle
        {
            ForegroundConsoleColor = style.PunctuationConsoleColor,
            ForegroundRgbColor = style.PunctuationRgbColor,
            BackgroundConsoleColor = style.BackgroundConsoleColor,
            BackgroundRgbColor = style.BackgroundRgbColor
        };

        col.Add("1 ", punctuationStyle);
        AppendSymbol(col, ChemicalElement.H, symbolStyle);
        col.Add(' ', 64, punctuationStyle);
        AppendSymbol(col, ChemicalElement.He, symbolStyle);
        col.AddNewLine();
        col.Add("  ", punctuationStyle);
        AppendNumber(col, 1, numberStyle);
        col.Add(' ', 64, punctuationStyle);
        AppendNumber(col, 2, numberStyle);
        col.AddNewLine();
        col.AddNewLine();

        col.Add("2 ", punctuationStyle);
        AppendSymbol(col, ChemicalElement.Li, symbolStyle);
        AppendSymbol(col, ChemicalElement.Be, symbolStyle);
        col.Add(' ', 40, punctuationStyle);
        AppendSymbol(col, ChemicalElement.B, symbolStyle);
        AppendSymbol(col, ChemicalElement.C, symbolStyle);
        AppendSymbol(col, ChemicalElement.N, symbolStyle);
        AppendSymbol(col, ChemicalElement.O, symbolStyle);
        AppendSymbol(col, ChemicalElement.F, symbolStyle);
        AppendSymbol(col, ChemicalElement.Ne, symbolStyle);
        col.AddNewLine();
        col.Add("  ", punctuationStyle);
        AppendNumber(col, 3, numberStyle, 2);
        col.Add(' ', 40, punctuationStyle);
        AppendNumber(col, 5, numberStyle, 6);
        col.AddNewLine();
        col.AddNewLine();

        col.Add("3 ", punctuationStyle);
        AppendSymbol(col, ChemicalElement.Na, symbolStyle);
        AppendSymbol(col, ChemicalElement.Mg, symbolStyle);
        col.Add(' ', 40, punctuationStyle);
        AppendSymbol(col, ChemicalElement.Al, symbolStyle);
        AppendSymbol(col, ChemicalElement.Si, symbolStyle);
        AppendSymbol(col, ChemicalElement.P, symbolStyle);
        AppendSymbol(col, ChemicalElement.S, symbolStyle);
        AppendSymbol(col, ChemicalElement.Cl, symbolStyle);
        AppendSymbol(col, ChemicalElement.Ar, symbolStyle);
        col.AddNewLine();
        col.Add("  ", punctuationStyle);
        AppendNumber(col, 11, numberStyle, 2);
        col.Add(' ', 40, punctuationStyle);
        AppendNumber(col, 13, numberStyle, 6);
        col.AddNewLine();
        col.AddNewLine();

        col.Add("4 ", punctuationStyle);
        for (var i = 19; i <= 36; i++)
        {
            AppendSymbol(col, ChemicalElement.Get(i), symbolStyle);
        }

        col.AddNewLine();
        col.Add("  ", punctuationStyle);
        AppendNumber(col, 19, numberStyle, 18);
        col.AddNewLine();
        col.AddNewLine();

        col.Add("5 ", punctuationStyle);
        for (var i = 37; i <= 54; i++)
        {
            AppendSymbol(col, ChemicalElement.Get(i), symbolStyle);
        }

        col.AddNewLine();
        col.Add("  ", punctuationStyle);
        AppendNumber(col, 37, numberStyle, 18);
        col.AddNewLine();
        col.AddNewLine();

        col.Add("6 ", punctuationStyle);
        for (var i = 55; i < 58; i++)
        {
            AppendSymbol(col, ChemicalElement.Get(i), symbolStyle);
        }

        for (var i = 72; i <= 86; i++)
        {
            AppendSymbol(col, ChemicalElement.Get(i), symbolStyle);
        }

        col.AddNewLine();
        col.Add("  ", punctuationStyle);
        AppendNumber(col, 55, numberStyle, 2);
        col.Add("... ", punctuationStyle);
        AppendNumber(col, 72, numberStyle, 15);
        col.AddNewLine();
        col.AddNewLine();

        col.Add("7 ", punctuationStyle);
        for (var i = 87; i < 90; i++)
        {
            AppendSymbol(col, ChemicalElement.Get(i), symbolStyle);
        }

        for (var i = 104; i <= 118; i++)
        {
            AppendSymbol(col, ChemicalElement.Get(i), symbolStyle);
        }

        col.AddNewLine();
        col.Add("  ", punctuationStyle);
        AppendNumber(col, 87, numberStyle, 2);
        col.Add("... ", punctuationStyle);
        AppendNumber(col, 104, numberStyle, 15);
        col.AddNewLine();
        col.AddNewLine();

        col.Add(ChemicalElement.La.Symbol, symbolStyle);
        col.Add('-', 1, punctuationStyle);
        col.Add(ChemicalElement.Lu.Symbol, symbolStyle);
        col.Add("\t  ", punctuationStyle);
        for (var i = 57; i <= 71; i++)
        {
            AppendSymbol(col, ChemicalElement.Get(i), symbolStyle);
        }

        col.AddNewLine();
        col.Add("\t  ", punctuationStyle);
        AppendNumber(col, 57, numberStyle, 15);
        col.AddNewLine();
        col.AddNewLine();

        col.Add(ChemicalElement.Ac.Symbol, symbolStyle);
        col.Add('-', 1, punctuationStyle);
        col.Add(ChemicalElement.Lr.Symbol, symbolStyle);
        col.Add("\t  ");
        for (var i = 89; i <= 103; i++)
        {
            AppendSymbol(col, ChemicalElement.Get(i), symbolStyle);
        }

        col.AddNewLine();
        col.Add("\t  ", punctuationStyle);
        AppendNumber(col, 89, numberStyle, 15);
        col.AddNewLine();
        col.AddNewLine();

        if (ChemicalElement.Has(119) && ChemicalElement.Has(120))
        {
            col.Add("8 ", punctuationStyle);
            col.Add("119 ", numberStyle);
            col.Add('-', 1, punctuationStyle);
            col.Add(" 168 ", numberStyle);
            AppendSymbol(col, ChemicalElement.Get(119), symbolStyle);
            AppendSymbol(col, ChemicalElement.Get(120), symbolStyle);
            for (var i = 121; i < 131; i++)
            {
                if (ChemicalElement.Has(i))
                    AppendSymbol(col, ChemicalElement.Get(i), symbolStyle);
                else
                    break;
            }

            col.Add("...", punctuationStyle);
            col.AddNewLine();
        }
        else
        {
            col.Add("8 ", punctuationStyle);
            col.Add("119 ", numberStyle);
            col.Add('-', 1, punctuationStyle);
            col.Add(" 168 ", numberStyle);
            col.AddNewLine();
        }

        col.Add("9 ", punctuationStyle);
        col.Add("169 ", numberStyle);
        col.Add('-', 1, punctuationStyle);
        col.Add(" 218 ", numberStyle);
        col.AddNewLine();

        (console ?? StyleConsole.Default).Write(col);
    }

    /// <summary>
    /// Writes the element information to the standard output stream.
    /// </summary>
    /// <param name="style">The console style.</param>
    /// <param name="element">A chemicial element.</param>
    /// <param name="containIsotopes">true if also write its isotopes; otherwise, false.</param>
    internal static List<ConsoleText> ToConsoleText(ChemicalElementConsoleStyle style, ChemicalElement element, bool containIsotopes = false)
    {
        if (element is null) return null;
        if (style == null) style = new();
        var name = element.Name;
        var col = new List<ConsoleText>
        {
            {
                $"{element.AtomicNumber:g}\t",
                new ConsoleTextStyle
                {
                    ForegroundConsoleColor = style.AtomicNumberConsoleColor,
                    ForegroundRgbColor = style.AtomicNumberRgbColor,
                    BackgroundConsoleColor = style.BackgroundConsoleColor,
                    BackgroundRgbColor = style.BackgroundRgbColor
                }
            },
            {
                element.Symbol,
                new ConsoleTextStyle
                {
                    ForegroundConsoleColor = style.SymbolConsoleColor,
                    ForegroundRgbColor = style.SymbolRgbColor,
                    BackgroundConsoleColor = style.BackgroundConsoleColor,
                    BackgroundRgbColor = style.BackgroundRgbColor
                }
            }
        };
        var nameStyle = new ConsoleTextStyle
        {
            ForegroundConsoleColor = style.NameConsoleColor,
            ForegroundRgbColor = style.NameRgbColor,
            BackgroundConsoleColor = style.BackgroundConsoleColor,
            BackgroundRgbColor = style.BackgroundRgbColor
        };
        if (!string.IsNullOrWhiteSpace(element.EnglishName) && !name.Equals(element.EnglishName))
            col.Add($"\t{element.EnglishName}", nameStyle);
        col.AddNewLine();
            col.Add($" {name}", nameStyle);
        var punctuationStyle = new ConsoleTextStyle
        {
            ForegroundConsoleColor = style.PunctuationConsoleColor,
            ForegroundRgbColor = style.PunctuationRgbColor,
            BackgroundConsoleColor = style.BackgroundConsoleColor,
            BackgroundRgbColor = style.BackgroundRgbColor
        };
        if (name.Length == 1 && name[0] >= 0x2E00)
            col.Add($"\t(U+{(int)name[0]:X4})", punctuationStyle);
        col.AddNewLine();
        var keyStyle = new ConsoleTextStyle
        {
            ForegroundConsoleColor = style.PropertyKeyConsoleColor,
            ForegroundRgbColor = style.PropertyKeyRgbColor,
            BackgroundConsoleColor = style.BackgroundConsoleColor,
            BackgroundRgbColor = style.BackgroundRgbColor
        };
        var valueStyle = new ConsoleTextStyle
        {
            ForegroundConsoleColor = style.PropertyValueConsoleColor,
            ForegroundRgbColor = style.PropertyValueRgbColor,
            BackgroundConsoleColor = style.BackgroundConsoleColor,
            BackgroundRgbColor = style.BackgroundRgbColor
        };
        if (!double.IsNaN(element.AtomicWeight))
        {
            col.Add(" Weight", keyStyle);
            col.Add(" = ", punctuationStyle);
            col.Add(element.AtomicWeight.ToString("#.########"), valueStyle);
            col.AddNewLine();
        }

        if (element.Period > 0 && element.Group > 0)
        {
            col.Add(" Period", keyStyle);
            col.Add(" = ", punctuationStyle);
            col.Add(element.Period.ToString("g"), valueStyle);
            col.Add("    ", punctuationStyle);
            col.Add("Group", keyStyle);
            col.Add(" = ", punctuationStyle);
            col.Add(element.Group.ToString("g"), valueStyle);
            if (!string.IsNullOrWhiteSpace(element.Block))
            {
                col.Add("    ", punctuationStyle);
                col.Add("Block", keyStyle);
                col.Add(" = ", punctuationStyle);
                col.Add(element.Block, valueStyle);
            }
        }

        if (!containIsotopes) return col;
        var isotopes = element.Isotopes();
        if (isotopes.Count < 1) return col;
        col.AddNewLine();
        var isotopeStyle = new ConsoleTextStyle
        {
            ForegroundConsoleColor = style.IsotopeConsoleColor,
            ForegroundRgbColor = style.IsotopeRgbColor,
            BackgroundConsoleColor = style.BackgroundConsoleColor,
            BackgroundRgbColor = style.BackgroundRgbColor
        };
        col.Add(isotopes.Count == 1 ? ChemistryResource.Isotope : ChemistryResource.Isotopes, isotopeStyle);
        col.Add($" ({isotopes.Count})", punctuationStyle);
        var i = 0;
        if (isotopes.Count > 150)
        {
            foreach (var isotope in isotopes)
            {
                if (i % 8 == 0)
                    col.AddNewLine();
                col.Add($"{isotope.AtomicMassNumber}\t", isotopeStyle);
                i++;
            }

            col.AddNewLine();
            return col;
        }

        col.AddNewLine();
        foreach (var isotope in isotopes)
        {
            if (isotope is null) continue;
            i++;
            var weight = isotope.HasAtomicWeight ? isotope.AtomicWeight.ToString("#.########") : string.Empty;
            col.Add($" {isotope.AtomicMassNumber}\t{weight}", isotopeStyle);
            if (weight.Length < 8) col.Add('\t', 1, isotopeStyle);
            if (i % 3 > 0) col.Add(" \t");
            else col.AddNewLine();
        }

        if (i % 3 > 0) col.AddNewLine();
        return col;
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="style">The console style.</param>
    /// <param name="element">A chemicial element.</param>
    /// <param name="writeEnglishName">true if writes English name.</param>
    internal static List<ConsoleText> ToSimpleConsoleText(ChemicalElementConsoleStyle style, ChemicalElement element, bool writeEnglishName)
    {
        if (string.IsNullOrWhiteSpace(element.Symbol) || element.Period < 1) return null;
        var name = writeEnglishName ? element.EnglishName : element.Name;
        if (style == null) style = new();
        var col = new List<ConsoleText>
        {
            {
                $"{element.AtomicNumber:g}\t",
                new ConsoleTextStyle
                {
                    ForegroundConsoleColor = style.AtomicNumberConsoleColor,
                    ForegroundRgbColor = style.AtomicNumberRgbColor,
                    BackgroundConsoleColor = style.BackgroundConsoleColor,
                    BackgroundRgbColor = style.BackgroundRgbColor
                }
            },
            {
                element.Symbol,
                new ConsoleTextStyle
                {
                    ForegroundConsoleColor = style.SymbolConsoleColor,
                    ForegroundRgbColor = style.SymbolRgbColor,
                    BackgroundConsoleColor = style.BackgroundConsoleColor,
                    BackgroundRgbColor = style.BackgroundRgbColor
                }
            },
            {
                $"\t{name}",
                new ConsoleTextStyle
                {
                    ForegroundConsoleColor = style.NameConsoleColor,
                    ForegroundRgbColor = style.NameRgbColor,
                    BackgroundConsoleColor = style.BackgroundConsoleColor,
                    BackgroundRgbColor = style.BackgroundRgbColor
                }
            }
        };
        if (!double.IsNaN(element.AtomicWeight))
        {
            var punctuationStyle = new ConsoleTextStyle
            {
                ForegroundConsoleColor = style.PunctuationConsoleColor,
                ForegroundRgbColor = style.PunctuationRgbColor,
                BackgroundConsoleColor = style.BackgroundConsoleColor,
                BackgroundRgbColor = style.BackgroundRgbColor
            };
            if (name.Length < 8) col.Add('\t', 2, punctuationStyle);
            else col.Add(" \t", punctuationStyle);
            col.Add(element.AtomicWeight.ToString("#.######"), punctuationStyle);
        }

        col.AddNewLine();
        return col;
    }

    internal static void AddNewLine(this List<ConsoleText> col)
        => col.Add(Environment.NewLine);

    internal static void AppendNumber(List<ConsoleText> col, int i, ConsoleTextStyle style, int count = 1)
    {
        var k = i + count;
        for (var j = i; j < k; j++)
        {
            var s = j.ToString("g");
            col.Add(s, style);
            col.Add(' ', 4 - s.Length, style);
        }
    }

    private static void AppendSymbol(List<ConsoleText> col, ChemicalElement element, ConsoleTextStyle style)
    {
        var s = element.Symbol?.Trim();
        if (string.IsNullOrEmpty(s)) return;
        if (s.Length > 4) s = s.Substring(0, 4);
        col.Add(s, style);
        col.Add(' ', 4 - s.Length, style);
    }
}
