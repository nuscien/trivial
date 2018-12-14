using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Trivial.Web
{
    /// <summary>
    /// The HTTP URI struct.
    /// </summary>
    public class HttpUri : IEquatable<HttpUri>
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
                if (value.IndexOf("://") == 0 || value.IndexOf("//") == 0) value = value.Substring(value.IndexOf("//") + 2);
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
                if (start >= 0) value = value.Substring(start + 1);
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
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(HttpUri other)
        {
            return other != null && other.ToString() == ToString();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The URI string.</returns>
        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append(IsSecure ? "https" : "http");
            str.Append(Host);
            str.Append("/");
            str.AppendFormat("{0}://{1}", IsSecure ? "https" : "http", Host);
            if (!Port.HasValue || (IsSecure && Port == 443) || (IsSecure && Port == 80)) str.AppendFormat(":{0}", Port);
            str.AppendFormat("/{0}", Path);
            if (Query.Count > 0) str.Append(Query.ToString());
            if (!string.IsNullOrWhiteSpace(Hash)) str.AppendFormat("#{0}", Hash);
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
            uri.Path = url;
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
            return uri != null ? Parse(uri.AbsoluteUri.ToString()) : null; ;
        }
    }

    /// <summary>
    /// The query data in URI.
    /// </summary>
    public class QueryData : Collection.KeyValuePairs<string, string>
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
        /// <returns>A query data instance.</returns>
        public int ParseSet(string query, bool append = false, Encoding encoding = null)
        {
            if (!append) Clear();
            if (string.IsNullOrWhiteSpace(query)) return 0;
            if (encoding == null) encoding = DefaultEncoding ?? Encoding.UTF8;
            var pos = query.IndexOf("?");
            if (pos >= 0) query = query.Substring(pos + 1);
            pos = query.IndexOf("#");
            if (pos >= 0) query = query.Substring(0, pos);
            var arr = query.Split('&');
            foreach (var item in arr)
            {
                pos = item.IndexOf("=");
                if (pos < 0) Add(HttpUtility.UrlDecode(item, encoding), string.Empty);
                else Add(HttpUtility.UrlDecode(item.Substring(0, pos), encoding), HttpUtility.UrlDecode(item.Substring(pos + 1), encoding));
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
            return url + (url.IndexOf("?") >= 0 ? "&" : "?") + ToString(encoding);
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
