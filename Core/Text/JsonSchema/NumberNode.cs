using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The base description for JSON number node.
/// </summary>
public class JsonNumberSchemaDescription : JsonNodeSchemaDescription
{
    /// <summary>
    /// Initializes a new instance of the JsonNumberSchemaDescription class.
    /// </summary>
    public JsonNumberSchemaDescription()
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonNumberSchemaDescription class.
    /// </summary>
    /// <param name="copy">The schema to copy.</param>
    public JsonNumberSchemaDescription(JsonIntegerSchemaDescription copy) : base(copy)
    {
        DefaultValue = JsonValues.ToDouble(copy.DefaultValue);
        ConstantValue = JsonValues.ToDouble(copy.ConstantValue);
        MultipleOf = JsonValues.ToDouble(copy.MultipleOf);
        Min = copy.Min;
        Max = copy.Max;
        IsMinExcluded = copy.IsMinExcluded;
        IsMaxExcluded = copy.IsMaxExcluded;
    }

    /// <summary>
    /// Initializes a new instance of the JsonNumberSchemaDescription class.
    /// </summary>
    /// <param name="copy">The schema to copy.</param>
    public JsonNumberSchemaDescription(JsonNumberSchemaDescription copy) : base(copy)
    {
        DefaultValue = copy.DefaultValue;
        ConstantValue = copy.ConstantValue;
        MultipleOf = copy.MultipleOf;
        Min = copy.Min;
        Max = copy.Max;
        IsMinExcluded = copy.IsMinExcluded;
        IsMaxExcluded = copy.IsMaxExcluded;
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public double? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the constant value.
    /// </summary>
    public double? ConstantValue { get; set; }

    /// <summary>
    /// Gets or sets a number that the value is multiple of.
    /// </summary>
    public double? MultipleOf { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the string.
    /// </summary>
    public double? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the string.
    /// </summary>
    public double? Max { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the minimum value is excluded.
    /// </summary>
    public bool IsMinExcluded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the maximum value is excluded.
    /// </summary>
    public bool IsMaxExcluded { get; set; }

    /// <summary>
    /// Fills the properties.
    /// </summary>
    /// <param name="node">The JSON object node.</param>
    protected override void FillProperties(JsonObjectNode node)
    {
        node.SetValueIfNotNull("default", DefaultValue);
        node.SetValueIfNotNull("const", DefaultValue);
        node.SetValueIfNotNull("multipleOf", MultipleOf);
        node.SetValueIfNotNull(IsMinExcluded ? "exclusiveMinimum" : "minimum", Min);
        node.SetValueIfNotNull(IsMaxExcluded ? "exclusiveMaximum" : "maximum", Max);
    }
}

/// <summary>
/// The base description for JSON number node.
/// </summary>
public class JsonIntegerSchemaDescription : JsonNodeSchemaDescription
{
    /// <summary>
    /// Initializes a new instance of the JsonIntegerSchemaDescription class.
    /// </summary>
    public JsonIntegerSchemaDescription()
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonIntegerSchemaDescription class.
    /// </summary>
    /// <param name="copy">The schema to copy.</param>
    public JsonIntegerSchemaDescription(JsonIntegerSchemaDescription copy) : base(copy)
    {
        DefaultValue = copy.DefaultValue;
        ConstantValue = copy.ConstantValue;
        MultipleOf = copy.MultipleOf;
        Min = copy.Min;
        Max = copy.Max;
        IsMinExcluded = copy.IsMinExcluded;
        IsMaxExcluded = copy.IsMaxExcluded;
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public int? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the constant value.
    /// </summary>
    public int? ConstantValue { get; set; }

    /// <summary>
    /// Gets or sets a number that the value is multiple of.
    /// </summary>
    public int? MultipleOf { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the string.
    /// </summary>
    public double? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the string.
    /// </summary>
    public double? Max { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the minimum value is excluded.
    /// </summary>
    public bool IsMinExcluded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the maximum value is excluded.
    /// </summary>
    public bool IsMaxExcluded { get; set; }

    /// <inheritdoc />
    protected override void FillProperties(JsonObjectNode node)
    {
        node.SetValueIfNotNull("default", DefaultValue);
        node.SetValueIfNotNull("const", DefaultValue);
        node.SetValueIfNotNull("multipleOf", MultipleOf);
        node.SetValueIfNotNull(IsMinExcluded ? "exclusiveMinimum" : "minimum", Min);
        node.SetValueIfNotNull(IsMaxExcluded ? "exclusiveMaximum" : "maximum", Max);
    }
}
