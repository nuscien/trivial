using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.Json;
using Trivial.Reflection;
using Trivial.Security;
using Trivial.Web;

namespace Trivial.Text;

internal delegate bool JsonNodeTryConvertHandler<T>(bool strict, out T result);

/// <summary>
/// Represents a specific JSON value.
/// </summary>
public interface IJsonValueNode
{
    /// <summary>
    /// Gets the type of the current JSON value.
    /// </summary>
    JsonValueKind ValueKind { get; }

    /// <summary>
    /// Gets the item value count; or 0, if the value kind is not expected.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a boolean.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    bool GetBoolean(bool strict = false);

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
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
    /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
    decimal GetDecimal(bool strict = false);

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
    /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
    float GetSingle(bool strict = false);

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    double GetDouble(bool strict = false);

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
    /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
    short GetInt16(bool strict = false);

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
    /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
    uint GetUInt32(bool strict = false);

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
    /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
    int GetInt32(bool strict = false);

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
    /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
    long GetInt64(bool strict = false);

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="InvalidCastException">The bit of value is more than the one need to convert.</exception>
    /// <exception cref="OverflowException">The value is greater than the most maximum value or less than the most minimum value defined of the number type.</exception>
    ulong GetUInt64(bool strict = false);

    /// <summary>
    /// Gets the value of the element as a string.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    string GetString(bool strict = false);

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
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(bool strict, out bool result);

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(out DateTime result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(bool strict, out decimal result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(bool strict, out float result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(bool strict, out double result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(bool strict, out short result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(bool strict, out uint result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(bool strict, out int result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(bool strict, out ulong result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(bool strict, out long result);

    /// <summary>
    /// Tries to get the value of the element as a string.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(bool strict, out string result);

    /// <summary>
    /// Tries to get the value of the element as a GUID.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryConvert(out Guid result);

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonValueNode GetValue(string key);

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonValueNode GetValue(ReadOnlySpan<char> key);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryGetValue(string key, out IJsonValueNode result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryGetValue(ReadOnlySpan<char> key, out IJsonValueNode result);

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonValueNode GetValue(int index);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryGetValue(int index, out IJsonValueNode result);

#if !NETFRAMEWORK
    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonValueNode GetValue(Index index);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool TryGetValue(Index index, out IJsonValueNode result);
#endif

    /// <summary>
    /// Gets all property keys.
    /// </summary>
    /// <returns>The property keys.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
    IEnumerable<string> Keys();

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <returns>An instance of the JSON node.</returns>
    System.Text.Json.Nodes.JsonNode ToJsonNode();
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
/// Represents a JSON container.
/// </summary>
public interface IJsonContainerNode : IJsonValueNode, ICloneable, IEnumerable
{
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
/// The host for JSON object node.
/// </summary>
public interface IJsonObjectHost
{
    /// <summary>
    /// Converts to JSON object node.
    /// </summary>
    /// <returns>The JSON object node about current instance.</returns>
    JsonObjectNode ToJson();
}
