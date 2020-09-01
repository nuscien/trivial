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

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <returns>The interval instance parsed.</returns>
        public static StructValueSimpleInterval<int> ParseForInt32(string s) => ParseForX(s, int.MinValue, int.MaxValue, false, (ele, pos) =>
        {
            if (int.TryParse(ele, out var r)) return (r, null);
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

            if (pos >= 0) return ((int)double.Parse(ele), false);
            var num = double.Parse(ele);
            if (num < int.MaxValue) num += 1;
            return ((int)num, false);
        }, null, null);

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <returns>The interval instance parsed.</returns>
        public static StructValueSimpleInterval<long> ParseForInt64(string s) => ParseForX(s, long.MinValue, long.MaxValue, false, (ele, pos) =>
        {
            if (long.TryParse(ele, out var r)) return (r, null);
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

            if (pos >= 0) return ((long)double.Parse(ele), false);
            var num = double.Parse(ele);
            if (num < long.MaxValue) num += 1;
            return ((long)num, false);
        }, null, null);

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <returns>The interval instance parsed.</returns>
        public static NullableValueSimpleInterval<int> ParseForNullableInt32(string s) => ParseForX<int>(s, (ele, pos) =>
        {
            if (int.TryParse(ele, out var r)) return (r, null);
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

            if (pos >= 0) return ((int)double.Parse(ele), false);
            var num = double.Parse(ele);
            if (num < int.MaxValue) num += 1;
            return ((int)num, false);
        });

        /// <summary>
        /// Parses the interval.
        /// </summary>
        /// <param name="s">The interval format string.</param>
        /// <returns>The interval instance parsed.</returns>
        /// <exception cref="FormatException">The string to parse is not the internal format.</exception>
        public static NullableValueSimpleInterval<long> ParseForNullableInt64(string s) => ParseForX<long>(s, (ele, pos) =>
        {
            if (long.TryParse(ele, out var r)) return (r, null);
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

            if (pos >= 0) return ((long)double.Parse(ele), false);
            var num = double.Parse(ele);
            if (num < long.MaxValue) num += 1;
            return ((long)num, false);
        });

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
                    supportInfinite || tuple.Item1.HasValue ? tuple.Item2 : false,
                    supportInfinite || tuple.Item3.HasValue ? tuple.Item4 : false,
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
    }

    /// <summary>
    /// The interface for simple closed interval.
    /// </summary>
    /// <typeparam name="T">The type of the object in interval.</typeparam>
    public interface ISimpleInterval<T>
    {
        /// <summary>
        /// Gets a value indicating whether it is left bounded.
        /// </summary>
        bool LeftBounded { get; }

        /// <summary>
        /// Gets a value indicating whether it is right bounded.
        /// </summary>
        bool RightBounded { get; }

        /// <summary>
        /// Gets or sets a value indicating whether it is left open.
        /// </summary>
        bool LeftOpen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is right open.
        /// </summary>
        bool RightOpen { get; set; }

        /// <summary>
        /// Gets or sets the minimum value of the interval.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value should be less than MaxValue.</exception>
        T MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of the interval.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value should be greater than MinValue.</exception>
        T MaxValue { get; set; }

        /// <summary>
        /// Checks if the value is less than MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
        bool IsLessThanMinValue(T value);

        /// <summary>
        /// Checks if the value is greater than MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
        bool IsGreaterThanMinValue(T value);

        /// <summary>
        /// Checks if the value equals MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
        bool EqualsMinValue(T value);

        /// <summary>
        /// Checks if the value is less than MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
        bool IsLessThanMaxValue(T value);

        /// <summary>
        /// Checks if the value is greater than MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
        bool IsGreaterThanMaxValue(T value);

        /// <summary>
        /// Checks if the value equals MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
        bool EqualsMaxValue(T value);

        /// <summary>
        /// Converts the interval to a tuple, which includes MinValue and MaxValue.
        /// </summary>
        /// <returns>A tuple including MinValue and MaxValue.</returns>
        Tuple<T, T> ToTuple();

        /// <summary>
        /// Converts the interval to a list, which includes MinValue and MaxValue.
        /// </summary>
        /// <returns>A list including MinValue and MaxValue.</returns>
        IList<T> ToList();
    }

    /// <summary>
    /// The simple closed interval base class.
    /// </summary>
    [DataContract]
    public abstract class SimpleInterval<T> : ISimpleInterval<T>
    {
        /// <summary>
        /// The minimum value of the interval in memory.
        /// </summary>
        private T _minValue;

        /// <summary>
        /// The maximum value of the interval in memory.
        /// </summary>
        private T _maxValue;

        /// <summary>
        /// The value indicating whether it is left open.
        /// </summary>
        private bool _leftOpen;

        /// <summary>
        /// The value indicating whether it is right open.
        /// </summary>
        private bool _rightOpen;

        /// <summary>
        /// Initializes a new instance of the SimpleInterval class.
        /// </summary>
        protected SimpleInterval()
        {
            LeftOpen = true;
            RightOpen = true;
        }

        /// <summary>
        /// Initializes a new instance of the SimpleInterval class.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="op">The operation.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        protected SimpleInterval(T value, BasicCompareOperator op, T minValue, T maxValue)
        {
            switch (op)
            {
                case BasicCompareOperator.Greater:
                    MinValue = value;
                    MaxValue = maxValue;
                    LeftOpen = true;
                    RightOpen = true;
                    break;
                case BasicCompareOperator.GreaterOrEqual:
                    MinValue = value;
                    MaxValue = maxValue;
                    LeftOpen = false;
                    RightOpen = true;
                    break;
                case BasicCompareOperator.Less:
                    MinValue = minValue;
                    MaxValue = value;
                    LeftOpen = true;
                    RightOpen = false;
                    break;
                case BasicCompareOperator.LessOrEqual:
                    MinValue = minValue;
                    MaxValue = value;
                    LeftOpen = true;
                    RightOpen = false;
                    break;
                default:
                    MinValue = value;
                    MaxValue = value;
                    LeftOpen = false;
                    RightOpen = false;
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SimpleInterval class.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="op">The operation.</param>
        protected SimpleInterval(T value, BasicCompareOperator op) : this(value, op, default, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        protected SimpleInterval(T valueA, T valueB) : this(valueA, valueB, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="autoFix">true if fix the order automatically; otherwise, false.</param>
        protected SimpleInterval(T valueA, T valueB, bool autoFix)
        {
            if (autoFix)
            {
                this.SetValues(valueA, valueB);
            }
            else
            {
                MinValue = valueA;
                MaxValue = valueB;
            }

            LeftOpen = valueA is null;
            RightOpen = valueB is null;
        }

        /// <summary>
        /// Initializes a new instance of the SimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
        /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
        protected SimpleInterval(T valueA, T valueB, bool leftOpen, bool rightOpen)
        {
            MinValue = valueA;
            MaxValue = valueB;
            LeftOpen = leftOpen;
            RightOpen = rightOpen;
        }

        /// <summary>
        /// Occurs on minimum (left) value is changed.
        /// </summary>
        public event Data.ChangeEventHandler<T> MinValueChanged;

        /// <summary>
        /// Occurs on maximum (right) value is changed.
        /// </summary>
        public event Data.ChangeEventHandler<T> MaxValueChanged;

        /// <summary>
        /// Gets a value indicating whether it is left bounded.
        /// </summary>
        [JsonIgnore]
        public abstract bool LeftBounded { get; }

        /// <summary>
        /// Gets a value indicating whether it is right bounded.
        /// </summary>
        [JsonIgnore]
        public abstract bool RightBounded { get; }

        /// <summary>
        /// Gets or sets a value indicating whether it is left open.
        /// </summary>
        [JsonPropertyName("leftopen")]
        [DataMember(Name = "leftopen")]
        public bool LeftOpen
        { 
            get
            {
                return _leftOpen || !LeftBounded;
            }

            set
            {
                _leftOpen = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is right open.
        /// </summary>
        [JsonPropertyName("rightopen")]
        [DataMember(Name = "rightopen")]
        public bool RightOpen {
            get
            {
                return _rightOpen || !RightBounded;
            }
            
            set
            {
                _rightOpen = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether throw an exception if the value is not valid.
        /// </summary>
        [JsonIgnore]
        public bool ValidateValueSetting { get; set; }

        /// <summary>
        /// Gets or sets the minimum (left) value of the interval.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value should be less than MaxValue.</exception>
        [JsonPropertyName("left")]
        [DataMember(Name = "left")]
        public T MinValue
        {
            get
            {
                return _minValue;
            }

            set
            {
                if (value is null && _minValue is null) return;
                if (ValidateValueSetting && IsGreaterThanMinValue(value) && IsGreaterThanMaxValue(value) && RightBounded)
                    throw new ArgumentOutOfRangeException("value", "The value should be less than MaxValue.");
                var oldValue = _minValue;
                _minValue = value;
                MinValueChanged?.Invoke(this, new Data.ChangeEventArgs<T>(oldValue, value, Data.ChangeMethods.Update, nameof(MinValue)));
            }
        }

        /// <summary>
        /// Gets or sets the maximum (right) value of the interval.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value should be greater than MinValue.</exception>
        [JsonPropertyName("right")]
        [DataMember(Name = "right")]
        public T MaxValue
        {
            get
            {
                return _maxValue;
            }

            set
            {
                if (value is null && _maxValue is null) return;
                if (ValidateValueSetting && IsLessThanMaxValue(value) && IsLessThanMinValue(value) && LeftBounded)
                    throw new ArgumentOutOfRangeException("value", "The value should be greater than MinValue.");
                var oldValue = _minValue;
                _maxValue = value;
                MaxValueChanged?.Invoke(this, new Data.ChangeEventArgs<T>(oldValue, value, Data.ChangeMethods.Update, nameof(MaxValue)));
            }
        }

        /// <summary>
        /// Checks if the value is less than MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
        public abstract bool IsLessThanMinValue(T value);

        /// <summary>
        /// Checks if the value is greater than MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
        public abstract bool IsGreaterThanMinValue(T value);

        /// <summary>
        /// Checks if the value equals MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
        public abstract bool EqualsMinValue(T value);

        /// <summary>
        /// Checks if the value is less than MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
        public abstract bool IsLessThanMaxValue(T value);

        /// <summary>
        /// Checks if the value is greater than MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
        public abstract bool IsGreaterThanMaxValue(T value);

        /// <summary>
        /// Checks if the value equals MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
        public abstract bool EqualsMaxValue(T value);

        /// <summary>
        /// Converts the interval to a tuple, which includes MinValue and MaxValue.
        /// </summary>
        /// <returns>A tuple including MinValue and MaxValue.</returns>
        public Tuple<T, T> ToTuple()
        {
            return new Tuple<T, T>(MinValue, MaxValue);
        }

        /// <summary>
        /// Converts the interval to a tuple, which includes MinValue and MaxValue.
        /// </summary>
        /// <returns>A tuple including MinValue and MaxValue.</returns>
        public ValueTuple<T, T> ToValueTuple()
        {
            return new ValueTuple<T, T>(MinValue, MaxValue);
        }

        /// <summary>
        /// Converts the interval to a two elements object, which includes MinValue and MaxValue.
        /// </summary>
        /// <returns>A multiple elements object including MinValue and MaxValue.</returns>
        public TwoElements<T> ToTwoElements()
        {
            return new TwoElements<T>{ ItemA = MinValue, ItemB = MaxValue };
        }

        /// <summary>
        /// Converts the interval to a list, which includes MinValue and MaxValue.
        /// </summary>
        /// <returns>A list including MinValue and MaxValue.</returns>
        public IList<T> ToList()
        {
            return new List<T> { MinValue, MaxValue };
        }

        /// <summary>
        /// Returns the interval string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this interval.</returns>
        public override string ToString()
        {
            var leftStr = LeftBounded ? ToValueString(MinValue) : NumberSymbols.NegativeInfiniteSymbol;
            var rightStr = RightBounded ? ToValueString(MaxValue) : NumberSymbols.PositiveInfiniteSymbol;
            var longStr = string.Format("{0} - {1}", leftStr, rightStr);
            var sep = false;
            if (longStr.IndexOfAny(new[] { ',', ';' }) > -1) sep = true;
            if (!sep && longStr.IndexOf(';') > -1)
            {
                const string quoteStr = "\"{0}\"";
                leftStr = string.Format(quoteStr, leftStr.Replace("\"", "\\\""));
                rightStr = string.Format(quoteStr, rightStr.Replace("\"", "\\\""));
            }

            return string.Format("{0}{1}{2} {3}{4}", LeftOpen ? "(" : "[", leftStr, sep ? ";" : ",", rightStr, RightOpen ? ")" : "]");
        }

        /// <summary>
        /// Gets the string format of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        protected virtual string ToValueString(T value)
        {
            if (value is null) return string.Empty;
            return value.ToString();
        }
    }

    /// <summary>
    /// The simple interval with class value.
    /// </summary>
    [DataContract]
    public class RefValueSimpleInterval<T> : SimpleInterval<T> where T : class, IComparable<T>
    {
        /// <summary>
        /// Initializes a new instance of the RefValueSimpleInterval class.
        /// </summary>
        public RefValueSimpleInterval()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RefValueSimpleInterval class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="op">The operation.</param>
        public RefValueSimpleInterval(T value, BasicCompareOperator op)
            : base(value, op, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RefValueSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        public RefValueSimpleInterval(T valueA, T valueB)
            : base(valueA, valueB)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RefValueSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="autoFix">true if fix the order automatically; otherwise, false.</param>
        public RefValueSimpleInterval(T valueA, T valueB, bool autoFix)
            : base(valueA, valueB, autoFix)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RefValueSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
        /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
        public RefValueSimpleInterval(T valueA, T valueB, bool leftOpen, bool rightOpen)
            : base(valueA, valueB, leftOpen, rightOpen)
        {
        }

        /// <summary>
        /// Gets a value indicating whether it is left bounded.
        /// </summary>
        [JsonIgnore]
        public override bool LeftBounded { get { return MinValue != null; } }

        /// <summary>
        /// Gets a value indicating whether it is right bounded.
        /// </summary>
        [JsonIgnore]
        public override bool RightBounded { get { return MaxValue != null; } }

        /// <summary>
        /// Checks if the value is less than MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
        public override bool IsLessThanMinValue(T value)
        {
            if (MinValue == null) return false;
            if (value == null) return true;
            return 0 < (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
        }

        /// <summary>
        /// Checks if the value is greater than MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
        public override bool IsGreaterThanMinValue(T value)
        {
            if (value == null) return false;
            if (MinValue == null) return true;
            return 0 > (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
        }

        /// <summary>
        /// Checks if the value equals MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
        public override bool EqualsMinValue(T value)
        {
            if (MinValue == null && value == null) return true;
            if (MinValue == null || value == null) return false;
            return 0 == (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
        }

        /// <summary>
        /// Checks if the value is less than MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
        public override bool IsLessThanMaxValue(T value)
        {
            if (value == null) return false;
            if (MaxValue == null) return true;
            return 0 < (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
        }

        /// <summary>
        /// Checks if the value is greater than MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
        public override bool IsGreaterThanMaxValue(T value)
        {
            if (MaxValue == null) return false;
            if (value == null) return true;
            return 0 > (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
        }

        /// <summary>
        /// Checks if the value equals MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value equals MaxValue; otherwise, false.</returns>
        public override bool EqualsMaxValue(T value)
        {
            if (MaxValue == null && value == null) return true;
            if (MaxValue == null || value == null) return false;
            return 0 == (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
        }

        /// <summary>
        /// Gets the value comparer.
        /// </summary>
        [JsonIgnore]
        protected virtual IComparer<T> ValueComparer { get; }
    }

    /// <summary>
    /// The simple interval with struct value.
    /// </summary>
    [DataContract]
    public class StructValueSimpleInterval<T> : SimpleInterval<T> where T : struct, IComparable<T>
    {
        private readonly T? positiveInfiniteValue;
        private readonly T? negativeInfiniteValue;

        /// <summary>
        /// Initializes a new instance of the StructValueSimpleInterval class.
        /// </summary>
        public StructValueSimpleInterval()
        {
        }

        /// <summary>
        /// Initializes a new instance of the StructValueSimpleInterval class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="op">The operation.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        public StructValueSimpleInterval(T value, BasicCompareOperator op, T minValue, T maxValue)
            : base(value, op, minValue, maxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StructValueSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        public StructValueSimpleInterval(T valueA, T valueB)
            : base(valueA, valueB)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StructValueSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="autoFix">true if fix the order automatically; otherwise, false.</param>
        public StructValueSimpleInterval(T valueA, T valueB, bool autoFix)
            : base(valueA, valueB, autoFix)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StructValueSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
        /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
        public StructValueSimpleInterval(T valueA, T valueB, bool leftOpen, bool rightOpen)
            : base(valueA, valueB, leftOpen, rightOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StructValueSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
        /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
        /// <param name="negativeInfinite">The negative infinite value.</param>
        /// <param name="positiveInfinite">The positive infinite value.</param>
        public StructValueSimpleInterval(T valueA, T valueB, bool leftOpen, bool rightOpen, T? negativeInfinite, T? positiveInfinite)
            : base(valueA, valueB, leftOpen, rightOpen)
        {
            positiveInfiniteValue = positiveInfinite;
            negativeInfiniteValue = negativeInfinite;
        }

        /// <summary>
        /// Gets a value indicating whether it is left bounded.
        /// </summary>
        [JsonIgnore]
        public override bool LeftBounded => negativeInfiniteValue.HasValue ? !negativeInfiniteValue.Value.Equals(MinValue) : true;

        /// <summary>
        /// Gets a value indicating whether it is right bounded.
        /// </summary>
        [JsonIgnore]
        public override bool RightBounded => positiveInfiniteValue.HasValue ? !positiveInfiniteValue.Value.Equals(MaxValue) : true;

        /// <summary>
        /// Checks if the value is less than MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
        public override bool IsLessThanMinValue(T value)
        {
            return 0 < (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
        }

        /// <summary>
        /// Checks if the value is greater than MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
        public override bool IsGreaterThanMinValue(T value)
        {
            return 0 > (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
        }

        /// <summary>
        /// Checks if the value equals MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value equals MinValue; otherwise, false.</returns>
        public override bool EqualsMinValue(T value)
        {
            return 0 == (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
        }

        /// <summary>
        /// Checks if the value is less than MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
        public override bool IsLessThanMaxValue(T value)
        {
            return 0 < (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
        }

        /// <summary>
        /// Checks if the value is greater than MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
        public override bool IsGreaterThanMaxValue(T value)
        {
            return 0 > (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
        }

        /// <summary>
        /// Checks if the value equals MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value equals MaxValue; otherwise, false.</returns>
        public override bool EqualsMaxValue(T value)
        {
            return 0 == (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
        }

        /// <summary>
        /// Gets the value comparer.
        /// </summary>
        [JsonIgnore]
        protected virtual IComparer<T> ValueComparer { get; }
    }

    /// <summary>
    /// The simple interval with nullable value.
    /// </summary>
    [DataContract]
    public class NullableValueSimpleInterval<T> : SimpleInterval<T?> where T : struct, IComparable<T>
    {
        /// <summary>
        /// Initializes a new instance of the NullableValueSimpleInterval class.
        /// </summary>
        public NullableValueSimpleInterval()
        {
        }

        /// <summary>
        /// Initializes a new instance of the NullableValuedSimpleInterval class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="op">The operation.</param>
        public NullableValueSimpleInterval(T value, BasicCompareOperator op)
            : base(value, op, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NullableValueSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        public NullableValueSimpleInterval(T? valueA, T? valueB)
            : base(valueA, valueB)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NullableValueSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="autoFix">true if fix the order automatically; otherwise, false.</param>
        public NullableValueSimpleInterval(T? valueA, T? valueB, bool autoFix)
            : base(valueA, valueB, autoFix)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NullableValueSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
        /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
        public NullableValueSimpleInterval(T? valueA, T? valueB, bool leftOpen, bool rightOpen)
            : base(valueA, valueB, leftOpen || !valueA.HasValue, rightOpen || !valueB.HasValue)
        {
        }

        /// <summary>
        /// Gets a value indicating whether it is left bounded.
        /// </summary>
        [JsonIgnore]
        public override bool LeftBounded { get { return MinValue != null; } }

        /// <summary>
        /// Gets a value indicating whether it is right bounded.
        /// </summary>
        [JsonIgnore]
        public override bool RightBounded { get { return MaxValue != null; } }

        /// <summary>
        /// Checks if the value is less than MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
        public override bool IsLessThanMinValue(T? value)
        {
            if (MinValue == null) return false;
            if (value == null) return true;
            return 0 < (ValueComparer?.Compare(MinValue.Value, value.Value) ?? MinValue.Value.CompareTo(value.Value));
        }

        /// <summary>
        /// Checks if the value is greater than MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
        public override bool IsGreaterThanMinValue(T? value)
        {
            if (value == null) return false;
            if (MinValue == null) return true;
            return 0 > (ValueComparer?.Compare(MinValue.Value, value.Value) ?? MinValue.Value.CompareTo(value.Value));
        }

        /// <summary>
        /// Checks if the value equals MinValue.
        /// </summary>
        /// <param name="value">A value to compare with MinValue.</param>
        /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
        public override bool EqualsMinValue(T? value)
        {
            if (MinValue == null && value == null) return true;
            if (MinValue == null || value == null) return false;
            return 0 == (ValueComparer?.Compare(MinValue.Value, value.Value) ?? MinValue.Value.CompareTo(value.Value));
        }

        /// <summary>
        /// Checks if the value is less than MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
        public override bool IsLessThanMaxValue(T? value)
        {
            if (value == null) return false;
            if (MaxValue == null) return true;
            return 0 < (ValueComparer?.Compare(MaxValue.Value, value.Value) ?? MaxValue.Value.CompareTo(value.Value));
        }

        /// <summary>
        /// Checks if the value is greater than MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
        public override bool IsGreaterThanMaxValue(T? value)
        {
            if (MaxValue == null) return false;
            if (value == null) return true;
            return 0 > (ValueComparer?.Compare(MaxValue.Value, value.Value) ?? MaxValue.Value.CompareTo(value.Value));
        }

        /// <summary>
        /// Checks if the value equals MaxValue.
        /// </summary>
        /// <param name="value">A value to compare with MaxValue.</param>
        /// <returns>true if the specific value equals MinValue; otherwise, false.</returns>
        public override bool EqualsMaxValue(T? value)
        {
            if (MaxValue == null && value == null) return true;
            if (MaxValue == null || value == null) return false;
            return 0 == (ValueComparer?.Compare(MaxValue.Value, value.Value) ?? MaxValue.Value.CompareTo(value.Value));
        }

        /// <summary>
        /// Gets the value comparer.
        /// </summary>
        [JsonIgnore]
        protected virtual IComparer<T> ValueComparer { get; }
    }

    /// <summary>
    /// The semantic version interval with class value.
    /// </summary>
    [DataContract]
    public class VersionSimpleInterval : RefValueSimpleInterval<string>
    {
        private Version minVer;
        private Version maxVer;

        /// <summary>
        /// Initializes a new instance of the VersionSimpleInterval class.
        /// </summary>
        public VersionSimpleInterval()
        {
            OnInit();
        }

        /// <summary>
        /// Initializes a new instance of the VersionSimpleInterval class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="op">The operation.</param>
        public VersionSimpleInterval(string value, BasicCompareOperator op)
            : base(value, op)
        {
            OnInit();
        }

        /// <summary>
        /// Initializes a new instance of the VersionSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        public VersionSimpleInterval(string valueA, string valueB)
            : base(valueA, valueB)
        {
            OnInit();
        }

        /// <summary>
        /// Initializes a new instance of the VersionSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="autoFix">true if fix the order automatically; otherwise, false.</param>
        public VersionSimpleInterval(string valueA, string valueB, bool autoFix)
            : base(valueA, valueB, autoFix)
        {
            OnInit();
        }

        /// <summary>
        /// Initializes a new instance of the VersionSimpleInterval class.
        /// </summary>
        /// <param name="valueA">The first value.</param>
        /// <param name="valueB">The seconde value.</param>
        /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
        /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
        public VersionSimpleInterval(string valueA, string valueB, bool leftOpen, bool rightOpen)
            : base(valueA, valueB, leftOpen, rightOpen)
        {
            OnInit();
        }

        /// <summary>
        /// Gets the minimum version.
        /// </summary>
        [JsonIgnore]
        public Version MinVersion
        {
            get
            {
                var v = minVer;
                if (v == null) v = Reflection.VersionComparer.ToVersion(MinValue);
                return minVer = v;
            }
        }

        /// <summary>
        /// Gets the maximum version.
        /// </summary>
        [JsonIgnore]
        public Version MaxVersion
        {
            get
            {
                var v = maxVer;
                if (v == null) v = Reflection.VersionComparer.ToVersion(MaxValue);
                return maxVer = v;
            }
        }

        /// <summary>
        /// Checks if a value is in the interval.
        /// </summary>
        /// <param name="value">A value to check.</param>
        /// <returns>true if the value is in the interval; otherwise, false.</returns>
        public bool IsInInterval(Version value) => value != null && this.IsInInterval(value.ToString());

        /// <summary>
        /// Parses a version interval.
        /// </summary>
        /// <param name="s">The version interval string.</param>
        /// <returns>A version interval.</returns>
        public static VersionSimpleInterval Parse(string s)
        {
            return IntervalUtility.Parse(s, (left, leftOpen, right, rightOpen) => new VersionSimpleInterval(left, right, leftOpen, rightOpen), ele => ele, null);
        }

        /// <summary>
        /// Gets the value comparer.
        /// </summary>
        [JsonIgnore]
        protected override IComparer<string> ValueComparer { get; } = new Reflection.VersionComparer { IsWideX = true };

        private void OnInit()
        {
            MinValueChanged += (sender, ev) =>
            {
                minVer = null;
            };
            MaxValueChanged += (sender, ev) =>
            {
                maxVer = null;
            };
        }
    }
}
