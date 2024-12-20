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
using Trivial.Reflection;

namespace Trivial.Text;

/// <summary>
/// JSON object node and JSON array node converter.
/// </summary>
public sealed class JsonValueNodeConverter : JsonConverterFactory
{
    /// <summary>
    /// JSON value node converter.
    /// </summary>
    internal sealed class CommonConverter : JsonConverter<IJsonValueNode>
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
            => typeof(IJsonValueNode).IsAssignableFrom(typeToConvert);

        /// <inheritdoc />
        public override IJsonValueNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonValues.SkipComments(ref reader);
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    if (typeToConvert == typeof(JsonObjectNode) || typeToConvert == typeof(JsonArrayNode)) return null;
                    if (typeToConvert == typeof(JsonStringNode)) return new JsonStringNode(null as string);
                    if (typeToConvert == typeof(IJsonValueNode) || typeToConvert == typeof(BaseJsonValueNode)) return JsonValues.Null;
                    return null;
                case JsonTokenType.StartObject:
                    return new JsonObjectNode(ref reader);
                case JsonTokenType.StartArray:
                    var arr = JsonArrayNode.ParseValue(ref reader);
                    if (arr is null) return null;
                    if (typeToConvert == typeof(JsonArrayNode)) return arr;
                    if (typeToConvert == typeof(JsonObjectNode)) return (JsonObjectNode)arr;
                    if (arr.Count <= 1 && (typeToConvert == typeof(JsonStringNode)
                        || typeToConvert == typeof(JsonIntegerNode)
                        || typeToConvert == typeof(JsonDoubleNode)
                        || typeToConvert == typeof(JsonBooleanNode))) return arr.Count == 1 ? arr[0] : default;
                    return arr;
                case JsonTokenType.String:
                    var str = reader.GetString();
                    if (typeToConvert == typeof(JsonStringNode) || typeToConvert == typeof(IJsonValueNode<string>)) return new JsonStringNode(str);
                    if (typeToConvert == typeof(JsonIntegerNode) || typeToConvert == typeof(IJsonValueNode<int>)) return new JsonIntegerNode(long.Parse(str.Trim()));
                    if (typeToConvert == typeof(JsonDoubleNode) || typeToConvert == typeof(IJsonValueNode<double>)) return new JsonDoubleNode(double.Parse(str.Trim()));
                    if (typeToConvert == typeof(IJsonNumberNode))
                    {
                        str = str.Trim();
                        if (str == "null") return null;
                        if (str.IndexOf('.') < 0 && long.TryParse(str, out var l))
                            return new JsonIntegerNode(l);
                        return new JsonDoubleNode(double.Parse(str));
                    }

                    if (typeToConvert == typeof(JsonBooleanNode) || typeToConvert == typeof(IJsonValueNode<bool>))
                    {
                        return JsonBooleanNode.TryParse(str);
                    }

                    return new JsonStringNode(str);
                case JsonTokenType.Number:
                    if (typeToConvert == typeof(JsonStringNode))
                    {
                        if (reader.TryGetInt64(out var l)) return new JsonStringNode(l);
                        return new JsonStringNode(reader.GetDouble());
                    }

