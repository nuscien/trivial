using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// The interface for JSON operation self-describing host.
/// </summary>
public interface IJsonOperationDescriptive
{
    /// <summary>
    /// Creates operation description.
    /// </summary>
    /// <returns>The operation description.</returns>
    public JsonOperationDescription CreateDescription();
}

/// <summary>
/// The interface for JSON operation self-describing host.
/// </summary>
/// <typeparam name="T">The type of the info.</typeparam>
public interface IJsonOperationDescriptive<T>
{
    /// <summary>
    /// Creates operation description.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="info">The info.</param>
    /// <returns>The operation description; or null, if does not support.</returns>
    public JsonOperationDescription CreateDescription(string id, T info);
}

/// <summary>
/// The attribute JSON operation descriptive .
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public sealed class JsonOperationDescriptiveAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the JsonOperationDescriptiveAttribute class.
    /// </summary>
    /// <param name="descriptiveType">The descriptive type.</param>
    /// <param name="id">The member identifier.</param>
    public JsonOperationDescriptiveAttribute(Type descriptiveType, string id)
    {
        DescriptiveType = descriptiveType;
        Id = id;
    }

    /// <summary>
    /// Gets or sets the descriptive type.
    /// </summary>
    public Type DescriptiveType { get; }

    /// <summary>
    /// Gets or sets the identifer.
    /// </summary>
    public string Id { get; }
}
