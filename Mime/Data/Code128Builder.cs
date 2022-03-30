using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Data;

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
    /// <param name="values">The byte array of symbol.</param>
    private Code128(List<byte> values)
    {
        v = values ?? new List<byte>();
    }

    /// <summary>
    /// Gets the count of symbol.
    /// </summary>
    public int Count => v.Count;

    /// <summary>
    /// Gets the start sub-type.
    /// </summary>
    public Subtypes Subtype => v.Count > 3 ? v[0] switch
    {
        103 => Subtypes.A,
        104 => Subtypes.B,
        105 => Subtypes.C,
        _ => (Subtypes)0
    } : 0;

    /// <summary>
    /// Gets the checksum; or 255, if not available.
    /// </summary>
    public byte Checksum => v.Count > 3 ? v[v.Count - 2] : (byte)255;

    /// <summary>
    /// Gets the value of the specific symbol.
    /// </summary>
    /// <param name="index">The index of symbol.</param>
    /// <returns>The value of the symbol.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public byte this[int index] => v[index];

    /// <summary>
    /// Returns symbol values without start code, checksum, and stop code. 
    /// </summary>
    /// <returns>A collection of symbol value without start code, checksum, and stop code.</returns>
    public IEnumerable<byte> TakeData()
        => v.Count > 3 ? v.Skip(1).Take(v.Count - 3) : new List<byte>();

    /// <summary>
    /// Converts to boolean list.
    /// White represented as false, black represented as true.
    /// </summary>
    /// <returns>The boolean list of barcode.</returns>
    public List<bool> ToBarcode()
    {
        var list = v.SelectMany(GetPattern).ToList();
        if (list.Count > 10)
        {
            list.Add(true);
            list.Add(true);
        }

        return list;
    }

    /// <summary>
    /// Gets all information of application identifier and its data value.
    /// </summary>
    /// <returns>A collection with application identifiers and their data value.</returns>
    public IEnumerable<string> GetAiData()
    {
        if (v.Count < 4) yield break;
        var subtype = v[0] switch
        {
            103 => Subtypes.A,
            104 => Subtypes.B,
            105 => Subtypes.C,
            _ => (Subtypes)0
        };
        if (subtype == 0) yield break;
        var sb = new StringBuilder();
        var subtype2 = subtype;
        var high = false;
        var rec = false;
        foreach (var b in v.Skip(1).Take(v.Count - 3))
        {
            if (b > 101)
            {
                if (b > 102) break;
                rec = true;
                if (sb.Length > 0)
                {
                    yield return sb.ToString();
                    sb.Clear();
                }

                continue;
            }

            var st = subtype;
            subtype = subtype2;
            if (st == Subtypes.C)
            {
                high = false;
                if (b < 100 || b == 102)
                {
                    if (rec) sb.Append(b.ToString("00"));
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
                    if (rec) sb.Append(c);
                }
                else
                {
                    if (rec) sb.Append(ToString(st, b));
                }

                continue;
            }

            switch (b)
            {
                case 96:
                    rec = false;
                    if (sb.Length > 0)
                    {
                        yield return sb.ToString();
                        sb.Clear();
                    }

                    break;
                case 97:
                    rec = false;
                    if (sb.Length > 0)
                    {
                        yield return sb.ToString();
                        sb.Clear();
                    }

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

        if (sb.Length > 0) yield return sb.ToString();
    }

    /// <summary>
    /// Returns a string that represents the barcode in stroke path for SVG or XAML.
    /// </summary>
    /// <param name="height">The bar height.</param>
    /// <returns>A stroke path string that represents the barcode.</returns>
    public string ToPathString(int height = 40)
    {
        var sb = new StringBuilder();
        var i = 9;
        sb.Append("M0,0 ");
        foreach (var b in v)
        {
            if (b > 106) continue;
            var binary = patterns[b];
            foreach (var c in binary)
            {
                i++;
                if (c == '1')
                    sb.Append($"M{i},0 L{i},{height} ");
            }
        }

        sb.Append($"M{i + 1},{0} L{i + 1},{height} M{i + 2},{0} L{i + 2},{height} M{i + 12},0");
        return sb.ToString();
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => ToString(Formats.Regular);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <param name="format">The output string format.</param>
    /// <returns>A string that represents the current object.</returns>
    public string ToString(Formats format)
    {
        if (v.Count < 4) return string.Empty;
        var subtype = v[0] switch
        {
            103 => Subtypes.A,
            104 => Subtypes.B,
            105 => Subtypes.C,
            _ => (Subtypes)0
        };
        if (subtype == 0) return string.Empty;
        var sb = new StringBuilder();
        switch (format)
        {
            case Formats.Regular:
            case Formats.Text:
                break;
            case Formats.Hex:
                foreach (var b in v)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            case Formats.Values:
                foreach (var b in v.Take(v.Count - 2))
                {
                    if (subtype == Subtypes.C && b < 100)
                    {
                        sb.Append(b);
                        sb.Append(' ');
                        continue;
                    }

                    switch (b)
                    {
                        case 96:
                            sb.Append("[FNC3] ");
                            break;
                        case 97:
                            sb.Append("[FNC2] ");
                            break;
                        case 98:
                            sb.Append(subtype == Subtypes.A ? "[Shift B] " : "[Shift A] ");
                            break;
                        case 99:
                            subtype = Subtypes.C;
                            sb.Append("[Code C] ");
                            break;
                        case 100:
                            if (subtype == Subtypes.B)
                            {
                                sb.Append("[FNC4] ");
                            }
                            else
                            {
                                subtype = Subtypes.B;
                                sb.Append("[Code B] ");
                            }

                            break;
                        case 101:
                            if (subtype == Subtypes.A)
                            {
                                sb.Append("[FNC4] ");
                            }
                            else
                            {
                                subtype = Subtypes.A;
                                sb.Append("[Code A] ");
                            }

                            break;
                        case 102:
                            sb.Append("[FNC1] ");
                            break;
                        case 103:
                            sb.Append("[Start A] ");
                            break;
                        case 104:
                            sb.Append("[Start B] ");
                            break;
                        case 105:
                            sb.Append("[Start C] ");
                            break;
                        default:
                            sb.Append(b);
                            sb.Append(' ');
                            break;
                    }
                }

                sb.Append($"[Check symbol {v[v.Count - 2]:g}] [Stop]");
                return sb.ToString().Trim();
            case Formats.Barcode:
                return ToBarcodeString();
            case Formats.Path:
                return ToPathString(40);
        }

        var appendFunc = format == Formats.Regular;
        var subtype2 = subtype;
        var high = false;
        var high2 = false;
        foreach (var b in v.Skip(1).Take(v.Count - 3))
        {
            if (b > 101)
            {
                if (b > 102) break;
                if (appendFunc) sb.Append("[FNC1]");
                high = high2 = false;
                continue;
            }

            var st = subtype;
            subtype = subtype2;
            if (st == Subtypes.C)
            {
                high = high2 = false;
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
                high = high2 = false;
                continue;
            }

            if (b < 96)
            {
                if (high)
                {
                    high = high2;
                    var c = (char)(ToString(st, b).FirstOrDefault() + 128);
                    sb.Append(c);
                }
                else
                {
                    high = high2;
                    sb.Append(ToString(st, b));
                }

                continue;
            }

            switch (b)
            {
                case 96:
                    if (appendFunc) sb.Append("[FNC3]");
                    break;
                case 97:
                    if (appendFunc) sb.Append("[FNC2]");
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
                        if (high != high2) high2 = high;
                        else high = !high;
                        continue;
                    }

                    subtype = subtype2 = Subtypes.B;
                    break;
                case 101:
                    if (isA)
                    {
                        if (high != high2) high2 = high;
                        else high = !high;
                        continue;
                    }

                    subtype = subtype2 = Subtypes.A;
                    break;
            }

            high = high2;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts to a string.
    /// </summary>
    /// <returns>The barcode string.</returns>
    public string ToBarcodeString()
    {
        var sb = new StringBuilder();
        foreach (var b in v)
        {
            sb.Append(GetPattern(b));
        }

        if (sb.Length > 10) sb.Append("11");
        return sb.ToString();
    }

    /// <summary>
    /// Converts to a string.
    /// </summary>
    /// <param name="black">The value of black represented.</param>
    /// <param name="white">The value of white represented.</param>
    /// <returns>The barcode string.</returns>
    public string ToBarcodeString(char black, char white)
        => string.Join(string.Empty, ToBarcode().Select(ele => ele ? black : white));

    /// <summary>
    /// Converts to a string.
    /// </summary>
    /// <param name="selector">The selector to convert boolean array to a string. Per boolean value, white represented as false, black represented as true.</param>
    /// <returns>The barcode string.</returns>
    public string ToBarcodeString(Func<bool, char> selector)
        => selector != null ? string.Join(string.Empty, ToBarcode().Select(selector)) : ToBarcodeString();

    /// <summary>
    /// Converts to a string.
    /// </summary>
    /// <param name="selector">The selector to convert boolean array to a string. Per boolean value, white represented as false, black represented as true.</param>
    /// <returns>The barcode string.</returns>
    public string ToBarcodeString(Func<bool, int, char> selector)
        => selector != null ? string.Join(string.Empty, ToBarcode().Select(selector)) : ToBarcodeString();

    /// <summary>
    /// Gets all sub-types used in value.
    /// </summary>
    /// <returns>A collection of sub-type.</returns>
    public IEnumerable<Subtypes> GetSubtypesUsed()
    {
        var subtype = Subtype;
        yield return subtype;
        foreach (var b in v.Skip(1).Take(v.Count - 3))
        {
            if (b < 98) continue;
            if (b > 102) break;
            switch (b)
            {
                case 99:
                    if (subtype == Subtypes.C) break;
                    subtype = Subtypes.C;
                    yield return subtype;
                    break;
                case 100:
                    if (subtype == Subtypes.B) break;
                    subtype = Subtypes.B;
                    yield return subtype;
                    break;
                case 101:
                    if (subtype == Subtypes.A) break;
                    subtype = Subtypes.A;
                    yield return subtype;
                    break;
            }
        }
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

    /// <summary>
    /// Creates with Start Code A.
    /// </summary>
    /// <param name="values">The value collection of symbol.</param>
    /// <returns>The Code 128 instance.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    public static Code128 CreateA(IEnumerable<byte> values)
        => Create(values, 103);

    /// <summary>
    /// Creates with Start Code A.
    /// </summary>
    /// <param name="s">The string value collection of symbol.</param>
    /// <returns>The Code 128 instance.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    public static Code128 CreateA(string s)
        => Create(Subtypes.A, s);

    /// <summary>
    /// Creates with Start Code B.
    /// </summary>
    /// <param name="values">The value collection of symbol.</param>
    /// <returns>The Code 128 instance.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    public static Code128 CreateB(IEnumerable<byte> values)
        => Create(values, 104);

    /// <summary>
    /// Creates with Start Code B.
    /// </summary>
    /// <param name="s">The string value collection of symbol.</param>
    /// <returns>The Code 128 instance.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    public static Code128 CreateB(string s)
        => Create(Subtypes.B, s);

    /// <summary>
    /// Creates with Start Code C.
    /// </summary>
    /// <param name="values">The value collection of symbol.</param>
    /// <returns>The Code 128 instance.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    public static Code128 CreateC(IEnumerable<byte> values)
        => Create(values, 105);

    /// <summary>
    /// Creates with Start Code C.
    /// </summary>
    /// <param name="s">The string value collection of symbol.</param>
    /// <returns>The Code 128 instance.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    public static Code128 CreateC(string s)
        => Create(Subtypes.C, s);

    /// <summary>
    /// Creates with Start Code C.
    /// </summary>
    /// <param name="s">The integer value of symbol.</param>
    /// <returns>The Code 128 instance.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    public static Code128 CreateC(long s)
        => Create(Subtypes.C, s.ToString("g"));

    /// <summary>
    /// Creates GS1-128 code.
    /// </summary>
    /// <param name="ai">The application identifier.</param>
    /// <param name="data">The data value.</param>
    /// <returns>The Code 128 instance.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">ai was less than 0.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    public static Code128 CreateGs1(int ai, string data)
    {
        if (ai < 0) throw new ArgumentOutOfRangeException(nameof(ai), "ai should not be less than zero.");
        var col = new List<byte> { 102 };
        Fill(col, Subtypes.C, $"{ai:00}{data}");
        return Create(Subtypes.C, col);
    }

    /// <summary>
    /// Creates GS1-128 code.
    /// </summary>
    /// <param name="part1">The string 1 with application identifier and its data value.</param>
    /// <param name="part2">The string 2 with application identifier and its data value.</param>
    /// <param name="part3">The string 3 with application identifier and its data value.</param>
    /// <param name="part4">The string 4 with application identifier and its data value.</param>
    /// <param name="part5">The string 5 with application identifier and its data value.</param>
    /// <param name="part6">The string 6 with application identifier and its data value.</param>
    /// <param name="part7">The string 7 with application identifier and its data value.</param>
    /// <param name="part8">The string 8 with application identifier and its data value.</param>
    /// <returns>The Code 128 instance.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">ai was less than 0.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    public static Code128 CreateGs1(string part1, string part2, string part3 = null, string part4 = null, string part5 = null, string part6 = null, string part7 = null, string part8 = null)
    {
        var col = new List<byte>();
        var subtype = Subtypes.C;
        if (!string.IsNullOrEmpty(part1))
        {
            col.Add(102);
            subtype = Fill(col, Subtypes.C, part1);
        }

        if (!string.IsNullOrEmpty(part2))
        {
            if (subtype != Subtypes.C) col.Add(99);
            col.Add(102);
            subtype = Fill(col, Subtypes.C, part2);
        }

        if (!string.IsNullOrEmpty(part3))
        {
            if (subtype != Subtypes.C) col.Add(99);
            col.Add(102);
            subtype = Fill(col, Subtypes.C, part3);
        }

        if (!string.IsNullOrEmpty(part4))
        {
            if (subtype != Subtypes.C) col.Add(99);
            col.Add(102);
            subtype = Fill(col, Subtypes.C, part4);
        }

        if (!string.IsNullOrEmpty(part5))
        {
            if (subtype != Subtypes.C) col.Add(99);
            col.Add(102);
            subtype = Fill(col, Subtypes.C, part5);
        }

        if (!string.IsNullOrEmpty(part6))
        {
            if (subtype != Subtypes.C) col.Add(99);
            col.Add(102);
            subtype = Fill(col, Subtypes.C, part6);
        }

        if (!string.IsNullOrEmpty(part7))
        {
            if (subtype != Subtypes.C) col.Add(99);
            col.Add(102);
            subtype = Fill(col, Subtypes.C, part7);
        }

        if (!string.IsNullOrEmpty(part8))
        {
            if (subtype != Subtypes.C) col.Add(99);
            col.Add(102);
            Fill(col, Subtypes.C, part8);
        }

        return Create(Subtypes.C, col);
    }

    /// <summary>
    /// Creates with the specific sub-type.
    /// </summary>
    /// <param name="subtype">The sub-type to use.</param>
    /// <param name="values">The value collection of symbol.</param>
    /// <returns>The Code 128 instance.</returns>
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
    /// <returns>The Code 128 instance.</returns>
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
    /// <returns>The Code 128 instance.</returns>
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
    /// Creates with the specific sub-type.
    /// </summary>
    /// <param name="subtype">The sub-type of start code.</param>
    /// <param name="s">The string to encode.</param>
    /// <returns>The Code 128 instance.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    public static Code128 Create(Subtypes subtype, string s)
    {
        var col = new List<byte>();
        Fill(col, subtype, s);
        return Create(subtype, col);
    }

    /// <summary>
    /// Fills symbols.
    /// </summary>
    /// <param name="col">The byte array of symbol value to fill.</param>
    /// <param name="subtype">The sub-type of start code.</param>
    /// <param name="s">The string to encode.</param>
    /// <returns>The subtype of last one.</returns>
    /// <exception cref="ArgumentNullException">values was null.</exception>
    /// <exception cref="ArgumentException">values was empty.</exception>
    /// <exception cref="InvalidOperationException">Any value is invalid.</exception>
    private static Subtypes Fill(List<byte> col, Subtypes subtype, string s)
    {
        if (s == null) return subtype;
        var bytes = Encoding.ASCII.GetBytes(s);
        byte? reserved = null;
        foreach (var b in bytes)
        {
            if (subtype == Subtypes.A)
            {
                if (reserved.HasValue)
                {
                    col.Add((byte)(reserved.Value + 16));
                    reserved = null;
                }

                if (b > 31 && b < 96)
                {
                    col.Add((byte)(b - 32));
                }
                else if (b < 32)
                {
                    col.Add((byte)(b + 64));
                }
                else if (b > 127)
                {
                    col.Add(101);
                    var b2 = b - 128;
                    if (b2 > 31 && b2 < 96)
                    {
                        col.Add((byte)(b2 - 32));
                    }
                    else if (b2 < 32)
                    {
                        col.Add((byte)(b2 + 64));
                    }
                    else
                    {
                        subtype = Subtypes.B;
                        col.Add(100);
                        col.Add((byte)(b2 - 32));
                    }
                }
                else
                {
                    subtype = Subtypes.B;
                    col.Add(100);
                    col.Add((byte)(b - 32));
                }
            }
            else if (subtype == Subtypes.B)
            {
                if (reserved.HasValue)
                {
                    col.Add((byte)(reserved.Value + 16));
                    reserved = null;
                }

                if (b > 31 && b < 128)
                {
                    col.Add((byte)(b - 32));
                }
                else if (b > 127)
                {
                    col.Add(100);
                    var b2 = b - 128;
                    if (b2 > 31)
                    {
                        col.Add((byte)(b - 32));
                    }
                    else
                    {
                        subtype = Subtypes.A;
                        col.Add(101);
                        col.Add((byte)(b2 + 64));
                    }
                }
                else
                {
                    subtype = Subtypes.A;
                    col.Add(101);
                    col.Add((byte)(b + 64));
                }
            }
            else if (subtype == Subtypes.C)
            {
                if (b < 48)
                {
                    if (b < 32)
                    {
                        subtype = Subtypes.A;
                        col.Add(101);
                        col.Add((byte)(b + 64));
                    }
                    else
                    {
                        subtype = Subtypes.B;
                        col.Add(100);
                        col.Add((byte)(b - 32));
                    }
                }
                else if (b > 57)
                {
                    if (b > 159)
                    {
                        subtype = Subtypes.A;
                        col.Add(100);
                        col.Add(100);
                        col.Add((byte)(b - 160));
                    }
                    else if (b > 127)
                    {
                        subtype = Subtypes.A;
                        col.Add(101);
                        col.Add(101);
                        col.Add((byte)(b - 64));
                    }
                    else
                    {
                        subtype = Subtypes.B;
                        col.Add(100);
                        col.Add((byte)(b - 32));
                    }
                }
                else if (reserved.HasValue)
                {
                    col.Add((byte)(reserved.Value * 10 + b - 48));
                    reserved = null;
                }
                else
                {
                    reserved = (byte)(b - 48);
                }
            }
            else
            {
                throw new InvalidOperationException($"The subtype is not valid.");
            }
        }

        if (reserved.HasValue)
        {
            if (subtype == Subtypes.C)
                col.Add(101);
            col.Add((byte)(reserved.Value + 16));
        }

        return subtype;
    }

    /// <summary>
    /// Creates with the specific start code.
    /// </summary>
    /// <param name="values">The value collection of symbol.</param>
    /// <param name="startCode">The value of start code.</param>
    /// <returns>The Code 128 instance.</returns>
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
    /// Adds all code 128 instances into one.
    /// </summary>
    /// <param name="col">The collection to combine.</param>
    /// <returns>A code 128 instance combined.</returns>
    public static Code128 Join(IEnumerable<Code128> col)
    {
        if (col is null) return null;
        var list = col.Where(ele => ele != null && ele.Count > 3).ToList();
        if (list.Count < 2) return list.Count == 1 ? list[0] : null;
        var first = list[0];
        var subtype = first.GetSubtypesUsed().Last();
        var bytes = first.TakeData().ToList();
        foreach (var c in list.Skip(1))
        {
            var rightSubtype = c.Subtype;
            if (subtype != rightSubtype) bytes.Add(rightSubtype switch
            {
                Subtypes.A => 101,
                Subtypes.B => 100,
                _ => 99
            });
            bytes.AddRange(c.TakeData());
            subtype = c.GetSubtypesUsed().Last();
        }

        return Create(first.Subtype, bytes);
    }

    /// <summary>
    /// Adds.
    /// leftValue + rightValue
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A result after Add operation.</returns>
    public static Code128 operator +(Code128 leftValue, Code128 rightValue)
    {
        if (rightValue is null) return leftValue;
        if (leftValue is null) return rightValue;
        var subtype = leftValue.GetSubtypesUsed().Last();
        var bytes = leftValue.TakeData().ToList();
        var rightSubtype = rightValue.Subtype;
        if (subtype != rightSubtype) bytes.Add(rightSubtype switch
        {
            Subtypes.A => 101,
            Subtypes.B => 100,
            _ => 99
        });
        bytes.AddRange(rightValue.TakeData());
        return Create(leftValue.Subtype, bytes);
    }
#pragma warning restore IDE0056
}
