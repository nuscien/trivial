﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a specific JSON object.
    /// </summary>
    public class JsonObject : IJsonComplex, IJsonValueResolver, IDictionary<string, IJsonValue>, IReadOnlyDictionary<string, IJsonValue>, IEquatable<JsonObject>, IEquatable<IJsonValue>
    {
        private readonly IDictionary<string, IJsonValue> store = new Dictionary<string, IJsonValue>();

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
        private JsonObject(IDictionary<string, IJsonValue> copy)
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
        /// Gets a collection containing the property values of the object.
        /// </summary>
        public ICollection<IJsonValue> Values => store.Values;

        /// <summary>
        /// Gets a collection containing the property keys of the object.
        /// </summary>
        IEnumerable<string> IReadOnlyDictionary<string, IJsonValue>.Keys => store.Keys;

        /// <summary>
        /// Gets a collection containing the property values of the object.
        /// </summary>
        IEnumerable<IJsonValue> IReadOnlyDictionary<string, IJsonValue>.Values => store.Values;

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
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
        public IJsonValue this[string key]
        {
            get => GetValue(key);
            set => store[key] = JsonValues.ConvertValue(value, this);
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
            return ContainsKey(new string(key));
        }

        /// <summary>
        /// Gets the raw value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
        public string GetRawText(ReadOnlySpan<char> key)
        {
            if (key == null) throw new ArgumentNullException("key", "key should not be null.");
            return GetRawText(new string(key));
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
            return GetValueKind(new string(key), strictMode);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="convert">true if want to convert to string if it is a number or a boolean; otherwise, false.</param>
        /// <returns>The value. It will be null if the value is null.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
        public string GetStringValue(string key, bool convert = false)
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

            if (convert) return data.ValueKind switch
            {
                JsonValueKind.True => JsonBoolean.TrueString,
                JsonValueKind.False => JsonBoolean.TrueString,
                JsonValueKind.Number => data.ToString(),
                _ => throw new InvalidOperationException($"The value type of property {key} should be string but it is {data.ValueKind.ToString().ToLowerInvariant()}.")
            };
            throw new InvalidOperationException($"The value type of property {key} should be string but it is {data.ValueKind.ToString().ToLowerInvariant()}.");
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="FormatException">The value is not in a recognized format.</exception>
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
        public bool GetBooleanValue(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonBoolean>(key, out var v)) return v.Value;
            var p = GetJsonValue<JsonString>(key);
            return p.Value?.ToLower() switch
            {
                JsonBoolean.TrueString => true,
                JsonBoolean.FalseString => false,
                _ => throw new InvalidOperationException($"The value type of property {key} should be boolean but it is string.")
            };
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
        public JsonObject GetObjectValue(string key, string subKey, params string[] keyPath)
        {
            AssertKey(key);
            var json = GetJsonValue<JsonObject>(key, JsonValueKind.Object, true);
            if (string.IsNullOrWhiteSpace(subKey))
            {
                if (keyPath.Length == 0) return json;
                throw new ArgumentNullException("subKey should not be null, empty, or consists only of white-space characters.");
            }

            if (json is null) throw new InvalidOperationException($"The value of property {key} is null.");
            json = json.GetJsonValue<JsonObject>(subKey, JsonValueKind.Object);
            foreach (var k in keyPath)
            {
                if (string.IsNullOrEmpty(k)) continue;
                if (json is null) throw new InvalidOperationException($"Cannot get property {k} of null.");
                json = json.GetJsonValue<JsonObject>(k, JsonValueKind.Object);
            }

            return json;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="keyPath">The property key path.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
        public JsonObject GetObjectValue(IEnumerable<string> keyPath)
        {
            var json = this;
            if (keyPath == null) return json;
            foreach (var k in keyPath)
            {
                if (string.IsNullOrEmpty(k)) continue;
                if (json is null) throw new InvalidOperationException($"Cannot get property {k} of null.");
                json = json.GetJsonValue<JsonObject>(k, JsonValueKind.Object, true);
            }

            return json;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
        public DateTime GetDateTimeValue(string key, bool useUnixTimestampsFallback = false)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonString>(key, out var s))
            {
                var date = Web.WebFormat.ParseDate(s.Value);
                if (date.HasValue) return date.Value;
                throw new InvalidOperationException("The value is not a date time.");
            }

            var num = GetJsonValue<JsonInteger>(key);
            return useUnixTimestampsFallback ? Web.WebFormat.ParseUnixTimestamp(num.Value) : Web.WebFormat.ParseDate(num.Value);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="FormatException">The value is not encoded as Base64 text and hence cannot be decoded to bytes.</exception>
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
        /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
        public T GetEnumValue<T>(string key) where T : struct, Enum
        {
            if (TryGetInt32Value(key, out var v)) return (T)(object)v;
            var str = GetStringValue(key);
            return Enum.Parse<T>(str);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
        public T GetEnumValue<T>(string key, bool ignoreCase) where T : struct, Enum
        {
            if (TryGetInt32Value(key, out var v)) return (T)(object)v;
            var str = GetStringValue(key);
            return Enum.Parse<T>(str, ignoreCase);
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
            return (store[key] as IJsonValueResolver) ?? JsonValues.Null;
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
            return GetValue(new string(key));
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is an object.");

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
            if (!string.IsNullOrWhiteSpace(subKey))
            {
                if (json is null) return null;
                json = json.TryGetObjectValue(subKey);
            }

            foreach (var k in keyPath)
            {
                if (json is null) return null;
                json = json.TryGetObjectValue(k);
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
        /// <exception cref="InvalidOperationException">The value type is not the expected one.</exception>
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
                var date = Web.WebFormat.ParseDate(s.Value);
                return date;
            }

            if (!TryGetJsonValue<JsonInteger>(key, out var num)) return null;
            return useUnixTimestampsFallback ? Web.WebFormat.ParseUnixTimestamp(num.Value) : Web.WebFormat.ParseDate(num.Value);
        }

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
                if (!string.IsNullOrWhiteSpace(key) && store.TryGetValue(key, out var value)) return (value as IJsonValueResolver) ?? JsonValues.Null;
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
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetValue(string key, out IJsonValueResolver result)
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
        /// Gets the value of the specific property.
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
        /// Sets null to the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetNullValue(string key)
        {
            AssertKey(key);
            store[key] = null;
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
            SetValue(key, Convert.ToBase64String(bytes, options));
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
            store[key] = new JsonInteger(Web.WebFormat.ParseDate(value));
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
            store[key] = new JsonInteger(Web.WebFormat.ParseUnixTimestamp(value));
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
            if (json == null) return count;
            if (skipDuplicate)
            {
                if (json == this) return count;
                foreach (var prop in json)
                {
                    if (ContainsKey(prop.Key)) continue;
                    store[prop.Key] = prop.Value;
                    count++;
                }
            }
            else
            {
                if (json == this) json = json.Clone();
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
        public void CopyTo(KeyValuePair<string, IJsonValue>[] array, int arrayIndex)
        {
            store.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific property from the JSON object.
        /// </summary>
        /// <param name="item">The property to remove from the JSON object.</param>
        /// <returns>true if property was successfully removed from the JSON object; otherwise, false. This method also returns false if property is not found in the original JSON object.</returns>
        public bool Remove(KeyValuePair<string, IJsonValue> item)
        {
            KeyValuePair<string, IJsonValue>? kvp = null;
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
                if (isNull) return false;
                JsonValues.Equals(prop.Value, r);
            }

            return false;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(IJsonValue other)
        {
            if (other is null) return false;
            if (other is JsonArray json) return Equals(json);
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
        public override int GetHashCode()
        {
            return store.GetHashCode();
        }

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
        /// Deserializes.
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
        public IEnumerator<KeyValuePair<string, IJsonValue>> GetEnumerator()
        {
            return store.GetEnumerator();
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
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        Guid IJsonValueResolver.GetGuid() => throw new InvalidOperationException("Expect a string but it is an object.");

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
        /// <returns>An instance of the JsonObject class.</returns>
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
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public static JsonObject Parse(string json, JsonDocumentOptions options = default)
        {
            return JsonDocument.Parse(json, options);
        }

        /// <summary>
        /// Parses a stream as UTF-8-encoded data representing a JSON object.
        /// The stream is read to completio
        /// </summary>
        /// <param name="utf8Json">The JSON data to parse.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public static JsonObject Parse(Stream utf8Json, JsonDocumentOptions options = default)
        {
            return JsonDocument.Parse(utf8Json, options);
        }

        /// <summary>
        /// Parses a stream as UTF-8-encoded data representing a JSON object.
        /// The stream is read to completio
        /// </summary>
        /// <param name="utf8Json">The JSON data to parse.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
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
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public static JsonObject ParseValue(ref Utf8JsonReader reader)
        {
            var obj = new JsonObject();
            obj.SetRange(ref reader);
            return obj;
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
    }
}