                    if (reader.TryGetInt64(out var int64v)) return new JsonIntegerNode(int64v);
                    return new JsonDoubleNode(reader.GetDouble());
                case JsonTokenType.True:
                    if (typeToConvert == typeof(JsonBooleanNode)) return JsonBooleanNode.True;
                    if (typeToConvert == typeof(JsonStringNode)) return new JsonStringNode(JsonBooleanNode.TrueString);
                    if (typeToConvert == typeof(JsonIntegerNode)) return new JsonIntegerNode(1);
                    if (typeToConvert == typeof(IJsonNumberNode)) return new JsonIntegerNode(1);
                    return JsonBooleanNode.True;
                case JsonTokenType.False:
                    if (typeToConvert == typeof(JsonBooleanNode)) return JsonBooleanNode.False;
                    if (typeToConvert == typeof(JsonStringNode)) return new JsonStringNode(JsonBooleanNode.FalseString);
                    if (typeToConvert == typeof(JsonIntegerNode)) return new JsonIntegerNode(0);
                    if (typeToConvert == typeof(IJsonNumberNode)) return new JsonIntegerNode(0);
                    return JsonBooleanNode.False;
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IJsonValueNode value, JsonSerializerOptions options)
            => JsonValueNodeConverter.Write(writer, value, options);
    }

    /// <summary>
    /// JSON value node converter.
    /// </summary>
    internal sealed class ValueConverter : JsonConverter<BaseJsonValueNode>
    {
        /// <inheritdoc />
        public override BaseJsonValueNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => JsonValues.ToJsonValue(ref reader);

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BaseJsonValueNode value, JsonSerializerOptions options)
            => JsonValueNodeConverter.Write(writer, value, options);
    }

    /// <summary>
    /// JSON object node converter.
    /// </summary>
    internal sealed class ObjectConverter : JsonConverter<JsonObjectNode>
    {
        /// <inheritdoc />
        public override JsonObjectNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonValues.SkipComments(ref reader);
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.StartObject:
                    return new(ref reader);
                case JsonTokenType.StartArray:
                    var arr = JsonArrayNode.ParseValue(ref reader);
                    if (arr is null) return null;
                    return (JsonObjectNode)arr;
                case JsonTokenType.String:
                    return JsonObjectNode.TryParse(reader.GetString()) ?? throw new JsonException("Expects an object but it was a string.");
                case JsonTokenType.False:
                    return null;
            }

            throw new JsonException("Expects an object.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, JsonObjectNode value, JsonSerializerOptions options)
            => value.WriteTo(writer);
    }

    /// <summary>
    /// JSON array node converter.
    /// </summary>
    internal sealed class ArrayConverter : JsonConverter<JsonArrayNode>
    {
        /// <inheritdoc />
        public override JsonArrayNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonValues.SkipComments(ref reader);
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.StartArray:
                    return JsonArrayNode.ParseValue(ref reader);
                case JsonTokenType.String:
                    return JsonArrayNode.TryParse(reader.GetString()) ?? throw new JsonException("Expects an array but it was a string.");
                case JsonTokenType.False:
                    return null;
            }

            throw new JsonException("Expects an array.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, JsonArrayNode value, JsonSerializerOptions options)
            => value.WriteTo(writer);
    }

    /// <summary>
    /// JSON string node converter.
    /// </summary>
    internal sealed class StringConverter : JsonConverter<JsonStringNode>
    {
        /// <inheritdoc />
        public override JsonStringNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonValues.SkipComments(ref reader);
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return new(null as string);
                case JsonTokenType.StartObject:
                    var obj = new JsonObjectNode(ref reader);
                    return new(obj?.ToString());
                case JsonTokenType.StartArray:
                    var arr = JsonArrayNode.ParseValue(ref reader);
                    if (arr is null || arr.Count == 0) return null;
                    if (arr.Count == 1)
                    {
                        var kind = arr.GetValueKind(0);
                        switch (kind)
                        {
                            case JsonValueKind.String:
                            case JsonValueKind.Number:
                            case JsonValueKind.True:
                            case JsonValueKind.False:
                                return arr.TryGetStringValue(0);
                            case JsonValueKind.Null:
                                return new(null as string);
                            case JsonValueKind.Undefined:
                                return null;
                        }
                    }

                    return new(arr.ToString());
                case JsonTokenType.String:
                    var str = reader.GetString();
                    return new(str);
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var l)) return new(l);
                    return new(reader.GetDouble());
                case JsonTokenType.True:
                    return new(JsonBooleanNode.TrueString);
                case JsonTokenType.False:
                    return new(JsonBooleanNode.FalseString);
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, JsonStringNode value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Value);
    }

    /// <summary>
    /// JSON integer node converter.
    /// </summary>
    internal sealed class IntegerConverter : JsonConverter<JsonIntegerNode>
    {
        /// <inheritdoc />
        public override JsonIntegerNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonValues.SkipComments(ref reader);
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String:
                    var str = reader.GetString();
                    if (Numbers.TryParseToInt64(str, 10, out var i)) return new(i);
                    break;
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var l)) return new(l);
                    return new(reader.GetDouble());
                case JsonTokenType.True:
                    return new(1);
                case JsonTokenType.False:
                    return new(0);
            }

            throw new JsonException("Expects an integer.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, JsonIntegerNode value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value.Value);
    }

    /// <summary>
    /// JSON integer node converter.
    /// </summary>
    internal sealed class DoubleConverter : JsonConverter<JsonDoubleNode>
    {
        /// <inheritdoc />
        public override JsonDoubleNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonValues.SkipComments(ref reader);
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return new(double.NaN);
                case JsonTokenType.String:
                    var str = reader.GetString();
                    if (double.TryParse(str, out var i)) return new(i);
                    break;
                case JsonTokenType.Number:
                    return new(reader.GetDouble());
                case JsonTokenType.True:
                    return new(1d);
                case JsonTokenType.False:
                    return new(0d);
            }

            throw new JsonException("Expects a number.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, JsonDoubleNode value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value.Value);
    }

    /// <summary>
    /// JSON integer node converter.
    /// </summary>
    internal sealed class DecimalConverter : JsonConverter<JsonDecimalNode>
    {
        /// <inheritdoc />
        public override JsonDecimalNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonValues.SkipComments(ref reader);
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String:
                    var str = reader.GetString();
                    if (decimal.TryParse(str, out var i)) return new(i);
                    break;
                case JsonTokenType.Number:
                    return new(reader.GetDouble());
                case JsonTokenType.True:
                    return new(1);
                case JsonTokenType.False:
                    return new(0);
            }

            throw new JsonException("Expects a number.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, JsonDecimalNode value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value.Value);
    }

    /// <summary>
    /// JSON integer node converter.
    /// </summary>
    internal sealed class BooleanConverter : JsonConverter<JsonBooleanNode>
    {
        /// <inheritdoc />
        public override JsonBooleanNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonValues.SkipComments(ref reader);
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String:
                    var str = reader.GetString();
                    return JsonBooleanNode.TryParse(str) ?? throw new JsonException("Expects a boolean but it was a string.");
                case JsonTokenType.Number:
                    var i = reader.GetDouble();
                    if (i == 1d) return JsonBooleanNode.True;
                    if (i == 0d) return JsonBooleanNode.False;
                    throw new JsonException("Expects a boolean but it was a number.");
                case JsonTokenType.True:
                    return JsonBooleanNode.True;
                case JsonTokenType.False:
                    return JsonBooleanNode.False;
            }

            throw new JsonException("Expects a number.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, JsonBooleanNode value, JsonSerializerOptions options)
            => writer.WriteBooleanValue(value.Value);
    }

    /// <summary>
    /// JSON object node schema description converter.
    /// </summary>
    internal sealed class SchemaConverter : JsonConverter<JsonNodeSchemaDescription>
    {
        /// <inheritdoc />
        public override JsonNodeSchemaDescription Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonValues.SkipComments(ref reader);
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return default;
                case JsonTokenType.StartObject:
                    var json = JsonObjectNode.ParseValue(ref reader);
                    return JsonValues.ConvertToObjectSchema(json);
                default:
                    throw new JsonException($"The token type is {reader.TokenType} but expect JSON object or null for {typeToConvert}.");
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, JsonNodeSchemaDescription value, JsonSerializerOptions options)
        {
            var json = value?.ToJson();
            if (json is null) writer.WriteNullValue();
            json.WriteTo(writer);
        }
    }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeof(IJsonValueNode).IsAssignableFrom(typeToConvert) || typeToConvert == typeof(JsonNodeSchemaDescription);

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(IJsonValueNode)) return new CommonConverter();
        if (typeToConvert == typeof(JsonObjectNode)) return new ObjectConverter();
        if (typeToConvert == typeof(JsonArrayNode)) return new ArrayConverter();
        if (typeToConvert == typeof(BaseJsonValueNode)) return new ValueConverter();
        if (typeToConvert == typeof(JsonStringNode)) return new StringConverter();
        if (typeToConvert == typeof(JsonIntegerNode)) return new IntegerConverter();
        if (typeToConvert == typeof(JsonDoubleNode)) return new DoubleConverter();
        if (typeToConvert == typeof(JsonDecimalNode)) return new DecimalConverter();
        if (typeToConvert == typeof(JsonBooleanNode)) return new BooleanConverter();
        if (typeToConvert == typeof(JsonNodeSchemaDescription)) return new SchemaConverter();
        return new CommonConverter();
    }

    /// <summary>
    /// Writes a specified value as JSON.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    internal static void Write(Utf8JsonWriter writer, IJsonValueNode value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else if (value is JsonObjectNode jObj) jObj.WriteTo(writer);
        else if (value is JsonArrayNode jArr) jArr.WriteTo(writer);
        else if (value is IJsonValueNode<string> jStr)
        {
            if (jStr.Value == null) writer.WriteNullValue();
            else writer.WriteStringValue(jStr.Value);
        }
        else if (value is IJsonValueNode<long> jInt64) writer.WriteNumberValue(jInt64.Value);
        else if (value is IJsonValueNode<double> jDouble) writer.WriteNumberValue(jDouble.Value);
        else if (value is IJsonValueNode<bool> jB) writer.WriteBooleanValue(jB.Value);
        else if (value is IJsonValueNode<int> jInt32) writer.WriteNumberValue(jInt32.Value);
        else if (value is IJsonValueNode<uint> jUInt32) writer.WriteNumberValue(jUInt32.Value);
        else if (value is IJsonValueNode<ulong> jUInt64) writer.WriteNumberValue(jUInt64.Value);
        else if (value is IJsonValueNode<float> jFloat) writer.WriteNumberValue(jFloat.Value);
        else if (value is IJsonValueNode<short> jInt16) writer.WriteNumberValue(jInt16.Value);
        else if (value is IJsonValueNode<ushort> jUInt16) writer.WriteNumberValue(jUInt16.Value);
        else if (value is IJsonValueNode<decimal> jDecimal) writer.WriteNumberValue(jDecimal.Value);
        else writer.WriteNullValue();
    }
}

