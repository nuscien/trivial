﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Data;
using Trivial.Maths;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Web;

namespace Trivial.Text;

/// <summary>
/// Represents a specific JSON object.
/// </summary>
[Serializable]
[System.Text.Json.Serialization.JsonConverter(typeof(JsonObjectNodeConverter))]
public class JsonObjectNode : IJsonContainerNode, IJsonDataNode, IDictionary<string, IJsonValueNode>, IDictionary<string, IJsonDataNode>, IReadOnlyDictionary<string, IJsonValueNode>, IReadOnlyDictionary<string, IJsonDataNode>, IEquatable<JsonObjectNode>, IEquatable<IJsonValueNode>, ISerializable
{
#pragma warning disable IDE0056,IDE0057
    private IDictionary<string, IJsonDataNode> store = new Dictionary<string, IJsonDataNode>();

    /// <summary>
    /// Initializes a new instance of the JsonObjectNode class.
    /// </summary>
    public JsonObjectNode()
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonObjectNode class.
    /// </summary>
    /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
    protected JsonObjectNode(SerializationInfo info, StreamingContext context)
    {
        if (info is null) return;
        foreach (var prop in info)
        {
            if (prop.Value is null)
            {
                SetNullValue(prop.Name);
            }
            else if (prop.ObjectType.IsValueType)
            {
                if (prop.Value is long l)
                    SetValue(prop.Name, l);
                else if (prop.Value is int i)
                    SetValue(prop.Name, i);
                else if (prop.Value is uint ui)
                    SetValue(prop.Name, ui);
                else if (prop.Value is double d)
                    SetValue(prop.Name, d);
                else if (prop.Value is float f)
                    SetValue(prop.Name, f);
                else if (prop.Value is decimal de)
                    SetValue(prop.Name, de);
                else if (prop.Value is DateTime dt)
                    SetValue(prop.Name, dt);
                else if (prop.Value is Guid g)
                    SetValue(prop.Name, g);
                else if (prop.Value is JsonElement ele)
                    SetValue(prop.Name, ele);
            }
            else if (prop.Value is string s)
            {
                SetValue(prop.Name, s);
            }
            else if (prop.Value is JsonObjectNode json)
            {
                SetValue(prop.Name, json);
            }
            else if (prop.Value is JsonArrayNode arr)
            {
                SetValue(prop.Name, arr);
            }
            else if (prop.Value is System.Text.Json.Nodes.JsonObject json2)
            {
                SetValue(prop.Name, json2);
            }
            else if (prop.Value is System.Text.Json.Nodes.JsonArray arr2)
            {
                SetValue(prop.Name, arr2);
            }
            else if (prop.Value is IJsonObjectHost json3)
            {
                SetValue(prop.Name, json3.ToJson());
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the JsonObjectNode class.
    /// </summary>
    /// <param name="reader">The UTF8 JSON reader to read from.</param>
    internal JsonObjectNode(ref Utf8JsonReader reader)
    {
        SetRange(ref reader);
    }

    /// <summary>
    /// Initializes a new instance of the JsonObjectNode class.
    /// </summary>
    /// <param name="copy">Properties to initialzie.</param>
    /// <param name="threadSafe">true if enable thread-safe; otherwise, false.</param>
    private JsonObjectNode(IDictionary<string, IJsonDataNode> copy, bool threadSafe = false)
    {
        if (threadSafe) store = new ConcurrentDictionary<string, IJsonDataNode>();
        if (copy == null) return;
        foreach (var ele in copy)
        {
            store[ele.Key] = ele.Value;
        }
    }

    /// <summary>
    /// Adds or removes the event handler raised on property changed.
    /// </summary>
    public event KeyValueEventHandler<string, IJsonDataNode> PropertyChanged;

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
    IEnumerable<string> IReadOnlyDictionary<string, IJsonValueNode>.Keys => store.Keys;

    /// <summary>
    /// Gets a collection containing the property keys of the object.
    /// </summary>
    IEnumerable<string> IReadOnlyDictionary<string, IJsonDataNode>.Keys => store.Keys;

    /// <summary>
    /// Gets a collection containing the property values of the object.
    /// </summary>
    public ICollection<IJsonDataNode> Values => store.Values;

    /// <summary>
    /// Gets a collection containing the property values of the object.
    /// </summary>
    ICollection<IJsonValueNode> IDictionary<string, IJsonValueNode>.Values => store.Select(ele => (ele.Value ?? JsonValues.Null) as IJsonValueNode).ToList();

    /// <summary>
    /// Gets a collection containing the property values of the object.
    /// </summary>
    IEnumerable<IJsonValueNode> IReadOnlyDictionary<string, IJsonValueNode>.Values => store.Values;

    /// <summary>
    /// Gets a collection containing the property values of the object.
    /// </summary>
    IEnumerable<IJsonDataNode> IReadOnlyDictionary<string, IJsonDataNode>.Values => store.Values;

    /// <summary>
    /// Gets a value indicating whether the JSON object is read-only.
    /// </summary>
    public bool IsReadOnly => store.IsReadOnly;

    /// <summary>
    /// Gets or sets the identifier of the JSON object.
    /// </summary>
    public string Id
    {
        get => TryGetStringValue("$id")?.Trim();
        set => SetValueOrRemove("$id", value);
    }

    /// <summary>
    /// Gets or sets the schema URL of the JSON object.
    /// </summary>
    public string Schema
    {
        get => TryGetStringValue("$schema");
        set => SetValueOrRemove("$schema", value);
    }

    /// <summary>
    /// Gets or sets the subschema/definitions of the JSON object.
    /// </summary>
    public JsonObjectNode LocalDefinitions
    {
        get => TryGetObjectValue("$defs");
        set => SetValueOrRemove("$defs", value);
    }

    /// <summary>
    /// Gets or sets the comment of the JSON object.
    /// </summary>
    public string CommentValue
    {
        get => TryGetStringValue("$comment");
        set => SetValueOrRemove("$comment", string.IsNullOrWhiteSpace(value) ? null : value);
    }

    /// <summary>
    /// Gets or sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public IJsonDataNode this[string key]
    {
        get => GetValue(key);
        set => SetProperty(key, JsonValues.ConvertValue(value, this));
    }

    /// <summary>
    /// Gets or sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    IJsonValueNode IDictionary<string, IJsonValueNode>.this[string key]
    {
        get => GetValue(key);
        set => SetProperty(key, JsonValues.ConvertValue(value, this));
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    IJsonValueNode IReadOnlyDictionary<string, IJsonValueNode>.this[string key]
    {
        get => GetValue(key);
    }

    /// <summary>
    /// Gets or sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="index">The index of the array.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist; or the index is less than zero.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public IJsonDataNode this[string key, int index]
    {
        get
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "index should be a natural number.");
            var result = GetValue(key);
            if (result is JsonArrayNode arr) return arr[index];
            else if (result is JsonObjectNode json) return ConvertObjectValueForProperty(json)[index.ToString("g")];
            else if (result is IJsonStringNode str) return new JsonStringNode(str.StringValue[index].ToString());
            else if (index == 0) return result;
            throw new InvalidOperationException($"The property of {key} should be an array, but its kind is {result?.ValueKind ?? JsonValueKind.Null}.");
        }
    }

    /// <summary>
    /// Gets or sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="index">The index of the array.</param>
    /// <param name="subIndex">The optional sub index of the value of the array.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist; or the index is less than zero.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public IJsonDataNode this[string key, int index, int subIndex]
    {
        get
        {
            if (subIndex < 0) throw new ArgumentOutOfRangeException(nameof(subIndex), "subIndex should be a natural number.");
            var result = this[key, index];
            try
            {
                if (result is JsonArrayNode arr) return arr[subIndex];
                else if (result is JsonObjectNode json) return json[subIndex.ToString("g")];
                else if (result is IJsonStringNode str) return new JsonStringNode(str.StringValue[subIndex].ToString());
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException(nameof(subIndex), ex.Message);
            }

            throw new InvalidOperationException($"The property of {key}.{index} should be an array, but its kind is {result?.ValueKind ?? JsonValueKind.Null}.");
        }
    }

    /// <summary>
    /// Gets or sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="index">The index of the array.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist; or the index is less than zero.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public IJsonDataNode this[string key, int index, params string[] keyPath]
    {
        get
        {
            var result = this[key, index];
            if (keyPath == null || keyPath.Length == 0) return result;
            if (result is JsonObjectNode json) return json.GetValue(keyPath);
            if (result is JsonArrayNode arr)
            {
                var subKey = keyPath[0];
                var i = 1;
                while (subKey == null)
                {
                    if (keyPath.Length <= i) return result;
                    subKey = keyPath[i];
                    i++;
                }

                if (int.TryParse(subKey, out var index2))
                {
                    if (keyPath.Length == i) return arr[index2];
                    return arr[index2, keyPath[i], keyPath.Skip(i + 1)?.ToArray()];
                }
            }

            throw new InvalidOperationException($"The property of {key}.{index} should be a JSON object, but its kind is {result?.ValueKind ?? JsonValueKind.Null}.");
        }
    }

    /// <summary>
    /// Gets or sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="subKey">The optional sub-property key of the value of the array.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public IJsonDataNode this[string key, string subKey, params string[] keyPath]
    {
        get
        {
            var path = new List<string>
            {
                key,
                subKey
            };
            path.AddRange(keyPath);
            return GetValue(path);
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
        if (store is ConcurrentDictionary<string, IJsonDataNode>)
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
                    store = new ConcurrentDictionary<string, IJsonDataNode>(store);
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
            if (ele.Value is JsonObjectNode json) json.EnableThreadSafeMode(depth, true);
            else if (ele.Value is JsonArrayNode arr) arr.EnableThreadSafeMode(depth, true);
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
        foreach (var prop in store)
        {
            switch (prop.Value.ValueKind)
            {
                case JsonValueKind.Undefined:
                    break;
                case JsonValueKind.Null:
                    break;
                case JsonValueKind.String:
                    if (prop.Value is JsonStringNode strJson) info.AddValue(prop.Key, strJson.Value, typeof(string));
                    break;
                case JsonValueKind.Number:
                    if (prop.Value is JsonIntegerNode intJson) info.AddValue(prop.Key, intJson.Value);
                    else if (prop.Value is JsonDoubleNode floatJson) info.AddValue(prop.Key, floatJson.Value);
                    break;
                case JsonValueKind.True:
                    info.AddValue(prop.Key, true);
                    break;
                case JsonValueKind.False:
                    info.AddValue(prop.Key, false);
                    break;
                case JsonValueKind.Object:
                    if (prop.Value is JsonObjectNode objJson) info.AddValue(prop.Key, objJson, typeof(JsonObjectNode));
                    break;
                case JsonValueKind.Array:
                    if (prop.Value is JsonArrayNode objArr) info.AddValue(prop.Key, objArr, typeof(JsonArrayNode));
                    break;
            }
        }
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
    /// Tests if the value kind of the property is the specific one.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="kind">The value kind expected.</param>
    /// <returns>true if the value kind is the specific one; otherwise, false.</returns>
    public bool IsValueKind(string key, JsonValueKind kind)
    {
        if (!store.TryGetValue(key, out var value) || value is null) return kind == JsonValueKind.Undefined;
        return value.ValueKind == kind;
    }

    /// <summary>
    /// Determines whether it contains a property with the specific key.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>true if it contains the property key; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty or consists only of white-space characters.</exception>
    public bool ContainsKey(string key)
    {
        AssertKey(key);
        return store.ContainsKey(key);
    }

    /// <summary>
    /// Determines whether it contains a property with the specific key.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>true if it contains the property key; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty or consists only of white-space characters.</exception>
    public bool ContainsKey(ReadOnlySpan<char> key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key), "key should not be null.");
        return ContainsKey(key.ToString());
    }

    /// <summary>
    /// Determines whether it contains the property only with the specific key.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>true if it contains the property key only; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty or consists only of white-space characters.</exception>
    public bool ContainsOnlyKey(string key)
    {
        AssertKey(key);
        return store.Count == 1 && store.ContainsKey(key);
    }

    /// <summary>
    /// Determines whether it contains the property only with the specific key.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>true if it contains the property key only; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty or consists only of white-space characters.</exception>
    public bool ContainsOnlyKey(string key, out string value)
    {
        AssertKey(key);
        value = TryGetStringValue(key);
        return store.Count == 1 && store.ContainsKey(key);
    }

    /// <summary>
    /// Determines whether it contains the property only with the specific key.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>true if it contains the property key only; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty or consists only of white-space characters.</exception>
    public bool ContainsOnlyKey(string key, out int value)
    {
        AssertKey(key);
        value = TryGetInt32Value(key) ?? default;
        return store.Count == 1 && store.ContainsKey(key);
    }

    /// <summary>
    /// Determines whether it contains the property only with the specific key.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>true if it contains the property key only; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty or consists only of white-space characters.</exception>
    public bool ContainsOnlyKey(string key, out long value)
    {
        AssertKey(key);
        value = TryGetInt64Value(key) ?? default;
        return store.Count == 1 && store.ContainsKey(key);
    }

    /// <summary>
    /// Determines whether it contains the property only with the specific key.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>true if it contains the property key only; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty or consists only of white-space characters.</exception>
    public bool ContainsOnlyKey(string key, out JsonObjectNode value)
    {
        AssertKey(key);
        value = TryGetObjectValue(key);
        return store.Count == 1 && store.ContainsKey(key);
    }

    /// <summary>
    /// Determines whether it contains the property only with the specific key.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>true if it contains the property key only; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty or consists only of white-space characters.</exception>
    public bool ContainsOnlyKey(string key, out JsonArrayNode value)
    {
        AssertKey(key);
        value = TryGetArrayValue(key);
        return store.Count == 1 && store.ContainsKey(key);
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
        if (key == null) throw new ArgumentNullException(nameof(key), "key should not be null.");
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
            if (strictMode) throw new ArgumentOutOfRangeException(nameof(key), "key does not exist.");
            return JsonValueKind.Undefined;
        }

        if (data is null) return JsonValueKind.Null;
        return data.ValueKind;
    }

    /// <summary>
    /// Gets the value kind of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The value result output.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public JsonValueKind GetValueKind(string key, out object result)
    {
        var kind = GetValueKind(key, false);
        switch (kind)
        {
            case JsonValueKind.String:
                result = TryGetStringValue(key);
                break;
            case JsonValueKind.True:
                result = true;
                break;
            case JsonValueKind.False:
                result = false;
                break;
            case JsonValueKind.Object:
                result = TryGetObjectValue(key);
                break;
            case JsonValueKind.Array:
                result = TryGetArrayValue(key);
                break;
            case JsonValueKind.Number:
                var d = TryGetDoubleValue(key);
                if (double.IsNaN(d))
                {
                    result = d;
                }
                else if (TryGetInt64Value(key, out var l) && d == l)
                {
                    if (l <= int.MaxValue && l >= int.MinValue)
                        result = (int)l;
                    else
                        result = l;
                }
                else
                {
                    result = d;
                }

                break;
            default:
                result = null;
                break;
        }

        return kind;
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
        if (key == null) throw new ArgumentNullException(nameof(key), "key should not be null.");
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

        if (data is JsonStringNode str)
        {
            return str.Value;
        }

        if (!strictMode) return data.ValueKind switch
        {
            JsonValueKind.True => JsonBooleanNode.TrueString,
            JsonValueKind.False => JsonBooleanNode.FalseString,
            JsonValueKind.Number => data.ToString(),
            _ => throw new InvalidOperationException($"The value kind of property {key} should be string but it is {data.ValueKind.ToString().ToLowerInvariant()}.")
        };
        throw new InvalidOperationException($"The value kind of property {key} should be string but it is {data.ValueKind.ToString().ToLowerInvariant()}.");
    }

    /// <summary>
    /// Gets the substring of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
    /// <param name="length">The number of characters in the substring.</param>
    /// <returns>The substring.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist; startIndex of length is less than 0; or startIndex plus length indicates a position not within this property string value.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public string GetSubstringValue(string key, int startIndex, int? length = null)
    {
        var s = GetStringValue(key, true) ?? throw new InvalidOperationException($"The value of property {key} is null.");
        return length.HasValue ? s.Substring(startIndex, length.Value) : s.Substring(startIndex);
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
        => Guid.Parse(GetStringValue(key));

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
        if (TryGetJsonValue<JsonIntegerNode>(key, out var v)) return (uint)v;
        if (TryGetJsonValue<JsonDoubleNode>(key, out var f)) return (uint)f;
        var p = GetJsonValue<JsonStringNode>(key, JsonValueKind.Number);
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
        if (TryGetJsonValue<JsonIntegerNode>(key, out var v)) return (int)v;
        if (TryGetJsonValue<JsonDoubleNode>(key, out var f)) return (int)f;
        var p = GetJsonValue<JsonStringNode>(key, JsonValueKind.Number);
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
        if (TryGetJsonValue<JsonIntegerNode>(key, out var v)) return v.Value;
        if (TryGetJsonValue<JsonDoubleNode>(key, out var f)) return (long)f;
        var p = GetJsonValue<JsonStringNode>(key, JsonValueKind.Number);
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
        if (TryGetJsonValue<JsonDoubleNode>(key, out var v)) return (float)v;
        if (TryGetJsonValue<JsonIntegerNode>(key, out var f)) return (float)f;
        var p = GetJsonValue<JsonStringNode>(key, JsonValueKind.Number);
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
        if (TryGetJsonValue<JsonDoubleNode>(key, out var v)) return v.Value;
        if (TryGetJsonValue<JsonIntegerNode>(key, out var f)) return (float)f;
        var p = GetJsonValue<JsonStringNode>(key, JsonValueKind.Number);
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
        if (TryGetJsonValue<JsonBooleanNode>(key, out var v)) return v.Value;
        var p = GetJsonValue<JsonStringNode>(key);
        return p.Value?.ToLower() switch
        {
            JsonBooleanNode.TrueString => true,
            JsonBooleanNode.FalseString => false,
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
    public JsonObjectNode GetObjectValue(string key)
    {
        AssertKey(key);
        var obj = GetJsonValue<JsonObjectNode>(key, JsonValueKind.Object, true);
        if (obj == null) return obj;
        if (obj.Count == 1 && obj.TryGetStringValue("$ref") == JsonValues.SELF_REF) return this;
        return obj;
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="subKey">The sub-key of the previous property.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">Cannot get the property value.</exception>
    public JsonObjectNode GetObjectValue(string key, string subKey, params string[] keyPath)
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
    public JsonObjectNode GetObjectValue(IEnumerable<string> keyPath)
    {
        var result = GetValue(keyPath);
        if (result is null) return null;
        if (result is JsonObjectNode jObj) return jObj;
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
    public JsonArrayNode GetArrayValue(string key)
    {
        AssertKey(key);
        return GetJsonValue<JsonArrayNode>(key, JsonValueKind.Array);
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
        if (TryGetJsonValue<JsonStringNode>(key, out var s))
        {
            var date = WebFormat.ParseDate(s.Value);
            if (date.HasValue) return date.Value;
            throw new InvalidOperationException("The value is not a date time.");
        }

        var num = GetJsonValue<JsonIntegerNode>(key);
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
    /// <typeparam name="T">An enumeration type.</typeparam>
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
    /// <typeparam name="T">An enumeration type.</typeparam>
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
    /// <param name="type">An enumeration type.</param>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="ArgumentException">The type is not an System.Enum. -or- the value is either an empty string or only contains white space.  -or- value is a name, but not one of the named constants defined for the enumeration.</exception>
    /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public object GetEnumValue(Type type, string key)
    {
        if (TryGetInt32Value(key, out var v)) return type is null ? v : Enum.ToObject(type, v);
        var str = GetStringValue(key);
        return type is null ? str : Enum.Parse(type, str);
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="type">An enumeration type.</param>
    /// <param name="key">The property key.</param>
    /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="ArgumentException">The type is not an System.Enum. -or- the value is either an empty string or only contains white space.  -or- value is a name, but not one of the named constants defined for the enumeration.</exception>
    /// <exception cref="OverflowException">value is outside the range of the underlying type of enumType.</exception>
    /// <exception cref="InvalidOperationException">The value kind is not the expected one.</exception>
    public object GetEnumValue(Type type, string key, bool ignoreCase)
    {
        if (TryGetInt32Value(key, out var v)) return type is null ? v : Enum.ToObject(type, v);
        var str = GetStringValue(key);
        return type is null ? str : Enum.Parse(type, str, ignoreCase);
    }

    /// <summary>
    /// Gets local definition.
    /// </summary>
    /// <param name="id">The definition identifier or key.</param>
    /// <returns>The definition description; or null, if non-exists.</returns>
    public JsonObjectNode GetLocalDefinition(string id)
        => LocalDefinitions?.TryGetObjectValue(id);

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    public IJsonDataNode GetValue(string key)
    {
        AssertKey(key);
        return store[key] ?? JsonValues.Null;
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="undefined">true if return undefined when the property does not exist; otherwise, false.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist and argument undefined is false.</exception>
    public IJsonDataNode GetValue(string key, bool undefined)
    {
        AssertKey(key);
        if (!undefined) return store[key] ?? JsonValues.Null;
        return store.TryGetValue(key, out var v) ? (v ?? JsonValues.Null) : JsonValues.Undefined;
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">Cannot get the property value.</exception>
    public IJsonDataNode GetValue(IEnumerable<string> keyPath)
    {
        if (keyPath == null) return this;
        IJsonDataNode json = this;
        var path = new StringBuilder();
        foreach (var key in keyPath)
        {
            if (string.IsNullOrEmpty(key)) continue;
            if (json is null || json.ValueKind == JsonValueKind.Null || json.ValueKind == JsonValueKind.Undefined)
            {
                path.Remove(0, 1);
                var message = $"Cannot get property {key} because property {path} is null.";
                throw new InvalidOperationException(
                    message,
                    new ArgumentException(message, nameof(keyPath)));
            }

            path.Append('.');
            path.Append(key);
            if (json is JsonObjectNode jObj)
            {
                if (!jObj.ContainsKey(key))
                {
                    path.Remove(0, 1);
                    var message = $"Cannot get property {key} in property {path}.";
                    throw new InvalidOperationException(
                        message,
                        new ArgumentOutOfRangeException(nameof(keyPath), message));
                }

                var jObjToken = jObj.TryGetValue(key);
                if (jObjToken is JsonObjectNode jObj2 && jObj2.Count == 1 && jObj2.TryGetStringValue("$ref") == JsonValues.SELF_REF) continue;
                json = jObjToken;
                continue;
            }

            if (json is JsonArrayNode jArr)
            {
                var jValue = jArr.TryGetValue(key);
                if (jValue is null)
                {
                    path.Remove(0, 1);
                    var message = $"Cannot get item at {key} in property {path}.";
                    throw new InvalidOperationException(
                        message,
                        new ArgumentOutOfRangeException(nameof(keyPath), message));
                }

                json = jValue;
                continue;
            }
                
            if (json is JsonStringNode)
            {
                if (json.TryGetValue(key, out var jValue))
                {
                    json = jValue;
                    continue;
                }
            }

            {
                path.Remove(0, 1);
                var message = $"Cannot get property {key} because property {path} is not a JSON object.";
                throw new InvalidOperationException(
                    message,
                    new ArgumentOutOfRangeException(nameof(keyPath), message));
            }
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
    public IJsonDataNode GetValue(string key, string subKey, params string[] keyPath)
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
    public IJsonDataNode GetValue(ReadOnlySpan<char> key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key), "key should not be null.");
        return GetValue(key.ToString());
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="type">The type of value.</param>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The type is not supported to convert.</exception>
    public object GetValue(Type type, string key)
    {
        if (type == null) return null;
        if (type == typeof(string)) return GetStringValue(key);
        if (type.IsEnum) return type == typeof(JsonValueKind) ? GetValueKind(key) : GetEnumValue(type, key, false);
        if (type.IsValueType)
        {
            var kind = GetValueKind(key);
            if (kind == JsonValueKind.Null || kind == JsonValueKind.Undefined)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) return null;
                throw new InvalidOperationException("The type is value type but the value is null or undefined.", new InvalidCastException("Cannot cast null to a struct."));
            }

            if (type == typeof(int)) return GetInt32Value(key);
            if (type == typeof(long)) return GetInt64Value(key);
            if (type == typeof(bool)) return GetBooleanValue(key);
            if (type == typeof(double)) return GetDoubleValue(key);
            if (type == typeof(float)) return GetSingleValue(key);
            if (type == typeof(decimal)) return (decimal)GetDoubleValue(key);
            if (type == typeof(uint)) return GetUInt32Value(key);
            if (type == typeof(ulong)) return (ulong)GetInt64Value(key);
            if (type == typeof(short)) return (short)GetInt32Value(key);
            if (type == typeof(Guid)) return GetGuidValue(key);
            if (type == typeof(DateTime)) return GetDateTimeValue(key);
        }

        if (type == typeof(JsonObjectNode)) return GetObjectValue(key);
        if (type == typeof(JsonArrayNode)) return GetArrayValue(key);
        if (type == typeof(JsonDocument)) return (JsonDocument)GetObjectValue(key);
        if (type == typeof(System.Text.Json.Nodes.JsonObject)) return (System.Text.Json.Nodes.JsonObject)GetObjectValue(key);
        if (type == typeof(System.Text.Json.Nodes.JsonArray)) return (System.Text.Json.Nodes.JsonArray)GetArrayValue(key);
        if (type == typeof(Type)) return GetValue(key).GetType();
        if (type == typeof(IJsonDataNode) || type == typeof(IJsonValueNode) || type == typeof(JsonStringNode) || type == typeof(IJsonStringNode) || type == typeof(JsonIntegerNode) || type == typeof(JsonDoubleNode) || type == typeof(JsonBooleanNode) || type == typeof(IJsonNumberNode))
            return GetValue(key);

        if (type.IsClass)
        {
            var json = TryGetObjectValue(key);
            if (json != null) return JsonSerializer.Deserialize(json.ToString(), type);
        }

        throw new InvalidOperationException("The type is not supported to convert.", new InvalidCastException("Cannot cast."));
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The type is not supported to convert.</exception>
    public T GetValue<T>(string key)
        => (T)GetValue(typeof(T), key);

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The type is not supported to convert.</exception>
    public T GetValue<T>(ReadOnlySpan<char> key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key), "key should not be null.");
        return (T)GetValue(typeof(T), key.ToString());
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="subKey">The sub-key of the previous property.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">Cannot get the property value or the type is not supported to convert.</exception>
    public T GetValue<T>(string key, string subKey, params string[] keyPath)
    {
        var path = new List<string>
        {
            key,
            subKey
        };
        path.AddRange(keyPath);
        return GetValue<T>(path);
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="keyPath">The property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">Cannot get the property value or the type is not supported to convert.</exception>
    public T GetValue<T>(IEnumerable<string> keyPath)
    {
        var path = keyPath?.Where(ele => !string.IsNullOrEmpty(ele))?.ToList();
        var t = typeof(T);
        if (path == null || path.Count == 0)
        {
            if (t == typeof(JsonObjectNode) || t == typeof(IJsonDataNode) || t == typeof(IJsonValueNode)) return (T)(object)this;
            if (t == typeof(JsonDocument)) return (T)(object)(JsonDocument)this;
            if (t == typeof(System.Text.Json.Nodes.JsonObject)) return (T)(object)(System.Text.Json.Nodes.JsonObject)this;
            if (t == typeof(string)) return (T)(object)ToString();
            throw new ArgumentException("The key was empty.");
        }

        var k = path.LastOrDefault();
        path = path.Take(path.Count - 1).ToList();
        var json = GetValue(path) ?? throw new InvalidOperationException($"Cannot get the leaf property {k} of null.", new NullReferenceException("There is a node that is null or undefined in the property path."));
        if (json is JsonObjectNode j) return j.GetValue<T>(k);
        else if (json is JsonArrayNode a && int.TryParse(k, out var i)) return a.GetValue<T>(i);
        throw new InvalidOperationException($"The property {string.Join(".", path)} was {json.ValueKind} so cannot get its property.");
    }

    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode IJsonDataNode.GetValue(int index) => throw new InvalidOperationException("Expect an array but it is an object.");

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetValue(int index, out IJsonDataNode result)
    {
        result = default;
        return false;
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonDataNode IJsonDataNode.GetValue(Index index) => throw new InvalidOperationException("Expect an array but it is an object.");

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetValue(Index index, out IJsonDataNode result)
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
        if (!store.TryGetValue(key, out var data)) return null;
        if (data is null) return null;
        if (data is IJsonStringNode str) return str.StringValue;
        return data.ValueKind switch
        {
            JsonValueKind.True => JsonBooleanNode.TrueString,
            JsonValueKind.False => JsonBooleanNode.TrueString,
            JsonValueKind.Number => data.ToString(),
            _ => null
        };
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strictMode">true if enable strict mode; otherwise, false, to return undefined for non-existing.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringValue(string key, bool strictMode, out string result)
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

        if (data is IJsonStringNode str)
        {
            result = str.StringValue;
            return true;
        }

        result = data.ValueKind switch
        {
            JsonValueKind.True => JsonBooleanNode.TrueString,
            JsonValueKind.False => JsonBooleanNode.FalseString,
            JsonValueKind.Number => data.ToString(),
            _ => null
        };

        return !strictMode && result != null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringValue(string key, out string result)
        => TryGetStringValue(key, false, out result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>A string.</returns>
    public string TryGetStringValue(string key, Func<JsonObjectNode, string> converter)
    {
        if (!store.TryGetValue(key, out var data)) return null;
        if (data is null) return null;
        if (data is IJsonStringNode str) return str.StringValue;
        if (data is JsonObjectNode json) return converter?.Invoke(json);
        return data.ValueKind switch
        {
            JsonValueKind.True => JsonBooleanNode.TrueString,
            JsonValueKind.False => JsonBooleanNode.FalseString,
            JsonValueKind.Number => data.ToString(),
            _ => null
        };
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>A string.</returns>
    public string TryGetStringValue(string key, IJsonPropertyResolver<string> converter)
        => TryGetStringValue(key, converter, out var result) ? result : null;

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringValue(string key, IJsonPropertyResolver<string> converter, out string result)
    {
        if (!store.TryGetValue(key, out var data))
        {
            result = null;
            return false;
        }

        if (data is null)
        {
            result = null;
            return false;
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

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <returns>A string.</returns>
    public string TryGetStringValue(IEnumerable<string> keyPath)
    {
        var value = TryGetValue(keyPath);
        if (value is null) return null;
        return value.TryGetString(out var s) ? s : null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringValue(IEnumerable<string> keyPath, out string result)
    {
        var value = TryGetValue(keyPath);
        if (value is null)
        {
            result = null;
            return false;
        }

        return value.TryGetString(out result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="returnNullIfEmpty">true if returns null when the value is empty or white space; otherwise, false.</param>
    /// <returns>A string trimmed.</returns>
    public string TryGetStringTrimmedValue(string key, bool returnNullIfEmpty = false)
    {
        var s = TryGetStringValue(key)?.Trim();
        if (returnNullIfEmpty && string.IsNullOrEmpty(s)) return null;
        return s;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="valueCase">The case.</param>
    /// <param name="invariant">true if uses the casing rules of invariant culture; otherwise, false.</param>
    /// <param name="returnNullIfEmpty">true if returns null when the value is empty or white space; otherwise, false.</param>
    /// <returns>A string trimmed.</returns>
    public string TryGetStringTrimmedValue(string key, Cases valueCase, bool invariant = false, bool returnNullIfEmpty = false)
    {
        var s = TryGetStringTrimmedValue(key, returnNullIfEmpty);
        return invariant ? s.ToSpecificCaseInvariant(valueCase) : s.ToSpecificCase(valueCase);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result trimmed.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringTrimmedValue(string key, out string result)
    {
        if (!TryGetStringValue(key, out var r))
        {
            result = default;
            return false;
        }
        
        result = r?.Trim();
        return true;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="trueString">The string value of true.</param>
    /// <param name="falseString">The string value of false.</param>
    /// <param name="ignoreIfNotBoolean">true if return null if the kind of property is not boolean; otherwise, false, that means it will return the string value if it is a string or a number.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public string TryGetBooleanStringValue(string key, string trueString, string falseString, bool ignoreIfNotBoolean = false)
    {
        if (TryGetBooleanValue(key, true, out var b)) return b ? trueString : falseString;
        if (ignoreIfNotBoolean) return null;
        return TryGetStringValue(key);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="trueString">The string value of true.</param>
    /// <param name="falseString">The string value of false.</param>
    /// <param name="isBoolean">true if the kind is boolean; otherwise, false.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public string TryGetBooleanStringValue(string key, string trueString, string falseString, out bool isBoolean)
    {
        if (TryGetBooleanValue(key, true, out var b))
        {
            isBoolean = true;
            return b ? trueString : falseString;
        }

        isBoolean = false;
        return TryGetStringValue(key);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="ignoreNotMatched">true if ignore any item which is not JSON string; otherwise, false.</param>
    /// <returns>The list; or null, if no such array property.</returns>
    public List<string> TryGetStringListValue(string key, bool ignoreNotMatched = false)
    {
        if (!TryGetJsonValue<JsonArrayNode>(key, out var p))
        {
            var item = TryGetStringValue(key);
            return string.IsNullOrEmpty(item) ? null : new() { item };
        }

        var col = ignoreNotMatched ? p.Select(ele => ele is IJsonStringNode s ? s.StringValue : null).Where(ele => ele is not null) : p.Select(ele =>
        {
            if (ele is IJsonStringNode s) return s.StringValue;
            if (ele is JsonIntegerNode i) return i.ToString();
            if (ele is JsonDoubleNode f) return f.ToString();
            if (ele is JsonBooleanNode b) return b.ToString();
            return null;
        });
        return col.ToList();
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public Uri TryGetUriValue(string key)
    {
        if (!TryGetStringValue(key, out var str) || string.IsNullOrWhiteSpace(str)) return null;
        return StringExtensions.TryCreateUri(str);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="kind">Specifies whether the URI string is a relative URI, absolute URI, or is indeterminate.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public Uri TryGetUriValue(string key, UriKind kind)
    {
        if (!TryGetStringValue(key, out var str) || string.IsNullOrWhiteSpace(str)) return null;
        return StringExtensions.TryCreateUri(str, kind);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetUriValue(string key, out Uri result)
    {
        if (!TryGetStringValue(key, out var str) || string.IsNullOrWhiteSpace(str))
        {
            result = null;
            return false;
        }

        result = StringExtensions.TryCreateUri(str);
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
    public ushort? TryGetUInt16Value(string key)
    {
        try
        {
            if (TryGetJsonValue<JsonIntegerNode>(key, out var p1)) return (ushort)p1;
            if (TryGetJsonValue<JsonDoubleNode>(key, out var p2)) return (ushort)Math.Round(p2.Value);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }

        var str = TryGetStringValue(key);
        if (string.IsNullOrWhiteSpace(str) || !Numbers.TryParseToUInt16(str, 10, out var p3)) return null;
        return p3;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetUInt16Value(string key, out ushort result)
    {
        var v = TryGetUInt16Value(key);
        result = v ?? default;
        return v.HasValue;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public uint? TryGetUInt32Value(string key)
    {
        try
        {
            if (TryGetJsonValue<JsonIntegerNode>(key, out var p1)) return (uint)p1;
            if (TryGetJsonValue<JsonDoubleNode>(key, out var p2)) return (uint)Math.Round(p2.Value);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }

        var str = TryGetStringValue(key);
        if (string.IsNullOrWhiteSpace(str) || !Numbers.TryParseToUInt32(str, 10, out var p3)) return null;
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
    public short? TryGetInt16Value(string key)
    {
        try
        {
            if (TryGetJsonValue<JsonIntegerNode>(key, out var p1)) return (short)p1;
            if (TryGetJsonValue<JsonDoubleNode>(key, out var p2)) return (short)Math.Round(p2.Value);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }

        var str = TryGetStringValue(key);
        if (string.IsNullOrWhiteSpace(str) || !Numbers.TryParseToInt16(str, 10, out var p3)) return null;
        return p3;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetInt16Value(string key, out short result)
    {
        var v = TryGetInt16Value(key);
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
        if (TryGetJsonValue<JsonIntegerNode>(key, out var p1)) return (int)p1;
        try
        {
            if (TryGetJsonValue<JsonDoubleNode>(key, out var p2)) return (int)Math.Round(p2.Value);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }

        var str = TryGetStringValue(key);
        if (string.IsNullOrWhiteSpace(str) || !Numbers.TryParseToInt32(str, 10, out var p3)) return null;
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
    /// <param name="converter">The converter.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public int? TryGetInt32Value(string key, IJsonPropertyResolver<int> converter)
        => TryGetInt32Value(key, converter, out var result) ? result : null;

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetInt32Value(string key, IJsonPropertyResolver<int> converter, out int result)
    {
        var v = TryGetInt32Value(key);
        if (v.HasValue)
        {
            result = v.Value;
            return true;
        }

        if (store.TryGetValue(key, out var data) && data is JsonObjectNode json && converter is not null)
        {
            return converter.TryGetValue(json, out result);
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <returns>A string.</returns>
    public int? TryGetInt32Value(IEnumerable<string> keyPath)
    {
        var value = TryGetValue(keyPath);
        if (value is null) return null;
        return value.TryGetInt32(out var s) ? s : null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetInt32Value(IEnumerable<string> keyPath, out int result)
    {
        var value = TryGetValue(keyPath);
        if (value is null)
        {
            result = default;
            return false;
        }

        return value.TryGetInt32(out result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public long? TryGetInt64Value(string key)
    {
        if (TryGetJsonValue<JsonIntegerNode>(key, out var p1)) return p1.Value;
        try
        {
            if (TryGetJsonValue<JsonDoubleNode>(key, out var p2)) return (long)Math.Round(p2.Value);
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }

        var str = TryGetStringValue(key);
        if (string.IsNullOrWhiteSpace(str) || !Numbers.TryParseToInt64(str, 10, out var p3)) return null;
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
    /// <param name="keyPath">The path of property key.</param>
    /// <returns>A string.</returns>
    public long? TryGetInt64Value(IEnumerable<string> keyPath)
    {
        var value = TryGetValue(keyPath);
        if (value is null) return null;
        return value.TryGetInt64(out var s) ? s : null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetInt64Value(IEnumerable<string> keyPath, out long result)
    {
        var value = TryGetValue(keyPath);
        if (value is null)
        {
            result = default;
            return false;
        }

        return value.TryGetInt64(out result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value; or NaN if fail to resolve.</returns>
    public float TryGetSingleValue(string key)
    {
        try
        {
            if (TryGetJsonValue<JsonDoubleNode>(key, out var p1)) return (float)p1;
        }
        catch (InvalidCastException)
        {
            return float.NaN;
        }
        catch (ArgumentException)
        {
            return float.NaN;
        }

        if (TryGetJsonValue<JsonIntegerNode>(key, out var p2)) return (float)p2;
        var str = TryGetStringValue(key);
        if (string.IsNullOrWhiteSpace(str) || !float.TryParse(str, out var p3)) return float.NaN;
        return p3;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public float? TryGetNullableSingleValue(string key)
    {
        var v = TryGetSingleValue(key);
        return float.IsNaN(v) ? null : v;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value; or the default value if fail to resolve.</returns>
    public float TryGetSingleValue(string key, float defaultValue)
    {
        var v = TryGetSingleValue(key);
        if (!float.IsNaN(v)) return v;
        return defaultValue;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetSingleValue(string key, out float result)
    {
        result = TryGetSingleValue(key);
        return float.IsNaN(result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetSingleValue(string key, IJsonPropertyResolver<float> converter, out float result)
    {
        var v = TryGetSingleValue(key);
        if (!float.IsNaN(v))
        {
            result = v;
            return true;
        }

        if (store.TryGetValue(key, out var data) && data is JsonObjectNode json && converter is not null)
        {
            return converter.TryGetValue(json, out result);
        }

        result = float.NaN;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>The value; or NaN if fail to resolve.</returns>
    public float TryGetSingleValue(string key, IJsonPropertyResolver<float> converter)
        => TryGetSingleValue(key, converter, out var result) ? result : float.NaN;

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public float? TryGetNullableSingleValue(string key, IJsonPropertyResolver<float> converter)
        => TryGetSingleValue(key, converter, out var result) ? result : null;

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <returns>The value; or NaN if fail to resolve.</returns>
    public float TryGetSingleValue(IEnumerable<string> keyPath)
    {
        var value = TryGetValue(keyPath);
        if (value is null) return float.NaN;
        return value.TryGetSingle(out var s) ? s : float.NaN;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value; or the default value if fail to resolve.</returns>
    public float TryGetSingleValue(IEnumerable<string> keyPath, float defaultValue)
    {
        var value = TryGetValue(keyPath);
        if (value is null) return defaultValue;
        return value.TryGetSingle(out var s) ? s : defaultValue;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetSingleValue(IEnumerable<string> keyPath, out float result)
    {
        var value = TryGetValue(keyPath);
        if (value is null)
        {
            result = float.NaN;
            return false;
        }

        return value.TryGetSingle(out result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public float? TryGetNullableSingleValue(IEnumerable<string> keyPath)
    {
        var v = TryGetSingleValue(keyPath);
        return float.IsNaN(v) ? null : v;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value; or NaN if fail to resolve.</returns>
    public double TryGetDoubleValue(string key)
    {
        if (TryGetJsonValue<JsonDoubleNode>(key, out var p1)) return p1.Value;
        if (TryGetJsonValue<JsonIntegerNode>(key, out var p2)) return (double)p2;
        var str = TryGetStringValue(key);
        if (string.IsNullOrWhiteSpace(str) || !double.TryParse(str, out var p3)) return double.NaN;
        return p3;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public double? TryGetNullableDoubleValue(string key)
    {
        var v = TryGetDoubleValue(key);
        return double.IsNaN(v) ? null : v;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value; or the default value if fail to resolve.</returns>
    public double TryGetDoubleValue(string key, double defaultValue)
    {
        var v = TryGetDoubleValue(key);
        if (!double.IsNaN(v)) return v;
        return defaultValue;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetDoubleValue(string key, out double result)
    {
        result = TryGetDoubleValue(key);
        return !double.IsNaN(result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetDoubleValue(string key, IJsonPropertyResolver<double> converter, out double result)
    {
        var v = TryGetDoubleValue(key);
        if (!double.IsNaN(v))
        {
            result = v;
            return true;
        }

        if (store.TryGetValue(key, out var data) && data is JsonObjectNode json && converter is not null)
        {
            return converter.TryGetValue(json, out result);
        }

        result = double.NaN;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>The value; or NaN if fail to resolve.</returns>
    public double TryGetDoubleValue(string key, IJsonPropertyResolver<double> converter)
        => TryGetDoubleValue(key, converter, out var result) ? result : double.NaN;

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public double? TryGetNullableDoubleValue(string key, IJsonPropertyResolver<double> converter)
        => TryGetDoubleValue(key, converter, out var result) ? result : null;

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <returns>The value; or NaN if fail to resolve.</returns>
    public double TryGetDoubleValue(IEnumerable<string> keyPath)
    {
        var value = TryGetValue(keyPath);
        if (value is null) return double.NaN;
        return value.TryGetDouble(out var s) ? s : double.NaN;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value; or the default value if fail to resolve.</returns>
    public double TryGetDoubleValue(IEnumerable<string> keyPath, double defaultValue)
    {
        var value = TryGetValue(keyPath);
        if (value is null) return defaultValue;
        return value.TryGetDouble(out var s) ? s : defaultValue;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public double? TryGetNullableDoubleValue(IEnumerable<string> keyPath)
    {
        var v = TryGetDoubleValue(keyPath);
        return double.IsNaN(v) ? null : v;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetDoubleValue(IEnumerable<string> keyPath, out double result)
    {
        var value = TryGetValue(keyPath);
        if (value is null)
        {
            result = double.NaN;
            return false;
        }

        return value.TryGetDouble(out result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public decimal? TryGetDecimalValue(string key)
    {
        try
        {
            if (TryGetJsonValue<JsonDoubleNode>(key, out var p1)) return (decimal)p1.Value;
        }
        catch (InvalidCastException)
        {
            return null;
        }
        catch (OverflowException)
        {
            return null;
        }

        if (TryGetJsonValue<JsonIntegerNode>(key, out var p2)) return (decimal)p2;
        var str = TryGetStringValue(key);
        if (string.IsNullOrWhiteSpace(str) || !decimal.TryParse(str, out var p3)) return null;
        return p3;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetDecimalValue(string key, out decimal result)
    {
        var v = TryGetDecimalValue(key);
        result = v ?? default;
        return v.HasValue;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <returns>A string.</returns>
    public decimal? TryGetDecimalValue(IEnumerable<string> keyPath)
    {
        var value = TryGetValue(keyPath);
        if (value is null) return null;
        return value.TryGetDecimal(out var s) ? s : null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetDecimalValue(IEnumerable<string> keyPath, out decimal result)
    {
        var value = TryGetValue(keyPath);
        if (value is null)
        {
            result = default;
            return false;
        }

        return value.TryGetDecimal(out result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public bool? TryGetBooleanValue(string key)
    {
        if (TryGetJsonValue<JsonBooleanNode>(key, out var p)) return p.Value;
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
    /// <param name="exactly">true if only boolean value kind; otherwise, false.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetBooleanValue(string key, bool exactly, out bool result)
    {
        if (TryGetJsonValue<JsonBooleanNode>(key, out var p))
        {
            result = p.Value;
            return true;
        }
        else if (exactly)
        {
            result = default;
            return false;
        }

        var str = TryGetStringValue(key);
        if (string.IsNullOrWhiteSpace(str))
        {
            result = default;
            return false;
        }

        var b = JsonBooleanNode.TryParse(str);
        if (b == null)
        {
            result = default;
            return false;
        }

        result = b.Value;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetBooleanValue(string key, IJsonPropertyResolver<bool> converter, out bool result)
    {
        var v = TryGetBooleanValue(key);
        if (v.HasValue)
        {
            result = v.Value;
            return true;
        }

        if (store.TryGetValue(key, out var data) && data is JsonObjectNode json && converter is not null)
        {
            return converter.TryGetValue(json, out result);
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>The value; or null if fail to resolve.</returns>
    public bool? TryGetBooleanValue(string key, IJsonPropertyResolver<bool> converter)
        => TryGetBooleanValue(key, converter, out var result) ? result : null;

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <returns>A string.</returns>
    public bool? TryGetBooleanValue(IEnumerable<string> keyPath)
    {
        var value = TryGetValue(keyPath);
        if (value is null) return null;
        return value.TryGetBoolean(out var s) ? s : null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetBooleanValue(IEnumerable<string> keyPath, out bool result)
    {
        var value = TryGetValue(keyPath);
        if (value is null)
        {
            result = default;
            return false;
        }

        return value.TryGetBoolean(out result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    public JsonObjectNode TryGetObjectValue(string key)
    {
        if (string.IsNullOrEmpty(key)) return this;
        if (!TryGetJsonValue<JsonObjectNode>(key, out var p)) return null;
        if (p.Count == 1 && p.TryGetStringValue("$ref") == JsonValues.SELF_REF) return this;
        return p;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="ignoreNotMatched">true if ignore any item which is not JSON object; otherwise, false.</param>
    /// <returns>The list; or null, if no such array property.</returns>
    public List<JsonObjectNode> TryGetObjectListValue(string key, bool ignoreNotMatched = false)
    {
        if (!TryGetJsonValue<JsonArrayNode>(key, out var p)) return null;
        return ignoreNotMatched ? p.OfType<JsonObjectNode>().ToList() : p.Select(ele => ele is JsonObjectNode json ? json : null).ToList();
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="subKey">The sub-key of the previous property.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    public JsonObjectNode TryGetObjectValue(string key, string subKey, params string[] keyPath)
    {
        var json = TryGetObjectValueByProperty(this, key);
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
    public JsonObjectNode TryGetObjectValue(IEnumerable<string> keyPath)
    {
        var json = this;
        if (keyPath == null) return json;
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
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetObjectValue(string key, out JsonObjectNode result)
    {
        var v = TryGetObjectValue(key);
        result = v;
        return v is not null || IsValueKind(key, JsonValueKind.Null);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The property key path.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetObjectValue(IEnumerable<string> keyPath, out JsonObjectNode result)
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
    /// <param name="root">The root node.</param>
    /// <returns>The value.</returns>
    public JsonObjectNode TryGetRefObjectValue(string key, JsonObjectNode root)
        => TryGetRefObjectValue(this, TryGetObjectValue(key), root);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="root">The root node.</param>
    /// <returns>The value.</returns>
    public JsonObjectNode TryGetRefObjectValue(string key, JsonDataResult root)
        => root == null ? null : TryGetRefObjectValue(key, root.ToJson());

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    public JsonArrayNode TryGetArrayValue(string key)
    {
        if (TryGetJsonValue<JsonArrayNode>(key, out var p)) return p;
        return null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetArrayValue(string key, out JsonArrayNode result)
    {
        var v = TryGetArrayValue(key);
        result = v;
        return v is not null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <returns>A string.</returns>
    public JsonArrayNode TryGetArrayValue(IEnumerable<string> keyPath)
        => TryGetValue(keyPath) as JsonArrayNode;

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The path of property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetArrayValue(IEnumerable<string> keyPath, out JsonArrayNode result)
    {
        var value = TryGetValue(keyPath);
        result = value as JsonArrayNode;
        return result is not null;
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
        if (TryGetJsonValue<JsonStringNode>(key, out var s))
        {
            var date = WebFormat.ParseDate(s.Value);
            return date;
        }

        if (TryGetJsonValue<JsonIntegerNode>(key, out var num))
            return useUnixTimestampsFallback ? WebFormat.ParseUnixTimestamp(num.Value) : WebFormat.ParseDate(num.Value);
        if (!TryGetJsonValue<JsonObjectNode>(key, out var obj)) return null;
        return JsonValues.TryGetDateTime(obj);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="parser">The parser.</param>
    /// <returns>The value.</returns>
    public DateTime? TryGetDateTimeValue(string key, Func<string, DateTime?> parser)
    {
        AssertKey(key);
        if (TryGetJsonValue<JsonStringNode>(key, out var s))
        {
            if (parser is not null) return parser(s.Value);
            var date = WebFormat.ParseDate(s.Value);
            return date;
        }

        if (TryGetJsonValue<JsonIntegerNode>(key, out var num))
        {
            if (parser is not null) return parser(num.ToString());
            return WebFormat.ParseDate(num.Value);
        }

        return null;
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="bytes">The result.</param>
    /// <param name="bytesWritten">The count of bytes written.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetBytesFromBase64(string key, Span<byte> bytes, out int bytesWritten)
    {
        var str = TryGetStringValue(key);
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
    /// <typeparam name="T">An enumeration type.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    public T? TryGetEnumValue<T>(string key) where T : struct, Enum
    {
        try
        {
            if (TryGetInt32Value(key, out var v)) return (T)(object)v;
            var str = TryGetStringValue(key);
            if (Enum.TryParse<T>(str, out var result)) return result;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidCastException)
        {
        }

        return null;
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <typeparam name="T">An enumeration type.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
    /// <returns>The value.</returns>
    public T? TryGetEnumValue<T>(string key, bool ignoreCase) where T : struct, Enum
    {
        try
        {
            if (TryGetInt32Value(key, out var v)) return (T)(object)v;
            var str = TryGetStringValue(key);
            if (Enum.TryParse<T>(str, ignoreCase, out var result)) return result;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidCastException)
        {
        }

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
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="type">An enumeration type.</param>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result output.</param>
    /// <returns>true if parse succeeded; otherwise, false.</returns>
    public bool TryGetEnumValue(Type type, string key, out object result)
    {
        try
        {
            if (TryGetInt32Value(key, out var v))
            {
                result = type is null ? v : Enum.ToObject(type, v);
                return true;
            }

            var str = GetStringValue(key);
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
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="type">An enumeration type.</param>
    /// <param name="key">The property key.</param>
    /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
    /// <param name="result">The result output.</param>
    /// <returns>true if parse succeeded; otherwise, false.</returns>
    public bool TryGetEnumValue(Type type, string key, bool ignoreCase, out object result)
    {
        try
        {
            if (TryGetInt32Value(key, out var v))
            {
                result = type is null ? v : Enum.ToObject(type, v);
                return true;
            }

            var str = GetStringValue(key);
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
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    public IJsonDataNode TryGetValue(string key)
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
    public IJsonDataNode TryGetValue(ReadOnlySpan<char> key)
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
    public bool TryGetValue(string key, out IJsonDataNode result)
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
    public bool TryGetValue(ReadOnlySpan<char> key, out IJsonDataNode result)
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
    public bool TryGetValue(IEnumerable<string> keyPath, out IJsonDataNode result)
    {
        if (keyPath == null)
        {
            result = this;
            return true;
        }

        IJsonDataNode json = this;
        foreach (var key in keyPath)
        {
            if (string.IsNullOrEmpty(key)) continue;
            if (json is null)
            {
                result = null;
                return false;
            }

            if (json is JsonObjectNode jObj)
            {
                if (!jObj.ContainsKey(key))
                {
                    result = null;
                    return false;
                }

                var jObjToken = jObj.TryGetValue(key);
                if (jObjToken is JsonObjectNode jObj2 && jObj2.Count == 1 && jObj2.TryGetStringValue("$ref") == JsonValues.SELF_REF) continue;
                json = jObjToken;
                continue;
            }

            if (json is JsonArrayNode jArr)
            {
                var jValue = jArr.TryGetValue(key);
                if (jValue is null)
                {
                    result = null;
                    return false;
                }

                json = jValue;
                continue;
            }

            if (json is JsonStringNode)
            {
                if (json.TryGetValue(key, out var jValue))
                {
                    json = jValue;
                    continue;
                }
            }

            result = null;
            return false;
        }

        result = json;
        return true;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The property key path.</param>
    /// <returns>The value.</returns>
    public IJsonDataNode TryGetValue(IEnumerable<string> keyPath)
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
    bool IDictionary<string, IJsonValueNode>.TryGetValue(string key, out IJsonValueNode result)
    {
        var v = TryGetValue(key);
        result = v;
        return v is not null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    bool IReadOnlyDictionary<string, IJsonValueNode>.TryGetValue(string key, out IJsonValueNode result)
    {
        var v = TryGetValue(key);
        result = v;
        return v is not null;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <typeparam name="T">The type of property value.</typeparam>
    /// <param name="resolver">The property resolver.</param>
    /// <param name="result">The result of the property.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetValue<T>(IJsonPropertyResolver<T> resolver, out T result)
    {
        if (resolver is null)
        {
            result = default;
            return false;
        }

        return resolver.TryGetValue(this, out result);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="subKey">The sub-key of the previous property.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    public IJsonDataNode TryGetValue(string key, string subKey, params string[] keyPath)
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
    /// <typeparam name="T">The type of value. Should be one of IJsonDataNode (or its sub-class), String, Int32, Int64, Boolean, Single, Double, Decimal or StringBuilder.</typeparam>
    /// <param name="keyPath">The property key path.</param>
    /// <returns>The value.</returns>
    public T TryGetValue<T>(params string[] keyPath)
        => TryGetValue<T>(keyPath, out var r) ? r : default;

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <typeparam name="T">The type of value. Should be one of IJsonDataNode (or its sub-class), String, Int32, Int64, Boolean, Single, Double, Decimal or StringBuilder.</typeparam>
    /// <param name="keyPath">The additional property key path.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetValue<T>(IEnumerable<string> keyPath, out T result)
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

        var type = typeof(T);
        try
        {
            if (type == typeof(string))
            {
                if (r.TryGetString(out var s))
                {
                    result = (T)(object)s;
                    return true;
                }
            }
            else if (type.IsEnum)
            {
                if (r is JsonIntegerNode i)
                {
                    result = (T)Enum.ToObject(type, i.Value);
                    return true;
                }
                else if (r is JsonStringNode s)
                {
                    result = (T)s.TryToEnum(type);
                    return true;
                }
            }
            else if (type.IsValueType)
            {
                if (r.ValueKind == JsonValueKind.Null || r.ValueKind == JsonValueKind.Undefined)
                {
                }
                else if (type == typeof(int))
                {
                    if (r.TryGetInt32(out var i))
                    {
                        result = (T)(object)i;
                        return true;
                    }
                }
                else if (type == typeof(long))
                {
                    if (r.TryGetInt64(out var l))
                    {
                        result = (T)(object)l;
                        return true;
                    }
                }
                else if (type == typeof(double))
                {
                    if (r.TryGetDouble(out var d))
                    {
                        result = (T)(object)d;
                        return true;
                    }
                }
                else if (type == typeof(float))
                {
                    if (r.TryGetSingle(out var f))
                    {
                        result = (T)(object)f;
                        return true;
                    }
                }
                else if (type == typeof(decimal))
                {
                    if (r.TryGetDecimal(out var d))
                    {
                        result = (T)(object)d;
                        return true;
                    }
                }
                else if (type == typeof(bool))
                {
                    if (r.TryGetBoolean(out var b))
                    {
                        result = (T)(object)b;
                        return true;
                    }
                }
                else if (type == typeof(DateTime))
                {
                    if (r.TryGetDateTime(out var d))
                    {
                        result = (T)(object)d;
                        return true;
                    }
                }
                else if (type == typeof(Guid))
                {
                    if (r.TryGetGuid(out var g))
                    {
                        result = (T)(object)g;
                        return true;
                    }
                }
                else if (type == typeof(uint))
                {
                    if (r.TryGetUInt32(out var i))
                    {
                        result = (T)(object)i;
                        return true;
                    }
                }
                else if (type == typeof(Uri))
                {
                    if (r.TryGetString(out var s) && Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out var u))
                    {
                        result = (T)(object)u;
                        return true;
                    }
                }
            }
            else if (type == typeof(StringBuilder))
            {
                if (r.TryGetString(out var s))
                {
                    result = (T)(object)new StringBuilder(s);
                    return true;
                }
            }
            else if (type.IsClass)
            {
                if (r is JsonObjectNode json)
                {
                    result = json.Deserialize<T>();
                    return true;
                }
                else if (r is JsonArrayNode arr)
                {
                    result = arr.Deserialize<T>();
                    return true;
                }
                else if (r is JsonStringNode s)
                {
                    json = TryParse(s.Value);
                    if (json != null)
                    {
                        result = json.Deserialize<T>();
                        return true;
                    }
                }
            }
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidCastException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (OverflowException)
        {
        }
        catch (FormatException)
        {
        }
        catch (JsonException)
        {
        }
        catch (NotSupportedException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key or key path.</param>
    /// <param name="isPath">true if the key is a path; otherwise, false.</param>
    /// <returns>The value.</returns>
    public IJsonDataNode TryGetValue(string key, bool isPath)
    {
        if (!isPath) return TryGetValue(key);
        if (key == null) return this;
        if (key.StartsWith("$.")) key = key.Substring(2);
        else if (key.StartsWith("$[")) key = key.Substring(1);
        var traditional = key.StartsWith("[") && key.EndsWith("]");
        var arr = traditional ? key.Substring(1, key.Length - 2).Split(new [] { "][" }, StringSplitOptions.None) : key.Split('.');
        var path = new List<string>();
        string quote = null;
        foreach (var ele in arr)
        {
            var prop = StringExtensions.ReplaceBackSlash(ele ?? string.Empty);
            if (quote != null)
            {
                var propTrim3 = prop.TrimEnd();
                if (propTrim3.EndsWith(quote)) quote = null;
                var appendStr = (traditional ? "][" : ".") + (quote != null ? prop : propTrim3.Substring(0, propTrim3.Length - 1));
                path[path.Count - 1] += appendStr;
                continue;
            }

            if (string.IsNullOrEmpty(prop)) continue;
            string quoteStart = null;
            var propTrim = prop.TrimStart();
            if (propTrim.StartsWith("\""))
            {
                quoteStart = "\"";
            }
            else if (propTrim.StartsWith("\'"))
            {
                quoteStart = "\'";
            }

            if (quoteStart == null)
            {
                path.Add(prop);
                continue;
            }

            propTrim = propTrim.Substring(1);
            var propTrim2 = propTrim.TrimEnd();
            if (propTrim2.EndsWith(quoteStart))
            {
                propTrim = propTrim2.Substring(0, propTrim2.Length - 1);
            }
            else
            {
                quote = quoteStart;
            }

            path.Add(propTrim);
        }

        return TryGetValue(path);
    }

    /// <summary>
    /// Deserializes a property value.
    /// </summary>
    /// <typeparam name="T">The type of model to deserialize.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if deserialize succeeded; otherwise, false.</returns>
    /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="JsonException">The JSON is invalid. -or- TValue is not compatible with the JSON.</exception>
    public bool TryDeserializeValue<T>(string key, JsonSerializerOptions options, out T result)
    {
        if (!store.TryGetValue(key, out var item))
        {
            result = default;
            return false;
        }

        try
        {
            if (item is null || item.ValueKind == JsonValueKind.Undefined)
            {
                result = default;
                return false;
            }

            if (item.ValueKind == JsonValueKind.Null)
            {
                result = default;
                return typeof(T).IsClass;
            }

            result = JsonSerializer.Deserialize<T>(item.ToString(), options);
            return true;
        }
        catch (NotSupportedException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (FormatException)
        {
        }
        catch (JsonException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (InvalidOperationException)
        {
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
        return RemoveProperty(key);
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
        return RemoveProperty(key.ToString());
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
            if (RemoveProperty(key)) count++;
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
        SetProperty(key, JsonValues.Null);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="_">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, DBNull _)
    {
        AssertKey(key);
        SetProperty(key, JsonValues.Null);
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
        if (value == null) SetProperty(key, JsonValues.Null);
        else SetProperty(key, new JsonStringNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property; or removes the property if the value is null.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueOrRemove(string key, string value)
    {
        AssertKey(key);
        if (value == null) RemoveProperty(key);
        else SetProperty(key, new JsonStringNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, string value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, StringBuilder value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, StringBuilder value)
    {
        if (value == null || value.Length < 1) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, char[] value)
    {
        AssertKey(key);
        if (value == null) SetProperty(key, JsonValues.Null);
        else SetProperty(key, new JsonStringNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, StringBuilder value)
        => SetValue(key, value?.ToString());

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="InvalidOperationException">The secure string is disposed.</exception>
    public void SetValue(string key, SecureString value)
    {
        AssertKey(key);
        if (value == null) SetProperty(key, JsonValues.Null);
        else SetProperty(key, new JsonStringNode(value));
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
        SetProperty(key, new JsonStringNode(string.Format(value, args)));
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
        SetProperty(key, new JsonStringNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, Uri value)
    {
        AssertKey(key);
        string url = null;
        try
        {
            url = value?.OriginalString;
        }
        catch (InvalidOperationException)
        {
        }

        if (string.IsNullOrEmpty(url)) SetProperty(key, JsonValues.Null);
        else SetProperty(key, new JsonStringNode(url));
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
        if (store.TryGetValue(key, out var data) && data is not null)
        {
            switch (data.ValueKind)
            {
                case JsonValueKind.Number:
                    SetProperty(key, new JsonIntegerNode(value));
                    return;
                case JsonValueKind.Object:
                    if (data is JsonObjectNode json && (json.ContainsKey("day") || json.ContainsKey("hour")))
                    {
                        var utc = value.ToUniversalTime();
                        SetProperty(key, new JsonObjectNode
                        {
                            { "value", WebFormat.ParseDate(utc) },
                            { "str", JsonStringNode.ToJson(value, true) },
                            { "year", utc.Year },
                            { "month", utc.Month },
                            { "day", utc.Day },
                            { "hour", utc.Hour },
                            { "minute", utc.Minute },
                            { "second", utc.Second },
                            { "millisecond", utc.Millisecond },
                            { "kind", "utc" }
                        });
                        return;
                    }

                    break;
            }
        }

        SetProperty(key, new JsonStringNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, Uri value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="format">A standard or custom date and time format string; or null, if force to use JavaScript date time string format..</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, DateTime value, string format)
    {
        AssertKey(key);
        SetProperty(key, new JsonStringNode(value, format));
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
        SetProperty(key, new JsonIntegerNode(value));
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
        SetProperty(key, new JsonIntegerNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, int value, string format, IFormatProvider provider = null)
    {
        AssertKey(key);
        SetProperty(key, new JsonStringNode(value, format, provider));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, int? value)
    {
        if (value.HasValue) SetValue(key, value.Value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="setter">The handler of value setter.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, Func<int, bool, int> setter)
    {
        AssertKey(key);
        if (setter is null) return;
        var i = TryGetInt32Value(key);
        var value = setter(i ?? 0, i.HasValue);
        SetProperty(key, new JsonIntegerNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="setter">The handler of value setter.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, Func<int?, int?> setter)
    {
        AssertKey(key);
        if (setter is null) return;
        var value = TryGetInt32Value(key);
        value = setter(value);
        if (value.HasValue) SetProperty(key, new JsonIntegerNode(value.Value));
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
        SetProperty(key, new JsonIntegerNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, long value, string format, IFormatProvider provider = null)
    {
        AssertKey(key);
        SetProperty(key, new JsonStringNode(value, format, provider));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, long? value)
    {
        if (value.HasValue) SetValue(key, value.Value);
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
        SetProperty(key, float.IsNaN(value) ? JsonValues.Null : new JsonDoubleNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, float? value)
    {
        if (value.HasValue && !float.IsNaN(value.Value)) SetValue(key, value.Value);
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
        SetProperty(key, double.IsNaN(value) ? JsonValues.Null : new JsonDoubleNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, double? value)
    {
        if (value.HasValue && !double.IsNaN(value.Value)) SetValue(key, value.Value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, decimal value)
    {
        AssertKey(key);
        SetProperty(key, new JsonDoubleNode(value));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, float value, string format, IFormatProvider provider = null)
    {
        AssertKey(key);
        SetProperty(key, new JsonStringNode(value, format, provider));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, double value, string format, IFormatProvider provider = null)
    {
        AssertKey(key);
        SetProperty(key, new JsonStringNode(value, format, provider));
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, decimal value, string format, IFormatProvider provider = null)
    {
        AssertKey(key);
        SetProperty(key, new JsonStringNode(value, format, provider));
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
        SetProperty(key, value ? JsonBooleanNode.True : JsonBooleanNode.False);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, bool? value)
    {
        if (value.HasValue) SetValue(key, value.Value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, JsonArrayNode value)
    {
        AssertKey(key);
        SetProperty(key, value ?? JsonValues.Null);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, JsonArrayNode value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, JsonArrayNode value)
    {
        if (value == null || value.Count < 1) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The JSON object node created.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    public void SetValue(string key, out JsonArrayNode value)
    {
        value = new();
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property; or removes the property if the value is null.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueOrRemove(string key, JsonArrayNode value)
    {
        AssertKey(key);
        if (value == null) RemoveProperty(key);
        else SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, JsonObjectNode value)
    {
        AssertKey(key);
        SetProperty(key, (ReferenceEquals(value, this) ? new()
        {
            { "$ref", JsonValues.SELF_REF}
        }: value) ?? JsonValues.Null);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The JSON object node created.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    public void SetValue(string key, out JsonObjectNode value)
    {
        value = new();
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="subKey">The sub property key.</param>
    /// <param name="value">The JSON object node created.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    public void SetValue(string key, string subKey, out JsonObjectNode value)
    {
        value = new();
        var obj = TryGetObjectValue(key);
        if (obj == null) SetValue(key, new JsonObjectNode
        {
            { subKey, value }
        });
        else obj.SetValue(subKey, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, JsonObjectNode value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property; or removes the property if the value is null.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueOrRemove(string key, JsonObjectNode value)
    {
        AssertKey(key);
        if (value == null) RemoveProperty(key);
        else SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, IJsonObjectHost value)
    {
        AssertKey(key);
        SetProperty(key, value?.ToJson() ?? JsonValues.Null);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, IJsonObjectHost value)
    {
        if (value is null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property; or removes the property if the value is null.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueOrRemove(string key, IJsonObjectHost value)
    {
        AssertKey(key);
        if (value is null) RemoveProperty(key);
        else SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, IEnumerable<IJsonObjectHost> value)
    {
        AssertKey(key);
        var arr = new JsonArrayNode();
        foreach (var item in value)
        {
            arr.Add(item?.ToJson());
        }

        SetProperty(key, arr);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, IEnumerable<IJsonObjectHost> value)
    {
        if (value is null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, ICollection<IJsonObjectHost> value)
    {
        if (value is null || value.Count < 1) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="converter">The converter.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters. The converter should not be null.</exception>
    public void SetValue<T>(string key, IEnumerable<T> value, Func<T, JsonObjectNode> converter)
    {
        AssertKey(key);
        if (converter == null) throw new ArgumentNullException(nameof(converter), "converter was null.");
        var arr = new JsonArrayNode();
        foreach (var item in value)
        {
            arr.Add(converter(item));
        }

        SetProperty(key, arr);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="converter">The converter.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters. The converter should not be null.</exception>
    public void SetValueIfNotNull<T>(string key, IEnumerable<T> value, Func<T, JsonObjectNode> converter)
    {
        if (value is null) return;
        SetValue(key, value, converter);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="converter">The converter.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters. The converter should not be null.</exception>
    public void SetValueIfNotEmpty<T>(string key, ICollection<T> value, Func<T, JsonObjectNode> converter)
    {
        if (value is null || value.Count < 1) return;
        SetValue(key, value, converter);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, JsonDocument value)
    {
        AssertKey(key);
        SetProperty(key, JsonValues.ToJsonValue(value) ?? JsonValues.Null);
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
        SetProperty(key, JsonValues.ToJsonValue(value) ?? JsonValues.Null);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, System.Text.Json.Nodes.JsonNode value)
    {
        AssertKey(key);
        SetProperty(key, JsonValues.ToJsonValue(value) ?? JsonValues.Null);
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
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        SetProperty(key, arr);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, IEnumerable<string> value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, ICollection<string> value)
    {
        if (value == null || value.Count < 1) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, IEnumerable<bool> value)
    {
        AssertKey(key);
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        SetProperty(key, arr);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, IEnumerable<bool> value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, ICollection<bool> value)
    {
        if (value == null || value.Count < 1) return;
        SetValue(key, value);
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
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        SetProperty(key, arr);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, IEnumerable<int> value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, ICollection<int> value)
    {
        if (value == null || value.Count < 1) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, IEnumerable<long> value)
    {
        AssertKey(key);
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        SetProperty(key, arr);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, IEnumerable<long> value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, ICollection<long> value)
    {
        if (value == null || value.Count < 1) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, IEnumerable<float> value)
    {
        AssertKey(key);
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        SetProperty(key, arr);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, IEnumerable<float> value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, ICollection<float> value)
    {
        if (value == null || value.Count < 1) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, IEnumerable<double> value)
    {
        AssertKey(key);
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        SetProperty(key, arr);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, IEnumerable<double> value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, ICollection<double> value)
    {
        if (value == null || value.Count < 1) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValue(string key, IEnumerable<JsonObjectNode> value)
    {
        AssertKey(key);
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        SetProperty(key, arr);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotNull(string key, IEnumerable<JsonObjectNode> value)
    {
        if (value == null) return;
        SetValue(key, value);
    }

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void SetValueIfNotEmpty(string key, ICollection<JsonObjectNode> value)
    {
        if (value == null || value.Count < 1) return;
        SetValue(key, value);
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
#if NETFRAMEWORK
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
        SetProperty(key, new JsonStringNode(value));
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
        SetProperty(key, new JsonIntegerNode(WebFormat.ParseDate(value)));
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
        SetProperty(key, new JsonIntegerNode(WebFormat.ParseUnixTimestamp(value)));
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
        SetProperty(key, new JsonIntegerNode(value.ToFileTimeUtc()));
    }

    /// <summary>
    /// Sets properties.
    /// </summary>
    /// <param name="data">Key value pairs to set.</param>
    /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
    /// <returns>The count to set.</returns>
    public int SetRange(IEnumerable<KeyValuePair<string, string>> data, bool skipDuplicate = false)
    {
        var count = 0;
        if (data == null) return count;
        if (skipDuplicate)
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key) || store.ContainsKey(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }
        else
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }

        return count;
    }

    /// <summary>
    /// Sets properties.
    /// </summary>
    /// <param name="data">Key value pairs to set.</param>
    /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
    /// <returns>The count to set.</returns>
    public int SetRange(IEnumerable<KeyValuePair<string, int>> data, bool skipDuplicate = false)
    {
        var count = 0;
        if (data == null) return count;
        if (skipDuplicate)
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key) || store.ContainsKey(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }
        else
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }

        return count;
    }

    /// <summary>
    /// Sets properties.
    /// </summary>
    /// <param name="data">Key value pairs to set.</param>
    /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
    /// <returns>The count to set.</returns>
    public int SetRange(IEnumerable<KeyValuePair<string, JsonObjectNode>> data, bool skipDuplicate = false)
    {
        var count = 0;
        if (data == null) return count;
        if (skipDuplicate)
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key) || store.ContainsKey(props.Key)) continue;
                count++;
                SetValue(props.Key, ReferenceEquals(this, props.Value) ? props.Value.Clone() : props.Value);
            }
        }
        else
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, ReferenceEquals(this, props.Value) ? props.Value.Clone() : props.Value);
            }
        }

        return count;
    }

    /// <summary>
    /// Sets properties.
    /// </summary>
    /// <param name="data">Key value pairs to set.</param>
    /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
    /// <returns>The count to set.</returns>
    public int SetRange(IEnumerable<KeyValuePair<string, JsonArrayNode>> data, bool skipDuplicate = false)
    {
        var count = 0;
        if (data == null) return count;
        if (skipDuplicate)
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key) || store.ContainsKey(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }
        else
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }

        return count;
    }

    /// <summary>
    /// Sets properties.
    /// </summary>
    /// <param name="data">Key value pairs to set.</param>
    /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
    /// <returns>The count to set.</returns>
    public int SetRange(IEnumerable<KeyValuePair<string, JsonElement>> data, bool skipDuplicate = false)
    {
        var count = 0;
        if (data == null) return count;
        if (skipDuplicate)
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key) || store.ContainsKey(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }
        else
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }

        return count;
    }

    /// <summary>
    /// Sets properties.
    /// </summary>
    /// <param name="data">Key value pairs to set.</param>
    /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
    /// <returns>The count to set.</returns>
    public int SetRange(IEnumerable<KeyValuePair<string, System.Text.Json.Nodes.JsonNode>> data, bool skipDuplicate = false)
    {
        var count = 0;
        if (data == null) return count;
        if (skipDuplicate)
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key) || store.ContainsKey(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }
        else
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }

        return count;
    }

    /// <summary>
    /// Sets properties.
    /// </summary>
    /// <param name="data">Key value pairs to set.</param>
    /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
    /// <returns>The count to set.</returns>
    public int SetRange(IEnumerable<KeyValuePair<string, IJsonObjectHost>> data, bool skipDuplicate = false)
    {
        var count = 0;
        if (data == null) return count;
        if (skipDuplicate)
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key) || store.ContainsKey(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }
        else
        {
            foreach (var props in data)
            {
                if (string.IsNullOrWhiteSpace(props.Key)) continue;
                count++;
                SetValue(props.Key, props.Value);
            }
        }

        return count;
    }

    /// <summary>
    /// Sets properties.
    /// </summary>
    /// <param name="json">Another JSON object to add.</param>
    /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
    /// <returns>The count of property added.</returns>
    public int SetRange(JsonObjectNode json, bool skipDuplicate = false)
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
    public int SetRange(JsonArrayNode array, IEnumerable<string> propertyMapping = null, bool skipDuplicate = false)
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
                SetProperty(key, array[i]);
            }

            return count2;
        }

        for (var i = 0; i < count; i++)
        {
            var key = keys[i];
            if (string.IsNullOrEmpty(key)) continue;
            SetProperty(key, array[i]);
        }

        return count;
    }

    /// <summary>
    /// Sets properties.
    /// </summary>
    /// <param name="col">A collection to add.</param>
    /// <param name="propertyMapping">The mapping of index to property key; or null for convert index to string format.</param>
    /// <param name="skipDuplicate">true if skip the duplicate properties; otherwise, false.</param>
    /// <returns>The count of property added.</returns>
    public int SetRange(IEnumerable<string> col, IEnumerable<string> propertyMapping, bool skipDuplicate = false)
    {
        if (col is null) return 0;
        var array = col.ToList();
        var keys = propertyMapping?.ToList() ?? Collection.ListExtensions.CreateNumberRange(0, array.Count).Select(ele => ele.ToString("g")).ToList();
        var count = Math.Min(keys.Count, array.Count);
        if (skipDuplicate)
        {
            var count2 = 0;
            for (var i = 0; i < count; i++)
            {
                var key = keys[i];
                if (string.IsNullOrEmpty(key) || store.ContainsKey(key)) continue;
                SetValue(key, array[i]);
            }

            return count2;
        }

        for (var i = 0; i < count; i++)
        {
            var key = keys[i];
            if (string.IsNullOrEmpty(key)) continue;
            SetValue(key, array[i]);
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
                        var obj = new JsonObjectNode();
                        obj.SetRange(ref reader);
                        SetValue(key, obj);
                    }

                    break;
                case JsonTokenType.StartArray:
                    if (!skipDuplicate || !ContainsKey(key))
                    {
                        var arr = JsonArrayNode.ParseValue(ref reader);
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
    /// Increases a number property.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="value">The value to increase.</param>
    /// <exception cref="ArgumentNullException">key was null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="InvalidOperationException">The property kind was not number.</exception>
    public void IncreaseValue(string key, int value = 1)
    {
        AssertKey(key);
        var v = TryGetValue(key);
        if (v is null || v.ValueKind == JsonValueKind.Null || v.ValueKind == JsonValueKind.Undefined)
        {
            SetValue(key, value);
            return;
        }

        if (v is JsonIntegerNode i)
        {
            SetValue(key, i.Value + value);
            return;
        }

        if (v is JsonDoubleNode j)
        {
            SetValue(key, j.Value + value);
            return;
        }

        if (v is JsonStringNode s)
        {
            if (s.Length == 0)
            {
                SetProperty(key, new JsonStringNode(value));
                return;
            }

            if (s.TryGetDouble(out var d))
            {
                if (s.TryGetInt64(out var l) && d == l) SetValue(key, l + value);
                else SetValue(key, d + value);
                return;
            }
        }

        if (v is JsonArrayNode a && a.Count == 0)
        {
            a.Add(value);
            return;
        }

        throw new InvalidOperationException($"Expect a number but its kind is {v.ValueKind}.");
    }

    /// <summary>
    /// Increases a number property.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="value">The value to increase.</param>
    /// <exception cref="ArgumentNullException">key was null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="InvalidOperationException">The property kind was not number.</exception>
    public void IncreaseValue(string key, long value)
    {
        AssertKey(key);
        var v = TryGetValue(key);
        if (v is null || v.ValueKind == JsonValueKind.Null || v.ValueKind == JsonValueKind.Undefined)
        {
            SetValue(key, value);
            return;
        }

        if (v is JsonIntegerNode i)
        {
            SetValue(key, i.Value + value);
            return;
        }

        if (v is JsonDoubleNode j)
        {
            SetValue(key, j.Value + value);
            return;
        }

        if (v is JsonStringNode s)
        {
            if (s.Length == 0)
            {
                SetProperty(key, new JsonStringNode(value));
                return;
            }

            if (s.TryGetDouble(out var d))
            {
                if (s.TryGetInt64(out var l) && d == l) SetValue(key, l + value);
                else SetValue(key, d + value);
                return;
            }
        }

        if (v is JsonArrayNode a && a.Count == 0)
        {
            a.Add(value);
            return;
        }

        throw new InvalidOperationException($"Expect a number but its kind is {v.ValueKind}.");
    }

    /// <summary>
    /// Increases a number property.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="value">The value to increase.</param>
    /// <exception cref="ArgumentNullException">key was null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="InvalidOperationException">The property kind was not number.</exception>
    public void IncreaseValue(string key, double value)
    {
        AssertKey(key);
        var v = TryGetValue(key);
        if (v is null || v.ValueKind == JsonValueKind.Null || v.ValueKind == JsonValueKind.Undefined)
        {
            SetValue(key, value);
            return;
        }

        if (v is JsonIntegerNode i)
        {
            SetValue(key, i.Value + value);
            return;
        }

        if (v is JsonDoubleNode j)
        {
            SetValue(key, j.Value + value);
            return;
        }

        if (v is JsonStringNode s)
        {
            if (s.Length == 0)
            {
                SetProperty(key, new JsonStringNode(value));
                return;
            }

            if (s.TryGetDouble(out var d))
            {
                SetValue(key, d + value);
                return;
            }
        }

        if (v is JsonArrayNode a && a.Count == 0)
        {
            a.Add(value);
            return;
        }

        throw new InvalidOperationException($"Expect a number but its kind is {v.ValueKind}.");
    }

    /// <summary>
    /// Decreases a number property.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="value">The value to decrease.</param>
    /// <exception cref="ArgumentNullException">key was null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="InvalidOperationException">The property kind was not number.</exception>
    public void DecreaseValue(string key, int value = 1)
        => IncreaseValue(key, -value);

    /// <summary>
    /// Decreases a number property.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="value">The value to decrease.</param>
    /// <exception cref="ArgumentNullException">key was null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="InvalidOperationException">The property kind was not number.</exception>
    public void DecreaseValue(string key, long value)
        => IncreaseValue(key, -value);

    /// <summary>
    /// Decreases a number property.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="value">The value to decrease.</param>
    /// <exception cref="ArgumentNullException">key was null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="InvalidOperationException">The property kind was not number.</exception>
    public void DecreaseValue(string key, double value)
        => IncreaseValue(key, -value);

    /// <summary>
    /// Initializes a value with an empty JSON object if the property does not exist.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="force">true if set a new one if the current is not JSON object; otherwise, false.</param>
    /// <param name="oldValueKind">The value kind before initializing.</param>
    /// <returns>true if the object is ready, that means no value there before; otherwise, false.</returns>
    public bool EnsureObjectValue(string key, bool force, out JsonValueKind oldValueKind)
    {
        oldValueKind = GetValueKind(key);
        if (oldValueKind == JsonValueKind.Object) return true;
        if (!force && oldValueKind != JsonValueKind.Null && oldValueKind != JsonValueKind.Undefined) return false;
        SetValue(key, new JsonObjectNode());
        return true;
    }

    /// <summary>
    /// Initializes a value with an empty JSON object if the property does not exist.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="oldValueKind">The value kind before initializing.</param>
    /// <returns>true if the object is ready, that means no value there before; otherwise, false.</returns>
    public bool EnsureObjectValue(string key, out JsonValueKind oldValueKind)
        => EnsureObjectValue(key, false, out oldValueKind);

    /// <summary>
    /// Initializes a value with an empty JSON array if the property does not exist.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="force">true if set a new one if the current is not JSON array; otherwise, false.</param>
    /// <param name="oldValueKind">The value kind before initializing.</param>
    /// <returns>true if the object is ready, that means no value there before; otherwise, false.</returns>
    public bool EnsureArrayValue(string key, bool force, out JsonValueKind oldValueKind)
    {
        oldValueKind = GetValueKind(key);
        if (oldValueKind == JsonValueKind.Array) return true;
        if (!force && oldValueKind != JsonValueKind.Null && oldValueKind != JsonValueKind.Undefined) return false;
        SetValue(key, new JsonArrayNode());
        return true;
    }

    /// <summary>
    /// Initializes a value with an empty JSON array if the property does not exist.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="oldValueKind">The value kind before initializing.</param>
    /// <returns>true if the object is ready, that means no value there before; otherwise, false.</returns>
    public bool EnsureArrayValue(string key, out JsonValueKind oldValueKind)
        => EnsureObjectValue(key, false, out oldValueKind);

    /// <summary>
    /// Appends a value into a string property.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="value">The string value to append.</param>
    /// <returns>The string value.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="InvalidOperationException">The property kind was not string.</exception>
    public string AppendValue(string key, string value)
    {
        AssertKey(key);
        var v = TryGetValue(key);
        if (v is null || v.ValueKind == JsonValueKind.Null || v.ValueKind == JsonValueKind.Undefined)
        {
            SetValue(key, value);
            return value;
        }

        if (v is JsonStringNode s)
        {
            value = (s.Value ?? string.Empty) + value;
            SetValue(key, value);
            return value;
        }

        if (v.ValueKind != JsonValueKind.Array && v.ValueKind != JsonValueKind.Object)
        {
            value = (v.ToString() ?? string.Empty) + value;
            SetValue(key, value);
            return value;
        }

        if (v is JsonArrayNode a && a.Count == 0)
        {
            a.Add(value);
            return value;
        }

        throw new InvalidOperationException($"Expect a string but its kind is {v.ValueKind}.");
    }

    /// <summary>
    /// Appends a value into a string property.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="value">The string value to append.</param>
    /// <returns>The string value.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="InvalidOperationException">The property kind was not string.</exception>
    public string AppendValue(string key, StringBuilder value)
        => AppendValue(key, value?.ToString());

    /// <summary>
    /// Appends a value into a string property.
    /// </summary>
    /// <param name="key">The key of the property.</param>
    /// <param name="format">The string value to append.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>The string value.</returns>
    /// <exception cref="ArgumentNullException">key was null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="InvalidOperationException">The property kind was not string.</exception>
    public string AppendValueFormat(string key, string format, params object[] args)
        => AppendValue(key, string.Format(format, args));

    /// <summary>
    /// Sets local definition.
    /// </summary>
    /// <param name="id">The definition identifier or key.</param>
    /// <param name="value">The JSON object of definition.</param>
    public void SetLocalDefinition(string id, JsonObjectNode value)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        var json = LocalDefinitions;
        if (json == null)
        {
            json = new();
            LocalDefinitions = json;
        }

        json.SetValue(id, value);
    }

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, IJsonDataNode> item)
        => AddProperty(item.Key, JsonValues.ConvertValue(item.Value, this));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, IJsonValueNode> item)
        => AddProperty(item.Key, JsonValues.ConvertValue(item.Value, this));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, IJsonStringNode> item)
        => AddProperty(item.Key, JsonValues.ConvertValue(item.Value, this));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, JsonStringNode> item)
        => AddProperty(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, JsonIntegerNode> item)
        => AddProperty(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, JsonDoubleNode> item)
        => AddProperty(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, JsonBooleanNode> item)
        => AddProperty(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, JsonArrayNode> item)
        => AddProperty(item.Key, JsonValues.ConvertValue(item.Value, this));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, JsonObjectNode> item)
        => AddProperty(item.Key, JsonValues.ConvertValue(item.Value, this));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, string> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, StringBuilder> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    /// <exception cref="InvalidOperationException">The secure string is disposed.</exception>
    public void Add(KeyValuePair<string, SecureString> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, Guid> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, short> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, int> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, uint> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, long> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, float> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, double> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, bool> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, JsonDocument> item)
        => AddProperty(item.Key, JsonValues.ToJsonValue(item.Value) ?? JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, JsonElement> item)
        => AddProperty(item.Key, JsonValues.ToJsonValue(item.Value) ?? JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, System.Text.Json.Nodes.JsonArray> item)
        => AddProperty(item.Key, JsonValues.ToJsonValue(item.Value) ?? JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="item">The property to add to the JSON object.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(KeyValuePair<string, System.Text.Json.Nodes.JsonObject> item)
        => AddProperty(item.Key, JsonValues.ToJsonValue(item.Value) ?? JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="_">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, DBNull _)
        => AddProperty(key, JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, IJsonDataNode value)
        => AddProperty(key, JsonValues.ConvertValue(value, this));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, IJsonValueNode value)
        => AddProperty(key, JsonValues.ConvertValue(value, this));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, JsonObjectNode value)
        => AddProperty(key, (ReferenceEquals(value, this) ? new()
        {
            { "$ref", JsonValues.SELF_REF }
        } : value) ?? JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, IJsonObjectHost value)
        => AddProperty(key, value?.ToJson() ?? JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="clone">true if clone the value before adding; otherwise, false.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, JsonObjectNode value, bool clone)
    {
        if (clone) AddProperty(key, value?.Clone() ?? JsonValues.Null);
        else Add(key, value);
    }

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, JsonDocument value)
        => AddProperty(key, JsonValues.ToJsonValue(value) ?? JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, JsonElement value)
        => AddProperty(key, JsonValues.ToJsonValue(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, System.Text.Json.Nodes.JsonNode value)
        => AddProperty(key, JsonValues.ToJsonValue(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, string value)
        => AddProperty(key, value != null ? new JsonStringNode(value) : JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, char[] value)
        => AddProperty(key, value != null ? new JsonStringNode(value) : JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, StringBuilder value)
        => AddProperty(key, value != null ? new JsonStringNode(value) : JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    /// <exception cref="InvalidOperationException">The secure string is disposed.</exception>
    public void Add(string key, SecureString value)
        => AddProperty(key, value != null ? new JsonStringNode(value) : JsonValues.Null);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, ReadOnlySpan<char> value)
        => AddProperty(key, new JsonStringNode(value.ToString()));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, short value)
        => AddProperty(key, new JsonIntegerNode(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, int value)
        => AddProperty(key, new JsonIntegerNode(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, int value, string format, IFormatProvider provider)
        => AddProperty(key, new JsonStringNode(value, format, provider));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, long value)
        => AddProperty(key, new JsonIntegerNode(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, long value, string format, IFormatProvider provider)
        => AddProperty(key, new JsonStringNode(value, format, provider));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, uint value)
        => AddProperty(key, new JsonIntegerNode(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, float value)
        => AddProperty(key, float.IsNaN(value) ? JsonValues.Null : new JsonDoubleNode(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, float value, string format, IFormatProvider provider)
        => AddProperty(key, float.IsNaN(value) ? JsonValues.Null : new JsonStringNode(value, format, provider));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, double value)
        => AddProperty(key, double.IsNaN(value) ? JsonValues.Null : new JsonDoubleNode(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, double value, string format, IFormatProvider provider)
        => AddProperty(key, double.IsNaN(value) ? JsonValues.Null : new JsonStringNode(value, format, provider));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, decimal value)
        => AddProperty(key, new JsonDoubleNode(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="format">A standard or custom time span format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, decimal value, string format, IFormatProvider provider)
        => AddProperty(key, new JsonStringNode(value, format, provider));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, bool value)
        => AddProperty(key, value ? JsonBooleanNode.True : JsonBooleanNode.False);

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, DateTime value)
        => AddProperty(key, new JsonStringNode(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="format">A standard or custom date and time format string.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, DateTime value, string format)
        => AddProperty(key, new JsonStringNode(value, format));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, DateTimeOffset value)
        => AddProperty(key, new JsonStringNode(value));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="format">A standard or custom date and time format string.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, DateTimeOffset value, string format)
        => AddProperty(key, new JsonStringNode(value, format));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, Guid value)
        => AddProperty(key, new JsonStringNode(value));

    /// <summary>
    /// Sets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void Add(string key, Uri value)
    {
        AssertKey(key);
        string url = null;
        try
        {
            url = value?.OriginalString;
        }
        catch (InvalidOperationException)
        {
        }

        AddProperty(key, string.IsNullOrEmpty(url) ? JsonValues.Null : new JsonStringNode(url));
    }

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="format">A standard or custom GUID format string.</param>
    /// <exception cref="ArgumentNullException">key is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the JSON object.</exception>
    public void Add(string key, Guid value, string format)
        => AddProperty(key, new JsonStringNode(value, format));

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void Add(string key, IEnumerable<string> value)
    {
        AssertKey(key);
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        AddProperty(key, arr);
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
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        AddProperty(key, arr);
    }

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void Add(string key, IEnumerable<long> value)
    {
        AssertKey(key);
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        AddProperty(key, arr);
    }

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void Add(string key, IEnumerable<double> value)
    {
        AssertKey(key);
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        AddProperty(key, arr);
    }

    /// <summary>
    /// Adds a property with the provided key and value to the JSON object.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    public void Add(string key, IEnumerable<JsonObjectNode> value)
    {
        AssertKey(key);
        var arr = new JsonArrayNode();
        arr.AddRange(value);
        AddProperty(key, arr);
    }

    /// <summary>
    /// Determines whether the JSON object contains a specific property.
    /// </summary>
    /// <param name="item">The property to locate in the JSON object.</param>
    /// <returns>true if property is found in the JSON object; otherwise, false.</returns>
    public bool Contains(KeyValuePair<string, IJsonDataNode> item)
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
    public bool Contains(KeyValuePair<string, IJsonValueNode> item)
    {
        foreach (var ele in store)
        {
            if (JsonValues.Equals(ele.Value, item.Value)) return true;
        }

        return false;
    }

    /// <summary>
    /// Copies the elements of the JSON object to an array, starting at a particular System.Array index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from JSON object. The System.Array must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentNullException">array is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="ArgumentException">The number of elements in the source  is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(KeyValuePair<string, IJsonDataNode>[] array, int arrayIndex)
        => store.CopyTo(array, arrayIndex);

    /// <summary>
    /// Copies the elements of the JSON object to an array, starting at a particular System.Array index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from JSON object. The System.Array must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentNullException">array is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="ArgumentException">The number of elements in the source  is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(KeyValuePair<string, IJsonValueNode>[] array, int arrayIndex)
    {
        store.Select(ele => new KeyValuePair<string, IJsonValueNode>(ele.Key, ele.Value ?? JsonValues.Null)).ToList().CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the first occurrence of a specific property from the JSON object.
    /// </summary>
    /// <param name="item">The property to remove from the JSON object.</param>
    /// <returns>true if property was successfully removed from the JSON object; otherwise, false. This method also returns false if property is not found in the original JSON object.</returns>
    public bool Remove(KeyValuePair<string, IJsonDataNode> item)
    {
        KeyValuePair<string, IJsonDataNode>? kvp = null;
        foreach (var ele in store)
        {
            if (!JsonValues.Equals(ele.Value, item.Value)) continue;
            kvp = ele;
            break;
        }

        if (!kvp.HasValue) return false;
        return RemoveProperty(kvp.Value);
    }

    /// <summary>
    /// Removes the first occurrence of a specific property from the JSON object.
    /// </summary>
    /// <param name="item">The property to remove from the JSON object.</param>
    /// <returns>true if property was successfully removed from the JSON object; otherwise, false. This method also returns false if property is not found in the original JSON object.</returns>
    public bool Remove(KeyValuePair<string, IJsonValueNode> item)
    {
        KeyValuePair<string, IJsonDataNode>? kvp = null;
        foreach (var ele in store)
        {
            if (!JsonValues.Equals(ele.Value, item.Value)) continue;
            kvp = ele;
            break;
        }

        if (!kvp.HasValue) return false;
        return RemoveProperty(kvp.Value);
    }

    /// <summary>
    /// Removes all properties from the object.
    /// </summary>
    public void Clear()
    {
        var keys = store.Keys;
        store.Clear();
        if (PropertyChanged == null) return;
        foreach (var key in keys)
        {
            PropertyChanged?.Invoke(this, new KeyValueEventArgs<string, IJsonDataNode>(key, JsonValues.Undefined));
        }
    }

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public bool Any()
        => store.Any();

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public bool Equals(JsonObjectNode other)
    {
        if (other is null) return false;
        if (base.Equals(other)) return true;
        if (other.Count != Count) return false;
        foreach (var prop in store)
        {
            var isNull = !other.TryGetValue(prop.Key, out var r) || r is null || r.ValueKind == JsonValueKind.Null || r.ValueKind == JsonValueKind.Undefined;
            if (!isNull && (prop.Value is null || prop.Value.ValueKind == JsonValueKind.Null || prop.Value.ValueKind == JsonValueKind.Undefined))
                return false;
            if (isNull || !JsonValues.Equals(prop.Value, r)) return false;
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
        if (other is JsonObjectNode json) return Equals(json);
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
        if (other is JsonObjectNode json) return Equals(json);
        return false;
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode()
        => store.GetHashCode();

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
                    if (prop.Value is JsonStringNode strJson) writer.WriteString(prop.Key, strJson.Value);
                    break;
                case JsonValueKind.Number:
                    if (prop.Value is JsonIntegerNode intJson) writer.WriteNumber(prop.Key, (long)intJson);
                    else if (prop.Value is JsonDoubleNode floatJson) writer.WriteNumber(prop.Key, (double)floatJson);
                    break;
                case JsonValueKind.True:
                    writer.WriteBoolean(prop.Key, true);
                    break;
                case JsonValueKind.False:
                    writer.WriteBoolean(prop.Key, false);
                    break;
                case JsonValueKind.Object:
                    writer.WritePropertyName(prop.Key);
                    if (prop.Value is JsonObjectNode objJson) objJson.WriteTo(writer);
                    break;
                case JsonValueKind.Array:
                    writer.WritePropertyName(prop.Key);
                    if (prop.Value is JsonArrayNode objArr) objArr.WriteTo(writer);
                    break;
            }
        }

        writer.WriteEndObject();
    }

    /// <summary>
    /// Writes to file.
    /// </summary>
    /// <param name="path">The path of the file. If the target file already exists, it is overwritten.</param>
    /// <param name="style">The indent style.</param>
    /// <exception cref="IOException">IO exception.</exception>
    /// <exception cref="SecurityException">Write failed because of security exception.</exception>
    /// <exception cref="ArgumentException">path was invalid..</exception>
    /// <exception cref="ArgumentNullException">path was null.</exception>
    /// <exception cref="NotSupportedException">path was not supported.</exception>
    /// <exception cref="UnauthorizedAccessException">Write failed because of unauthorized access exception.</exception>
    public void WriteTo(string path, IndentStyles style = IndentStyles.Minified)
        => File.WriteAllText(path, ToString(style) ?? "null");

    /// <summary>
    /// Writes to file.
    /// </summary>
    /// <param name="file">The file to write.</param>
    /// <param name="style">The indent style.</param>
    /// <exception cref="IOException">IO exception.</exception>
    /// <exception cref="SecurityException">Write failed because of security exception.</exception>
    /// <exception cref="ArgumentNullException">The file path was null.</exception>
    /// <exception cref="NotSupportedException">The file was not supported.</exception>
    /// <exception cref="UnauthorizedAccessException">Write failed because of unauthorized access exception.</exception>
    public void WriteTo(FileInfo file, IndentStyles style = IndentStyles.Minified)
    {
        if (file == null) throw new ArgumentNullException(nameof(file), "file should not be null.");
        File.WriteAllText(file.FullName, ToString(style) ?? "null");
        file.Refresh();
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Writes to file.
    /// </summary>
    /// <param name="path">The path of the file. If the target file already exists, it is overwritten.</param>
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
    /// Tries to write to a file.
    /// </summary>
    /// <param name="path">The path of the file. If the target file already exists, it is overwritten.</param>
    /// <param name="style">The indent style.</param>
    /// <returns>true if write succeeded; otherwise, false.</returns>
    public bool TryWriteTo(string path, IndentStyles style = IndentStyles.Minified)
    {
        try
        {
            WriteTo(path, style);
            return true;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (IOException)
        {
        }
        catch (JsonException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ApplicationException)
        {
        }
        catch (ExternalException)
        {
        }

        return false;
    }

    /// <summary>
    /// Tries to write to a file.
    /// </summary>
    /// <param name="file">The file to save.</param>
    /// <param name="style">The indent style.</param>
    /// <returns>true if write succeeded; otherwise, false.</returns>
    public bool TryWriteTo(FileInfo file, IndentStyles style = IndentStyles.Minified)
    {
        try
        {
            WriteTo(file, style);
            return true;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (IOException)
        {
        }
        catch (JsonException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ApplicationException)
        {
        }
        catch (ExternalException)
        {
        }

        return false;
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
            str.Append(JsonStringNode.ToJson(prop.Key));
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
        => ConvertToString(indentStyle, 0);

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
            str.Append(JsonStringNode.ToJson(prop.Key));
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
                    str.Append((prop.Value is JsonArrayNode jArr) ? jArr.ConvertToString(indentStyle, indentLevel) : "[]");
                    break;
                case JsonValueKind.Object:
                    str.Append((prop.Value is JsonObjectNode jObj) ? jObj.ConvertToString(indentStyle, indentLevel) : "{}");
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
        var nextIndentLevel = indentLevel + 1;
        var str = new StringBuilder();
        foreach (var prop in store)
        {
            str.Append(indentStr);
            str.Append(prop.Key.IndexOfAny(StringExtensions.YamlSpecialChars) >= 0
                ? JsonStringNode.ToJson(prop.Key)
                : prop.Key);
            str.Append(": ");
            if (prop.Value is null)
            {
                str.AppendLine("~");
                continue;
            }

            switch (prop.Value.ValueKind)
            {
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                    str.AppendLine("~");
                    break;
                case JsonValueKind.Array:
                    if (prop.Value is not JsonArrayNode jArr)
                    {
                        str.AppendLine("[]");
                        break;
                    }
                        
                    str.AppendLine();
                    str.Append(jArr.ConvertToYamlString(indentLevel));
                    break;
                case JsonValueKind.Object:
                    if (prop.Value is not JsonObjectNode jObj)
                    {
                        str.AppendLine("{}");
                        break;
                    }

                    str.AppendLine();
                    str.Append(jObj.ConvertToYamlString(nextIndentLevel));
                    break;
                case JsonValueKind.String:
                    if (prop.Value is not JsonStringNode jStr)
                    {
                        str.AppendLine(prop.Value.ToString());
                        break;
                    }

                    var text = jStr.StringValue;
                    if (text == null)
                    {
                        str.AppendLine("~");
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
                            str.AppendLine(text.Length < 2 || text.Length > 100 || text.Equals("null", StringComparison.OrdinalIgnoreCase) || text.IndexOfAny(StringExtensions.YamlSpecialChars) >= 0
                                ? JsonStringNode.ToJson(text)
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
    /// <exception cref="JsonException">The JSON is invalid. -or- The value type is not compatible with the JSON.</exception>
    public T Deserialize<T>(JsonSerializerOptions options = default)
        => JsonSerializer.Deserialize<T>(ToString(), options);

    /// <summary>
    /// Deserializes.
    /// </summary>
    /// <param name="returnType">The type of the object to convert to and return.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
    /// <exception cref="JsonException">The JSON is invalid. -or- The value is type not compatible with the JSON.</exception>
    public object Deserialize(Type returnType, JsonSerializerOptions options = default)
        => JsonSerializer.Deserialize(ToString(), returnType, options);

    /// <summary>
    /// Deserializes a property value.
    /// </summary>
    /// <typeparam name="T">The type of model to deserialize.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="JsonException">The JSON is invalid. -or- The value type is not compatible with the JSON.</exception>
    public T DeserializeValue<T>(string key, JsonSerializerOptions options = default)
    {
        AssertKey(key);
        var item = store[key];
        if (item is null || item.ValueKind == JsonValueKind.Null || item.ValueKind == JsonValueKind.Undefined) return default;
        return JsonSerializer.Deserialize<T>(item.ToString(), options);
    }

    /// <summary>
    /// Deserializes a property value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="returnType">The type of the object to convert to and return.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="ArgumentException">readerOptions contains unsupported options.</exception>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="JsonException">The JSON is invalid. -or- The value type is not compatible with the JSON.</exception>
    public object DeserializeValue(string key, Type returnType, JsonSerializerOptions options = default)
    {
        AssertKey(key);
        var item = store[key];
        if (item is null || item.ValueKind == JsonValueKind.Null || item.ValueKind == JsonValueKind.Undefined) return default;
        return JsonSerializer.Deserialize(item.ToString(), returnType, options);
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
        if (item is null || item.ValueKind == JsonValueKind.Null || item.ValueKind == JsonValueKind.Undefined) return default;
        if (parser != null && item is IJsonStringNode s) return parser(s.StringValue);
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
    /// Projects each element into a new form by incorporation the key.
    /// </summary>
    /// <typeparam name="T">The type of element output.</typeparam>
    /// <param name="selector">The transform function.</param>
    /// <returns>A collection whose elements are the result of invoking the transform function on each property.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public IEnumerable<T> Select<T>(Func<JsonValueKind, object, string, T> selector)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector), "selector was null.");
        foreach (var item in store)
        {
            var value = item.Value;
            yield return selector(value.ValueKind, JsonValues.GetValue(value), item.Key);
        }
    }

    /// <summary>
    /// Creates a new JSON object filtered by specific predication.
    /// </summary>
    /// <param name="predicate">An optional function to test each source element for a condition; the second parameter of the function represents the property key; the third is the index of the element after filter.</param>
    /// <returns>A new JSON object after filter.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public JsonObjectNode Where(Func<JsonValueKind, object, string, int, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate), "predicate was null.");
        var json = new JsonObjectNode();
        var i = -1;
        foreach (var item in store)
        {
            var value = item.Value;
            i++;
            if (!predicate(value.ValueKind, JsonValues.GetValue(value), item.Key, i)) continue;
            json[item.Key] = value;
        }

        return json;
    }

    /// <summary>
    /// Creates a new JSON object of specific keys.
    /// </summary>
    /// <param name="keys">The properties keys.</param>
    /// <param name="removeNull">true if skip null values; otherwise, false.</param>
    /// <returns>A new JSON object of the specific keys.</returns>
    public JsonObjectNode Where(IEnumerable<string> keys, bool removeNull = false)
    {
        var dict = new JsonObjectNode();
        if (removeNull)
        {
            foreach (var item in keys)
            {
                if (store.TryGetValue(item, out var r) && r is not null && r.ValueKind != JsonValueKind.Null && r.ValueKind != JsonValueKind.Undefined)
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
    /// Creates a new JSON object filtered by specific predication.
    /// </summary>
    /// <param name="kind">The data type of JSON value to filter.</param>
    /// <param name="predicate">An optional function to test each source element for a condition; the second parameter of the function represents the property key; the third is the index of the element after filter.</param>
    /// <returns>A new JSON object after filter.</returns>
    public JsonObjectNode Where(JsonValueKind kind, Func<IJsonDataNode, string, int, bool> predicate = null)
    {
        predicate ??= PassTrue;
        var dict = new JsonObjectNode();
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
    public IEnumerable<KeyValuePair<string, IJsonDataNode>> Where(Func<KeyValuePair<string, IJsonDataNode>, bool> predicate)
    {
        if (predicate == null) return store.Select(ele => ele.Value is null ? new KeyValuePair<string, IJsonDataNode>(ele.Key, JsonValues.Null) : ele);
        return store.Select(ele => ele.Value is null ? new KeyValuePair<string, IJsonDataNode>(ele.Key, JsonValues.Null) : ele).Where(predicate);
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
    /// <returns>A collection that contains elements from the input sequence that satisfy the condition.</returns>
    public IEnumerable<KeyValuePair<string, IJsonDataNode>> Where(Func<KeyValuePair<string, IJsonDataNode>, int, bool> predicate)
    {
        if (predicate == null) return store.Select(ele => ele.Value is null ? new KeyValuePair<string, IJsonDataNode>(ele.Key, JsonValues.Null) : ele);
        return store.Select(ele => ele.Value is null ? new KeyValuePair<string, IJsonDataNode>(ele.Key, JsonValues.Null) : ele).Where(predicate);
    }

    /// <summary>
    /// Creates a dictionary from this instance.
    /// </summary>
    /// <returns>A dictionary that contains the key value pairs from this instance.</returns>
    public Dictionary<string, IJsonDataNode> ToDictionary()
        => new(store);

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public JsonObjectNode Clone()
        => new(store, store is ConcurrentDictionary<string, IJsonDataNode>);

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="keys">The keys to copy; or null to clone all.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public JsonObjectNode Clone(IEnumerable<string> keys)
    {
        if (keys == null) return new JsonObjectNode(store, store is ConcurrentDictionary<string, IJsonDataNode>);
        var json = new JsonObjectNode();
        if (store is ConcurrentDictionary<string, IJsonDataNode>) json.EnableThreadSafeMode();
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
        => Clone();

    /// <summary>
    /// Returns an enumerator that iterates through the properties collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the properties collection.</returns>
    public IEnumerator<KeyValuePair<string, IJsonDataNode>> GetEnumerator()
        => store.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the properties collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the properties collection.</returns>
    IEnumerator<KeyValuePair<string, IJsonValueNode>> IEnumerable<KeyValuePair<string, IJsonValueNode>>.GetEnumerator()
        => store.Select(ele => new KeyValuePair<string, IJsonValueNode>(ele.Key, ele.Value ?? JsonValues.Null)).GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the properties collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the properties collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
        => store.GetEnumerator();

    /// <summary>
    /// Gets the value of the element as a boolean.
    /// </summary>
    /// <returns>The value of the element as a boolean.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    bool IJsonDataNode.GetBoolean() => throw new InvalidOperationException("Expect a boolean but it is an object.");

    /// <summary>
    /// Gets the value of the element as a byte array.
    /// </summary>
    /// <returns>The value decoded as a byte array.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    byte[] IJsonDataNode.GetBytesFromBase64() => throw new InvalidOperationException("Expect a string but it is an object.");

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <returns>The value of the element as a date time.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    DateTime IJsonDataNode.GetDateTime() => throw new InvalidOperationException("Expect a date time but it is an object.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    decimal IJsonDataNode.GetDecimal() => throw new InvalidOperationException("Expect a number but it is an object.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    float IJsonDataNode.GetSingle() => throw new InvalidOperationException("Expect a number but it is an object.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    double IJsonDataNode.GetDouble() => throw new InvalidOperationException("Expect a number but it is an object.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    short IJsonDataNode.GetInt16() => throw new InvalidOperationException("Expect a number but it is an object.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    uint IJsonDataNode.GetUInt32() => throw new InvalidOperationException("Expect a number but it is an object.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    int IJsonDataNode.GetInt32() => throw new InvalidOperationException("Expect a number but it is an object.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    long IJsonDataNode.GetInt64() => throw new InvalidOperationException("Expect a number but it is an object.");

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <returns>The value of the element as a number.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    string IJsonDataNode.GetString() => throw new InvalidOperationException("Expect a string but it is an object.");

    /// <summary>
    /// Gets the value of the element as a GUID.
    /// </summary>
    /// <returns>The value of the element as a GUID.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    Guid IJsonDataNode.GetGuid() => throw new InvalidOperationException("Expect a string but it is an object.");

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
        result = WebFormat.ParseDate(0);
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
        result = float.NaN;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonDataNode.TryGetDouble(out double result)
    {
        result = double.NaN;
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
    /// Gets all property keys.
    /// </summary>
    /// <returns>The property keys.</returns>
    IEnumerable<string> IJsonDataNode.GetKeys() => Keys;

    /// <summary>
    /// Gets all property keys.
    /// </summary>
    /// <returns>The property keys.</returns>
    IEnumerable<string> IJsonContainerNode.GetKeys() => Keys;

    private void AddProperty(string key, IJsonDataNode value)
    {
        store.Add(key, value);
        PropertyChanged?.Invoke(this, new KeyValueEventArgs<string, IJsonDataNode>(key, value));
    }

    private void SetProperty(string key, IJsonDataNode value)
    {
        store[key] = value;
        PropertyChanged?.Invoke(this, new KeyValueEventArgs<string, IJsonDataNode>(key, value));
    }

    private bool RemoveProperty(string key)
    {
        var b = store.Remove(key);
        if (b) PropertyChanged?.Invoke(this, new KeyValueEventArgs<string, IJsonDataNode>(key, JsonValues.Undefined));
        return b;
    }

    private bool RemoveProperty(KeyValuePair<string, IJsonDataNode> kvp)
    {
        var b = store.Remove(kvp);
        if (b) PropertyChanged?.Invoke(this, new KeyValueEventArgs<string, IJsonDataNode>(kvp.Key, JsonValues.Undefined));
        return b;
    }

    private JsonObjectNode ConvertObjectValueForProperty(JsonObjectNode json)
        => json?.Count == 1 && json.TryGetStringValue("$ref", true, out var s) && s == JsonValues.SELF_REF ? this : json;

    private T GetJsonValue<T>(string key, JsonValueKind? valueKind = null, bool ignoreNull = false) where T : IJsonValueNode
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

    private bool TryGetJsonValue<T>(string key, out T property) where T : IJsonValueNode
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

    private static void AssertKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), "key should not be null, empty, or consists only of white-space characters.");
    }

    /// <summary>
    /// Converts to JSON document.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonDocument class.</returns>
    public static explicit operator JsonDocument(JsonObjectNode json)
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
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonObjectNode class.</returns>
    public static explicit operator System.Text.Json.Nodes.JsonObject(JsonObjectNode json)
    {
        if (json == null) return null;
        var node = new System.Text.Json.Nodes.JsonObject();
        foreach (var prop in json.store)
        {
            var v = JsonValues.ToJsonNode(prop.Value);
            node[prop.Key] = v;
        }

        return node;
    }

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonNode class.</returns>
    public static explicit operator System.Text.Json.Nodes.JsonNode(JsonObjectNode json)
        => (System.Text.Json.Nodes.JsonObject)json;

    /// <summary>
    /// Converts from JSON document.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonObjectNode class.</returns>
    /// <exception cref="JsonException">json does not represent a valid JSON object.</exception>
    public static implicit operator JsonObjectNode(JsonDocument json)
    {
        if (json is null) return null;
        return json.RootElement;
    }

    /// <summary>
    /// Converts from JSON element.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonObjectNode class.</returns>
    /// <exception cref="JsonException">json does not represent a valid JSON object.</exception>
    public static implicit operator JsonObjectNode(JsonElement json)
    {
        if (json.ValueKind != JsonValueKind.Object)
        {
            return json.ValueKind switch
            {
                JsonValueKind.Null => null,
                JsonValueKind.Undefined => null,
                _ => throw new JsonException("json is not a JSON object.")
            };
        }

        var result = new JsonObjectNode();
        var enumerator = json.EnumerateObject();
        while (enumerator.MoveNext())
        {
            result.SetValue(enumerator.Current);
        }

        return result;
    }

    /// <summary>
    /// Converts from JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonObjectNode class.</returns>
    /// <exception cref="JsonException">json does not represent a valid JSON object.</exception>
    public static implicit operator JsonObjectNode(System.Text.Json.Nodes.JsonObject json)
    {
        if (json is null) return null;
        var result = new JsonObjectNode();
        foreach (var prop in json)
        {
            result.SetValue(prop.Key, prop.Value);
        }

        return result;
    }

    /// <summary>
    /// Converts from JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonObjectNode class.</returns>
    /// <exception cref="JsonException">json does not represent a valid JSON object.</exception>
    public static implicit operator JsonObjectNode(System.Text.Json.Nodes.JsonNode json)
    {
        if (json is System.Text.Json.Nodes.JsonObject obj) return obj;
        throw new JsonException("json is not a JSON object.");
    }

    /// <summary>
    /// Parses JSON object.
    /// </summary>
    /// <param name="json">A specific JSON object string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static JsonObjectNode Parse(string json, JsonDocumentOptions options = default)
        => JsonDocument.Parse(json, options);

    /// <summary>
    /// Parses JSON object.
    /// </summary>
    /// <param name="json">A specific JSON object string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static JsonObjectNode Parse(System.Buffers.ReadOnlySequence<byte> json, JsonDocumentOptions options)
        => JsonDocument.Parse(json, options);

    /// <summary>
    /// Parses JSON object.
    /// </summary>
    /// <param name="json">A specific JSON object string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static JsonObjectNode Parse(ReadOnlyMemory<byte> json, JsonDocumentOptions options = default)
        => JsonDocument.Parse(json, options);

    /// <summary>
    /// Parses JSON object.
    /// </summary>
    /// <param name="json">A specific JSON object string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static JsonObjectNode Parse(System.Buffers.ReadOnlySequence<byte> json, JsonReaderOptions options = default)
    {
        var reader = new Utf8JsonReader(json, options);
        var obj = new JsonObjectNode();
        obj.SetRange(ref reader);
        return obj;
    }

    /// <summary>
    /// Parses JSON object.
    /// </summary>
    /// <param name="json">A specific JSON object string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static JsonObjectNode Parse(ReadOnlySpan<byte> json, JsonReaderOptions options = default)
    {
        var reader = new Utf8JsonReader(json, options);
        var obj = new JsonObjectNode();
        obj.SetRange(ref reader);
        return obj;
    }

    /// <summary>
    /// Parses JSON object.
    /// </summary>
    /// <param name="json">A specific JSON object string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static JsonObjectNode Parse(ReadOnlyMemory<char> json, JsonDocumentOptions options = default)
        => JsonDocument.Parse(json, options);

    /// <summary>
    /// Parses a stream as UTF-8-encoded data representing a JSON object.
    /// The stream is read to completion.
    /// </summary>
    /// <param name="utf8Json">The JSON data to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static JsonObjectNode Parse(Stream utf8Json, JsonDocumentOptions options = default)
        => JsonDocument.Parse(utf8Json, options);

    /// <summary>
    /// Parses a stream as UTF-8-encoded data representing a JSON object.
    /// The stream is read to completion.
    /// </summary>
    /// <param name="file">A file with JSON object string content to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    /// <exception cref="IOException">The entry is already currently open for writing, or the entry has been deleted from the archive.</exception>
    /// <exception cref="ObjectDisposedException">The zip archive has been disposed.</exception>
    /// <exception cref="NotSupportedException">The zip archive does not support reading.</exception>
    /// <exception cref="InvalidDataException">The zip archive is corrupt, and the entry cannot be retrieved.</exception>
    public static JsonObjectNode Parse(FileInfo file, JsonDocumentOptions options = default)
    {
        if (file == null || !file.Exists) return null;
        using var stream = file.OpenRead();
        return Parse(stream, options);
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
    public static async Task<JsonObjectNode> ParseAsync(Stream utf8Json, JsonDocumentOptions options, CancellationToken cancellationToken = default)
        => await JsonDocument.ParseAsync(utf8Json, options, cancellationToken);

    /// <summary>
    /// Parses a stream as UTF-8-encoded data representing a JSON object.
    /// The stream is read to completion.
    /// </summary>
    /// <param name="utf8Json">The JSON data to parse.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    public static async Task<JsonObjectNode> ParseAsync(Stream utf8Json, CancellationToken cancellationToken = default)
        => await JsonDocument.ParseAsync(utf8Json, default, cancellationToken);

    /// <summary>
    /// Parses a stream as UTF-8-encoded data representing a JSON object.
    /// The stream is read to completion.
    /// </summary>
    /// <param name="file">A file with JSON object string content to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    /// <exception cref="IOException">The entry is already currently open for writing, or the entry has been deleted from the archive.</exception>
    /// <exception cref="ObjectDisposedException">The zip archive has been disposed.</exception>
    /// <exception cref="NotSupportedException">The zip archive does not support reading.</exception>
    /// <exception cref="InvalidDataException">The zip archive is corrupt, and the entry cannot be retrieved.</exception>
    public static async Task<JsonObjectNode> ParseAsync(FileInfo file, JsonDocumentOptions options = default, CancellationToken cancellationToken = default)
    {
        if (file == null || !file.Exists) return null;
        using var stream = file.OpenRead();
        return await ParseAsync(stream, options, cancellationToken);
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Parses a stream as UTF-8-encoded data representing a JSON object.
    /// The stream is read to completion.
    /// </summary>
    /// <param name="zip">The zip file.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that identifies the entry to retrieve.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    /// <exception cref="IOException">The entry is already currently open for writing, or the entry has been deleted from the archive.</exception>
    /// <exception cref="ObjectDisposedException">The zip archive has been disposed.</exception>
    /// <exception cref="NotSupportedException">The zip archive does not support reading.</exception>
    /// <exception cref="InvalidDataException">The zip archive is corrupt, and the entry cannot be retrieved.</exception>
    public static Task<JsonObjectNode> ParseAsync(System.IO.Compression.ZipArchive zip, string entryName, JsonDocumentOptions options, CancellationToken cancellationToken = default)
    {
        var entry = zip?.GetEntry(entryName);
        if (entry == null) return Task.FromResult<JsonObjectNode>(null);
        using var stream = entry.Open();
        return ParseAsync(stream, options, cancellationToken);
    }

    /// <summary>
    /// Parses a stream as UTF-8-encoded data representing a JSON object.
    /// The stream is read to completion.
    /// </summary>
    /// <param name="zip">The zip file.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that identifies the entry to retrieve.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    /// <exception cref="ArgumentException">options contains unsupported options.</exception>
    /// <exception cref="IOException">The entry is already currently open for writing, or the entry has been deleted from the archive.</exception>
    /// <exception cref="ObjectDisposedException">The zip archive has been disposed.</exception>
    /// <exception cref="NotSupportedException">The zip archive does not support reading.</exception>
    /// <exception cref="InvalidDataException">The zip archive is corrupt, and the entry cannot be retrieved.</exception>
    public static Task<JsonObjectNode> ParseAsync(System.IO.Compression.ZipArchive zip, string entryName, CancellationToken cancellationToken = default)
        => ParseAsync(zip, entryName, default, cancellationToken);
#endif

    /// <summary>
    /// Parses JSON object.
    /// </summary>
    /// <param name="reader">A JSON object.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">json does not represent a valid single JSON object.</exception>
    public static JsonObjectNode ParseValue(ref Utf8JsonReader reader)
    {
        var obj = new JsonObjectNode();
        obj.SetRange(ref reader);
        return obj;
    }

    /// <summary>
    /// Tries to parse a string to a JSON object.
    /// </summary>
    /// <param name="json">A specific JSON object string to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance; or null, if error format.</returns>
    public static JsonObjectNode TryParse(string json, JsonDocumentOptions options = default)
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
    /// Tries to parse a string to a JSON object.
    /// </summary>
    /// <param name="file">A file with JSON object string content to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance; or null, if error format.</returns>
    public static JsonObjectNode TryParse(FileInfo file, JsonDocumentOptions options = default)
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
    /// <exception cref="JsonException">obj or its property does not represent a valid single JSON object.</exception>
    public static JsonObjectNode ConvertFrom(object obj, JsonSerializerOptions options = default)
    {
        if (obj is null) return null;
        if (obj is IJsonValueNode)
        {
            if (obj is JsonObjectNode jObj) return jObj;
            if (obj is JsonArrayNode jArr)
            {
                var r = new JsonObjectNode();
                var i = 0;
                foreach (var item in jArr)
                {
                    r.Add(i.ToString(), item);
                    i++;
                }

                return r;
            }

            if (obj is JsonNullNode) return null;
            var valueKind = (obj as IJsonValueNode).ValueKind;
            switch (valueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                case JsonValueKind.False:
                    return null;
                case JsonValueKind.True:
                    return new();
            }
        }

        if (obj is JsonDocument doc) return doc;
        if (obj is string str) return Parse(str);
        if (obj is StringBuilder sb) return Parse(sb.ToString());
        if (obj is System.Text.Json.Nodes.JsonObject jo) return jo;
        if (obj is IJsonObjectHost jh) return jh.ToJson();
        if (obj is Stream stream) return Parse(stream);
        if (obj is IEnumerable<KeyValuePair<string, object>> dict)
        {
            var r = new JsonObjectNode();
            foreach (var kvp in dict)
            {
                if (string.IsNullOrWhiteSpace(kvp.Key)) continue;
                r.SetValue(kvp.Key, ConvertFrom(kvp.Value));
            }

            return r;
        }

        if (obj is DBNull) return null;
        if (obj is FileInfo file) return TryParse(file);
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
    public static bool operator ==(JsonObjectNode leftValue, IJsonValueNode rightValue)
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
    public static bool operator !=(JsonObjectNode leftValue, IJsonValueNode rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        if (rightValue is null || leftValue is null) return true;
        return !leftValue.Equals(rightValue);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <param name="root">The root node.</param>
    /// <returns>The value.</returns>
    internal static JsonObjectNode TryGetRefObjectValue(JsonObjectNode source, JsonObjectNode property, JsonObjectNode root)
    {
        if (property == null) return null;
        root ??= source;
        var refPath = property.TryGetStringValue("$ref");
        if (refPath == null) return property;
        if (string.IsNullOrWhiteSpace(refPath) || refPath == "#") return root;
        if (refPath == JsonValues.SELF_REF) return source;
        if (refPath.StartsWith("#/"))
        {
            var path = refPath.Substring(2).Split('/');
            return root?.TryGetObjectValue(path);
        }

        if (refPath == "$") return root;
        if (refPath.StartsWith("./"))
        {
            try
            {
                var file = new FileInfo(refPath);
                return TryParse(file);
            }
            catch (ArgumentException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (IOException)
            {
            }
            catch (ExternalException)
            {
            }

            return property;
        }

        if (refPath.StartsWith("http") && refPath.Contains("://"))
        {
            property = new JsonObjectNode
            {
                { "$ref", refPath }
            };
            _ = TryParse(null, refPath, property);
            return property;
        }

        return root?.TryGetValue(refPath, true) as JsonObjectNode;
    }

    internal static async Task TryParse(JsonHttpClient<JsonObjectNode> http, string url, JsonObjectNode json)
    {
        var resp = await (http ?? new()).GetAsync(url);
        if (json == null) json = new();
        else json.Clear();
        json.SetRange(resp);
    }

    private static JsonObjectNode TryGetObjectValueByProperty(JsonObjectNode json, string key)
    {
        if (json.GetValueKind(key) == JsonValueKind.Array)
        {
            if (!int.TryParse(key, out var i)) return null;
            return json.TryGetArrayValue(key).TryGetObjectValue(i);
        }

        return json.TryGetObjectValue(key);
    }

    private static bool PassTrue(IJsonDataNode data, string key, int index)
    {
        return true;
    }
#pragma warning restore IDE0056,IDE0057
}
