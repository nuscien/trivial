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
        Properties = new();
    }

    /// <summary>
    /// Initializes a new instance of the JsonObjectSchemaDescription class.
    /// </summary>
    /// <param name="properties">The description of properties.</param>
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
        ExtendedProperties.Remove("default");
        ExtendedProperties.Remove("properties");
        ExtendedProperties.Remove("patternProperties");
        ExtendedProperties.Remove("required");
        ExtendedProperties.Remove("dependentRequired");
        ExtendedProperties.Remove("dependentSchemas");
        ExtendedProperties.Remove("propertyNames");
        ExtendedProperties.Remove("minProperties");
        ExtendedProperties.Remove("maxProperties");
        ExtendedProperties.Remove("additionalProperties");
        ExtendedProperties.Remove("if");
        ExtendedProperties.Remove("then");
        ExtendedProperties.Remove("else");
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
    /// Converts the schema to TypeScript definition string.
    /// </summary>
    /// <param name="name">The interface name.</param>
    /// <returns>A TypeScript definition string.</returns>
    public string ToTypeScriptDefinitionString(string name)
    {
        var sb = new StringBuilder();
        sb.Append("export interface ");
        sb.Append(name);
        sb.Append(' ');
        ToTypeScriptDefinitionString(sb, 0);
        sb.AppendLine();
        return sb.ToString();
    }

    /// <summary>
    /// Converts the schema to Type Script definition string.
    /// </summary>
    /// <returns>A Type Script definition string.</returns>
    public string ToTypeScriptDefinitionString()
    {
        var sb = new StringBuilder();
        ToTypeScriptDefinitionString(sb, 0);
        sb.AppendLine();
        return sb.ToString();
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

    /// <summary>
    /// Converts the schema to TypeScript definition string.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="indent">The indent level.</param>
    /// <returns>A TypeScript definition string.</returns>
    private void ToTypeScriptDefinitionString(StringBuilder sb, int indent)
    {
        var indentStr = new string(' ', indent * 2);
        sb.AppendLine("{");
        foreach (var prop in Properties)
        {
            var key = prop.Key;
            var v = prop.Value;
            if (string.IsNullOrWhiteSpace(key) || v == null) continue;
            if (!string.IsNullOrWhiteSpace(v.Description))
            {
                sb.AppendLine();
                sb.Append(indentStr);
                sb.AppendLine("  /** ");
                sb.Append(indentStr);
                sb.Append("   * ");
                sb.AppendLine(v.Description);
                sb.Append(indentStr);
                sb.AppendLine("   */");
            }

            sb.Append(indentStr);
            sb.Append("  ");
            sb.Append(key);
            if (!RequiredPropertyNames.Contains(key)) sb.Append('?');
            sb.Append(": ");
            if (v is JsonStringSchemaDescription str)
            {
                sb.Append(str.ConstantValue ?? "string");
            }
            else if (v is JsonNumberSchemaDescription || v is JsonIntegerSchemaDescription)
            {
                sb.Append("number");
            }
            else if (v is JsonBooleanSchemaDescription)
            {
                sb.Append("boolean");
            }
            else if (v is JsonObjectSchemaDescription obj)
            {
                obj.ToTypeScriptDefinitionString(sb, indent + 1);
            }
            else if (v is JsonArraySchemaDescription arr)
            {
                var item = arr.DefaultItems;
                if (item != null && (arr.FixedItems == null || arr.FixedItems.Count == 0))
                {
                    if (item is JsonStringSchemaDescription str2)
                    {
                        sb.Append(str2.ConstantValue ?? "string");
                    }
                    else if (item is JsonNumberSchemaDescription || item is JsonIntegerSchemaDescription)
                    {
                        sb.Append("number");
                    }
                    else if (item is JsonBooleanSchemaDescription)
                    {
                        sb.Append("boolean");
                    }
                    else if (item is JsonObjectSchemaDescription obj2)
                    {
                        obj2.ToTypeScriptDefinitionString(sb, indent + 1);
                    }
                    else
                    {
                        sb.Append("any");
                    }
                }

                sb.Append("[]");
            }
            else
            {
                sb.Append("any");
            }

            if (v.EnumItems != null && v.EnumItems.Count > 0)
            {
                sb.Append(" | ");
                sb.Append(string.Join(" | ", v.EnumItems.Select(o => o.ToString())));
            }

            sb.AppendLine(";");
        }

        if (!DisableAdditionalProperties)
        {
            sb.Append(indentStr);
            sb.AppendLine("  [key: string]: any;");
        }

        sb.Append(indentStr);
        sb.Append('}');
    }
}
