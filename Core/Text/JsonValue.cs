using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a specific JSON value.
    /// </summary>
    public interface IJsonValue
    {
        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind { get; }
    }

    /// <summary>
    /// Represents a specific JSON value with source.
    /// </summary>
    /// <typeparam name="T">The type of source value.</typeparam>
    public interface IJsonValue<T> : IJsonValue, IEquatable<IJsonValue<T>>, IEquatable<T>
    {
        /// <summary>
        /// Gets the source value.
        /// </summary>
        public T Value { get; }
    }

    /// <summary>
    /// Represents a complex JSON value.
    /// </summary>
    public interface IJsonComplexValue : IJsonValue, ICloneable, IEnumerable
    {
        /// <summary>
        /// Gets the number of elements contained in the System.Collections.Generic.ICollection`1
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Deserializes.
        /// </summary>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public T Deserialize<T>(JsonSerializerOptions options = default);

        /// <summary>
        /// Writes this instance to the specified writer as a JSON value.
        /// </summary>
        /// <param name="writer">The writer to which to write this instance.</param>
        public void WriteTo(Utf8JsonWriter writer);

        /// <summary>
        /// Removes all items from the array.
        /// </summary>
        public void Clear();
    }

    /// <summary>
    /// Represents a specific JSON string value.
    /// </summary>
    public struct JsonStringValue : IJsonValue<string>, IComparable<IJsonValue<string>>, IComparable<string>, IReadOnlyList<char>
    {
        /// <summary>
        /// Initializes a new instance of the JsonStringValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonStringValue(string value)
        {
            Value = value;
            ValueKind = value != null ? JsonValueKind.String : JsonValueKind.Null;
        }

        /// <summary>
        /// Initializes a new instance of the JsonStringValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonStringValue(DateTime value)
        {
            Value = value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            ValueKind = JsonValueKind.String;
        }

        /// <summary>
        /// Initializes a new instance of the JsonStringValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonStringValue(Guid value)
        {
            Value = value.ToString();
            ValueKind = JsonValueKind.String;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the number of characters in the source value.
        /// </summary>
        public int Length => Value?.Length ?? 0;

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind { get; private set; }

        /// <summary>
        /// Gets the number of characters in the source value.
        /// </summary>
        public int Count => Value?.Length ?? 0;

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
                if (s == null) throw new ArgumentOutOfRangeException("s is null");
                return s[index];
            }
        }

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
                if (s == null) throw new ArgumentOutOfRangeException("s is null");
                return s[index.IsFromEnd ? s.Length - index.Value - 1 : index.Value];
            }
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
            return Value.CompareTo(other?.Value);
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
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the string.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the string.</returns>
        public IEnumerator<char> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the string.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the string.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A string.</returns>
        public static explicit operator string(JsonStringValue json)
        {
            return json.Value;
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
    }

    /// <summary>
    /// Represents a specific JSON boolean value.
    /// </summary>
    public class JsonBooleanValue : IJsonValue<bool>
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
        public readonly static JsonBooleanValue True = new JsonBooleanValue(true);

        /// <summary>
        /// Represents the Boolean value false of JSON as a string.
        /// This field is read-only.
        /// </summary>
        public readonly static JsonBooleanValue False = new JsonBooleanValue(false);

        /// <summary>
        /// Initializes a new instance of the JsonBooleanValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonBooleanValue(bool value)
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
        public JsonValueKind ValueKind { get; private set; }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns>The JSON format string of the boolean.</returns>
        public override string ToString()
        {
            return Value ? TrueString : FalseString;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue<bool> other)
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
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is IJsonValue<bool> bJson) return Value.Equals(bJson.Value);
            return Value.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A boolean.</returns>
        public static explicit operator bool(JsonBooleanValue json)
        {
            return json?.Value ?? false;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A boolean.</returns>
        public static explicit operator bool?(JsonBooleanValue json)
        {
            return json?.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator int(JsonBooleanValue json)
        {
            if (json is null) return 0;
            return json.Value ? 1 : 0;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator int?(JsonBooleanValue json)
        {
            if (json is null) return null;
            return json.Value ? 1 : 0;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A string.</returns>
        public static explicit operator string(JsonBooleanValue json)
        {
            if (json is null) return null;
            return json.Value ? TrueString : FalseString;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonBooleanValue leftValue, IJsonValue<bool> rightValue)
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
        public static bool operator !=(JsonBooleanValue leftValue, IJsonValue<bool> rightValue)
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
        public static bool operator ==(JsonBooleanValue leftValue, bool rightValue)
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
        public static bool operator !=(JsonBooleanValue leftValue, bool rightValue)
        {
            return leftValue is null || leftValue.Value != rightValue;
        }
    }

    internal class JsonNull : IJsonValue, IEquatable<JsonNull>
    {
        /// <summary>
        /// Initializes a new instance of the JsonNull class.
        /// </summary>
        /// <param name="valueKind">The JSON value kind.</param>
        public JsonNull(JsonValueKind valueKind)
        {
            ValueKind = valueKind;
        }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind { get; private set; }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns>The string null.</returns>
        public override string ToString()
        {
            return "null";
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(JsonNull other)
        {
            return true;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return true;
            if (other is IJsonValue json)
            {
                switch (ValueKind)
                {
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        return json.ValueKind == JsonValueKind.Null || json.ValueKind == JsonValueKind.Undefined;
                    case JsonValueKind.True:
                        return json.ValueKind == JsonValueKind.True;
                    case JsonValueKind.False:
                        return json.ValueKind == JsonValueKind.False;
                    default:
                        return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            return ValueKind.GetHashCode();
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonNull leftValue, JsonNull rightValue)
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
        public static bool operator !=(JsonNull leftValue, JsonNull rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (leftValue is null || rightValue is null) return false;
            return leftValue.Equals(rightValue);
        }
    }

    /// <summary>
    /// The extensions for class IJsonValue, JsonDocument, JsonElement, etc.
    /// </summary>
    public static class JsonValues
    {
        /// <summary>
        /// JSON null.
        /// </summary>
        public static readonly IJsonValue Null = new JsonNull(JsonValueKind.Null);

        /// <summary>
        /// JSON undefined.
        /// </summary>
        public static readonly IJsonValue Undefined = new JsonNull(JsonValueKind.Undefined);

        /// <summary>
        /// Converts from JSON document.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        public static IJsonValue ToJsonValue(JsonDocument json)
        {
            return ToJsonValue(json.RootElement);
        }

        /// <summary>
        /// Converts from JSON element.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        public static IJsonValue ToJsonValue(JsonElement json)
        {
            return json.ValueKind switch
            {
                JsonValueKind.Undefined => Undefined,
                JsonValueKind.Null => Null,
                JsonValueKind.String => new JsonStringValue(json.GetString()),
                JsonValueKind.Number => json.TryGetInt64(out var l)
                    ? new JsonIntegerValue(l)
                    : (json.TryGetDouble(out var d) ? new JsonFloatValue(d) : Null),
                JsonValueKind.True => new JsonBooleanValue(true),
                JsonValueKind.False => new JsonBooleanValue(false),
                JsonValueKind.Array => (JsonArray)json,
                JsonValueKind.Object => (JsonObject)json,
                _ => null
            };
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
                    value = Web.WebFormat.ParseDate(tick);
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
    }
}
