using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Text.Json;

namespace Trivial.Text
{
    /// <summary>
    /// The string extension and helper.
    /// </summary>
    internal static class InternalHelper

    {
        private const long ticksOffset = 621355968000000000;

        /// <summary>
        /// Special characters of YAML.
        /// </summary>
        internal static readonly char[] YamlSpecialChars = new[] { ':', '\r', '\n', '\\', '\'', '\"', '\t', ' ', '#', '.', '[', '{', '\\', '/', '@' };

        /// <summary>
        /// Breaks lines.
        /// </summary>
        /// <param name="text">The original string.</param>
        /// <param name="length">The count of a line.</param>
        /// <param name="newLine">The optional newline string.</param>
        /// <returns>A new text with line break.</returns>
        public static string BreakLines(string text, int length, string newLine = null)
        {
            var idx = 0;
            var len = text.Length;
            var str = new StringBuilder();
            while (idx < len)
            {
                if (idx > 0)
                {
                    str.Append(newLine ?? Environment.NewLine);
                }

                if (idx + length >= len)
                {
                    str.Append(text.Substring(idx));
                }
                else
                {
                    str.Append(text.Substring(idx, length));
                }

                idx += length;
            }

            return str.ToString();
        }

        /// <summary>
        /// Breaks lines.
        /// </summary>
        /// <param name="text">The original string.</param>
        /// <param name="length">The count of a line.</param>
        /// <param name="newLine">The newline character.</param>
        /// <returns>A new text with line break.</returns>
        public static string BreakLines(string text, int length, char newLine)
        {
            var idx = 0;
            var len = text.Length;
            var str = new StringBuilder();
            while (idx < len)
            {
                if (idx > 0)
                {
                    str.Append(newLine);
                }

                if (idx + length >= len)
                {
                    str.Append(text.Substring(idx));
                }
                else
                {
                    str.Append(text.Substring(idx, length));
                }

                idx += length;
            }

            return str.ToString();
        }

        /// <summary>
        /// Splits a string into a number of substrings based on the string in an enumerable strings.
        /// </summary>
        /// <param name="source">The source string to split.</param>
        /// <param name="seperator">A string that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
        /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
        public static IEnumerable<string> YieldSplit(this string source, string seperator, StringSplitOptions options = StringSplitOptions.None)
        {
            if (options == StringSplitOptions.None && string.IsNullOrEmpty(source)) yield break;
            if (options == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrWhiteSpace(source)) yield break;
            if (string.IsNullOrEmpty(seperator))
            {
                yield return source;
                yield break;
            }

            var index = 0;
            if (options == StringSplitOptions.None)
            {
                while (true)
                {
                    var i = source.IndexOf(seperator, index);
                    if (i < 0)
                    {
                        yield return source.Substring(index);
                        break;
                    }

                    yield return source.SubRangeString(index, i);
                    index = i + seperator.Length;
                    if (index >= source.Length) break;
                }
            }
            else
            {
                while (true)
                {
                    var i = source.IndexOf(seperator, index);
                    if (i < 0)
                    {
                        yield return source.Substring(index);
                        break;
                    }

                    if (i <= index) continue;
                    yield return source.SubRangeString(index, i);
                    index = i + seperator.Length;
                    if (index >= source.Length) break;
                }
            }
        }

        /// <summary>
        /// Splits a string into a number of substrings based on the character in an enumerable strings.
        /// </summary>
        /// <param name="source">The source string to split.</param>
        /// <param name="seperator">A character that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
        /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
        public static IEnumerable<string> YieldSplit(this string source, char seperator, StringSplitOptions options = StringSplitOptions.None)
        {
            return YieldSplit(source, new[] { seperator }, options);
        }

        /// <summary>
        /// Splits a string into a number of substrings based on the characters in an enumerable strings.
        /// </summary>
        /// <param name="source">The source string to split.</param>
        /// <param name="seperatorA">A character that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="seperatorB">Another character that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
        /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
        public static IEnumerable<string> YieldSplit(this string source, char seperatorA, char seperatorB, StringSplitOptions options = StringSplitOptions.None)
        {
            return YieldSplit(source, new[] { seperatorA, seperatorB }, options);
        }

        /// <summary>
        /// Splits a string into a number of substrings based on the characters in an enumerable strings.
        /// </summary>
        /// <param name="source">The source string to split.</param>
        /// <param name="seperatorA">The character A that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="seperatorB">The character B that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="seperatorC">The character C that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
        /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
        public static IEnumerable<string> YieldSplit(this string source, char seperatorA, char seperatorB, char seperatorC, StringSplitOptions options = StringSplitOptions.None)
        {
            return YieldSplit(source, new[] { seperatorA, seperatorB, seperatorC }, options);
        }

