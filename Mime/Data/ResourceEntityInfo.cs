using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Data;

/// <summary>
/// The entity information.
/// </summary>
public abstract class BaseResourceEntityInfo : BaseObservableProperties, IJsonObjectHost
{
    /// <summary>
    /// Initializes a new instance of the BaseResourceObservableProperties class.
    /// </summary>
    public BaseResourceEntityInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseResourceObservableProperties class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public BaseResourceEntityInfo(Guid id)
        : this(id.ToString("N"))
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseResourceObservableProperties class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public BaseResourceEntityInfo(string id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [DataMember(Name = "id")]
    [JsonPropertyName("id")]
    public string Id
    {
        get => GetCurrentProperty<string>();
        protected set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <returns>A JSON object.</returns>
    public virtual JsonObjectNode ToJson()
        => new()
        {
            { "id", Id },
        };

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>A property value.</returns>
    public new T GetProperty<T>(string key, T defaultValue = default)
        => base.GetProperty(key, defaultValue);

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="result">The property value.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public new bool GetProperty<T>(string key, out T result)
        => base.GetProperty(key, out result);
}
