using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Reflection;

namespace Trivial.Collection;

/// <summary>
/// The mapping for keyed data.
/// </summary>
/// <typeparam name="T">The type of value.</typeparam>
public class KeyedDataMapping<T> : IDictionary<string, T>
{
    /// <summary>
    /// The mapping Collection.
    /// </summary>
    private readonly Dictionary<string, T> mapping = new();

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The case-insensitive key.</param>
    /// <returns>The value associated with the specified key.</returns>
    /// <exception cref="ArgumentNullException">key was null.</exception>
    /// <exception cref="ArgumentException">key was empty.</exception>
    /// <exception cref="KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
    public T this[string key]
    {
        get
        {
            if (key == null) throw ObjectConvert.ArgumentNull(nameof(key));
            key = key.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("key should not be empty.", nameof(key));
            if (mapping.TryGetValue(key, out var r)) return r;
            var h = BackupGetter;
            if (h != null && h.Invoke(key, out r))
            {
                try
                {
                    if (r is not null && !IsAutoCacheDisabled) mapping[key] = r;
                }
                catch (InvalidOperationException)
                {
                }
                catch (NullReferenceException)
                {
                }

                return r;
            }

            return mapping[key];
        }

        set
        {
            if (key == null) throw ObjectConvert.ArgumentNull(nameof(key));
            key = key.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(key) || value is null) return;
            try
            {
                if (value is null) mapping.Remove(key);
                else mapping[key] = value;
                return;
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }

            if (value is null) mapping.Remove(key);
            else mapping[key] = value;
        }
    }

    /// <summary>
    /// Gets or sets an hanlder for backup getter.
    /// </summary>
    public KeyedInstanceResolver<T> BackupGetter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether auto Collection for backup getter result is disabled.
    /// </summary>
    public bool IsAutoCacheDisabled { get; set; }

    /// <summary>
    /// Gets a collection containing the case-insensitive keys in the mapping.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<string> Keys => mapping.Keys;

    /// <summary>
    /// Gets a collection containing the values in the mapping.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<T> Values => mapping.Values;

    /// <summary>
    /// Gets the number of key/value pairs contained in the mapping.
    /// </summary>
    public int Count => mapping.Count;

    /// <summary>
    /// Gets a value indicating whether the mapping is read-only.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<KeyValuePair<string, T>>.IsReadOnly => false;

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    /// <param name="key">The case-insensitive key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <exception cref="ArgumentException">An element with the same key already exists in the mapping.</exception>
    public void Add(string key, T value)
    {
        key = key?.Trim()?.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(key)) return;
        mapping.Add(key, value);
    }

    /// <summary>
    /// Adds the specified key and value to the dictionary.
    /// </summary>
    /// <param name="item">The key value pair to add.</param>
    /// <exception cref="ArgumentException">An element with the same key already exists in the mapping.</exception>
    public void Add(KeyValuePair<string, T> item)
    {
        Add(item.Key, item.Value);
    }

    /// <summary>
    /// Removes all keys and values from the mapping.
    /// </summary>
    void ICollection<KeyValuePair<string, T>>.Clear()
    {
        mapping.Clear();
    }

    /// <summary>
    /// Removes all keys and values from the mapping.
    /// </summary>
    /// <returns>The rows removed.</returns>
    public int Clear()
    {
        var i = mapping.Count;
        mapping.Clear();
        return i;
    }

    /// <summary>
    /// Determines whether the mapping contains the specified key.
    /// </summary>
    /// <param name="item">The item to test.</param>
    /// <returns>true if the mapping contains the specific element; otherwise, false.</returns>
    public bool Contains(KeyValuePair<string, T> item)
    {
        if (item.Key == null) return false;
        var key = item.Key.Trim().ToLowerInvariant();
        if (!mapping.TryGetValue(key, out T value)) return false;
        return value is null || value.Equals(item.Value);
    }

    /// <summary>
    /// Determines whether the mapping contains the specified key.
    /// </summary>
    /// <param name="key">The case-insensitive key to locate in the mapping.</param>
    /// <returns>true if the mapping contains an element with the specified key; otherwise, false.</returns>
    public bool ContainsKey(string key)
    {
        key = key?.Trim()?.ToLowerInvariant();
        if (string.IsNullOrEmpty(key)) return false;
        return mapping.ContainsKey(key);
    }

    /// <summary>
    /// Determines whether the mapping contains the specified value.
    /// </summary>
    /// <param name="value">The value to locate in the mapping.</param>
    /// <returns>true if the mapping contains an element with the specified value; otherwise, false.</returns>
    public bool ContainsValue(T value)
    {
        return mapping.ContainsValue(value);
    }

    /// <summary>
    /// Copies the elements of the mapping to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the elements copied from mapping. The array must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentNullException">array is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
    /// <exception cref="ArgumentException">The number of elements in the source is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
    {
        (mapping as ICollection<KeyValuePair<string, T>>).CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the mapping.
    /// </summary>
    /// <returns>An enumerator structure for the mapping.</returns>
    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return mapping.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the mapping.
    /// </summary>
    /// <returns>An enumerator structure for the mapping.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)mapping).GetEnumerator();
    }

    /// <summary>
    /// Removes the value with the specified key from the mapping.
    /// </summary>
    /// <param name="key">The case-insensitive key of the element to remove.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(string key)
    {
        if (key == null) return false;
        try
        {
            return mapping.Remove(key.Trim().ToLowerInvariant());
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }

        return mapping.Remove(key.Trim().ToLowerInvariant());
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Removes the value with the specified key from the mapping and copies the element to the value parameter..
    /// </summary>
    /// <param name="key">The case-insensitive key of the element to remove.</param>
    /// <param name="value">The removed element.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(string key, out T value)
    {
        if (key == null)
        {
            value = default;
            return false;
        }

        return mapping.Remove(key.Trim().ToLowerInvariant(), out value);
    }
#endif

    /// <summary>
    /// Removes the value with the specified key from the mapping.
    /// </summary>
    /// <param name="item">The key value pair to remove.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(KeyValuePair<string, T> item)
    {
        if (item.Key == null) return false;
        var key = item.Key.Trim().ToLowerInvariant();
        if (!mapping.TryGetValue(key, out T value)) return false;
        if (value is not null && !value.Equals(item.Value)) return false;
        mapping.Remove(key);
        return true;
    }

    /// <summary>
    /// Removes the value with the specified key from the mapping.
    /// </summary>
    /// <param name="keys">All specific case-insensitive keys to remove.</param>
    /// <returns>The count of rows removed.</returns>
    public int Remove(IEnumerable<string> keys)
    {
        var i = 0;
        if (keys == null) return i;
        foreach (var item in keys)
        {
            var k = item?.Trim()?.ToLowerInvariant();
            if (string.IsNullOrEmpty(k)) continue;
            try
            {
                if (mapping.Remove(k)) i++;
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        return i;
    }

    /// <summary>
    /// Sets a value.
    /// </summary>
    /// <param name="key">The case-insensitive key of the element to set.</param>
    /// <param name="value">The element.</param>
    /// <param name="overrideIfExist">true if override the existed one; otherwise, false.</param>
    /// <returns>The count of rows added or changed.</returns>
    public bool Set(string key, T value, bool overrideIfExist = false)
    {
        key = key?.Trim()?.ToLowerInvariant();
        if (string.IsNullOrEmpty(key)) return false;
        try
        {
            if (!overrideIfExist && mapping.ContainsKey(key)) return false;
            if (value is null) mapping.Remove(key);
            else mapping[key] = value;
            return true;
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }

        if (!overrideIfExist && mapping.ContainsKey(key)) return false;
        if (value is null) mapping.Remove(key);
        else mapping[key] = value;
        return true;
    }

    /// <summary>
    /// Sets values batched.
    /// </summary>
    /// <param name="value">The values.</param>
    /// <param name="overrideIfExist">true if override the existed one; otherwise, false.</param>
    /// <returns>The count of rows added or changed.</returns>
    public int Set(IDictionary<string, T> value, bool overrideIfExist = false)
    {
        var i = 0;
        if (value == null) return i;
        foreach (var p in value)
        {
            var k = p.Key?.Trim()?.ToLowerInvariant();
            if (string.IsNullOrEmpty(k) || p.Value is null) continue;
            try
            {
                if (!overrideIfExist && mapping.ContainsKey(k)) continue;
                mapping[k] = p.Value;
                i++;
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        return i;
    }

    /// <summary>
    /// Tries to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The case-insensitive key whose value to get.</param>
    /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if the object that implements mapping contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(string key, out T value)
    {
        key = key?.Trim()?.ToLowerInvariant();
        if (string.IsNullOrEmpty(key))
        {
            value = default;
            return false;
        }

        var result = true;
        try
        {
            result = mapping.TryGetValue(key, out value);
            if (result) return true;
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }

        try
        {
            if (result)
            {
                result = mapping.TryGetValue(key, out value);
                if (result) return true;
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }

        try
        {
            if (result)
            {
                result = mapping.TryGetValue(key, out value);
                if (result) return true;
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }

        var h = BackupGetter;
        if (!result && h != null && h.Invoke(key, out value))
        {
            try
            {
                if (value is not null && !IsAutoCacheDisabled) mapping[key] = value;
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }

            return true;
        }

        value = default;
        return false;
    }
}
