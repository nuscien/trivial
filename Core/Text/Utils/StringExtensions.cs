using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Reflection;

namespace Trivial.Text;

/// <summary>
/// The string extension and helper.
/// </summary>
public static class StringExtensions
{
#pragma warning disable IDE0056, IDE0057, IDE0058
    private static readonly string[] NewLineSeparators = ["\r\n", "\n", "\r"];

    /// <summary>
    /// Special characters of YAML.
    /// </summary>
    internal static readonly char[] YamlSpecialChars = [':', '\r', '\n', '\\', '\'', '\"', '\t', ' ', '#', '.', '[', '{', '\\', '/', '@'];

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
    /// Returns a copy of this string converted to specific case, using the casing rules of the specified culture.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="options">The specific case.</param>
    /// <param name="culture">An object that supplies culture-specific casing rules.</param>
    /// <returns>The specific case equivalent of the current string.</returns>
    /// <exception cref="ArgumentNullException">culture is null.</exception>
    public static string ToSpecificCase(this IObjectRef<string> source, Cases options, CultureInfo culture = null)
        => ToSpecificCase(source?.Value, options, culture);

    /// <summary>
    /// Returns a copy of this string converted to specific case, using the casing rules of the invariant culture.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="options">The specific case.</param>
    /// <returns>The specific case equivalent of the current string.</returns>
    public static string ToSpecificCaseInvariant(this IObjectRef<string> source, Cases options)
        => ToSpecificCase(source?.Value, options, CultureInfo.InvariantCulture);

    /// <summary>
    /// Breaks lines.
    /// </summary>
    /// <param name="text">The original string.</param>
    /// <param name="length">The count of a line.</param>
    /// <param name="newLine">The optional newline string; or null, by default, to use the newline defined for current environment.</param>
    /// <returns>A new text with line break.</returns>
    public static string BreakLines(string text, int length, string newLine = null)
    {
        var idx = 0;
        var len = text.Length;
        var str = new StringBuilder();
        newLine ??= Environment.NewLine;
        while (idx < len)
        {
            if (idx > 0)
            {
                str.Append(newLine);
            }

            if (idx + length >= len)
            {
                AppendSubstring(str, text, idx);
            }
            else
            {
                AppendSubstring(str, text, idx, length);
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
                str.AppendSubstring(text, idx);
            }
            else
            {
                str.AppendSubstring(text, idx, length);
            }

            idx += length;
        }

        return str.ToString();
    }

    /// <summary>
    /// Splits a string into a number of substrings based on the string in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="separator">A string that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, string separator, StringSplitOptions options = StringSplitOptions.None)
    {
        if (options == StringSplitOptions.None && string.IsNullOrEmpty(source)) yield break;
        if (options == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrWhiteSpace(source)) yield break;
        if (string.IsNullOrEmpty(separator))
        {
            yield return source;
            yield break;
        }
        
        var index = 0;
        if (options == StringSplitOptions.None)
        {
            while (true)
            {
                var i = source.IndexOf(separator, index);
                if (i < 0)
                {
                    yield return source.Substring(index);
                    break;
                }

                yield return source.SubRangeString(index, i);
                index = i + separator.Length;
                if (index >= source.Length) break;
            }
        }
        else
        {
            while (true)
            {
                var i = source.IndexOf(separator, index);
                if (i < 0)
                {
                    yield return source.Substring(index);
                    break;
                }

                if (i <= index) continue;
                yield return source.SubRangeString(index, i);
                index = i + separator.Length;
                if (index >= source.Length) break;
            }
        }
    }

    /// <summary>
    /// Splits a string into a number of substrings based on the character in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="separator">A character that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, char separator, StringSplitOptions options = StringSplitOptions.None)
        => YieldSplit(source, new[] { separator }, options);

    /// <summary>
    /// Splits a string into a number of substrings based on the characters in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="separatorA">A character that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="separatorB">Another character that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, char separatorA, char separatorB, StringSplitOptions options = StringSplitOptions.None)
        => YieldSplit(source, new[] { separatorA, separatorB }, options);

