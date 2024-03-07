using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The base description for JSON schema node.
/// </summary>
public abstract class BaseJsonNodeSchemaDescription : IJsonObjectHost
{
    private readonly IJsonNodeSchemaDescriptionHandler handler;

    /// <summary>
    /// Initializes a new instance of the BaseJsonNodeSchemaDescription class.
    /// </summary>
    public BaseJsonNodeSchemaDescription()
    {
        handler = InternalEmptyJsonNodeSchemaDescriptionHandler.Instance;
    }

    /// <summary>
    /// Initializes a new instance of the BaseJsonNodeSchemaDescription class.
    /// </summary>
    /// <param name="handler">The handler to monitor the JSON schema convertion.</param>
    public BaseJsonNodeSchemaDescription(IJsonNodeSchemaDescriptionHandler handler)
    {
        this.handler = handler ?? InternalEmptyJsonNodeSchemaDescriptionHandler.Instance;
    }

    /// <summary>
    /// Initializes a new instance of the BaseJsonNodeSchemaDescription class.
    /// </summary>
    /// <param name="copy">The schema to copy.</param>
    /// <param name="handler">The handler to monitor the JSON schema convertion.</param>
    public BaseJsonNodeSchemaDescription(BaseJsonNodeSchemaDescription copy, IJsonNodeSchemaDescriptionHandler handler = null)
    {
        this.handler = handler ?? InternalEmptyJsonNodeSchemaDescriptionHandler.Instance;
        if (copy == null) return;
        Id = copy.Id;
        Schema = copy.Schema;
        Description = copy.Description;
        Examples = copy.Examples;
        ExtendedProperties = copy.ExtendedProperties?.Clone();
        SkipDuplicatedExtendedProperties = copy.SkipDuplicatedExtendedProperties;
    }

