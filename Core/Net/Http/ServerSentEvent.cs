using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Trivial.IO;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Net;

/// <summary>
/// The record item of Server-Sent Events response.
/// </summary>
public class ServerSentEventInfo
{
    private readonly Dictionary<string, string> dict = new();

    /// <summary>
    /// Initializes a new instance of the ServerSentEventRecord class.
    /// </summary>
    public ServerSentEventInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ServerSentEventRecord class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="eventName">The event name.</param>
    /// <param name="data">he data.</param>
    /// <param name="retry">The duration for timeout retry.</param>
    public ServerSentEventInfo(string id, string eventName, string data, TimeSpan? retry = null)
    {
        Id = id;
        OriginalEventName = eventName;
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
    public ServerSentEventInfo(string id, string eventName, JsonObjectNode data, TimeSpan? retry = null)
        : this(id, eventName, data?.ToString(), retry)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ServerSentEventRecord class.
    /// </summary>
    /// <param name="value">The dictionary value.</param>
    public ServerSentEventInfo(IDictionary<string, string> value)
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
    public ServerSentEventInfo(string data, IDictionary<string, string> value)
        : this(value)
    {
        DataString = data;
    }

    /// <summary>
    /// Initializes a new instance of the ServerSentEventRecord class.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="value">The dictionary value.</param>
    public ServerSentEventInfo(JsonObjectNode data, IDictionary<string, string> value)
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
    [JsonIgnore]
    public string EventName
    {
        get
        {
            var s = OriginalEventName;
            return string.IsNullOrWhiteSpace(s) ? "message" : s.Trim();
        }
    }

    /// <summary>
    /// Gets the original event name.
    /// </summary>
    [JsonPropertyName("event")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string OriginalEventName { get; private set; }

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
    /// Tests if the specific value is the event name.
    /// </summary>
    /// <param name="value">The event name to test.</param>
    /// <returns>true if they are the same event name; otherwise, false.</returns>
    public bool IsEventName(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) value = "message";
        else value = value.Trim().ToLowerInvariant();
        return value == EventName;
    }

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
                value = OriginalEventName;
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
    /// Writes the event information into a stream.
    /// </summary>
    /// <param name="stream">The stream to write.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <exception cref="ArgumentNullException">The stream should not be null.</exception>
    /// <exception cref="NotSupportedException">The stream does not support writing.</exception>
    /// <exception cref="IOException">An I/O error occured, such as the specified file cannot be found.</exception>
    /// <exception cref="ObjectDisposedException">The stream was closed.</exception>
    public void WriteTo(Stream stream, Encoding encoding = null)
    {
        if (stream == null) throw ObjectConvert.ArgumentNull(nameof(stream));
        if (!stream.CanWrite) throw new NotSupportedException("The stream does not support writing.");
        var s = ToResponseString(true);
        encoding ??= Encoding.UTF8;
        try
        {
            if (stream.Position > 0)
            {
                var prefix = encoding.GetBytes("\n");
                stream.Write(prefix, 0, prefix.Length);
            }
        }
        catch (EncoderFallbackException)
        {
        }
        catch (IOException)
        {
        }

        var bytes = encoding.GetBytes(s);
        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Writes the event information into a stream.
    /// </summary>
    /// <param name="writer">The stream writer.</param>
    /// <exception cref="ArgumentNullException">The stream writer should not be null.</exception>
    /// <exception cref="NotSupportedException">Cannot write the information to the stream writer.</exception>
    /// <exception cref="IOException">An I/O error occured.</exception>
    /// <exception cref="ObjectDisposedException">The stream writer was closed.</exception>
    public void WriteTo(StreamWriter writer)
    {
        if (writer == null) throw ObjectConvert.ArgumentNull(nameof(writer));
        writer.Write(ToResponseString(true));
    }

    /// <summary>
    /// Gets a JSON object parsed from data string.
    /// </summary>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object of data.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public JsonObjectNode GetJsonData(JsonDocumentOptions options = default)
        => JsonObjectNode.Parse(DataString, options);

    /// <summary>
    /// Tries to get a JSON object parsed from data string.
    /// </summary>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object of data.</returns>
    public JsonObjectNode TryGetJsonData(JsonDocumentOptions options = default)
        => JsonObjectNode.TryParse(DataString, options);

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
        AppendProperty(sb, string.Empty, Comment, newLineN);
        AppendProperty(sb, "id", Id, newLineN);
        AppendProperty(sb, "event", OriginalEventName, newLineN);
        AppendProperty(sb, "data", DataString, newLineN);
        if (RetryDuration.HasValue)
        {
            var ms = RetryDuration.Value.TotalMilliseconds;
            if (!double.IsNaN(ms) && ms > 0 && !double.IsInfinity(ms)) AppendProperty(sb, "retry", ms.ToString("g"), newLineN);
        }

        foreach (var prop in dict)
        {
            if (string.IsNullOrWhiteSpace(prop.Key)) continue;
            switch (prop.Key.Trim().TrimEnd(':').ToLowerInvariant())
            {
                case "id":
                case "event":
                case "data":
                case "retry":
                    break;
                default:
                    AppendProperty(sb, prop.Key, prop.Value, newLineN);
                    break;
            }
        }
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
                OriginalEventName = value;
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
            sb.Append(line);
            sb.Append(newLineN ? "\n" : Environment.NewLine);
        }
    }

