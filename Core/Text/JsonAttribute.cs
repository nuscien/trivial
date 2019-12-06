using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Trivial.Web;

namespace Trivial.Text
{
    /// <summary>
    /// Javascript ticks JSON number converter.
    /// </summary>
    public sealed class JsonJavaScriptTicksConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// Nullable Javascript ticks JSON number converter.
        /// </summary>
        public sealed class NullableConverter : JsonConverter<DateTime?>
        {
            /// <inheritdoc />
            public override bool CanConvert(Type typeToConvert)
            {
                return base.CanConvert(typeToConvert) || typeToConvert == typeof(DateTime);
            }

            /// <inheritdoc />
            public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;
                if (reader.TokenType == JsonTokenType.Number)
                    return WebFormat.ParseDate(reader.GetInt64());
                return WebFormat.ParseDate(reader.GetString());
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            {
                var num = WebFormat.ParseDate(value);
                if (num.HasValue) writer.WriteNumberValue(num.Value);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Nullable date time JSON converter with Javascript ticks fallback.
        /// </summary>
        public sealed class FallbackNullableConverter : JsonConverter<DateTime?>
        {
            /// <inheritdoc />
            public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;
                if (reader.TokenType == JsonTokenType.String)
                    return WebFormat.ParseDate(reader.GetString());
                return WebFormat.ParseDate(reader.GetInt64());
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteStringValue(JsonStringValue.ToJson(value.Value, true));
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Date time JSON converter with Javascript ticks fallback.
        /// </summary>
        public sealed class FallbackConverter : JsonConverter<DateTime>
        {
            /// <inheritdoc />
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                    return WebFormat.ParseDate(reader.GetString()) ?? default;
                return WebFormat.ParseDate(reader.GetInt64());
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(JsonStringValue.ToJson(value, true));
            }
        }

        /// <inheritdoc />
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return WebFormat.ParseDate(reader.GetInt64());
            var v = WebFormat.ParseDate(reader.GetString());
            return v ?? default;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var num = WebFormat.ParseDate(value);
            writer.WriteNumberValue(num);
        }
    }

    /// <summary>
    /// Unix timestamp JSON number converter.
    /// </summary>
    public sealed class JsonUnixTimestampConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// Nullable Unix timestamp JSON number converter.
        /// </summary>
        public sealed class NullableConverter : JsonConverter<DateTime?>
        {
            /// <inheritdoc />
            public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;
                if (reader.TokenType == JsonTokenType.Number)
                    return WebFormat.ParseUnixTimestamp(reader.GetInt64());
                return WebFormat.ParseDate(reader.GetString());
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            {
                var num = WebFormat.ParseUnixTimestamp(value);
                if (num.HasValue) writer.WriteNumberValue(num.Value);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Nullable date time JSON converter with Unix timestamp fallback.
        /// </summary>
        public sealed class FallbackNullableConverter : JsonConverter<DateTime?>
        {
            /// <inheritdoc />
            public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;
                if (reader.TokenType == JsonTokenType.String)
                    return WebFormat.ParseDate(reader.GetString());
                return WebFormat.ParseUnixTimestamp(reader.GetInt64());
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteStringValue(JsonStringValue.ToJson(value.Value, true));
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Date time JSON converter with Unix timestamp fallback.
        /// </summary>
        public sealed class FallbackConverter : JsonConverter<DateTime>
        {
            /// <inheritdoc />
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                    return WebFormat.ParseDate(reader.GetString()) ?? default;
                return WebFormat.ParseUnixTimestamp(reader.GetInt64());
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(JsonStringValue.ToJson(value, true));
            }
        }

        /// <inheritdoc />
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return WebFormat.ParseUnixTimestamp(reader.GetInt64());
            var v = WebFormat.ParseDate(reader.GetString());
            return v ?? default;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var num = WebFormat.ParseUnixTimestamp(value);
            writer.WriteNumberValue(num);
        }
    }

    /// <summary>
    /// Nullable Unix timestamp JSON string converter.
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
                    return null;
                case JsonTokenType.StartObject:
                    var obj = new JsonObject();
                    obj.SetRange(ref reader);
                    return obj;
                case JsonTokenType.StartArray:
                    return JsonArray.ParseValue(ref reader);
                case JsonTokenType.String:
                    return new JsonStringValue(reader.GetString());
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var int64v)) return new JsonIntegerValue(int64v);
                    return new JsonFloatValue(reader.GetDouble());
                case JsonTokenType.True:
                    return JsonBooleanValue.True;
                case JsonTokenType.False:
                    return JsonBooleanValue.False;
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
            else writer.WriteNullValue();
        }
    }
}