    /// <summary>
    /// Initializes a new instance of the BaseJsonNodeSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    /// <param name="skipExtendedProperties">true if only fill the properties without extended; otherwise, false.</param>
    /// <param name="handler">The handler to monitor the JSON schema convertion.</param>
    protected BaseJsonNodeSchemaDescription(JsonObjectNode json, bool skipExtendedProperties, IJsonNodeSchemaDescriptionHandler handler = null)
    {
        this.handler = handler ?? InternalEmptyJsonNodeSchemaDescriptionHandler.Instance;
        if (json == null) return;
        Id = json.Id;
        Schema = json.Schema;
        ReferencePath = json.TryGetStringTrimmedValue("$ref", true);
        Description = json.TryGetStringValue("description");
        Comment = json.CommentValue;
        Examples.AddRange(json.TryGetArrayValue("examples"));
        if (skipExtendedProperties) return;
        ExtendedProperties.SetRange(json);
        JsonValues.RemoveJsonNodeSchemaDescriptionExtendedProperties(ExtendedProperties, true);
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
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the comment.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Gets the examples.
    /// </summary>
    public JsonArrayNode Examples { get; } = new();

    /// <summary>
    /// Gets the extended properties.
    /// </summary>
    public JsonObjectNode ExtendedProperties { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether need skip overriding the existed properties by the extended.
    /// </summary>
    public bool SkipDuplicatedExtendedProperties { get; set; }

    /// <summary>
    /// Gets or sets an additional tag object.
    /// </summary>
    public object Tag { get; set; }

    /// <summary>
    /// Fills the properties.
    /// </summary>
    /// <param name="node">The JSON object node to fill properties.</param>
    protected abstract void FillProperties(JsonObjectNode node);

    /// <summary>
    /// Gets the type defined.
    /// </summary>
    /// <returns>The type string.</returns>
    public abstract string GetValueType();

    /// <summary>
    /// Converts to YAML format string.
    /// </summary>
    public string ToYamlString()
        => ToJson().ToYamlString();

    /// <summary>
    /// The host for JSON object node.
    /// </summary>
    public JsonObjectNode ToJson()
    {
        var node = new JsonObjectNode();
        if (!string.IsNullOrEmpty(Id)) node.Id = Id;
        if (!string.IsNullOrEmpty(Schema)) node.Schema = Schema;
        if (!string.IsNullOrEmpty(ReferencePath)) node.SetValue("$ref", ReferencePath);
        node.SetValueIfNotEmpty("type", GetValueType());
        node.SetValueIfNotEmpty("description", Description);
        node.SetValueIfNotEmpty("examples", Examples);
        node.CommentValue = Comment;
        handler.OnPropertiesFilling(this, node);
        FillProperties(node);
        node.SetRange(ExtendedProperties, SkipDuplicatedExtendedProperties);
        handler.OnPropertiesFilled(this, node);
        return node;
    }
}

/// <summary>
/// The base description for JSON schema node.
/// </summary>
[JsonConverter(typeof(JsonNodeSchemaConverter))]
public class JsonNodeSchemaDescription : BaseJsonNodeSchemaDescription
{
    /// <summary>
    /// Initializes a new instance of the BaseJsonSchemaDescription class.
    /// </summary>
    public JsonNodeSchemaDescription() : base(InternalJsonNodeSchemaDescriptionHandler.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseJsonSchemaDescription class.
    /// </summary>
    /// <param name="copy">The schema to copy.</param>
    public JsonNodeSchemaDescription(JsonNodeSchemaDescription copy) : base(copy, InternalJsonNodeSchemaDescriptionHandler.Instance)
    {
        if (copy == null) return;
        ReferencePath = copy.ReferencePath;
        Subschemas = new(copy.Subschemas);
        Title = copy.Title;
        ReadOnly = copy.ReadOnly;
        WriteOnly = copy.WriteOnly;
        IsDeprecated = copy.IsDeprecated;
        EnumItems = copy.EnumItems?.Clone();
        MatchAllOf = new(copy.MatchAllOf);
        MatchAnyOf = new(copy.MatchAnyOf);
        MatchOneOf = new(copy.MatchOneOf);
        NotMatch = copy.NotMatch;
        DefinitionsNode = new(copy.DefinitionsNode);
    }

    /// <summary>
    /// Initializes a new instance of the JsonNodeSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    internal JsonNodeSchemaDescription(JsonObjectNode json)
        : this(json, false)
    {
        if (json == null) return;
        ExtendedProperties.SetValueIfNotEmpty("type", json.TryGetStringValue("type"));
    }

    /// <summary>
    /// Initializes a new instance of the JsonNodeSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    /// <param name="skipExtendedProperties">true if only fill the properties without extended; otherwise, false.</param>
    protected JsonNodeSchemaDescription(JsonObjectNode json, bool skipExtendedProperties)
        : base(json, true, InternalJsonNodeSchemaDescriptionHandler.Instance)
    {
        if (json == null) return;
        Title = json.TryGetStringTrimmedValue("title", true);
        IsDeprecated = json.TryGetBooleanValue("deprecated") ?? false;
        ReadOnly = json.TryGetBooleanValue("readOnly") ?? false;
        WriteOnly = json.TryGetBooleanValue("writeOnly") ?? false;
        JsonValues.FillObjectSchema(MatchAllOf, json, "allOf");
        JsonValues.FillObjectSchema(MatchAnyOf, json, "anyOf");
        JsonValues.FillObjectSchema(MatchOneOf, json, "oneOf");
        NotMatch = JsonValues.ConvertToObjectSchema(json.TryGetObjectValue("not"));
        var arr = json.TryGetArrayValue("enum");
        if (arr != null) EnumItems.AddRange(arr);
        JsonValues.FillObjectSchema(DefinitionsNode, json, "definitions");
        if (skipExtendedProperties) return;
        ExtendedProperties.SetRange(json);
        JsonValues.RemoveJsonNodeSchemaDescriptionExtendedProperties(ExtendedProperties, false);
    }

    /// <summary>
    /// Gets the definitions.
    /// </summary>
    public Dictionary<string, JsonNodeSchemaDescription> Subschemas { get; } = new();

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title { get; set; }

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
    /// Gets the enum items.
    /// </summary>
    public JsonArrayNode EnumItems { get; } = new();

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
    /// Gets the definitions.
    /// </summary>
    public Dictionary<string, JsonNodeSchemaDescription> DefinitionsNode { get; } = new();

    /// <summary>
    /// Converts a JSON schema.
    /// </summary>
    /// <param name="value">The JSON object.</param>
    /// <returns>The JSON schema description.</returns>
    public static implicit operator JsonNodeSchemaDescription(JsonObjectNode value)
        => JsonValues.ConvertToObjectSchema(value);

    /// <inheritdoc />
    protected override void FillProperties(JsonObjectNode node)
    {
    }

    /// <summary>
    /// Gets the type defined.
    /// </summary>
    /// <returns>The type string.</returns>
    public override string GetValueType()
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
    internal JsonObjectNode ToJsonInternal(bool exportType)
    {
        var node = ToJson();
        if (exportType) return node;
        node.Remove("type");
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
    /// Initializes a new instance of the JsonBooleanSchemaDescription class.
    /// </summary>
    /// <param name="copy">The schema to copy.</param>
    public JsonBooleanSchemaDescription(JsonBooleanSchemaDescription copy) : base(copy)
    {
        if (copy == null) return;
        DefaultValue = copy.DefaultValue;
        ConstantValue = copy.ConstantValue;
    }

    /// <summary>
    /// Initializes a new instance of the JsonBooleanSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    public JsonBooleanSchemaDescription(JsonObjectNode json)
        : this(json, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonBooleanSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    /// <param name="skipExtendedProperties">true if only fill the properties without extended; otherwise, false.</param>
    protected JsonBooleanSchemaDescription(JsonObjectNode json, bool skipExtendedProperties)
        : base(json, true)
    {
        if (json == null) return;
        DefaultValue = json.TryGetBooleanValue("default");
        ConstantValue = json.TryGetBooleanValue("const");
        if (skipExtendedProperties) return;
        ExtendedProperties.SetRange(json);
        JsonValues.RemoveJsonNodeSchemaDescriptionExtendedProperties(ExtendedProperties, false);
        ExtendedProperties.Remove("default");
        ExtendedProperties.Remove("const");
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public bool? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the constant value.
    /// </summary>
    public bool? ConstantValue { get; set; }

    /// <inheritdoc />
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
    /// Initializes a new instance of the JsonNullSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    internal JsonNullSchemaDescription(JsonObjectNode json)
        : base(json, false)
    {
    }

    /// <inheritdoc />
    protected override void FillProperties(JsonObjectNode node)
    {
    }
}
