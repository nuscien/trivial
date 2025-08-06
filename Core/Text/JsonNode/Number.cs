
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Trivial.Data;
using Trivial.Maths;
using Trivial.Reflection;
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
[System.Text.Json.Serialization.JsonConverter(typeof(JsonValueNodeConverter.IntegerConverter))]
[Guid("46D19567-7B97-42B8-9BC5-01CFB01E320B")]
public sealed class JsonIntegerNode : BaseJsonValueNode<long>, IObjectRef<int>, IObjectRef<decimal>, IObjectRef<float>, IObjectRef<double>, IJsonNumberNode, IComparable<JsonIntegerNode>, IComparable<JsonDoubleNode>, IComparable<uint>, IComparable<int>, IComparable<long>, IComparable<double>, IComparable<float>, IEquatable<uint>, IEquatable<int>, IEquatable<float>, IEquatable<double>, IFormattable, IConvertible, IAdvancedAdditionCapable<JsonIntegerNode>
#if NET8_0_OR_GREATER
    , IParsable<JsonIntegerNode>, IUnaryNegationOperators<JsonIntegerNode, JsonIntegerNode>, IAdditionOperators<JsonIntegerNode, IJsonValueNode<long>, JsonIntegerNode>, IAdditionOperators<JsonIntegerNode, IJsonValueNode<int>, JsonIntegerNode>, IAdditionOperators<JsonIntegerNode, long, JsonIntegerNode>, IAdditionOperators<JsonIntegerNode, int, JsonIntegerNode>, ISubtractionOperators<JsonIntegerNode, IJsonValueNode<long>, JsonIntegerNode>, ISubtractionOperators<JsonIntegerNode, IJsonValueNode<int>, JsonIntegerNode>, ISubtractionOperators<JsonIntegerNode, long, JsonIntegerNode>, ISubtractionOperators<JsonIntegerNode, int, JsonIntegerNode>
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
        : base(JsonValueKind.Number, isUnixTimestamp ? WebFormat.ParseUnixTimestamp(value) : WebFormat.ParseDate(value))
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
    /// Gets a value indicating whether the value is negative.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsNegative => Value < 0;

    /// <summary>
    /// Gets a value indicating whether the current value is zero.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IAdvancedAdditionCapable<JsonIntegerNode>.IsZero => Value == 0L;

    /// <summary>
    /// Gets the source value.
    /// </summary>
    int IObjectRef<int>.Value => (int)Value;

    /// <summary>
    /// Gets the source value.
    /// </summary>
    decimal IObjectRef<decimal>.Value => Value;

    /// <summary>
    /// Gets the source value.
    /// </summary>
    float IObjectRef<float>.Value => Value;

    /// <summary>
    /// Gets the source value.
    /// </summary>
    double IObjectRef<double>.Value => Value;

    /// <summary>
    /// Tests if the number value is macthed by the specific condition.
    /// </summary>
    /// <param name="condition">The condition to test the number value.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(Int64Condition condition)
        => condition == null || condition.IsMatched(Value);

    /// <summary>
    /// Tests if the number value is macthed by the specific condition.
    /// </summary>
    /// <param name="condition">The condition to test the number value.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(Int32Condition condition)
        => condition == null || condition.IsMatched(Value);

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
        => Numbers.ToPositionalNotationString(Value, type);

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
        if (other is IJsonValueNode<string> s) return !string.IsNullOrEmpty(s.Value) && Numbers.TryParseToInt64(s.Value, 10, out var intParsed) && intParsed == Value;
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

