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
            public Item(ChemicalElement element, int count = 1)
            {
                Element = element;
                Count = count;
            }

            /// <summary>
            /// Initializes a new instance of the MolecularFormula.Item class.
            /// </summary>
            /// <param name="symbol">The symbol of the chemical element.</param>
            /// <param name="count">The atom count.</param>
            public Item(string symbol, int count = 1)
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

            /// <summary>
            /// Compares two molecular formula items to indicate if they are same.
            /// leftValue == rightValue
            /// </summary>
            /// <param name="leftValue">The left value to compare.</param>
            /// <param name="rightValue">The right value to compare.</param>
            /// <returns>true if they are same; otherwise, false.</returns>
            public static bool operator ==(Item leftValue, Item rightValue)
            {
                if (ReferenceEquals(leftValue, rightValue)) return true;
                if (leftValue is null || rightValue is null) return false;
                return leftValue.Equals(rightValue);
            }

            /// <summary>
            /// Compares two molecular formula items to indicate if they are different.
            /// leftValue != rightValue
            /// </summary>
            /// <param name="leftValue">The left value to compare.</param>
            /// <param name="rightValue">The right value to compare.</param>
            /// <returns>true if they are different; otherwise, false.</returns>
            public static bool operator !=(Item leftValue, Item rightValue)
            {
                if (ReferenceEquals(leftValue, rightValue)) return false;
                if (leftValue is null || rightValue is null) return true;
                return !leftValue.Equals(rightValue);
            }
        }

        /// <summary>
        /// The source cache.
        /// </summary>
        private readonly List<Item> list;

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
        /// Initializes a new instance of the MolecularFormula class.
        /// </summary>
        /// <param name="element">The chemical element.</param>
        /// <param name="count">The count of the specific chemical element.</param>
        public MolecularFormula(ChemicalElement element, int count = 1)
        {
            list = string.IsNullOrEmpty(element?.Symbol) || count < 1
                ? new List<Item>()
                : new List<Item> { new Item(element, count) };
        }

        /// <summary>
        /// Initializes a new instance of the MolecularFormula class.
        /// </summary>
        /// <param name="elementA">The chemical element A.</param>
        /// <param name="countA">The count of the specific chemical element A.</param>
        /// <param name="elementB">The chemical element B.</param>
        /// <param name="countB">The count of the specific chemical element B.</param>
        /// <param name="elementC">The chemical element C.</param>
        /// <param name="countC">The count of the specific chemical element C.</param>
        /// <param name="elementD">The chemical element D.</param>
        /// <param name="countD">The count of the specific chemical element D.</param>
        /// <param name="elementE">The chemical element F.</param>
        /// <param name="countE">The count of the specific chemical element E.</param>
        /// <param name="elementF">The chemical element E.</param>
        /// <param name="countF">The count of the specific chemical element F.</param>
        /// <param name="elementG">The chemical element G.</param>
        /// <param name="countG">The count of the specific chemical element G.</param>
        public MolecularFormula(ChemicalElement elementA, int countA, ChemicalElement elementB, int countB, ChemicalElement elementC = null, int countC = 1, ChemicalElement elementD = null, int countD = 1, ChemicalElement elementE = null, int countE = 1, ChemicalElement elementF = null, int countF = 1, ChemicalElement elementG = null, int countG = 1)
        {
            list = new List<Item>();
            if (!string.IsNullOrEmpty(elementA?.Symbol) && countA > 0)
                list.Add(new Item(elementA, countA));
            if (!string.IsNullOrEmpty(elementB?.Symbol) && countB > 0)
                list.Add(new Item(elementB, countB));
            if (!string.IsNullOrEmpty(elementC?.Symbol) && countC > 0)
                list.Add(new Item(elementC, countC));
            if (!string.IsNullOrEmpty(elementD?.Symbol) && countD > 0)
                list.Add(new Item(elementD, countD));
            if (!string.IsNullOrEmpty(elementE?.Symbol) && countE > 0)
                list.Add(new Item(elementE, countE));
            if (!string.IsNullOrEmpty(elementF?.Symbol) && countF > 0)
                list.Add(new Item(elementF, countF));
            if (!string.IsNullOrEmpty(elementG?.Symbol) && countG > 0)
                list.Add(new Item(elementG, countG));
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
        /// Gets all elements.
        /// </summary>
        public IEnumerable<ChemicalElement> Elements
            => list.Select(ele => ele.Element).Where(ele => !string.IsNullOrEmpty(ele?.Symbol)).Distinct();

        /// <summary>
        /// Gets the atom count.
        /// </summary>
        public int Count => list.Sum(ele => ele.Count);

        /// <summary>
        /// Gets the proton numbers.
        /// </summary>
        public int ProtonNumber => list.Sum(ele => ele.Count * (ele.Element?.AtomicNumber ?? 0));

        /// <summary>
        /// Gets a collection of all elements and their numbers.
        /// </summary>
        /// <returns>A collection of all elements and their numbers.</returns>
        public IEnumerable<Item> Zip()
        {
            var list = new List<Item>();
            foreach (var item in this.list)
            {
                Zip(list, item);
            }

            return list;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in this instance.
        /// </summary>
        /// <param name="symbol">The string to seek.</param>
        /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
        /// <exception cref="ArgumentNullException">symbol should not be null.</exception>
        /// <exception cref="ArgumentException">symbol should not be empty.</exception>
        public int IndexOf(string symbol)
        {
            if (symbol == null) throw new ArgumentNullException(nameof(symbol), "symbol was null");
            symbol = symbol.Trim();
            if (symbol.Length < 1) throw new ArgumentException("symbol was empty", nameof(symbol));
            return list.FindIndex(ele => symbol.Equals(ele.Element?.Symbol, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in this instance.
        /// </summary>
        /// <param name="symbol">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The optional number of character positions to examine.</param>
        /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
        /// <exception cref="ArgumentNullException">symbol should not be null.</exception>
        public int IndexOf(string symbol, int startIndex, int? count = null)
        {
            if (symbol == null) throw new ArgumentNullException(nameof(symbol), "symbol was null");
            symbol = symbol.Trim();
            if (symbol.Length < 1) throw new ArgumentException("symbol was empty", nameof(symbol));
            return count.HasValue
                ? list.FindIndex(startIndex, count.Value, ele => symbol.Equals(ele.Element?.Symbol, StringComparison.OrdinalIgnoreCase))
                : list.FindIndex(startIndex, ele => symbol.Equals(ele.Element?.Symbol, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of a specified string within this instance.
        /// </summary>
        /// <param name="symbol">The string to seek.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is the last index position in this instance.</returns>
        /// <exception cref="ArgumentNullException">symbol should not be null.</exception>
        public int LastIndexOf(string symbol)
        {
            if (symbol == null) throw new ArgumentNullException(nameof(symbol), "symbol was null");
            symbol = symbol.Trim();
            if (symbol.Length < 1) throw new ArgumentException("symbol was empty", nameof(symbol));
            return list.FindLastIndex(ele => symbol.Equals(ele.Element?.Symbol, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of a specified string within this instance.
        /// </summary>
        /// <param name="symbol">The string to seek.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from startIndex toward the beginning of this instance.</param>
        /// <param name="count">The optional number of character positions to examine.</param>
        /// <returns>The zero-based starting index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is the last index position in this instance.</returns>
        /// <exception cref="ArgumentNullException">symbol should not be null.</exception>
        public int LastIndexOf(string symbol, int startIndex, int? count = null)
        {
            if (symbol == null) throw new ArgumentNullException(nameof(symbol), "symbol was null");
            symbol = symbol.Trim();
            if (symbol.Length < 1) throw new ArgumentException("symbol was empty", nameof(symbol));
            return count.HasValue
                ? list.FindLastIndex(startIndex, count.Value, ele => symbol.Equals(ele.Element?.Symbol, StringComparison.OrdinalIgnoreCase))
                : list.FindLastIndex(startIndex, ele => symbol.Equals(ele.Element?.Symbol, StringComparison.OrdinalIgnoreCase));
        }

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
            return list.Where(ele => ele.Element == element)?.Sum(ele => ele.Count) ?? 0;
        }

        /// <summary>
        /// Gets the count of the specific chemical element.
        /// </summary>
        /// <param name="symbol">The chemcial symbol to get count.</param>
        /// <returns>The total.</returns>
        public int GetCount(string symbol)
        {
            symbol = symbol?.Trim();
            if (string.IsNullOrEmpty(symbol)) return 0;
            return list.Where(ele => symbol.Equals(ele.Element?.Symbol, StringComparison.OrdinalIgnoreCase))?.Sum(ele => ele.Count) ?? 0;
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

            if (sb.Length == 0) sb.Append('e');
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
        /// Compares two molecular formula instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(MolecularFormula leftValue, MolecularFormula rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (leftValue is null || rightValue is null) return false;
            return leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Compares two molecular formula instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(MolecularFormula leftValue, MolecularFormula rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (leftValue is null || rightValue is null) return true;
            return !leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Pluses a molecular formula and a chemical element.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static MolecularFormula operator +(MolecularFormula leftValue, ChemicalElement rightValue)
        {
            var isRightEmpty = string.IsNullOrEmpty(rightValue?.Symbol);
            if (leftValue is null && isRightEmpty) return null;
            if (leftValue is null) return new MolecularFormula(rightValue);
            if (isRightEmpty) return leftValue;
            var col = leftValue.list.ToList();
            col.Add(new Item(rightValue, 1));
            return new MolecularFormula(col);
        }

        /// <summary>
        /// Pluses a molecular formula and a chemical element.
        /// </summary>
        /// <param name="rightValue">The left value for addition operator.</param>
        /// <param name="leftValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static MolecularFormula operator +(ChemicalElement leftValue, MolecularFormula rightValue)
        {
            var isLeftEmpty = string.IsNullOrEmpty(leftValue?.Symbol);
            if (rightValue is null && isLeftEmpty) return null;
            if (rightValue is null) return new MolecularFormula(leftValue);
            if (isLeftEmpty) return rightValue;
            var col = rightValue.list.ToList();
            col.Add(new Item(leftValue, 1));
            return new MolecularFormula(col);
        }

        /// <summary>
        /// Pluses two molecular formula instances.
        /// </summary>
        /// <param name="rightValue">The left value for addition operator.</param>
        /// <param name="leftValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static MolecularFormula operator +(MolecularFormula leftValue, MolecularFormula rightValue)
        {
            if (leftValue is null || leftValue.Count == 0) return rightValue;
            if (rightValue is null || rightValue.Count == 0) return leftValue;
            var col = leftValue.list.ToList();
            col.AddRange(rightValue.list);
            return new MolecularFormula(col);
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

        /// <summary>
        /// Gets a collection of all elements and their numbers.
        /// </summary>
        /// <param name="col">The collection.</param>
        /// <returns>A collection of all elements and their numbers.</returns>
        public static IEnumerable<Item> Zip(IEnumerable<MolecularFormula> col)
        {
            var list = new List<Item>();
            if (col is null) return list;
            foreach (var item in col.SelectMany(ele => ele.list))
            {
                Zip(list, item);
            }

            return list;
        }

        /// <summary>
        /// Tests the elements in the two molecular formula instances are passed by the law of conservation of mass.
        /// </summary>
        /// <param name="a">The left collection.</param>
        /// <param name="b">The right collection.</param>
        /// <returns>true if conservation of mass; otherwise, false.</returns>
        public static bool ConservationOfMass(IEnumerable<MolecularFormula> a, IEnumerable<MolecularFormula> b)
        {
            try
            {
                var dictA = Zip(a).ToList();
                var dictB = Zip(b).ToList();
                if (dictA.Count != dictB.Count) return false;
                var chargeA = a.Sum(ele => ele.ChargeNumber);
                var chargeB = b.Sum(ele => ele.ChargeNumber);
                if (chargeA != chargeB) return false;
                foreach (var item in dictA)
                {
                    if (item is null) continue;
                    if (!dictB.Any(ele => item.Equals(ele))) return false;
                }

                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <param name="formulas">The molecular fomula collection.</param>
        /// <returns>A string that represents the current object.</returns>
        public static string ToString(IEnumerable<MolecularFormula> formulas)
        {
            if (formulas is null) return string.Empty;
            var dict = new Dictionary<MolecularFormula, int>();
            foreach (var item in formulas)
            {
                if (item is null) continue;
                var f = dict.Keys.FirstOrDefault(ele => ele == item);
                if (f is null) dict[item] = 1;
                else dict[f]++;
            }

            return string.Join(" + ", dict.Select(ele => {
                if (ele.Value > 1) return ele.Value.ToString("g") + ele.Key.ToString();
                return ele.Value.ToString();
            }));
        }

        private static void Zip(List<Item> list, Item item)
        {
            if (item?.Element is null || item.Count < 1) return;
            var count = list.Where(ele => ele.Element == item.Element).Sum(ele => ele.Count);
            list.RemoveAll(ele => ele.Element == item.Element);
            list.Add(new Item(item.Element, count + item.Count));
        }
    }
}
