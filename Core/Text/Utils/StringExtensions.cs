﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trivial.Text;

/// <summary>
/// Letter cases.
/// </summary>
public enum Cases : byte
{
    /// <summary>
    /// Keep original.
    /// </summary>
    Original = 0,

    /// <summary>
    /// Uppercase.
    /// </summary>
    Upper = 1,

    /// <summary>
    /// Lowercase.
    /// </summary>
    Lower = 2,

    /// <summary>
    /// First letter uppercase and rest keeping original.
    /// </summary>
    Capitalize = 3,

    /// <summary>
    /// First letter lowercase and rest keeping original.
    /// </summary>
    Uncapitalize = 4
}

/// <summary>
/// The string extension and helper.
/// </summary>
public static class StringExtensions
{
#pragma warning disable IDE0056, IDE0057
    /// <summary>
    /// Special characters of YAML.
    /// </summary>
    internal static readonly char[] YamlSpecialChars = new[] { ':', '\r', '\n', '\\', '\'', '\"', '\t', ' ', '#', '.', '[', '{', '\\', '/', '@' };

    /// <summary>
    /// Gets the MIME value of plain text format text.
    /// </summary>
    public const string PlainTextMIME = "text/plain";

    /// <summary>
    /// Gets the MIME value of plain text format text.
    /// </summary>
    public const string RichTextMIME = "text/richtext";

    /// <summary>
    /// Returns a copy of this string converted to specific case, using the casing rules of the specified culture.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="options">The specific case.</param>
    /// <param name="culture">An object that supplies culture-specific casing rules.</param>
    /// <returns>The specific case equivalent of the current string.</returns>
    /// <exception cref="ArgumentNullException">culture is null.</exception>
    public static string ToSpecificCase(this string source, Cases options, CultureInfo culture = null)
    {
        if (string.IsNullOrWhiteSpace(source)) return source;
        switch (options)
        {
            case Cases.Original:
                return source;
            case Cases.Upper:
                return ToUpper(source, culture);
            case Cases.Lower:
                return ToLower(source, culture);
            case Cases.Capitalize:
                {
                    var s = source.TrimStart();
                    return $"{source.Substring(0, source.Length - s.Length)}{ToUpper(s.Substring(0, 1), culture)}{s.Substring(1)}";
                }
            case Cases.Uncapitalize:
                {
                    var s = source.TrimStart();
                    return $"{source.Substring(0, source.Length - s.Length)}{ToLower(s.Substring(0, 1), culture)}{s.Substring(1)}";
                }
            default:
                return source;
        }
    }

    /// <summary>
    /// Returns a copy of this string converted to specific case, using the casing rules of the invariant culture.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="options">The specific case.</param>
    /// <returns>The specific case equivalent of the current string.</returns>
    public static string ToSpecificCaseInvariant(this string source, Cases options)
        => ToSpecificCase(source, options, CultureInfo.InvariantCulture);

    /// <summary>
    /// Breaks lines.
    /// </summary>
    /// <param name="text">The original string.</param>
    /// <param name="length">The count of a line.</param>
    /// <param name="newLine">The optional newline string.</param>
    /// <returns>A new text with line break.</returns>
    public static string BreakLines(string text, int length, string newLine = null)
    {
        var idx = 0;
        var len = text.Length;
        var str = new StringBuilder();
        while (idx < len)
        {
            if (idx > 0)
            {
                str.Append(newLine ?? Environment.NewLine);
            }

            if (idx + length >= len)
            {
                str.Append(text.Substring(idx));
            }
            else
            {
                str.Append(text.Substring(idx, length));
            }

            idx += length;
        }

        return str.ToString();
    }

    /// <summary>
    /// Breaks lines.
    /// </summary>
    /// <param name="text">The original string.</param>
    /// <param name="length">The count of a line.</param>
    /// <param name="newLine">The newline character.</param>
    /// <returns>A new text with line break.</returns>
    public static string BreakLines(string text, int length, char newLine)
    {
        var idx = 0;
        var len = text.Length;
        var str = new StringBuilder();
        while (idx < len)
        {
            if (idx > 0)
            {
                str.Append(newLine);
            }

            if (idx + length >= len)
            {
                str.Append(text.Substring(idx));
            }
            else
            {
                str.Append(text.Substring(idx, length));
            }

            idx += length;
        }

        return str.ToString();
    }

