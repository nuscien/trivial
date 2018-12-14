using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
}
