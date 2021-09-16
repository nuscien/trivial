using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.CommandLine
{
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
    /// The console text creator for the specific data model.
    /// </summary>
    /// <typeparam name="T">The type of data model.</typeparam>
    public interface IConsoleTextCreator<T>
    {
        /// <summary>
        /// Creates the console text collection based on this style.
        /// </summary>
        /// <param name="data">The data model to output.</param>
        /// <returns>A collection of console text.</returns>
        IEnumerable<ConsoleText> CreateTextCollection(T data);
    }
}
