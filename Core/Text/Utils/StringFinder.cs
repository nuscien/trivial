using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        Value = string.Empty;
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
    /// Gets the previous sub-string.
    /// </summary>
    public string Previous { get; private set; }

    /// <summary>
    /// Gets the sub-string to get for current request.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets the offset of the string.
    /// </summary>
    public int Offset { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the rest is empty.
    /// </summary>
    public bool IsEnd => string.IsNullOrEmpty(Rest);

    /// <summary>
    /// Gets the length of rest string.
    /// </summary>
    public int RestLength => Rest.Length;

    /// <summary>
    /// Gets the sub-string before a search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <param name="offset">The zero-based offset.</param>
    /// <returns>The sub-string.</returns>
    public string Before(string q, bool skip, out int offset)
    {
        if (string.IsNullOrEmpty(q))
        {
            offset = Offset;
            return string.Empty;
        }

        Previous = Value;
        var s = Rest;
        var index = s.IndexOf(q);
        if (index < 0)
        {
            Value = string.Empty;
            offset = Offset = Source.Length;
            return string.Empty;
        }

        var value = Rest.Substring(0, index);
        if (skip) index += q.Length;
        offset = Offset + index;
        Offset = offset;
        Rest = Rest.Substring(index);
        return Value = value;
    }

    /// <summary>
    /// Gets the sub-string before a search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="offset">The zero-based offset.</param>
    /// <returns>The sub-string.</returns>
    public string Before(string q, out int offset)
        => Before(q, false, out offset);

    /// <summary>
    /// Gets the sub-string before a search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <returns>The sub-string.</returns>
    public string Before(string q, bool skip = false)
        => Before(q, skip, out _);

    /// <summary>
    /// Gets the sub-string before a search string. The search string is in the rest string.
    /// </summary>
    /// <param name="index">The zero-based index of the search string in the rest string.</param>
    /// <returns>The sub-string.</returns>
    public string Before(int index)
    {
        if (index == 0) return string.Empty;
        Offset += index;
        Previous = Value;
        if (index < 0)
        {
            if (Offset < 0) Offset = 0;
            Rest = Source.Substring(Offset);
            return Value = string.Empty;
        }

        if (index >= Rest.Length)
        {
            Offset = Source.Length;
            var rest = Rest;
            Rest = string.Empty;
            return Value = rest;
        }

        var value = Rest.Substring(0, index);
        Rest = Rest.Substring(index);
        return Value = value;
    }

    /// <summary>
    /// Moves offsets before a specific search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <param name="value">The sub-string.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    public bool BeforeIfContains(string q, bool skip, out string value)
    {
        if (string.IsNullOrEmpty(q))
        {
            value = null;
            return false;
        }

        Previous = Value;
        var s = Rest;
        var index = s.IndexOf(q);
        if (index < 0)
        {
            value = null;
            return false;
        }

        value = Value = Rest.Substring(0, index);
        if (skip) index += q.Length;
        Offset += index;
        Rest = Rest.Substring(index);
        return true;
    }

    /// <summary>
    /// Moves offsets before a specific search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string collection.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <param name="value">The sub-string.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    public bool BeforeIfContains(IEnumerable<string> q, bool skip, out string value)
    {
        if (q == null)
        {
            value = null;
            return false;
        }

        foreach (var item in q)
        {
            if (BeforeIfContains(item, skip, out value)) return true;
        }

        value = null;
        return false;
    }

    /// <summary>
    /// Moves offsets before a specific search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="value">The sub-string.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    public bool BeforeIfContains(string q, out string value)
        => BeforeIfContains(q, false, out value);

    /// <summary>
    /// Moves offsets before a specific search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="value">The sub-string.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    public bool BeforeIfContains(IEnumerable<string> q, out string value)
        => BeforeIfContains(q, false, out value);

    /// <summary>
    /// Moves offsets before a specific search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    public bool BeforeIfContains(string q, bool skip = false)
        => BeforeIfContains(q, skip, out _);

    /// <summary>
    /// Moves offsets before a specific search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string collection.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    public bool BeforeIfContains(IEnumerable<string> q, bool skip = false)
        => BeforeIfContains(q, skip, out _);

    /// <summary>
    /// Gets the sub-string until that a search string appears. The search string is in the sub-string returned.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="offset">The zero-based offset.</param>
    /// <returns>The sub-string.</returns>
    public string Until(string q, out int offset)
    {
        var s = Rest;
        var index = s.IndexOf(q) + q.Length;
        Previous = Value;
        if (index < 0)
        {
            Value = string.Empty;
            offset = Offset = Source.Length;
            return string.Empty;
        }

        var value = Rest.Substring(0, index);
        offset = Offset + index;
        Offset = offset;
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

    /// <summary>
    /// Moves offsets until a specific search string appeared. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="value">The sub-string.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    public bool UntilIfContains(string q, out string value)
    {
        var s = Rest;
        var index = s.IndexOf(q) + q.Length;
        Previous = Value;
        if (index < 0)
        {
            value = string.Empty;
            return false;
        }

        value = Value = Rest.Substring(0, index);
        Offset += index;
        Rest = Rest.Substring(index);
        return true;
    }

    /// <summary>
    /// Moves offsets until a specific search string appeared. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    public bool UntilIfContains(string q)
        => UntilIfContains(q, out _);

    /// <summary>
    /// Clears the value to empty string.
    /// </summary>
    public void ClearValue()
    {
        Previous = Value;
        Value = string.Empty;
    }

    /// <summary>
    /// Tests a value indicating whether a specific string occurs within the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <returns>true if contains the search string in the rest string; otherwise, false.</returns>
    public bool Contains(string q)
        => Rest.Contains(q);
#pragma warning restore IDE0057
}
