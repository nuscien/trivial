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
        /// Nullable Javascript ticks JSON string converter.
        /// </summary>
        public sealed class NullableStringConverter : JsonConverter<DateTime?>
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
        /// Javascript ticks JSON string converter.
        /// </summary>
        public sealed class StringConverter : JsonConverter<DateTime>
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
    public sealed class JsonUnixTimetampConverter : JsonConverter<DateTime>
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
        /// Nullable Unix timestamp JSON string converter.
        /// </summary>
        public sealed class NullableStringConverter : JsonConverter<DateTime?>
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
        /// Unix timestamp JSON string converter.
        /// </summary>
        public sealed class StringConverter : JsonConverter<DateTime>
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
}
