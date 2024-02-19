using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The base description for JSON array node.
/// </summary>
public class JsonArraySchemaDescription : JsonNodeSchemaDescription
{
    /// <summary>
    /// Initializes a new instance of the JsonArraySchemaDescription class.
    /// </summary>
    public JsonArraySchemaDescription()
    {
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public JsonArrayNode DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the schema of item.
    /// </summary>
    public JsonNodeSchemaDescription DefaultItems { get; set; }

    /// <summary>
    /// Gets or sets the schema of item.
    /// </summary>
    public List<JsonNodeSchemaDescription> FixedItems { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether need disable additional items.
    /// </summary>
    public bool DisableAdditionalItems { get; set; }

    /// <summary>
    /// Gets or sets the schema of item that requires at least.
    /// </summary>
    public JsonNodeSchemaDescription ContainItem { get; set; }

    /// <summary>
    /// Gets or sets the minimum length of the string.
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the string.
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets value indicating whether the items are unique.
    /// </summary>
    public bool UniqueItems { get; set; }

    /// <inheritdoc />
    protected override void FillProperties(JsonObjectNode node)
    {
        node.SetValueIfNotNull("default", DefaultValue);
        node.SetValueIfNotNull("minItems", MinLength);
        node.SetValueIfNotNull("maxItems", MaxLength);
        if (FixedItems.Count > 0)
        {
            node.SetValueIfNotEmpty("items", FixedItems, JsonValues.ToJson);
            if (DisableAdditionalItems) node.SetValue("additionalItems", false);
            else node.SetValueIfNotNull("additionalItems", DefaultItems);
        }
        else
        {
            node.SetValueIfNotNull("items", DefaultItems);
            if (DisableAdditionalItems) node.SetValue("additionalItems", false);
        }

        node.SetValueIfNotNull("contains", ContainItem);
        if (UniqueItems) node.SetValue("uniqueItems", true);
    }
}
