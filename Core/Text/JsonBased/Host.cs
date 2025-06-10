using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
/// <param name="parent">The parent JSON object node.</param>
public class JsonObjectHostService(JsonObjectNode parent) : BaseJsonObjectHostService(parent), IReadOnlyDictionary<string, BaseJsonValueNode>
{
    BaseJsonValueNode IReadOnlyDictionary<string, BaseJsonValueNode>.this[string key] => throw new NotImplementedException();

    /// <summary>
    /// Gets the JSON object source.
    /// </summary>
    public new JsonObjectNode Source => base.Source;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<string> IReadOnlyDictionary<string, BaseJsonValueNode>.Keys => Source.Keys;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IEnumerable<BaseJsonValueNode> IReadOnlyDictionary<string, BaseJsonValueNode>.Values => Source.Values;

    int IReadOnlyCollection<KeyValuePair<string, BaseJsonValueNode>>.Count => Source.Count;

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The result.</returns>
    public new T TryGetValue<T>(string key) where T : class
        => base.TryGetValue(key, false, out T value) ? value : default;

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="reload">true if reload the value of the property; otherwise, false.</param>
    /// <returns>The result.</returns>
    public new T TryGetValue<T>(string key, bool reload) where T : class
        => base.TryGetValue(key, reload, out T value) ? value : default;

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public new bool TryGetValue<T>(string key, out T result) where T : class
        => base.TryGetValue(key, false, default, out result);

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="reload">true if reload the value of the property; otherwise, false.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public new bool TryGetValue<T>(string key, bool reload, out T result) where T : class
        => base.TryGetValue(key, reload, default, out result);

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="reload">true if reload the value of the property; otherwise, false.</param>
    /// <param name="defaultValue">The default value for null.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    public new bool TryGetValue<T>(string key, bool reload, T defaultValue, out T result) where T : class
        => base.TryGetValue(key, reload, defaultValue, out result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>A string.</returns>
    public new string TryGetStringValue(string key)
        => Source.TryGetStringValue(key);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strictMode">true if enable strict mode; otherwise, false, to return undefined for non-existing.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public new bool TryGetStringValue(string key, bool strictMode, out string result)
        => Source.TryGetStringValue(key, strictMode, out result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public new bool TryGetStringValue(string key, out string result)
        => Source.TryGetStringValue(key, false, out result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="returnNullIfEmpty">true if returns null when the value is empty or white space; otherwise, false.</param>
    /// <returns>A string trimmed.</returns>
    public new string TryGetStringTrimmedValue(string key, bool returnNullIfEmpty = false)
        => Source.TryGetStringTrimmedValue(key, returnNullIfEmpty);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="valueCase">The case.</param>
    /// <param name="invariant">true if uses the casing rules of invariant culture; otherwise, false.</param>
    /// <param name="returnNullIfEmpty">true if returns null when the value is empty or white space; otherwise, false.</param>
    /// <returns>A string trimmed.</returns>
    public new string TryGetStringTrimmedValue(string key, Cases valueCase, bool invariant = false, bool returnNullIfEmpty = false)
        => Source.TryGetStringTrimmedValue(key, valueCase, invariant, returnNullIfEmpty);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result trimmed.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    public new bool TryGetStringTrimmedValue(string key, out string result)
        => Source.TryGetStringTrimmedValue(key, out result);

    /// <summary>
    /// Synchronizes back to parent node.
    /// </summary>
    /// <returns>The properties synchronized.</returns>
    public new int SyncToParent()
        => base.SyncToParent();

    /// <summary>
    /// Clears cache.
    /// </summary>
    public new void ClearCache()
        => base.ClearCache();

    /// <summary>
    /// Resets the parent JSON object node.
    /// </summary>
    /// <param name="parent">The parent JSON object node.</param>
    /// <param name="keepCache">true if need keeping the cache; otherwise, false.</param>
    public new void ResetParent(JsonObjectNode parent, bool keepCache = false)
        => base.ResetParent(parent, keepCache);

    /// <summary>
    /// Tries to load from a file.
    /// </summary>
    /// <param name="file">A file with JSON object string content to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>true if load succeded; otherwise, false.</returns>
    public new bool TryLoadFile(FileInfo file, JsonDocumentOptions options = default)
        => base.TryLoadFile(file, options);

    /// <summary>
    /// Tries to load from a file.
    /// </summary>
    /// <param name="path">The file path with JSON object string content to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>true if load succeded; otherwise, false.</returns>
    public new bool TryLoadFile(string path, JsonDocumentOptions options = default)
        => base.TryLoadFile(path, options);

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
    public new void WriteTo(string path, IndentStyles style = IndentStyles.Minified)
        => base.WriteTo(path, style);

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
    public new void WriteTo(FileInfo file, IndentStyles style = IndentStyles.Minified)
        => base.WriteTo(file, style);

    /// <summary>
    /// Tries to write to a file.
    /// </summary>
    /// <param name="path">The path of the file. If the target file already exists, it is overwritten.</param>
    /// <param name="style">The indent style.</param>
    /// <returns>true if write succeeded; otherwise, false.</returns>
    public new bool TryWriteTo(string path, IndentStyles style = IndentStyles.Minified)
        => base.TryWriteTo(path, style);

    /// <summary>
    /// Tries to write to a file.
    /// </summary>
    /// <param name="file">The file to save.</param>
    /// <param name="style">The indent style.</param>
    /// <returns>true if write succeeded; otherwise, false.</returns>
    public new bool TryWriteTo(FileInfo file, IndentStyles style = IndentStyles.Minified)
        => base.TryWriteTo(file, style);

    bool IReadOnlyDictionary<string, BaseJsonValueNode>.ContainsKey(string key)
        => Source.ContainsKey(key);

    IEnumerator<KeyValuePair<string, BaseJsonValueNode>> IEnumerable<KeyValuePair<string, BaseJsonValueNode>>.GetEnumerator()
        => Source.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => Source.GetEnumerator();

    bool IReadOnlyDictionary<string, BaseJsonValueNode>.TryGetValue(string key, out BaseJsonValueNode value)
        => Source.TryGetValue(key, out value);
}
