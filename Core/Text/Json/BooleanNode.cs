using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;

using Trivial.Web;

using SystemJsonValue = System.Text.Json.Nodes.JsonValue;
using SystemJsonNode = System.Text.Json.Nodes.JsonNode;

namespace Trivial.Text;

/// <summary>
/// Represents a specific JSON boolean value.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(JsonObjectNodeConverter))]
public class JsonBooleanNode : IJsonValueNode<bool>, IJsonDataNode
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
    public readonly static JsonBooleanNode True = new(true);

    /// <summary>
    /// Represents the Boolean value false of JSON as a string.
    /// This field is read-only.
    /// </summary>
    public readonly static JsonBooleanNode False = new(false);

    /// <summary>
    /// Initializes a new instance of the JsonBooleanNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonBooleanNode(bool value)
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
        => Value ? TrueString : FalseString;

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(IJsonValueNode<bool> other)
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
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (other is IJsonValueNode<bool> bJson) return Value.Equals(bJson.Value);
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
    bool IJsonDataNode.GetBoolean() => Value;

    /// <summary>
    /// Gets the value of the element as a byte array.
    /// </summary>
    /// <returns>The value decoded as a byte array.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    byte[] IJsonDataNode.GetBytesFromBase64() => throw new InvalidOperationException("Expect a string but it is a boolean value.");

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <returns>The value of the element as a date time.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    DateTime IJsonDataNode.GetDateTime() => throw new InvalidOperationException("Expect a date time but it is a boolean value.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    decimal IJsonDataNode.GetDecimal() => Value ? 1 : 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    float IJsonDataNode.GetSingle() => Value ? 1 : 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    double IJsonDataNode.GetDouble() => Value ? 1 : 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    short IJsonDataNode.GetInt16() => (short)(Value ? 1 : 0);

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    uint IJsonDataNode.GetUInt32() => (uint)(Value ? 1 : 0);

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    int IJsonDataNode.GetInt32() => Value ? 1 : 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    long IJsonDataNode.GetInt64() => Value ? 1 : 0;

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    string IJsonDataNode.GetString() => ToString();

    /// <summary>
    /// Gets the value of the element as a GUID.
    /// </summary>
    /// <returns>The value of the element as a GUID.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    Guid IJsonDataNode.GetGuid() => throw new InvalidOperationException("Expect a string but it is a boolean value.");

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetBoolean(out bool result)
    {
        result = Value;
        return true;
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
        result = Value ? 1 : 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetSingle(out float result)
    {
        result = Value ? 1 : 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetDouble(out double result)
    {
        result = Value ? 1 : 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetUInt32(out uint result)
    {
        result = (uint)(Value ? 1 : 0);
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetInt32(out int result)
    {
        result = Value ? 1 : 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetInt64(out long result)
    {
        result = Value ? 1 : 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetString(out string result)
    {
        result = ToString();
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
    IJsonDataNode IJsonDataNode.GetValue(string key) => throw new InvalidOperationException("Expect an object but it is a boolean value.");

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode IJsonDataNode.GetValue(ReadOnlySpan<char> key) => throw new InvalidOperationException("Expect an object but it is a boolean value.");

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
    IJsonDataNode IJsonDataNode.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is a boolean value.");

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
    IJsonDataNode IJsonDataNode.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is a boolean value.");

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
    IEnumerable<string> IJsonDataNode.GetKeys()
        => throw new InvalidOperationException("Expect an object but it is a boolean value.");

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonBooleanNode(bool value)
        => value ? True : False;

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonBooleanNode(SystemJsonValue value)
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
    public static implicit operator JsonBooleanNode(SystemJsonNode value)
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
    public static explicit operator bool(JsonBooleanNode json)
        => json?.Value ?? false;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A boolean.</returns>
    public static explicit operator bool?(JsonBooleanNode json)
        => json?.Value;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A number.</returns>
    public static explicit operator int(JsonBooleanNode json)
    {
        if (json is null) return 0;
        return json.Value ? 1 : 0;
    }

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A number.</returns>
    public static explicit operator int?(JsonBooleanNode json)
    {
        if (json is null) return null;
        return json.Value ? 1 : 0;
    }

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator string(JsonBooleanNode json)
    {
        if (json is null) return null;
        return json.Value ? TrueString : FalseString;
    }

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonNode class.</returns>
    public static explicit operator SystemJsonNode(JsonBooleanNode json)
        => SystemJsonValue.Create(json.Value);

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonNode class.</returns>
    public static explicit operator SystemJsonValue(JsonBooleanNode json)
        => SystemJsonValue.Create(json.Value);

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonBooleanNode leftValue, IJsonValueNode<bool> rightValue)
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
    public static bool operator !=(JsonBooleanNode leftValue, IJsonValueNode<bool> rightValue)
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
    public static bool operator ==(JsonBooleanNode leftValue, bool rightValue)
        => leftValue is not null && leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonBooleanNode leftValue, bool rightValue)
        => leftValue is null || leftValue.Value != rightValue;

    /// <summary>
    /// And operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if they are all true; otherwise, false.</returns>
    public static bool operator &(JsonBooleanNode leftValue, bool rightValue)
        => leftValue?.Value == true && rightValue;

    /// <summary>
    /// And operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if they are all true; otherwise, false.</returns>
    public static bool operator &(JsonBooleanNode leftValue, JsonBooleanNode rightValue)
        => leftValue?.Value == true && rightValue?.Value == true;

    /// <summary>
    /// Or operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if any is true; otherwise, false.</returns>
    public static bool operator |(JsonBooleanNode leftValue, bool rightValue)
        => leftValue?.Value == true || rightValue;

    /// <summary>
    /// Or operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if any is true; otherwise, false.</returns>
    public static bool operator |(JsonBooleanNode leftValue, JsonBooleanNode rightValue)
        => leftValue?.Value == true || rightValue?.Value == true;

    /// <summary>
    /// Or operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A number.</returns>
    public static int operator |(JsonBooleanNode leftValue, int rightValue)
        => leftValue?.Value == true ? (rightValue + 1) : rightValue;

    /// <summary>
    /// Or operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A number.</returns>
    public static long operator |(JsonBooleanNode leftValue, long rightValue)
        => leftValue?.Value == true ? (rightValue + 1) : rightValue;

    /// <summary>
    /// Or operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A number.</returns>
    public static long operator |(JsonBooleanNode leftValue, float rightValue)
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
    public static long operator |(JsonBooleanNode leftValue, double rightValue)
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
    public static long operator |(JsonBooleanNode leftValue, decimal rightValue)
        => leftValue?.Value == true ? ((long)Math.Floor(rightValue) + 1) : (long)Math.Floor(rightValue);

    /// <summary>
    /// Or operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A number.</returns>
    public static JsonIntegerNode operator |(JsonBooleanNode leftValue, JsonIntegerNode rightValue)
    {
        if (rightValue is null) return new JsonIntegerNode(leftValue?.Value == true ? 1 : 0);
        return leftValue?.Value == true ? new JsonIntegerNode(rightValue.Value + 1) : rightValue;
    }

    /// <summary>
    /// Or operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A number.</returns>
    public static JsonIntegerNode operator |(JsonBooleanNode leftValue, JsonDoubleNode rightValue)
    {
        if (rightValue is null) return new JsonIntegerNode(leftValue?.Value == true ? 1 : 0);
        return new JsonIntegerNode(leftValue?.Value == true ? ((long)Math.Floor(rightValue.Value) + 1) : (long)Math.Floor(rightValue.Value));
    }

    /// <summary>
    /// Or operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A number.</returns>
    public static JsonIntegerNode operator |(JsonBooleanNode leftValue, JsonDecimalNode rightValue)
    {
        if (rightValue is null) return new JsonIntegerNode(leftValue?.Value == true ? 1 : 0);
        return new JsonIntegerNode(leftValue?.Value == true ? ((long)Math.Floor(rightValue.Value) + 1) : (long)Math.Floor(rightValue.Value));
    }

    /// <summary>
    /// Or operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A number.</returns>
    public static JsonIntegerNode operator |(JsonBooleanNode leftValue, string rightValue)
        => leftValue?.Value == true ? 1 : 0;

    /// <summary>
    /// Or operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>A number.</returns>
    public static JsonIntegerNode operator |(JsonBooleanNode leftValue, JsonStringNode rightValue)
        => new(leftValue?.Value == true ? 1 : 0);

    /// <summary>
    /// Parses a string to JSON boolean token.
    /// </summary>
    /// <param name="s">The specific string to parse.</param>
    /// <returns>A JSON boolean token.</returns>
    /// <exception cref="FormatException">s was true of false.</exception>
    public static JsonBooleanNode Parse(string s)
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
    public static JsonBooleanNode TryParse(string s)
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
