using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Trivial.Collection
{
    /// <summary>
    /// Key value pairs.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class KeyValuePairs<TKey, TValue> : List<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Adds an object to the end of the key value pairs.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Inserts an element into the key value pairs at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Insert(int index, TKey key, TValue value)
        {
            Insert(index, new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <returns>The keys.</returns>
        public IEnumerable<TKey> Keys()
        {
            return this.Select(item => item.Key).Distinct();
        }

        /// <summary>
        /// Gets the values by a specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The values.</returns>
        public IEnumerable<TValue> Values(TKey key)
        {
            return key != null ? this.Where(item => key.Equals(item.Key)).Select(item => item.Value) : this.Where(item => item.Key == null).Select(item => item.Value);
        }

        /// <summary>
        /// Removes all the elements by the specific key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>The number of elements removed from the key value pairs.</returns>
        public int Remove(TKey key)
        {
            return key == null ? RemoveAll(item => item.Key == null) : RemoveAll(item => key.Equals(item.Key));
        }

        /// <summary>
        /// Groups the key value pairs.
        /// </summary>
        /// <returns>The groups.</returns>
        public IEnumerable<IGrouping<TKey, TValue>> ToGroup()
        {
            return this.GroupBy(item => item.Key, item => item.Value);
        }

        /// <summary>
        /// Creates a dictionary from the key value pairs.
        /// </summary>
        /// <returns>A dictionary with key and the value collection.</returns>
        public Dictionary<TKey, IEnumerable<TValue>> ToDictionary()
        {
            return ToGroup().ToDictionary(item => item.Key, item => item as IEnumerable<TValue>);
        }
    }

    /// <summary>
    /// The key value pairs with string key and string value.
    /// </summary>
    public class StringKeyValuePairs : KeyValuePairs<string, string>
    {
        /// <summary>
        /// Gets the equal sign for key and value.
        /// </summary>
        public virtual string EqualSign => "=";

        /// <summary>
        /// Gets the separator.
        /// </summary>
        public virtual string Separator => "&";

        /// <summary>
        /// Encodes the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The key encoded.</returns>
        protected virtual string EncodeKey(string key)
        {
            return HttpUtility.UrlEncode(key);
        }

        /// <summary>
        /// Encodes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value encoded.</returns>
        protected virtual string EncodeValue(string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A query string.</returns>
        public override string ToString()
        {
            var arr = new List<string>();
            foreach (var item in this)
            {
                arr.Add(string.Format("{0}{2}{1}", EncodeKey(item.Key), EncodeValue(item.Value), EqualSign));
            }

            return string.Join(Separator, arr);
        }
    }
}
