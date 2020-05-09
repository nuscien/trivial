using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Text.Json;

using Trivial.Text;

namespace Trivial.Web
{
    /// <summary>
    /// Web format utility.
    /// </summary>
    public static partial class WebFormat
    {
        private const long ticksOffset = 621355968000000000;

        /// <summary>
        /// Parses JavaScript date tick to date and time.
        /// </summary>
        /// <param name="ticks">The JavaScript date ticks.</param>
        /// <returns>A date and time.</returns>
        public static DateTime ParseDate(long ticks)
        {
            var time = new DateTime(ticks * 10000 + ticksOffset, DateTimeKind.Utc);
            return time.ToLocalTime();
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time.
        /// </summary>
        /// <param name="ticks">The JavaScript date ticks.</param>
        /// <returns>A date and time.</returns>
        public static DateTime? ParseDate(long? ticks)
        {
            if (!ticks.HasValue) return null;
            return ParseDate(ticks.Value);
        }

        /// <summary>
        /// Parses JavaScript date ticks to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date ticks.</returns>
        public static long ParseDate(DateTime date)
        {
            return (date.ToUniversalTime().Ticks - ticksOffset) / 10000;
        }

        /// <summary>
        /// Parses JavaScript date ticks to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date ticks.</returns>
        public static long ParseDate(DateTimeOffset date)
        {
            return (date.ToUniversalTime().Ticks - ticksOffset) / 10000;
        }

        /// <summary>
        /// Parses JavaScript date ticks to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date ticks.</returns>
        public static long? ParseDate(DateTime? date)
        {
            if (!date.HasValue) return null;
            return ParseDate(date.Value);
        }

        /// <summary>
        /// Parses JavaScript date ticks to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date ticks.</returns>
        public static long? ParseDate(DateTimeOffset? date)
        {
            if (!date.HasValue) return null;
            return ParseDate(date.Value);
        }

        /// <summary>
        /// Parses a ISO 8601 date string to date time.
        /// </summary>
        /// <param name="s">The JSON token value of JavaScript date.</param>
        /// <returns>A date and time.</returns>
        public static DateTime? ParseDate(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim().ToUpperInvariant();
            if (s.Length == 8)
            {
                var y2 = GetNaturalNumber(s, 0, 4);
                if (y2 < 0) return null;
                if (s[4] == 'W')
                {
                    var w3 = GetNaturalNumber(s, 5, 2);
                    if (w3 < 0) return null;
                    var d3 = GetNaturalNumber(s, 7);
                    if (d3 < 0) return null;
                    var r = new DateTime(y2, 1, 4, 0, 0, 0, DateTimeKind.Utc);
                    r = r.AddDays((r.DayOfWeek == DayOfWeek.Sunday ? -7 : -(int)r.DayOfWeek) + w3 * 7 - 7 + d3);
                    return r;
                }

                var m2 = GetNaturalNumber(s, 4, 2);
                if (m2 < 0) return null;
                var d2 = GetNaturalNumber(s, 6);
                if (d2 < 0) return null;
                return new DateTime(y2, m2, d2, 0, 0, 0, DateTimeKind.Utc);
            }
            
            if (s.Length < 10 || s[4] != '-') return null;
            var y = GetNaturalNumber(s, 0, 4);
            if (y < 0) return null;
            if (s[5] == 'W')
            {
                var w3 = GetNaturalNumber(s, 6, 2);
                if (w3 < 0) return null;
                var d3 = GetNaturalNumber(s, s[8] == '-' ? 9 : 8, 1);
                if (d3 < 0) return null;
                var r = new DateTime(y, 1, 4, 0, 0, 0, DateTimeKind.Utc);
                r = r.AddDays((r.DayOfWeek == DayOfWeek.Sunday ? -7 : -(int)r.DayOfWeek) + w3 * 7 - 7 + d3);
                return r;
            }

            var pos = s[7] == '-' ? 8 : 7;
            var m = GetNaturalNumber(s, 5, 2);
            if (m < 0)
            {
                if (s[6] == '-') m = GetNaturalNumber(s, 5, 1);
                if (m < 0) return null;
            }

            var d = GetNaturalNumber(s, pos, 2);
            if (d < 1)
            {
                pos += 4;
                d = GetNaturalNumber(s, pos, 1);
                if (d < 1) return null;
            }
            else
            {
                pos += 3;
            }

            var date = new DateTime(y, m, d, 0, 0, 0, DateTimeKind.Utc);
            if (pos >= s.Length) return date;
            s = s.Substring(pos);
            var arr = s.Split(':');
            if (arr.Length < 2) return date;
            if (!int.TryParse(arr[0], out var h)) return date;
            if (!int.TryParse(arr[1], out var mm)) return date;
            var t = new DateTime(y, m, d, h, mm, 0, DateTimeKind.Utc).ToLocalTime();
            if (arr.Length == 2) return t;
            if (arr.Length == 3)
            {
                if (!double.TryParse(arr[2].Replace("Z", string.Empty), out var sf)) return t;
                return t.AddSeconds(sf);
            }

            if (arr[2].Length < 5)
            {
                var sf = GetNaturalNumber(arr[2], 0, 2);
                return sf > 0 ? t.AddSeconds(sf) : t;
            }

            var sec = GetNaturalNumber(arr[2], 0, 2);
            if (sec < 0 || !int.TryParse(arr[3], out var rm)) return t;
            var neg = arr[2][2] == '-' ? 1 : -1;
            var hasSep = (neg == 1) || (arr[2][2] == '+');
            var rh = GetNaturalNumber(arr[2], hasSep ? 3 : 2);
            return t.AddSeconds(sec).AddMinutes(neg * rm).AddHours(neg * rh);
        }

        /// <summary>
        /// Parses Unix timestamp to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        internal static long ParseUnixTimestamp(DateTime date)
        {
            return ParseDate(date) / 1000;
        }

        /// <summary>
        /// Parses Unix timestamp to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        internal static long? ParseUnixTimestamp(DateTime? date)
        {
            if (!date.HasValue) return null;
            return ParseUnixTimestamp(date.Value);
        }

        /// <summary>
        /// Parses Unix timestamp to date and time.
        /// </summary>
        /// <param name="timestamp">The JavaScript date timestamp.</param>
        /// <returns>A date and time.</returns>
        internal static DateTime ParseUnixTimestamp(long timestamp)
        {
            return ParseDate(timestamp * 1000);
        }

        /// <summary>
        /// Parses Unix timestamp to date and time.
        /// </summary>
        /// <param name="timestamp">The Unix timestamp.</param>
        /// <returns>A date and time.</returns>
        internal static DateTime? ParseUnixTimestamp(long? timestamp)
        {
            if (!timestamp.HasValue) return null;
            return ParseUnixTimestamp(timestamp.Value);
        }

        /// <summary>
        /// Encodes a specific byte array into Base64Url format.
        /// </summary>
        /// <param name="bytes">The value to encode.</param>
        /// <returns>A Base64Url string.</returns>
        public static string Base64UrlEncode(byte[] bytes)
        {
            if (bytes == null) return null;
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", string.Empty);
        }

        /// <summary>
        /// Encodes a specific string into Base64Url format.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <param name="encoding">Optional text encoding.</param>
        /// <returns>A Base64Url string.</returns>
        public static string Base64UrlEncode(string value, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var bytes = (encoding ?? Encoding.UTF8).GetBytes(value);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", string.Empty);
        }

        /// <summary>
        /// Encodes a specific object into JSON Base64Url format.
        /// </summary>
        /// <param name="obj">The object to encode.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A Base64Url string.</returns>
        public static string Base64UrlEncode(object obj, JsonSerializerOptions options = null)
        {
            if (obj == null) return string.Empty;
            var t = obj.GetType();
            if (t == typeof(string)) return Base64UrlEncode(obj.ToString(), Encoding.UTF8);
            return Base64UrlEncode(StringExtensions.ToJson(obj, options ?? new JsonSerializerOptions
            {
                IgnoreNullValues = true
            }));
        }

        /// <summary>
        /// Decodes the string from a Base64Url format.
        /// </summary>
        /// <param name="s">A Base64Url encoded string.</param>
        /// <returns>A plain text.</returns>
        public static byte[] Base64UrlDecode(string s)
        {
            if (s == null) return null;
            if (s == string.Empty) return Array.Empty<byte>();
            s = s.Replace("-", "+").Replace("_", "/");
            var rest = s.Length % 4;
            if (rest > 0) s = s.PadRight(4 - rest + s.Length, '=');
            var bytes = Convert.FromBase64String(s);
            return bytes;
        }

        /// <summary>
        /// Decodes the string from a Base64Url format.
        /// </summary>
        /// <param name="s">A Base64Url encoded string.</param>
        /// <param name="encoding">Optional text encoding.</param>
        /// <returns>A plain text.</returns>
        public static string Base64UrlDecodeToString(string s, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var bytes = Base64UrlDecode(s);
            return (encoding ?? Encoding.ASCII).GetString(bytes);
        }

        /// <summary>
        /// Decodes and deserializes the object from a JSON Base64Url format.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="s">A Base64Url encoded string.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>The object typed.</returns>
        public static T Base64UrlDecodeTo<T>(string s, JsonSerializerOptions options = null)
        {
            if (string.IsNullOrEmpty(s)) return default;
            s = Base64UrlDecodeToString(s, Encoding.UTF8);
            var d = GetJsonDeserializer<T>();
            if (d != null) return d(s);
            return (T)JsonSerializer.Deserialize(s, typeof(T), options);
        }

        /// <summary>
        /// Gets the JSON deserializer.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <returns>A function for deserialization.</returns>
        internal static Func<string, T> GetJsonDeserializer<T>(bool ignoreJsonDoc = false)
        {
            var t = typeof(T);
            if (t == typeof(JsonDocument))
            {
                if (ignoreJsonDoc) return null;
                return str =>
                {
                    if (string.IsNullOrWhiteSpace(str)) return default;
                    return (T)(object)JsonDocument.Parse(str);
                };
            }
            else if (t == typeof(JsonObject))
            {
                if (ignoreJsonDoc) return null;
                return str =>
                {
                    if (string.IsNullOrWhiteSpace(str)) return default;
                    return (T)(object)JsonObject.Parse(str);
                };
            }
            else if (t == typeof(JsonArray))
            {
                if (ignoreJsonDoc) return null;
                return str =>
                {
                    if (string.IsNullOrWhiteSpace(str)) return default;
                    return (T)(object)JsonArray.Parse(str);
                };
            }
            else if (t.FullName.StartsWith("System.Text.Json.Json", StringComparison.InvariantCulture) && t.IsClass)
            {
                try
                {
                    var n = t;
                    if (t.Name.Equals("JsonNode", StringComparison.InvariantCulture))
                    {
                    }
                    else if (t.Name.Equals("JsonObject", StringComparison.InvariantCulture) || t.Name.Equals("JsonArray", StringComparison.InvariantCulture))
                    {
                        n = t.Assembly.GetType("System.Text.Json.JsonNode", false);
                        if (n == null) return null;
                    }
                    else
                    {
                        return null;
                    }

                    var parser = n.GetMethod("Parse", new[] { typeof(string) });
                    if (parser != null && parser.IsStatic)
                    {
                        return str =>
                        {
                            if (string.IsNullOrWhiteSpace(str)) return default;
                            return (T)parser.Invoke(null, new object[] { str });
                        };
                    }

                    parser = n.GetMethod("Parse", new[] { typeof(string), typeof(JsonDocumentOptions) });
                    if (parser != null && parser.IsStatic)
                    {
                        return str =>
                        {
                            if (string.IsNullOrWhiteSpace(str)) return default;
                            return (T)parser.Invoke(null, new object[] { str, default(JsonDocumentOptions) });
                        };
                    }
                }
                catch (AmbiguousMatchException)
                {
                }
                catch (ArgumentException)
                {
                }
            }
            else if (t.FullName.StartsWith("Newtonsoft.Json.Linq.J", StringComparison.InvariantCulture))
            {
                try
                {
                    var parser = t.GetMethod("Parse", new[] { typeof(string) });
                    if (parser != null && parser.IsStatic)
                    {
                        return str =>
                        {
                            if (string.IsNullOrWhiteSpace(str)) return default;
                            return (T)parser.Invoke(null, new object[] { str });
                        };
                    }
                }
                catch (AmbiguousMatchException)
                {
                }
                catch (ArgumentException)
                {
                }
            }
            else if (t == typeof(string))
            {
                return str => (T)(object)str;
            }

            return null;
        }

        /// <summary>
        /// Tries to get the integer from a part of the specific string.
        /// </summary>
        /// <param name="s">A specific string.</param>
        /// <param name="start">The start index of the string to get the integer.</param>
        /// <param name="len">The length to get.</param>
        /// <returns>A natural number; or -1, if failed.</returns>
        private static int GetNaturalNumber(string s, int start, int? len = null)
        {
            const uint ZERO = '0';
            var end = len.HasValue ? Math.Min(start + len.Value, s.Length) : s.Length;
            uint n = 0;
            for (var i = start; i < end; i++)
            {
                var c = s[i];
                var j = c - ZERO;
                if (j > 9) return -1;
                n = n * 10 + j;
            }

            return (int)n;
        }
    }
}
