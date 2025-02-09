using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Trivial.CommandLine;
using Trivial.Data;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Collection;

/// <summary>
/// The list utility.
/// </summary>
public static partial class ListExtensions
{
    /// <summary>
    /// Tries to get the string value.
    /// </summary>
    /// <param name="dict">The source dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value output.</param>
    /// <returns>true if exist and not empty; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">dict or key was null.</exception>
    public static bool TryGetNotEmptyValue(this IDictionary<string, string> dict, string key, out string value)
    {
        if (dict == null) throw ObjectConvert.ArgumentNull(nameof(dict));
        value = dict.TryGetValue(key, out var v) && !string.IsNullOrEmpty(v) ? v : null;
        return v != null;
    }

    /// <summary>
    /// Tries to get the string value.
    /// </summary>
    /// <param name="dict">The source dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns>The value; or null if not exist or empty.</returns>
    /// <exception cref="ArgumentNullException">dict or key was null.</exception>
    public static string TryGetNotEmptyValue(this IDictionary<string, string> dict, string key)
    {
        if (dict == null) throw ObjectConvert.ArgumentNull(nameof(dict));
        return dict.TryGetValue(key, out var v) && !string.IsNullOrEmpty(v) ? v : null;
    }

    /// <summary>
    /// Adds a key and a value to the end of the key value pairs.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="clearOthers">true if clear the others of the property before adding; otherwise, false.</param>
    public static void Add<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, TKey key, TValue value, bool clearOthers = false)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        if (clearOthers) list.Remove(key);
        list.Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    /// <summary>
    /// Adds a console text.
    /// </summary>
    /// <param name="list">The console text collection.</param>
    /// <param name="s">The string value.</param>
    /// <param name="style">The style.</param>
    public static void Add(this IList<ConsoleText> list, string s, ConsoleTextStyle style = null)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        list.Add(new ConsoleText(s, style));
    }

    /// <summary>
    /// Adds a console text.
    /// </summary>
    /// <param name="list">The console text collection.</param>
    /// <param name="s">The string value.</param>
    /// <param name="style">The style.</param>
    public static void Add(this IList<ConsoleText> list, StringBuilder s, ConsoleTextStyle style = null)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        list.Add(new ConsoleText(s, style));
    }

    /// <summary>
    /// Adds a console text.
    /// </summary>
    /// <param name="list">The console text collection.</param>
    /// <param name="c">The character.</param>
    /// <param name="repeatCount">The number of times to append value.</param>
    /// <param name="style">The style.</param>
    public static void Add(this IList<ConsoleText> list, char c, int repeatCount = 1, ConsoleTextStyle style = null)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        list.Add(new ConsoleText(c, repeatCount, style));
    }

    /// <summary>
    /// Inserts an element into the key value pairs at the specified index.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public static void Insert<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index, TKey key, TValue value)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        list.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate that the element is not null.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">A collection to filter.</param>
    /// <returns>A collection that contains elements from the input sequence that satisfy the not null condition.</returns>
    public static IEnumerable<T> WhereNotNull<T>(IEnumerable<T> source)
        => source?.Where(IsNotNull);

    /// <summary>
    /// Filters a sequence of values based on a predicate that the element is not null or empty.
    /// </summary>
    /// <param name="source">A collection to filter.</param>
    /// <returns>A collection that contains elements from the input sequence that satisfy the not empty condition.</returns>
    public static IEnumerable<string> WhereNotNullOrEmpty(this IEnumerable<string> source)
        => source?.Where(IsNotNullOrEmpty);

    /// <summary>
    /// Filters a sequence of values based on a predicate that the element is not null, empty or consists only of white-space characters.
    /// </summary>
    /// <param name="source">A collection to filter.</param>
    /// <returns>A collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<string> WhereNotNullOrWhiteSpace(this IEnumerable<string> source)
        => source?.Where(IsNotNullOrWhiteSpace);

    /// <summary>
    /// Creates a new list to copy the input collection and insert a specific number of default value at beginning.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="col">The input collection.</param>
    /// <param name="count">The count of padding item.</param>
    /// <param name="value">The default value.</param>
    /// <returns>A new list with padding and the input collection.</returns>
    public static List<T> PadBegin<T>(IEnumerable<T> col, int count, T value = default)
    {
        var list = new List<T>();
        if (count < 0)
        {
            if (col != null) list.AddRange(col.Skip(count));
            return list;
        }

        for (var i = 0; i < count; i++)
        {
            list.Add(value);
        }

        if (col != null) list.AddRange(col);
        return list;
    }

    /// <summary>
    /// Creates a new list to copy the input collection and insert a specific number of default value at ending.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="col">The input collection.</param>
    /// <param name="count">The count of padding item.</param>
    /// <param name="value">The default value.</param>
    /// <returns>A new list with padding and the input collection.</returns>
    public static List<T> PadEnd<T>(IEnumerable<T> col, int count, T value = default)
    {
        var list = col == null ? new List<T>() : new List<T>(col);
        if (count < 0)
        {
            if (col == null) return list;
            list.AddRange(col);
            for (var i = 0; i < count; i++)
            {
                var len = list.Count - 1;
                if (len < 0) break;
                list.RemoveAt(len);
            }

            return list;
        }

        for (var i = 0; i < count; i++)
        {
            list.Add(value);
        }

        return list;
    }

    /// <summary>
    /// Insert a specific number of default value at beginning to the input collection.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="col">The input collection.</param>
    /// <param name="count">The count of padding item.</param>
    /// <param name="value">The default value.</param>
    /// <returns>A new list with padding and the input collection.</returns>
    /// <exception cref="ArgumentNullException">col is null.</exception>
    public static void PadBeginTo<T>(IList<T> col, int count, T value = default)
    {
        if (col == null) throw ObjectConvert.ArgumentNull(nameof(col));
        if (count < 0)
        {
            for (var i = 0; i < count; i++)
            {
                if (col.Count < 1) return;
                col.RemoveAt(0);
            }

            return;
        }

        for (var i = 0; i < count; i++)
        {
            col.Insert(0, value);
        }
    }

    /// <summary>
    /// Insert a specific number of default value at ending to the input collection.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="col">The input collection.</param>
    /// <param name="count">The count of padding item.</param>
    /// <param name="value">The default value.</param>
    /// <returns>A new list with padding and the input collection.</returns>
    /// <exception cref="ArgumentNullException">col is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">count is less than 0.</exception>
    public static void PadEndTo<T>(ICollection<T> col, int count, T value = default)
    {
        if (col == null) throw ObjectConvert.ArgumentNull(nameof(col));
        if (count < 0)
        {
            if (col is not IList<T> list) throw new ArgumentOutOfRangeException(nameof(count), "count should be a natural number.");
            for (var i = 0; i < count; i++)
            {
                var len = col.Count - 1;
                if (len < 0) break;
                list.RemoveAt(len);
            }

            return;
        }

        for (var i = 0; i < count; i++)
        {
            col.Add(value);
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="trueValue">The value for true.</param>
    /// <param name="falseValue">The value for false.</param>
    /// <returns>The string collection converted.</returns>
    public static IEnumerable<T> Select<T>(IEnumerable<bool> input, T trueValue, T falseValue)
    {
        if (input == null) yield break;
        foreach (var item in input)
        {
            yield return item ? trueValue : falseValue;
        }
    }

    /// <summary>
    /// Converts a collection of boolean to strings.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <param name="trueValue">The value for true.</param>
    /// <param name="falseValue">The value for false.</param>
    /// <returns>The string collection converted.</returns>
    public static T[] Select<T>(bool[] input, T trueValue, T falseValue)
    {
        if (input == null) return null;
        var arr = new T[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            arr[i] = input[i] ? trueValue : falseValue;
        }

        return arr;
    }

    /// <summary>
    /// Unions the property values into the specific source collection.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="json">The JSON object to get data.</param>
    /// <param name="key">The property key.</param>
    /// <param name="ignoreNotMatched">true if ignore any item which is not JSON string; otherwise, false.</param>
    /// <returns>The list; or null, if no such array property.</returns>
    public static IEnumerable<string> Union(IEnumerable<string> source, JsonObjectNode json, string key, bool ignoreNotMatched)
    {
        if (source != null)
        {
            foreach (var element in source)
            {
                yield return element;
            }
        }

        var arr = json?.TryGetStringListValue(key, ignoreNotMatched);
        if (arr == null) yield break;
        foreach (var element in arr)
        {
            yield return element;
        }
    }

    /// <summary>
    /// Gets the keys.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <returns>The keys.</returns>
    public static IEnumerable<TKey> Keys<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        return list.Select(item => item.Key).Distinct();
    }

    /// <summary>
    /// Gets the values by a specific key.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key.</param>
    /// <returns>The values.</returns>
    public static IEnumerable<TValue> GetValues<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        return key != null ? list.Where(item => key.Equals(item.Key)).Select(item => item.Value) : list.Where(item => item.Key == null).Select(item => item.Value);
    }

    /// <summary>
    /// Gets the value items by a specific key.
    /// </summary>
    /// <param name="list">The key value items pairs.</param>
    /// <param name="key">The key.</param>
    /// <returns>The value items.</returns>
    public static IEnumerable<TValue> GetValueItems<TKey, TValue, TList>(this IEnumerable<KeyValuePair<TKey, TList>> list, TKey key) where TList : IEnumerable<TValue>
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        return key != null ? list.Where(item => key.Equals(item.Key)).SelectMany(item => item.Value) : list.Where(item => item.Key == null).SelectMany(item => item.Value);
    }

    /// <summary>
    /// Tries to get the value of the specific key.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key.</param>
    /// <param name="index">The index of the value.</param>
    /// <param name="value">The value output.</param>
    /// <returns>true if has; otherwise, false.</returns>
    public static bool TryGetValue<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, TKey key, int index, out TValue value)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        var col = list.GetValues(key).ToList();
        if (list.Count <= index)
        {
            value = default;
            return false;
        }

        value = col[index];
        return true;
    }

    /// <summary>
    /// Gets the value by a specific key.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key.</param>
    /// <param name="index">The index of the value for multiple values.</param>
    /// <returns>The value. The first one for multiple values.</returns>
    /// <exception cref="IndexOutOfRangeException">index is less than 0, or is equals to or greater than the length of the values of the specific key.</exception>
    public static TValue GetValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key, int index)
        => GetValues(list, key).ToList()[index];

    /// <summary>
    /// Gets the value by a specific key.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key.</param>
    /// <returns>The value. The first one for multiple values.</returns>
    /// <exception cref="IndexOutOfRangeException">index is less than 0, or is equals to or greater than the length of the values of the specific key.</exception>
    public static IGrouping<TKey, TValue> Get<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        list = key == null ? list.Where(item => item.Key == null) : list.Where(item => key.Equals(item.Key));
        return list.GroupBy(item => item.Key, item => item.Value).SingleOrDefault();
    }

    /// <summary>
    /// Determines whether the instance contains the specified
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key to locate in the instance.</param>
    /// <returns>true if the instance contains an element with the specified key; otherwise, false.</returns>
    public static bool ContainsKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        if (key == null)
        {
            foreach (var item in list)
            {
                if (item.Key == null) return true;
            }
        }
        else
        {
            foreach (var item in list)
            {
                if (key.Equals(item.Key)) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes all the elements by the specific key.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="keys">The keys to remove.</param>
    /// <returns>The number of elements removed from the key value pairs.</returns>
    public static int Remove<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, params TKey[] keys)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        var count = 0;
        foreach (var key in keys)
        {
            count = key == null ? list.RemoveAll(item => item.Key == null) : list.RemoveAll(item => key.Equals(item.Key));
        }

        return count;
    }

    /// <summary>
    /// Removes all the elements by the specific key.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="keys">The keys to remove.</param>
    /// <returns>The number of elements removed from the key value pairs.</returns>
    public static int Remove<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, params ReadOnlySpan<TKey> keys)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        var count = 0;
        foreach (var key in keys)
        {
            count = key == null ? list.RemoveAll(item => item.Key == null) : list.RemoveAll(item => key.Equals(item.Key));
        }

        return count;
    }

    /// <summary>
    /// Searches for the specified key and returns the zero-based index of the last occurrence within the entire key value pairs.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key.</param>
    public static int LastIndexOf<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        return IndexOf(list.Reverse(), key);
    }

    /// <summary>
    /// Searches for the specified key and returns the zero-based index of the first occurrence within the entire key value pairs.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key.</param>
    public static int IndexOf<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        var i = -1;
        if (key == null)
        {
            foreach (var item in list)
            {
                i++;
                if (item.Key == null) return i;
            }
        }
        else
        {
            foreach (var item in list)
            {
                i++;
                if (key.Equals(item.Key)) return i;
            }
        }

        return i;
    }

    /// <summary>
    /// Returns the first element of a sequence, or a null if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="col">The collection to return the first element of.</param>
    /// <returns>The first element in source; or null, if empty.</returns>
    public static T? FirstOrNull<T>(IEnumerable<T> col) where T : struct
    {
        if (col is null) return null;
        foreach (var item in col)
        {
            return item;
        }

        return null;
    }

    /// <summary>
    /// Returns the first element of a sequence, or a null if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="col">The collection to return the first element of.</param>
    /// <param name="defaultValue">The default value used when no element.</param>
    /// <returns>The first element in source; or null, if empty.</returns>
    public static T First<T>(IEnumerable<T> col, T defaultValue) where T : struct
        => FirstOrNull(col) ?? defaultValue;

    /// <summary>
    /// Returns the last element of a sequence, or a null if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="col">The collection to return the last element of.</param>
    /// <returns>The last element in source; or null, if empty.</returns>
    public static T? LastOrNull<T>(IEnumerable<T> col) where T : struct
    {
        if (col is null) return null;
        if (col is ICollection<T> col2) return col2.Count > 0 ? col2.Last() : null;
        if (col is T[] col3) return col3.Length > 0 ? col3.Last() : null;
        col = col.Reverse();
        foreach (var item in col)
        {
            return item;
        }

        return null;
    }

    /// <summary>
    /// Returns the last element of a sequence, or a null if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="col">The collection to return the last element of.</param>
    /// <param name="defaultValue">The default value used when no element.</param>
    /// <returns>The last element in source; or null, if empty.</returns>
    public static T Last<T>(IEnumerable<T> col, T defaultValue) where T : struct
        => LastOrNull(col) ?? defaultValue;

    /// <summary>
    /// Searches for the specified object and returns the zero-based index array of the all occurrence within the entire key value pairs.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="test">The object to test.</param>
    /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
    /// <param name="count">The number of elements in the section to search.</param>
    public static IEnumerable<int> AllIndexesOf<T>(this IEnumerable<T> list, T test, int index = 0, int? count = null)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        if (index > 0) list = list.Skip(index);
        if (count.HasValue) list = list.Take(count.Value);
        var i = -1;
        if (test == null)
        {
            foreach (var item in list)
            {
                i++;
                if (item == null) yield return i;
            }
        }
        else
        {
            foreach (var item in list)
            {
                i++;
                if (test.Equals(item)) yield return i;
            }
        }
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index array of the all occurrence within the entire key value pairs.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="test">The function to test.</param>
    /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
    /// <param name="count">The number of elements in the section to search.</param>
    public static IEnumerable<int> AllIndexesOf<T>(this IEnumerable<T> list, Func<T, bool> test, int index = 0, int? count = null)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        if (test == null) throw ObjectConvert.ArgumentNull(nameof(test));
        if (index > 0) list = list.Skip(index);
        if (count.HasValue) list = list.Take(count.Value);
        var i = -1;
        foreach (var item in list)
        {
            i++;
            if (test(item)) yield return i;
        }
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index array of the all occurrence within the entire key value pairs.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key to test.</param>
    public static IEnumerable<int> AllIndexesOf<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        var i = -1;
        if (key == null)
        {
            foreach (var item in list)
            {
                i++;
                if (item.Key == null) yield return i;
            }
        }
        else
        {
            foreach (var item in list)
            {
                i++;
                if (key.Equals(item.Key)) yield return i;
            }
        }
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index array of the all occurrence within the entire key value pairs.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key to test.</param>
    /// <param name="value">The value to test.</param>
    /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
    /// <param name="count">The number of elements in the section to search.</param>
    public static IEnumerable<int> AllIndexesOf<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key, TValue value, int index = 0, int? count = null)
    {
        Func<KeyValuePair<TKey, TValue>, bool> test;
        if (key == null && value == null) test = item => item.Key == null && item.Value == null;
        else if (key != null && value == null) test = item => key.Equals(item.Key) && item.Value == null;
        else if (key == null && value != null) test = item => item.Key == null && value.Equals(item.Value);
        else test = item => key.Equals(item.Key) && value.Equals(item.Value);
        return AllIndexesOf(list, test, index, count);
    }

    /// <summary>
    /// Sets a key value.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="insertAtLast">true if insert at last; otherwise, false.</param>
    public static void Set<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, TKey key, TValue value, bool insertAtLast = false)
    {
        if (list == null) throw ObjectConvert.ArgumentNull(nameof(list));
        var i = insertAtLast ? -1 : list.IndexOf(key);
        if (i >= 0)
        {
            Remove(list, key);
            Insert(list, i, key, value);
        }
        else
        {
            Add(list, key, value, true);
        }
    }

    /// <summary>
    /// Replaces an item to a new one for all.
    /// </summary>
    /// <param name="col">The list to replace items.</param>
    /// <param name="oldItem">The old item to remove.</param>
    /// <param name="newItem">The new item to update.</param>
    /// <param name="maxCount">The maximum count of item to replace.</param>
    /// <param name="compare">The optional compare handler. Or null to test by reference equaling.</param>
    /// <returns>The count of item to replace.</returns>
    public static int Replace<T>(IList<T> col, T oldItem, T newItem, int maxCount, Func<T, T, bool> compare = null)
    {
        if (col is null) return 0;
        if (col is SynchronizedList<T> syncList) return syncList.Replace(oldItem, newItem, maxCount, compare);
        if (ReferenceEquals(oldItem, newItem)) return 0;
        compare ??= ObjectRef<T>.ReferenceEquals;
        var count = 0;
        try
        {
            for (var i = 0; i < col.Count; i++)
            {
                var test = col[i];
                if (!compare(test, oldItem)) continue;
                col[i] = newItem;
                count++;
                if (count >= maxCount) break;
            }
        }
        catch (ArgumentOutOfRangeException)
        {
        }

        return count;
    }

    /// <summary>
    /// Replaces an item to a new one for all.
    /// </summary>
    /// <param name="col">The list to replace items.</param>
    /// <param name="oldItem">The old item to remove.</param>
    /// <param name="newItem">The new item to update.</param>
    /// <param name="compare">The optional compare handler. Or null to test by reference equaling.</param>
    /// <returns>The count of item to replace.</returns>
    public static int Replace<T>(IList<T> col, T oldItem, T newItem, Func<T, T, bool> compare = null)
    {
        if (col is null) return 0;
        if (col is SynchronizedList<T> syncList) return syncList.Replace(oldItem, newItem, compare);
        if (ReferenceEquals(oldItem, newItem)) return 0;
        compare ??= ObjectRef<T>.ReferenceEquals;
        var count = 0;
        try
        {
            for (var i = 0; i < col.Count; i++)
            {
                var test = col[i];
                if (!compare(test, oldItem)) continue;
                col[i] = newItem;
                count++;
            }
        }
        catch (ArgumentOutOfRangeException)
        {
        }

        return count;
    }

    /// <summary>
    /// Filters the source collection but keep the default value.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="flags">The flags. It is a boolean collection that each item indicating whether need keep the source item by index.</param>
    /// <param name="defaultValue">The default value for removing.</param>
    /// <param name="callback">A callback.</param>
    /// <returns>The new collection after filtering.</returns>
    public static IEnumerable<T> KeepWithDefault<T>(IEnumerable<T> source, IEnumerable<bool> flags, T defaultValue = default, Action<T, bool, int> callback = null)
    {
        if (flags is not IList<bool> f) f = flags.ToList();
        var i = -1;
        foreach (var item in source)
        {
            i++;
            if (i < f.Count)
            {
                var flag = f[i];
                callback?.Invoke(item, flag, i);
                if (flag) yield return item;
                else yield return defaultValue;
            }
            else
            {
                callback?.Invoke(item, false, i);
                yield return defaultValue;
            }
        }
    }

    /// <summary>
    /// Filters the source collection and remove the ones no longer needed.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="flags">The flags. It is a boolean collection that each item indicating whether need keep the source item by index.</param>
    /// <param name="callback">A callback.</param>
    /// <returns>The new collection after filtering.</returns>
    public static IEnumerable<T> Keep<T>(IEnumerable<T> source, IEnumerable<bool> flags, Action<T, bool, int> callback = null)
    {
        if (flags is not IList<bool> f) f = flags.ToList();
        var i = 0;
        if (callback == null)
        {
            foreach (var item in source)
            {
                if (i >= f.Count) break;
                i++;
                var flag = f[i];
                callback?.Invoke(item, flag, i);
                if (flag) yield return item;
            }
        }
        else
        {
            foreach (var item in source)
            {
                i++;
                if (i >= f.Count)
                {
                    callback?.Invoke(item, false, i);
                }
                else
                {
                    var flag = f[i];
                    callback?.Invoke(item, flag, i);
                    if (flag) yield return item;
                }
            }
        }
    }

    /// <summary>
    /// Groups the key value pairs.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <returns>The groups.</returns>
    public static IEnumerable<IGrouping<TKey, TValue>> ToGroups<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list)
        => list?.GroupBy(item => item.Key, item => item.Value);

    /// <summary>
    /// Creates a dictionary from the key value pairs.
    /// </summary>
    /// <returns>A dictionary with key and the value collection.</returns>
    public static Dictionary<TKey, IEnumerable<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> list)
        => list?.ToDictionary(item => item.Key, item => item as IEnumerable<TValue>);

    /// <summary>
    /// Creates a dictionary from the key value pairs.
    /// </summary>
    /// <returns>A dictionary with key and the value collection.</returns>
    public static Dictionary<TKey, IEnumerable<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list)
        => ToGroups(list)?.ToDictionary(item => item.Key, item => item as IEnumerable<TValue>);

    /// <summary>
    /// Creates a synchronized list from the source collection.
    /// </summary>
    /// <typeparam name="T">The type of list item.</typeparam>
    /// <param name="list">The source collection.</param>
    /// <returns>A synchronized list.</returns>
    public static SynchronizedList<T> ToSynchronizedList<T>(IEnumerable<T> list)
        => new(list);

    /// <summary>
    /// Creates a synchronized list from the source collection.
    /// </summary>
    /// <typeparam name="T">The type of list item.</typeparam>
    /// <param name="list">The source collection.</param>
    /// <param name="useSource">true if set the collection as source directly instead of copying; otherwise, false.</param>
    /// <returns>A synchronized list.</returns>
    public static SynchronizedList<T> ToSynchronizedList<T>(List<T> list, bool useSource)
        => new(LockRecursionPolicy.NoRecursion, list, useSource);

    /// <summary>
    /// Creates a synchronized list from the source collection.
    /// </summary>
    /// <typeparam name="T">The type of list item.</typeparam>
    /// <param name="list">The source collection.</param>
    /// <param name="syncRoot">The object used to synchronize access the thread-safe collection.</param>
    /// <returns>A synchronized list.</returns>
