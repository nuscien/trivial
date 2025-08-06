using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

/// <summary>
/// The JSON value node base.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
[DebuggerDisplay("{Value}")]
public abstract class BaseJsonValueNode<T> : BaseJsonValueNode, IJsonValueNode<T>, IObjectRef<T> where T : IEquatable<T>
{
    /// <summary>
    /// Initializes a new instance of the JsonNull class.
    /// </summary>
    /// <param name="valueKind">The JSON value kind.</param>
    /// <param name="value">The source value.</param>
    internal BaseJsonValueNode(JsonValueKind valueKind, T value)
        : base(valueKind)
    {
        Value = value;
    }

    /// <inheritdoc />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected override object RawValue => Value;

    /// <summary>
    /// Gets the source value.
    /// </summary>
    public T Value { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object IObjectRef.Value => Value;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool IObjectRef.IsValueCreated => true;

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public virtual bool Equals(T other)
        => Value is null ? other is null : Value.Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public virtual bool Equals(IJsonValueNode<T> other)
        => Value is null ? other is null : Value.Equals(other.Value);

    /// <summary>
    /// Gets the JSON format string of the value.
    /// </summary>
    /// <returns>A string that represents the value.</returns>
    public override string ToString()
        => Value is null ? string.Empty : Value.ToString();

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <returns>A boolean; or null, if not supported.</returns>
    public bool? TryGetBoolean()
        => TryConvert(false, out bool v) ? v : null;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetBoolean(out bool value)
        => TryConvert(false, out value);

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <returns>A date time; or null, if not supported.</returns>
    public DateTime? TryGetDateTime()
        => TryConvert(out DateTime v) ? v : null;

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetDateTime(out DateTime value)
        => TryConvert(out value);

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>An integer; or null, if not supported.</returns>
    public short? TryGetInt16()
        => TryConvert(false, out short v) ? v : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetInt16(out short value)
        => TryConvert(false, out value);

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>An integer; or null, if not supported.</returns>
    public int? TryGetInt32()
        => TryConvert(false, out int v) ? v : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetInt32(out int value)
        => TryConvert(false, out value);

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>An integer; or null, if not supported.</returns>
    public uint? TryGetUInt32()
        => TryConvert(false, out uint v) ? v : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetUInt32(out uint value)
        => TryConvert(false, out value);

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>An integer; or null, if not supported.</returns>
    public long? TryGetInt64()
        => TryConvert(false, out long v) ? v : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetInt64(out long value)
        => TryConvert(false, out value);

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>An integer; or null, if not supported.</returns>
    public ulong? TryGetUInt64()
        => TryConvert(false, out ulong v) ? v : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetUInt64(out ulong value)
        => TryConvert(false, out value);

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <returns>A floating number. Returns Single.NaN if not supported.</returns>
    public float TryGetSingle()
        => TryConvert(false, out float v) ? v : float.NaN;

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>A floating number.</returns>
    public float TryGetSingle(float defaultValue)
        => TryConvert(false, out float v) ? v : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetSingle(out float value)
        => TryConvert(false, out value);

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <returns>A floating number. Returns Double.NaN if not supported.</returns>
    public double TryGetDouble()
        => TryConvert(false, out double v) ? v : double.NaN;

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>A floating number.</returns>
    public double TryGetDouble(double defaultValue)
        => TryConvert(false, out double v) ? v : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetDouble(out double value)
        => TryConvert(false, out value);

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <returns>A floating number; or null, if not supported.</returns>
    public decimal? TryGetDecimal()
        => TryConvert(false, out decimal v) ? v : null;

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetDeciaml(out decimal value)
        => TryConvert(false, out value);

    /// <summary>
    /// Tries to get the value of the element as a string.
    /// </summary>
    /// <returns>A string.</returns>
    public string TryGetString()
        => TryConvert(false, out string v) ? v : null;

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <returns>An instance of the JSON node.</returns>
    public override System.Text.Json.Nodes.JsonNode ToJsonNode()
        => ToJsonValue();

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <returns>An instance of the JSON value.</returns>
    public abstract System.Text.Json.Nodes.JsonValue ToJsonValue();

    /// <summary>
    /// Writes this instance to the specified writer as a JSON value.
    /// </summary>
    /// <param name="writer">The writer to which to write this instance.</param>
    public override void WriteTo(Utf8JsonWriter writer)
    {
        if (writer is null) return;
        var type = typeof(T);
        if (Value is null) writer.WriteNullValue();
        else if (type == typeof(string)) writer.WriteStringValue((string)(object)Value);
        else if (type == typeof(long)) writer.WriteNumberValue((long)(object)Value);
        else if (type == typeof(double)) writer.WriteNumberValue((double)(object)Value);
        else if (type == typeof(decimal)) writer.WriteNumberValue((decimal)(object)Value);
        else if (type == typeof(bool)) writer.WriteBooleanValue((bool)(object)Value);
        else if (type == typeof(int)) writer.WriteNumberValue((int)(object)Value);
        else if (type == typeof(float)) writer.WriteNumberValue((float)(object)Value);
        else if (type == typeof(short)) writer.WriteNumberValue((short)(object)Value);
        else if (type == typeof(DateTime)) writer.WriteStringValue((DateTime)(object)Value);
        else if (type == typeof(DateTimeOffset)) writer.WriteStringValue((DateTimeOffset)(object)Value);
        else if (type == typeof(Guid)) writer.WriteStringValue((Guid)(object)Value);
        else if (type == typeof(Uri)) writer.WriteStringValue(((Uri)(object)Value).OriginalString);
        else writer.WriteStringValue(Value.ToString());
    }

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonNode class.</returns>
    public static explicit operator System.Text.Json.Nodes.JsonValue(BaseJsonValueNode<T> json)
        => json?.ToJsonValue();
}

/// <summary>
/// Json null.
/// </summary>
internal sealed class JsonNullNode : BaseJsonValueNode
{
    /// <summary>
    /// Initializes a new instance of the JsonNull class.
    /// </summary>
    /// <param name="valueKind">The JSON value kind.</param>
    public JsonNullNode(JsonValueKind valueKind)
        : base(valueKind)
    {
    }

