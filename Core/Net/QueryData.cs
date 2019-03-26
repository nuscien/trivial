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
    /// The query data in URI after question mark.
    /// </summary>
    public class QueryData : StringKeyValuePairs
    {
        private static string numbool = "-0123456789.tfnuTFNU";
        private static string chars = "_$-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

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
                            var ignoreBackSlash = false;
                            switch (c)
                            {
                                case 'x':
                                case 'X':
                                case 'u':
                                case 'U':
                                    backSlash.Append(c);
                                    ignoreBackSlash = true;
                                    break;
                                case 'R':
                                case 'r':
                                    sb.Append('\r');
                                    break;
                                case 'N':
                                case 'n':
                                    sb.Append('\n');
                                    break;
                                case 'A':
                                case 'a':
                                    sb.Append('\a');
                                    break;
                                case 'B':
                                case 'b':
                                    sb.Append('\b');
                                    break;
                                case 'T':
                                case 't':
                                    sb.Append('\t');
                                    break;
                                case '0':
                                    sb.Append('\0');
                                    break;
                                default:
                                    sb.Append(c);
                                    break;
                            }

                            if (!ignoreBackSlash) backSlash = null;
                            continue;
                        }

                        var len = 0;
                        var firstBackSlash = backSlash[0];
                        if (firstBackSlash == 'x' || firstBackSlash == 'X')
                        {
                            len = 3;
                        }
                        else if (firstBackSlash == 'u' || firstBackSlash == 'U')
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

                    if (sb == null)
                    {
                        if (c == '"')
                        {
                            sb = new StringBuilder();
                            level.Add('"');
                            continue;
                        }

                        if (name == null)
                        {
                            if (chars.IndexOf(c) > -1)
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
                        else if (numbool.IndexOf(c) > -1)
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
                            var sbStr = sb.ToString();
                            if (c == ',')
                            {
                                sbStr = sbStr.Trim();
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

                            ListUtility.Add(this, name, sbStr);
                            name = null;
                            sb = null;
                            continue;
                        }
                    }
                    else if (c == '"') level.Add('"');
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
                    if (sbStr != "null" && sbStr != "undefined") ListUtility.Add(this, name, sbStr);
                }

                return count;
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
