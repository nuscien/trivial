﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;

using Trivial.Web;

using SystemJsonObject = System.Text.Json.Nodes.JsonObject;
using SystemJsonArray = System.Text.Json.Nodes.JsonArray;
using SystemJsonValue = System.Text.Json.Nodes.JsonValue;
using SystemJsonNode = System.Text.Json.Nodes.JsonNode;

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
        JsonValueKind ValueKind { get; }
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
        T Value { get; }
    }

    /// <summary>
    /// Represents a specific JSON value with source.
    /// </summary>
    public interface IJsonValueResolver : IJsonValue
    {
        /// <summary>
        /// Gets the item value count; or 0, if the value kind is not expected.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the value of the element as a boolean.
        /// </summary>
        /// <returns>The value of the element as a boolean.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        bool GetBoolean();

        /// <summary>
        /// Gets the value of the element as a byte array.
        /// </summary>
        /// <returns>The value decoded as a byte array.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="FormatException">The value is not encoded as Base64 text and hence cannot be decoded to bytes.</exception>
        byte[] GetBytesFromBase64();

        /// <summary>
        /// Gets the value of the element as a date time.
        /// </summary>
        /// <returns>The value of the element as a date time.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="FormatException">The value is not formatted for a date time.</exception>
        DateTime GetDateTime();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        decimal GetDecimal();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        float GetSingle();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        double GetDouble();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        short GetInt16();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        uint GetUInt32();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        int GetInt32();

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
        /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
        long GetInt64();

        /// <summary>
        /// Gets the value of the element as a string.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        string GetString();

        /// <summary>
        /// Gets the value of the element as a GUID.
        /// </summary>
        /// <returns>The value of the element as a GUID.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="FormatException">The value is not formatted for a GUID.</exception>
        Guid GetGuid();

        /// <summary>
        /// Tries to get the value of the element as a boolean.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetBoolean(out bool result);

        /// <summary>
        /// Tries to get the value of the element as a date time.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetDateTime(out DateTime result);

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetDecimal(out decimal result);

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetSingle(out float result);

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetDouble(out double result);

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetUInt32(out uint result);

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetInt32(out int result);

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetInt64(out long result);

        /// <summary>
        /// Tries to get the value of the element as a string.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetString(out string result);

        /// <summary>
        /// Tries to get the value of the element as a GUID.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetGuid(out Guid result);

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver GetValue(string key);

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver GetValue(ReadOnlySpan<char> key);

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetValue(string key, out IJsonValueResolver result);

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetValue(ReadOnlySpan<char> key, out IJsonValueResolver result);

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver GetValue(int index);

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetValue(int index, out IJsonValueResolver result);

#if !NETOLDVER
        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver GetValue(Index index);

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool TryGetValue(Index index, out IJsonValueResolver result);
#endif

        /// <summary>
        /// Gets all property keys.
        /// </summary>
        /// <returns>The property keys.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
        IEnumerable<string> GetKeys();
    }

    /// <summary>
    /// Represents a complex JSON value.
    /// </summary>
    public interface IJsonComplex : IJsonValue, ICloneable, IEnumerable
    {
        /// <summary>
        /// Gets the number of elements contained in JSON container.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Deserializes.
        /// </summary>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        T Deserialize<T>(JsonSerializerOptions options = default);

        /// <summary>
        /// Writes this instance to the specified writer as a JSON value.
        /// </summary>
        /// <param name="writer">The writer to which to write this instance.</param>
        void WriteTo(Utf8JsonWriter writer);

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <param name="indentStyle">The indent style.</param>
        /// <returns>A JSON format string.</returns>
        string ToString(IndentStyles indentStyle);

        /// <summary>
        /// Removes all items from the array.
        /// </summary>
        void Clear();
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
        public readonly static JsonBoolean True = new(true);

        /// <summary>
        /// Represents the Boolean value false of JSON as a string.
        /// This field is read-only.
        /// </summary>
        public readonly static JsonBoolean False = new(false);

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
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Gets the item value count.
        /// It always return 0 because it is not an array or object.
        /// </summary>
        public int Count => 0;

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
        /// Gets the value of the element as a GUID.
        /// </summary>
        /// <returns>The value of the element as a GUID.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        Guid IJsonValueResolver.GetGuid() => throw new InvalidOperationException("Expect a string but it is a boolean value.");

        /// <summary>
        /// Tries to get the value of the element as a boolean.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetBoolean(out bool result)
        {
            result = Value;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a date time.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDateTime(out DateTime result)
        {
            result = WebFormat.ParseDate(0);
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDecimal(out decimal result)
        {
            result = Value ? 1 : 0;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetSingle(out float result)
        {
            result = Value ? 1 : 0;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDouble(out double result)
        {
            result = Value ? 1 : 0;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetUInt32(out uint result)
        {
            result = (uint)(Value ? 1 : 0);
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetInt32(out int result)
        {
            result = Value ? 1 : 0;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetInt64(out long result)
        {
            result = Value ? 1 : 0;
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
        IJsonValueResolver IJsonValueResolver.GetValue(string key) => throw new InvalidOperationException("Expect an object but it is a boolean value.");

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(ReadOnlySpan<char> key) => throw new InvalidOperationException("Expect an object but it is a boolean value.");

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
        IJsonValueResolver IJsonValueResolver.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is a boolean value.");

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
        IJsonValueResolver IJsonValueResolver.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is a boolean value.");

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
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonBoolean(SystemJsonValue value)
        {
            if (value is null) return null;
            if (value.TryGetValue(out bool b))
                return b ? True : False;
            if (value.TryGetValue(out int i))
                return i > 0 ? True : False;
            if (value.TryGetValue(out long l))
                return l > 0 ? True : False;
            if (value.TryGetValue(out string s))
            {
                var v = TryParse(s);
                if (v != null) return v;
            }

            throw new InvalidCastException("Expect a boolean to convert.");
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonBoolean(SystemJsonNode value)
        {
            if (value is null) return null;
            if (value is SystemJsonValue v) return v;
            throw new InvalidCastException($"Only supports JsonValue but its type is {value.GetType().Name}.");
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
        /// Converts to JSON node.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>An instance of the JsonNode class.</returns>
        public static explicit operator SystemJsonNode(JsonBoolean json)
        {
            return SystemJsonValue.Create(json.Value);
        }

        /// <summary>
        /// Converts to JSON node.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>An instance of the JsonNode class.</returns>
        public static explicit operator SystemJsonValue(JsonBoolean json)
        {
            return SystemJsonValue.Create(json.Value);
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

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>A number.</returns>
        public static int operator |(JsonBoolean leftValue, int rightValue)
        {
            return leftValue?.Value == true ? (rightValue + 1) : rightValue;
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>A number.</returns>
        public static long operator |(JsonBoolean leftValue, long rightValue)
        {
            return leftValue?.Value == true ? (rightValue + 1) : rightValue;
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>A number.</returns>
        public static long operator |(JsonBoolean leftValue, float rightValue)
        {
            if (float.IsNaN(rightValue)) return leftValue?.Value == true ? 1L : 0L;
            return leftValue?.Value == true ? ((long)Math.Floor(rightValue) + 1) : (long)Math.Floor(rightValue);
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>A number.</returns>
        public static long operator |(JsonBoolean leftValue, double rightValue)
        {
            if (double.IsNaN(rightValue)) return leftValue?.Value == true ? 1L : 0L;
            return leftValue?.Value == true ? ((long)Math.Floor(rightValue) + 1) : (long)Math.Floor(rightValue);
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>A number.</returns>
        public static long operator |(JsonBoolean leftValue, decimal rightValue)
        {
            return leftValue?.Value == true ? ((long)Math.Floor(rightValue) + 1) : (long)Math.Floor(rightValue);
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>A number.</returns>
        public static JsonInteger operator |(JsonBoolean leftValue, JsonInteger rightValue)
        {
            if (rightValue is null) return new JsonInteger(leftValue?.Value == true ? 1 : 0);
            return leftValue?.Value == true ? new JsonInteger(rightValue.Value + 1) : rightValue;
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>A number.</returns>
        public static JsonInteger operator |(JsonBoolean leftValue, JsonDouble rightValue)
        {
            if (rightValue is null) return new JsonInteger(leftValue?.Value == true ? 1 : 0);
            return new JsonInteger(leftValue?.Value == true ? ((long)Math.Floor(rightValue.Value) + 1) : (long)Math.Floor(rightValue.Value));
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>A number.</returns>
        public static JsonInteger operator |(JsonBoolean leftValue, string rightValue)
        {
            return leftValue?.Value == true ? 1 : 0;
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns>A number.</returns>
        public static JsonInteger operator |(JsonBoolean leftValue, JsonString rightValue)
        {
            return new JsonInteger(leftValue?.Value == true ? 1 : 0);
        }

        /// <summary>
        /// Parses a string to JSON boolean token.
        /// </summary>
        /// <param name="s">The specific string to parse.</param>
        /// <returns>A JSON boolean token.</returns>
        /// <exception cref="FormatException">s was true of false.</exception>
        public static JsonBoolean Parse(string s)
        {
            if (s == null) return null;
            var result = TryParse(s);
            if (result is null) throw new FormatException("s is not in the correct format.");
            return result;
        }

        /// <summary>
        /// Tries to parse a string to JSON boolean token.
        /// </summary>
        /// <param name="s">The specific string to parse.</param>
        /// <returns>A JSON boolean token; or null, if format error.</returns>
        public static JsonBoolean TryParse(string s)
        {
            if (s == null) return null;
            s = s.Trim();
            if (s.Length == 0) return False;
            if (bool.TryParse(s, out var b)) return b;
            if (long.TryParse(s, out var l)) return l > 0;
            return s.ToLowerInvariant() switch
            {
                TrueString => True,
                "t" => True,
                "a" => True,
                "y" => True,
                "yes" => True,
                "ok" => True,
                "good" => True,
                "s" => True,
                "sel" => True,
                "select" => True,
                "selected" => True,
                "c" => True,
                "check" => True,
                "checked" => True,
                "r" => True,
                "right" => True,
                "correct" => True,
                "真" => True,
                "是" => True,
                "对" => True,
                "好" => True,
                "要" => True,
                "确定" => True,
                "正确" => True,
                "选中" => True,
                "√" => True,
                "✅" => True,
                "🆗" => True,
                "✔" => True,
                "🈶" => True,
                "∞" => True,
                FalseString => True,
                "f" => False,
                "b" => False,
                "n" => False,
                "no" => False,
                "x" => False,
                "bad" => False,
                "u" => False,
                "un" => False,
                "unsel" => False,
                "unselect" => False,
                "unselected" => False,
                "uncheck" => False,
                "unchecked" => False,
                "w" => False,
                "wrong" => False,
                "incorrect" => False,
                "假" => False,
                "非" => False,
                "否" => False,
                "错" => False,
                "不" => False,
                "无" => False,
                "取消" => False,
                "错误" => False,
                "×" => False,
                "❎" => False,
                "🚫" => False,
                "❌" => False,
                "🈚" => False,
                _ => null
            };
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
                return ValueKind switch
                {
                    JsonValueKind.Null or JsonValueKind.Undefined => json.ValueKind == JsonValueKind.Null || json.ValueKind == JsonValueKind.Undefined,
                    JsonValueKind.True => json.ValueKind == JsonValueKind.True,
                    JsonValueKind.False => json.ValueKind == JsonValueKind.False,
                    _ => false,
                };
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode() => ValueKind.GetHashCode();

        /// <summary>
        /// Gets the item value count.
        /// It always return 0 because it is not an array or object.
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
        /// Gets the value of the element as a GUID.
        /// </summary>
        /// <returns>The value of the element as a GUID.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        Guid IJsonValueResolver.GetGuid() => throw new InvalidOperationException("Expect a string but it is null.");

        /// <summary>
        /// Tries to get the value of the element as a boolean.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetBoolean(out bool result)
        {
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
            result = WebFormat.ParseDate(0);
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDecimal(out decimal result)
        {
            result = 0;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetSingle(out float result)
        {
            result = 0;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDouble(out double result)
        {
            result = 0;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetUInt32(out uint result)
        {
            result = 0;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetInt32(out int result)
        {
            result = 0;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetInt64(out long result)
        {
            result = 0;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetString(out string result)
        {
            result = null;
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
        IJsonValueResolver IJsonValueResolver.GetValue(string key) => throw new InvalidOperationException("Expect an object but it is null.");

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(ReadOnlySpan<char> key) => throw new InvalidOperationException("Expect an object but it is null.");

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
        IJsonValueResolver IJsonValueResolver.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is null.");

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
        IJsonValueResolver IJsonValueResolver.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is null.");

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
    /// The indent styles.
    /// </summary>
    public enum IndentStyles
    {
        /// <summary>
        /// Minified format. Without any extra white space.
        /// </summary>
        Minified = 0,

        /// <summary>
        /// Without any extra white space.
        /// </summary>
        Empty = 1,

        /// <summary>
        /// Tab indent style.
        /// </summary>
        Tab = 2,

        /// <summary>
        /// 4 white spaces indent style.
        /// </summary>
        Normal = 3,

        /// <summary>
        /// 2 white spaces indent style.
        /// </summary>
        Compact = 4,

        /// <summary>
        /// 8 white spaces indent style.
        /// </summary>
        Wide = 5,

        /// <summary>
        /// 1 white space indent style.
        /// </summary>
        Space = 6
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
        /// <returns>The JSON value.</returns>
        public static IJsonValueResolver ToJsonValue(JsonDocument json)
        {
            if (json is null) return null;
            return ToJsonValue(json.RootElement);
        }

        /// <summary>
        /// Converts from JSON element.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>The JSON value.</returns>
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
                JsonValueKind.True => JsonBoolean.True,
                JsonValueKind.False => JsonBoolean.False,
                JsonValueKind.Array => (JsonArray)json,
                JsonValueKind.Object => (JsonObject)json,
                _ => null
            };
        }

        /// <summary>
        /// Converts from JSON element.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>The JSON value.</returns>
        public static IJsonValueResolver ToJsonValue(SystemJsonNode json)
        {
            if (json is null)
                return Null;
            if (json is SystemJsonObject obj)
                return (JsonObject)obj;
            if (json is SystemJsonArray arr)
                return (JsonArray)arr;
            if (!(json is SystemJsonValue token))
                return null;
            if (token.TryGetValue(out string s))
                return new JsonString(s);
            if (token.TryGetValue(out bool b))
                return b ? JsonBoolean.True : JsonBoolean.False;
            if (token.TryGetValue(out long l))
                return new JsonInteger(l);
            if (token.TryGetValue(out int i))
                return new JsonInteger(i);
            if (token.TryGetValue(out uint ui))
                return new JsonInteger(ui);
            if (token.TryGetValue(out short sh))
                return new JsonInteger(sh);
            if (token.TryGetValue(out ushort ush))
                return new JsonInteger(ush);
            if (token.TryGetValue(out sbyte sb))
                return new JsonInteger(sb);
            if (token.TryGetValue(out byte by))
                return new JsonInteger(by);
            if (token.TryGetValue(out double d))
                return new JsonDouble(d);
            if (token.TryGetValue(out float f))
                return new JsonDouble(f);
            if (token.TryGetValue(out decimal de))
                return new JsonDouble(de);
            if (token.TryGetValue(out Guid g))
                return new JsonString(g);
            if (token.TryGetValue(out DateTime dt))
                return new JsonString(dt);
            if (token.TryGetValue(out DateTimeOffset dto))
                return new JsonString(dto);
            if (token.TryGetValue(out char c))
                return new JsonString(c);
            if (token.TryGetValue(out JsonElement e))
                return ToJsonValue(e);
            return null;
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
                    value = WebFormat.ParseDate(tick);
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

        /// <summary>
        /// Converts to JSON node.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>The JSON node.</returns>
        internal static SystemJsonNode ToJsonNode(IJsonValueResolver json)
        {
            if (json is null)
                return null;
            if (json is JsonObject obj)
                return (SystemJsonObject)obj;
            if (json is JsonArray arr)
                return (SystemJsonArray)arr;
            if (json is JsonString s)
                return (SystemJsonValue)s;
            if (json is JsonInteger i)
                return (SystemJsonValue)i;
            if (json is JsonDouble f)
                return (SystemJsonValue)f;
            if (json is JsonBoolean b)
                return (SystemJsonValue)b;
            return null;
        }

        internal static IJsonValueResolver ConvertValue(IJsonValue value, IJsonValue thisInstance = null)
        {
            if (value is null || value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined) return Null;
            if (value is JsonObject obj)
            {
                if (ReferenceEquals(obj, thisInstance)) return obj.Clone();
                return obj;
            }

            if (value is JsonArray arr)
            {
                if (ReferenceEquals(arr, thisInstance)) return arr.Clone();
                return arr;
            }

            if (value is JsonString || value is JsonInteger || value is JsonDouble || value is JsonBoolean) return value as IJsonValueResolver;
            if (value.ValueKind == JsonValueKind.True) return JsonBoolean.True;
            if (value.ValueKind == JsonValueKind.False) return JsonBoolean.False;
            if (value.ValueKind == JsonValueKind.String)
            {
                if (value is IJsonValue<string> str) return new JsonString(str.Value);
                if (value is IJsonValue<DateTime> date) return new JsonString(date.Value);
                if (value is IJsonValue<Guid> guid) return new JsonString(guid.Value);
                if (value is IJsonString js) return new JsonString(js.StringValue);
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
                return Null;
            }

            return Null;
        }
    }
}
