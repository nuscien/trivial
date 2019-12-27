using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a specific JSON value.
    /// </summary>
    public interface IJsonValue
    {
        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind { get; }
    }

    /// <summary>
    /// Represents a specific JSON value with source.
    /// </summary>
    /// <typeparam name="T">The type of source value.</typeparam>
    public interface IJsonValue<T> : IJsonValue, IEquatable<IJsonValue<T>>, IEquatable<T>
    {
        /// <summary>
        /// Gets the source value.
        /// </summary>
        public T Value { get; }
    }

    /// <summary>
    /// Represents a specific JSON value with source.
    /// </summary>
    public interface IJsonValueResolver : IJsonValue
    {
        /// <summary>
        /// Gets the item value count.
        /// </summary>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        public int Count { get; }

        /// <summary>
        /// Gets the value of the element as a boolean.
        /// </summary>
        /// <returns>The value of the element as a boolean.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        public bool GetBoolean();

        /// <summary>
        /// Gets the value of the element as a byte array.
        /// </summary>
        /// <returns>The value decoded as a byte array.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="FormatException">The value is not encoded as Base64 text and hence cannot be decoded to bytes.</exception>
        public byte[] GetBytesFromBase64();

        /// <summary>
        /// Gets the value of the element as a date time.
        /// </summary>
        /// <returns>The value of the element as a date time.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="FormatException">The value is not formatted for a date time.</exception>
        public DateTime GetDateTime();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        public decimal GetDecimal();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
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
        public short GetInt16();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        public uint GetUInt32();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        public int GetInt32();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        public long GetInt64();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        public string GetString();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="FormatException">The value is not formatted for a Guid.</exception>
        public Guid GetGuid();

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        public IJsonValueResolver GetValue(string key);

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        public IJsonValueResolver GetValue(ReadOnlySpan<char> key);

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        public IJsonValueResolver GetValue(int index);

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        public IJsonValueResolver GetValue(Index index);

        /// <summary>
        /// Gets all property keys.
        /// </summary>
        /// <returns>The property keys.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
        public IEnumerable<string> GetKeys();
    }

    /// <summary>
    /// Represents a complex JSON value.
    /// </summary>
    public interface IJsonComplex : IJsonValue, ICloneable, IEnumerable
    {
        /// <summary>
        /// Gets the number of elements contained in the System.Collections.Generic.ICollection`1
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Deserializes.
        /// </summary>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public T Deserialize<T>(JsonSerializerOptions options = default);

        /// <summary>
        /// Writes this instance to the specified writer as a JSON value.
        /// </summary>
        /// <param name="writer">The writer to which to write this instance.</param>
        public void WriteTo(Utf8JsonWriter writer);

        /// <summary>
        /// Removes all items from the array.
        /// </summary>
        public void Clear();
    }

    /// <summary>
    /// Represents a specific JSON boolean value.
    /// </summary>
    public class JsonBoolean : IJsonValue<bool>, IJsonValueResolver
    {
        /// <summary>
        /// Represents the Boolean value true of JSON as a string.
        /// This field is read-only.
        /// </summary>
        public const string TrueString = "true";

        /// <summary>
        /// Represents the Boolean value false of JSON as a string.
        /// This field is read-only.
        /// </summary>
        public const string FalseString = "false";

        /// <summary>
        /// Represents the Boolean value true of JSON as a string.
        /// This field is read-only.
        /// </summary>
        public readonly static JsonBoolean True = new JsonBoolean(true);

        /// <summary>
        /// Represents the Boolean value false of JSON as a string.
        /// This field is read-only.
        /// </summary>
        public readonly static JsonBoolean False = new JsonBoolean(false);

        /// <summary>
        /// Initializes a new instance of the JsonBoolean class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonBoolean(bool value)
        {
            Value = value;
            ValueKind = value ? JsonValueKind.True : JsonValueKind.False;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind { get; }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns>The JSON format string of the boolean.</returns>
        public override string ToString()
        {
            return Value ? TrueString : FalseString;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<bool> other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(bool other)
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
            if (other is IJsonValue<bool> bJson) return Value.Equals(bJson.Value);
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
        /// Gets the item value count.
        /// </summary>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        public int Count => throw new InvalidOperationException("It is not an array nor object.");

        /// <summary>
        /// Gets the value of the element as a boolean.
        /// </summary>
        /// <returns>The value of the element as a boolean.</returns>
        bool IJsonValueResolver.GetBoolean() => Value;

        /// <summary>
        /// Gets the value of the element as a byte array.
        /// </summary>
        /// <returns>The value decoded as a byte array.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        byte[] IJsonValueResolver.GetBytesFromBase64() => throw new InvalidOperationException("Expect a string but it is a boolean value.");

        /// <summary>
        /// Gets the value of the element as a date time.
        /// </summary>
        /// <returns>The value of the element as a date time.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        DateTime IJsonValueResolver.GetDateTime() => throw new InvalidOperationException("Expect a date time but it is a boolean value.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        decimal IJsonValueResolver.GetDecimal() => Value ? 1 : 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        float IJsonValueResolver.GetSingle() => Value ? 1 : 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        double IJsonValueResolver.GetDouble() => Value ? 1 : 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        short IJsonValueResolver.GetInt16() => (short)(Value ? 1 : 0);

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        uint IJsonValueResolver.GetUInt32() => (uint)(Value ? 1 : 0);

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        int IJsonValueResolver.GetInt32() => Value ? 1 : 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        long IJsonValueResolver.GetInt64() => Value ? 1 : 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        string IJsonValueResolver.GetString() => ToString();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        Guid IJsonValueResolver.GetGuid() => throw new InvalidOperationException("Expect a string but it is a boolean value.");

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(string key) => throw new InvalidOperationException("Expect an object but it is a boolean value.");

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(ReadOnlySpan<char> key) => throw new InvalidOperationException("Expect an object but it is a boolean value.");

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is a boolean value.");

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is a boolean value.");

        /// <summary>
        /// Gets all property keys.
        /// </summary>
        /// <returns>The property keys.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
        IEnumerable<string> IJsonValueResolver.GetKeys() => throw new InvalidOperationException("Expect an object but it is a boolean value.");

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonBoolean(bool value)
        {
            return value ? True : False;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A boolean.</returns>
        public static explicit operator bool(JsonBoolean json)
        {
            return json?.Value ?? false;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A boolean.</returns>
        public static explicit operator bool?(JsonBoolean json)
        {
            return json?.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator int(JsonBoolean json)
        {
            if (json is null) return 0;
            return json.Value ? 1 : 0;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator int?(JsonBoolean json)
        {
            if (json is null) return null;
            return json.Value ? 1 : 0;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A string.</returns>
        public static explicit operator string(JsonBoolean json)
        {
            if (json is null) return null;
            return json.Value ? TrueString : FalseString;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonBoolean leftValue, IJsonValue<bool> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null || leftValue is null) return false;
            return leftValue.Value == rightValue.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonBoolean leftValue, IJsonValue<bool> rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null || leftValue is null) return true;
            return leftValue.Value != rightValue.Value;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonBoolean leftValue, bool rightValue)
        {
            return !(leftValue is null) && leftValue.Value == rightValue;
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonBoolean leftValue, bool rightValue)
        {
            return leftValue is null || leftValue.Value != rightValue;
        }

        /// <summary>
        /// And operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>true if they are all true; otherwise, false.</returns>
        public static bool operator &(JsonBoolean leftValue, bool rightValue)
        {
            return leftValue?.Value == true && rightValue;
        }

        /// <summary>
        /// And operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>true if they are all true; otherwise, false.</returns>
        public static bool operator &(JsonBoolean leftValue, JsonBoolean rightValue)
        {
            return leftValue?.Value == true && rightValue?.Value == true;
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>true if any is true; otherwise, false.</returns>
        public static bool operator |(JsonBoolean leftValue, bool rightValue)
        {
            return leftValue?.Value == true || rightValue;
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>true if any is true; otherwise, false.</returns>
        public static bool operator |(JsonBoolean leftValue, JsonBoolean rightValue)
        {
            return leftValue?.Value == true || rightValue?.Value == true;
        }
    }

    /// <summary>
    /// Json null.
    /// </summary>
    internal class JsonNull : IJsonValue, IJsonValueResolver, IEquatable<JsonNull>
    {
        /// <summary>
        /// Initializes a new instance of the JsonNull class.
        /// </summary>
        /// <param name="valueKind">The JSON value kind.</param>
        public JsonNull(JsonValueKind valueKind)
        {
            ValueKind = valueKind;
        }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind { get; }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns>The string null.</returns>
        public override string ToString()
        {
            return "null";
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(JsonNull other)
        {
            return true;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return true;
            if (other is IJsonValue json)
            {
                switch (ValueKind)
                {
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        return json.ValueKind == JsonValueKind.Null || json.ValueKind == JsonValueKind.Undefined;
                    case JsonValueKind.True:
                        return json.ValueKind == JsonValueKind.True;
                    case JsonValueKind.False:
                        return json.ValueKind == JsonValueKind.False;
                    default:
                        return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            return ValueKind.GetHashCode();
        }

        /// <summary>
        /// Gets the item value count.
        /// </summary>
        public int Count => 0;

        /// <summary>
        /// Gets the value of the element as a boolean.
        /// </summary>
        /// <returns>The value of the element as a boolean.</returns>
        bool IJsonValueResolver.GetBoolean() => false;

        /// <summary>
        /// Gets the value of the element as a byte array.
        /// </summary>
        /// <returns>The value decoded as a byte array.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        byte[] IJsonValueResolver.GetBytesFromBase64() => throw new InvalidOperationException("Expect a string but it is null.");

        /// <summary>
        /// Gets the value of the element as a date time.
        /// </summary>
        /// <returns>The value of the element as a date time.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        DateTime IJsonValueResolver.GetDateTime() => throw new InvalidOperationException("Expect a date time but it is null.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        decimal IJsonValueResolver.GetDecimal() => 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        float IJsonValueResolver.GetSingle() => 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        double IJsonValueResolver.GetDouble() => 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        short IJsonValueResolver.GetInt16() => 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        uint IJsonValueResolver.GetUInt32() => 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        int IJsonValueResolver.GetInt32() => 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        long IJsonValueResolver.GetInt64() => 0;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        string IJsonValueResolver.GetString() => null;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        Guid IJsonValueResolver.GetGuid() => throw new InvalidOperationException("Expect a string but it is null.");

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(string key) => throw new InvalidOperationException("Expect an object but it is null.");

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(ReadOnlySpan<char> key) => throw new InvalidOperationException("Expect an object but it is null.");

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is null.");

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is null.");

        /// <summary>
        /// Gets all property keys.
        /// </summary>
        /// <returns>The property keys.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
        IEnumerable<string> IJsonValueResolver.GetKeys() => throw new InvalidOperationException("Expect an object but it is null.");

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonNull leftValue, JsonNull rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (leftValue is null || rightValue is null) return false;
            return leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonNull leftValue, JsonNull rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (leftValue is null || rightValue is null) return false;
            return leftValue.Equals(rightValue);
        }
    }

    /// <summary>
    /// The extensions for class IJsonValue, JsonDocument, JsonElement, etc.
    /// </summary>
    public static class JsonValues
    {
        /// <summary>
        /// JSON null.
        /// </summary>
        public static readonly IJsonValueResolver Null = new JsonNull(JsonValueKind.Null);

        /// <summary>
        /// JSON undefined.
        /// </summary>
        public static readonly IJsonValueResolver Undefined = new JsonNull(JsonValueKind.Undefined);

        /// <summary>
        /// Converts from JSON document.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        public static IJsonValueResolver ToJsonValue(JsonDocument json)
        {
            return ToJsonValue(json.RootElement);
        }

        /// <summary>
        /// Converts from JSON element.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        public static IJsonValueResolver ToJsonValue(JsonElement json)
        {
            return json.ValueKind switch
            {
                JsonValueKind.Undefined => Undefined,
                JsonValueKind.Null => Null,
                JsonValueKind.String => new JsonString(json.GetString()),
                JsonValueKind.Number => json.TryGetInt64(out var l)
                    ? new JsonInteger(l)
                    : (json.TryGetDouble(out var d) ? new JsonDouble(d) : Null),
                JsonValueKind.True => new JsonBoolean(true),
                JsonValueKind.False => new JsonBoolean(false),
                JsonValueKind.Array => (JsonArray)json,
                JsonValueKind.Object => (JsonObject)json,
                _ => null
            };
        }

        /// <summary>
        /// Attempts to represent the current JSON string or JavaScript date tidks number as a date time.
        /// </summary>
        /// <param name="json">The JSON element.</param>
        /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
        /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
        /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
        public static bool TryGetJavaScriptDateTicks(this JsonElement json, out DateTime value)
        {
            switch (json.ValueKind)
            {
                case JsonValueKind.String:
                    if (json.TryGetDateTime(out DateTime tmp)) break;
                    value = tmp;
                    return true;
                case JsonValueKind.Number:
                    if (!json.TryGetInt64(out long tick)) break;
                    value = Web.WebFormat.ParseDate(tick);
                    return true;
                default:
                    throw new InvalidOperationException("The value kind should be string or number.");
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to represent the current JSON string or Unix timestamps number as a date time.
        /// </summary>
        /// <param name="json">The JSON element.</param>
        /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
        /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
        /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
        public static bool TryGetUnixTimestamps(this JsonElement json, out DateTime value)
        {
            switch (json.ValueKind)
            {
                case JsonValueKind.String:
                    if (json.TryGetDateTime(out DateTime tmp)) break;
                    value = tmp;
                    return true;
                case JsonValueKind.Number:
                    if (!json.TryGetInt64(out long tick)) break;
                    value = Web.WebFormat.ParseUnixTimestamp(tick);
                    return true;
                default:
                    throw new InvalidOperationException("The value kind should be string or number.");
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to represent the current JSON string or Windows file time number as a date time.
        /// </summary>
        /// <param name="json">The JSON element.</param>
        /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
        /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
        /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
        public static bool TryGetWindowsFileTimeUtc(this JsonElement json, out DateTime value)
        {
            switch (json.ValueKind)
            {
                case JsonValueKind.String:
                    if (json.TryGetDateTime(out DateTime tmp)) break;
                    value = tmp;
                    return true;
                case JsonValueKind.Number:
                    if (!json.TryGetInt64(out long tick)) break;
                    value = DateTime.FromFileTimeUtc(tick);
                    return true;
                default:
                    throw new InvalidOperationException("The value kind should be string or number.");
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        internal static bool Equals(IJsonValue leftValue, IJsonValue rightValue)
        {
            if (leftValue is null || leftValue.ValueKind == JsonValueKind.Null || leftValue.ValueKind == JsonValueKind.Undefined)
            {
                return rightValue is null || rightValue.ValueKind == JsonValueKind.Null || rightValue.ValueKind == JsonValueKind.Undefined;
            }

            if (rightValue is null || rightValue.ValueKind != leftValue.ValueKind) return false;
            return leftValue.Equals(rightValue);
        }

        internal static IJsonValue ConvertValue(IJsonValue value, IJsonValue thisInstance = null)
        {
            if (value is null || value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined) return null;
            if (value is JsonObject obj) return obj == thisInstance ? obj.Clone() : obj;
            if (value is JsonArray || value is JsonString || value is JsonInteger || value is JsonDouble || value is JsonBoolean) return value;
            if (value.ValueKind == JsonValueKind.True) return JsonBoolean.True;
            if (value.ValueKind == JsonValueKind.False) return JsonBoolean.False;
            if (value.ValueKind == JsonValueKind.String)
            {
                if (value is IJsonValue<string> str) return new JsonString(str.Value);
                if (value is IJsonValue<DateTime> date) return new JsonString(date.Value);
                if (value is IJsonValue<Guid> guid) return new JsonString(guid.Value);
            }

            if (value.ValueKind == JsonValueKind.Number)
            {
                if (value is IJsonValue<int> int32) return new JsonInteger(int32.Value);
                if (value is IJsonValue<long> int64) return new JsonInteger(int64.Value);
                if (value is IJsonValue<short> int16) return new JsonInteger(int16.Value);
                if (value is IJsonValue<double> d) return new JsonDouble(d.Value);
                if (value is IJsonValue<float> f) return new JsonDouble(f.Value);
                if (value is IJsonValue<decimal> fd) return new JsonDouble((double)fd.Value);
                if (value is IJsonValue<bool> b) return b.Value ? JsonBoolean.True : JsonBoolean.False;
                if (value is IJsonValue<uint> uint32) return new JsonInteger(uint32.Value);
                if (value is IJsonValue<ulong> uint64) return new JsonDouble(uint64.Value);
                if (value is IJsonValue<ushort> uint16) return new JsonInteger(uint16.Value);
                if (value is IJsonValue<DateTime> date) return new JsonInteger(date.Value);
                var s = value.ToString();
                if (long.TryParse(s, out var l)) return new JsonInteger(l);
                if (double.TryParse(s, out var db)) return new JsonDouble(db);
                return null;
            }

            return null;
        }
    }
}