#if NET9_0_OR_GREATER
    public static IList<T> ToSynchronizedList<T>(IEnumerable<T> list, Lock syncRoot)
#else
    public static IList<T> ToSynchronizedList<T>(IEnumerable<T> list, object syncRoot)
#endif
        => new ConcurrentList<T>(syncRoot, list);

    /// <summary>
    /// Creates a synchronized list from the source collection.
    /// </summary>
    /// <typeparam name="T">The type of list item.</typeparam>
    /// <param name="list">The source collection.</param>
    /// <param name="syncRoot">The object used to synchronize access the thread-safe collection.</param>
    /// <param name="useSource">true if set the collection as source directly instead of copying; otherwise, false.</param>
    /// <returns>A synchronized list.</returns>
#if NET9_0_OR_GREATER
    public static IList<T> ToSynchronizedList<T>(List<T> list, Lock syncRoot, bool useSource)
#else
    public static IList<T> ToSynchronizedList<T>(List<T> list, object syncRoot, bool useSource)
#endif
        => new ConcurrentList<T>(syncRoot, list, useSource);

    /// <summary>
    /// Determines whether it contains the property only with the specific key.
    /// </summary>
    /// <param name="col">The source collection.</param>
    /// <param name="keys">The property keys to test.</param>
    /// <param name="matched">The keys matched.</param>
    /// <returns>true if it contains the property key only; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">col is null.</exception>
    public static bool ContainsOnly(this IEnumerable<string> col, IEnumerable<string> keys, out List<string> matched)
    {
        if (col == null) throw ObjectConvert.ArgumentNull(nameof(col));
        matched = new();
        if (keys == null) return !col.Any();
        var b = true;
        foreach (var item in col)
        {
            if (keys.Contains(item)) matched.Add(item);
            else b = false;
        }

        return b;
    }

    /// <summary>
    /// Determines whether it contains the property only with the specific key.
    /// </summary>
    /// <param name="col">The source collection.</param>
    /// <param name="keys">The property keys to test.</param>
    /// <param name="matched">The keys matched.</param>
    /// <param name="rest">The rest property keys.</param>
    /// <returns>true if it contains the property key only; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">col is null.</exception>
    public static bool ContainsOnly(this IEnumerable<string> col, IEnumerable<string> keys, out List<string> matched, out List<string> rest)
    {
        if (col == null) throw ObjectConvert.ArgumentNull(nameof(col));
        matched = new();
        if (keys == null)
        {
            rest = col.ToList();
            return rest.Count == 0;
        }

        rest = new();
        var b = true;
        foreach (var item in col)
        {
            if (keys.Contains(item))
            {
                matched.Add(item);
            }
            else
            {
                b = false;
                rest.Add(item);
            }
        }

        return b;
    }

    /// <summary>
    /// Tests if they are same.
    /// </summary>
    /// <param name="a">Collection a.</param>
    /// <param name="b">Collection b.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool Equals<T>(T[] a, T[] b)
    {
        if (a.Length != b.Length) return false;
        try
        {
            for (var i = 0; i < a.Length; i++)
            {
                if ((a[i] == null && b[i] != null) || !a[i].Equals(b[i])) return false;
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Tests if they are same.
    /// </summary>
    /// <param name="a">Collection a.</param>
    /// <param name="b">Collection b.</param>
    /// <param name="compare">The equaling handler.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool Equals<T>(T[] a, T[] b, Func<T, T, bool> compare)
    {
        if (a.Length != b.Length) return false;
        compare ??= ObjectRef<T>.ReferenceEquals;
        for (var i = 0; i < a.Length; i++)
        {
            if (!compare(a[i], b[i])) return false;
        }

        return true;
    }

    /// <summary>
    /// Tests if they are same.
    /// </summary>
    /// <param name="a">Collection a.</param>
    /// <param name="b">Collection b.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool Equals<T>(IList<T> a, IList<T> b)
    {
        if (a.Count != b.Count) return false;
        try
        {
            for (var i = 0; i < a.Count; i++)
            {
                if ((a[i] == null && b[i] != null) || !a[i].Equals(b[i])) return false;
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Tests if they are same.
    /// </summary>
    /// <param name="a">Collection a.</param>
    /// <param name="b">Collection b.</param>
    /// <param name="compare">The equaling handler.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool Equals<T>(IList<T> a, IList<T> b, Func<T, T, bool> compare)
    {
        if (a.Count != b.Count) return false;
        compare ??= ObjectRef<T>.ReferenceEquals;
        for (var i = 0; i < a.Count; i++)
        {
            if (!compare(a[i], b[i])) return false;
        }

        return true;
    }

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A string collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A string collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<string> Where(this IEnumerable<string> source, StringCondition condition)
        => condition == null ? source : source.Where(condition.IsMatched);

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A number collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A number collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<int> Where(this IEnumerable<int> source, Int32Condition condition)
        => condition == null ? source : source.Where(condition.IsMatched);

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A number collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A number collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<long> Where(this IEnumerable<long> source, Int64Condition condition)
        => condition == null ? source : source.Where(condition.IsMatched);

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A number collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A number collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<float> Where(this IEnumerable<float> source, SingleCondition condition)
        => condition == null ? source : source.Where(condition.IsMatched);

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A number collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A number collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<double> Where(this IEnumerable<double> source, DoubleCondition condition)
        => condition == null ? source : source.Where(condition.IsMatched);

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A number collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A number collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<decimal> Where(this IEnumerable<decimal> source, DecimalCondition condition)
        => condition == null ? source : source.Where(condition.IsMatched);

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A date time collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A date time collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<DateTime> Where(this IEnumerable<DateTime> source, DateTimeCondition condition)
        => condition == null ? source : source.Where(condition.IsMatched);

    /// <summary>
    /// Converts to JSON object node collection.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON object node collection converted.</returns>
    public static IEnumerable<JsonObjectNode> ToJsonObjectNodes(this IEnumerable<ConciseModel> collection)
    {
        if (collection == null) yield break;
        foreach (var item in collection)
        {
            if (item == null) yield return null;
            yield return item.ToJson();
        }
    }

    /// <summary>
    /// Converts to JSON array.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON array node converted.</returns>
    public static JsonArrayNode ToJsonArrayNode(this IEnumerable<ConciseModel> collection)
    {
        if (collection == null) return null;
        var list = ToJsonObjectNodes(collection);
        var arr = new JsonArrayNode();
        arr.AddRange(list);
        return arr;
    }

    /// <summary>
    /// Converts to JSON object node collection.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON object node collection converted.</returns>
    public static IEnumerable<JsonObjectNode> ToJsonObjectNodes(this IEnumerable<ServerSentEventInfo> collection)
    {
        if (collection == null) yield break;
        foreach (var item in collection)
        {
            if (item == null) yield return null;
            yield return (JsonObjectNode)item;
        }
    }

    /// <summary>
    /// Converts to JSON object node collection.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <param name="eventName">The event name.</param>
    /// <returns>A JSON object node collection converted.</returns>
    public static IEnumerable<JsonObjectNode> ToJsonObjectNodes(this IEnumerable<ServerSentEventInfo> collection, string eventName)
    {
        if (collection == null) yield break;
        eventName = HttpClientExtensions.GetServerSentEventName(eventName);
        foreach (var item in collection)
        {
            if (item == null || item.EventName != eventName) continue;
            yield return (JsonObjectNode)item;
        }
    }

    /// <summary>
    /// Converts to JSON array.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON array node converted.</returns>
    public static JsonArrayNode ToJsonArrayNode(this IEnumerable<ServerSentEventInfo> collection)
    {
        if (collection == null) return null;
        var list = ToJsonObjectNodes(collection);
        var arr = new JsonArrayNode();
        arr.AddRange(list);
        return arr;
    }

    /// <summary>
    /// Converts to JSON object node collection.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON object node collection converted.</returns>
    public async static IAsyncEnumerable<JsonObjectNode> ToJsonObjectNodesAsync(this IAsyncEnumerable<ServerSentEventInfo> collection)
    {
        if (collection == null) yield break;
        await foreach (var item in collection)
        {
            if (item == null) yield return null;
            yield return (JsonObjectNode)item;
        }
    }

    /// <summary>
    /// Converts to JSON object node collection.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <param name="eventName">The event name.</param>
    /// <returns>A JSON object node collection converted.</returns>
    public async static IAsyncEnumerable<JsonObjectNode> ToJsonObjectNodesAsync(this IAsyncEnumerable<ServerSentEventInfo> collection, string eventName)
    {
        if (collection == null) yield break;
        eventName = HttpClientExtensions.GetServerSentEventName(eventName);
        await foreach (var item in collection)
        {
            if (item == null || item.EventName != eventName) continue;
            yield return (JsonObjectNode)item;
        }
    }

    /// <summary>
    /// Converts to JSON array.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON array node converted.</returns>
    public async static Task<JsonArrayNode> ToJsonArrayNodeAsync(this IAsyncEnumerable<ServerSentEventInfo> collection)
    {
        if (collection == null) return null;
        var list = ToJsonObjectNodesAsync(collection);
        var arr = new JsonArrayNode();
        await foreach (var item in list)
        {
            arr.Add(item);
        }

        return arr;
    }

    /// <summary>
    /// Raises on the specific key.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="eventName">The event name.</param>
    /// <param name="callback">The callback on item matched.</param>
    /// <returns>The collection.</returns>
    public static IEnumerable<ServerSentEventInfo> On(this IEnumerable<ServerSentEventInfo> collection, string eventName, Action<ServerSentEventInfo> callback)
    {
        if (collection == null || callback == null) yield break;
        foreach (var item in collection)
        {
            if (item == null) continue;
            if (item.EventName == eventName) callback(item);
            yield return item;
        }
    }

    /// <summary>
    /// Raises on the specific key.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="eventName">The event name.</param>
    /// <param name="callback">The callback on item matched.</param>
    /// <returns>The collection.</returns>
    public static IEnumerable<ServerSentEventInfo> On(this IEnumerable<ServerSentEventInfo> collection, string eventName, Action<ServerSentEventInfo, int> callback)
    {
        if (collection == null || callback == null) yield break;
        var i = -1;
        foreach (var item in collection)
        {
            if (item == null) continue;
            i++;
            if (item.EventName == eventName) callback(item, i);
            yield return item;
        }
    }

    /// <summary>
    /// Raises on all the specific key.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="handlers">The dictionary with key of event name and value of callback on item matched.</param>
    /// <returns>The collection.</returns>
    public static IEnumerable<ServerSentEventInfo> On(this IEnumerable<ServerSentEventInfo> collection, IDictionary<string, Action<ServerSentEventInfo>> handlers)
    {
        if (collection == null || handlers == null) yield break;
        foreach (var item in collection)
        {
            if (item == null) continue;
            if (handlers.TryGetValue(item.EventName, out var handler) && handler != null) handler(item);
            yield return item;
        }
    }

    /// <summary>
    /// Raises on all the specific key.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="handlers">The dictionary with key of event name and value of callback on item matched.</param>
    /// <returns>The collection.</returns>
    public static IEnumerable<ServerSentEventInfo> On(this IEnumerable<ServerSentEventInfo> collection, IDictionary<string, Action<ServerSentEventInfo, int>> handlers)
    {
        if (collection == null || handlers == null) yield break;
        var i = -1;
        foreach (var item in collection)
        {
            if (item == null) continue;
            i++;
            if (handlers.TryGetValue(item.EventName, out var handler) && handler != null) handler(item, i);
            yield return item;
        }
    }

    /// <summary>
    /// Raises on the specific key.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="eventName">The event name.</param>
    /// <param name="callback">The callback on item matched.</param>
    /// <returns>The collection.</returns>
    public async static IAsyncEnumerable<ServerSentEventInfo> OnAsync(this IAsyncEnumerable<ServerSentEventInfo> collection, string eventName, Action<ServerSentEventInfo> callback)
    {
        if (collection == null || callback == null) yield break;
        await foreach (var item in collection)
        {
            if (item == null) continue;
            if (item.EventName == eventName) callback(item);
            yield return item;
        }
    }

    /// <summary>
    /// Raises on the specific key.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="eventName">The event name.</param>
    /// <param name="callback">The callback on item matched.</param>
    /// <returns>The collection.</returns>
    public async static IAsyncEnumerable<ServerSentEventInfo> OnAsync(this IAsyncEnumerable<ServerSentEventInfo> collection, string eventName, Action<ServerSentEventInfo, int> callback)
    {
        if (collection == null || callback == null) yield break;
        var i = -1;
        await foreach (var item in collection)
        {
            if (item == null) continue;
            i++;
            if (item.EventName == eventName) callback(item, i);
            yield return item;
        }
    }

    /// <summary>
    /// Raises on all the specific key.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="handlers">The dictionary with key of event name and value of callback on item matched.</param>
    /// <returns>The collection.</returns>
    public async static IAsyncEnumerable<ServerSentEventInfo> OnAsync(this IAsyncEnumerable<ServerSentEventInfo> collection, IDictionary<string, Action<ServerSentEventInfo>> handlers)
    {
        if (collection == null || handlers == null) yield break;
        var i = -1;
        await foreach (var item in collection)
        {
            if (item == null) continue;
            i++;
            if (handlers.TryGetValue(item.EventName, out var handler) && handler != null) handler(item);
            yield return item;
        }
    }

    /// <summary>
    /// Raises on all the specific key.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="handlers">The dictionary with key of event name and value of callback on item matched.</param>
    /// <returns>The collection.</returns>
    public async static IAsyncEnumerable<ServerSentEventInfo> OnAsync(this IAsyncEnumerable<ServerSentEventInfo> collection, IDictionary<string, Action<ServerSentEventInfo, int>> handlers)
    {
        if (collection == null || handlers == null) yield break;
        var i = -1;
        await foreach (var item in collection)
        {
            if (item == null) continue;
            i++;
            if (handlers.TryGetValue(item.EventName, out var handler) && handler != null) handler(item, i);
            yield return item;
        }
    }

    /// <summary>
    /// Raises on all the specific key.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="callback">The callback.</param>
    /// <returns>The count of item received.</returns>
    public async static Task<int> ProcessAsync(this IAsyncEnumerable<ServerSentEventInfo> collection, Action<ServerSentEventInfo> callback = null)
    {
        if (collection == null) return 0;
        var i = -1;
        await foreach (var item in collection)
        {
            if (item == null) continue;
            i++;
            callback?.Invoke(item);
        }

        return i + 1;
    }

    /// <summary>
    /// Raises on all the specific key.
    /// </summary>
    /// <param name="collection">The source collection.</param>
    /// <param name="callback">The callback.</param>
    /// <returns>The count of item received.</returns>
    public async static Task<int> ProcessAsync(this IAsyncEnumerable<ServerSentEventInfo> collection, Action<ServerSentEventInfo, int> callback)
    {
        if (collection == null) return 0;
        var i = -1;
        await foreach (var item in collection)
        {
            if (item == null) continue;
            i++;
            callback?.Invoke(item, i);
        }

        return i + 1;
    }

    internal static IEnumerable<TResult> Select<TItem, TResult>(IEnumerable<TItem> leftValue, IEnumerable<TItem> rightValue, TItem padding, Func<TItem, bool, TItem, bool, int, TResult> callback)
    {
        var a = leftValue.GetEnumerator();
        var b = rightValue.GetEnumerator();
        var hasA = true;
        var hasB = true;
        var i = -1;
        while (true)
        {
            i++;
            var vA = padding;
            if (hasA)
            {
                hasA = a.MoveNext();
                if (hasA) vA = a.Current;
                else a.Dispose();
            }

            var vB = padding;
            if (hasB)
            {
                hasB = b.MoveNext();
                if (hasB) vB = b.Current;
                else b.Dispose();
            }

            if (!hasA && !hasB) yield break;
            yield return callback(vA, hasA, vB, hasB, i);
        }
    }

    internal static List<T> ToList<T>(IEnumerable<T> col, bool create)
    {
        if (col is null) return create ? new() : null;
        return col is List<T> list ? list : col.ToList();
    }

    private static bool IsNotNull<T>(T obj)
        => obj is not null;

    private static bool IsNotNullOrEmpty(string s)
        => !string.IsNullOrEmpty(s);

    private static bool IsNotNullOrWhiteSpace(string s)
        => !string.IsNullOrWhiteSpace(s);
}
