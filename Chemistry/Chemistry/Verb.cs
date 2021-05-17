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
    public class ChemicalElementVerb : CommandLine.BaseCommandVerb
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        public static string Description => ChemistryResource.ChemicalElement;

        /// <inheritdoc />
        protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
        {
            await RunAsync(null, cancellationToken);
            var s = Arguments.Get("element", "number", "z", "e", "symbol")?.MergedValue?.Trim();
            if (string.IsNullOrEmpty(s)) s = Arguments.Verb?.TryGet(0);
            if (string.IsNullOrEmpty(s))
            {
                Console.WriteLine(ChemistryResource.ChemicalElement);
                var start = false;
                foreach (var i in ChemicalElement.Where(null))
                {
                    if (i is null) continue;
                    if (start) Console.Write(" \t");
                    else start = true;
                    Console.Write(i.ToString());
                }

                Console.WriteLine();
                return;
            }

            var eles = s.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Distinct().Select(i => ChemicalElement.Get(i)).Where(i => i != null).Distinct().ToList();
            if (eles.Count == 0)
            {
                Console.WriteLine("Not found.");
                return;
            }

            if (eles.Count > 1)
            {
                foreach (var i in eles)
                {
                    WriteLine(i);
                }

                return;
            }

            var element = eles[0];
            Console.WriteLine("{0}\t{1}", element.AtomicNumber, element.Symbol);
            var name = element.Name;
            Console.WriteLine(" {0}", element.Name);
            if (!string.IsNullOrWhiteSpace(element.EnglishName) && !name.Equals(element.EnglishName))
                Console.WriteLine(" {0}", element.EnglishName);
            if (element.HasAtomicWeight)
                Console.WriteLine(" Weight = {0:#.########}", element.AtomicWeight);
            var sb = new StringBuilder();
            if (element.Period > 0 && element.Group > 0)
            {
                sb.AppendFormat(" Period = {0}    Group = {1}", element.Period, element.Group);
                if (!string.IsNullOrWhiteSpace(element.Block))
                    sb.AppendFormat("    Block = {0}", element.Block);
                Console.WriteLine(sb.ToString());
            }

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
            if (string.IsNullOrWhiteSpace(element.Symbol) || element.Period < 1) return;
            var sb = new StringBuilder();
            sb.AppendFormat("{0}\t{1}\t{2}", element.AtomicNumber, element.Symbol, element.Name);
            if (element.HasAtomicWeight)
            {
                sb.AppendFormat(" \t");
                sb.AppendFormat(element.AtomicWeight.ToString("#.######"));
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
