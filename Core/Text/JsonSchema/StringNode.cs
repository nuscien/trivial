using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The base description for JSON string node.
/// </summary>
public class JsonStringSchemaDescription : JsonNodeSchemaDescription
{
    /// <summary>
    /// Initializes a new instance of the JsonStringSchemaDescription class.
    /// </summary>
    public JsonStringSchemaDescription()
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonStringSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    public JsonStringSchemaDescription(JsonObjectNode json)
        : this(json, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonStringSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    /// <param name="skipExtendedProperties">true if only fill the properties without extended; otherwise, false.</param>
    protected JsonStringSchemaDescription(JsonObjectNode json, bool skipExtendedProperties)
        : base(json, true)
    {
        if (json == null) return;
        DefaultValue = json.TryGetStringValue("default");
        ConstantValue = json.TryGetStringValue("const");
        ContentType = json.TryGetStringValue("contentMediaType");
        ContentEncoding = json.TryGetStringValue("contentEncoding");
        MinLength = json.TryGetInt32Value("minLength");
        MaxLength = json.TryGetInt32Value("maxLength");
        Pattern = json.TryGetStringValue("pattern");
        Format = json.TryGetStringValue("format");
        if (skipExtendedProperties) return;
        ExtendedProperties.SetRange(json);
        JsonValues.RemoveJsonNodeSchemaDescriptionExtendedProperties(ExtendedProperties, false);
        ExtendedProperties.Remove("default");
        ExtendedProperties.Remove("const");
        ExtendedProperties.Remove("contentMediaType");
        ExtendedProperties.Remove("contentEncoding");
        ExtendedProperties.Remove("minLength");
        ExtendedProperties.Remove("maxLength");
        ExtendedProperties.Remove("pattern");
        ExtendedProperties.Remove("format");
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public string DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the constant value.
    /// </summary>
    public string ConstantValue { get; set; }

    /// <summary>
    /// Gets or sets the content type (MIME).
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the content encoding type.
    /// </summary>
    public string ContentEncoding { get; set; }

    /// <summary>
    /// Gets or sets the minimum length of the string.
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the string.
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the pattern of the string.
    /// </summary>
    public string Pattern { get; set; }

    /// <summary>
    /// Gets or sets the format of the string.
    /// </summary>
    public string Format { get; set; }

    /// <inheritdoc />
    protected override void FillProperties(JsonObjectNode node)
    {
        node.SetValueIfNotNull("default", DefaultValue);
        node.SetValueIfNotNull("const", ConstantValue);
        node.SetValueIfNotEmpty("contentMediaType", ContentType);
        node.SetValueIfNotEmpty("contentEncoding", ContentEncoding);
        node.SetValueIfNotNull("minLength", MinLength);
        node.SetValueIfNotNull("maxLength", MaxLength);
        node.SetValueIfNotEmpty("pattern", Pattern);
        node.SetValueIfNotEmpty("format", Format);
    }
}
