using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Trivial.Reflection;
using Trivial.Tasks;

namespace Trivial.Text;

/// <summary>
/// The JSON converter for enum. The output will be an integer value.
/// </summary>
public class JsonIntegerEnumCompatibleConverter : JsonConverterFactory, IJsonNodeSchemaCreationHandler<Type>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsEnum;

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => CreateConverter(typeof(JsonIntegerEnumCompatibleConverter<>), typeToConvert, options);

    internal static JsonConverter CreateConverter(Type converterType, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            var t = converterType.MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(t);
        }
        catch (InvalidOperationException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (InvalidCastException)
        {
        }

        throw new JsonException("Only integer enum supported.");
    }

    JsonNodeSchemaDescription IJsonNodeSchemaCreationHandler<Type>.Convert(Type type, JsonNodeSchemaDescription result, NodePathBreadcrumb<Type> breadcrumb)
        => result is JsonIntegerSchemaDescription desc ? desc : new JsonIntegerSchemaDescription();
}

/// <summary>
/// The JSON converter for enum. The output will be an integer value.
/// </summary>
public class JsonIntegerEnumCompatibleConverter<T> : JsonConverter<T>, IJsonNodeSchemaCreationHandler<Type>
    where T : struct, Enum
{
    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Read(ref reader, typeToConvert);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WriteNumberValue(System.Runtime.CompilerServices.Unsafe.As<T, int>(ref value));

    internal static T Read(ref Utf8JsonReader reader, Type typeToConvert)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return default;
            case JsonTokenType.Number:
                try
                {
                    return reader.TryGetInt32(out var i)
                        ? (T)Enum.ToObject(typeToConvert, i)
                        : (T)Enum.ToObject(typeToConvert, reader.GetDouble());
                }
                catch (ArgumentException)
                {
                    return default;
                }
                catch (InvalidCastException)
                {
                    return default;
                }
            case JsonTokenType.String:
                try
                {
                    var s = reader.GetString();
                    return Enum.TryParse<T>(s, true, out var r)
                        ? r
                        : (int.TryParse(s, out var i)
                        ? (T)Enum.ToObject(typeToConvert, i)
                        : (double.TryParse(s, out var d) ? (T)Enum.ToObject(typeToConvert, d) : default));
                }
                catch (InvalidCastException)
                {
                    return default;
                }
            default:
                throw new JsonException("Expect an integer or a string.");
        }
    }

    JsonNodeSchemaDescription IJsonNodeSchemaCreationHandler<Type>.Convert(Type type, JsonNodeSchemaDescription result, NodePathBreadcrumb<Type> breadcrumb)
        => result is JsonIntegerSchemaDescription desc ? desc : new JsonIntegerSchemaDescription();
}

/// <summary>
/// The JSON converter for enum. The output will be a string value.
/// </summary>
public class JsonStringEnumCompatibleConverter : JsonConverterFactory, IJsonNodeSchemaCreationHandler<Type>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsEnum;

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => JsonIntegerEnumCompatibleConverter.CreateConverter(typeof(JsonStringEnumCompatibleConverter<>), typeToConvert, options);

    JsonNodeSchemaDescription IJsonNodeSchemaCreationHandler<Type>.Convert(Type type, JsonNodeSchemaDescription result, NodePathBreadcrumb<Type> breadcrumb)
        => result is JsonStringSchemaDescription desc ? desc : new JsonStringSchemaDescription();
}

/// <summary>
/// The JSON converter for enum. The output will be a string value.
/// </summary>
public class JsonStringEnumCompatibleConverter<T> : JsonConverter<T>, IJsonNodeSchemaCreationHandler<Type>
    where T : struct, Enum
{
    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonIntegerEnumCompatibleConverter<T>.Read(ref reader, typeToConvert);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());

    JsonNodeSchemaDescription IJsonNodeSchemaCreationHandler<Type>.Convert(Type type, JsonNodeSchemaDescription result, NodePathBreadcrumb<Type> breadcrumb)
        => result is JsonStringSchemaDescription desc ? desc : new JsonStringSchemaDescription();
}
