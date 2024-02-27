using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Json;

using Trivial.Collection;
using Trivial.Data;
using Trivial.Reflection;
using Trivial.Tasks;
using Trivial.Web;

using SystemJsonObject = System.Text.Json.Nodes.JsonObject;
using SystemJsonArray = System.Text.Json.Nodes.JsonArray;
using SystemJsonValue = System.Text.Json.Nodes.JsonValue;
using SystemJsonNode = System.Text.Json.Nodes.JsonNode;
using System.Threading.Tasks;
using System.Security;

namespace Trivial.Text;

/// <summary>
/// The extensions for class IJsonValueNode, JsonDocument, JsonElement, etc.
/// </summary>
public static class JsonValues
{
    internal const string SELF_REF = "@";

    /// <summary>
    /// JSON null.
    /// </summary>
    public static readonly IJsonDataNode Null = new JsonNullNode(JsonValueKind.Null);

    /// <summary>
    /// JSON undefined.
    /// </summary>
    public static readonly IJsonDataNode Undefined = new JsonNullNode(JsonValueKind.Undefined);

    /// <summary>
    /// Converts from JSON document.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>The JSON value.</returns>
    public static IJsonDataNode ToJsonValue(JsonDocument json)
    {
        if (json is null) return null;
        return ToJsonValue(json.RootElement);
    }

    /// <summary>
    /// Converts from JSON element.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>The JSON value.</returns>
    public static IJsonDataNode ToJsonValue(JsonElement json)
    {
        return json.ValueKind switch
        {
            JsonValueKind.Undefined => Undefined,
            JsonValueKind.Null => Null,
            JsonValueKind.String => new JsonStringNode(json.GetString()),
            JsonValueKind.Number => json.TryGetInt64(out var l)
                ? new JsonIntegerNode(l)
                : (json.TryGetDouble(out var d) ? new JsonDoubleNode(d) : Null),
            JsonValueKind.True => JsonBooleanNode.True,
            JsonValueKind.False => JsonBooleanNode.False,
            JsonValueKind.Array => (JsonArrayNode)json,
            JsonValueKind.Object => (JsonObjectNode)json,
            _ => null
        };
    }

    /// <summary>
    /// Converts from JSON element.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>The JSON value.</returns>
    public static IJsonDataNode ToJsonValue(SystemJsonNode json)
    {
        if (json is null)
            return Null;
        if (json is SystemJsonObject obj)
            return (JsonObjectNode)obj;
        if (json is SystemJsonArray arr)
            return (JsonArrayNode)arr;
        if (json is not SystemJsonValue token)
            return null;
        if (token.TryGetValue(out string s))
            return new JsonStringNode(s);
        if (token.TryGetValue(out bool b))
            return b ? JsonBooleanNode.True : JsonBooleanNode.False;
        if (token.TryGetValue(out long l))
            return new JsonIntegerNode(l);
        if (token.TryGetValue(out int i))
            return new JsonIntegerNode(i);
        if (token.TryGetValue(out uint ui))
            return new JsonIntegerNode(ui);
        if (token.TryGetValue(out short sh))
            return new JsonIntegerNode(sh);
        if (token.TryGetValue(out ushort ush))
            return new JsonIntegerNode(ush);
        if (token.TryGetValue(out sbyte sb))
            return new JsonIntegerNode(sb);
        if (token.TryGetValue(out byte by))
            return new JsonIntegerNode(by);
        if (token.TryGetValue(out double d))
            return new JsonDoubleNode(d);
        if (token.TryGetValue(out float f))
            return new JsonDoubleNode(f);
        if (token.TryGetValue(out decimal de))
            return new JsonDoubleNode(de);
        if (token.TryGetValue(out Guid g))
            return new JsonStringNode(g);
        if (token.TryGetValue(out DateTime dt))
            return new JsonStringNode(dt);
        if (token.TryGetValue(out DateTimeOffset dto))
            return new JsonStringNode(dto);
        if (token.TryGetValue(out char c))
            return new JsonStringNode(c);
        if (token.TryGetValue(out JsonElement e))
            return ToJsonValue(e);
        return null;
    }

