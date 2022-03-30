using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Chemistry;
using Trivial.Collection;

namespace Trivial.CommandLine;

/// <summary>
/// Chemistry command line verb.
/// </summary>
public class ChemistryVerb : BaseCommandVerb
{
#pragma warning disable IDE0057
    /// <summary>
    /// Gets the description.
    /// </summary>
    public static string Description => ChemistryResource.Chemistry;

    /// <summary>
    /// Gets or sets the help document.
    /// </summary>
    public static string HelpInfo { get; set; }


    /// <summary>
    /// Gets or sets the tips header for usages.
    /// </summary>
    public static string UsagesTips { get; set; }

    /// <summary>
    /// Gets or sets the tips header for examples.
    /// </summary>
    public static string ExamplesTips { get; set; }

    /// <summary>
    /// Gets or sets the tips for error.
    /// </summary>
    public static string ErrorTips { get; set; }

    /// <summary>
    /// Gets or sets the tips for empty.
    /// </summary>
    public static string EmptyTips { get; set; }

    /// <summary>
    /// Gets or sets the tips for not found.
    /// </summary>
    public static string NotFoundTips { get; set; }

    /// <summary>
    /// Gets or sets the tips for getting chemical element.
    /// </summary>
    public static string ElementTips { get; set; }

    /// <summary>
    /// Gets or sets the tips for showing periodic table.
    /// </summary>
    public static string PeriodicTableTips { get; set; }

    /// <summary>
    /// Gets or sets the tips for chemical elements in a specific period.
    /// </summary>
    public static string PeriodTips { get; set; }

    /// <summary>
    /// Gets or sets the tips for all elements.
    /// </summary>
    public static string AllElementsTips { get; set; }

    /// <summary>
    /// Gets or sets the tips for elements.
    /// </summary>
    public static string ElementListTips { get; set; }

    /// <summary>
    /// Gets or sets the tips for molecular formula.
    /// </summary>
    public static string MolecularFormulaTips { get; set; }

    /// <summary>
    /// Gets or sets the color of verb in help document.
    /// </summary>
    public static System.Drawing.Color VerbColorInHelp { get; set; } = System.Drawing.Color.FromArgb(0xF0, 0xE8, 0xA0);

    /// <summary>
    /// Gets or sets the console style of chemical element.
    /// </summary>
    public ChemicalElementConsoleStyle ChemicalElementStyle { get; set; }

    /// <inheritdoc />
    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        await RunAsync(null, cancellationToken);
        var s = Arguments.Verb?.TryGet(0);
        var console = GetConsole();
        if (s == "period" || s == "p" || s == "周期" || (s == "Period" && Arguments.Verb.Count > 1))
        {
            s = Arguments.Verb.TryGet(1);
            if (string.IsNullOrEmpty(s))
            {
                ChemistryCommandLine.WriteTable(GetConsole(), ChemicalElementStyle);
            }
            else if (int.TryParse(s, out var period) && period > 0)
            {
                if (period > 7)
                {
                    var start = ChemicalElement.FirstAtomicNumberInPeriod(period);
                    var end = ChemicalElement.LastAtomicNumberInPeriod(period);
                    if (start <= 0 || end <= 0 || start >= end)
                    {
                        WriteError(" The period was too large.");
                    }
                    else
                    {
                        console.WriteLine(ChemistryResource.ChemicalElement);
                        console.WriteLine($"{start} - {end} (Total {end - start + 1})");
                    }

                    return;
                }

                var elements = ChemicalElement.GetExisted();
                var has = false;
                foreach (var element in elements)
                {
                    if (element is null) continue;
                    if (element.Period != period)
                    {
                        if (has) break;
                        continue;
                    }

                    WriteSimpleInfo(element, Arguments.Has("en"));
                }
            }
            else if (s.Equals("help", StringComparison.OrdinalIgnoreCase) || s.Equals("?", StringComparison.Ordinal))
            {
                WriteHelp(Arguments.Verb?.Key, "period");
            }
            else
            {
                WriteError(" The period should be a natural number.");
            }

            return;
        }

        var q = Arguments.GetMergedValue("q")?.Trim();
        if (q.Length < 1) q = null;

