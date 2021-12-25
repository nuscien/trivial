using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Trivial.Maths;
using Trivial.Web;

namespace Trivial.Text;

/// <summary>
/// Json number converter with number string fallback.
/// </summary>
public sealed class JsonNumberConverter : JsonConverterFactory
{
    delegate bool TryParseNumberHandler<T>(string s, int radix, out T number);

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
                JsonTokenType.String => ParseNumber<short>(ref reader, Numbers.TryParseToInt16),
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
                JsonTokenType.String => ParseNumber<int>(ref reader, Numbers.TryParseToInt32),
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
                JsonTokenType.String => ParseNumber<long>(ref reader, Numbers.TryParseToInt64),
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
                JsonTokenType.String => ParseNumber<ushort>(ref reader, Numbers.TryParseToUInt16),
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
                JsonTokenType.String => ParseNumber<uint>(ref reader, Numbers.TryParseToUInt32),
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
                JsonTokenType.String => ParseNumber<ulong>(ref reader, TryParseToUInt64),
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

        internal static bool TryParseToUInt64(string s, int radix, out ulong num)
        {
            if (ulong.TryParse(s, out num)) return true;
            if (Numbers.TryParseToInt64(s, radix, out var n) && n >= 0)
            {
                num = (ulong)n;
                return true;
            }

            num = default;
            return false;
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
                JsonTokenType.String => ParseNullableNumber<short>(ref reader, Numbers.TryParseToInt16),
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
                JsonTokenType.String => ParseNullableNumber<int>(ref reader, Numbers.TryParseToInt32),
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
                JsonTokenType.String => ParseNullableNumber<long>(ref reader, Numbers.TryParseToInt64),
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
                JsonTokenType.String => ParseNullableNumber<ushort>(ref reader, Numbers.TryParseToUInt16),
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
                JsonTokenType.String => ParseNullableNumber<uint>(ref reader, Numbers.TryParseToUInt32),
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
                JsonTokenType.String => ParseNullableNumber<ulong>(ref reader, UInt64Converter.TryParseToUInt64),
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
                JsonTokenType.False => JsonBooleanNode.FalseString,
                JsonTokenType.True => JsonBooleanNode.TrueString,
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
            if (lower == JsonBooleanNode.TrueString)
            {
                writer.WriteBooleanValue(true);
                return;
            }

            if (lower == JsonBooleanNode.FalseString)
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
    sealed class JsonNumberInterfaceConverter : JsonConverter<IJsonNumberNode>
    {
        /// <summary>
        /// Gets or sets a value indicating whether need also write to a string.
        /// </summary>
        public bool NeedWriteAsString { get; set; }

        /// <inheritdoc />
        public override IJsonNumberNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var l1)) return new JsonIntegerNode(l1);
                    return new JsonDoubleNode(reader.GetDouble());
                case JsonTokenType.String:
                    var str = reader.GetString();
                    if (string.IsNullOrWhiteSpace(str)) return null;
                    str = str.Trim();
                    if (str == "null") return null;
                    if (str.IndexOf('.') < 0 && long.TryParse(str, out var l2)) return new JsonIntegerNode(l2);
                    return new JsonDoubleNode(double.Parse(str));
                case JsonTokenType.False:
                    return new JsonIntegerNode(0);
                case JsonTokenType.True:
                    return new JsonIntegerNode(1);
                default:
                    throw new JsonException($"The token type is {reader.TokenType} but expect number.");
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IJsonNumberNode value, JsonSerializerOptions options)
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
    sealed class JsonIntegerConverter : JsonConverter<JsonIntegerNode>
    {
        /// <summary>
        /// Gets or sets a value indicating whether need also write to a string.
        /// </summary>
        public bool NeedWriteAsString { get; set; }

        /// <inheritdoc />
        public override JsonIntegerNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            return num.HasValue ? new JsonIntegerNode(num.Value) : null;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, JsonIntegerNode value, JsonSerializerOptions options)
        {
            if (value is null) writer.WriteNullValue();
            else if (NeedWriteAsString) writer.WriteStringValue(value.ToString());
            else writer.WriteNumberValue(value.Value);
        }
    }

    /// <summary>
    /// Json number converter with number string fallback.
    /// </summary>
    sealed class JsonDoubleConverter : JsonConverter<JsonDoubleNode>
    {
        /// <summary>
        /// Gets or sets a value indicating whether need also write to a string.
        /// </summary>
        public bool NeedWriteAsString { get; set; }

        /// <inheritdoc />
        public override JsonDoubleNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            return num.HasValue ? new JsonDoubleNode(num.Value) : null;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, JsonDoubleNode value, JsonSerializerOptions options)
        {
            if (value is null) writer.WriteNullValue();
            else if (NeedWriteAsString) writer.WriteStringValue(value.ToString());
            else writer.WriteNumberValue(value.Value);
        }
    }

    /// <summary>
    /// Json hex color string value converter.
    /// </summary>
    sealed class HexColorConverter : JsonConverter<System.Drawing.Color>
    {
        /// <inheritdoc />
        public override System.Drawing.Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Drawing.ColorCalculator.ParseValue(ref reader);

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, System.Drawing.Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(Drawing.ColorCalculator.ToHexString(value));
        }
    }

    /// <summary>
    /// Json hex color string value converter.
    /// </summary>
    sealed class NullableHexColorConverter : JsonConverter<System.Drawing.Color?>
    {
        /// <inheritdoc />
        public override System.Drawing.Color? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType switch
            {
                JsonTokenType.Null or JsonTokenType.False => null,
                _ => Drawing.ColorCalculator.ParseValue(ref reader)
            };

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, System.Drawing.Color? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(Drawing.ColorCalculator.ToHexString(value.Value));
            else
                writer.WriteNullValue();
        }
    }

    /// <summary>
    /// Json RGBA color string value converter.
    /// </summary>
    sealed class RgbaColorConverter : JsonConverter<System.Drawing.Color>
    {
        /// <inheritdoc />
        public override System.Drawing.Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Drawing.ColorCalculator.ParseValue(ref reader);

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, System.Drawing.Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(Drawing.ColorCalculator.ToRgbaString(value));
        }
    }

    /// <summary>
    /// Json RGBA color string value converter.
    /// </summary>
    sealed class NullableRgbaColorConverter : JsonConverter<System.Drawing.Color?>
    {
        /// <inheritdoc />
        public override System.Drawing.Color? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType switch
            {
                JsonTokenType.Null or JsonTokenType.False => null,
                _ => Drawing.ColorCalculator.ParseValue(ref reader)
            };

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, System.Drawing.Color? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(Drawing.ColorCalculator.ToRgbaString(value.Value));
            else
                writer.WriteNullValue();
        }
    }

    /// <summary>
    /// Json Int32 interval converter.
    /// </summary>
    sealed class Int32IntervalConverter : JsonConverter<StructValueSimpleInterval<int>>
    {
        /// <inheritdoc />
        public override StructValueSimpleInterval<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.Number => FromNumber(ref reader),
                JsonTokenType.String => FromString(ref reader, IntervalUtility.ParseForInt32),
                JsonTokenType.StartObject => JsonSerializer.Deserialize<StructValueSimpleInterval<int>>(ref reader, options),
                _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object or an interval format string.")
            };
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, StructValueSimpleInterval<int> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        internal static StructValueSimpleInterval<T> FromString<T>(ref Utf8JsonReader reader, Func<string, NumberStyles, IFormatProvider, StructValueSimpleInterval<T>> convert) where T : struct, IComparable<T>
        {
            var s = reader.GetString()?.Trim();
            try
            {
                return convert(s, NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                throw new JsonException(ex.Message, ex.InnerException ?? ex);
            }
        }

        internal static NullableValueSimpleInterval<T> FromString<T>(ref Utf8JsonReader reader, Func<string, NumberStyles, IFormatProvider, NullableValueSimpleInterval<T>> convert) where T : struct, IComparable<T>
        {
            var s = reader.GetString()?.Trim();
            try
            {
                return convert(s, NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
                throw new JsonException(ex.Message, ex.InnerException ?? ex);
            }
        }

        private static StructValueSimpleInterval<int> FromNumber(ref Utf8JsonReader reader)
        {
            try
            {
                var i = GetInt32(ref reader);
                return new StructValueSimpleInterval<int>(i, i, true, true);
            }
            catch (OverflowException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (JsonException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }

        }
    }

    /// <summary>
    /// Json nullable Int32 interval converter.
    /// </summary>
    sealed class NullableInt32IntervalConverter : JsonConverter<NullableValueSimpleInterval<int>>
    {
        /// <inheritdoc />
        public override NullableValueSimpleInterval<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.Number => FromNumber(ref reader),
                JsonTokenType.String => Int32IntervalConverter.FromString(ref reader, IntervalUtility.ParseForNullableInt32),
                JsonTokenType.StartObject => JsonSerializer.Deserialize<NullableValueSimpleInterval<int>>(ref reader, options),
                _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object or an interval format string.")
            };
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, NullableValueSimpleInterval<int> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        private static NullableValueSimpleInterval<int> FromNumber(ref Utf8JsonReader reader)
        {
            try
            {
                var i = reader.TryGetInt32(out var integer) ? integer : (int)reader.GetDouble();
                return new NullableValueSimpleInterval<int>(i, i, true, true);
            }
            catch (OverflowException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (JsonException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
        }
    }

    /// <summary>
    /// Json Int64 interval converter.
    /// </summary>
    sealed class Int64IntervalConverter : JsonConverter<StructValueSimpleInterval<long>>
    {
        /// <inheritdoc />
        public override StructValueSimpleInterval<long> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.Number => FromNumber(ref reader),
                JsonTokenType.String => Int32IntervalConverter.FromString(ref reader, IntervalUtility.ParseForInt64),
                JsonTokenType.StartObject => JsonSerializer.Deserialize<StructValueSimpleInterval<long>>(ref reader, options),
                _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object or an interval format string.")
            };
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, StructValueSimpleInterval<long> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        private static StructValueSimpleInterval<long> FromNumber(ref Utf8JsonReader reader)
        {
            try
            {
                var i = reader.TryGetInt32(out var integer) ? integer : (long)reader.GetDouble();
                return new StructValueSimpleInterval<long>(i, i, true, true);
            }
            catch (OverflowException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (JsonException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }

        }
    }

    /// <summary>
    /// Json nullable Int64 interval converter.
    /// </summary>
    sealed class NullableInt64IntervalConverter : JsonConverter<NullableValueSimpleInterval<long>>
    {
        /// <inheritdoc />
        public override NullableValueSimpleInterval<long> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.Number => FromNumber(ref reader),
                JsonTokenType.String => Int32IntervalConverter.FromString(ref reader, IntervalUtility.ParseForNullableInt64),
                JsonTokenType.StartObject => JsonSerializer.Deserialize<NullableValueSimpleInterval<long>>(ref reader, options),
                _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object or an interval format string.")
            };
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, NullableValueSimpleInterval<long> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        private static NullableValueSimpleInterval<long> FromNumber(ref Utf8JsonReader reader)
        {
            try
            {
                var i = reader.TryGetInt32(out var integer) ? integer : (long)reader.GetDouble();
                return new NullableValueSimpleInterval<long>(i, i, true, true);
            }
            catch (OverflowException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }
            catch (JsonException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex);
            }

        }
    }

    /// <summary>
    /// Json double interval converter.
    /// </summary>
    sealed class DoubleIntervalConverter : JsonConverter<StructValueSimpleInterval<double>>
    {
        /// <inheritdoc />
        public override StructValueSimpleInterval<double> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.Number => FromNumber(ref reader),
                JsonTokenType.String => Int32IntervalConverter.FromString(ref reader, IntervalUtility.ParseForDouble),
                JsonTokenType.StartObject => JsonSerializer.Deserialize<StructValueSimpleInterval<double>>(ref reader, options),
                _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object or an interval format string.")
            };
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, StructValueSimpleInterval<double> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        private static StructValueSimpleInterval<double> FromNumber(ref Utf8JsonReader reader)
        {
            var i = reader.GetDouble();
            return new StructValueSimpleInterval<double>(i, i, true, true);
        }
    }

    /// <summary>
    /// Json version converter.
    /// </summary>
    sealed class VersionIntervalConverter : JsonConverter<VersionSimpleInterval>
    {
        /// <inheritdoc />
        public override VersionSimpleInterval Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.Number => FromNumber(ref reader),
                JsonTokenType.String => FromString(ref reader),
                JsonTokenType.StartObject => JsonSerializer.Deserialize<VersionSimpleInterval>(ref reader, options),
                _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object or an interval format string.")
            };
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, VersionSimpleInterval value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        private static VersionSimpleInterval FromString(ref Utf8JsonReader reader)
        {
            var s = reader.GetString();
            try
            {
                return VersionSimpleInterval.Parse(s);
            }
            catch (FormatException ex)
            {
                throw new JsonException(IntervalUtility.ErrorParseMessage, ex.InnerException ?? ex);
            }
        }

        private static VersionSimpleInterval FromNumber(ref Utf8JsonReader reader)
        {
            if (reader.TryGetInt64(out var i)) return new VersionSimpleInterval(i.ToString("g"), BasicCompareOperator.Equal);
            var s = reader.GetDouble().ToString("g");
            var arr = s.Split(new[] { '.', ',' });
            if (arr.Length == 0 || !int.TryParse(arr[0], out var a) || s.StartsWith("-") || s.IndexOf("e", StringComparison.OrdinalIgnoreCase) >= 0 || s.IndexOf("i", StringComparison.OrdinalIgnoreCase) >= 0)
                throw new JsonException("The token value should be an interval format string but it is a floating number.");
            if (arr.Length == 1 || !int.TryParse(arr[1], out var b)) return new VersionSimpleInterval(a.ToString("g"), BasicCompareOperator.Equal);
            return new VersionSimpleInterval(a.ToString("g"), b.ToString("g"));
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
            if (typeToConvert == typeof(JsonIntegerNode)) return new JsonIntegerConverter { NeedWriteAsString = true };
            if (typeToConvert == typeof(JsonDoubleNode)) return new JsonDoubleConverter { NeedWriteAsString = true };
            if (typeToConvert == typeof(IJsonNumberNode)) return new JsonNumberInterfaceConverter { NeedWriteAsString = true };
            if (typeToConvert == typeof(string)) return new StringConverter { NeedWriteAsString = true };
            if (typeToConvert == typeof(StructValueSimpleInterval<int>)) return new Int32IntervalConverter();
            if (typeToConvert == typeof(NullableValueSimpleInterval<int>)) return new NullableInt32IntervalConverter();
            if (typeToConvert == typeof(StructValueSimpleInterval<long>)) return new Int64IntervalConverter();
            if (typeToConvert == typeof(NullableValueSimpleInterval<long>)) return new NullableInt64IntervalConverter();
            if (typeToConvert == typeof(StructValueSimpleInterval<double>)) return new DoubleIntervalConverter();
            if (typeToConvert == typeof(VersionSimpleInterval)) return new VersionIntervalConverter();
            if (typeToConvert == typeof(System.Drawing.Color)) return new RgbaColorConverter();
            if (typeToConvert == typeof(System.Drawing.Color?)) return new NullableRgbaColorConverter();
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
            if (typeToConvert == typeof(JsonIntegerNode)) return new JsonIntegerConverter();
            if (typeToConvert == typeof(JsonDoubleNode)) return new JsonDoubleConverter();
            if (typeToConvert == typeof(IJsonNumberNode)) return new JsonNumberInterfaceConverter();
            if (typeToConvert == typeof(string)) return new StringConverter();
            if (typeToConvert == typeof(StructValueSimpleInterval<int>)) return new Int32IntervalConverter();
            if (typeToConvert == typeof(NullableValueSimpleInterval<int>)) return new NullableInt32IntervalConverter();
            if (typeToConvert == typeof(StructValueSimpleInterval<long>)) return new Int64IntervalConverter();
            if (typeToConvert == typeof(NullableValueSimpleInterval<long>)) return new NullableInt64IntervalConverter();
            if (typeToConvert == typeof(StructValueSimpleInterval<double>)) return new DoubleIntervalConverter();
            if (typeToConvert == typeof(VersionSimpleInterval)) return new VersionIntervalConverter();
            if (typeToConvert == typeof(System.Drawing.Color)) return new HexColorConverter();
            if (typeToConvert == typeof(System.Drawing.Color?)) return new NullableHexColorConverter();
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
        if (typeToConvert == typeof(JsonIntegerNode)) return new JsonIntegerConverter();
        if (typeToConvert == typeof(JsonDoubleNode)) return new JsonDoubleConverter();
        if (typeToConvert == typeof(IJsonNumberNode)) return new JsonNumberInterfaceConverter();
        if (typeToConvert == typeof(string)) return new StringConverter();
        if (typeToConvert == typeof(StructValueSimpleInterval<int>)) return new Int32IntervalConverter();
        if (typeToConvert == typeof(NullableValueSimpleInterval<int>)) return new NullableInt32IntervalConverter();
        if (typeToConvert == typeof(StructValueSimpleInterval<long>)) return new Int64IntervalConverter();
        if (typeToConvert == typeof(NullableValueSimpleInterval<long>)) return new NullableInt64IntervalConverter();
        if (typeToConvert == typeof(StructValueSimpleInterval<double>)) return new DoubleIntervalConverter();
        if (typeToConvert == typeof(VersionSimpleInterval)) return new VersionIntervalConverter();
        if (typeToConvert == typeof(System.Drawing.Color)) return new HexColorConverter();
        if (typeToConvert == typeof(System.Drawing.Color?)) return new NullableHexColorConverter();
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
            || typeToConvert == typeof(JsonIntegerNode)
            || typeToConvert == typeof(JsonDoubleNode)
            || typeToConvert == typeof(IJsonNumberNode)
            || typeToConvert == typeof(string)
            || typeToConvert == typeof(StructValueSimpleInterval<int>)
            || typeToConvert == typeof(NullableValueSimpleInterval<int>)
            || typeToConvert == typeof(StructValueSimpleInterval<long>)
            || typeToConvert == typeof(NullableValueSimpleInterval<long>)
            || typeToConvert == typeof(StructValueSimpleInterval<double>)
            || typeToConvert == typeof(VersionSimpleInterval)
            || typeToConvert == typeof(Angle)
            || typeToConvert == typeof(Angle.Model)
            || typeToConvert == typeof(Angle?)
            || typeToConvert == typeof(System.Drawing.Color)
            || typeToConvert == typeof(System.Drawing.Color?);
    }

    internal static T ParseNumber<T>(ref Utf8JsonReader reader, Func<string, T> parser) where T : struct
    {
        var str = reader.GetString();
        if (string.IsNullOrWhiteSpace(str)) return default;
        return parser(str.Trim());
    }

    private static T ParseNumber<T>(ref Utf8JsonReader reader, TryParseNumberHandler<T> parser) where T : struct
    {
        var str = reader.GetString();
        if (string.IsNullOrWhiteSpace(str)) return default;
        if (parser(str, 10, out var num)) return num;
        throw new JsonException($"The token type is {reader.TokenType} but expect number.");
    }

    internal static T? ParseNullableNumber<T>(ref Utf8JsonReader reader, Func<string, T> parser) where T : struct
    {
        var str = reader.GetString();
        if (string.IsNullOrWhiteSpace(str)) return null;
        str = str.Trim();
        if (str == "null") return null;
        return parser(str);
    }

    private static T? ParseNullableNumber<T>(ref Utf8JsonReader reader, TryParseNumberHandler<T> parser) where T : struct
    {
        var str = reader.GetString();
        if (string.IsNullOrWhiteSpace(str) || str.Trim() == "null") return null;
        if (parser(str, 10, out var num)) return num;
        throw new JsonException($"The token type is {reader.TokenType} but expect number.");
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
