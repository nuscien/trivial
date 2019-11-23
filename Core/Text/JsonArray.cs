using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a specific JSON object.
    /// </summary>
    public class JsonArray : IJsonValue, IReadOnlyList<IJsonValue>, ICloneable
    {
        private readonly IList<IJsonValue> store = new List<IJsonValue>();

        /// <summary>
        /// Initializes a new instance of the JsonArray class.
        /// </summary>
        public JsonArray()
        {
        }

        /// <summary>
        /// Initializes a new instance of the JsonArray class.
        /// </summary>
        /// <param name="copy">Properties to initialzie.</param>
        private JsonArray(IList<IJsonValue> copy)
        {
            if (copy == null) return;
            foreach (var ele in copy)
            {
                store.Add(ele);
            }
        }

        /// <summary>
        /// Gets the type of the current JSON value.
        /// </summary>
        public JsonValueKind ValueKind => JsonValueKind.Array;

        /// <summary>
        /// Gets the number of elements contained in the array.
        /// </summary>
        public int Count => store.Count;

        /// <summary>
        /// Gets the element at the specified index in the array.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the array.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public IJsonValue this[int index] => GetValue(index);

        /// <summary>
        /// Determines the property value of the specific key is null or undefined.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>true if there is no such key or the property value is null; otherwise, false.</returns>
        public bool IsNullOrUndefined(int index)
        {
            if (index < 0 && index >= store.Count) return true;
            try
            {
                var value = store[index];
                return value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined;
            }
            catch (ArgumentException)
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether it contains an property value with the specific key.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>true if there is no such key; otherwise, false.</returns>
        public bool Contains(int index)
        {
            return index >= 0 && index < store.Count;
        }

        /// <summary>
        /// Gets the raw value of the specific value.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public string GetRawText(int index)
        {
            var data = store[index];
            if (data is null) return null;
            return data.ToString();
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="convert">true if want to convert to string; otherwise, false.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public string GetStringValue(int index, bool convert = false)
        {
            var data = store[index];
            if (data is null) return null;
            if (data is JsonStringValue str)
            {
                return str.Value;
            }

            if (convert) return data.ToString();
            throw new InvalidCastException($"The type of item {index} is {data.ValueKind}.");
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="FormatException">The value is not in a recognized format.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public Guid GetGuidValue(int index)
        {
            return Guid.Parse(GetStringValue(index));
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public uint GetUInt32Value(int index)
        {
            if (TryGetJsonValue<JsonIntegerValue>(index, out var v)) return (uint)v;
            var p = GetJsonValue<JsonFloatValue>(index);
            return (uint)p;
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public int GetInt32Value(int index)
        {
            if (TryGetJsonValue<JsonIntegerValue>(index, out var v)) return (int)v;
            var p = GetJsonValue<JsonFloatValue>(index);
            return (int)p;
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public long GetInt64Value(int index)
        {
            if (TryGetJsonValue<JsonIntegerValue>(index, out var v)) return v.Value;
            var p = GetJsonValue<JsonFloatValue>(index);
            return (long)p;
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public float GetSingleValue(int index)
        {
            if (TryGetJsonValue<JsonFloatValue>(index, out var v)) return (float)v;
            var p = GetJsonValue<JsonIntegerValue>(index);
            return (float)p;
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public double GetDoubleValue(int index)
        {
            if (TryGetJsonValue<JsonFloatValue>(index, out var v)) return v.Value;
            var p = GetJsonValue<JsonIntegerValue>(index);
            return (double)p;
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public bool GetBooleanValue(int index)
        {
            var p = GetJsonValue<JsonBooleanValue>(index);
            return p.Value;
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public JsonObject GetObjectValue(int index)
        {
            return GetJsonValue<JsonObject>(index);
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public JsonArray GetArrayValue(int index)
        {
            return GetJsonValue<JsonArray>(index);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="useUnixTimestampsFallback">true if use Unix timestamp to convert if the value is a number; otherwise, false, to use JavaScript date ticks fallback.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="FormatException">The value is not in a recognized format.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public DateTime GetDateTimeValue(int index, bool useUnixTimestampsFallback = false)
        {
            if (TryGetJsonValue<JsonStringValue>(index, out var s))
            {
                var date = Web.WebFormat.ParseDate(s.Value);
                if (date.HasValue) return date.Value;
                throw new FormatException("The value is not a date time.");
            }

            var num = GetJsonValue<JsonIntegerValue>(index);
            return useUnixTimestampsFallback ? Web.WebFormat.ParseUnixTimestamp(num.Value) : Web.WebFormat.ParseDate(num.Value);
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidCastException">The value type is not the expected one.</exception>
        public IJsonValue GetValue(int index)
        {
            return store[index] ?? JsonValueExtensions.Null;
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="convert">true if want to convert to string; otherwise, false.</param>
        /// <returns>A string.</returns>
        public string TryGetStringValue(int index, bool convert = false)
        {
            if (index < 0 || index >= store.Count)
            {
                return null;
            }

            try
            {
                var data = store[index];
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
            }
            catch (ArgumentException)
            {
            }

            return null;
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <param name="convert">true if want to convert to string; otherwise, false.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetStringValue(int index, out string result, bool convert = false)
        {
            if (index < 0 || index >= store.Count)
            {
                result = null;
                return false;
            }

            try
            {
                var data = store[index];
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
            }
            catch(ArgumentException)
            {
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Tries to gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public Guid? TryGetGuidValue(int index)
        {
            if (!TryGetStringValue(index, out var str) || string.IsNullOrWhiteSpace(str) || !Guid.TryParse(str, out var result))
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Tries to gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetGuidValue(int index, out Guid result)
        {
            if (!TryGetStringValue(index, out var str) || string.IsNullOrWhiteSpace(str) || !Guid.TryParse(str, out result))
            {
                result = default;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public uint? TryGetUInt32Value(int index)
        {
            if (TryGetJsonValue<JsonIntegerValue>(index, out var p1)) return (uint)p1;
            if (TryGetJsonValue<JsonFloatValue>(index, out var p2)) return (uint)p2;
            return null;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetUInt32Value(int index, out uint result)
        {
            var v = TryGetUInt32Value(index);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public int? TryGetInt32Value(int index)
        {
            if (TryGetJsonValue<JsonIntegerValue>(index, out var p1)) return (int)p1;
            if (TryGetJsonValue<JsonFloatValue>(index, out var p2)) return (int)p2;
            return null;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetInt32Value(int index, out int result)
        {
            var v = TryGetInt32Value(index);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public long? TryGetInt64Value(int index)
        {
            if (TryGetJsonValue<JsonIntegerValue>(index, out var p1)) return p1.Value;
            if (TryGetJsonValue<JsonFloatValue>(index, out var p2)) return (long)p2;
            return null;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetInt64Value(int index, out long result)
        {
            var v = TryGetInt64Value(index);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public float? TryGetFloatValue(int index)
        {
            if (TryGetJsonValue<JsonFloatValue>(index, out var p1)) return (float)p1;
            if (TryGetJsonValue<JsonIntegerValue>(index, out var p2)) return (float)p2;
            return null;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetFloatValue(int index, out float result)
        {
            var v = TryGetFloatValue(index);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public double? TryGetDoubleValue(int index)
        {
            if (TryGetJsonValue<JsonFloatValue>(index, out var p1)) return p1.Value;
            if (TryGetJsonValue<JsonIntegerValue>(index, out var p2)) return (double)p2;
            return null;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetDoubleValue(int index, out double result)
        {
            var v = TryGetDoubleValue(index);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public bool? TryGetBooleanValue(int index)
        {
            if (TryGetJsonValue<JsonBooleanValue>(index, out var p)) return p.Value;
            return null;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetBooleanValue(int index, out bool result)
        {
            var v = TryGetBooleanValue(index);
            result = v ?? default;
            return v.HasValue;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        public JsonObject TryGetObjectValue(int index)
        {
            if (TryGetJsonValue<JsonObject>(index, out var p)) return p;
            return null;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetObjectValue(int index, out JsonObject result)
        {
            var v = TryGetObjectValue(index);
            result = v;
            return !(v is null);
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        public JsonArray TryGetArrayValue(int index)
        {
            if (TryGetJsonValue<JsonArray>(index, out var p)) return p;
            return null;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetArrayValue(int index, out JsonArray result)
        {
            var v = TryGetArrayValue(index);
            result = v;
            return !(v is null);
        }

        /// <summary>
        /// Tries to gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="useUnixTimestampsFallback">true if use Unix timestamp to convert if the value is a number; otherwise, false, to use JavaScript date ticks fallback.</param>
        /// <returns>The value.</returns>
        public DateTime? TryGetDateTimeValue(int index, bool useUnixTimestampsFallback = false)
        {
            if (TryGetJsonValue<JsonStringValue>(index, out var s))
            {
                var date = Web.WebFormat.ParseDate(s.Value);
                return date;
            }

            if (!TryGetJsonValue<JsonIntegerValue>(index, out var num)) return null;
            return useUnixTimestampsFallback ? Web.WebFormat.ParseUnixTimestamp(num.Value) : Web.WebFormat.ParseDate(num.Value);
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        public IJsonValue TryGetValue(int index)
        {
            if (index < 0 || index >= store.Count)
            {
                return default;
            }

            try
            {
                return store[index] ?? JsonValueExtensions.Null;
            }
            catch (ArgumentException)
            {
            }

            return default;
        }

        /// <summary>
        /// Tries to gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetValue(int index, out IJsonValue result)
        {
            var v = TryGetValue(index);
            result = v;
            return !(v is null);
        }

        /// <summary>
        /// Removes the element at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Remove(int index)
        {
            store.RemoveAt(index);
        }

        /// <summary>
        /// Sets null at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetNullValue(int index)
        {
            store[index] = null;
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, string value)
        {
            store[index] = new JsonStringValue(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, Guid value)
        {
            store[index] = new JsonStringValue(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, uint value)
        {
            store[index] = new JsonIntegerValue(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, int value)
        {
            store[index] = new JsonIntegerValue(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, long value)
        {
            store[index] = new JsonIntegerValue(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, float value)
        {
            store[index] = new JsonFloatValue(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, double value)
        {
            store[index] = new JsonFloatValue(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, bool value)
        {
            store[index] = new JsonBooleanValue(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, JsonArray value)
        {
            store[index] = value != this ? value : value.Clone();
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, JsonObject value)
        {
            store[index] = value;
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, JsonElement value)
        {
            store[index] = JsonValueExtensions.ToJsonValue(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetDateTimeStringValue(int index, DateTime value)
        {
            store[index] = new JsonStringValue(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetJavaScriptDateTicksValue(int index, DateTime value)
        {
            store[index] = new JsonIntegerValue(Web.WebFormat.ParseDate(value));
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetUnixTimestampValue(int index, DateTime value)
        {
            store[index] = new JsonIntegerValue(Web.WebFormat.ParseUnixTimestamp(value));
        }

        /// <summary>
        /// Add null.
        /// </summary>
        public void AddNull()
        {
            store.Add(null);
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(string value)
        {
            store.Add(new JsonStringValue(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(Guid value)
        {
            store.Add(new JsonStringValue(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(uint value)
        {
            store.Add(new JsonIntegerValue(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(int value)
        {
            store.Add(new JsonIntegerValue(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(long value)
        {
            store.Add(new JsonIntegerValue(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(float value)
        {
            store.Add(new JsonFloatValue(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(double value)
        {
            store.Add(new JsonFloatValue(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(bool value)
        {
            store.Add(new JsonBooleanValue(value));
        }


        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(JsonArray value)
        {
            store.Add(value != this ? value : value.Clone());
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(JsonObject value)
        {
            store.Add(value);
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(JsonElement value)
        {
            store.Add(JsonValueExtensions.ToJsonValue(value));
        }

        /// <summary>
        /// Adds a date time string.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void AddDateTimeString(DateTime value)
        {
            store.Add(new JsonStringValue(value));
        }

        /// <summary>
        /// Adds a JavaScript date ticks.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void AddJavaScriptDateTicks(DateTime value)
        {
            store.Add(new JsonIntegerValue(Web.WebFormat.ParseDate(value)));
        }

        /// <summary>
        /// Adds a Unix timestamp.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void AddUnixTimestamp(DateTime value)
        {
            store.Add(new JsonIntegerValue(Web.WebFormat.ParseUnixTimestamp(value)));
        }

        /// <summary>
        /// Inserts null at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void InsertNull(int index)
        {
            store.Insert(index, null);
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, string value)
        {
            store.Insert(index, new JsonStringValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, Guid value)
        {
            store.Insert(index, new JsonStringValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, uint value)
        {
            store.Insert(index, new JsonIntegerValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, int value)
        {
            store.Insert(index, new JsonIntegerValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, long value)
        {
            store.Insert(index, new JsonIntegerValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, float value)
        {
            store.Insert(index, new JsonFloatValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, double value)
        {
            store.Insert(index, new JsonFloatValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, bool value)
        {
            store.Insert(index, new JsonBooleanValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void Insert(int index, JsonArray value)
        {
            store.Insert(index, value != this ? value : value.Clone());
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void Insert(int index, JsonObject value)
        {
            store.Insert(index, value);
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        public void Insert(int index, JsonElement value)
        {
            store.Insert(index, JsonValueExtensions.ToJsonValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void InsertDateTimeString(int index, DateTime value)
        {
            store.Insert(index, new JsonStringValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void InsertJavaScriptDateTicks(int index, DateTime value)
        {
            store.Insert(index, new JsonIntegerValue(Web.WebFormat.ParseDate(value)));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void InsertUnixTimestamp(int index, DateTime value)
        {
            store.Insert(index, new JsonIntegerValue(Web.WebFormat.ParseUnixTimestamp(value)));
        }

        /// <summary>
        /// Removes all items from the array.
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
            writer.WriteStartArray();
            foreach (var prop in store)
            {
                if (prop is null)
                {
                    writer.WriteNullValue();
                    continue;
                }

                switch (prop.ValueKind)
                {
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Null:
                        writer.WriteNullValue();
                        break;
                    case JsonValueKind.String:
                        writer.WriteStringValue(prop.ToString());
                        break;
                    case JsonValueKind.Number:
                        if (prop is JsonIntegerValue intJson) writer.WriteNumberValue((long)intJson);
                        else if (prop is JsonFloatValue floatJson) writer.WriteNumberValue((double)floatJson);
                        break;
                    case JsonValueKind.True:
                        writer.WriteBooleanValue(true);
                        break;
                    case JsonValueKind.False:
                        writer.WriteBooleanValue(false);
                        break;
                    case JsonValueKind.Object:
                        if (prop is JsonObject objJson) objJson.WriteTo(writer);
                        break;
                    case JsonValueKind.Array:
                        if (prop is JsonArray objArr) objArr.WriteTo(writer);
                        break;
                }
            }

            writer.WriteEndArray();
        }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var str = new StringBuilder("[");
            foreach (var prop in store)
            {
                if (prop is null)
                {
                    str.Append("null,");
                    continue;
                }

                switch (prop.ValueKind)
                {
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Null:
                        str.Append("null");
                        break;
                    default:
                        str.Append(prop.ToString());
                        break;
                }

                str.Append(',');
            }

            if (str.Length > 1) str.Remove(str.Length - 1, 1);
            str.Append(']');
            return str.ToString();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the array.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the array.</returns>
        public IEnumerator<IJsonValue> GetEnumerator()
        {
            return store.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the array.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the array.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return store.GetEnumerator();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public JsonArray Clone()
        {
            return new JsonArray(store);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        private T GetJsonValue<T>(int index) where T : IJsonValue
        {
            var data = store[index];
            if (data is null) throw new InvalidCastException($"The item {index} is null.");
            if (data is T v)
            {
                return v;
            }

            throw new InvalidCastException($"The type of item {index} is {data.ValueKind}.");
        }

        private bool TryGetJsonValue<T>(int index, out T property) where T : IJsonValue
        {
            if (index < 0 || index >= store.Count)
            {
                property = default;
                return false;
            }

            try
            {
                var data = store[index];
                if (data is T v)
                {
                    property = v;
                    return true;
                }
            }
            catch (ArgumentException)
            {
            }

            property = default;
            return false;
        }

        /// <summary>
        /// Converts to JSON document.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>An instance of the JsonObject class.</returns>
        public static explicit operator JsonDocument(JsonArray json)
        {
            return json != null ? JsonDocument.Parse(json.ToString()) : null;
        }

        /// <summary>
        /// Converts from JSON document.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>An instance of the JsonObject class.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
        public static implicit operator JsonArray(JsonDocument json)
        {
            return json.RootElement;
        }

        /// <summary>
        /// Converts from JSON element.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>An instance of the JsonObject class.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
        public static implicit operator JsonArray(JsonElement json)
        {
            if (json.ValueKind != JsonValueKind.Array)
            {
                return json.ValueKind switch
                {
                    JsonValueKind.Null => null,
                    JsonValueKind.Undefined => null,
                    _ => throw new JsonException("json is not a JSON array.")
                };
            }

            var result = new JsonArray();
            for (var i = 0; i < json.GetArrayLength(); i++)
            {
                result.Add(json[i]);
            }

            return result;
        }

        /// <summary>
        /// Parses JSON array.
        /// </summary>
        /// <param name="json">A JSON array string.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <returns>A JSON array instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public static JsonArray Parse(string json, JsonDocumentOptions options = default)
        {
            return JsonDocument.Parse(json, options);
        }
    }
}
