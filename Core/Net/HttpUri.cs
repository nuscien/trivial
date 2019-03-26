using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

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
}
