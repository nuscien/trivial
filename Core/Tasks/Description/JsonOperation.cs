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
public class JsonOperationDescription
{
    /// <summary>
    /// Gets or sets the operation identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the operation description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the schema for request argument.
    /// </summary>
    public JsonNodeSchemaDescription ArgumentSchema { get; set; }

    /// <summary>
    /// Gets or sets the schema for response data.
    /// </summary>
    public JsonNodeSchemaDescription ResultSchema { get; set; }

    /// <summary>
    /// Gets or sets the schema for error data.
    /// </summary>
    public JsonNodeSchemaDescription ErrorSchema { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    public object Tag { get; set; }

    /// <summary>
    /// Gets the additional store data.
    /// </summary>
    public JsonObjectNode Data { get; } = new();

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
        handler ??= EmptyJsonNodeSchemaCreationHandler<Type>.Instance;
        var info = new JsonOperationDescription()
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
    public static JsonOperationDescription Create(Type type, string method, string id = null)
    {
        var m = type.GetMethod(method);
        return Create(m, id);
    }
}
