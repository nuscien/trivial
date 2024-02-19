using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The base description for JSON object node.
/// </summary>
public class JsonObjectSchemaDescription : JsonNodeSchemaDescription
{
    /// <summary>
    /// Initializes a new instance of the JsonObjectSchemaDescription class.
    /// </summary>
    public JsonObjectSchemaDescription()
    {
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public JsonObjectNode DefaultValue { get; set; }

    /// <summary>
    /// Gets the description of properties.
    /// The dictionary keys are the property names.
    /// </summary>
    public Dictionary<string, JsonNodeSchemaDescription> Properties { get; } = new();

    /// <summary>
    /// Gets the pattern description properties.
    /// The dictionary keys are the property name patterns.
    /// </summary>
    public Dictionary<string, JsonNodeSchemaDescription> PatternProperties { get; } = new();
    
    /// <summary>
    /// Gets or sets the minimum count of property.
    /// </summary>
    public int? MinProperties { get; set; }

    /// <summary>
    /// Gets or sets the maximum count of property.
    /// </summary>
    public int? MaxProperties { get; set; }

    /// <summary>
    /// Gets the name list of required property.
    /// </summary>
    public List<string> RequiredPropertyNames { get; } = new();

    /// <summary>
    /// Gets the additional properties description.
    /// </summary>
    public JsonNodeSchemaDescription AdditionalProperties { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether disable additonal properties.
    /// </summary>
    public bool DisableAdditionalProperties { get; set; }

    /// <summary>
    /// Gets or sets the policy of each property name.
    /// </summary>
    public JsonStringSchemaDescription PropertyNamePolicy { get; set; }

    /// <summary>
    /// Gets the name list of dependent required property.
    /// </summary>
    public Dictionary<string, List<string>> DependentRequiredPropertyNames { get; } = new();

    /// <summary>
    /// Gets the policy of dependent required property.
    /// The dictionary keys are the property names.
    /// </summary>
    public Dictionary<string, JsonObjectSchemaDescription> DependentPropertyPolicy { get; } = new();

    /// <summary>
    /// Gets or sets the if policy in if-then-else condition.
    /// </summary>
    public JsonObjectSchemaDescription IfPolicy { get; set; }

    /// <summary>
    /// Gets or sets the then policy in if-then-else condition.
    /// </summary>
    public JsonObjectSchemaDescription ThenPolicy { get; set; }

    /// <summary>
    /// Gets or sets the else policy in if-then-else condition.
    /// </summary>
    public JsonObjectSchemaDescription ElsePolicy { get; set; }

    /// <inheritdoc />
    protected override void FillProperties(JsonObjectNode node)
    {
        node.SetValueIfNotNull("default", DefaultValue);
        if (Properties.Count > 0) node.SetValue("properties", JsonValues.ToJson(Properties));
        if (PatternProperties.Count > 0) node.SetValue("patternProperties", JsonValues.ToJson(PatternProperties));
        if (RequiredPropertyNames.Count > 0) node.SetValue("required", RequiredPropertyNames);
        if (DependentRequiredPropertyNames.Count > 0)
        {
            var json = new JsonObjectNode();
            foreach (var prop in DependentRequiredPropertyNames)
            {
                json.SetValue(prop.Key, prop.Value);
            }

            node.SetValue("dependentRequired", json);
        }

        if (DependentPropertyPolicy.Count > 0)
        {
            var json = new JsonObjectNode();
            foreach (var prop in DependentPropertyPolicy)
            {
                json.SetValueIfNotNull(prop.Key, prop.Value?.ToJsonInternal(false));
            }

            node.SetValue("dependentSchemas", json);
        }

        node.SetValueIfNotNull("propertyNames", PropertyNamePolicy?.ToJsonInternal(false));
        node.SetValueIfNotNull("minProperties", MinProperties);
        node.SetValueIfNotNull("maxProperties", MaxProperties);
        if (DisableAdditionalProperties) node.SetValue("additionalProperties", false);
        else node.SetValueIfNotNull("additionalProperties", AdditionalProperties);
        node.SetValueIfNotNull("if", IfPolicy?.ToJsonInternal(false));
        node.SetValueIfNotNull("then", ThenPolicy?.ToJsonInternal(false));
        node.SetValueIfNotNull("else", ElsePolicy?.ToJsonInternal(false));
    }
}
