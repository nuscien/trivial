using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Reflection;
using Trivial.Web;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a specific JSON object.
    /// </summary>
    public class JsonObject : IJsonComplex, IJsonValueResolver, IDictionary<string, IJsonValue>, IDictionary<string, IJsonValueResolver>, IReadOnlyDictionary<string, IJsonValue>, IReadOnlyDictionary<string, IJsonValueResolver>, IEquatable<JsonObject>, IEquatable<IJsonValue>
    {
        private readonly IDictionary<string, IJsonValueResolver> store = new Dictionary<string, IJsonValueResolver>();

        /// <summary>
        /// Initializes a new instance of the JsonObject class.
        /// </summary>
        public JsonObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonObject class.
        /// </summary>
        /// <param name="copy">Properties to initialzie.</param>
        private JsonObject(IDictionary<string, IJsonValueResolver> copy)
        {
            if (copy == null) return;
            foreach (var ele in copy)
            {
                store[ele.Key] = ele.Value;
            }
        }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind => JsonValueKind.Object;

        /// <summary>
        /// Gets the number of elements contained in the System.Collections.Generic.ICollection`1
        /// </summary>
        public int Count => store.Count;

        /// <summary>
        /// Gets a collection containing the property keys of the object.
        /// </summary>
        public ICollection<string> Keys => store.Keys;

        /// <summary>
        /// Gets a collection containing the property keys of the object.
        /// </summary>
        IEnumerable<string> IReadOnlyDictionary<string, IJsonValue>.Keys => store.Keys;

        /// <summary>
        /// Gets a collection containing the property keys of the object.
        /// </summary>
        IEnumerable<string> IReadOnlyDictionary<string, IJsonValueResolver>.Keys => store.Keys;

        /// <summary>
        /// Gets a collection containing the property values of the object.
        /// </summary>
        public ICollection<IJsonValueResolver> Values => store.Values;

        /// <summary>
        /// Gets a collection containing the property values of the object.
        /// </summary>
        ICollection<IJsonValue> IDictionary<string, IJsonValue>.Values => store.Select(ele => (ele.Value ?? JsonValues.Null) as IJsonValue).ToList();

        /// <summary>
        /// Gets a collection containing the property values of the object.
        /// </summary>
        IEnumerable<IJsonValue> IReadOnlyDictionary<string, IJsonValue>.Values => store.Values;

        /// <summary>
        /// Gets a collection containing the property values of the object.
        /// </summary>
        IEnumerable<IJsonValueResolver> IReadOnlyDictionary<string, IJsonValueResolver>.Values => store.Values;

        /// <summary>
        /// Gets a value indicating whether the JSON object is read-only.
        /// </summary>
        public bool IsReadOnly => store.IsReadOnly;

        /// <summary>
        /// Gets or sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public IJsonValueResolver this[string key]
        {
            get => GetValue(key);
            set => store[key] = JsonValues.ConvertValue(value, this);
        }

        /// <summary>
        /// Gets or sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        IJsonValue IDictionary<string, IJsonValue>.this[string key]
        {
            get => GetValue(key);
            set => store[key] = JsonValues.ConvertValue(value, this);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        IJsonValue IReadOnlyDictionary<string, IJsonValue>.this[string key]
        {
            get => GetValue(key);
        }

        /// <summary>
        /// Determines the property value of the specific key is null.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>true if there is no such key or the property value is null; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        public bool IsNull(string key)
        {
            AssertKey(key);
            var value = store[key];
            return value.ValueKind == JsonValueKind.Null;
        }

        /// <summary>
        /// Determines the property value of the specific key is null, undefined or nonexisted.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>true if there is no such key or the property value is null; otherwise, false.</returns>
        public bool IsNullOrUndefined(string key)
        {
            if (!store.TryGetValue(key, out var value)) return true;
            return value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined;
        }

        /// <summary>
        /// Determines whether it contains an property value with the specific key.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>true if there is no such key; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">key was null, empty or consists only of white-space characters.</exception>
        public bool ContainsKey(string key)
        {
            AssertKey(key);
            return store.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether it contains an property value with the specific key.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>true if there is no such key; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">key was null, empty or consists only of white-space characters.</exception>
        public bool ContainsKey(ReadOnlySpan<char> key)
        {
            if (key == null) throw new ArgumentNullException("key", "key should not be null.");
            return ContainsKey(key.ToString());
        }

        /// <summary>
        /// Gets the raw value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public string GetRawText(string key)
        {
            AssertKey(key);
            var data = store[key];
            if (data is null) return null;
            return data.ToString();
        }

        /// <summary>
        /// Gets the raw value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public string GetRawText(ReadOnlySpan<char> key)
        {
            if (key == null) throw new ArgumentNullException("key", "key should not be null.");
            return GetRawText(key.ToString());
        }

        /// <summary>
        /// Gets the value kind of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="strictMode">true if enable strict mode; otherwise, false, to return undefined for non-existing.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        public JsonValueKind GetValueKind(string key, bool strictMode = false)
        {
            if (strictMode) AssertKey(key);
            if (string.IsNullOrWhiteSpace(key) || !store.TryGetValue(key, out var data))
            {
                if (strictMode) throw new ArgumentOutOfRangeException("key does not exist.");
                return JsonValueKind.Undefined;
            }

            if (data is null) return JsonValueKind.Null;
            return data.ValueKind;
        }

        /// <summary>
        /// Gets the value kind of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="strictMode">true if enable strict mode; otherwise, false, to return undefined for non-existing.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        public JsonValueKind GetValueKind(ReadOnlySpan<char> key, bool strictMode = false)
        {
            if (key == null) throw new ArgumentNullException("key", "key should not be null.");
            return GetValueKind(key.ToString(), strictMode);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="strictMode">true if want to convert to string only when it is a string; otherwise, false.</param>
        /// <returns>The value. It will be null if the value is null.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public string GetStringValue(string key, bool strictMode = false)
        {
            AssertKey(key);
            var data = store[key];
            if (data is null)
            {
                return null;
            }

            if (data is JsonString str)
            {
                return str.Value;
            }

            if (!strictMode) return data.ValueKind switch
            {
                JsonValueKind.True => JsonBoolean.TrueString,
                JsonValueKind.False => JsonBoolean.TrueString,
                JsonValueKind.Number => data.ToString(),
                _ => throw new InvalidOperationException($"The value kind of property {key} should be string but it is {data.ValueKind.ToString().ToLowerInvariant()}.")
            };
            throw new InvalidOperationException($"The value kind of property {key} should be string but it is {data.ValueKind.ToString().ToLowerInvariant()}.");
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="FormatException">The value is not in a recognized format.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public Guid GetGuidValue(string key)
        {
            return Guid.Parse(GetStringValue(key));
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        /// <exception cref="FormatException">The value is not of the correct format.</exception>
        /// <exception cref="OverflowException">represents a number that is less than the minimum number or greater than the maximum number.</exception>
        public uint GetUInt32Value(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonInteger>(key, out var v)) return (uint)v;
            if (TryGetJsonValue<JsonDouble>(key, out var f)) return (uint)f;
            var p = GetJsonValue<JsonString>(key, JsonValueKind.Number);
            return uint.Parse(p.Value);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        /// <exception cref="FormatException">The value is not of the correct format.</exception>
        /// <exception cref="OverflowException">represents a number that is less than the minimum number or greater than the maximum number.</exception>
        public int GetInt32Value(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonInteger>(key, out var v)) return (int)v;
            if (TryGetJsonValue<JsonDouble>(key, out var f)) return (int)f;
            var p = GetJsonValue<JsonString>(key, JsonValueKind.Number);
            return int.Parse(p.Value);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        /// <exception cref="FormatException">The value is not of the correct format.</exception>
        /// <exception cref="OverflowException">represents a number that is less than the minimum number or greater than the maximum number.</exception>
        public long GetInt64Value(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonInteger>(key, out var v)) return v.Value;
            if (TryGetJsonValue<JsonDouble>(key, out var f)) return (long)f;
            var p = GetJsonValue<JsonString>(key, JsonValueKind.Number);
            return long.Parse(p.Value);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        /// <exception cref="FormatException">The value is not of the correct format.</exception>
        /// <exception cref="OverflowException">represents a number that is less than the minimum number or greater than the maximum number.</exception>
        public float GetSingleValue(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonDouble>(key, out var v)) return (float)v;
            if (TryGetJsonValue<JsonInteger>(key, out var f)) return (float)f;
            var p = GetJsonValue<JsonString>(key, JsonValueKind.Number);
            return float.Parse(p.Value);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public double GetDoubleValue(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonDouble>(key, out var v)) return v.Value;
            if (TryGetJsonValue<JsonInteger>(key, out var f)) return (float)f;
            var p = GetJsonValue<JsonString>(key, JsonValueKind.Number);
            return double.Parse(p.Value);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public bool GetBooleanValue(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonBoolean>(key, out var v)) return v.Value;
            var p = GetJsonValue<JsonString>(key);
            return p.Value?.ToLower() switch
            {
                JsonBoolean.TrueString => true,
                JsonBoolean.FalseString => false,
                _ => throw new InvalidOperationException($"The value kind of property {key} should be boolean but it is string.")
            };
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public JsonObject GetObjectValue(string key)
        {
            AssertKey(key);
            return GetJsonValue<JsonObject>(key, JsonValueKind.Object, true);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="subKey">The sub-key of the previous property.</param>
        /// <param name="keyPath">The additional property key path.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">Cannot get the property value.</exception>
        public JsonObject GetObjectValue(string key, string subKey, params string[] keyPath)
        {
            var path = new List<string>
            {
                key,
                subKey
            };
            path.AddRange(keyPath);
            return GetObjectValue(path);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="keyPath">The property key path.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">Cannot get the property value.</exception>
        public JsonObject GetObjectValue(IEnumerable<string> keyPath)
        {
            var result = GetValue(keyPath);
            if (result is null) return null;
            if (result is JsonObject jObj) return jObj;
            throw new InvalidOperationException($"The property {string.Join(".", keyPath)} is not a JSON object.", new InvalidCastException("The result is not a JSON object."));
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public JsonArray GetArrayValue(string key)
        {
            AssertKey(key);
            return GetJsonValue<JsonArray>(key, JsonValueKind.Array);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="useUnixTimestampsFallback">true if use Unix timestamp to convert if the value is a number; otherwise, false, to use JavaScript date ticks fallback.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="FormatException">The value is not in a recognized format.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public DateTime GetDateTimeValue(string key, bool useUnixTimestampsFallback = false)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonString>(key, out var s))
            {
                var date = WebFormat.ParseDate(s.Value);
                if (date.HasValue) return date.Value;
                throw new InvalidOperationException("The value is not a date time.");
            }

            var num = GetJsonValue<JsonInteger>(key);
            return useUnixTimestampsFallback ? WebFormat.ParseUnixTimestamp(num.Value) : WebFormat.ParseDate(num.Value);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="FormatException">The value is not encoded as Base64 text and hence cannot be decoded to bytes.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public byte[] GetBytesFromBase64(string key)
        {
            var str = GetStringValue(key);
            if (string.IsNullOrEmpty(str)) return Array.Empty<byte>();
            return Convert.FromBase64String(str);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="ArgumentException">The type is not an System.Enum. -or- the value is either an empty string or only contains white space.  -or- value is a name, but not one of the named constants defined for the enumeration.</exception>
        /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public T GetEnumValue<T>(string key) where T : struct, Enum
        {
            if (TryGetInt32Value(key, out var v)) return (T)(object)v;
            var str = GetStringValue(key);
            return ObjectConvert.ParseEnum<T>(str);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="ArgumentException">The type is not an System.Enum. -or- the value is either an empty string or only contains white space.  -or- value is a name, but not one of the named constants defined for the enumeration.</exception>
        /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public T GetEnumValue<T>(string key, bool ignoreCase) where T : struct, Enum
        {
            if (TryGetInt32Value(key, out var v)) return (T)(object)v;
            var str = GetStringValue(key);
            return ObjectConvert.ParseEnum<T>(str, ignoreCase);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        public IJsonValueResolver GetValue(string key)
        {
            AssertKey(key);
            return store[key] ?? JsonValues.Null;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="keyPath">The property key path.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">Cannot get the property value.</exception>
        public IJsonValueResolver GetValue(IEnumerable<string> keyPath)
        {
            if (keyPath == null) return this;
            IJsonValueResolver json = this;
            var path = new StringBuilder();
            foreach (var key in keyPath)
            {
                if (string.IsNullOrEmpty(key)) continue;
                if (json is null)
                {
                    path.Remove(0, 1);
                    var message = $"Cannot get property {key} because property {path} is null.";
                    throw new InvalidOperationException(
                        message,
                        new ArgumentException(message, nameof(keyPath)));
                }

                path.Append(".");
                path.Append(key);
                if (json is JsonObject jObj)
                {
                    if (!jObj.ContainsKey(key))
                    {
                        path.Remove(0, 1);
                        var message = $"Cannot get property {key} because property {path} is not a JSON object.";
                        throw new InvalidOperationException(
                            message,
                            new ArgumentOutOfRangeException(nameof(keyPath), message));
                    }

                    json = jObj.TryGetValue(key);
                    continue;
                }

                if (!(json is JsonArray jArr) || !int.TryParse(key, out var i))
                {
                    path.Remove(0, 1);
                    var message = $"Cannot get property {key} because property {path} is not a JSON object.";
                    throw new InvalidOperationException(
                        message,
                        new ArgumentOutOfRangeException(nameof(keyPath), message));
                }

                if (!jArr.Contains(i))
                {
                    path.Remove(0, 1);
                    var message = $"Cannot get item {key} because property {path} is not a JSON array.";
                    throw new InvalidOperationException(
                        message,
                        new ArgumentOutOfRangeException(nameof(keyPath), message));
                }

                json = jArr.TryGetValue(i);
            }

            return json;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="subKey">The sub-key of the previous property.</param>
        /// <param name="keyPath">The additional property key path.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">Cannot get the property value.</exception>
        public IJsonValueResolver GetValue(string key, string subKey, params string[] keyPath)
        {
            var path = new List<string>
            {
                key,
                subKey
            };
            path.AddRange(keyPath);
            return GetValue(path);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        public IJsonValueResolver GetValue(ReadOnlySpan<char> key)
        {
            return GetValue(key.ToString());
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is an object.");

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetValue(int index, out IJsonValueResolver result)
        {
            result = default;
            return false;
        }

#if !NETSTANDARD2_0
        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is an object.");

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetValue(Index index, out IJsonValueResolver result)
        {
            result = default;
            return false;
        }
#endif

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>A string.</returns>
        public string TryGetStringValue(string key)
        {
            if (!store.TryGetValue(key, out var data))
            {
                return null;
            }

            if (data is null)
            {
                return null;
            }

            if (data is JsonString str)
            {
                return str.Value;
            }

            return data.ValueKind switch
            {
                JsonValueKind.True => JsonBoolean.TrueString,
                JsonValueKind.False => JsonBoolean.TrueString,
                JsonValueKind.Number => data.ToString(),
                _ => null
            };
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetStringValue(string key, out string result)
        {
            if (!store.TryGetValue(key, out var data))
            {
                result = null;
                return false;
            }

            if (data is null)
            {
                result = null;
                return true;
            }

            if (data is JsonString str)
            {
                result = str.Value;
                return true;
            }

            result = data.ValueKind switch
            {
                JsonValueKind.True => JsonBoolean.TrueString,
                JsonValueKind.False => JsonBoolean.TrueString,
                JsonValueKind.Number => data.ToString(),
                _ => null
            };

            return result != null;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public Guid? TryGetGuidValue(string key)
        {
            if (!TryGetStringValue(key, out var str) || string.IsNullOrWhiteSpace(str) || !Guid.TryParse(str, out var result))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetGuidValue(string key, out Guid result)
        {
            if (!TryGetStringValue(key, out var str) || string.IsNullOrWhiteSpace(str) || !Guid.TryParse(str, out result))
            {
                result = default;
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public uint? TryGetUInt32Value(string key)
        {
            if (TryGetJsonValue<JsonInteger>(key, out var p1)) return (uint)p1;
            if (TryGetJsonValue<JsonDouble>(key, out var p2)) return (uint)p2;
            var str = TryGetStringValue(key);
            if (string.IsNullOrWhiteSpace(str) || !uint.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetUInt32Value(string key, out uint result)
        {
            var v = TryGetUInt32Value(key);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public int? TryGetInt32Value(string key)
        {
            if (TryGetJsonValue<JsonInteger>(key, out var p1)) return (int)p1;
            if (TryGetJsonValue<JsonDouble>(key, out var p2)) return (int)p2;
            var str = TryGetStringValue(key);
            if (string.IsNullOrWhiteSpace(str) || !int.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetInt32Value(string key, out int result)
        {
            var v = TryGetInt32Value(key);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public long? TryGetInt64Value(string key)
        {
            if (TryGetJsonValue<JsonInteger>(key, out var p1)) return p1.Value;
            if (TryGetJsonValue<JsonDouble>(key, out var p2)) return (long)p2;
            var str = TryGetStringValue(key);
            if (string.IsNullOrWhiteSpace(str) || !long.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetInt64Value(string key, out long result)
        {
            var v = TryGetInt64Value(key);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public float? TryGetFloatValue(string key)
        {
            if (TryGetJsonValue<JsonDouble>(key, out var p1)) return (float)p1;
            if (TryGetJsonValue<JsonInteger>(key, out var p2)) return (float)p2;
            var str = TryGetStringValue(key);
            if (string.IsNullOrWhiteSpace(str) || !float.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetFloatValue(string key, out float result)
        {
            var v = TryGetFloatValue(key);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public double? TryGetDoubleValue(string key)
        {
            if (TryGetJsonValue<JsonDouble>(key, out var p1)) return p1.Value;
            if (TryGetJsonValue<JsonInteger>(key, out var p2)) return (double)p2;
            var str = TryGetStringValue(key);
            if (string.IsNullOrWhiteSpace(str) || !double.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetDoubleValue(string key, out double result)
        {
            var v = TryGetDoubleValue(key);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public bool? TryGetBooleanValue(string key)
        {
            if (TryGetJsonValue<JsonBoolean>(key, out var p)) return p.Value;
            var str = TryGetStringValue(key);
            if (string.IsNullOrWhiteSpace(str) || !bool.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetBooleanValue(string key, out bool result)
        {
            var v = TryGetBooleanValue(key);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        public JsonObject TryGetObjectValue(string key)
        {
            if (string.IsNullOrEmpty(key)) return this;
            if (TryGetJsonValue<JsonObject>(key, out var p)) return p;
            return null;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="subKey">The sub-key of the previous property.</param>
        /// <param name="keyPath">The additional property key path.</param>
        /// <returns>The value.</returns>
        public JsonObject TryGetObjectValue(string key, string subKey, params string[] keyPath)
        {
            var json = TryGetObjectValue(key);
            if (json is null) return null;
            if (!string.IsNullOrWhiteSpace(subKey)) json = TryGetObjectValueByProperty(json, subKey);
            foreach (var k in keyPath)
            {
                if (json is null) return null;
                json = TryGetObjectValueByProperty(json, k);
            }

            return json;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="keyPath">The property key path.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public JsonObject TryGetObjectValue(IEnumerable<string> keyPath)
        {
            var json = this;
            if (keyPath == null) return json;
            foreach (var k in keyPath)
            {
                if (json is null) throw new InvalidOperationException($"Cannot get property {k} of null.");
                json = json.TryGetObjectValue(k);
            }

            return json;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetObjectValue(string key, out JsonObject result)
        {
            var v = TryGetObjectValue(key);
            result = v;
            return !(v is null) || IsNull(key);
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="keyPath">The property key path.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetObjectValue(IEnumerable<string> keyPath, out JsonObject result)
        {
            var json = this;
            if (keyPath == null)
            {
                result = json;
                return false;
            }

            foreach (var k in keyPath)
            {
                if (json is null)
                {
                    result = null;
                    return false;
                }

                json = json.TryGetObjectValue(k);
            }

            result = json;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        public JsonArray TryGetArrayValue(string key)
        {
            if (TryGetJsonValue<JsonArray>(key, out var p)) return p;
            return null;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetArrayValue(string key, out JsonArray result)
        {
            var v = TryGetArrayValue(key);
            result = v;
            return !(v is null);
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="useUnixTimestampsFallback">true if use Unix timestamp to convert if the value is a number; otherwise, false, to use JavaScript date ticks fallback.</param>
        /// <returns>The value.</returns>
        public DateTime? TryGetDateTimeValue(string key, bool useUnixTimestampsFallback = false)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonString>(key, out var s))
            {
                var date = WebFormat.ParseDate(s.Value);
                return date;
            }

            if (!TryGetJsonValue<JsonInteger>(key, out var num)) return null;
            return useUnixTimestampsFallback ? WebFormat.ParseUnixTimestamp(num.Value) : WebFormat.ParseDate(num.Value);
        }

#if !NETSTANDARD2_0
        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="bytes">The result.</param>
        /// <param name="bytesWritten">The count of bytes written.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetBytesFromBase64(string key, Span<byte> bytes, out int bytesWritten)
        {
            var str = GetStringValue(key);
            if (string.IsNullOrEmpty(str))
            {
                bytesWritten = 0;
                return false;
            }

            return Convert.TryFromBase64String(str, bytes, out bytesWritten);
        }
#endif

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        public T? TryGetEnumValue<T>(string key) where T : struct, Enum
        {
            if (TryGetInt32Value(key, out var v)) return (T)(object)v;
            var str = GetStringValue(key);
            if (Enum.TryParse<T>(str, out var result)) return result;
            return null;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
        /// <returns>The value.</returns>
        public T? TryGetEnumValue<T>(string key, bool ignoreCase) where T : struct, Enum
        {
            if (TryGetInt32Value(key, out var v)) return (T)(object)v;
            var str = GetStringValue(key);
            if (Enum.TryParse<T>(str, ignoreCase, out var result)) return result;
            return null;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result output.</param>
        /// <returns>true if parse succeeded; otherwise, false.</returns>
        public bool TryGetEnumValue<T>(string key, out T result) where T : struct, Enum
        {
            var r = TryGetEnumValue<T>(key);
            if (r == null)
            {
                result = default;
                return false;
            }

            result = r.Value;
            return true;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
        /// <param name="result">The result output.</param>
        /// <returns>true if parse succeeded; otherwise, false.</returns>
        public bool TryGetEnumValue<T>(string key, bool ignoreCase, out T result) where T : struct, Enum
        {
            var r = TryGetEnumValue<T>(key, ignoreCase);
            if (r == null)
            {
                result = default;
                return false;
            }

            result = r.Value;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        public IJsonValueResolver TryGetValue(string key)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(key) && store.TryGetValue(key, out var value)) return value ?? JsonValues.Null;
            }
            catch (ArgumentException)
            {
            }

            return default;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        public IJsonValueResolver TryGetValue(ReadOnlySpan<char> key)
        {
            if (key == null) return null;
            return TryGetValue(key.ToString());
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetValue(string key, out IJsonValueResolver result)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(key) && store.TryGetValue(key, out var value))
                {
                    result = value ?? JsonValues.Null;
                    return true;
                }
            }
            catch (ArgumentException)
            {
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        public bool TryGetValue(ReadOnlySpan<char> key, out IJsonValueResolver result)
        {
            if (key == null)
            {
                result = default;
                return false;
            }

            return TryGetValue(key.ToString(), out result);
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="keyPath">The property key path.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetValue(IEnumerable<string> keyPath, out IJsonValueResolver result)
        {
            if (keyPath == null)
            {
                result = this;
                return true;
            }

            IJsonValueResolver json = this;
            foreach (var key in keyPath)
            {
                if (string.IsNullOrEmpty(key)) continue;
                if (json is null)
                {
                    result = null;
                    return false;
                }

                if (json is JsonObject jObj)
                {
                    if (!jObj.ContainsKey(key))
                    {
                        result = null;
                        return false;
                    }

                    json = jObj.TryGetValue(key);
                    continue;
                }

                if (!(json is JsonArray jArr) || !int.TryParse(key, out var i))
                {
                    result = null;
                    return false;
                }

                if (!jArr.Contains(i))
                {
                    result = null;
                    return false;
                }

                json = jArr.TryGetValue(i);
            }

            result = json;
            return true;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="keyPath">The property key path.</param>
        /// <returns>The value.</returns>
        public IJsonValueResolver TryGetValue(IEnumerable<string> keyPath)
        {
            if (TryGetValue(keyPath, out var result)) return result;
            return default;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        bool IDictionary<string, IJsonValue>.TryGetValue(string key, out IJsonValue result)
        {
            var v = TryGetValue(key);
            result = v;
            return !(v is null);
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        bool IReadOnlyDictionary<string, IJsonValue>.TryGetValue(string key, out IJsonValue result)
        {
            var v = TryGetValue(key);
            result = v;
            return !(v is null);
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="subKey">The sub-key of the previous property.</param>
        /// <param name="keyPath">The additional property key path.</param>
        /// <returns>The value.</returns>
        public IJsonValueResolver TryGetValue(string key, string subKey, params string[] keyPath)
        {
            var path = new List<string>
            {
                key,
                subKey
            };
            if (keyPath != null) path.AddRange(keyPath);
            return TryGetValue(path);
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="keyPath">The additional property key path.</param>
        /// <returns>The value.</returns>
        public T TryGetValue<T>(params string[] keyPath) where T : IJsonValueResolver
        {
            return TryGetValue(keyPath) is T result ? result : default;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="keyPath">The additional property key path.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetValue<T>(IEnumerable<string> keyPath, out T result) where T : IJsonValueResolver
        {
            if (!TryGetValue(keyPath, out var r))
            {
                result = default;
                return false;
            }

            if (r is T v)
            {
                result = v;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Removes property of the specific key.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public bool Remove(string key)
        {
            AssertKey(key);
            return store.Remove(key);
        }

        /// <summary>
        /// Removes property of the specific key.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>true if the element is successfully removed; otherwise, false. This method also returns false if key was not found.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public bool Remove(ReadOnlySpan<char> key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "key was null.");
            return store.Remove(key.ToString());
        }

        /// <summary>
        /// Removes all properties of the specific key.
        /// </summary>
        /// <param name="keys">The property key collection.</param>
        /// <returns>The count of value removed.</returns>
        public int Remove(IEnumerable<string> keys)
        {
            var count = 0;
            foreach (var key in keys)
            {
                if (string.IsNullOrEmpty(key)) continue;
                if (store.Remove(key)) count++;
            }

            return count;
        }

        /// <summary>
        /// Sets null to the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetNullValue(string key)
        {
            AssertKey(key);
            store[key] = JsonValues.Null;
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, string value)
        {
            AssertKey(key);
            store[key] = new JsonString(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, SecureString value)
        {
            AssertKey(key);
            store[key] = new JsonString(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetFormatValue(string key, string value, params object[] args)
        {
            AssertKey(key);
            store[key] = new JsonString(string.Format(value, args));
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, Guid value)
        {
            AssertKey(key);
            store[key] = new JsonString(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, DateTime value)
        {
            AssertKey(key);
            store[key] = new JsonString(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, uint value)
        {
            AssertKey(key);
            store[key] = new JsonInteger(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, int value)
        {
            AssertKey(key);
            store[key] = new JsonInteger(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, long value)
        {
            AssertKey(key);
            store[key] = new JsonInteger(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, float value)
        {
            AssertKey(key);
            store[key] = new JsonDouble(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, double value)
        {
            AssertKey(key);
            store[key] = new JsonDouble(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, bool value)
        {
            AssertKey(key);
            store[key] = value ? JsonBoolean.True : JsonBoolean.False;
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, JsonArray value)
        {
            AssertKey(key);
            store[key] = value;
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, JsonObject value)
        {
            AssertKey(key);
            store[key] = value != this ? value : value.Clone();
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, JsonElement value)
        {
            AssertKey(key);
            store[key] = JsonValues.ToJsonValue(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(JsonProperty property)
        {
            SetValue(property.Name, property.Value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, IEnumerable<string> value)
        {
            AssertKey(key);
            var arr = new JsonArray();
            arr.AddRange(value);
            store[key] = arr;
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, IEnumerable<int> value)
        {
            AssertKey(key);
            var arr = new JsonArray();
            arr.AddRange(value);
            store[key] = arr;
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, IEnumerable<JsonObject> value)
        {
            AssertKey(key);
            var arr = new JsonArray();
            arr.AddRange(value);
            store[key] = arr;
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="inArray">An array of 8-bit unsigned integers.</param>
        /// <param name="options">A formatting options.</param>
        /// <exception cref="ArgumentNullException">The property key and bytes should not be null, empty, or consists only of white-space characters.</exception>
        public void SetBase64(string key, byte[] inArray, Base64FormattingOptions options = Base64FormattingOptions.None)
        {
            SetValue(key, Convert.ToBase64String(inArray, options));
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="bytes">The bytes to convert to base 64 string.</param>
        /// <param name="options">A formatting options.</param>
        /// <exception cref="ArgumentNullException">The property key and bytes should not be null, empty, or consists only of white-space characters.</exception>
        public void SetBase64(string key, Span<byte> bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
        {
#if NETSTANDARD2_0
            SetValue(key, Convert.ToBase64String(bytes.ToArray(), options));
#else
            SetValue(key, Convert.ToBase64String(bytes, options));
#endif
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetDateTimeStringValue(string key, DateTime value)
        {
            AssertKey(key);
            store[key] = new JsonString(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetJavaScriptDateTicksValue(string key, DateTime value)
        {
            AssertKey(key);
            store[key] = new JsonInteger(WebFormat.ParseDate(value));
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetUnixTimestampValue(string key, DateTime value)
        {
            AssertKey(key);
            store[key] = new JsonInteger(WebFormat.ParseUnixTimestamp(value));
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetWindowsFileTimeUtcValue(string key, DateTime value)
        {
            AssertKey(key);
            store[key] = new JsonInteger(value.ToFileTimeUtc());
        }

        /// <summary>
        /// Sets properties.
        /// </summary>
        /// <param name="data">Key value pairs to set.</param>
        /// <returns>The count to set.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public int SetRange(IEnumerable<KeyValuePair<string, string>> data)
        {
            var count = 0;
            if (data == null) return count;
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }

            return count;
        }

        /// <summary>
        /// Sets properties.
        /// </summary>
        /// <param name="data">Key value pairs to set.</param>
        /// <returns>The count to set.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public int SetRange(IEnumerable<KeyValuePair<string, int>> data)
        {
            var count = 0;
            if (data == null) return count;
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }

            return count;
        }

        /// <summary>
        /// Sets properties.
        /// </summary>
        /// <param name="data">Key value pairs to set.</param>
        /// <returns>The count to set.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public int SetRange(IEnumerable<KeyValuePair<string, JsonObject>> data)
        {
            var count = 0;
            if (data == null) return count;
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }

            return count;
        }

        /// <summary>
        /// Sets properties.
        /// </summary>
        /// <param name="data">Key value pairs to set.</param>
        /// <returns>The count to set.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public int SetRange(IEnumerable<KeyValuePair<string, JsonArray>> data)
        {
            var count = 0;
            if (data == null) return count;
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }

            return count;
        }

        /// <summary>
        /// Sets properties.
        /// </summary>
        /// <param name="json">Another JSON object to add.</param>
        /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
        /// <returns>The count of property added.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public int SetRange(JsonObject json, bool skipDuplicate = false)
        {
            var count = 0;
            if (json is null) return count;
            if (skipDuplicate)
            {
                if (ReferenceEquals(json, this)) return count;
                foreach (var prop in json)
                {
                    if (ContainsKey(prop.Key)) continue;
                    store[prop.Key] = prop.Value;
                    count++;
                }
            }
            else
            {
                if (ReferenceEquals(json, this)) json = json.Clone();
                foreach (var prop in json)
                {
                    store[prop.Key] = prop.Value;
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Sets properties.
        /// </summary>
        /// <param name="array">The JSON array to add.</param>
        /// <param name="propertyMapping">The mapping of index to property key; or null for convert index to string format.</param>
        /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
        /// <returns>The count of property added.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public int SetRange(JsonArray array, IEnumerable<string> propertyMapping = null, bool skipDuplicate = false)
        {
            if (array is null) return 0;
            if (propertyMapping == null)
            {
                if (skipDuplicate)
                {
                    var count2 = 0;
                    for (var i = 0; i < array.Count; i++)
                    {
                        var key = i.ToString("g");
                        if (store.ContainsKey(key)) continue;
                        store[i.ToString("g")] = array[i];
                        count2++;
                    }

                    return count2;
                }

                for (var i = 0; i < array.Count; i++)
                {
                    store[i.ToString("g")] = array[i];
                }

                return array.Count;
            }

            var keys = propertyMapping.ToList();
            var count = Math.Min(keys.Count, array.Count);
            if (skipDuplicate)
            {
                var count2 = 0;
                for (var i = 0; i < count; i++)
                {
                    var key = keys[i];
                    if (string.IsNullOrEmpty(key) || store.ContainsKey(key)) continue;
                    store[key] = array[i];
                }

                return count2;
            }

            for (var i = 0; i < count; i++)
            {
                var key = keys[i];
                if (string.IsNullOrEmpty(key)) continue;
                store[key] = array[i];
            }

            return count;
        }

        /// <summary>
        /// Sets properties.
        /// </summary>
        /// <param name="reader">The reader to read.</param>
        /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
        /// <returns>The count of property added.</returns>
        /// <exception cref="JsonException">reader contains unsupported options or value.</exception>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public int SetRange(ref Utf8JsonReader reader, bool skipDuplicate = false)
        {
            var count = 0;
            switch (reader.TokenType)
            {
                case JsonTokenType.None:        // A new reader was created and has never been read, so we need to move to the first token; or a reader has terminated and we're about to throw.
                case JsonTokenType.PropertyName:// Using a reader loop the caller has identified a property they wish to hydrate into a JsonDocument. Move to the value first.
                case JsonTokenType.Comment:     // Ignore comment.
                    if (!reader.Read()) return 0;
                    break;
                case JsonTokenType.Null:        // Nothing to read for value null.
                    return 0;
            }

            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("JSON object only.");
            while (reader.Read())
            {
                var needBreak = false;
                while (reader.TokenType == JsonTokenType.None || reader.TokenType == JsonTokenType.Comment)
                {
                    if (reader.Read()) continue;
                    needBreak = true;
                    break;
                }

                if (needBreak || reader.TokenType != JsonTokenType.PropertyName) break;
                var key = reader.GetString();
                if (!reader.Read()) break;
                while (reader.TokenType == JsonTokenType.None || reader.TokenType == JsonTokenType.Comment)
                {
                    if (reader.Read()) continue;
                    needBreak = true;
                    break;
                }

                if (needBreak || reader.TokenType == JsonTokenType.EndObject) break;
                switch (reader.TokenType)
                {
                    case JsonTokenType.Null:
                        if (!skipDuplicate || !ContainsKey(key)) SetNullValue(key);
                        break;
                    case JsonTokenType.String:
                        if (!skipDuplicate || !ContainsKey(key)) SetValue(key, reader.GetString());
                        break;
                    case JsonTokenType.Number:
                        if (!skipDuplicate || !ContainsKey(key))
                        {
                            if (reader.TryGetInt64(out var int64v)) SetValue(key, int64v);
                            else SetValue(key, reader.GetDouble());
                        }

                        break;
                    case JsonTokenType.True:
                        if (!skipDuplicate || !ContainsKey(key)) SetValue(key, true);
                        break;
                    case JsonTokenType.False:
                        if (!skipDuplicate || !ContainsKey(key)) SetValue(key, false);
                        break;
                    case JsonTokenType.StartObject:
                        if (!skipDuplicate || !ContainsKey(key))
                        {
                            var obj = new JsonObject();
                            obj.SetRange(ref reader);
                            SetValue(key, obj);
                        }

                        break;
                    case JsonTokenType.StartArray:
                        if (!skipDuplicate || !ContainsKey(key))
                        {
                            var arr = JsonArray.ParseValue(ref reader);
                            SetValue(key, arr);
                        }

                        break;
                    default:
                        count--;
                        break;
                }

                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="item">The property to add to the JSON object.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(KeyValuePair<string, IJsonValueResolver> item)
        {
            store.Add(item.Key, JsonValues.ConvertValue(item.Value, this));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="item">The property to add to the JSON object.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(KeyValuePair<string, IJsonValue> item)
        {
            store.Add(item.Key, JsonValues.ConvertValue(item.Value, this));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, IJsonValueResolver value)
        {
            store.Add(key, JsonValues.ConvertValue(value, this));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, IJsonValue value)
        {
            store.Add(key, JsonValues.ConvertValue(value, this));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, string value)
        {
            store.Add(key, new JsonString(value));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, SecureString value)
        {
            store.Add(key, new JsonString(value));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, int value)
        {
            store.Add(key, new JsonInteger(value));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, long value)
        {
            store.Add(key, new JsonInteger(value));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, uint value)
        {
            store.Add(key, new JsonInteger(value));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, float value)
        {
            store.Add(key, new JsonDouble(value));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, double value)
        {
            store.Add(key, new JsonDouble(value));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, bool value)
        {
            store.Add(key, value ? JsonBoolean.True : JsonBoolean.False);
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, DateTime value)
        {
            store.Add(key, new JsonString(value));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
        public void Add(string key, Guid value)
        {
            store.Add(key, new JsonString(value));
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void Add(string key, IEnumerable<string> value)
        {
            AssertKey(key);
            var arr = new JsonArray();
            arr.AddRange(value);
            store.Add(key, arr);
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void Add(string key, IEnumerable<int> value)
        {
            AssertKey(key);
            var arr = new JsonArray();
            arr.AddRange(value);
            store.Add(key, arr);
        }

        /// <summary>
        /// Adds a property with the provided key and value to the JSON object.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void Add(string key, IEnumerable<JsonObject> value)
        {
            AssertKey(key);
            var arr = new JsonArray();
            arr.AddRange(value);
            store.Add(key, arr);
        }

        /// <summary>
        /// Determines whether the JSON object contains a specific property.
        /// </summary>
        /// <param name="item">The property to locate in the JSON object.</param>
        /// <returns>true if property is found in the JSON object; otherwise, false.</returns>
        public bool Contains(KeyValuePair<string, IJsonValueResolver> item)
        {
            foreach (var ele in store)
            {
                if (JsonValues.Equals(ele.Value, item.Value)) return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the JSON object contains a specific property.
        /// </summary>
        /// <param name="item">The property to locate in the JSON object.</param>
        /// <returns>true if property is found in the JSON object; otherwise, false.</returns>
        public bool Contains(KeyValuePair<string, IJsonValue> item)
        {
            foreach (var ele in store)
            {
                if (JsonValues.Equals(ele.Value, item.Value)) return true;
            }

            return false;
        }

        /// <summary>
        /// Copies the elements of the JSON object to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from JSON object. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source  is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(KeyValuePair<string, IJsonValueResolver>[] array, int arrayIndex)
        {
            store.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the elements of the JSON object to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from JSON object. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source  is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(KeyValuePair<string, IJsonValue>[] array, int arrayIndex)
        {
            store.Select(ele => new KeyValuePair<string, IJsonValue>(ele.Key, ele.Value ?? JsonValues.Null)).ToList().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific property from the JSON object.
        /// </summary>
        /// <param name="item">The property to remove from the JSON object.</param>
        /// <returns>true if property was successfully removed from the JSON object; otherwise, false. This method also returns false if property is not found in the original JSON object.</returns>
        public bool Remove(KeyValuePair<string, IJsonValueResolver> item)
        {
            KeyValuePair<string, IJsonValueResolver>? kvp = null;
            foreach (var ele in store)
            {
                if (!JsonValues.Equals(ele.Value, item.Value)) continue;
                kvp = ele;
                break;
            }

            if (!kvp.HasValue) return false;
            return store.Remove(kvp.Value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific property from the JSON object.
        /// </summary>
        /// <param name="item">The property to remove from the JSON object.</param>
        /// <returns>true if property was successfully removed from the JSON object; otherwise, false. This method also returns false if property is not found in the original JSON object.</returns>
        public bool Remove(KeyValuePair<string, IJsonValue> item)
        {
            KeyValuePair<string, IJsonValueResolver>? kvp = null;
            foreach (var ele in store)
            {
                if (!JsonValues.Equals(ele.Value, item.Value)) continue;
                kvp = ele;
                break;
            }

            if (!kvp.HasValue) return false;
            return store.Remove(kvp.Value);
        }

        /// <summary>
        /// Removes all properties from the object.
        /// </summary>
        public void Clear()
        {
            store.Clear();
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(JsonObject other)
        {
            if (other is null) return false;
            if (base.Equals(other)) return true;
            if (other.Count != Count) return false;
            foreach (var prop in store)
            {
                var isNull = !other.TryGetValue(prop.Key, out var r) || r is null || r.ValueKind == JsonValueKind.Null || r.ValueKind == JsonValueKind.Undefined;
                if (prop.Value is null || prop.Value.ValueKind == JsonValueKind.Null || prop.Value.ValueKind == JsonValueKind.Undefined)
                    return isNull;
                if (isNull || !JsonValues.Equals(prop.Value, r)) return false;
            }

            return true;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue other)
        {
            if (other is null) return false;
            if (other is JsonObject json) return Equals(json);
            return false;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is JsonObject json) return Equals(json);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode() => store.GetHashCode();

        /// <summary>
        /// Writes this instance to the specified writer as a JSON value.
        /// </summary>
        /// <param name="writer">The writer to which to write this instance.</param>
        public void WriteTo(Utf8JsonWriter writer)
        {
            if (writer == null) return;
            writer.WriteStartObject();
            foreach (var prop in store)
            {
                if (prop.Value is null)
                {
                    writer.WriteNull(prop.Key);
                    continue;
                }

                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.Undefined:
                        break;
                    case JsonValueKind.Null:
                        writer.WriteNull(prop.Key);
                        break;
                    case JsonValueKind.String:
                        if (prop.Value is JsonString strJson) writer.WriteString(prop.Key, strJson.Value);
                        break;
                    case JsonValueKind.Number:
                        if (prop.Value is JsonInteger intJson) writer.WriteNumber(prop.Key, (long)intJson);
                        else if (prop.Value is JsonDouble floatJson) writer.WriteNumber(prop.Key, (double)floatJson);
                        break;
                    case JsonValueKind.True:
                        writer.WriteBoolean(prop.Key, true);
                        break;
                    case JsonValueKind.False:
                        writer.WriteBoolean(prop.Key, false);
                        break;
                    case JsonValueKind.Object:
                        writer.WritePropertyName(prop.Key);
                        if (prop.Value is JsonObject objJson) objJson.WriteTo(writer);
                        break;
                    case JsonValueKind.Array:
                        writer.WritePropertyName(prop.Key);
                        if (prop.Value is JsonArray objArr) objArr.WriteTo(writer);
                        break;
                }
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns>A JSON format string.</returns>
        public override string ToString()
        {
            var str = new StringBuilder("{");
            foreach (var prop in store)
            {
                str.Append(JsonString.ToJson(prop.Key));
                str.Append(':');
                if (prop.Value is null)
                {
                    str.Append("null,");
                    continue;
                }

                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Null:
                        str.Append("null");
                        break;
                    default:
                        str.Append(prop.Value.ToString());
                        break;
                }

                str.Append(',');
            }

            if (str.Length > 1) str.Remove(str.Length - 1, 1);
            str.Append('}');
            return str.ToString();
        }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <param name="indentStyle">The indent style.</param>
        /// <returns>A JSON format string.</returns>
        public string ToString(IndentStyles indentStyle)
        {
            return ConvertToString(indentStyle, 0);
        }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <param name="indentStyle">The indent style.</param>
        /// <param name="indentLevel">The current indent level.</param>
        /// <returns>A JSON format string.</returns>
        internal string ConvertToString(IndentStyles indentStyle, int indentLevel)
        {
            if (indentStyle == IndentStyles.Minified) return ToString();
            var indentStr = StringExtensions.GetString(indentStyle);
            var indentPrefix = new StringBuilder();
            for (var i = 0; i < indentLevel; i++)
            {
                indentPrefix.Append(indentStr);
            }

            var indentStr2 = indentPrefix.ToString();
            indentPrefix.Append(indentStr);
            indentStr = indentPrefix.ToString();
            indentLevel++;
            var str = new StringBuilder("{");
            foreach (var prop in store)
            {
                str.AppendLine();
                str.Append(indentStr);
                str.Append(JsonString.ToJson(prop.Key));
                str.Append(": ");
                if (prop.Value is null)
                {
                    str.Append("null,");
                    continue;
                }

                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Null:
                        str.Append("null");
                        break;
                    case JsonValueKind.Array:
                        str.Append((prop.Value is JsonArray jArr) ? jArr.ConvertToString(indentStyle, indentLevel) : "[]");
                        break;
                    case JsonValueKind.Object:
                        str.Append((prop.Value is JsonObject jObj) ? jObj.ConvertToString(indentStyle, indentLevel) : "{}");
                        break;
                    default:
                        str.Append(prop.Value.ToString());
                        break;
                }

                str.Append(',');
            }

            if (str.Length > 1) str.Remove(str.Length - 1, 1);
            str.AppendLine();
            str.Append(indentStr2);
            str.Append('}');
            return str.ToString();
        }

        /// <summary>
        /// Gets the YAML format string of the value.
        /// </summary>
        /// <returns>A YAML format string.</returns>
        public string ToYamlString()
        {
            return ConvertToYamlString(0);
        }

        /// <summary>
        /// Gets the YAML format string of the value.
        /// </summary>
        /// <param name="indentLevel">The current indent level.</param>
        /// <returns>A YAML format string.</returns>
        internal string ConvertToYamlString(int indentLevel)
        {
            var indentStr = "  ";
            var indentPrefix = new StringBuilder();
            for (var i = 0; i < indentLevel; i++)
            {
                indentPrefix.Append(indentStr);
            }

            indentStr = indentPrefix.ToString();
            var nextIndentLevel = indentLevel + 1;
            var str = new StringBuilder();
            foreach (var prop in store)
            {
                str.Append(indentStr);
                str.Append(prop.Key.IndexOfAny(StringExtensions.YamlSpecialChars) >= 0
                    ? JsonString.ToJson(prop.Key)
                    : prop.Key);
                str.Append(": ");
                if (prop.Value is null)
                {
                    str.AppendLine("!!null null");
                    continue;
                }

                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Null:
                        str.AppendLine("!!null null");
                        break;
                    case JsonValueKind.Array:
                        if (!(prop.Value is JsonArray jArr))
                        {
                            str.AppendLine("[]");
                            break;
                        }
                        
                        str.AppendLine();
                        str.Append(jArr.ConvertToYamlString(indentLevel));
                        break;
                    case JsonValueKind.Object:
                        if (!(prop.Value is JsonObject jObj))
                        {
                            str.AppendLine("{}");
                            break;
                        }

                        str.AppendLine();
                        str.Append(jObj.ConvertToYamlString(nextIndentLevel));
                        break;
                    case JsonValueKind.String:
                        if (!(prop.Value is JsonString jStr))
                        {
                            str.AppendLine(prop.Value.ToString());
                            break;
                        }

                        var text = jStr.StringValue;
                        if (text == null)
                        {
                            str.AppendLine("!!null null");
                            break;
                        }

                        switch (jStr.ValueType)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 5:
                                str.AppendLine(text);
                                break;
                            default:
                                str.AppendLine(text.Length == 0 || text.Length > 100 || text.IndexOfAny(StringExtensions.YamlSpecialChars) >= 0
                                    ? JsonString.ToJson(text)
                                    : text);
                                break;
                        }

                        break;
                    default:
                        str.AppendLine(prop.Value.ToString());
                        break;
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// Deserializes.
        /// </summary>
        /// <typeparam name="T">The type of model to deserialize.</typeparam>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        /// <exception cref="JsonException">The JSON is invalid. -or- TValue is not compatible with the JSON.</exception>
        public T Deserialize<T>(JsonSerializerOptions options = default)
        {
            return JsonSerializer.Deserialize<T>(ToString(), options);
        }

        /// <summary>
        /// Deserializes a property value.
        /// </summary>
        /// <typeparam name="T">The type of model to deserialize.</typeparam>
        /// <param name="key">The property key.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="JsonException">The JSON is invalid. -or- TValue is not compatible with the JSON.</exception>
        public T DeserializeValue<T>(string key, JsonSerializerOptions options = default)
        {
            AssertKey(key);
            var item = store[key];
            if (item is null || item.ValueKind == JsonValueKind.Null || item.ValueKind == JsonValueKind.Undefined)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(item.ToString(), options);
        }

        /// <summary>
        /// Deserializes a property value.
        /// </summary>
        /// <typeparam name="T">The type of model to deserialize.</typeparam>
        /// <param name="key">The property key.</param>
        /// <param name="parser">The string parser.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="JsonException">The JSON is invalid. -or- TValue is not compatible with the JSON.</exception>
        public T DeserializeValue<T>(string key, Func<string, T> parser, JsonSerializerOptions options = default)
        {
            AssertKey(key);
            var item = store[key];
            if (item is null || item.ValueKind == JsonValueKind.Null || item.ValueKind == JsonValueKind.Undefined)
            {
                return default;
            }

            if (parser != null && item is IJsonString s) return parser(s.StringValue);
            return JsonSerializer.Deserialize<T>(item.ToString(), options);
        }

        /// <summary>
        /// Gets the JSON value kind groups.
        /// </summary>
        /// <returns>A dictionary of JSON value kind summary.</returns>
        public IDictionary<JsonValueKind, List<string>> GetJsonValueKindGroups()
        {
            var dict = new Dictionary<JsonValueKind, List<string>>
            {
                { JsonValueKind.Array, new List<string>() },
                { JsonValueKind.False, new List<string>() },
                { JsonValueKind.Null, new List<string>() },
                { JsonValueKind.Number, new List<string>() },
                { JsonValueKind.Object, new List<string>() },
                { JsonValueKind.String, new List<string>() },
                { JsonValueKind.True, new List<string>() },
            };
            foreach (var item in store)
            {
                var valueKind = item.Value?.ValueKind ?? JsonValueKind.Null;
                if (valueKind == JsonValueKind.Undefined) continue;
                dict[valueKind].Add(item.Key);
            }

            return dict;
        }

        /// <summary>
        /// Gets a dictionary of specific keys.
        /// </summary>
        /// <param name="keys">The properties keys.</param>
        /// <param name="removeNull">true if skip null values; otherwise, false.</param>
        /// <returns>A dictionary of values of the specific keys.</returns>
        public IDictionary<string, IJsonValueResolver> Where(IEnumerable<string> keys, bool removeNull = false)
        {
            var dict = new Dictionary<string, IJsonValueResolver>();
            if (removeNull)
            {
                foreach (var item in keys)
                {
                    if (store.TryGetValue(item, out var r) && !(r is null) && r.ValueKind != JsonValueKind.Null && r.ValueKind != JsonValueKind.Undefined)
                        dict[item] = r;
                }
            }
            else
            {
                foreach (var item in keys)
                {
                    if (store.TryGetValue(item, out var r))
                        dict[item] = r;
                }
            }

            return dict;
        }

        /// <summary>
        /// Gets a dictionary of JSON value kind.
        /// </summary>
        /// <param name="kind">The data type of JSON value to filter.</param>
        /// <param name="predicate">An optional function to test each source element for a condition; the second parameter of the function represents the property key; the third is the index of the element after filter.</param>
        /// <returns>A dictionary of values of the specific keys.</returns>
        public IDictionary<string, IJsonValueResolver> Where(JsonValueKind kind, Func<IJsonValueResolver, string, int, bool> predicate = null)
        {
            if (predicate == null) predicate = PassTrue;
            var dict = new Dictionary<string, IJsonValueResolver>();
            var i = -1;
            if (kind == JsonValueKind.Null || kind == JsonValueKind.Undefined)
            {
                foreach (var item in store)
                {
                    var value = item.Value;
                    if (value == null)
                    {
                        i++;
                        if (predicate(JsonValues.Null, item.Key, i)) dict[item.Key] = JsonValues.Null;
                        continue;
                    }

                    if (value.ValueKind == kind)
                    {
                        i++;
                        if (predicate(value, item.Key, i)) dict[item.Key] = value;
                    }
                }
            }
            else
            {
                foreach (var item in store)
                {
                    var value = item.Value;
                    if (value != null && value.ValueKind == kind)
                    {
                        i++;
                        if (predicate(value, item.Key, i)) dict[item.Key] = value;
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each source element for a condition.</param>
        /// <returns>A collection that contains elements from the input sequence that satisfy the condition.</returns>
        public IEnumerable<KeyValuePair<string, IJsonValueResolver>> Where(Func<KeyValuePair<string, IJsonValueResolver>, bool> predicate)
        {
            if (predicate == null) return store.Where(ele => true);
            return store.Where(predicate);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
        /// <returns>A collection that contains elements from the input sequence that satisfy the condition.</returns>
        public IEnumerable<KeyValuePair<string, IJsonValueResolver>> Where(Func<KeyValuePair<string, IJsonValueResolver>, int, bool> predicate)
        {
            if (predicate == null) return store.Where(ele => true);
            return store.Where(predicate);
        }

        /// <summary>
        /// Creates a dictionary from this instance.
        /// </summary>
        /// <returns>A dictionary that contains the key value pairs from this instance.</returns>
        public Dictionary<string, IJsonValueResolver> ToDictionary()
        {
            return new Dictionary<string, IJsonValueResolver>(store);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public JsonObject Clone()
        {
            return new JsonObject(store);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <param name="keys">The keys to copy; or null to clone all.</param>
        /// <returns>A new object that is a copy of this instance.</returns>
        public JsonObject Clone(IEnumerable<string> keys)
        {
            if (keys == null) return new JsonObject(store);
            var json = new JsonObject();
            foreach (var key in keys)
            {
                if (store.TryGetValue(key, out var v)) json.store.Add(key, v);
            }

            return json;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the properties collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the properties collection.</returns>
        public IEnumerator<KeyValuePair<string, IJsonValueResolver>> GetEnumerator()
        {
            return store.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the properties collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the properties collection.</returns>
        IEnumerator<KeyValuePair<string, IJsonValue>> IEnumerable<KeyValuePair<string, IJsonValue>>.GetEnumerator()
        {
            return store.Select(ele =>  new KeyValuePair<string, IJsonValue>(ele.Key, ele.Value ?? JsonValues.Null)).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the properties collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the properties collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return store.GetEnumerator();
        }

        /// <summary>
        /// Gets the value of the element as a boolean.
        /// </summary>
        /// <returns>The value of the element as a boolean.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        bool IJsonValueResolver.GetBoolean() => throw new InvalidOperationException("Expect a boolean but it is an object.");

        /// <summary>
        /// Gets the value of the element as a byte array.
        /// </summary>
        /// <returns>The value decoded as a byte array.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        byte[] IJsonValueResolver.GetBytesFromBase64() => throw new InvalidOperationException("Expect a string but it is an object.");

        /// <summary>
        /// Gets the value of the element as a date time.
        /// </summary>
        /// <returns>The value of the element as a date time.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        DateTime IJsonValueResolver.GetDateTime() => throw new InvalidOperationException("Expect a date time but it is an object.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        decimal IJsonValueResolver.GetDecimal() => throw new InvalidOperationException("Expect a number but it is an object.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        float IJsonValueResolver.GetSingle() => throw new InvalidOperationException("Expect a number but it is an object.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        double IJsonValueResolver.GetDouble() => throw new InvalidOperationException("Expect a number but it is an object.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        short IJsonValueResolver.GetInt16() => throw new InvalidOperationException("Expect a number but it is an object.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        uint IJsonValueResolver.GetUInt32() => throw new InvalidOperationException("Expect a number but it is an object.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        int IJsonValueResolver.GetInt32() => throw new InvalidOperationException("Expect a number but it is an object.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        long IJsonValueResolver.GetInt64() => throw new InvalidOperationException("Expect a number but it is an object.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        string IJsonValueResolver.GetString() => throw new InvalidOperationException("Expect a string but it is an object.");

        /// <summary>
        /// Gets the value of the element as a GUID.
        /// </summary>
        /// <returns>The value of the element as a GUID.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        Guid IJsonValueResolver.GetGuid() => throw new InvalidOperationException("Expect a string but it is an object.");

        /// <summary>
        /// Tries to get the value of the element as a boolean.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetBoolean(out bool result)
        {
            result = false;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a date time.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDateTime(out DateTime result)
        {
            result = WebFormat.ParseDate(0);
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDecimal(out decimal result)
        {
            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetSingle(out float result)
        {
            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetDouble(out double result)
        {
            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetUInt32(out uint result)
        {
            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetInt32(out int result)
        {
            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetInt64(out long result)
        {
            result = 0;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a number.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetString(out string result)
        {
            result = null;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the element as a GUID.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>true if the kind is the one expected; otherwise, false.</returns>
        bool IJsonValueResolver.TryGetGuid(out Guid result)
        {
            result = Guid.Empty;
            return false;
        }

        /// <summary>
        /// Gets all property keys.
        /// </summary>
        /// <returns>The property keys.</returns>
        IEnumerable<string> IJsonValueResolver.GetKeys() => Keys;

        private T GetJsonValue<T>(string key, JsonValueKind? valueKind = null, bool ignoreNull = false) where T : IJsonValue
        {
            var data = store[key];
            if (data is null)
            {
                if (ignoreNull) return default;
                throw new InvalidOperationException($"The value of property {key} is null.");
            }

            if (data is T v)
            {
                return v;
            }

            throw new InvalidOperationException(valueKind.HasValue
                ? $"The value kind of property {key} should be {valueKind.Value.ToString().ToLowerInvariant()} but it is {data.ValueKind.ToString().ToLowerInvariant()}."
                : $"The value kind of property {key} is {data.ValueKind.ToString().ToLowerInvariant()}, not expected.");
        }

        private bool TryGetJsonValue<T>(string key, out T property) where T : IJsonValue
        {
            if (string.IsNullOrWhiteSpace(key) || !store.TryGetValue(key, out var data) || data is null)
            {
                property = default;
                return false;
            }

            if (data is T v)
            {
                property = v;
                return true;
            }

            property = default;
            return false;
        }

        private void AssertKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key", "key should not be null, empty, or consists only of white-space characters.");
        }

        /// <summary>
        /// Converts to JSON document.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>An instance of the JsonDocument class.</returns>
        public static explicit operator JsonDocument(JsonObject json)
        {
            return json != null ? JsonDocument.Parse(json.ToString()) : null;
        }

        /// <summary>
        /// Converts from JSON document.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>An instance of the JsonDocument class.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
        public static implicit operator JsonObject(JsonDocument json)
        {
            if (json is null) return null;
            return json.RootElement;
        }

        /// <summary>
        /// Converts from JSON element.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
        public static implicit operator JsonObject(JsonElement json)
        {
            if (json.ValueKind != JsonValueKind.Object)
            {
                return json.ValueKind switch
                {
                    JsonValueKind.Null => null,
                    JsonValueKind.Undefined => null,
                    _ => throw new JsonException("json is not a JSON array.")
                };
            }

            var result = new JsonObject();
            var enumerator = json.EnumerateObject();
            while (enumerator.MoveNext())
            {
                result.SetValue(enumerator.Current);
            }

            return result;
        }

        /// <summary>
        /// Parses JSON object.
        /// </summary>
        /// <param name="json">A JSON object.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
        /// <exception cref="ArgumentException">options contains unsupported options.</exception>
        public static JsonObject Parse(string json, JsonDocumentOptions options = default)
        {
            return JsonDocument.Parse(json, options);
        }

        /// <summary>
        /// Parses a stream as UTF-8-encoded data representing a JSON object.
        /// The stream is read to completion.
        /// </summary>
        /// <param name="utf8Json">The JSON data to parse.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
        /// <exception cref="ArgumentException">options contains unsupported options.</exception>
        public static JsonObject Parse(Stream utf8Json, JsonDocumentOptions options = default)
        {
            return JsonDocument.Parse(utf8Json, options);
        }

        /// <summary>
        /// Parses a stream as UTF-8-encoded data representing a JSON object.
        /// The stream is read to completion.
        /// </summary>
        /// <param name="utf8Json">The JSON data to parse.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
        /// <exception cref="ArgumentException">options contains unsupported options.</exception>
        public static async Task<JsonObject> ParseAsync(Stream utf8Json, JsonDocumentOptions options = default, CancellationToken cancellationToken = default)
        {
            return await JsonDocument.ParseAsync(utf8Json, options, cancellationToken);
        }

        /// <summary>
        /// Parses JSON object.
        /// </summary>
        /// <param name="reader">A JSON object.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
        public static JsonObject ParseValue(ref Utf8JsonReader reader)
        {
            var obj = new JsonObject();
            obj.SetRange(ref reader);
            return obj;
        }

        /// <summary>
        /// Converts an object to JSON object.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
        public static JsonObject ConvertFrom(object obj, JsonSerializerOptions options = default)
        {
            if (obj is null) return null;
            if (obj is IJsonValue)
            {
                if (obj is JsonObject jObj) return jObj;
                if (obj is JsonArray jArr)
                {
                    var r = new JsonObject();
                    var i = 0;
                    foreach (var item in jArr)
                    {
                        r.Add(i.ToString(), item);
                        i++;
                    }

                    return r;
                }

                if (obj is JsonNull) return null;
                var valueKind = (obj as IJsonValue).ValueKind;
                switch (valueKind)
                {
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                    case JsonValueKind.False:
                        return null;
                    case JsonValueKind.True:
                        return new JsonObject();
                }
            }

            if (obj is JsonDocument doc) return doc;
            if (obj is string str) return Parse(str);
            if (obj is StringBuilder sb) return Parse(sb.ToString());
            if (obj is Stream stream) return Parse(stream);
            if (obj is IEnumerable<KeyValuePair<string, object>> dict)
            {
                var r = new JsonObject();
                foreach (var kvp in dict)
                {
                    if (string.IsNullOrWhiteSpace(kvp.Key)) continue;
                    r.SetValue(kvp.Key, ConvertFrom(kvp.Value));
                }

                return r;
            }

            var s = JsonSerializer.Serialize(obj, obj.GetType(), options);
            return Parse(s);
        }

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonObject leftValue, IJsonValue rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null || leftValue is null) return false;
            return leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Compares two instances to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(JsonObject leftValue, IJsonValue rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null || leftValue is null) return true;
            return !leftValue.Equals(rightValue);
        }

        private static JsonObject TryGetObjectValueByProperty(JsonObject json, string key)
        {
            if (json.GetValueKind(key) == JsonValueKind.Array)
            {
                if (!int.TryParse(key, out var i)) return null;
                return json.TryGetArrayValue(key).TryGetObjectValue(i);
            }

            return json.TryGetObjectValue(key);
        }

        private static bool PassTrue(IJsonValueResolver data, string key, int index)
        {
            return true;
        }
    }
}
