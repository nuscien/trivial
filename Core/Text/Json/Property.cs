using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The property resolver of JSON object.
/// </summary>
/// <typeparam name="T">The type of property value.</typeparam>
public interface IJsonPropertyResolver<T>
{
    /// <summary>
    /// Tries to get the property value.
    /// </summary>
    /// <param name="source">The source (parent) node.</param>
    /// <param name="result">The value of the property.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    bool TryGetValue(JsonObjectNode source, out T result);
}

/// <summary>
/// The JSON object property value getting policy.
/// </summary>
public interface IJsonPropertyRoutePolicy
{
    /// <summary>
    /// Tries to get the JSON object value of the specific property.
    /// </summary>
    /// <param name="source">The source (parent) node.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">THe JSON object value of the property.</param>
    /// <param name="exactKey">The exact key to resolve the property.</param>
    /// <returns>true if gets succeeded; otherwise, false, includes the scenarios that it does NOT exist or its type is not expected.</returns>
    bool TryGetObjectValue(JsonObjectNode source, string key, out JsonObjectNode value, out string exactKey);
}