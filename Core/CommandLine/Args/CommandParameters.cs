using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;

using Trivial.Net;
using Trivial.Text;

namespace Trivial.CommandLine;

/// <summary>
/// The arguments parameter.
/// </summary>
public class CommandParameter: IEquatable<CommandParameter>, IEquatable<CommandParameters>, IEquatable<string>, IReadOnlyList<string>
{
    /// <summary>
    /// A code for format.
    /// 0 for standard; 1 for query; 2 for path.
    /// </summary>
    private readonly int formatCode;

    /// <summary>
    /// Initializes a new instance of the Parameter class.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="values">The words of value.</param>
    public CommandParameter(string key, IEnumerable<string> values) : this(key, values, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Parameter class.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="values">The words of value.</param>
    /// <param name="urlLike">true if it is URL-like format; otherwise, false.</param>
    internal CommandParameter(string key, IEnumerable<string> values, int urlLike)
    {
        formatCode = urlLike;
        OriginalKey = key;
        Key = FormatKey(key);
        Values = (values != null ? values.Where(item =>
        {
            return item != null;
        }).ToList() : new List<string>()).AsReadOnly();
        Count = Values.Count;
        IsEmpty = Count == 0;
        if (IsEmpty)
        {
            Value = string.Empty;
            return;
        }

        var str = new StringBuilder(Values[0]);
        var sep = urlLike switch
        {
            1 => ',',
            2 => '/',
            _ => ' '
        };
        for (var i = 1; i < Values.Count; i++)
        {
            str.Append(sep);
            str.Append(Values[i]);
        }

        Value = str.ToString();
    }

    /// <summary>
    /// Gets the parameter key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the original parameter key.
    /// </summary>
    public string OriginalKey { get; }

    /// <summary>
    /// Gets the value string.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Get a value indicating whether the value is empty.
    /// </summary>
    public bool IsEmpty { get; }

    /// <summary>
    /// Gets a value indicating whether the value of the arguments is URL-Query-like format.
    /// </summary>
    public bool IsUrlQueryLike => formatCode == 1;

    /// <summary>
    /// Gets a value indicating whether the value of the arguments is URL-Path-like format.
    /// </summary>
    public bool IsUrlPathLike => formatCode == 2;

    /// <summary>
    /// Gets the words of the value.
    /// </summary>
    public IReadOnlyList<string> Values { get; }

    /// <summary>
    /// Gets the count of words of the value.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the specific word of the value.
    /// </summary>
    /// <param name="index">The index of the word.</param>
    /// <returns>A word in the value.</returns>
    public string this[int index] => Values[index];

    /// <summary>
    /// Tries to get a value.
    /// </summary>
    /// <param name="index">The index of the word.</param>
    /// <param name="result">A word in the value output.</param>
    /// <returns>true if exists; otherwise, false.</returns>
    public bool TryGet(int index, out string result)
    {
        if (index < 0 || index >= Values.Count)
        {
            result = null;
            return false;
        }

        try
        {
            result = Values[index];
            return true;
        }
        catch (ArgumentException)
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Tries to get a value.
    /// </summary>
    /// <param name="index">The index of the word.</param>
    /// <returns>A word in the value; or null; if not found.</returns>
    public string TryGet(int index)
        => TryGet(index, out var r) ? r : null;

    /// <summary>
    /// Converts the value to its boolean equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryParse(out bool result)
    {
        if (!string.IsNullOrWhiteSpace(Value)) return bool.TryParse(Value, out result);
        result = true;
        return true;
    }

    /// <summary>
    /// Converts the value to its 32-bit signed integer equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryParse(out int result)
        => Maths.Numbers.TryParseToInt32(Value, 10, out result);

    /// <summary>
    /// Converts the value to its 64-bit signed integer equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryParse(out long result)
        => Maths.Numbers.TryParseToInt64(Value, 10, out result);

    /// <summary>
    /// Converts the value to its single-precision floating-point number equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryParse(out float result)
        => float.TryParse(Value, out result);

    /// <summary>
    /// Converts the value to its double-precision floating-point number equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryParse(out double result)
        => double.TryParse(Value, out result);

    /// <summary>
    /// Converts the value to its GUID equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryParse(out Guid result)
        => Guid.TryParse(Value, out result);

    /// <summary>
    /// Converts the value to its date and time equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryParse(out DateTime result)
        => DateTime.TryParse(Value, out result);

    /// <summary>
    /// Converts the value to its date and time with offset relative to UTC equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryParse(out DateTimeOffset result)
        => DateTimeOffset.TryParse(Value, out result);

    /// <summary>
    /// Converts the value to its time span equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryParse(out TimeSpan result)
        => TimeSpan.TryParse(Value, out result);

    /// <summary>
    /// Converts the value to its URI equivalent.
    /// </summary>
    /// <returns>The result value converted.</returns>
    public Uri ParseToUri()
        => !string.IsNullOrWhiteSpace(Value) ? new Uri(Value) : null;

    /// <summary>
    /// Converts the value to its file information object equivalent.
    /// </summary>
    /// <returns>The result value converted.</returns>
    public FileInfo ParseToFileInfo()
    {
        if (string.IsNullOrWhiteSpace(Value)) return null;
        try
        {
            return new FileInfo(Value);
        }
        catch (ArgumentException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (PathTooLongException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (FileNotFoundException)
        {
        }

        return null;
    }

    /// <summary>
    /// Converts the value to its directory information object equivalent.
    /// </summary>
    /// <returns>The result value converted.</returns>
    public DirectoryInfo ParseToDirectoryInfo()
    {
        if (string.IsNullOrWhiteSpace(Value)) return null;
        try
        {
            return new DirectoryInfo(Value);
        }
        catch (ArgumentException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (PathTooLongException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (DirectoryNotFoundException)
        {
        }

        return null;
    }

    /// <summary>
    /// Converts the value of this instance to the parameter string.
    /// </summary>
    /// <returns>A string whose value is the same as this instance.</returns>
    public override string ToString()
    {
        var str = new StringBuilder();
        switch (formatCode)
        {
            case 1:
                if (!string.IsNullOrEmpty(OriginalKey)) str.Append(Web.WebFormat.UrlEncode(OriginalKey));
                str.Append('=');
                if (Values.Count > 0)
                {
#if NETFRAMEWORK
                    str.Append(string.Join(",", Values.Select(ele => Web.WebFormat.UrlEncode(ele)).ToArray()));
#else
                    str.Append(string.Join(",", Values.Select(ele => Web.WebFormat.UrlEncode(ele))));
#endif
                }

                return str.ToString();
            case 2:
                str.Append(OriginalKey);
                if (Values.Count > 0)
                {
                    str.Append('/');
#if NETFRAMEWORK
                    str.Append(string.Join("/", Values.Where(ele => !string.IsNullOrEmpty(ele)).ToArray()));
#else
                    str.Append(string.Join("/", Values.Where(ele => !string.IsNullOrEmpty(ele))));
#endif
                }

                return str.ToString();
            default:
                str.Append(OriginalKey);
                if (string.IsNullOrEmpty(Value)) return str.ToString();
                if (str.Length > 0) str.Append(' ');
                str.Append(Value);
                return str.ToString();
        }
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    /// Determines whether the value of this instance and the specified one have the same value.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
    public bool Equals(CommandParameter other)
    {
        if (other == null) return false;
        return ToString() == other.ToString();
    }

    /// <summary>
    /// Determines whether the value of this instance and the specified one have the same value.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
    public bool Equals(CommandParameters other)
    {
        if (other == null) return false;
        return ToString() == other.ToString();
    }

    /// <summary>
    /// Determines whether the value of this instance and the specified one have the same value.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
    public bool Equals(string other)
    {
        return ToString() == other;
    }

    /// <summary>
    /// Determines whether the value of this instance and the specified one have the same value.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (other is CommandParameters parameters) return Equals(parameters);
        if (other is CommandParameter parameter) return Equals(parameter);
        if (other is string str) return Equals(str);
        return false;
    }

    /// <summary>
    /// Returns an enumerator that iterates through this instance.
    /// </summary>
    /// <returns>A enumerator.</returns>
    public IEnumerator<string> GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through this instance.
    /// </summary>
    /// <returns>A enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    /// <summary>
    /// Formats the parameter key.
    /// </summary>
    /// <param name="key">The parameter key to format.</param>
    /// <param name="removePrefix">true if remove the prefix; otherwise, false.</param>
    /// <returns>The parameter key formatted.</returns>
    internal static string FormatKey(string key, bool removePrefix = true)
    {
        if (key == null) return null;
        key = key.Trim();
        if (removePrefix)
        {
#pragma warning disable IDE0057
            if (key.IndexOf("--") == 0) return key.Substring(2).ToLower();
            if (key.IndexOf('-') == 0) return key.Substring(1).ToLower();
            if (key.IndexOf('/') == 0) return key.Substring(1).ToLower();
#pragma warning restore IDE0057
        }

        return key.ToLower();
    }
}

/// <summary>
/// A set of parameters with the same key or the related keys.
/// </summary>
public class CommandParameters
    : IEquatable<CommandParameters>, IEquatable<CommandParameter>, IEquatable<string>, IEnumerable<CommandParameter>
{
    /// <summary>
    /// Initializes a new instance of the Parameters instance.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="value">The collection of parameter.</param>
    /// <param name="additionalKeys">The additional keys.</param>
    public CommandParameters(string key, IEnumerable<CommandParameter> value, IEnumerable<string> additionalKeys = null)
        : this(key, value, additionalKeys, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Parameters instance.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="value">The collection of parameter.</param>
    /// <param name="additionalKeys">The additional keys.</param>
    /// <param name="urlLike">true if it is URL-like format; otherwise, false.</param>
    public CommandParameters(string key, IEnumerable<CommandParameter> value, IEnumerable<string> additionalKeys, bool urlLike)
    {
        IsUrlQueryLike = urlLike;
        Key = key;
        Items = (value != null ? value.Where(item =>
        {
            return item != null;
        }).ToList() : new List<CommandParameter>()).AsReadOnly();
        ItemCount = Items.Count;
        AdditionalKeys = (additionalKeys != null ? additionalKeys.ToList() : new List<string>()).AsReadOnly();
        var keys = new List<string>
        {
            Key
        };
        keys.AddRange(AdditionalKeys);
        AllKeys = keys.AsReadOnly();
        IsEmpty = ItemCount == 0;
        if (IsEmpty)
        {
            FirstValues = new List<string>().AsReadOnly();
            FirstValue = string.Empty;
            LastValues = new List<string>().AsReadOnly();
            LastValue = string.Empty;
            MergedValues = new List<string>().AsReadOnly();
            MergedValue = string.Empty;
            return;
        }

        FirstItem = Items[0];
        FirstValues = FirstItem.Values;
        FirstValue = FirstItem.Value;
        LastItem = Items[ItemCount - 1];
        LastValues = LastItem.Values;
        LastValue = LastItem.Value;
        var mergedList = new List<string>();
        var mergedStr = new StringBuilder();
        var sep = urlLike ? ',' : ' ';
        foreach (var item in Items)
        {
            mergedList.AddRange(item.Values);
            if (mergedStr.Length > 0) mergedStr.Append(sep);
            mergedStr.Append(item.Value);
        }

        MergedValues = mergedList.AsReadOnly();
        MergedValue = mergedStr.ToString();
    }

    /// <summary>
    /// Gets the primary parameter key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets all of the keys matched.
    /// </summary>
    public IReadOnlyList<string> AllKeys { get; }

    /// <summary>
    /// Gets the addtional keys.
    /// </summary>
    public IReadOnlyList<string> AdditionalKeys { get; }

    /// <summary>
    /// Gets a value indicating whether there is any parameter matched.
    /// </summary>
    public bool IsEmpty { get; }

    /// <summary>
    /// Gets a readonly list of the parameter matched the key.
    /// </summary>
    public IReadOnlyList<CommandParameter> Items { get; }

    /// <summary>
    /// Gets the first parameter matched the key.
    /// </summary>
    public CommandParameter FirstItem { get; }

    /// <summary>
    /// Gets the first parameter matched the key.
    /// </summary>
    public CommandParameter LastItem { get; }

    /// <summary>
    /// Gets the count of the parameter matched the key.
    /// </summary>
    public int ItemCount { get; }

    /// <summary>
    /// Gets the string value of the first parameter matched the key.
    /// </summary>
    public string FirstValue { get; }

    /// <summary>
    /// Gets the words of the value of the first parameter matched the key.
    /// </summary>
    public IReadOnlyList<string> FirstValues { get; }

    /// <summary>
    /// Gets the string value of the last parameter matched the key.
    /// </summary>
    public string LastValue { get; }

    /// <summary>
    /// Gets the words of the value of the last parameter matched the key.
    /// </summary>
    public IReadOnlyList<string> LastValues { get; }

    /// <summary>
    /// Gets the string value merged by all the parameter matched the key.
    /// </summary>
    public string MergedValue { get; }

    /// <summary>
    /// Gets the words of the value merged by all the parameter matched the key.
    /// </summary>
    public IReadOnlyList<string> MergedValues { get; }

    /// <summary>
    /// Gets a value indicating whether the value of the arguments is URL-Query-like format.
    /// </summary>
    public bool IsUrlQueryLike { get; private set; }

    /// <summary>
    /// Gets the string value of a specific parameter matched the key.
    /// </summary>
    /// <param name="mode">The parameter getting mode.</param>
    /// <returns>A string value of a specific parameter.</returns>
    public string Value(CommandParameterModes mode = CommandParameterModes.First)
    {
        return mode switch
        {
            CommandParameterModes.All => MergedValue,
            CommandParameterModes.Last => LastValue,
            _ => FirstValue,
        };
    }

    /// <summary>
    /// Gets the string value of a specific parameter matched the key.
    /// </summary>
    /// <param name="index">The parameter index.</param>
    /// <returns>A string value of a specific parameter.</returns>
    public string Value(int index)
    {
        return Items[index].Value;
    }

    /// <summary>
    /// Gets the words of the value of a specific parameter matched the key.
    /// </summary>
    /// <param name="mode">The parameter getting mode.</param>
    /// <returns>The words of the value of a specific parameter.</returns>
    public IReadOnlyList<string> Values(CommandParameterModes mode = CommandParameterModes.First)
    {
        return mode switch
        {
            CommandParameterModes.All => MergedValues,
            CommandParameterModes.Last => LastValues,
            _ => FirstValues,
        };
    }

    /// <summary>
    /// Gets the words of the string value of a specific parameter matched the key.
    /// </summary>
    /// <param name="index">The parameter index.</param>
    /// <returns>The words of the value of a specific parameter.</returns>
    public IReadOnlyList<string> Values(int index)
    {
        return Items[index].Values;
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Gets the string value of a specific parameter matched the key.
    /// </summary>
    /// <param name="index">The parameter index.</param>
    /// <returns>A string value of a specific parameter.</returns>
    public string Value(Index index)
    {
        return Items[index].Value;
    }

    /// <summary>
    /// Gets the words of the string value of a specific parameter matched the key.
    /// </summary>
    /// <param name="index">The parameter index.</param>
    /// <returns>The words of the value of a specific parameter.</returns>
    public IReadOnlyList<string> Values(Index index)
    {
        return Items[index].Values;
    }
#endif

    /// <summary>
    /// Converts the value to its boolean equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryToParse(out bool result, CommandParameterModes mode = CommandParameterModes.First)
    {
        var str = Value(mode);
        if (!string.IsNullOrWhiteSpace(str)) return bool.TryParse(str, out result);
        result = true;
        return true;
    }

    /// <summary>
    /// Converts the value to its 32-bit signed integer equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryToParse(out int result, CommandParameterModes mode = CommandParameterModes.First)
    {
        return Maths.Numbers.TryParseToInt32(Value(mode), 10, out result);
    }

    /// <summary>
    /// Converts the value to its 64-bit signed integer equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryToParse(out long result, CommandParameterModes mode = CommandParameterModes.First)
    {
        return Maths.Numbers.TryParseToInt64(Value(mode), 10, out result);
    }

    /// <summary>
    /// Converts the value to its single-precision floating-point number equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryToParse(out float result, CommandParameterModes mode = CommandParameterModes.First)
    {
        return float.TryParse(Value(mode), out result);
    }

    /// <summary>
    /// Converts the value to its double-precision floating-point number equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryToParse(out double result, CommandParameterModes mode = CommandParameterModes.First)
    {
        return double.TryParse(Value(mode), out result);
    }

    /// <summary>
    /// Converts the value to its GUID equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryToParse(out Guid result, CommandParameterModes mode = CommandParameterModes.First)
    {
        return Guid.TryParse(Value(mode), out result);
    }

    /// <summary>
    /// Converts the value to its date and time equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryToParse(out DateTime result, CommandParameterModes mode = CommandParameterModes.First)
    {
        return DateTime.TryParse(Value(mode), out result);
    }

    /// <summary>
    /// Converts the value to its date and time with offset relative to UTC equivalent.
    /// </summary>
    /// <param name="result">The result value converted when this method returns.</param>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>true if the value was converted successfully; otherwise, false.</returns>
    public bool TryToParse(out DateTimeOffset result, CommandParameterModes mode = CommandParameterModes.First)
    {
        return DateTimeOffset.TryParse(Value(mode), out result);
    }

    /// <summary>
    /// Converts the value to its URI equivalent.
    /// </summary>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>The result value converted.</returns>
    public Uri ParseToUri(CommandParameterModes mode = CommandParameterModes.First)
    {
        var v = Value(mode);
        if (string.IsNullOrWhiteSpace(v)) return null;
        return new Uri(v);
    }

    /// <summary>
    /// Converts the value to its HTTP URI equivalent.
    /// </summary>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>The result value converted.</returns>
    public HttpUri ParseToHttpUri(CommandParameterModes mode = CommandParameterModes.First)
    {
        var v = Value(mode);
        if (string.IsNullOrWhiteSpace(v)) return null;
        return HttpUri.Parse(v);
    }

    /// <summary>
    /// Converts the value to its query data equivalent.
    /// </summary>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>The result value converted.</returns>
    public QueryData ParseToQueryData(CommandParameterModes mode = CommandParameterModes.First)
    {
        var v = Value(mode);
        if (string.IsNullOrWhiteSpace(v)) return null;
        return QueryData.Parse(v);
    }

    /// <summary>
    /// Converts the value to its JSON object equivalent.
    /// </summary>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>The result value converted.</returns>
    public JsonObjectNode ParseToJsonObject(CommandParameterModes mode = CommandParameterModes.First)
    {
        var v = Value(mode);
        if (string.IsNullOrWhiteSpace(v)) return null;
        return JsonObjectNode.Parse(v);
    }

    /// <summary>
    /// Converts the value to its JSON array equivalent.
    /// </summary>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>The result value converted.</returns>
    public JsonArrayNode ParseToJsonArray(CommandParameterModes mode = CommandParameterModes.First)
    {
        var v = Value(mode);
        if (string.IsNullOrWhiteSpace(v)) return null;
        return JsonArrayNode.Parse(v);
    }

    /// <summary>
    /// Converts the value to its JSON document equivalent.
    /// </summary>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>The result value converted.</returns>
    public JsonDocument ParseToJsonDocument(CommandParameterModes mode = CommandParameterModes.First)
    {
        var v = Value(mode);
        if (string.IsNullOrWhiteSpace(v)) return null;
        return JsonDocument.Parse(v);
    }

    /// <summary>
    /// Converts the value to its file information object equivalent.
    /// </summary>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>The result value converted.</returns>
    public FileInfo ParseToFileInfo(CommandParameterModes mode = CommandParameterModes.First)
    {
        var v = Value(mode);
        if (string.IsNullOrWhiteSpace(v)) return null;
        try
        {
            return new FileInfo(v);
        }
        catch (ArgumentException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (PathTooLongException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (FileNotFoundException)
        {
        }

        return null;
    }

    /// <summary>
    /// Converts the value to its directory information object equivalent.
    /// </summary>
    /// <param name="mode">The parameter resolving mode.</param>
    /// <returns>The result value converted.</returns>
    public DirectoryInfo ParseToDirectoryInfo(CommandParameterModes mode = CommandParameterModes.First)
    {
        var v = Value(mode);
        if (string.IsNullOrWhiteSpace(v)) return null;
        try
        {
            return new DirectoryInfo(v);
        }
        catch (ArgumentException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (PathTooLongException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (DirectoryNotFoundException)
        {
        }

        return null;
    }

    /// <summary>
    /// Converts the value of this instance to the parameters string.
    /// </summary>
    /// <returns>A string whose value is the same as this instance.</returns>
    public override string ToString()
    {
        var str = new StringBuilder();
        var sep = IsUrlQueryLike ? '&' : ' ';
        foreach (var item in Items)
        {
            if (str.Length > 0) str.Append(sep);
            str.Append(item.ToString());
        }

        return str.ToString();
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /// <summary>
    /// Determines whether the value of this instance and the specified one have the same value.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
    public bool Equals(CommandParameters other)
    {
        if (other == null) return false;
        return ToString() == other.ToString();
    }

    /// <summary>
    /// Determines whether the value of this instance and the specified one have the same value.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
    public bool Equals(CommandParameter other)
    {
        if (other == null) return false;
        return ToString() == other.ToString();
    }

    /// <summary>
    /// Determines whether the value of this instance and the specified one have the same value.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
    public bool Equals(string other)
    {
        return ToString() == other;
    }

    /// <summary>
    /// Determines whether the value of this instance and the specified one have the same value.
    /// </summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (other is CommandParameters parameters) return Equals(parameters);
        if (other is CommandParameter parameter) return Equals(parameter);
        if (other is string str) return Equals(str);
        return false;
    }

    /// <summary>
    /// Returns an enumerator that iterates through this instance.
    /// </summary>
    /// <returns>A enumerator.</returns>
    public IEnumerator<CommandParameter> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through this instance.
    /// </summary>
    /// <returns>A enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Items.GetEnumerator();
    }
}
