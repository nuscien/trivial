using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Trivial.Web;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a specific string JSON value with source.
    /// </summary>
    public interface IJsonNumber : IJsonValue, IEquatable<IJsonNumber>, IEquatable<long>, IEquatable<int>, IEquatable<double>, IEquatable<float>
    {
        /// <summary>
        /// Gets a value indicating whether the number value is an whole number.
        /// </summary>
        public bool IsInteger { get; }

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        public decimal GetDecimal();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        public float GetSingle();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        public double GetDouble();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        public ushort GetUInt16();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        public short GetInt16();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        public uint GetUInt32();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        public int GetInt32();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        public long GetInt64();
    }

    /// <summary>
    /// Represents a specific JSON integer number value.
    /// </summary>
    public class JsonInteger : IJsonValue<long>, IJsonValueResolver, IJsonNumber, IComparable<JsonInteger>, IComparable<JsonDouble>, IComparable<uint>, IComparable<int>, IComparable<long>, IComparable<double>, IComparable<float>, IEquatable<IJsonValue<uint>>, IEquatable<IJsonValue<int>>, IEquatable<IJsonValue<float>>, IEquatable<IJsonValue<double>>, IEquatable<uint>, IEquatable<int>, IEquatable<float>, IEquatable<double>, IFormattable
    {
        /// <summary>
        /// Maximum safe integer in JavaScript and JSON.
        /// </summary>
        public const long MaxSafeInteger = 9007199254740991;

        /// <summary>
        /// Minimum safe integer in JavaScript and JSON.
        /// </summary>
        public const long MinSafeInteger = -9007199254740991;

        /// <summary>
        /// Initializes a new instance of the JsonInteger class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonInteger(int value)
        {
            Value = value;
            IsSafe = true;
        }

        /// <summary>
        /// Initializes a new instance of the JsonInteger class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonInteger(uint value)
        {
            Value = value;
            IsSafe = true;
        }

        /// <summary>
        /// Initializes a new instance of the JsonInteger class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonInteger(long value)
        {
            Value = value;
            IsSafe = value <= MaxSafeInteger && value >= MinSafeInteger;
        }

        /// <summary>
        /// Initializes a new instance of the JsonInteger class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonInteger(double value)
        {
            Value = (long)Math.Round(value);
            IsSafe = value <= MaxSafeInteger && value >= MinSafeInteger;
        }

        /// <summary>
        /// Initializes a new instance of the JsonInteger class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isUnixTimestamp">true if uses Unix timestamp; otherwise, false, to use JavaScript ticks, by default.</param>
        public JsonInteger(DateTime value, bool isUnixTimestamp = false)
        {
            Value = isUnixTimestamp ? Web.WebFormat.ParseUnixTimestamp(value) : Web.WebFormat.ParseDate(value);
            IsSafe = true;
        }

        /// <summary>
        /// Initializes a new instance of the JsonInteger class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonInteger(TimeSpan value)
        {
            Value = (long)value.TotalSeconds;
            IsSafe = true;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public long Value { get; }

        /// <summary>
        /// Gets a value indicating whether the number value is an whole number.
        /// </summary>
        public bool IsInteger => true;

        /// <summary>
        /// Gets a value indicating whether the integer is safe.
        /// </summary>
        public bool IsSafe { get; }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind => JsonValueKind.Number;

        /// <summary>
        /// Converts to a date time instance.
        /// </summary>
        /// <param name="isUnixTimestamp">true if uses Unix timestamp; otherwise, false, to use JavaScript ticks, by default.</param>
        /// <returns>A date time.</returns>
        public DateTime ToDateTime(bool isUnixTimestamp = false)
        {
            return isUnixTimestamp ? Web.WebFormat.ParseUnixTimestamp(Value) : Web.WebFormat.ParseDate(Value);
        }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns>The JSON format string of the integer.</returns>
        public override string ToString()
        {
            return Value.ToString("g", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
        /// <returns>The JSON format string of the integer.</returns>
        public string ToString(IFormatProvider provider)
        {
            return Value.ToString(provider);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
        /// <returns>The JSON format string of the integer.</returns>
        public string ToString(string format, IFormatProvider provider = null)
        {
            return provider != null ? Value.ToString(format, provider) : Value.ToString(format);
        }

        /// <summary>
        /// Converts to a specific positional notation format string.
        /// </summary>
        /// <param name="type">The positional notation.</param>
        /// <returns>A string of the number in the specific positional notation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">type should be in 2-36.</exception>
        public string ToPositionalNotationString(int type)
        {
            return Maths.Arithmetic.ToPositionalNotationString(Value, type);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<double> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<float> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<long> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<int> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<uint> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonNumber other)
        {
            if (other is null || !other.IsInteger) return false;
            return Value.Equals(other.GetInt64());
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(double other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(float other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(long other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(uint other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(int other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is IJsonValue<double> dJson) return Value.Equals(dJson.Value);
            if (other is IJsonValue<long> lJson) return Value.Equals(lJson.Value);
            if (other is IJsonValue nJson && nJson.ValueKind == JsonValueKind.Number)
            {
                if (other is IJsonValue<float> fJson) return Value.Equals(fJson.Value);
                if (other is IJsonValue<int> iJson) return Value.Equals(iJson.Value);
                if (other is IJsonValue<uint> uiJson) return Value.Equals(uiJson.Value);
                if (other is IJsonValue<short> sJson) return Value.Equals(sJson.Value);
                if (other is IJsonValue<ulong> ulJson) return Value.Equals(ulJson.Value);
                if (other is IJsonValue<decimal> dcmJson) return Value.Equals(dcmJson.Value);
                if (other is IJsonValue<ushort> usJson) return Value.Equals(usJson.Value);
                return ToString().Equals(other.ToString(), StringComparison.InvariantCultureIgnoreCase);
            }

            return Value.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(JsonInteger other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(JsonDouble other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(long other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(int other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(uint other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(double other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(float other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Gets the item value count.
        /// It always return 0 because it is not an array or object.
        /// </summary>
        public int Count => 0;

        /// <summary>
        /// Gets the value of the element as a boolean.
        /// </summary>
        /// <returns>The value of the element as a boolean.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        bool IJsonValueResolver.GetBoolean()
        {
            if (Value == 0) return false;
            if (Value == 1) return true;
            throw new InvalidOperationException("Expect a boolean but it is a number.");
        }

        /// <summary>
        /// Gets the value of the element as a byte array.
        /// </summary>
        /// <returns>The value decoded as a byte array.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        byte[] IJsonValueResolver.GetBytesFromBase64() => throw new InvalidOperationException("Expect a string but it is a number.");

        /// <summary>
        /// Gets the value of the element as a date time.
        /// </summary>
        /// <returns>The value of the element as a date time.</returns>
        public DateTime GetDateTime()
        {
            return Web.WebFormat.ParseDate(Value);
        }

        /// <summary>
        /// Gets the value of the element as a date time.
        /// </summary>
        /// <param name="useUnixTimestamps">true if use Unix timestamp to convert; otherwise, false, to use JavaScript date ticks.</param>
        /// <returns>The value of the element as a date time.</returns>
        public DateTime GetDateTime(bool useUnixTimestamps)
        {
            return useUnixTimestamps ? Web.WebFormat.ParseUnixTimestamp(Value) : Web.WebFormat.ParseDate(Value);
        }

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        decimal IJsonValueResolver.GetDecimal() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        float IJsonValueResolver.GetSingle() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        double IJsonValueResolver.GetDouble() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Int64.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        short IJsonValueResolver.GetInt16() => (short)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Int64.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        uint IJsonValueResolver.GetUInt32() => (uint)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Int64.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        int IJsonValueResolver.GetInt32() => (int)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        long IJsonValueResolver.GetInt64() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        decimal IJsonNumber.GetDecimal() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        float IJsonNumber.GetSingle() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        double IJsonNumber.GetDouble() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Int64.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        ushort IJsonNumber.GetUInt16() => (ushort)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Int64.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        short IJsonNumber.GetInt16() => (short)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Int64.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        uint IJsonNumber.GetUInt32() => (uint)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Int64.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        int IJsonNumber.GetInt32() => (int)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        long IJsonNumber.GetInt64() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        string IJsonValueResolver.GetString() => ToString();

        /// <summary>
        /// Gets the value of the element as a GUID.
        /// </summary>
        /// <returns>The value of the element as a GUID.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        Guid IJsonValueResolver.GetGuid() => throw new InvalidOperationException("Expect a string but it is a number.");

        /// <summary>
        /// Tries to get the value of the element as a boolean.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetBoolean(out bool result)
        {
            if (Value == 0)
            {
                result = false;
                return true;
            }

            if (Value == 1)
            {
                result = true;
                return true;
            }

            result = false;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a date time.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDateTime(out DateTime result)
        {
            result = WebFormat.ParseDate(Value);
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDecimal(out decimal result)
        {
            try
            {
                result = Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetSingle(out float result)
        {
            try
            {
                result = Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDouble(out double result)
        {
            try
            {
                result = Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetUInt16(out ushort result)
        {
            try
            {
                result = (ushort)Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetUInt32(out uint result)
        {
            try
            {
                result = (uint)Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetInt32(out int result)
        {
            try
            {
                result = (int)Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetInt64(out long result)
        {
            result = Value;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetString(out string result)
        {
            result = ToString();
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a GUID.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetGuid(out Guid result)
        {
            result = Guid.Empty;
            return false;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(string key) => throw new InvalidOperationException("Expect an object but it is a number.");

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(ReadOnlySpan<char> key) => throw new InvalidOperationException("Expect an object but it is a number.");

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetValue(string key, out IJsonValueResolver result)
        {
            result = default;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetValue(ReadOnlySpan<char> key, out IJsonValueResolver result)
        {
            result = default;
            return false;
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is a number.");

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetValue(int index, out IJsonValueResolver result)
        {
            result = default;
            return false;
        }

#if !NETOLDVER
        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is a number.");

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetValue(Index index, out IJsonValueResolver result)
        {
            result = default;
            return false;
        }
#endif

        /// <summary>
        /// Gets all property keys.
        /// </summary>
        /// <returns>The property keys.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
        IEnumerable<string> IJsonValueResolver.GetKeys() => throw new InvalidOperationException("Expect an object but it is a number.");

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonInteger(uint value)
        {
            return new JsonInteger(value);
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonInteger(int value)
        {
            return new JsonInteger(value);
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonInteger(long value)
        {
            return new JsonInteger(value);
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator long(JsonInteger json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator int(JsonInteger json)
        {
            return (int)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator uint(JsonInteger json)
        {
            return (uint)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator float(JsonInteger json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator double(JsonInteger json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator System.Numerics.BigInteger(JsonInteger json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A string.</returns>
        public static explicit operator string(JsonInteger json)
        {
            return json.ToString();
        }

        /// <summary>
        /// Converts to a JSON string.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A JSON string instance.</returns>
        public static explicit operator JsonString(JsonInteger json)
        {
            return new JsonString(json.ToString());
        }

        /// <summary>
        /// Converts to a JSON double object.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A JSON double instance.</returns>
        public static explicit operator JsonDouble(JsonInteger json)
        {
            return new JsonDouble(json.Value);
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonInteger leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Value == rightValue.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonInteger leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return true;
            return leftValue.Value != rightValue.Value;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonInteger leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return true;
            return leftValue.Value < rightValue.Value;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonInteger leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return false;
            return leftValue.Value < rightValue.Value;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonInteger leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return true;
            return leftValue.Value <= rightValue.Value;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonInteger leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Value <= rightValue.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonInteger leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Value == rightValue.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonInteger leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return true;
            return leftValue.Value != rightValue.Value;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonInteger leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return true;
            return leftValue.Value < rightValue.Value;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonInteger leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return false;
            return leftValue.Value < rightValue.Value;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonInteger leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return true;
            return leftValue.Value <= rightValue.Value;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonInteger leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Value <= rightValue.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonInteger leftValue, long rightValue)
        {
            return leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonInteger leftValue, long rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonInteger leftValue, long rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonInteger leftValue, long rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonInteger leftValue, long rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonInteger leftValue, long rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonInteger leftValue, int rightValue)
        {
            return leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonInteger leftValue, int rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonInteger leftValue, int rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonInteger leftValue, int rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonInteger leftValue, int rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonInteger leftValue, int rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonInteger leftValue, uint rightValue)
        {
            return leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonInteger leftValue, uint rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonInteger leftValue, uint rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonInteger leftValue, uint rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonInteger leftValue, uint rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonInteger leftValue, uint rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonInteger leftValue, double rightValue)
        {
            return leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonInteger leftValue, double rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonInteger leftValue, double rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonInteger leftValue, double rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonInteger leftValue, double rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonInteger leftValue, double rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonInteger leftValue, float rightValue)
        {
            return leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonInteger leftValue, float rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonInteger leftValue, float rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonInteger leftValue, float rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonInteger leftValue, float rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonInteger leftValue, float rightValue)
        {
            return leftValue.Value <= rightValue;
        }
    }

    /// <summary>
    /// Represents a specific JSON double float number value.
    /// </summary>
    public class JsonDouble : IJsonValue<double>, IJsonValueResolver, IJsonNumber, IComparable<JsonInteger>, IComparable<JsonDouble>, IComparable<uint>, IComparable<int>, IComparable<long>, IComparable<double>, IComparable<float>, IEquatable<IJsonValue<uint>>, IEquatable<IJsonValue<int>>, IEquatable<IJsonValue<long>>, IEquatable<IJsonValue<float>>, IEquatable<uint>, IEquatable<int>, IEquatable<long>, IEquatable<float>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the JsonDouble class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonDouble(int value)
        {
            Value = value;
            ValueKind = JsonValueKind.Number;
            IsInteger = true;
        }

        /// <summary>
        /// Initializes a new instance of the JsonDouble class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonDouble(long value)
        {
            Value = value;
            ValueKind = JsonValueKind.Number;
            IsInteger = true;
        }

        /// <summary>
        /// Initializes a new instance of the JsonDouble class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonDouble(uint value)
        {
            Value = value;
            ValueKind = JsonValueKind.Number;
            IsInteger = true;
        }

        /// <summary>
        /// Initializes a new instance of the JsonDouble class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonDouble(ulong value)
        {
            Value = value;
            ValueKind = JsonValueKind.Number;
            IsInteger = true;
        }

        /// <summary>
        /// Initializes a new instance of the JsonDoubleValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonDouble(float value)
        {
            Value = value;
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                ValueKind = JsonValueKind.Null;
                return;
            }

            ValueKind = JsonValueKind.Number;
            try
            {
                if (value > long.MaxValue || value < long.MinValue) return;
                IsInteger = (long)value == value;
            }
            catch (InvalidCastException)
            {
            }
            catch (OverflowException)
            {
            }
        }

        /// <summary>
        /// Initializes a new instance of the JsonDouble class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonDouble(double value)
        {
            Value = value;
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                ValueKind = JsonValueKind.Null;
                return;
            }

            ValueKind = JsonValueKind.Number;
            try
            {
                if (value > long.MaxValue || value < long.MinValue) return;
                IsInteger = (long)value == value;
            }
            catch (InvalidCastException)
            {
            }
            catch (OverflowException)
            {
            }
        }

        /// <summary>
        /// Initializes a new instance of the JsonDouble class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonDouble(decimal value)
        {
            Value = (double)value;
            ValueKind = JsonValueKind.Number;
            try
            {
                if (value > long.MaxValue || value < long.MinValue) return;
                IsInteger = (long)value == value;
            }
            catch (InvalidCastException)
            {
            }
            catch (OverflowException)
            {
            }
        }

        /// <summary>
        /// Initializes a new instance of the JsonDouble class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonDouble(TimeSpan value)
        {
            Value = value.TotalSeconds;
            ValueKind = JsonValueKind.Number;
            IsInteger = value.Milliseconds == 0;
        }

        /// <summary>
        /// Gets a value indicating whether the number value is an whole number.
        /// </summary>
        public bool IsInteger { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind { get; }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns>The JSON format string of the float number.</returns>
        public override string ToString()
        {
            return Value.ToString("g", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
        /// <returns>The JSON format string of the integer.</returns>
        public string ToString(IFormatProvider provider)
        {
            return Value.ToString(provider);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
        /// <returns>The JSON format string of the integer.</returns>
        public string ToString(string format, IFormatProvider provider = null)
        {
            return provider != null ? Value.ToString(format, provider) : Value.ToString(format);
        }

        /// <summary>
        /// Converts to a specific positional notation format string.
        /// </summary>
        /// <param name="type">The positional notation.</param>
        /// <returns>A string of the number in the specific positional notation.</returns>
        public string ToPositionalNotationString(int type)
        {
            return Maths.Arithmetic.ToPositionalNotationString(Value, type);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<double> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<float> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<long> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<int> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<uint> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonNumber other)
        {
            if (other is null) return false;
            return Value.Equals(other.GetDouble());
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(double other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(float other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(long other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(uint other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(int other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is IJsonValue<double> dJson) return Value.Equals(dJson.Value);
            if (other is IJsonValue<long> lJson) return Value.Equals(lJson.Value);
            if (other is IJsonValue nJson && nJson.ValueKind == JsonValueKind.Number)
            {
                if (other is IJsonValue<float> fJson) return Value.Equals(fJson.Value);
                if (other is IJsonValue<int> iJson) return Value.Equals(iJson.Value);
                if (other is IJsonValue<uint> uiJson) return Value.Equals(uiJson.Value);
                if (other is IJsonValue<short> sJson) return Value.Equals(sJson.Value);
                if (other is IJsonValue<ulong> ulJson) return Value.Equals(ulJson.Value);
                if (other is IJsonValue<decimal> dcmJson) return Value.Equals(dcmJson.Value);
                if (other is IJsonValue<ushort> usJson) return Value.Equals(usJson.Value);
                return ToString().Equals(other.ToString(), StringComparison.InvariantCultureIgnoreCase);
            }

            return Value.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(JsonInteger other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(JsonDouble other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(long other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(int other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(uint other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(double other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Compares this instance to a specified number and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A number to compare.</param>
        /// <returns>
        /// <para>
        /// A signed number indicating the relative values of this instance and value.
        /// </para>
        /// <list type="bullet">
        /// <listheader>Return Value Description:</listheader>
        /// <item>Less than zero This instance is less than value;</item>
        /// <item>Zero This instance is equal to value;</item>
        /// <item>Greater than zero This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(float other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Gets the item value count.
        /// It always return 0 because it is not an array or object.
        /// </summary>
        public int Count => 0;

        /// <summary>
        /// Gets the value of the element as a boolean.
        /// </summary>
        /// <returns>The value of the element as a boolean.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        bool IJsonValueResolver.GetBoolean()
        {
            if (Value == 0) return false;
            if (Value == 1) return true;
            throw new InvalidOperationException("Expect a boolean but it is a number.");
        }

        /// <summary>
        /// Gets the value of the element as a byte array.
        /// </summary>
        /// <returns>The value decoded as a byte array.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        byte[] IJsonValueResolver.GetBytesFromBase64() => throw new InvalidOperationException("Expect a string but it is a number.");

        /// <summary>
        /// Gets the value of the element as a date time.
        /// </summary>
        /// <returns>The value of the element as a date time.</returns>
        /// <exception cref="InvalidCastException">The value is an Double but expect a Int64.</exception>
        DateTime IJsonValueResolver.GetDateTime()
        {
            return Web.WebFormat.ParseDate((long)Value);
        }

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        decimal IJsonValueResolver.GetDecimal() => (decimal)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        float IJsonValueResolver.GetSingle() => (float)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        double IJsonValueResolver.GetDouble() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        short IJsonValueResolver.GetInt16() => (short)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        uint IJsonValueResolver.GetUInt32() => (uint)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        int IJsonValueResolver.GetInt32() => (int)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        long IJsonValueResolver.GetInt64() => (long)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        decimal IJsonNumber.GetDecimal() => (decimal)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        float IJsonNumber.GetSingle() => (float)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        double IJsonNumber.GetDouble() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        ushort IJsonNumber.GetUInt16() => (ushort)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        short IJsonNumber.GetInt16() => (short)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        uint IJsonNumber.GetUInt32() => (uint)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        int IJsonNumber.GetInt32() => (int)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidCastException">The value is an Double.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        long IJsonNumber.GetInt64() => (long)Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        string IJsonValueResolver.GetString() => ToString();

        /// <summary>
        /// Gets the value of the element as a GUID.
        /// </summary>
        /// <returns>The value of the element as a GUID.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        Guid IJsonValueResolver.GetGuid() => throw new InvalidOperationException("Expect a string but it is a number.");

        /// <summary>
        /// Tries to get the value of the element as a boolean.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetBoolean(out bool result)
        {
            if (Value == 0)
            {
                result = false;
                return true;
            }

            if (Value == 1)
            {
                result = true;
                return true;
            }

            result = false;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a date time.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetDateTime(out DateTime result)
        {
            try
            {
                if (double.IsNaN(Value))
                {
                    result = WebFormat.ParseDate(0);
                    return false;
                }

                result = WebFormat.ParseDate((long)Value);
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = WebFormat.ParseDate(0);
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetDecimal(out decimal result)
        {
            try
            {
                if (double.IsNaN(Value))
                    result = 0;
                else
                    result = (decimal)Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetSingle(out float result)
        {
            try
            {
                result = (float)Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDouble(out double result)
        {
            result = Value;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetUInt32(out uint result)
        {
            try
            {
                if (double.IsNaN(Value))
                    result = 0;
                else
                    result = (uint)Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetInt32(out int result)
        {
            try
            {
                if (double.IsNaN(Value))
                    result = 0;
                else
                    result = (int)Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetInt64(out long result)
        {
            try
            {
                if (double.IsNaN(Value))
                    result = 0;
                else
                    result = (long)Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetUInt64(out ulong result)
        {
            try
            {
                if (double.IsNaN(Value))
                    result = 0;
                else
                    result = (ulong)Value;
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (InvalidCastException)
            {
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetString(out string result)
        {
            result = ToString();
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a GUID.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetGuid(out Guid result)
        {
            result = Guid.Empty;
            return false;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(string key) => throw new InvalidOperationException("Expect an object but it is a number.");

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(ReadOnlySpan<char> key) => throw new InvalidOperationException("Expect an object but it is a number.");

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetValue(string key, out IJsonValueResolver result)
        {
            result = default;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetValue(ReadOnlySpan<char> key, out IJsonValueResolver result)
        {
            result = default;
            return false;
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is a number.");

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetValue(int index, out IJsonValueResolver result)
        {
            result = default;
            return false;
        }

#if !NETOLDVER
        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is a number.");

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetValue(Index index, out IJsonValueResolver result)
        {
            result = default;
            return false;
        }
#endif

        /// <summary>
        /// Gets all property keys.
        /// </summary>
        /// <returns>The property keys.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
        IEnumerable<string> IJsonValueResolver.GetKeys() => throw new InvalidOperationException("Expect an object but it is a number.");

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonDouble(float value)
        {
            return new JsonDouble(value);
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonDouble(double value)
        {
            return new JsonDouble(value);
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator double(JsonDouble json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator float(JsonDouble json)
        {
            return (float)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator uint(JsonDouble json)
        {
            return (uint)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator int(JsonDouble json)
        {
            return (int)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator ulong(JsonDouble json)
        {
            return (ulong)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator long(JsonDouble json)
        {
            return (long)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A string.</returns>
        public static explicit operator string(JsonDouble json)
        {
            return json.ToString();
        }

        /// <summary>
        /// Converts to a JSON string.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A JSON string instance.</returns>
        public static explicit operator JsonString(JsonDouble json)
        {
            return new JsonString(json.ToString());
        }

        /// <summary>
        /// Converts to a JSON integer object.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A JSON integer instance.</returns>
        public static explicit operator JsonInteger(JsonDouble json)
        {
            return new JsonInteger((long)json.Value);
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonDouble leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Value == rightValue.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonDouble leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return true;
            return leftValue.Value != rightValue.Value;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonDouble leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return true;
            return leftValue.Value < rightValue.Value;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonDouble leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return false;
            return leftValue.Value < rightValue.Value;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonDouble leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return true;
            return leftValue.Value <= rightValue.Value;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonDouble leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Value <= rightValue.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonDouble leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Value == rightValue.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonDouble leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return true;
            return leftValue.Value != rightValue.Value;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonDouble leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return true;
            return leftValue.Value < rightValue.Value;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonDouble leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return false;
            return leftValue.Value < rightValue.Value;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonDouble leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return true;
            return leftValue.Value <= rightValue.Value;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonDouble leftValue, IJsonValue<long> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Value <= rightValue.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonDouble leftValue, double rightValue)
        {
            return leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonDouble leftValue, double rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonDouble leftValue, double rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonDouble leftValue, double rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonDouble leftValue, double rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonDouble leftValue, double rightValue)
        {
            return leftValue.Value <= rightValue;
        }
        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonDouble leftValue, float rightValue)
        {
            return leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonDouble leftValue, float rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonDouble leftValue, float rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonDouble leftValue, float rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonDouble leftValue, float rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonDouble leftValue, float rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonDouble leftValue, long rightValue)
        {
            return leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonDouble leftValue, long rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonDouble leftValue, long rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonDouble leftValue, long rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonDouble leftValue, long rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonDouble leftValue, long rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonDouble leftValue, int rightValue)
        {
            return leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonDouble leftValue, int rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonDouble leftValue, int rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonDouble leftValue, int rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonDouble leftValue, int rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonDouble leftValue, int rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonDouble leftValue, uint rightValue)
        {
            return leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonDouble leftValue, uint rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonDouble leftValue, uint rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonDouble leftValue, uint rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonDouble leftValue, uint rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonDouble leftValue, uint rightValue)
        {
            return leftValue.Value <= rightValue;
        }
    }
}
