using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Reflection;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a specific JSON object.
    /// </summary>
    public class JsonArray : IJsonComplex, IJsonValueResolver, IReadOnlyList<IJsonValue>, IReadOnlyList<IJsonValueResolver>, IEquatable<JsonArray>, IEquatable<IJsonValue>
    {
        private readonly IList<IJsonValueResolver> store = new List<IJsonValueResolver>();

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
        private JsonArray(IList<IJsonValueResolver> copy)
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
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public IJsonValueResolver this[int index] => GetValue(index);

        /// <summary>
        /// Gets the element at the specified index in the array.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        IJsonValue IReadOnlyList<IJsonValue>.this[int index] => GetValue(index);

#if !NETSTANDARD2_0
        /// <summary>
        /// Gets the System.Char object at a specified position in the source value.
        /// </summary>
        /// <param name="index">A position in the current string.</param>
        /// <returns>The character at position index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public IJsonValueResolver this[Index index] => GetValue(index.IsFromEnd ? Count - index.Value : index.Value);

        /// <summary>
        /// Gets the System.Char object at a specified position in the source value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>The character at position index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The range is out of range.</exception>
        public JsonArray this[Range range]
        {
            get
            {
                var startIndex = range.Start;
                var start = startIndex.IsFromEnd ? Count - startIndex.Value : startIndex.Value;
                var endIndex = range.End;
                var end = endIndex.IsFromEnd ? Count - endIndex.Value : endIndex.Value;
                var arr = new List<IJsonValueResolver>();
                for (var i = start; i <= end; i++)
                {
                    arr.Add(GetValue(i));
                }

                return new JsonArray(arr);
            }
        }
#endif

        /// <summary>
        /// Determines the property value of the specific key is null.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>true if there is no such key or the property value is null; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        public bool IsNull(int index)
        {
            var value = store[index];
            return value is null || value.ValueKind == JsonValueKind.Null;
        }

        /// <summary>
        /// Determines the property value of the specific key is null, undefined or nonexisted.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>true if there is no such key or the property value is null; otherwise, false.</returns>
        public bool IsNullOrUndefined(int index)
        {
            if (index < 0 && index >= store.Count) return true;
            try
            {
                var value = store[index];
                return value is null || value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined;
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
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public string GetRawText(int index)
        {
            var data = store[index];
            if (data is null) return null;
            return data.ToString();
        }

#if !NETSTANDARD2_0
        /// <summary>
        /// Gets the raw value of the specific value.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public string GetRawText(Index index)
        {
            var data = store[index.IsFromEnd ? Count - index.Value : index.Value];
            if (data is null) return null;
            return data.ToString();
        }
#endif

        /// <summary>
        /// Gets the value kind of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="strictMode">true if enable strict mode; otherwise, false, to return undefined for non-existing.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        public JsonValueKind GetValueKind(int index, bool strictMode = false)
        {
            if (strictMode)
            {
                var data = store[index];
                if (data is null) return JsonValueKind.Null;
                return data.ValueKind;
            }

            if (index < 0 || index >= store.Count) return JsonValueKind.Undefined;
            try
            {
                var data = store[index];
                if (data is null) return JsonValueKind.Null;
                return data.ValueKind;
            }
            catch (ArgumentException)
            {
            }

            return JsonValueKind.Undefined;
        }

#if !NETSTANDARD2_0
        /// <summary>
        /// Gets the value kind of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="strictMode">true if enable strict mode; otherwise, false, to return undefined for non-existing.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        public JsonValueKind GetValueKind(Index index, bool strictMode = false)
        {
            return GetValueKind(index.IsFromEnd ? store.Count - index.Value : index.Value, strictMode);
        }
#endif

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="convert">true if want to convert to string if it is a number or a boolean; otherwise, false.</param>
        /// <returns>The value. It will be null if the value is null.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public string GetStringValue(int index, bool convert = false)
        {
            var data = store[index];
            if (data is null) return null;
            if (data is JsonString str)
            {
                return str.Value;
            }

            if (convert) return data.ValueKind switch
            {
                JsonValueKind.True => JsonBoolean.TrueString,
                JsonValueKind.False => JsonBoolean.TrueString,
                JsonValueKind.Number => data.ToString(),
                _ => throw new InvalidOperationException($"The type of item {index} should be string but it is {data.ValueKind.ToString().ToLowerInvariant()}.")
            };
            throw new InvalidOperationException($"The type of item {index} should be string but it is {data.ValueKind.ToString().ToLowerInvariant()}.");
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="FormatException">The value is not in a recognized format.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
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
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public uint GetUInt32Value(int index)
        {
            if (TryGetJsonValue<JsonInteger>(index, out var v)) return (uint)v;
            if (TryGetJsonValue<JsonDouble>(index, out var f)) return (uint)f;
            var p = GetJsonValue<JsonString>(index, JsonValueKind.Number);
            return uint.Parse(p.Value);
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public int GetInt32Value(int index)
        {
            if (TryGetJsonValue<JsonInteger>(index, out var v)) return (int)v;
            if (TryGetJsonValue<JsonDouble>(index, out var f)) return (int)f;
            var p = GetJsonValue<JsonString>(index, JsonValueKind.Number);
            return int.Parse(p.Value);
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public long GetInt64Value(int index)
        {
            if (TryGetJsonValue<JsonInteger>(index, out var v)) return v.Value;
            if (TryGetJsonValue<JsonDouble>(index, out var f)) return (long)f;
            var p = GetJsonValue<JsonString>(index, JsonValueKind.Number);
            return long.Parse(p.Value);
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public float GetSingleValue(int index)
        {
            if (TryGetJsonValue<JsonDouble>(index, out var v)) return (float)v;
            if (TryGetJsonValue<JsonInteger>(index, out var f)) return (float)f;
            var p = GetJsonValue<JsonString>(index, JsonValueKind.Number);
            return float.Parse(p.Value);
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public double GetDoubleValue(int index)
        {
            if (TryGetJsonValue<JsonDouble>(index, out var v)) return v.Value;
            if (TryGetJsonValue<JsonInteger>(index, out var f)) return (double)f;
            var p = GetJsonValue<JsonString>(index, JsonValueKind.Number);
            return double.Parse(p.Value);
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public bool GetBooleanValue(int index)
        {
            if (TryGetJsonValue<JsonBoolean>(index, out var v)) return v.Value;
            var p = GetJsonValue<JsonString>(index);
            return p.Value?.ToLower() switch
            {
                JsonBoolean.TrueString => true,
                JsonBoolean.FalseString => false,
                _ => throw new InvalidOperationException($"The type of item {index} should be boolean but it is string.")
            };
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public JsonObject GetObjectValue(int index)
        {
            return GetJsonValue<JsonObject>(index, JsonValueKind.Object);
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public JsonArray GetArrayValue(int index)
        {
            return GetJsonValue<JsonArray>(index, JsonValueKind.Array);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="useUnixTimestampsFallback">true if use Unix timestamp to convert if the value is a number; otherwise, false, to use JavaScript date ticks fallback.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="FormatException">The value is not in a recognized format.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public DateTime GetDateTimeValue(int index, bool useUnixTimestampsFallback = false)
        {
            if (TryGetJsonValue<JsonString>(index, out var s))
            {
                var date = Web.WebFormat.ParseDate(s.Value);
                if (date.HasValue) return date.Value;
                throw new FormatException("The value is not a date time.");
            }

            var num = GetJsonValue<JsonInteger>(index);
            return useUnixTimestampsFallback ? Web.WebFormat.ParseUnixTimestamp(num.Value) : Web.WebFormat.ParseDate(num.Value);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="FormatException">The value is not encoded as Base64 text and hence cannot be decoded to bytes.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public byte[] GetBytesFromBase64(int index)
        {
            var str = GetStringValue(index);
            if (string.IsNullOrEmpty(str)) return Array.Empty<byte>();
            return Convert.FromBase64String(str);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="ArgumentException">The type is not an System.Enum. -or- the value is either an empty string or only contains white space.  -or- value is a name, but not one of the named constants defined for the enumeration.</exception>
        /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public T GetEnumValue<T>(int index) where T : struct, Enum
        {
            if (TryGetInt32Value(index, out var v)) return (T)(object)v;
            var str = GetStringValue(index);
            return ObjectConvert.ParseEnum<T>(str);
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="ArgumentException">The type is not an System.Enum. -or- the value is either an empty string or only contains white space.  -or- value is a name, but not one of the named constants defined for the enumeration.</exception>
        /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
        public T GetEnumValue<T>(int index, bool ignoreCase) where T : struct, Enum
        {
            if (TryGetInt32Value(index, out var v)) return (T)(object)v;
            var str = GetStringValue(index);
            return ObjectConvert.ParseEnum<T>(str, ignoreCase);
        }

        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public IJsonValueResolver GetValue(int index)
        {
            return (store[index] as IJsonValueResolver) ?? JsonValues.Null;
        }

#if !NETSTANDARD2_0
        /// <summary>
        /// Gets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public IJsonValueResolver GetValue(Index index)
        {
            return (store[index.IsFromEnd ? store.Count - index.Value : index.Value] as IJsonValueResolver) ?? JsonValues.Null;
        }
#endif

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(string key)
        {
            if (int.TryParse(key, out var i)) return GetValue(i);
            throw new InvalidOperationException("key should be an integer.", new FormatException("key should be an integer."));
        }

        /// <summary>
        /// Gets the value of the specific property.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        IJsonValueResolver IJsonValueResolver.GetValue(ReadOnlySpan<char> key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), "key should not be null.");
            return (this as IJsonValueResolver).GetValue(key.ToString());
        }

        /// <summary>
        /// Gets the value as a string collection.
        /// </summary>
        /// <returns>The string collection.</returns>
        /// <exception cref="InvalidOperationException">The item value kind is not string.</exception>
        public IEnumerable<string> GetStringCollection()
        {
            foreach (var item in store)
            {
                if (item is null) yield return JsonValues.Null.ToString();
                if (item is IJsonValueResolver ele) yield return ele.GetString();
                else yield return null;
            }
        }

        /// <summary>
        /// Gets the value as a string collection.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The value. It will be null if the value is null.</returns>
        /// <exception cref="InvalidOperationException">The item value kind is not string.</exception>
        public IEnumerable<string> GetStringCollection(Func<IJsonValueResolver, bool> predicate)
        {
            return store.Select(ele =>
            {
                if (ele is null) return JsonValues.Null;
                return ele as IJsonValueResolver;
            }).Where(predicate).Select(ele => ele?.GetString());
        }

        /// <summary>
        /// Gets the value as a string collection.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The value. It will be null if the value is null.</returns>
        /// <exception cref="InvalidOperationException">The item value kind is not string.</exception>
        public IEnumerable<string> GetStringCollection(Func<IJsonValueResolver, int, bool> predicate)
        {
            return store.Select(ele =>
            {
                if (ele is null) return JsonValues.Null;
                return ele as IJsonValueResolver;
            }).Where(predicate).Select(ele => ele?.GetString());
        }

        /// <summary>
        /// Gets all string values.
        /// </summary>
        /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
        /// <returns>The JSON string value collection.</returns>
        public IEnumerable<JsonString> GetStringValues(bool nullForOtherKinds = false)
        {
            return GetSpecificKindValues<JsonString>(nullForOtherKinds);
        }

        /// <summary>
        /// Gets all integer values.
        /// </summary>
        /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
        /// <returns>The JSON integer value collection.</returns>
        public IEnumerable<JsonInteger> GetIntegerValues(bool nullForOtherKinds = false)
        {
            return GetSpecificKindValues<JsonInteger>(nullForOtherKinds);
        }

        /// <summary>
        /// Gets all double float number values.
        /// </summary>
        /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
        /// <returns>The JSON double float number value collection.</returns>
        public IEnumerable<JsonDouble> GetDoubleValues(bool nullForOtherKinds = false)
        {
            return GetSpecificKindValues<JsonDouble>(nullForOtherKinds);
        }

        /// <summary>
        /// Gets all boolean values.
        /// </summary>
        /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
        /// <returns>The JSON boolean value collection.</returns>
        public IEnumerable<JsonBoolean> GetBooleanValues(bool nullForOtherKinds = false)
        {
            return GetSpecificKindValues<JsonBoolean>(nullForOtherKinds);
        }

        /// <summary>
        /// Gets all JSON object values.
        /// </summary>
        /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
        /// <returns>The JSON object value collection.</returns>
        public IEnumerable<JsonObject> GetObjectValues(bool nullForOtherKinds = false)
        {
            return GetSpecificKindValues<JsonObject>(nullForOtherKinds);
        }

        /// <summary>
        /// Gets all JSON array values.
        /// </summary>
        /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
        /// <returns>The JSON array value collection.</returns>
        public IEnumerable<JsonArray> GetArrayValues(bool nullForOtherKinds = false)
        {
            return GetSpecificKindValues<JsonArray>(nullForOtherKinds);
        }

        /// <summary>
        /// Gets the value as a string collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>A string.</returns>
        public string TryGetStringValue(int index)
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
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetStringValue(int index, out string result)
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
            catch (ArgumentException)
            {
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
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
        /// Tries to get the value of the specific property.
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
        /// Tries to get the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public uint? TryGetUInt32Value(int index)
        {
            if (TryGetJsonValue<JsonInteger>(index, out var p1)) return (uint)p1;
            if (TryGetJsonValue<JsonDouble>(index, out var p2)) return (uint)p2;
            var str = TryGetStringValue(index);
            if (string.IsNullOrWhiteSpace(str) || !uint.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value at the specific index.
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
        /// Tries to get the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public int? TryGetInt32Value(int index)
        {
            if (TryGetJsonValue<JsonInteger>(index, out var p1)) return (int)p1;
            if (TryGetJsonValue<JsonDouble>(index, out var p2)) return (int)p2;
            var str = TryGetStringValue(index);
            if (string.IsNullOrWhiteSpace(str) || !int.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value at the specific index.
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
        /// Tries to get the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public long? TryGetInt64Value(int index)
        {
            if (TryGetJsonValue<JsonInteger>(index, out var p1)) return p1.Value;
            if (TryGetJsonValue<JsonDouble>(index, out var p2)) return (long)p2;
            var str = TryGetStringValue(index);
            if (string.IsNullOrWhiteSpace(str) || !long.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value at the specific index.
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
        /// Tries to get the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public float? TryGetFloatValue(int index)
        {
            if (TryGetJsonValue<JsonDouble>(index, out var p1)) return (float)p1;
            if (TryGetJsonValue<JsonInteger>(index, out var p2)) return (float)p2;
            var str = TryGetStringValue(index);
            if (string.IsNullOrWhiteSpace(str) || !float.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value at the specific index.
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
        /// Tries to get the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public double? TryGetDoubleValue(int index)
        {
            if (TryGetJsonValue<JsonDouble>(index, out var p1)) return p1.Value;
            if (TryGetJsonValue<JsonInteger>(index, out var p2)) return (double)p2;
            var str = TryGetStringValue(index);
            if (string.IsNullOrWhiteSpace(str) || !double.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value at the specific index.
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
        /// Tries to get the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value; or null if fail to resolve.</returns>
        public bool? TryGetBooleanValue(int index)
        {
            if (TryGetJsonValue<JsonBoolean>(index, out var p)) return p.Value;
            var str = TryGetStringValue(index);
            if (string.IsNullOrWhiteSpace(str) || !bool.TryParse(str, out var p3)) return null;
            return p3;
        }

        /// <summary>
        /// Tries to get the value at the specific index.
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
        /// Tries to get the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        public JsonObject TryGetObjectValue(int index)
        {
            if (TryGetJsonValue<JsonObject>(index, out var p)) return p;
            return null;
        }

        /// <summary>
        /// Tries to get the value at the specific index.
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
        /// Tries to get the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The value.</returns>
        public JsonArray TryGetArrayValue(int index)
        {
            if (TryGetJsonValue<JsonArray>(index, out var p)) return p;
            return null;
        }

        /// <summary>
        /// Tries to get the value at the specific index.
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
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="useUnixTimestampsFallback">true if use Unix timestamp to convert if the value is a number; otherwise, false, to use JavaScript date ticks fallback.</param>
        /// <returns>The value.</returns>
        public DateTime? TryGetDateTimeValue(int index, bool useUnixTimestampsFallback = false)
        {
            if (TryGetJsonValue<JsonString>(index, out var s))
            {
                var date = Web.WebFormat.ParseDate(s.Value);
                return date;
            }

            if (!TryGetJsonValue<JsonInteger>(index, out var num)) return null;
            return useUnixTimestampsFallback ? Web.WebFormat.ParseUnixTimestamp(num.Value) : Web.WebFormat.ParseDate(num.Value);
        }

#if !NETSTANDARD2_0
        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="bytes">The result.</param>
        /// <param name="bytesWritten">The count of bytes written.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetBytesFromBase64(int index, Span<byte> bytes, out int bytesWritten)
        {
            var str = GetStringValue(index);
            if (string.IsNullOrEmpty(str))
            {
                bytesWritten = 0;
                return false;
            }

            return Convert.TryFromBase64String(str, bytes, out bytesWritten);
        }
#endif

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The enum.</returns>
        public T? TryGetEnumValue<T>(int index) where T : struct, Enum
        {
            if (TryGetEnumValue<T>(index, out var v)) return v;
            return null;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
        /// <returns>The enum.</returns>
        public T? TryGetEnumValue<T>(int index, bool ignoreCase) where T : struct, Enum
        {
            if (TryGetEnumValue<T>(index, ignoreCase, out var v)) return v;
            return null;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetEnumValue<T>(int index, out T result) where T : struct, Enum
        {
            if (TryGetInt32Value(index, out var v))
            {
                result = (T)(object)v;
                return true;
            }

            var str = TryGetStringValue(index);
            if (string.IsNullOrWhiteSpace(str))
            {
                result = default;
                return false;
            }

            if (Enum.TryParse<T>(str, out var obj))
            {
                result = obj;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Tries to get the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
        /// <param name="result">The result.</param>
        /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
        public bool TryGetEnumValue<T>(int index, bool ignoreCase, out T result) where T : struct, Enum
        {
            if (TryGetInt32Value(index, out var v))
            {
                result = (T)(object)v;
                return true;
            }

            var str = TryGetStringValue(index);
            if (string.IsNullOrWhiteSpace(str))
            {
                result = default;
                return false;
            }

            if (Enum.TryParse<T>(str, ignoreCase, out var obj))
            {
                result = obj;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Tries to get the value at the specific index.
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
                return store[index] ?? JsonValues.Null;
            }
            catch (ArgumentException)
            {
            }

            return default;
        }

        /// <summary>
        /// Tries to get the value at the specific index.
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

#if !NETSTANDARD2_0
        /// <summary>
        /// Removes the element at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Remove(Index index)
        {
            store.RemoveAt(index.IsFromEnd ? store.Count - index.Value : index.Value);
        }
#endif

        /// <summary>
        /// Removes the first occurrence of a specific value from the array.
        /// </summary>
        /// <param name="item">The item matched.</param>
        /// <returns>true if item was successfully removed from the array; otherwise, false. This method also returns false if item is not found in the array.</returns>
        public bool Remove(IJsonValue item)
        {
            if (!(item is IJsonValueResolver ele)) return false;
            return store.Remove(ele);
        }

        /// <summary>
        /// Removes all null value.
        /// </summary>
        /// <returns>true if item was successfully removed from the array; otherwise, false. This method also returns false if item is not found in the array.</returns>
        public int RemoveNull()
        {
            var count = 0;
            while (store.Remove(null)) count++;
            return count;
        }

        /// <summary>
        /// Sets null at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetNullValue(int index)
        {
            store[index] = JsonValues.Null;
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, string value)
        {
            store[index] = new JsonString(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, SecureString value)
        {
            store[index] = new JsonString(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValueFormat(int index, string value, params object[] args)
        {
            store[index] = new JsonString(string.Format(value, args));
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="inArray">An array of 8-bit unsigned integers.</param>
        /// <param name="options">A formatting options.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetBase64(int index, byte[] inArray, Base64FormattingOptions options = Base64FormattingOptions.None)
        {
            store[index] = new JsonString(Convert.ToBase64String(inArray, options));
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="bytes">The bytes to convert to base 64 string.</param>
        /// <param name="options">A formatting options.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="ArgumentNullException">The bytes should not be null.</exception>
        public void SetBase64(int index, Span<byte> bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
        {
#if NETSTANDARD2_0
            if (bytes == null) throw new ArgumentNullException(nameof(bytes), "bytes should not be null.");
            store[index] = new JsonString(Convert.ToBase64String(bytes.ToArray(), options));
#else
            store[index] = new JsonString(Convert.ToBase64String(bytes, options));
#endif
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="ArgumentNullException">The bytes should not be null.</exception>
        public void SetValue(int index, Guid value)
        {
            store[index] = new JsonString(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, DateTime value)
        {
            store[index] = new JsonString(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, uint value)
        {
            store[index] = new JsonInteger(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, int value)
        {
            store[index] = new JsonInteger(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, long value)
        {
            store[index] = new JsonInteger(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, float value)
        {
            store[index] = new JsonDouble(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, double value)
        {
            store[index] = new JsonDouble(value);
        }

        /// <summary>
        /// Sets the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetValue(int index, bool value)
        {
            store[index] = value ? JsonBoolean.True : JsonBoolean.False;
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
            store[index] = JsonValues.ToJsonValue(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetDateTimeStringValue(int index, DateTime value)
        {
            store[index] = new JsonString(value);
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetJavaScriptDateTicksValue(int index, DateTime value)
        {
            store[index] = new JsonInteger(Web.WebFormat.ParseDate(value));
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetUnixTimestampValue(int index, DateTime value)
        {
            store[index] = new JsonInteger(Web.WebFormat.ParseUnixTimestamp(value));
        }

        /// <summary>
        /// Sets the value of the specific property.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void SetWindowsFileTimeUtcValue(int index, DateTime value)
        {
            store[index] = new JsonInteger(value.ToFileTimeUtc());
        }

        /// <summary>
        /// Add null.
        /// </summary>
        public void AddNull()
        {
            store.Add(JsonValues.Null);
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(string value)
        {
            store.Add(new JsonString(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(SecureString value)
        {
            store.Add(new JsonString(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void AddFormat(string value, params object[] args)
        {
            store.Add(new JsonString(string.Format(value, args)));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(Guid value)
        {
            store.Add(new JsonString(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(DateTime value)
        {
            store.Add(new JsonString(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(uint value)
        {
            store.Add(new JsonInteger(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(int value)
        {
            store.Add(new JsonInteger(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(long value)
        {
            store.Add(new JsonInteger(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(float value)
        {
            store.Add(new JsonDouble(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(double value)
        {
            store.Add(new JsonDouble(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void Add(bool value)
        {
            store.Add(value ? JsonBoolean.True : JsonBoolean.False);
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
            store.Add(JsonValues.ToJsonValue(value));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="inArray">An array of 8-bit unsigned integers.</param>
        /// <param name="options">A formatting options.</param>
        /// <exception cref="ArgumentNullException">The bytes should not be null.</exception>
        public void AddBase64(byte[] inArray, Base64FormattingOptions options = Base64FormattingOptions.None)
        {
            store.Add(new JsonString(Convert.ToBase64String(inArray, options)));
        }

        /// <summary>
        /// Adds a value.
        /// </summary>
        /// <param name="bytes">The bytes to convert to base 64 string.</param>
        /// <param name="options">A formatting options.</param>
        /// <exception cref="ArgumentNullException">The bytes should not be null.</exception>
        public void AddBase64(Span<byte> bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
        {
#if NETSTANDARD2_0
            if (bytes == null) throw new ArgumentNullException(nameof(bytes), "bytes should not be null.");
            store.Add(new JsonString(Convert.ToBase64String(bytes.ToArray(), options)));
#else
            store.Add(new JsonString(Convert.ToBase64String(bytes, options)));
#endif
        }

        /// <summary>
        /// Adds a date time string.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void AddDateTimeString(DateTime value)
        {
            store.Add(new JsonString(value));
        }

        /// <summary>
        /// Adds a JavaScript date ticks.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void AddJavaScriptDateTicks(DateTime value)
        {
            store.Add(new JsonInteger(Web.WebFormat.ParseDate(value)));
        }

        /// <summary>
        /// Adds a Unix timestamp.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void AddUnixTimestamp(DateTime value)
        {
            store.Add(new JsonInteger(Web.WebFormat.ParseUnixTimestamp(value)));
        }

        /// <summary>
        /// Adds a collection of string.
        /// </summary>
        /// <param name="values">A string collection to add.</param>
        /// <returns>The count of item added.</returns>
        public int AddRange(IEnumerable<string> values)
        {
            var count = 0;
            if (values == null) return count;
            foreach (var item in values)
            {
                store.Add(new JsonString(item));
                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds a collection of integer.
        /// </summary>
        /// <param name="values">An integer collection to add.</param>
        /// <returns>The count of item added.</returns>
        public int AddRange(IEnumerable<int> values)
        {
            var count = 0;
            if (values == null) return count;
            foreach (var item in values)
            {
                store.Add(new JsonInteger(item));
                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds a collection of JSON object.
        /// </summary>
        /// <param name="values">A JSON object collection to add.</param>
        /// <returns>The count of item added.</returns>
        public int AddRange(IEnumerable<JsonObject> values)
        {
            var count = 0;
            if (values == null) return count;
            foreach (var item in values)
            {
                store.Add(item);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds a JSON array.
        /// </summary>
        /// <param name="json">Another JSON array to add.</param>
        /// <returns>The count of item added.</returns>
        public int AddRange(JsonArray json)
        {
            var count = 0;
            if (json == null) return count;
            if (json == this) json = json.Clone();
            foreach (var props in json)
            {
                store.Add(props);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds a set of value from a JSON object.
        /// </summary>
        /// <param name="json">A JSON object to copy its properties to add.</param>
        /// <param name="propertyKeys">A sort of property keys to copy.</param>
        /// <returns>The count of item added.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public int AddRange(JsonObject json, IEnumerable<string> propertyKeys)
        {
            var count = 0;
            if (json == null || propertyKeys == null) return count;
            foreach (var key in propertyKeys)
            {
                store.Add(json.TryGetValue(key) ?? JsonValues.Null);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Inserts null at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void InsertNull(int index)
        {
            store.Insert(index, JsonValues.Null);
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, string value)
        {
            store.Insert(index, new JsonString(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, SecureString value)
        {
            store.Insert(index, new JsonString(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void InsertFormat(int index, string value, params object[] args)
        {
            store.Insert(index, new JsonString(string.Format(value, args)));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, Guid value)
        {
            store.Insert(index, new JsonString(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, DateTime value)
        {
            store.Insert(index, new JsonString(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, uint value)
        {
            store.Insert(index, new JsonInteger(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, int value)
        {
            store.Insert(index, new JsonInteger(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, long value)
        {
            store.Insert(index, new JsonInteger(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, float value)
        {
            store.Insert(index, new JsonDouble(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, double value)
        {
            store.Insert(index, new JsonDouble(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void Insert(int index, bool value)
        {
            store.Insert(index, value ? JsonBoolean.True : JsonBoolean.False);
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
            store.Insert(index, JsonValues.ToJsonValue(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="inArray">An array of 8-bit unsigned integers.</param>
        /// <param name="options">A formatting options.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="ArgumentNullException">The bytes should not be null.</exception>
        public void InsertBase64(int index, byte[] inArray, Base64FormattingOptions options = Base64FormattingOptions.None)
        {
            store.Insert(index, new JsonString(Convert.ToBase64String(inArray, options)));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="bytes">The bytes to convert to base 64 string.</param>
        /// <param name="options">A formatting options.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="ArgumentNullException">The bytes should not be null.</exception>
        public void InsertBase64(int index, Span<byte> bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
        {
#if NETSTANDARD2_0
            if (bytes == null) throw new ArgumentNullException(nameof(bytes), "bytes should not be null.");
            store.Insert(index, new JsonString(Convert.ToBase64String(bytes.ToArray(), options)));
#else
            store.Insert(index, new JsonString(Convert.ToBase64String(bytes, options)));
#endif
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void InsertDateTimeString(int index, DateTime value)
        {
            store.Insert(index, new JsonString(value));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void InsertJavaScriptDateTicks(int index, DateTime value)
        {
            store.Insert(index, new JsonInteger(Web.WebFormat.ParseDate(value)));
        }

        /// <summary>
        /// Inserts the value at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public void InsertUnixTimestamp(int index, DateTime value)
        {
            store.Insert(index, new JsonInteger(Web.WebFormat.ParseUnixTimestamp(value)));
        }

        /// <summary>
        /// Adds a JSON array.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="values">A string collection to add.</param>
        /// <returns>The count of item added.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public int InsertRange(int index, IEnumerable<string> values)
        {
            var count = 0;
            if (values == null) return count;
            foreach (var item in values)
            {
                store.Insert(index + count, new JsonString(item));
                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds a JSON array.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="values">A string collection to add.</param>
        /// <returns>The count of item added.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public int InsertRange(int index, IEnumerable<int> values)
        {
            var count = 0;
            if (values == null) return count;
            foreach (var item in values)
            {
                store.Insert(index + count, new JsonInteger(item));
                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds a JSON array.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="values">A string collection to add.</param>
        /// <returns>The count of item added.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public int InsertRange(int index, IEnumerable<JsonObject> values)
        {
            var count = 0;
            if (values == null) return count;
            foreach (var item in values)
            {
                store.Insert(index + count, item);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds a JSON array.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="json">Another JSON array to add.</param>
        /// <returns>The count of item added.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public int InsertRange(int index, JsonArray json)
        {
            var count = 0;
            if (json == null) return count;
            if (json == this) json = json.Clone();
            foreach (var item in json)
            {
                store.Insert(index + count, item);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds a set of value from a JSON object.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="json">A JSON object to copy its properties to add.</param>
        /// <param name="propertyKeys">A sort of property keys to copy.</param>
        /// <returns>The count of item added.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        public int InsertRange(int index, JsonObject json, IEnumerable<string> propertyKeys)
        {
            var count = 0;
            if (json == null || propertyKeys == null) return count;
            foreach (var key in propertyKeys)
            {
                store.Insert(index + count, json.TryGetValue(key) ?? JsonValues.Null);
                count++;
            }

            return count;
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
                        if (prop is JsonString strJson) writer.WriteStringValue(strJson.Value);
                        break;
                    case JsonValueKind.Number:
                        if (prop is JsonInteger intJson) writer.WriteNumberValue((long)intJson);
                        else if (prop is JsonDouble floatJson) writer.WriteNumberValue((double)floatJson);
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
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
        /// <exception cref="JsonException">The JSON is invalid. -or- TValue is not compatible with the JSON.</exception>
        public T DeserializeValue<T>(int index, JsonSerializerOptions options = default)
        {
            var item = store[index];
            if (item is null || item.ValueKind == JsonValueKind.Null || item.ValueKind == JsonValueKind.Undefined)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(item.ToString(), options);
        }

        /// <summary>
        /// Gets the JSON value kind groups.
        /// </summary>
        /// <returns>A dictionary of JSON value kind summary.</returns>
        public IDictionary<JsonValueKind, List<int>> GetJsonValueKindGroups()
        {
            var dict = new Dictionary<JsonValueKind, List<int>>
            {
                { JsonValueKind.Array, new List<int>() },
                { JsonValueKind.False, new List<int>() },
                { JsonValueKind.Null, new List<int>() },
                { JsonValueKind.Number, new List<int>() },
                { JsonValueKind.Object, new List<int>() },
                { JsonValueKind.String, new List<int>() },
                { JsonValueKind.True, new List<int>() },
            };
            var i = -1;
            foreach (var item in store)
            {
                i++;
                var valueKind = item?.ValueKind ?? JsonValueKind.Null;
                if (valueKind == JsonValueKind.Undefined) continue;
                dict[valueKind].Add(i);
            }

            return dict;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(JsonArray other)
        {
            if (other is null) return false;
            if (base.Equals(other)) return true;
            if (other.Count != Count) return false;
            for (var i = 0; i < Count; i++)
            {
                var isNull = !other.TryGetValue(i, out var r) || r is null || r.ValueKind == JsonValueKind.Null || r.ValueKind == JsonValueKind.Undefined;
                var prop = store[i];
                if (prop is null || prop.ValueKind == JsonValueKind.Null || prop.ValueKind == JsonValueKind.Undefined)
                    return isNull;
                if (isNull) return false;
                JsonValues.Equals(prop, r);
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
            if (other is JsonArray json) return Equals(json);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode() => store.GetHashCode();

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <returns>A JSON format string.</returns>
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
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <param name="indentStyle">The indent style.</param>
        /// <returns>A JSON format string.</returns>
        public string ToString(IndentStyles indentStyle)
        {
            return ToString(indentStyle, 0);
        }

        /// <summary>
        /// Gets the JSON format string of the value.
        /// </summary>
        /// <param name="indentStyle">The indent style.</param>
        /// <param name="indentLevel">The current indent level.</param>
        /// <returns>A JSON format string.</returns>
        internal string ToString(IndentStyles indentStyle, int indentLevel)
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
            var str = new StringBuilder();
            indentLevel++;
            str.Append("[");
            foreach (var prop in store)
            {
                str.AppendLine();
                str.Append(indentStr);
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
                    case JsonValueKind.Array:
                        str.Append((prop is JsonArray jArr) ? jArr.ToString(indentStyle, indentLevel) : "[]");
                        break;
                    case JsonValueKind.Object:
                        str.Append((prop is JsonObject jObj) ? jObj.ConvertToString(indentStyle, indentLevel) : "{}");
                        break;
                    default:
                        str.Append(prop.ToString());
                        break;
                }

                str.Append(',');
            }

            if (str.Length > 1) str.Remove(str.Length - 1, 1);
            str.AppendLine();
            str.Append(indentStr2);
            str.Append(']');
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
            indentLevel++;
            var str = new StringBuilder();
            foreach (var item in store)
            {
                str.Append(indentStr);
                str.Append("- ");
                if (item is null)
                {
                    str.AppendLine("!!null null");
                    continue;
                }

                switch (item.ValueKind)
                {
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Null:
                        str.AppendLine("!!null null");
                        break;
                    case JsonValueKind.Array:
                        if (!(item is JsonArray jArr))
                        {
                            str.AppendLine("[]");
                            break;
                        }

                        str.AppendLine(jArr.ToString());
                        break;
                    case JsonValueKind.Object:
                        if (!(item is JsonObject jObj))
                        {
                            str.AppendLine("{}");
                            break;
                        }

                        str.AppendLine();
                        str.Append(jObj.ConvertToYamlString(indentLevel));
                        break;
                    case JsonValueKind.String:
                        if (!(item is IJsonString jStr))
                        {
                            str.AppendLine(item.ToString());
                            break;
                        }

                        var text = jStr.StringValue;
                        if (text == null)
                        {
                            str.AppendLine("!!null null");
                            break;
                        }

                        str.AppendLine(text.Length == 0 || text.Length > 100 || text.IndexOfAny(StringExtensions.YamlSpecialChars) >= 0
                            ? JsonString.ToJson(text)
                            : text);
                        break;
                    default:
                        str.AppendLine(item.ToString());
                        break;
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the array.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the array.</returns>
        public IEnumerator<IJsonValueResolver> GetEnumerator()
        {
            return store.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the array.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the array.</returns>
        IEnumerator<IJsonValue> IEnumerable<IJsonValue>.GetEnumerator()
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
        /// Gets the value of the element as a boolean.
        /// </summary>
        /// <returns>The value of the element as a boolean.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        bool IJsonValueResolver.GetBoolean() => throw new InvalidOperationException("Expect a boolean but it is an array.");

        /// <summary>
        /// Gets the value of the element as a byte array.
        /// </summary>
        /// <returns>The value decoded as a byte array.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        byte[] IJsonValueResolver.GetBytesFromBase64() => throw new InvalidOperationException("Expect a string but it is an array.");

        /// <summary>
        /// Gets the value of the element as a date time.
        /// </summary>
        /// <returns>The value of the element as a date time.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        DateTime IJsonValueResolver.GetDateTime() => throw new InvalidOperationException("Expect a date time but it is an array.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        decimal IJsonValueResolver.GetDecimal() => throw new InvalidOperationException("Expect a number but it is an array.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        float IJsonValueResolver.GetSingle() => throw new InvalidOperationException("Expect a number but it is an array.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        double IJsonValueResolver.GetDouble() => throw new InvalidOperationException("Expect a number but it is an array.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        short IJsonValueResolver.GetInt16() => throw new InvalidOperationException("Expect a number but it is an array.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        uint IJsonValueResolver.GetUInt32() => throw new InvalidOperationException("Expect a number but it is an array.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        int IJsonValueResolver.GetInt32() => throw new InvalidOperationException("Expect a number but it is an array.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        long IJsonValueResolver.GetInt64() => throw new InvalidOperationException("Expect a number but it is an array.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        string IJsonValueResolver.GetString() => throw new InvalidOperationException("Expect a string but it is an array.");

        /// <summary>
        /// Gets the value of the element as a number.
        /// </summary>
        /// <returns>The value of the element as a number.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
        Guid IJsonValueResolver.GetGuid() => throw new InvalidOperationException("Expect a string but it is an array.");

        /// <summary>
        /// Gets all property keys.
        /// </summary>
        /// <returns>The property keys.</returns>
        /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
        IEnumerable<string> IJsonValueResolver.GetKeys() => throw new InvalidOperationException("Expect an object but it is a number.");

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

        /// <summary>
        /// Compares two instances to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(JsonArray leftValue, IJsonValue rightValue)
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
        public static bool operator !=(JsonArray leftValue, IJsonValue rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null || leftValue is null) return true;
            return !leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Gets all values of specific kind.
        /// </summary>
        /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
        /// <returns>The JSON value collection.</returns>
        private IEnumerable<T> GetSpecificKindValues<T>(bool nullForOtherKinds) where T : IJsonValueResolver
        {
            if (nullForOtherKinds)
            {
                foreach (var item in store)
                {
                    yield return item is T ele ? ele : default;
                }
            }
            else
            {
                foreach (var item in store)
                {
                    if (item is T ele && item.ValueKind != JsonValueKind.Null) yield return ele;
                }
            }
        }

        private T GetJsonValue<T>(int index, JsonValueKind? valueKind = null) where T : IJsonValue
        {
            var data = store[index];
            if (data is null) throw new InvalidOperationException($"The item {index} is null.");
            if (data is T v)
            {
                return v;
            }

            throw new InvalidOperationException(valueKind.HasValue
                ? $"The kind of item {index} should be {valueKind.Value.ToString().ToLowerInvariant()} but it is {data.ValueKind.ToString().ToLowerInvariant()}."
                : $"The kind of item {index} is {data.ValueKind.ToString().ToLowerInvariant()}, not expected.");
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
        /// <returns>An instance of the JsonArray class.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
        public static implicit operator JsonArray(JsonDocument json)
        {
            return json.RootElement;
        }

        /// <summary>
        /// Converts from JSON element.
        /// </summary>
        /// <param name="json">The JSON value.</param>
        /// <returns>An instance of the JsonArray class.</returns>
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

        /// <summary>
        /// Parses a stream as UTF-8-encoded data representing a JSON array.
        /// The stream is read to completio
        /// </summary>
        /// <param name="utf8Json">The JSON data to parse.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public static JsonArray Parse(Stream utf8Json, JsonDocumentOptions options = default)
        {
            return JsonDocument.Parse(utf8Json, options);
        }

        /// <summary>
        /// Parses a stream as UTF-8-encoded data representing a JSON array.
        /// The stream is read to completion.
        /// </summary>
        /// <param name="utf8Json">The JSON data to parse.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public static async Task<JsonArray> ParseAsync(Stream utf8Json, JsonDocumentOptions options = default, CancellationToken cancellationToken = default)
        {
            return await JsonDocument.ParseAsync(utf8Json, options, cancellationToken);
        }

        /// <summary>
        /// Parses JSON array.
        /// </summary>
        /// <param name="reader">A JSON object.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
        /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
        public static JsonArray ParseValue(ref Utf8JsonReader reader)
        {
            return JsonDocument.ParseValue(ref reader);
        }

        /// <summary>
        /// Converts an object to JSON object.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <returns>A JSON object instance.</returns>
        /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
        public static JsonArray ConvertFrom(object obj, JsonSerializerOptions options = default)
        {
            if (obj is null) return null;
            if (obj is IJsonValue)
            {
                if (obj is JsonArray jArr) return jArr;
                if (obj is JsonObject jObj) return new JsonArray { jObj };
                if (obj is IJsonString jStr) return new JsonArray { jStr.StringValue };
                if (obj is IJsonValue<string> jStr2) return new JsonArray { jStr2.Value };
                if (obj is IJsonValue<bool> jBool) return new JsonArray { jBool.Value };
                if (obj is JsonInteger jInt) return new JsonArray { jInt.Value };
                if (obj is JsonDouble jFloat) return new JsonArray { jFloat.Value };
                if (obj is JsonNull) return null;
                var jValue = obj as IJsonValue;
                var valueKind = jValue.ValueKind;
                switch (valueKind)
                {
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        return null;
                    case JsonValueKind.False:
                        return new JsonArray { false };
                    case JsonValueKind.True:
                        return new JsonArray { true };
                    case JsonValueKind.Number:
                        return new JsonArray { jValue.ToString() };
                }
            }

            if (obj is JsonDocument doc) return doc;
            if (obj is string str) return Parse(str);
            if (obj is StringBuilder sb) return Parse(sb.ToString());
            if (obj is Stream stream) return Parse(stream);
            if (obj is IEnumerable<object> arr)
            {
                var r = new JsonArray();
                foreach (var item in arr)
                {
                    r.Add(ConvertFrom(item));
                }

                return r;
            }

            if (obj is IEnumerable<JsonObject> arr2)
            {
                var r = new JsonArray();
                foreach (var item in arr2)
                {
                    r.Add(item);
                }

                return r;
            }

            var s = JsonSerializer.Serialize(obj, obj.GetType(), options);
            return Parse(s);
        }
    }
}
