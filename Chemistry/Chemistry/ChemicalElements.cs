using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.Chemistry
{
    /// <summary>
    /// The periodic table of chemical element.
    /// </summary>
    public partial class ChemicalElement
    {
        /// <summary>
        /// Periodic table initialization locker.
        /// </summary>
        private readonly static object locker = new();

        /// <summary>
        /// The latin numbers.
        /// </summary>
        private readonly static string[] latinNumbers = new[]
        {
            "Nil", "Un", "Bi", "Tri", "Quad", "Pent", "Hex", "Sept", "Oct", "Enn"
        };

        /// <summary>
        /// The periodic table.
        /// </summary>
        private static ChemicalElement[] periodicTable;

        /// <summary>
        /// The periodic table by symbol indexing.
        /// </summary>
        private static Dictionary<string, ChemicalElement> symbols;

        /// <summary>
        /// Other elements.
        /// </summary>
        private readonly static ConcurrentDictionary<int, ChemicalElement> others = new();

        /// <summary>
        /// Gets a chemical element.
        /// </summary>
        /// <param name="number">The atomic number (or proton number, symbol Z) of the chemical element. The number is one-based.</param>
        /// <returns>The chemical element; or null, if not found.</returns>
        public static ChemicalElement Get(int number)
        {
            Init();
            if (number < 1) return null;
            if (number <= periodicTable.Length) return periodicTable[number - 1];
            if (others.TryGetValue(number, out var r)) return r;
            r = new ChemicalElement(number, null, null, double.NaN);
            if (r.Period > 0) others.TryAdd(number, r);
            return r;
        }

        /// <summary>
        /// Gets a chemical element.
        /// </summary>
        /// <param name="symbol">The element symbol.</param>
        /// <returns>The chemical element; or null, if not found.</returns>
        public static ChemicalElement Get(string symbol)
        {
            Init();
            symbol = symbol.Trim();
            if (string.IsNullOrEmpty(symbol)) return null;
            if (symbols.TryGetValue(symbol, out var r)) return r;
            foreach (var item in others.Values)
            {
                if (symbol.Equals(item?.Symbol, StringComparison.OrdinalIgnoreCase))
                    return item;
            }

            if (int.TryParse(symbol, out var i) && i > 0) return Get(i);
            if (symbol.Length < 3) return null;
            i = symbol[0] switch
            {
                'U' => 1,
                'B' => 2,
                'T' => 3,
                'Q' => 4,
                'P' => 5,
                'H' => 6,
                'S' => 7,
                'O' => 8,
                'E' => 9,
                _ => -1
            };
            if (i < 0) return null;
            for (var j = 1; j < symbol.Length; j++)
            {
                i *= 10;
                var k = symbol[j] switch
                {
                    'n' => 0,
                    'u' => 1,
                    'b' => 2,
                    't' => 3,
                    'q' => 4,
                    'p' => 5,
                    'h' => 6,
                    's' => 7,
                    'o' => 8,
                    'e' => 9,
                    _ => -1
                };
                if (k < 0) return null;
                i += k;
            }

            return Get(i);
        }

        /// <summary>
        /// Gets a value indicating whether the symbol is valid.
        /// </summary>
        /// <param name="symbol">The element symbol.</param>
        /// <returns>true if valid; otherwise, false.</returns>
        public static bool IsValid(string symbol)
        {
            return Get(symbol) != null;
        }

        /// <summary>
        /// Registers a chemical element into periodic table.
        /// </summary>
        /// <param name="element">The chemical element to register.</param>
        public static void Register(ChemicalElement element)
        {
            if (string.IsNullOrWhiteSpace(element?.Symbol)) return;
            Init();
            var number = element.AtomicNumber;
            if (number < 1) return;
            lock (locker)
            {
                if (number < periodicTable.Length) periodicTable[number - 1] = element;
                else others[number] = element;
                symbols[element.Symbol.Trim()] = element;
            }
        }

        /// <summary>
        /// Registers a chemical element into periodic table.
        /// </summary>
        /// <param name="elements">The chemical element to register.</param>
        public static void Register(IEnumerable<ChemicalElement> elements)
        {
            if (elements is null) return;
            Init();
            lock (locker)
            {
                foreach (var element in elements)
                {
                    if (string.IsNullOrWhiteSpace(element?.Symbol)) continue;
                    var number = element.AtomicNumber;
                    if (number < 1) continue;
                    if (number < periodicTable.Length) periodicTable[number - 1] = element;
                    else others[number] = element;
                    symbols[element.Symbol.Trim()] = element;
                }
            }
        }

        /// <summary>
        /// Filters the periodic table based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An enumerable instance that contains elements from the periodic table that satisfy the condition.</returns>
        public static IEnumerable<ChemicalElement> Where(Func<ChemicalElement, bool> predicate)
        {
            foreach (var item in periodicTable.Where(predicate))
            {
                yield return item;
            }

            foreach (var item in others.Values.Where(predicate))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Gets the symbol of unidentified element.
        /// </summary>
        /// <param name="number">The atomic number.</param>
        /// <returns>The symbol.</returns>
        private static string GetSymbol(int number)
        {
            if (number < 1) return "?";
            var s = number.ToString("g");
            for (var i = 0; i < 10; i++)
            {
                s = s.Replace(i.ToString("g"), latinNumbers[i][0].ToString());
            }

            return s.ToLowerInvariant().ToSpecificCaseInvariant(Cases.FirstLetterUpper);
        }

        /// <summary>
        /// Gets the name in English of unidentified element.
        /// </summary>
        /// <param name="number">The atomic number.</param>
        /// <returns>The name.</returns>
        private static string GetName(int number)
        {
            var s = number.ToString("g");
            for (var i = 0; i < 10; i++)
            {
                s = s.Replace(i.ToString("g"), latinNumbers[i]);
            }

            return s.ToLowerInvariant().ToSpecificCaseInvariant(Cases.FirstLetterUpper) + (s.EndsWith("i") ? "um" : "ium");
        }

        /// <summary>
        /// Initializes the periodic table.
        /// </summary>
        private static void Init()
        {
            if (periodicTable != null) return;
            lock (locker)
            {
                if (periodicTable != null) return;
                periodicTable = new ChemicalElement[]
                {
                    new ChemicalElement(1, "H", "Hydrogen", 1.008),
                    new ChemicalElement(2, "He", "Helium", 4.002602),
                    new ChemicalElement(3, "Li", "Lithium", 6.94),
                    new ChemicalElement(4, "Be", "Beryllium", 9.0121831),
                    new ChemicalElement(5, "B", "Boron", 10.81),
                    new ChemicalElement(6, "C", "Carbon", 12.011),
                    new ChemicalElement(7, "N", "Nitrogen", 14.007),
                    new ChemicalElement(8, "O", "Oxygen", 15.999),
                    new ChemicalElement(9, "F", "Fluorine", 18.998403163),
                    new ChemicalElement(10, "Ne", "Neon", 20.1797),
                    new ChemicalElement(11, "Na", "Sodium", 22.98976928),
                    new ChemicalElement(12, "Mg", "Magnesium", 24.305),
                    new ChemicalElement(13, "Al", "Aluminium", 26.9815384),
                    new ChemicalElement(14, "Si", "Silicon", 28.085),
                    new ChemicalElement(15, "P", "Phosphorus", 30.973761998),
                    new ChemicalElement(16, "S", "Sulfur", 32.06),
                    new ChemicalElement(17, "Cl", "Chlorine", 35.45),
                    new ChemicalElement(18, "Ar", "Argon", 39.95),
                    new ChemicalElement(19, "K", "Potassium", 39.0983),
                    new ChemicalElement(20, "Ca", "Calcium", 40.078),
                    new ChemicalElement(21, "Sc", "Scandium", 44.955908),
                    new ChemicalElement(22, "Ti", "Titanium", 47.867),
                    new ChemicalElement(23, "V", "Vanadium", 50.9415),
                    new ChemicalElement(24, "Cr", "Chromium", 51.9961),
                    new ChemicalElement(25, "Mn", "Manganese", 54.938043),
                    new ChemicalElement(26, "Fe", "Iron", 55.845),
                    new ChemicalElement(27, "Co", "Cobalt", 58.933194),
                    new ChemicalElement(28, "Ni", "Nickel", 58.6934),
                    new ChemicalElement(29, "Cu", "Copper", 63.546),
                    new ChemicalElement(30, "Zn", "Zinc", 65.38),
                    new ChemicalElement(31, "Ga", "Gallium", 69.723),
                    new ChemicalElement(32, "Ge", "Germanium", 72.630),
                    new ChemicalElement(33, "As", "Arsenic", 74.921595),
                    new ChemicalElement(34, "Se", "Selenium", 78.971),
                    new ChemicalElement(35, "Br", "Bromine", 79.904),
                    new ChemicalElement(36, "Kr", "Krypton", 83.798),
                    new ChemicalElement(37, "Rb", "Rubidium", 85.4678),
                    new ChemicalElement(38, "Sr", "Strontium", 87.62),
                    new ChemicalElement(39, "Y", "Yttrium", 88.90584),
                    new ChemicalElement(40, "Zr", "Zirconium", 91.224),
                    new ChemicalElement(41, "Nb", "Niobium", 92.90637),
                    new ChemicalElement(42, "Mo", "Molybdenum", 95.95),
                    new ChemicalElement(43, "Tc", "Technetium", 98),
                    new ChemicalElement(44, "Ru", "Ruthenium", 101.07),
                    new ChemicalElement(45, "Rh", "Rhodium", 102.90549),
                    new ChemicalElement(46, "Pd", "Palladium", 106.42),
                    new ChemicalElement(47, "Ag", "Silver", 107.8682),
                    new ChemicalElement(48, "Cd", "Cadmium", 112.414),
                    new ChemicalElement(49, "In", "Indium", 114.818),
                    new ChemicalElement(50, "Sn", "Tin", 118.710),
                    new ChemicalElement(51, "Sb", "Antimony", 121.760),
                    new ChemicalElement(52, "Te", "Tellurium", 127.60),
                    new ChemicalElement(53, "I", "Iodine", 126.90447),
                    new ChemicalElement(54, "Xe", "Xenon", 131.293),
                    new ChemicalElement(55, "Cs", "Caesium", 132.90545196),
                    new ChemicalElement(56, "Ba", "Barium", 137.327),
                    new ChemicalElement(57, "La", "Lanthanum", 138.90547),
                    new ChemicalElement(58, "Ce", "Cerium", 140.116),
                    new ChemicalElement(59, "Pr", "Praseodymium", 140.90766),
                    new ChemicalElement(60, "Nd", "Neodymium", 144.242),
                    new ChemicalElement(61, "Pm", "Promethium", 145),
                    new ChemicalElement(62, "Sm", "Samarium", 150.36),
                    new ChemicalElement(63, "Eu", "Europium", 151.964),
                    new ChemicalElement(64, "Gd", "Gadolinium", 157.25),
                    new ChemicalElement(65, "Tb", "Terbium", 158.925354),
                    new ChemicalElement(66, "Dy", "Dysprosium", 162.500),
                    new ChemicalElement(67, "Ho", "Holmium", 164.930328),
                    new ChemicalElement(68, "Er", "Erbium", 167.259),
                    new ChemicalElement(69, "Tm", "Thulium", 168.934218),
                    new ChemicalElement(70, "Yb", "Ytterbium", 173.045),
                    new ChemicalElement(71, "Lu", "Lutetium", 174.9668),
                    new ChemicalElement(72, "Hf", "Hafnium", 178.49),
                    new ChemicalElement(73, "Ta", "Tantalum", 180.94788),
                    new ChemicalElement(74, "W", "Tungsten", 183.84),
                    new ChemicalElement(75, "Re", "Rhenium", 186.207),
                    new ChemicalElement(76, "Os", "Osmium", 190.23),
                    new ChemicalElement(77, "Ir", "Iridium", 192.217),
                    new ChemicalElement(78, "Pt", "Platinum", 195.084),
                    new ChemicalElement(79, "Au", "Gold", 196.966570),
                    new ChemicalElement(80, "Hg", "Mercury", 200.592),
                    new ChemicalElement(81, "Tl", "Thallium", 204.38),
                    new ChemicalElement(82, "Pb", "Lead", 207.2),
                    new ChemicalElement(83, "Bi", "Bismuth", 208.98040),
                    new ChemicalElement(84, "Po", "Polonium", 209),
                    new ChemicalElement(85, "At", "Astatine", 210),
                    new ChemicalElement(86, "Rn", "Radon", 222),
                    new ChemicalElement(87, "Fr", "Francium", 223),
                    new ChemicalElement(88, "Ra", "Radium", 226),
                    new ChemicalElement(89, "Ac", "Actinium", 227),
                    new ChemicalElement(90, "Th", "Thorium", 232.0377),
                    new ChemicalElement(91, "Pa", "Protactinium", 231.03588),
                    new ChemicalElement(92, "U", "Uranium", 238.02891),
                    new ChemicalElement(93, "Np", "Neptunium", 237),
                    new ChemicalElement(94, "Pu", "Plutonium", 244),
                    new ChemicalElement(95, "Am", "Americium", 243),
                    new ChemicalElement(96, "Cm", "Curium", 247),
                    new ChemicalElement(97, "Bk", "Berkelium", 247),
                    new ChemicalElement(98, "Cf", "Californium", 251),
                    new ChemicalElement(99, "Es", "Einsteinium", 252),
                    new ChemicalElement(100, "Fm", "Fermium", 257),
                    new ChemicalElement(101, "Md", "Mendelevium", 258),
                    new ChemicalElement(102, "No", "Nobelium", 259),
                    new ChemicalElement(103, "Lr", "Lawrencium", 266),
                    new ChemicalElement(104, "Rf", "Rutherfordium", 267),
                    new ChemicalElement(105, "Db", "Dubnium", 268),
                    new ChemicalElement(106, "Sg", "Seaborgium", 269),
                    new ChemicalElement(107, "Bh", "Bohrium", 270),
                    new ChemicalElement(108, "Hs", "Hassium", 270),
                    new ChemicalElement(109, "Mt", "Meitnerium", 278),
                    new ChemicalElement(110, "Ds", "Darmstadtium", 281),
                    new ChemicalElement(111, "Rg", "Roentgenium", 282),
                    new ChemicalElement(112, "Cn", "Copernicium", 285),
                    new ChemicalElement(113, "Nh", "Nihonium", 286),
                    new ChemicalElement(114, "Fl", "Flerovium", 289),
                    new ChemicalElement(115, "Mc", "Moscovium", 290),
                    new ChemicalElement(116, "Lv", "Livermorium", 293),
                    new ChemicalElement(117, "Ts", "Tennessine", 294),
                    new ChemicalElement(118, "Og", "Oganesson", 294)
                };

                symbols = periodicTable.ToDictionary(ele => ele.Symbol);
            }
        }
    }
}
