using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;

using Trivial.Reflection;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a specific string JSON value with source.
    /// </summary>
    public interface IJsonString : IJsonValue, IEquatable<IJsonValue<string>>, IEquatable<IJsonString>, IEquatable<string>
    {
        /// <summary>
        /// Gets the source value.
        /// </summary>
        public string StringValue { get; }

        /// <summary>
        /// Indicates whether the specified string is null or an empty string ("").
        /// </summary>
        /// <returns>true if the source value is null or an empty string (""); otherwise, false.</returns>
        public bool IsNullOrEmpty();
    }

    /// <summary>
    /// Represents a specific JSON string value.
    /// </summary>
    public class JsonString : IJsonString, IJsonValue<string>, IJsonValueResolver, IComparable<IJsonValue<string>>, IComparable<string>, IEquatable<IJsonValue<string>>, IEquatable<string>, IReadOnlyList<char>
    {
        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(string value)
        {
            Value = value;
            ValueKind = value != null ? JsonValueKind.String : JsonValueKind.Null;
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(DateTime value)
        {
            Value = ToJson(value, true);
            ValueKind = JsonValueKind.String;
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(DateTimeOffset value)
        {
            Value = value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
            ValueKind = JsonValueKind.String;
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(Guid value)
        {
            Value = value.ToString();
            ValueKind = JsonValueKind.String;
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(Uri value)
        {
            Value = value.OriginalString;
            ValueKind = JsonValueKind.String;
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(System.Security.SecureString value) : this(Security.SecureStringExtensions.ToUnsecureString(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(StringBuilder value) : this(value?.ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(char[] value) : this(value != null ? new string(value) : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(ReadOnlySpan<char> value) : this(value != null ? value.ToString() : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(TimeSpan value) : this(value.ToString("c"))
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(int value) : this(value.ToString("g", CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(long value) : this(value.ToString("g", CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(uint value) : this(value.ToString("g", CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(ulong value) : this(value.ToString("g", CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(double value) : this(value.ToString("g", CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(float value) : this(value.ToString("g", CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(decimal value) : this(value.ToString("g", CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonString class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonString(bool value) : this(value ? JsonBoolean.TrueString : JsonBoolean.FalseString)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string StringValue => Value;

        /// <summary>
        /// Gets the number of characters in the source value.
        /// </summary>
        public int Length => Value?.Length ?? 0;

        /// <summary>
        /// Gets the number of characters in the source value.
        /// </summary>
        public int Count => Value?.Length ?? 0;

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind { get; }

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
                if (s == null) throw new ArgumentOutOfRangeException("s is null", new InvalidOperationException("s is null"));
                return s[index];
            }
        }

#if !NETSTANDARD2_0
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
                if (s == null) throw new ArgumentOutOfRangeException("s is null", new InvalidOperationException("s is null"));
                return s[index.IsFromEnd ? s.Length - index.Value : index.Value];
            }
        }
#endif

        /// <summary>
        /// Indicates whether the specified string is null or an empty string ("").
        /// </summary>
        /// <returns>true if the source value is null or an empty string (""); otherwise, false.</returns>
        public bool IsNullOrEmpty()
        {
            return string.IsNullOrEmpty(Value);
        }

        /// <summary>
        /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <returns>true if the source value is null or System.String.Empty, or if value consists exclusively of white-space characters; otherwise, false.</returns>
        public bool IsNullOrWhiteSpace()
        {
            return string.IsNullOrWhiteSpace(Value);
        }

        /// <summary>
        /// Converts to a date time instance.
        /// </summary>
        /// <returns>A date time.</returns>
        public DateTime? TryGetDateTime()
        {
            return Web.WebFormat.ParseDate(Value);
        }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value != null ? ToJson(Value) : "null";
        }

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
        /// <typeparam name="T">The enum type to parse.</typeparam>
        /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
        /// <returns>An enum.</returns>
        /// <exception cref="InvalidOperationException">Value was null.</exception>
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
        /// <exception cref="InvalidOperationException">Value was null.</exception>
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
        /// <exception cref="InvalidOperationException">Value was null.</exception>
        public bool TryToEnum<T>(bool ignoreCase, out T result) where T : struct, Enum
        {
            if (Value != null && Enum.TryParse(Value, ignoreCase, out result))
                return true;

            result = default;
            return false;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonString other)
        {
            if (Value is null) return other is null || other.StringValue is null;
            return Value.Equals(other.StringValue);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<string> other)
        {
            if (Value is null) return other is null || other.Value is null;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(string other)
        {
            if (Value is null) return other is null;
            return Value.Equals(other);
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
        public override bool Equals(object other)
        {
            if (Value is null)
            {
                if (other is null) return true;
                if (other is IJsonValue<string> nJson) return nJson.Value is null;
                if (other is IJsonValue gJson) return gJson.ValueKind == JsonValueKind.Null || gJson.ValueKind == JsonValueKind.Undefined;
                return false;
            }

            if (other is null) return false;
            if (other is IJsonValue<string> sJson) return Value.Equals(sJson.Value);
            if (other is IJsonString strJson) return Value.Equals(strJson.StringValue);
            if (other is IJsonValue vJson) return ToString().Equals(vJson.ToString(), StringComparison.InvariantCulture);
            if (other is StringBuilder sb) return Value.Equals(sb.ToString());
            return Value.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            return Value == null ? (-1).GetHashCode() : Value.GetHashCode();
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
        public int CompareTo(IJsonValue<string> other)
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
        {
            return (Value ?? string.Empty).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the string.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the string.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Value ?? string.Empty).GetEnumerator();
        }

        /// <summary>
        /// Gets the value of the element as a boolean.
        /// </summary>
        /// <returns>The value of the element as a boolean.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        bool IJsonValueResolver.GetBoolean()
        {
            if (string.IsNullOrEmpty(Value)) return false;
            var v = Value.ToLower();
            return v switch
            {
                JsonBoolean.TrueString => true,
                JsonBoolean.FalseString => false,
                _ => throw new InvalidOperationException("Expect a boolean but it is a string.")
            };
        }

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
        /// Gets the value of the element as a date time.
        /// </summary>
        /// <returns>The value of the element as a date time.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="FormatException">The value is not formatted for a date time.</exception>
        public DateTime GetDateTime()
        {
            var date = Web.WebFormat.ParseDate(Value);
            if (date.HasValue) return date.Value;
            throw new FormatException("The string is not JSON date format.");
        }

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        decimal IJsonValueResolver.GetDecimal()
        {
            if (string.IsNullOrEmpty(Value)) return 0;
            if (decimal.TryParse(Value, out var num)) return num;
            throw new InvalidOperationException("Expect a number but it is a string.");
        }

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        float IJsonValueResolver.GetSingle()
        {
            if (string.IsNullOrEmpty(Value)) return 0;
            if (float.TryParse(Value, out var num)) return num;
            throw new InvalidOperationException("Expect a number but it is a string.");
        }


        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        double IJsonValueResolver.GetDouble()
        {
            if (string.IsNullOrEmpty(Value)) return 0;
            if (double.TryParse(Value, out var num)) return num;
            throw new InvalidOperationException("Expect a number but it is a string.");
        }


        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        short IJsonValueResolver.GetInt16()
        {
            if (string.IsNullOrEmpty(Value)) return 0;
            if (short.TryParse(Value, out var num)) return num;
            throw new InvalidOperationException("Expect a number but it is a string.");
        }


        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        uint IJsonValueResolver.GetUInt32()
        {
            if (string.IsNullOrEmpty(Value)) return 0;
            if (uint.TryParse(Value, out var num)) return num;
            throw new InvalidOperationException("Expect a number but it is a string.");
        }


        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        int IJsonValueResolver.GetInt32()
        {
            if (string.IsNullOrEmpty(Value)) return 0;
            if (int.TryParse(Value, out var num)) return num;
            throw new InvalidOperationException("Expect a number but it is a string.");
        }


        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        long IJsonValueResolver.GetInt64()
        {
            if (string.IsNullOrEmpty(Value)) return 0;
            if (long.TryParse(Value, out var num)) return num;
            throw new InvalidOperationException("Expect a number but it is a string.");
        }

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        string IJsonValueResolver.GetString() => Value;

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        /// <exception cref="FormatException">The value is not formatted for a Guid.</exception>
        Guid IJsonValueResolver.GetGuid()
        {
            if (string.IsNullOrEmpty(Value)) return Guid.Empty;
            return Guid.Parse(Value);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(string key) => throw new InvalidOperationException("Expect an object but it is a string.");

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(ReadOnlySpan<char> key) => throw new InvalidOperationException("Expect an object but it is a string.");

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is a string.");

#if !NETSTANDARD2_0
        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is a string.");
#endif

        /// <summary>
        /// Gets all property keys.
        /// </summary>
        /// <returns>The property keys.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
        IEnumerable<string> IJsonValueResolver.GetKeys() => throw new InvalidOperationException("Expect an object but it is a string.");

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonString(string value)
        {
            return new JsonString(value);
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonString(ReadOnlySpan<char> value)
        {
            return new JsonString(value);
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonString(char[] value)
        {
            return new JsonString(value);
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonString(StringBuilder value)
        {
            return new JsonString(value);
        }

        /// <summary>
        /// Converts to JSON value.
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <returns>A JSON value.</returns>
        public static implicit operator JsonString(Guid value)
        {
            return new JsonString(value);
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A string.</returns>
        public static explicit operator string(JsonString json)
        {
            return json?.Value;
        }

        /// <summary>
        /// Indicates whether the specified string is null or an empty string ("").
        /// </summary>
        /// <param name="value">A string JSON value to test</param>
        /// <returns>true if the source value is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(IJsonValue<string> value)
        {
            return string.IsNullOrEmpty(value?.Value);
        }

        /// <summary>
        /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="value">A string JSON value to test</param>
        /// <returns>true if the source value is null or System.String.Empty, or if value consists exclusively of white-space characters; otherwise, false.</returns>
        public static bool IsNullOrWhiteSpace(IJsonValue<string> value)
        {
            return string.IsNullOrWhiteSpace(value?.Value);
        }

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
        public static bool operator ==(JsonString leftValue, IJsonValue<string> rightValue)
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
        public static bool operator !=(JsonString leftValue, IJsonValue<string> rightValue)
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
        public static bool operator ==(JsonString leftValue, string rightValue)
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
        public static bool operator !=(JsonString leftValue, string rightValue)
        {
            return leftValue is null || leftValue.Value != rightValue;
        }
    }
}
