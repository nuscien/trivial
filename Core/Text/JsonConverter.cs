using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trivial.Text
{
    /// <summary>
    /// JSON string list and json array converter.
    /// </summary>
    public class JsonStringListConverter : JsonConverter<IEnumerable<string>>
    {
        private readonly char[] chars;
        private readonly bool trim;

        /// <summary>
        /// JSON string collection with white space separated.
        /// </summary>
        public sealed class WhiteSpaceSeparatedConverter : JsonStringListConverter
        {
            private static readonly char[] splitChars = new[] { ' ', '　', '\r', '\n', '\t' };

            /// <summary>
            /// Initializes a new instance of the WhiteSpaceSeparatedConverter class.
            /// </summary>
            public WhiteSpaceSeparatedConverter() : base(splitChars, false)
            {
            }
        }

        /// <summary>
        /// JSON string collection with comma separated.
        /// </summary>
        public sealed class CommaSeparatedConverter : JsonStringListConverter
        {
            /// <summary>
            /// Initializes a new instance of the CommaSeparatedConverter class.
            /// </summary>
            public CommaSeparatedConverter() : base(',')
            {
            }
        }

        /// <summary>
        /// JSON string collection with semicolon separated.
        /// </summary>
        public sealed class SemicolonSeparatedConverter : JsonStringListConverter
        {
            /// <summary>
            /// Initializes a new instance of the SemicolonSeparatedConverter class.
            /// </summary>
            public SemicolonSeparatedConverter() : base(';')
            {
            }
        }

        /// <summary>
        /// Initializes a new instance of the JsonStringListConverter class.
        /// </summary>
        public JsonStringListConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonStringListConverter class.
        /// </summary>
        /// <param name="split">The split characters.</param>
        /// <param name="needTrim">true if need trim each string item; otherwise, false. Default value is true.</param>
        public JsonStringListConverter(char split, bool needTrim = true)
        {
            chars = new[] { split };
            trim = needTrim;
        }

        /// <summary>
        /// Initializes a new instance of the JsonStringListConverter class.
        /// </summary>
        /// <param name="split">The split characters.</param>
        /// <param name="needTrim">true if need trim each string item; otherwise, false. Default value is true.</param>
        public JsonStringListConverter(char[] split, bool needTrim = true)
        {
            chars = split;
            trim = needTrim;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IEnumerable<string>).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override IEnumerable<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var col = new List<string>();
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (TryGetString(ref reader, out var str))
            {
                if (str != null)
                {
                    if (chars != null && chars.Length > 0)
                    {
                        IEnumerable<string> arr = str.Split(chars, StringSplitOptions.RemoveEmptyEntries);
                        if (trim) arr = arr.Select(ele => ele.Trim()).Where(ele => ele.Length > 0);
                        else col.AddRange(arr);
                    }
                    else
                    {
                        col.Add(str);
                    }
                }
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    while (reader.TokenType == JsonTokenType.Comment || reader.TokenType == JsonTokenType.None || reader.TokenType == JsonTokenType.Null)
                    {
                        reader.Read();
                    }

                    if (!TryGetString(ref reader, out var value))
                    {
                        throw new JsonException($"The token type is {reader.TokenType} but expect string or null.");
                    }

                    if (trim) value = value.Trim();
                    col.Add(value);
                }
            }

            return ToList(col, typeToConvert);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IEnumerable<string> value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            if (chars != null && chars.Length > 0)
            {
                var arr = value.Where(ele => !string.IsNullOrWhiteSpace(ele));
                if (trim) arr = arr.Select(ele => ele.Trim()).Where(ele => ele.Length > 0);
                var str = string.Join(chars[0], arr);
                writer.WriteStringValue(str);
                return;
            }

            writer.WriteStartArray();
            foreach (var item in value)
            {
                if (item == null) writer.WriteNullValue();
                else if (trim) writer.WriteStringValue(item.Trim());
                else writer.WriteStringValue(item);
            }

            writer.WriteEndArray();
        }

        private static IEnumerable<string> ToList(List<string> col, Type typeToConvert)
        {
            if (typeToConvert == typeof(List<string>) || typeToConvert.IsInterface)
            {
                return col;
            }

            if (typeToConvert == typeof(string[]))
            {
                return col.ToArray();
            }

            return (IEnumerable<string>)Activator.CreateInstance(typeToConvert, new[] { col });
        }

        private static bool TryGetString(ref Utf8JsonReader reader, out string result)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                case JsonTokenType.False:
                    result = null;
                    return true;
                case JsonTokenType.String:
                    result = reader.GetString();
                    return true;
                case JsonTokenType.Number:
                    result = reader.TryGetInt64(out var int64v)
                        ? int64v.ToString("g", CultureInfo.InvariantCulture)
                        : reader.GetDouble().ToString("g", CultureInfo.InvariantCulture);
                    return true;
                default:
                    result = null;
                    return false;
            };
        }
    }

    /// <summary>
    /// JSON object and JSON array converter.
    /// </summary>
    public sealed class JsonObjectConverter : JsonConverter<IJsonValue>
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IJsonValue).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override IJsonValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    if (typeToConvert == typeof(JsonObject) || typeToConvert == typeof(JsonArray)) return null;
                    if (typeToConvert == typeof(JsonString)) return new JsonString(null as string);
                    if (typeToConvert == typeof(IJsonValue)) return JsonValues.Null;
                    return null;
                case JsonTokenType.StartObject:
                    var obj = new JsonObject();
                    obj.SetRange(ref reader);
                    return obj;
                case JsonTokenType.StartArray:
                    return JsonArray.ParseValue(ref reader);
                case JsonTokenType.String:
                    return new JsonString(reader.GetString());
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var int64v)) return new JsonInteger(int64v);
                    return new JsonDouble(reader.GetDouble());
                case JsonTokenType.True:
                    return JsonBoolean.True;
                case JsonTokenType.False:
                    return JsonBoolean.False;
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IJsonValue value, JsonSerializerOptions options)
        {
            if (value is null) writer.WriteNullValue();
            else if (value is JsonObject jObj) jObj.WriteTo(writer);
            else if (value is JsonArray jArr) jArr.WriteTo(writer);
            else if (value is IJsonValue<string> jStr)
            {
                if (jStr.Value == null) writer.WriteNullValue();
                else writer.WriteStringValue(jStr.Value);
            }
            else if (value is IJsonValue<long> jInt64) writer.WriteNumberValue(jInt64.Value);
            else if (value is IJsonValue<double> jDouble) writer.WriteNumberValue(jDouble.Value);
            else if (value is IJsonValue<bool> jB) writer.WriteBooleanValue(jB.Value);
            else if (value is IJsonValue<int> jInt32) writer.WriteNumberValue(jInt32.Value);
            else if (value is IJsonValue<uint> jUInt32) writer.WriteNumberValue(jUInt32.Value);
            else if (value is IJsonValue<ulong> jUInt64) writer.WriteNumberValue(jUInt64.Value);
            else if (value is IJsonValue<float> jFloat) writer.WriteNumberValue(jFloat.Value);
            else if (value is IJsonValue<short> jInt16) writer.WriteNumberValue(jInt16.Value);
            else if (value is IJsonValue<ushort> jUInt16) writer.WriteNumberValue(jUInt16.Value);
            else if (value is IJsonValue<decimal> jDecimal) writer.WriteNumberValue(jDecimal.Value);
            else writer.WriteNullValue();
        }
    }
}
