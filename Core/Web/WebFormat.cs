using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Trivial.Web
{
    /// <summary>
    /// Web format utility.
    /// </summary>
    public static partial class WebFormat
    {
        /// <summary>
        /// Parses JavaScript date tick to date and time.
        /// </summary>
        /// <param name="tick">The JavaScript date tick.</param>
        /// <returns>A date and time.</returns>
        public static DateTime ParseDate(long tick)
        {
            var time = new DateTime(tick * 10000 + 621355968000000000, DateTimeKind.Utc);
            return time.ToLocalTime();
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time.
        /// </summary>
        /// <param name="tick">The JavaScript date tick.</param>
        /// <returns>A date and time.</returns>
        public static DateTime? ParseDate(long? tick)
        {
            if (!tick.HasValue) return null;
            return ParseDate(tick.Value);
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        public static long ParseDate(DateTime date)
        {
            return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        public static long ParseDate(DateTimeOffset date)
        {
            return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        public static long? ParseDate(DateTime? date)
        {
            if (!date.HasValue) return null;
            return ParseDate(date.Value);
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        public static long? ParseDate(DateTimeOffset? date)
        {
            if (!date.HasValue) return null;
            return ParseDate(date.Value);
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
        /// <returns>A Base64Url string.</returns>
        public static string Base64UrlEncode(object obj)
        {
            var t = obj.GetType();
            if (t.FullName.Equals("System.Text.Json.JsonDocument", StringComparison.InvariantCulture))
            {
                try
                {
                    var prop = t.GetProperty("RootElement");
                    if (prop != null && prop.CanRead)
                    {
                        var ele = prop.GetValue(obj, null);
                        if (ele is null) return string.Empty;
                        return Base64UrlEncode(ele.ToString());
                    }
                }
                catch (AmbiguousMatchException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (TargetException)
                {
                }
                catch (TargetParameterCountException)
                {
                }
                catch (TargetInvocationException)
                {
                }
                catch (MemberAccessException)
                {
                }
            }

            if (t.FullName.StartsWith("Newtonsoft.Json.Linq.J", StringComparison.InvariantCulture))
            {
                if (t.FullName.Equals("Newtonsoft.Json.Linq.JObject", StringComparison.InvariantCulture)
                    || t.FullName.Equals("Newtonsoft.Json.Linq.JArray", StringComparison.InvariantCulture))
                    return Base64UrlEncode(obj.ToString());
            }

            if (t == typeof(string)) return Base64UrlEncode(obj.ToString());

            var serializer = new DataContractJsonSerializer(t);
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                stream.Position = 0;
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return Base64UrlEncode(bytes);
            }
        }

        /// <summary>
        /// Decodes the string from a Base64Url format.
        /// </summary>
        /// <param name="s">A Base64Url encoded string.</param>
        /// <returns>A plain text.</returns>
        public static byte[] Base64UrlDecode(string s)
        {
            if (s == null) return null;
            if (s == string.Empty) return new byte[0];
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
        /// <returns>The object typed.</returns>
        public static T Base64UrlDecodeTo<T>(string s)
        {
            if (string.IsNullOrEmpty(s)) return default;
            var bytes = Base64UrlDecode(s);
            var d = GetJsonDeserializer<T>();
            if (d != null) return d(s);
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream(bytes))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Gets the JSON deserializer.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <returns>A function for deserialization.</returns>
        internal static Func<string, T> GetJsonDeserializer<T>()
        {
            var t = typeof(T);
            if (t.FullName.Equals("System.Text.Json.JsonDocument", StringComparison.InvariantCulture))
            {
                foreach (var method in t.GetMethods())
                {
                    if (!method.IsStatic || method.Name != "Parse") continue;
                    var parameters = method.GetParameters();
                    if (parameters.Length != 2 && parameters[0].ParameterType != typeof(string) && !parameters[1].IsOptional) continue;
                    return str =>
                    {
                        return (T)method.Invoke(null, new object[] { str, null });
                    };
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
    }
}