#if NET8_0_OR_GREATER
    /// <summary>
    /// Tries to format the value of the current double instance into the provided span of characters.
    /// </summary>
    /// <param name="destination">The span in which to write this instance's value formatted as a span of characters.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters that were written in destination.</param>
    /// <param name="format">A span containing the characters that represent a standard or custom format string that defines the acceptable format for destination.</param>
    /// <param name="provider">An optional object that supplies culture-specific formatting information for destination.</param>
    /// <returns>true if the formatting was successful; otherwise, false.</returns>
    public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax("NumericFormat")] ReadOnlySpan<char> format = default, IFormatProvider provider = null)
        => Value.TryFormat(destination, out charsWritten, format, provider);

    /// <summary>
    /// Tries to format the value of the current instance as UTF-8 into the provided span of bytes.
    /// </summary>
    /// <param name="utf8Destination">The span in which to write this instance's value formatted as a span of bytes.</param>
    /// <param name="bytesWritten">When this method returns, contains the number of bytes that were written in utf8Destination.</param>
    /// <param name="format">A span containing the characters that represent a standard or custom format string that defines the acceptable format for utf8Destination.</param>
    /// <param name="provider">An optional object that supplies culture-specific formatting information for utf8Destination.</param>
    /// <returns>true if the formatting was successful; otherwise, false.</returns>
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, [StringSyntax("NumericFormat")] ReadOnlySpan<char> format = default, IFormatProvider provider = null)
        => Value.TryFormat(utf8Destination, out bytesWritten, format, provider);
