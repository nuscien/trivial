using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

using Trivial.Collection;

namespace Trivial.Net
{
    /// <summary>
    /// The HTTP URI struct.
    /// </summary>
    public class HttpUri : IEquatable<HttpUri>, IEquatable<Uri>, IEquatable<string>
    {
        private string host;
        private string path;

        /// <summary>
        /// Gets or sets a value indicating whether it is on secure layer.
        /// </summary>
        public bool IsSecure { get; set; } = true;

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
                if (value.IndexOf("://") >= 0 || value.IndexOf("//") >= 0) value = value.Substring(value.IndexOf("//") + 2);
                var i = value.Length;
                var last = new[] { "/", "\\", ":", "?", "#" };
                foreach (var item in last)
                {
                    var j = value.IndexOf(item);
                    if (j >= 0 && j < i) i = j;
                }

                var atPos = value.IndexOf("@") + 1;
                if (atPos > i) atPos = 0;
                host = value.Substring(atPos, i);
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public uint? Port { get; set; }

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
            }
        }

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
        public bool Equals(Uri other)
        {
            return other != null && other.ToString() == ToString();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The instance to compare.</param>
        /// <returns>true if they are equal; otherwise, false.</returns>
        public bool Equals(string other)
        {
            return other != null && other == ToString();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The URI string.</returns>
        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendFormat("{0}://{1}", IsSecure ? "https" : "http", Host);
            if (Port.HasValue && Port != (IsSecure ? 443 : 80)) str.AppendFormat(":{0}", Port);
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
        {
            return new Uri(ToString());
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

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
                if (url.IndexOf("https") < 0) uri.IsSecure = false;
            }

            uri.Host = url;
            var pathPos = url.IndexOf("/", hostPos > 0 ? hostPos : 0);
            var portPos = url.LastIndexOf(":");
            if (portPos > 0 && portPos > hostPos)
            {
                portPos++;
                var portStr = url.Substring(portPos, (pathPos > 0 ? pathPos : url.Length) - portPos);
                if (!string.IsNullOrWhiteSpace(portStr) && int.TryParse(portStr, out int port) && port >= 0) uri.Port = (uint)port;
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
        {
            return uri?.ToUri();
        }

        /// <summary>
        /// Converts to a URI object.
        /// </summary>
        /// <param name="uri">The instance to convert.</param>
        public static implicit operator HttpUri(Uri uri)
        {
            return uri != null ? Parse(uri.ToString()) : null;
        }
    }

    /// <summary>
    /// The query data in URI after question mark.
    /// </summary>
    public class QueryData : StringKeyValuePairs
    {
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
                foreach (var c in queryTrim)
                {
                    if (c == '\\')
                    {
                        backSlash = new StringBuilder();
                        continue;
                    }

                    if (backSlash != null)
                    {
                        if (backSlash.Length == 0)
                        {
                            if (c != 'x' && c != 'u')
                            {
                                sb.Append(c);
                                backSlash = null;
                            }

                            backSlash.Append(c);
                            continue;
                        }

                        var len = 0;
                        if (backSlash[0] == 'x')
                        {
                            len = 3;
                        }
                        else if (backSlash[0] == 'u')
                        {
                            len = 5;
                        }
                        else
                        {
                            backSlash = null;
                            continue;
                        }

                        if (backSlash.Length < len)
                        {
                            backSlash.Append(c);
                            continue;
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

                        backSlash = null;
                        continue;
                    }

                    if (c == '"' && level.Count == 0)
                    {
                        if (sb == null)
                        {
                            sb = new StringBuilder();
                            if (name != null) level.Add('"');
                            continue;
                        }

                        if (name == null)
                        {
                            name = sb.ToString();
                            sb = null;
                            continue;
                        }

                        level.Clear();
                        if (string.IsNullOrWhiteSpace(name)) continue;
                        ListUtility.Add(this, name, sb.ToString());
                        count++;
                        continue;
                    }

                    if (level.Count > 0 && c == level[level.Count - 1])
                    {
                        level.RemoveAt(level.Count - 1);
                        if (sb != null) sb.Append(c);
                        continue;
                    }

                    if (name != null && sb == null)
                    {
                        if (c != '{' && c != '[') continue;
                        level.Add(c);
                    }

                    if (sb == null) sb = new StringBuilder();
                    sb.Append(c);
                }

                return 0; // count;
            }

            if (encoding == null) encoding = DefaultEncoding ?? Encoding.UTF8;
            var pos = query.IndexOf("?");
            if (pos >= 0) query = query.Substring(pos + 1);
            pos = query.IndexOf("#");
            if (pos >= 0) query = query.Substring(0, pos);
            var arr = query.Split('&');
            foreach (var item in arr)
            {
                pos = item.IndexOf("=");
                if (pos < 0) ListUtility.Add(this, HttpUtility.UrlDecode(item, encoding), string.Empty);
                else ListUtility.Add(this, HttpUtility.UrlDecode(item.Substring(0, pos), encoding), HttpUtility.UrlDecode(item.Substring(pos + 1), encoding));
            }

            return arr.Length;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A query string.</returns>
        public override string ToString()
        {
            return ToString(DefaultEncoding);
        }

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
                arr.Add(string.Format("{0}={1}", HttpUtility.UrlEncode(item.Key, encoding), HttpUtility.UrlEncode(item.Value, encoding)));
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
        /// Returns a string HTTP request content.
        /// </summary>
        /// <param name="encoding">The optional encoding to use for the content. Or null for default.</param>
        /// <param name="mediaType">The optional media type to use for the content. Or null for default.</param>
        /// <returns>A string HTTP request content.</returns>
        public StringContent ToStringContent(Encoding encoding = null, string mediaType = null)
        {
            return new StringContent(ToString(), encoding ?? DefaultEncoding ?? Encoding.UTF8, mediaType ?? "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// Parses a string to the query data.
        /// </summary>
        /// <param name="query">The query string.</param>
        /// <returns>A query data instance.</returns>
        public static QueryData Parse(string query)
        {
            var q = new QueryData();
            q.ParseSet(query);
            return q;
        }
    }
}
