using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a specific JSON object.
    /// </summary>
    public class JsonObject : IJsonValue, IReadOnlyDictionary<string, IJsonValue>, ICloneable
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
        /// Gets a collection containing the property keys of the object.
        /// </summary>
        public IEnumerable<string> Keys => store.Keys;

        /// <summary>
        /// Gets a collection containing the property values of the object.
        /// </summary>
        public IEnumerable<IJsonValue> Values => store.Values;

        /// <summary>
        /// Gets the number of elements contained in the System.Collections.Generic.ICollection`1
        /// </summary>
        public int Count => store.Count;

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public IJsonValue this[string key] => GetValue(key);

        /// <summary>
        /// Determines the property value of the specific key is null or undefined.
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
        public bool ContainsKey(string key)
        {
            return store.ContainsKey(key);
        }

        /// <summary>
        /// Gets the raw value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public string GetRawText(string key)
        {
            AssertKey(key);
            var data = store[key];
            if (data is null) return null;
            return data.ToString();
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
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="convert">true if want to convert to string; otherwise, false.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public string GetStringValue(string key, bool convert = false)
        {
            AssertKey(key);
            var data = store[key];
            if (data is null) return null;
            if (data is JsonStringValue str)
            {
                return str.Value;
            }

            if (convert) return data.ToString();
            throw new InvalidCastException($"The value type of property {key} is {data.ValueKind}.");
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="FormatException">The value is not in a recognized format.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
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
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public uint GetUInt32Value(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonIntegerValue>(key, out var v)) return (uint)v;
            var p = GetJsonValue<JsonFloatValue>(key);
            return (uint)p;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public int GetInt32Value(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonIntegerValue>(key, out var v)) return (int)v;
            var p = GetJsonValue<JsonFloatValue>(key);
            return (int)p;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public long GetInt64Value(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonIntegerValue>(key, out var v)) return v.Value;
            var p = GetJsonValue<JsonFloatValue>(key);
            return (long)p;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public float GetSingleValue(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonFloatValue>(key, out var v)) return (float)v;
            var p = GetJsonValue<JsonIntegerValue>(key);
            return (float)p;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public double GetDoubleValue(string key)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonFloatValue>(key, out var v)) return v.Value;
            var p = GetJsonValue<JsonIntegerValue>(key);
            return (double)p;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public bool GetBooleanValue(string key)
        {
            AssertKey(key);
            var p = GetJsonValue<JsonBooleanValue>(key);
            return p.Value;
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public JsonObject GetObjectValue(string key)
        {
            AssertKey(key);
            return GetJsonValue<JsonObject>(key);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public JsonArray GetArrayValue(string key)
        {
            AssertKey(key);
            return GetJsonValue<JsonArray>(key);
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
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public DateTime GetDateTimeValue(string key, bool useUnixTimestampsFallback = false)
        {
            AssertKey(key);
            if (TryGetJsonValue<JsonStringValue>(key, out var s))
            {
                var date = Web.WebFormat.ParseDate(s.Value);
                if (date.HasValue) return date.Value;
                throw new InvalidCastException("The value is not a date time.");
            }

            var num = GetJsonValue<JsonIntegerValue>(key);
            return useUnixTimestampsFallback ? Web.WebFormat.ParseUnixTimestamp(num.Value) : Web.WebFormat.ParseDate(num.Value);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public IJsonValue GetValue(string key)
        {
            AssertKey(key);
            return store[key] ?? JsonValues.Null;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="convert">true if want to convert to string; otherwise, false.</param>
        /// <returns>A string.</returns>
        public string TryGetStringValue(string key, bool convert = false)
        {
            if (!store.TryGetValue(key, out var data))
            {
                return null;
            }

            if (data is null)
            {
                return null;
            }

            if (data is JsonStringValue str)
            {
                return str.Value;
            }

            if (convert)
            {
                return data.ToString();
            }

            return null;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <param name="convert">true if want to convert to string; otherwise, false.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetStringValue(string key, out string result, bool convert = false)
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

            if (data is JsonStringValue str)
            {
                result = str.Value;
                return true;
            }

            if (convert)
            {
                result = data.ToString();
                return true;
            }

            result = null;
            return false;
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
            if (TryGetJsonValue<JsonIntegerValue>(key, out var p1)) return (uint)p1;
            if (TryGetJsonValue<JsonFloatValue>(key, out var p2)) return (uint)p2;
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
            if (TryGetJsonValue<JsonIntegerValue>(key, out var p1)) return (int)p1;
            if (TryGetJsonValue<JsonFloatValue>(key, out var p2)) return (int)p2;
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
            if (TryGetJsonValue<JsonIntegerValue>(key, out var p1)) return p1.Value;
            if (TryGetJsonValue<JsonFloatValue>(key, out var p2)) return (long)p2;
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
            if (TryGetJsonValue<JsonFloatValue>(key, out var p1)) return (float)p1;
            if (TryGetJsonValue<JsonIntegerValue>(key, out var p2)) return (float)p2;
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
            if (TryGetJsonValue<JsonFloatValue>(key, out var p1)) return p1.Value;
            if (TryGetJsonValue<JsonIntegerValue>(key, out var p2)) return (double)p2;
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
            if (TryGetJsonValue<JsonBooleanValue>(key, out var p)) return p.Value;
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
            if (TryGetJsonValue<JsonObject>(key, out var p)) return p;
            return null;
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
            return !(v is null);
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
            if (TryGetJsonValue<JsonStringValue>(key, out var s))
            {
                var date = Web.WebFormat.ParseDate(s.Value);
                return date;
            }

            if (!TryGetJsonValue<JsonIntegerValue>(key, out var num)) return null;
            return useUnixTimestampsFallback ? Web.WebFormat.ParseUnixTimestamp(num.Value) : Web.WebFormat.ParseDate(num.Value);
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        public IJsonValue TryGetValue(string key)
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
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetValue(string key, out IJsonValue result)
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
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void SetValue(string key, string value)
        {
            AssertKey(key);
            store[key] = new JsonStringValue(value);
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
            store[key] = new JsonStringValue(value);
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
            store[key] = new JsonIntegerValue(value);
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
            store[key] = new JsonIntegerValue(value);
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
            store[key] = new JsonIntegerValue(value);
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
            store[key] = new JsonFloatValue(value);
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
            store[key] = new JsonFloatValue(value);
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
            store[key] = new JsonBooleanValue(value);
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
        public void SetDateTimeStringValue(string key, DateTime value)
        {
            AssertKey(key);
            store[key] = new JsonStringValue(value);
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
            store[key] = new JsonIntegerValue(Web.WebFormat.ParseDate(value));
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
            store[key] = new JsonIntegerValue(Web.WebFormat.ParseUnixTimestamp(value));
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
            store[key] = new JsonIntegerValue(value.ToFileTimeUtc());
        }

        /// <summary>
        /// Removes all properties from the object.
        /// </summary>
        public void Clear()
        {
            store.Clear();
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
                        writer.WriteString(prop.Key, prop.ToString());
                        break;
                    case JsonValueKind.Number:
                        if (prop.Value is JsonIntegerValue intJson) writer.WriteNumber(prop.Key, (long)intJson);
                        else if (prop.Value is JsonFloatValue floatJson) writer.WriteNumber(prop.Key, (double)floatJson);
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
        /// <returns></returns>
        public override string ToString()
        {
            var str = new StringBuilder("{");
            foreach (var prop in store)
            {
                str.Append(JsonStringValue.ToJson(prop.Key));
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
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public T Deserialize<T>(JsonSerializerOptions options = default)
        {
            return JsonSerializer.Deserialize<T>(ToString(), options);
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

        private T GetJsonValue<T>(string key) where T : IJsonValue
        {
            var data = store[key];
            if (data is null) throw new InvalidCastException($"The value of property {key} is null.");
            if (data is T v)
            {
                return v;
            }

            throw new InvalidCastException($"The value type of property {key} is {data.ValueKind}.");
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
            return JsonDocument.ParseValue(ref reader);
        }
    }
}
