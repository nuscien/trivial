using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Chemistry
{
    /// <summary>
    /// The model of the molecular formula.
    /// </summary>
    public class MolecularFormula : IEquatable<MolecularFormula>
    {
        /// <summary>
        /// The item.
        /// </summary>
        public class Item : IEquatable<Item>
        {
            /// <summary>
            /// Initializes a new instance of the MolecularFormula.Item class.
            /// </summary>
            /// <param name="element">The chemical element.</param>
            /// <param name="count">The atom count.</param>
            public Item(ChemicalElement element, int count = 0)
            {
                Element = element;
                Count = count;
            }

            /// <summary>
            /// Initializes a new instance of the MolecularFormula.Item class.
            /// </summary>
            /// <param name="symbol">The symbol of the chemical element.</param>
            /// <param name="count">The atom count.</param>
            public Item(string symbol, int count = 0)
                : this(ChemicalElement.Get(symbol), count)
            {
            }

            /// <summary>
            /// Gets the chemical element.
            /// </summary>
            public ChemicalElement Element { get; }

            /// <summary>
            /// Gets the atom count.
            /// </summary>
            public int Count { get; }

            /// <summary>
            /// Peturns a string that represents the current chemical element information.
            /// </summary>
            /// <returns>A string that represents the current chemical element information.</returns>
            public override string ToString()
            {
                var symbol = Element?.Symbol?.Trim();
                if (string.IsNullOrWhiteSpace(symbol) || Count < 1) return null;
                if (Count == 1) return symbol;
                return symbol + Count
                    .ToString("g")
                    .Replace('0', '₀')
                    .Replace('1', '₁')
                    .Replace('2', '₂')
                    .Replace('3', '₃')
                    .Replace('4', '₄')
                    .Replace('5', '₅')
                    .Replace('6', '₆')
                    .Replace('7', '₇')
                    .Replace('8', '₈')
                    .Replace('9', '₉');
            }

            /// <inheritdoc />
            public override int GetHashCode()
                =>new Tuple<ChemicalElement, int>(Element, Count).GetHashCode();

            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <param name="other">The object to compare with the current instance.</param>
            /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
            public bool Equals(Item other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return Element == other.Element
                    && Count == other.Count;
            }

            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <param name="other">The object to compare with the current instance.</param>
            /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
            public override bool Equals(object other)
            {
                if (other is null) return false;
                if (other is Item isotope) return Equals(isotope);
                if (other is ChemicalElement element) return Count == 1 && element.Equals(Element);
                return false;
            }
        }

        /// <summary>
        /// The source cache.
        /// </summary>
        private readonly List<Item> list = new();

        /// <summary>
        /// The string format cache.
        /// </summary>
        private string s;

        /// <summary>
        /// Initializes a new instance of the MolecularFormula class.
        /// </summary>
        /// <param name="col">The items.</param>
        public MolecularFormula(IEnumerable<Item> col)
        {
            list = new List<Item>(col);
        }

        /// <summary>
        /// Initializes a new instance of the MolecularFormula class.
        /// </summary>
        /// <param name="col">The items.</param>
        /// <param name="chargeNumber">The ionic charge numbe.</param>
        public MolecularFormula(IEnumerable<Item> col, int chargeNumber)
        {
            list = new List<Item>(col.Where(ele => ele?.Element != null && ele.Count > 0));
            ChargeNumber = chargeNumber;
        }

        /// <summary>
        /// Gets the ionic charge number.
        /// </summary>
        public int ChargeNumber { get; }

        /// <summary>
        /// Gets a value indicating whether this is an ion.
        /// </summary>
        public bool IsIon => ChargeNumber != 0;

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IReadOnlyList<Item> Items => list.AsReadOnly();

        /// <summary>
        /// Gets the atom count.
        /// </summary>
        public int Count => list.Sum(ele => ele.Count);

        ///// <summary>
        ///// Reports the zero-based index of the first occurrence of the specified string in this instance.
        ///// </summary>
        ///// <param name="symbol">The string to seek.</param>
        ///// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
        ///// <exception cref="ArgumentNullException">symbol should not be null.</exception>
        //public int IndexOf(string symbol)
        //    => ToString().IndexOf(symbol);

        ///// <summary>
        ///// Reports the zero-based index of the first occurrence of the specified string in this instance.
        ///// </summary>
        ///// <param name="symbol">The string to seek.</param>
        ///// <param name="startIndex">The search starting position.</param>
        ///// <param name="count">The optional number of character positions to examine.</param>
        ///// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
        ///// <exception cref="ArgumentNullException">symbol should not be null.</exception>
        //public int IndexOf(string symbol, int startIndex, int? count = null)
        //    => count.HasValue ? ToString().IndexOf(symbol, startIndex, count.Value) : ToString().IndexOf(symbol, startIndex);

        ///// <summary>
        ///// Reports the zero-based index position of the last occurrence of a specified string within this instance.
        ///// </summary>
        ///// <param name="symbol">The string to seek.</param>
        ///// <returns>The zero-based starting index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is the last index position in this instance.</returns>
        ///// <exception cref="ArgumentNullException">symbol should not be null.</exception>
        //public int LastIndexOf(string symbol)
        //    => ToString().LastIndexOf(symbol);

        ///// <summary>
        ///// Reports the zero-based index position of the last occurrence of a specified string within this instance.
        ///// </summary>
        ///// <param name="symbol">The string to seek.</param>
        ///// <param name="startIndex">The search starting position. The search proceeds from startIndex toward the beginning of this instance.</param>
        ///// <param name="count">The optional number of character positions to examine.</param>
        ///// <returns>The zero-based starting index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is the last index position in this instance.</returns>
        ///// <exception cref="ArgumentNullException">symbol should not be null.</exception>
        //public int LastIndexOf(string symbol, int startIndex, int? count = null)
        //    => count.HasValue ? ToString().LastIndexOf(symbol, startIndex, count.Value) : ToString().LastIndexOf(symbol, startIndex);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in this instance.
        /// </summary>
        /// <param name="element">The chemical element to seek.</param>
        /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
        /// <exception cref="ArgumentNullException">element should not be null.</exception>
        public int IndexOf(ChemicalElement element)
            => element != null ? list.FindIndex(ele => ele.Element == element) : throw new ArgumentNullException(nameof(element), "element was null.");

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of a specified string within this instance.
        /// </summary>
        /// <param name="element">The chemical element to seek.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is the last index position in this instance.</returns>
        /// <exception cref="ArgumentNullException">element should not be null.</exception>
        public int LastIndexOf(ChemicalElement element)
            => element != null ? list.FindLastIndex(ele => ele.Element == element) : throw new ArgumentNullException(nameof(element), "element was null.");

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in this instance.
        /// </summary>
        /// <param name="element">The chemical element to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The optional number of character positions to examine.</param>
        /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
        /// <exception cref="ArgumentNullException">element should not be null.</exception>
        public int IndexOf(ChemicalElement element, int startIndex, int? count = null)
            => element != null ? (count.HasValue
            ? list.FindIndex(startIndex, count.Value, ele => ele.Element == element)
            : list.FindIndex(startIndex, ele => ele.Element == element)) : throw new ArgumentNullException(nameof(element), "element was null.");

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of a specified string within this instance.
        /// </summary>
        /// <param name="element">The chemical element to seek.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from startIndex toward the beginning of this instance.</param>
        /// <param name="count">The optional number of character positions to examine.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is the last index position in this instance.</returns>
        /// <exception cref="ArgumentNullException">element should not be null.</exception>
        public int LastIndexOf(ChemicalElement element, int startIndex, int? count = null)
            => element != null ? (count.HasValue
            ? list.FindLastIndex(startIndex, count.Value, ele => ele.Element == element)
            : list.FindLastIndex(startIndex, ele => ele.Element == element)) : throw new ArgumentNullException(nameof(element), "element was null.");

        /// <summary>
        /// Gets the count of the specific chemical element.
        /// </summary>
        /// <param name="element">The chemcial element to get count.</param>
        /// <returns>The total.</returns>
        public int GetCount(ChemicalElement element)
        {
            if (element is null) return 0;
            return list.Where(ele => ele.Element == element).Sum(ele => ele.Count);
        }

        /// <summary>
        /// Gets the count of the specific chemical element.
        /// </summary>
        /// <param name="symbol">The chemcial symbol to get count.</param>
        /// <returns>The total.</returns>
        public int GetCount(string symbol)
        {
            var element = ChemicalElement.Get(symbol);
            return GetCount(element);
        }

        /// <summary>
        /// Peturns a string that represents the current chemical element information.
        /// </summary>
        /// <returns>A string that represents the current chemical element information.</returns>
        public override string ToString()
        {
            if (s != null) return s;
            var sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.Append(item.ToString());
            }

            if (ChargeNumber != 0)
            {
                if (ChargeNumber > 1 || ChargeNumber < -1) sb.Append(Math.Abs(ChargeNumber)
                    .ToString("g")
                    .Replace('0', '⁰')
                    .Replace('1', '¹')
                    .Replace('2', '²')
                    .Replace('3', '³')
                    .Replace('4', '⁴')
                    .Replace('5', '⁵')
                    .Replace('6', '⁶')
                    .Replace('7', '⁷')
                    .Replace('8', '⁸')
                    .Replace('9', '⁹'));
                sb.Append(ChargeNumber > 0 ? '﹢' : 'ˉ');
            }

            return s = sb.ToString();
        }

        /// <inheritdoc />
        public override int GetHashCode()
            => new Tuple<List<Item>, int>(list, ChargeNumber).GetHashCode();

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(MolecularFormula other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ChargeNumber != other.ChargeNumber) return false;
            return Collection.ListExtensions.Equals(list, other.list);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is MolecularFormula isotope) return Equals(isotope);
            if (other is IEnumerable<Item> col) return ChargeNumber == 0 && Collection.ListExtensions.Equals(list, col.ToList());
            return false;
        }

        /// <summary>
        /// Parses.
        /// </summary>
        /// <param name="s">The input.</param>
        /// <returns>A molecular formula instance parsed.</returns>
        public static MolecularFormula Parse(string s)
        {
            s = s?.Trim();
            if (string.IsNullOrWhiteSpace(s)) return null;
            return TryParse(s) ?? throw new FormatException("s is invalid.");
        }

        /// <summary>
        /// Tries to parse a molecular formula string.
        /// </summary>
        /// <param name="s">The input.</param>
        /// <param name="result">The result output.</param>
        /// <returns>true if parse succeeded; otherwise, false.</returns>
        public static bool TryParse(string s, out MolecularFormula result)
        {
            result = TryParse(s);
            return result != null;
        }

        /// <summary>
        /// Tries to parse a molecular formula string.
        /// </summary>
        /// <param name="s">The input.</param>
        /// <returns>A molecular formula instance parsed.</returns>
        public static MolecularFormula TryParse(string s)
        {
            s = s?.Trim();
            if (string.IsNullOrWhiteSpace(s)) return null;
            var col = new List<Item>();
            var sb = new StringBuilder();
            var count = 0;
            var hasCount = false;
            var sup = 0;
            foreach (var c in s)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    sup = 0;
                    if (sb.Length > 0)
                    {
                        var item = new Item(sb.ToString(), hasCount ? count : 1);
                        if (item.Element is null) return null;
                        col.Add(item);
                    }

                    count = 0;
                    hasCount = false;
                    sb.Clear();
                    sb.Append(c);
                    continue;
                }

                if (c >= 'a' && c <= 'z')
                {
                    sup = 0;
                    count = 0;
                    hasCount = false;
                    sb.Append(c);
                    continue;
                }

                var push = false;
                switch (c)
                {
                    case '0':
                    case '₀':
                        sup = 0;
                        count *= 10;
                        break;
                    case '1':
                    case '₁':
                        sup = 0;
                        count = count * 10 + 1;
                        hasCount = true;
                        break;
                    case '2':
                    case '₂':
                        sup = 0;
                        count = count * 10 + 2;
                        hasCount = true;
                        break;
                    case '3':
                    case '₃':
                        sup = 0;
                        count = count * 10 + 3;
                        hasCount = true;
                        break;
                    case '4':
                    case '₄':
                        sup = 0;
                        count = count * 10 + 4;
                        hasCount = true;
                        break;
                    case '5':
                    case '₅':
                        sup = 0;
                        count = count * 10 + 5;
                        hasCount = true;
                        break;
                    case '6':
                    case '₆':
                        sup = 0;
                        count = count * 10 + 6;
                        hasCount = true;
                        break;
                    case '7':
                    case '₇':
                        sup = 0;
                        count = count * 10 + 7;
                        hasCount = true;
                        break;
                    case '8':
                    case '₈':
                        count = count * 10 + 8;
                        hasCount = true;
                        break;
                    case '9':
                    case '₉':
                        sup = 0;
                        count = count * 10 + 9;
                        hasCount = true;
                        break;
                    case ' ':
                    case '_':
                    case '·':
                    case '.':
                    case ',':
                    case '~':
                        sup = 0;
                        push = true;
                        break;
                    case '-':
                    case '+':
                    case 'ˉ':
                    case '﹢':
                        push = true;
                        break;
                    case '⁰':
                        sup *= 10;
                        push = true;
                        break;
                    case '¹':
                        sup = sup * 10 + 1;
                        push = true;
                        break;
                    case '²':
                        sup = sup * 10 + 2;
                        push = true;
                        break;
                    case '³':
                        sup = sup * 10 + 3;
                        push = true;
                        break;
                    case '⁴':
                        sup = sup * 10 + 4;
                        push = true;
                        break;
                    case '⁵':
                        sup = sup * 10 + 5;
                        push = true;
                        break;
                    case '⁶':
                        sup = sup * 10 + 6;
                        push = true;
                        break;
                    case '⁷':
                        sup = sup * 10 + 7;
                        push = true;
                        break;
                    case '⁸':
                        sup = sup * 10 + 8;
                        push = true;
                        break;
                    case '⁹':
                        sup = sup * 10 + 9;
                        push = true;
                        break;
                    default:
                        return null;
                }

                if (push)
                {
                    if (sb.Length > 0)
                    {
                        var item = new Item(sb.ToString(), hasCount ? count : 1);
                        if (item.Element is null) return null;
                        col.Add(item);
                        sb.Clear();
                    }

                    count = 0;
                    hasCount = false;
                }
            }

            if (sb.Length > 0)
            {
                var item = new Item(sb.ToString(), hasCount ? count : 1);
                if (item.Element is null) return null;
                col.Add(item);
                sb.Clear();
            }

            var ion = 0;
            if (s.EndsWith("+") || s.EndsWith("﹢"))
            {
                ion = sup;
            }
            else if (s.EndsWith("-") || s.EndsWith("ˉ"))
            {
                ion = -sup;
            }

            var result = new MolecularFormula(col, ion);
            return result.list.Count > 0 ? result : null;
        }
    }
}
