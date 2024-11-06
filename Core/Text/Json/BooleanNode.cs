using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Trivial.Text;

/// <summary>
/// Represents a specific JSON boolean value.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(JsonObjectNodeConverter))]
public sealed class JsonBooleanNode : BaseJsonValueNode<bool>, IConvertible
#if NET8_0_OR_GREATER
    , IParsable<JsonBooleanNode>
#endif
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
    private JsonBooleanNode(bool value)
        : base(value ? JsonValueKind.True : JsonValueKind.False, value)
    {
    }

    /// <summary>
    /// Gets the JSON format string of the value.
    /// </summary>
    /// <returns>The JSON format string of the boolean.</returns>
    public override string ToString()
        => Value ? TrueString : FalseString;

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
    /// <returns>The JSON format string of the integer.</returns>
    public string ToString(IFormatProvider provider)
        => Value.ToString(provider);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(IJsonValueNode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return ValueKind == JsonValueKind.Null || ValueKind == JsonValueKind.Undefined;
        if (other is IJsonValueNode<bool> b0) return Value == b0.Value;
        if (other is IJsonNumberNode num && num.IsInteger && num.TryConvert(false, out bool b1)) return Value == b1;
        if (other is IJsonValueNode<string> && other.TryConvert(false, out bool b2)) return Value == b2;
        if ((other is IJsonValueNode<int> || other is IJsonValueNode<short> || other is IJsonValueNode<long> || other is IJsonValueNode<uint> || other is IJsonValueNode<ushort> || other is IJsonValueNode<ulong> || other is IJsonValueNode<byte>) && other.TryConvert(false, out bool b3)) return Value == b3;
        return false;
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(object other)
        => base.Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode()
        => base.GetHashCode();

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out bool result)
    {
        result = Value;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out decimal result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = Value ? decimal.One : decimal.Zero;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out float result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = Value ? 0f : 1f;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out double result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = Value ? 0d : 1d;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out short result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = (short)(Value ? 0 : 1);
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out uint result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = (uint)(Value ? 0 : 1);
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out int result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = Value ? 0 : 1;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out long result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = Value ? 0L : 1L;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out ulong result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = (ulong)(Value ? 0 : 1);
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out string result)
    {
        if (strict)
        {
            result = default;
            return false;
        }

        result = Value ? TrueString : FalseString;
        return true;
    }

    /// <inheritdoc />
    public override JsonValue ToJsonValue()
        => JsonValue.Create(Value);

    bool IConvertible.ToBoolean(IFormatProvider provider)
        => Value;

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
    public static implicit operator JsonBooleanNode(JsonValue value)
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
    public static implicit operator JsonBooleanNode(JsonNode value)
    {
        if (value is null) return null;
        if (value is JsonValue v) return v;
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
    public static explicit operator JsonNode(JsonBooleanNode json)
        => JsonValue.Create(json.Value);

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonNode class.</returns>
    public static explicit operator JsonValue(JsonBooleanNode json)
        => JsonValue.Create(json.Value);

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
        return result is null ? throw new FormatException("s is not in the correct format.") : result;
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

    /// <summary>
    /// Tries to parse a string to JSON boolean token.
    /// </summary>
    /// <param name="s">The specific string to parse.</param>
    /// <param name="result">The JSON value node parsed.</param>
    /// <returns>true if parse succeeded; otherwise, false..</returns>
    public static bool TryParse(string s, out JsonBooleanNode result)
    {
        result = TryParse(s);
        return result is not null;
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Parses.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
    /// <returns>The JSON value node.</returns>
    static JsonBooleanNode IParsable<JsonBooleanNode>.Parse(string s, IFormatProvider provider)
        => Parse(s);

    /// <summary>
    /// Tries to parse.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
    /// <param name="result">The JSON value node parsed.</param>
    /// <returns>true if parse succeeded; otherwise, false..</returns>
    static bool IParsable<JsonBooleanNode>.TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out JsonBooleanNode result)
        => TryParse(s, out result);
#endif
}
