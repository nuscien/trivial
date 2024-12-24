using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Data;
using Trivial.Reflection;
using Trivial.Tasks;
using Trivial.Web;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using Trivial.IO;

namespace Trivial.Text;

/// <summary>
/// The extensions for class IJsonValueNode, JsonDocument, JsonElement, etc.
/// </summary>
public static class JsonValues
{
    internal const string SELF_REF = "@";

    /// <summary>
    /// The string of null.
    /// </summary>
    public const string NullString = "null";

    /// <summary>
    /// Gets the MIME content type of JSON format text.
    /// </summary>
    public const string JsonMIME = "application/json";

    /// <summary>
    /// Gets the MIME content type of JSON lines format text.
    /// </summary>
    public const string JsonlMIME = "application/jsonl";

    /// <summary>
    /// The default JSON serializer options.
    /// </summary>
    internal static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>
    /// JSON null.
    /// </summary>
    public static readonly BaseJsonValueNode Null = new JsonNullNode(JsonValueKind.Null);

    /// <summary>
    /// JSON undefined.
    /// </summary>
    public static readonly BaseJsonValueNode Undefined = new JsonNullNode(JsonValueKind.Undefined);

    /// <summary>
    /// Parses JSON.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader.</param>
    /// <returns>The JSON node instance.</returns>
    public static BaseJsonValueNode Parse(ref Utf8JsonReader reader)
        => Parse(ref reader, false);

    /// <summary>
    /// Parses JSON.
    /// </summary>
    /// <param name="reader">The UTF-8 JSON reader.</param>
    /// <param name="hasRead">true if do not read next but parse directly; otherwise, false.</param>
    /// <returns>The JSON node instance.</returns>
    internal static BaseJsonValueNode Parse(ref Utf8JsonReader reader, bool hasRead)
    {
        if (!hasRead && !reader.Read()) return null;
        SkipComments(ref reader);
        return reader.TokenType switch
        {
            JsonTokenType.Null => Null,
            JsonTokenType.StartObject => new JsonObjectNode(ref reader),
            JsonTokenType.StartArray => JsonArrayNode.ParseValue(ref reader),
            JsonTokenType.String => new JsonStringNode(reader.GetString()),
            JsonTokenType.Number => reader.TryGetInt64(out var int64v) ? new JsonIntegerNode(int64v) : new JsonDoubleNode(reader.GetDouble()),
            JsonTokenType.True => JsonBooleanNode.True,
            JsonTokenType.False => JsonBooleanNode.False,
            _ => null,
        };
    }

    /// <summary>
    /// Parses JSON.
    /// </summary>
    /// <param name="json">The UTF-8 encoded JSON text to process.</param>
    /// <param name="options">Options that define customized behavior of the UTF-8 JSON reader that differs from the JSON RFC (for example, how to handle comments or maximum depth allowed when reading). By default, the reader follows the JSON RFC strictly; comments within the JSON are invalid, and the maximum depth is 64.</param>
    /// <returns>The JSON node instance.</returns>
    public static BaseJsonValueNode Parse(ReadOnlySpan<byte> json, JsonReaderOptions options)
    {
        var reader = new Utf8JsonReader(json, options);
        if (!reader.Read()) return null;
        return Parse(ref reader);
    }

    /// <summary>
    /// Parses JSON.
    /// </summary>
    /// <param name="json">The UTF-8 encoded JSON text to process.</param>
    /// <param name="options">Options that define customized behavior of the UTF-8 JSON reader that differs from the JSON RFC (for example, how to handle comments or maximum depth allowed when reading). By default, the reader follows the JSON RFC strictly; comments within the JSON are invalid, and the maximum depth is 64.</param>
    /// <returns>The JSON node instance.</returns>
    public static BaseJsonValueNode Parse(ReadOnlySequence<byte> json, JsonReaderOptions options)
    {
        var reader = new Utf8JsonReader(json, options);
        if (!reader.Read()) return null;
        return Parse(ref reader);
    }

    /// <summary>
    /// Converts from JSON document.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>The JSON value.</returns>
    public static BaseJsonValueNode ToJsonValue(JsonDocument json)
    {
        if (json is null) return null;
        return ToJsonValue(json.RootElement);
    }

    /// <summary>
    /// Converts from JSON element.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>The JSON value; or null, if not supported.</returns>
    public static BaseJsonValueNode ToJsonValue(JsonElement json)
    {
        return json.ValueKind switch
        {
            JsonValueKind.Undefined => Undefined,
            JsonValueKind.Null => Null,
            JsonValueKind.String => new JsonStringNode(json.GetString()),
            JsonValueKind.Number => json.TryGetInt64(out var l)
                ? new JsonIntegerNode(l)
                : (json.TryGetDouble(out var d) ? new JsonDoubleNode(d) : Null),
            JsonValueKind.True => JsonBooleanNode.True,
            JsonValueKind.False => JsonBooleanNode.False,
            JsonValueKind.Array => (JsonArrayNode)json,
            JsonValueKind.Object => (JsonObjectNode)json,
            _ => null
        };
    }

