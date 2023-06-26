using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;

using Trivial.Web;

namespace Trivial.Text;

/// <summary>
/// Represents a specific JSON value.
/// </summary>
public interface IJsonValueNode
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
public interface IJsonValueNode<T> : IJsonValueNode, IEquatable<IJsonValueNode<T>>, IEquatable<T>
{
    /// <summary>
    /// Gets the source value.
    /// </summary>
    T Value { get; }
}

/// <summary>
/// Represents a specific JSON value with source.
/// </summary>
public interface IJsonDataNode : IJsonValueNode
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
    IJsonDataNode GetValue(string key);

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode GetValue(ReadOnlySpan<char> key);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryGetValue(string key, out IJsonDataNode result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryGetValue(ReadOnlySpan<char> key, out IJsonDataNode result);

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode GetValue(int index);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryGetValue(int index, out IJsonDataNode result);

#if !NETFRAMEWORK
    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode GetValue(Index index);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryGetValue(Index index, out IJsonDataNode result);
#endif

    /// <summary>
    /// Gets all property keys.
    /// </summary>
    /// <returns>The property keys.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
    IEnumerable<string> GetKeys();
}

/// <summary>
/// Represents a JSON container.
/// </summary>
public interface IJsonContainerNode : IJsonValueNode, ICloneable, IEnumerable
{
    /// <summary>
    /// Gets the number of elements contained in JSON container.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode GetValue(string key);

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode GetValue(ReadOnlySpan<char> key);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryGetValue(string key, out IJsonDataNode result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryGetValue(ReadOnlySpan<char> key, out IJsonDataNode result);

    /// <summary>
    /// Gets all property keys.
    /// </summary>
    /// <returns>The property keys.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
    IEnumerable<string> GetKeys();

    /// <summary>
    /// Removes all items from the array.
    /// </summary>
    void Clear();

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
}

/// <summary>
/// The indent styles.
/// </summary>
public enum IndentStyles : byte
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
/// Json null.
/// </summary>
internal class JsonNullNode : IJsonValueNode, IJsonDataNode, IEquatable<JsonNullNode>
{
    /// <summary>
    /// Initializes a new instance of the JsonNull class.
    /// </summary>
    /// <param name="valueKind">The JSON value kind.</param>
    public JsonNullNode(JsonValueKind valueKind)
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
        => "null";

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(JsonNullNode other)
        => true;

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return true;
        if (other is IJsonValueNode json)
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
    bool IJsonDataNode.GetBoolean() => false;

    /// <summary>
    /// Gets the value of the element as a byte array.
    /// </summary>
    /// <returns>The value decoded as a byte array.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    byte[] IJsonDataNode.GetBytesFromBase64() => throw new InvalidOperationException("Expect a string but it is null.");

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <returns>The value of the element as a date time.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    DateTime IJsonDataNode.GetDateTime() => throw new InvalidOperationException("Expect a date time but it is null.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    decimal IJsonDataNode.GetDecimal() => 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    float IJsonDataNode.GetSingle() => 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    double IJsonDataNode.GetDouble() => 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    short IJsonDataNode.GetInt16() => 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    uint IJsonDataNode.GetUInt32() => 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    int IJsonDataNode.GetInt32() => 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    long IJsonDataNode.GetInt64() => 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    string IJsonDataNode.GetString() => null;

    /// <summary>
    /// Gets the value of the element as a GUID.
    /// </summary>
    /// <returns>The value of the element as a GUID.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    Guid IJsonDataNode.GetGuid() => throw new InvalidOperationException("Expect a string but it is null.");

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetBoolean(out bool result)
    {
        result = false;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetDateTime(out DateTime result)
    {
        result = WebFormat.ParseDate(0);
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetDecimal(out decimal result)
    {
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetSingle(out float result)
    {
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetDouble(out double result)
    {
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetUInt32(out uint result)
    {
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetInt32(out int result)
    {
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetInt64(out long result)
    {
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetString(out string result)
    {
        result = null;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a GUID.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetGuid(out Guid result)
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
    IJsonDataNode IJsonDataNode.GetValue(string key) => throw new InvalidOperationException("Expect an object but it is null.");

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode IJsonDataNode.GetValue(ReadOnlySpan<char> key) => throw new InvalidOperationException("Expect an object but it is null.");

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetValue(string key, out IJsonDataNode result)
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
    bool IJsonDataNode.TryGetValue(ReadOnlySpan<char> key, out IJsonDataNode result)
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
    IJsonDataNode IJsonDataNode.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is null.");

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetValue(int index, out IJsonDataNode result)
    {
        result = default;
        return false;
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode IJsonDataNode.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is null.");

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetValue(Index index, out IJsonDataNode result)
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
    IEnumerable<string> IJsonDataNode.GetKeys() => throw new InvalidOperationException("Expect an object but it is null.");

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonNullNode leftValue, JsonNullNode rightValue)
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
    public static bool operator !=(JsonNullNode leftValue, JsonNullNode rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (leftValue is null || rightValue is null) return false;
        return leftValue.Equals(rightValue);
    }
}
