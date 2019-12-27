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
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether need throw exception for null value.
            /// </summary>
            public bool NeedThrowForNull { get; set; }

            /// <inheritdoc />
            public override short Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => GetDefaultValue<short>(NeedThrowForNull),
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
                if (NeedWriteAsString) writer.WriteStringValue(value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int32Converter : JsonConverter<int>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether need throw exception for null value.
            /// </summary>
            public bool NeedThrowForNull { get; set; }

            /// <inheritdoc />
            public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => GetDefaultValue<int>(NeedThrowForNull),
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
                if (NeedWriteAsString) writer.WriteStringValue(value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int64Converter : JsonConverter<long>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether need throw exception for null value.
            /// </summary>
            public bool NeedThrowForNull { get; set; }

            /// <inheritdoc />
            public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => GetDefaultValue<long>(NeedThrowForNull),
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
                if (NeedWriteAsString) writer.WriteStringValue(value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value);
            }
        }
        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt16Converter : JsonConverter<ushort>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether need throw exception for null value.
            /// </summary>
            public bool NeedThrowForNull { get; set; }

            /// <inheritdoc />
            public override ushort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => GetDefaultValue<ushort>(NeedThrowForNull),
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
                if (NeedWriteAsString) writer.WriteStringValue(value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt32Converter : JsonConverter<uint>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether need throw exception for null value.
            /// </summary>
            public bool NeedThrowForNull { get; set; }

            /// <inheritdoc />
            public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => GetDefaultValue<uint>(NeedThrowForNull),
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
                if (NeedWriteAsString) writer.WriteStringValue(value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt64Converter : JsonConverter<ulong>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether need throw exception for null value.
            /// </summary>
            public bool NeedThrowForNull { get; set; }

            /// <inheritdoc />
            public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => GetDefaultValue<ulong>(NeedThrowForNull),
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
                if (NeedWriteAsString) writer.WriteStringValue(value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class DecimalConverter : JsonConverter<decimal>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether need throw exception for null value.
            /// </summary>
            public bool NeedThrowForNull { get; set; }

            /// <inheritdoc />
            public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => GetDefaultValue<decimal>(NeedThrowForNull),
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
                if (NeedWriteAsString) writer.WriteStringValue(value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class SingleConverter : JsonConverter<float>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether need throw exception for null value.
            /// </summary>
            public bool NeedThrowForNull { get; set; }

            /// <inheritdoc />
            public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => GetDefaultValue<float>(NeedThrowForNull),
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
                if (NeedWriteAsString) writer.WriteStringValue(value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class DoubleConverter : JsonConverter<double>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether need throw exception for null value.
            /// </summary>
            public bool NeedThrowForNull { get; set; }

            /// <inheritdoc />
            public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => GetDefaultValue<double>(NeedThrowForNull),
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
                if (NeedWriteAsString) writer.WriteStringValue(value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int16NullableConverter : JsonConverter<short?>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                if (!value.HasValue) writer.WriteNullValue(); 
                else if (NeedWriteAsString) writer.WriteStringValue(value.Value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int32NullableConverter : JsonConverter<int?>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                if (!value.HasValue) writer.WriteNullValue();
                else if (NeedWriteAsString) writer.WriteStringValue(value.Value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class Int64NullableConverter : JsonConverter<long?>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                if (!value.HasValue) writer.WriteNullValue();
                else if (NeedWriteAsString) writer.WriteStringValue(value.Value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value.Value);
            }
        }
        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt16NullableConverter : JsonConverter<ushort?>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                if (!value.HasValue) writer.WriteNullValue();
                else if (NeedWriteAsString) writer.WriteStringValue(value.Value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt32NullableConverter : JsonConverter<uint?>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                if (!value.HasValue) writer.WriteNullValue();
                else if (NeedWriteAsString) writer.WriteStringValue(value.Value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class UInt64NullableConverter : JsonConverter<ulong?>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                if (!value.HasValue) writer.WriteNullValue();
                else if (NeedWriteAsString) writer.WriteStringValue(value.Value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class DecimalNullableConverter : JsonConverter<decimal?>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                if (!value.HasValue) writer.WriteNullValue();
                else if (NeedWriteAsString) writer.WriteStringValue(value.Value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class SingleNullableConverter : JsonConverter<float?>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                if (!value.HasValue) writer.WriteNullValue();
                else if (NeedWriteAsString) writer.WriteStringValue(value.Value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class DoubleNullableConverter : JsonConverter<double?>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                if (!value.HasValue) writer.WriteNullValue();
                else if (NeedWriteAsString) writer.WriteStringValue(value.Value.ToString("g", CultureInfo.InvariantCulture));
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class StringConverter : JsonConverter<string>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <inheritdoc />
            public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.GetDouble().ToString("g", CultureInfo.InvariantCulture),
                    JsonTokenType.String => reader.GetString(),
                    JsonTokenType.False => JsonBoolean.FalseString,
                    JsonTokenType.True => JsonBoolean.TrueString,
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            {
                if (value == null) writer.WriteNullValue();
                if (NeedWriteAsString || value.Length == 0)
                {
                    writer.WriteStringValue(value);
                    return;
                }

                var lower = value.ToLowerInvariant();
                if (lower == JsonBoolean.TrueString)
                {
                    writer.WriteBooleanValue(true);
                    return;
                }

                if (lower == JsonBoolean.FalseString)
                {
                    writer.WriteBooleanValue(true);
                    return;
                }

                if (long.TryParse(lower, out var l))
                {
                    writer.WriteNumberValue(l);
                    return;
                }

                if (ulong.TryParse(lower, out var ul))
                {
                    writer.WriteNumberValue(ul);
                    return;
                }


                if (double.TryParse(lower, out var d))
                {
                    writer.WriteNumberValue(d);
                    return;
                }

                writer.WriteStringValue(value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class JsonIntegerConverter : JsonConverter<JsonInteger>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                else if (NeedWriteAsString) writer.WriteStringValue(value.ToString());
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback.
        /// </summary>
        sealed class JsonDoubleConverter : JsonConverter<JsonDouble>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

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
                else if (NeedWriteAsString) writer.WriteStringValue(value.ToString());
                else writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Json angle struct converter.
        /// </summary>
        sealed class AngleConverter : JsonConverter<Maths.Angle>
        {
            /// <inheritdoc />
            public override Maths.Angle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Number => reader.GetDouble(),
                    JsonTokenType.String => ParseNumber(ref reader, double.Parse),
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, Maths.Angle value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value.Degrees);
            }
        }

        /// <summary>
        /// Json nullable angle converter.
        /// </summary>
        sealed class AngleNullableConverter : JsonConverter<Maths.Angle?>
        {
            /// <inheritdoc />
            public override Maths.Angle? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var num = reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, double.Parse),
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
                if (num.HasValue) return num.Value;
                return null;
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, Maths.Angle? value, JsonSerializerOptions options)
            {
                if (value.HasValue) writer.WriteNumberValue(value.Value.Degrees);
                else writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Json angle model converter.
        /// </summary>
        sealed class AngleModelConverter : JsonConverter<Maths.Angle.Model>
        {
            /// <inheritdoc />
            public override Maths.Angle.Model Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var num = reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => reader.GetDouble(),
                    JsonTokenType.String => ParseNullableNumber(ref reader, double.Parse),
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
                };
                return num.HasValue ? new Maths.Angle.Model(num.Value) : null;
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, Maths.Angle.Model value, JsonSerializerOptions options)
            {
                if (value is null) writer.WriteNullValue();
                else writer.WriteNumberValue(value.Degrees);
            }
        }

        /// <summary>
        /// Json number string converter.
        /// </summary>
        public sealed class NumberStringConverter : JsonConverterFactory
        {
            /// <inheritdoc />
            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                if (typeToConvert == typeof(int)) return new Int32Converter { NeedWriteAsString = true };
                if (typeToConvert == typeof(long)) return new Int64Converter { NeedWriteAsString = true };
                if (typeToConvert == typeof(double)) return new DoubleConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(ulong)) return new UInt64Converter { NeedWriteAsString = true };
                if (typeToConvert == typeof(uint)) return new UInt32Converter { NeedWriteAsString = true };
                if (typeToConvert == typeof(float)) return new SingleConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(decimal)) return new SingleConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(short)) return new Int16Converter { NeedWriteAsString = true };
                if (typeToConvert == typeof(ushort)) return new UInt16Converter { NeedWriteAsString = true };
                if (typeToConvert == typeof(DateTime)) return new JsonJavaScriptTicksConverter.FallbackConverter();
                if (typeToConvert == typeof(TimeSpan)) return new JsonTimeSpanSecondConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(int?)) return new Int32NullableConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(long?)) return new Int64NullableConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(double?)) return new DoubleNullableConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(ulong?)) return new UInt64NullableConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(uint?)) return new UInt32NullableConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(float?)) return new SingleNullableConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(decimal?)) return new SingleNullableConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(short?)) return new Int16NullableConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(ushort?)) return new UInt16NullableConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(DateTime?)) return new JsonJavaScriptTicksConverter.FallbackNullableConverter();
                if (typeToConvert == typeof(TimeSpan?)) return new JsonTimeSpanSecondConverter.NullableConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(JsonInteger)) return new JsonIntegerConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(JsonDouble)) return new JsonDoubleConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(string)) return new StringConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(Maths.Angle)) return new AngleConverter();
                if (typeToConvert == typeof(Maths.Angle.Model)) return new AngleModelConverter();
                if (typeToConvert == typeof(Maths.Angle?)) return new AngleNullableConverter();
                throw new JsonException(typeToConvert.Name + " is not expected.");
            }

            /// <inheritdoc />
            public override bool CanConvert(Type typeToConvert)
            {
                return CanConvertType(typeToConvert);
            }
        }

        /// <summary>
        /// Json number converter with number string fallback and zero for null.
        /// </summary>
        public sealed class StrictConverter : JsonConverterFactory
        {
            /// <inheritdoc />
            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                if (typeToConvert == typeof(int)) return new Int32Converter { NeedThrowForNull = true };
                if (typeToConvert == typeof(long)) return new Int64Converter { NeedThrowForNull = true };
                if (typeToConvert == typeof(double)) return new DoubleConverter { NeedThrowForNull = true };
                if (typeToConvert == typeof(ulong)) return new UInt64Converter { NeedThrowForNull = true };
                if (typeToConvert == typeof(uint)) return new UInt32Converter { NeedThrowForNull = true };
                if (typeToConvert == typeof(float)) return new SingleConverter { NeedThrowForNull = true };
                if (typeToConvert == typeof(decimal)) return new SingleConverter { NeedThrowForNull = true };
                if (typeToConvert == typeof(short)) return new Int16Converter { NeedThrowForNull = true };
                if (typeToConvert == typeof(ushort)) return new UInt16Converter { NeedThrowForNull = true };
                if (typeToConvert == typeof(DateTime)) return new JsonJavaScriptTicksConverter();
                if (typeToConvert == typeof(TimeSpan)) return new JsonTimeSpanSecondConverter { NeedThrowForNull = true };
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
                if (typeToConvert == typeof(string)) return new StringConverter();
                if (typeToConvert == typeof(Maths.Angle)) return new AngleConverter();
                if (typeToConvert == typeof(Maths.Angle.Model)) return new AngleModelConverter();
                if (typeToConvert == typeof(Maths.Angle?)) return new AngleNullableConverter();
                throw new JsonException(typeToConvert.Name + " is not expected.");
            }

            /// <inheritdoc />
            public override bool CanConvert(Type typeToConvert)
            {
                return CanConvertType(typeToConvert);
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
            if (typeToConvert == typeof(string)) return new StringConverter();
            if (typeToConvert == typeof(Maths.Angle)) return new AngleConverter();
            if (typeToConvert == typeof(Maths.Angle.Model)) return new AngleModelConverter();
            if (typeToConvert == typeof(Maths.Angle?)) return new AngleNullableConverter();
            throw new JsonException(typeToConvert.Name + " is not expected.");
        }

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return CanConvertType(typeToConvert);
        }

        private static bool CanConvertType(Type typeToConvert)
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
                || typeToConvert == typeof(JsonDouble)
                || typeToConvert == typeof(string);
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

        private static T GetDefaultValue<T>(bool throwForNull) where T : struct
        {
            if (throwForNull) throw new JsonException("The value should not be null.");
            return default;
        }
    }
}
