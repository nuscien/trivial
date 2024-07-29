using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Trivial.IO;
using Trivial.Text;

namespace Trivial.Net;

/// <summary>
/// The record item of Server-Sent Events response.
/// </summary>
public class ServerSentEventRecord
{
    private readonly Dictionary<string, string> dict = new();

    /// <summary>
    /// Initializes a new instance of the ServerSentEventRecord class.
    /// </summary>
    public ServerSentEventRecord()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ServerSentEventRecord class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="eventName">The event name.</param>
    /// <param name="data">he data.</param>
    /// <param name="retry">The duration for timeout retry.</param>
    public ServerSentEventRecord(string id, string eventName, string data, TimeSpan? retry = null)
    {
        Id = id;
        EventName = eventName;
        DataString = data;
        RetryDuration = retry;
    }

    /// <summary>
    /// Initializes a new instance of the ServerSentEventRecord class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="eventName">The event name.</param>
    /// <param name="data">he data.</param>
    /// <param name="retry">The duration for timeout retry.</param>
    public ServerSentEventRecord(string id, string eventName, JsonObjectNode data, TimeSpan? retry = null)
        : this(id, eventName, data?.ToString(), retry)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ServerSentEventRecord class.
    /// </summary>
    /// <param name="value">The dictionary value.</param>
    public ServerSentEventRecord(IDictionary<string, string> value)
    {
        if (value == null) return;
        foreach (var kvp in value)
        {
            SetValue(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the ServerSentEventRecord class.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="value">The dictionary value.</param>
    public ServerSentEventRecord(string data, IDictionary<string, string> value)
        : this(value)
    {
        DataString = data;
    }

    /// <summary>
    /// Initializes a new instance of the ServerSentEventRecord class.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="value">The dictionary value.</param>
    public ServerSentEventRecord(JsonObjectNode data, IDictionary<string, string> value)
        : this(data?.ToString(), value)
    {
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Id { get; private set; }

    /// <summary>
    /// Gets the event name.
    /// </summary>
    [JsonPropertyName("event")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string EventName { get; private set; }

    /// <summary>
    /// Gets the data.
    /// </summary>
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string DataString { get; private set; }

    /// <summary>
    /// Gets the duration for timeout retry.
    /// </summary>
    [JsonPropertyName("retry")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TimeSpan? RetryDuration { get; private set; }

    /// <summary>
    /// Gets the milliseconds of the duration for timeout retry.
    /// </summary>
    [JsonIgnore]
    public long? RetryDurationInMillisecond
    {
        get
        {
            var duration = RetryDuration;
            if (!duration.HasValue) return null;
            var seconds = duration.Value.TotalSeconds;
            if (double.IsNaN(seconds) || double.IsInfinity(seconds)) return null;
            try
            {
                return (long)seconds;
            }
            catch (InvalidCastException)
            {
            }
            catch (ArgumentException)
            {
            }

            return null;
        }
    }

    /// <summary>
    /// Gets or sets the comment.
    /// </summary>
    [JsonIgnore]
    public string Comment { get; set; }

    /// <summary>
    /// Tries to get value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>true if contains such property; otherwise, false.</returns>
    public bool TryGetValue(string key, out string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            value = Comment;
            return !string.IsNullOrEmpty(value);
        }

        switch (key.Trim().ToLowerInvariant())
        {
            case "id":
                value = Id;
                break;
            case "event":
                value = EventName;
                break;
            case "data":
                value = DataString;
                break;
            case "retry":
                value = RetryDurationInMillisecond?.ToString("g");
                break;
            default:
                return dict.TryGetValue(key, out value);
        }

        return !string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Converts the record to string.
    /// </summary>
    /// <returns>The string in SSE format.</returns>
    public override string ToString()
        => ToResponseString(false);

    /// <summary>
    /// Converts the record to string.
    /// </summary>
    /// <param name="newLineN">true if use \n instead of new line.</param>
    /// <returns>The string in SSE format.</returns>
    public string ToResponseString(bool newLineN)
    {
        var sb = new StringBuilder();
        ToResponseString(sb, newLineN);
        return sb.ToString();
    }

    /// <summary>
    /// Gets a JSON object parsed from data string.
    /// </summary>
    /// <returns>A JSON object of data.</returns>
    public JsonObjectNode GetJsonData()
        => JsonObjectNode.Parse(DataString);

    /// <summary>
    /// Converts the record to string.
    /// </summary>
    /// <param name="sb">The string builder to append information.</param>
    /// <param name="newLineN">true if use \n instead of new line.</param>
    /// <returns>The string in SSE format.</returns>
    internal void ToResponseString(StringBuilder sb, bool newLineN)
    {
        if (sb == null) return;
        if (sb.Length > 1) sb.Append(newLineN ? "\n" : Environment.NewLine);
        AppendProperty(sb, "id", Id, newLineN);
        AppendProperty(sb, "event", EventName, newLineN);
        AppendProperty(sb, "data", DataString, newLineN);
        if (RetryDuration.HasValue)
        {
            var ms = RetryDuration.Value.TotalMilliseconds;
            if (!double.IsNaN(ms) && ms > 0 && !double.IsInfinity(ms)) AppendProperty(sb, "retry", ms.ToString("g"), newLineN);
        }

        AppendProperty(sb, string.Empty, Comment, newLineN);
    }

    internal void SetValue(string key, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        key = key?.Trim()?.ToLowerInvariant() ?? string.Empty;
        switch (key)
        {
            case "":
                Comment = value;
                break;
            case "id":
                Id = value;
                break;
            case "event":
                EventName = value;
                break;
            case "data":
                DataString = value;
                break;
            case "retry":
                if (long.TryParse(value, out var ms) && ms >= -1) RetryDuration = TimeSpan.FromMilliseconds(ms);
                else RetryDuration = null;
                break;
            default:
                dict[key.ToLowerInvariant()] = value;
                break;
        }
    }

    private void AppendProperty(StringBuilder sb, string key, string value, bool newLineN)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        if (key.Contains('\n') || key.Contains('\r')) return;
        key = key.Trim();
        foreach (var line in lines)
        {
            sb.Append(key);
            sb.Append(": ");
            sb.Append(value);
            sb.Append(newLineN ? "\n" : Environment.NewLine);
        }
    }

    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <returns>A collection of server-sent event record.</returns>
    public static IEnumerable<ServerSentEventRecord> Parse(Stream stream)
    {
        var lines = stream.ReadLines(Encoding.UTF8);
        return Parse(lines);
    }

    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>A collection of server-sent event record.</returns>
    public static IEnumerable<ServerSentEventRecord> Parse(string s)
    {
        if (string.IsNullOrEmpty(s)) Parse(null as IEnumerable<string>);
        var newLineStr = s.Contains('\r') ? Environment.NewLine : "\n";
        var lines = s.Split(new[] { newLineStr }, StringSplitOptions.None);
        return Parse(lines);
    }

    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="lines">The input lines.</param>
    /// <returns>A collection of server-sent event record.</returns>
    public static IEnumerable<ServerSentEventRecord> Parse(IEnumerable<string> lines)
    {
        if (lines == null) yield break;
        ServerSentEventRecord record = null;
        string key = null;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                key = null;
                if (record != null)
                {
                    yield return record;
                    record = null;
                }

                continue;
            }

            var keyIndex = line.IndexOf(':');
            if (keyIndex < 0)
            {
                if (key == "data")
                {
                    record ??= new();
                    record.SetValue(key, string.IsNullOrEmpty(record.DataString) ? line : string.Concat(record.DataString, Environment.NewLine, line));
                }
                else if (key == string.Empty)
                {
                    record ??= new();
                    record.SetValue(key, string.IsNullOrEmpty(record.Comment) ? line : string.Concat(record.Comment, Environment.NewLine, line));
                }
                else
                {
                    key = null;
                }

                continue;
            }

            if (keyIndex + 1 == line.Length) continue;
            record ??= new();
            var l = line.Substring(keyIndex + 1);
            if (l.Length > 1 && l.FirstOrDefault() == ' ') l = l.Substring(1);
            if (keyIndex == 0)
            {
                key = string.Empty;
                record.SetValue(key, string.IsNullOrEmpty(record.Comment) ? l : string.Concat(record.Comment, Environment.NewLine, l));
                continue;
            }

            var currentKey = line.Substring(0, keyIndex).Trim().ToLowerInvariant();
            if (currentKey == key && record.TryGetValue(key, out var l2)) l = string.Concat(l2, Environment.NewLine, l);
            key = currentKey;
            record.SetValue(key, l);
        }

        if (record != null) yield return record;
    }

    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>A collection of server-sent event record.</returns>
    public static async Task<IEnumerable<ServerSentEventRecord>> ParseAsync(HttpResponseMessage response)
    {
        var content = response?.Content;
        var resp = content == null ? null : await content.ReadAsStreamAsync();
        return Parse(resp);
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A collection of server-sent event record.</returns>
    public static async Task<IEnumerable<ServerSentEventRecord>> ParseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = response?.Content;
        var resp = content == null ? null : await content.ReadAsStreamAsync(cancellationToken);
        return Parse(resp);
    }

    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A collection of server-sent event record.</returns>
    public static IEnumerable<ServerSentEventRecord> Parse(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        var resp = response?.Content?.ReadAsStream(cancellationToken);
        return Parse(resp);
    }
#endif
}