        /// <summary>
        /// Splits a string into a number of substrings based on the characters in an enumerable strings.
        /// </summary>
        /// <param name="source">The source string to split.</param>
        /// <param name="seperatorA">The character A that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="seperatorB">The character B that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="seperatorC">The character C that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="seperatorD">The character D that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
        /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
        public static IEnumerable<string> YieldSplit(this string source, char seperatorA, char seperatorB, char seperatorC, char seperatorD, StringSplitOptions options = StringSplitOptions.None)
        {
            return YieldSplit(source, new[] { seperatorA, seperatorB, seperatorC, seperatorD }, options);
        }

        /// <summary>
        /// Splits a string into a number of substrings based on the characters in an enumerable strings.
        /// </summary>
        /// <param name="source">The source string to split.</param>
        /// <param name="seperators">A character array that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
        /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
        public static IEnumerable<string> YieldSplit(this string source, char[] seperators, StringSplitOptions options = StringSplitOptions.None)
        {
            if (options == StringSplitOptions.None && string.IsNullOrEmpty(source)) yield break;
            if (options == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrWhiteSpace(source)) yield break;
            if (seperators == null || seperators.Length == 0)
            {
                yield return source;
                yield break;
            }

            var index = 0;
            if (options == StringSplitOptions.None)
            {
                while (true)
                {
                    var i = source.IndexOfAny(seperators, index);
                    if (i < 0)
                    {
                        yield return source.Substring(index);
                        break;
                    }

                    yield return source.SubRangeString(index, i);
                    index = i + 1;
                    if (index >= source.Length) break;
                }
            }
            else
            {
                while (true)
                {
                    var i = source.IndexOfAny(seperators, index);
                    if (i < 0)
                    {
                        yield return source.Substring(index);
                        break;
                    }

                    if (i <= index) continue;
                    yield return source.SubRangeString(index, i);
                    index = i + 1;
                    if (index >= source.Length) break;
                }
            }
        }

        /// <summary>
        /// Splits a string into a number of substrings based on the strings in an enumerable strings.
        /// </summary>
        /// <param name="source">The source string to split.</param>
        /// <param name="seperators">A string array that delimits the substrings in the source string, an empty array that contains no delimiters, or null.</param>
        /// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements from the array returned; or System.StringSplitOptions.None to include empty array elements in the array returned.</param>
        /// <returns>A string enumerable instance whose elements contain the substrings in the source string that are delimited by one or more characters in separator.</returns>
        public static IEnumerable<string> YieldSplit(this string source, IEnumerable<string> seperators, StringSplitOptions options = StringSplitOptions.None)
        {
            if (options == StringSplitOptions.None && string.IsNullOrEmpty(source)) yield break;
            if (options == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrWhiteSpace(source)) yield break;
            if (seperators == null)
            {
                yield return source;
                yield break;
            }

            var index = 0;
            if (options == StringSplitOptions.None)
            {
                while (true)
                {
                    var indexes = seperators.Select(ele => (Index: source.IndexOf(ele, index), ele.Length)).Where(ele => ele.Index >= 0).ToList();
                    if (indexes.Count == 0)
                    {
                        yield return source.Substring(index);
                        break;
                    }

                    var i = indexes.Min(ele => ele.Index);
                    var len = indexes.Where(ele => ele.Index == i).Max(ele => ele.Length);
                    yield return source.SubRangeString(index, i);
                    index = i + len;
                    if (index >= source.Length) break;
                }
            }
            else
            {
                while (true)
                {
                    var indexes = seperators.Select(ele => (Index: source.IndexOf(ele, index), ele.Length)).Where(ele => ele.Index >= 0).ToList();
                    if (indexes.Count == 0)
                    {
                        yield return source.Substring(index);
                        break;
                    }

                    var i = indexes.Min(ele => ele.Index);
                    if (i <= index) continue;
                    var len = indexes.Where(ele => ele.Index == i).Max(ele => ele.Length);
                    yield return source.SubRangeString(index, i);
                    index = i + len;
                    if (index >= source.Length) break;
                }
            }
        }

