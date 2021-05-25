using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Chemistry
{
    /// <summary>
    /// Chemistry command line verb.
    /// </summary>
    public class ChemistryVerb : CommandLine.BaseCommandVerb
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        public static string Description => ChemistryResource.Chemistry;

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

        /// <inheritdoc />
        protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
        {
            await RunAsync(null, cancellationToken);
            var s = Arguments.Verb?.TryGet(0);
            if (s == "period" || s == "周期" || (s == "Period" && Arguments.Verb.Count > 1))
            {
                s = Arguments.Verb.TryGet(1);
                if (string.IsNullOrEmpty(s))
                {
                    WritePeriodicTable();
                }
                else if (int.TryParse(s, out var period) && period > 0)
                {
                    if (period > 7)
                    {
                        var start = ChemicalElement.FirstAtomicNumberInPeriod(period);
                        var end = ChemicalElement.LastAtomicNumberInPeriod(period);
                        if (start <= 0 || end <= 0 || start >= end)
                        {
                            Console.WriteLine((ErrorTips ?? "Error!") + "The period was too large.");
                        }
                        else
                        {
                            Console.WriteLine(ChemistryResource.ChemicalElement);
                            Console.WriteLine($"{start} - {end} (Total {end - start + 1})");
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

                        WriteLine(element, Arguments.Has("en"));
                    }
                }
                else if (s.Equals("help", StringComparison.OrdinalIgnoreCase) || s.Equals("?", StringComparison.Ordinal))
                {
                    WriteHelp(Arguments.Verb?.Key, "period");
                }
                else
                {
                    Console.WriteLine((ErrorTips ?? "Error!") + " The period should be a natural number.");
                }

                return;
            }

            var q = Arguments.GetMergedValue("q")?.Trim();
            if (q.Length < 1) q = null;

            if (s == "*" || s == "all")
            {
                foreach (var i in ChemicalElement.Where(GetFilter(q)))
                {
                    WriteLine(i, Arguments.Has("en"));
                }

                return;
            }

            if (s == "ls" || s == "list" || s == "dir" || s == "全部")
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
                    WriteLine(i, Arguments.Has("en"));
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
                    Console.WriteLine(EmptyTips ?? "Empty");
                    return;
                }

                var m = MolecularFormula.TryParse(s);
                if (m is null)
                {
                    if (s.Equals("help", StringComparison.OrdinalIgnoreCase) || s.Equals("?", StringComparison.Ordinal))
                        WriteHelp(Arguments.Verb?.Key, "molecular");
                    else
                        Console.WriteLine((ErrorTips ?? "Error!") + " Invalid molecular formula.");
                    return;
                }

                Console.WriteLine(m.ToString());
                var sb = new StringBuilder();
                foreach (var ele in m.Zip())
                {
                    if (ele.Element is null) continue;
                    sb.Clear();
                    var name = Arguments.Has("en") ? ele.Element.EnglishName : ele.Element.Name;
                    sb.AppendFormat("{0}\t{1}\t{2}", ele.Element.AtomicNumber, ele.Element.Symbol, name);
                    if (name.Length < 8) sb.Append('\t');
                    sb.Append(" \tx ");
                    sb.Append(ele.Count);
                    Console.WriteLine(sb.ToString());
                }

                Console.WriteLine("Proton numbers \t" + m.ProtonNumber);
                Console.WriteLine("Atom count \t" + m.Count);
                if (m.IsIon) Console.WriteLine("Charge number \t" + m.ChargeNumber);
                return;
            }

            if (s == "periodic" || s == "table" || s == "元素周期表" || s == "周期表")
            {
                WritePeriodicTable();
                return;
            }

            if (s == "help" || s == "usages" || s == "?" || s == "帮助")
            {
                WriteHelp(Arguments.Verb.Key, Arguments.Verb.TryGet(1));
                return;
            }

            if (s == "name")
            {
                Console.WriteLine(ChemistryResource.Chemistry);
                return;
            }

            if (s == "element" || s == "z" || s == "元素" || (s == "Z" && Arguments.Verb.Count > 1))
            {
                s = Arguments.Verb.TryGet(1);
                if (s.Equals("help", StringComparison.Ordinal) || s.Equals("?", StringComparison.Ordinal))
                {
                    WriteHelp(Arguments.Verb?.Key, "element");
                    return;
                }

                if (s.Equals("name", StringComparison.Ordinal))
                {
                    Console.WriteLine(ChemistryResource.ChemicalElement);
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
                Console.WriteLine(ChemistryResource.ChemicalElement);
                var col = ChemicalElement.Where(GetFilter(q)).ToList();
                if (col.Count <= 64)
                {
                    foreach (var i in col)
                    {
                        WriteLine(i, Arguments.Has("en"));
                    }
                }
                else if (Arguments.Has("en"))
                {
                    var i = 0;
                    var sb = new StringBuilder();
                    foreach (var element in col)
                    {
                        if (element is null || element.AtomicNumber < 0) continue;
                        i++;
                        if (element.AtomicNumber < 1000) AppendNumber(sb, element.AtomicNumber);
                        else sb.Append(element.AtomicNumber.ToString("g") + " ");
                        sb.Append(element.EnglishName ?? element.Symbol ?? string.Empty);
                        if (i % 3 == 0)
                        {
                            Console.WriteLine(sb.ToString());
                            sb.Clear();
                            continue;
                        }

                        sb.Append(' ', 24 - sb.Length);
                        Console.Write(sb.ToString());
                        sb.Clear();
                    }

                    if (i % 3 > 0) Console.WriteLine();
                }
                else
                {
                    var start = false;
                    foreach (var element in col)
                    {
                        if (element is null) continue;
                        if (start) Console.Write(" \t");
                        else start = true;
                        Console.Write(element.ToString());
                    }

                    Console.WriteLine();
                }
                return;
            }

            var eles = s.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Distinct().Select(i => ChemicalElement.Get(i)).Where(i => i != null).Distinct().ToList();
            if (eles.Count < 1)
            {
                Console.WriteLine(NotFoundTips ?? "Not found.");
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
                Console.WriteLine();
                WriteInfo(eles[1]);
                if (eles.Count > 2)
                {
                    Console.WriteLine();
                    WriteInfo(eles[2]);
                }

                return;
            }

            var filter3 = GetFilter(q);
            foreach (var i in filter3 is null ? eles : eles.Where(filter3))
            {
                WriteLine(i, Arguments.Has("en"));
            }
        }

        /// <inheritdoc />
        protected override void OnGetHelp()
        {
            WriteHelp(Arguments.Verb?.Key, Arguments.Verb?.TryGet(0));
        }

        /// <summary>
        /// Writes the element information to the standard output stream.
        /// </summary>
        /// <param name="element">A chemicial element.</param>
        /// <param name="containIsotopes">true if also write its isotopes; otherwise, false.</param>
        public static void WriteInfo(ChemicalElement element, bool containIsotopes = false)
        {
            if (element is null) return;
            var name = element.Name;
            if (!string.IsNullOrWhiteSpace(element.EnglishName) && !name.Equals(element.EnglishName))
                Console.WriteLine("{0}\t{1}\t{2}", element.AtomicNumber, element.Symbol, element.EnglishName);
            else
                Console.WriteLine("{0}\t{1}", element.AtomicNumber, element.Symbol);
            if (name.Length == 1 && name[0] >= 0x2E00)
                Console.WriteLine(" {0}\t(U+{1:X4})", name, (int)name[0]);
            else
                Console.WriteLine(" {0}", name);
            if (!double.IsNaN(element.AtomicWeight))
                Console.WriteLine(" Weight = {0:#.########}", element.AtomicWeight);
            var sb = new StringBuilder();
            if (element.Period > 0 && element.Group > 0)
            {
                sb.AppendFormat(" Period = {0}    Group = {1}", element.Period, element.Group);
                if (!string.IsNullOrWhiteSpace(element.Block))
                    sb.AppendFormat("    Block = {0}", element.Block);
                Console.WriteLine(sb.ToString());
            }

            if (!containIsotopes) return;
            var isotopes = element.Isotopes();
            if (isotopes.Count < 1) return;
            Console.WriteLine();
            sb.Clear();
            sb.AppendFormat("{0} ({1})", isotopes.Count == 1 ? ChemistryResource.Isotope : ChemistryResource.Isotopes, isotopes.Count);
            var i = 0;
            if (isotopes.Count > 150)
            {
                foreach (var isotope in isotopes)
                {
                    if (i % 8 == 0)
                        sb.AppendLine();
                    sb.Append(isotope.AtomicMassNumber);
                    sb.Append('\t');
                    i++;
                }

                Console.WriteLine(sb.ToString());
                return;
            }

            sb.AppendLine();
            foreach (var isotope in isotopes)
            {
                if (isotope is null) continue;
                i++;
                var weight = isotope.HasAtomicWeight ? isotope.AtomicWeight.ToString("#.########") : string.Empty;
                sb.AppendFormat(" {0} \t{1}", isotope.AtomicMassNumber, weight);
                if (weight.Length < 8) sb.Append('\t');
                if (i % 3 > 0) sb.Append(" \t");
                else sb.AppendLine();
            }

            if (i % 3 > 0) Console.WriteLine(sb.ToString());
            else Console.Write(sb.ToString());
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="element">A chemicial element.</param>
        public static void WriteLine(ChemicalElement element)
        {
            WriteLine(element, false);
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

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="element">A chemicial element.</param>
        /// <param name="writeEnglishName">true if writes English name.</param>
        private static void WriteLine(ChemicalElement element, bool writeEnglishName)
        {
            if (string.IsNullOrWhiteSpace(element.Symbol) || element.Period < 1) return;
            var sb = new StringBuilder();
            var name = writeEnglishName ? element.EnglishName : element.Name;
            sb.AppendFormat("{0}\t{1}\t{2}", element.AtomicNumber, element.Symbol, name);
            if (!double.IsNaN(element.AtomicWeight))
            {
                if (name.Length < 8) sb.Append('\t');
                sb.Append(" \t");
                sb.Append(element.AtomicWeight.ToString("#.######"));
            }

            Console.WriteLine(sb.ToString());
        }

        private static void WriteHelp(string verb, string key = null)
        {
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
                case "element":
                case "z":
                case "Z":
                case "元素":
                    WriteTips(verb, "element <symbol>", ElementTips, "Get details of the specific chemical element by symbol or atomic numbers.");
                    Console.WriteLine();
                    Console.WriteLine(ExamplesTips ?? "Examples");
                    Console.WriteLine();
                    Console.WriteLine(verb + " element 6");
                    Console.WriteLine("{0} element {1}", verb, ChemicalElement.Au?.Symbol ?? ChemicalElement.Pt?.Symbol ?? ChemicalElement.O?.Symbol ?? "1");
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
                    Console.WriteLine();
                    Console.WriteLine(ExamplesTips ?? "Examples");
                    Console.WriteLine();
                    Console.WriteLine(verb + " period 2");
                    Console.WriteLine(verb + " period 7");
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
                    Console.WriteLine();
                    Console.WriteLine(ExamplesTips ?? "Examples");
                    Console.WriteLine();
                    Console.WriteLine("{0} {1} 10-29", verb, lsKey);
                    Console.WriteLine("{0} {1} 20", verb, lsKey);
                    Console.WriteLine("{0} {1} 80 -q h", verb, lsKey);
                    return;
                case "molecular":
                case "formula":
                case "分子":
                    WriteTips(verb, "molecular <formula>", ElementTips, "Get the information of the specific molecular formula.");
                    Console.WriteLine();
                    Console.WriteLine(ExamplesTips ?? "Examples");
                    Console.WriteLine();
                    Console.WriteLine(verb + " molecular Fe3O4");
                    Console.WriteLine(verb + " molecular R-COOH");
                    return;
                case "copyright":
                    if (!ChemistryResource.Chemistry.Equals("Chemistry", StringComparison.Ordinal))
                        Console.WriteLine(ChemistryResource.Chemistry);
                    var ver = System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version;
                    var sb = new StringBuilder();
                    sb.AppendLine("Trivial.Chemistry");
                    if (ver != null)
                    {
                        sb.AppendFormat("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
                        sb.AppendLine();
                    }

                    sb.AppendLine("Copyright (c) 2021 Kingcean Tuan. All rights reserved.");
                    sb.AppendLine("MIT licensed.");
                    Console.Write(sb.ToString());
                    return;
                default:
                    break;
            }

            Console.WriteLine(UsagesTips ?? "Usages");
            Console.WriteLine();
            WriteTips(verb, "element <symbol>", ElementTips, "Get details of the specific chemical element by symbol or atomic numbers.");
            Console.WriteLine();
            WriteTips(verb, "table", PeriodicTableTips, "Output the periodic table.");
            Console.WriteLine();
            WriteTips(verb, "period <period>", PeriodTips, "List chemical elements in the specific period.");
            Console.WriteLine();
            WriteTips(verb, "all", AllElementsTips, "List all chemical elements.");
            Console.WriteLine();
            WriteTips(verb, lsKey + " <start>-<end>", ElementListTips, "List a speicific range of chemical elements.");
            Console.WriteLine();
            WriteTips(verb, "molecular <formula>", ElementTips, "Get the information of the specific molecular formula.");
        }

        private static void WriteTips(string verb, string cmd, string desc, string descBackup)
        {
            Console.WriteLine("{0} {1}{2} {3}", verb, cmd, Environment.NewLine, desc ?? descBackup);
        }

        private static string WritePeriodicTable()
        {
            var sb = new StringBuilder();

            sb.Append("1 ");
            AppendSymbol(sb, ChemicalElement.H);
            sb.Append(' ', 64);
            AppendSymbol(sb, ChemicalElement.He);
            sb.AppendLine();
            sb.Append("  ");
            AppendNumber(sb, 1);
            sb.Append(' ', 64);
            AppendNumber(sb, 2);
            sb.AppendLine();
            sb.AppendLine();

            sb.Append("2 ");
            AppendSymbol(sb, ChemicalElement.Li);
            AppendSymbol(sb, ChemicalElement.Be);
            sb.Append(' ', 40);
            AppendSymbol(sb, ChemicalElement.B);
            AppendSymbol(sb, ChemicalElement.C);
            AppendSymbol(sb, ChemicalElement.N);
            AppendSymbol(sb, ChemicalElement.O);
            AppendSymbol(sb, ChemicalElement.F);
            AppendSymbol(sb, ChemicalElement.Ne);
            sb.AppendLine();
            sb.Append("  ");
            AppendNumber(sb, 3, 2);
            sb.Append(' ', 40);
            AppendNumber(sb, 5, 6);
            sb.AppendLine();
            sb.AppendLine();

            sb.Append("3 ");
            AppendSymbol(sb, ChemicalElement.Na);
            AppendSymbol(sb, ChemicalElement.Mg);
            sb.Append(' ', 40);
            AppendSymbol(sb, ChemicalElement.Al);
            AppendSymbol(sb, ChemicalElement.Si);
            AppendSymbol(sb, ChemicalElement.P);
            AppendSymbol(sb, ChemicalElement.S);
            AppendSymbol(sb, ChemicalElement.Cl);
            AppendSymbol(sb, ChemicalElement.Ar);
            sb.AppendLine();
            sb.Append("  ");
            AppendNumber(sb, 11, 2);
            sb.Append(' ', 40);
            AppendNumber(sb, 13, 6);
            sb.AppendLine();
            sb.AppendLine();

            sb.Append("4 ");
            for (var i = 19; i <= 36; i++)
            {
                AppendSymbol(sb, ChemicalElement.Get(i));
            }

            sb.AppendLine();
            sb.Append("  ");
            AppendNumber(sb, 19, 18);
            sb.AppendLine();
            sb.AppendLine();

            sb.Append("5 ");
            for (var i = 37; i <= 54; i++)
            {
                AppendSymbol(sb, ChemicalElement.Get(i));
            }

            sb.AppendLine();
            sb.Append("  ");
            AppendNumber(sb, 37, 18);
            sb.AppendLine();
            sb.AppendLine();

            sb.Append("6 ");
            for (var i = 55; i < 58; i++)
            {
                AppendSymbol(sb, ChemicalElement.Get(i));
            }

            for (var i = 72; i <= 86; i++)
            {
                AppendSymbol(sb, ChemicalElement.Get(i));
            }

            sb.AppendLine();
            sb.Append("  ");
            AppendNumber(sb, 55, 2);
            sb.Append("... ");
            AppendNumber(sb, 72, 15);
            sb.AppendLine();
            sb.AppendLine();

            sb.Append("7 ");
            for (var i = 87; i < 90; i++)
            {
                AppendSymbol(sb, ChemicalElement.Get(i));
            }

            for (var i = 104; i <= 118; i++)
            {
                AppendSymbol(sb, ChemicalElement.Get(i));
            }

            sb.AppendLine();
            sb.Append("  ");
            AppendNumber(sb, 87, 2);
            sb.Append("... ");
            AppendNumber(sb, 104, 15);
            sb.AppendLine();
            sb.AppendLine();

            sb.Append(ChemicalElement.La.Symbol);
            sb.Append('-');
            sb.Append(ChemicalElement.Lu.Symbol);
            sb.Append("\t  ");
            for (var i = 57; i <= 71; i++)
            {
                AppendSymbol(sb, ChemicalElement.Get(i));
            }

            sb.AppendLine();
            sb.Append("\t  ");
            AppendNumber(sb, 57, 15);
            sb.AppendLine();
            sb.AppendLine();

            sb.Append(ChemicalElement.Ac.Symbol);
            sb.Append('-');
            sb.Append(ChemicalElement.Lr.Symbol);
            sb.Append("\t  ");
            for (var i = 89; i <= 103; i++)
            {
                AppendSymbol(sb, ChemicalElement.Get(i));
            }

            sb.AppendLine();
            sb.Append("\t  ");
            AppendNumber(sb, 89, 15);
            sb.AppendLine();
            sb.AppendLine();

            if (ChemicalElement.Has(119) && ChemicalElement.Has(120))
            {
                sb.Append("8 119-168 ");
                AppendSymbol(sb, ChemicalElement.Get(119));
                AppendSymbol(sb, ChemicalElement.Get(120));
                for (var i = 121; i < 131; i++)
                {
                    if (ChemicalElement.Has(i))
                        AppendSymbol(sb, ChemicalElement.Get(i));
                    else
                        break;
                }

                sb.AppendLine("...");
            }
            else
            {
                sb.AppendLine("8 119-168");
            }

            sb.AppendLine("9 169-218");

            var s = sb.ToString();
            Console.Write(s);
            return s;
        }

        private static void AppendSymbol(StringBuilder sb, ChemicalElement element)
        {
            var s = element.Symbol?.Trim();
            if (string.IsNullOrEmpty(s)) return;
            if (s.Length > 4) s = s.Substring(0, 4);
            sb.Append(s);
            sb.Append(' ', 4 - s.Length);
        }

        private static void AppendNumber(StringBuilder sb, int i, int count = 1)
        {
            var k = i + count;
            for (var j = i; j < k; j++)
            {
                var s = j.ToString("g");
                sb.Append(s);
                sb.Append(' ', 4 - s.Length);
            }
        }
    }
}
