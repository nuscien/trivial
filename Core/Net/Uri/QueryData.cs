﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using Trivial.Collection;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Net;

/// <summary>
/// The query data in URI after question mark.
/// </summary>
[JsonConverter(typeof(QueryDataConverter))]
[Guid("E17633F1-1C54-484C-9751-C049368AE6FD")]
public class QueryData : StringKeyValuePairs
{
#pragma warning disable IDE0056, IDE0057, CA1834
    private const string NumBoolChars = "-0123456789.tfnuTFNU";
    private const string NameChars = "_$-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Initializes a new instance of the QueryData class.
    /// </summary>
    public QueryData() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the QueryData class.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    public QueryData(IEnumerable<KeyValuePair<string, string>> collection) : base(collection)
    {
    }

    /// <summary>
    /// Initializes a new instance of the QueryData class.
    /// </summary>
    /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
    protected QueryData(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    /// <summary>
    /// Gets or sets the default encoding.
    /// </summary>
    public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Parses a string to the query data.
    /// </summary>
    /// <param name="query">The query string.</param>
    /// <param name="append">true if append instead of override; otherwise, false.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>The count of query item added.</returns>
    /// <exception cref="FormatException">The format is not correct.</exception>
    public int ParseSet(string query, bool append = false, Encoding encoding = null)
    {
        if (!append) Clear();
        if (string.IsNullOrWhiteSpace(query)) return 0;
        var queryTrim = query.TrimStart();
        if (queryTrim.IndexOf('{') == 0)
        {
            queryTrim = queryTrim.Substring(1).Trim();
            var lastPos = queryTrim.LastIndexOf('}');
            if (lastPos >= 0) queryTrim = queryTrim.Substring(0, lastPos);
            var count = 0;

            string name = null;
            StringBuilder sb = null;
            var level = new List<char>();
            StringBuilder backSlash = null;
            bool ignoreRest = false;
            bool lastBackSlash = false;
            foreach (var c in queryTrim)
            {
                if (ignoreRest)
                {
                    switch (c)
                    {
                        case '\r':
                        case '\n':
                            ignoreRest = false;
                            break;
                    }

                    lastBackSlash = false;
                    continue;
                }

                if (c == '\\')
                {
                    if (lastBackSlash)
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        backSlash = new StringBuilder();
                    }

                    lastBackSlash = !lastBackSlash;
                    continue;
                }

                lastBackSlash = false;
                if (backSlash != null)
                {
                    if (StringExtensions.ReplaceBackSlash(sb, backSlash, c)) backSlash = null;
                    continue;
                }

                if (sb == null)
                {
                    if (c == '/')
                    {
                        ignoreRest = true;
                        continue;
                    }

                    if (c == '"')
                    {
                        sb = new StringBuilder();
                        level.Add('"');
                        continue;
                    }

                    if (name == null)
                    {
                        if (NameChars.IndexOf(c) > -1)
                        {
                            sb = new StringBuilder();
                            level.Add(':');
                            sb.Append(c);
                        }

                        continue;
                    }

                    if (c == '{')
                    {
                        level.Add('}');
                    }
                    else if (c == '[')
                    {
                        level.Add(']');
                    }
                    else if (NumBoolChars.IndexOf(c) > -1)
                    {
                        level.Add(',');
                    }
                    else
                    {
                        continue;
                    }

                    sb = new StringBuilder();
                    sb.Append(c);
                    continue;
                }

                if (level.Count == 0) continue;
                var lastLevelChar = level[level.Count - 1];
                if (c == lastLevelChar)
                {
                    level.RemoveAt(level.Count - 1);
                    if (name == null)
                    {
                        name = sb.ToString();
                        sb = null;
                        continue;
                    }
                    else if (level.Count == 0)
                    {
                        if (c == ',')
                        {
                            var sbStr = sb.ToString().Trim();
                            sb = new StringBuilder(sbStr);
                            if (sbStr == "null" || sbStr == "undefined")
                            {
                                name = null;
                                sb = null;
                                continue;
                            }
                        }
                        else if (c == '"')
                        {
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        ListExtensions.Add(this, name, sb.ToString());
                        name = null;
                        sb = null;
                        continue;
                    }
                }
                else if (lastLevelChar == ':' && name == null)
                {
                    if (c == '\r' || c == '\n' || c == '\t' || c == ' ' || c == ',' || c == '+')
                    {
                        sb = null;
                    }
                    else if (c == '/')
                    {
                        sb = null;
                        ignoreRest = true;
                    }
                    else if (NameChars.IndexOf(c) < 0)
                    {
                        throw new FormatException("The format of query string is not correct.");
                    }

                    if (sb == null)
                    {
                        level.RemoveAt(level.Count - 1);
                        continue;
                    }
                }
                else if (c == '"')
                {
                    level.Add('"');
                }
                else if (lastLevelChar != '"')
                {
                    if (c == '{')
                    {
                        level.Add('}');
                    }
                    else if (c == '[')
                    {
                        level.Add(']');
                    }
                }

                if (sb == null) sb = new StringBuilder();
                sb.Append(c);
            }

            if (!string.IsNullOrWhiteSpace(name) && sb != null)
            {
                var sbStr = sb.ToString().Trim();
                if (sbStr != "null" && sbStr != "undefined") ListExtensions.Add(this, name, sbStr);
            }

            return count;
        }

        if (encoding == null) encoding = DefaultEncoding ?? Encoding.UTF8;
        var pos = query.IndexOf("?");
        if (pos >= 0) query = query.Substring(pos + 1);
        pos = query.IndexOf("#");
        if (pos >= 0) query = query.Substring(0, pos);
        var arr = query.Split('&', '\r', '\n');
        foreach (var item in arr)
        {
            pos = item.IndexOf("=");
            if (pos < 0)
            {
                if (item.Length == 0) continue;
                ListExtensions.Add(this, WebFormat.UrlDecode(item, encoding), string.Empty);
            }
            else
            {
                var key = item.Substring(0, pos);
                var value = item.Substring(pos + 1);
                if (key.Length == 0 && value.Length == 0) continue;
                ListExtensions.Add(this, WebFormat.UrlDecode(key, encoding), WebFormat.UrlDecode(value, encoding));
            }
        }

        return arr.Length;
    }

    /// <summary>
    /// Parses a string to the query data.
    /// </summary>
    /// <param name="query">The query string.</param>
    /// <param name="append">true if append instead of override; otherwise, false.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>The count of query item added.</returns>
    /// <exception cref="FormatException">The format is not correct.</exception>
    public int ParseSet(ReadOnlySpan<char> query, bool append = false, Encoding encoding = null)
        => ParseSet(query.ToString(), append, encoding);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A query string.</returns>
    public override string ToString()
        => ToString(DefaultEncoding);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <param name="encoding">The encoding.</param>
    /// <returns>A query string.</returns>
    public string ToString(Encoding encoding)
    {
        if (encoding == null) encoding = DefaultEncoding ?? Encoding.UTF8;
        var arr = new List<string>();
        foreach (var item in this)
        {
            arr.Add(string.Format("{0}={1}", WebFormat.UrlEncode(item.Key, encoding), WebFormat.UrlEncode(item.Value, encoding)));
        }

        return string.Join("&", arr);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>A query string.</returns>
    public string ToString(string url, Encoding encoding = null)
    {
        var qIndex = url.IndexOf("?");
        var str = new StringBuilder(url);
        if (qIndex >= 0)
        {
            if (qIndex < url.Length - 1) str.Append("&");
        }
        else
        {
            str.Append("?");
        }

        str.Append(ToString(encoding));
        return str.ToString();
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>A query string.</returns>
    public string ToString(Uri uri, Encoding encoding = null)
        => ToString(uri?.OriginalString, encoding);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>A query string.</returns>
    public string ToString(StringBuilder url, Encoding encoding = null)
        => ToString(url?.ToString(), encoding);

    /// <summary>
    /// Returns a string HTTP request content.
    /// </summary>
    /// <param name="encoding">The optional encoding to use for the content. Or null for default.</param>
    /// <param name="mediaType">The optional media type to use for the content. Or null for default.</param>
    /// <returns>A string HTTP request content.</returns>
    public StringContent ToStringContent(Encoding encoding = null, string mediaType = null)
        => new(ToString(), encoding ?? DefaultEncoding ?? Encoding.UTF8, mediaType ?? "application/x-www-form-urlencoded");

    /// <summary>
    /// Parses a string to the query data.
    /// </summary>
    /// <param name="query">The query string.</param>
    /// <returns>A query data instance.</returns>
    /// <exception cref="FormatException">The format is not correct.</exception>
    public static QueryData Parse(string query)
    {
        var q = new QueryData();
        q.ParseSet(query);
        return q;
    }

    /// <summary>
    /// Parses a string to the query data.
    /// </summary>
    /// <param name="query">The query string.</param>
    /// <returns>A query data instance.</returns>
    /// <exception cref="FormatException">The format is not correct.</exception>
    public static QueryData Parse(ReadOnlySpan<char> query)
    {
        var q = new QueryData();
        q.ParseSet(query);
        return q;
    }

    /// <summary>
    /// Parses a string to the query data.
    /// </summary>
    /// <param name="stream">Thestream of the query string.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>A query data instance.</returns>
    /// <exception cref="FormatException">The format is not correct.</exception>
    public static QueryData Parse(Stream stream, Encoding encoding = null)
    {
        if (stream == null) return null;
        encoding ??= Encoding.UTF8;
        using var reader = new StreamReader(stream, encoding);
        var query = reader.ReadToEnd();
        var q = new QueryData();
        q.ParseSet(query, false, encoding);
        return q;
    }

    /// <summary>
    /// Parses a string to the query data.
    /// </summary>
    /// <param name="stream">Thestream of the query string.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>A query data instance.</returns>
    /// <exception cref="FormatException">The format is not correct.</exception>
    public static async Task<QueryData> ParseAsync(Stream stream, Encoding encoding = null)
    {
        if (stream == null) return null;
        encoding ??= Encoding.UTF8;
        using var reader = new StreamReader(stream, encoding);
        var query = await reader.ReadToEndAsync();
        var q = new QueryData();
        q.ParseSet(query, false, encoding);
        return q;
    }
#pragma warning restore IDE0056, IDE0057, CA1834
}

/// <summary>
/// The JSON converter for query data.
/// </summary>
internal class QueryDataConverter : JsonConverter<QueryData>
{
    /// <inheritdoc />
    public override QueryData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
            case JsonTokenType.False:
                return null;
            case JsonTokenType.True:
                return new();
            case JsonTokenType.String:
                return QueryData.Parse(reader.GetString());
            case JsonTokenType.StartObject:
                {
                    var json = JsonObjectNode.ParseValue(ref reader);
                    if (json is null) return null;
                    var q = new QueryData();
                    foreach (var prop in json)
                    {
                        if (string.IsNullOrWhiteSpace(prop.Key)) continue;
                        q.Add(prop.Key, prop.Value);
                    }

                    return q;
                }
            default:
                throw new JsonException($"The JSON value should be a string.");
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, QueryData value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteStringValue(value.ToString());
    }
}
