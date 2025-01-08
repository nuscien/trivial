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
    /// Initializes a new instance of the JsonNumberSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    public JsonNumberSchemaDescription(JsonObjectNode json)
        : this(json, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonNumberSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    /// <param name="skipExtendedProperties">true if only fill the properties without extended; otherwise, false.</param>
    protected JsonNumberSchemaDescription(JsonObjectNode json, bool skipExtendedProperties)
        : base(json, true)
    {
        if (json == null) return;
        DefaultValue = json.TryGetDoubleValue("default", false);
        ConstantValue = json.TryGetDoubleValue("const", false);
        MultipleOf = json.TryGetDoubleValue("multipleOf", false);
        var exclusiveMin = json.TryGetDoubleValue("exclusiveMinimum", false);
        var exclusiveMax = json.TryGetDoubleValue("exclusiveMaximum", false);
        var min = json.TryGetDoubleValue("minimum", false);
        var max = json.TryGetDoubleValue("maximum", false);
        if (double.IsNaN(exclusiveMax))
        {
            Max = max;
        }
        else if (double.IsNaN(max))
        {
            Max = exclusiveMax;
            IsMaxExcluded = true;
        }
        else if (max < exclusiveMax)
        {
            Max = max;
        }
        else
        {
            Max = exclusiveMax;
            IsMaxExcluded = true;
        }

        if (double.IsNaN(exclusiveMin))
        {
            Min = min;
        }
        else if (double.IsNaN(min))
        {
            Min = exclusiveMin;
            IsMinExcluded = true;
        }
        else if (min < exclusiveMin)
        {
            Min = min;
        }
        else
        {
            Min = exclusiveMin;
            IsMinExcluded = true;
        }

        if (skipExtendedProperties) return;
        ExtendedProperties.SetRange(json);
        JsonValues.RemoveJsonNodeSchemaDescriptionExtendedProperties(ExtendedProperties, false);
        ExtendedProperties.RemoveRange("default", "const", "multipleOf", "exclusiveMinimum", "exclusiveMaximum", "minimum", "maximum");
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public double DefaultValue { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the constant value.
    /// </summary>
    public double ConstantValue { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets a number that the value is multiple of.
    /// </summary>
    public double MultipleOf { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the maximum length of the string.
    /// </summary>
    public double Min { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the maximum length of the string.
    /// </summary>
    public double Max { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets a value indicating whether the minimum value is excluded.
    /// </summary>
    public bool IsMinExcluded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the maximum value is excluded.
    /// </summary>
    public bool IsMaxExcluded { get; set; }

    /// <summary>
    /// Tests if a value is in the scope.
    /// </summary>
    /// <param name="number">The number to test.</param>
    /// <returns>true if test succeeded; otherwise, false.</returns>
    public bool Test(double number)
    {
        if (double.IsNaN(number)) return false;
        if (!double.IsNaN(Min) && (IsMinExcluded ? number < Min : number <= Min)) return false;
        if (!double.IsNaN(Max) && (IsMaxExcluded ? number > Max : number >= Max)) return false;
        return true;
    }

    /// <summary>
    /// Fills the properties.
    /// </summary>
    /// <param name="node">The JSON object node.</param>
    protected override void FillProperties(JsonObjectNode node)
    {
        node.SetValueIfNotNull("default", DefaultValue);
        node.SetValueIfNotNull("const", ConstantValue);
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
    /// Initializes a new instance of the JsonIntegerSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    public JsonIntegerSchemaDescription(JsonObjectNode json)
        : this(json, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonIntegerSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    /// <param name="skipExtendedProperties">true if only fill the properties without extended; otherwise, false.</param>
    protected JsonIntegerSchemaDescription(JsonObjectNode json, bool skipExtendedProperties)
        : base(json, true)
    {
        if (json == null) return;
        DefaultValue = json.TryGetInt32Value("default");
        ConstantValue = json.TryGetInt32Value("const");
        MultipleOf = json.TryGetInt32Value("multipleOf");
        var exclusiveMin = json.TryGetDoubleValue("exclusiveMinimum", false);
        var exclusiveMax = json.TryGetDoubleValue("exclusiveMaximum", false);
        var min = json.TryGetDoubleValue("minimum", false);
        var max = json.TryGetDoubleValue("maximum", false);
        if (double.IsNaN(exclusiveMax))
        {
            Max = max;
        }
        else if (double.IsNaN(max))
        {
            Max = exclusiveMax;
            IsMaxExcluded = true;
        }
        else if (max < exclusiveMax)
        {
            Max = max;
        }
        else
        {
            Max = exclusiveMax;
            IsMaxExcluded = true;
        }

        if (double.IsNaN(exclusiveMin))
        {
            Min = min;
        }
        else if (double.IsNaN(min))
        {
            Min = exclusiveMin;
            IsMinExcluded = true;
        }
        else if (min < exclusiveMin)
        {
            Min = min;
        }
        else
        {
            Min = exclusiveMin;
            IsMinExcluded = true;
        }

        if (skipExtendedProperties) return;
        ExtendedProperties.SetRange(json);
        JsonValues.RemoveJsonNodeSchemaDescriptionExtendedProperties(ExtendedProperties, false);
        ExtendedProperties.RemoveRange("default", "const", "multipleOf", "exclusiveMinimum", "exclusiveMaximum", "minimum", "maximum");
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
    public double Min { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the maximum length of the string.
    /// </summary>
    public double Max { get; set; } = double.NaN;

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