#if NET7_0_OR_GREATER || NET462_OR_GREATER
/// <summary>
/// JSON object node host converter.
/// </summary>
public sealed class JsonObjectHostConverter : JsonConverter<IJsonObjectHost>
{
    private readonly static JsonConverter<IJsonObjectHost> defaultConverter = (JsonConverter<IJsonObjectHost>)JsonSerializerOptions.Default.GetConverter(typeof(IJsonObjectHost));

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeof(IJsonObjectHost).IsAssignableFrom(typeToConvert);

    /// <inheritdoc />
    public override IJsonObjectHost Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonValues.SkipComments(ref reader);
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return default;
            case JsonTokenType.StartObject:
                if (typeToConvert.IsInterface) throw new NotSupportedException();
                var constructor = typeToConvert.GetConstructor(new[] { typeof(JsonObjectNode), typeof(JsonSerializerOptions) });
                if (constructor != null) return (IJsonObjectHost)constructor.Invoke(new object[] { new JsonObjectNode(ref reader), options });
                constructor = typeToConvert.GetConstructor(new[] { typeof(JsonObjectNode) });
                if (constructor != null) return (IJsonObjectHost)constructor.Invoke(new[] { new JsonObjectNode(ref reader) });
                return defaultConverter.Read(ref reader, typeToConvert, options);
            default:
                throw new JsonException($"The token type is {reader.TokenType} but expect JSON object or null for {typeToConvert}.");
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IJsonObjectHost value, JsonSerializerOptions options)
    {
        var json = value?.ToJson();
        if (json is null) writer.WriteNullValue();
        json.WriteTo(writer);
    }
}
#endif