    /// <summary>
    /// Splits a string into a number of substrings based on the string in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="seperator">A string that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, string seperator, StringSplitOptions options = StringSplitOptions.None)
    {
        if (options == StringSplitOptions.None && string.IsNullOrEmpty(source)) yield break;
        if (options == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrWhiteSpace(source)) yield break;
        if (string.IsNullOrEmpty(seperator))
        {
            yield return source;
            yield break;
        }

        var index = 0;
        if (options == StringSplitOptions.None)
        {
            while (true)
            {
                var i = source.IndexOf(seperator, index);
                if (i < 0)
                {
                    yield return source.Substring(index);
                    break;
                }

                yield return source.SubRangeString(index, i);
                index = i + seperator.Length;
                if (index >= source.Length) break;
            }
        }
        else
        {
            while (true)
            {
                var i = source.IndexOf(seperator, index);
                if (i < 0)
                {
                    yield return source.Substring(index);
                    break;
                }

                if (i <= index) continue;
                yield return source.SubRangeString(index, i);
                index = i + seperator.Length;
                if (index >= source.Length) break;
            }
        }
    }

    /// <summary>
    /// Splits a string into a number of substrings based on the character in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="seperator">A character that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, char seperator, StringSplitOptions options = StringSplitOptions.None)
        => YieldSplit(source, new[] { seperator }, options);

    /// <summary>
    /// Splits a string into a number of substrings based on the characters in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="seperatorA">A character that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="seperatorB">Another character that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, char seperatorA, char seperatorB, StringSplitOptions options = StringSplitOptions.None)
        => YieldSplit(source, new[] { seperatorA, seperatorB }, options);

    /// <summary>
    /// Splits a string into a number of substrings based on the characters in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="seperatorA">The character A that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="seperatorB">The character B that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="seperatorC">The character C that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, char seperatorA, char seperatorB, char seperatorC, StringSplitOptions options = StringSplitOptions.None)
        => YieldSplit(source, new[] { seperatorA, seperatorB, seperatorC }, options);

    /// <summary>
    /// Splits a string into a number of substrings based on the characters in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="seperatorA">The character A that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="seperatorB">The character B that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="seperatorC">The character C that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="seperatorD">The character D that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, char seperatorA, char seperatorB, char seperatorC, char seperatorD, StringSplitOptions options = StringSplitOptions.None)
        => YieldSplit(source, new[] { seperatorA, seperatorB, seperatorC, seperatorD }, options);

    /// <summary>
    /// Splits a string into a number of substrings based on the characters in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="seperators">A character array that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, char[] seperators, StringSplitOptions options = StringSplitOptions.None)
    {
        if (options == StringSplitOptions.None && string.IsNullOrEmpty(source)) yield break;
        if (options == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrWhiteSpace(source)) yield break;
        if (seperators == null || seperators.Length == 0)
        {
            yield return source;
            yield break;
        }

        var index = 0;
        if (options == StringSplitOptions.None)
        {
            while (true)
            {
                var i = source.IndexOfAny(seperators, index);
                if (i < 0)
                {
                    yield return source.Substring(index);
                    break;
                }

                yield return source.SubRangeString(index, i);
                index = i + 1;
                if (index >= source.Length) break;
            }
        }
        else
        {
            while (true)
            {
                var i = source.IndexOfAny(seperators, index);
                if (i < 0)
                {
                    yield return source.Substring(index);
                    break;
                }

                if (i <= index) continue;
                yield return source.SubRangeString(index, i);
                index = i + 1;
                if (index >= source.Length) break;
            }
        }
    }

    /// <summary>
    /// Splits a string into a number of substrings based on the strings in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="seperators">A string array that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, IEnumerable<string> seperators, StringSplitOptions options = StringSplitOptions.None)
    {
        if (options == StringSplitOptions.None && string.IsNullOrEmpty(source)) yield break;
        if (options == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrWhiteSpace(source)) yield break;
        if (seperators == null)
        {
            yield return source;
            yield break;
        }

        var index = 0;
        if (options == StringSplitOptions.None)
        {
            while (true)
            {
                var indexes = seperators.Select(ele => (Index: source.IndexOf(ele, index), ele.Length)).Where(ele => ele.Index >= 0).ToList();
                if (indexes.Count == 0)
                {
                    yield return source.Substring(index);
                    break;
                }

                var i = indexes.Min(ele => ele.Index);
                var len = indexes.Where(ele => ele.Index == i).Max(ele => ele.Length);
                yield return source.SubRangeString(index, i);
                index = i + len;
                if (index >= source.Length) break;
            }
        }
        else
        {
            while (true)
            {
                var indexes = seperators.Select(ele => (Index: source.IndexOf(ele, index), ele.Length)).Where(ele => ele.Index >= 0).ToList();
                if (indexes.Count == 0)
                {
                    yield return source.Substring(index);
                    break;
                }

                var i = indexes.Min(ele => ele.Index);
                if (i <= index) continue;
                var len = indexes.Where(ele => ele.Index == i).Max(ele => ele.Length);
                yield return source.SubRangeString(index, i);
                index = i + len;
                if (index >= source.Length) break;
            }
        }
    }

