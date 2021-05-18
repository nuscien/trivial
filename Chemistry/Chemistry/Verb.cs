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
                            Console.WriteLine("The period was too large.");
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

                        WriteLine(element);
                    }
                }
                else
                {
                    Console.WriteLine("The period should be a natural number.");
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
                var col = filter != null && filter.Length == 2 && int.TryParse(filter[0], out var begin) && int.TryParse(filter[1], out var end)
                    ? ChemicalElement.Range(begin, end - begin).ToList()
                    : (filter != null && filter.Length == 1 && int.TryParse(filter[0], out end) ? ChemicalElement.Range(0, end).ToList() : ChemicalElement.Where(null).ToList());
                var filter2 = GetFilter(q);
                foreach (var i in filter2 is null ? col : col.Where(filter2))
                {
                    WriteLine(i, Arguments.Has("en"));
                }

                return;
            }

            if (s == "molecular" || s == "formula")
            {
                s = Arguments.Verb.Value?.Trim() ?? string.Empty;
                if (s.StartsWith("molecular ")) s = s.Substring(10)?.Trim();
                if (s.StartsWith("formula ")) s = s.Substring(8)?.Trim();
                if (s.Length < 1 || s == "molecular" || s == "formula")
                {
                    Console.WriteLine("Empty.");
                    return;
                }

                var m = MolecularFormula.TryParse(s);
                if (m is null)
                {
                    Console.WriteLine("Invalid molecular formula.");
                    return;
                }

                Console.WriteLine(m.ToString());
                foreach (var ele in m.Zip())
                {
                    if (ele.Element is null) continue;
                    Console.WriteLine("{0} x\t {1} \t{2} \t{3}", ele.Count, ele.Element.AtomicNumber, ele.Element.Symbol, ele.Element.Name);
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
                WriteHelp(Arguments.Verb.Key);
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
                var start = false;
                var col = ChemicalElement.Where(GetFilter(q)).ToList();
                if (col.Count <= 20)
                {
                    foreach (var i in col)
                    {
                        WriteLine(i, Arguments.Has("en"));
                    }
                }
                else
                {
                    foreach (var i in col)
                    {
                        if (i is null) continue;
                        if (start) Console.Write(" \t");
                        else start = true;
                        Console.Write(i.ToString());
                    }

                    Console.WriteLine();
                }
                return;
            }

            var eles = s.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Distinct().Select(i => ChemicalElement.Get(i)).Where(i => i != null).Distinct().ToList();
            if (eles.Count < 1)
            {
                Console.WriteLine("Not found.");
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
            WriteHelp(Arguments.Verb?.Key);
        }

        /// <summary>
        /// Writes the element information to the standard output stream.
        /// </summary>
        /// <param name="element">A chemicial element.</param>
        /// <param name="containIsotopes">true if also write its isotopes; otherwise, false.</param>
        public static void WriteInfo(ChemicalElement element, bool containIsotopes = false)
        {
            if (element is null) return;
            Console.WriteLine("{0}\t{1}", element.AtomicNumber, element.Symbol);
            var name = element.Name;
            Console.WriteLine(" {0}", element.Name);
            if (!string.IsNullOrWhiteSpace(element.EnglishName) && !name.Equals(element.EnglishName))
                Console.WriteLine(" {0}", element.EnglishName);
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
            sb.AppendFormat("Isotopes ({0})", isotopes.Count);
            sb.AppendLine();
            var isOdd = false;
            foreach (var isotope in isotopes)
            {
                isOdd = !isOdd;
                var weight = isotope.HasAtomicWeight ? isotope.AtomicWeight.ToString("#.########") : string.Empty;
                sb.AppendFormat(" {0} \t{1}", isotope.AtomicMassNumber, weight);
                if (weight.Length < 8) sb.Append('\t');
                if (isOdd) sb.Append(" \t");
                else sb.AppendLine();
            }

            if (isOdd) Console.WriteLine(sb.ToString());
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

        private Func<ChemicalElement, bool> GetFilter(string q)
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
            sb.AppendFormat("{0}\t{1}\t{2}", element.AtomicNumber, element.Symbol, writeEnglishName ? element.EnglishName : element.Name);
            if (!double.IsNaN(element.AtomicWeight))
            {
                sb.AppendFormat(" \t");
                sb.AppendFormat(element.AtomicWeight.ToString("#.######"));
            }

            Console.WriteLine(sb.ToString());
        }

        private static void WriteHelp(string verb)
        {
            verb = verb?.Trim();
            if (string.IsNullOrEmpty(verb)) verb = "~";
            Console.WriteLine("Usages");
            Console.WriteLine();
            Console.WriteLine(verb + " element <symbol>");
            Console.WriteLine(" Get details of the specific chemical element by symbol or atomic numbers.");
            Console.WriteLine();
            Console.WriteLine(verb + " period <period>");
            Console.WriteLine(" List chemical elements in the specific period.");
            Console.WriteLine();
            Console.WriteLine(verb + " table");
            Console.WriteLine(" Output the periodic table.");
            Console.WriteLine();
            Console.WriteLine(verb + " all");
            Console.WriteLine(" List all chemical elements.");
            Console.WriteLine();
            Console.WriteLine(verb + " ls <start>-<end>");
            Console.WriteLine(" List a speicific range of chemical elements.");
            Console.WriteLine();
            Console.WriteLine(verb + " molecular <formula>");
            Console.WriteLine(" Get the information of the specific molecular formula.");
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

            sb.AppendLine("8 119-168");
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
