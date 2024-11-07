using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Trivial.Text;

/// <summary>
/// Tries to get the result from a JSON node.
/// </summary>
/// <typeparam name="T">The type of result.</typeparam>
/// <param name="node">The JSON node to test and convert.</param>
/// <param name="result">The result converted.</param>
/// <returns>true if tests passed and converts succeeded; otherwise, false.</returns>
public delegate bool JsonSwitchPredicate<T>(IJsonValueNode node, out T result);

/// <summary>
/// Tries to get the result from a JSON node.
/// </summary>
/// <typeparam name="TResult">The type of result.</typeparam>
/// <typeparam name="TInfo">The type of additional info to return.</typeparam>
/// <param name="node">The JSON node to test and convert.</param>
/// <param name="result">The result converted.</param>
/// <param name="info">The additional info to return.</param>
/// <returns>true if tests passed and converts succeeded; otherwise, false.</returns>
public delegate bool JsonSwitchPredicate<TResult, TInfo>(IJsonValueNode node, out TResult result, out TInfo info);

/// <summary>
/// Tries to get the result from a JSON node.
/// </summary>
/// <typeparam name="TArgs">The type of args.</typeparam>
/// <typeparam name="TResult">The type of result.</typeparam>
/// <param name="node">The JSON node to test and convert.</param>
/// <param name="args">The args.</param>
/// <param name="result">The result converted.</param>
/// <returns>true if tests passed and converts succeeded; otherwise, false.</returns>
public delegate bool JsonSwitchArgsPredicate<TArgs, TResult>(IJsonValueNode node, TArgs args, out TResult result);

/// <summary>
/// Tries to get the result from a JSON node.
/// </summary>
/// <typeparam name="TArgs">The type of args.</typeparam>
/// <typeparam name="TResult">The type of result.</typeparam>
/// <typeparam name="TInfo">The type of additional info to return.</typeparam>
/// <param name="node">The JSON node to test and convert.</param>
/// <param name="args">The args.</param>
/// <param name="result">The result converted.</param>
/// <param name="info">The additional info to return.</param>
/// <returns>true if tests passed and converts succeeded; otherwise, false.</returns>
public delegate bool JsonSwitchArgsPredicate<TArgs, TResult, TInfo>(IJsonValueNode node, TArgs args, out TResult result, out TInfo info);

/// <summary>
/// The interface of JSON switch context info bag.
/// </summary>
public interface IJsonSwitchContextInfo
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the creation date time.
    /// </summary>
    public DateTime CreationTime { get; }

    /// <summary>
    /// Gets the latest test time.
    /// </summary>
    public DateTime LatestTestTime { get; }

    /// <summary>
    /// Gets the count of cases run.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the args.
    /// </summary>
    public object Args { get; }

    /// <summary>
    /// Gets the JSON node source.
    /// </summary>
    public IJsonValueNode Source { get; }

    /// <summary>
    /// Gets or sets the additional tag.
    /// </summary>
    public object Tag { get; set; }

    /// <summary>
    /// Gets a value indicating whether the switch is passed all cases.
    /// </summary>
    public bool IsPassed { get; }
}

/// <summary>
/// The interface of JSON switch context info bag.
/// </summary>
public interface IJsonSwitchContextInfo<T> : IJsonSwitchContextInfo
{
    /// <summary>
    /// Gets or sets the args.
    /// </summary>
    public new T Args { get; set; }
}
