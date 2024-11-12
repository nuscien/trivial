using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.Maths;

/// <summary>
/// Json angle struct converter.
/// </summary>
sealed class JsonAngleConverter : JsonConverter<Angle>
{
    /// <inheritdoc />
    public override Angle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonValues.SkipComments(ref reader);
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String => JsonNumberConverter.ParseNumber(ref reader, double.Parse),
            _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Angle value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Degrees);
    }
}

/// <summary>
/// Json angle model converter.
/// </summary>
sealed class JsonAngleModelConverter : JsonConverter<Angle.Model>
{
    /// <inheritdoc />
    public override Angle.Model Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonValues.SkipComments(ref reader);
        var num = reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String => JsonNumberConverter.ParseNullableNumber(ref reader, double.Parse),
            _ => throw new JsonException($"The token type is {reader.TokenType} but expect number.")
        };
        return num.HasValue ? new Angle.Model(num.Value) : null;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Angle.Model value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteNumberValue(value.Degrees);
    }
}
