using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Web;
using Trivial.Text;

namespace Trivial.Net;

/// <summary>
/// The HTTP URI struct.
/// </summary>
[JsonConverter(typeof(HttpUriConverter))]
public class HttpUri : IEquatable<HttpUri>, IEquatable<AppDeepLinkUri>, IEquatable<Uri>, IEquatable<string>
{
#pragma warning disable IDE0057
    private string host;
    private string path;
    private List<string> folders = new();

    /// <summary>
    /// Gets or sets a value indicating whether it is on secure layer.
    /// </summary>
    public bool IsSecure { get; set; } = true;

    /// <summary>
    /// Gets or sets the account information.
    /// </summary>
    public string AccountInfo { get; set; }

    /// <summary>
    /// Gets or sets the host.
    /// </summary>
    public string Host
    {
        get
        {
            return host;
        }

        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                host = null;
                return;
            }

            value = value.Trim();
            #pragma warning disable CA2249
            if (value.IndexOf("://") >= 0 || value.IndexOf("//") >= 0)
                value = value.Substring(value.IndexOf("//") + 2);
            #pragma warning restore CA2249
            var atPos = value.IndexOf("@");
            if (atPos > 0)
                AccountInfo = value.Substring(0, atPos);
            value = value.Substring(atPos + 1);
            var i = value.Length;
            var last = new[] { "/", "\\", ":", "?", "#" };
            foreach (var item in last)
            {
                var j = value.IndexOf(item);
                if (j >= 0 && j < i) i = j;
            }

