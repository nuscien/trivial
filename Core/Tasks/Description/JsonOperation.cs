using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Tasks;

/// <summary>
/// The operation description for API.
/// </summary>
public class JsonOperationDescription : BaseObservableProperties
{
    /// <summary>
    /// Gets or sets the operation identifier.
    /// </summary>
    public string Id
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the operation description.
    /// </summary>
    public string Description
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the schema for request argument.
    /// </summary>
    public JsonNodeSchemaDescription ArgumentSchema
    {
        get => GetCurrentProperty<JsonNodeSchemaDescription>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the schema for response data.
    /// </summary>
    public JsonNodeSchemaDescription ResultSchema
    {
        get => GetCurrentProperty<JsonNodeSchemaDescription>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the schema for error data.
    /// </summary>
    public JsonNodeSchemaDescription ErrorSchema
    {
        get => GetCurrentProperty<JsonNodeSchemaDescription>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    public object Tag
    {
        get => GetCurrentProperty<object>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets the additional store data.
    /// </summary>
    public JsonObjectNode Data { get; } = new();

    /// <summary>
    /// Tries to create a description from a property. The property should be an IJsonOperationDescriptive object.
    /// </summary>
    /// <param name="obj">The target object.</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <returns>A description instance; or null, if the property does not exist or is not supported.</returns>
    public static JsonOperationDescription CreateByProperty(object obj, string propertyName, string id = null, IJsonNodeSchemaCreationHandler<Type> handler = null)
    {
        var prop = obj?.GetType()?.GetProperty(propertyName);
        if (prop == null) return null;
        var desc = StringExtensions.GetDescription(prop);
        var info = JsonValues.CreateDescriptionByAttribute(prop);
        if (info == null)
        {
            if (ObjectConvert.TryGetProperty(obj, prop, out IJsonOperationDescriptive d) && d != null)
                info = d.CreateDescription();
        }

        if (info != null)
        {
            if (!string.IsNullOrWhiteSpace(desc)) info.Description = desc;
            if (!string.IsNullOrWhiteSpace(id)) info.Id = id;
            if (string.IsNullOrWhiteSpace(info.Id)) info.Id = $"{prop.ReflectedType?.Name ?? "global-"}-{prop.Name}";
            return info;
        }

        return info;
    }

    /// <summary>
    /// Tries to create a description.
    /// </summary>
    /// <param name="method">The method info to search.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <returns>A description instance.</returns>
    public static JsonOperationDescription Create(MethodInfo method, string id = null, IJsonNodeSchemaCreationHandler<Type> handler = null)
    {
        var desc = StringExtensions.GetDescription(method);
        var info = JsonValues.CreateDescriptionByAttribute(method);
        if (info != null)
        {
            if (!string.IsNullOrWhiteSpace(desc)) info.Description = desc;
            if (!string.IsNullOrWhiteSpace(id)) info.Id = id;
            if (string.IsNullOrWhiteSpace(info.Id)) info.Id = $"{method.ReflectedType?.Name ?? "global-"}-{method.Name}";
            return info;
        }

        handler ??= EmptyJsonNodeSchemaCreationHandler<Type>.Instance;
        info = new JsonOperationDescription()
        {
            Id = id ?? $"{method.ReflectedType?.Name ?? "global-"}-{method.Name}",
            Description = desc
        };
        var parameters = method.GetParameters();
        var param = parameters?.FirstOrDefault();
        if (param != null)
        {
            desc = StringExtensions.GetDescription(param);
            info.ArgumentSchema = JsonValues.CreateSchema(param.ParameterType, desc, handler);
        }

        var result = method.ReturnType;
        if (result != null)
        {
            desc = StringExtensions.GetDescription(result);
            info.ResultSchema = JsonValues.CreateSchema(result, desc, handler);
        }

        return info;
    }

    /// <summary>
    /// Tries to create a description.
    /// </summary>
    /// <param name="type">The type to search.</param>
    /// <param name="method">The method name.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>A description instance.</returns>
    /// <exception cref="ArgumentNullException">method name should not be null.</exception>
    /// <exception cref="AmbiguousMatchException">More than one method is found with the specified name.</exception>
    public static JsonOperationDescription Create(Type type, string method, string id = null)
    {
        var m = type.GetMethod(method);
        return Create(m, id);
    }

    /// <summary>
    /// Tries to create a description.
    /// </summary>
    /// <param name="type">The type to search.</param>
    /// <param name="method">The method name.</param>
    /// <param name="argumentTypes">An array of type objects representing the number, order, and type of the parameters for the method to get.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>A description instance.</returns>
    /// <exception cref="ArgumentNullException">method name should not be null.</exception>
    /// <exception cref="AmbiguousMatchException">More than one method is found with the specified name.</exception>
    public static JsonOperationDescription Create(Type type, string method, Type[] argumentTypes, string id = null)
    {
        var m = type.GetMethod(method, argumentTypes);
        return Create(m, id);
    }
}