    /// <summary>
    /// Converts to JSON document.
    /// </summary>
    /// <param name="info">The Server-Sent Event record item.</param>
    /// <returns>An instance of the JSON object node.</returns>
    public static explicit operator JsonObjectNode(ServerSentEventInfo info)
    {
        if (info == null) return null;
        var json = new JsonObjectNode
        {
            { "id", info.Id },
            { "event", info.OriginalEventName },
            { "data", info.DataString },
            { "retry", info.RetryDurationInMillisecond }
        };
        if (!string.IsNullOrWhiteSpace(info.DataString) && info.DataString.StartsWith('{') && info.DataString.EndsWith('}'))
        {
            var obj = JsonObjectNode.TryParse(info.DataString);
            if (obj != null) json.SetValue("data", obj);
        }

        json.CommentValue = info.Comment;
        json.SetRange(info.dict, true);
        return json;
    }

    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>A collection of server-sent event record.</returns>
    /// <exception cref="NotSupportedException">Cannot read the information to the stream.</exception>
    /// <exception cref="IOException">An I/O error occured.</exception>
    /// <exception cref="ObjectDisposedException">The stream was closed.</exception>
    public static IEnumerable<ServerSentEventInfo> Parse(Stream stream, Encoding encoding = null)
    {
        var lines = stream.ReadLines(encoding ?? Encoding.UTF8);
        return Parse(lines);
    }

    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="reader">The input stream.</param>
    /// <returns>A collection of server-sent event record.</returns>
    /// <exception cref="NotSupportedException">Cannot write the information to the stream writer.</exception>
    /// <exception cref="IOException">An I/O error occured.</exception>
    /// <exception cref="ObjectDisposedException">The stream writer was closed.</exception>
    public static IEnumerable<ServerSentEventInfo> Parse(StreamReader reader)
    {
        var lines = reader.ReadLines();
        return Parse(lines);
    }

    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>A collection of server-sent event record.</returns>
    public static IEnumerable<ServerSentEventInfo> Parse(string s)
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
    public static IEnumerable<ServerSentEventInfo> Parse(IEnumerable<string> lines)
    {
        if (lines == null) yield break;
        ServerSentEventInfo record = null;
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
    /// <exception cref="NotSupportedException">Cannot read the information to the stream.</exception>
    /// <exception cref="IOException">An I/O error occured.</exception>
    /// <exception cref="ObjectDisposedException">The response was closed.</exception>
    public static async Task<IEnumerable<ServerSentEventInfo>> ParseAsync(HttpResponseMessage response)
    {
        var content = response?.Content;
        var resp = content == null ? null : await content.ReadAsStreamAsync();
        return Parse(resp);
    }

    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="http">The HTTP client.</param>
    /// <returns>A collection of server-sent event record.</returns>
    /// <exception cref="InvalidOperationException">The request message was already sent by the HTTP client instance.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="IOException">An I/O error occured.</exception>
    public static async Task<IEnumerable<ServerSentEventInfo>> ParseAsync(HttpRequestMessage request, HttpClient http = null)
    {
        var resp = await (http ?? new HttpClient()).SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        return await ParseAsync(resp);
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Parses server-sent event record.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A collection of server-sent event record.</returns>
    public static async Task<IEnumerable<ServerSentEventInfo>> ParseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
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
    public static IEnumerable<ServerSentEventInfo> Parse(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        var resp = response?.Content?.ReadAsStream(cancellationToken);
        return Parse(resp);
    }
#endif
}
