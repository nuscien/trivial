﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Data;
using Trivial.Maths;
using Trivial.Reflection;
using Trivial.Web;

namespace Trivial.Text;

/// <summary>
/// Represents a specific JSON array.
/// </summary>
[Serializable]
[System.Text.Json.Serialization.JsonConverter(typeof(JsonObjectNodeConverter))]
public class JsonArrayNode : IJsonContainerNode, IJsonDataNode, IReadOnlyList<IJsonValueNode>, IReadOnlyList<IJsonDataNode>, IEquatable<JsonArrayNode>, IEquatable<IJsonValueNode>, ISerializable
{
    private IList<IJsonDataNode> store = new List<IJsonDataNode>();

    /// <summary>
    /// Initializes a new instance of the JsonArrayNode class.
    /// </summary>
    public JsonArrayNode()
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonArrayNode class.
    /// </summary>
    /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
    protected JsonArrayNode(SerializationInfo info, StreamingContext context)
    {
        if (info is null) return;
        var dict = new Dictionary<int, object>();
        foreach (var prop in info)
        {
            if (string.IsNullOrWhiteSpace(prop.Name) || !int.TryParse(prop.Name, out var i)) continue;
            dict[i] = prop.Value;
        }

        var max = dict.Keys.Max();
        for (var i = 0; i <= max; i++)
        {
            if (!dict.TryGetValue(i, out var v) || v is null)
            {
                AddNull();
            }
            else if (v.GetType().IsValueType)
            {
                if (v is long l)
                    Add(l);
                else if (v is int i2)
                    Add(i2);
                else if (v is uint ui)
                    Add(ui);
                else if (v is double d)
                    Add(d);
                else if (v is float f)
                    Add(f);
                else if (v is decimal de)
                    Add(de);
                else if (v is DateTime dt)
                    Add(dt);
                else if (v is Guid g)
                    Add(g);
                else if (v is JsonElement ele)
                    Add(ele);
            }
            else if (v is string s)
            {
                Add(s);
            }
            else if (v is JsonObjectNode json)
            {
                Add(json);
            }
            else if (v is JsonArrayNode arr)
            {
                Add(arr);
            }
            else if (v is System.Text.Json.Nodes.JsonObject json2)
            {
                Add(json2);
            }
            else if (v is System.Text.Json.Nodes.JsonArray arr2)
            {
                Add(arr2);
            }
            else
            {
                AddNull();
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the JsonArrayNode class.
    /// </summary>
    /// <param name="copy">Properties to initialzie.</param>
    /// <param name="threadSafe">true if enable thread-safe; otherwise, false.</param>
    private JsonArrayNode(IList<IJsonDataNode> copy, bool threadSafe = false)
    {
        if (threadSafe) store = new SynchronizedList<IJsonDataNode>();
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
    /// Gets the number of elements contained in the array.
    /// </summary>
    public int Length => store.Count;

    /// <summary>
    /// Gets the element at the specified index in the array.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index in the array.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public IJsonDataNode this[int index] => GetValue(index);

    /// <summary>
    /// Gets the element at the specified index in the array.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index in the array.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    IJsonValueNode IReadOnlyList<IJsonValueNode>.this[int index] => GetValue(index);

#if !NETFRAMEWORK
    /// <summary>
    /// Gets the System.Char object at a specified position in the source value.
    /// </summary>
    /// <param name="index">A position in the current string.</param>
    /// <returns>The character at position index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public IJsonDataNode this[Index index] => GetValue(index.IsFromEnd ? Count - index.Value : index.Value);

    /// <summary>
    /// Gets the System.Char object at a specified position in the source value.
    /// </summary>
    /// <param name="range">The range.</param>
    /// <returns>The character at position index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The range is out of range.</exception>
    public JsonArrayNode this[Range range]
    {
        get
        {
            var startIndex = range.Start;
            var start = startIndex.IsFromEnd ? Count - startIndex.Value : startIndex.Value;
            var endIndex = range.End;
            var end = endIndex.IsFromEnd ? Count - endIndex.Value : endIndex.Value;
            var arr = new List<IJsonDataNode>();
            for (var i = start; i <= end; i++)
            {
                arr.Add(GetValue(i));
            }

            return new JsonArrayNode(arr);
        }
    }
#endif

    /// <summary>
    /// Gets the element at the specified index in the array.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="subKey">The optional sub-property key of the value of the array.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public IJsonDataNode this[int index, string subKey, params string[] keyPath]
    {
        get
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "index was less than zero.");
            var result = store[index];
            if (result is JsonObjectNode json)
            {
                var keyArr = new List<string> { subKey };
                if (keyPath != null) keyArr.AddRange(keyPath);
                return json.GetValue(keyArr);
            }

            var keyPath2 = keyPath?.Where(ele => ele != null)?.ToList() ?? new List<string>();
            if (result is JsonArrayNode arr)
            {
                if (subKey != null)
                {
                    result = arr.TryGetValue(subKey);
                    if (result is null) throw new InvalidOperationException($"The element at {index} should be a JSON object but it is a JSON array; or subKey should be a natural number.");
                    if (keyPath2.Count == 0) return result;
                    if (result is JsonObjectNode json2) return json2.GetValue(keyPath2);
                    if (result is JsonArrayNode arr2) arr = arr2;
                    else throw new InvalidOperationException($"The element at {index}.{subKey} should be a JSON object or array, but its kind is {result?.ValueKind ?? JsonValueKind.Null}.");
                }

                if (int.TryParse(keyPath2[0] ?? string.Empty, out index))
                {
                    if (keyPath2.Count == 1) return arr[index];
                    try
                    {
                        return arr[index, keyPath[1], keyPath.Skip(2).ToArray()];
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidOperationException($"Get value failed.", ex);
                    }
                }
            }

            if (subKey == null && keyPath2.Count == 0) return result ?? JsonValues.Null;
            throw new InvalidOperationException($"The element at {index} should be a JSON object or array, but its kind is {result?.ValueKind ?? JsonValueKind.Null}.");
        }
    }

    /// <summary>
    /// Gets the element at the specified index in the array.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="subIndex">The optional sub-index of the value of the array.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The indexor subIndex is out of range.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public IJsonDataNode this[int index, int subIndex, params string[] keyPath]
    {
        get
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "index was less than zero.");
            if (subIndex < 0) throw new ArgumentOutOfRangeException(nameof(subIndex), "subIndex was less than zero.");
            var result = store[index];
            if (result is JsonObjectNode json)
            {
                if (json.Count == 1 && json.TryGetStringValue("$ref") == JsonValues.SELF_REF)
                {
                    result = this;
                }
                else
                {
                    var keyArr = new List<string> { subIndex.ToString("g") };
                    if (keyPath != null) keyArr.AddRange(keyPath);
                    return json.GetValue(keyArr);
                }
            }

            var keyPath2 = keyPath?.Where(ele => ele != null)?.ToList() ?? new List<string>();
            if (result is JsonArrayNode arr)
            {
                result = arr[subIndex];
                if (keyPath2.Count == 0) return result ?? JsonValues.Null;
                if (result is JsonObjectNode json2) return json2.GetValue(keyPath2);
                if (result is JsonArrayNode arr2) arr = arr2;
                else throw new InvalidOperationException($"The element at {index}.{subIndex} should be a JSON object, but its kind is {result?.ValueKind ?? JsonValueKind.Null}.");

                if (int.TryParse(keyPath2[0] ?? string.Empty, out index))
                {
                    if (keyPath2.Count == 1) return arr[index];
                    try
                    {
                        return arr[index, keyPath[1], keyPath.Skip(2).ToArray()];
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidOperationException($"Get value failed.", ex);
                    }
                }
            }

            throw new InvalidOperationException($"The element at {index} should be a JSON object, but its kind is {result?.ValueKind ?? JsonValueKind.Null}.");
        }
    }

    /// <summary>
    /// Enables thread-safe (concurrent) mode.
    /// </summary>
    public void EnableThreadSafeMode()
    {
        EnableThreadSafeMode(1);
        EnableThreadSafeMode(0, true);
    }

    /// <summary>
    /// Enables thread-safe (concurrent) mode.
    /// </summary>
    /// <param name="depth">The recurrence depth.</param>
    /// <param name="skipIfEnabled">true if skip if this instance is in thread-safe (concurrent) mode; otherwise, false.</param>
    public void EnableThreadSafeMode(int depth, bool skipIfEnabled = false)
    {
        if (store is SynchronizedList<IJsonDataNode>)
        {
            if (skipIfEnabled) return;
        }
        else
        {
            if (depth < 0) return;
            var i = 3;
            while (i > 0)
            {
                try
                {
                    store = new SynchronizedList<IJsonDataNode>(store);
                    break;
                }
                catch (ArgumentException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (NullReferenceException)
                {
                }
                catch (NotSupportedException)
                {
                    break;
                }

                i--;
            }
        }

        if (depth < 1) return;
        depth--;
        foreach (var ele in store)
        {
            if (ele is JsonObjectNode json) json.EnableThreadSafeMode(depth);
            else if (ele is JsonArrayNode arr) arr.EnableThreadSafeMode(depth);
        }
    }

    /// <summary>
    /// Populates a SerializationInfo with the data needed to serialize the target object.
    /// </summary>
    /// <param name="info">The SerializationInfo to populate with data.</param>
    /// <param name="context">The destination for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        var i = -1;
        foreach (var prop in store)
        {
            i++;
            switch (prop.ValueKind)
            {
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    break;
                case JsonValueKind.String:
                    if (prop is JsonStringNode strJson) info.AddValue(i.ToString("g", CultureInfo.InvariantCulture), strJson.Value, typeof(string));
                    break;
                case JsonValueKind.Number:
                    if (prop is JsonIntegerNode intJson) info.AddValue(i.ToString("g", CultureInfo.InvariantCulture), intJson.Value);
                    else if (prop is JsonDoubleNode floatJson) info.AddValue(i.ToString("g", CultureInfo.InvariantCulture), floatJson.Value);
                    break;
                case JsonValueKind.True:
                    info.AddValue(i.ToString("g", CultureInfo.InvariantCulture), true);
                    break;
                case JsonValueKind.False:
                    info.AddValue(i.ToString("g", CultureInfo.InvariantCulture), false);
                    break;
                case JsonValueKind.Object:
                    if (prop is JsonObjectNode objJson) info.AddValue(i.ToString("g", CultureInfo.InvariantCulture), objJson, typeof(JsonObjectNode));
                    break;
                case JsonValueKind.Array:
                    if (prop is JsonArrayNode objArr) info.AddValue(i.ToString("g", CultureInfo.InvariantCulture), objArr, typeof(JsonArrayNode));
                    break;
            }
        }
    }

    /// <summary>
    /// Determines the item value of the specific index is null.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>true if the item value is null; otherwise, false.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index does not exist.</exception>
    public bool IsNull(int index)
    {
        var value = store[index];
        return value is null || value.ValueKind == JsonValueKind.Null;
    }

    /// <summary>
    /// Determines the item value of the specific index is null, undefined or nonexisted.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>true if there is no such index or the item value is null; otherwise, false.</returns>
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
    /// Tests if the value kind of the property is the specific one.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="kind">The value kind expected.</param>
    /// <returns>true if the value kind is the specific one; otherwise, false.</returns>
    public bool IsValueKind(int index, JsonValueKind kind)
    {
        if (index < 0 && index >= store.Count) return kind == JsonValueKind.Undefined;
        try
        {
            var value = store[index];
            if (value is not null) return value.ValueKind == kind;
        }
        catch (ArgumentException)
        {
        }

        return kind == JsonValueKind.Undefined;
    }

    /// <summary>
    /// Determines whether it contains an item value with the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>true if there is no such index; otherwise, false.</returns>
    public bool Contains(int index)
        => index >= 0 && index < store.Count;

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

#if !NETFRAMEWORK
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
    /// Gets the value kind of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="strictMode">true if enable strict mode; otherwise, false, to return undefined for non-existing.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index does not exist.</exception>
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

#if !NETFRAMEWORK
    /// <summary>
    /// Gets the value kind of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="strictMode">true if enable strict mode; otherwise, false, to return undefined for non-existing.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index does not exist.</exception>
    public JsonValueKind GetValueKind(Index index, bool strictMode = false)
        => GetValueKind(index.IsFromEnd ? store.Count - index.Value : index.Value, strictMode);
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
        if (data is JsonStringNode str)
        {
            return str.Value;
        }

