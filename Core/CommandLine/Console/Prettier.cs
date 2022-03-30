using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.CommandLine;

/// <summary>
/// The prettier for console text.
/// </summary>
public interface IConsoleTextPrettier
{
    /// <summary>
    /// Creates the console text collection based on this style.
    /// </summary>
    /// <param name="s">The text.</param>
    /// <returns>A collection of console text.</returns>
    IEnumerable<ConsoleText> CreateTextCollection(string s);
}

/// <summary>
/// The console text creator.
/// </summary>
public interface IConsoleTextCreator
{
    /// <summary>
    /// Gets a value indicating whether it has already contained a line terminator.
    /// </summary>
    bool ContainsTerminator { get; }

    /// <summary>
    /// Creates the console text collection based on this style.
    /// </summary>
    /// <returns>A collection of console text.</returns>
    IEnumerable<ConsoleText> CreateTextCollection();
}

/// <summary>
/// The console text creator for the specific data model.
/// </summary>
/// <typeparam name="T">The type of data model.</typeparam>
public interface IConsoleTextCreator<T>
{
    /// <summary>
    /// Gets a value indicating whether it has already contained a line terminator.
    /// </summary>
    bool ContainsTerminator { get; }

    /// <summary>
    /// Creates the console text collection based on this style.
    /// </summary>
    /// <param name="data">The data model to output.</param>
    /// <returns>A collection of console text.</returns>
    IEnumerable<ConsoleText> CreateTextCollection(T data);
}

/// <summary>
/// The console text creator for the specific data model.
/// </summary>
/// <typeparam name="TData">The type of data model.</typeparam>
/// <typeparam name="TOptions">The additional options.</typeparam>
public interface IConsoleTextCreator<TData, TOptions>
{
    /// <summary>
    /// Gets a value indicating whether it has already contained a line terminator.
    /// </summary>
    bool ContainsTerminator { get; }

    /// <summary>
    /// Creates the console text collection based on this style.
    /// </summary>
    /// <param name="data">The data model to output.</param>
    /// <param name="options">The additional options.</param>
    /// <returns>A collection of console text.</returns>
    IEnumerable<ConsoleText> CreateTextCollection(TData data, TOptions options);
}
