using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        Properties = new();
    }

    /// <summary>
    /// Initializes a new instance of the JsonObjectSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    public JsonObjectSchemaDescription(JsonObjectNode json)
        : this(json, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonObjectSchemaDescription class.
    /// </summary>
    /// <param name="properties">The description of properties.</param>
    /// <param name="requiredPropertyNames">The  name list of required property.</param>
    public JsonObjectSchemaDescription(Dictionary<string, JsonNodeSchemaDescription> properties, IEnumerable<string> requiredPropertyNames = null)
    {
        Properties = properties;
        if (requiredPropertyNames == null) return;
        foreach (var prop in requiredPropertyNames)
        {
            if (string.IsNullOrWhiteSpace(prop) || RequiredPropertyNames.Contains(prop)) continue;
            RequiredPropertyNames.Add(prop);
        }
    }

    /// <summary>
    /// Initializes a new instance of the JsonObjectSchemaDescription class.
    /// </summary>
    /// <param name="json">The JSON object to load.</param>
    /// <param name="skipExtendedProperties">true if only fill the properties without extended; otherwise, false.</param>
    protected JsonObjectSchemaDescription(JsonObjectNode json, bool skipExtendedProperties)
        : base(json, true)
    {
        Properties = new();
        if (json == null) return;
        DefaultValue = json.TryGetObjectValue("default");
        JsonValues.FillObjectSchema(Properties, json, "properties");
        JsonValues.FillObjectSchema(PatternProperties, json, "patternProperties");
        var strArr = json.TryGetStringListValue("required", true);
        if (strArr != null) RequiredPropertyNames.AddRange(strArr);
        var obj = json.TryGetObjectValue("dependentRequired");
        if (obj != null)
        {
            foreach (var key in obj.Keys)
            {
                strArr = obj.TryGetStringListValue(key, true);
                if (strArr != null) DependentRequiredPropertyNames[key] = strArr;
            }
        }

        obj = json.TryGetObjectValue("dependentSchemas");
        if (obj != null)
        {
            foreach (var prop in obj)
            {
                if (prop.Value is not JsonObjectNode item) continue;
                DependentPropertyPolicy[prop.Key] = new JsonObjectSchemaDescription(item);
            }
        }

        obj = json.TryGetObjectValue("propertyNames");
        if (obj != null) PropertyNamePolicy = new JsonStringSchemaDescription(obj);
        MinProperties = json.TryGetInt32Value("minProperties");
        MaxProperties = json.TryGetInt32Value("maxProperties");
        var hasAdditionalProperties = json.TryGetBooleanValue("additionalProperties");
        if (hasAdditionalProperties.HasValue) DisableAdditionalProperties = !hasAdditionalProperties.Value;
        else AdditionalProperties = JsonValues.ConvertToObjectSchema(json.TryGetObjectValue("additionalProperties"));
        obj = json.TryGetObjectValue("if");
        if (obj != null) IfPolicy = new JsonObjectSchemaDescription(obj);
        obj = json.TryGetObjectValue("then");
        if (obj != null) ThenPolicy = new JsonObjectSchemaDescription(obj);
        obj = json.TryGetObjectValue("else");
        if (obj != null) ElsePolicy = new JsonObjectSchemaDescription(obj);

        if (skipExtendedProperties) return;
        ExtendedProperties.SetRange(json);
        JsonValues.RemoveJsonNodeSchemaDescriptionExtendedProperties(ExtendedProperties, false);
        ExtendedProperties.RemoveRange("default", "properties", "patternProperties", "required", "dependentRequired", "dependentSchemas", "propertyNames", "minProperties", "maxProperties", "additionalProperties", "if", "then", "else");
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public JsonObjectNode DefaultValue { get; set; }

    /// <summary>
    /// Gets the description of properties.
    /// The dictionary keys are the property names.
    /// </summary>
    public Dictionary<string, JsonNodeSchemaDescription> Properties { get; }

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

    /// <summary>
    /// Gets the schema.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The property schema; or null, if non-exist.</returns>
    public JsonNodeSchemaDescription GetProperty(string propertyName)
        => Properties.TryGetValue(propertyName, out var value) ? value : null;

    /// <summary>
    /// Adds a property.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="value">The property schema.</param>
    public void SetProperty(string propertyName, JsonNodeSchemaDescription value)
        => Properties[propertyName] = value;

    /// <summary>
    /// Adds a property.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="description">The description.</param>
    /// <param name="pattern">The pattern.</param>
    public JsonStringSchemaDescription SetStringProperty(string propertyName, string description, string pattern = null)
        => SetPropertyInternal(propertyName, new JsonStringSchemaDescription
        {
            Description = description,
            Pattern = pattern
        });

    /// <summary>
    /// Adds a property.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="description">The description.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public JsonNumberSchemaDescription SetNumberProperty(string propertyName, string description, double? min = null, double? max = null)
        => SetPropertyInternal(propertyName, new JsonNumberSchemaDescription
        {
            Description = description,
            Min = min ?? double.NaN,
            Max = max ?? double.NaN
        });

    /// <summary>
    /// Adds a property.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="description">The description.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public JsonBooleanSchemaDescription SetBooleanProperty(string propertyName, string description, double? min = null, double? max = null)
        => SetPropertyInternal(propertyName, new JsonBooleanSchemaDescription
        {
            Description = description
        });

    /// <summary>
    /// Removes the value with the specified key from the properties.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if key is not found in the mapping.</returns>
    public bool RemoveProperty(string propertyName)
        => Properties.Remove(propertyName);

    /// <summary>
    /// Marks the specific properties are required.
    /// </summary>
    /// <param name="propertyNames">The names of the properties required.</param>
    public void MarkPropertyRequired(params string[] propertyNames)
    {
        foreach (var prop in propertyNames)
        {
            if (string.IsNullOrWhiteSpace(prop) || RequiredPropertyNames.Contains(prop)) continue;
            RequiredPropertyNames.Add(prop);
        }
    }

    /// <summary>
    /// Marks the specific properties are required.
    /// </summary>
    /// <param name="propertyNames">The names of the properties required.</param>
    public void MarkPropertyRequired(params ReadOnlySpan<string> propertyNames)
    {
        foreach (var prop in propertyNames)
        {
            if (string.IsNullOrWhiteSpace(prop) || RequiredPropertyNames.Contains(prop)) continue;
            RequiredPropertyNames.Add(prop);
        }
    }

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

    private T SetPropertyInternal<T>(string propertyName, T value) where T : JsonNodeSchemaDescription
    {
        Properties[propertyName] = value;
        return value;
    }
}
