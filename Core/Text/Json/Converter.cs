using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Trivial.Reflection;

namespace Trivial.Text;

/// <summary>
/// JSON string collection and json array converter.
/// </summary>
public class JsonStringListConverter : JsonConverter<IEnumerable<string>>, IJsonNodeSchemaCreationHandler<Type>
{
    private readonly char[] chars;
    private readonly bool trim;

    /// <summary>
    /// JSON string collection with white space and new line separated.
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
    /// JSON string collection with vertical bar separated.
    /// </summary>
    public sealed class VerticalBarSeparatedConverter : JsonStringListConverter
    {
        /// <summary>
        /// Initializes a new instance of the VerticalBarSeparatedConverter class.
        /// </summary>
        public VerticalBarSeparatedConverter() : base('|')
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
    /// <param name="needTrim">true if need trim each string item and ignore empty; otherwise, false. Default value is true.</param>
    public JsonStringListConverter(char split, bool needTrim = true)
    {
        chars = new[] { split };
        trim = needTrim;
    }

    /// <summary>
    /// Initializes a new instance of the JsonStringListConverter class.
    /// </summary>
    /// <param name="split">The split characters.</param>
    /// <param name="needTrim">true if need trim each string item and ignore empty; otherwise, false. Default value is true.</param>
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
                while (reader.TokenType == JsonTokenType.Comment || reader.TokenType == JsonTokenType.None)
                {
                    reader.Read();
                }

                if (!TryGetString(ref reader, out var value))
                {
                    throw new JsonException($"The token type is {reader.TokenType} but expect string or null.");
                }

                if (trim)
                {
                    if (value == null) continue;
                    value = value.Trim();
                    if (value.Length == 0) continue;
                }

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
#if NETFRAMEWORK
            var str = string.Join(new string(chars[0], 1), arr);
#else
            var str = string.Join(chars[0], arr);
#endif
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

    JsonNodeSchemaDescription IJsonNodeSchemaCreationHandler<Type>.Convert(Type type, JsonNodeSchemaDescription result, NodePathBreadcrumb<Type> breadcrumb)
        => result is JsonArraySchemaDescription desc ? desc : new JsonArraySchemaDescription
        {
            DefaultItems = new JsonStringSchemaDescription()
        };

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

        if (col.Count == 0)
        {
            try
            {
                return (IEnumerable<string>)Activator.CreateInstance(typeToConvert);
            }
            catch (MemberAccessException)
            {
            }
        }

        try
        {
            return (IEnumerable<string>)Activator.CreateInstance(typeToConvert, new[] { col });
        }
        catch (MemberAccessException ex)
        {
            if (!typeof(ICollection<string>).IsAssignableFrom(typeToConvert))
                throw new JsonException("The enumerable type is not supported.", ex);

            try
            {
                var c = (ICollection<string>)Activator.CreateInstance(typeToConvert);
                if (c.IsReadOnly) throw new JsonException("Cannot add items because the collection is read-only.", new NotSupportedException("The collection is read-only."));
                foreach (var item in col)
                {
                    c.Add(item);
                }

                return c;
            }
            catch (MemberAccessException ex2)
            {
                throw new JsonException("Cannot create the enumerable instance by no argument.", ex2);
            }
        }
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
/// JSON object node and JSON array node converter.
/// </summary>
public sealed class JsonObjectNodeConverter : JsonConverter<IJsonValueNode>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IJsonValueNode).IsAssignableFrom(typeToConvert);
    }

    /// <inheritdoc />
    public override IJsonValueNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                if (typeToConvert == typeof(JsonObjectNode) || typeToConvert == typeof(JsonArrayNode)) return null;
                if (typeToConvert == typeof(JsonStringNode)) return new JsonStringNode(null as string);
                if (typeToConvert == typeof(IJsonValueNode)) return JsonValues.Null;
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
                if (typeToConvert == typeof(JsonStringNode) || typeToConvert == typeof(IJsonStringNode) || typeToConvert == typeof(IJsonValueNode<string>)) return new JsonStringNode(str);
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

/// <summary>
/// JSON object node schema description converter.
/// </summary>
public sealed class JsonNodeSchemaConverter : JsonConverter<JsonNodeSchemaDescription>
{
    /// <inheritdoc />
    public override JsonNodeSchemaDescription Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
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

#if NET7_0_OR_GREATER || NET462_OR_GREATER
/// <summary>
/// JSON object node host converter.
/// </summary>
public sealed class JsonObjectHostConverter : JsonConverter<IJsonObjectHost>
{
    private readonly static JsonConverter<IJsonObjectHost> defaultConverter = (JsonConverter<IJsonObjectHost>)JsonSerializerOptions.Default.GetConverter(typeof(IJsonObjectHost));

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IJsonObjectHost).IsAssignableFrom(typeToConvert);
    }

    /// <inheritdoc />
    public override IJsonObjectHost Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
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