        if (convert) return data.ValueKind switch
        {
            JsonValueKind.True => JsonBooleanNode.TrueString,
            JsonValueKind.False => JsonBooleanNode.FalseString,
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
        => Guid.Parse(GetStringValue(index));

    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public uint GetUInt32Value(int index)
    {
        if (TryGetJsonValue<JsonIntegerNode>(index, out var v)) return (uint)v;
        if (TryGetJsonValue<JsonDoubleNode>(index, out var f)) return (uint)f;
        var p = GetJsonValue<JsonStringNode>(index, JsonValueKind.Number);
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
        if (TryGetJsonValue<JsonIntegerNode>(index, out var v)) return (int)v;
        if (TryGetJsonValue<JsonDoubleNode>(index, out var f)) return (int)f;
        var p = GetJsonValue<JsonStringNode>(index, JsonValueKind.Number);
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
        if (TryGetJsonValue<JsonIntegerNode>(index, out var v)) return v.Value;
        if (TryGetJsonValue<JsonDoubleNode>(index, out var f)) return (long)f;
        var p = GetJsonValue<JsonStringNode>(index, JsonValueKind.Number);
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
        if (TryGetJsonValue<JsonDoubleNode>(index, out var v)) return (float)v;
        if (TryGetJsonValue<JsonIntegerNode>(index, out var f)) return (float)f;
        var p = GetJsonValue<JsonStringNode>(index, JsonValueKind.Number);
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
        if (TryGetJsonValue<JsonDoubleNode>(index, out var v)) return v.Value;
        if (TryGetJsonValue<JsonIntegerNode>(index, out var f)) return (double)f;
        var p = GetJsonValue<JsonStringNode>(index, JsonValueKind.Number);
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
        if (TryGetJsonValue<JsonBooleanNode>(index, out var v)) return v.Value;
        var p = GetJsonValue<JsonStringNode>(index);
        return p.Value?.ToLower() switch
        {
            JsonBooleanNode.TrueString => true,
            JsonBooleanNode.FalseString => false,
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
    public JsonObjectNode GetObjectValue(int index)
        => GetJsonValue<JsonObjectNode>(index, JsonValueKind.Object);

    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public JsonArrayNode GetArrayValue(int index)
    {
        try
        {
            return GetJsonValue<JsonArrayNode>(index, JsonValueKind.Array);
        }
        catch (InvalidOperationException ex)
        {
            try
            {
                var obj = GetJsonValue<JsonObjectNode>(index);
                if (obj.Count == 1 && obj.TryGetStringValue("$ref") == JsonValues.SELF_REF) return this;
                throw;
            }
            catch (Exception)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// Gets the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="useUnixTimestampsFallback">true if use Unix timestamp to convert if the value is a number; otherwise, false, to use JavaScript date ticks fallback.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="FormatException">The value is not in a recognized format.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public DateTime GetDateTimeValue(int index, bool useUnixTimestampsFallback = false)
    {
        if (TryGetJsonValue<JsonStringNode>(index, out var s))
        {
            var date = Web.WebFormat.ParseDate(s.Value);
            if (date.HasValue) return date.Value;
            throw new FormatException("The value is not a date time.");
        }

        var num = GetJsonValue<JsonIntegerNode>(index);
        return useUnixTimestampsFallback ? Web.WebFormat.ParseUnixTimestamp(num.Value) : Web.WebFormat.ParseDate(num.Value);
    }

    /// <summary>
    /// Gets the value of the specific index.
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
    /// Gets the value of the specific index.
    /// </summary>
    /// <typeparam name="T">An enumeration type.</typeparam>
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
    /// Gets the value of the specific index.
    /// </summary>
    /// <typeparam name="T">An enumeration type.</typeparam>
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
    /// Gets the value of the specific index.
    /// </summary>
    /// <param name="type">An enumeration type.</param>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index does not exist.</exception>
    /// <exception cref="ArgumentException">The type is not an System.Enum. -or- the value is either an empty string or only contains white space.  -or- value is a name, but not one of the named constants defined for the enumeration.</exception>
    /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public object GetEnumValue(Type type, int index)
    {
        if (TryGetInt32Value(index, out var v)) return type is null ? v : Enum.ToObject(type, v);
        var str = GetStringValue(index);
        return type is null ? str : Enum.Parse(type, str);
    }

    /// <summary>
    /// Gets the value of the specific index.
    /// </summary>
    /// <param name="type">An enumeration type.</param>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index does not exist.</exception>
    /// <exception cref="ArgumentException">The type is not an System.Enum. -or- the value is either an empty string or only contains white space.  -or- value is a name, but not one of the named constants defined for the enumeration.</exception>
    /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public object GetEnumValue(Type type, int index, bool ignoreCase)
    {
        if (TryGetInt32Value(index, out var v)) return type is null ? v : Enum.ToObject(type, v);
        var str = GetStringValue(index);
        return type is null ? str : Enum.Parse(type, str, ignoreCase);
    }

    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public IJsonDataNode GetValue(int index)
        => store[index] ?? JsonValues.Null;

#if !NETFRAMEWORK
    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public IJsonDataNode GetValue(Index index)
        => store[index.IsFromEnd ? store.Count - index.Value : index.Value] ?? JsonValues.Null;
#endif

    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="type">The type of value.</param>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index does not exist.</exception>
    /// <exception cref="InvalidOperationException">The type is not supported to convert.</exception>
    public object GetValue(Type type, int index)
    {
        if (type == null) return null;
        if (type == typeof(string)) return GetStringValue(index);
        if (type.IsEnum) return type == typeof(JsonValueKind) ? GetValueKind(index) : GetEnumValue(type, index, false);
        if (type.IsValueType)
        {
            var kind = GetValueKind(index);
            if (kind == JsonValueKind.Null || kind == JsonValueKind.Undefined)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) return null;
                throw new InvalidOperationException("The type is value type but the value is null or undefined.", new InvalidCastException("Cannot cast null to a struct."));
            }

            if (type == typeof(int)) return GetInt32Value(index);
            if (type == typeof(long)) return GetInt64Value(index);
            if (type == typeof(bool)) return GetBooleanValue(index);
            if (type == typeof(double)) return GetDoubleValue(index);
            if (type == typeof(float)) return GetSingleValue(index);
            if (type == typeof(decimal)) return (decimal)GetDoubleValue(index);
            if (type == typeof(uint)) return GetUInt32Value(index);
            if (type == typeof(ulong)) return (ulong)GetInt64Value(index);
            if (type == typeof(short)) return (short)GetInt32Value(index);
            if (type == typeof(Guid)) return GetGuidValue(index);
            if (type == typeof(DateTime)) return GetDateTimeValue(index);
        }

        if (type == typeof(JsonObjectNode)) return GetObjectValue(index);
        if (type == typeof(JsonArrayNode)) return GetArrayValue(index);
        if (type == typeof(JsonDocument)) return (JsonDocument)GetObjectValue(index);
        if (type == typeof(System.Text.Json.Nodes.JsonObject)) return (System.Text.Json.Nodes.JsonObject)GetObjectValue(index);
        if (type == typeof(System.Text.Json.Nodes.JsonArray)) return (System.Text.Json.Nodes.JsonArray)GetArrayValue(index);
        if (type == typeof(Type)) return GetValue(index).GetType();
        if (type == typeof(IJsonDataNode) || type == typeof(IJsonValueNode) || type == typeof(JsonStringNode) || type == typeof(IJsonStringNode) || type == typeof(JsonIntegerNode) || type == typeof(JsonDoubleNode) || type == typeof(JsonBooleanNode) || type == typeof(IJsonNumberNode))
            return GetValue(index);

        if (type.IsClass)
        {
            var json = TryGetObjectValue(index);
            if (json != null) return JsonSerializer.Deserialize(json.ToString(), type);
        }

        throw new InvalidOperationException("The type is not supported to convert.", new InvalidCastException("Cannot cast."));
    }

    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index does not exist.</exception>
    /// <exception cref="InvalidOperationException">The type is not supported to convert.</exception>
    public T GetValue<T>(int index)
        => (T)GetValue(typeof(T), index);

    /// <summary>
    /// Gets the value of the specific index.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode IJsonDataNode.GetValue(string key)
    {
        if (TryGetValue(key, out var result)) return result;
        throw new InvalidOperationException("key should be an integer.", new FormatException("key should be an integer."));
    }

    /// <summary>
    /// Gets the value of the specific index.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode IJsonDataNode.GetValue(ReadOnlySpan<char> key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key), "key should not be null.");
        return (this as IJsonDataNode).GetValue(key.ToString());
    }

    /// <summary>
    /// Gets the value of the specific index.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode IJsonContainerNode.GetValue(string key)
    {
        if (TryGetValue(key, out var result)) return result;
        throw new InvalidOperationException("key should be an integer.", new FormatException("key should be an integer."));
    }

    /// <summary>
    /// Gets the value of the specific index.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode IJsonContainerNode.GetValue(ReadOnlySpan<char> key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key), "key should not be null.");
        return (this as IJsonDataNode).GetValue(key.ToString());
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
            if (item is null) yield return null;
            if (item is IJsonDataNode ele) yield return ele.GetString();
        }
    }

    /// <summary>
    /// Gets the value as a string collection.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The value. It will be null if the value is null.</returns>
    /// <exception cref="InvalidOperationException">The item value kind is not string.</exception>
    public IEnumerable<string> GetStringCollection(Func<IJsonDataNode, bool> predicate)
        => store.Select(ele =>
        {
            if (ele is null) return JsonValues.Null;
            return ele;
        }).Where(predicate).Select(ele => ele?.GetString());

    /// <summary>
    /// Gets the value as a string collection.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The value. It will be null if the value is null.</returns>
    /// <exception cref="InvalidOperationException">The item value kind is not string.</exception>
    public IEnumerable<string> GetStringCollection(Func<IJsonDataNode, int, bool> predicate)
        => store.Select(ele =>
        {
            if (ele is null) return JsonValues.Null;
            return ele;
        }).Where(predicate).Select(ele => ele?.GetString());

    /// <summary>
    /// Gets the value as a string collection.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The value. It will be null if the value is null.</returns>
    /// <exception cref="InvalidOperationException">The item value kind is not string.</exception>
    public IEnumerable<string> GetStringCollection(Func<string, bool> predicate)
        => store.Select(ele => ele?.GetString()).Where(predicate);

    /// <summary>
    /// Gets the value as a string collection.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The value. It will be null if the value is null.</returns>
    /// <exception cref="InvalidOperationException">The item value kind is not string.</exception>
    public IEnumerable<string> GetStringCollection(Func<string, int, bool> predicate)
        => store.Select(ele => ele?.GetString()).Where(predicate);

    /// <summary>
    /// Gets all string values.
    /// </summary>
    /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
    /// <returns>The JSON string value collection.</returns>
    public IEnumerable<JsonStringNode> GetStringValues(bool nullForOtherKinds = false)
        => GetSpecificKindValues<JsonStringNode>(nullForOtherKinds);

    /// <summary>
    /// Gets all integer values.
    /// </summary>
    /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
    /// <returns>The JSON integer value collection.</returns>
    public IEnumerable<JsonIntegerNode> GetIntegerValues(bool nullForOtherKinds = false)
        => GetSpecificKindValues<JsonIntegerNode>(nullForOtherKinds);

    /// <summary>
    /// Gets all double float number values.
    /// </summary>
    /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
    /// <returns>The JSON double float number value collection.</returns>
    public IEnumerable<JsonDoubleNode> GetDoubleValues(bool nullForOtherKinds = false)
        => GetSpecificKindValues<JsonDoubleNode>(nullForOtherKinds);

    /// <summary>
    /// Gets all boolean values.
    /// </summary>
    /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
    /// <returns>The JSON boolean value collection.</returns>
    public IEnumerable<JsonBooleanNode> GetBooleanValues(bool nullForOtherKinds = false)
        => GetSpecificKindValues<JsonBooleanNode>(nullForOtherKinds);

