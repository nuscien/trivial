
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Trivial.Web;

namespace Trivial.Text;

/// <summary>
/// Represents a specific string JSON value with source.
/// </summary>
public interface IJsonNumberNode : IJsonValueNode, IEquatable<IJsonNumberNode>, IEquatable<long>, IEquatable<int>, IEquatable<double>, IEquatable<float>
{
    /// <summary>
    /// Gets a value indicating whether the number value is an whole number (integer).
    /// </summary>
    public bool IsInteger { get; }
}

/// <summary>
/// Represents a specific JSON integer number value.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(JsonObjectNodeConverter))]
public sealed class JsonIntegerNode : BaseJsonValueNode<long>, IJsonNumberNode, IComparable<JsonIntegerNode>, IComparable<JsonDoubleNode>, IComparable<uint>, IComparable<int>, IComparable<long>, IComparable<double>, IComparable<float>, IEquatable<uint>, IEquatable<int>, IEquatable<float>, IEquatable<double>, IFormattable, IConvertible
#if NET8_0_OR_GREATER
    , IParsable<JsonIntegerNode>
#endif
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
    /// Initializes a new instance of the JsonIntegerNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonIntegerNode(int value)
        : base(JsonValueKind.Number, value)
    {
        IsSafe = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonIntegerNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonIntegerNode(uint value)
        : base(JsonValueKind.Number, value)
    {
        IsSafe = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonIntegerNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonIntegerNode(long value)
        : base(JsonValueKind.Number, value)
    {
        IsSafe = value <= MaxSafeInteger && value >= MinSafeInteger;
    }

    /// <summary>
    /// Initializes a new instance of the JsonIntegerNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonIntegerNode(short value)
        : base(JsonValueKind.Number, value)
    {
        IsSafe = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonIntegerNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonIntegerNode(double value)
        : base(JsonValueKind.Number, (long)Math.Round(value))
    {
        IsSafe = value <= MaxSafeInteger && value >= MinSafeInteger;
    }

    /// <summary>
    /// Initializes a new instance of the JsonIntegerNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="isUnixTimestamp">true if uses Unix timestamp; otherwise, false, to use JavaScript ticks, by default.</param>
    public JsonIntegerNode(DateTime value, bool isUnixTimestamp = false)
        : base(JsonValueKind.Number, isUnixTimestamp ? Web.WebFormat.ParseUnixTimestamp(value) : Web.WebFormat.ParseDate(value))
    {
        IsSafe = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonIntegerNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonIntegerNode(TimeSpan value)
        : base(JsonValueKind.Number, (long)value.TotalSeconds)
    {
        IsSafe = true;
    }

    /// <summary>
    /// Gets a value indicating whether the number value is an whole number.
    /// </summary>
    public bool IsInteger => true;

    /// <summary>
    /// Gets a value indicating whether the integer is safe.
    /// </summary>
    public bool IsSafe { get; }

    /// <summary>
    /// Gets the JSON format string of the value.
    /// </summary>
    /// <returns>The JSON format string of the integer.</returns>
    public override string ToString()
        => Value.ToString("g", CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
    /// <returns>The JSON format string of the integer.</returns>
    public string ToString(IFormatProvider provider)
        => Value.ToString(provider);

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// <param name="format">A numeric format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
    /// <returns>The JSON format string of the integer.</returns>
    public string ToString(string format, IFormatProvider provider = null)
        => provider != null ? Value.ToString(format, provider) : Value.ToString(format);

    /// <summary>
    /// Converts to a specific positional notation format string.
    /// </summary>
    /// <param name="type">The positional notation.</param>
    /// <returns>A string of the number in the specific positional notation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">type should be in 2-36.</exception>
    public string ToPositionalNotationString(int type)
        => Maths.Numbers.ToPositionalNotationString(Value, type);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(IJsonValueNode<long> other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value);
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(IJsonNumberNode other)
    {
        if (other is null || !other.IsInteger) return false;
        if (other is IJsonValueNode<long> n) return Value.Equals(n.Value);
        return Value.Equals(other.GetInt64());
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(IJsonValueNode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return ValueKind == JsonValueKind.Null || ValueKind == JsonValueKind.Undefined;
        if (other is IJsonNumberNode num) return Equals(num);
        if (other is IJsonValueNode<int> i1) return Value.Equals(i1.Value);
        if (other is IJsonValueNode<long> i2) return Value.Equals(i2.Value);
        if (other is IJsonValueNode<short> i3) return Value.Equals(i3.Value);
        if (other is IJsonValueNode<byte> i5) return Value.Equals(i5.Value);
        if (other is IJsonValueNode<uint> i6) return Value.Equals(i6.Value);
        if (other is IJsonValueNode<ulong> i7) return Value.Equals(i7.Value);
        if (other is IJsonValueNode<ushort> i8) return Value.Equals(i8.Value);
        if (other is IJsonValueNode<float> f1) return Value.Equals(f1.Value);
        if (other is IJsonValueNode<double> f2) return Value.Equals(f2.Value);
        if (other is IJsonValueNode<decimal> f3) return Value.Equals(f3.Value);
#if NET6_0_OR_GREATER
        if (other is IJsonValueNode<Half> f4) return Value.Equals(f4.Value);
#endif
#if NET8_0_OR_GREATER
        if (other is IJsonValueNode<Int128> i4) return Value.Equals(i4.Value);
        if (other is IJsonValueNode<UInt128> i9) return Value.Equals(i9.Value);
#endif
        if (other is IJsonValueNode<string> s) return !string.IsNullOrEmpty(s.Value) && Maths.Numbers.TryParseToInt64(s.Value, 10, out var intParsed) && intParsed == Value;
        return false;
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(double other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(float other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(long other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(uint other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(int other)
        => Value.Equals(other);

    /// <inheritdoc />
    public override bool Equals(object other)
        => base.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => base.GetHashCode();

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
    public int CompareTo(JsonIntegerNode other)
        => Value.CompareTo(other.Value);

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
    public int CompareTo(JsonDoubleNode other)
        => Value.CompareTo(other.Value);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out bool result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = Value > 0;
        return Value == 0 || Value == 1;
    }

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <returns>The value of the element as a date time.</returns>
    public DateTime GetDateTime()
        => WebFormat.ParseDate(Value);

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <param name="useUnixTimestamps">true if use Unix timestamp to convert; otherwise, false, to use JavaScript date ticks.</param>
    /// <returns>The value of the element as a date time.</returns>
    public DateTime GetDateTime(bool useUnixTimestamps)
        => useUnixTimestamps ? WebFormat.ParseUnixTimestamp(Value) : WebFormat.ParseDate(Value);

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(out DateTime result)
    {
        result = WebFormat.ParseDate(Value);
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out decimal result)
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
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out float result)
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
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out double result)
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
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result; or null, if overflow.</returns>
    public ushort? TryToUInt16()
        => TryConvert(out ushort result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="defaultValue">The fallback default value.</param>
    /// <returns>The result.</returns>
    public ushort TryConvert(ushort defaultValue)
        => TryConvert(out ushort result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    public bool TryConvert(out ushort result)
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
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result; or null, if overflow.</returns>
    public short? TryToInt16()
        => TryConvert(false, out short result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="defaultValue">The fallback default value.</param>
    /// <returns>The result.</returns>
    public short TryConvert(short defaultValue)
        => TryConvert(false, out short result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    public bool TryConvert(out short result)
        => TryConvert(false, out result);

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out short result)
    {
        try
        {
            result = (short)Value;
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
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result; or null, if overflow.</returns>
    public uint? TryToUInt32()
        => TryConvert(false, out uint result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="defaultValue">The fallback default value.</param>
    /// <returns>The result.</returns>
    public uint TryConvert(uint defaultValue)
        => TryConvert(false, out uint result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    public bool TryConvert(out uint result)
        => TryConvert(false, out result);

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out uint result)
    {
        if (Value < 0)
        {
            result = default;
            return false;
        }

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
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result; or null, if overflow.</returns>
    public int? TryToInt32()
        => TryConvert(false, out int result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="defaultValue">The fallback default value.</param>
    /// <returns>The result.</returns>
    public int TryConvert(int defaultValue)
        => TryConvert(false, out int result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    public bool TryConvert(out int result)
        => TryConvert(false, out result);

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out int result)
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
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out long result)
    {
        result = Value;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out ulong result)
    {
        if (Value < 0)
        {
            result = default;
            return false;
        }

        result = (ulong)Value;
        return true;
    }

    /// <inheritdoc />
    public override JsonValue ToJsonValue()
        => JsonValue.Create(Value);

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out string result)
    {
        result = ToString();
        return !strict;
    }

    long IConvertible.ToInt64(IFormatProvider provider)
        => Value;

    /// <summary>
    /// Parses.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>The JSON value node parsed.</returns>
    public static JsonIntegerNode Parse(string s)
    {
        var i = Maths.Numbers.ParseToInt64(s, 10);
        return new(i);
    }

    /// <summary>
    /// Tries to parse.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>The JSON value node parsed; or null, if failed to parse.</returns>
    public static JsonIntegerNode TryParse(string s)
        => Maths.Numbers.TryParseToInt64(s, 10, out var i) ? new(i) : default;

    /// <summary>
    /// Tries to parse.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="result">The JSON value node parsed.</param>
    /// <returns>true if parse succeeded; otherwise, false..</returns>
    public static bool TryParse(string s, out JsonIntegerNode result)
    {
        if (Maths.Numbers.TryParseToInt64(s, 10, out var i))
        {
            result = new(i);
            return true;
        }

        result = default;
        return false;
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Parses.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
    /// <returns>The JSON value node.</returns>
    static JsonIntegerNode IParsable<JsonIntegerNode>.Parse(string s, IFormatProvider provider)
        => Parse(s);

    /// <summary>
    /// Tries to parse.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
    /// <param name="result">The JSON value node parsed.</param>
    /// <returns>true if parse succeeded; otherwise, false..</returns>
    static bool IParsable<JsonIntegerNode>.TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out JsonIntegerNode result)
        => TryParse(s, out result);
#endif

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonIntegerNode(uint value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonIntegerNode(int value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonIntegerNode(long value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonIntegerNode(JsonValue value)
    {
        if (value is null) return null;
        if (value.TryGetValue(out int i))
            return new JsonIntegerNode(i);
        if (value.TryGetValue(out uint ui))
            return new JsonIntegerNode(ui);
        if (value.TryGetValue(out long l))
            return new JsonIntegerNode(l);
        if (value.TryGetValue(out short sh))
            return new JsonIntegerNode(sh);
        if (value.TryGetValue(out bool b))
            return new JsonIntegerNode(b ? 1 : 0);
        if (value.TryGetValue(out string s) && long.TryParse(s, out var l2))
            return new JsonIntegerNode(l2);
        throw new InvalidCastException("Expect an integer to convert.");
    }

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonIntegerNode(JsonNode value)
    {
        if (value is null) return null;
        if (value is JsonValue v) return v;
        throw new InvalidCastException($"Only supports JsonValue but its type is {value.GetType().Name}.");
    }

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A number.</returns>
    public static explicit operator System.Numerics.BigInteger(JsonIntegerNode json)
        => json.Value;

    /// <summary>
    /// Converts to a JSON string.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON string instance.</returns>
    public static explicit operator JsonStringNode(JsonIntegerNode json)
        => new(json.ToString());

    /// <summary>
    /// Converts to a JSON double object.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON double instance.</returns>
    public static explicit operator JsonDoubleNode(JsonIntegerNode json)
        => new(json.Value);

    /// <summary>
    /// Converts to a JSON double object.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON double instance.</returns>
    public static explicit operator JsonDecimalNode(JsonIntegerNode json)
        => new(json.Value);

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonIntegerNode leftValue, IJsonValueNode<long> rightValue)
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
    public static bool operator >(JsonIntegerNode leftValue, IJsonValueNode<long> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (rightValue is null) return false;
        return leftValue.Value > rightValue.Value;
    }

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonIntegerNode leftValue, IJsonValueNode<long> rightValue)
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
    public static bool operator >=(JsonIntegerNode leftValue, IJsonValueNode<long> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return true;
        if (rightValue is null) return false;
        return leftValue.Value >= rightValue.Value;
    }

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonIntegerNode leftValue, IJsonValueNode<double> rightValue)
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
    public static bool operator >(JsonIntegerNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (rightValue is null) return false;
        return leftValue.Value > rightValue.Value;
    }

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonIntegerNode leftValue, IJsonValueNode<double> rightValue)
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
    public static bool operator >=(JsonIntegerNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return true;
        if (rightValue is null) return false;
        return leftValue.Value >= rightValue.Value;
    }

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonIntegerNode leftValue, long rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonIntegerNode leftValue, long rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonIntegerNode leftValue, long rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonIntegerNode leftValue, long rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonIntegerNode leftValue, long rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonIntegerNode leftValue, long rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonIntegerNode leftValue, int rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonIntegerNode leftValue, int rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonIntegerNode leftValue, int rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonIntegerNode leftValue, int rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonIntegerNode leftValue, int rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonIntegerNode leftValue, int rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonIntegerNode leftValue, uint rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonIntegerNode leftValue, uint rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonIntegerNode leftValue, uint rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonIntegerNode leftValue, uint rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonIntegerNode leftValue, uint rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonIntegerNode leftValue, uint rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonIntegerNode leftValue, double rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonIntegerNode leftValue, double rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonIntegerNode leftValue, double rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonIntegerNode leftValue, double rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonIntegerNode leftValue, double rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonIntegerNode leftValue, double rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonIntegerNode leftValue, float rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonIntegerNode leftValue, float rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonIntegerNode leftValue, float rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonIntegerNode leftValue, float rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonIntegerNode leftValue, float rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonIntegerNode leftValue, float rightValue)
        => leftValue.Value >= rightValue;
}

/// <summary>
/// Represents a specific JSON double float number value.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(JsonObjectNodeConverter))]
public sealed class JsonDoubleNode : BaseJsonValueNode<double>, IJsonNumberNode, IComparable<JsonIntegerNode>, IComparable<JsonDoubleNode>, IComparable<JsonDecimalNode>, IComparable<uint>, IComparable<int>, IComparable<long>, IComparable<double>, IComparable<float>, IComparable<decimal>, IEquatable<uint>, IEquatable<int>, IEquatable<long>, IEquatable<float>, IEquatable<decimal>, IFormattable, IConvertible
#if NET8_0_OR_GREATER
    , IParsable<JsonDoubleNode>
#endif
{
    /// <summary>
    /// Initializes a new instance of the JsonDoubleNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDoubleNode(int value)
        : base(JsonValueKind.Number, value)
    {
        IsInteger = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonDoubleNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDoubleNode(long value)
        : base(JsonValueKind.Number, value)
    {
        IsInteger = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonDoubleNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDoubleNode(uint value)
        : base(JsonValueKind.Number, value)
    {
        IsInteger = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonDoubleNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDoubleNode(ulong value)
        : base(JsonValueKind.Number, value)
    {
        IsInteger = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonDoubleValue class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDoubleNode(float value)
        : base(float.IsNaN(value) || float.IsInfinity(value) ? JsonValueKind.Null : JsonValueKind.Number, value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value)) return;
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
    /// Initializes a new instance of the JsonDoubleNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDoubleNode(double value)
        : base(double.IsNaN(value) || double.IsInfinity(value) ? JsonValueKind.Null : JsonValueKind.Number, value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) return;
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
    /// Initializes a new instance of the JsonDoubleNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDoubleNode(decimal value)
        : base(JsonValueKind.Number, (double)value)
    {
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
    /// Initializes a new instance of the JsonDoubleNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDoubleNode(TimeSpan value)
        : base(JsonValueKind.Number, value.TotalSeconds)
    {
        IsInteger = value.Milliseconds == 0;
    }

    /// <summary>
    /// Gets a value indicating whether the number value is an whole number.
    /// </summary>
    public bool IsInteger { get; }

    /// <summary>
    /// Gets the JSON format string of the value.
    /// </summary>
    /// <returns>The JSON format string of the float number.</returns>
    public override string ToString()
        => double.IsNaN(Value) ? JsonValues.NullString : Value.ToString("g", CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
    /// <returns>The JSON format string of the integer.</returns>
    public string ToString(IFormatProvider provider)
        => Value.ToString(provider);

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// <param name="format">A numeric format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
    /// <returns>The JSON format string of the integer.</returns>
    public string ToString(string format, IFormatProvider provider = null)
        => provider != null ? Value.ToString(format, provider) : Value.ToString(format);

    /// <summary>
    /// Converts to a specific positional notation format string.
    /// </summary>
    /// <param name="type">The positional notation.</param>
    /// <returns>A string of the number in the specific positional notation.</returns>
    public string ToPositionalNotationString(int type)
        => Maths.Numbers.ToPositionalNotationString(Value, type);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(IJsonValueNode<double> other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value);
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(IJsonNumberNode other)
    {
        if (other is null) return false;
        if (other is IJsonValueNode<double> n) return Value.Equals(n.Value);
        return Value.Equals(other.GetDouble());
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(IJsonValueNode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return ValueKind == JsonValueKind.Null || ValueKind == JsonValueKind.Undefined;
        if (other is IJsonNumberNode num) return Equals(num);
        if (other is IJsonValueNode<int> i1) return Value.Equals(i1.Value);
        if (other is IJsonValueNode<long> i2) return Value.Equals(i2.Value);
        if (other is IJsonValueNode<short> i3) return Value.Equals(i3.Value);
        if (other is IJsonValueNode<byte> i5) return Value.Equals(i5.Value);
        if (other is IJsonValueNode<uint> i6) return Value.Equals(i6.Value);
        if (other is IJsonValueNode<ulong> i7) return Value.Equals(i7.Value);
        if (other is IJsonValueNode<ushort> i8) return Value.Equals(i8.Value);
        if (other is IJsonValueNode<float> f1) return Value.Equals(f1.Value);
        if (other is IJsonValueNode<double> f2) return Value.Equals(f2.Value);
        if (other is IJsonValueNode<decimal> f3) return Value.Equals(f3.Value);
#if NET6_0_OR_GREATER
        if (other is IJsonValueNode<Half> f4) return Value.Equals(f4.Value);
#endif
#if NET8_0_OR_GREATER
        if (other is IJsonValueNode<Int128> i4) return Value.Equals(i4.Value);
        if (other is IJsonValueNode<UInt128> i9) return Value.Equals(i9.Value);
#endif
        if (other is IJsonValueNode<string> s) return !string.IsNullOrEmpty(s.Value) && double.TryParse(s.Value, out var parsed) && parsed == Value;
        return false;
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(double other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(float other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(decimal other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(long other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(uint other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(int other)
        => Value.Equals(other);


    /// <inheritdoc />
    public override bool Equals(object other)
        => base.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => base.GetHashCode();

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
    public int CompareTo(JsonIntegerNode other)
        => Value.CompareTo(other.Value);

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
    public int CompareTo(JsonDoubleNode other)
        => Value.CompareTo(other.Value);

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
    public int CompareTo(JsonDecimalNode other)
        => Value.CompareTo(other.Value);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
    public int CompareTo(decimal other)
        => Value.CompareTo(other);

    /// <summary>
    /// Gets the value of the element as a boolean.
    /// </summary>
    /// <returns>The value of the element as a boolean.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    public bool GetBoolean(bool strict)
    {
        if (Value == 0) return false;
        if (Value == 1) return true;
        throw new InvalidOperationException("Expect a boolean but it is a number.");
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out bool result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = Value > 0;
        return Value == 0 || Value == 1;
    }

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <param name="useUnixTimestamps">true if use Unix timestamp to convert; otherwise, false, to use JavaScript date ticks.</param>
    /// <returns>The value of the element as a date time.</returns>
    /// <exception cref="OverflowException">The value is out of safe integer range.</exception>
    public DateTime GetDateTime(bool useUnixTimestamps)
    {
        if (Value > JsonIntegerNode.MaxSafeInteger) throw new OverflowException("The value is greater than safe number.");
        if (Value < JsonIntegerNode.MinSafeInteger) throw new OverflowException("The value is less than safe number.");
        return useUnixTimestamps ? WebFormat.ParseUnixTimestamp((long)Value) : WebFormat.ParseDate((long)Value);
    }

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <returns>The value of the element as a date time.</returns>
    /// <exception cref="OverflowException">The value is out of safe integer range.</exception>
    public DateTime GetDateTime()
        => GetDateTime(false);

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(out DateTime result)
    {
        if (Value > long.MaxValue || Value < long.MinValue) return base.TryConvert(out result);
        try
        {
            result = WebFormat.ParseDate((long)Value);
            return true;
        }
        catch (OverflowException)
        {
        }
        catch (InvalidCastException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a floating number.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public decimal GetDecimal() => (decimal)Value;

    /// <summary>
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a floating number.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public decimal GetDecimal(bool strict) => (decimal)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out decimal result)
    {
        try
        {
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
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public float GetSingle() => (float)Value;

    /// <summary>
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a floating number.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public float GetSingle(bool strict) => (float)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out float result)
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
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a floating number.</returns>
    public double GetDouble() => Value;

    /// <summary>
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a floating number.</returns>
    public double GetDouble(bool strict) => Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out double result)
    {
        result = Value;
        return true;
    }

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public short GetInt16() => (short)Value;

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public short GetInt16(bool strict) => (short)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out short result)
    {
        try
        {
            result = (short)Value;
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
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public uint GetUInt32() => (uint)Value;

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public uint GetUInt32(bool strict) => (uint)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out uint result)
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
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public int GetInt32() => (int)Value;

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public int GetInt32(bool strict) => (int)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out int result)
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
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public long GetInt64() => (long)Value;

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public long GetInt64(bool strict) => (long)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out long result)
    {
        try
        {
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
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public ulong GetUInt64() => (ulong)Value;

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public ulong GetUInt64(bool strict) => (ulong)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out ulong result)
    {
        try
        {
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
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out string result)
    {
        result = ToString();
        return !strict;
    }

    /// <inheritdoc />
    public override JsonValue ToJsonValue()
        => JsonValue.Create(Value);

    double IConvertible.ToDouble(IFormatProvider provider)
        => Value;

    /// <summary>
    /// Parses.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
    /// <returns>The JSON value node.</returns>
    public static JsonDoubleNode Parse(string s, IFormatProvider provider = null)
    {
        var i = provider is null ? double.Parse(s) : double.Parse(s, provider);
        return new(i);
    }

    /// <summary>
    /// Tries to parse.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>The JSON value node parsed; or null, if failed to parse.</returns>
    public static JsonDoubleNode TryParse(string s)
        => double.TryParse(s, out var i) ? new(i) : default;

    /// <summary>
    /// Tries to parse.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
    /// <param name="result">The JSON value node parsed.</param>
    /// <returns>true if parse succeeded; otherwise, false..</returns>
    public static bool TryParse(string s, IFormatProvider provider, out JsonDoubleNode result)
    {
        if (provider is null ? double.TryParse(s, out var i) : double.TryParse(s, NumberStyles.Number, provider, out i))
        {
            result = new(i);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonDoubleNode(float value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonDoubleNode(double value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonDoubleNode(decimal value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonDoubleNode(JsonValue value)
    {
        if (value is null) return null;
        if (value.TryGetValue(out double d))
            return new JsonDoubleNode(d);
        if (value.TryGetValue(out float f))
            return new JsonDoubleNode(f);
        if (value.TryGetValue(out decimal de))
            return new JsonDoubleNode(de);
        if (value.TryGetValue(out int i))
            return new JsonDoubleNode(i);
        if (value.TryGetValue(out uint ui))
            return new JsonDoubleNode(ui);
        if (value.TryGetValue(out long l))
            return new JsonDoubleNode(l);
        if (value.TryGetValue(out ulong ul))
            return new JsonDoubleNode(ul);
        if (value.TryGetValue(out bool b))
            return new JsonDoubleNode(b ? 1 : 0);
        if (value.TryGetValue(out string s) && double.TryParse(s, out var d2))
            return new JsonDoubleNode(d2);
        throw new InvalidCastException("Expect an float number to convert.");
    }

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonDoubleNode(JsonNode value)
    {
        if (value is null) return null;
        if (value is JsonValue v) return v;
        throw new InvalidCastException($"Only supports JsonValue but its type is {value.GetType().Name}.");
    }

    /// <summary>
    /// Converts to a JSON string.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON string instance.</returns>
    public static explicit operator JsonStringNode(JsonDoubleNode json)
        => new(json.ToString());

    /// <summary>
    /// Converts to a JSON integer object.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON integer instance.</returns>
    public static explicit operator JsonIntegerNode(JsonDoubleNode json)
        => new((long)json.Value);

    /// <summary>
    /// Converts to a JSON integer object.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON integer instance.</returns>
    public static explicit operator JsonDecimalNode(JsonDoubleNode json)
        => new(json.Value);

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDoubleNode leftValue, IJsonValueNode<double> rightValue)
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
    public static bool operator !=(JsonDoubleNode leftValue, IJsonValueNode<double> rightValue)
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
    public static bool operator <(JsonDoubleNode leftValue, IJsonValueNode<double> rightValue)
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
    public static bool operator >(JsonDoubleNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (rightValue is null) return false;
        return leftValue.Value > rightValue.Value;
    }

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDoubleNode leftValue, IJsonValueNode<double> rightValue)
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
    public static bool operator >=(JsonDoubleNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return true;
        if (rightValue is null) return false;
        return leftValue.Value >= rightValue.Value;
    }

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDoubleNode leftValue, IJsonValueNode<long> rightValue)
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
    public static bool operator !=(JsonDoubleNode leftValue, IJsonValueNode<long> rightValue)
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
    public static bool operator <(JsonDoubleNode leftValue, IJsonValueNode<long> rightValue)
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
    public static bool operator >(JsonDoubleNode leftValue, IJsonValueNode<long> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (rightValue is null) return false;
        return leftValue.Value > rightValue.Value;
    }

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDoubleNode leftValue, IJsonValueNode<long> rightValue)
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
    public static bool operator >=(JsonDoubleNode leftValue, IJsonValueNode<long> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return true;
        if (rightValue is null) return false;
        return leftValue.Value >= rightValue.Value;
    }

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDoubleNode leftValue, double rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDoubleNode leftValue, double rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDoubleNode leftValue, double rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDoubleNode leftValue, double rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDoubleNode leftValue, double rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDoubleNode leftValue, double rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDoubleNode leftValue, float rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDoubleNode leftValue, float rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDoubleNode leftValue, float rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDoubleNode leftValue, float rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDoubleNode leftValue, float rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDoubleNode leftValue, float rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDoubleNode leftValue, long rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDoubleNode leftValue, long rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDoubleNode leftValue, long rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDoubleNode leftValue, long rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDoubleNode leftValue, long rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDoubleNode leftValue, long rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDoubleNode leftValue, int rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDoubleNode leftValue, int rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDoubleNode leftValue, int rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDoubleNode leftValue, int rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDoubleNode leftValue, int rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDoubleNode leftValue, int rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDoubleNode leftValue, uint rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDoubleNode leftValue, uint rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDoubleNode leftValue, uint rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDoubleNode leftValue, uint rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDoubleNode leftValue, uint rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDoubleNode leftValue, uint rightValue)
        => leftValue.Value >= rightValue;
}

/// <summary>
/// Represents a specific JSON decimal float number value.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(JsonObjectNodeConverter))]
public sealed class JsonDecimalNode : BaseJsonValueNode<decimal>, IJsonNumberNode, IComparable<JsonIntegerNode>, IComparable<JsonDoubleNode>, IComparable<JsonDecimalNode>, IComparable<uint>, IComparable<int>, IComparable<long>, IComparable<double>, IComparable<float>, IComparable<decimal>, IEquatable<uint>, IEquatable<int>, IEquatable<long>, IEquatable<double>, IEquatable<float>, IFormattable, IConvertible
#if NET8_0_OR_GREATER
    , IParsable<JsonDecimalNode>
#endif
{
    /// <summary>
    /// Initializes a new instance of the JsonDecimalNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDecimalNode(int value)
        : base(JsonValueKind.Number, value)
    {
        IsInteger = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonDecimalNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDecimalNode(long value)
        : base(JsonValueKind.Number, value)
    {
        IsInteger = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonDecimalNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDecimalNode(uint value)
        : base(JsonValueKind.Number, value)
    {
        IsInteger = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonDecimalNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDecimalNode(ulong value)
        : base(JsonValueKind.Number, value)
    {
        IsInteger = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonDecimalNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDecimalNode(float value)
        : base(float.IsNaN(value) || float.IsInfinity(value) ? JsonValueKind.Null : JsonValueKind.Number, (decimal)value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value)) return;
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
    /// Initializes a new instance of the JsonDecimalNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDecimalNode(double value)
        : base(double.IsNaN(value) || double.IsInfinity(value) ? JsonValueKind.Null : JsonValueKind.Number, (decimal)value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) return;
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
    /// Initializes a new instance of the JsonDecimalNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDecimalNode(decimal value)
        : base(JsonValueKind.Number, value)
    {
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
    /// Initializes a new instance of the JsonDecimalNode class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonDecimalNode(TimeSpan value)
        : base(JsonValueKind.Number, (decimal)value.TotalSeconds)
    {
        IsInteger = value.Milliseconds == 0;
    }

    /// <summary>
    /// Gets a value indicating whether the number value is an whole number.
    /// </summary>
    public bool IsInteger { get; }

    /// <summary>
    /// Gets the JSON format string of the value.
    /// </summary>
    /// <returns>The JSON format string of the float number.</returns>
    public override string ToString()
        => Value.ToString("g", CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
    /// <returns>The JSON format string of the integer.</returns>
    public string ToString(IFormatProvider provider)
        => Value.ToString(provider);

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// <param name="format">A numeric format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about this instance.</param>
    /// <returns>The JSON format string of the integer.</returns>
    public string ToString(string format, IFormatProvider provider = null)
        => provider != null ? Value.ToString(format, provider) : Value.ToString(format);

    /// <summary>
    /// Converts to a specific positional notation format string.
    /// </summary>
    /// <param name="type">The positional notation.</param>
    /// <returns>A string of the number in the specific positional notation.</returns>
    public string ToPositionalNotationString(int type)
        => Maths.Numbers.ToPositionalNotationString(Value, type);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(IJsonValueNode<decimal> other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value);
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(IJsonNumberNode other)
    {
        if (other is null) return false;
        if (other is IJsonValueNode<decimal> n) return Value.Equals(n.Value);
        return Value.Equals(other.GetDouble());
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(IJsonValueNode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return ValueKind == JsonValueKind.Null || ValueKind == JsonValueKind.Undefined;
        if (other is IJsonNumberNode num) return Equals(num);
        if (other is IJsonValueNode<int> i1) return Value.Equals(i1.Value);
        if (other is IJsonValueNode<long> i2) return Value.Equals(i2.Value);
        if (other is IJsonValueNode<short> i3) return Value.Equals(i3.Value);
        if (other is IJsonValueNode<byte> i5) return Value.Equals(i5.Value);
        if (other is IJsonValueNode<uint> i6) return Value.Equals(i6.Value);
        if (other is IJsonValueNode<ulong> i7) return Value.Equals(i7.Value);
        if (other is IJsonValueNode<ushort> i8) return Value.Equals(i8.Value);
        if (other is IJsonValueNode<float> f1) return Value.Equals(f1.Value);
        if (other is IJsonValueNode<double> f2) return Value.Equals(f2.Value);
        if (other is IJsonValueNode<decimal> f3) return Value.Equals(f3.Value);
#if NET6_0_OR_GREATER
        if (other is IJsonValueNode<Half> f4) return Value.Equals(f4.Value);
#endif
#if NET8_0_OR_GREATER
        if (other is IJsonValueNode<Int128> i4) return Value.Equals(i4.Value);
        if (other is IJsonValueNode<UInt128> i9) return Value.Equals(i9.Value);
#endif
        if (other is IJsonValueNode<string> s) return !string.IsNullOrEmpty(s.Value) && decimal.TryParse(s.Value, out var parsed) && parsed == Value;
        return false;
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(double other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(float other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(decimal other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(long other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(uint other)
        => Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(int other)
        => Value.Equals(other);

    /// <inheritdoc />
    public override bool Equals(object other)
        => base.Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => base.GetHashCode();

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
    public int CompareTo(JsonIntegerNode other)
        => Value.CompareTo(other.Value);

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
    public int CompareTo(JsonDoubleNode other)
        => Value.CompareTo(other.Value);

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
    public int CompareTo(JsonDecimalNode other)
        => Value.CompareTo(other.Value);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
        => Value.CompareTo(other);

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
    public int CompareTo(decimal other)
        => Value.CompareTo(other);


    /// <summary>
    /// Gets the value of the element as a boolean.
    /// </summary>
    /// <returns>The value of the element as a boolean.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    public bool GetBoolean(bool strict)
    {
        if (Value == 0) return false;
        if (Value == 1) return true;
        throw new InvalidOperationException("Expect a boolean but it is a number.");
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out bool result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = Value > 0;
        return Value == 0 || Value == 1;
    }

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <param name="useUnixTimestamps">true if use Unix timestamp to convert; otherwise, false, to use JavaScript date ticks.</param>
    /// <returns>The value of the element as a date time.</returns>
    /// <exception cref="OverflowException">The value is out of safe integer range.</exception>
    public DateTime GetDateTime(bool useUnixTimestamps)
    {
        if (Value > JsonIntegerNode.MaxSafeInteger) throw new OverflowException("The value is greater than safe number.");
        if (Value < JsonIntegerNode.MinSafeInteger) throw new OverflowException("The value is less than safe number.");
        return useUnixTimestamps ? WebFormat.ParseUnixTimestamp((long)Value) : WebFormat.ParseDate((long)Value);
    }

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <returns>The value of the element as a date time.</returns>
    /// <exception cref="OverflowException">The value is out of safe integer range.</exception>
    public DateTime GetDateTime()
        => GetDateTime(false);

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(out DateTime result)
    {
        if (Value > long.MaxValue || Value < long.MinValue) return base.TryConvert(out result);
        try
        {
            result = WebFormat.ParseDate((long)Value);
            return true;
        }
        catch (OverflowException)
        {
        }
        catch (InvalidCastException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a floating number.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public decimal GetDecimal() => Value;

    /// <summary>
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a floating number.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public decimal GetDecimal(bool strict) => Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out decimal result)
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
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public float GetSingle() => (float)Value;

    /// <summary>
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a floating number.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public float GetSingle(bool strict) => (float)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out float result)
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
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a floating number.</returns>
    public double GetDouble() => (double)Value;

    /// <summary>
    /// Gets the value of the element as a floating number.
    /// </summary>
    /// <returns>The value of the element as a floating number.</returns>
    public double GetDouble(bool strict) => (double)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out double result)
    {
        result = (double)Value;
        return true;
    }

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public short GetInt16() => (short)Value;

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public short GetInt16(bool strict) => (short)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out short result)
    {
        try
        {
            result = (short)Value;
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
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public uint GetUInt32() => (uint)Value;

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public uint GetUInt32(bool strict) => (uint)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out uint result)
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
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public int GetInt32() => (int)Value;

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public int GetInt32(bool strict) => (int)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out int result)
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
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public long GetInt64() => (long)Value;

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public long GetInt64(bool strict) => (long)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out long result)
    {
        try
        {
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
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public ulong GetUInt64() => (ulong)Value;

    /// <summary>
    /// Gets the value of the element as an integer.
    /// </summary>
    /// <returns>The value of the element as an integer.</returns>
    /// <exception cref="OverflowException">The value is out of range.</exception>
    /// <exception cref="InvalidCastException">The value is out of range.</exception>
    public ulong GetUInt64(bool strict) => (ulong)Value;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out ulong result)
    {
        try
        {
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

    /// <inheritdoc />
    public override JsonValue ToJsonValue()
        => JsonValue.Create(Value);

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out string result)
    {
        result = ToString();
        return !strict;
    }

    decimal IConvertible.ToDecimal(IFormatProvider provider)
        => Value;

    /// <summary>
    /// Parses.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
    /// <returns>The JSON value node.</returns>
    public static JsonDecimalNode Parse(string s, IFormatProvider provider = null)
    {
        var i = provider is null ? decimal.Parse(s) : decimal.Parse(s, provider);
        return new(i);
    }

    /// <summary>
    /// Tries to parse.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>The JSON value node parsed; or null, if failed to parse.</returns>
    public static JsonDecimalNode TryParse(string s)
        => decimal.TryParse(s, out var i) ? new(i) : default;

    /// <summary>
    /// Tries to parse.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
    /// <param name="result">The JSON value node parsed.</param>
    /// <returns>true if parse succeeded; otherwise, false..</returns>
    public static bool TryParse(string s, IFormatProvider provider, out JsonDecimalNode result)
    {
        if (provider is null ? decimal.TryParse(s, out var i) : decimal.TryParse(s, NumberStyles.Number, provider, out i))
        {
            result = new(i);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonDecimalNode(float value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonDecimalNode(double value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonDecimalNode(decimal value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonDecimalNode(JsonValue value)
    {
        if (value is null) return null;
        if (value.TryGetValue(out double d))
            return new(d);
        if (value.TryGetValue(out float f))
            return new(f);
        if (value.TryGetValue(out decimal de))
            return new(de);
        if (value.TryGetValue(out int i))
            return new(i);
        if (value.TryGetValue(out uint ui))
            return new(ui);
        if (value.TryGetValue(out long l))
            return new(l);
        if (value.TryGetValue(out ulong ul))
            return new(ul);
        if (value.TryGetValue(out bool b))
            return new(b ? 1 : 0);
        if (value.TryGetValue(out string s) && double.TryParse(s, out var d2))
            return new(d2);
        throw new InvalidCastException("Expect an float number to convert.");
    }

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonDecimalNode(JsonNode value)
    {
        if (value is null) return null;
        if (value is JsonValue v) return v;
        throw new InvalidCastException($"Only supports JsonValue but its type is {value.GetType().Name}.");
    }

    /// <summary>
    /// Converts to a JSON string.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON string instance.</returns>
    public static explicit operator JsonStringNode(JsonDecimalNode json)
        => new(json.ToString());

    /// <summary>
    /// Converts to a JSON integer object.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON integer instance.</returns>
    public static explicit operator JsonIntegerNode(JsonDecimalNode json)
        => new((long)json.Value);

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDecimalNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return true;
        if (rightValue is null) return false;
        return (double)leftValue.Value == rightValue.Value;
    }

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDecimalNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (rightValue is null) return true;
        return (double)leftValue.Value != rightValue.Value;
    }

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDecimalNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (rightValue is null) return true;
        return (double)leftValue.Value < rightValue.Value;
    }

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDecimalNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (rightValue is null) return false;
        return (double)leftValue.Value > rightValue.Value;
    }

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDecimalNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return true;
        if (rightValue is null) return true;
        return (double)leftValue.Value <= rightValue.Value;
    }

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDecimalNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return true;
        if (rightValue is null) return false;
        return (double)leftValue.Value >= rightValue.Value;
    }

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDecimalNode leftValue, IJsonValueNode<decimal> rightValue)
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
    public static bool operator !=(JsonDecimalNode leftValue, IJsonValueNode<decimal> rightValue)
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
    public static bool operator <(JsonDecimalNode leftValue, IJsonValueNode<decimal> rightValue)
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
    public static bool operator >(JsonDecimalNode leftValue, IJsonValueNode<decimal> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (rightValue is null) return false;
        return leftValue.Value > rightValue.Value;
    }

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDecimalNode leftValue, IJsonValueNode<decimal> rightValue)
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
    public static bool operator >=(JsonDecimalNode leftValue, IJsonValueNode<decimal> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return true;
        if (rightValue is null) return false;
        return leftValue.Value >= rightValue.Value;
    }

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDecimalNode leftValue, IJsonValueNode<long> rightValue)
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
    public static bool operator !=(JsonDecimalNode leftValue, IJsonValueNode<long> rightValue)
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
    public static bool operator <(JsonDecimalNode leftValue, IJsonValueNode<long> rightValue)
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
    public static bool operator >(JsonDecimalNode leftValue, IJsonValueNode<long> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (rightValue is null) return false;
        return leftValue.Value > rightValue.Value;
    }

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDecimalNode leftValue, IJsonValueNode<long> rightValue)
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
    public static bool operator >=(JsonDecimalNode leftValue, IJsonValueNode<long> rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return true;
        if (rightValue is null) return false;
        return leftValue.Value >= rightValue.Value;
    }

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDecimalNode leftValue, double rightValue)
        => (double)leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDecimalNode leftValue, double rightValue)
        => (double)leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDecimalNode leftValue, double rightValue)
        => (double)leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDecimalNode leftValue, double rightValue)
        => (double)leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDecimalNode leftValue, double rightValue)
        => (double)leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDecimalNode leftValue, double rightValue)
        => (double)leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDecimalNode leftValue, float rightValue)
        => (float)leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDecimalNode leftValue, float rightValue)
        => (float)leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDecimalNode leftValue, float rightValue)
        => (float)leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDecimalNode leftValue, float rightValue)
        => (float)leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDecimalNode leftValue, float rightValue)
        => (float)leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDecimalNode leftValue, float rightValue)
        => (float)leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDecimalNode leftValue, decimal rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDecimalNode leftValue, decimal rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDecimalNode leftValue, decimal rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDecimalNode leftValue, decimal rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDecimalNode leftValue, decimal rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDecimalNode leftValue, decimal rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDecimalNode leftValue, long rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDecimalNode leftValue, long rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDecimalNode leftValue, long rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDecimalNode leftValue, long rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDecimalNode leftValue, long rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDecimalNode leftValue, long rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDecimalNode leftValue, int rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDecimalNode leftValue, int rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDecimalNode leftValue, int rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDecimalNode leftValue, int rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDecimalNode leftValue, int rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDecimalNode leftValue, int rightValue)
        => leftValue.Value >= rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonDecimalNode leftValue, uint rightValue)
        => leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonDecimalNode leftValue, uint rightValue)
        => leftValue.Value != rightValue;

    /// <summary>
    /// Compares if left is smaller than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
    public static bool operator <(JsonDecimalNode leftValue, uint rightValue)
        => leftValue.Value < rightValue;

    /// <summary>
    /// Compares if left is greater than right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
    public static bool operator >(JsonDecimalNode leftValue, uint rightValue)
        => leftValue.Value > rightValue;

    /// <summary>
    /// Compares if left is smaller than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
    public static bool operator <=(JsonDecimalNode leftValue, uint rightValue)
        => leftValue.Value <= rightValue;

    /// <summary>
    /// Compares if left is greater than or equals to right.
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
    public static bool operator >=(JsonDecimalNode leftValue, uint rightValue)
        => leftValue.Value >= rightValue;
}
