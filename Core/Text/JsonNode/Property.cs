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
    /// <returns><c>true</c> if has the property and the type is the one expected; otherwise, <c>false</c>.</returns>
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
    /// <returns><c>true</c> if gets succeeded; otherwise, false, includes the scenarios that it does NOT exist or its type is not expected.</returns>
    bool TryGetObjectValue(JsonObjectNode source, string key, out JsonObjectNode value, out string exactKey);
}

/// <summary>
/// A handler for JSON value node.
/// </summary>
public interface IJsonValueNodeHandler
{
    /// <summary>
    /// Occurs on the value is a string.
    /// </summary>
    /// <param name="value">The value.</param>
    void Is(string value);

    /// <summary>
    /// Occurs on the value is an integer.
    /// </summary>
    /// <param name="value">The value.</param>
    void Is(long value);

    /// <summary>
    /// Occurs on the value is a double floating-point number.
    /// </summary>
    /// <param name="value">The value.</param>
    void Is(double value);

    /// <summary>
    /// Occurs on the value is a boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    void Is(bool value);

    /// <summary>
    /// Occurs on the value is a JSON object.
    /// </summary>
    /// <param name="value">The value.</param>
    void Is(JsonObjectNode value);

    /// <summary>
    /// Occurs on the value is a JSON array.
    /// </summary>
    /// <param name="value">The value.</param>
    void Is(JsonArrayNode value);

    /// <summary>
    /// Occurs on the value is null.
    /// </summary>
    void IsNull();

    /// <summary>
    /// Occurs on the value is undefined.
    /// </summary>
    void Undefine();
}

/// <summary>
/// A handler for JSON value node.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public interface IJsonValueNodeHandler<T>
{
    /// <summary>
    /// Occurs on the value is a string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    T Is(string value);

    /// <summary>
    /// Occurs on the value is an integer.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    T Is(long value);

    /// <summary>
    /// Occurs on the value is a double floating-point number.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    T Is(double value);

    /// <summary>
    /// Occurs on the value is a boolean.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    T Is(bool value);

    /// <summary>
    /// Occurs on the value is a JSON object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    T Is(JsonObjectNode value);

    /// <summary>
    /// Occurs on the value is a JSON array.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    T Is(JsonArrayNode value);

    /// <summary>
    /// Occurs on the value is null.
    /// </summary>
    /// <returns>The result.</returns>
    T IsNull();

    /// <summary>
    /// Occurs on the value is undefined.
    /// </summary>
    /// <returns>The result.</returns>
    T Undefine();
}
