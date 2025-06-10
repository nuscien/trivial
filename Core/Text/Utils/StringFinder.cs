using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Trivial.Collection;

namespace Trivial.Text;

/// <summary>
/// The rules to match the string in a collection.
/// </summary>
public enum StringsMatchingRules : byte
{
    /// <summary>
    /// In sequence of the given collection.
    /// </summary>
    Sequence = 0,

    /// <summary>
    /// The smallest index to match if has.
    /// </summary>
    Front = 1,

    /// <summary>
    /// The largest index to match if has.
    /// </summary>
    Rear = 2,

    /// <summary>
    /// None.
    /// </summary>
    None = 7,
}

/// <summary>
/// The string finder used to find a sub-string in a source string.
/// </summary>
[DebuggerDisplay("{Offset} → {Value} → {RestLength}")]
public class StringFinder
{
#pragma warning disable IDE0057
    /// <summary>
    /// Initializes a new instance of the StringFinder class.
    /// </summary>
    /// <param name="s">The source string.</param>
    public StringFinder(string s)
    {
        Rest = Source = s ?? string.Empty;
        Value = Previous = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the StringFinder class.
    /// </summary>
    /// <param name="s">The source string.</param>
    public StringFinder(StringBuilder s)
        : this(s?.ToString())
    {
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
    /// Gets the sub-string before a search string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="offset">The zero-based offset.</param>
    /// <returns>The sub-string.</returns>
    public string PreviewBefore(string q, out int offset)
    {
        if (string.IsNullOrEmpty(q))
        {
            offset = Offset;
            return string.Empty;
        }

        var s = Rest;
        var index = s.IndexOf(q);
        if (index < 0)
        {
            Value = string.Empty;
            offset = Source.Length;
            return string.Empty;
        }

        var value = Rest.Substring(0, index);
        offset = Offset + index;
        return value;
    }

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
    /// <param name="q">The search string.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <param name="offset">The zero-based offset.</param>
    /// <returns>The sub-string.</returns>
    public string Before(char q, bool skip, out int offset)
        => Before(q.ToString(), skip, out offset);

    /// <summary>
    /// Gets the sub-string before a search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="offset">The zero-based offset.</param>
    /// <returns>The sub-string.</returns>
    public string Before(char q, out int offset)
        => Before(q, false, out offset);

    /// <summary>
    /// Gets the sub-string before a search string. The search string is in the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <returns>The sub-string.</returns>
    public string Before(char q, bool skip = false)
        => Before(q, skip, out _);

    /// <summary>
    /// Gets the sub-string before a search string. The search string is in the rest string.
    /// </summary>
    /// <param name="index">The zero-based index of the search string in the rest string.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <returns>The sub-string.</returns>
    public string Before(int index, bool skip = false)
    {
        Previous = Value;
        if (index == 0)
        {
            if (skip && Rest.Length > 0)
            {
                Offset++;
                Rest = Rest.Substring(1);
            }

            return Value = string.Empty;
        }

        Offset += index;
        if (index < 0)
        {
            if (Offset < 0) Offset = 0;
            Rest = Source.Substring(Offset);
            if (skip && Rest.Length > 0)
            {
                Offset++;
                Rest = Rest.Substring(1);
            }

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
        if (skip)
        {
            index++;
            Offset++;
        }

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
    /// <param name="rule">The matching rule.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <param name="value">The sub-string.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">rule is not supported.</exception>
    public bool BeforeIfContains(IEnumerable<string> q, StringsMatchingRules rule, bool skip, out string value)
    {
        if (q == null)
        {
            value = null;
            return false;
        }

        switch (rule)
        {
            case StringsMatchingRules.Sequence:
                foreach (var item in q)
                {
                    if (BeforeIfContains(item, skip, out value)) return true;
                }

                break;
            case StringsMatchingRules.None:
                break;
            case StringsMatchingRules.Front:
                {
                    var col = IndexOf(q).Where(i => i >= 0).ToList();
                    if (col.Count < 1) break;
                    value = Before(col.Min(), skip);
                    return true;
                }
            case StringsMatchingRules.Rear:
                {
                    var col = IndexOf(q).Where(i => i >= 0).ToList();
                    if (col.Count < 1) break;
                    value = Before(col.Max(), skip);
                    return true;
                }
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
    /// <param name="rule">The matching rule.</param>
    /// <param name="value">The sub-string.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">rule is not supported.</exception>
    public bool BeforeIfContains(IEnumerable<string> q, StringsMatchingRules rule, out string value)
        => BeforeIfContains(q, rule, false, out value);

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
    /// <param name="rule">The matching rule.</param>
    /// <param name="skip">true if skip the search string then; otherwise, false.</param>
    /// <returns>true if the rest string contains the search string; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">rule is not supported.</exception>
    public bool BeforeIfContains(IEnumerable<string> q, StringsMatchingRules rule = StringsMatchingRules.Sequence, bool skip = false)
        => BeforeIfContains(q, rule, skip, out _);

    /// <summary>
    /// Read next character.
    /// </summary>
    /// <returns>The character; or \0 if at the end.</returns>
    public char Read()
        => Read(out var c) ? c : '\0';

    /// <summary>
    /// Read next character.
    /// </summary>
    /// <returns>The character; or \0 if at the end.</returns>
    public bool Read(out char c)
    {
        var rest = Rest;
        if (string.IsNullOrEmpty(rest))
        {
            c = '\0';
            return false;
        }

        Previous = Value;
        c = rest[0];
        Value = c.ToString();
        Offset++;
        Rest = rest.Substring(1);
        return true;
    }

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

    /// <summary>
    /// Tests a value indicating whether a specific string occurs within the rest string.
    /// </summary>
    /// <param name="q">The search string.</param>
    /// <returns>true if contains the search string in the rest string; otherwise, false.</returns>
    public bool Contains(char q)
        => Rest.Contains(q);

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified string in the rest string.
    /// </summary>
    /// <param name="q">The string to seek.</param>
    /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
    public int IndexOf(string q)
        => Rest.IndexOf(q);

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified string in the rest string.
    /// </summary>
    /// <param name="q">The string to seek.</param>
    /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
    public int IndexOf(char q)
        => Rest.IndexOf(q);

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified string in the rest string.
    /// </summary>
    /// <param name="q">The string to seek.</param>
    /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
    public IEnumerable<int> IndexOf(IEnumerable<string> q)
    {
        foreach (var item in q)
        {
            yield return Rest.IndexOf(item);
        }
    }

    /// <summary>
    /// Determines whether the beginning of this string instance matches the specified string.
    /// </summary>
    /// <param name="q">The string to compare.</param>
    /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public bool StartsWith(string q)
        => Rest.StartsWith(q);

    /// <summary>
    /// Determines whether the beginning of this string instance matches the specified character.
    /// </summary>
    /// <param name="q">The character to compare.</param>
    /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public bool StartsWith(char q)
        => Rest.StartsWith(q);

    /// <summary>
    /// Resets the state.
    /// </summary>
    public void Reset()
    {
        Offset = 0;
        Rest = Source;
        Value = Previous = string.Empty;
    }
#pragma warning restore IDE0057
}
