using System;
using System.Collections.Generic;
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
    /// Represents a specific JSON string value.
    /// </summary>
    public struct JsonStringValue : IJsonValue
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
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind { get; private set; }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value != null ? ToJson(Value) : "null";
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
    /// Represents a specific JSON integer number value.
    /// </summary>
    public struct JsonIntegerValue : IJsonValue
    {
        /// <summary>
        /// Initializes a new instance of the JsonIntegerValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonIntegerValue(uint value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public long Value { get; }

        /// <summary>
        /// Initializes a new instance of the JsonIntegerValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonIntegerValue(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the JsonIntegerValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonIntegerValue(long value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the JsonIntegerValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonIntegerValue(DateTime value)
        {
            Value = Web.WebFormat.ParseDate(value);
        }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind => JsonValueKind.Number;

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator long(JsonIntegerValue json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator int(JsonIntegerValue json)
        {
            return (int)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator uint(JsonIntegerValue json)
        {
            return (uint)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator float(JsonIntegerValue json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator double(JsonIntegerValue json)
        {
            return json.Value;
        }
    }

    /// <summary>
    /// Represents a specific JSON float number value.
    /// </summary>
    public struct JsonFloatValue : IJsonValue
    {
        /// <summary>
        /// Initializes a new instance of the JsonFloatValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonFloatValue(float value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the JsonFloatValue class.
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonFloatValue(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind => JsonValueKind.Number;

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator double(JsonFloatValue json)
        {
            return json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator float(JsonFloatValue json)
        {
            return (float)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator uint(JsonFloatValue json)
        {
            return (uint)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator int(JsonFloatValue json)
        {
            return (int)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator ulong(JsonFloatValue json)
        {
            return (ulong)json.Value;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A number.</returns>
        public static explicit operator long(JsonFloatValue json)
        {
            return (long)json.Value;
        }
    }

    /// <summary>
    /// Represents a specific JSON boolean value.
    /// </summary>
    public struct JsonBooleanValue : IJsonValue
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
        /// <returns></returns>
        public override string ToString()
        {
            return Value ? TrueString : FalseString;
        }

        /// <summary>
        /// Converts the JSON raw back.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>A boolean.</returns>
        public static explicit operator bool(JsonBooleanValue json)
        {
            return json.Value;
        }
    }

    internal class JsonNull : IJsonValue
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
    }

    /// <summary>
    /// The extensions for class IJsonValue, JsonDocument, JsonElement, etc.
    /// </summary>
    public static class JsonValueExtensions
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
    }
}