        /// <summary>
        /// Reads lines from a string.
        /// </summary>
        /// <param name="source">The source string to split.</param>
        /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
        /// <returns>Lines from the specific string.</returns>
        public static IEnumerable<string> ReadLines(string source, bool removeEmptyLine = false)
        {
            return YieldSplit(source, new[] { "\r\n", "\n", "\r" }, removeEmptyLine ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        /// <summary>
        /// Converts a secure string to unsecure string.
        /// </summary>
        /// <param name="value">The secure string to convert.</param>
        /// <returns>The unsecure string.</returns>
        public static string ToUnsecureString(this SecureString value)
        {
            if (value == null) return null;
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
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
        /// Groups the key value pairs.
        /// </summary>
        /// <param name="list">The key value pairs.</param>
        /// <returns>The groups.</returns>
        public static IEnumerable<IGrouping<TKey, TValue>> ToGroups<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list), "list should not be null.");
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

        /// <summary>
        /// Tests if they are same.
        /// </summary>
        /// <param name="a">Collection a.</param>
        /// <param name="b">Collection b.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool Equals<T>(T[] a, T[] b)
        {
            if (a.Length != b.Length) return false;
            for (var i = 0; i < a.Length; i++)
            {
                if ((a[i] == null && b[i] != null) || !a[i].Equals(b[i])) return false;
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
            for (var i = 0; i < a.Count; i++)
            {
                if ((a[i] == null && b[i] != null) || !a[i].Equals(b[i])) return false;
            }

            return true;
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time.
        /// </summary>
        /// <param name="ticks">The JavaScript date ticks.</param>
        /// <returns>A date and time.</returns>
        public static DateTime ParseDate(long ticks)
        {
            var time = new DateTime(ticks * 10000 + ticksOffset, DateTimeKind.Utc);
            return time.ToLocalTime();
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time.
        /// </summary>
        /// <param name="ticks">The JavaScript date ticks.</param>
        /// <returns>A date and time.</returns>
        public static DateTime? ParseDate(long? ticks)
        {
            if (!ticks.HasValue) return null;
            return ParseDate(ticks.Value);
        }

        /// <summary>
        /// Parses JavaScript date ticks to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date ticks.</returns>
        public static long ParseDate(DateTime date)
        {
            return (date.ToUniversalTime().Ticks - ticksOffset) / 10000;
        }

        /// <summary>
        /// Parses JavaScript date ticks to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date ticks.</returns>
        public static long ParseDate(DateTimeOffset date)
        {
            return (date.ToUniversalTime().Ticks - ticksOffset) / 10000;
        }

        /// <summary>
        /// Parses JavaScript date ticks to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date ticks.</returns>
        public static long? ParseDate(DateTime? date)
        {
            if (!date.HasValue) return null;
            return ParseDate(date.Value);
        }

        /// <summary>
        /// Parses JavaScript date ticks to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date ticks.</returns>
        public static long? ParseDate(DateTimeOffset? date)
        {
            if (!date.HasValue) return null;
            return ParseDate(date.Value);
        }

        /// <summary>
        /// Parses a ISO 8601 date string to date time.
        /// </summary>
        /// <param name="s">The JSON token value of JavaScript date.</param>
        /// <returns>A date and time.</returns>
        public static DateTime? ParseDate(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim().ToUpperInvariant();
            if (s.Length == 8)
            {
                var y2 = GetNaturalNumber(s, 0, 4);
                if (y2 < 0) return null;
                if (s[4] == 'W')
                {
                    var w3 = GetNaturalNumber(s, 5, 2);
                    if (w3 < 0) return null;
                    var d3 = GetNaturalNumber(s, 7);
                    if (d3 < 0) return null;
                    var r = new DateTime(y2, 1, 4, 0, 0, 0, DateTimeKind.Utc);
                    r = r.AddDays((r.DayOfWeek == DayOfWeek.Sunday ? -7 : -(int)r.DayOfWeek) + w3 * 7 - 7 + d3);
                    return r;
                }

                var m2 = s[4] == '-' ? GetNaturalNumber(s, 5, 1) : GetNaturalNumber(s, 4, 2);
                if (m2 < 0) return null;
                var d2 = GetNaturalNumber(s, s[6] == '-' ? 7 : 6);
                if (d2 < 0) return null;
                return new DateTime(y2, m2, d2, 0, 0, 0, DateTimeKind.Utc);
            }

            if (s.Length > 27)
            {
                if (s[3] == ',' && s[4] == ' ')
                {
                    var d2 = GetNaturalNumber(s, 5, 2);
                    var m2 = GetMonth(s, 8);
                    if (d2 > 0 && m2 > 0)
                    {
                        var y2 = GetNaturalNumber(s, 12, 4);
                        var h2 = GetNaturalNumber(s, 17, 2);
                        var mm2 = GetNaturalNumber(s, 20, 2);
                        var s2 = GetNaturalNumber(s, 23, 2);
                        try
                        {
                            var time = new DateTime(y2, m2, d2, h2, mm2, s2, DateTimeKind.Utc);
                            if (s.Length > 33 && (s[29] == '+' || s[29] == '-'))
                            {
                                var offsetH = GetNaturalNumber(s, 30, 2);
                                if (offsetH < 0) offsetH = 0;
                                if (s[29] == '+') offsetH = -offsetH;
                                time = time.AddHours(offsetH);
                                var offsetM = GetNaturalNumber(s, 32, 2);
                                if (offsetM > 0) time = time.AddMinutes(-offsetM);
                            }

                            return time;
                        }
                        catch (ArgumentException)
                        {
                            return null;
                        }
                    }
                }
                else if (s[3] == ' ' && s[7] == ' ')
                {
                    var m2 = GetMonth(s, 4);
                    var d2 = GetNaturalNumber(s, 8);
                    if (d2 > 0 && m2 > 0)
                    {
                        var y2 = GetNaturalNumber(s, 11, 4);
                        var h2 = GetNaturalNumber(s, 16, 2);
                        var mm2 = GetNaturalNumber(s, 19, 2);
                        var s2 = GetNaturalNumber(s, 22, 2);
                        try
                        {
                            var time = new DateTime(y2, m2, d2, h2, mm2, s2, DateTimeKind.Utc);
                            if (s.Length > 32 && (s[28] == '+' || s[28] == '-'))
                            {
                                var offsetH = GetNaturalNumber(s, 29, 2);
                                if (offsetH < 0) offsetH = 0;
                                if (s[28] == '+') offsetH = -offsetH;
                                time = time.AddHours(offsetH);
                                var offsetM = GetNaturalNumber(s, 31, 2);
                                if (offsetM > 0) time = time.AddMinutes(-offsetM);
                            }

                            return time;
                        }
                        catch (ArgumentException)
                        {
                            return null;
                        }
                    }
                }
            }

            if (s.Length < 10)
            {
                if (s.Length > 3 && (s[1] == '月' || (s[2] == '月' && (s[0] == '1' || s[0] == '0'))) && (s.IndexOf('日') > 2 || s.IndexOf('号') > 2) && s.IndexOf('年') < 0)
                    s = $"{DateTime.Now.Year:g}-{s.Replace('月', '-').Replace("日", string.Empty).Replace("号", string.Empty)}";
                else
                    return s.ToLowerInvariant() switch
                    {
                        "now" or "现在" or "現在" or "今" or "지금" or "ahora" or "agora" or "сейчас" or "теперь" => DateTime.UtcNow,
                        "today" or "今日" or "今天" or "当天" or "kyo" or "오늘" or "hoy" or "сегодня" => DateTime.UtcNow.Date,
                        "tomorrow" or "明日" or "明天" or "次日" or "翌日" or "demain" or "내일" or "mañana" or "amanhã" or "завтра" => DateTime.UtcNow.AddDays(1),
                        "yesterday" or "昨日" or "昨天" or "hier" or "어제" or "ayer" or "ontem" or "вчера" => DateTime.UtcNow.AddDays(-1),
                        _ => null
                    };
            }

            if (s[4] != '-')
            {
                if (s[4] == '年')
                    s = s.Replace('年', '-').Replace('月', '-').Replace("日", string.Empty).Replace("号", string.Empty);
                else
                    return s.ToLowerInvariant() switch
                    {
                        "maintenant" => DateTime.UtcNow,
                        "aujourd'hui" => DateTime.UtcNow.Date,
                        _ => null
                    };
            }

            var y = GetNaturalNumber(s, 0, 4);
            if (y < 0) return null;
            if (s[5] == 'W')
            {
                var w3 = GetNaturalNumber(s, 6, 2);
                if (w3 < 0) return null;
                var d3 = GetNaturalNumber(s, s[8] == '-' ? 9 : 8, 1);
                if (d3 < 0) return null;
                var r = new DateTime(y, 1, 4, 0, 0, 0, DateTimeKind.Utc);
                r = r.AddDays((r.DayOfWeek == DayOfWeek.Sunday ? -7 : -(int)r.DayOfWeek) + w3 * 7 - 7 + d3);
                return r;
            }

            var pos = s[7] == '-' ? 8 : 7;
            var m = GetNaturalNumber(s, 5, 2);
            if (m < 0)
            {
                if (s[6] == '-') m = GetNaturalNumber(s, 5, 1);
                if (m < 0) return null;
            }

            var d = GetNaturalNumber(s, pos, 2);
            if (d < 1)
            {
                pos += 4;
                d = GetNaturalNumber(s, pos, 1);
                if (d < 1) return null;
            }
            else
            {
                pos += 3;
            }

            var date = new DateTime(y, m, d, 0, 0, 0, DateTimeKind.Utc);
            if (pos >= s.Length) return date;
            #pragma warning disable IDE0057
            s = s.Substring(pos);
            #pragma warning restore IDE0057
            var arr = s.Split(':');
            if (arr.Length < 2) return date;
            if (!int.TryParse(arr[0], out var h)) return date;
            if (!int.TryParse(arr[1], out var mm)) return date;
            var t = new DateTime(y, m, d, h, mm, 0, DateTimeKind.Utc).ToLocalTime();
            if (arr.Length == 2) return t;
            if (arr.Length == 3)
            {
                if (!double.TryParse(arr[2].Replace("Z", string.Empty), out var sf)) return t;
                return t.AddSeconds(sf);
            }

            if (arr[2].Length < 5)
            {
                var sf = GetNaturalNumber(arr[2], 0, 2);
                return sf > 0 ? t.AddSeconds(sf) : t;
            }

            var sec = GetNaturalNumber(arr[2], 0, 2);
            if (sec < 0 || !int.TryParse(arr[3], out var rm)) return t;
            var neg = arr[2][2] == '-' ? 1 : -1;
            var hasSep = (neg == 1) || (arr[2][2] == '+');
            var rh = GetNaturalNumber(arr[2], hasSep ? 3 : 2);
            return t.AddSeconds(sec).AddMinutes(neg * rm).AddHours(neg * rh);
        }

        /// <summary>
        /// Parses Unix timestamp to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        internal static long ParseUnixTimestamp(DateTime date)
        {
            return ParseDate(date) / 1000;
        }

        /// <summary>
        /// Parses Unix timestamp to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        internal static long? ParseUnixTimestamp(DateTime? date)
        {
            if (!date.HasValue) return null;
            return ParseUnixTimestamp(date.Value);
        }

        /// <summary>
        /// Parses Unix timestamp to date and time.
        /// </summary>
        /// <param name="timestamp">The JavaScript date timestamp.</param>
        /// <returns>A date and time.</returns>
        internal static DateTime ParseUnixTimestamp(long timestamp)
        {
            return ParseDate(timestamp * 1000);
        }

        /// <summary>
        /// Parses Unix timestamp to date and time.
        /// </summary>
        /// <param name="timestamp">The Unix timestamp.</param>
        /// <returns>A date and time.</returns>
        internal static DateTime? ParseUnixTimestamp(long? timestamp)
        {
            if (!timestamp.HasValue) return null;
            return ParseUnixTimestamp(timestamp.Value);
        }

        /// <summary>
        /// Encodes a specific byte array into Base64Url format.
        /// </summary>
        /// <param name="bytes">The value to encode.</param>
        /// <returns>A Base64Url string.</returns>
        public static string Base64UrlEncode(byte[] bytes)
        {
            if (bytes == null) return null;
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", string.Empty);
        }

        /// <summary>
        /// Encodes a specific string into Base64Url format.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <param name="encoding">Optional text encoding.</param>
        /// <returns>A Base64Url string.</returns>
        public static string Base64UrlEncode(string value, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var bytes = (encoding ?? Encoding.UTF8).GetBytes(value);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", string.Empty);
        }

        /// <summary>
        /// Encodes a specific object into JSON Base64Url format.
        /// </summary>
        /// <param name="obj">The object to encode.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>A Base64Url string.</returns>
        public static string Base64UrlEncode(object obj, JsonSerializerOptions options = null)
        {
            if (obj == null) return string.Empty;
            var t = obj.GetType();
            if (t == typeof(string)) return Base64UrlEncode(obj.ToString(), Encoding.UTF8);
            return Base64UrlEncode(InternalHelper.ToJson(obj, options ?? new JsonSerializerOptions
            {
                IgnoreNullValues = true
            }));
        }

        /// <summary>
        /// Decodes the string from a Base64Url format.
        /// </summary>
        /// <param name="s">A Base64Url encoded string.</param>
        /// <returns>A plain text.</returns>
        public static byte[] Base64UrlDecode(string s)
        {
            if (s == null) return null;
            if (s == string.Empty) return Array.Empty<byte>();
            s = s.Replace("-", "+").Replace("_", "/");
            var rest = s.Length % 4;
            if (rest > 0) s = s.PadRight(4 - rest + s.Length, '=');
            var bytes = Convert.FromBase64String(s);
            return bytes;
        }

        /// <summary>
        /// Decodes the string from a Base64Url format.
        /// </summary>
        /// <param name="s">A Base64Url encoded string.</param>
        /// <param name="encoding">Optional text encoding.</param>
        /// <returns>A plain text.</returns>
        public static string Base64UrlDecodeToString(string s, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var bytes = Base64UrlDecode(s);
            return (encoding ?? Encoding.ASCII).GetString(bytes);
        }

        /// <summary>
        /// Decodes and deserializes the object from a JSON Base64Url format.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="s">A Base64Url encoded string.</param>
        /// <param name="options">Options to control the behavior during parsing.</param>
        /// <returns>The object typed.</returns>
        public static T Base64UrlDecodeTo<T>(string s, JsonSerializerOptions options = null)
        {
            if (string.IsNullOrEmpty(s)) return default;
            s = Base64UrlDecodeToString(s, Encoding.UTF8);
            var d = GetJsonDeserializer<T>();
            if (d != null) return d(s);
            return (T)JsonSerializer.Deserialize(s, typeof(T), options);
        }

        /// <summary>
        /// Gets the JSON deserializer.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <returns>A function for deserialization.</returns>
        internal static Func<string, T> GetJsonDeserializer<T>(bool ignoreJsonDoc = false)
        {
            var t = typeof(T);
            if (t == typeof(JsonDocument))
            {
                if (ignoreJsonDoc) return null;
                return str =>
                {
                    if (string.IsNullOrWhiteSpace(str)) return default;
                    return (T)(object)JsonDocument.Parse(str);
                };
            }
            else if (t == typeof(JsonObject))
            {
                if (ignoreJsonDoc) return null;
                return str =>
                {
                    if (string.IsNullOrWhiteSpace(str)) return default;
                    return (T)(object)JsonObject.Parse(str);
                };
            }
            else if (t == typeof(JsonArray))
            {
                if (ignoreJsonDoc) return null;
                return str =>
                {
                    if (string.IsNullOrWhiteSpace(str)) return default;
                    return (T)(object)JsonArray.Parse(str);
                };
            }
            else if (t.FullName.StartsWith("System.Text.Json.Json", StringComparison.InvariantCulture) && t.IsClass)
            {
                try
                {
                    var n = t;
                    if (t.Name.Equals("JsonNode", StringComparison.InvariantCulture))
                    {
                    }
                    else if (t.Name.Equals("JsonObject", StringComparison.InvariantCulture) || t.Name.Equals("JsonArray", StringComparison.InvariantCulture))
                    {
                        n = t.Assembly.GetType("System.Text.Json.JsonNode", false);
                        if (n == null) return null;
                    }
                    else
                    {
                        return null;
                    }

                    var parser = n.GetMethod("Parse", new[] { typeof(string) });
                    if (parser != null && parser.IsStatic)
                    {
                        return str =>
                        {
                            if (string.IsNullOrWhiteSpace(str)) return default;
                            return (T)parser.Invoke(null, new object[] { str });
                        };
                    }

                    parser = n.GetMethod("Parse", new[] { typeof(string), typeof(JsonDocumentOptions) });
                    if (parser != null && parser.IsStatic)
                    {
                        return str =>
                        {
                            if (string.IsNullOrWhiteSpace(str)) return default;
                            return (T)parser.Invoke(null, new object[] { str, default(JsonDocumentOptions) });
                        };
                    }
                }
                catch (AmbiguousMatchException)
                {
                }
                catch (ArgumentException)
                {
                }
            }
            else if (t.FullName.StartsWith("Newtonsoft.Json.Linq.J", StringComparison.InvariantCulture))
            {
                try
                {
                    var parser = t.GetMethod("Parse", new[] { typeof(string) });
                    if (parser != null && parser.IsStatic)
                    {
                        return str =>
                        {
                            if (string.IsNullOrWhiteSpace(str)) return default;
                            return (T)parser.Invoke(null, new object[] { str });
                        };
                    }
                }
                catch (AmbiguousMatchException)
                {
                }
                catch (ArgumentException)
                {
                }
            }
            else if (t == typeof(string))
            {
                return str => (T)(object)str;
            }

            return null;
        }

        /// <summary>
        /// Tries to get the integer from a part of the specific string.
        /// </summary>
        /// <param name="s">A specific string.</param>
        /// <param name="start">The start index of the string to get the integer.</param>
        /// <param name="len">The length to get.</param>
        /// <returns>A natural number; or -1, if failed.</returns>
        private static int GetNaturalNumber(string s, int start, int? len = null)
        {
            const uint ZERO = '0';
            var end = len.HasValue ? Math.Min(start + len.Value, s.Length) : s.Length;
            uint n = 0;
            for (var i = start; i < end; i++)
            {
                var c = s[i];
                var j = c - ZERO;
                if (j > 9) return -1;
                n = n * 10 + j;
            }

            return (int)n;
        }

        internal static T ParseEnum<T>(string s) where T : struct
        {
#if NETOLDVER
            return (T)Enum.Parse(typeof(T), s);
#else
            return Enum.Parse<T>(s);
#endif
        }

        internal static T ParseEnum<T>(string s, bool ignoreCase) where T : struct
        {
#if NETOLDVER
            return (T)Enum.Parse(typeof(T), s, ignoreCase);
#else
            return Enum.Parse<T>(s, ignoreCase);
#endif
        }

        /// <summary>
        /// Gets the indent string.
        /// </summary>
        /// <param name="indentStyle">The indent style.</param>
        /// <param name="indentLevel">The current indent level.</param>
        /// <returns>A string.</returns>
        public static string GetString(IndentStyles indentStyle, int indentLevel = 1)
        {
            if (indentLevel < 1) return indentLevel == 0 ? string.Empty : null;
            var str = indentStyle switch
            {
                IndentStyles.Minified => string.Empty,
                IndentStyles.Empty => string.Empty,
                IndentStyles.Tab => "\t",
                IndentStyles.Space => " ",
                IndentStyles.Compact => "  ",
                IndentStyles.Wide => "        ",
                _ => "    "
            };
            if (indentLevel < 1) return str;
            var sb = new StringBuilder(str);
            for (var i = 1; i < indentLevel; i++)
            {
                sb.Append(str);
            }

            return sb.ToString();
        }

        internal static string SubRangeString(this string s, int start, int end, bool reverseEnd = false)
        {
#if NETOLDVER
            return s.Substring(start, reverseEnd ? (s.Length - end - start) : (end - start));
#else
            return reverseEnd ? s[start..^end] : s[start..end];
#endif
        }

        /// <summary>
        /// Serializes an object into JSON format.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="options">The optional serializer settings.</param>
        /// <returns>A JSON string.</returns>
        internal static string ToJson(object obj, JsonSerializerOptions options = null)
        {
            return ToJson(obj, (o, t) =>
            {
                return JsonSerializer.Serialize(o, t, options);
            });
        }

        /// <summary>
        /// Serializes an object into JSON format.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="options">The optional serializer settings.</param>
        /// <returns>A JSON string.</returns>
        internal static string ToJson(object obj, DataContractJsonSerializerSettings options)
        {
            return ToJson(obj, (o, t) =>
            {
                var serializer = options != null ? new DataContractJsonSerializer(t, options) : new DataContractJsonSerializer(t);
                using var stream = new MemoryStream();
                serializer.WriteObject(stream, o);
                stream.Position = 0;
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return Encoding.UTF8.GetString(bytes);
            });
        }

        private static int GetMonth(string s, int startIndex)
        {
            return s.Substring(startIndex, 3) switch
            {
                "JAN" => 1,
                "FEB" => 2,
                "MAR" => 3,
                "APR" => 4,
                "MAY" => 5,
                "JUN" => 6,
                "JUL" => 7,
                "AUG" => 8,
                "SEP" => 9,
                "OCT" => 10,
                "NOV" => 11,
                "DEC" => 12,
                _ => -1
            };
        }

        /// <summary>
        /// Serializes an object into JSON format.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="converter">The fallback converter.</param>
        /// <returns>A JSON string.</returns>
        private static string ToJson(object obj, Func<object, Type, string> converter)
        {
            if (obj == null) return "null";
            var t = obj.GetType();
            if (obj is JsonDocument jsonDoc)
            {
                return jsonDoc.RootElement.ToString();
            }

            if (obj is JsonElement jsonEle)
            {
                return jsonEle.ToString();
            }

            if (t.FullName.StartsWith("Newtonsoft.Json.Linq.J", StringComparison.InvariantCulture))
            {
                if (t.FullName.Equals("Newtonsoft.Json.Linq.JObject", StringComparison.InvariantCulture)
                    || t.FullName.Equals("Newtonsoft.Json.Linq.JArray", StringComparison.InvariantCulture))
                    return obj.ToString();
            }

            if (t.FullName.StartsWith("System.Text.Json.Json", StringComparison.InvariantCulture) && t.IsClass)
            {
                var method = t.GetMethod("ToJsonString", Type.EmptyTypes);
                if (method != null && !method.IsStatic)
                    return method.Invoke(obj, null)?.ToString();
            }

            if (t.FullName.StartsWith("Trivial.Security.TokenRequest", StringComparison.InvariantCulture))
            {
                try
                {
                    var m = t.GetMethod("ToJsonString", Type.EmptyTypes);
                    if (m != null) return m.Invoke(obj, null)?.ToString();
                }
                catch (ArgumentException)
                {
                }
                catch (AmbiguousMatchException)
                {
                }
                catch (TargetException)
                {
                }
                catch (MemberAccessException)
                {
                }
                catch (NotSupportedException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }

            if (obj is IEnumerable<KeyValuePair<string, string>> col)
            {
                var str = new StringBuilder("{");
                foreach (var kvp in col)
                {
                    str.AppendFormat("\"{0}\":\"{1}\",", JsonString.ToJson(kvp.Key), JsonString.ToJson(kvp.Value));
                }

                str.Remove(str.Length - 1, 1);
                str.Append("}");
                return str.ToString();
            }

            if (t == typeof(string)) return JsonString.ToJson(obj.ToString());
            if (t.FullName.StartsWith("Trivial.Net.HttpUri", StringComparison.InvariantCulture)) return JsonString.ToJson(obj.ToString());
            if (obj is Uri uri)
            {
                try
                {
                    return JsonString.ToJson(uri.OriginalString);
                }
                catch (InvalidOperationException)
                {
                    return JsonString.ToJson(uri.ToString());
                }
            }

            if (obj is IJsonValue)
            {
                return obj.ToString();
            }

            if (t.IsValueType)
            {
                if (obj is bool b)
                    return b ? "true" : "false";
                if (obj is int i32)
                    return i32.ToString("g", CultureInfo.InvariantCulture);
                if (obj is long i64)
                    return i64.ToString("g", CultureInfo.InvariantCulture);
                if (obj is float f1)
                    return f1.ToString("g", CultureInfo.InvariantCulture);
                if (obj is uint i32u)
                    return i32u.ToString("g", CultureInfo.InvariantCulture);
                if (obj is uint i64u)
                    return i64u.ToString("g", CultureInfo.InvariantCulture);
                if (obj is DateTime d)
                    return JsonString.ToJson(d.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                if (obj is DateTimeOffset dto)
                    return JsonString.ToJson(dto.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
                if (obj is double f2)
                    return f2.ToString("g", CultureInfo.InvariantCulture);
                if (obj is TimeSpan ts)
                    return ts.TotalSeconds.ToString("g", CultureInfo.InvariantCulture);

                if (t == typeof(Guid)
                    || t.FullName.StartsWith("Trivial.Maths.Angle", StringComparison.InvariantCulture))
                    return JsonString.ToJson(obj.ToString());
                if (t == typeof(byte)
                    || t == typeof(short)
                    || t == typeof(ushort))
                    return obj.ToString();
            }

            if (t == typeof(DBNull)) return "null";
            return converter(obj, t);
        }

        internal static bool ReplaceBackSlash(StringBuilder sb, StringBuilder backSlash, char c)
        {
            if (backSlash.Length == 0)
            {
                switch (c)
                {
                    case 'x':
                    case 'X':
                    case 'u':
                    case 'U':
                        backSlash.Append(c);
                        return false;
                    case 'R':
                    case 'r':
                        sb.Append('\r');
                        break;
                    case 'N':
                    case 'n':
                        sb.Append('\n');
                        break;
                    case 'A':
                    case 'a':
                        sb.Append('\a');
                        break;
                    case 'B':
                    case 'b':
                        sb.Append('\b');
                        break;
                    case 'T':
                    case 't':
                        sb.Append('\t');
                        break;
                    case 'F':
                    case 'f':
                        sb.Append('\f');
                        break;
                    case 'V':
                    case 'v':
                        sb.Append('\v');
                        break;
                    case '0':
                        sb.Append('\0');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }

                return true;
            }

            var firstBackSlash = backSlash[0];
            int len;
            if (firstBackSlash == 'x' || firstBackSlash == 'X')
            {
                len = 3;
            }
            else if (firstBackSlash == 'u' || firstBackSlash == 'U')
            {
                len = 5;
            }
            else
            {
                return true;
            }

            if (backSlash.Length < len)
            {
                backSlash.Append(c);
                return false;
            }

            try
            {
                var num = Convert.ToInt32(backSlash.ToString().Substring(1), 16);
                sb.Append(char.ConvertFromUtf32(num));
            }
            catch (FormatException)
            {
                sb.Append(backSlash.ToString());
            }
            catch (ArgumentException)
            {
                sb.Append(backSlash.ToString());
            }

            return true;
        }

        internal static string ReplaceBackSlash(IEnumerable<char> s)
        {
            var sb = new StringBuilder();
            StringBuilder backSlash = null;
            foreach (var c in s)
            {
                if (backSlash != null)
                {
                    if (ReplaceBackSlash(sb, backSlash, c)) backSlash = null;
                    continue;
                }

                if (c == '\\')
                {
                    backSlash = new StringBuilder();
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static string ToUpper(string source, CultureInfo culture)
        {
            return culture == null ? source.ToUpper() : source.ToUpper(culture);
        }

        private static string ToLower(string source, CultureInfo culture)
        {
            return culture == null ? source.ToLower() : source.ToLower(culture);
        }
    }
}