    /// <summary>
    /// Attempts to represent the current JSON string or JavaScript date tidks number as a date time.
    /// </summary>
    /// <param name="json">The JSON element.</param>
    /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
    /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    public static bool TryGetJavaScriptDateTicks(this JsonElement json, out DateTime value)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.String:
                if (json.TryGetDateTime(out DateTime tmp)) break;
                value = tmp;
                return true;
            case JsonValueKind.Number:
                if (!json.TryGetInt64(out long tick)) break;
                value = WebFormat.ParseDate(tick);
                return true;
            default:
                throw new InvalidOperationException("The value kind should be string or number.");
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to represent the current JSON string or Unix timestamps number as a date time.
    /// </summary>
    /// <param name="json">The JSON element.</param>
    /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
    /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    public static bool TryGetUnixTimestamps(this JsonElement json, out DateTime value)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.String:
                if (json.TryGetDateTime(out DateTime tmp)) break;
                value = tmp;
                return true;
            case JsonValueKind.Number:
                if (!json.TryGetInt64(out long tick)) break;
                value = Web.WebFormat.ParseUnixTimestamp(tick);
                return true;
            default:
                throw new InvalidOperationException("The value kind should be string or number.");
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to represent the current JSON string or Windows file time number as a date time.
    /// </summary>
    /// <param name="json">The JSON element.</param>
    /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
    /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    public static bool TryGetWindowsFileTimeUtc(this JsonElement json, out DateTime value)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.String:
                if (json.TryGetDateTime(out DateTime tmp)) break;
                value = tmp;
                return true;
            case JsonValueKind.Number:
                if (!json.TryGetInt64(out long tick)) break;
                value = DateTime.FromFileTimeUtc(tick);
                return true;
            default:
                throw new InvalidOperationException("The value kind should be string or number.");
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific property for each object.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <returns>The property list.</returns>
    public static List<IJsonDataNode> TryGetValues(this IEnumerable<JsonObjectNode> col, string key)
    {
        var list = new List<IJsonDataNode>();
        if (col == null) return list;
        foreach (var item in col)
        {
            var json = item?.TryGetObjectValue(key);
            list.Add(json ?? Undefined);
        }

        return list;
    }