            host = value.Substring(0, i);
        }
    }

    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// Gets or sets the host.
    /// </summary>
    public string Path
    {
        get
        {
            return path;
        }

        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                path = null;
                folders.Clear();
                return;
            }

            value = value.Trim();
            var i = value.Length;
            var last = new[] { "?", "#" };
            foreach (var item in last)
            {
                var j = value.IndexOf(item);
                if (j >= 0 && j < i) i = j;
            }

            value = value.Substring(0, i);
            var start = value.IndexOf(":");
            if (start >= 0)
            {
                start = value.IndexOf("/", start);
                value = value.Substring(start);
            }

            path = value;
            folders = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }

    /// <summary>
    /// Gets the path in collection.
    /// </summary>
    public IReadOnlyList<string> PathItems => folders.AsReadOnly();

    /// <summary>
    /// Gets the query.
    /// </summary>
    public QueryData Query { get; } = new QueryData();

    /// <summary>
    /// Gets or sets the hash.
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The instance to compare.</param>
    /// <returns>true if they are equal; otherwise, false.</returns>
    public bool Equals(HttpUri other)
    {
        return other != null && other.ToString() == ToString();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The instance to compare.</param>
    /// <returns>true if they are equal; otherwise, false.</returns>
    public bool Equals(AppDeepLinkUri other)
        => other != null && other.ToString() == ToString();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The instance to compare.</param>
    /// <returns>true if they are equal; otherwise, false.</returns>
    public bool Equals(Uri other)
        => other != null && other.ToString() == ToString();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The instance to compare.</param>
    /// <returns>true if they are equal; otherwise, false.</returns>
    public bool Equals(string other)
        => other != null && other == ToString();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The instance to compare.</param>
    /// <returns>true if they are equal; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other is HttpUri h) return Equals(h);
        if (other is AppDeepLinkUri d) return Equals(d);
        if (other is Uri u) return Equals(u);
        if (other is string s) return Equals(s);
        return false;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>The URI string.</returns>
    public override string ToString()
    {
        var str = new StringBuilder();
        str.Append(IsSecure ? "https" : "http");
        str.Append("://");
        if (!string.IsNullOrWhiteSpace(AccountInfo))
        {
            str.Append(AccountInfo);
            str.Append('@');
        }

        str.Append(Host ?? "localhost");
        if (Port.HasValue && Port != (IsSecure ? 443 : 80) && Port > 0) str.AppendFormat(":{0}", Port);
        if (!string.IsNullOrWhiteSpace(Path))
        {
            if (Path[0] != '/') str.Append('/');
            str.Append(Path);
        }
        else
        {
            str.Append('/');
        }

        if (Query.Count > 0)
        {
            str.Append('?');
            str.Append(Query.ToString());
        }

        if (!string.IsNullOrWhiteSpace(Hash))
        {
            if (Hash[0] != '#') str.Append('#');
            str.Append(Hash);
        }

        return str.ToString();
    }

    /// <summary>
    /// Returns a URI object.
    /// </summary>
    /// <returns>The URI object.</returns>
    public Uri ToUri()
        => new(ToString());

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
        => ToString().GetHashCode();

    /// <summary>
    /// Parses a URL to the HTTP URI.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>An HTTP URI instance.</returns>
    public static HttpUri Parse(string url)
    {
        var hostPos = url.IndexOf("://");
        var uri = new HttpUri();
        if (hostPos < 0)
        {
            if (url.IndexOf("//") == 0) hostPos = 2;
        }
        else
        {
            hostPos += 3;
            if (url.IndexOf("https") < 0 && url.IndexOf("ftps") < 0 && url.IndexOf("sftp") < 0)
                uri.IsSecure = false;
        }

        uri.Host = url;
        var pathPos = url.IndexOf("/", hostPos > 0 ? hostPos : 0);
        var portPos = url.LastIndexOf(":");
        if (portPos > 0 && portPos > hostPos)
        {
            portPos++;
            var portStr = url.Substring(portPos, (pathPos > 0 ? pathPos : url.Length) - portPos);
            if (!string.IsNullOrWhiteSpace(portStr) && int.TryParse(portStr, out int port) && port >= 0) uri.Port = port;
        }

        if (pathPos >= 0) uri.Path = url.Substring(pathPos);
        var queryPos = url.IndexOf("?");
        var hashPos = url.IndexOf("#");
        if (hashPos >= 0)
        {
            uri.Hash = url.Substring(hashPos);
            url = url.Substring(0, hashPos);
        }

        if (queryPos >= 0) uri.Query.ParseSet(url.Substring(queryPos + 1));
        return uri;
    }

    /// <summary>
    /// Converts to a URI object.
    /// </summary>
    /// <param name="uri">The instance to convert.</param>
    public static explicit operator Uri(HttpUri uri)
        => uri?.ToUri();

    /// <summary>
    /// Converts to a URI object.
    /// </summary>
    /// <param name="uri">The instance to convert.</param>
    public static implicit operator HttpUri(Uri uri)
        => uri != null ? Parse(uri.ToString()) : null;
#pragma warning restore IDE0057
}

/// <summary>
/// The JSON converter for enum. The output will be an integer value.
/// </summary>
internal class HttpUriConverter : JsonConverter<HttpUri>
{
    /// <inheritdoc />
    public override HttpUri Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                try
                {
                    return HttpUri.Parse(reader.GetString());
                }
                catch (FormatException ex)
                {
                    throw new JsonException("Invalid HTTPS URI format.", ex);
                }

            case JsonTokenType.StartObject:
                var json = JsonObjectNode.ParseValue(ref reader);
                var v = new HttpUri
                {
                    IsSecure = json.TryGetBooleanValue("secure") ?? true,
                    AccountInfo = json.TryGetStringTrimmedValue("user", true),
                    Host = json.TryGetStringTrimmedValue("host", true),
                    Port = json.TryGetInt32Value("port"),
                    Path = json.TryGetStringTrimmedValue("path", true),
                    Hash = json.TryGetStringTrimmedValue("hash", true),
                };
                v.Query.ParseSet(json.TryGetStringTrimmedValue("query", true) ?? json.TryGetStringTrimmedValue("q", true), true);
                return v;
        }

        throw new JsonException($"Expects a string in HTTPS URI format but {reader.TokenType}.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HttpUri value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteStringValue(value.ToString());
    }
}
