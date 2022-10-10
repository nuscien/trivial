using System;
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

#if !NETOLDVER
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

#if !NETOLDVER
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

/// <summary>
/// The extensions for class IJsonValueNode, JsonDocument, JsonElement, etc.
/// </summary>
public static class JsonValues
{
    /// <summary>
    /// JSON null.
    /// </summary>
    public static readonly IJsonDataNode Null = new JsonNullNode(JsonValueKind.Null);

    /// <summary>
    /// JSON undefined.
    /// </summary>
    public static readonly IJsonDataNode Undefined = new JsonNullNode(JsonValueKind.Undefined);

    /// <summary>
    /// Converts from JSON document.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>The JSON value.</returns>
    public static IJsonDataNode ToJsonValue(JsonDocument json)
    {
        if (json is null) return null;
        return ToJsonValue(json.RootElement);
    }

    /// <summary>
    /// Converts from JSON element.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>The JSON value.</returns>
    public static IJsonDataNode ToJsonValue(JsonElement json)
    {
        return json.ValueKind switch
        {
            JsonValueKind.Undefined => Undefined,
            JsonValueKind.Null => Null,
            JsonValueKind.String => new JsonStringNode(json.GetString()),
            JsonValueKind.Number => json.TryGetInt64(out var l)
                ? new JsonIntegerNode(l)
                : (json.TryGetDouble(out var d) ? new JsonDoubleNode(d) : Null),
            JsonValueKind.True => JsonBooleanNode.True,
            JsonValueKind.False => JsonBooleanNode.False,
            JsonValueKind.Array => (JsonArrayNode)json,
            JsonValueKind.Object => (JsonObjectNode)json,
            _ => null
        };
    }

