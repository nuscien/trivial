using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a specific JSON integer number value.
    /// </summary>
    public struct JsonIntegerValue : IJsonValue<long>, IComparable<JsonIntegerValue>, IComparable<JsonFloatValue>, IComparable<uint>, IComparable<int>, IComparable<long>, IComparable<double>, IComparable<float>, IEquatable<IJsonValue<uint>>, IEquatable<IJsonValue<int>>, IEquatable<IJsonValue<float>>, IEquatable<IJsonValue<double>>, IEquatable<uint>, IEquatable<int>, IEquatable<float>, IEquatable<double>, IFormattable
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public long Value { get; }

        /// <summary>
        /// Initializes a new instance of the JsonIntegerValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonIntegerValue(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the JsonIntegerValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonIntegerValue(uint value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the JsonIntegerValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonIntegerValue(long value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the JsonIntegerValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isUnixTimestamp">true if uses Unix timestamp; otherwise, false, to use JavaScript ticks, by default.</param>
        public JsonIntegerValue(DateTime value, bool isUnixTimestamp = false)
        {
            Value = isUnixTimestamp ? Web.WebFormat.ParseUnixTimestamp(value) : Web.WebFormat.ParseDate(value);
        }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind => JsonValueKind.Number;

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
            }

            return Value.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
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
        public int CompareTo(JsonIntegerValue other)
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
        public int CompareTo(JsonFloatValue other)
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
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonIntegerValue(uint value)
        {
            return new JsonIntegerValue(value);
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonIntegerValue(int value)
        {
            return new JsonIntegerValue(value);
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonIntegerValue(long value)
        {
            return new JsonIntegerValue(value);
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator long(JsonIntegerValue json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator int(JsonIntegerValue json)
        {
            return (int)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator uint(JsonIntegerValue json)
        {
            return (uint)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator float(JsonIntegerValue json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator double(JsonIntegerValue json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A string.</returns>
        public static explicit operator string(JsonIntegerValue json)
        {
            return json.ToString();
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonIntegerValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator !=(JsonIntegerValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator <(JsonIntegerValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator >(JsonIntegerValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator <=(JsonIntegerValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator >=(JsonIntegerValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator ==(JsonIntegerValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator !=(JsonIntegerValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator <(JsonIntegerValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator >(JsonIntegerValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator <=(JsonIntegerValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator >=(JsonIntegerValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator ==(JsonIntegerValue leftValue, long rightValue)
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
        public static bool operator !=(JsonIntegerValue leftValue, long rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonIntegerValue leftValue, long rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonIntegerValue leftValue, long rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonIntegerValue leftValue, long rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonIntegerValue leftValue, long rightValue)
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
        public static bool operator ==(JsonIntegerValue leftValue, int rightValue)
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
        public static bool operator !=(JsonIntegerValue leftValue, int rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonIntegerValue leftValue, int rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonIntegerValue leftValue, int rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonIntegerValue leftValue, int rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonIntegerValue leftValue, int rightValue)
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
        public static bool operator ==(JsonIntegerValue leftValue, uint rightValue)
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
        public static bool operator !=(JsonIntegerValue leftValue, uint rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonIntegerValue leftValue, uint rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonIntegerValue leftValue, uint rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonIntegerValue leftValue, uint rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonIntegerValue leftValue, uint rightValue)
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
        public static bool operator ==(JsonIntegerValue leftValue, double rightValue)
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
        public static bool operator !=(JsonIntegerValue leftValue, double rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonIntegerValue leftValue, double rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonIntegerValue leftValue, double rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonIntegerValue leftValue, double rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonIntegerValue leftValue, double rightValue)
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
        public static bool operator ==(JsonIntegerValue leftValue, float rightValue)
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
        public static bool operator !=(JsonIntegerValue leftValue, float rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonIntegerValue leftValue, float rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonIntegerValue leftValue, float rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonIntegerValue leftValue, float rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonIntegerValue leftValue, float rightValue)
        {
            return leftValue.Value <= rightValue;
        }
    }

    /// <summary>
    /// Represents a specific JSON float number value.
    /// </summary>
    public struct JsonFloatValue : IJsonValue<double>, IComparable<JsonIntegerValue>, IComparable<JsonFloatValue>, IComparable<uint>, IComparable<int>, IComparable<long>, IComparable<double>, IComparable<float>, IEquatable<IJsonValue<uint>>, IEquatable<IJsonValue<int>>, IEquatable<IJsonValue<long>>, IEquatable<IJsonValue<float>>, IEquatable<uint>, IEquatable<int>, IEquatable<long>, IEquatable<float>, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the JsonFloatValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonFloatValue(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the JsonFloatValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonFloatValue(long value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the JsonFloatValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonFloatValue(float value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the JsonFloatValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonFloatValue(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind => JsonValueKind.Number;

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
            }

            return Value.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
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
        public int CompareTo(JsonIntegerValue other)
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
        public int CompareTo(JsonFloatValue other)
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
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonFloatValue(float value)
        {
            return new JsonFloatValue(value);
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonFloatValue(double value)
        {
            return new JsonFloatValue(value);
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator double(JsonFloatValue json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator float(JsonFloatValue json)
        {
            return (float)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator uint(JsonFloatValue json)
        {
            return (uint)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator int(JsonFloatValue json)
        {
            return (int)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator ulong(JsonFloatValue json)
        {
            return (ulong)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator long(JsonFloatValue json)
        {
            return (long)json.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonFloatValue leftValue, IJsonValue<double> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Value == rightValue.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A string.</returns>
        public static explicit operator string(JsonFloatValue json)
        {
            return json.ToString();
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonFloatValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator <(JsonFloatValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator >(JsonFloatValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator <=(JsonFloatValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator >=(JsonFloatValue leftValue, IJsonValue<double> rightValue)
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
        public static bool operator ==(JsonFloatValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator !=(JsonFloatValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator <(JsonFloatValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator >(JsonFloatValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator <=(JsonFloatValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator >=(JsonFloatValue leftValue, IJsonValue<long> rightValue)
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
        public static bool operator ==(JsonFloatValue leftValue, double rightValue)
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
        public static bool operator !=(JsonFloatValue leftValue, double rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonFloatValue leftValue, double rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonFloatValue leftValue, double rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonFloatValue leftValue, double rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonFloatValue leftValue, double rightValue)
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
        public static bool operator ==(JsonFloatValue leftValue, float rightValue)
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
        public static bool operator !=(JsonFloatValue leftValue, float rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonFloatValue leftValue, float rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonFloatValue leftValue, float rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonFloatValue leftValue, float rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonFloatValue leftValue, float rightValue)
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
        public static bool operator ==(JsonFloatValue leftValue, long rightValue)
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
        public static bool operator !=(JsonFloatValue leftValue, long rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonFloatValue leftValue, long rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonFloatValue leftValue, long rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonFloatValue leftValue, long rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonFloatValue leftValue, long rightValue)
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
        public static bool operator ==(JsonFloatValue leftValue, int rightValue)
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
        public static bool operator !=(JsonFloatValue leftValue, int rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonFloatValue leftValue, int rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonFloatValue leftValue, int rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonFloatValue leftValue, int rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonFloatValue leftValue, int rightValue)
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
        public static bool operator ==(JsonFloatValue leftValue, uint rightValue)
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
        public static bool operator !=(JsonFloatValue leftValue, uint rightValue)
        {
            return leftValue.Value != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(JsonFloatValue leftValue, uint rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(JsonFloatValue leftValue, uint rightValue)
        {
            return leftValue.Value < rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(JsonFloatValue leftValue, uint rightValue)
        {
            return leftValue.Value <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(JsonFloatValue leftValue, uint rightValue)
        {
            return leftValue.Value <= rightValue;
        }
    }
}
