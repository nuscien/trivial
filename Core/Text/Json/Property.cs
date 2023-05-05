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
    /// Gets the property value.
    /// </summary>
    /// <param name="node">The source node.</param>
    /// <param name="result">The value of the property.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    bool TryGetValue(JsonObjectNode node, out T result);
}