        if (s == "*" || s == "all" || s == "a")
        {
            foreach (var i in ChemicalElement.Where(GetFilter(q)))
            {
                WriteSimpleInfo(i, Arguments.Has("en"));
            }

            return;
        }

        if (s == "ls" || s == "l" || s == "list" || s == "dir" || s == "全部")
        {
            var filter = Arguments.Verb.TryGet(1)?.Trim()?.Split('-');
            List<ChemicalElement> col = null;
            if (filter is null || filter.Length < 1 || string.IsNullOrWhiteSpace(filter[0]))
            {
            }
            else if (filter[0].Equals("help", StringComparison.OrdinalIgnoreCase) || filter[0].Equals("?", StringComparison.Ordinal))
            {
                WriteHelp(Arguments.Verb?.Key, "ls");
                return;
            }
            else if (!int.TryParse(filter[0], out var begin))
            {
            }
            else if (filter.Length == 1)
            {
                col = ChemicalElement.Range(0, begin).ToList();
            }
            else if (filter.Length == 2 && int.TryParse(filter[1], out var end))
            {
                col = ChemicalElement.Range(begin, end - begin).ToList();
            }

            if (col is null)
                col = ChemicalElement.Where(null).ToList();
            var filter2 = GetFilter(q);
            foreach (var i in filter2 is null ? col : col.Where(filter2))
            {
                WriteSimpleInfo(i, Arguments.Has("en"));
            }

            return;
        }

