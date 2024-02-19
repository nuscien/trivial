using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The handler interface of JSON node schema description.
/// </summary>
public interface IJsonNodeSchemaDescriptionHandler
{
    /// <summary>
    /// Occurs on the properties is filling when generate JSON object.
    /// </summary>
    /// <param name="description">The JSON schema description.</param>
    /// <param name="node">The JSON object to fill properties.</param>
    void OnPropertiesFilling(BaseJsonNodeSchemaDescription description, JsonObjectNode node);

    /// <summary>
    /// Occurs on the properties is filling when generate JSON object.
    /// </summary>
    /// <param name="description">The JSON schema description.</param>
    /// <param name="node">The JSON object to fill properties.</param>
    void OnPropertiesFilled(BaseJsonNodeSchemaDescription description, JsonObjectNode node);
}

internal class InternalEmptyJsonNodeSchemaDescriptionHandler : IJsonNodeSchemaDescriptionHandler
{
    public static IJsonNodeSchemaDescriptionHandler Instance { get; } = new InternalEmptyJsonNodeSchemaDescriptionHandler();

    void IJsonNodeSchemaDescriptionHandler.OnPropertiesFilling(BaseJsonNodeSchemaDescription description, JsonObjectNode node)
    {
    }

    void IJsonNodeSchemaDescriptionHandler.OnPropertiesFilled(BaseJsonNodeSchemaDescription description, JsonObjectNode node)
    {
    }
}

internal class InternalJsonNodeSchemaDescriptionHandler : IJsonNodeSchemaDescriptionHandler
{
    public static IJsonNodeSchemaDescriptionHandler Instance { get; } = new InternalJsonNodeSchemaDescriptionHandler();

    void IJsonNodeSchemaDescriptionHandler.OnPropertiesFilling(BaseJsonNodeSchemaDescription description, JsonObjectNode node)
    {
        if (description is not JsonNodeSchemaDescription d) return;
        if (!string.IsNullOrEmpty(d.ReferencePath)) node.SetValue("$ref", d.ReferencePath);
        if (d.Subschemas.Count > 0) node.SetValueIfNotNull("$defs", JsonValues.ToJson(d.Subschemas));
        node.SetValueIfNotNull("title", d.Title);
        if (d.IsDeprecated) node.SetValue("deprecated", d.IsDeprecated);
        if (d.ReadOnly) node.SetValue("readOnly", d.ReadOnly);
        if (d.WriteOnly) node.SetValue("writeOnly", d.WriteOnly);
        node.SetValueIfNotEmpty("allOf", d.MatchAllOf, JsonValues.ToJson);
        node.SetValueIfNotEmpty("anyOf", d.MatchAnyOf, JsonValues.ToJson);
        node.SetValueIfNotEmpty("oneOf", d.MatchOneOf, JsonValues.ToJson);
        node.SetValueIfNotNull("not", d.NotMatch);
        node.SetValueIfNotNull("enum", d.EnumItems);
        if (d.DefinitionsNode.Count > 0) node.SetValueIfNotNull("definitions", JsonValues.ToJson(d.DefinitionsNode));
    }

    void IJsonNodeSchemaDescriptionHandler.OnPropertiesFilled(BaseJsonNodeSchemaDescription description, JsonObjectNode node)
    {
    }
}

/// <summary>
/// The base handler of JSON node schema description.
/// </summary>
public abstract class BaseJsonNodeSchemaDescriptionHandler<T> : IJsonNodeSchemaDescriptionHandler where T : BaseJsonNodeSchemaDescription
{
    /// <summary>
    /// Occurs on the properties is filling when generate JSON object.
    /// </summary>
    /// <param name="description">The JSON schema description.</param>
    /// <param name="node">The JSON object to fill properties.</param>
    public virtual void OnPropertiesFilling(T description, JsonObjectNode node)
    {
    }

    /// <summary>
    /// Occurs on the properties is filling when generate JSON object.
    /// </summary>
    /// <param name="description">The JSON schema description.</param>
    /// <param name="node">The JSON object to fill properties.</param>
    public virtual void OnPropertiesFilled(T description, JsonObjectNode node)
    {
    }

    /// <summary>
    /// Occurs on the properties is filling when generate JSON object.
    /// </summary>
    /// <param name="description">The JSON schema description.</param>
    /// <param name="node">The JSON object to fill properties.</param>
    void IJsonNodeSchemaDescriptionHandler.OnPropertiesFilling(BaseJsonNodeSchemaDescription description, JsonObjectNode node)
    {
        if (description is not T d) return;
        OnPropertiesFilled(d, node);
    }

    /// <summary>
    /// Occurs on the properties is filling when generate JSON object.
    /// </summary>
    /// <param name="description">The JSON schema description.</param>
    /// <param name="node">The JSON object to fill properties.</param>
    void IJsonNodeSchemaDescriptionHandler.OnPropertiesFilled(BaseJsonNodeSchemaDescription description, JsonObjectNode node)
    {
        if (description is not T d) return;
        OnPropertiesFilled(d, node);
    }
}
