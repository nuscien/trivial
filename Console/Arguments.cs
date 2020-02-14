using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Trivial.Console
{
    /// <summary>
    /// The console arguments object with parsing and deserializing.
    /// </summary>
    public class Arguments: IEquatable<Arguments>, IEquatable<string>, IReadOnlyList<string>
    {
        /// <summary>
        /// Original arguments list.
        /// </summary>
        private readonly List<string> args;

        /// <summary>
        /// The parameters.
        /// </summary>
        private readonly List<Parameter> parameters = new List<Parameter>();

        /// <summary>
        /// Initializes a new instance of the Arguments class.
        /// </summary>
        /// <param name="args">The original arguments.</param>
        public Arguments(IEnumerable<string> args)
        {
            this.args = args.ToList();
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the Arguments class.
        /// </summary>
        /// <param name="args">The original arguments.</param>
        public Arguments(string args)
        {
            var list = args.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            this.args = new List<string>();
            var merge = false;
            foreach (var item in list)
            {
                if (merge)
                {
                    var str = " " + item;
                    if (item.LastIndexOf("\"") == item.Length - 1 && item.LastIndexOf("\\\"") == item.Length - 2)
                    {
                        merge = false;
                        str = item[0..^1];
                    }

                    this.args[this.args.Count - 1] += str;
                }
                else
                {
                    if (item.IndexOf("\"") == 0)
                    {
                        merge = true;
                        this.args.Add(item.Substring(1));
                    }
                    else
                    {
                        this.args.Add(item);
                    }
                }
            }

            Init();
        }

        /// <summary>
        /// Gets the item of arguments splitted by white space.
        /// </summary>
        /// <param name="index">The index of the word of the arguments.</param>
        /// <returns>A word in the arguments.</returns>
        public string this[int index]
        {
            get
            {
                return args[index];
            }
        }

        /// <summary>
        /// Gets the parameter value of arguments.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <returns>A parameter value.</returns>
        public string this[string key]
        {
            get
            {
                key = Parameter.FormatKey(key);
                if (string.IsNullOrEmpty(key)) return Verb.Value;
                foreach (var p in parameters)
                {
                    if (p.Key == key) return p.Value;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the count of word in the arguments.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the verb parameter.
        /// </summary>
        public Parameter Verb { get; private set; }

        /// <summary>
        /// Gets a value indicating whether has the verb.
        /// </summary>
        public bool HasVerb
        {
            get
            {
                return Verb != null;
            }
        }

        /// <summary>
        /// Gets the specific parameters by key.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="additionalKeys">The additional keys including alias and short name.</param>
        /// <returns>A set of parameter.</returns>
        public Parameters Get(string key, params string[] additionalKeys)
        {
            return Get(key, additionalKeys as IEnumerable<string>);
        }

        /// <summary>
        /// Gets the specific parameters by key.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="additionalKeys">The additional keys including alias and short name.</param>
        /// <returns>A set of parameter.</returns>
        public Parameters Get(string key, IEnumerable<string> additionalKeys)
        {
            if (string.IsNullOrWhiteSpace(key)) return new Parameters(string.Empty, new List<Parameter>() {
                Verb
            });
            key = key.ToLower();
            var rest = additionalKeys != null ? additionalKeys.Where(item =>
            {
                return !string.IsNullOrWhiteSpace(item);
            }).Select(item =>
            {
                return Parameter.FormatKey(item, false);
            }) : new List<string>();
            return new Parameters(key, parameters.Where(item =>
            {
                return item.Key == key || rest.Contains(item.Key);
            }), rest);
        }

        /// <summary>
        /// Get the first parameter of the specific key.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <returns>A parameter matched.</returns>
        public Parameter GetFirst(params string[] key)
        {
            var keys = key.Where(item =>
            {
                return !string.IsNullOrWhiteSpace(item);
            }).Select(item =>
            {
                return Parameter.FormatKey(item, false);
            }).ToList();
            if (keys.Count == 0) return Verb;
            foreach (var p in parameters)
            {
                if (keys.IndexOf(p.Key) >= 0) return p;
            }

            return null;
        }

        /// <summary>
        /// Determines whether there is a parameter which is matched the specific key.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <returns>true if has such parameter; otherwise, false.</returns>
        public bool Has(params string[] key)
        {
            foreach (var k in key)
            {
                var str = Parameter.FormatKey(k, false);
                if (string.IsNullOrWhiteSpace(str)) return Verb != null;
                foreach (var p in parameters)
                {
                    if (p.Key == str) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether a word is in the arguments.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return args.Contains(key);
        }

        /// <summary>
        /// Converts the value of this instance to the arguments string.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var item in args)
            {
                if (str.Length > 0) str.Append(' ');
                str.Append(item);
            }

            return str.ToString();
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Determines whether the value of this instance and the specified one have the same value.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
        public bool Equals(Arguments other)
        {
            if (other == null) return false;
            return ToString() == other.ToString();
        }

        /// <summary>
        /// Determines whether this instance and the specified one have the same value.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
        public bool Equals(string other)
        {
            return ToString() == other;
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>A enumerator.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return args.GetEnumerator();
        }

        /// <summary>
        /// Deserializes the value of this instance to a specific object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">An object.</param>
        /// <param name="overrideProperty">true if override the properties which they have values; otherwise, false.</param>
        public void Deserialize<T>(T obj, bool overrideProperty = true) where T : class
        {
            if (obj == null) return;
            foreach (var item in obj.GetType().GetProperties())
            {
                if (!item.CanWrite) continue;
                if (item.CanRead && !overrideProperty && item.GetValue(obj) != null) continue;
                var atts = item.GetCustomAttributes(typeof(ArgumentAttribute), true);
                if (atts == null || atts.Length == 0) continue;
                if (!(atts[0] is ArgumentAttribute att) || string.IsNullOrWhiteSpace(att.Name)) continue;
                var attName = Parameter.FormatKey(att.Name, false);
                if (string.IsNullOrWhiteSpace(attName)) attName = item.Name.ToLower();
                var restNames = new List<string>();
                if (att.Short) restNames.Add(attName.Substring(0, 1));
                if (!string.IsNullOrWhiteSpace(att.SecondaryName)) restNames.Add(att.SecondaryName);
                if (!string.IsNullOrWhiteSpace(att.AnotherSecondaryName)) restNames.Add(att.AnotherSecondaryName);
                var p = Get(attName, restNames);
                if (p.ItemCount == 0) continue;
                var itemType = item.PropertyType;
                if (item == null && itemType.IsClass)
                {
                    item.SetValue(obj, null);
                }
                else if (itemType == typeof(string))
                {
                    item.SetValue(obj, p.Value(att.Mode));
                }
                else if (itemType == typeof(bool))
                {
                    if (p.TryToParse(out bool itemValue, att.Mode)) item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(int))
                {
                    if (p.TryToParse(out int itemValue, att.Mode)) item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(long))
                {
                    if (p.TryToParse(out long itemValue, att.Mode)) item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(float))
                {
                    if (p.TryToParse(out float itemValue, att.Mode)) item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(double))
                {
                    if (p.TryToParse(out double itemValue, att.Mode)) item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(Guid))
                {
                    if (p.TryToParse(out Guid itemValue, att.Mode)) item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(DateTime))
                {
                    if (p.TryToParse(out DateTime itemValue, att.Mode)) item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(DateTimeOffset))
                {
                    if (p.TryToParse(out DateTimeOffset itemValue, att.Mode)) item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(Uri))
                {
                    var itemValue = p.ParseToUri(att.Mode);
                    item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(FileInfo))
                {
                    var itemValue = p.ParseToFileInfo(att.Mode);
                    item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(DirectoryInfo))
                {
                    var itemValue = p.ParseToDirectoryInfo(att.Mode);
                    item.SetValue(obj, itemValue);
                }
            }
        }

        /// <summary>
        /// Deserializes the value of this instance to an object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>An object with properties from this instance.</returns>
        public T Deserialize<T>() where T : class
        {
            var obj = Activator.CreateInstance<T>();
            Deserialize(obj);
            return obj;
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>A enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return args.GetEnumerator();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Init()
        {
            Count = args.Count;
            if (Count == 0) return;
            var list = new List<int>();
            for (var i = 0; i < args.Count; i++)
            {
                if (args[i].IndexOf('-') == 0 || args[i].IndexOf('/') == 0) list.Add(i);
            }

            if (args[0].IndexOf('-') != 0 && args[0].IndexOf('/') != 0)
            {
                var len = args.Count;
                if (list.Count > 0) len = list[0];
                Verb = new Parameter(args[0], args.Take(len).Skip(1));
            }

            for (var i = 0; i < list.Count; i++)
            {
                var index = list[i];
                IEnumerable<string> rest = null;
                var len = args.Count;
                if (i + 1 < list.Count) len = list[i + 1];
                len -= index + 1;
                if (len > 0) rest = args.Skip(index + 1).Take(len);
                parameters.Add(new Parameter(args[index], rest));
            }
        }
    }
}
