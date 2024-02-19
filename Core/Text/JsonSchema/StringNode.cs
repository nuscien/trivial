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

    /// <summary>
    /// Fills the properties.
    /// </summary>
    /// <param name="node">The JSON object node.</param>
    protected override void FillProperties(JsonObjectNode node)
    {
        node.SetValueIfNotNull("default", DefaultValue);
        node.SetValueIfNotNull("const", DefaultValue);
        node.SetValueIfNotEmpty("contentMediaType", ContentType);
        node.SetValueIfNotEmpty("contentEncoding", ContentEncoding);
        node.SetValueIfNotNull("minLength", MinLength);
        node.SetValueIfNotNull("maxLength", MaxLength);
        node.SetValueIfNotEmpty("pattern", Pattern);
        node.SetValueIfNotEmpty("format", Format);
    }
}
