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
                ListUtility.SetValue(this, key, value);
            }
        }

        /// <summary>
        /// Gets the query value by a specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="separator">The optional string to use as a separator is included in the returned string only if it has more than one value.</param>
        /// <returns>The query value.</returns>
        public string GetValue(string key, string separator = null)
        {
            return string.Join(separator ?? ValueSeparator ?? string.Empty, ListUtility.Values(this, key));
        }

        /// <summary>
        /// Gets the query value by a specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ignoreEmpty">true if ignore empty; otherwise, false.</param>
        /// <returns>The query value.</returns>
        public string GetFirstValue(string key, bool ignoreEmpty = false)
        {
            return ListUtility.Values(this, key).Where(item => !string.IsNullOrWhiteSpace(item)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the query value by a specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ignoreEmpty">true if ignore empty; otherwise, false.</param>
        /// <returns>The query value. The last one for multiple values..</returns>
        public string GetLastValue(string key, bool ignoreEmpty = false)
        {
            return ListUtility.Values(this, key).Where(item => !string.IsNullOrWhiteSpace(item)).LastOrDefault();
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
    }
}