    /// <inheritdoc />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected override object RawValue => DBNull.Value;

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
    public override bool Equals(IJsonValueNode other)
        => other is null || ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => other.ValueKind == JsonValueKind.Null || other.ValueKind == JsonValueKind.Undefined,
            JsonValueKind.True => other.ValueKind == JsonValueKind.True,
            JsonValueKind.False => other.ValueKind == JsonValueKind.False,
            _ => false,
        };

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return true;
        if (other is IJsonValueNode json) return Equals(json);
        return other is DBNull
            || (other is double d && double.IsNaN(d))
            || (other is float f && float.IsNaN(f));
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode() => ValueKind.GetHashCode();

    /// <summary>
    /// Writes this instance to the specified writer as a JSON value.
    /// </summary>
    /// <param name="writer">The writer to which to write this instance.</param>
    public override void WriteTo(Utf8JsonWriter writer)
        => writer?.WriteNullValue();

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out bool result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = false;
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
        if (strict) return base.TryConvert(strict, out result);
        result = decimal.Zero;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out float result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = 0f;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out double result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = 0d;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out short result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out uint result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out int result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out long result)
    {
        if (strict) return base.TryConvert(strict, out result);
        result = 0L;
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
        if (strict) return base.TryConvert(strict, out result);
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out string result)
    {
        result = null;
        return !strict;
    }

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <returns>An instance of the JSON node.</returns>
    public override System.Text.Json.Nodes.JsonNode ToJsonNode()
        => null;

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonNullNode leftValue, JsonNullNode rightValue)
        => true;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonNullNode leftValue, JsonNullNode rightValue)
        => false;
}
