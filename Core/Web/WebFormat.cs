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
using System.Web;
using Trivial.Net;
using Trivial.Text;

namespace Trivial.Web;

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
    /// <example>
    /// var t = WebFormat.ParseDate(1594958400000);
    /// </example>
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
    /// Parses an ISO 8601 date string, or a date and time string of RFC 822 and RFC 7231, to date time.
    /// </summary>
    /// <param name="s">The JSON token value of JavaScript date.</param>
    /// <returns>A date and time; or null, if the input is not a date and time string.</returns>
    /// <example>
    /// var d1 = WebFormat.ParseDate("2020W295");
    /// var d2 = WebFormat.ParseDate("2020-7-17 12:00:00");
    /// var d3 = WebFormat.ParseDate("2020-07-17T12:00:00Z");
    /// var d4 = WebFormat.ParseDate("Fri, 17 Jul 2020 12:00:00 GMT");
    /// var d5 = WebFormat.ParseDate("Fri, 17 Jul 2020 04:00:00 GMT-0800");
    /// var d6 = WebFormat.ParseDate("Fri Jul 17 2020 12:00:00 GMT");
    /// var d7 = WebFormat.ParseDate("2020年7月17日 12:00:00");
    /// </example>
    public static DateTime? ParseDate(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim().ToUpperInvariant();
        if (s.Length == 8)
        {
            var y2 = GetNaturalNumber(s, 0, 4);
            if (y2 < 0) return null;
            if (s[4] == 'W')    // [ISO 8601] 2020W295
            {
                var w3 = GetNaturalNumber(s, 5, 2);
                if (w3 < 0) return null;
                var d3 = GetNaturalNumber(s, 7);
                if (d3 < 0) return null;
                var r = new DateTime(y2, 1, 4, 0, 0, 0, DateTimeKind.Utc);
                r = r.AddDays((r.DayOfWeek == DayOfWeek.Sunday ? -7 : -(int)r.DayOfWeek) + w3 * 7 - 7 + d3);
                return r;
            }

            // 20200717 || 2020-7-17
            var m2 = s[4] == '-' ? GetNaturalNumber(s, 5, 1) : GetNaturalNumber(s, 4, 2);
            if (m2 < 0) return null;
            var d2 = GetNaturalNumber(s, s[6] == '-' ? 7 : 6);
            if (d2 < 0) return null;
            return new DateTime(y2, m2, d2, 0, 0, 0, DateTimeKind.Utc);
        }
        
        if (s.Length > 27)
        {
            if (s[3] == ',' && s[4] == ' ') // [RFC 822 Date Time & RFC 7231 Date Time] Fri, 17 Jul 2020 12:00:00 GMT
            {
                var d2 = GetNaturalNumber(s, 5, 2);
                var m2 = GetMonth(s, 8);
                if (d2 > 0 && m2 > 0)
                {
                    var y2 = GetNaturalNumber(s, 12, 4);
                    var h2 = GetNaturalNumber(s, 17, 2);
                    var mm2 = GetNaturalNumber(s, 20, 2);
                    var s2 = GetNaturalNumber(s, 23, 2);
                    try
                    {
                        var time = new DateTime(y2, m2, d2, h2, mm2, s2, DateTimeKind.Utc);
                        if (s.Length > 33 && (s[29] == '+' || s[29] == '-'))
                        {
                            var offsetH = GetNaturalNumber(s, 30, 2);
                            if (offsetH < 0) offsetH = 0;
                            if (s[29] == '+') offsetH = -offsetH;
                            time = time.AddHours(offsetH);
                            var offsetM = GetNaturalNumber(s, 32, 2);
                            if (offsetM > 0) time = time.AddMinutes(-offsetM);
                        }

                        return time;
                    }
                    catch (ArgumentException)
                    {
                        return null;
                    }
                }
            }
            else if (s[3] == ' ' && s[7] == ' ')    // [JS Date] Fri Jul 17 2020 12:00:00 GMT
            {
                var m2 = GetMonth(s, 4);
                var d2 = GetNaturalNumber(s, 8);
                if (d2 > 0 && m2 > 0)
                {
                    var y2 = GetNaturalNumber(s, 11, 4);
                    var h2 = GetNaturalNumber(s, 16, 2);
                    var mm2 = GetNaturalNumber(s, 19, 2);
                    var s2 = GetNaturalNumber(s, 22, 2);
                    try
                    {
                        var time = new DateTime(y2, m2, d2, h2, mm2, s2, DateTimeKind.Utc);
                        if (s.Length > 32 && (s[28] == '+' || s[28] == '-'))
                        {
                            var offsetH = GetNaturalNumber(s, 29, 2);
                            if (offsetH < 0) offsetH = 0;
                            if (s[28] == '+') offsetH = -offsetH;
                            time = time.AddHours(offsetH);
                            var offsetM = GetNaturalNumber(s, 31, 2);
                            if (offsetM > 0) time = time.AddMinutes(-offsetM);
                        }

                        return time;
                    }
                    catch (ArgumentException)
                    {
                        return null;
                    }
                }
            }
        }

        if (s.Length < 10)
        {
            if (s.Length > 3 && (s[1] == '月' || (s[2] == '月' && (s[0] == '1' || s[0] == '0'))) && (s.IndexOf('日') > 2 || s.IndexOf('号') > 2) && s.IndexOf('年') < 0)
                s = $"{DateTime.Now.Year:g}-{s.Replace('月', '-').Replace("日", string.Empty).Replace("号", string.Empty)}";
            else
                return s switch
                {
                    "NOW" or "现在" or "現在" or "今" or "지금" or "AHORA" or "AGORA" or "СЕЙЧАС" or "ТЕПЕРЬ" => DateTime.UtcNow,
                    "TODAY" or "今日" or "今天" or "当天" or "KYO" or "오늘" or "HOY" or "СЕГОДНЯ" => DateTime.UtcNow.Date,
                    "TOMORROW" or "明日" or "明天" or "次日" or "翌日" or "DEMAIN" or "내일" or "MAÑANA" or "AMANHÃ" or "ЗАВТРА" => DateTime.UtcNow.AddDays(1),
                    "YESTERDAY" or "昨日" or "昨天" or "HIER" or "어제" or "AYER" or "ONTEM" or "ВЧЕРА" => DateTime.UtcNow.AddDays(-1),
                    _ => null
                };
        }

        var y = GetNaturalNumber(s, 0, 4);
        if (s[4] != '-')
        {
            if (s[4] == '年')    // 2020年7月17日 12:00:00
            {
                s = s.Replace('年', '-').Replace('月', '-').Replace("日", string.Empty).Replace("号", string.Empty);
            }
            else
            {
                if (s.Length < 11 || (s[6] != ' ' && s[6] != '-'))
                    return s switch
                    {
                        "MAINTENANT" => DateTime.UtcNow,
                        "AUJOURD'HUI" => DateTime.UtcNow.Date,
                        _ => null
                    };

                // 17-Jul-2020 12:00:00
                y = GetNaturalNumber(s, 7, 4);
                if (y < 0) return null;
                int m2;
                int d2;
                if (s[2] == ' ' || s[2] == '-')
                {
                    d2 = GetNaturalNumber(s, 0, 2);
                    m2 = GetMonth(s, 3);
                }
                else
                {
                    d2 = GetNaturalNumber(s, 4, 2);
                    m2 = GetMonth(s, 0);
                }

                if (d2 < 0 || m2 < 0) return null;
                if (s.Length < 16) return new DateTime(y, m2, d2, 0, 0, 0, DateTimeKind.Utc);
                #pragma warning disable IDE0057
                var arr2 = s.Substring(12).Split(new[] { ':' }, StringSplitOptions.None);
                #pragma warning restore IDE0057
                if (arr2.Length < 2 || string.IsNullOrEmpty(arr2[0]) || string.IsNullOrEmpty(arr2[1]) || !int.TryParse(arr2[0], out var h2) || !int.TryParse(arr2[1], out var min))
                    return new DateTime(y, m2, d2, 0, 0, 0, DateTimeKind.Utc);
                if (arr2.Length == 2 || string.IsNullOrEmpty(arr2[2]) || !int.TryParse(arr2[2], out var s2)) s2 = 0;
                return new DateTime(y, m2, d2, h2, min, s2, DateTimeKind.Utc);
            }
        }

        if (y < 0) return null;
        if (s[5] == 'W')    // [ISO 8601] 2020-W29-5 || 2020-W295
        {
            var w3 = GetNaturalNumber(s, 6, 2);
            if (w3 < 0) return null;
            var d3 = GetNaturalNumber(s, s[8] == '-' ? 9 : 8, 1);
            if (d3 < 0) return null;
            var r = new DateTime(y, 1, 4, 0, 0, 0, DateTimeKind.Utc);
            r = r.AddDays((r.DayOfWeek == DayOfWeek.Sunday ? -7 : -(int)r.DayOfWeek) + w3 * 7 - 7 + d3);
            return r;
        }

        // [ISO 8601] 2020-07-17T12:00:00Z
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
        #pragma warning disable IDE0057
        s = s.Substring(pos);
        #pragma warning restore IDE0057
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
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
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
    /// <param name="ignoreJsonDoc">true if need return null for JSON format; otherwise, false.</param>
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

        if (t == typeof(JsonObjectNode))
        {
            if (ignoreJsonDoc) return null;
            return str =>
            {
                if (string.IsNullOrWhiteSpace(str)) return default;
                return (T)(object)JsonObjectNode.Parse(str);
            };
        }

        if (t == typeof(JsonArrayNode))
        {
            if (ignoreJsonDoc) return null;
            return str =>
            {
                if (string.IsNullOrWhiteSpace(str)) return default;
                return (T)(object)JsonArrayNode.Parse(str);
            };
        }
        
        if (t == typeof(System.Text.Json.Nodes.JsonNode) || t.IsSubclassOf(typeof(System.Text.Json.Nodes.JsonNode)))
        {
            if (ignoreJsonDoc) return null;
            return str =>
            {
                if (string.IsNullOrWhiteSpace(str)) return default;
                var value = System.Text.Json.Nodes.JsonNode.Parse(str);
                if (value is null) return default;
                return (T)(object)value;
            };
        }
        
        if (t.FullName.StartsWith("Newtonsoft.Json.Linq.J", StringComparison.InvariantCulture))
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
        
        if (t == typeof(string))
        {
            return str => (T)(object)str;
        }

        if (t == typeof(IEnumerable<ServerSentEventInfo>))
        {
            if (ignoreJsonDoc) return null;
            return str => (T)(object)ServerSentEventInfo.Parse(str);
        }

        return null;
    }

    /// <summary>
    /// Encodes a URL string using the specified encoding object.
    /// </summary>
    /// <param name="str">The text to encode.</param>
    /// <param name="e">The encoding object that specifies the encoding scheme.</param>
    /// <returns>An encoded string.</returns>
    internal static string UrlEncode(string str, Encoding e = null)
    {
#if NETFRAMEWORK
        return System.Net.WebUtility.UrlEncode(str);
#else
        return e == null ? HttpUtility.UrlEncode(str) : HttpUtility.UrlEncode(str, e);
#endif
    }

    /// <summary>
    /// Converts a URL-encoded string into a decoded string, using the specified encoding object.
    /// </summary>
    /// <param name="str">The string to decode.</param>
    /// <param name="e">The encoding object that specifies the decoding scheme.</param>
    /// <returns>A decoded string.</returns>
    internal static string UrlDecode(string str, Encoding e = null)
    {
#if NETFRAMEWORK
        return System.Net.WebUtility.UrlDecode(str);
#else
        return e == null ? HttpUtility.UrlDecode(str) : HttpUtility.UrlDecode(str, e);
#endif
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

    private static int GetMonth(string s, int startIndex)
    {
        return s.Substring(startIndex, 3) switch
        {
            "JAN" => 1,
            "FEB" => 2,
            "MAR" => 3,
            "APR" => 4,
            "MAY" => 5,
            "JUN" => 6,
            "JUL" => 7,
            "AUG" => 8,
            "SEP" => 9,
            "OCT" => 10,
            "NOV" => 11,
            "DEC" => 12,
            _ => -1
        };
    }
}
