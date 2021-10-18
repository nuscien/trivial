using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Chemistry;
using Trivial.Collection;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The chemistry command line.
    /// </summary>
    public static class ChemistryCommandLine
    {
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="element">A chemicial element.</param>
        /// <param name="kind">The kind to represent the information of chemical element.</param>
        public static void WriteLine(ChemicalElement element, ChemicalElementRepresentationKinds kind)
            => WriteLine(StyleConsole.Default, new ChemicalElementConsoleStyle(), element, kind);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="console">The console instance.</param>
        /// <param name="element">A chemicial element.</param>
        /// <param name="kind">The kind to represent the information of chemical element.</param>
        public static void WriteLine(this StyleConsole console, ChemicalElement element, ChemicalElementRepresentationKinds kind)
            => WriteLine(console, new ChemicalElementConsoleStyle(), element, kind);

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="console">The console instance.</param>
        /// <param name="style">The style.</param>
        /// <param name="element">A chemicial element.</param>
        /// <param name="kind">The kind to represent the information of chemical element.</param>
        public static void WriteLine(this StyleConsole console, ChemicalElementConsoleStyle style, ChemicalElement element, ChemicalElementRepresentationKinds kind)
        {
            if (console == null) console = StyleConsole.Default;
            IEnumerable<ConsoleText> col;
            switch (kind)
            {
                case ChemicalElementRepresentationKinds.Details:
                    col = ToConsoleText(style, element, false);
                    break;
                case ChemicalElementRepresentationKinds.DetailsAndIsotopes:
                    col = ToConsoleText(style, element, true);
                    break;
                default:
                    col = ToSimpleConsoleText(style, element, false);
                    break;
            }

            console.Write(col);
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
    }
}
