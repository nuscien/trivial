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
                    JsonTokenType.Number => GetInt32(ref reader),
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
                    JsonTokenType.Number => GetInt32(ref reader),
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
        sealed class JsonNumberInterfaceConverter : JsonConverter<IJsonNumber>
        {
            /// <summary>
            /// Gets or sets a value indicating whether need also write to a string.
            /// </summary>
            public bool NeedWriteAsString { get; set; }

            /// <inheritdoc />
            public override IJsonNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.Null:
                        return null;
                    case JsonTokenType.Number:
                        if (reader.TryGetInt64(out var l1)) return new JsonInteger(l1);
                        return new JsonDouble(reader.GetDouble());
                    case JsonTokenType.String:
                        var str = reader.GetString();
                        if (string.IsNullOrWhiteSpace(str)) return null;
                        str = str.Trim();
                        if (str == "null") return null;
                        if (str.IndexOf('.') < 0 && long.TryParse(str, out var l2)) return new JsonInteger(l2);
                        return new JsonDouble(double.Parse(str));
                    case JsonTokenType.False:
                        return new JsonInteger(0);
                    case JsonTokenType.True:
                        return new JsonInteger(1);
                    default:
                        throw new JsonException($"The token type is {reader.TokenType} but expect number.");
                }
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, IJsonNumber value, JsonSerializerOptions options)
            {
                if (value is null) writer.WriteNullValue();
                else if (NeedWriteAsString) writer.WriteStringValue(value.ToString());
                else if (value.IsInteger) writer.WriteNumberValue(value.GetInt64());
                else writer.WriteNumberValue(value.GetDouble());
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
        /// Json angle struct converter.
        /// </summary>
        sealed class Int32IntervalConverter : JsonConverter<Maths.StructValueSimpleInterval<int>>
        {
            private const string ErrorParseMessage = "The value is not the internal format string.";
            private const string digits = "0123456789+-∞";

            /// <inheritdoc />
            public override Maths.StructValueSimpleInterval<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => FromNumber(ref reader),
                    JsonTokenType.String => FromString(ref reader, 0, int.Parse),
                    JsonTokenType.StartObject => JsonSerializer.Deserialize<Maths.StructValueSimpleInterval<int>>(ref reader, options),
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object or an interval format string.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, Maths.StructValueSimpleInterval<int> value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }

            internal static Maths.StructValueSimpleInterval<T> FromString<T>(ref Utf8JsonReader reader, T defaultValue, Func<string, T> convert) where T : struct, IComparable<T>
            {
                var s = reader.GetString()?.Trim();
                if (string.IsNullOrEmpty(s) || s.Length < 2) return null;
                var v = new Maths.StructValueSimpleInterval<T>(defaultValue, defaultValue, false, false);
                if (s[0] == '(' || s[0] == ']')
                {
                    v.LeftOpen = false;
                }
                else if (s[0] != '[')
                {
                    if (digits.Contains(s[0]))
                    {
                        try
                        {
                            v.MaxValue = v.MinValue = convert(s);
                            return v;
                        }
                        catch (OverflowException ex)
                        {
                            throw new JsonException(ErrorParseMessage, ex);
                        }
                        catch (FormatException ex)
                        {
                            throw new JsonException(ErrorParseMessage, ex);
                        }
                    }

                    throw new JsonException(ErrorParseMessage, new FormatException($"Expect the first character is ( or [ but it is {s[0]}."));
                }

                var last = s.Length - 1;
                if (s[last] == ')' || s[last] == '[') v.LeftOpen = false;
                else if (s[last] != ']') throw new JsonException(ErrorParseMessage, new FormatException($"Expect the last character ] or ) but it is {s[last]}."));
                var split = ';';
                if (s.IndexOf(split) < 0) split = ',';

                #pragma warning disable IDE0057
                var arr = s.Substring(1, s.Length - 2).Split(split);
                #pragma warning restore IDE0057
                
                if (arr.Length == 0) return v;
                try
                {
                    var ele = arr[0]?.Trim();
                    var n = string.IsNullOrEmpty(ele) ? defaultValue : convert(ele);
                    if (v.IsGreaterThanMaxValue(n)) v.MaxValue = n; 
                    v.MinValue = string.IsNullOrEmpty(ele) ? defaultValue : convert(ele);
                }
                catch (OverflowException ex)
                {
                    throw new JsonException(ErrorParseMessage, ex);
                }
                catch (FormatException ex)
                {
                    throw new JsonException(ErrorParseMessage, ex);
                }

                try
                {
                    var ele = arr[1]?.Trim();
                    var n = string.IsNullOrEmpty(ele) ? defaultValue : convert(ele);
                    if (v.IsLessThanMinValue(n))
                    {
                        v.MaxValue = v.MinValue;
                        v.MinValue = n;
                    }
                    else
                    {
                        v.MaxValue = n;
                    }
                }
                catch (OverflowException ex)
                {
                    throw new JsonException(ErrorParseMessage, ex);
                }
                catch (FormatException ex)
                {
                    throw new JsonException(ErrorParseMessage, ex);
                }

                return v;
            }

            private static Maths.StructValueSimpleInterval<int> FromNumber(ref Utf8JsonReader reader)
            {
                var i = GetInt32(ref reader);
                return new Maths.StructValueSimpleInterval<int>(i, i, true, true);
            }
        }

        /// <summary>
        /// Json angle struct converter.
        /// </summary>
        sealed class Int64IntervalConverter : JsonConverter<Maths.StructValueSimpleInterval<long>>
        {
            /// <inheritdoc />
            public override Maths.StructValueSimpleInterval<long> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => FromNumber(ref reader),
                    JsonTokenType.String => Int32IntervalConverter.FromString(ref reader, 0, long.Parse),
                    JsonTokenType.StartObject => JsonSerializer.Deserialize<Maths.StructValueSimpleInterval<long>>(ref reader, options),
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object or an interval format string.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, Maths.StructValueSimpleInterval<long> value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }

            private static Maths.StructValueSimpleInterval<long> FromNumber(ref Utf8JsonReader reader)
            {
                var i = reader.TryGetInt32(out var integer) ? integer : (long)reader.GetDouble();
                return new Maths.StructValueSimpleInterval<long>(i, i, true, true);
            }
        }

        /// <summary>
        /// Json angle struct converter.
        /// </summary>
        sealed class DoubleIntervalConverter : JsonConverter<Maths.StructValueSimpleInterval<double>>
        {
            /// <inheritdoc />
            public override Maths.StructValueSimpleInterval<double> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.TokenType switch
                {
                    JsonTokenType.Null => null,
                    JsonTokenType.Number => FromNumber(ref reader),
                    JsonTokenType.String => Int32IntervalConverter.FromString(ref reader, 0, double.Parse),
                    JsonTokenType.StartObject => JsonSerializer.Deserialize<Maths.StructValueSimpleInterval<double>>(ref reader, options),
                    _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object or an interval format string.")
                };
            }

            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, Maths.StructValueSimpleInterval<double> value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }

            private static Maths.StructValueSimpleInterval<double> FromNumber(ref Utf8JsonReader reader)
            {
                var i = reader.GetDouble();
                return new Maths.StructValueSimpleInterval<double>(i, i, true, true);
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
                if (typeToConvert == typeof(IJsonNumber)) return new JsonNumberInterfaceConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(string)) return new StringConverter { NeedWriteAsString = true };
                if (typeToConvert == typeof(Maths.StructValueSimpleInterval<int>)) return new Int32IntervalConverter();
                if (typeToConvert == typeof(Maths.StructValueSimpleInterval<long>)) return new Int64IntervalConverter();
                if (typeToConvert == typeof(Maths.StructValueSimpleInterval<double>)) return new DoubleIntervalConverter();
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
                if (typeToConvert == typeof(IJsonNumber)) return new JsonNumberInterfaceConverter();
                if (typeToConvert == typeof(string)) return new StringConverter();
                if (typeToConvert == typeof(Maths.StructValueSimpleInterval<int>)) return new Int32IntervalConverter();
                if (typeToConvert == typeof(Maths.StructValueSimpleInterval<long>)) return new Int64IntervalConverter();
                if (typeToConvert == typeof(Maths.StructValueSimpleInterval<double>)) return new DoubleIntervalConverter();
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
            if (typeToConvert == typeof(IJsonNumber)) return new JsonNumberInterfaceConverter();
            if (typeToConvert == typeof(string)) return new StringConverter();
            if (typeToConvert == typeof(Maths.StructValueSimpleInterval<int>)) return new Int32IntervalConverter();
            if (typeToConvert == typeof(Maths.StructValueSimpleInterval<long>)) return new Int64IntervalConverter();
            if (typeToConvert == typeof(Maths.StructValueSimpleInterval<double>)) return new DoubleIntervalConverter();
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

        /// <summary>
        /// Tests if the specific type can convert.
        /// </summary>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <returns>true if can; otherwise, false.</returns>
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
                || typeToConvert == typeof(IJsonNumber)
                || typeToConvert == typeof(string)
                || typeToConvert == typeof(Maths.Angle)
                || typeToConvert == typeof(Maths.Angle.Model)
                || typeToConvert == typeof(Maths.Angle?);
        }

        private static T ParseNumber<T>(ref Utf8JsonReader reader, Func<string, T> parser) where T : struct
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str)) return default;
            return parser(str.Trim());
        }

        private static T? ParseNullableNumber<T>(ref Utf8JsonReader reader, Func<string, T> parser) where T : struct
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str)) return null;
            str = str.Trim();
            if (str == "null") return null;
            return parser(str);
        }

        private static T GetDefaultValue<T>(bool throwForNull) where T : struct
        {
            if (throwForNull) throw new JsonException("The value should not be null.");
            return default;
        }

        private static int GetInt32(ref Utf8JsonReader reader)
        {
            return reader.TryGetInt32(out var integer) ? integer : (int)reader.GetDouble();
        }
    }
}
