// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arithmetic\Basic.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The basic arithmetic functions.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The utility for arithmetic.
    /// </summary>
    public static partial class Arithmetic
    {
        private const string num36 = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Gets a result of factorial for a specific number.
        /// </summary>
        /// <param name="value">A number to calculate.</param>
        /// <returns>A number of result.</returns>
        /// <example>
        /// <code>
        /// var factorialNum = Arithmetic.Factorial(20); // => 2432902008176640000
        /// </code>
        /// </example>
        public static long Factorial(uint value)
        {
            if (value < 2) return 1;
            var resultNum = (long)value;
            for (uint step = 2; step < value; step++)
            {
                resultNum *= step;
            }

            return resultNum;
        }

        /// <summary>
        /// Gets a result of factorial for a specific number.
        /// </summary>
        /// <param name="value">A number to calculate.</param>
        /// <returns>A number of result.</returns>
        /// <example>
        /// <code>
        /// var factorialNum = Arithmetic.FactorialApproximate(100); // 9.33262154439442e+157
        /// </code>
        /// </example>
        public static double FactorialApproximate(uint value)
        {
            if (value < 2) return 1;
            var resultNum = (double)value;
            for (double step = 2; step < value; step++)
            {
                resultNum *= step;
            }

            return resultNum;
        }

        /// <summary>
        /// Calculates the value times 1024 of the specific power.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="e">The exponential.</param>
        /// <returns>The result calculated.</returns>
        /// <remarks>You can use this to calculate such as 80K or 4M.</remarks>
        public static long Times1024(int value, int e = 1)
        {
            return value * (long)Math.Pow(1024, e);
        }

        /// <summary>
        /// Calculates the value times 1024 of the specific power.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="e">The exponential.</param>
        /// <returns>The result calculated.</returns>
        /// <remarks>You can use this to calculate such as 80K or 4M.</remarks>
        public static double Times1024(long value, int e = 1)
        {
            return value * Math.Pow(1024, e);
        }

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
            try
            {
                return (short)result;
            }
            catch (InvalidCastException)
            {
            }
            catch (OverflowException)
            {
            }

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
            if (TryParseToInt32(s, radix, out var i))
            {
                try
                {
                    result = (short)i;
                    return true;
                }
                catch (InvalidCastException)
                {
                }
                catch (OverflowException)
                {
                }
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
            if (s[0] == '-')
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
            if (s[0] == '-')
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
    }
}
