using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

internal class StringFinder
{
#pragma warning disable IDE0057
    /// <summary>
    /// Initializes a new instance of the StringFinder class.
    /// </summary>
    /// <param name="s">The source string.</param>
    public StringFinder(string s)
    {
        Rest = Source = s ?? string.Empty;
    }

    /// <summary>
    /// Gets the source string.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Gets the rest string.
    /// </summary>
    public string Rest { get; private set; }

    /// <summary>
    /// Gets the sub-string to get for current request.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the rest is empty.
    /// </summary>
    public bool IsEnd => string.IsNullOrEmpty(Rest);

    /// <summary>
    /// Gets the sub-string before a search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="index">The zero-based index of the search string in the source string.</param>
    /// <returns>The sub-string.</returns>
    public string Before(string q, out int index)
    {
        var s = Rest;
        index = s.IndexOf(q);
        if (index < 0) return string.Empty;
        var value = Rest.Substring(0, index);
        Rest = Rest.Substring(index);
        return Value = value;
    }

    /// <summary>
    /// Gets the sub-string before a search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <returns>The sub-string.</returns>
    public string Before(string q)
        => Before(q, out _);

    /// <summary>
    /// Gets the sub-string until that a search string appears. The search string is in the sub-string returned.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="index">The zero-based index of the search string in the source string.</param>
    /// <returns>The sub-string.</returns>
    public string Until(string q, out int index)
    {
        var s = Rest;
        index = s.IndexOf(q) + q.Length;
        if (index < 0) return string.Empty;
        var value = Rest.Substring(0, index);
        Rest = Rest.Substring(index);
        return Value = value;
    }

    /// <summary>
    /// Gets the sub-string until that a search string appears. The search string is in the sub-string returned.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <returns>The sub-string.</returns>
    public string Until(string q)
        => Until(q, out _);
#pragma warning restore IDE0057
}
