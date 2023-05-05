using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Trivial.Text;

namespace Trivial.Chemistry;

/// <summary>
/// Javascript ticks JSON number converter.
/// </summary>
sealed class ChemicalElementJsonConverter : JsonConverter<ChemicalElement>
{
    /// <inheritdoc />
    public override ChemicalElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        try
        {
            if (reader.TokenType == JsonTokenType.Number)
                return ChemicalElement.Get(reader.GetInt32());
        }
        catch (FormatException ex)
        {
            throw new JsonException("Expect a valid element symbol or the atomic numbers.", ex);
        }

        if (reader.TokenType == JsonTokenType.String)
            return ChemicalElement.Get(reader.GetString());
        if (reader.TokenType != JsonTokenType.StartObject) 
            throw new JsonException("The format is not correct.", new FormatException("The value should be a date time JSON token format."));
        var obj = new JsonObjectNode();
        obj.SetRange(ref reader);
        var z = obj.TryGetInt32Value("number") ?? obj.TryGetInt32Value("z");
        var s = obj.TryGetStringValue("symbol")?.Trim();
        var n = obj.TryGetStringValue("name_en")?.Trim() ?? obj.TryGetStringValue("name")?.Trim();
        var w = obj.TryGetDoubleValue("weight");
        ChemicalElement ele = null;
        if (z.HasValue && z != 0)
        {
            if (z.Value < 1) return null;
            ele = ChemicalElement.Get(z.Value);
        }
        else if (!string.IsNullOrEmpty(s))
        {
            ele = ChemicalElement.Get(s);
        }
        else if (!string.IsNullOrEmpty(n))
        {
            ele = ChemicalElement.Where(ele => n.Equals(ele.EnglishName, StringComparison.Ordinal)).FirstOrDefault();
            if (ele is null && n.Length < 4) ele = ChemicalElement.Get(n);
        }
        else
        {
            return null;
        }
            
        if (ele != null
            && (!z.HasValue || ele.AtomicNumber == z.Value)
            && (string.IsNullOrEmpty(s) || s.Equals(ele.Symbol, StringComparison.OrdinalIgnoreCase))
            && (double.IsNaN(w) || (ele.HasAtomicWeight && !double.IsNaN(w) && Math.Abs(ele.AtomicWeight - w) < 0.000000001)))
            return ele;

        if (!z.HasValue || z.Value < 1 || string.IsNullOrEmpty(s))
            return null;
        return string.IsNullOrEmpty(n)
            ? new ChemicalElement(z.Value, s, null, w)
            : new ChemicalElement(z.Value, s, n, true, w);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ChemicalElement value, JsonSerializerOptions options)
    {
        value.WriteTo(writer);
    }
}
