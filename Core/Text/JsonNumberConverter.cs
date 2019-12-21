using System;
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
    /// Json number converter with number string fallback.
    /// </summary>
    public sealed class JsonNumberConverter : JsonConverterFactory
    {
        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int16Converter : JsonConverter<short>
        {
            /// <inheritdoc />
            public override short Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => 0,
                    JsonTokenType.Number => reader.TryGetInt16(out var integer) ? integer : (short)reader.GetDouble(),
                    JsonTokenType.String => ParseNumber(ref reader, short.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, short value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int32Converter : JsonConverter<int>
        {
            /// <inheritdoc />
            public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => 0,
                    JsonTokenType.Number => reader.TryGetInt32(out var integer) ? integer : (int)reader.GetDouble(),
                    JsonTokenType.String => ParseNumber(ref reader, int.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int64Converter : JsonConverter<long>
        {
            /// <inheritdoc />
            public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => 0,
                    JsonTokenType.Number => reader.TryGetInt64(out var integer) ? integer : (long)reader.GetDouble(),
                    JsonTokenType.String => ParseNumber(ref reader, long.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }
        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt16Converter : JsonConverter<ushort>
        {
            /// <inheritdoc />
            public override ushort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => 0,
                    JsonTokenType.Number => reader.TryGetUInt16(out var integer) ? integer : (ushort)reader.GetDouble(),
                    JsonTokenType.String => ParseNumber(ref reader, ushort.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, ushort value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt32Converter : JsonConverter<uint>
        {
            /// <inheritdoc />
            public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => 0,
                    JsonTokenType.Number => reader.TryGetUInt32(out var integer) ? integer : (uint)reader.GetDouble(),
                    JsonTokenType.String => ParseNumber(ref reader, uint.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt64Converter : JsonConverter<ulong>
        {
            /// <inheritdoc />
            public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => 0,
                    JsonTokenType.Number => reader.TryGetUInt64(out var integer) ? integer : (ulong)reader.GetDouble(),
                    JsonTokenType.String => ParseNumber(ref reader, ulong.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class DecimalConverter : JsonConverter<decimal>
        {
            /// <inheritdoc />
            public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => 0,
                    JsonTokenType.Number => reader.GetDecimal(),
                    JsonTokenType.String => ParseNumber(ref reader, decimal.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class SingleConverter : JsonConverter<float>
        {
            /// <inheritdoc />
            public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => 0,
                    JsonTokenType.Number => reader.GetSingle(),
                    JsonTokenType.String => ParseNumber(ref reader, float.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class DoubleConverter : JsonConverter<double>
        {
            /// <inheritdoc />
            public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => 0,
                    JsonTokenType.Number => reader.GetDouble(),
                    JsonTokenType.String => ParseNumber(ref reader, double.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int16NullableConverter : JsonConverter<short?>
        {
            /// <inheritdoc />
            public override short? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.TryGetInt16(out var integer) ? integer : (short)reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, short.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, short? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue(value.Value);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int32NullableConverter : JsonConverter<int?>
        {
            /// <inheritdoc />
            public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.TryGetInt32(out var integer) ? integer : (int)reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, int.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue(value.Value);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int64NullableConverter : JsonConverter<long?>
        {
            /// <inheritdoc />
            public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.TryGetInt64(out var integer) ? integer : (long)reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, long.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue(value.Value);
                else writer.WriteNullValue();
            }
        }
        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt16NullableConverter : JsonConverter<ushort?>
        {
            /// <inheritdoc />
            public override ushort? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.TryGetUInt16(out var integer) ? integer : (ushort)reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, ushort.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, ushort? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue(value.Value);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt32NullableConverter : JsonConverter<uint?>
        {
            /// <inheritdoc />
            public override uint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.TryGetUInt32(out var integer) ? integer : (uint)reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, uint.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, uint? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue(value.Value);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt64NullableConverter : JsonConverter<ulong?>
        {
            /// <inheritdoc />
            public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.TryGetUInt64(out var integer) ? integer : (ulong)reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, ulong.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue(value.Value);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class DecimalNullableConverter : JsonConverter<decimal?>
        {
            /// <inheritdoc />
            public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.GetDecimal(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, decimal.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue(value.Value);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class SingleNullableConverter : JsonConverter<float?>
        {
            /// <inheritdoc />
            public override float? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.GetSingle(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, float.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, float? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue(value.Value);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class DoubleNullableConverter : JsonConverter<double?>
        {
            /// <inheritdoc />
            public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, double.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue(value.Value);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class JsonIntegerConverter : JsonConverter<JsonInteger>
        {
            /// <inheritdoc />
            public override JsonInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var num = reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.TryGetInt64(out var integer) ? integer : (long)reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, long.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
                return num.HasValue ? new JsonInteger(num.Value) : null;
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, JsonInteger value, JsonSerializerOptions options)
            {
                if (value is null) writer.WriteNullValue();
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class JsonDoubleConverter : JsonConverter<JsonDouble>
        {
            /// <inheritdoc />
            public override JsonDouble Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var num = reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, double.Parse),
                    JsonTokenType.False => 0,
                    JsonTokenType.True => 1,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
                return num.HasValue ? new JsonDouble(num.Value) : null;
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, JsonDouble value, JsonSerializerOptions options)
            {
                if (value is null) writer.WriteNullValue(); 
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert == typeof(int)) return new Int32Converter();
            if (typeToConvert == typeof(long)) return new Int64Converter();
            if (typeToConvert == typeof(double)) return new DoubleConverter();
            if (typeToConvert == typeof(ulong)) return new UInt64Converter();
            if (typeToConvert == typeof(uint)) return new UInt32Converter();
            if (typeToConvert == typeof(float)) return new SingleConverter();
            if (typeToConvert == typeof(decimal)) return new SingleConverter();
            if (typeToConvert == typeof(short)) return new Int16Converter();
            if (typeToConvert == typeof(ushort)) return new UInt16Converter();
            if (typeToConvert == typeof(DateTime)) return new JsonJavaScriptTicksConverter();
            if (typeToConvert == typeof(TimeSpan)) return new JsonTimeSpanSecondConverter();
            if (typeToConvert == typeof(int?)) return new Int32NullableConverter();
            if (typeToConvert == typeof(long?)) return new Int64NullableConverter();
            if (typeToConvert == typeof(double?)) return new DoubleNullableConverter();
            if (typeToConvert == typeof(ulong?)) return new UInt64NullableConverter();
            if (typeToConvert == typeof(uint?)) return new UInt32NullableConverter();
            if (typeToConvert == typeof(float?)) return new SingleNullableConverter();
            if (typeToConvert == typeof(decimal?)) return new SingleNullableConverter();
            if (typeToConvert == typeof(short?)) return new Int16NullableConverter();
            if (typeToConvert == typeof(ushort?)) return new UInt16NullableConverter();
            if (typeToConvert == typeof(DateTime?)) return new JsonJavaScriptTicksConverter.NullableConverter();
            if (typeToConvert == typeof(TimeSpan?)) return new JsonTimeSpanSecondConverter.NullableConverter();
            if (typeToConvert == typeof(JsonInteger)) return new JsonIntegerConverter();
            if (typeToConvert == typeof(JsonDouble)) return new JsonDoubleConverter();
            throw new JsonException(typeToConvert.Name + " is not expected.");
        }

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(int)
                || typeToConvert == typeof(long)
                || typeToConvert == typeof(double)
                || typeToConvert == typeof(uint)
                || typeToConvert == typeof(ulong)
                || typeToConvert == typeof(float)
                || typeToConvert == typeof(decimal)
                || typeToConvert == typeof(short)
                || typeToConvert == typeof(ushort)
                || typeToConvert == typeof(DateTime)
                || typeToConvert == typeof(TimeSpan)
                || typeToConvert == typeof(int?)
                || typeToConvert == typeof(long?)
                || typeToConvert == typeof(double?)
                || typeToConvert == typeof(uint?)
                || typeToConvert == typeof(ulong?)
                || typeToConvert == typeof(float?)
                || typeToConvert == typeof(decimal?)
                || typeToConvert == typeof(short?)
                || typeToConvert == typeof(ushort?)
                || typeToConvert == typeof(DateTime?)
                || typeToConvert == typeof(TimeSpan?)
                || typeToConvert == typeof(JsonInteger)
                || typeToConvert == typeof(JsonDouble);
        }

        private static T ParseNumber<T>(ref Utf8JsonReader reader, Func<string, T> parser) where T : struct
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str)) return default;
            return parser(str);
        }

        private static T? ParseNullableNumber<T>(ref Utf8JsonReader reader, Func<string, T> parser) where T : struct
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str)) return null;
            return parser(str);
        }
    }
}
