using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Trivial.Collection
{
    /// <summary>
    /// The key value pairs with string key and string value.
    /// </summary>
    public class StringKeyValuePairs : List<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Initializes a new instance of the StringKeyValuePairs class.
        /// </summary>
        public StringKeyValuePairs() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the StringKeyValuePairs class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public StringKeyValuePairs(IEnumerable<KeyValuePair<string, string>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Gets the equal sign for key and value.
        /// </summary>
        public virtual string EqualSign => "=";

        /// <summary>
        /// Gets the separator which is used between each key values.
        /// </summary>
        public virtual string Separator => "&";

        /// <summary>
        /// Gets the separator which is used between each values for a key if has more than one.
        /// </summary>
        public virtual string ValueSeparator => ",";

        /// <summary>
        /// Gets or sets the value of the specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A value of the specific key.</returns>
        public string this[string key]
        {
            get
            {
                return GetValue(key);
            }

            set
            {
                ListExtensions.Set(this, key, value);
            }
        }

        /// <summary>
        /// Gets the query value by a specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="separator">An optional string to use as a separator is included in the returned string only if it has more than one value.</param>
        /// <returns>The query value.</returns>
        public string GetValue(string key, string separator = null)
        {
            var arr = ListExtensions.GetValues(this, key).ToList();
            if (arr.Count == 0) return null;
            return string.Join(separator ?? ValueSeparator ?? string.Empty, arr);
        }

        /// <summary>
        /// Gets the query value by a specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ignoreEmpty">true if ignore empty; otherwise, false.</param>
        /// <returns>The query value.</returns>
        public string GetFirstValue(string key, bool ignoreEmpty = false)
        {
            var col = ListExtensions.GetValues(this, key);
            if (ignoreEmpty) col = col.Where(item => !string.IsNullOrWhiteSpace(item));
            return col.FirstOrDefault();
        }

        /// <summary>
        /// Gets the query value by a specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ignoreEmpty">true if ignore empty; otherwise, false.</param>
        /// <returns>The query value. The last one for multiple values..</returns>
        public string GetLastValue(string key, bool ignoreEmpty = false)
        {
            var col = ListExtensions.GetValues(this, key);
            if (ignoreEmpty) col = col.Where(item => !string.IsNullOrWhiteSpace(item));
            return col.LastOrDefault();
        }

        /// <summary>
        /// Gets the query value as an interger by a specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The query value as Int32.</returns>
        public int? TryGetInt32Value(string key)
        {
            var v = GetFirstValue(key, true);
            if (v != null && int.TryParse(v, out int result)) return result;
            return null;
        }

        /// <summary>
        /// Gets the query value as an enumeration by a specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ignoreCase">true to ignore case; false to regard case; null to use default settings.</param>
        /// <returns>The query value as Enum.</returns>
        /// <exception cref="ArgumentException">TEnum is not an enumeration type.</exception>
        public TEnum? TryGetEnumValue<TEnum>(string key, bool? ignoreCase = null) where TEnum : struct
        {
            var v = GetFirstValue(key, true);
            if (v == null) return null;
            if (ignoreCase.HasValue)
            {
                if (Enum.TryParse(v, ignoreCase.Value, out TEnum result)) return result;
            }
            else
            {
                if (Enum.TryParse(v, out TEnum result)) return result;
            }

            return null;
        }

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

        /// <summary>
        /// Adds 2 elements.
        /// </summary>
        /// <param name="a">Left element.</param>
        /// <param name="b">Right element.</param>
        /// <returns>The result.</returns>
        public static StringKeyValuePairs operator +(StringKeyValuePairs a, IEnumerable<KeyValuePair<string, string>> b)
        {
            if (a == null && b == null) return null;
            var col = new StringKeyValuePairs();
            if (a != null) col.AddRange(a);
            if (b != null) col.AddRange(b);
            return col;
        }

        /// <summary>
        /// Adds 2 elements.
        /// </summary>
        /// <param name="a">Left element.</param>
        /// <param name="b">Right element.</param>
        /// <returns>The result.</returns>
        public static StringKeyValuePairs operator +(StringKeyValuePairs a, KeyValuePair<string, string> b)
        {
            if (a == null) return null;
            var col = new StringKeyValuePairs();
            if (a != null) col.AddRange(a);
            col.Add(b);
            return col;
        }

        /// <summary>
        /// Deletes.
        /// </summary>
        /// <param name="a">Left element.</param>
        /// <param name="b">Right element.</param>
        /// <returns>The result.</returns>
        public static StringKeyValuePairs operator -(StringKeyValuePairs a, IEnumerable<KeyValuePair<string, string>> b)
        {
            if (a == null) return null;
            var col = new StringKeyValuePairs();
            col.AddRange(a);
            if (b == null) return col;
            foreach (var kvp in b)
            {
                a.Remove(kvp.Key, kvp.Value);
            }

            return col;
        }

        /// <summary>
        /// Deletes.
        /// </summary>
        /// <param name="a">Left element.</param>
        /// <param name="b">Right element.</param>
        /// <returns>The result.</returns>
        public static StringKeyValuePairs operator -(StringKeyValuePairs a, KeyValuePair<string, string> b)
        {
            if (a == null) return null;
            var col = new StringKeyValuePairs();
            col.AddRange(a);
            a.Remove(b.Key, b.Value);
            return col;
        }
    }
}
