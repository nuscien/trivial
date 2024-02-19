using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The base description for JSON schema node.
/// </summary>
public class JsonNodeSchemaDescription : IJsonObjectHost
{
    /// <summary>
    /// Initializes a new instance of the BaseJsonSchemaDescription class.
    /// </summary>
    public JsonNodeSchemaDescription()
    {
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the schema URI.
    /// </summary>
    public string Schema { get; set; }

    /// <summary>
    /// Gets or sets the reference path or identifier.
    /// </summary>
    public string ReferencePath { get; set; }

    /// <summary>
    /// Gets the definitions.
    /// </summary>
    public Dictionary<string, JsonNodeSchemaDescription> Subschemas { get; } = new();

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value is read-only.
    /// Especially used for PUT HTTP request.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value is write-only.
    /// Especially used for PUT HTTP request.
    /// </summary>
    public bool WriteOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value is deprecated.
    /// </summary>
    public bool IsDeprecated { get; set; }

    /// <summary>
    /// Gets or sets the comment.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Gets the enum items.
    /// </summary>
    public JsonArrayNode EnumItems { get; } = new();

    /// <summary>
    /// Gets the examples.
    /// </summary>
    public JsonArrayNode Examples { get; } = new();

    /// <summary>
    /// Gets the list that the expected matches all of.
    /// </summary>
    public List<JsonNodeSchemaDescription> MatchAllOf { get; } = new();

    /// <summary>
    /// Gets the list that the expected matches any of.
    /// </summary>
    public List<JsonNodeSchemaDescription> MatchAnyOf { get; } = new();

    /// <summary>
    /// Gets the list that the expected matches one of.
    /// </summary>
    public List<JsonNodeSchemaDescription> MatchOneOf { get; } = new();

    /// <summary>
    /// Gets or sets the schema that must not be.
    /// </summary>
    public JsonNodeSchemaDescription NotMatch { get; set; }

    /// <summary>
    /// Gets the extended properties.
    /// </summary>
    public JsonObjectNode ExtendedProperties { get; } = new();

    /// <summary>
    /// Gets the definitions.
    /// </summary>
    public Dictionary<string, JsonNodeSchemaDescription> Definitions { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether need skip overriding the existed properties by the extended.
    /// </summary>
    public bool SkipDuplicatedExtendedProperties { get; set; }

    /// <summary>
    /// Fills the properties.
    /// </summary>
    /// <param name="node">The JSON object node.</param>
    protected virtual void FillProperties(JsonObjectNode node)
    {
    }

    /// <summary>
    /// Gets the type defined.
    /// </summary>
    /// <returns>The type string.</returns>
    public string GetValueType()
    {
        if (this is JsonObjectSchemaDescription) return "object";
        if (this is JsonStringSchemaDescription) return "string";
        if (this is JsonNumberSchemaDescription) return "number";
        if (this is JsonIntegerSchemaDescription) return "integer";
        if (this is JsonBooleanSchemaDescription) return "boolean";
        if (this is JsonArraySchemaDescription) return "array";
        if (this is JsonNullSchemaDescription) return "null";
        return null;
    }

    /// <summary>
    /// The host for JSON object node.
    /// </summary>
    public JsonObjectNode ToJson()
        => ToJsonInternal(true);

    /// <summary>
    /// The host for JSON object node.
    /// </summary>
    internal JsonObjectNode ToJsonInternal(bool exportType)
    {
        var node = new JsonObjectNode();
        if (!string.IsNullOrEmpty(Id)) node.Id = Id;
        if (!string.IsNullOrEmpty(Schema)) node.Schema = Schema;
        if (!string.IsNullOrEmpty(ReferencePath)) node.SetValue("$ref", ReferencePath);
        if (Subschemas.Count > 0) node.SetValueIfNotNull("$defs", JsonValues.ToJson(Subschemas));
        if (exportType) node.SetValueIfNotNull("type", GetValueType());
        node.SetValueIfNotNull("title", Title);
        node.SetValueIfNotNull("description", Description);
        if (IsDeprecated) node.SetValue("deprecated", IsDeprecated);
        if (ReadOnly) node.SetValue("readOnly", ReadOnly);
        if (WriteOnly) node.SetValue("writeOnly", WriteOnly);
        node.SetValueIfNotEmpty("$comment", Comment);
        node.SetValueIfNotEmpty("allOf", MatchAllOf, JsonValues.ToJson);
        node.SetValueIfNotEmpty("anyOf", MatchAnyOf, JsonValues.ToJson);
        node.SetValueIfNotEmpty("oneOf", MatchOneOf, JsonValues.ToJson);
        node.SetValueIfNotNull("not", NotMatch);
        node.SetValueIfNotNull("enum", EnumItems);
        node.SetValueIfNotNull("examples", Examples);
        if (Definitions.Count > 0) node.SetValueIfNotNull("definitions", JsonValues.ToJson(Definitions));
        FillProperties(node);
        node.SetRange(ExtendedProperties, SkipDuplicatedExtendedProperties);
        return node;
    }
}

/// <summary>
/// The base description for JSON number node.
/// </summary>
public class JsonBooleanSchemaDescription : JsonNodeSchemaDescription
{
    /// <summary>
    /// Initializes a new instance of the JsonBooleanSchemaDescription class.
    /// </summary>
    public JsonBooleanSchemaDescription()
    {
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public bool? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the constant value.
    /// </summary>
    public bool? ConstantValue { get; set; }

    /// <summary>
    /// Fills the properties.
    /// </summary>
    /// <param name="node">The JSON object node.</param>
    protected override void FillProperties(JsonObjectNode node)
    {
        node.SetValueIfNotNull("default", DefaultValue);
        node.SetValueIfNotNull("const", DefaultValue);
    }
}

/// <summary>
/// The base description for JSON number node.
/// </summary>
public class JsonNullSchemaDescription : JsonNodeSchemaDescription
{
    /// <summary>
    /// Initializes a new instance of the JsonNullSchemaDescription class.
    /// </summary>
    public JsonNullSchemaDescription()
    {
    }

    /// <summary>
    /// Fills the properties.
    /// </summary>
    /// <param name="node">The JSON object node.</param>
    protected override void FillProperties(JsonObjectNode node)
    {
    }
}