#endif

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

    /// <summary>
    /// Writes this instance to the specified writer as a JSON value.
    /// </summary>
    /// <param name="writer">The writer to which to write this instance.</param>
    public override void WriteTo(Utf8JsonWriter writer)
        => writer?.WriteNumberValue(Value);

    long IConvertible.ToInt64(IFormatProvider provider)
        => Value;

    /// <summary>
    /// Creates an element descibed zero.
    /// </summary>
    /// <returns>An JSON number node about zero.</returns>
    JsonIntegerNode IAdvancedAdditionCapable<JsonIntegerNode>.GetElementZero()
        => Value == 0L ? this : new(0L);

    /// <summary>
    /// Pluses another value to return.
    /// </summary>
    /// <param name="value">A given value to be added.</param>
    /// <returns>A result after addition.</returns>
    public JsonIntegerNode Plus(JsonIntegerNode value)
    {
        if (value is null || value.Value == 0L) return this;
        if (Value == 0L) return value;
        return new(Value + value.Value);
    }

    /// <summary>
    /// Minuses another value to return.
    /// </summary>
    /// <param name="value">A given value to be added.</param>
    /// <returns>A result after subtraction.</returns>
    public JsonIntegerNode Minus(JsonIntegerNode value)
        => value is null || value.Value == 0L ? this : new(Value - value.Value);

    /// <summary>
    /// Negates the current value to return.
    /// </summary>
    /// <returns>A result after negation.</returns>
    public JsonIntegerNode Negate()
        => Value == 0L ? this : new(-Value);

    /// <summary>
    /// Parses.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>The JSON value node parsed.</returns>
    public static JsonIntegerNode Parse(string s)
    {
        var i = Numbers.ParseToInt64(s, 10);
        return new(i);
    }

    /// <summary>
    /// Tries to parse.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>The JSON value node parsed; or null, if failed to parse.</returns>
    public static JsonIntegerNode TryParse(string s)
        => Numbers.TryParseToInt64(s, 10, out var i) ? new(i) : default;

    /// <summary>
    /// Tries to parse.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="result">The JSON value node parsed.</param>
    /// <returns>true if parse succeeded; otherwise, false..</returns>
    public static bool TryParse(string s, out JsonIntegerNode result)
    {
        if (Numbers.TryParseToInt64(s, 10, out var i))
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

#if NET8_0_OR_GREATER
    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A number.</returns>
    public static explicit operator Int128(JsonIntegerNode json)
        => json.Value;
#endif

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A number.</returns>
    public static explicit operator BigInteger(JsonIntegerNode json)
        => json.Value;

    /// <summary>
    /// Converts to a JSON string.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON string instance.</returns>
    public static explicit operator JsonStringNode(JsonIntegerNode json)
        => new(json?.ToString());

    /// <summary>
    /// Converts to a JSON double object.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON double instance.</returns>
    public static explicit operator JsonDoubleNode(JsonIntegerNode json)
        => new(json?.Value ?? double.NaN);

    /// <summary>
    /// Converts to a JSON double object.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A JSON double instance.</returns>
    /// <exception cref="ArgumentNullException">The value is null.</exception>
    public static explicit operator JsonDecimalNode(JsonIntegerNode json)
        => new(json.Value);

    /// <summary>
    /// Converts to string builder.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    /// <exception cref="ArgumentNullException">The value is null.</exception>
    public static explicit operator JsonEncodedText(JsonIntegerNode json)
        => JsonEncodedText.Encode(json?.ToString());

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

    /// <summary>
    /// Negation.
    /// </summary>
    /// <param name="value">The left value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator -(JsonIntegerNode value)
        => value?.Negate() ?? new(0L);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator +(JsonIntegerNode leftValue, IJsonValueNode<long> rightValue)
    {
        if (rightValue is null) return leftValue;
        if (leftValue is null || leftValue.Value == 0L) return rightValue is JsonIntegerNode node ? node : new(rightValue.Value);
        if (rightValue.Value == 0L) return leftValue;
        return new(leftValue.Value + rightValue.Value);
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator -(JsonIntegerNode leftValue, IJsonValueNode<long> rightValue)
        => rightValue is null || rightValue.Value == 0L ? leftValue : new((leftValue?.Value ?? 0L) - rightValue.Value);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator +(JsonIntegerNode leftValue, IJsonValueNode<int> rightValue)
        => rightValue is null ? leftValue : new((leftValue?.Value ?? 0L) + rightValue.Value);

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator -(JsonIntegerNode leftValue, IJsonValueNode<int> rightValue)
        => (rightValue is null || rightValue.Value == 0) ? leftValue : new((leftValue?.Value ?? 0L) - rightValue.Value);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator +(JsonIntegerNode leftValue, long rightValue)
    {
        if (leftValue is null) return new(rightValue);
        if (rightValue == 0L) return leftValue;
        return new(leftValue.Value + rightValue);
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator -(JsonIntegerNode leftValue, long rightValue)
        => rightValue == 0L ? leftValue : new((leftValue?.Value ?? 0L) - rightValue);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator +(long leftValue, JsonIntegerNode rightValue)
        => rightValue + leftValue;

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator -(long leftValue, JsonIntegerNode rightValue)
        => new(leftValue - (rightValue?.Value ?? 0L));

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator +(JsonIntegerNode leftValue, int rightValue)
    {
        if (leftValue is null) return new(rightValue);
        if (rightValue == 0) return leftValue;
        return new(leftValue.Value + rightValue);
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator -(JsonIntegerNode leftValue, int rightValue)
        => rightValue == 0 ? leftValue : new((leftValue?.Value ?? 0L) - rightValue);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator +(int leftValue, JsonIntegerNode rightValue)
        => rightValue + leftValue;

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonIntegerNode operator -(int leftValue, JsonIntegerNode rightValue)
        => new(leftValue - (rightValue?.Value ?? 0L));
}

/// <summary>
/// Represents a specific JSON double float number value.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(JsonValueNodeConverter.DoubleConverter))]
[Guid("25098B53-EC11-48E1-A8FD-AFB685C38BA9")]
public sealed class JsonDoubleNode : BaseJsonValueNode<double>, IObjectRef<float>, IJsonNumberNode, IComparable<JsonIntegerNode>, IComparable<JsonDoubleNode>, IComparable<JsonDecimalNode>, IComparable<uint>, IComparable<int>, IComparable<long>, IComparable<double>, IComparable<float>, IComparable<decimal>, IEquatable<uint>, IEquatable<int>, IEquatable<long>, IEquatable<float>, IEquatable<decimal>, IFormattable, IConvertible, IAdvancedAdditionCapable<JsonDoubleNode>
#if NET8_0_OR_GREATER
    , IParsable<JsonDoubleNode>, IUnaryNegationOperators<JsonDoubleNode, JsonDoubleNode>, IAdditionOperators<JsonDoubleNode, IJsonValueNode<double>, JsonDoubleNode>, IAdditionOperators<JsonDoubleNode, IJsonValueNode<float>, JsonDoubleNode>, IAdditionOperators<JsonDoubleNode, double, JsonDoubleNode>, ISubtractionOperators<JsonDoubleNode, IJsonValueNode<double>, JsonDoubleNode>, ISubtractionOperators<JsonDoubleNode, IJsonValueNode<float>, JsonDoubleNode>, ISubtractionOperators<JsonDoubleNode, double, JsonDoubleNode>
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
    /// Gets a value indicating whether the value is negative.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsNegative => Value < 0d;

    /// <summary>
    /// Gets a value indicating whether the current value is zero.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IAdvancedAdditionCapable<JsonDoubleNode>.IsZero => Value == 0d;

    /// <summary>
    /// Gets the source value.
    /// </summary>
    float IObjectRef<float>.Value => (float)Value;

    /// <summary>
    /// Tests if the number value is macthed by the specific condition.
    /// </summary>
    /// <param name="condition">The condition to test the number value.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(DoubleCondition condition)
        => condition == null || condition.IsMatched(Value);

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
        => Numbers.ToPositionalNotationString(Value, type);

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

#if NET8_0_OR_GREATER
    /// <summary>
    /// Tries to format the value of the current double instance into the provided span of characters.
    /// </summary>
    /// <param name="destination">The span in which to write this instance's value formatted as a span of characters.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters that were written in destination.</param>
    /// <param name="format">A span containing the characters that represent a standard or custom format string that defines the acceptable format for destination.</param>
    /// <param name="provider">An optional object that supplies culture-specific formatting information for destination.</param>
    /// <returns>true if the formatting was successful; otherwise, false.</returns>
    public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax("NumericFormat")] ReadOnlySpan<char> format = default, IFormatProvider provider = null)
        => Value.TryFormat(destination, out charsWritten, format, provider);

    /// <summary>
    /// Tries to format the value of the current instance as UTF-8 into the provided span of bytes.
    /// </summary>
    /// <param name="utf8Destination">The span in which to write this instance's value formatted as a span of bytes.</param>
    /// <param name="bytesWritten">When this method returns, contains the number of bytes that were written in utf8Destination.</param>
    /// <param name="format">A span containing the characters that represent a standard or custom format string that defines the acceptable format for utf8Destination.</param>
    /// <param name="provider">An optional object that supplies culture-specific formatting information for utf8Destination.</param>
    /// <returns>true if the formatting was successful; otherwise, false.</returns>
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, [StringSyntax("NumericFormat")] ReadOnlySpan<char> format = default, IFormatProvider provider = null)
        => Value.TryFormat(utf8Destination, out bytesWritten, format, provider);
