using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Text
{
    /// <summary>
    /// The indent styles.
    /// </summary>
    public enum JsonIndentStyles
    {
        /// <summary>
        /// Minified format. Without any extra white space.
        /// </summary>
        Minified = 0,

        /// <summary>
        /// Without any extra white space.
        /// </summary>
        Empty = 1,

        /// <summary>
        /// Tab indent style.
        /// </summary>
        Tab = 2,

        /// <summary>
        /// 4 white spaces indent style.
        /// </summary>
        Normal = 3,

        /// <summary>
        /// 2 white spaces indent style.
        /// </summary>
        Compact = 4,

        /// <summary>
        /// 8 white spaces indent style.
        /// </summary>
        Wide = 5,

        /// <summary>
        /// 1 white space indent style.
        /// </summary>
        Space = 6
    }

    /// <summary>
    /// The extensions for class IJsonValue, JsonDocument, JsonElement, etc.
    /// </summary>
    public static class JsonExtensions
    {
        internal const string Num36 = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// JSON null.
        /// </summary>
        public static readonly IJsonValueResolver Null = new JsonNull(JsonValueKind.Null);

        /// <summary>
        /// JSON undefined.
        /// </summary>
        public static readonly IJsonValueResolver Undefined = new JsonNull(JsonValueKind.Undefined);
        /// <summary>
        /// Gets the MIME value of JSON format text.
        /// </summary>
        public const string MIME = "application/json";

        /// <summary>
        /// Converts from JSON document.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        public static IJsonValueResolver ToJsonValue(JsonDocument json)
        {
            return ToJsonValue(json.RootElement);
        }

        /// <summary>
        /// Converts from JSON element.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        public static IJsonValueResolver ToJsonValue(JsonElement json)
        {
            return json.ValueKind switch
            {
                JsonValueKind.Undefined => Undefined,
                JsonValueKind.Null => Null,
                JsonValueKind.String => new JsonString(json.GetString()),
                JsonValueKind.Number => json.TryGetInt64(out var l)
                    ? new JsonInteger(l)
                    : (json.TryGetDouble(out var d) ? new JsonDouble(d) : Null),
                JsonValueKind.True => new JsonBoolean(true),
                JsonValueKind.False => new JsonBoolean(false),
                JsonValueKind.Array => (JsonArray)json,
                JsonValueKind.Object => (JsonObject)json,
                _ => null
            };
        }

        /// <summary>
        /// Attempts to represent the current JSON string or JavaScript date tidks number as a date time.
        /// </summary>
        /// <param name="json">The JSON element.</param>
        /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
        /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
        /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
        public static bool TryGetJavaScriptDateTicks(this JsonElement json, out DateTime value)
        {
            switch (json.ValueKind)
            {
                case JsonValueKind.String:
                    if (json.TryGetDateTime(out DateTime tmp)) break;
                    value = tmp;
                    return true;
                case JsonValueKind.Number:
                    if (!json.TryGetInt64(out long tick)) break;
                    value = InternalHelper.ParseDate(tick);
                    return true;
                default:
                    throw new InvalidOperationException("The value kind should be string or number.");
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to represent the current JSON string or Unix timestamps number as a date time.
        /// </summary>
        /// <param name="json">The JSON element.</param>
        /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
        /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
        /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
        public static bool TryGetUnixTimestamps(this JsonElement json, out DateTime value)
        {
            switch (json.ValueKind)
            {
                case JsonValueKind.String:
                    if (json.TryGetDateTime(out DateTime tmp)) break;
                    value = tmp;
                    return true;
                case JsonValueKind.Number:
                    if (!json.TryGetInt64(out long tick)) break;
                    value = InternalHelper.ParseUnixTimestamp(tick);
                    return true;
                default:
                    throw new InvalidOperationException("The value kind should be string or number.");
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to represent the current JSON string or Windows file time number as a date time.
        /// </summary>
        /// <param name="json">The JSON element.</param>
        /// <param name="value">When this method returns, contains the date and time value equivalent to the current JSON string.</param>
        /// <returns>true if the string can be represented as a System.DateTime; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">This value kind is not string or number.</exception>
        /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
        public static bool TryGetWindowsFileTimeUtc(this JsonElement json, out DateTime value)
        {
            switch (json.ValueKind)
            {
                case JsonValueKind.String:
                    if (json.TryGetDateTime(out DateTime tmp)) break;
                    value = tmp;
                    return true;
                case JsonValueKind.Number:
                    if (!json.TryGetInt64(out long tick)) break;
                    value = DateTime.FromFileTimeUtc(tick);
                    return true;
                default:
                    throw new InvalidOperationException("The value kind should be string or number.");
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Deserializes the HTTP JSON content into an object as the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task<T> DeserializeJsonAsync<T>(this HttpContent httpContent, CancellationToken cancellationToken = default)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
            using var stream = await httpContent.ReadAsStreamAsync();
            var type = typeof(T);
            if (type == typeof(JsonObject)) return (T)(object)await JsonObject.ParseAsync(stream, default, cancellationToken);
            if (type == typeof(JsonDocument)) return (T)(object)await JsonDocument.ParseAsync(stream, default, cancellationToken);
            if (type == typeof(JsonArray)) return (T)(object)await JsonArray.ParseAsync(stream, default, cancellationToken);
            return await JsonSerializer.DeserializeAsync<T>(stream, default(JsonSerializerOptions), cancellationToken);
        }

        /// <summary>
        /// Deserializes the HTTP JSON content into an object as the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="options">The options for serialization.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task<T> DeserializeJsonAsync<T>(this HttpContent httpContent, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
            using var stream = await httpContent.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
        }

        /// <summary>
        /// Deserializes the HTTP JSON content into an object as the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="options">The options for serialization.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task<T> DeserializeJsonAsync<T>(this HttpContent httpContent, DataContractJsonSerializerSettings options)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
            using var stream = await httpContent.ReadAsStreamAsync();
            var serializer = options != null ? new DataContractJsonSerializer(typeof(T), options) : new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }

        /// <summary>
        /// Deserializes the HTTP JSON content into an object as the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="webResponse">The web response.</param>
        /// <param name="options">The options for serialization.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static ValueTask<T> DeserializeJsonAsync<T>(this WebResponse webResponse, JsonSerializerOptions options = null)
        {
            if (webResponse == null) throw new ArgumentNullException(nameof(webResponse), "webResponse should not be null.");
            using var stream = webResponse.GetResponseStream();
            return JsonSerializer.DeserializeAsync<T>(stream, options);
        }

        /// <summary>
        /// Deserializes the HTTP JSON content into an object as the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="webResponse">The web response.</param>
        /// <param name="options">The options for serialization.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static T DeserializeJson<T>(this WebResponse webResponse, DataContractJsonSerializerSettings options)
        {
            if (webResponse == null) throw new ArgumentNullException(nameof(webResponse), "webResponse should not be null.");
            using var stream = webResponse.GetResponseStream();
            var serializer = options != null ? new DataContractJsonSerializer(typeof(T), options) : new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }

        /// <summary>
        /// Creates an HTTP content from a JSON of the specific object.
        /// </summary>
        /// <param name="value">The object.</param>
        /// <param name="options">An optional serialization options.</param>
        /// <returns>The HTTP content a JSON of the specific object.</returns>
        public static StringContent CreateJsonContent(object value, JsonSerializerOptions options = null)
        {
            if (value == null) return null;
            var json = ToJson(value, options);
            if (json == null) return null;
            return new StringContent(json, Encoding.UTF8, MIME);
        }

        /// <summary>
        /// Creates an HTTP content from a JSON of the specific object.
        /// </summary>
        /// <param name="value">The object.</param>
        /// <param name="options">An optional serialization options.</param>
        /// <returns>The HTTP content a JSON of the specific object.</returns>
        public static StringContent CreateJsonContent(object value, DataContractJsonSerializerSettings options)
        {
            if (value == null) return null;
            var json = ToJson(value, options);
            if (json == null) return null;
            return new StringContent(json, Encoding.UTF8, MIME);
        }

        /// <summary>
        /// Adds a JSON string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
        /// </summary>
        /// <param name="content">The HTTP content of multipart form data.</param>
        /// <param name="key">The property key.</param>
        /// <param name="value">The JSON object value.</param>
        /// <return>The HTTP content to add.</return>
        public static StringContent Add(this MultipartFormDataContent content, string key, JsonObject value)
        {
            if (content == null || string.IsNullOrWhiteSpace(key)) return null;
            var c = CreateJsonContent(value);
            content.Add(c, key);
            return c;
        }

        /// <summary>
        /// Adds a JSON string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
        /// </summary>
        /// <param name="content">The HTTP content of multipart form data.</param>
        /// <param name="key">The property key.</param>
        /// <param name="value">The JSON object value.</param>
        /// <return>The HTTP content to add.</return>
        public static StringContent Add(this MultipartFormDataContent content, string key, JsonArray value)
        {
            if (content == null || string.IsNullOrWhiteSpace(key)) return null;
            var c = CreateJsonContent(value);
            content.Add(c, key);
            return c;
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        internal static bool Equals(IJsonValue leftValue, IJsonValue rightValue)
        {
            if (leftValue is null || leftValue.ValueKind == JsonValueKind.Null || leftValue.ValueKind == JsonValueKind.Undefined)
            {
                return rightValue is null || rightValue.ValueKind == JsonValueKind.Null || rightValue.ValueKind == JsonValueKind.Undefined;
            }

            if (rightValue is null || rightValue.ValueKind != leftValue.ValueKind) return false;
            return leftValue.Equals(rightValue);
        }

        internal static IJsonValueResolver ConvertValue(IJsonValue value, IJsonValue thisInstance = null)
        {
            if (value is null || value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined) return Null;
            if (value is JsonObject obj)
            {
                if (ReferenceEquals(obj, thisInstance)) return obj.Clone();
                return obj;
            }

            if (value is JsonArray arr)
            {
                if (ReferenceEquals(arr, thisInstance)) return arr.Clone();
                return arr;
            }

            if (value is JsonString || value is JsonInteger || value is JsonDouble || value is JsonBoolean) return value as IJsonValueResolver;
            if (value.ValueKind == JsonValueKind.True) return JsonBoolean.True;
            if (value.ValueKind == JsonValueKind.False) return JsonBoolean.False;
            if (value.ValueKind == JsonValueKind.String)
            {
                if (value is IJsonValue<string> str) return new JsonString(str.Value);
                if (value is IJsonValue<DateTime> date) return new JsonString(date.Value);
                if (value is IJsonValue<Guid> guid) return new JsonString(guid.Value);
            }

            if (value.ValueKind == JsonValueKind.Number)
            {
                if (value is IJsonValue<int> int32) return new JsonInteger(int32.Value);
                if (value is IJsonValue<long> int64) return new JsonInteger(int64.Value);
                if (value is IJsonValue<short> int16) return new JsonInteger(int16.Value);
                if (value is IJsonValue<double> d) return new JsonDouble(d.Value);
                if (value is IJsonValue<float> f) return new JsonDouble(f.Value);
                if (value is IJsonValue<decimal> fd) return new JsonDouble((double)fd.Value);
                if (value is IJsonValue<bool> b) return b.Value ? JsonBoolean.True : JsonBoolean.False;
                if (value is IJsonValue<uint> uint32) return new JsonInteger(uint32.Value);
                if (value is IJsonValue<ulong> uint64) return new JsonDouble(uint64.Value);
                if (value is IJsonValue<ushort> uint16) return new JsonInteger(uint16.Value);
                if (value is IJsonValue<DateTime> date) return new JsonInteger(date.Value);
                var s = value.ToString();
                if (long.TryParse(s, out var l)) return new JsonInteger(l);
                if (double.TryParse(s, out var db)) return new JsonDouble(db);
                return Null;
            }

            return Null;
        }

        /// <summary>
        /// Special characters of YAML.
        /// </summary>
        internal static readonly char[] YamlSpecialChars = new[] { ':', '\r', '\n', '\\', '\'', '\"', '\t', ' ', '#', '.', '[', '{', '\\', '/', '@' };

        /// <summary>
        /// Gets the indent string.
        /// </summary>
        /// <param name="indentStyle">The indent style.</param>
        /// <param name="indentLevel">The current indent level.</param>
        /// <returns>A string.</returns>
        internal static string GetString(JsonIndentStyles indentStyle, int indentLevel = 1)
        {
            if (indentLevel < 1) return indentLevel == 0 ? string.Empty : null;
            var str = indentStyle switch
            {
                JsonIndentStyles.Minified => string.Empty,
                JsonIndentStyles.Empty => string.Empty,
                JsonIndentStyles.Tab => "\t",
                JsonIndentStyles.Space => " ",
                JsonIndentStyles.Compact => "  ",
                JsonIndentStyles.Wide => "        ",
                _ => "    "
            };
            if (indentLevel < 1) return str;
            var sb = new StringBuilder(str);
            for (var i = 1; i < indentLevel; i++)
            {
                sb.Append(str);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Serializes an object into JSON format.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="options">The optional serializer settings.</param>
        /// <returns>A JSON string.</returns>
        internal static string ToJson(object obj, JsonSerializerOptions options = null)
        {
            return ToJson(obj, (o, t) =>
            {
                return JsonSerializer.Serialize(o, t, options);
            });
        }

        /// <summary>
        /// Serializes an object into JSON format.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="options">The optional serializer settings.</param>
        /// <returns>A JSON string.</returns>
        internal static string ToJson(object obj, DataContractJsonSerializerSettings options)
        {
            return ToJson(obj, (o, t) =>
            {
                var serializer = options != null ? new DataContractJsonSerializer(t, options) : new DataContractJsonSerializer(t);
                using var stream = new MemoryStream();
                serializer.WriteObject(stream, o);
                stream.Position = 0;
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return Encoding.UTF8.GetString(bytes);
            });
        }

        /// <summary>
        /// Converts a secure string to unsecure string.
        /// </summary>
        /// <param name="value">The secure string to convert.</param>
        /// <returns>The unsecure string.</returns>
        internal static string ToUnsecureString(this SecureString value)
        {
            if (value == null) return null;
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Serializes an object into JSON format.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="converter">The fallback converter.</param>
        /// <returns>A JSON string.</returns>
        private static string ToJson(object obj, Func<object, Type, string> converter)
        {
            if (obj == null) return "null";
            var t = obj.GetType();
            if (obj is JsonDocument jsonDoc)
            {
                return jsonDoc.RootElement.ToString();
            }

            if (obj is JsonElement jsonEle)
            {
                return jsonEle.ToString();
            }

            if (t.FullName.StartsWith("Newtonsoft.Json.Linq.J", StringComparison.InvariantCulture))
            {
                if (t.FullName.Equals("Newtonsoft.Json.Linq.JObject", StringComparison.InvariantCulture)
                    || t.FullName.Equals("Newtonsoft.Json.Linq.JArray", StringComparison.InvariantCulture))
                    return obj.ToString();
            }

            if (t.IsClass)
            {
                try
                {
                    var method = t.GetMethod("ToJsonString", Type.EmptyTypes);
                    if (method != null && !method.IsStatic)
                    {
                        var str = method.Invoke(obj, null);
                        if (str != null && str.GetType() == typeof(string)) return (string)str;
                    }
                }
                catch (AmbiguousMatchException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (NotSupportedException)
                {
                }
                catch (TargetException)
                {
                }
            }

            if (obj is IEnumerable<KeyValuePair<string, string>> col)
            {
                var str = new StringBuilder("{");
                foreach (var kvp in col)
                {
                    str.AppendFormat("\"{0}\":\"{1}\",", JsonString.ToJson(kvp.Key), JsonString.ToJson(kvp.Value));
                }

                str.Remove(str.Length - 1, 1);
                str.Append("}");
                return str.ToString();
            }

            if (t == typeof(string)) return JsonString.ToJson(obj.ToString());
            if (t.FullName.Equals("Trivial.Net.HttpUri", StringComparison.InvariantCulture))
            {
                return JsonString.ToJson(obj.ToString());
            }

            if (obj is Uri uri)
            {
                try
                {
                    return JsonString.ToJson(uri.OriginalString);
                }
                catch (InvalidOperationException)
                {
                    return JsonString.ToJson(uri.ToString());
                }
            }

            if (obj is IJsonValue)
            {
                return obj.ToString();
            }

            if (t.IsValueType)
            {
                if (obj is bool b)
                    return b ? "true" : "false";
                if (obj is int i32)
                    return i32.ToString("g", CultureInfo.InvariantCulture);
                if (obj is long i64)
                    return i64.ToString("g", CultureInfo.InvariantCulture);
                if (obj is float f1)
                    return f1.ToString("g", CultureInfo.InvariantCulture);
                if (obj is uint i32u)
                    return i32u.ToString("g", CultureInfo.InvariantCulture);
                if (obj is uint i64u)
                    return i64u.ToString("g", CultureInfo.InvariantCulture);
                if (obj is DateTime d)
                    return JsonString.ToJson(d.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                if (obj is DateTimeOffset dto)
                    return JsonString.ToJson(dto.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
                if (obj is double f2)
                    return f2.ToString("g", CultureInfo.InvariantCulture);
                if (obj is TimeSpan ts)
                    return ts.TotalSeconds.ToString("g", CultureInfo.InvariantCulture);

                if (t == typeof(Guid))
                    return JsonString.ToJson(obj.ToString());
                if (t == typeof(byte)
                    || t == typeof(short)
                    || t == typeof(ushort))
                    return obj.ToString();
            }

            if (t == typeof(DBNull)) return "null";
            return converter(obj, t);
        }
    }
}
