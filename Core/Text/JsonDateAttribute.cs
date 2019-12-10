﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
                if (reader.TokenType == JsonTokenType.Null || reader.TokenType == JsonTokenType.False)
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
                if (reader.TokenType == JsonTokenType.Null || reader.TokenType == JsonTokenType.False)
                    return null;
                if (reader.TokenType == JsonTokenType.String)
                    return WebFormat.ParseDate(reader.GetString());
                return WebFormat.ParseDate(reader.GetInt64());
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteStringValue(JsonString.ToJson(value.Value, true));
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
                writer.WriteStringValue(JsonString.ToJson(value, true));
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
                if (reader.TokenType == JsonTokenType.Null || reader.TokenType == JsonTokenType.False)
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
                if (reader.TokenType == JsonTokenType.Null || reader.TokenType == JsonTokenType.False)
                    return null;
                if (reader.TokenType == JsonTokenType.String)
                    return WebFormat.ParseDate(reader.GetString());
                return WebFormat.ParseUnixTimestamp(reader.GetInt64());
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteStringValue(JsonString.ToJson(value.Value, true));
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
                writer.WriteStringValue(JsonString.ToJson(value, true));
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
    /// Nullable Unix timestamp JSON number converter.
    /// </summary>
    internal sealed class JsonTimeSpanSecondConverter : JsonConverter<TimeSpan>
    {
        /// <summary>
        /// Nullable Unix timestamp JSON number converter.
        /// </summary>
        internal sealed class NullableConverter : JsonConverter<TimeSpan?>
        {
            /// <inheritdoc />
            public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null || reader.TokenType == JsonTokenType.False)
                    return null;
                if (reader.TokenType == JsonTokenType.Number)
                {
                    if (reader.TryGetInt64(out var integer))
                        return TimeSpan.FromSeconds(integer);
                    return TimeSpan.FromSeconds(reader.GetDouble());
                }

                var str = reader.GetString();
                if (string.IsNullOrWhiteSpace(str)) return null;
                if (long.TryParse(str, out var integerParsed))
                    return TimeSpan.FromSeconds(integerParsed);
                return TimeSpan.FromSeconds(double.Parse(str));
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue((long)value.Value.TotalSeconds);
                else writer.WriteNullValue();
            }
        }

        /// <inheritdoc />
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null || reader.TokenType == JsonTokenType.False)
                return TimeSpan.Zero;
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out var integer))
                    return TimeSpan.FromSeconds(integer);
                return TimeSpan.FromSeconds(reader.GetDouble());
            }

            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str)) return TimeSpan.Zero;
            if (long.TryParse(str, out var integerParsed))
                return TimeSpan.FromSeconds(integerParsed);
            return TimeSpan.FromSeconds(double.Parse(str));
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((long)value.TotalSeconds);
        }
    }
}