    /// <summary>
    /// Converts from JSON element.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>The JSON value.</returns>
    public static IJsonDataNode ToJsonValue(SystemJsonNode json)
    {
        if (json is null)
            return Null;
        if (json is SystemJsonObject obj)
            return (JsonObjectNode)obj;
        if (json is SystemJsonArray arr)
            return (JsonArrayNode)arr;
        if (json is not SystemJsonValue token)
            return null;
        if (token.TryGetValue(out string s))
            return new JsonStringNode(s);
        if (token.TryGetValue(out bool b))
            return b ? JsonBooleanNode.True : JsonBooleanNode.False;
        if (token.TryGetValue(out long l))
            return new JsonIntegerNode(l);
        if (token.TryGetValue(out int i))
            return new JsonIntegerNode(i);
        if (token.TryGetValue(out uint ui))
            return new JsonIntegerNode(ui);
        if (token.TryGetValue(out short sh))
            return new JsonIntegerNode(sh);
        if (token.TryGetValue(out ushort ush))
            return new JsonIntegerNode(ush);
        if (token.TryGetValue(out sbyte sb))
            return new JsonIntegerNode(sb);
        if (token.TryGetValue(out byte by))
            return new JsonIntegerNode(by);
        if (token.TryGetValue(out double d))
            return new JsonDoubleNode(d);
        if (token.TryGetValue(out float f))
            return new JsonDoubleNode(f);
        if (token.TryGetValue(out decimal de))
            return new JsonDoubleNode(de);
        if (token.TryGetValue(out Guid g))
            return new JsonStringNode(g);
        if (token.TryGetValue(out DateTime dt))
            return new JsonStringNode(dt);
        if (token.TryGetValue(out DateTimeOffset dto))
            return new JsonStringNode(dto);
        if (token.TryGetValue(out char c))
            return new JsonStringNode(c);
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
    /// Tries to get the value of the specific property for each object.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <returns>The property list.</returns>
    public static List<IJsonDataNode> TryGetValues(this IEnumerable<JsonObjectNode> col, string key)
    {
        var list = new List<IJsonDataNode>();
        if (col == null) return list;
        foreach (var item in col)
        {
            var json = item?.TryGetObjectValue(key);
            list.Add(json ?? Undefined);
        }

        return list;
    }

    /// <summary>
    /// Tries to get the value of the specific property for each object.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="ignoreNotMatched">true if ignore any item which is not JSON object; otherwise, false.</param>
    /// <returns>The property list.</returns>
    public static List<JsonObjectNode> TryGetObjectValues(this IEnumerable<JsonObjectNode> col, string key, bool ignoreNotMatched = false)
    {
        var list = new List<JsonObjectNode>();
        if (col == null) return list;
        if (ignoreNotMatched)
        {
            foreach (var item in col)
            {
                var json = item?.TryGetObjectValue(key);
                if (json == null) continue;
                list.Add(json);
            }
        }
        else
        {
            foreach (var item in col)
            {
                var json = item?.TryGetObjectValue(key);
                list.Add(json);
            }
        }

        return list;
    }

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    internal static bool Equals(IJsonValueNode leftValue, IJsonValueNode rightValue)
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
    internal static SystemJsonNode ToJsonNode(IJsonDataNode json)
    {
        if (json is null)
            return null;
        if (json is JsonObjectNode obj)
            return (SystemJsonObject)obj;
        if (json is JsonArrayNode arr)
            return (SystemJsonArray)arr;
        if (json is JsonStringNode s)
            return (SystemJsonValue)s;
        if (json is JsonIntegerNode i)
            return (SystemJsonValue)i;
        if (json is JsonDoubleNode f)
            return (SystemJsonValue)f;
        if (json is JsonBooleanNode b)
            return (SystemJsonValue)b;
        return null;
    }

    internal static IJsonDataNode ConvertValue(IJsonValueNode value, IJsonValueNode thisInstance = null)
    {
        if (value is null || value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined) return Null;
        if (value is JsonObjectNode obj)
        {
            if (ReferenceEquals(obj, thisInstance)) return obj.Clone();
            return obj;
        }

        if (value is JsonArrayNode arr)
        {
            if (ReferenceEquals(arr, thisInstance)) return arr.Clone();
            return arr;
        }

        if (value is JsonStringNode || value is JsonIntegerNode || value is JsonDoubleNode || value is JsonBooleanNode) return value as IJsonDataNode;
        if (value.ValueKind == JsonValueKind.True) return JsonBooleanNode.True;
        if (value.ValueKind == JsonValueKind.False) return JsonBooleanNode.False;
        if (value.ValueKind == JsonValueKind.String)
        {
            if (value is IJsonValueNode<string> str) return new JsonStringNode(str.Value);
            if (value is IJsonValueNode<DateTime> date) return new JsonStringNode(date.Value);
            if (value is IJsonValueNode<Guid> guid) return new JsonStringNode(guid.Value);
            if (value is IJsonStringNode js) return new JsonStringNode(js.StringValue);
        }

        if (value.ValueKind == JsonValueKind.Number)
        {
            if (value is IJsonValueNode<int> int32) return new JsonIntegerNode(int32.Value);
            if (value is IJsonValueNode<long> int64) return new JsonIntegerNode(int64.Value);
            if (value is IJsonValueNode<short> int16) return new JsonIntegerNode(int16.Value);
            if (value is IJsonValueNode<double> d) return new JsonDoubleNode(d.Value);
            if (value is IJsonValueNode<float> f) return new JsonDoubleNode(f.Value);
            if (value is IJsonValueNode<decimal> fd) return new JsonDoubleNode((double)fd.Value);
            if (value is IJsonValueNode<bool> b) return b.Value ? JsonBooleanNode.True : JsonBooleanNode.False;
            if (value is IJsonValueNode<uint> uint32) return new JsonIntegerNode(uint32.Value);
            if (value is IJsonValueNode<ulong> uint64) return new JsonDoubleNode(uint64.Value);
            if (value is IJsonValueNode<ushort> uint16) return new JsonIntegerNode(uint16.Value);
            if (value is IJsonValueNode<DateTime> date) return new JsonIntegerNode(date.Value);
            var s = value.ToString();
            if (long.TryParse(s, out var l)) return new JsonIntegerNode(l);
            if (double.TryParse(s, out var db)) return new JsonDoubleNode(db);
            return Null;
        }

        return Null;
    }
}