#endif

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

    /// <summary>
    /// Writes this instance to the specified writer as a JSON value.
    /// </summary>
    /// <param name="writer">The writer to which to write this instance.</param>
    public override void WriteTo(Utf8JsonWriter writer)
    {
        if (writer == null) return;
        if (double.IsNaN(Value)) writer.WriteNullValue();
        else writer.WriteNumberValue(Value);
    }

    double IConvertible.ToDouble(IFormatProvider provider)
        => Value;

    /// <summary>
    /// Creates an element descibed zero.
    /// </summary>
    /// <returns>An JSON number node about zero.</returns>
    JsonDoubleNode IAdvancedAdditionCapable<JsonDoubleNode>.GetElementZero()
        => Value == 0L ? this : new(0d);

    /// <summary>
    /// Pluses another value to return.
    /// </summary>
    /// <param name="value">A given value to be added.</param>
    /// <returns>A result after addition.</returns>
    public JsonDoubleNode Plus(JsonDoubleNode value)
    {
        if (value is null || value.Value == 0d) return this;
        if (Value == 0d) return value;
        return new(Value + value.Value);
    }

    /// <summary>
    /// Minuses another value to return.
    /// </summary>
    /// <param name="value">A given value to be added.</param>
    /// <returns>A result after subtraction.</returns>
    public JsonDoubleNode Minus(JsonDoubleNode value)
        => value is null || value.Value == 0d ? this : new(Value - value.Value);

    /// <summary>
    /// Negates the current value to return.
    /// </summary>
    /// <returns>A result after negation.</returns>
    public JsonDoubleNode Negate()
        => Value == 0d ? this : new(-Value);

    /// <summary>
    /// Parses.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about s.</param>
    /// <returns>The JSON value node.</returns>
    public static JsonDoubleNode Parse(string s, IFormatProvider provider = null)
        => new(provider is null ? double.Parse(s) : double.Parse(s, provider));

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

    /// <summary>
    /// Negation.
    /// </summary>
    /// <param name="value">The left value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDoubleNode operator -(JsonDoubleNode value)
        => value?.Negate() ?? new(0d);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDoubleNode operator +(JsonDoubleNode leftValue, IJsonValueNode<double> rightValue)
    {
        if (rightValue is null) return leftValue;
        if (leftValue is null || leftValue.Value == 0d) return rightValue is JsonDoubleNode node ? node : new(rightValue.Value);
        if (rightValue.Value == 0d) return leftValue;
        return new(leftValue.Value + rightValue.Value);
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDoubleNode operator -(JsonDoubleNode leftValue, IJsonValueNode<double> rightValue)
        => rightValue is null || rightValue.Value == 0d ? leftValue : new((leftValue?.Value ?? 0d) - rightValue.Value);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDoubleNode operator +(JsonDoubleNode leftValue, IJsonValueNode<float> rightValue)
        => rightValue is null ? leftValue : new((leftValue?.Value ?? 0d) + rightValue.Value);

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDoubleNode operator -(JsonDoubleNode leftValue, IJsonValueNode<float> rightValue)
        => (rightValue is null || rightValue.Value == 0f) ? leftValue : new((leftValue?.Value ?? 0d) - rightValue.Value);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDoubleNode operator +(JsonDoubleNode leftValue, double rightValue)
    {
        if (leftValue is null) return new(rightValue);
        if (rightValue == 0d) return leftValue;
        return new(leftValue.Value + rightValue);
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDoubleNode operator -(JsonDoubleNode leftValue, double rightValue)
        => rightValue == 0d ? leftValue : new((leftValue?.Value ?? 0d) - rightValue);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDoubleNode operator +(double leftValue, JsonDoubleNode rightValue)
        => rightValue + leftValue;

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDoubleNode operator -(double leftValue, JsonDoubleNode rightValue)
        => new(leftValue - (rightValue?.Value ?? 0d));
}

/// <summary>
/// Represents a specific JSON decimal float number value.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(JsonValueNodeConverter.DecimalConverter))]
[Guid("2DE9E34B-A394-4BF7-B814-2A6765420E05")]
public sealed class JsonDecimalNode : BaseJsonValueNode<decimal>, IObjectRef<double>, IJsonNumberNode, IComparable<JsonIntegerNode>, IComparable<JsonDoubleNode>, IComparable<JsonDecimalNode>, IComparable<uint>, IComparable<int>, IComparable<long>, IComparable<double>, IComparable<float>, IComparable<decimal>, IEquatable<uint>, IEquatable<int>, IEquatable<long>, IEquatable<double>, IEquatable<float>, IFormattable, IConvertible, IAdvancedAdditionCapable<JsonDecimalNode>
#if NET8_0_OR_GREATER
    , IParsable<JsonDecimalNode>, IUnaryNegationOperators<JsonDecimalNode, JsonDecimalNode>, IAdditionOperators<JsonDecimalNode, IJsonValueNode<decimal>, JsonDecimalNode>, IAdditionOperators<JsonDecimalNode, decimal, JsonDecimalNode>, ISubtractionOperators<JsonDecimalNode, IJsonValueNode<decimal>, JsonDecimalNode>, ISubtractionOperators<JsonDecimalNode, decimal, JsonDecimalNode>
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
    /// Gets a value indicating whether the value is negative.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsNegative => Value < decimal.Zero;

    /// <summary>
    /// Gets a value indicating whether the current value is zero.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IAdvancedAdditionCapable<JsonDecimalNode>.IsZero => Value == decimal.Zero;

    /// <summary>
    /// Gets the source value.
    /// </summary>
    double IObjectRef<double>.Value => (double)Value;

    /// <summary>
    /// Tests if the number value is macthed by the specific condition.
    /// </summary>
    /// <param name="condition">The condition to test the number value.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(DecimalCondition condition)
        => condition == null || condition.IsMatched(Value);

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
        => Numbers.ToPositionalNotationString(Value, type);

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

#if NET8_0_OR_GREATER
    /// <summary>
    /// Tries to format the value of the current double instance into the provided span of characters.
    /// </summary>
    /// <param name="destination">The span in which to write this instance's value formatted as a span of characters.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters that were written in destination.</param>
    /// <param name="format">A span containing the characters that represent a standard or custom format string that defines the acceptable format for destination.</param>
    /// <param name="provider">An optional object that supplies culture-specific formatting information for destination.</param>
    /// <returns>true if the formatting was successful; otherwise, false.</returns>
    public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax("NumericFormat")] ReadOnlySpan<char> format = default, IFormatProvider provider = null)
        => Value.TryFormat(destination, out charsWritten, format, provider);

    /// <summary>
    /// Tries to format the value of the current instance as UTF-8 into the provided span of bytes.
    /// </summary>
    /// <param name="utf8Destination">The span in which to write this instance's value formatted as a span of bytes.</param>
    /// <param name="bytesWritten">When this method returns, contains the number of bytes that were written in utf8Destination.</param>
    /// <param name="format">A span containing the characters that represent a standard or custom format string that defines the acceptable format for utf8Destination.</param>
    /// <param name="provider">An optional object that supplies culture-specific formatting information for utf8Destination.</param>
    /// <returns>true if the formatting was successful; otherwise, false.</returns>
    public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, [StringSyntax("NumericFormat")] ReadOnlySpan<char> format = default, IFormatProvider provider = null)
        => Value.TryFormat(utf8Destination, out bytesWritten, format, provider);