    /// <summary>
    /// Tries to get the value of the specific property for each object.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="ignoreNotMatched">true if ignore any item which is not JSON object; otherwise, false.</param>
    /// <returns>The property list.</returns>
    public static List<JsonObjectNode> TryGetObjectValues(this IEnumerable<JsonObjectNode> col, string key, bool ignoreNotMatched = false)
    {
        var list = new List<JsonObjectNode>();
        if (col == null) return list;
        if (ignoreNotMatched)
        {
            foreach (var item in col)
            {
                var json = item?.TryGetObjectValue(key);
                if (json == null) continue;
                list.Add(json);
            }
        }
        else
        {
            foreach (var item in col)
            {
                var json = item?.TryGetObjectValue(key);
                list.Add(json);
            }
        }

        return list;
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public static StringBuilder Append(this StringBuilder sb, IJsonStringNode value)
    {
        if (sb == null) return null;
        sb.Append(value.StringValue);
        return sb;
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, string value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item?.TryGetStringValue(key, true, out var s) != true || value != s) continue;
            yield return item;
        }
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <param name="callback">The callback with item, original index and filtered index.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, string value, Action<JsonObjectNode, int, int> callback)
    {
        if (col == null) yield break;
        var i = -1;
        var j = -1;
        foreach (var item in col)
        {
            i++;
            if (item?.TryGetStringValue(key, true, out var s) != true || value != s) continue;
            j++;
            yield return item;
            callback?.Invoke(item, i, j);
        }
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, string value, StringComparison comparisonType)
    {
        if (col == null) yield break;
        if (value == null)
        {
            foreach (var item in col)
            {
                if (!item.IsNullOrUndefined(key)) continue;
                yield return item;
            }

            yield break;
        }

        foreach (var item in col)
        {
            if (item?.TryGetStringValue(key, true, out var s) != true || !value.Equals(s, comparisonType)) continue;
            yield return item;
        }
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <param name="callback">The callback with item, original index and filtered index.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, string value, StringComparison comparisonType, Action<JsonObjectNode, int, int> callback)
    {
        if (col == null) yield break;
        var i = -1;
        var j = -1;
        if (value == null)
        {
            foreach (var item in col)
            {
                i++;
                if (!item.IsNullOrUndefined(key)) continue;
                j++;
                yield return item;
                callback?.Invoke(item, i, j);
            }

            yield break;
        }

        foreach (var item in col)
        {
            i++;
            if (item?.TryGetStringValue(key, true, out var s) != true || !value.Equals(s, comparisonType)) continue;
            j++;
            yield return item;
            callback?.Invoke(item, i, j);
        }
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, int value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item?.TryGetInt32Value(key, out var s) != true || value != s) continue;
            yield return item;
        }
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, long value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item?.TryGetInt64Value(key, out var s) != true || value != s) continue;
            yield return item;
        }
    }

    /// <summary>
    /// Converts to string format.
    /// </summary>
    /// <param name="value">The JSON object.</param>
    /// <param name="indent">The indent style.</param>
    /// <returns>The string converted in JSON format.</returns>
    public static string ToString(IJsonObjectHost value, IndentStyles indent = IndentStyles.Minified)
    {
        var json = value?.ToJson();
        if (json is null) return Null.ToString();
        return json.ToString(indent);
    }

    /// <summary>
    /// Gets JSON property name.
    /// </summary>
    /// <param name="property">The property name.</param>
    /// <returns>The JSON property name; or null, if ignore JSON serialization.</returns>
    public static string GetPropertyName(PropertyInfo property)
    {
        try
        {
            var attr = property.GetCustomAttributes<JsonIgnoreAttribute>()?.FirstOrDefault();
            if (attr != null && attr.Condition == JsonIgnoreCondition.Always) return null;
        }
        catch (NotSupportedException)
        {
        }
        catch (TypeLoadException)
        {
        }

        try
        {
            var attr = property.GetCustomAttributes<JsonPropertyNameAttribute>()?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(attr?.Name)) return attr.Name;
        }
        catch (NotSupportedException)
        {
        }
        catch (TypeLoadException)
        {
        }

        return property.Name;
    }

    /// <summary>
    /// Creates a JSON schema of a type.
    /// </summary>
    /// <param name="json">The JSON object to create schema.</param>
    /// <param name="desc">The description.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <returns>The JSON schema description instance; or null, if not supported.</returns>
    public static JsonNodeSchemaDescription CreateSchema(JsonObjectNode json, string desc = null, IJsonNodeSchemaCreationHandler<IJsonDataNode> handler = null)
        => CreateSchema(json, 2, desc, handler);

    /// <summary>
    /// Creates a JSON schema of a type.
    /// </summary>
    /// <typeparam name="T">The type to create JSON schema.</typeparam>
    /// <param name="desc">The description.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <returns>The JSON schema description instance; or null, if not supported.</returns>
    public static JsonNodeSchemaDescription CreateSchema<T>(string desc = null, IJsonNodeSchemaCreationHandler<Type> handler = null)
        => CreateSchema(typeof(T), 2, desc, handler);

    /// <summary>
    /// Creates a JSON schema of a type.
    /// </summary>
    /// <param name="type">The type to create JSON schema.</param>
    /// <param name="desc">The description.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <returns>The JSON schema description instance; or null, if not supported.</returns>
    public static JsonNodeSchemaDescription CreateSchema(Type type, string desc = null, IJsonNodeSchemaCreationHandler<Type> handler = null)
        => CreateSchema(type, 2, desc, handler);

    internal static JsonOperationDescription CreateDescriptionByAttribute(MemberInfo member)
    {
        var attr = member?.GetCustomAttributes<JsonOperationDescriptiveAttribute>()?.FirstOrDefault();
        if (attr?.DescriptiveType == null || !attr.DescriptiveType.IsClass) return null;
        if (member is PropertyInfo prop)
        {
            if (ObjectConvert.TryCreateInstance<IJsonOperationDescriptive<PropertyInfo>>(attr.DescriptiveType, out var d))
                return d.CreateDescription(attr.Id, prop);
        }

        if (member is MethodInfo method)
        {
            if (ObjectConvert.TryCreateInstance<IJsonOperationDescriptive<MethodInfo>>(attr.DescriptiveType, out var d))
                return d.CreateDescription(attr.Id, method);
        }

        return ObjectConvert.TryCreateInstance<IJsonOperationDescriptive>(attr.DescriptiveType, out var desc) ? desc.CreateDescription() : null;
    }

    /// <summary>
    /// Creates a JSON schema of a type.
    /// </summary>
    /// <param name="type">The type to create JSON schema.</param>
    /// <param name="level">The maximum level to create schema for JSON object.</param>
    /// <param name="desc">The description.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <param name="breadcrumb">The path breadcrumb.</param>
    /// <returns>The JSON schema description instance; or null, if not supported.</returns>
    private static JsonNodeSchemaDescription CreateSchema(Type type, int level, string desc = null, IJsonNodeSchemaCreationHandler<Type> handler = null, NodePathBreadcrumb<Type> breadcrumb = null)
    {
        handler ??= EmptyJsonNodeSchemaCreationHandler<Type>.Instance;
        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(ValueTask<>) || genericType == typeof(Task<>) || genericType == typeof(Lazy<>))
            {
                type = type.GetGenericArguments().FirstOrDefault();
                if (type == null) return null;
            }
        }

        breadcrumb ??= new(type, null);
        level--;
        if (string.IsNullOrWhiteSpace(desc)) desc = StringExtensions.GetDescription(type);
        if (ObjectConvert.IsNullableValueType(type, out var valueType)) type = valueType;
        if (type == typeof(object))
        {
            return handler.Convert(type, null, breadcrumb);
        }
        else if (type == typeof(string))
        {
            return handler.Convert(type, new JsonStringSchemaDescription
            {
                Description = desc,
            }, breadcrumb);
        }
        else if (type == typeof(JsonObjectNode) || type == typeof(System.Text.Json.Nodes.JsonObject))
        {
            return handler.Convert(type, new JsonObjectSchemaDescription
            {
                Description = desc,
            }, breadcrumb);
        }
        else if (type == typeof(JsonArrayNode) || type == typeof(System.Text.Json.Nodes.JsonArray))
        {
            return handler.Convert(type, new JsonArraySchemaDescription
            {
                Description = desc,
            }, breadcrumb);
        }
        else if (type.IsEnum)
        {
            var d = new JsonStringSchemaDescription
            {
                Description = desc
            };
            try
            {
                d.EnumItems.AddRange(Enum.GetNames(type));
            }
            catch (InvalidOperationException)
            {
            }

            return handler.Convert(type, d, breadcrumb);
        }
        else if (ObjectConvert.IsGenericEnumerable(type))
        {
            var d = new JsonArraySchemaDescription
            {
                Description = desc
            };
            if (level < 0) return handler.Convert(type, d, breadcrumb);
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(List<>) || genericType == typeof(IEnumerable<>) || genericType == typeof(ICollection<>) || genericType == typeof(IList<>))
                {
                    var genericParamType = type.GetGenericArguments().FirstOrDefault();
                    if (genericParamType == null) return handler.Convert(type, d, breadcrumb);
                    d.DefaultItems = CreateSchema(genericParamType, level, null, handler);
                }
            }

            return handler.Convert(type, d, breadcrumb);
        }
        else if (type.IsValueType)
        {
            if (type == typeof(int) || type == typeof(long) || type == typeof(uint) || type == typeof(short) || type == typeof(byte))
            {
                return handler.Convert(type, new JsonIntegerSchemaDescription
                {
                    Description = desc
                }, breadcrumb);
            }
            else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                return handler.Convert(type, new JsonNumberSchemaDescription
                {
                    Description = desc
                }, breadcrumb);
            }
            else if (type == typeof(bool))
            {
                return handler.Convert(type, new JsonBooleanSchemaDescription
                {
                    Description = desc
                }, breadcrumb);
            }
            else if (type == typeof(Guid))
            {
                return handler.Convert(type, new JsonStringSchemaDescription
                {
                    Description = desc
                }, breadcrumb);
            }
            else if (type == typeof(DateTime))
            {
                return handler.Convert(type, new JsonStringSchemaDescription
                {
                    Description = desc,
                    Format = "date-time"
                }, breadcrumb);
            }