    /// <summary>
    /// Gets all JSON object values.
    /// </summary>
    /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
    /// <returns>The JSON object value collection.</returns>
    public IEnumerable<JsonObjectNode> GetObjectValues(bool nullForOtherKinds = false)
        => GetSpecificKindValues<JsonObjectNode>(nullForOtherKinds);

    /// <summary>
    /// Gets all JSON array values.
    /// </summary>
    /// <param name="nullForOtherKinds">true if set null in the result for other kinds; otherwise, false, to skip.</param>
    /// <returns>The JSON array value collection.</returns>
    public IEnumerable<JsonArrayNode> GetArrayValues(bool nullForOtherKinds = false)
        => GetSpecificKindValues<JsonArrayNode>(nullForOtherKinds);

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

            if (data is JsonStringNode str)
            {
                return str.Value;
            }

            return data.ValueKind switch
            {
                JsonValueKind.True => JsonBooleanNode.TrueString,
                JsonValueKind.False => JsonBooleanNode.FalseString,
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
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringValue(int index, out string result)
        => TryGetStringValue(index, null, out result);

    /// <summary>
    /// Gets the value as a string collection.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>A string.</returns>
    public string TryGetStringValue(int index, Func<JsonObjectNode, string> converter)
    {
        if (index < 0 || index >= store.Count)
        {
            return null;
        }

        try
        {
            var data = store[index];
            if (data is null) return null;
            if (data is JsonStringNode str) return str.Value;
            if (data is JsonObjectNode json) return converter?.Invoke(json);
            return data.ValueKind switch
            {
                JsonValueKind.True => JsonBooleanNode.TrueString,
                JsonValueKind.False => JsonBooleanNode.FalseString,
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
    /// Gets the value as a string collection.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>A string.</returns>
    public string TryGetStringValue(int index, IJsonPropertyResolver<string> converter)
        => TryGetStringValue(index, converter, out var result) ? result : null;

    /// <summary>
    /// Gets the value as a string collection.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringValue(int index, IJsonPropertyResolver<string> converter, out string result)
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

            if (data is IJsonStringNode str)
            {
                result = str.StringValue;
                return true;
            }

            if (data is JsonObjectNode json && converter is not null)
            {
                return converter.TryGetValue(json, out result);
            }

            result = data.ValueKind switch
            {
                JsonValueKind.True => JsonBooleanNode.TrueString,
                JsonValueKind.False => JsonBooleanNode.FalseString,
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
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="returnNullIfEmpty">true if returns null when the value is empty or white space; otherwise, false.</param>
    /// <returns>A string trimmed.</returns>
    public string TryGetStringTrimmedValue(int index, bool returnNullIfEmpty = false)
    {
        var s = TryGetStringValue(index)?.Trim();
        if (returnNullIfEmpty && string.IsNullOrEmpty(s)) return null;
        return s;
    }

    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result trimmed.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringTrimmedValue(int index, out string result)
    {
        if (!TryGetStringValue(index, out var r))
        {
            result = default;
            return false;
        }

        result = r?.Trim();
        return true;
    }

    /// <summary>
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public Uri TryGetUriValue(int index)
    {
        if (!TryGetStringValue(index, out var str) || string.IsNullOrWhiteSpace(str)) return null;
        return StringExtensions.TryCreateUri(str);
    }

    /// <summary>
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="kind">Specifies whether the URI string is a relative URI, absolute URI, or is indeterminate.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public Uri TryGetUriValue(int index, UriKind kind)
    {
        if (!TryGetStringValue(index, out var str) || string.IsNullOrWhiteSpace(str)) return null;
        return StringExtensions.TryCreateUri(str, kind);
    }

    /// <summary>
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetUriValue(int index, out Uri result)
    {
        if (!TryGetStringValue(index, out var str) || string.IsNullOrWhiteSpace(str))
        {
            result = null;
            return false;
        }

        result = StringExtensions.TryCreateUri(str);
        return result != null;
    }

    /// <summary>
    /// Tries to get the value of the specific index.
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
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
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
    public ushort? TryGetUInt16Value(int index)
    {
        try
        {
            if (TryGetJsonValue<JsonIntegerNode>(index, out var p1)) return (ushort)p1;
            if (TryGetJsonValue<JsonDoubleNode>(index, out var p2)) return (ushort)Math.Round(p2.Value);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }

        var str = TryGetStringValue(index);
        if (string.IsNullOrWhiteSpace(str) || !Numbers.TryParseToUInt16(str, 10, out var p3)) return null;
        return p3;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetUInt16Value(int index, out ushort result)
    {
        var v = TryGetUInt16Value(index);
        result = v ?? default;
        return v.HasValue;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public uint? TryGetUInt32Value(int index)
    {
        try
        {
            if (TryGetJsonValue<JsonIntegerNode>(index, out var p1)) return (uint)p1;
            if (TryGetJsonValue<JsonDoubleNode>(index, out var p2)) return (uint)Math.Round(p2.Value);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }

        var str = TryGetStringValue(index);
        if (string.IsNullOrWhiteSpace(str) || !Numbers.TryParseToUInt32(str, 10, out var p3)) return null;
        return p3;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
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
    public short? TryGetInt16Value(int index)
    {
        try
        {
            if (TryGetJsonValue<JsonIntegerNode>(index, out var p1)) return (short)p1;
            if (TryGetJsonValue<JsonDoubleNode>(index, out var p2)) return (short)Math.Round(p2.Value);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }

        var str = TryGetStringValue(index);
        if (string.IsNullOrWhiteSpace(str) || !Numbers.TryParseToInt16(str, 10, out var p3)) return null;
        return p3;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetInt16Value(int index, out short result)
    {
        var v = TryGetInt16Value(index);
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
        try
        {
            if (TryGetJsonValue<JsonIntegerNode>(index, out var p1)) return (int)p1;
            if (TryGetJsonValue<JsonDoubleNode>(index, out var p2)) return (int)Math.Round(p2.Value);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }

        var str = TryGetStringValue(index);
        if (string.IsNullOrWhiteSpace(str) || !Numbers.TryParseToInt32(str, 10, out var p3)) return null;
        return p3;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
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
        if (TryGetJsonValue<JsonIntegerNode>(index, out var p1)) return p1.Value;
        try
        {
            if (TryGetJsonValue<JsonDoubleNode>(index, out var p2)) return (long)Math.Round(p2.Value);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }

        var str = TryGetStringValue(index);
        if (string.IsNullOrWhiteSpace(str) || !Numbers.TryParseToInt64(str, 10, out var p3)) return null;
        return p3;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
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
    /// <returns>The value; or NaN if fail to resolve.</returns>
    public float TryGetSingleValue(int index)
    {
        try
        {
            if (TryGetJsonValue<JsonDoubleNode>(index, out var p1)) return (float)p1;
        }
        catch (InvalidCastException)
        {
            return float.NaN;
        }
        catch (ArgumentException)
        {
            return float.NaN;
        }

        if (TryGetJsonValue<JsonIntegerNode>(index, out var p2)) return (float)p2;
        var str = TryGetStringValue(index);
        if (string.IsNullOrWhiteSpace(str) || !float.TryParse(str, out var p3)) return float.NaN;
        return p3;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public float? TryGetNullableSingleValue(int index)
    {
        var v = TryGetSingleValue(index);
        return float.IsNaN(v) ? null : v;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetSingleValue(int index, out float result)
    {
        result = TryGetSingleValue(index);
        return !float.IsNaN(result);
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value; or the default value if fail to resolve.</returns>
    public float TryGetSingleValue(int index, float defaultValue)
    {
        var v = TryGetSingleValue(index);
        return float.IsNaN(v) ? defaultValue : v;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value; or NaN if fail to resolve.</returns>
    public double TryGetDoubleValue(int index)
    {
        if (TryGetJsonValue<JsonDoubleNode>(index, out var p1)) return p1.Value;
        if (TryGetJsonValue<JsonIntegerNode>(index, out var p2)) return (double)p2;
        var str = TryGetStringValue(index);
        if (string.IsNullOrWhiteSpace(str) || !double.TryParse(str, out var p3)) return double.NaN;
        return p3;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public double? TryGetNullableDoubleValue(int index)
    {
        var v = TryGetDoubleValue(index);
        return double.IsNaN(v) ? null : v;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value; or the default value if fail to resolve.</returns>
    public double TryGetDoubleValue(int index, double defaultValue)
    {
        var v = TryGetDoubleValue(index);
        return double.IsNaN(v) ? defaultValue : v;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetDoubleValue(int index, out double result)
    {
        result = TryGetDoubleValue(index);
        return !double.IsNaN(result);
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public decimal? TryGetDecimalValue(int index)
    {
        try
        {
            if (TryGetJsonValue<JsonDoubleNode>(index, out var p1)) return (decimal)p1.Value;
            if (TryGetJsonValue<JsonIntegerNode>(index, out var p2)) return (decimal)p2.Value;
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (OverflowException)
        {
            return null;
        }

        var str = TryGetStringValue(index);
        if (string.IsNullOrWhiteSpace(str) || !decimal.TryParse(str, out var p3)) return null;
        return p3;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetDecimalValue(int index, out decimal result)
    {
        var v = TryGetDecimalValue(index);
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
        if (TryGetJsonValue<JsonBooleanNode>(index, out var p)) return p.Value;
        var str = TryGetStringValue(index);
        if (string.IsNullOrWhiteSpace(str) || !bool.TryParse(str, out var p3)) return null;
        return p3;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
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
    public JsonObjectNode TryGetObjectValue(int index)
        => TryGetJsonValue<JsonObjectNode>(index, out var p) ? p : null;

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetObjectValue(int index, out JsonObjectNode result)
    {
        var v = TryGetObjectValue(index);
        result = v;
        return v is not null;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="id">The identifier of the object.</param>
    /// <returns>The value.</returns>
    public JsonObjectNode TryGetObjectValueById(string id)
        => TryGetObjectValueById(id, out var p) ? p : null;

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="id">The identifier of the object.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the object; otherwise, false.</returns>
    public bool TryGetObjectValueById(string id, out JsonObjectNode result)
    {
        id = id?.Trim();
        if (string.IsNullOrEmpty(id))
        {
            result = null;
            return false;
        }

        foreach (var item in store)
        {
            if (item is not JsonObjectNode json) continue;
            if (json.Id?.Trim() != id && json.TryGetStringTrimmedValue("id") != id) continue;
            result = json;
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="root">The root node.</param>
    /// <returns>The value.</returns>
    public JsonObjectNode TryGetRefObjectValue(int index, JsonObjectNode root)
        => JsonObjectNode.TryGetRefObjectValue(null, TryGetObjectValue(index), root);

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="root">The root node.</param>
    /// <returns>The value.</returns>
    public JsonObjectNode TryGetRefObjectValue(int index, JsonDataResult root)
        => root == null ? null : TryGetRefObjectValue(index, root.ToJson());

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    public JsonArrayNode TryGetArrayValue(int index)
    {
        if (TryGetJsonValue<JsonArrayNode>(index, out var p)) return p;
        if (TryGetJsonValue<JsonObjectNode>(index, out var obj) && obj.Count == 1 && obj.TryGetStringValue("ref") == JsonValues.SELF_REF) return this;
        return null;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetArrayValue(int index, out JsonArrayNode result)
    {
        var v = TryGetArrayValue(index);
        result = v;
        return v is not null;
    }

    /// <summary>
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="useUnixTimestampsFallback">true if use Unix timestamp to convert if the value is a number; otherwise, false, to use JavaScript date ticks fallback.</param>
    /// <returns>The value.</returns>
    public DateTime? TryGetDateTimeValue(int index, bool useUnixTimestampsFallback = false)
    {
        if (TryGetJsonValue<JsonStringNode>(index, out var s))
        {
            var date = WebFormat.ParseDate(s.Value);
            return date;
        }

        if (TryGetJsonValue<JsonIntegerNode>(index, out var num)) return useUnixTimestampsFallback ? WebFormat.ParseUnixTimestamp(num.Value) : WebFormat.ParseDate(num.Value);
        if (!TryGetJsonValue<JsonObjectNode>(index, out var obj)) return null;
        return JsonValues.TryGetDateTime(obj);
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="bytes">The result.</param>
    /// <param name="bytesWritten">The count of bytes written.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
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
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The enum.</returns>
    public T? TryGetEnumValue<T>(int index) where T : struct, Enum
    {
        if (TryGetEnumValue<T>(index, out var v)) return v;
        return null;
    }

    /// <summary>
    /// Tries to get the value of the specific index.
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
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
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
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
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
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="type">An enumeration type.</param>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result output.</param>
    /// <returns>true if parse succeeded; otherwise, false.</returns>
    public bool TryGetEnumValue(Type type, int index, out object result)
    {
        try
        {
            if (TryGetInt32Value(index, out var v))
            {
                result = type is null ? v : Enum.ToObject(type, v);
                return true;
            }

            var str = GetStringValue(index);
            if (type is null)
            {
                result = str;
                return true;
            }

#if NETFRAMEWORK
            result = Enum.Parse(type, str);
            return true;
#else
            if (Enum.TryParse(type, str, out result))
            {
                return true;
            }
#endif
        }
        catch (ArgumentException)
        {
        }
        catch (OverflowException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="type">An enumeration type.</param>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
    /// <param name="result">The result output.</param>
    /// <returns>true if parse succeeded; otherwise, false.</returns>
    public bool TryGetEnumValue(Type type, int index, bool ignoreCase, out object result)
    {
        try
        {
            if (TryGetInt32Value(index, out var v))
            {
                result = type is null ? v : Enum.ToObject(type, v);
                return true;
            }

            var str = GetStringValue(index);
            if (type is null)
            {
                result = str;
                return true;
            }

#if NETFRAMEWORK
            result = Enum.Parse(type, str, ignoreCase);
            return true;
#else
            if (Enum.TryParse(type, str, ignoreCase, out result))
            {
                return true;
            }
#endif
        }
        catch (ArgumentException)
        {
        }
        catch (OverflowException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    public IJsonDataNode TryGetValue(int index)
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
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetValue(int index, out IJsonDataNode result)
    {
        var v = TryGetValue(index);
        result = v;
        return v is not null;
    }

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="subKey">The optional sub-property key of the value of the array.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    public IJsonDataNode TryGetValue(int index, string subKey, params string[] keyPath)
    {
        if (index < 0 || index >= store.Count)
        {
            return default;
        }

        try
        {
            var result = store[index];
            if (result is JsonObjectNode json)
            {
                var keyArr = new List<string> { subKey };
                if (keyPath != null) keyArr.AddRange(keyPath);
                return json.TryGetValue(keyArr);
            }

            var keyPath2 = keyPath?.Where(ele => ele != null)?.ToList() ?? new List<string>();
            if (result is JsonArrayNode arr)
            {
                if (subKey != null)
                {
                    if (!int.TryParse(subKey, out index)) return default;
                    result = arr[index];
                    if (keyPath2.Count == 0) return result ?? JsonValues.Null;
                    if (result is JsonObjectNode json2) return json2.TryGetValue(keyPath2);
                    if (result is JsonArrayNode arr2) arr = arr2;
                    else return default;
                }

                if (int.TryParse(keyPath2[0], out index))
                {
                    if (keyPath2.Count == 1) return arr[index];
                    return arr.TryGetValue(index, keyPath[1], keyPath.Skip(2).ToArray());
                }
            }

            if (subKey == null && keyPath2.Count == 0) return result ?? JsonValues.Null;
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
    /// <param name="subIndex">The optional sub-index of the value of the array.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    public IJsonDataNode TryGetValue(int index, int subIndex, params string[] keyPath)
    {
        if (index < 0 || index >= store.Count)
        {
            return default;
        }

        try
        {
            var result = store[index];
            if (result is JsonObjectNode json)
            {
                var keyArr = new List<string> { subIndex.ToString("g") };
                if (keyPath != null) keyArr.AddRange(keyPath);
                return json.TryGetValue(keyArr);
            }

            var keyPath2 = keyPath?.Where(ele => ele != null)?.ToList() ?? new List<string>();
            if (result is JsonArrayNode arr)
            {
                result = arr[subIndex];
                if (keyPath2.Count == 0) return result ?? JsonValues.Null;
                if (result is JsonObjectNode json2) return json2.TryGetValue(keyPath2);
                if (result is JsonArrayNode arr2) arr = arr2;
                else return default;

                if (int.TryParse(keyPath2[0], out index))
                {
                    if (keyPath2.Count == 1) return arr[index];
                    return arr.TryGetValue(index, keyPath[1], keyPath.Skip(2).ToArray());
                }
            }

            if (keyPath2.Count == 0)
            {
                if (result is IJsonStringNode && result.TryGetValue(subIndex, out var subStr)) return subStr;
                if (subIndex == 0) return result ?? JsonValues.Null;
            }
        }
        catch (ArgumentException)
        {
        }

        return default;
    }

    /// <summary>
    /// Removes the element at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void Remove(int index)
        => store.RemoveAt(index);

#if !NETFRAMEWORK
    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    public IJsonDataNode TryGetValue(Index index)
        => TryGetValue(index.IsFromEnd ? Count - index.Value : index.Value);

    /// <summary>
    /// Tries to get the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the index and the type is the one expected; otherwise, false.</returns>
    public bool TryGetValue(Index index, out IJsonDataNode result)
        => TryGetValue(index.IsFromEnd ? Count - index.Value : index.Value, out result);

    /// <summary>
    /// Removes the element at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void Remove(Index index)
        => store.RemoveAt(index.IsFromEnd ? store.Count - index.Value : index.Value);
#endif

    /// <summary>
    /// Removes the first occurrence of a specific value from the array.
    /// </summary>
    /// <param name="item">The item matched.</param>
    /// <returns>true if item was successfully removed from the array; otherwise, false. This method also returns false if item is not found in the array.</returns>
    public bool Remove(IJsonValueNode item)
    {
        if (item is not IJsonDataNode ele) return false;
        return store.Remove(ele);
    }

    /// <summary>
    /// Removes all null value.
    /// </summary>
    /// <returns>The count of item removed.</returns>
    public int RemoveNull()
    {
        var count = 0;
        while (store.Remove(null)) count++;
        var list = new List<IJsonDataNode>();
        foreach (var ele in store)
        {
            if (ele is null || ele.ValueKind == JsonValueKind.Null || ele.ValueKind == JsonValueKind.Undefined) list.Add(ele);
        }

        foreach (var ele in list)
        {
            while (store.Remove(ele)) count++;
        }

        return count;
    }

    /// <summary>
    /// Removes all the specific value.
    /// </summary>
    /// <param name="value">The value to delete.</param>
    /// <returns>The count of item removed.</returns>
    public int RemoveValue(string value)
    {
        if (value == null) return RemoveNull();
        var count = 0;
        var list = new List<IJsonDataNode>();
        foreach (var ele in store)
        {
            if (ele is JsonStringNode s && value == s.Value) list.Add(ele);
        }

        foreach (var ele in list)
        {
            while (store.Remove(ele)) count++;
        }

        return count;
    }

    /// <summary>
    /// Removes all the specific value.
    /// </summary>
    /// <param name="value">The value to delete.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>The count of item removed.</returns>
    public int RemoveValue(string value, StringComparison comparisonType)
    {
        if (value == null) return RemoveNull();
        var count = 0;
        var list = new List<IJsonDataNode>();
        foreach (var ele in store)
        {
            if (ele is JsonStringNode s && value.Equals(s.Value, comparisonType)) list.Add(ele);
        }

        foreach (var ele in list)
        {
            while (store.Remove(ele)) count++;
        }

        return count;
    }

    /// <summary>
    /// Removes all the specific value.
    /// </summary>
    /// <param name="value">The value to delete.</param>
    /// <returns>The count of item removed.</returns>
    public int RemoveValue(int value)
    {
        var count = 0;
        var list = new List<IJsonDataNode>();
        foreach (var ele in store)
        {
            if (ele is JsonIntegerNode s)
            {
                if (value == s.Value) list.Add(ele);
            }
            else if (ele is JsonDoubleNode d)
            {
                if (value == d.Value) list.Add(ele);
            }
        }

        foreach (var ele in list)
        {
            while (store.Remove(ele)) count++;
        }

        return count;
    }

    /// <summary>
    /// Removes all the specific value.
    /// </summary>
    /// <param name="value">The value to delete.</param>
    /// <returns>The count of item removed.</returns>
    public int RemoveValue(long value)
    {
        var count = 0;
        var list = new List<IJsonDataNode>();
        foreach (var ele in store)
        {
            if (ele is JsonIntegerNode s)
            {
                if (value == s.Value) list.Add(ele);
            }
            else if (ele is JsonDoubleNode d)
            {
                if (value == d.Value) list.Add(ele);
            }
        }

        foreach (var ele in list)
        {
            while (store.Remove(ele)) count++;
        }

        return count;
    }

    /// <summary>
    /// Removes all the specific value.
    /// </summary>
    /// <param name="value">The value to delete.</param>
    /// <returns>The count of item removed.</returns>
    public int RemoveValue(double value)
    {
        var count = 0;
        var list = new List<IJsonDataNode>();
        foreach (var ele in store)
        {
            if (ele is JsonDoubleNode d)
            {
                if (value == d.Value) list.Add(ele);
            }
            else if (ele is JsonIntegerNode s)
            {
                if (value == s.Value) list.Add(ele);
            }
        }

        foreach (var ele in list)
        {
            while (store.Remove(ele)) count++;
        }

        return count;
    }

    /// <summary>
    /// Removes all the specific value.
    /// </summary>
    /// <param name="value">The value to delete.</param>
    /// <returns>The count of item removed.</returns>
    public int RemoveValue(bool value)
    {
        var count = 0;
        var list = new List<IJsonDataNode>();
        foreach (var ele in store)
        {
            if (ele is JsonBooleanNode b && value == b.Value) list.Add(ele);
        }

        foreach (var ele in list)
        {
            while (store.Remove(ele)) count++;
        }

        return count;
    }

    /// <summary>
    /// Removes all the specific value.
    /// </summary>
    /// <param name="value">The value to delete.</param>
    /// <returns>The count of item removed.</returns>
    public int RemoveValue(JsonObjectNode value)
    {
        if (value == null) return RemoveNull();
        var count = 0;
        var list = new List<IJsonDataNode>();
        foreach (var ele in store)
        {
            if (value == ele) list.Add(ele);
        }

        foreach (var ele in list)
        {
            while (store.Remove(ele)) count++;
        }

        return count;
    }

    /// <summary>
    /// Removes all the specific value.
    /// </summary>
    /// <param name="value">The value to delete.</param>
    /// <returns>The count of item removed.</returns>
    public int RemoveValue(JsonArrayNode value)
    {
        if (value == null) return RemoveNull();
        var count = 0;
        var list = new List<IJsonDataNode>();
        foreach (var ele in store)
        {
            if (value == ele) list.Add(ele);
        }

        foreach (var ele in list)
        {
            while (store.Remove(ele)) count++;
        }

        return count;
    }

    /// <summary>
    /// Sets null at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetNullValue(int index)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = JsonValues.Null;
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="_">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, DBNull _)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
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
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonStringNode(value);
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, IJsonStringNode value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = value != null ? new JsonStringNode(value) : JsonValues.Null;
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="InvalidOperationException">The secure string is disposed.</exception>
    public void SetValue(int index, SecureString value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonStringNode(value);
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
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonStringNode(string.Format(value, args));
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
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonStringNode(Convert.ToBase64String(inArray, options));
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
        if (store.Count == index) store.Add(JsonValues.Null);
#if NETFRAMEWORK
        if (bytes == null) throw new ArgumentNullException(nameof(bytes), "bytes should not be null.");
        store[index] = new JsonStringNode(Convert.ToBase64String(bytes.ToArray(), options));
#else
        store[index] = new JsonStringNode(Convert.ToBase64String(bytes, options));
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
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonStringNode(value);
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, DateTime value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonStringNode(value);
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, uint value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonIntegerNode(value);
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, int value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonIntegerNode(value);
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, long value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonIntegerNode(value);
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, float value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = float.IsNaN(value) ? JsonValues.Null : new JsonDoubleNode(value);
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, double value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = double.IsNaN(value) ? JsonValues.Null : new JsonDoubleNode(value);
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, decimal value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonDoubleNode(value);
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, bool value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = value ? JsonBooleanNode.True : JsonBooleanNode.False;
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, bool? value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        if (value.HasValue) store[index] = value.Value ? JsonBooleanNode.True : JsonBooleanNode.False;
        else SetNullValue(index);
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, JsonBooleanNode value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = value ?? JsonValues.Null;
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, JsonArrayNode value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        if (value is null) store[index] = JsonValues.Null;
        else store[index] = ReferenceEquals(value, this) ? value.Clone() : value;
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, JsonObjectNode value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = value ?? JsonValues.Null;
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, IJsonObjectHost value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = value?.ToJson() ?? JsonValues.Null;
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, JsonElement value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = JsonValues.ToJsonValue(value) ?? JsonValues.Null;
    }

    /// <summary>
    /// Sets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetValue(int index, System.Text.Json.Nodes.JsonNode value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = JsonValues.ToJsonValue(value) ?? JsonValues.Null;
    }

    /// <summary>
    /// Sets the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetDateTimeStringValue(int index, DateTime value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonStringNode(value);
    }

    /// <summary>
    /// Sets the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetJavaScriptDateTicksValue(int index, DateTime value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonIntegerNode(Web.WebFormat.ParseDate(value));
    }

    /// <summary>
    /// Sets the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetUnixTimestampValue(int index, DateTime value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonIntegerNode(Web.WebFormat.ParseUnixTimestamp(value));
    }

    /// <summary>
    /// Sets the value of the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void SetWindowsFileTimeUtcValue(int index, DateTime value)
    {
        if (store.Count == index) store.Add(JsonValues.Null);
        store[index] = new JsonIntegerNode(value.ToFileTimeUtc());
    }

    /// <summary>
    /// Replaces the old value to a new one by reference equaling.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="onlyFirst">true if replace first only; otherwise, false.</param>
    /// <returns>The count of item replaced.</returns>
    public int ReplaceValue(JsonObjectNode oldValue, JsonObjectNode newValue, bool onlyFirst = false)
        => onlyFirst ? ListExtensions.Replace(store, oldValue, newValue, 1) : ListExtensions.Replace(store, oldValue, newValue);

    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by selector.</typeparam>
    /// <param name="removeNotMatched">true if remove the type does not match the expected; otherwise, false.</param>
    /// <returns>A collection whose elements are the result of invoking the transform function on each element of source.</returns>
    public IEnumerable<T> Select<T>(bool removeNotMatched) where T : IJsonDataNode
    {
        foreach (var item in store)
        {
            if (item is T t) yield return t;
            if (!removeNotMatched) yield return default;
        }
    }

    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>A collection whose elements are the result of invoking the transform function on each element of source.</returns>
    public IEnumerable<T> Select<T>(Func<IJsonDataNode, T> selector)
        => store.Select(ele => selector(ele ?? JsonValues.Null));

    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by selector.</typeparam>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>A collection whose elements are the result of invoking the transform function on each element of source.</returns>
    public IEnumerable<T> Select<T>(Func<IJsonDataNode, int, T> selector)
        => store.Select((ele, i) => selector(ele ?? JsonValues.Null, i));

    /// <summary>
    /// Projects each element into a new form by incorporation the index.
    /// </summary>
    /// <typeparam name="T">The type of element output.</typeparam>
    /// <param name="selector">The transform function.</param>
    /// <returns>A collection whose elements are the result of invoking the transform function on each property.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public IEnumerable<T> Select<T>(Func<JsonValueKind, object, int, T> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector), "selector was null.");
        var index = -1;
        foreach (var value in store)
        {
            index++;
            yield return selector(value.ValueKind, JsonValues.GetValue(value), index);
        }
    }

    /// <summary>
    /// Gets a collection of all property values for each element.
    /// </summary>
    /// <param name="key">The specific property key.</param>
    /// <returns>A collection whose elements are the result of invoking the transform function on each element of source.</returns>
    public IEnumerable<IJsonDataNode> SelectProperty(string key)
        => Select(ele =>
        {
            if (ele is null) return null;
            if (ele is JsonObjectNode json) return json.TryGetValue(key);
            if (ele is JsonArrayNode jArr) return jArr.TryGetValue(key);
            if (ele is JsonStringNode jStr && (jStr as IJsonDataNode).TryGetValue(key, out var subStr)) return subStr;
            return null;
        });

    /// <summary>
    /// Fills items by null until the count matches the specific minimum requirement.
    /// </summary>
    /// <param name="count">The minimum count required.</param>
    /// <returns>The count to add.</returns>
    public int EnsureCount(int count)
    {
        var rest = count - store.Count;
        if (rest <= 0) return 0;
        for (var i = 0; i < rest; i++)
        {
            store.Add(JsonValues.Null);
        }

        return rest;
    }

    /// <summary>
    /// Add null.
    /// </summary>
    public void AddNull()
        => store.Add(JsonValues.Null);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="_">The value to set.</param>
    public void Add(DBNull _)
        => store.Add(JsonValues.Null);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(string value)
        => store.Add(new JsonStringNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(IJsonStringNode value)
        => store.Add(value != null ? new JsonStringNode(value) : JsonValues.Null);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <exception cref="InvalidOperationException">The secure string is disposed.</exception>
    public void Add(SecureString value)
        => store.Add(new JsonStringNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public void AddFormat(string value, params object[] args)
        => store.Add(new JsonStringNode(string.Format(value, args)));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(Guid value)
        => store.Add(new JsonStringNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(DateTime value)
        => store.Add(new JsonStringNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(short value)
        => store.Add(new JsonIntegerNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(uint value)
        => store.Add(new JsonIntegerNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(int value)
        => store.Add(new JsonIntegerNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(long value)
        => store.Add(new JsonIntegerNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(float value)
        => store.Add(float.IsNaN(value) ? JsonValues.Null : new JsonDoubleNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(double value)
        => store.Add(double.IsNaN(value) ? JsonValues.Null : new JsonDoubleNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(decimal value)
        => store.Add(new JsonDoubleNode(value));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(bool value)
        => store.Add(value ? JsonBooleanNode.True : JsonBooleanNode.False);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(JsonBooleanNode value)
        => store.Add(value ?? JsonValues.Null);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(JsonArrayNode value)
    {
        if (value is null) store.Add(JsonValues.Null);
        else store.Add(ReferenceEquals(value, this) ? value.Clone() : value);
    }

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(JsonObjectNode value)
        => store.Add(value ?? JsonValues.Null);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(IJsonObjectHost value)
        => store.Add(value?.ToJson() ?? JsonValues.Null);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(JsonDocument value)
        => store.Add(JsonValues.ToJsonValue(value) ?? JsonValues.Null);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(JsonElement value)
        => store.Add(JsonValues.ToJsonValue(value) ?? JsonValues.Null);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Add(System.Text.Json.Nodes.JsonNode value)
        => store.Add(JsonValues.ToJsonValue(value) ?? JsonValues.Null);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="inArray">An array of 8-bit unsigned integers.</param>
    /// <param name="options">A formatting options.</param>
    /// <exception cref="ArgumentNullException">The bytes should not be null.</exception>
    public void AddBase64(byte[] inArray, Base64FormattingOptions options = Base64FormattingOptions.None)
        => store.Add(new JsonStringNode(Convert.ToBase64String(inArray, options)));

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="bytes">The bytes to convert to base 64 string.</param>
    /// <param name="options">A formatting options.</param>
    /// <exception cref="ArgumentNullException">The bytes should not be null.</exception>
    public void AddBase64(Span<byte> bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
    {
#if NETFRAMEWORK
        if (bytes == null) throw new ArgumentNullException(nameof(bytes), "bytes should not be null.");
        store.Add(new JsonStringNode(Convert.ToBase64String(bytes.ToArray(), options)));
#else
        store.Add(new JsonStringNode(Convert.ToBase64String(bytes, options)));
#endif
    }

    /// <summary>
    /// Adds a date time string.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void AddDateTimeString(DateTime value)
        => store.Add(new JsonStringNode(value));

    /// <summary>
    /// Adds a JavaScript date ticks.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void AddJavaScriptDateTicks(DateTime value)
        => store.Add(new JsonIntegerNode(Web.WebFormat.ParseDate(value)));

    /// <summary>
    /// Adds a Unix timestamp.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void AddUnixTimestamp(DateTime value)
        => store.Add(new JsonIntegerNode(Web.WebFormat.ParseUnixTimestamp(value)));

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
            store.Add(new JsonStringNode(item));
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
            store.Add(new JsonIntegerNode(item));
            count++;
        }

        return count;
    }

    /// <summary>
    /// Adds a collection of integer.
    /// </summary>
    /// <param name="values">An integer collection to add.</param>
    /// <returns>The count of item added.</returns>
    public int AddRange(IEnumerable<long> values)
    {
        var count = 0;
        if (values == null) return count;
        foreach (var item in values)
        {
            store.Add(new JsonIntegerNode(item));
            count++;
        }

        return count;
    }

    /// <summary>
    /// Adds a collection of floating number.
    /// </summary>
    /// <param name="values">An floating number collection to add.</param>
    /// <returns>The count of item added.</returns>
    public int AddRange(IEnumerable<float> values)
    {
        var count = 0;
        if (values == null) return count;
        foreach (var item in values)
        {
            store.Add(new JsonDoubleNode(item));
            count++;
        }

        return count;
    }

    /// <summary>
    /// Adds a collection of floating number.
    /// </summary>
    /// <param name="values">An floating number collection to add.</param>
    /// <returns>The count of item added.</returns>
    public int AddRange(IEnumerable<double> values)
    {
        var count = 0;
        if (values == null) return count;
        foreach (var item in values)
        {
            store.Add(new JsonDoubleNode(item));
            count++;
        }

        return count;
    }

    /// <summary>
    /// Adds a collection of boolean value.
    /// </summary>
    /// <param name="values">An boolean value collection to add.</param>
    /// <returns>The count of item added.</returns>
    public int AddRange(IEnumerable<bool> values)
    {
        var count = 0;
        if (values == null) return count;
        foreach (var item in values)
        {
            store.Add(new JsonBooleanNode(item));
            count++;
        }

        return count;
    }

    /// <summary>
    /// Adds a collection of JSON object.
    /// </summary>
    /// <param name="values">A JSON object collection to add.</param>
    /// <returns>The count of item added.</returns>
    public int AddRange(IEnumerable<JsonObjectNode> values)
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
    public int AddRange(JsonArrayNode json)
    {
        var count = 0;
        if (json is null) return count;
        if (ReferenceEquals(json, this)) json = json.Clone();
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
    public int AddRange(JsonObjectNode json, IEnumerable<string> propertyKeys)
    {
        var count = 0;
        if (json is null || propertyKeys == null) return count;
        foreach (var key in propertyKeys)
        {
            store.Add(json.TryGetValue(key) ?? JsonValues.Undefined);
            count++;
        }

        return count;
    }

    /// <summary>
    /// Adds a set of value from a JSON object.
    /// </summary>
    /// <param name="array">A JSON array to copy its properties to add.</param>
    /// <returns>The count of item added.</returns>
    /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
    public int AddRange(System.Text.Json.Nodes.JsonArray array)
    {
        var count = 0;
        if (array is null) return count;
        foreach (var item in array)
        {
            Add(item);
            count++;
        }

        return count;
    }

    /// <summary>
    /// Determines the index of a specific item in the JSON array.
    /// </summary>
    /// <param name="value">The object to locate in the JSON array.</param>
    /// <returns>The index of item if found in the JSON array; otherwise, -1.</returns>
    public int IndexOf(JsonObjectNode value)
        => store.IndexOf(value);

    /// <summary>
    /// Determines the index of a specific item in the JSON array.
    /// </summary>
    /// <param name="value">The object to locate in the JSON array.</param>
    /// <returns>The index of item if found in the JSON array; otherwise, -1.</returns>
    public int IndexOf(JsonArrayNode value)
        => store.IndexOf(value);

    /// <summary>
    /// Inserts null at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void InsertNull(int index)
        => store.Insert(index, JsonValues.Null);

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="_">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, DBNull _)
        => store.Insert(index, JsonValues.Null);

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, string value)
        => store.Insert(index, new JsonStringNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, IJsonStringNode value)
        => store.Insert(index, value != null ? new JsonStringNode(value) : JsonValues.Null);

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    /// <exception cref="InvalidOperationException">The secure string is disposed.</exception>
    public void Insert(int index, SecureString value)
        => store.Insert(index, new JsonStringNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void InsertFormat(int index, string value, params object[] args)
        => store.Insert(index, new JsonStringNode(string.Format(value, args)));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    public void Insert(int index, Guid value)
        => store.Insert(index, new JsonStringNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, DateTime value)
        => store.Insert(index, new JsonStringNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, uint value)
        => store.Insert(index, new JsonIntegerNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, int value)
        => store.Insert(index, new JsonIntegerNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, long value)
        => store.Insert(index, new JsonIntegerNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, float value)
        => store.Insert(index, float.IsNaN(value) ? JsonValues.Null : new JsonDoubleNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, double value)
        => store.Insert(index, double.IsNaN(value) ? JsonValues.Null : new JsonDoubleNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, decimal value)
        => store.Insert(index, new JsonDoubleNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, bool value)
        => store.Insert(index, value ? JsonBooleanNode.True : JsonBooleanNode.False);

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, JsonBooleanNode value)
        => store.Insert(index, value ?? JsonValues.Null);

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, JsonArrayNode value)
    {
        if (value is null) store.Insert(index, JsonValues.Null);
        else store.Insert(index, ReferenceEquals(value, this) ? value.Clone() : value);
    }

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, JsonObjectNode value)
        => store.Insert(index, value ?? JsonValues.Null);

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, IJsonObjectHost value)
        => store.Insert(index, value?.ToJson() ?? JsonValues.Null);

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, JsonElement value)
        => store.Insert(index, JsonValues.ToJsonValue(value) ?? JsonValues.Null);

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void Insert(int index, System.Text.Json.Nodes.JsonNode value)
        => store.Insert(index, JsonValues.ToJsonValue(value) ?? JsonValues.Null);

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="inArray">An array of 8-bit unsigned integers.</param>
    /// <param name="options">A formatting options.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    /// <exception cref="ArgumentNullException">The bytes should not be null.</exception>
    public void InsertBase64(int index, byte[] inArray, Base64FormattingOptions options = Base64FormattingOptions.None)
        => store.Insert(index, new JsonStringNode(Convert.ToBase64String(inArray, options)));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="bytes">The bytes to convert to base 64 string.</param>
    /// <param name="options">A formatting options.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    /// <exception cref="ArgumentNullException">The bytes should not be null.</exception>
    public void InsertBase64(int index, Span<byte> bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
    {
#if NETFRAMEWORK
        if (bytes == null) throw new ArgumentNullException(nameof(bytes), "bytes should not be null.");
        store.Insert(index, new JsonStringNode(Convert.ToBase64String(bytes.ToArray(), options)));
#else
        store.Insert(index, new JsonStringNode(Convert.ToBase64String(bytes, options)));
#endif
    }

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void InsertDateTimeString(int index, DateTime value)
        => store.Insert(index, new JsonStringNode(value));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void InsertJavaScriptDateTicks(int index, DateTime value)
        => store.Insert(index, new JsonIntegerNode(Web.WebFormat.ParseDate(value)));

    /// <summary>
    /// Inserts the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public void InsertUnixTimestamp(int index, DateTime value)
        => store.Insert(index, new JsonIntegerNode(Web.WebFormat.ParseUnixTimestamp(value)));

    /// <summary>
    /// Adds a JSON array.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="values">A string collection to add.</param>
    /// <returns>The count of item added.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public int InsertRange(int index, IEnumerable<string> values)
    {
        var count = 0;
        if (values == null) return count;
        foreach (var item in values)
        {
            store.Insert(index + count, new JsonStringNode(item));
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
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public int InsertRange(int index, IEnumerable<int> values)
    {
        var count = 0;
        if (values == null) return count;
        foreach (var item in values)
        {
            store.Insert(index + count, new JsonIntegerNode(item));
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
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public int InsertRange(int index, IEnumerable<double> values)
    {
        var count = 0;
        if (values == null) return count;
        foreach (var item in values)
        {
            store.Insert(index + count, new JsonDoubleNode(item));
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
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public int InsertRange(int index, IEnumerable<JsonObjectNode> values)
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
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public int InsertRange(int index, JsonArrayNode json)
    {
        var count = 0;
        if (json is null) return count;
        if (ReferenceEquals(json, this)) json = json.Clone();
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
    /// <exception cref="ArgumentOutOfRangeException">The index was out of range.</exception>
    public int InsertRange(int index, JsonObjectNode json, IEnumerable<string> propertyKeys)
    {
        var count = 0;
        if (json is null || propertyKeys == null) return count;
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
                    if (prop is JsonStringNode strJson) writer.WriteStringValue(strJson.Value);
                    break;
                case JsonValueKind.Number:
                    if (prop is JsonIntegerNode intJson) writer.WriteNumberValue((long)intJson);
                    else if (prop is JsonDoubleNode floatJson) writer.WriteNumberValue((double)floatJson);
                    break;
                case JsonValueKind.True:
                    writer.WriteBooleanValue(true);
                    break;
                case JsonValueKind.False:
                    writer.WriteBooleanValue(false);
                    break;
                case JsonValueKind.Object:
                    if (prop is JsonObjectNode objJson) objJson.WriteTo(writer);
                    break;
                case JsonValueKind.Array:
                    if (prop is JsonArrayNode objArr) objArr.WriteTo(writer);
                    break;
            }
        }

        writer.WriteEndArray();
    }

    /// <summary>
    /// Writes to file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="style">The indent style.</param>
    /// <exception cref="IOException">IO exception.</exception>
    /// <exception cref="SecurityException">Write failed because of security exception.</exception>
    /// <exception cref="ArgumentException">path was invalid..</exception>
    /// <exception cref="ArgumentNullException">path was null.</exception>
    /// <exception cref="NotSupportedException">path was not supported.</exception>
    /// <exception cref="UnauthorizedAccessException">Write failed because of unauthorized access exception.</exception>
    public void WriteTo(string path, IndentStyles style = IndentStyles.Minified)
        => File.WriteAllText(path, ToString(style) ?? "null");

#if NET6_0_OR_GREATER
    /// <summary>
    /// Writes to file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="style">The indent style.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="IOException">IO exception.</exception>
    /// <exception cref="SecurityException">Write failed because of security exception.</exception>
    /// <exception cref="ArgumentException">path was invalid..</exception>
    /// <exception cref="ArgumentNullException">path was null.</exception>
    /// <exception cref="NotSupportedException">path was not supported.</exception>
    /// <exception cref="UnauthorizedAccessException">Write failed because of unauthorized access exception.</exception>
    public Task WriteToAsync(string path, IndentStyles style = IndentStyles.Minified, CancellationToken cancellationToken = default)
        => File.WriteAllTextAsync(path, ToString(style) ?? "null", cancellationToken);
#endif

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
    /// Deserializes an item.
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
    /// Deserializes an item.
    /// </summary>
    /// <typeparam name="T">The type of model to deserialize.</typeparam>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="parser">The string parser.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
    /// <exception cref="JsonException">The JSON is invalid. -or- TValue is not compatible with the JSON.</exception>
    public T DeserializeValue<T>(int index, Func<string, T> parser, JsonSerializerOptions options = default)
    {
        var item = store[index];
        if (item is null || item.ValueKind == JsonValueKind.Null || item.ValueKind == JsonValueKind.Undefined)
        {
            return default;
        }

        if (parser != null && item is IJsonStringNode s) return parser(s.StringValue);
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
    /// Gets a collection of the specific JSON value kind.
    /// </summary>
    /// <param name="kind">The data type of JSON value to filter.</param>
    /// <param name="predicate">An optional function to test each source element for a condition; the second parameter of the function represents the index of the source element; the third is the index of the element after filter.</param>
    /// <returns>A collection of values of the specific JSON value kind.</returns>
    public IEnumerable<IJsonDataNode> Where(JsonValueKind kind, Func<IJsonDataNode, int, int, bool> predicate = null)
    {
        predicate ??= PassTrue;
        var i = -1;
        var j = -1;
        if (kind == JsonValueKind.Null || kind == JsonValueKind.Undefined)
        {
            foreach (var item in store)
            {
                i++;
                if (item == null)
                {
                    j++;
                    if (predicate(JsonValues.Null, i, j)) yield return JsonValues.Null;
                    continue;
                }
                    
                if (item.ValueKind == kind)
                {
                    j++;
                    if (predicate(item, i, j)) yield return item;
                }
            }

            yield break;
        }

        foreach (var item in store)
        {
            i++;
            if (item != null && item.ValueKind == kind)
            {
                j++;
                if (predicate(item, i, j)) yield return item;
            }
        }
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each source element for a condition.</param>
    /// <returns>A collection that contains elements from the input sequence that satisfy the condition.</returns>
    public IEnumerable<IJsonDataNode> Where(Func<JsonValueKind, object, int, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate), "predicate was null.");
        var index = -1;
        foreach (var value in store)
        {
            index++;
            if (predicate(value.ValueKind, JsonValues.GetValue(value), index)) yield return value;
        }
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each source element for a condition.</param>
    /// <returns>A collection that contains elements from the input sequence that satisfy the condition.</returns>
    public IEnumerable<IJsonDataNode> Where(Func<IJsonDataNode, bool> predicate)
    {
        if (predicate == null) return store.Select(ele => ele ?? JsonValues.Null);
        return store.Select(ele => ele ?? JsonValues.Null).Where(predicate);
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
    /// <returns>A collection that contains elements from the input sequence that satisfy the condition.</returns>
    public IEnumerable<IJsonDataNode> Where(Func<IJsonDataNode, int, bool> predicate)
    {
        if (predicate == null) return store.Select(ele => ele ?? JsonValues.Null);
        return store.Select(ele => ele ?? JsonValues.Null).Where(predicate);
    }

    /// <summary>
    /// Tries to get the first element of a sequence, or null if the sequence contains no elements.
    /// </summary>
    /// <param name="predicate">An optional function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
    /// <returns>the first element in the array; or null, if the array is empty.</returns>
    public IJsonDataNode FirstOrDefault(Func<IJsonDataNode, bool> predicate = null)
        => Where(predicate).FirstOrDefault();

    /// <summary>
    /// Tries to get the last element of a sequence, or null if the sequence contains no elements.
    /// </summary>
    /// <param name="predicate">An optional function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
    /// <returns>the last element in the array; or null, if the array is empty.</returns>
    public IJsonDataNode LastOrDefault(Func<IJsonDataNode, bool> predicate = null)
        => Where(predicate).LastOrDefault();

    /// <summary>
    /// Bypasses a specified number of elements in the array and then returns the remaining elements.
    /// </summary>
    /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
    /// <returns>A new enumerable collection that contains the elements that occur after the specified index in the array.</returns>
    public IEnumerable<IJsonDataNode> Skip(int count)
        => store.Select(ele => ele ?? JsonValues.Null).Skip(count);

    /// <summary>
    /// Gets a specified number of contiguous elements from the start of the array.
    /// </summary>
    /// <param name="count">The number of elements to return.</param>
    /// <returns>A new enumerable collection that contains the specified number of elements from the start of the array.</returns>
    public IEnumerable<IJsonDataNode> Take(int count)
        => store.Select(ele => ele ?? JsonValues.Null).Take(count);

#if NET6_0_OR_GREATER
    /// <summary>
    /// Gets a new enumerable collection that contains the elements from source with the last count elements of the array.
    /// </summary>
    /// <param name="count">The number of elements to omit from the end of the array.</param>
    /// <returns>A new enumerable collection that contains the elements from source with the last count elements of the array.</returns>
    public IEnumerable<IJsonDataNode> SkipLast(int count)
        => store.Select(ele => ele ?? JsonValues.Null).SkipLast(count);

    /// <summary>
    /// Gets a new enumerable collection that contains the last count elements from the array.
    /// </summary>
    /// <param name="count">The number of elements to return.</param>
    /// <returns>A new enumerable collection that contains the last count elements from the array.</returns>
    public IEnumerable<IJsonDataNode> TakeLast(int count)
        => store.Select(ele => ele ?? JsonValues.Null).TakeLast(count);
#endif

    /// <summary>
    /// Filters the elements of the JSON array based on a specific type.
    /// </summary>
    /// <typeparam name="T">The type to filter the element.</typeparam>
    /// <returns>A collection that contains elements from the input sequence of the specific type.</returns>
    public IEnumerable<T> OfType<T>()
    {
        var type = typeof(T);
        List<IJsonDataNode> col;
        try
        {
            col = store.Where(ele => ele != null && ele.ValueKind != JsonValueKind.Null && ele.ValueKind != JsonValueKind.Undefined).ToList();
        }
        catch (InvalidOperationException)
        {
            col = store.Where(ele => ele != null && ele.ValueKind != JsonValueKind.Null && ele.ValueKind != JsonValueKind.Undefined).ToList();
        }

        if (type == typeof(string))
        {
            foreach (var item in col)
            {
                if (item.TryGetString(out var r)) yield return (T)(object)r;
            }
        }
        else if (type == typeof(int))
        {
            foreach (var item in col)
            {
                if (item.ValueKind != JsonValueKind.True && item.ValueKind != JsonValueKind.False && item.TryGetInt32(out var r)) yield return (T)(object)r;
            }
        }
        else if (type == typeof(long))
        {
            foreach (var item in col)
            {
                if (item.ValueKind != JsonValueKind.True && item.ValueKind != JsonValueKind.False && item.TryGetInt64(out var r)) yield return (T)(object)r;
            }
        }
        else if (type == typeof(float))
        {
            foreach (var item in col)
            {
                if (item.ValueKind != JsonValueKind.True && item.ValueKind != JsonValueKind.False && item.TryGetSingle(out var r)) yield return (T)(object)r;
            }
        }
        else if (type == typeof(double))
        {
            foreach (var item in col)
            {
                if (item.ValueKind != JsonValueKind.True && item.ValueKind != JsonValueKind.False && item.TryGetDouble(out var r)) yield return (T)(object)r;
            }
        }
        else if (type == typeof(decimal))
        {
            foreach (var item in col)
            {
                if (item.ValueKind != JsonValueKind.True && item.ValueKind != JsonValueKind.False && item.TryGetDecimal(out var r)) yield return (T)(object)r;
            }
        }
        else if (type == typeof(bool))
        {
            foreach (var item in col)
            {
                if (item.TryGetBoolean(out var r)) yield return (T)(object)r;
            }
        }
        else if (type == typeof(uint))
        {
            foreach (var item in col)
            {
                if (item.TryGetUInt32(out var r)) yield return (T)(object)r;
            }
        }
        else if (type == typeof(Guid))
        {
            foreach (var item in col)
            {
                if (item.TryGetGuid(out var r)) yield return (T)(object)r;
            }
        }
        else if (type == typeof(DateTime))
        {
            foreach (var item in col)
            {
                if (item.TryGetDateTime(out var r)) yield return (T)(object)r;
            }
        }
        else if (type == typeof(JsonValueKind))
        {
            foreach (var item in store)
            {
                yield return (T)(object)(item != null ? item.ValueKind : JsonValueKind.Undefined);
            }
        }
        else if (type == typeof(StringBuilder))
        {
            foreach (var item in col)
            {
                if (item.TryGetString(out var r)) yield return (T)(object)new StringBuilder(r);
            }
        }
        else if (type == typeof(Uri))
        {
            foreach (var item in col)
            {
                if (item.TryGetString(out var r))
                {
                    var uri = StringExtensions.TryCreateUri(r);
                    if (uri != null) yield return (T)(object)uri;
                }
            }
        }
        else
        {
            List<T> list;
            try
            {
                list = store.OfType<T>().ToList();
            }
            catch (InvalidOperationException)
            {
                list = store.OfType<T>().ToList();
            }

            foreach (var item in list)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Creates a list from this instance.
    /// </summary>
    /// <returns>A list that contains elements from this sequence.</returns>
    public List<IJsonDataNode> ToList()
    {
        try
        {
            return store.Select(ele => ele ?? JsonValues.Null).ToList();
        }
        catch (InvalidOperationException)
        {
            return store.Select(ele => ele ?? JsonValues.Null).ToList();
        }
    }

    /// <summary>
    /// Creates an array from this instance.
    /// </summary>
    /// <returns>An array that contains elements from this sequence.</returns>
    public IJsonDataNode[] ToArray()
    {
        try
        {
            return store.Select(ele => ele ?? JsonValues.Null).ToArray();
        }
        catch (InvalidOperationException)
        {
            return store.Select(ele => ele ?? JsonValues.Null).ToArray();
        }
    }

    /// <summary>
    /// Creates a dictionary from this instance.
    /// </summary>
    /// <returns>A dictionary that contains elements from this sequence.</returns>
    public Dictionary<int, IJsonDataNode> ToDictionary()
    {
        try
        {
            var d = new Dictionary<int, IJsonDataNode>();
            var i = 0;
            foreach (var item in store)
            {
                d[i] = item ?? JsonValues.Null;
                i++;
            }

            return d;
        }
        catch (InvalidOperationException)
        {
            var d = new Dictionary<int, IJsonDataNode>();
            var i = 0;
            foreach (var item in store)
            {
                d[i] = item ?? JsonValues.Null;
                i++;
            }

            return d;
        }
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(JsonArrayNode other)
    {
        if (other is null) return false;
        if (base.Equals(other)) return true;
        if (other.Count != Count) return false;
        for (var i = 0; i < Count; i++)
        {
            var isNull = !other.TryGetValue(i, out var r) || r is null || r.ValueKind == JsonValueKind.Null || r.ValueKind == JsonValueKind.Undefined;
            var prop = store[i];
            if (!isNull && (prop is null || prop.ValueKind == JsonValueKind.Null || prop.ValueKind == JsonValueKind.Undefined))
                return false;
            if (isNull || !JsonValues.Equals(prop, r)) return false;
        }

        return true;
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(IJsonValueNode other)
    {
        if (other is null) return false;
        if (other is JsonArrayNode json) return Equals(json);
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
        if (other is JsonArrayNode json) return Equals(json);
        return false;
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode() => store.GetHashCode();

    /// <summary>
    /// Gets the JSON array format string of the value.
    /// </summary>
    /// <returns>A JSON array format string.</returns>
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
    /// Gets the JSON array format string of the value.
    /// </summary>
    /// <param name="indentStyle">The indent style.</param>
    /// <returns>A JSON array format string.</returns>
    public string ToString(IndentStyles indentStyle)
        => ConvertToString(indentStyle, 0);

    /// <summary>
    /// Filters by getting JSON object list only.
    /// </summary>
    /// <returns>A list of value matched.</returns>
    internal IList<JsonObjectNode> SelectObjects()
    {
        try
        {
            return store.OfType<JsonObjectNode>().ToList();
        }
        catch (InvalidOperationException)
        {
            return store.OfType<JsonObjectNode>().ToList();
        }
    }

    /// <summary>
    /// Gets the JSON array format string of the value.
    /// </summary>
    /// <param name="indentStyle">The indent style.</param>
    /// <param name="indentLevel">The current indent level.</param>
    /// <returns>A JSON array format string.</returns>
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
        var str = new StringBuilder();
        indentLevel++;
        #pragma warning disable CA1834
        str.Append("[");
        #pragma warning restore CA1834
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
                    str.Append((prop is JsonArrayNode jArr) ? jArr.ConvertToString(indentStyle, indentLevel) : "[]");
                    break;
                case JsonValueKind.Object:
                    str.Append((prop is JsonObjectNode jObj) ? jObj.ConvertToString(indentStyle, indentLevel) : "{}");
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
    /// Gets the JSON lines format string of the value.
    /// </summary>
    /// <returns>A JSON lines format string.</returns>
    public string ToJsonlString()
    {
        var str = new StringBuilder();
        foreach (var prop in store)
        {
            if (prop is null)
            {
                str.AppendLine("null");
                continue;
            }

            switch (prop.ValueKind)
            {
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    str.AppendLine("null");
                    break;
                default:
                    str.AppendLine(prop.ToString());
                    break;
            }
        }

        return str.ToString();
    }

    /// <summary>
    /// Gets the YAML format string of the value.
    /// </summary>
    /// <returns>A YAML format string.</returns>
    public string ToYamlString()
        => ConvertToYamlString(0);

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
                str.AppendLine("~");
                continue;
            }

            switch (item.ValueKind)
            {
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    str.AppendLine("~");
                    break;
                case JsonValueKind.Array:
                    if (item is not JsonArrayNode jArr)
                    {
                        str.AppendLine("[]");
                        break;
                    }

                    str.AppendLine(jArr.ToString());
                    break;
                case JsonValueKind.Object:
                    if (item is not JsonObjectNode jObj)
                    {
                        str.AppendLine("{}");
                        break;
                    }

                    str.AppendLine();
                    str.Append(jObj.ConvertToYamlString(indentLevel));
                    break;
                case JsonValueKind.String:
                    if (item is not IJsonStringNode jStr)
                    {
                        str.AppendLine(item.ToString());
                        break;
                    }

                    var text = jStr.StringValue;
                    if (text == null)
                    {
                        str.AppendLine("~");
                        break;
                    }

                    str.AppendLine(text.Length == 0 || text.Length > 100 || text.IndexOfAny(StringExtensions.YamlSpecialChars) >= 0
                        ? JsonStringNode.ToJson(text)
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
    public IEnumerator<IJsonDataNode> GetEnumerator()
        => store.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the array.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the array.</returns>
    IEnumerator<IJsonValueNode> IEnumerable<IJsonValueNode>.GetEnumerator()
        => store.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the array.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the array.</returns>
    IEnumerator IEnumerable.GetEnumerator()
        => store.GetEnumerator();

    /// <summary>
    /// Gets the value of the element as a boolean.
    /// </summary>
    /// <returns>The value of the element as a boolean.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    bool IJsonDataNode.GetBoolean() => throw new InvalidOperationException("Expect a boolean but it is an array.");

    /// <summary>
    /// Gets the value of the element as a byte array.
    /// </summary>
    /// <returns>The value decoded as a byte array.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    byte[] IJsonDataNode.GetBytesFromBase64() => throw new InvalidOperationException("Expect a string but it is an array.");

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <returns>The value of the element as a date time.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    DateTime IJsonDataNode.GetDateTime() => throw new InvalidOperationException("Expect a date time but it is an array.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    decimal IJsonDataNode.GetDecimal() => throw new InvalidOperationException("Expect a number but it is an array.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    float IJsonDataNode.GetSingle() => throw new InvalidOperationException("Expect a number but it is an array.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    double IJsonDataNode.GetDouble() => throw new InvalidOperationException("Expect a number but it is an array.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    short IJsonDataNode.GetInt16() => throw new InvalidOperationException("Expect a number but it is an array.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    uint IJsonDataNode.GetUInt32() => throw new InvalidOperationException("Expect a number but it is an array.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    int IJsonDataNode.GetInt32() => throw new InvalidOperationException("Expect a number but it is an array.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    long IJsonDataNode.GetInt64() => throw new InvalidOperationException("Expect a number but it is an array.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    string IJsonDataNode.GetString() => throw new InvalidOperationException("Expect a string but it is an array.");

    /// <summary>
    /// Gets the value of the element as a GUID.
    /// </summary>
    /// <returns>The value of the element as a GUID.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    Guid IJsonDataNode.GetGuid() => throw new InvalidOperationException("Expect a string but it is an array.");
    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetBoolean(out bool result)
    {
        result = false;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetDateTime(out DateTime result)
    {
        result = Web.WebFormat.ParseDate(0);
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetDecimal(out decimal result)
    {
        result = 0;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetSingle(out float result)
    {
        result = 0;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetDouble(out double result)
    {
        result = 0;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetUInt32(out uint result)
    {
        result = 0;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetInt32(out int result)
    {
        result = 0;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetInt64(out long result)
    {
        result = 0;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetString(out string result)
    {
        result = null;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a GUID.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetGuid(out Guid result)
    {
        result = Guid.Empty;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    public bool TryGetValue(string key, out IJsonDataNode result)
    {
        if (key != null)
        {
            if (int.TryParse(key.Trim(), out var i)) return TryGetValue(i, out result);
            switch (key.Trim().ToLower())
            {
                case "len":
                case "length":
                case "count":
                    result = new JsonIntegerNode(Count);
                    return true;
                case "first":
                    if (Count == 0) break;
                    return TryGetValue(0, out result);
                case "last":
                    if (Count == 0) break;
                    return TryGetValue(Count - 1, out result);
                case "*":
                case "all":
                case "":
                    result = this;
                    return true;
                case "reverse":
                    result = Reverse();
                    return true;
            }
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    public bool TryGetValue(ReadOnlySpan<char> key, out IJsonDataNode result)
    {
        if (key != null) return TryGetValue(key.ToString(), out result);
        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific index.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The element; or null, if get failed.</returns>
    public IJsonDataNode TryGetValue(string key)
        => TryGetValue(key, out var result) ? result : default;

    /// <summary>
    /// Gets all property keys.
    /// </summary>
    /// <returns>The property keys.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
    IEnumerable<string> IJsonDataNode.GetKeys()
    {
        var list = new List<string>();
        for (var i = 0; i < Count; i++)
        {
            list.Add(i.ToString("g"));
        }

        return list;
    }

    /// <summary>
    /// Gets all property keys.
    /// </summary>
    /// <returns>The property keys.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
    IEnumerable<string> IJsonContainerNode.GetKeys()
    {
        var list = new List<string>();
        for (var i = 0; i < Count; i++)
        {
            list.Add(i.ToString("g"));
        }

        return list;
    }

    /// <summary>
    /// Inverts the order of the elements in a sequence.
    /// </summary>
    /// <returns>A sequence whose elements correspond to those of the input sequence in reverse order.</returns>
    public JsonArrayNode Reverse()
        => new(store.Reverse().ToList());

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public JsonArrayNode Clone()
        => new(store, store is SynchronizedList<IJsonDataNode>);

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneable.Clone()
        => Clone();

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(JsonArrayNode leftValue, IJsonValueNode rightValue)
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
    public static bool operator !=(JsonArrayNode leftValue, IJsonValueNode rightValue)
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
    private IEnumerable<T> GetSpecificKindValues<T>(bool nullForOtherKinds) where T : IJsonDataNode
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

    private T GetJsonValue<T>(int index, JsonValueKind? valueKind = null) where T : IJsonValueNode
    {
        var data = store[index] ?? throw new InvalidOperationException($"The item {index} is null.");
        if (data is T v) return v;
        throw new InvalidOperationException(valueKind.HasValue
            ? $"The kind of item {index} should be {valueKind.Value.ToString().ToLowerInvariant()} but it is {data.ValueKind.ToString().ToLowerInvariant()}."
            : $"The kind of item {index} is {data.ValueKind.ToString().ToLowerInvariant()}, not expected.");
    }

    private bool TryGetJsonValue<T>(int index, out T property) where T : IJsonValueNode
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
    /// <returns>An instance of the JsonDocument class.</returns>
    public static explicit operator JsonDocument(JsonArrayNode json)
    {
        if (json == null) return null;
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        json.WriteTo(writer);
        writer.Flush();
        stream.Position = 0;
        return JsonDocument.Parse(stream);
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonObjectNode class.</returns>
    public static explicit operator JsonObjectNode(JsonArrayNode json)
    {
        if (json == null) return null;
        var i = 0;
        var obj = new JsonObjectNode();
        foreach (var item in json)
        {
            obj[i.ToString("g")] = item;
            i++;
        }

        return obj;
    }

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonArrayNode class.</returns>
    public static explicit operator System.Text.Json.Nodes.JsonArray(JsonArrayNode json)
    {
        if (json == null) return null;
        var node = new System.Text.Json.Nodes.JsonArray();
        foreach (var item in json.store)
        {
            var v = JsonValues.ToJsonNode(item);
            node.Add(v);
        }

        return node;
    }

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonArrayNode class.</returns>
    public static explicit operator System.Text.Json.Nodes.JsonNode(JsonArrayNode json)
        => (System.Text.Json.Nodes.JsonArray)json;

    /// <summary>
    /// Converts from JSON document.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonArrayNode class.</returns>
    /// <exception cref="JsonException">json does not represent a valid JSON array.</exception>
    public static implicit operator JsonArrayNode(JsonDocument json)
        => json.RootElement;

    /// <summary>
    /// Converts from JSON element.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonArrayNode class.</returns>
    /// <exception cref="JsonException">json does not represent a valid JSON array.</exception>
    public static implicit operator JsonArrayNode(JsonElement json)
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

        var result = new JsonArrayNode();
        for (var i = 0; i < json.GetArrayLength(); i++)
        {
            result.Add(json[i]);
        }

        return result;
    }
    /// <summary>
    /// Converts from JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonArrayNode class.</returns>
    /// <exception cref="JsonException">json does not represent a valid JSON array.</exception>
    public static implicit operator JsonArrayNode(System.Text.Json.Nodes.JsonArray json)
    {
        if (json is null) return null;
        var result = new JsonArrayNode();
        foreach (var item in json)
        {
            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// Converts from JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonArrayNode class.</returns>
    /// <exception cref="JsonException">json does not represent a valid JSON array.</exception>
    public static implicit operator JsonArrayNode(System.Text.Json.Nodes.JsonNode json)
    {
        if (json is System.Text.Json.Nodes.JsonArray obj) return obj;
        throw new JsonException("json is not a JSON array.");
    }

    /// <summary>
    /// Parses JSON array.
    /// </summary>
    /// <param name="json">A specific JSON array string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON array instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
    /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
    public static JsonArrayNode Parse(string json, JsonDocumentOptions options = default)
    {
        return JsonDocument.Parse(json, options);
    }

    /// <summary>
    /// Parses JSON object.
    /// </summary>
    /// <param name="json">A specific JSON object string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static JsonArrayNode Parse(System.Buffers.ReadOnlySequence<byte> json, JsonDocumentOptions options = default)
    {
        return JsonDocument.Parse(json, options);
    }

    /// <summary>
    /// Parses JSON object.
    /// </summary>
    /// <param name="json">A specific JSON object string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static JsonArrayNode Parse(ReadOnlyMemory<byte> json, JsonDocumentOptions options = default)
    {
        return JsonDocument.Parse(json, options);
    }

    /// <summary>
    /// Parses JSON object.
    /// </summary>
    /// <param name="json">A specific JSON object string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static JsonArrayNode Parse(ReadOnlyMemory<char> json, JsonDocumentOptions options = default)
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
    public static JsonArrayNode Parse(Stream utf8Json, JsonDocumentOptions options = default)
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
    public static async Task<JsonArrayNode> ParseAsync(Stream utf8Json, JsonDocumentOptions options, CancellationToken cancellationToken = default)
    {
        return await JsonDocument.ParseAsync(utf8Json, options, cancellationToken);
    }

    /// <summary>
    /// Parses a stream as UTF-8-encoded data representing a JSON array.
    /// The stream is read to completion.
    /// </summary>
    /// <param name="utf8Json">The JSON data to parse.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
    /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
    public static async Task<JsonArrayNode> ParseAsync(Stream utf8Json, CancellationToken cancellationToken = default)
    {
        return await JsonDocument.ParseAsync(utf8Json, default, cancellationToken);
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Parses a stream as UTF-8-encoded data representing a JSON array.
    /// The stream is read to completion.
    /// </summary>
    /// <param name="zip">The zip file.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that identifies the entry to retrieve.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    /// <exception cref="IOException">The entry is already currently open for writing, or the entry has been deleted from the archive.</exception>
    /// <exception cref="ObjectDisposedException">The zip archive has been disposed.</exception>
    /// <exception cref="NotSupportedException">The zip archive does not support reading.</exception>
    /// <exception cref="InvalidDataException">The zip archive is corrupt, and the entry cannot be retrieved.</exception>
    public static Task<JsonArrayNode> ParseAsync(System.IO.Compression.ZipArchive zip, string entryName, JsonDocumentOptions options, CancellationToken cancellationToken = default)
    {
        var entry = zip?.GetEntry(entryName);
        if (entry == null) return Task.FromResult<JsonArrayNode>(null);
        using var stream = entry.Open();
        return ParseAsync(stream, options, cancellationToken);
    }

    /// <summary>
    /// Parses a stream as UTF-8-encoded data representing a JSON array.
    /// The stream is read to completion.
    /// </summary>
    /// <param name="zip">The zip file.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that identifies the entry to retrieve.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    /// <exception cref="IOException">The entry is already currently open for writing, or the entry has been deleted from the archive.</exception>
    /// <exception cref="ObjectDisposedException">The zip archive has been disposed.</exception>
    /// <exception cref="NotSupportedException">The zip archive does not support reading.</exception>
    /// <exception cref="InvalidDataException">The zip archive is corrupt, and the entry cannot be retrieved.</exception>
    public static Task<JsonArrayNode> ParseAsync(System.IO.Compression.ZipArchive zip, string entryName, CancellationToken cancellationToken = default)
        => ParseAsync(zip, entryName, default, cancellationToken);
#endif

    /// <summary>
    /// Parses JSON array.
    /// </summary>
    /// <param name="reader">A JSON object.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON array.</exception>
    /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
    public static JsonArrayNode ParseValue(ref Utf8JsonReader reader)
        => JsonDocument.ParseValue(ref reader);

    /// <summary>
    /// Tries to parse a string to a JSON array.
    /// </summary>
    /// <param name="json">A specific JSON array string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON array instance; or null, if error format.</returns>
    public static JsonArrayNode TryParse(string json, JsonDocumentOptions options = default)
    {
        try
        {
            return Parse(json, options);
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (JsonException)
        {
        }
        catch (FormatException)
        {
        }
        catch (InvalidCastException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (AggregateException)
        {
        }

        return null;
    }

    /// <summary>
    /// Tries to parse a string to a JSON array.
    /// </summary>
    /// <param name="file">A file with JSON array string content to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON array instance; or null, if error format.</returns>
    public static JsonArrayNode TryParse(FileInfo file, JsonDocumentOptions options = default)
    {
        try
        {
            if (file == null || !file.Exists) return null;
            using var stream = file.OpenRead();
            return Parse(stream, options);
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (JsonException)
        {
        }
        catch (FormatException)
        {
        }
        catch (InvalidCastException)
        {
        }
        catch (IOException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (AggregateException)
        {
        }
        catch (ExternalException)
        {
        }

        return null;
    }

    /// <summary>
    /// Converts an object to JSON object.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    public static JsonArrayNode ConvertFrom(object obj, JsonSerializerOptions options = default)
    {
        if (obj is null) return null;
        if (obj is IJsonValueNode)
        {
            if (obj is JsonArrayNode jArr) return jArr;
            if (obj is JsonObjectNode jObj) return new JsonArrayNode { jObj };
            if (obj is IJsonStringNode jStr) return new JsonArrayNode { jStr.StringValue };
            if (obj is IJsonValueNode<string> jStr2) return new JsonArrayNode { jStr2.Value };
            if (obj is IJsonValueNode<bool> jBool) return new JsonArrayNode { jBool.Value };
            if (obj is JsonIntegerNode jInt) return new JsonArrayNode { jInt.Value };
            if (obj is JsonDoubleNode jFloat) return new JsonArrayNode { jFloat.Value };
            if (obj is JsonNullNode) return null;
            var jValue = obj as IJsonValueNode;
            var valueKind = jValue.ValueKind;
            switch (valueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                case JsonValueKind.False:
                    return new JsonArrayNode { false };
                case JsonValueKind.True:
                    return new JsonArrayNode { true };
                case JsonValueKind.Number:
                    return new JsonArrayNode { jValue.ToString() };
            }
        }

        if (obj is JsonDocument doc) return doc;
        if (obj is string str) return Parse(str);
        if (obj is StringBuilder sb) return Parse(sb.ToString());
        if (obj is System.Text.Json.Nodes.JsonArray ja) return ja;
        if (obj is Stream stream) return Parse(stream);
        if (obj is IEnumerable<object> arr)
        {
            var r = new JsonArrayNode();
            foreach (var item in arr)
            {
                r.Add(ConvertFrom(item));
            }

            return r;
        }

        if (obj is IEnumerable<JsonObjectNode> arr2)
        {
            var r = new JsonArrayNode();
            foreach (var item in arr2)
            {
                r.Add(item);
            }

            return r;
        }

        var s = JsonSerializer.Serialize(obj, obj.GetType(), options);
        return Parse(s);
    }

    private static bool PassTrue(IJsonDataNode data, int index, int index2)
        => true;
}
