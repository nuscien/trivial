// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interval.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The interval object.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Trivial.Maths
{
    /// <summary>
    /// The utility of interval.
    /// </summary>
    public static class IntervalUtility
    {
        internal const string ErrorParseMessage = "The string to parse was not in the internal format.";

        #region Simple interval

        /// <summary>
        /// Loads a copier into current instance.
        /// </summary>
        /// <param name="interval">The interval to be filled.</param>
        /// <param name="value">The instance to copy.</param>
        public static void Load<T>(this ISimpleInterval<T> interval, ISimpleInterval<T> value)
        {
            if (value == null) return;
            interval.MinValue = value.MinValue;
            interval.MaxValue = value.MaxValue;
            interval.LeftOpen = value.LeftOpen;
            interval.RightOpen = value.RightOpen;
        }

        /// <summary>
        /// Sets the values for the simple closed interval.
        /// The greater one will be set as MaxValue, the less one will be set as MinValue.
        /// </summary>
        /// <param name="interval">The interval to be filled.</param>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        public static void SetValues<T>(this ISimpleInterval<T> interval, T valueA, T valueB)
        {
            if (interval.IsGreaterThanOrEqualMinValue(valueA)) interval.MaxValue = valueA;
            interval.MinValue = valueA;
            interval.MaxValue = valueA;
            if (interval.IsGreaterThanOrEqualMinValue(valueB) || interval.IsGreaterThanOrEqualMaxValue(valueB))
            {
                interval.MaxValue = valueB;
                return;
            }

            interval.MinValue = valueB;
            interval.MaxValue = valueA;
        }

        /// <summary>
        /// Sets the values for the simple closed interval.
        /// The greater one will be set as MaxValue, the less one will be set as MinValue.
        /// </summary>
        /// <param name="interval">The interval to be filled.</param>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
        /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
        public static void SetValues<T>(this ISimpleInterval<T> interval, T valueA, T valueB, bool leftOpen, bool rightOpen)
        {
            interval.LeftOpen = leftOpen;
            interval.RightOpen = rightOpen;
            interval.SetValues(valueA, valueB);
        }

        /// <summary>
        /// Sets a greater value if the it is not in the interval.
        /// If the parameter is greater than MaxValue, this value will instead MaxValue; otherwise, nothing will be changed.
        /// </summary>
        /// <param name="interval">The interval to be filled.</param>
        /// <param name="value">A greater value to make sure the interval can include.</param>
        public static void SetGreaterValue<T>(this ISimpleInterval<T> interval, T value)
        {
            if (interval.IsGreaterThanOrEqualMaxValue(value))
            {
                interval.MaxValue = value;
            }
        }

        /// <summary>
        /// Sets a less value if the it is not in the interval.
        /// If the parameter is less than MinValue, this value will instead MinValue; otherwise, nothing will be changed.
        /// </summary>
        /// <param name="interval">The interval to be filled.</param>
        /// <param name="value">A less value to make sure the interval can include.</param>
        public static void SetLessValue<T>(this ISimpleInterval<T> interval, T value)
        {
            if (interval.IsLessThanOrEqualMinValue(value))
            {
                interval.MinValue = value;
            }
        }

        /// <summary>
        /// Sets a greater value if the it is not in the interval.
        /// If the parameter is greater than MaxValue, this value will instead MaxValue; otherwise, nothing will be changed.
        /// </summary>
        /// <param name="interval">The interval to be filled.</param>
        /// <param name="value">A greater value to make sure the interval can include.</param>
        /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
        public static void SetGreaterValue<T>(this ISimpleInterval<T> interval, T value, bool rightOpen)
        {
            interval.RightOpen = rightOpen;
            if (interval.IsGreaterThanOrEqualMaxValue(value))
            {
                interval.MaxValue = value;
            }
        }

        /// <summary>
        /// Sets a less value if the it is not in the interval.
        /// If the parameter is less than MinValue, this value will instead MinValue; otherwise, nothing will be changed.
        /// </summary>
        /// <param name="interval">The interval to be filled.</param>
        /// <param name="value">A less value to make sure the interval can include.</param>
        /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
        public static void SetLessValue<T>(this ISimpleInterval<T> interval, T value, bool leftOpen)
        {
            interval.LeftOpen = leftOpen;
            if (interval.IsLessThanOrEqualMinValue(value))
            {
                interval.MinValue = value;
            }
        }

        /// <summary>
        /// Checks if a value is in the interval.
        /// </summary>
        /// <param name="interval">The interval to compare.</param>
        /// <param name="value">A value to check.</param>
        /// <returns>true if the value is in the interval; otherwise, false.</returns>
        public static bool IsInInterval<T>(this ISimpleInterval<T> interval, T value)
        {
            var compareLeft = interval.LeftOpen ? interval.IsGreaterThanMinValue(value) : interval.IsGreaterThanOrEqualMinValue(value);
            if (!compareLeft) return false;
            return interval.RightOpen ? interval.IsLessThanMaxValue(value) : interval.IsLessThanOrEqualMaxValue(value);
        }

        /// <summary>
        /// Checks if the value is less than or equal MinValue.
        /// </summary>
        /// <param name="interval">The interval to compare.</param>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
        public static bool IsLessThanOrEqualMinValue<T>(this ISimpleInterval<T> interval, T value)
        {
            return interval.IsLessThanMinValue(value) || interval.EqualsMinValue(value);
        }

        /// <summary>
        /// Checks if the value is greater than or equal MinValue.
        /// </summary>
        /// <param name="interval">The interval to compare.</param>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
        public static bool IsGreaterThanOrEqualMinValue<T>(this ISimpleInterval<T> interval, T value)
        {
            return interval.IsGreaterThanMinValue(value) || interval.EqualsMinValue(value);
        }

        /// <summary>
        /// Checks if the value is less than or equal MaxValue.
        /// </summary>
        /// <param name="interval">The interval to compare.</param>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
        public static bool IsLessThanOrEqualMaxValue<T>(this ISimpleInterval<T> interval, T value)
        {
            return interval.IsLessThanMaxValue(value) || interval.EqualsMaxValue(value);
        }

        /// <summary>
        /// Checks if the value is greater than or equal MaxValue.
        /// </summary>
        /// <param name="interval">The interval to compare.</param>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
        public static bool IsGreaterThanOrEqualMaxValue<T>(this ISimpleInterval<T> interval, T value)
        {
            return interval.IsGreaterThanMaxValue(value) || interval.EqualsMaxValue(value);
        }

        #endregion

        #region Parse number interval

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <param name="parseInt32">The parser for integer.</param>
        /// <param name="parseDouble">The parser for floating-point number.</param>
        /// <returns>The interval instance parsed.</returns>
        private static StructValueSimpleInterval<int> ParseForInt32(string s, Func<string, int?> parseInt32, Func<string, double> parseDouble) => ParseForX(s, int.MinValue, int.MaxValue, false, (ele, pos) =>
        {
            var r = parseInt32(ele);
            if (r.HasValue) return (r.Value, null);
            if (ele.StartsWith("0") && ele.Length > 2)
            {
                switch (ele[1])
                {
                    case 'x':
                    case 'X':
                        return (Convert.ToInt32(ele.Substring(2), 16), null);
                    case 'b':
                    case 'B':
                        return (Convert.ToInt32(ele.Substring(2), 2), null);
                    case 'o':
                    case 'O':
                        return (Convert.ToInt32(ele.Substring(2), 8), null);
                }
            }

            if (ele == NumberSymbols.InfiniteSymbol || ele == NumberSymbols.PositiveInfiniteSymbol || ele == "Infinity" || ele == "NaN" || ele == "null")
            {
                if (pos >= 0) return (int.MaxValue, false);
                return (int.MaxValue, true);
            }
            else if (ele == NumberSymbols.NegativeInfiniteSymbol || ele == "-Infinity" || ele == "NaN" || ele == "null")
            {
                if (pos <= 0) return (int.MinValue, false);
                return (int.MinValue, true);
            }

            var num = parseDouble(ele);
            if (pos >= 0) return (num >= int.MaxValue ? int.MaxValue : (int)num, false);
            if (num >= int.MaxValue) return (int.MaxValue, true);
            if (num < int.MinValue) return (int.MinValue, false);
            num += 1;
            return ((int)num, false);
        }, null, null);

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <returns>The interval instance parsed.</returns>
        public static StructValueSimpleInterval<int> ParseForInt32(string s) => ParseForInt32(s, ele =>
        {
            if (int.TryParse(ele, out var r)) return r;
            return null;
        }, double.Parse);

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements that can be present in s.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
        /// <returns>The interval instance parsed.</returns>
        public static StructValueSimpleInterval<int> ParseForInt32(string s, System.Globalization.NumberStyles style, IFormatProvider provider) => ParseForInt32(s, ele =>
        {
            if (int.TryParse(ele, style, provider, out var r)) return r;
            return null;
        }, ele => double.Parse(ele, style, provider));

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <param name="parseInt64">The parser for integer.</param>
        /// <param name="parseDouble">The parser for floating-point number.</param>
        /// <returns>The interval instance parsed.</returns>
        private static StructValueSimpleInterval<long> ParseForInt64(string s, Func<string, long?> parseInt64, Func<string, double> parseDouble) => ParseForX(s, long.MinValue, long.MaxValue, false, (ele, pos) =>
        {
            var r = parseInt64(ele);
            if (r.HasValue) return (r.Value, null);
            if (ele.StartsWith("0") && ele.Length > 2)
            {
                switch (ele[1])
                {
                    case 'x':
                    case 'X':
                        return (Convert.ToInt64(ele.Substring(2), 16), null);
                    case 'b':
                    case 'B':
                        return (Convert.ToInt64(ele.Substring(2), 2), null);
                    case 'o':
                    case 'O':
                        return (Convert.ToInt64(ele.Substring(2), 8), null);
                }
            }

            if (ele == NumberSymbols.InfiniteSymbol || ele == NumberSymbols.PositiveInfiniteSymbol || ele == "Infinity" || ele == "NaN" || ele == "null")
            {
                if (pos >= 0) return (long.MaxValue, false);
                return (long.MaxValue, true);
            }
            else if (ele == NumberSymbols.NegativeInfiniteSymbol || ele == "-Infinity" || ele == "NaN" || ele == "null")
            {
                if (pos <= 0) return (long.MinValue, false);
                return (long.MinValue, true);
            }

            var num = parseDouble(ele);
            if (pos >= 0) return (num >= long.MaxValue ? long.MaxValue : (long)num, false);
            if (num >= long.MaxValue) return (long.MaxValue, true);
            if (num < long.MinValue) return (long.MinValue, false);
            num += 1;
            return ((long)num, false);
        }, null, null);

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <returns>The interval instance parsed.</returns>
        public static StructValueSimpleInterval<long> ParseForInt64(string s) => ParseForInt64(s, ele =>
        {
            if (long.TryParse(ele, out var r)) return r;
            return null;
        }, double.Parse);

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements that can be present in s.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
        /// <returns>The interval instance parsed.</returns>
        public static StructValueSimpleInterval<long> ParseForInt64(string s, System.Globalization.NumberStyles style, IFormatProvider provider) => ParseForInt64(s, ele =>
        {
            if (long.TryParse(ele, style, provider, out var r)) return r;
            return null;
        }, ele => double.Parse(ele, style, provider));

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <param name="parseInt32">The parser for integer.</param>
        /// <param name="parseDouble">The parser for floating-point number.</param>
        /// <returns>The interval instance parsed.</returns>
        private static NullableValueSimpleInterval<int> ParseForNullableInt32(string s, Func<string, int?> parseInt32, Func<string, double> parseDouble) => ParseForX<int>(s, (ele, pos) =>
        {
            var r = parseInt32(ele);
            if (r.HasValue) return (r.Value, null);
            if (ele.StartsWith("0") && ele.Length > 2)
            {
                switch (ele[1])
                {
                    case 'x':
                    case 'X':
                        return (Convert.ToInt32(ele.Substring(2), 16), null);
                    case 'b':
                    case 'B':
                        return (Convert.ToInt32(ele.Substring(2), 2), null);
                    case 'o':
                    case 'O':
                        return (Convert.ToInt32(ele.Substring(2), 8), null);
                }
            }

            if (ele == NumberSymbols.InfiniteSymbol || ele == NumberSymbols.PositiveInfiniteSymbol || ele == "Infinity" || ele == "NaN" || ele == "null")
            {
                if (pos >= 0) return (null, true);
                return (int.MaxValue, true);
            }
            else if (ele == NumberSymbols.NegativeInfiniteSymbol || ele == "-Infinity" || ele == "NaN" || ele == "null")
            {
                if (pos <= 0) return (null, true);
                return (int.MinValue, true);
            }

            var num = parseDouble(ele);
            if (pos >= 0) return (num >= int.MaxValue ? int.MaxValue : (int)num, false);
            if (num >= int.MaxValue) return (int.MaxValue, true);
            if (num < int.MinValue) return (int.MinValue, false);
            num += 1;
            return ((int)num, false);
        });

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <returns>The interval instance parsed.</returns>
        public static NullableValueSimpleInterval<int> ParseForNullableInt32(string s) => ParseForNullableInt32(s, ele =>
        {
            if (int.TryParse(ele, out var r)) return r;
            return null;
        }, double.Parse);

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements that can be present in s.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
        /// <returns>The interval instance parsed.</returns>
        public static NullableValueSimpleInterval<int> ParseForNullableInt32(string s, System.Globalization.NumberStyles style, IFormatProvider provider) => ParseForNullableInt32(s, ele =>
        {
            if (int.TryParse(ele, style, provider, out var r)) return r;
            return null;
        }, ele => double.Parse(ele, style, provider));

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <param name="parseInt64">The parser for integer.</param>
        /// <param name="parseDouble">The parser for floating-point number.</param>
        /// <returns>The interval instance parsed.</returns>
        /// <exception cref="FormatException">The string to parse is not the internal format.</exception>
        private static NullableValueSimpleInterval<long> ParseForNullableInt64(string s, Func<string, long?> parseInt64, Func<string, double> parseDouble) => ParseForX<long>(s, (ele, pos) =>
        {
            var r = parseInt64(ele);
            if (r.HasValue) return (r.Value, null);
            if (ele.StartsWith("0") && ele.Length > 2)
            {
                switch (ele[1])
                {
                    case 'x':
                    case 'X':
                        return (Convert.ToInt64(ele.Substring(2), 16), null);
                    case 'b':
                    case 'B':
                        return (Convert.ToInt64(ele.Substring(2), 2), null);
                    case 'o':
                    case 'O':
                        return (Convert.ToInt64(ele.Substring(2), 8), null);
                }
            }

            if (ele == NumberSymbols.InfiniteSymbol || ele == NumberSymbols.PositiveInfiniteSymbol || ele == "Infinity" || ele == "NaN" || ele == "null")
            {
                if (pos >= 0) return (null, true);
                return (long.MaxValue, true);
            }
            else if (ele == NumberSymbols.NegativeInfiniteSymbol || ele == "-Infinity" || ele == "NaN" || ele == "null")
            {
                if (pos <= 0) return (null, true);
                return (long.MinValue, true);
            }

            var num = parseDouble(ele);
            if (pos >= 0) return (num >= long.MaxValue ? long.MaxValue : (long)num, false);
            if (num >= long.MaxValue) return (long.MaxValue, true);
            if (num < long.MinValue) return (long.MinValue, false);
            num += 1;
            return ((long)num, false);
        });

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <returns>The interval instance parsed.</returns>
        /// <exception cref="FormatException">The string to parse is not the internal format.</exception>
        public static NullableValueSimpleInterval<long> ParseForNullableInt64(string s) => ParseForNullableInt64(s, ele =>
        {
            if (long.TryParse(ele, out var r)) return r;
            return null;
        }, double.Parse);

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements that can be present in s.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
        /// <returns>The interval instance parsed.</returns>
        /// <exception cref="FormatException">The string to parse is not the internal format.</exception>
        public static NullableValueSimpleInterval<long> ParseForNullableInt64(string s, System.Globalization.NumberStyles style, IFormatProvider provider) => ParseForNullableInt64(s, ele =>
        {
            if (long.TryParse(ele, style, provider, out var r)) return r;
            return null;
        }, ele => double.Parse(ele, style, provider));

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <returns>The interval instance parsed.</returns>
        /// <exception cref="FormatException">The string to parse is not the internal format.</exception>
        public static StructValueSimpleInterval<double> ParseForDouble(string s) => ParseForX(
            s,
            double.NegativeInfinity,
            double.PositiveInfinity,
            true,
            (ele, pos) => (double.Parse(ele), null),
            double.NegativeInfinity,
            double.PositiveInfinity);

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
        /// <returns>The interval instance parsed.</returns>
        /// <exception cref="FormatException">The string to parse is not the internal format.</exception>
        public static StructValueSimpleInterval<double> ParseForDouble(string s, IFormatProvider provider) => ParseForX(
            s,
            double.NegativeInfinity,
            double.PositiveInfinity,
            true,
            (ele, pos) => (double.Parse(ele, provider), null),
            double.NegativeInfinity,
            double.PositiveInfinity);

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicate the style elements that can be present in s.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
        /// <returns>The interval instance parsed.</returns>
        /// <exception cref="FormatException">The string to parse is not the internal format.</exception>
        public static StructValueSimpleInterval<double> ParseForDouble(string s, System.Globalization.NumberStyles style, IFormatProvider provider) => ParseForX(
            s,
            double.NegativeInfinity,
            double.PositiveInfinity,
            true,
            (ele, pos) => (double.Parse(ele, style, provider), null),
            double.NegativeInfinity,
            double.PositiveInfinity);

        private const string digits = "0123456789+-∞";

        private static StructValueSimpleInterval<T> ParseForX<T>(string s, T minValue, T maxValue, bool supportInfinite, Func<string, int, (T?, bool?)> convert, T? negativeInfinite, T? positiveInfinite) where T : struct, IComparable<T>
        {
            try
            {
                if (string.IsNullOrEmpty(s) || s.Length < 2) return null;
                if (s.StartsWith("{") && s.IndexOf(":") > 0) return System.Text.Json.JsonSerializer.Deserialize<StructValueSimpleInterval<T>>(s);
                var tuple = ParseFromString(s, convert, null);
                return new StructValueSimpleInterval<T>(
                    tuple.Item1 ?? minValue,
                    tuple.Item3 ?? maxValue,
                    (supportInfinite || tuple.Item1.HasValue) && tuple.Item2,
                    (supportInfinite || tuple.Item3.HasValue) && tuple.Item4,
                    negativeInfinite,
                    positiveInfinite);
            }
            catch (FormatException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (OverflowException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (ArgumentException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
        }
        private static NullableValueSimpleInterval<T> ParseForX<T>(string s, Func<string, int, (T?, bool?)> convert) where T : struct, IComparable<T>
        {
            try
            {
                if (string.IsNullOrEmpty(s) || s.Length < 2) return null;
                if (s.StartsWith("{") && s.IndexOf(":") > 0) return System.Text.Json.JsonSerializer.Deserialize<NullableValueSimpleInterval<T>>(s);
                var tuple = ParseFromString(s, convert, null);
                return new NullableValueSimpleInterval<T>(tuple.Item1, tuple.Item3, tuple.Item2, tuple.Item4);
            }
            catch (FormatException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (OverflowException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (ArgumentException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
        }

        private static (T, bool, T, bool) ParseFromString<T>(string s, Func<string, int, (T, bool?)> convert, T defaultValue)
        {
            var left = defaultValue;
            var leftOpen = false;
            var right = defaultValue;
            var rightOpen = false;
            if (s[0] == '(' || s[0] == ']')
            {
                leftOpen = true;
            }
            else if (s[0] != '[')
            {
                while (s[0] == '=') s = s.Substring(1);
                if (s.Length < 1) throw new FormatException("Expect it is an interval format string but it only contains equal sign.");

                var side = 0;
                if (s.StartsWith("<="))
                {
                    side = 1;
                    leftOpen = true;
                    s = s.Substring(2);
                }
                else if (s.StartsWith(">="))
                {
                    side = -1;
                    rightOpen = true;
                    s = s.Substring(2);
                }
                else if (s.StartsWith("≤"))
                {
                    side = 1;
                    leftOpen = true;
                    s = s.Substring(1);
                }
                else if (s.StartsWith("≥"))
                {
                    side = -1;
                    rightOpen = true;
                    s = s.Substring(1);
                }
                else if (s.StartsWith("<"))
                {
                    side = 1;
                    leftOpen = true;
                    rightOpen = true;
                    s = s.Substring(1);
                }
                else if (s.StartsWith(">"))
                {
                    side = -1;
                    leftOpen = true;
                    rightOpen = true;
                    s = s.Substring(1);
                }

                if (s.Length < 1) throw new FormatException("Expect it is an interval format string but it only contains operator.");

#if !NETSTANDARD2_0
                if (digits.Contains(s[0]))
#else
                if (digits.Contains(s[0].ToString()))
#endif
                {
                    try
                    {
                        if (side <= 0)
                        {
                            var resultTuple = convert(s, side - 1);
                            left = resultTuple.Item1;
                            if (resultTuple.Item2.HasValue) leftOpen = resultTuple.Item2.Value;
                        }

                        if (side >= 0)
                        {
                            var resultTuple = convert(s, side + 1);
                            right = resultTuple.Item1;
                            if (resultTuple.Item2.HasValue) rightOpen = resultTuple.Item2.Value;
                        }

                        return (left, leftOpen, right, rightOpen);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new FormatException("Expect it is an interval format string.", ex);
                    }
                    catch (OverflowException ex)
                    {
                        throw new FormatException("Expect it is an interval format string.", ex);
                    }
                }

                throw new FormatException("Expect the first character is ( or [.");
            }

            var last = s.Length - 1;
            if (s[last] == ')' || s[last] == '[') rightOpen = true;
            else if (s[last] != ']') throw new FormatException("Expect the last character ] or ).");
            var split = ';';
            if (s.IndexOf(split) < 0) split = ',';

            #pragma warning disable IDE0057
            var arr = s.Substring(1, s.Length - 2).Split(split);
            #pragma warning restore IDE0057

            if (arr.Length == 0) return (left, leftOpen, right, rightOpen);
            var ele = arr[0]?.Trim();
            if (!string.IsNullOrEmpty(ele) && ele != NumberSymbols.NegativeInfiniteSymbol && ele != "-Infinity" && ele != "NaN" && ele != "null")
            {
                var resultTuple = convert(ele, arr.Length > 1 ? -2 : -1);
                left = resultTuple.Item1;
                if (resultTuple.Item2.HasValue) leftOpen = resultTuple.Item2.Value;
            }

            var rightPos = 2;
            if (arr.Length > 1) ele = arr[1]?.Trim();
            else rightPos = 1;
            if (!string.IsNullOrEmpty(ele) && ele != NumberSymbols.InfiniteSymbol && ele != NumberSymbols.PositiveInfiniteSymbol && ele != "Infinity" && ele != "NaN" && ele != "null")
            {
                var resultTuple = convert(ele, rightPos);
                right = resultTuple.Item1;
                if (resultTuple.Item2.HasValue) rightOpen = resultTuple.Item2.Value;
            }

            return (left, leftOpen, right, rightOpen);
        }

        /// <summary>
        /// Parses an interval string.
        /// </summary>
        /// <typeparam name="T1">The type of the interval.</typeparam>
        /// <typeparam name="T2">The type of the value.</typeparam>
        /// <param name="s">The string to parse.</param>
        /// <param name="factory">The interval factory.</param>
        /// <param name="convert">The value parser.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>An interval instance.</returns>
        /// <exception cref="FormatException">The string to parse is not the internal format.</exception>
        public static T1 Parse<T1, T2>(string s, Func<T2, bool, T2, bool, T1> factory, Func<string, T2> convert, T2 defaultValue) where T1 : ISimpleInterval<T2>
        {
            try
            {
                if (string.IsNullOrEmpty(s) || s.Length < 2) return default;
                if (s.StartsWith("{") && s.IndexOf(":") > 0) return System.Text.Json.JsonSerializer.Deserialize<T1>(s);
                var tuple = ParseFromString(s, (ele, pos) => (convert(ele), null), defaultValue);
                return factory(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
            }
            catch (FormatException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (OverflowException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (ArgumentException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new FormatException(ErrorParseMessage, ex);
            }
        }

        #endregion
    }
}
