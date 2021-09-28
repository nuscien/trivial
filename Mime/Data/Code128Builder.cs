using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Data
{
    /// <summary>
    /// The code-128, which is a high-density linear barcode symbology defined in ISO/IEC 15417:2007.
    /// It is used for alphanumeric or numeric-only barcodes.
    /// </summary>
    public partial class Code128 : IReadOnlyList<byte>
    {
#pragma warning disable IDE0056
        private readonly List<byte> v;

        /// <summary>
        /// Initializes a new instance of Code128 class.
        /// </summary>
        /// <param name="values"></param>
        private Code128(List<byte> values)
        {
            v = values ?? new List<byte>();
        }

        /// <summary>
        /// Gets the count of symbol.
        /// </summary>
        public int Count => v.Count;

        /// <summary>
        /// Gets the value of the specific symbol.
        /// </summary>
        /// <param name="index">The index of symbol.</param>
        /// <returns>The value of the symbol.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
        public byte this[int index] => v[index];

        /// <summary>
        /// Converts to boolean list.
        /// White represented as false, black represented as true.
        /// </summary>
        /// <returns>The boolean list of barcode.</returns>
        public List<bool> ToBarcode()
        {
            var list = v.SelectMany(GetPattern).ToList();
            list.Add(true);
            list.Add(true);
            return list;
        }

        /// <summary>
        /// Gets all information of application identifier and its data value.
        /// </summary>
        /// <returns>A collection with application identifiers and their data value.</returns>
        public IEnumerable<string> GetAiData()
#if NETFRAMEWORK || NETSTANDARD2_0
            => ToString().Split(new[] { "[FNC1]" }, StringSplitOptions.None).Skip(1).Select(ele =>
#else
            => ToString().Split("[FNC1]", StringSplitOptions.None).Skip(1).Select(ele =>
#endif
            {
                var pos = ele.IndexOf('[');
                return pos < 0 ? ele : ele.Substring(0, pos);
            });

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (v.Count < 4) return string.Empty;
            var subtype = v[0] switch
            {
                103 => Subtypes.A,
                104 => Subtypes.B,
                105 => Subtypes.C,
                _ => (Subtypes)0
            };
            if (subtype == 0) return string.Empty;
            var subtype2 = subtype;
            var high = false;
            foreach (var b in v.Skip(1).Take(v.Count - 3))
            {
                if (b > 101)
                {
                    if (b > 102) break;
                    sb.Append("[FNC1]");
                    continue;
                }

                var st = subtype;
                subtype = subtype2;
                if (st == Subtypes.C)
                {
                    high = false;
                    if (b < 100 || b == 102)
                    {
                        sb.Append(b.ToString("00"));
                        continue;
                    }

                    switch (b)
                    {
                        case 100:
                            subtype = subtype2 = Subtypes.B;
                            break;
                        case 101:
                            subtype = subtype2 = Subtypes.A;
                            break;
                    }

                    continue;
                }

                var isA = st == Subtypes.A;
                if (!isA && st != Subtypes.B)
                {
                    high = false;
                    continue;
                }

                if (b < 96)
                {
                    if (high)
                    {
                        high = false;
                        var c = (char)(ToString(st, b).FirstOrDefault() + 128);
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append(ToString(st, b));
                    }

                    continue;
                }

                switch (b)
                {
                    case 96:
                        sb.Append("[FNC3]");
                        break;
                    case 97:
                        sb.Append("[FNC2]");
                        break;
                    case 98:
                        subtype = isA ? Subtypes.B : Subtypes.A;
                        continue;
                    case 99:
                        subtype = subtype2 = Subtypes.C;
                        break;
                    case 100:
                        if (!isA)
                        {
                            high = !high;
                            continue;
                        }

                        subtype = subtype2 = Subtypes.B;
                        break;
                    case 101:
                        if (isA)
                        {
                            high = !high;
                            continue;
                        }

                        subtype = subtype2 = Subtypes.A;
                        break;
                }

                high = false;
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <returns>The barcode string.</returns>
        /// <exception cref="InvalidOperationException">It was not an EAN-13 ro EAN-8 code.</exception>
        public string ToBarcodeString()
        {
            var sb = new StringBuilder();
            foreach (var b in v)
            {
                sb.Append(GetPattern(b));
            }

            sb.Append("11");
            return sb.ToString();
        }

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <param name="black">The value of black represented.</param>
        /// <param name="white">The value of white represented.</param>
        /// <returns>The barcode string.</returns>
        /// <exception cref="InvalidOperationException">It was not an EAN-13 ro EAN-8 code.</exception>
        public string ToBarcodeString(char black, char white)
            => string.Join(string.Empty, ToBarcode().Select(ele => ele ? black : white));

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <param name="selector">The selector to convert boolean array to a string. Per boolean value, white represented as false, black represented as true.</param>
        /// <returns>The barcode string.</returns>
        /// <exception cref="InvalidOperationException">It was not an EAN-13 ro EAN-8 code.</exception>
        public string ToBarcodeString(Func<bool, char> selector)
            => selector != null ? string.Join(string.Empty, ToBarcode().Select(selector)) : ToBarcodeString();

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <param name="selector">The selector to convert boolean array to a string. Per boolean value, white represented as false, black represented as true.</param>
        /// <returns>The barcode string.</returns>
        /// <exception cref="InvalidOperationException">It was not an EAN-13 ro EAN-8 code.</exception>
        public string ToBarcodeString(Func<bool, int, char> selector)
            => selector != null ? string.Join(string.Empty, ToBarcode().Select(selector)) : ToBarcodeString();

        /// <summary>
        /// Creates with Start Code A.
        /// </summary>
        /// <param name="values">The value collection of symbol.</param>
        /// <returns>The Code 128 builder.</returns>
        /// <exception cref="ArgumentNullException">values was null.</exception>
        /// <exception cref="ArgumentException">values was empty.</exception>
        /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
        public static Code128 CreateA(IEnumerable<byte> values)
            => Create(values, 103);

        /// <summary>
        /// Creates with Start Code B.
        /// </summary>
        /// <param name="values">The value collection of symbol.</param>
        /// <returns>The Code 128 builder.</returns>
        /// <exception cref="ArgumentNullException">values was null.</exception>
        /// <exception cref="ArgumentException">values was empty.</exception>
        /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
        public static Code128 CreateB(IEnumerable<byte> values)
            => Create(values, 104);

        /// <summary>
        /// Creates with Start Code C.
        /// </summary>
        /// <param name="values">The value collection of symbol.</param>
        /// <returns>The Code 128 builder.</returns>
        /// <exception cref="ArgumentNullException">values was null.</exception>
        /// <exception cref="ArgumentException">values was empty.</exception>
        /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
        public static Code128 CreateC(IEnumerable<byte> values)
            => Create(values, 105);

        /// <summary>
        /// Creates with Start Code C.
        /// </summary>
        /// <param name="subtype">The sub-type.</param>
        /// <param name="values">The value collection of symbol.</param>
        /// <returns>The Code 128 builder.</returns>
        /// <exception cref="ArgumentNullException">values was null.</exception>
        /// <exception cref="ArgumentException">values was empty.</exception>
        /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
        public static Code128 Create(Subtypes subtype, IEnumerable<byte> values)
            => Create(values, subtype switch
            {
                Subtypes.A => 103,
                Subtypes.B => 104,
                Subtypes.C => 105,
                _ => throw new InvalidOperationException("subtype is not valid.")
            });

        /// <summary>
        /// Creates by boolean collection that white represented as false and black represented as true..
        /// </summary>
        /// <param name="values">The value collection of symbol.</param>
        /// <returns>The Code 128 builder.</returns>
        /// <exception cref="ArgumentNullException">values was null.</exception>
        /// <exception cref="ArgumentException">values was empty.</exception>
        /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
        public static Code128 Create(IEnumerable<bool> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values), "values should not be null.");
            var s = string.Join(string.Empty, values.Select(b => b ? '1' : '0')).Trim('0');
            if (s.Length < 25) throw new ArgumentException("The count of values is too less.", nameof(values));
            if (s.StartsWith("1100011101011")) s = new string(s.Reverse().ToArray());
            var col = new List<byte>();
            var first = s.Substring(0, 7) switch
            {
                "11010000100" => 103,
                "11010010000" => 104,
                "11010011100" => 105,
                _ => throw new InvalidOperationException("The start code is not valid.")
            };
            for (var i = 1; i < s.Length; i += 7)
            {
                try
                {
                    var item = s.Substring(i, 7);
                    var j = patterns.IndexOf(item);
                    if (j < 0) throw new InvalidOperationException($"Contains invalid symbol at position {i}.");
                    if (j > 102)
                    {
                        if (j == 106) break;
                        throw new InvalidOperationException($"Contains invalid symbol at position {i}.");
                    }
                }
                catch (ArgumentException)
                {
                }
            }

            return Create(col, (byte)first);
        }

        /// <summary>
        /// Creates by symbols.
        /// </summary>
        /// <param name="values">The value collection of symbol.</param>
        /// <returns>The Code 128 builder.</returns>
        /// <exception cref="ArgumentNullException">values was null.</exception>
        /// <exception cref="ArgumentException">values was empty.</exception>
        /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
        public static Code128 Create(IEnumerable<byte> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values), "values should not be null.");
            var first = values.First();
            if (first < 103 || first > 105) throw new InvalidOperationException("The start code is not valid.");
            return Create(values.Skip(1), first);
        }

        /// <summary>
        /// Creates with the specific start code.
        /// </summary>
        /// <param name="values">The value collection of symbol.</param>
        /// <param name="startCode">The value of start code.</param>
        /// <returns>The Code 128 builder.</returns>
        /// <exception cref="ArgumentNullException">values was null.</exception>
        /// <exception cref="ArgumentException">values was empty.</exception>
        /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
        private static Code128 Create(IEnumerable<byte> values, byte startCode)
        {
            if (values == null) throw new ArgumentNullException(nameof(values), "values should not be null.");
            var col = values.ToList();
            if (col.Count < 1) throw new ArgumentException("values should not be empty.", nameof(values));
            if (col[0] != startCode)
            {
                if (col[0] > 102) throw new InvalidOperationException($"The first value {col[0]} is not valid.");
                col.Insert(0, startCode);
            }

            if (col[col.Count - 1] != 106)
            {
                long check = startCode;
                for (var i = 1; i < col.Count; i++)
                {
                    check += col[i] * i;
                }

                col.Add((byte)(check % 103));
                col.Add(106);
            }

            var count = col.Count - 1;
            for (var i = 1; i < count; i++)
            {
                if (col[i] > 102) throw new InvalidOperationException($"The value {col[i]} at position {i} is not valid.");
            }

            return new Code128(col);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the symbol collection.
        /// </summary>
        /// <returns>A enumerator of the symbol collection.</returns>
        public IEnumerator<byte> GetEnumerator()
            => v.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the symbol collection.
        /// </summary>
        /// <returns>A enumerator of the symbol collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)v).GetEnumerator();
#pragma warning restore IDE0056
    }
}
