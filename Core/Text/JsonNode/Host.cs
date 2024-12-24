using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.IO;

namespace Trivial.Text;

/// <summary>
/// The host service for JSON object node to provide a way to access its JSON properties.
/// </summary>
public class JsonObjectHostService : IJsonObjectHost, IReadOnlyDictionary<string, BaseJsonValueNode>
{
    private readonly Dictionary<string, object> cache = new();

    /// <summary>
    /// Initializes a new instance of the JsonObjectHostService class.
    /// </summary>
    /// <param name="parent">The parent JSON object node.</param>
    public JsonObjectHostService(JsonObjectNode parent)
    {
        Parent = parent ?? new();
        parent.PropertyChanged += OnPropertyChanged;
    }

    BaseJsonValueNode IReadOnlyDictionary<string, BaseJsonValueNode>.this[string key] => throw new NotImplementedException();

    /// <summary>
    /// Gets the parent JSON object.
    /// </summary>
    public JsonObjectNode Parent { get; private set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<string> IReadOnlyDictionary<string, BaseJsonValueNode>.Keys => Parent.Keys;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<BaseJsonValueNode> IReadOnlyDictionary<string, BaseJsonValueNode>.Values => Parent.Values;

    int IReadOnlyCollection<KeyValuePair<string, BaseJsonValueNode>>.Count => Parent.Count;

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The result.</returns>
    public T TryGetValue<T>(string key) where T : class
        => TryGetValue(key, false, out T value) ? value : default;

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="reload">true if reload the value of the property; otherwise, false.</param>
    /// <returns>The result.</returns>
    public T TryGetValue<T>(string key, bool reload) where T : class
        => TryGetValue(key, reload, out T value) ? value : default;

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetValue<T>(string key, out T result) where T : class
        => TryGetValue(key, false, default, out result);

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="reload">true if reload the value of the property; otherwise, false.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetValue<T>(string key, bool reload, out T result) where T : class
        => TryGetValue(key, reload, default, out result);

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="reload">true if reload the value of the property; otherwise, false.</param>
    /// <param name="defaultValue">The default value for null.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public bool TryGetValue<T>(string key, bool reload, T defaultValue, out T result) where T : class
    {
        try
        {
            if (!reload && cache.TryGetValue(key, out var value))
            {
                if (value is T v)
                {
                    result = v;
                    return true;
                }

                if (value is null)
                {
                    result = defaultValue;
                    return true;
                }
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (ExternalException)
        {
        }

        var json = Parent.TryGetObjectValue(key);
        if (json is null)
        {
            if (reload) cache[key] = defaultValue;
            result = defaultValue;
            return true;
        }

        try
        {
            var v = CreateObject<T>(json);
            cache[key] = v;
            result = v;
            return true;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (InvalidCastException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (JsonException)
        {
        }
        catch (ApplicationException)
        {
        }
        catch (ExternalException)
        {
        }

        result = defaultValue;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>A string.</returns>
    public string TryGetStringValue(string key)
        => Parent.TryGetStringValue(key);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strictMode">true if enable strict mode; otherwise, false, to return undefined for non-existing.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringValue(string key, bool strictMode, out string result)
        => Parent.TryGetStringValue(key, strictMode, out result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringValue(string key, out string result)
        => Parent.TryGetStringValue(key, false, out result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="returnNullIfEmpty">true if returns null when the value is empty or white space; otherwise, false.</param>
    /// <returns>A string trimmed.</returns>
    public string TryGetStringTrimmedValue(string key, bool returnNullIfEmpty = false)
        => Parent.TryGetStringTrimmedValue(key, returnNullIfEmpty);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="valueCase">The case.</param>
    /// <param name="invariant">true if uses the casing rules of invariant culture; otherwise, false.</param>
    /// <param name="returnNullIfEmpty">true if returns null when the value is empty or white space; otherwise, false.</param>
    /// <returns>A string trimmed.</returns>
    public string TryGetStringTrimmedValue(string key, Cases valueCase, bool invariant = false, bool returnNullIfEmpty = false)
        => Parent.TryGetStringTrimmedValue(key, valueCase, invariant, returnNullIfEmpty);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result trimmed.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public bool TryGetStringTrimmedValue(string key, out string result)
        => Parent.TryGetStringTrimmedValue(key, out result);

    /// <summary>
    /// Synchronizes back to parent node.
    /// </summary>
    /// <returns>The properties synchronized.</returns>
    public int SyncToParent()
    {
        var col = new Dictionary<string, object>(cache);
        foreach (var item in col)
        {
            var json = JsonObjectNode.ConvertFrom(item.Value);
            Parent.SetValue(item.Key, json);
            cache[item.Key] = item.Value;
        }

        return col.Count;
    }

    /// <summary>
    /// Clears cache.
    /// </summary>
    public void ClearCache()
        => cache.Clear();

    /// <summary>
    /// Resets the parent JSON object node.
    /// </summary>
    /// <param name="parent">The parent JSON object node.</param>
    /// <param name="keepCache">true if need keeping the cache; otherwise, false.</param>
    public void ResetParent(JsonObjectNode parent, bool keepCache = false)
    {
        var oldParent = Parent;
        if (ReferenceEquals(parent, oldParent))
        {
            if (keepCache) SyncToParent();
            return;
        }

        if (oldParent != null) oldParent.PropertyChanged -= OnPropertyChanged;
        parent ??= new();
        Parent = parent;
        if (keepCache) SyncToParent();
        else cache.Clear();
        parent.PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Tries to load from a file.
    /// </summary>
    /// <param name="file">A file with JSON object string content to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>true if load succeded; otherwise, false.</returns>
    public bool TryLoadFile(FileInfo file, JsonDocumentOptions options = default)
    {
        var json = JsonObjectNode.TryParse(file, options);
        if (json == null) return false;
        ResetParent(json);
        return true;
    }

    /// <summary>
    /// Tries to load from a file.
    /// </summary>
    /// <param name="path">The file path with JSON object string content to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>true if load succeded; otherwise, false.</returns>
    public bool TryLoadFile(string path, JsonDocumentOptions options = default)
    {
        var file = FileSystemInfoUtility.TryGetFileInfo(path);
        return TryLoadFile(file, options);
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
    {
        SyncToParent();
        Parent.WriteTo(path, style);
    }

    /// <summary>
    /// Writes to file.
    /// </summary>
    /// <param name="file">The file to write.</param>
    /// <param name="style">The indent style.</param>
    /// <exception cref="IOException">IO exception.</exception>
    /// <exception cref="SecurityException">Write failed because of security exception.</exception>
    /// <exception cref="ArgumentNullException">The file path was null.</exception>
    /// <exception cref="NotSupportedException">The file path was not supported.</exception>
    /// <exception cref="UnauthorizedAccessException">Write failed because of unauthorized access exception.</exception>
    public void WriteTo(FileInfo file, IndentStyles style = IndentStyles.Minified)
    {
        SyncToParent();
        Parent.WriteTo(file, style);
    }

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
            SyncToParent();
            Parent.WriteTo(path, style);
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
            SyncToParent();
            Parent.WriteTo(file, style);
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
    /// Creates the object from a JSON object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    protected virtual T CreateObject<T>(JsonObjectNode json)
    {
        if (json == null) return default;
        var type = typeof(T);
        if (type.IsInterface) throw new NotSupportedException();
        var constructor = type.GetConstructor(new[] { typeof(JsonObjectNode) });
        if (constructor != null) return (T)constructor.Invoke(new[] { json });
        return json.Deserialize<T>();
    }

    bool IReadOnlyDictionary<string, BaseJsonValueNode>.ContainsKey(string key)
        => Parent.ContainsKey(key);

    IEnumerator<KeyValuePair<string, BaseJsonValueNode>> IEnumerable<KeyValuePair<string, BaseJsonValueNode>>.GetEnumerator()
        => Parent.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => Parent.GetEnumerator();

    /// <summary>
    /// The host for JSON object node.
    /// </summary>
    JsonObjectNode IJsonObjectHost.ToJson()
        => Parent;

    bool IReadOnlyDictionary<string, BaseJsonValueNode>.TryGetValue(string key, out BaseJsonValueNode value)
        => Parent.TryGetValue(key, out value);

    private void OnPropertyChanged(object sender, KeyValueEventArgs<string, BaseJsonValueNode> e)
    {
        var key = e.Key;
        if (string.IsNullOrEmpty(key)) return;
        cache.Remove(key);
    }
}