    /// <summary>
    /// Splits a string into a number of substrings based on the characters in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="separatorA">The character A that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="separatorB">The character B that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="separatorC">The character C that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, char separatorA, char separatorB, char separatorC, StringSplitOptions options = StringSplitOptions.None)
        => YieldSplit(source, new[] { separatorA, separatorB, separatorC }, options);

    /// <summary>
    /// Splits a string into a number of substrings based on the characters in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="separatorA">The character A that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="separatorB">The character B that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="separatorC">The character C that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="separatorD">The character D that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, char separatorA, char separatorB, char separatorC, char separatorD, StringSplitOptions options = StringSplitOptions.None)
        => YieldSplit(source, new[] { separatorA, separatorB, separatorC, separatorD }, options);

    /// <summary>
    /// Splits a string into a number of substrings based on the characters in an enumerable strings.
    /// </summary>
    /// <param name="source">The source string to split.</param>
    /// <param name="separator">A character array that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, char[] separator, StringSplitOptions options = StringSplitOptions.None)
    {
        if (options == StringSplitOptions.None && string.IsNullOrEmpty(source)) yield break;
        if (options == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrWhiteSpace(source)) yield break;
        if (separator == null || separator.Length == 0)
        {
            yield return source;
            yield break;
        }

        var index = 0;
        if (options == StringSplitOptions.None)
        {
            while (true)
            {
                var i = source.IndexOfAny(separator, index);
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
                var i = source.IndexOfAny(separator, index);
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
    /// <param name="separator">A string array that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
    /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
    /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
    public static IEnumerable<string> YieldSplit(this string source, IEnumerable<string> separator, StringSplitOptions options = StringSplitOptions.None)
    {
        if (options == StringSplitOptions.None && string.IsNullOrEmpty(source)) yield break;
        if (options == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrWhiteSpace(source)) yield break;
        if (separator == null)
        {
            yield return source;
            yield break;
        }

        var index = 0;
        if (options == StringSplitOptions.None)
        {
            while (true)
            {
                var indexes = separator.Select(ele => (Index: source.IndexOf(ele, index), ele.Length)).Where(ele => ele.Index >= 0).ToList();
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
                var indexes = separator.Select(ele => (Index: source.IndexOf(ele, index), ele.Length)).Where(ele => ele.Index >= 0).ToList();
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
        => YieldSplit(source, NewLineSeparators, removeEmptyLine ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

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
    /// Removes all leading and trailing white-space characters from each string item of the collection.
    /// </summary>
    /// <param name="source">The source string collection.</param>
    /// <param name="removeEmpty">rue if remove empty or null string; otherwise, false.</param>
    /// <returns>A new string collection that each item has been trimmed.</returns>
    public static IEnumerable<string> TrimForEach(this IEnumerable<string> source, bool removeEmpty = false)
        => source == null ? null : (removeEmpty ? source.Select(Trim).Where(IsNotEmpty) : source.Select(Trim));

    /// <summary>
    /// Removes all leading and trailing white-space characters from each string item of the collection.
    /// </summary>
    /// <param name="source">The source string collection.</param>
    /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
    /// <returns>A new string collection that each item has been trimmed.</returns>
    public static IEnumerable<string> TrimForEach(this IEnumerable<string> source, char[] trimChars)
        => source?.Select(s => s.Trim(trimChars));

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified read-only memory of char.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="value">A string to seek.</param>
    /// <param name="startOffset">The search starting position.</param>
    /// <returns>The zero-based index position of value if that string is found, or -1 if it is not.</returns>
    public static int IndexOf(ReadOnlySpan<char> source, string value, int startOffset = 0)
    {
        if (string.IsNullOrEmpty(value)) return -1;
        var test = 0;
        for (var i = startOffset; i < source.Length; i++)
        {
            if (source[i] != value[test])
            {
                test = 0;
                continue;
            }

            test++;
            if (test >= value.Length) return i;
        }

        return -1;
    }

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified read-only memory of char.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="value">A string to seek.</param>
    /// <param name="startOffset">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>The zero-based index position of value if that string is found, or -1 if it is not.</returns>
    public static int IndexOf(ReadOnlySpan<char> source, string value, int startOffset, int count)
    {
        if (string.IsNullOrEmpty(value)) return -1;
        var test = 0;
        var len = Math.Min(startOffset + count, source.Length);
        for (var i = startOffset; i < len; i++)
        {
            if (source[i] != value[test])
            {
                test = 0;
                continue;
            }

            test++;
            if (test >= value.Length) return i;
        }

        return -1;
    }

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified read-only memory of char.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="value">A char to seek.</param>
    /// <param name="startOffset">The search starting position.</param>
    /// <returns>The zero-based index position of value if that string is found, or -1 if it is not.</returns>
    public static int IndexOf(ReadOnlySpan<char> source, char value, int startOffset = 0)
    {
        for (var i = startOffset; i < source.Length; i++)
        {
            if (source[i] != value) continue;
            return i;
        }

        return -1;
    }

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified read-only memory of char.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="value">A char to seek.</param>
    /// <param name="startOffset">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>The zero-based index position of value if that string is found, or -1 if it is not.</returns>
    public static int IndexOf(ReadOnlySpan<char> source, char value, int startOffset, int count)
    {
        var len = Math.Min(startOffset + count, source.Length);
        for (var i = startOffset; i < len; i++)
        {
            if (source[i] != value) continue;
            return i;
        }

        return -1;
    }

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
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
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
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
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
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="withQ">true if the result includes the query; otherwise, false.</param>
    /// <param name="startIndex">The index at which to begin the searching.</param>
    /// <param name="offset">The offset of the query string.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, string q, bool withQ, int startIndex, out int offset)
        => After(source, string.IsNullOrEmpty(q) ? ReadOnlySpan<char>.Empty : q.AsSpan(), withQ, startIndex, out offset);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="withQ">true if the result includes the query; otherwise, false.</param>
    /// <param name="offset">The offset of the query string.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, string q, bool withQ, out int offset)
        => After(source, q, withQ, 0, out offset);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="startIndex">The index at which to begin the searching.</param>
    /// <param name="offset">The offset of the query string.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, string q, int startIndex, out int offset)
        => After(source, q, false, startIndex, out offset);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="offset">The offset of the query string.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, string q, out int offset)
        => After(source, q, false, 0, out offset);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="withQ">true if the result includes the query; otherwise, false.</param>
    /// <param name="startIndex">The index at which to begin the searching.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, string q, bool withQ = false, int startIndex = 0)
        => After(source, q, withQ, startIndex, out _);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="startIndex">The index at which to begin the searching.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, string q, int startIndex)
        => After(source, q, false, startIndex, out _);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="withQ">true if the result includes the query; otherwise, false.</param>
    /// <param name="startIndex">The index at which to begin the searching.</param>
    /// <param name="offset">The offset of the query string.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, ReadOnlySpan<char> q, bool withQ, int startIndex, out int offset)
    {
        if (startIndex > 0) source = source.Slice(startIndex);
        else if (startIndex < 0) startIndex = 0;
        var i = source.IndexOf(q);
        if (i < 0)
        {
            offset = -1;
            return ReadOnlySpan<char>.Empty;
        }

        if (!withQ)
        {
            i += q.Length;
            if (i >= source.Length)
            {
                offset = -1;
                return ReadOnlySpan<char>.Empty;
            }
        }

        offset = i;
        return source.Slice(i);
    }

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="withQ">true if the result includes the query; otherwise, false.</param>
    /// <param name="offset">The offset of the query string.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, ReadOnlySpan<char> q, bool withQ, out int offset)
        => After(source, q, withQ, 0, out offset);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="startIndex">The index at which to begin the searching.</param>
    /// <param name="offset">The offset of the query string.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, ReadOnlySpan<char> q, int startIndex, out int offset)
        => After(source, q, false, startIndex, out offset);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="offset">The offset of the query string.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, ReadOnlySpan<char> q, out int offset)
        => After(source, q, false, 0, out offset);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="withQ">true if the result includes the query; otherwise, false.</param>
    /// <param name="startIndex">The index at which to begin the searching.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, ReadOnlySpan<char> q, bool withQ = false, int startIndex = 0)
        => After(source, q, withQ, startIndex, out _);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="startIndex">The index at which to begin the searching.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> After(ReadOnlySpan<char> source, ReadOnlySpan<char> q, int startIndex)
        => After(source, q, false, startIndex, out _);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="withQ">true if the result includes the query; otherwise, false.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> Before(ReadOnlySpan<char> source, ReadOnlySpan<char> q, bool withQ)
    {
        var i = source.IndexOf(q);
        if (i < 0) return ReadOnlySpan<char>.Empty;
        if (withQ)
        {
            i += q.Length;
            if (i >= source.Length) return source;
        }

        return source.Slice(0, i);
    }

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="q">The query string.</param>
    /// <param name="withQ">true if the result includes the query; otherwise, false.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> Before(ReadOnlySpan<char> source, string q, bool withQ)
        => Before(source, string.IsNullOrEmpty(q) ? ReadOnlySpan<char>.Empty : q.AsSpan(), withQ);

    /// <summary>
    /// Gets the sub-string after the first occurrence of a specific string.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="length">The desired length for the slice.</param>
    /// <returns>The result.</returns>
    public static ReadOnlySpan<char> Before(ReadOnlySpan<char> source, int length)
    {
        if (length < 0) return ReadOnlySpan<char>.Empty;
        if (length >= source.Length) return source;
        return source.Slice(0, length);
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
    public static string GetIfNotEmpty(params ReadOnlySpan<string> values)
    {
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
    public static string GetIfNotEmptyTrimmed(params ReadOnlySpan<string> values)
    {
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
    /// Indicates whether the specified string is null or an empty string ("").
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <returns>true if the source value is null or an empty string (""); otherwise, false.</returns>
    public static bool IsNullOrEmpty(this IObjectRef<string> obj)
        => string.IsNullOrEmpty(obj?.Value);

    /// <summary>
    /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <returns>true if the source value is null or System.String.Empty, or if value consists exclusively of white-space characters; otherwise, false.</returns>
    public static bool IsNullOrWhiteSpace(this IObjectRef<string> obj)
        => string.IsNullOrWhiteSpace(obj?.Value);

    /// <summary>
    /// Tests if the number value is macthed by the specific condition.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="condition">The condition to test the number value.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public static bool IsMatched(this IObjectRef<string> obj, Data.StringCondition condition)
        => condition == null || condition.IsMatched(obj?.Value);

    /// <summary>
    /// Creates a new read-only span over the string value.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <returns>The read-only span representation of the string.</returns>
    public static ReadOnlySpan<char> AsSpan(this IObjectRef<string> obj)
        => obj?.Value == null ? ReadOnlySpan<char>.Empty : obj.Value.AsSpan();

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified string in this instance. The search starts at a specified character position.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <returns>The zero-based index position of value from the start of the current instance if that string is found, or -1 if it is not.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public static int IndexOf(this IObjectRef<string> obj, string value, int start = 0)
        => obj?.Value?.IndexOf(value, start) ?? (value == null ? string.Empty.IndexOf(value) : -1);

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified string in this instance. The search starts at a specified character position.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>count or startIndex is negative. -or- startIndex is greater than the length of this string. -or- count is greater than the length of this string minus start index.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public static int IndexOf(this IObjectRef<string> obj, string value, int start, int count)
        => obj?.Value?.IndexOf(value, start, count) ?? (value == null ? string.Empty.IndexOf(value) : -1);

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified string in this instance. The search starts at a specified character position and examines a specified number of character positions.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentException">count or startIndex is negative. -or- startIndex is greater than the length of this string. -or- count is greater than the length of this string minus startIndex.</exception>
    public static int IndexOf(this IObjectRef<string> obj, string value, StringComparison comparisonType, int startIndex = 0, int? count = null)
        => obj?.Value == null ? -1 : (count.HasValue ? obj.Value.IndexOf(value, startIndex, count.Value, comparisonType) : obj.Value.IndexOf(value, startIndex, comparisonType));

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified character in this instance. The search starts at a specified character position.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <returns>The zero-based index position of value from the start of the current instance if that string is found, or -1 if it is not.</returns>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public static int IndexOf(this IObjectRef<string> obj, char value, int start = 0)
        => obj?.Value?.IndexOf(value, start) ?? -1;

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified character in this instance. The search starts at a specified character position.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>count or startIndex is negative. -or- startIndex is greater than the length of this string. -or- count is greater than the length of this string minus start index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public static int IndexOf(this IObjectRef<string> obj, char value, int start, int count)
        => obj?.Value?.IndexOf(value, start, count) ?? -1;

    /// <summary>
    /// Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <returns>The zero-based index position of value from the start of the current instance if that string is found, or -1 if it is not.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public static int LastIndexOf(this IObjectRef<string> obj, string value, int start = 0)
        => obj?.Value?.LastIndexOf(value, start) ?? (value == null ? string.Empty.LastIndexOf(value) : -1);

    /// <summary>
    /// Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>count or startIndex is negative. -or- startIndex is greater than the length of this string. -or- count is greater than the length of this string minus start index.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public static int LastIndexOf(this IObjectRef<string> obj, string value, int start, int count)
        => obj?.Value?.LastIndexOf(value, start, count) ?? (value == null ? string.Empty.LastIndexOf(value) : -1);

    /// <summary>
    /// Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string for the specified number of character positions. A parameter specifies the type of comparison to performwhen searching for the specified string.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentException">count or startIndex is negative. -or- startIndex is greater than the length of this string. -or- count is greater than the length of this string minus startIndex.</exception>
    public static int LastIndexOf(this IObjectRef<string> obj, string value, StringComparison comparisonType, int startIndex = 0, int? count = null)
        => obj?.Value == null ? -1 : (count.HasValue ? obj.Value.LastIndexOf(value, startIndex, count.Value, comparisonType) : obj.Value.LastIndexOf(value, startIndex, comparisonType));

    /// <summary>
    /// Reports the zero-based index position of the last occurrence of a specified character within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <returns>The zero-based index position of value from the start of the current instance if that string is found, or -1 if it is not.</returns>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public static int LastIndexOf(this IObjectRef<string> obj, char value, int start = 0)
        => obj?.Value?.LastIndexOf(value, start) ?? -1;

    /// <summary>
    /// Reports the zero-based index position of the last occurrence of a specified character within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="start">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>count or startIndex is negative. -or- startIndex is greater than the length of this string. -or- count is greater than the length of this string minus start index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 (zero) or greater than the length of this string.</exception>
    public static int LastIndexOf(this IObjectRef<string> obj, char value, int start, int count)
        => obj?.Value?.LastIndexOf(value, start, count) ?? -1;

    /// <summary>
    /// Returns a value indicating whether a specified substring occurs within this string value.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to seek.</param>
    /// <returns>true if the value parameter occurs within this string, or if value is the empty string (""); otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool Contains(this IObjectRef<string> obj, string value)
        => obj?.Value?.Contains(value) ?? false;

    /// <summary>
    /// Returns a value indicating whether a specified character occurs within this string value.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The character to seek.</param>
    /// <returns>true if the value parameter occurs within this string, or if value is the empty string (""); otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool Contains(this IObjectRef<string> obj, char value)
        => obj?.Value?.Contains(value) ?? false;

    /// <summary>
    /// Determines whether a specified string is a prefix of the current instance.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to compare.</param>
    /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool StartsWith(this IObjectRef<string> obj, string value)
        => obj?.Value?.StartsWith(value) ?? false;

    /// <summary>
    /// Determines whether a specified string is a prefix of the current instance.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to compare.</param>
    /// <param name="comparisonType">One of the enumeration values that determines how this string and value are compared.</param>
    /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool StartsWith(this IObjectRef<string> obj, string value, StringComparison comparisonType)
        => obj?.Value?.StartsWith(value, comparisonType) ?? false;

    /// <summary>
    /// Determines whether a specified string is a prefix of the current instance.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The character to compare.</param>
    /// <returns>true if value matches the beginning of this string; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool StartsWith(this IObjectRef<string> obj, char value)
        => obj?.Value?.StartsWith(value) ?? false;

    /// <summary>
    /// Determines whether the end of this string instance matches the specified string.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to compare to the substring at the end of this instance.</param>
    /// <returns>true if value matches the end of this instance; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool EndsWith(this IObjectRef<string> obj, string value)
        => obj?.Value?.EndsWith(value) ?? false;

    /// <summary>
    /// Determines whether the end of this string instance matches the specified string.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The string to compare to the substring at the end of this instance.</param>
    /// <param name="comparisonType">One of the enumeration values that determines how this string and value are compared.</param>
    /// <returns>true if value matches the end of this instance; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool EndsWith(this IObjectRef<string> obj, string value, StringComparison comparisonType)
        => obj?.Value?.EndsWith(value, comparisonType) ?? false;

    /// <summary>
    /// Determines whether the end of this string instance matches the specified string.
    /// </summary>
    /// <param name="obj">The source object.</param>
    /// <param name="value">The character to compare.</param>
    /// <returns>true if value matches the end of this instance; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    public static bool EndsWith(this IObjectRef<string> obj, char value)
        => obj?.Value?.EndsWith(value) ?? false;

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>The description.</returns>
    public static string GetDescription(Enum value)
    {
        if (value == null) return null;
        try
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (string.IsNullOrWhiteSpace(name)) return value.ToString();
            var field = type.GetField(name);
            var attr = field.GetCustomAttributes<DescriptionAttribute>()?.FirstOrDefault();
            return string.IsNullOrWhiteSpace(attr?.Description) ? name : attr.Description;
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
    /// Gets the description mapping of each enum.
    /// </summary>
    /// <param name="type">The enum type.</param>
    /// <returns>A mapping of enum and description.</returns>
    public static Dictionary<string, string> GetEnumDescriptionMapping(Type type)
    {
        if (type == null || !type.IsEnum) return null;
        var values = Enum.GetValues(type);
        var dict = new Dictionary<string, string>();
        foreach (Enum value in values)
        {
            dict[value.ToString()] = GetDescription(value);
        }

        return dict;
    }

    /// <summary>
    /// Gets the description mapping of each enum.
    /// </summary>
    /// <typeparam name="T">THe type of enum.</typeparam>
    /// <returns>A mapping of enum and description.</returns>
    public static Dictionary<T, string> GetEnumDescriptionMapping<T>() where T : struct, Enum
    {
#if NETFRAMEWORK
        var values = Enum.GetValues(typeof(T));
#else
        var values = Enum.GetValues<T>();
#endif
        var dict = new Dictionary<T, string>();
        foreach (T value in values)
        {
            dict[value] = GetDescription(value);
        }

        return dict;
    }

    internal static Dictionary<string, string> TryParseJsonStrings(string s)
    {
        var col = s.Split(',');
        var r = new Dictionary<string, string>();
        string key = null;
        foreach (var item in col)
        {
            if (key != null)
            {
                var rest = IsEndWithQuote(item);
                if (rest < 0)
                {
                    r[key] += string.Concat(',', item);
                }
                else
                {
                    r[key] = ExtractFromJsonString(string.Concat(r[key], ',', item.Substring(0, rest)));
                    key = null;
                }

                continue;
            }

            var i = item.IndexOf('=');
            if (i < 0)
            {
                r[item.Trim()] = null;
                continue;
            }

            var k = item.Substring(0, i).Trim();
            var v = item.Substring(i + 1).TrimStart();
            if (v.StartsWith('"'))
            {
                v = v.Substring(1);
                var rest = IsEndWithQuote(v);
                if (rest < 0) key = k;
                else v = ExtractFromJsonString(v.Substring(0, rest));
            }

            r[k] = v;
        }

        if (key != null) r[key] = ExtractFromJsonString(r[key]);
        return r;
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>A string converted from the stream.</returns>
    public async static Task<string> ToStringAsync(Stream stream, Encoding encoding = null)
    {
        using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A string converted from the stream.</returns>
    public async static Task<string> ToStringAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A string converted from the stream.</returns>
    public async static Task<string> ToStringAsync(Stream stream, Encoding encoding, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        return await reader.ReadToEndAsync(cancellationToken);
    }
#endif

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns>A string converted from the stream.</returns>
    public static string ToString(Stream stream, Encoding encoding = null)
    {
        using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        return reader.ReadToEnd();
    }

    internal static string ToString(char[] value, int start = 0, int? count = null)
    {
        if (start == 0 && count == null)
            return new string(value);
        var list = value.Skip(start);
        if (count.HasValue) list = list.Take(count.Value);
        return new string(list.ToArray());
    }

    internal static string ToString(ReadOnlySpan<char> value, int start = 0, int? count = null)
    {
        if (start == 0 && count == null)
            return value.ToString();
        var list = count.HasValue ? value.Slice(start, count.Value) : value.Slice(start);
        return list.ToString();
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
    /// Asserts the value is not null, empty or consists only of white-space characters.
    /// </summary>
    /// <param name="paramName">The parameter name.</param>
    /// <param name="value">The value to test.</param>
    /// <exception cref="ArgumentException">The value was empty or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentNullException">The value was null.</exception>
    internal static void AssertNotWhiteSpace(string paramName, string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw value is null ? ObjectConvert.ArgumentNull(paramName) : new ArgumentException(string.Concat(paramName, " should not be null, empty or consists only of white-space characters."), nameof(value));
    }

#if NETFRAMEWORK
    internal static StringBuilder Append(this StringBuilder sb, char value)
        => sb.Append(value, 1);
#endif

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

    internal static List<string> GetCultureCodes(CultureInfo culture)
    {
        try
        {
            culture ??= CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture;
        }
        catch (ArgumentException)
        {
        }
        catch (NotImplementedException)
        {
        }

        var name = culture?.Name;
        if (string.IsNullOrWhiteSpace(name)) return null;
        var names = new List<string>();
        var col = name.Split('-');
        var last = string.Empty;
        foreach (var item in col)
        {
            if (string.IsNullOrEmpty(item)) continue;
            if (last.Length == 0)
            {
                last = item;
                names.Add(item);
            }
            else
            {
                last = string.Concat(last, "-", item);
                names.Insert(0, last);
            }
        }

        if (name.StartsWith("zh-"))
        {
            switch (name)
            {
                case "zh-CN":
                    names.Insert(1, "zh-Hans");
                    names.Insert(0, "zh-Hans-CN");
                    break;
                case "zh-SG":
                    names.Insert(1, "zh-Hans");
                    names.Insert(0, "zh-Hans-SG");
                    break;
                case "zh-TW":
                    names.Insert(1, "zh-Hant");
                    names.Insert(0, "zh-Hant-TW");
                    names.Add("zh-Hans");
                    break;
                case "zh-HK":
                    names.Insert(1, "zh-Hant");
                    names.Insert(0, "zh-Hant-HK");
                    names.Add("zh-Hans");
                    break;
                case "zh-MO":
                    names.Insert(1, "zh-Hant");
                    names.Insert(0, "zh-Hant-SG");
                    names.Add("zh-Hans");
                    break;
            }
        }

        return names;
    }

    internal static void AppendSubstring(this StringBuilder sb, string input, int startIndex)
#if NET6_0_OR_GREATER
        => sb.Append(input.AsSpan(startIndex));
#else
        => sb.Append(input.Substring(startIndex));
#endif

    internal static void AppendSubstring(this StringBuilder sb, string input, int startIndex, int length)
#if NET6_0_OR_GREATER
        => sb.Append(input.AsSpan(startIndex, length));
#else
        => sb.Append(input.Substring(startIndex, length));
#endif

    private static string ToUpper(string source, CultureInfo culture)
        => culture == null ? source.ToUpper() : source.ToUpper(culture);

    private static string ToLower(string source, CultureInfo culture)
        => culture == null ? source.ToLower() : source.ToLower(culture);

    private static string Trim(string s)
        => s?.Trim();

    private static bool IsNotEmpty(string s)
        => !string.IsNullOrEmpty(s);

    private static int IsEndWithQuote(string s)
    {
        s = s.TrimEnd();
        if (!s.EndsWith('"')) return -1;
        if (s.Length == 1) return 0;
        var i = s.Length - 1;
        if (s[s.Length - 2] != '\\') return i;
        if (s.Length == 2 || s[s.Length - 3] != '\\' || s.Length == 3) return -1;
        s = s.Substring(0, s.Length - 3).TrimEnd('\\');
        return (s.Length - i) % 2 == 0 ? i : -1;
    }

    private static string ExtractFromJsonString(string s)
        => JsonObjectNode.TryParse(string.Concat("{ \"v\": \"", s, "\" }"))?.TryGetStringValue("v");
#pragma warning restore IDE0056, IDE0057, IDE0058
}
