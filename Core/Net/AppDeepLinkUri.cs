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
    /// The deep link URI struct for apps.
    /// </summary>
    public class AppDeepLinkUri : IEquatable<AppDeepLinkUri>, IEquatable<HttpUri>, IEquatable<Uri>, IEquatable<string>
    {
#pragma warning disable IDE0057
        private string path;
        private List<string> folders = new();

        /// <summary>
        /// Gets or sets the protocal name.
        /// </summary>
        public string Protocal { get; set; } = "https";

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
        public bool Equals(AppDeepLinkUri other)
        {
            return other != null && other.ToString() == ToString();
        }

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
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The instance to compare.</param>
        /// <returns>true if they are equal; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other is AppDeepLinkUri d) return Equals(d);
            if (other is HttpUri h) return Equals(h);
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
            str.AppendFormat("{0}://{1}", Protocal, Path);
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
        public static AppDeepLinkUri Parse(string url)
        {
            var hostPos = url.IndexOf("://");
            var uri = new AppDeepLinkUri();
            if (hostPos >= 0)
            {
                if (hostPos > 0) uri.Protocal = url.Substring(0, hostPos);
                url = url.Substring(hostPos + 3);
            }
            else if (url.StartsWith("//"))
            {
                url = url.Substring(2);
            }
            else if (url.StartsWith("/"))
            {
                url = url.Substring(1);
            }

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
        public static explicit operator Uri(AppDeepLinkUri uri)
        {
            return uri?.ToUri();
        }

        /// <summary>
        /// Converts to a URI object.
        /// </summary>
        /// <param name="uri">The instance to convert.</param>
        public static explicit operator HttpUri(AppDeepLinkUri uri)
        {
            return uri?.ToUri();
        }

        /// <summary>
        /// Converts to a URI object.
        /// </summary>
        /// <param name="uri">The instance to convert.</param>
        public static implicit operator AppDeepLinkUri(Uri uri)
        {
            return uri != null ? Parse(uri.ToString()) : null;
        }

        /// <summary>
        /// Converts to a URI object.
        /// </summary>
        /// <param name="uri">The instance to convert.</param>
        public static implicit operator AppDeepLinkUri(HttpUri uri)
        {
            return uri != null ? Parse(uri.ToString()) : null;
        }
#pragma warning restore IDE0057
    }
}
