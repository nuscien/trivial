using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Trivial.Collection
{
    /// <summary>
    /// The list utility.
    /// </summary>
    public static class ListUtility
    {
        /// <summary>
        /// Adds a key and a value to the end of the key value pairs.
        /// </summary>
        /// <param name="list">The key value pairs.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="clearOthers">true if clear the others of the property before adding; otherwise, false.</param>
        public static void Add<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, TKey key, TValue value, bool clearOthers = false)
        {
            if (clearOthers) list.Remove(key);
            list.Add(new KeyValuePair<TKey, TValue>(key, value));
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
            list.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <param name="list">The key value pairs.</param>
        /// <returns>The keys.</returns>
        public static IEnumerable<TKey> Keys<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list)
        {
            return list.Select(item => item.Key).Distinct();
        }

        /// <summary>
        /// Gets the values by a specific key.
        /// </summary>
        /// <param name="list">The key value pairs.</param>
        /// <param name="key">The key.</param>
        /// <returns>The values.</returns>
        public static IEnumerable<TValue> Values<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
        {
            return key != null ? list.Where(item => key.Equals(item.Key)).Select(item => item.Value) : list.Where(item => item.Key == null).Select(item => item.Value);
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
            var col = list.Values(key).ToList();
            if (list.Count <= index)
            {
                value = default;
                return false;
            }

            value = col[index];
            return true;
        }

        /// <summary>
        /// Gets the query value by a specific key.
        /// </summary>
        /// <param name="list">The key value pairs.</param>
        /// <param name="key">The key.</param>
        /// <param name="index">The index of the value for multiple values.</param>
        /// <returns>The query value. The first one for multiple values.</returns>
        /// <exception cref="IndexOutOfRangeException">index is less than 0, or is equals to or greater than the length of the values of the specific key.</exception>
        public static TValue GetValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key, int index)
        {
            return Values(list, key).ToList()[index];
        }

        /// <summary>
        /// Determines whether the instance contains the specified
        /// </summary>
        /// <param name="list">The key value pairs.</param>
        /// <param name="key">The key to locate in the instance.</param>
        /// <returns>true if the instance contains an element with the specified key; otherwise, false.</returns>
        public static bool ContainsKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
        {
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
            var count = 0;
            foreach (var key in keys)
            {
                count = key == null ? list.RemoveAll(item => item.Key == null) : list.RemoveAll(item => key.Equals(item.Key));
            }

            return count;
        }

        /// <summary>
        /// Searches for the specified key and returns the zero-based index of the first occurrence within the entire key value pairs.
        /// </summary>
        /// <param name="list">The key value pairs.</param>
        /// <param name="key">The key.</param>
        public static int IndexOf<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list, TKey key)
        {
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
        public static IEnumerable<int> IndexOfAll<T>(this IEnumerable<T> list, T test, int index = 0, int? count = null)
        {
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
        public static IEnumerable<int> IndexOfAll<T>(this IEnumerable<T> list, Func<T, bool> test, int index = 0, int? count = null)
        {
            if (test == null) throw new ArgumentNullException(nameof(test));
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
        public static IEnumerable<int> IndexOfAll<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, TKey key)
        {
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
        public static IEnumerable<int> IndexOfAll<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, TKey key, TValue value, int index = 0, int? count = null)
        {
            Func<KeyValuePair<TKey, TValue>, bool> test;
            if (key == null && value == null) test = item => item.Key == null && item.Value == null;
            else if (key != null && value == null) test = item => key.Equals(item.Key) && item.Value == null;
            else if (key == null && value != null) test = item => item.Key == null && value.Equals(item.Value);
            else test = item => key.Equals(item.Key) && value.Equals(item.Value);
            return IndexOfAll(list, test, index, count);
        }

        /// <summary>
        /// Sets a key value.
        /// </summary>
        /// <param name="list">The key value pairs.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="insertAtLast">true if insert at last; otherwise, false.</param>
        public static void SetValue<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, TKey key, TValue value, bool insertAtLast = false)
        {
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
        /// Groups the key value pairs.
        /// </summary>
        /// <param name="list">The key value pairs.</param>
        /// <returns>The groups.</returns>
        public static IEnumerable<IGrouping<TKey, TValue>> ToGroups<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list)
        {
            return list.GroupBy(item => item.Key, item => item.Value);
        }

        /// <summary>
        /// Creates a dictionary from the key value pairs.
        /// </summary>
        /// <returns>A dictionary with key and the value collection.</returns>
        public static Dictionary<TKey, IEnumerable<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list)
        {
            return ToGroups(list).ToDictionary(item => item.Key, item => item as IEnumerable<TValue>);
        }
    }
}