    /// <summary>
    /// Reads lines from a string.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific string.</returns>
    public static IEnumerable<string> ReadLines(string source, bool removeEmptyLine = false)
        => YieldSplit(source, new[] { "\r\n", "\n", "\r" }, removeEmptyLine ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

    /// <summary>
    /// Reads lines from a specific stream reader.
    /// </summary>
    /// <param name="reader">The stream reader.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific string.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<string> ReadLines(TextReader reader, bool removeEmptyLine = false)
        => IO.CharsReader.ReadLines(reader, removeEmptyLine);

    /// <summary>
    /// Gets the indent string.
    /// </summary>
    /// <param name="indentStyle">The indent style.</param>
    /// <param name="indentLevel">The current indent level.</param>
    /// <returns>A string.</returns>
    public static string GetString(IndentStyles indentStyle, int indentLevel = 1)
    {
        if (indentLevel < 1) return indentLevel == 0 ? string.Empty : null;
        var str = indentStyle switch
        {
            IndentStyles.Minified => string.Empty,
            IndentStyles.Empty => string.Empty,
            IndentStyles.Tab => "\t",
            IndentStyles.Space => " ",
            IndentStyles.Compact => "  ",
            IndentStyles.Wide => "        ",
            _ => "    "
        };
        if (indentLevel < 1) return str;
        var sb = new StringBuilder(str);
        for (var i = 1; i < indentLevel; i++)
        {
            sb.Append(str);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Determines whether the beginning of the string matches the specified string.
    /// </summary>
    /// <param name="str">The input string.</param>
    /// <param name="value">The string to compare.</param>
    /// <param name="rest">The rest string after the value.</param>
    /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool StartWith(string str, string value, out string rest)
    {
        if (value == null) throw new ArgumentNullException(nameof(value), "value is null.");
        if (str?.StartsWith(value) != true)
        {
            rest = str;
            return false;
        }

        rest = value.Substring(str.Length);
        return true;
    }

    /// <summary>
    /// Determines whether the beginning of the string matches the specified string.
    /// </summary>
    /// <param name="str">The input string.</param>
    /// <param name="value">The string to compare.</param>
    /// <param name="comparisonType">One of the enumeration values that determines how this string and value are compared.</param>
    /// <param name="rest">The rest string after the value.</param>
    /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
    /// <exception cref="ArgumentException">comparisonType is not a System.StringComparison value.</exception>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool StartWith(string str, string value, StringComparison comparisonType, out string rest)
    {
        if (value == null) throw new ArgumentNullException(nameof(value), "value is null.");
        if (str?.StartsWith(value, comparisonType) != true)
        {
            rest = str;
            return false;
        }

        rest = value.Substring(str.Length);
        return true;
    }

    /// <summary>
    /// Gets the first substring between two keywords.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="start">The start keyword; or null, if start from begin.</param>
    /// <param name="end">The end keword; or null, if end to last.</param>
    /// <param name="include">true if output includes the keywords; otherwise, false.</param>
    /// <param name="startIndex">The start index to search.</param>
    /// <returns>The substring.</returns>
    public static string Between(string s, string start, string end, bool include = false, int startIndex = 0)
    {
        if (string.IsNullOrEmpty(s)) return s;
        var i = string.IsNullOrEmpty(start) ? startIndex : s.IndexOf(start, startIndex);
        if (i < 0) return string.Empty;
        if (include)
        {
            s = s.Substring(i);
            if (string.IsNullOrEmpty(end)) return s;
            i = s.IndexOf(end, start.Length);
            return i < 0 ? s : s.Substring(0, i + end.Length);
        }

        s = s.Substring(i + start.Length);
        if (string.IsNullOrEmpty(end)) return s;
        i = s.IndexOf(end);
        return i < 0 ? s : s.Substring(0, i);
    }

    /// <summary>
    /// Gets the first substring between two keywords.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="start">The start keyword; or null, if start from begin.</param>
    /// <param name="end">The end keword; or null, if end to last.</param>
    /// <param name="comparisonType">One of the enumeration values that determines how this string and value are compared.</param>
    /// <param name="include">true if output includes the keywords; otherwise, false.</param>
    /// <param name="startIndex">The start index to search.</param>
    /// <returns>The substring.</returns>
    public static string Between(string s, string start, string end, StringComparison comparisonType, bool include = false, int startIndex = 0)
    {
        if (string.IsNullOrEmpty(s)) return s;
        var i = string.IsNullOrEmpty(start) ? startIndex : s.IndexOf(start, startIndex, comparisonType);
        if (i < 0) return string.Empty;
        if (include)
        {
            s = s.Substring(i);
            if (string.IsNullOrEmpty(end)) return s;
            i = s.IndexOf(end, start.Length, comparisonType);
            return i < 0 ? s : s.Substring(0, i + end.Length);
        }

        s = s.Substring(i + start.Length);
        if (string.IsNullOrEmpty(end)) return s;
        i = s.IndexOf(end, comparisonType);
        return i < 0 ? s : s.Substring(0, i);
    }

    /// <summary>
    /// Gets the first item which is not empty.
    /// </summary>
    /// <param name="values">The string values.</param>
    /// <returns>The first item which is not empty; or null if no match.</returns>
    public static string GetIfNotEmpty(params string[] values)
    {
        if (values == null) return null;
        foreach (var item in values)
        {
            if (!string.IsNullOrEmpty(item)) return item;
        }

        return null;
    }

    /// <summary>
    /// Gets the first item which is not empty.
    /// </summary>
    /// <param name="values">The string values.</param>
    /// <returns>The first item which is not empty; or null if no match.</returns>
    public static string GetIfNotEmpty(IEnumerable<string> values)
    {
        if (values == null) return null;
        foreach (var item in values)
        {
            if (!string.IsNullOrEmpty(item)) return item;
        }

        return null;
    }

    /// <summary>
    /// Gets the first item which is not empty after trimming.
    /// </summary>
    /// <param name="values">The string values.</param>
    /// <returns>The first item which is not empty after trimming; or null if no match.</returns>
    public static string GetIfNotEmptyTrimmed(params string[] values)
    {
        if (values == null) return null;
        string s;
        foreach (var item in values)
        {
            s = item?.Trim();
            if (!string.IsNullOrEmpty(s)) return s;
        }

        return null;
    }

    /// <summary>
    /// Gets the first item which is not empty after trimming.
    /// </summary>
    /// <param name="values">The string values.</param>
    /// <returns>The first item which is not empty after trimming; or null if no match.</returns>
    public static string GetIfNotEmptyTrimmed(IEnumerable<string> values)
    {
        if (values == null) return null;
        string s;
        foreach (var item in values)
        {
            s = item?.Trim();
            if (!string.IsNullOrEmpty(s)) return s;
        }

        return null;
    }

    /// <summary>
    /// Tries to parse a URI.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>The URI.</returns>
    public static Uri TryCreateUri(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        try
        {
            return new Uri(url, UriKind.RelativeOrAbsolute);
        }
        catch (UriFormatException)
        {
        }
        catch (ArgumentException)
        {
        }

        return null;
    }

    /// <summary>
    /// Tries to parse a URI.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="kind">Specifies whether the URI string is a relative URI, absolute URI, or is indeterminate.</param>
    /// <returns>The URI.</returns>
    public static Uri TryCreateUri(string url, UriKind kind)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        try
        {
            return new Uri(url, kind);
        }
        catch (UriFormatException)
        {
        }
        catch (ArgumentException)
        {
        }

        return null;
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <param name="memberInfo">The member information.</param>
    /// <returns>The description.</returns>
    public static string GetDescription(MemberInfo memberInfo)
    {
        try
        {
            var attr = memberInfo.GetCustomAttributes<DescriptionAttribute>()?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(attr?.Description)) return attr.Description;
        }
        catch (NotSupportedException)
        {
        }
        catch (TypeLoadException)
        {
        }

        return null;
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <param name="parameterInfo">The parameter information.</param>
    /// <returns>The description.</returns>
    public static string GetDescription(ParameterInfo parameterInfo)
    {
        try
        {
            var attr = parameterInfo.GetCustomAttributes<DescriptionAttribute>()?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(attr.Description)) return attr.Description;
        }
        catch (NotSupportedException)
        {
        }
        catch (TypeLoadException)
        {
        }

        return null;
    }

    internal static StringBuilder Append(StringBuilder sb, StringBuilder value)
    {
        if (value != null)
#if NETFRAMEWORK
            sb.Append(value.ToString());
#else
            sb.Append(value);
#endif
        return sb;
    }

    internal static string ToString(char[] value, int start = 0, int? count = null)
    {
        if (start == 0 && count == null)
            return new string(value);
        var list = value.Skip(start);
        if (count.HasValue) list = list.Take(count.Value);
        return new string(list.ToArray());
    }

#if NETFRAMEWORK
    internal static bool StartsWith(this string s, char value)
        => s.StartsWith(value.ToString());

    internal static bool EndsWith(this string s, char value)
        => s.EndsWith(value.ToString());

    internal static bool Contains(this string s, char value)
        => s.Contains(value.ToString());
#endif

    internal static string Join(char value, IEnumerable<string> col)
#if NETFRAMEWORK
        => string.Join(value.ToString(), col);
#else
        => string.Join(value, col);
#endif

    internal static string SubRangeString(this string s, int start, int end, bool reverseEnd = false)
    {
#if NETFRAMEWORK
        return s.Substring(start, reverseEnd ? (s.Length - end - start) : (end - start));
#else
        return reverseEnd ? s[start..^end] : s[start..end];
#endif
    }

    internal static bool IsLast(string s, char last, char except, bool moreThanOne)
        => moreThanOne
            ? s.Length > 1 && s[s.Length - 1] == last && (s.Length - s.Substring(0, s.Length - 1).TrimEnd(except).Length) % 2 > 0
            : s.Length > 0 && s[s.Length - 1] == last && (s.Length == 1 || (s.Length - s.Substring(0, s.Length - 1).TrimEnd(except).Length) % 2 > 0);

    /// <summary>
    /// Serializes an object into JSON format.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="options">The optional serializer settings.</param>
    /// <returns>A JSON string.</returns>
    internal static string ToJson(object obj, JsonSerializerOptions options = null)
        => ToJson(obj, (o, t) =>
        {
            return JsonSerializer.Serialize(o, t, options);
        });

#if NETFRAMEWORK
    internal static StringBuilder Append(this StringBuilder sb, char value)
        => sb.Append(value, 1);
#endif

    /// <summary>
    /// Serializes an object into JSON format.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="options">The optional serializer settings.</param>
    /// <returns>A JSON string.</returns>
    internal static string ToJson(object obj, DataContractJsonSerializerSettings options)
        => ToJson(obj, (o, t) =>
        {
            var serializer = options != null ? new DataContractJsonSerializer(t, options) : new DataContractJsonSerializer(t);
            using var stream = new MemoryStream();
            serializer.WriteObject(stream, o);
            stream.Position = 0;
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            return Encoding.UTF8.GetString(bytes);
        });

    /// <summary>
    /// Serializes an object into JSON format.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="converter">The fallback converter.</param>
    /// <returns>A JSON string.</returns>
    private static string ToJson(object obj, Func<object, Type, string> converter)
    {
        if (obj == null) return "null";
        var t = obj.GetType();
        if (obj is JsonDocument jsonDoc)
        {
            return jsonDoc.RootElement.ToString();
        }

        if (obj is JsonElement jsonEle)
        {
            return jsonEle.ToString();
        }

        if (obj is System.Text.Json.Nodes.JsonNode jsonNode)
        {
            return jsonNode.ToJsonString();
        }

        if (t.FullName.StartsWith("Newtonsoft.Json.Linq.J", StringComparison.InvariantCulture))
        {
            if (t.FullName.Equals("Newtonsoft.Json.Linq.JObject", StringComparison.InvariantCulture)
                || t.FullName.Equals("Newtonsoft.Json.Linq.JArray", StringComparison.InvariantCulture))
                return obj.ToString();
        }

        if (obj is Security.TokenRequest tokenReq)
        {
            return tokenReq.ToJsonString();
        }

        if (obj is IEnumerable<KeyValuePair<string, string>> col)
        {
            var str = new StringBuilder("{");
            foreach (var kvp in col)
            {
                str.AppendFormat("\"{0}\":\"{1}\",", JsonStringNode.ToJson(kvp.Key), JsonStringNode.ToJson(kvp.Value));
            }

            str.Remove(str.Length - 1, 1);
            str.Append('}');
            return str.ToString();
        }

        if (t == typeof(string)) return JsonStringNode.ToJson(obj.ToString());
        if (obj is Net.HttpUri uri2) return JsonStringNode.ToJson(uri2.ToString());
        if (obj is Net.AppDeepLinkUri uri3) return JsonStringNode.ToJson(uri3.ToString());
        if (obj is Uri uri)
        {
            try
            {
                return JsonStringNode.ToJson(uri.OriginalString);
            }
            catch (InvalidOperationException)
            {
                return JsonStringNode.ToJson(uri.ToString());
            }
        }

        if (obj is IJsonValueNode)
        {
            return obj.ToString();
        }

        if (t.IsValueType)
        {
            if (obj is bool b)
                return b ? "true" : "false";
            if (obj is int i32)
                return i32.ToString("g", CultureInfo.InvariantCulture);
            if (obj is long i64)
                return i64.ToString("g", CultureInfo.InvariantCulture);
            if (obj is float f1)
                return f1.ToString("g", CultureInfo.InvariantCulture);
            if (obj is uint i32u)
                return i32u.ToString("g", CultureInfo.InvariantCulture);
            if (obj is ulong i64u)
                return i64u.ToString("g", CultureInfo.InvariantCulture);
            if (obj is DateTime d)
                return JsonStringNode.ToJson(d.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            if (obj is DateTimeOffset dto)
                return JsonStringNode.ToJson(dto.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
            if (obj is double f2)
                return f2.ToString("g", CultureInfo.InvariantCulture);
            if (obj is decimal f3)
                return f3.ToString("g", CultureInfo.InvariantCulture);
            if (obj is TimeSpan ts)
                return ts.TotalSeconds.ToString("g", CultureInfo.InvariantCulture);

            if (t == typeof(Guid)
                || t == typeof(Maths.Angle))
                return JsonStringNode.ToJson(obj.ToString());
            if (t == typeof(byte)
                || t == typeof(short)
                || t == typeof(ushort))
                return obj.ToString();

#if NET6_0_OR_GREATER
            if (obj is Half ha)
                return ha.ToString("g", CultureInfo.InvariantCulture);
#endif
        }

        if (t == typeof(DBNull)) return "null";
        return converter(obj, t);
    }

    internal static bool ReplaceBackSlash(StringBuilder sb, StringBuilder backSlash, char c)
    {
        if (backSlash.Length == 0)
        {
            switch (c)
            {
                case 'x':
                case 'X':
                case 'u':
                case 'U':
                    backSlash.Append(c);
                    return false;
                case 'R':
                case 'r':
                    sb.Append('\r');
                    break;
                case 'N':
                case 'n':
                    sb.Append('\n');
                    break;
                case 'A':
                case 'a':
                    sb.Append('\a');
                    break;
                case 'B':
                case 'b':
                    sb.Append('\b');
                    break;
                case 'T':
                case 't':
                    sb.Append('\t');
                    break;
                case 'F':
                case 'f':
                    sb.Append('\f');
                    break;
                case 'V':
                case 'v':
                    sb.Append('\v');
                    break;
                case '0':
                    sb.Append('\0');
                    break;
                default:
                    sb.Append(c);
                    break;
            }

            return true;
        }

        var firstBackSlash = backSlash[0];
        int len;
        if (firstBackSlash == 'x' || firstBackSlash == 'X')
        {
            len = 3;
        }
        else if (firstBackSlash == 'u' || firstBackSlash == 'U')
        {
            len = 5;
        }
        else
        {
            return true;
        }

        if (backSlash.Length < len)
        {
            backSlash.Append(c);
            return false;
        }

        try
        {
            var num = Convert.ToInt32(backSlash.ToString().Substring(1), 16);
            sb.Append(char.ConvertFromUtf32(num));
        }
        catch (FormatException)
        {
            sb.Append(backSlash.ToString());
        }
        catch (ArgumentException)
        {
            sb.Append(backSlash.ToString());
        }

        return true;
    }

    internal static string ReplaceBackSlash(IEnumerable<char> s)
    {
        var sb = new StringBuilder();
        StringBuilder backSlash = null;
        foreach (var c in s)
        {
            if (backSlash != null)
            {
                if (ReplaceBackSlash(sb, backSlash, c)) backSlash = null;
                continue;
            }

            if (c == '\\')
            {
                backSlash = new StringBuilder();
                continue;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    private static string ToUpper(string source, CultureInfo culture)
    {
        return culture == null ? source.ToUpper() : source.ToUpper(culture);
    }

    private static string ToLower(string source, CultureInfo culture)
    {
        return culture == null ? source.ToLower() : source.ToLower(culture);
    }
#pragma warning restore IDE0056, IDE0057
}
