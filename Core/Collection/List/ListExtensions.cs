using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        if (dict == null) throw new ArgumentNullException(nameof(dict), "dict should not be null.");
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
        if (dict == null) throw new ArgumentNullException(nameof(dict), "dict should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
        list.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
    }

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
        if (col == null) throw new ArgumentNullException(nameof(col), "col should not be null.");
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
        if (col == null) throw new ArgumentNullException(nameof(col), "col should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
    {
        return GetValues(list, key).ToList()[index];
    }

    /// <summary>
    /// Gets the value by a specific key.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key.</param>
    /// <returns>The value. The first one for multiple values.</returns>
    /// <exception cref="IndexOutOfRangeException">index is less than 0, or is equals to or greater than the length of the values of the specific key.</exception>
    public static IGrouping<TKey, TValue> Get<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
    {
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static List<int> CreateNumberRange(int start, int count, int step = 1)
    {
        var list = new List<int>();
        if (step == 1)
        {
            for (var i = 0; i < count; i++)
            {
                list.Add(i + start);
            }
        }
        else if (step == 0)
        {
            for (var i = 0; i < count; i++)
            {
                list.Add(start);
            }
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                list.Add(i * step + start);
            }
        }

        return list;
    }

    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static List<uint> CreateNumberRange(uint start, int count, uint step = 1)
    {
        var list = new List<uint>();
        if (step == 1)
        {
            for (uint i = 0; i < count; i++)
            {
                list.Add(i + start);
            }
        }
        else if (step == 0)
        {
            for (uint i = 0; i < count; i++)
            {
                list.Add(start);
            }
        }
        else
        {
            for (uint i = 0; i < count; i++)
            {
                list.Add(i * step + start);
            }
        }

        return list;
    }

    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static IEnumerable<long> CreateNumberRange(long start, int count, long step = 1)
    {
        if (step == 1)
        {
            for (var i = 0L; i < count; i++)
            {
                yield return i + start;
            }
        }
        else if (step == 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return start;
            }
        }

        for (var i = 0L; i < count; i++)
        {
            yield return i * step + start;
        }
    }

    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static IEnumerable<float> CreateNumberRange(float start, int count, float step = 1)
    {
        if (step == 1)
        {
            for (var i = 0; i < count; i++)
            {
                yield return i + start;
            }
        }
        else if (step == 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return start;
            }
        }

        for (var i = 0; i < count; i++)
        {
            yield return i * step + start;
        }
    }

    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static IEnumerable<double> CreateNumberRange(double start, int count, double step = 1)
    {
        if (step == 1)
        {
            for (var i = 0; i < count; i++)
            {
                yield return i + start;
            }
        }
        else if (step == 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return start;
            }
        }

        for (var i = 0; i < count; i++)
        {
            yield return i * step + start;
        }
    }

    /// <summary>
    /// Creates a range of number.
    /// </summary>
    /// <param name="start">A number to start.</param>
    /// <param name="count">The length.</param>
    /// <param name="step">The optional step.</param>
    /// <returns>A list.</returns>
    public static IEnumerable<decimal> CreateNumberRange(decimal start, decimal count, decimal step = 1)
    {
        if (step == 1)
        {
            for (var i = 0; i < count; i++)
            {
                yield return i + start;
            }
        }
        else if (step == 0)
        {
            for (var i = 0; i < count; i++)
            {
                yield return start;
            }
        }

        for (var i = 0; i < count; i++)
        {
            yield return i * step + start;
        }
    }

    /// <summary>
    /// Creates a collection with a specific number of item.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="count">The count.</param>
    /// <param name="item">The default item.</param>
    /// <returns>A sequence.</returns>
    public static IEnumerable<T> Create<T>(int count, T item = default)
    {
        for (var i = 0; i < count; i++)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Creates an array with a specific number of item.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="count">The count.</param>
    /// <param name="item">The default item.</param>
    /// <returns>An array.</returns>
    public static T[] CreateArray<T>(int count, T item = default)
    {
        var arr = new T[count];
        for (var i = 0; i < count; i++)
        {
            arr[i] = item;
        }

        return arr;
    }

    /// <summary>
    /// Removes all the elements by the specific key.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="keys">The keys to remove.</param>
    /// <returns>The number of elements removed from the key value pairs.</returns>
    public static int Remove<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, params TKey[] keys)
    {
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
        return IndexOf(list.Reverse(), key);
    }

    /// <summary>
    /// Searches for the specified key and returns the zero-based index of the first occurrence within the entire key value pairs.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="key">The key.</param>
    public static int IndexOf<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
    {
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
    /// Searches for the specified object and returns the zero-based index array of the all occurrence within the entire key value pairs.
    /// </summary>
    /// <param name="list">The key value pairs.</param>
    /// <param name="test">The object to test.</param>
    /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
    /// <param name="count">The number of elements in the section to search.</param>
    public static IEnumerable<int> AllIndexesOf<T>(this IEnumerable<T> list, T test, int index = 0, int? count = null)
    {
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
        if (test == null) throw new ArgumentNullException(nameof(test), "test should be a function to test.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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
        => new(System.Threading.LockRecursionPolicy.NoRecursion, list, useSource);

    /// <summary>
    /// Creates a synchronized list from the source collection.
    /// </summary>
    /// <typeparam name="T">The type of list item.</typeparam>
    /// <param name="list">The source collection.</param>
    /// <param name="syncRoot">The object used to synchronize access the thread-safe collection.</param>
    /// <returns>A synchronized list.</returns>
    public static IList<T> ToSynchronizedList<T>(IEnumerable<T> list, object syncRoot)
        => new ConcurrentList<T>(syncRoot, list);

    /// <summary>
    /// Creates a synchronized list from the source collection.
    /// </summary>
    /// <typeparam name="T">The type of list item.</typeparam>
    /// <param name="list">The source collection.</param>
    /// <param name="syncRoot">The object used to synchronize access the thread-safe collection.</param>
    /// <param name="useSource">true if set the collection as source directly instead of copying; otherwise, false.</param>
    /// <returns>A synchronized list.</returns>
    public static IList<T> ToSynchronizedList<T>(List<T> list, object syncRoot, bool useSource)
        => new ConcurrentList<T>(syncRoot, list, useSource);

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
    {
        if (condition == null) return source;
        return source.Where(ele => condition.IsMatched(ele));
    }

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A number collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A number collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<int> Where(this IEnumerable<int> source, Int32Condition condition)
    {
        if (condition == null) return source;
        return source.Where(ele => condition.IsMatched(ele));
    }

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A number collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A number collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<long> Where(this IEnumerable<long> source, Int64Condition condition)
    {
        if (condition == null) return source;
        return source.Where(ele => condition.IsMatched(ele));
    }

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A number collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A number collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<float> Where(this IEnumerable<float> source, SingleCondition condition)
    {
        if (condition == null) return source;
        return source.Where(ele => condition.IsMatched(ele));
    }

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A number collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A number collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<double> Where(this IEnumerable<double> source, DoubleCondition condition)
    {
        if (condition == null) return source;
        return source.Where(ele => condition.IsMatched(ele));
    }

    /// <summary>
    /// Filters a sequence of values based on a condition.
    /// </summary>
    /// <param name="source">A date time collection to filter.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>A date time collection that contains elements from the input sequence that satisfy the condition.</returns>
    public static IEnumerable<DateTime> Where(this IEnumerable<DateTime> source, DateTimeCondition condition)
    {
        if (condition == null) return source;
        return source.Where(ele => condition.IsMatched(ele));
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key, string value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item is not null && item.TryGetStringValue(key) == value) yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key, string value, StringComparison comparisonType)
    {
        if (col == null) yield break;
        if (value == null)
        {
            foreach (var item in col)
            {
                if (item is not null && item.IsNullOrUndefined(key)) yield return item;
            }
        }
        else
        {
            foreach (var item in col)
            {
                if (item is not null && value.Equals(item.TryGetStringValue(key), comparisonType)) yield return item;
            }
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key, int value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item is not null && item.TryGetInt32Value(key) == value) yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key, bool value)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item is not null && item.TryGetBooleanValue(key) == value) yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="kind">The kind of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key, JsonValueKind kind)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item is not null && item.GetValueKind(key) == kind) yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the collection.
    /// </summary>
    /// <param name="col">The input collection.</param>
    /// <param name="key">The property key required.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this IEnumerable<JsonObjectNode> col, string key)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            if (item is not null && item.ContainsKey(key)) yield return item;
        }
    }

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key, string value)
        => WithProperty(array?.SelectObjects(), key, value);

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key, string value, StringComparison comparisonType)
        => WithProperty(array?.SelectObjects(), key, value, comparisonType);

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key, int value)
        => WithProperty(array?.SelectObjects(), key, value);

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key, bool value)
        => WithProperty(array?.SelectObjects(), key, value);

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <param name="kind">The kind of the property.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key, JsonValueKind kind)
        => WithProperty(array?.SelectObjects(), key, kind);

    /// <summary>
    /// Gets the all JSON object items which contains the specific property from the array.
    /// </summary>
    /// <param name="array">The input JSON array node.</param>
    /// <param name="key">The property key required.</param>
    /// <returns>A collection of the JSON object node.</returns>
    public static IEnumerable<JsonObjectNode> WithProperty(this JsonArrayNode array, string key)
        => WithProperty(array?.SelectObjects(), key);

    /// <summary>
    /// Converts to JSON object node collection.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON object node collection converted.</returns>
    public static IEnumerable<JsonObjectNode> ToJsonObjectNodes(this IEnumerable<IJsonObjectHost> collection)
    {
        if (collection == null) yield break;
        foreach (var item in collection)
        {
            yield return item?.ToJson();
        }
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
    /// Converts to JSON array.
    /// </summary>
    /// <param name="collection">The collection of the item to convert.</param>
    /// <returns>A JSON array node converted.</returns>
    public static JsonArrayNode ToJsonArrayNode(this IEnumerable<IJsonObjectHost> collection)
    {
        if (collection == null) return null;
        var arr = new JsonArrayNode();
        foreach (var item in collection)
        {
            arr.Add(item);
        }

        return arr;
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
}
