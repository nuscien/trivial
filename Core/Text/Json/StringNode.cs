using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

using Trivial.Maths;
using Trivial.Reflection;
using Trivial.Web;

namespace Trivial.Text;

/// <summary>
/// Represents a specific JSON string value.
/// </summary>
[System.Text.Json.Serialization.JsonConverter(typeof(JsonObjectNodeConverter))]
public sealed class JsonStringNode : BaseJsonValueNode<string>, IComparable<IJsonValueNode<string>>, IComparable<string>, IEquatable<string>, IEquatable<StringBuilder>, IEquatable<char>, IReadOnlyList<char>
{
    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(string value)
        : base(value != null ? JsonValueKind.String : JsonValueKind.Null, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(IJsonValueNode<string> value)
        : this(value?.Value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(DateTime value)
        : base(JsonValueKind.String, ToJson(value, true))
    {
        ValueType = 1;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">A standard or custom date and time format string.</param>
    public JsonStringNode(DateTime value, string format)
        : base(JsonValueKind.String, format == null ? ToJson(value, true) : value.ToString(format))
    {
        ValueType = 1;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(DateTimeOffset value)
        : base(JsonValueKind.String, value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"))
    {
        ValueType = 1;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">A standard or custom date and time format string.</param>
    public JsonStringNode(DateTimeOffset value, string format)
        : base(JsonValueKind.String, value.ToString(format))
    {
        ValueType = 1;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(Guid value)
        : base(JsonValueKind.String, value.ToString())
    {
        ValueType = 4;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">A standard or custom GUID format string.</param>
    public JsonStringNode(Guid value, string format)
        : base(JsonValueKind.String, value.ToString(format))
    {
        ValueType = 4;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(Uri value)
        : base(JsonValueKind.String, value.OriginalString)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(System.Security.SecureString value)
        : this(Security.SecureStringExtensions.ToUnsecureString(value))
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(StringBuilder value)
        : this(value?.ToString())
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(char[] value)
        : this(value != null ? new string(value) : null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(ReadOnlySpan<char> value)
        : this(value?.ToString())
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(TimeSpan value)
        : this(value.ToString("c"))
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    public JsonStringNode(TimeSpan value, string format)
        : this(value.ToString(format))
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(short value)
        : this(value.ToString("g", CultureInfo.InvariantCulture))
    {
        ValueType = 2;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(int value)
        : this(value.ToString("g", CultureInfo.InvariantCulture))
    {
        ValueType = 2;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    public JsonStringNode(int value, string format, IFormatProvider provider = null)
        : this(provider == null ? value.ToString(format) : value.ToString(format, provider))
    {
        ValueType = 2;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(long value)
        : this(value.ToString("g", CultureInfo.InvariantCulture))
    {
        ValueType = 2;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    public JsonStringNode(long value, string format, IFormatProvider provider = null)
        : this(provider == null ? value.ToString(format) : value.ToString(format, provider))
    {
        ValueType = 2;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(uint value)
        : this(value.ToString("g", CultureInfo.InvariantCulture))
    {
        ValueType = 2;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(ulong value)
        : this(value.ToString("g", CultureInfo.InvariantCulture))
    {
        ValueType = 2;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(double value)
        : this(value.ToString("g", CultureInfo.InvariantCulture))
    {
        ValueType = 3;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    public JsonStringNode(double value, string format, IFormatProvider provider = null)
        : this(provider == null ? value.ToString(format) : value.ToString(format, provider))
    {
        ValueType = 3;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(float value)
        : this(value.ToString("g", CultureInfo.InvariantCulture))
    {
        ValueType = 3;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    public JsonStringNode(float value, string format, IFormatProvider provider = null)
        : this(provider == null ? value.ToString(format) : value.ToString(format, provider))
    {
        ValueType = 3;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(decimal value)
        : this(value.ToString("g", CultureInfo.InvariantCulture))
    {
        ValueType = 3;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    public JsonStringNode(decimal value, string format, IFormatProvider provider = null)
        : this(provider == null ? value.ToString(format) : value.ToString(format, provider))
    {
        ValueType = 3;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(BigInteger value)
        : this(value.ToString("g", CultureInfo.InvariantCulture))
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    public JsonStringNode(BigInteger value, string format, IFormatProvider provider = null)
        : this(provider == null ? value.ToString(format) : value.ToString(format, provider))
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(bool value)
        : this(value ? JsonBooleanNode.TrueString : JsonBooleanNode.FalseString)
    {
        ValueType = 5;
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(Net.QueryData value)
        : this(value?.ToString())
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(Net.HttpUri value)
        : this(value?.ToString())
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(Net.AppDeepLinkUri value)
        : this(value?.ToString())
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonString class.
    /// </summary>
    /// <param name="value">The value.</param>
    public JsonStringNode(Fraction value)
        : this(value.ToString())
    {
    }

    /// <summary>
    /// Gets the number of characters in the source value.
    /// </summary>
    public int Length => Value?.Length ?? 0;

    /// <summary>
    /// Gets or sets the internal type mark.
    /// <list>
    /// <item><term>0</term><description>Literal</description></item>
    /// <item><term>1</term><description>Date</description></item>
    /// <item><term>2</term><description>Integer</description></item>
    /// <item><term>3</term><description>Floating number</description></item>
    /// <item><term>4</term><description>Guid</description></item>
    /// <item><term>5</term><description>Boolean</description></item>
    /// </list>
    /// </summary>
    internal int ValueType { get; }

    /// <summary>
    /// Gets the System.Char object at a specified position in the source value.
    /// </summary>
    /// <param name="index">A position in the current string.</param>
    /// <returns>The character at position index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    public char this[int index]
    {
        get
        {
            var s = Value;
            return s == null ? throw new ArgumentOutOfRangeException("s is null", new InvalidOperationException("s is null")) : s[index];
        }
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Gets the System.Char object at a specified position in the source value.
    /// </summary>
    /// <param name="index">A position in the current string.</param>
    /// <returns>The character at position index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    public char this[Index index]
    {
        get
        {
            var s = Value;
            return s == null
                ? throw new ArgumentOutOfRangeException("s is null", new InvalidOperationException("s is null"))
                : s[index.IsFromEnd ? s.Length - index.Value : index.Value];
        }
    }

    /// <summary>
    /// Gets the sub-string in the source value.
    /// </summary>
    /// <param name="range">A range in the current string.</param>
    /// <returns>The sub-string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    public string this[Range range]
    {
        get
        {
            var s = Value;
            return s == null ? throw new ArgumentOutOfRangeException("s is null", new InvalidOperationException("s is null")) : s[range];
        }
    }
#endif

    /// <summary>
    /// Indicates whether the specified string is null or an empty string ("").
    /// </summary>
    /// <returns>true if the source value is null or an empty string (""); otherwise, false.</returns>
    public bool IsNullOrEmpty()
        => string.IsNullOrEmpty(Value);

    /// <summary>
    /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <returns>true if the source value is null or System.String.Empty, or if value consists exclusively of white-space characters; otherwise, false.</returns>
    public bool IsNullOrWhiteSpace()
        => string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Gets the JSON format string of the value.
    /// </summary>
    /// <returns>The JSON format string.</returns>
    public override string ToString()
        => Value != null ? ToJson(Value) : "null";

    /// <summary>
    /// Gets the JSON format string of the value.
    /// </summary>
    /// <param name="removeQuotes">true if remove the quotes; otherwise, false.</param>
    /// <returns>The JSON format string.</returns>
    public string ToString(bool removeQuotes)
        => Value != null ? ToJson(Value, removeQuotes) : (removeQuotes ? null : "null");

    /// <summary>
    /// Parses the value to an enum.
    /// </summary>
    /// <typeparam name="T">The enum type to parse.</typeparam>
    /// <returns>An enum.</returns>
    /// <exception cref="InvalidOperationException">Value was null.</exception>
    public T ToEnum<T>() where T : struct, Enum
    {
        if (Value == null) throw new InvalidOperationException("Value is null.");
        return ObjectConvert.ParseEnum<T>(Value);
    }

    /// <summary>
    /// Parses the value to an enum.
    /// </summary>
    /// <typeparam name="T">The enum type to parse.</typeparam>
    /// <returns>An enum.</returns>
    /// <exception cref="InvalidOperationException">Value was null.</exception>
    public T ToEnum<T>(bool ignoreCase) where T : struct, Enum
    {
        if (Value == null) throw new InvalidOperationException("Value is null.");
        return ObjectConvert.ParseEnum<T>(Value, ignoreCase);
    }

    /// <summary>
    /// Tries to parse the value to an enum.
    /// </summary>
    /// <typeparam name="T">The enum type to parse.</typeparam>
    /// <returns>An enum.</returns>
    /// <exception cref="InvalidOperationException">Value was null.</exception>
    public T? TryToEnum<T>() where T : struct, Enum
    {
        if (Value == null) return null;
        if (Enum.TryParse<T>(Value, out var result)) return result;
        return null;
    }

    /// <summary>
    /// Tries to parse the value to an enum.
    /// </summary>
    /// <param name="type">The type of enum.</param>
    /// <returns>An enum.</returns>
    public object TryToEnum(Type type)
    {
        if (string.IsNullOrWhiteSpace(Value)) return null;
        if (type == null || !type.IsEnum) return null;
        try
        {
#if NETFRAMEWORK
            return Enum.Parse(type, Value);
#else
            if (Enum.TryParse(type, Value, out var r)) return r;
#endif
        }
        catch (ArgumentException)
        {
        }
        catch (OverflowException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        return null;
    }

    /// <summary>
    /// Tries to parse the value to an enum.
    /// </summary>
    /// <param name="type">The type of enum.</param>
    /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
    /// <returns>An enum.</returns>
    public object TryToEnum(Type type, bool ignoreCase)
    {
        if (string.IsNullOrWhiteSpace(Value)) return null;
        if (type == null || !type.IsEnum) return null;
        try
        {
#if NETFRAMEWORK
            return Enum.Parse(type, Value, ignoreCase);
#else
            if (Enum.TryParse(type, Value, ignoreCase, out var r)) return r;
#endif
        }
        catch (ArgumentException)
        {
        }
        catch (OverflowException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        return null;
    }

    /// <summary>
    /// Tries to parse the value to an enum.
    /// </summary>
    /// <typeparam name="T">The enum type to parse.</typeparam>
    /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
    /// <returns>An enum.</returns>
    public T? TryToEnum<T>(bool ignoreCase) where T : struct, Enum
    {
        if (Value == null) return null;
        if (Enum.TryParse<T>(Value, ignoreCase, out var result)) return result;
        return null;
    }

    /// <summary>
    /// Tries to parse the value to an enum.
    /// </summary>
    /// <typeparam name="T">The enum type to parse.</typeparam>
    /// <param name="result">The result output.</param>
    /// <returns>true if parse succeeded; otherwise, false.</returns>
    public bool TryToEnum<T>(out T result) where T : struct, Enum
    {
        if (Value != null && Enum.TryParse(Value, out result))
            return true;

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to parse the value to an enum.
    /// </summary>
    /// <typeparam name="T">The enum type to parse.</typeparam>
    /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
    /// <param name="result">The result output.</param>
    /// <returns>true if parse succeeded; otherwise, false.</returns>
    public bool TryToEnum<T>(bool ignoreCase, out T result) where T : struct, Enum
    {
        if (Value != null && Enum.TryParse(Value, ignoreCase, out result))
            return true;

        result = default;
        return false;
    }

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified string in this instance. The search starts at a specified character position.
    /// </summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <returns>The zero-based index position of value from the start of the current instance if that string is found, or -1 if it is not.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public int IndexOf(string value, int start = 0)
        => Value?.IndexOf(value, start) ?? (value == null ? string.Empty.IndexOf(value) : -1);

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified string in this instance. The search starts at a specified character position.
    /// </summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>count or startIndex is negative. -or- startIndex is greater than the length of this string. -or- count is greater than the length of this string minus start index.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public int IndexOf(string value, int start, int count)
        => Value?.IndexOf(value, start, count) ?? (value == null ? string.Empty.IndexOf(value) : -1);

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified character in this instance. The search starts at a specified character position.
    /// </summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <returns>The zero-based index position of value from the start of the current instance if that string is found, or -1 if it is not.</returns>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public int IndexOf(char value, int start = 0)
        => Value?.IndexOf(value, start) ?? -1;

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified character in this instance. The search starts at a specified character position.
    /// </summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>count or startIndex is negative. -or- startIndex is greater than the length of this string. -or- count is greater than the length of this string minus start index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public int IndexOf(char value, int start, int count)
        => Value?.IndexOf(value, start, count) ?? -1;

    /// <summary>
    /// Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.
    /// </summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <returns>The zero-based index position of value from the start of the current instance if that string is found, or -1 if it is not.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public int LastIndexOf(string value, int start = 0)
        => Value?.LastIndexOf(value, start) ?? (value == null ? string.Empty.LastIndexOf(value) : -1);

    /// <summary>
    /// Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.
    /// </summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>count or startIndex is negative. -or- startIndex is greater than the length of this string. -or- count is greater than the length of this string minus start index.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public int LastIndexOf(string value, int start, int count)
        => Value?.LastIndexOf(value, start, count) ?? (value == null ? string.Empty.LastIndexOf(value) : -1);

    /// <summary>
    /// Reports the zero-based index position of the last occurrence of a specified character within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.
    /// </summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <returns>The zero-based index position of value from the start of the current instance if that string is found, or -1 if it is not.</returns>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public int LastIndexOf(char value, int start = 0)
        => Value?.LastIndexOf(value, start) ?? -1;

    /// <summary>
    /// Reports the zero-based index position of the last occurrence of a specified character within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.
    /// </summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>count or startIndex is negative. -or- startIndex is greater than the length of this string. -or- count is greater than the length of this string minus start index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public int LastIndexOf(char value, int start, int count)
        => Value?.LastIndexOf(value, start, count) ?? -1;

    /// <summary>
    /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
    /// </summary>
    /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
    /// <param name="length">The number of characters in the substring.</param>
    /// <returns>A JSON string that is equivalent to the substring of length length that begins at startIndex in this instance, or instance with empty string value if startIndex is equal to the length of this instance and length is zero.</returns>
    public JsonStringNode Substring(int startIndex, int? length = null)
    {
        if (Value == null) return this;
        var str = length.HasValue ? Value.Substring(startIndex, length.Value) : Value.Substring(startIndex);
        if (str != null && str.Length == Length) return this;
        return new(str);
    }

    /// <summary>
    /// Converts to a JSON array.
    /// </summary>
    /// <param name="separator">A character array that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <param name="trimEachItem">true if trim each item; otherwise, false.</param>
    /// <returns>A JSON array node.</returns>
    public JsonArrayNode ToJsonArray(char[] separator, StringSplitOptions options = StringSplitOptions.None, bool trimEachItem = false)
    {
        if (Value == null) return null;
        var arr = new JsonArrayNode();
        if (!trimEachItem)
        {
            arr.AddRange(Value.Split(separator, options));
            return arr;
        }

        var col = StringExtensions.YieldSplit(Value, separator);
        col = StringExtensions.TrimForEach(col, options == StringSplitOptions.RemoveEmptyEntries);
        arr.AddRange(col);
        return arr;
    }

    /// <summary>
    /// Converts to a JSON array.
    /// </summary>
    /// <param name="separator">A character array that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <param name="trimEachItem">true if trim each item; otherwise, false.</param>
    /// <returns>A JSON array node.</returns>
    public JsonArrayNode ToJsonArray(char separator, StringSplitOptions options = StringSplitOptions.None, bool trimEachItem = false)
        => ToJsonArray([separator], options, trimEachItem);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(IJsonValueNode other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return ValueKind == JsonValueKind.Null || ValueKind == JsonValueKind.Undefined;
        if (other is JsonStringNode n) return n.Value == Value;
        if (other is JsonArrayNode arr)
        {
            switch (arr.Count)
            {
                case 0:
                    return string.IsNullOrEmpty(Value);
                case 1:
                    var first = arr.FirstOrDefault();
                    if (first is JsonArrayNode arr2 && arr2.Count < 2) first = arr2.FirstOrDefault();
                    if (first.ValueKind == JsonValueKind.Array) return false;
                    return Equals(first);
            }
        }

        if (other.ValueKind == JsonValueKind.Object || other.ValueKind == JsonValueKind.Array) return false;
        if (!other.TryConvert(false, out string s)) return false;
        return s == Value;
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(IJsonValueNode other, StringComparison comparisonType)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return ValueKind == JsonValueKind.Null || ValueKind == JsonValueKind.Undefined;
        if (other is JsonStringNode n) return string.Equals(n.Value, Value, comparisonType);
        if (other is JsonArrayNode arr)
        {
            switch (arr.Count)
            {
                case 0:
                    return string.IsNullOrEmpty(Value);
                case 1:
                    var first = arr.FirstOrDefault();
                    if (first is JsonArrayNode arr2 && arr2.Count < 2) first = arr2.FirstOrDefault();
                    if (first.ValueKind == JsonValueKind.Array) return false;
                    return Equals(first, comparisonType);
            }
        }

        if (other.ValueKind == JsonValueKind.Object || other.ValueKind == JsonValueKind.Array) return false;
        if (!other.TryConvert(false, out string s)) return false;
        return string.Equals(s, Value, comparisonType);
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(string other, StringComparison comparisonType)
    {
        if (Value is null) return other is null;
        return Value.Equals(other, comparisonType);
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(StringBuilder other)
    {
        if (Value is null) return other is null;
        return Value.Equals(other.ToString());
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(StringBuilder other, StringComparison comparisonType)
    {
        if (Value is null) return other is null;
        return Value.Equals(other.ToString(), comparisonType);
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(ReadOnlySpan<char> other)
    {
        if (Value is null) return false;
        return Value.Equals(other.ToString());
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(char other)
    {
        if (Value is null || Value.Length != 1) return false;
        return Value[0].Equals(other);
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
    public int CompareTo(IJsonValueNode<string> other)
    {
        if (Value == null && other?.Value == null) return 0;
        return Value == null ? -other.Value.CompareTo(Value) : Value.CompareTo(other?.Value);
    }

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
    public int CompareTo(string other)
    {
        if (Value == null && other == null) return 0;
        return Value == null ? -other.CompareTo(Value) : Value.CompareTo(other);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the string.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the string.</returns>
    public IEnumerator<char> GetEnumerator()
        => (Value ?? string.Empty).GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the string.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the string.</returns>
    IEnumerator IEnumerable.GetEnumerator()
        => (Value ?? string.Empty).GetEnumerator();

    /// <summary>
    /// Gets the value of the element as a byte array.
    /// </summary>
    /// <returns>The value decoded as a byte array.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="FormatException">The value is not encoded as Base64 text and hence cannot be decoded to bytes.</exception>
    public byte[] GetBytesFromBase64()
    {
        if (string.IsNullOrEmpty(Value)) return Array.Empty<byte>();
        return Convert.FromBase64String(Value);
    }

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <returns>The result.</returns>
    public bool? TryToBoolean()
        => TryConvert(false, out bool result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <returns>The result.</returns>
    public bool TryToBoolean(bool defaultValue)
        => TryConvert(false, out bool result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out bool result)
    {
        if (strict || string.IsNullOrEmpty(Value)) return base.TryConvert(strict, out result);
        var b = JsonBooleanNode.TryParse(Value);
        if (b == null) return base.TryConvert(strict, out result);
        result = b.Value;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <returns>The result.</returns>
    public DateTime? TryToDateTime()
        => TryConvert(out DateTime result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(out DateTime result)
    {
        var date = TryGetDateTimeOrNull();
        if (date.HasValue)
        {
            result = date.Value;
            return true;
        }

        result = WebFormat.ZeroTick;
        return false;
    }

    /// <summary>
    /// Converts to a date time instance.
    /// </summary>
    /// <returns>A date time.</returns>
    private DateTime? TryGetDateTimeOrNull()
    {
        var s = Value;
        if (s == null) return null;
        switch (ValueType)
        {
            case 2:
                if (long.TryParse(s, out var l)) return Web.WebFormat.ParseDate(l);
                break;
            default:
                break;
        }

        return WebFormat.ParseDate(s);
    }

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <returns>The result.</returns>
    public decimal? TryToDecimal()
        => TryConvert(false, out decimal result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <returns>The result.</returns>
    public decimal TryToDecimal(decimal defaultValue)
        => TryConvert(false, out decimal result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out decimal result)
    {
        if (strict) return base.TryConvert(strict, out result);
        if (!string.IsNullOrEmpty(Value)) return decimal.TryParse(Value, out result);
        result = decimal.Zero;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <returns>The result.</returns>
    public float? TryToSingle()
        => TryConvert(false, out float result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <returns>The result.</returns>
    public float TryToSingle(float defaultValue)
        => TryConvert(false, out float result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <returns>The result.</returns>
    public float TryToSingle(bool zeroAsDefault)
        => TryConvert(false, out float result) ? result : (zeroAsDefault ? 0f : float.NaN);

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out float result)
    {
        if (strict) return base.TryConvert(strict, out result);
        if (!string.IsNullOrEmpty(Value)) return float.TryParse(Value, out result);
        result = float.NaN;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <returns>The result.</returns>
    public double? TryToDouble()
        => TryConvert(false, out double result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <returns>The result.</returns>
    public double TryToDouble(double defaultValue)
        => TryConvert(false, out double result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <returns>The result.</returns>
    public double TryToDouble(bool zeroAsDefault)
        => TryConvert(false, out double result) ? result : (zeroAsDefault ? 0d : double.NaN);

    /// <summary>
    /// Tries to get the value of the element as a floating number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out double result)
    {
        if (strict) return base.TryConvert(strict, out result);
        if (!string.IsNullOrEmpty(Value)) return double.TryParse(Value, out result);
        if (ValueType == 1)
        {
            var date = TryGetDateTimeOrNull();
            if (date.HasValue)
            {
                result = WebFormat.ParseDate(date.Value);
                return true;
            }
        }

        result = double.NaN;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result.</returns>
    public short? TryToInt16()
        => TryConvert(false, out short result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result.</returns>
    public short TryToInt16(short defaultValue)
        => TryConvert(false, out short result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out short result)
    {
        if (strict) return base.TryConvert(strict, out result);
        if (!string.IsNullOrEmpty(Value)) return Numbers.TryParseToInt16(Value, 10, out result);
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result.</returns>
    public uint? TryToUInt32()
        => TryConvert(false, out uint result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result.</returns>
    public uint TryToUInt32(uint defaultValue)
        => TryConvert(false, out uint result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out uint result)
    {
        if (strict) return base.TryConvert(strict, out result);
        if (!string.IsNullOrEmpty(Value)) return Numbers.TryParseToUInt32(Value, 10, out result);
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result.</returns>
    public int? TryToInt32()
        => TryConvert(false, out int result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result.</returns>
    public int TryToInt32(int defaultValue)
        => TryConvert(false, out int result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out int result)
    {
        if (strict) return base.TryConvert(strict, out result);
        if (!string.IsNullOrEmpty(Value)) return Numbers.TryParseToInt32(Value, 10, out result);
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result.</returns>
    public long? TryToInt64()
        => TryConvert(false, out long result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result.</returns>
    public long TryToInt64(long defaultValue)
        => TryConvert(false, out long result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out long result)
    {
        if (strict) return base.TryConvert(strict, out result);
        if (!string.IsNullOrEmpty(Value)) return Numbers.TryParseToInt64(Value, 10, out result);
        result = 0L;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result.</returns>
    public ulong? TryToUInt64()
        => TryConvert(false, out ulong result) ? result : null;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <returns>The result.</returns>
    public ulong TryToUInt64(ulong defaultValue)
        => TryConvert(false, out ulong result) ? result : defaultValue;

    /// <summary>
    /// Tries to get the value of the element as an integer.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out ulong result)
    {
        if (strict) return base.TryConvert(strict, out result);
        if (!string.IsNullOrEmpty(Value)) return ulong.TryParse(Value, out result);
        result = 0;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a string.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override bool TryConvert(bool strict, out string result)
    {
        result = Value;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the element as a GUID.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    public bool TryConvert(out Guid result)
        => ((IJsonValueNode)this).TryConvert(out result);

    /// <summary>
    /// Tries to get the value of the element as a URI.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    public bool TryConvert(out Uri result)
        => TryConvert(UriKind.RelativeOrAbsolute, out result);

    /// <summary>
    /// Tries to get the value of the element as a URI.
    /// </summary>
    /// <param name="kind">The type of the URI.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    public bool TryConvert(UriKind kind, out Uri result)
    {
        if (string.IsNullOrEmpty(Value))
        {
            result = null;
            return false;
        }

        return Uri.TryCreate(Value, kind, out result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override BaseJsonValueNode TryGetValueOrNull(string key)
    {
        if (key == null) return null;
        var s = Value?.Trim();
        if (string.IsNullOrEmpty(s)) return null;
        BaseJsonValueNode result;
        if (s.StartsWith("{") && s.EndsWith("}"))
        {
            var json = JsonObjectNode.TryParse(s);
            if (json is null) return null;
            return json.TryGetValue(key, out result) ? result : null;
        }

        if (s.StartsWith("[") && s.EndsWith("]"))
        {
            var jArr = JsonArrayNode.TryParse(s);
            if (jArr is null) return null;
            return jArr.TryGetValue(key, out result) ? result : null;
        }

        switch (key.ToLowerInvariant())
        {
            case "len":
            case "length":
                return new JsonIntegerNode(Length);
            case "*":
            case "all":
            case "":
                return this;
        }

        if (int.TryParse(key.Trim(), out var i) && i >= 0 && i < Length)
        {
            try
            {
                return new JsonStringNode(Value[i].ToString());
            }
            catch (ArgumentException)
            {
            }
        }

        return null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The value of property.</param>
    /// <returns>true if get succeeded; otherwise, false..</returns>
    public bool TryGetValue(string key, out BaseJsonValueNode result)
    {
        result = TryGetValueOrNull(key);
        return result != null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected override BaseJsonValueNode TryGetValueOrNull(int index)
    {
        if (index < 0) return null;
        var s = Value?.Trim();
        if (string.IsNullOrEmpty(s)) return null;
        BaseJsonValueNode result;
        if (Value.StartsWith("[") && Value.EndsWith("]"))
        {
            try
            {
                var arr = JsonArrayNode.Parse(Value);
                return arr.TryGetValue(index, out result) ? result : null;
            }
            catch (ArgumentException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (JsonException)
            {
            }
            catch (FormatException)
            {
            }
            catch (InvalidCastException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (AggregateException)
            {
            }
        }

        if (index >= 0 && index < Length)
        {
            try
            {
                return new JsonStringNode(Value[index].ToString());
            }
            catch (ArgumentException)
            {
            }
        }

        return null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The value of property.</param>
    /// <returns>true if get succeeded; otherwise, false..</returns>
    public bool TryGetValue(int index, out BaseJsonValueNode result)
    {
        result = TryGetValueOrNull(index);
        return result != null;
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    public BaseJsonValueNode TryGetValue(Index index)
    {
        if (!string.IsNullOrWhiteSpace(Value) && Value.StartsWith("[") && Value.EndsWith("]"))
        {
            try
            {
                var arr = JsonArrayNode.Parse(Value);
                return arr.TryGetValue(index, out var result) ? result : null;
            }
            catch (ArgumentException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (JsonException)
            {
            }
            catch (FormatException)
            {
            }
            catch (InvalidCastException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (AggregateException)
            {
            }
        }

        try
        {
            return new JsonStringNode(Value[index].ToString());
        }
        catch (ArgumentException)
        {
        }

        return null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The value of property.</param>
    /// <returns>true if get succeeded; otherwise, false..</returns>
    public bool TryGetValue(Index index, out BaseJsonValueNode result)
    {
        result = TryGetValue(index);
        return result != null;
    }
#endif

    /// <inheritdoc />
    public override System.Text.Json.Nodes.JsonValue ToJsonValue()
        => System.Text.Json.Nodes.JsonValue.Create(Value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonStringNode(string value)
    {
        return new JsonStringNode(value);
    }

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonStringNode(ReadOnlySpan<char> value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonStringNode(char[] value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonStringNode(StringBuilder value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonStringNode(Guid value)
        => new(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonStringNode(System.Text.Json.Nodes.JsonValue value)
    {
        if (value is null) return null;
        if (!value.TryGetValue(out string s)) s = value.ToString();
        return new JsonStringNode(s);
    }

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator JsonStringNode(System.Text.Json.Nodes.JsonNode value)
    {
        if (value is null) return null;
        if (value is System.Text.Json.Nodes.JsonValue v) return v;
        throw new InvalidCastException($"Only supports JsonValue but its type is {value.GetType().Name}.");
    }

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator string(JsonStringNode json)
        => json?.Value;

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonNode class.</returns>
    public static explicit operator System.Text.Json.Nodes.JsonNode(JsonStringNode json)
        => System.Text.Json.Nodes.JsonValue.Create(json.Value);

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonNode class.</returns>
    public static explicit operator System.Text.Json.Nodes.JsonValue(JsonStringNode json)
        => System.Text.Json.Nodes.JsonValue.Create(json.Value);

    /// <summary>
    /// Indicates whether the specified string is null or an empty string ("").
    /// </summary>
    /// <param name="value">A string JSON value to test</param>
    /// <returns>true if the source value is null or an empty string (""); otherwise, false.</returns>
    public static bool IsNullOrEmpty(IJsonValueNode<string> value)
        => string.IsNullOrEmpty(value?.Value);

    /// <summary>
    /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">A string JSON value to test</param>
    /// <returns>true if the source value is null or System.String.Empty, or if value consists exclusively of white-space characters; otherwise, false.</returns>
    public static bool IsNullOrWhiteSpace(IJsonValueNode<string> value)
        => string.IsNullOrWhiteSpace(value?.Value);

    /// <summary>
    /// Converts a string to JSON format.
    /// </summary>
    /// <param name="s">The value.</param>
    /// <param name="removeQuotes">true if remove the quotes; otherwise, false.</param>
    /// <returns>A JSON format string.</returns>
    public static string ToJson(string s, bool removeQuotes = false)
    {
        if (s == null) return removeQuotes ? null : "null";
        s = s
            .Replace("\\", "\\\\")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("\a", "\\n")
            .Replace("\b", "\\t")
            .Replace("\f", "\\f")
            .Replace("\v", "\\v")
            .Replace("\0", "\\0")
            .Replace("\"", "\\\"");
        if (!removeQuotes) s = string.Format("\"{0}\"", s);
        return s;
    }

    /// <summary>
    /// Converts a string to JSON format.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="removeQuotes">true if remove the quotes; otherwise, false.</param>
    /// <returns>A JSON format string.</returns>
    public static string ToJson(DateTime value, bool removeQuotes = false)
    {
        var s = value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        if (!removeQuotes) s = string.Format("\"{0}\"", s);
        return s;
    }

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonStringNode leftValue, IJsonValueNode<string> rightValue)
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
    public static bool operator !=(JsonStringNode leftValue, IJsonValueNode<string> rightValue)
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
    public static bool operator ==(JsonStringNode leftValue, string rightValue)
        => leftValue is not null && leftValue.Value == rightValue;

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(JsonStringNode leftValue, string rightValue)
        => leftValue is null || leftValue.Value != rightValue;
}