    /// <summary>
    /// Converts from JSON element.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>The JSON value; or null, if failed.</returns>
    public static BaseJsonValueNode ToJsonValue(System.Text.Json.Nodes.JsonNode json)
    {
        try
        {
            return (BaseJsonValueNode)json;
        }
        catch (InvalidCastException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (OverflowException)
        {
        }
        catch (ArgumentException)
        {
        }

        return null;
    }

    /// <summary>
    /// Converts from JSON reader.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <returns>The JSON value; or null, if failed.</returns>
    public static BaseJsonValueNode ToJsonValue(ref Utf8JsonReader reader)
    {
        SkipComments(ref reader);
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return Null;
            case JsonTokenType.StartObject:
                return new JsonObjectNode(ref reader);
            case JsonTokenType.StartArray:
                return JsonArrayNode.ParseValue(ref reader);
            case JsonTokenType.String:
                var str = reader.GetString();
                return new JsonStringNode(str);
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out var int64v)) return new JsonIntegerNode(int64v);
                return new JsonDoubleNode(reader.GetDouble());
            case JsonTokenType.True:
                return JsonBooleanNode.True;
            case JsonTokenType.False:
                return JsonBooleanNode.False;
            default:
                return null;
        }
    }

    /// <summary>
    /// Gets the JSON array format string of the value.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <returns>A JSON array format string.</returns>
    public static string ToJsonArrayString(this IEnumerable<JsonObjectNode> col)
    {
        var arr = new JsonArrayNode();
        arr.AddRange(col);
        return arr.ToString();
    }

    /// <summary>
    /// Gets the JSON array format string of the value.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="indentStyle">The indent style.</param>
    /// <returns>A JSON array format string.</returns>
    public static string ToJsonArrayString(this IEnumerable<JsonObjectNode> col, IndentStyles indentStyle)
    {
        var arr = new JsonArrayNode();
        arr.AddRange(col);
        return arr.ToString(indentStyle);
    }

    /// <summary>
    /// Gets the JSON lines format string of the value.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <returns>A JSON lines format string.</returns>
    public static string ToJsonlString(this IEnumerable<JsonObjectNode> col)
    {
        if (col == null) return null;
        var str = new StringBuilder();
        foreach (var prop in col)
        {
            if (prop is null)
            {
                str.AppendLine("null");
                continue;
            }

            str.AppendLine(prop.ToString());
        }

        return str.ToString();
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="source">The collection source.</param>
    /// <param name="predicate">A function to test each source element for a condition.</param>
    /// <returns>A collection that contains elements from the input sequence that satisfy the condition.</returns>
    /// <exception cref="ArgumentNullException">source or predicate is null.</exception>
    public static IEnumerable<BaseJsonValueNode> Where(this IEnumerable<BaseJsonValueNode> source, Func<JsonValueKind, object, int, bool> predicate)
    {
        if (source == null) throw ObjectConvert.ArgumentNull(nameof(source));
        if (predicate == null) throw ObjectConvert.ArgumentNull(nameof(predicate));
        var index = -1;
        foreach (var value in source)
        {
            index++;
            if (predicate(value.ValueKind, GetValue(value), index)) yield return value;
        }
    }

    /// <summary>
    /// Attempts to represent the current JSON string or JavaScript date tidks number as a date time.
    /// </summary>
    /// <param name="json">The JSON element.</param>
    /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
    /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    public static bool TryGetJavaScriptDateTicks(this JsonElement json, out DateTime value)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.String:
                if (json.TryGetDateTime(out DateTime tmp)) break;
                value = tmp;
                return true;
            case JsonValueKind.Number:
                if (!json.TryGetInt64(out long tick)) break;
                value = WebFormat.ParseDate(tick);
                return true;
            default:
                throw new InvalidOperationException("The value kind should be string or number.");
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to represent the current JSON string or Unix timestamps number as a date time.
    /// </summary>
    /// <param name="json">The JSON element.</param>
    /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
    /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    public static bool TryGetUnixTimestamps(this JsonElement json, out DateTime value)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.String:
                if (json.TryGetDateTime(out DateTime tmp)) break;
                value = tmp;
                return true;
            case JsonValueKind.Number:
                if (!json.TryGetInt64(out long tick)) break;
                value = WebFormat.ParseUnixTimestamp(tick);
                return true;
            default:
                throw new InvalidOperationException("The value kind should be string or number.");
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to represent the current JSON string or Windows file time number as a date time.
    /// </summary>
    /// <param name="json">The JSON element.</param>
    /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
    /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    public static bool TryGetWindowsFileTimeUtc(this JsonElement json, out DateTime value)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.String:
                if (json.TryGetDateTime(out DateTime tmp)) break;
                value = tmp;
                return true;
            case JsonValueKind.Number:
                if (!json.TryGetInt64(out long tick)) break;
                value = DateTime.FromFileTimeUtc(tick);
                return true;
            default:
                throw new InvalidOperationException("The value kind should be string or number.");
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to a string.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetString(IJsonValueNode node, out string result)
    {
        if (node != null) return node.TryConvert(false, out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to a string.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static string TryGetString(IJsonValueNode node)
        => node != null && node.TryConvert(false, out string result) ? result : null;

    /// <summary>
    /// Tries to convert a JSON value node to an integer.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetInt16(IJsonValueNode node, out short result)
    {
        if (node != null) return node.TryConvert(false, out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to an integer.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static short? TryGetInt16(IJsonValueNode node)
        => node != null && node.TryConvert(false, out short result) ? result : null;

    /// <summary>
    /// Tries to convert a JSON value node to an integer.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetUInt32(IJsonValueNode node, out uint result)
    {
        if (node != null) return node.TryConvert(false, out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to an integer.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static uint? TryGetUInt32(IJsonValueNode node)
        => node != null && node.TryConvert(false, out uint result) ? result : null;

    /// <summary>
    /// Tries to convert a JSON value node to an integer.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetInt32(IJsonValueNode node, out int result)
    {
        if (node != null) return node.TryConvert(false, out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to an integer.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static int? TryGetInt32(IJsonValueNode node)
        => node != null && node.TryConvert(false, out int result) ? result : null;

    /// <summary>
    /// Tries to convert a JSON value node to an integer.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetUInt64(IJsonValueNode node, out ulong result)
    {
        if (node != null) return node.TryConvert(false, out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to an integer.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static ulong? TryGetUInt64(IJsonValueNode node)
        => node != null && node.TryConvert(false, out ulong result) ? result : null;

    /// <summary>
    /// Tries to convert a JSON value node to an integer.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetInt64(IJsonValueNode node, out long result)
    {
        if (node != null) return node.TryConvert(false, out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to an integer.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static long? TryGetInt64(IJsonValueNode node)
        => node != null && node.TryConvert(false, out long result) ? result : null;

    /// <summary>
    /// Tries to convert a JSON value node to an floating number.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetSingle(IJsonValueNode node, out float result)
    {
        if (node != null) return node.TryConvert(false, out result);
        result = float.NaN;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to an floating number.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static float TryGetSingle(IJsonValueNode node)
        => node != null && node.TryConvert(false, out float result) ? result : float.NaN;

    /// <summary>
    /// Tries to convert a JSON value node to an floating number.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetDouble(IJsonValueNode node, out double result)
    {
        if (node != null) return node.TryConvert(false, out result);
        result = double.NaN;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to an floating number.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static double TryGetDouble(IJsonValueNode node)
        => node != null && node.TryConvert(false, out double result) ? result : double.NaN;

    /// <summary>
    /// Tries to convert a JSON value node to an floating number.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetDecimal(IJsonValueNode node, out decimal result)
    {
        if (node != null) return node.TryConvert(false, out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to an floating number.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static decimal? TryGetDecimal(IJsonValueNode node)
        => node != null && node.TryConvert(false, out decimal result) ? result : null;

    /// <summary>
    /// Tries to convert a JSON value node to a boolean.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetBoolean(IJsonValueNode node, out bool result)
    {
        if (node != null) return node.TryConvert(false, out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to a boolean.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static bool? TryGetBoolean(IJsonValueNode node)
        => node != null && node.TryConvert(false, out bool result) ? result : null;

    /// <summary>
    /// Tries to convert a JSON value node to a date time.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetDateTime(IJsonValueNode node, out DateTime result)
    {
        if (node != null) return node.TryConvert(out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to a date time.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static DateTime? TryGetDateTime(IJsonValueNode node)
        => node != null && node.TryConvert(out DateTime result) ? result : WebFormat.ZeroTick;

    /// <summary>
    /// Tries to convert a JSON value node to a GUID.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if convert succeeded; otherwise, false.</returns>
    public static bool TryGetGuid(IJsonValueNode node, out Guid result)
    {
        if (node != null) return node.TryConvert(out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to convert a JSON value node to a GUID.
    /// </summary>
    /// <param name="node">The JSON value node.</param>
    /// <returns>The result converted; or null, if not supported.</returns>
    public static Guid? TryGetGuid(IJsonValueNode node)
        => node != null && node.TryConvert(out Guid result) ? result : Guid.Empty;

    /// <summary>
    /// Tries to get the value of the specific property for each object.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="ignoreNotMatched">true if ignore any item which is not JSON object; otherwise, false.</param>
    /// <returns>The property list.</returns>
    public static List<BaseJsonValueNode> TryGetValues(this IEnumerable<JsonObjectNode> col, string key, bool ignoreNotMatched = false)
    {
        var list = new List<BaseJsonValueNode>();
        if (col == null) return list;
        if (ignoreNotMatched)
        {
            foreach (var item in col)
            {
                var json = item?.TryGetValue(key);
                if (json == null || json.ValueKind == JsonValueKind.Undefined) continue;
                list.Add(json);
            }
        }
        else
        {
            foreach (var item in col)
            {
                var json = item?.TryGetValue(key);
                list.Add(json ?? Undefined);
            }
        }

        return list;
    }

    /// <summary>
    /// Tries to get the value of the specific property for each object.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="ignoreNotMatched">true if ignore any item which is not JSON object; otherwise, false.</param>
    /// <returns>The property list.</returns>
    public static List<JsonObjectNode> TryGetObjectValues(this IEnumerable<JsonObjectNode> col, string key, bool ignoreNotMatched = false)
    {
        var list = new List<JsonObjectNode>();
        if (col == null) return list;
        if (ignoreNotMatched)
        {
            foreach (var item in col)
            {
                var json = item?.TryGetObjectValue(key);
                if (json == null) continue;
                list.Add(json);
            }
        }
        else
        {
            foreach (var item in col)
            {
                var json = item?.TryGetObjectValue(key);
                list.Add(json);
            }
        }

        return list;
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public static StringBuilder Append(this StringBuilder sb, IJsonValueNode<string> value)
    {
        if (sb == null) return null;
        sb.Append(value.Value);
        return sb;
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public static StringBuilder Append(this StringBuilder sb, IJsonValueNode<int> value)
    {
        if (sb == null) return null;
        sb.Append(value.Value);
        return sb;
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public static StringBuilder Append(this StringBuilder sb, IJsonValueNode<long> value)
    {
        if (sb == null) return null;
        sb.Append(value.Value);
        return sb;
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public static StringBuilder Append(this StringBuilder sb, IJsonValueNode<float> value)
    {
        if (sb == null) return null;
        sb.Append(value.Value);
        return sb;
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public static StringBuilder Append(this StringBuilder sb, IJsonValueNode<double> value)
    {
        if (sb == null) return null;
        sb.Append(value.Value);
        return sb;
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public static StringBuilder Append(this StringBuilder sb, IJsonValueNode<decimal> value)
    {
        if (sb == null) return null;
        sb.Append(value.Value);
        return sb;
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public static StringBuilder Append(this StringBuilder sb, IJsonValueNode<bool> value)
    {
        if (sb == null) return null;
        sb.Append(value.Value);
        return sb;
    }

    /// <summary>
    /// Switches.
    /// </summary>
    /// <typeparam name="T">The type of the JSON node.</typeparam>
    /// <param name="source">The JSON node source.</param>
    /// <returns>A switch-case context for the JSON node.</returns>
    public static JsonSwitchContext<T, JsonObjectNode> Switch<T>(this T source) where T : IJsonValueNode
        => new(source, new());

    /// <summary>
    /// Switches.
    /// </summary>
    /// <typeparam name="TNode">The type of JSON node.</typeparam>
    /// <typeparam name="TArgs">The type of args.</typeparam>
    /// <param name="source">The JSON node source.</param>
    /// <param name="args">The argument object.</param>
    /// <returns>A switch-case context for the JSON node.</returns>
    public static JsonSwitchContext<TNode, TArgs> Switch<TNode, TArgs>(this TNode source, TArgs args) where TNode : IJsonValueNode
        => new(source, args);

    /// <summary>
    /// Tests if the JSON node is a specific kind.
    /// </summary>
    /// <param name="value">The source JSON node.</param>
    /// <param name="kind">The JSON value kind to test.</param>
    /// <param name="callback">The optional callback handler.</param>
    /// <param name="fallback">The optional fallback handler.</param>
    /// <returns>true the value kind is the same; otherwise, false.</returns>
    public static bool Is(IJsonValueNode value, JsonValueKind kind, Action<IJsonValueNode> callback = null, Action<IJsonValueNode> fallback = null)
    {
        value ??= Null;
        if (value.ValueKind != kind)
        {
            fallback?.Invoke(value);
            return false;
        }

        callback?.Invoke(value);
        return true;
    }

    /// <summary>
    /// Tests if the JSON node is a specific kind.
    /// </summary>
    /// <typeparam name="T">The type of args.</typeparam>
    /// <param name="value">The source JSON node.</param>
    /// <param name="args">The context args of callback handler.</param>
    /// <param name="kind">The JSON value kind to test.</param>
    /// <param name="callback">The optional callback handler.</param>
    /// <param name="fallback">The optional fallback handler.</param>
    /// <returns>true the value kind is the same; otherwise, false.</returns>
    public static bool Is<T>(IJsonValueNode value, JsonValueKind kind, T args, Action<IJsonValueNode, T> callback, Action<IJsonValueNode, T> fallback = null)
    {
        value ??= Null;
        if (value.ValueKind != kind)
        {
            fallback?.Invoke(value, args);
            return false;
        }

        callback?.Invoke(value, args);
        return true;
    }

    /// <summary>
    /// Appends a copy of the specified string to this instance.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="value">The string to append.</param>
    /// <returns>A reference to this instance after the append operation has completed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed System.Text.StringBuilder.MaxCapacity.</exception>
    public static StringBuilder AppendLine(this StringBuilder sb, IJsonValueNode<string> value)
    {
        if (sb == null) return null;
        sb.AppendLine(value.Value);
        return sb;
    }

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A string collection to filter.</param>
    /// <param name="kind">The kind to filter.</param>
    /// <returns>A string collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<IJsonValueNode> Where(this IEnumerable<IJsonValueNode> source, JsonValueKind kind)
    {
        if (source == null) return null;
        return source.Where(ele => ele.ValueKind == kind);
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, string value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item?.TryGetStringValue(key, true, out var s) != true || value != s) continue;
            yield return item;
        }
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <param name="callback">The callback with item, original index and filtered index.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, string value, Action<JsonObjectNode, int, int> callback)
    {
        if (col == null) yield break;
        var i = -1;
        var j = -1;
        foreach (var item in col)
        {
            i++;
            if (item?.TryGetStringValue(key, true, out var s) != true || value != s) continue;
            j++;
            yield return item;
            callback?.Invoke(item, i, j);
        }
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, string value, StringComparison comparisonType)
    {
        if (col == null) yield break;
        if (value == null)
        {
            foreach (var item in col)
            {
                if (!item.IsNullOrUndefined(key)) continue;
                yield return item;
            }

            yield break;
        }

        foreach (var item in col)
        {
            if (item?.TryGetStringValue(key, true, out var s) != true || !value.Equals(s, comparisonType)) continue;
            yield return item;
        }
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <param name="callback">The callback with item, original index and filtered index.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, string value, StringComparison comparisonType, Action<JsonObjectNode, int, int> callback)
    {
        if (col == null) yield break;
        var i = -1;
        var j = -1;
        if (value == null)
        {
            foreach (var item in col)
            {
                i++;
                if (!item.IsNullOrUndefined(key)) continue;
                j++;
                yield return item;
                callback?.Invoke(item, i, j);
            }

            yield break;
        }

        foreach (var item in col)
        {
            i++;
            if (item?.TryGetStringValue(key, true, out var s) != true || !value.Equals(s, comparisonType)) continue;
            j++;
            yield return item;
            callback?.Invoke(item, i, j);
        }
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, int value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item?.TryGetInt32Value(key, out var s) != true || value != s) continue;
            yield return item;
        }
    }

    /// <summary>
    /// Filters the JSON object collection by the property.
    /// </summary>
    /// <param name="col">The JSON object collection.</param>
    /// <param name="key">The property key.</param>
    /// <param name="value">The expected value of the property.</param>
    /// <returns>The collection after filter.</returns>
    public static IEnumerable<JsonObjectNode> WherePropertyEquals(this IEnumerable<JsonObjectNode> col, string key, long value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item?.TryGetInt64Value(key, out var s) != true || value != s) continue;
            yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key, string value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item is not null && item.TryGetStringValue(key) == value) yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key, string value, StringComparison comparisonType)
    {
        if (col == null) yield break;
        if (value == null)
        {
            foreach (var item in col)
            {
                if (item is not null && item.IsNullOrUndefined(key)) yield return item;
            }
        }
        else
        {
            foreach (var item in col)
            {
                if (item is not null && value.Equals(item.TryGetStringValue(key), comparisonType)) yield return item;
            }
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key, int value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item is not null && item.TryGetInt32Value(key) == value) yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key, bool value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item is not null && item.TryGetBooleanValue(key) == value) yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="kind">The kind of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key, JsonValueKind kind)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item is not null && item.GetValueKind(key) == kind) yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item is not null && item.ContainsKey(key)) yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key, string value)
        => WithProperty(array?.SelectObjects(), key, value);

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key, string value, StringComparison comparisonType)
        => WithProperty(array?.SelectObjects(), key, value, comparisonType);

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key, int value)
        => WithProperty(array?.SelectObjects(), key, value);

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key, bool value)
        => WithProperty(array?.SelectObjects(), key, value);

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="kind">The kind of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key, JsonValueKind kind)
        => WithProperty(array?.SelectObjects(), key, kind);

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key)
        => WithProperty(array?.SelectObjects(), key);

    /// <summary>
    /// Converts to JSON object node collection.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <param name="ignoreNotMatched">true if ignore any item which is not JSON object; otherwise, false.</param>
    /// <returns>A JSON object node collection converted.</returns>
    public static IEnumerable<JsonObjectNode> ToJsonObjectNodes(this IEnumerable<IJsonValueNode> collection, bool ignoreNotMatched = false)
    {
        if (collection == null) yield break;
        if (ignoreNotMatched)
        {
            foreach (var item in collection)
            {
                if (item is JsonObjectNode json) yield return json;
            }
        }
        else
        {
            foreach (var item in collection)
            {
                yield return item as JsonObjectNode;
            }
        }
    }

    /// <summary>
    /// Converts to JSON object node collection.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON object node collection converted.</returns>
    public static IEnumerable<JsonObjectNode> ToJsonObjectNodes(this IEnumerable<IJsonObjectHost> collection)
    {
        if (collection == null) yield break;
        foreach (var item in collection)
        {
            yield return item?.ToJson();
        }
    }

    /// <summary>
    /// Converts to JSON array.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON array node converted.</returns>
    public static JsonArrayNode ToJsonArrayNode(this IEnumerable<IJsonObjectHost> collection)
    {
        if (collection == null) return null;
        var arr = new JsonArrayNode();
        foreach (var item in collection)
        {
            arr.Add(item);
        }

        return arr;
    }

    /// <summary>
    /// Converts to JSON array.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON array node converted.</returns>
    public static JsonArrayNode ToJsonArrayNode(this IEnumerable<JsonObjectNode> collection)
    {
        if (collection == null) return null;
        var arr = new JsonArrayNode();
        arr.AddRange(collection);
        return arr;
    }

    /// <summary>
    /// Converts to JSON array.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON array node converted.</returns>
    public static JsonArrayNode ToJsonArrayNode(IEnumerable<string> collection)
    {
        if (collection == null) return null;
        var arr = new JsonArrayNode();
        arr.AddRange(collection);
        return arr;
    }

    /// <summary>
    /// Converts to JSON array.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON array node converted.</returns>
    public static JsonArrayNode ToJsonArrayNode(IEnumerable<int> collection)
    {
        if (collection == null) return null;
        var arr = new JsonArrayNode();
        arr.AddRange(collection);
        return arr;
    }

    /// <summary>
    /// Converts a collection of JSON value node to a string collection.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="strict">true if enable strict mode what only its JSON value kind is string; otherwise, false.</param>
    /// <param name="removeNotMatched">true if remove the ones not matched; otherwise, false.</param>
    /// <returns>A string collection converted.</returns>
    public static IEnumerable<string> OfStringType<T>(this IEnumerable<T> collection, bool strict, bool removeNotMatched) where T : IJsonValueNode
    {
        if (collection == null) return null;
        var col = collection.Select(ele => ele.TryConvert(strict, out string s) ? s : null);
        if (removeNotMatched) col = col.Where(ele => ele != null);
        return col;
    }

    /// <summary>
    /// Converts a collection of JSON value node to a string collection.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>A string collection converted.</returns>
    /// <exception cref="ArgumentNullException">predicate is null.</exception>
    public static IEnumerable<string> OfStringType<T>(this IEnumerable<T> collection, Func<string, int, bool, JsonValueKind, bool> predicate) where T : IJsonValueNode
    {
        if (predicate is null) throw ObjectConvert.ArgumentNull(nameof(predicate));
        if (collection == null) yield break;
        var i = -1;
        foreach (var item in collection)
        {
            i++;
            var b = item.TryConvert(false, out string s);
            if (predicate(s, i, b, item.ValueKind)) yield return s;
        }
    }

    /// <summary>
    /// Converts a collection of JSON value node to a string collection.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>A string collection converted.</returns>
    /// <exception cref="ArgumentNullException">predicate is null.</exception>
    public static IEnumerable<string> OfStringType<T>(this IEnumerable<T> collection, Func<string, JsonValueKind, bool> predicate) where T : IJsonValueNode
    {
        if (predicate is null) throw ObjectConvert.ArgumentNull(nameof(predicate));
        if (collection == null) yield break;
        var i = -1;
        foreach (var item in collection)
        {
            i++;
            if (!item.TryConvert(false, out string s)) s = null;
            if (predicate(s, item.ValueKind)) yield return s;
        }
    }

    /// <summary>
    /// Converts a collection of JSON value node to a string collection.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="converter">A converter hanlder.</param>
    /// <param name="skipNull">true if skip null; otherwise, false.</param>
    /// <returns>A string collection converted.</returns>
    public static IEnumerable<string> OfStringType<T>(this IEnumerable<T> collection, Func<T, string> converter, bool skipNull) where T : IJsonValueNode
    {
        if (collection == null) yield break;
        var i = -1;
        converter ??= node => node.TryConvert(false, out string s) ? s : null;
        if (skipNull)
        {
            foreach (var item in collection)
            {
                i++;
                var s = converter(item);
                if (s != null) yield return s;
            }
        }
        else
        {
            foreach (var item in collection)
            {
                i++;
                yield return converter(item);
            }
        }
    }

    /// <summary>
    /// Converts a collection of JSON value node to a string collection.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="converter">A converter hanlder.</param>
    /// <param name="skipNull">true if skip null; otherwise, false.</param>
    /// <returns>A string collection converted.</returns>
    public static IEnumerable<string> OfStringType<T>(this IEnumerable<T> collection, Func<T, int, string> converter, bool skipNull) where T : IJsonValueNode
    {
        if (collection == null) yield break;
        var i = -1;
        converter ??= (node, i) => node.TryConvert(false, out string s) ? s : null;
        if (skipNull)
        {
            foreach (var item in collection)
            {
                i++;
                var s = converter(item, i);
                if (s != null) yield return s;
            }
        }
        else
        {
            foreach (var item in collection)
            {
                i++;
                yield return converter(item, i);
            }
        }
    }

    /// <summary>
    /// Converts to string format.
    /// </summary>
    /// <param name="value">The JSON object.</param>
    /// <param name="indent">The indent style.</param>
    /// <returns>The string converted in JSON format.</returns>
    public static string ToString(IJsonObjectHost value, IndentStyles indent = IndentStyles.Minified)
    {
        var json = value?.ToJson();
        if (json is null) return Null.ToString();
        return json.ToString(indent);
    }

    /// <summary>
    /// Converts to a specific type.
    /// </summary>
    /// <param name="kvp">The key value pair of JSON.</param>
    /// <param name="type">The type to convert.</param>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value converted.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="OverflowException">The value is outside the range of the underlying type of enum expected.</exception>
    /// <exception cref="NotSupportedException">The type is not supported to convert.</exception>
    public static object As(KeyValuePair<string, BaseJsonValueNode> kvp, Type type, bool strict = false)
        => (kvp.Value ?? Undefined).As(type, strict);

    /// <summary>
    /// Converts to a specific type.
    /// </summary>
    /// <typeparam name="T">The type to convert.</typeparam>
    /// <param name="kvp">The key value pair of JSON.</param>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value converted.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="OverflowException">The value is outside the range of the underlying type of enum expected.</exception>
    /// <exception cref="NotSupportedException">The type is not supported to convert.</exception>
    public static T As<T>(KeyValuePair<string, BaseJsonValueNode> kvp, bool strict = false)
        => (kvp.Value ?? Undefined).As<T>(strict);

    /// <summary>
    /// Converts to a specific type.
    /// </summary>
    /// <typeparam name="T">The type to convert.</typeparam>
    /// <param name="kvp">The key value pair of JSON.</param>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value converted.</returns>
    public static T TryConvert<T>(KeyValuePair<string, BaseJsonValueNode> kvp, bool strict = false)
        => (kvp.Value ?? Undefined).TryConvert<T>(strict);

    /// <summary>
    /// Writes this instance to the specified writer as a JSON value.
    /// </summary>
    /// <param name="kvp">The key value pair of JSON.</param>
    /// <param name="writer">The writer to which to write this instance.</param>
    public static void WriteTo(KeyValuePair<string, BaseJsonValueNode> kvp, Utf8JsonWriter writer)
    {
        if (writer is null) return;
        writer.WritePropertyName(kvp.Key);
        (kvp.Value ?? Undefined).WriteTo(writer);
    }

    /// <summary>
    /// Gets JSON property name.
    /// </summary>
    /// <param name="property">The property name.</param>
    /// <returns>The JSON property name; or null, if ignore JSON serialization.</returns>
    public static string GetPropertyName(PropertyInfo property)
    {
        try
        {
            var attr = property.GetCustomAttributes<JsonIgnoreAttribute>()?.FirstOrDefault();
            if (attr != null && attr.Condition == JsonIgnoreCondition.Always) return null;
        }
        catch (NotSupportedException)
        {
        }
        catch (TypeLoadException)
        {
        }

        try
        {
            var attr = property.GetCustomAttributes<JsonPropertyNameAttribute>()?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(attr?.Name)) return attr.Name;
        }
        catch (NotSupportedException)
        {
        }
        catch (TypeLoadException)
        {
        }

        return property.Name;
    }

    /// <summary>
    /// Creates a JSON schema of a type.
    /// </summary>
    /// <param name="json">The JSON object to create schema.</param>
    /// <param name="desc">The description.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <returns>The JSON schema description instance; or null, if not supported.</returns>
    public static JsonNodeSchemaDescription CreateSchema(JsonObjectNode json, string desc = null, IJsonNodeSchemaCreationHandler<IJsonValueNode> handler = null)
        => CreateSchema(json, 10, desc, handler);

    /// <summary>
    /// Creates a JSON schema of a type.
    /// </summary>
    /// <typeparam name="T">The type to create JSON schema.</typeparam>
    /// <param name="desc">The description.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <returns>The JSON schema description instance; or null, if not supported.</returns>
    public static JsonNodeSchemaDescription CreateSchema<T>(string desc = null, IJsonNodeSchemaCreationHandler<Type> handler = null)
        => CreateSchema(typeof(T), 10, desc, handler);

    /// <summary>
    /// Creates a JSON schema of a type.
    /// </summary>
    /// <param name="type">The type to create JSON schema.</param>
    /// <param name="desc">The description.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <returns>The JSON schema description instance; or null, if not supported.</returns>
    public static JsonNodeSchemaDescription CreateSchema(Type type, string desc = null, IJsonNodeSchemaCreationHandler<Type> handler = null)
        => CreateSchema(type, 10, desc, handler);

    /// <summary>
    /// Creates media type header value of JSON.
    /// </summary>
    /// <returns>The media type header value of JSON.</returns>
    public static MediaTypeHeaderValue GetJsonMediaTypeHeaderValue()
#if NET8_0_OR_GREATER
        => new(JsonMIME, "utf-8");
#else
        => new(JsonMIME);
#endif

    /// <summary>
    /// Creates media type header value of JSON lines.
    /// </summary>
    /// <returns>The media type header value of JSON lines.</returns>
    public static MediaTypeHeaderValue GetJsonlMediaTypeHeaderValue()
#if NET8_0_OR_GREATER
        => new(JsonlMIME, "utf-8");
#else
        => new(JsonlMIME);
#endif

    /// <summary>
    /// Serializes an object into JSON format.
    /// </summary>
    /// <param name="stream">The stream to write.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="options">The optional serializer settings.</param>
    /// <returns>A JSON string.</returns>
    internal static void WriteTo(Stream stream, object obj, DataContractJsonSerializerSettings options)
        => WriteTo(stream, obj, null, (s, o, t, opt) =>
        {
            var serializer = options != null ? new DataContractJsonSerializer(t, options) : new DataContractJsonSerializer(t);
            serializer.WriteObject(stream, o);
        });

    /// <summary>
    /// Serializes an object into JSON format.
    /// </summary>
    /// <param name="stream">The stream to write.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="options">The serializer options.</param>
    /// <param name="converter">The optional fallback converter.</param>
    /// <returns>A JSON string.</returns>
    public static void WriteTo(Stream stream, object obj, JsonSerializerOptions options = null, Action<Stream, object, Type, JsonSerializerOptions> converter = null)
    {
        if (stream?.CanWrite != true) return;
        var writer = new Utf8JsonWriter(stream);
        var t = obj.GetType();
        if (obj == null)
        {
            writer.WriteNullValue();
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (obj is BaseJsonValueNode json)
        {
            json.WriteTo(writer);
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (obj is JsonDocument jsonDoc)
        {
            jsonDoc.WriteTo(writer);
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (obj is JsonElement jsonEle)
        {
            jsonEle.WriteTo(writer);
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (obj is System.Text.Json.Nodes.JsonNode jsonNode)
        {
            jsonNode.WriteTo(writer);
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (t.FullName.StartsWith("Newtonsoft.Json.Linq.J", StringComparison.InvariantCulture))
        {
            if (t.FullName.Equals("Newtonsoft.Json.Linq.JObject", StringComparison.InvariantCulture)
                || t.FullName.Equals("Newtonsoft.Json.Linq.JArray", StringComparison.InvariantCulture))
            {
                var str = obj.ToString();
                var sw = new StreamWriter(stream, Encoding.UTF8);
                sw.Write(str);
                sw.Flush();
                stream.Position = 0;
                return;
            }
        }

        if (obj is Security.TokenRequest tokenReq)
        {
            tokenReq.WriteTo(writer);
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (obj is IEnumerable<KeyValuePair<string, string>> col)
        {
            writer.WriteStartObject();
            foreach (var kvp in col)
            {
                writer.WriteString(kvp.Key, kvp.Value);
            }

            writer.WriteEndObject();
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (obj is string s)
        {
            writer.WriteStringValue(s);
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (obj is Uri uri)
        {
            try
            {
                writer.WriteStringValue(uri.OriginalString);
            }
            catch (InvalidOperationException)
            {
                writer.WriteStringValue(uri.ToString());
            }

            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (obj is IJsonValueNode jv)
        {
            var node = jv.ToJsonNode();
            if (node is null) writer.WriteNullValue();
            else node.WriteTo(writer);
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (obj is Net.HttpUri || obj is Net.AppDeepLinkUri uri3)
        {
            writer.WriteStringValue(obj.ToString());
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (t.IsValueType)
        {
            if (obj is bool b)
            {
                writer.WriteBooleanValue(b);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is int i32)
            {
                writer.WriteNumberValue(i32);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is long i64)
            {
                writer.WriteNumberValue(i64);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is float f1)
            {
                writer.WriteNumberValue(f1);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is uint i32u)
            {
                writer.WriteNumberValue(i32u);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is ulong i64u)
            {
                writer.WriteNumberValue(i64u);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is DateTime d)
            {
                writer.WriteStringValue(d.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is DateTimeOffset dto)
            {
                writer.WriteStringValue(dto.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is double f2)
            {
                writer.WriteNumberValue(f2);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is decimal f3)
            {
                writer.WriteNumberValue(f3);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is TimeSpan ts)
            {
                writer.WriteStringValue(ts.TotalSeconds.ToString("g", CultureInfo.InvariantCulture));
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (t == typeof(Guid) || t == typeof(Maths.Angle) || t == typeof(Geography.Longitude) || t == typeof(Geography.Latitude))
            {
                writer.WriteStringValue(obj.ToString());
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is byte i8)
            {
                writer.WriteNumberValue(i8);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is short i16)
            {
                writer.WriteNumberValue(i16);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is ushort i16u)
            {
                writer.WriteNumberValue(i16u);
                writer.Flush();
                stream.Position = 0;
                return;
            }

#if NET6_0_OR_GREATER
            if (obj is Half ha)
            {
                writer.WriteNumberValue((float)ha);
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is DateOnly date)
            {
                writer.WriteStringValue(date.ToString("yyyy-MM-dd"));
                writer.Flush();
                stream.Position = 0;
                return;
            }
#endif
#if NET8_0_OR_GREATER
            if (obj is Int128 i128)
            {
                if (i128 >= JsonIntegerNode.MinSafeInteger && i128 <= JsonIntegerNode.MaxSafeInteger) writer.WriteNumberValue((long)i128);
                else writer.WriteStringValue(i128.ToString("g"));
                writer.Flush();
                stream.Position = 0;
                return;
            }

            if (obj is UInt128 i128u)
            {
                if (i128u <= JsonIntegerNode.MaxSafeInteger) writer.WriteNumberValue((long)i128u);
                else writer.WriteStringValue(i128u.ToString("g"));
                writer.Flush();
                stream.Position = 0;
                return;
            }
#endif
        }

        if (t == typeof(DBNull))
        {
            writer.WriteNullValue();
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (t == typeof(StringBuilder) || t == typeof(Maths.Angle.Model) || t == typeof(Geography.Longitude.Model) || t == typeof(Geography.Latitude.Model))
        {
            writer.WriteStringValue(obj.ToString());
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (obj is IJsonObjectHost jsonHost)
        {
            json = jsonHost.ToJson();
            if (json is null) writer.WriteNullValue();
            else json.WriteTo(writer);
            writer.Flush();
            stream.Position = 0;
            return;
        }

        if (converter is not null) converter(stream, obj, t, options);
        else if (options is null) JsonSerializer.Serialize(stream, obj, t);
        else JsonSerializer.Serialize(stream, obj, t, options);
        stream.Position = 0;
        return;
    }

    internal static void SkipComments(ref Utf8JsonReader reader)
    {
        while (reader.TokenType == JsonTokenType.Comment)
        {
            reader.Skip();
        }
    }

    internal static JsonOperationDescription CreateDescriptionByAttribute(MemberInfo member)
    {
        var attr = member?.GetCustomAttributes<JsonOperationDescriptiveAttribute>()?.FirstOrDefault();
        if (attr?.DescriptiveType == null || !attr.DescriptiveType.IsClass) return null;
        if (member is PropertyInfo prop)
        {
            if (ObjectConvert.TryCreateInstance<IJsonOperationDescriptive<PropertyInfo>>(attr.DescriptiveType, out var d))
                return d.CreateDescription(attr.Id, prop);
        }

        if (member is MethodInfo method)
        {
            if (ObjectConvert.TryCreateInstance<IJsonOperationDescriptive<MethodInfo>>(attr.DescriptiveType, out var d))
                return d.CreateDescription(attr.Id, method);
        }

        return ObjectConvert.TryCreateInstance<IJsonOperationDescriptive>(attr.DescriptiveType, out var desc) ? desc.CreateDescription() : null;
    }

    /// <summary>
    /// Creates a JSON schema of a type.
    /// </summary>
    /// <param name="type">The type to create JSON schema.</param>
    /// <param name="level">The maximum level to create schema for JSON object.</param>
    /// <param name="desc">The description.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <param name="breadcrumb">The path breadcrumb.</param>
    /// <returns>The JSON schema description instance; or null, if not supported.</returns>
    private static JsonNodeSchemaDescription CreateSchema(Type type, int level, string desc = null, IJsonNodeSchemaCreationHandler<Type> handler = null, NodePathBreadcrumb<Type> breadcrumb = null)
    {
        handler ??= EmptyJsonNodeSchemaCreationHandler<Type>.Instance;
        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(ValueTask<>) || genericType == typeof(Task<>) || genericType == typeof(Lazy<>))
            {
                type = type.GetGenericArguments().FirstOrDefault();
                if (type == null) return null;
            }
        }

        breadcrumb ??= new(type, null);
        level--;
        if (string.IsNullOrWhiteSpace(desc)) desc = StringExtensions.GetDescription(type);
        var guid = ObjectConvert.GetGuid(type);
        if (ObjectConvert.IsNullableValueType(type, out var valueType)) type = valueType;
        if (type == typeof(object))
        {
            return handler.Convert(type, null, breadcrumb);
        }
        else if (type == typeof(string))
        {
            return handler.Convert(type, new JsonStringSchemaDescription
            {
                Description = desc,
                Tag = guid,
            }, breadcrumb);
        }
        else if (type == typeof(JsonObjectNode) || type == typeof(System.Text.Json.Nodes.JsonObject))
        {
            return handler.Convert(type, new JsonObjectSchemaDescription
            {
                Description = desc,
                Tag = guid,
            }, breadcrumb);
        }
        else if (type == typeof(JsonArrayNode) || type == typeof(System.Text.Json.Nodes.JsonArray))
        {
            return handler.Convert(type, new JsonArraySchemaDescription
            {
                Description = desc,
                Tag = guid,
            }, breadcrumb);
        }
        else if (type.IsEnum)
        {
            var d = new JsonStringSchemaDescription
            {
                Description = desc,
                Tag = guid,
            };
            CreateEnumSchema(type, d);
            return handler.Convert(type, d, breadcrumb);
        }
        else if (type.IsValueType)
        {
            if (type == typeof(int) || type == typeof(long) || type == typeof(uint) || type == typeof(short) || type == typeof(byte))
            {
                return handler.Convert(type, new JsonIntegerSchemaDescription
                {
                    Description = desc,
                    Tag = guid,
                }, breadcrumb);
            }
            else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                return handler.Convert(type, new JsonNumberSchemaDescription
                {
                    Description = desc,
                    Tag = guid,
                }, breadcrumb);
            }
            else if (type == typeof(bool))
            {
                return handler.Convert(type, new JsonBooleanSchemaDescription
                {
                    Description = desc,
                    Tag = guid,
                }, breadcrumb);
            }
            else if (type == typeof(Guid))
            {
                return handler.Convert(type, new JsonStringSchemaDescription
                {
                    Description = desc,
                    Tag = guid,
                }, breadcrumb);
            }
            else if (type == typeof(DateTime))
            {
                return handler.Convert(type, new JsonStringSchemaDescription
                {
                    Description = desc,
                    Format = "date-time",
                    Tag = guid,
                }, breadcrumb);
            }
#if NET6_0_OR_GREATER
            else if (type == typeof(Half))
            {
                return handler.Convert(type, new JsonNumberSchemaDescription
                {
                    Description = desc,
                    Tag = guid,
                }, breadcrumb);
            }
            else if (type == typeof(DateOnly))
            {
                return handler.Convert(type, new JsonStringSchemaDescription
                {
                    Description = desc,
                    Format = "date",
                    Tag = guid,
                }, breadcrumb);
            }
            else if (type == typeof(TimeOnly))
            {
                return handler.Convert(type, new JsonStringSchemaDescription
                {
                    Description = desc,
                    Format = "time",
                    Tag = guid,
                }, breadcrumb);
            }
#endif

            return handler.Convert(type, null, breadcrumb);
        }
        else if (type == typeof(StringBuilder))
        {
            return handler.Convert(type, new JsonStringSchemaDescription
            {
                Description = desc,
                Tag = guid,
            }, breadcrumb);
        }
        else if (type == typeof(Uri))
        {
            return handler.Convert(type, new JsonStringSchemaDescription
            {
                Description = desc,
                Format = "uri-reference",
                Tag = guid,
            }, breadcrumb);
        }
        else if (ObjectConvert.IsGenericEnumerable(type, out var genericParamType))
        {
            var d = new JsonArraySchemaDescription
            {
                Description = desc,
                Tag = guid,
            };
            if (level < 0) return handler.Convert(type, d, breadcrumb);
            try
            {
                var jsonSerializer = type.GetCustomAttributes<JsonConverterAttribute>()?.FirstOrDefault();
                if (jsonSerializer?.ConverterType != null)
                {
                    var jsonSchema2 = new JsonObjectSchemaDescription
                    {
                        Description = desc,
                        Tag = guid,
                    };
                    return handler.Convert(type, CreateSchema(jsonSerializer, type, jsonSchema2) ?? jsonSchema2, breadcrumb);
                }
            }
            catch (NotSupportedException)
            {
            }
            catch (TypeLoadException)
            {
            }

            if (genericParamType == null) return handler.Convert(type, d, breadcrumb);
            var itemSchema = CreateSchema(genericParamType, level, null, handler);
            if (itemSchema != null)
            {
                itemSchema.Description ??= "The item of the list.";
                d.DefaultItems = itemSchema;
            }

            return handler.Convert(type, d, breadcrumb);
        }

        var jsonSchema = new JsonObjectSchemaDescription
        {
            Description = desc,
            Tag = guid,
        };
        if (level < 0 || type.IsInterface) return handler.Convert(type, jsonSchema, breadcrumb);
        try
        {
            var jsonSerializer = type.GetCustomAttributes<JsonConverterAttribute>()?.FirstOrDefault();
            if (jsonSerializer?.ConverterType != null) return handler.Convert(type, CreateSchema(jsonSerializer, type, jsonSchema) ?? jsonSchema, breadcrumb);
        }
        catch (NotSupportedException)
        {
        }
        catch (TypeLoadException)
        {
        }

        var props = type.GetProperties();
        foreach (var prop in props)
        {
            var name = GetPropertyName(prop);
            if (string.IsNullOrEmpty(name)) continue;
            desc = StringExtensions.GetDescription(prop);
            try
            {
                var jsonSerializer = prop.GetCustomAttributes<JsonConverterAttribute>()?.FirstOrDefault();
                var converter = GetSchemaCreationHandler(jsonSerializer);
                if (converter != null)
                {
                    var propJsonSchema = new JsonObjectSchemaDescription
                    {
                        Description = desc,
                        Tag = guid,
                    };
                    var propDesc = converter.Convert(prop.PropertyType, propJsonSchema, new(type, null));
                    if (propDesc != null) jsonSchema.Properties[name] = handler.Convert(prop.PropertyType, propDesc, breadcrumb);
                    continue;
                }
            }
            catch (NotSupportedException)
            {
            }
            catch (TypeLoadException)
            {
            }

            var propSchema = CreateSchema(prop.PropertyType, level, desc, handler, new(prop.PropertyType, breadcrumb, name));
            if (propSchema != null) jsonSchema.Properties[name] = propSchema;
        }

        return handler.Convert(type, jsonSchema, breadcrumb);
    }

    /// <summary>
    /// Creates a JSON schema of a type.
    /// </summary>
    /// <param name="json">The JSON object to create schema.</param>
    /// <param name="level">The maximum level to create schema for JSON object.</param>
    /// <param name="desc">The description.</param>
    /// <param name="handler">The additional handler to control the creation.</param>
    /// <param name="breadcrumb">The path breadcrumb.</param>
    /// <returns>The JSON schema description instance; or null, if not supported.</returns>
    private static JsonObjectSchemaDescription CreateSchema(JsonObjectNode json, int level, string desc = null, IJsonNodeSchemaCreationHandler<IJsonValueNode> handler = null, NodePathBreadcrumb<IJsonValueNode> breadcrumb = null)
    {
        if (json is null) return null;
        handler ??= EmptyJsonNodeSchemaCreationHandler<IJsonValueNode>.Instance;
        breadcrumb ??= new(json, null);
        var schema = new JsonObjectSchemaDescription
        {
            Description = desc,
            Tag = StringExtensions.TryCreateUri(json.Schema)
        };
        level--;
        foreach (var prop in json)
        {
            var value = prop.Value;
            var bc = new NodePathBreadcrumb<IJsonValueNode>(value, breadcrumb, prop.Key);
            switch (value.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    continue;
                case JsonValueKind.String:
                    schema.Properties[prop.Key] = handler.Convert(value, new JsonStringSchemaDescription(), bc);
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    schema.Properties[prop.Key] = handler.Convert(value, new JsonBooleanSchemaDescription(), bc);
                    break;
                case JsonValueKind.Number:
                    schema.Properties[prop.Key] = handler.Convert(value, new JsonNumberSchemaDescription(), bc);
                    break;
                case JsonValueKind.Object:
                    if (level < 0 && value is JsonObjectNode obj) schema.Properties[prop.Key] = level < 1
                        ? handler.Convert(value, new JsonObjectSchemaDescription(), bc)
                        : CreateSchema(obj, level, null, handler, bc);
                    break;
                case JsonValueKind.Array:
                    schema.Properties[prop.Key] = handler.Convert(value, new JsonArraySchemaDescription(), bc);
                    break;
            }
        }

        return handler.Convert(json, schema, breadcrumb) as JsonObjectSchemaDescription;
    }

    internal static object GetValue(BaseJsonValueNode value)
    {
        if (value == null) return null;
        switch (value.ValueKind)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.String:
                if (value is IJsonValueNode<string> s) return s.Value;
                return null;
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Number:
                if (value is JsonIntegerNode i) return i.Value;
                else if (value is JsonDoubleNode d) return d.Value;
                else if (value is JsonDecimalNode m) return m.Value;
                else if (value is IJsonNumberNode n) return n.IsInteger ? n.GetInt64() : n.GetDouble();
                return 0;
            case JsonValueKind.Object:
            case JsonValueKind.Array:
                return value;
            default:
                return value;
        }
    }

    internal static JsonNodeSchemaDescription ConvertToObjectSchema(JsonObjectNode json)
    {
        if (json == null) return null;
        var type = json.TryGetStringTrimmedValue("type") ?? string.Empty;
        return type.Trim().ToLowerInvariant() switch
        {
            "object" => new JsonObjectSchemaDescription(json),
            "string" => new JsonStringSchemaDescription(json),
            "number" => new JsonNumberSchemaDescription(json),
            "integer" => new JsonIntegerSchemaDescription(json),
            "boolean" => new JsonBooleanSchemaDescription(json),
            "array" => new JsonArraySchemaDescription(json),
            NullString => new JsonNullSchemaDescription(json),
            _ => new(json)
        };
    }

    internal static IEnumerable<JsonNodeSchemaDescription> ConvertToObjectSchema(IEnumerable<JsonObjectNode> col)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            var desc = ConvertToObjectSchema(item);
            if (desc != null) yield return desc;
        }
    }

    internal static void FillObjectSchema(List<JsonNodeSchemaDescription> list, JsonObjectNode source, string propertyName)
    {
        var col = ConvertToObjectSchema(source?.TryGetObjectListValue(propertyName, true));
        if (col != null) list.AddRange(col);
    }

    internal static void FillObjectSchema(Dictionary<string, JsonNodeSchemaDescription> dict, JsonObjectNode source, string propertyName)
    {
        var json = source?.TryGetObjectValue(propertyName);
        if (json == null || dict == null) return;
        foreach (var prop in json)
        {
            var obj = ConvertToObjectSchema(prop.Value as JsonObjectNode);
            if (obj != null) dict[prop.Key] = obj;
        }
    }

    internal static DateTime? TryGetDateTime(JsonObjectNode json, bool unixTimestamp = false)
    {
        var jsTickNode = json.TryGetValue("value");
        if (jsTickNode is IJsonValueNode<long> jsTick) return unixTimestamp ? WebFormat.ParseUnixTimestamp(jsTick.Value) : WebFormat.ParseDate(jsTick.Value);
        var year = json.TryGetInt32Value("year") ?? json.TryGetInt32Value("fullYear") ?? DateTime.Now.Year;
        var month = json.TryGetInt32Value("month") ?? 0;
        if (month < 1 || month > 12) return null;
        var day = json.TryGetInt32Value("day") ?? 0;
        if (day < 1 || day > 31) return null;
        var hour = json.TryGetInt32Value("hour") ?? 0;
        if (hour < 0 || hour > 23) return null;
        var minute = json.TryGetInt32Value("minute") ?? 0;
        if (minute < 0 || minute > 59) return null;
        var second = json.TryGetInt32Value("second") ?? 0;
        if (second < 0 || second > 59) return null;
        var millisecond = json.TryGetInt32Value("millisecond") ?? 0;
        if (millisecond < 0 || millisecond > 1000) return null;
        var kind = json.TryGetStringTrimmedValue("kind", true)?.ToLowerInvariant() ?? "utc";
        if (kind == "utc" || kind == "z" || kind == "0" || kind == "0000" || kind == "+00:00" || kind == "+0000" || kind == "universal" || kind == "false")
            return new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
        if (kind == "local" || kind == "server")
            return new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Local);
        if (kind.StartsWith("gmt")) kind = kind.Substring(3);
        if (kind.Length != 5)
        {
            if (kind.Length == 6 && kind[3] == ':') kind = kind.Replace(":", string.Empty);
            if (kind.Length != 5) return null;
        }

        if ((kind.StartsWith('+') || kind.StartsWith('-')) && int.TryParse(kind, out var offset) && offset <= 3000 && offset >= -3000)
        {
            var span = new TimeSpan(offset / 100, Math.Abs(offset % 100), 0);
            var dt = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, span);
            return dt.ToUniversalTime().DateTime;
        }

        return null;
    }

    /// <summary>
    /// Tries to get the value of the specific JSON node.
    /// </summary>
    /// <param name="node">The JSON node.</param>
    /// <param name="useUnixTimestampsFallback">true if use Unix timestamp to convert if the value is a number; otherwise, false, to use JavaScript date ticks fallback.</param>
    /// <returns>The value.</returns>
    internal static DateTime? TryGetDateTime(BaseJsonValueNode node, bool useUnixTimestampsFallback)
    {
        if (node is IJsonValueNode<string> s) return WebFormat.ParseDate(s.Value);
        if (node is IJsonValueNode<long> i) return useUnixTimestampsFallback ? WebFormat.ParseUnixTimestamp(i.Value) : WebFormat.ParseDate(i.Value);
        if (node is JsonObjectNode json) return TryGetDateTime(json, useUnixTimestampsFallback);
        return null;
    }

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    internal static bool Equals(IJsonValueNode leftValue, IJsonValueNode rightValue)
    {
        if (leftValue is null || leftValue.ValueKind == JsonValueKind.Null || leftValue.ValueKind == JsonValueKind.Undefined)
        {
            return rightValue is null || rightValue.ValueKind == JsonValueKind.Null || rightValue.ValueKind == JsonValueKind.Undefined;
        }

        if (rightValue is null || rightValue.ValueKind != leftValue.ValueKind) return false;
        return leftValue.Equals(rightValue);
    }

    internal static BaseJsonValueNode ConvertValue(IJsonValueNode value, IJsonValueNode thisInstance = null)
    {
        if (value is null || value.ValueKind == JsonValueKind.Null) return Null;
        if (value.ValueKind == JsonValueKind.Undefined) return Undefined;
        if (value is JsonObjectNode obj)
        {
            if (ReferenceEquals(obj, thisInstance)) return obj.Clone();
            return obj;
        }

        if (value is JsonArrayNode arr)
        {
            if (ReferenceEquals(arr, thisInstance)) return arr.Clone();
            return arr;
        }

        if (value is JsonStringNode || value is JsonIntegerNode || value is JsonDoubleNode || value is JsonDecimalNode) return value as BaseJsonValueNode;
        if (value.ValueKind == JsonValueKind.True) return JsonBooleanNode.True;
        if (value.ValueKind == JsonValueKind.False) return JsonBooleanNode.False;
        if (value.ValueKind == JsonValueKind.String)
        {
            if (value is IJsonValueNode<string> str) return new JsonStringNode(str.Value);
            if (value is IJsonValueNode<DateTime> date) return new JsonStringNode(date.Value);
            if (value is IJsonValueNode<Guid> guid) return new JsonStringNode(guid.Value);
        }

        if (value.ValueKind == JsonValueKind.Number)
        {
            if (value is IJsonValueNode<int> int32) return new JsonIntegerNode(int32.Value);
            if (value is IJsonValueNode<long> int64) return new JsonIntegerNode(int64.Value);
            if (value is IJsonValueNode<short> int16) return new JsonIntegerNode(int16.Value);
            if (value is IJsonValueNode<double> d) return new JsonDoubleNode(d.Value);
            if (value is IJsonValueNode<float> f) return new JsonDoubleNode(f.Value);
            if (value is IJsonValueNode<decimal> m) return new JsonDecimalNode(m.Value);
            if (value is IJsonValueNode<bool> b) return b.Value ? JsonBooleanNode.True : JsonBooleanNode.False;
            if (value is IJsonValueNode<uint> uint32) return new JsonIntegerNode(uint32.Value);
            if (value is IJsonValueNode<ulong> uint64) return new JsonDoubleNode(uint64.Value);
            if (value is IJsonValueNode<ushort> uint16) return new JsonIntegerNode(uint16.Value);
            if (value is IJsonValueNode<DateTime> date) return new JsonIntegerNode(date.Value);
            var s = value.ToString();
            if (long.TryParse(s, out var l)) return new JsonIntegerNode(l);
            if (double.TryParse(s, out var db)) return new JsonDoubleNode(db);
            if (decimal.TryParse(s, out var dm)) return new JsonDecimalNode(dm);
            return Null;
        }

        return Null;
    }

    internal static JsonObjectNode ToJson(JsonNodeSchemaDescription schema)
        => schema?.ToJson();

    internal static JsonObjectNode ToJson(Dictionary<string, JsonNodeSchemaDescription> schema)
    {
        if (schema == null) return null;
        var json = new JsonObjectNode();
        foreach (var prop in schema)
        {
            json.SetValue(prop.Key, prop.Value);
        }

        return json;
    }

    internal static double ToDouble(int? value)
        => value.HasValue ? value.Value : double.NaN;

    internal static JsonNodeSchemaDescription CreateEnumSchema(Type type, JsonNodeSchemaDescription result = null)
    {
        if (result is not JsonStringSchemaDescription desc) desc = new JsonStringSchemaDescription();
        try
        {
            desc.EnumItems.Clear();
            desc.EnumItems.AddRange(Enum.GetNames(type));
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        return desc;
    }


    internal static void RemoveJsonNodeSchemaDescriptionExtendedProperties(JsonObjectNode json, bool onlyBase)
    {
        json.SetRange(json);
        json.Id = null;
        json.Schema = null;
        json.Remove(new[] { "$ref", "description", "examples" });
        if (onlyBase) return;
        json.Remove(new[] { "type", "title", "deprecated", "readOnly", "writeOnly", "allOf", "anyOf", "oneOf", "not", "enum", "definitions" });
    }

    private static JsonNodeSchemaDescription CreateSchema(JsonConverterAttribute jsonSerializer, Type type, JsonObjectSchemaDescription jsonSchema)
    {
        var converter = GetSchemaCreationHandler(jsonSerializer);
        return converter == null ? jsonSchema : converter.Convert(type, jsonSchema, new(type, null));
    }

    private static IJsonNodeSchemaCreationHandler<Type> GetSchemaCreationHandler(JsonConverterAttribute jsonSerializer)
    {
        if (jsonSerializer?.ConverterType == null || !typeof(IJsonNodeSchemaCreationHandler<Type>).IsAssignableFrom(jsonSerializer.ConverterType)) return null;
        try
        {
            return Activator.CreateInstance(jsonSerializer.ConverterType) as IJsonNodeSchemaCreationHandler<Type>;
        }
        catch (ArgumentException)
        {
        }
        catch (AmbiguousMatchException)
        {
        }
        catch (TargetException)
        {
        }
        catch (TargetInvocationException)
        {
        }
        catch (TargetParameterCountException)
        {
        }
        catch (MemberAccessException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (ExternalException)
        {
        }

        return null;
    }
}