#if NET6_0_OR_GREATER
            else if (type == typeof(Half))
            {
                return handler.Convert(type, new JsonNumberSchemaDescription
                {
                    Description = desc
                }, breadcrumb);
            }
            else if (type == typeof(DateOnly))
            {
                return handler.Convert(type, new JsonStringSchemaDescription
                {
                    Description = desc,
                    Format = "date"
                }, breadcrumb);
            }
            else if (type == typeof(TimeOnly))
            {
                return handler.Convert(type, new JsonStringSchemaDescription
                {
                    Description = desc,
                    Format = "time"
                }, breadcrumb);
            }
#endif

            return handler.Convert(type, null, breadcrumb);
        }
        else if (type == typeof(StringBuilder))
        {
            return handler.Convert(type, new JsonStringSchemaDescription
            {
                Description = desc,
            }, breadcrumb);
        }
        else if (type == typeof(Uri))
        {
            return handler.Convert(type, new JsonStringSchemaDescription
            {
                Description = desc,
                Format = "uri-reference"
            }, breadcrumb);
        }

        var jsonSchema = new JsonObjectSchemaDescription
        {
            Description = desc
        };
        if (level < 0 || type.IsInterface) return handler.Convert(type, jsonSchema, breadcrumb);
        try
        {
            var jsonSerializer = type.GetCustomAttributes<JsonConverterAttribute>()?.FirstOrDefault();
            if (jsonSerializer != null) return handler.Convert(type, jsonSchema, breadcrumb);
        }
        catch (NotSupportedException)
        {
        }
        catch (TypeLoadException)
        {
        }

        var props = type.GetProperties();
        foreach (var prop in props)
        {
            var name = GetPropertyName(prop);
            if (string.IsNullOrEmpty(name)) continue;
            desc = StringExtensions.GetDescription(prop);
            var propSchema = CreateSchema(prop.PropertyType, level, desc, handler, new(prop.PropertyType, breadcrumb, name));
            if (propSchema != null) jsonSchema.Properties[name] = propSchema;
        }

        return handler.Convert(type, jsonSchema, breadcrumb);
    }

    /// <summary>
    /// Creates a JSON schema of a type.
    /// </summary>
    /// <param name="json">The JSON object to create schema.</param>
    /// <param name="level">The maximum level to create schema for JSON object.</param>
    /// <param name="desc">The description.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <param name="breadcrumb">The path breadcrumb.</param>
    /// <returns>The JSON schema description instance; or null, if not supported.</returns>
    private static JsonObjectSchemaDescription CreateSchema(JsonObjectNode json, int level, string desc = null, IJsonNodeSchemaCreationHandler<IJsonDataNode> handler = null, NodePathBreadcrumb<IJsonDataNode> breadcrumb = null)
    {
        handler ??= EmptyJsonNodeSchemaCreationHandler<IJsonDataNode>.Instance;
        breadcrumb ??= new(json, null);
        var schema = new JsonObjectSchemaDescription
        {
            Description = desc
        };
        level--;
        foreach (var prop in json)
        {
            var value = prop.Value;
            var bc = new NodePathBreadcrumb<IJsonDataNode>(value, breadcrumb, prop.Key);
            switch (value.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    continue;
                case JsonValueKind.String:
                    schema.Properties[prop.Key] = handler.Convert(value, new JsonStringSchemaDescription(), bc);
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    schema.Properties[prop.Key] = handler.Convert(value, new JsonBooleanSchemaDescription(), bc);
                    break;
                case JsonValueKind.Number:
                    schema.Properties[prop.Key] = handler.Convert(value, new JsonNumberSchemaDescription(), bc);
                    break;
                case JsonValueKind.Object:
                    if (level < 0 && value is JsonObjectNode obj) schema.Properties[prop.Key] = level < 1
                        ? handler.Convert(value, new JsonObjectSchemaDescription(), bc)
                        : CreateSchema(obj, level, null, handler, bc);
                    break;
                case JsonValueKind.Array:
                    schema.Properties[prop.Key] = handler.Convert(value, new JsonArraySchemaDescription(), bc);
                    break;
            }
        }

        return handler.Convert(json, schema, breadcrumb) as JsonObjectSchemaDescription;
    }

    internal static object GetValue(IJsonDataNode value)
    {
        if (value == null) return null;
        switch (value.ValueKind)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.String:
                if (value is IJsonStringNode s) return s.StringValue;
                else if (value is IJsonValueNode<string> s2) return s2.Value;
                return null;
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Number:
                if (value is JsonIntegerNode i) return i.Value;
                else if (value is JsonDoubleNode d) return d.Value;
                else if (value is IJsonNumberNode n) return n.IsInteger ? n.GetInt64() : n.GetDouble();
                return 0;
            case JsonValueKind.Object:
            case JsonValueKind.Array:
                return value;
            default:
                return value;
        }
    }

    internal static DateTime? TryGetDateTime(JsonObjectNode json)
    {
        var jsTick = json.TryGetInt64Value("value");
        if (jsTick.HasValue) return WebFormat.ParseDate(jsTick.Value);
        var year = json.TryGetInt32Value("year");
        var month = json.TryGetInt32Value("month");
        var day = json.TryGetInt32Value("day");
        if (!year.HasValue || !month.HasValue || !day.HasValue) return null;
        var hour = json.TryGetInt32Value("hour") ?? 0;
        var minute = json.TryGetInt32Value("minute") ?? 0;
        var second = json.TryGetInt32Value("second") ?? 0;
        var millisecond = json.TryGetInt32Value("millisecond") ?? 0;
        var kind = json.TryGetStringTrimmedValue("kind", true)?.ToLowerInvariant() ?? "utc";
        if (kind == "utc" || kind == "z" || kind == "0" || kind == "0000" || kind == "+00:00" || kind == "+0000" || kind == "universal")
            return new DateTime(year.Value, month.Value, day.Value, hour, minute, second, millisecond, DateTimeKind.Utc);
        if (kind == "local" || kind == "server")
            return new DateTime(year.Value, month.Value, day.Value, hour, minute, second, millisecond, DateTimeKind.Local);
        if (kind.Length != 5)
        {
            if (kind.Length == 6 && kind[3] == ':') kind = kind.Replace(":", string.Empty);
            if (kind.Length != 5) return null;
        }

        if ((kind.StartsWith('+') || kind.StartsWith('-')) && int.TryParse(kind, out var offset))
        {
            var span = new TimeSpan(offset / 100, Math.Abs(offset % 100), 0);
            var dt = new DateTimeOffset(year.Value, month.Value, day.Value, hour, minute, second, millisecond, span);
            return dt.ToUniversalTime().DateTime;
        }

        return null;
    }

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    internal static bool Equals(IJsonValueNode leftValue, IJsonValueNode rightValue)
    {
        if (leftValue is null || leftValue.ValueKind == JsonValueKind.Null || leftValue.ValueKind == JsonValueKind.Undefined)
        {
            return rightValue is null || rightValue.ValueKind == JsonValueKind.Null || rightValue.ValueKind == JsonValueKind.Undefined;
        }

        if (rightValue is null || rightValue.ValueKind != leftValue.ValueKind) return false;
        return leftValue.Equals(rightValue);
    }

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>The JSON node.</returns>
    internal static SystemJsonNode ToJsonNode(IJsonDataNode json)
    {
        if (json is null)
            return null;
        if (json is JsonObjectNode obj)
            return (SystemJsonObject)obj;
        if (json is JsonArrayNode arr)
            return (SystemJsonArray)arr;
        if (json is JsonStringNode s)
            return (SystemJsonValue)s;
        if (json is JsonIntegerNode i)
            return (SystemJsonValue)i;
        if (json is JsonDoubleNode f)
            return (SystemJsonValue)f;
        if (json is JsonBooleanNode b)
            return (SystemJsonValue)b;
        return null;
    }

    internal static IJsonDataNode ConvertValue(IJsonValueNode value, IJsonValueNode thisInstance = null)
    {
        if (value is null || value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined) return Null;
        if (value is JsonObjectNode obj)
        {
            if (ReferenceEquals(obj, thisInstance)) return obj.Clone();
            return obj;
        }

        if (value is JsonArrayNode arr)
        {
            if (ReferenceEquals(arr, thisInstance)) return arr.Clone();
            return arr;
        }

        if (value is JsonStringNode || value is JsonIntegerNode || value is JsonDoubleNode || value is JsonBooleanNode) return value as IJsonDataNode;
        if (value.ValueKind == JsonValueKind.True) return JsonBooleanNode.True;
        if (value.ValueKind == JsonValueKind.False) return JsonBooleanNode.False;
        if (value.ValueKind == JsonValueKind.String)
        {
            if (value is IJsonValueNode<string> str) return new JsonStringNode(str.Value);
            if (value is IJsonValueNode<DateTime> date) return new JsonStringNode(date.Value);
            if (value is IJsonValueNode<Guid> guid) return new JsonStringNode(guid.Value);
            if (value is IJsonStringNode js) return new JsonStringNode(js.StringValue);
        }

        if (value.ValueKind == JsonValueKind.Number)
        {
            if (value is IJsonValueNode<int> int32) return new JsonIntegerNode(int32.Value);
            if (value is IJsonValueNode<long> int64) return new JsonIntegerNode(int64.Value);
            if (value is IJsonValueNode<short> int16) return new JsonIntegerNode(int16.Value);
            if (value is IJsonValueNode<double> d) return new JsonDoubleNode(d.Value);
            if (value is IJsonValueNode<float> f) return new JsonDoubleNode(f.Value);
            if (value is IJsonValueNode<decimal> fd) return new JsonDoubleNode((double)fd.Value);
            if (value is IJsonValueNode<bool> b) return b.Value ? JsonBooleanNode.True : JsonBooleanNode.False;
            if (value is IJsonValueNode<uint> uint32) return new JsonIntegerNode(uint32.Value);
            if (value is IJsonValueNode<ulong> uint64) return new JsonDoubleNode(uint64.Value);
            if (value is IJsonValueNode<ushort> uint16) return new JsonIntegerNode(uint16.Value);
            if (value is IJsonValueNode<DateTime> date) return new JsonIntegerNode(date.Value);
            var s = value.ToString();
            if (long.TryParse(s, out var l)) return new JsonIntegerNode(l);
            if (double.TryParse(s, out var db)) return new JsonDoubleNode(db);
            return Null;
        }

        return Null;
    }

    internal static JsonObjectNode ToJson(JsonNodeSchemaDescription schema)
        => schema?.ToJson();

    internal static JsonObjectNode ToJson(Dictionary<string, JsonNodeSchemaDescription> schema)
    {
        if (schema == null) return null;
        var json = new JsonObjectNode();
        foreach (var prop in schema)
        {
            json.SetValue(prop.Key, prop.Value);
        }

        return json;
    }

    internal static double? ToDouble(int? value)
        => value.HasValue ? value.Value : null;

    internal static void RemoveJsonNodeSchemaDescriptionExtendedProperties(JsonObjectNode json, bool onlyBase)
    {
        json.SetRange(json);
        json.Id = null;
        json.Schema = null;
        json.Remove(new[] { "$ref", "description", "examples" });
        if (onlyBase) return;
        json.Remove(new[] { "title", "deprecated", "readOnly", "writeOnly", "allOf", "anyOf", "oneOf", "not", "enum", "definitions" });
    }
}