#endif

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
    /// Writes this instance to the specified writer as a JSON value.
    /// </summary>
    /// <param name="writer">The writer to which to write this instance.</param>
    public override void WriteTo(Utf8JsonWriter writer)
        => writer?.WriteNumberValue(Value);

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
    /// Creates an element descibed zero.
    /// </summary>
    /// <returns>An JSON number node about zero.</returns>
    JsonDecimalNode IAdvancedAdditionCapable<JsonDecimalNode>.GetElementZero()
        => Value == decimal.Zero ? this : new(decimal.Zero);

    /// <summary>
    /// Pluses another value to return.
    /// </summary>
    /// <param name="value">A given value to be added.</param>
    /// <returns>A result after addition.</returns>
    public JsonDecimalNode Plus(JsonDecimalNode value)
    {
        if (value is null || value.Value == decimal.Zero) return this;
        if (Value == decimal.Zero) return value;
        return new(Value + value.Value);
    }

    /// <summary>
    /// Minuses another value to return.
    /// </summary>
    /// <param name="value">A given value to be added.</param>
    /// <returns>A result after subtraction.</returns>
    public JsonDecimalNode Minus(JsonDecimalNode value)
        => value is null || value.Value == decimal.Zero ? this : new(Value - value.Value);

    /// <summary>
    /// Negates the current value to return.
    /// </summary>
    /// <returns>A result after negation.</returns>
    public JsonDecimalNode Negate()
        => Value == decimal.Zero ? this : new(-Value);

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

    /// <summary>
    /// Negation.
    /// </summary>
    /// <param name="value">The left value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDecimalNode operator -(JsonDecimalNode value)
        => value?.Negate() ?? new(decimal.Zero);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDecimalNode operator +(JsonDecimalNode leftValue, IJsonValueNode<decimal> rightValue)
    {
        if (rightValue is null) return leftValue;
        if (leftValue is null || leftValue.Value == decimal.Zero) return rightValue is JsonDecimalNode node ? node : new(rightValue.Value);
        if (rightValue.Value == decimal.Zero) return leftValue;
        return new(leftValue.Value + rightValue.Value);
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDecimalNode operator -(JsonDecimalNode leftValue, IJsonValueNode<decimal> rightValue)
        => rightValue is null || rightValue.Value == decimal.Zero ? leftValue : new((leftValue?.Value ?? decimal.Zero) - rightValue.Value);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDecimalNode operator +(JsonDecimalNode leftValue, decimal rightValue)
    {
        if (leftValue is null) return new(rightValue);
        if (rightValue == decimal.Zero) return leftValue;
        return new(leftValue.Value + rightValue);
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDecimalNode operator -(JsonDecimalNode leftValue, decimal rightValue)
        => rightValue == decimal.Zero ? leftValue : new((leftValue?.Value ?? decimal.Zero) - rightValue);

    /// <summary>
    /// Pluses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDecimalNode operator +(decimal leftValue, JsonDecimalNode rightValue)
        => rightValue + leftValue;

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>The result number node after computing.</returns>
    public static JsonDecimalNode operator -(decimal leftValue, JsonDecimalNode rightValue)
        => new(leftValue - (rightValue?.Value ?? decimal.Zero));
}
