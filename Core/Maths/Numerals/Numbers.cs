// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Numbers.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The number symbols and functions.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The class for numbers.
    /// </summary>
    public static class Numbers
    {
        private const string num36 = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Sign of plus minus.
        /// </summary>
        public const string PlusMinusSign = "±";

        /// <summary>
        /// Sign of positive.
        /// </summary>
        public const string PositiveSign = "+";

        /// <summary>
        /// Sign of negative.
        /// </summary>
        public const string NegativeSign = "-";

        /// <summary>
        /// The infinite.
        /// </summary>
        public const string InfiniteSymbol = "∞";

        /// <summary>
        /// The positive infinite.
        /// </summary>
        public const string PositiveInfiniteSymbol = "+∞";

        /// <summary>
        /// The negative infinite.
        /// </summary>
        public const string NegativeInfiniteSymbol = "-∞";

        /// <summary>
        /// Number of 0.
        /// </summary>
        public const string NumberZero = "0";

        /// <summary>
        /// Number of 1/8.
        /// </summary>
        public const string NumberOneEighth = "⅛";

        /// <summary>
        /// Number of 1/4.
        /// </summary>
        public const string NumberQuarter = "¼";

        /// <summary>
        /// Number of 1/3.
        /// </summary>
        public const string NumberOneThrid = "⅓";

        /// <summary>
        /// Number of 3/8.
        /// </summary>
        public const string NumberThreeEighths = "⅜";

        /// <summary>
        /// Number of 1/2.
        /// </summary>
        public const string NumberHalf = "½";

        /// <summary>
        /// Number of 5/8.
        /// </summary>
        public const string NumberFiveEighths = "⅝";

        /// <summary>
        /// Number of 2/3.
        /// </summary>
        public const string NumberTwoThrids = "⅔";

        /// <summary>
        /// Number of 3/4.
        /// </summary>
        public const string NumberThreeQuarters = "¾";

        /// <summary>
        /// Number of 7/8.
        /// </summary>
        public const string NumberSevenEighths = "⅞";

        /// <summary>
        /// Number of 1.
        /// </summary>
        public const string NumberOne = "1";

        /// <summary>
        /// Number of 2.
        /// </summary>
        public const string NumberTwo = "2";

        /// <summary>
        /// Number of 3.
        /// </summary>
        public const string NumberThree = "3";

        /// <summary>
        /// Number of 4.
        /// </summary>
        public const string NumberFour = "4";

        /// <summary>
        /// Number of 5.
        /// </summary>
        public const string NumberFive = "5";

        /// <summary>
        /// Number of 6.
        /// </summary>
        public const string NumberSix = "6";

        /// <summary>
        /// Number of 7.
        /// </summary>
        public const string NumberSeven = "7";

        /// <summary>
        /// Number of 8.
        /// </summary>
        public const string NumberEight = "8";

        /// <summary>
        /// Number of 9.
        /// </summary>
        public const string NumberNine = "9";

        /// <summary>
        /// Number of 10 in hexadecimal.
        /// </summary>
        public const string NumberA = "A";

        /// <summary>
        /// Number of 11 in hexadecimal.
        /// </summary>
        public const string NumberB = "B";

        /// <summary>
        /// Number of 12 in hexadecimal.
        /// </summary>
        public const string NumberC = "C";

        /// <summary>
        /// Number of 13 in hexadecimal.
        /// </summary>
        public const string NumberD = "D";

        /// <summary>
        /// Number of 14 in hexadecimal.
        /// </summary>
        public const string NumberE = "E";

        /// <summary>
        /// Number of 15 in hexadecimal.
        /// </summary>
        public const string NumberF = "F";

        /// <summary>
        /// The symbol of PI.
        /// </summary>
        public const string PiSymbol = "π";

        /// <summary>
        /// The symbol of natural logarithmic base.
        /// </summary>
        public const string NaturalLogarithmicBaseSymbol = "e";

        /// <summary>
        /// The symbol of complex base.
        /// </summary>
        public const string ComplexBaseSymbol = "i";

        /// <summary>
        /// The sign of empty set of nothing.
        /// </summary>
        public const string EmptySetSymbol = "ø";

        /// <summary>
        /// Converts a number to a specific positional notation format string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <returns>A string of the number in the specific positional notation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
        public static string ToPositionalNotationString(short value, int radix)
        {
            return ToPositionalNotationString((long)value, radix);
        }

        /// <summary>
        /// Converts a number to a specific positional notation format string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <returns>A string of the number in the specific positional notation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
        public static string ToPositionalNotationString(int value, int radix)
        {
            return ToPositionalNotationString((long)value, radix);
        }

        /// <summary>
        /// Converts a number to a specific positional notation format string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <returns>A string of the number in the specific positional notation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
        public static string ToPositionalNotationString(uint value, int radix)
        {
            return ToPositionalNotationString((long)value, radix);
        }

        /// <summary>
        /// Converts a number to a specific positional notation format string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <returns>A string of the number in the specific positional notation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
        public static string ToPositionalNotationString(long value, int radix)
        {
            if (radix < 2 || radix > 36) throw new ArgumentOutOfRangeException(nameof(radix), "radix should be in 2-36.");
            var integerStr = string.Empty;
            var integerPart = Math.Abs(value);
            if (integerPart == 0) return "0";
            while (integerPart != 0)
            {
                integerStr = num36[(int)(integerPart % radix)] + integerStr;
                integerPart /= radix;
            }

            if (value < 0) return "-" + integerStr;
            return integerStr;
        }

        /// <summary>
        /// Converts a number to a specific positional notation format string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <returns>A string of the number in the specific positional notation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
        public static string ToPositionalNotationString(float value, int radix)
        {
            return ToPositionalNotationString((double)value, radix);
        }

        /// <summary>
        /// Converts a number to a specific positional notation format string.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <returns>A string of the number in the specific positional notation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
        public static string ToPositionalNotationString(double value, int radix)
        {
            if (radix < 2 || radix > 36) throw new ArgumentOutOfRangeException(nameof(radix), "radix should be in 2-36.");
            var integerStr = string.Empty;
            var fractionalStr = string.Empty;
            var integerPart = Math.Abs((long)value);
            var fractionalPart = Math.Abs(value) - integerPart;
            if (integerPart == 0)
            {
                integerStr = "0";
            }

            while (integerPart != 0)
            {
                integerStr = num36[(int)(integerPart % radix)] + integerStr;
                integerPart /= radix;
            }

            for (int i = 0; i < 10; i++)
            {
                if (fractionalPart == 0)
                {
                    break;
                }

                var pos = (int)(fractionalPart * radix);
                if (pos < 35 && Math.Abs(pos + 1 - fractionalPart * radix) < 0.00000000001)
                {
                    fractionalStr += num36[pos + 1];
                    break;
                }

                fractionalStr += num36[pos];
                fractionalPart = fractionalPart * radix - pos;
            }

            while (fractionalStr.Length > 0 && fractionalStr.LastIndexOf('0') == (fractionalStr.Length - 1))
                fractionalStr = fractionalStr.Remove(fractionalStr.Length - 1);

            var str = new StringBuilder();
            if (value < 0) str.Append('-');
            str.Append(integerStr);
            if (!string.IsNullOrEmpty(fractionalStr))
            {
                str.Append('.');
                str.Append(fractionalStr);
            }

            return str.ToString();
        }

        /// <summary>
        /// Parses a string to a number.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <exception cref="ArgumentNullException">s was null.</exception>
        /// <exception cref="ArgumentException">s was empty or consists only of white-space characters..</exception>
        /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
        /// <exception cref="FormatException">s was in an incorrect format.</exception>
        /// <returns>A number parsed.</returns>
        public static short ParseToInt16(string s, int radix)
        {
            var result = ParseToInt32(s, radix);
            if (result >= short.MinValue && result <= short.MaxValue)
                return (short)result;
            throw new FormatException("s was too small or too large.", new OverflowException("s was too small or too large."));
        }

        /// <summary>
        /// Parses a string to a number.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <exception cref="ArgumentNullException">s was null.</exception>
        /// <exception cref="ArgumentException">s was empty or consists only of white-space characters..</exception>
        /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
        /// <exception cref="FormatException">s was in an incorrect format.</exception>
        /// <returns>A number parsed.</returns>
        public static int ParseToInt32(string s, int radix)
        {
            if (s == null) throw new ArgumentNullException(nameof(s), "s should not be null.");
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException("s should not be empty or consists only of white-space characters.", nameof(s));
            if (radix < 2 || radix > 36) throw new ArgumentOutOfRangeException(nameof(radix), "radix should be in 2-36.");
            if (TryParseToInt32(s, radix, out var result)) return result;
            var message = $"{nameof(s)} is incorrect. It should be in base {radix} number format.";
            throw new FormatException(message, new ArgumentException(message, nameof(s)));
        }

        /// <summary>
        /// Parses a string to a number.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <exception cref="ArgumentNullException">s was null.</exception>
        /// <exception cref="ArgumentException">s was empty or consists only of white-space characters..</exception>
        /// <exception cref="ArgumentOutOfRangeException">radix was less than 2 or greater than 36.</exception>
        /// <exception cref="FormatException">s was in an incorrect format.</exception>
        /// <returns>A number parsed.</returns>
        public static long ParseToInt64(string s, int radix)
        {
            if (s == null) throw new ArgumentNullException(nameof(s), "s should not be null.");
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException("s should not be empty or consists only of white-space characters.", nameof(s));
            if (radix < 2 || radix > 36) throw new ArgumentOutOfRangeException(nameof(radix), "radix should be in 2-36.");
            if (TryParseToInt64(s, radix, out var result)) return result;
            var message = $"{nameof(s)} is incorrect. It should be in base {radix} number format.";
            throw new FormatException(message, new ArgumentException(message, nameof(s)));
        }

        /// <summary>
        /// Tries to parse a string to a number.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if parse succeeded; otherwise, false.</returns>
        public static bool TryParseToInt16(string s, int radix, out short result)
        {
            if (TryParseToInt32(s, radix, out var i) && i >= short.MinValue && i <= short.MaxValue)
            {
                result = (short)i;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Tries to parse a string to a number.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if parse succeeded; otherwise, false.</returns>
        public static bool TryParseToInt32(string s, int radix, out int result)
        {
            s = s.Trim().ToLowerInvariant();
            if (radix < 2 || radix > 36 || string.IsNullOrEmpty(s))
            {
                result = default;
                return false;
            }

            if (radix == 10 && int.TryParse(s, out result)) return true;
            var num = 0;
            var pos = 0;
            var neg = false;
            if (radix == 16)
            {
                if (s.StartsWith("0x-") || s.StartsWith("-0x") || s.StartsWith("&h-") || s.StartsWith("-&h"))
                {
                    pos += 3;
                    neg = true;
                }
                else if (s.StartsWith("0x") || s.StartsWith("&h"))
                {
                    pos += 2;
                }
                else if (s.StartsWith("x-") || s.StartsWith("-x"))
                {
                    pos += 2;
                    neg = true;
                }
                else if (s.StartsWith("x"))
                {
                    pos++;
                }
            }
            else if (s[0] == '-')
            {
                neg = true;
                pos++;
            }

            for (; pos < s.Length; pos++)
            {
                var c = s[pos];
                num *= radix;
                var i = num36.IndexOf(c);
                if (i < 0)
                {
                    if (c == ' ' || c == '_' || c == ',') continue;
                    if (c == '.' || c == '\t' || c == '\r' || c == '\n' || c == '\0') break;
                    result = default;
                    return false;
                }
                else if (i >= radix || num < 0)
                {
                    result = default;
                    return false;
                }

                num += i;
            }

            result = neg ? -num : num;
            return true;
        }

        /// <summary>
        /// Tries to parse a string to a number.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="radix">The positional notation. Should be an integer in 2-36.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if parse succeeded; otherwise, false.</returns>
        public static bool TryParseToInt64(string s, int radix, out long result)
        {
            s = s.Trim().ToLowerInvariant();
            if (radix < 2 || radix > 36 || string.IsNullOrEmpty(s))
            {
                result = default;
                return false;
            }

            if (radix == 10 && long.TryParse(s, out result)) return true;
            var num = 0L;
            var pos = 0;
            var neg = false;
            if (radix == 16)
            {
                if (s.StartsWith("0x-") || s.StartsWith("-0x") || s.StartsWith("&h-") || s.StartsWith("-&h"))
                {
                    pos += 3;
                    neg = true;
                }
                else if (s.StartsWith("0x") || s.StartsWith("&h"))
                {
                    pos += 2;
                }
                else if (s.StartsWith("x-") || s.StartsWith("-x"))
                {
                    pos += 2;
                    neg = true;
                }
                else if (s.StartsWith("x"))
                {
                    pos++;
                }
            }
            else if (s[0] == '-')
            {
                neg = true;
                pos++;
            }

            for (; pos < s.Length; pos++)
            {
                var c = s[pos];
                num *= radix;
                var i = num36.IndexOf(c);
                if (i < 0)
                {
                    if (c == ' ' || c == '_' || c == ',') continue;
                    if (c == '.' || c == '\t' || c == '\r' || c == '\n' || c == '\0') break;
                    result = default;
                    return false;
                }
                else if (i >= radix || num < 0)
                {
                    result = default;
                    return false;
                }

                num += i;
            }

            result = neg ? -num : num;
            return true;
        }

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="numbers">The number localization instance.</param>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public static string ToString(this IIntegerLocalization numbers, int number, bool digitOnly = false)
        {
            if (numbers == null) return number.ToString();
            return numbers.ToString((long)number, digitOnly);
        }

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="numbers">The number localization instance.</param>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public static string ToString(this IIntegerLocalization numbers, uint number, bool digitOnly = false)
        {
            if (numbers == null) return number.ToString();
            return numbers.ToString((ulong)number, digitOnly);
        }

        /// <summary>
        /// Gets the exponent string.
        /// </summary>
        /// <param name="value">The exponent number.</param>
        /// <returns>A string about the exponent.</returns>
        internal static string ToExponentString(int value)
        {
            return value
                .ToString(CultureInfo.InvariantCulture)
                .Replace("1", "¹")
                .Replace("2", "²")
                .Replace("3", "³")
                .Replace("4", "⁴")
                .Replace("5", "⁵")
                .Replace("6", "⁶")
                .Replace("7", "⁷")
                .Replace("8", "⁸")
                .Replace("9", "⁹")
                .Replace("0", "⁰");
        }

        internal static (long, string, int) SplitNumber(double value)
        {
            const string zero = "0";
            var numStr = value.ToString(CultureInfo.InvariantCulture);
            var dotPos = numStr.IndexOf(".");
            if (dotPos < 0)
            {
                dotPos = numStr.IndexOf("E");
                if (dotPos < 0) return ((long)value, string.Empty, 0);
                var integerPart2 = numStr.Substring(0, dotPos);
                var exponentialPart2 = numStr.Substring(dotPos + 1);
                return (long.Parse(integerPart2, CultureInfo.InvariantCulture), string.Empty, int.Parse(exponentialPart2, CultureInfo.InvariantCulture));
            }

            var integerPart = numStr.Substring(0, dotPos);
            if (integerPart.Length == 0) integerPart = zero;
            var integerPartNum = long.Parse(integerPart, CultureInfo.InvariantCulture);
            var fractionalPart = numStr.Substring(dotPos + 1);
            var ePos = fractionalPart.IndexOf("E");
            if (ePos < 0) return (integerPartNum, fractionalPart, 0);
            var exponentialPart = fractionalPart.Substring(ePos + 1);
            fractionalPart = fractionalPart.Substring(0, ePos);
            if (exponentialPart.Length == 0) exponentialPart = zero;
            return (integerPartNum, fractionalPart, int.Parse(exponentialPart, CultureInfo.InvariantCulture));
        }
    }
}
