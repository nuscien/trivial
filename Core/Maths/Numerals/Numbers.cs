﻿// --------------------------------------------------------------------------------------------------------------------
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
            var i = TryParseNumericWord(s);
            if (i.HasValue) return i.Value;
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
            var i = TryParseNumericWord(s);
            if (i.HasValue) return i.Value;
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
            if (string.IsNullOrEmpty(s))
            {
                result = 0;
                return false;
            }

            s = s.Trim().ToLowerInvariant();
            if (radix < 2 || radix > 36 || string.IsNullOrEmpty(s))
            {
                result = default;
                return false;
            }

            if (radix == 10)
            {
                if (int.TryParse(s, out result)) return true;
                var i = TryParseNumericWord(s);
                if (i.HasValue)
                {
                    result = i.Value;
                    return true;
                }
            }

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
            if (string.IsNullOrEmpty(s))
            {
                result = 0;
                return false;
            }

            s = s.Trim().ToLowerInvariant();
            if (radix < 2 || radix > 36 || string.IsNullOrEmpty(s))
            {
                result = default;
                return false;
            }

            if (radix == 10)
            {
                if (long.TryParse(s, out result)) return true;
                var i = TryParseNumericWord(s);
                if (i.HasValue)
                {
                    result = i.Value;
                    return true;
                }
            }

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
        /// Parses a character to an integer.
        /// </summary>
        /// <param name="c">A character</param>
        /// <returns>An integer parsed.</returns>
        public static int ParseToInt32(char c)
            => TryParseToInt32(c) ?? throw new InvalidOperationException("The character is not a number.");

        /// <summary>
        /// Parses a character to an integer.
        /// </summary>
        /// <param name="c">A character</param>
        /// <param name="result">An integer parsed.</param>
        /// <returns>true if parses succeeded; otherwise, false.</returns>
        public static bool TryParseToInt32(char c, out int result)
        {
            var i = TryParseToInt32(c);
            result = i ?? default;
            return i.HasValue;
        }

        /// <summary>
        /// Parses a character to an integer.
        /// </summary>
        /// <param name="c">A character</param>
        /// <returns>An integer parsed.</returns>
        public static int? TryParseToInt32(char c)
        {
            return c switch
            {
                '0' => 0,
                '1' => 1,
                '2' => 2,
                '3' => 3,
                '4' => 4,
                '5' => 5,
                '6' => 6,
                '7' => 7,
                '8' => 8,
                '9' => 9,
                '零' or '〇' or '凌' or '空' or '无' or '無' or 'O' or '０' or 'ｏ' or 'ｏ' or '₀' or '⁰' or '○' or '蛋' or '圈' or '栋' or '영' or '공' => 0,
                '一' or '壹' or '①' or 'I' or 'i' or 'Ⅰ' or 'ⅰ' or '⒈' or '１' or '㈠' or '⑴' or '¹' or '₁' or 'a' or '单' or '幺' or '奇' or '独' or '甲' or '일' => 1,
                '二' or '贰' or '②' or 'Ⅱ' or 'ⅱ' or '⒉' or '２' or '㈡' or '⑵' or '²' or '₂' or 'に' or '两' or '兩' or '俩' or '倆' or '双' or '乙' or '이' or '둘' => 2,
                '三' or '叄' or '③' or 'Ⅲ' or 'ⅲ' or '⒊' or '３' or '㈢' or '⑶' or '³' or '₃' or '仨' or '丙' or '삼' or '셋' => 3,
                '四' or '肆' or '④' or 'Ⅳ' or 'ⅳ' or '⒋' or '４' or '㈣' or '⑷' or '⁴' or '₄' or '亖' or '丁' or '罒' or 'し' or '사' or '넷' => 4,
                '五' or '伍' or '⑤' or 'V' or 'v' or 'Ⅴ' or 'ⅴ' or '⒌' or '５' or '㈤' or '⑸' or '⁵' or '₅' or '戊' or 'ご' or '오' => 5,
                '六' or '陆' or '⑥' or 'Ⅵ' or 'ⅵ' or '⒍' or '６' or '㈥' or '⑹' or '⁶' or '₆' or '顺' or '己' or '육' => 6,
                '七' or '柒' or '⑦' or 'Ⅶ' or 'ⅶ' or '⒎' or '７' or '㈦' or '⑺' or '⁷' or '₇' or '拐' or '庚' or '칠' => 7,
                '八' or '捌' or '⑧' or 'Ⅷ' or 'ⅷ' or '⒏' or '８' or '㈧' or '⑻' or '⁸' or '₈' or '发' or '發' or '辛' or '팔' => 8,
                '九' or '玖' or '⑨' or 'Ⅸ' or 'ⅸ' or '⒐' or '９' or '㈨' or '⑼' or '⁹' or '₉' or '酒' or '壬' or '구' => 9,
                '十' or '拾' or '⑩' or 'Ⅹ' or 'X' or 'ⅹ' or '⒑' or '㈩' or '⑽' or '癸' or '십' or '열' => 10,
                'Ⅺ' or '⒒' or '⑾' => 11,
                'Ⅻ' or '⒓' or '⑿' => 12,
                '⒔' or '⒀' => 13,
                '⒕' or '⒁' => 14,
                '⒖' or '⒂' => 15,
                '⒗' or '⒃' => 16,
                '⒘' or '⒄' => 17,
                '⒙' or '⒅' => 18,
                '⒚' or '⒆' => 19,
                '廿' or '⒛' or '⒇' => 20,
                '卅' => 30,
                '卌' => 40,
                '圩' or 'L' => 50,
                '圆' => 60,
                '进' => 70,
                '枯' => 80,
                '枠' => 90,
                '百' or '佰' or 'C' or 'c' => 100,
                '皕' => 200,
                'D' => 500,
                '千' or '仟' or 'K' or 'k' => 1000,
                '万' or '萬' => 10000,
                '亿' or '億' => 100000000,
                'G' => 1000000000,
                _ => null
            };
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

        private static int? TryParseNumericWord(string word)
        {
            if (word.Length == 1) return TryParseToInt32(word[0]);
            return word.ToLowerInvariant() switch
            {
                "zero" or "nought" or "nill" or "れい" or "zéro" => 0,
                "one" or "first" or "いち" or "하나" or "un" or "une" => 1,
                "two" or "second" or "ii" or "deux" => 2,
                "three" or "third" or "iii" or "さん" or "trois" => 3,
                "four" or "fourth" or "forth" or "iv" or "quatre" => 4,
                "five" or "fifth" or "다섯" or "cinq" => 5,
                "six" or "sixth" or "vi" or "half dozen" or "半打" or "ろく" or "여섯" => 6,
                "seven" or "seventh" or "vii" or "しち" or "일곱" or "sept" => 7,
                "eight" or "eighth" or "viii" or "はち" or "여덟" or "huit" => 8,
                "nine" or "ninth" or "ix" or "きゅう" or "아홉" or "neuf" => 9,
                "ten" or "tenth" or "一十" or "壹拾" or "壹拾整" or "一零" or "一〇" or "１０" or "じゅう" or "dix" => 10,
                "eleven" or "eleventh" or "xi" or "一十一" or "十一" or "一一" or "１１" or "onze" => 11,
                "twelve" or "twelfth" or "xii" or "dozen" or "a dozen" or "一十二" or "十二" or "一二" or "一打" or "１２" or "douze" => 12,
                "thirteen" or "xiii" or "一十三" or "十三" or "一三" or "１３" or "treize" => 13,
                "fourteen" or "xiv" or "一十四" or "十四" or "一四" or "１４" or "quatorze" => 14,
                "fifteen" or "xv" or "一十五" or "十五" or "一五" or "１５" or "quinze" => 15,
                "sixteen" or "xvi" or "一十六" or "十六" or "一六" or "１６" or "seize" => 16,
                "seventeen" or "xvii" or "一十七" or "十七" or "一七" or "１７" or "dix-sept" => 17,
                "eighteen" or "xviii" or "一十八" or "十八" or "一八" or "１８" or "dix-huit" => 18,
                "ninteen" or "xix" or "一十九" or "十九" or "一九" or "１９" or "dix-neuf" => 19,
                "twenty" or "xx" or "二十" or "贰拾" or "贰拾整" or "二零" or "二〇" or "２０" or "vingt" => 20,
                "twenty-one" or "xxi" or "二十一" or "廿一" or "二一" or "２１" or "vingt et un" => 21,
                "twenty-two" or "xxii" or "二十二" or "廿二" or "二二" or "２２" or "vingt-deux" => 22,
                "twenty-three" or "xxiii" or "二十三" or "廿三" or "二三" or "２３" or "vingt-trois" => 23,
                "twenty-four" or "xxiv" or "二十四" or "廿四" or "二四" or "２４" or "vingt-quatre" => 24,
                "twenty-fiv" or "xxv" or "二十五" or "廿五" or "二五" or "２５" or "vingt-cinq" => 25,
                "thirty" or "xxx" or "三十" or "叄拾" or "叄拾整" or "三零" or "三〇" or "３０" or "trente" => 30,
                "forty" or "xl" or "四十" or "肆拾" or "肆拾整" or "四零" or "四〇" or "４０" or "quarante" => 40,
                "fifty" or "五十" or "伍拾" or "伍拾整" or "五零" or "五〇" or "半百" or "５０" or "cinquante" => 50,
                "sixty" or "六十" or "六零" or "六〇" or "６０" or "soixante" => 60,
                "seventy" or "七十" or "七零" or "七〇" or "７０" or "soixante-dix" => 70,
                "eighty" or "八十" or "八零" or "八〇" or "８０" or "quatre-vingts" => 80,
                "ninty" or "九十" or "九零" or "九〇" or "９０" or "quatre-vingt-dix" => 90,
                "一百" or "壹佰" or "壹佰整" or "1百" or "一零零" or "一〇〇" or "１００" or "hundred" or "a hundred" or "one hundred" or "ひゃく" or "いちひゃく" or "cent" => 100,
                "两百" or "二百" or "贰佰" or "贰佰整" or "2百" or "二零零" or "二〇〇" or "２００" or "two hundred" or "cc" or "deux cents" => 200,
                "五百" or "伍佰" or "伍佰整" or "5百" or "五零零" or "五〇〇" or "５００" or "five hundred" or "cinq cents" => 500,
                "1k" or "kilo" or "a kilo" or "thousand" or "a thousand" or "one thousand" or "一千" or "壹仟" or "壹仟整" or "1千" or "１０００" or "せん" or "いちせん" or "mille" or "millennium" => 1000,
                "一万" or "壹萬" or "壹万" or "壹萬整" or "壹万整" or "1万" or "10千" or "１００００" or "1w" or "10k" or "ten thousand" or "ten kilo" or "まん" or "いちまん" => 10000,
                _ => null
            };
        }
    }
}