        if (s == "molecular" || s == "formula" || s == "分子")
        {
            s = Arguments.Verb.Value?.Trim() ?? string.Empty;
            if (s.StartsWith("molecular ")) s = s.Substring(10)?.Trim();
            if (s.StartsWith("formula ")) s = s.Substring(8)?.Trim();
            if (s.Length < 1 || s == "molecular" || s == "formula")
            {
                console.WriteLine(EmptyTips ?? "Empty");
                return;
            }

            var m = MolecularFormula.TryParse(s);
            if (m is null)
            {
                if (s.Equals("help", StringComparison.OrdinalIgnoreCase) || s.Equals("?", StringComparison.Ordinal))
                    WriteHelp(Arguments.Verb?.Key, "molecular");
                else
                    WriteError(" Invalid molecular formula.");
                return;
            }

            console.WriteLine(m.ToString());
            var col = new List<ConsoleText>();
            var style = ChemicalElementStyle ?? new();
            var numberStyle = new ConsoleTextStyle
            {
                ForegroundConsoleColor = style.AtomicNumberConsoleColor,
                ForegroundRgbColor = style.AtomicNumberRgbColor,
                BackgroundConsoleColor = style.BackgroundConsoleColor,
                BackgroundRgbColor = style.BackgroundRgbColor
            };
            var nameStyle = new ConsoleTextStyle
            {
                ForegroundConsoleColor = style.NameConsoleColor,
                ForegroundRgbColor = style.NameRgbColor,
                BackgroundConsoleColor = style.BackgroundConsoleColor,
                BackgroundRgbColor = style.BackgroundRgbColor
            };
            var symbolStyle = new ConsoleTextStyle
            {
                ForegroundConsoleColor = style.SymbolConsoleColor,
                ForegroundRgbColor = style.SymbolRgbColor,
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
            foreach (var ele in m.Zip())
            {
                if (ele.Element is null) continue;
                var name = Arguments.Has("en") ? ele.Element.EnglishName : ele.Element.Name;
                col.Add($"{ele.Element.AtomicNumber:g}\t", numberStyle);
                col.Add(ele.Element.Symbol, symbolStyle);
                col.Add($"\t{name}", nameStyle);
                if (name.Length < 8) col.Add('\t', 1, punctuationStyle);
                col.Add(" \tx ", punctuationStyle);
                col.Add(ele.Count.ToString("g"), valueStyle);
                col.AddNewLine();
            }

            col.Add("Proton numbers \t", keyStyle);
            col.Add(m.ProtonNumber.ToString("g"), valueStyle);
            col.AddNewLine();
            col.Add("Atom count \t", keyStyle);
            col.Add(m.Count.ToString("g"), valueStyle);
            col.AddNewLine();
            if (m.IsIon)
            {
                col.Add("Charge number \t", keyStyle);
                col.Add(m.ChargeNumber.ToString("g"), valueStyle);
                col.AddNewLine();
            }

            console.Write(col);
            return;
        }

        if (s == "periodic" || s == "table" || s == "t" || s == "元素周期表" || s == "周期表")
        {
            ChemistryCommandLine.WriteTable(GetConsole(), ChemicalElementStyle);
            return;
        }

        if (s == "help" || s == "get-help" || s == "usages" || s == "?" || s == "帮助")
        {
            WriteHelp(Arguments.Verb.Key, Arguments.Verb.TryGet(1));
            return;
        }

        if (s == "name")
        {
            console.WriteLine(ChemistryResource.Chemistry);
            return;
        }

        if (s == "element" || s == "get-element" || s == "z" || s == "元素" || (s == "Z" && Arguments.Verb.Count > 1))
        {
            s = Arguments.Verb.TryGet(1);
            if (s.Equals("help", StringComparison.Ordinal) || s.Equals("?", StringComparison.Ordinal))
            {
                WriteHelp(Arguments.Verb?.Key, "element");
                return;
            }

            if (s.Equals("name", StringComparison.Ordinal))
            {
                console.WriteLine(ChemistryResource.ChemicalElement);
                return;
            }
        }
        else
        {
            s = null;
        }

        if (string.IsNullOrEmpty(s))
            s = Arguments.Get("element", "number", "z", "e", "symbol", "s")?.MergedValue?.Trim();
        if (string.IsNullOrEmpty(s))
            s = Arguments.Verb?.TryGet(0);
        if (string.IsNullOrEmpty(s))
        {
            console.WriteLine(ChemistryResource.ChemicalElement);
            var elements = ChemicalElement.Where(GetFilter(q)).ToList();
            if (elements.Count <= 64)
            {
                foreach (var i in elements)
                {
                    WriteSimpleInfo(i, Arguments.Has("en"));
                }
            }
            else if (Arguments.Has("en"))
            {
                var i = 0;
                var style = ChemicalElementStyle ?? new();
                var numberStyle = new ConsoleTextStyle
                {
                    ForegroundConsoleColor = style.AtomicNumberConsoleColor,
                    ForegroundRgbColor = style.AtomicNumberRgbColor,
                    BackgroundConsoleColor = style.BackgroundConsoleColor,
                    BackgroundRgbColor = style.BackgroundRgbColor
                };
                var nameStyle = new ConsoleTextStyle
                {
                    ForegroundConsoleColor = style.NameConsoleColor,
                    ForegroundRgbColor = style.NameRgbColor,
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
                var col = new List<ConsoleText>();
                foreach (var element in elements)
                {
                    if (element is null || element.AtomicNumber < 0) continue;
                    i++;
                    if (element.AtomicNumber < 1000)
                    {
                        ChemistryCommandLine.AppendNumber(col, element.AtomicNumber, numberStyle);
                    }
                    else
                    {
                        col.Add(element.AtomicNumber.ToString("g"), numberStyle);
                        col.Add(' ', 1, numberStyle);
                    }

                    col.Add(element.EnglishName ?? element.Symbol ?? string.Empty, nameStyle);
                    if (i % 3 == 0)
                    {
                        col.AddNewLine();
                        continue;
                    }

                    col.Add(' ', Math.Max(1, 24 - col[col.Count - 1].Length - col[col.Count - 2].Length - col[col.Count - 3].Length), punctuationStyle);
                }

                if (i % 3 > 0) col.AddNewLine();
                GetConsole().Write(col);
            }
            else
            {
                var start = false;
                var style = ChemicalElementStyle ?? new();
                var numberStyle = new ConsoleTextStyle
                {
                    ForegroundConsoleColor = style.AtomicNumberConsoleColor,
                    ForegroundRgbColor = style.AtomicNumberRgbColor,
                    BackgroundConsoleColor = style.BackgroundConsoleColor,
                    BackgroundRgbColor = style.BackgroundRgbColor
                };
                var nameStyle = new ConsoleTextStyle
                {
                    ForegroundConsoleColor = style.NameConsoleColor,
                    ForegroundRgbColor = style.NameRgbColor,
                    BackgroundConsoleColor = style.BackgroundConsoleColor,
                    BackgroundRgbColor = style.BackgroundRgbColor
                };
                var symbolStyle = new ConsoleTextStyle
                {
                    ForegroundConsoleColor = style.SymbolConsoleColor,
                    ForegroundRgbColor = style.SymbolRgbColor,
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
                var col = new List<ConsoleText>();
                foreach (var element in elements)
                {
                    if (element is null) continue;
                    if (start) col.Add(" \t", punctuationStyle);
                    else start = true;
                    col.Add(element.Name, nameStyle);
                    col.Add(" (", punctuationStyle);
                    if (element.AtomicNumber < 1)
                        col.Add('?', 1, numberStyle);
                    else
                        col.Add(element.AtomicNumber.ToString("g"), numberStyle);

                    if (!element.Name.Equals(element.Symbol, StringComparison.OrdinalIgnoreCase)
                        && !"?".Equals(element.Symbol, StringComparison.OrdinalIgnoreCase))
                    {
                        col.Add(' ', 1, punctuationStyle);
                        col.Add(element.Symbol, symbolStyle);
                    }

                    col.Add(')', 1, punctuationStyle);
                }

                col.AddNewLine();
                GetConsole().Write(col);
            }
            return;
        }

        var eles = s.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Distinct().Select(i => ChemicalElement.Get(i)).Where(i => i != null).Distinct().ToList();
        if (eles.Count < 1)
        {
            console.WriteLine(ConsoleColor.Red, NotFoundTips ?? "Not found.");
            return;
        }

        if (eles.Count == 1)
        {
            WriteInfo(eles[0], true);
            return;
        }

        if (eles.Count < 4)
        {
            WriteInfo(eles[0]);
            GetConsole().WriteLine();
            WriteInfo(eles[1]);
            if (eles.Count > 2)
            {
                GetConsole().WriteLine();
                WriteInfo(eles[2]);
            }

            return;
        }

        var filter3 = GetFilter(q);
        foreach (var i in filter3 is null ? eles : eles.Where(filter3))
        {
            WriteSimpleInfo(i, Arguments.Has("en"));
        }
    }

    /// <inheritdoc />
    protected override void OnGetHelp()
    {
        WriteHelp(Arguments.Verb?.Key, Arguments.Verb?.TryGet(0));
    }

    private void WriteInfo(ChemicalElement element, bool containIsotopes = false)
    {
        var col = ChemistryCommandLine.ToConsoleText(ChemicalElementStyle, element, containIsotopes);
        GetConsole().Write(col);
    }

    private void WriteSimpleInfo(ChemicalElement element, bool writeEnglishName)
    {
        var col = ChemistryCommandLine.ToSimpleConsoleText(ChemicalElementStyle, element, writeEnglishName);
        GetConsole().Write(col);
    }

    private void WriteError(string message)
        => GetConsole().WriteLine(new List<ConsoleText>
        {
            {
                ErrorTips ?? "Error!",
                new ConsoleTextStyle
                {
                    ForegroundConsoleColor = ConsoleColor.Red
                }
            },
            message
        });

    private void WriteHelp(string verb, string key = null)
    {
        var console = GetConsole();
        verb = verb?.Trim();
        if (string.IsNullOrEmpty(verb)) verb = "~";
#if NETFRAMEWORK
        var lsKey = "dir";
#else
        var lsKey = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) ? "dir" : "ls";
#endif
        switch (key?.Trim() ?? string.Empty)
        {
            case "":
                break;
            case "get-element":
            case "element":
            case "z":
            case "Z":
            case "元素":
                WriteTips(verb, "element <symbol>", ElementTips, "Get details of the specific chemical element by symbol or atomic numbers.");
                console.WriteLine();
                console.WriteLine(ExamplesTips ?? "EXAMPLES");
                console.WriteLine();
                console.WriteLine(verb + " element 6");
                console.WriteLine("{0} element {1}", verb, ChemicalElement.Au?.Symbol ?? ChemicalElement.Pt?.Symbol ?? ChemicalElement.O?.Symbol ?? "1");
                return;
            case "periodic":
            case "table":
            case "周期表":
            case "元素周期表":
                WriteTips(verb, "table", PeriodicTableTips, "Output the periodic table.");
                return;
            case "period":
            case "周期":
            case "Period":
                WriteTips(verb, "period <period>", PeriodTips, "List chemical elements in the specific period.");
                console.WriteLine();
                console.WriteLine(ExamplesTips ?? "EXAMPLES");
                console.WriteLine();
                console.WriteLine(verb + " period 2");
                console.WriteLine(verb + " period 7");
                return;
            case "*":
            case "all":
                WriteTips(verb, "all", AllElementsTips, "List all chemical elements.");
                return;
            case "ls":
            case "list":
            case "dir":
            case "全部":
                WriteTips(verb, lsKey + " <start>-<end>", ElementListTips, "List a speicific range of chemical elements.");
                console.WriteLine();
                console.WriteLine(ExamplesTips ?? "EXAMPLES");
                console.WriteLine();
                console.WriteLine("{0} {1} 10-29", verb, lsKey);
                console.WriteLine("{0} {1} 20", verb, lsKey);
                console.WriteLine("{0} {1} 80 -q h", verb, lsKey);
                return;
            case "molecular":
            case "formula":
            case "分子":
                WriteTips(verb, "molecular <formula>", MolecularFormulaTips, "Get the information of the specific molecular formula.");
                console.WriteLine();
                console.WriteLine(ExamplesTips ?? "EXAMPLES");
                console.WriteLine();
                console.WriteLine(verb + " molecular Fe3O4");
                console.WriteLine(verb + " molecular R-COOH");
                return;
            case "about":
            case "copyright":
                if (!ChemistryResource.Chemistry.Equals("Chemistry", StringComparison.Ordinal))
                    console.WriteLine(ChemistryResource.Chemistry);
                var ver = System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version;
                console.WriteLine(System.Drawing.Color.FromArgb(0xF0, 0xAA, 0xBB), "Trivial.Chemistry");
                var sb = new StringBuilder();
                if (ver != null)
                {
                    sb.AppendFormat("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
                    sb.AppendLine();
                }

                sb.AppendLine("Copyright (c) 2021 Kingcean Tuan. All rights reserved.");
                sb.AppendLine("MIT licensed.");
                console.Write(sb.ToString());
                return;
            default:
                break;
        }

        var s = HelpInfo?.Trim();
        if (!string.IsNullOrEmpty(s))
        {
            console.WriteLine(s);
            console.WriteLine();
        }

        console.WriteLine(UsagesTips ?? "USAGES");
        console.WriteLine();
        WriteTips(verb, "element <symbol>", ElementTips, "Get details of the specific chemical element by symbol or atomic numbers.");
        console.WriteLine();
        WriteTips(verb, "table", PeriodicTableTips, "Output the periodic table.");
        console.WriteLine();
        WriteTips(verb, "period <period>", PeriodTips, "List chemical elements in the specific period.");
        console.WriteLine();
        WriteTips(verb, "all", AllElementsTips, "List all chemical elements.");
        console.WriteLine();
        WriteTips(verb, lsKey + " <start>-<end>", ElementListTips, "List a speicific range of chemical elements.");
        console.WriteLine();
        WriteTips(verb, "molecular <formula>", MolecularFormulaTips, "Get the information of the specific molecular formula.");
    }

    private void WriteTips(string verb, string cmd, string desc, string descBackup)
    {
        GetConsole().WriteLine(VerbColorInHelp, $"{verb} {cmd}");
        GetConsole().WriteLine($"  {desc ?? descBackup}");
    }

    private static Func<ChemicalElement, bool> GetFilter(string q)
    {
        if (string.IsNullOrEmpty(q)) return null;
        return (ChemicalElement ele) =>
        {
#if NETOLDVER
            if (string.IsNullOrEmpty(ele?.EnglishName)) return false;
            if (ele.EnglishName.Contains(q)) return true;
            var name = ele.Name;
            if (string.IsNullOrEmpty(name) || name == ele.EnglishName) return false;
            return name.Contains(q);
#else
            if (string.IsNullOrEmpty(ele?.EnglishName)) return false;
            if (ele.EnglishName.Contains(q, StringComparison.OrdinalIgnoreCase)) return true;
            var name = ele.Name;
            if (string.IsNullOrEmpty(name) || name == ele.EnglishName) return false;
            return name.Contains(q, StringComparison.OrdinalIgnoreCase);
#endif
        };
    }
#pragma warning restore IDE0057
}
