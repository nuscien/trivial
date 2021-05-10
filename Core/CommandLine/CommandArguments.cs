using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

using Trivial.Collection;
using Trivial.Net;
using Trivial.Text;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The console arguments object with parsing and deserializing.
    /// </summary>
    public class CommandArguments : IEquatable<CommandArguments>, IEquatable<string>, IReadOnlyList<string>
    {
        /// <summary>
        /// Original arguments list.
        /// </summary>
        private readonly List<string> args;

        /// <summary>
        /// The parameters.
        /// </summary>
        private readonly List<CommandParameter> parameters = new();

        /// <summary>
        /// Initializes a new instance of the Arguments class.
        /// </summary>
        /// <param name="args">The original arguments.</param>
        public CommandArguments(IEnumerable<string> args)
        {
            if (args == null)
            {
                this.args = new List<string>();
                return;
            }

            this.args = args.ToList();
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the Arguments class.
        /// </summary>
        /// <param name="args">The original arguments.</param>
        public CommandArguments(string args)
        {
            if (args == null)
            {
                this.args = new List<string>();
                return;
            }

            var list = args.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            this.args = new List<string>();
            var ignoreKeys = new List<int>();
            var merge = false;
            if (list.Length == 1 && !list[0].StartsWith("-") && !list[0].StartsWith("/") && list[0].IndexOf("&") > 0 && list[0].IndexOf("=") > 0)
            {
                var q = QueryData.Parse(list[0]).ToGroups();
                foreach (var p in q)
                {
                    parameters.Add(new CommandParameter(p.Key, p));
                }

                return;
            }

            foreach (var item in list)
            {
                #pragma warning disable IDE0056, IDE0057
                if (merge)
                {
                    var str = " " + item;
                    if (item.LastIndexOf("\"") == item.Length - 1 && item.LastIndexOf("\\\"") == item.Length - 2)
                    {
                        merge = false;
                        str = item.Substring(0, item.Length - 1);
                    }

                    this.args[this.args.Count - 1] += str;
                }
                else
                {
                    if (item.StartsWith("\""))
                    {
                        ignoreKeys.Add(this.args.Count);
                        if (item.Length > 1 && item.EndsWith("\""))
                        {
                            this.args.Add(item.Substring(1, item.Length - 2));
                        }
                        else
                        {
                            merge = true;
                            this.args.Add(item.Substring(1));
                        }
                    }
                    else
                    {
                        this.args.Add(item);
                    }
                }
                #pragma warning restore IDE0056, IDE0057
            }

            Init(ignoreKeys);
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
                key = CommandParameter.FormatKey(key);
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
        public CommandParameter Verb { get; private set; }

        /// <summary>
        /// Gets a value indicating whether has the verb.
        /// </summary>
        public bool HasVerb => Verb != null;

        /// <summary>
        /// Gets the specific parameters by key.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="additionalKeys">The additional keys including alias and short name.</param>
        /// <returns>A set of parameter.</returns>
        public CommandParameters Get(string key, params string[] additionalKeys)
        {
            return Get(key, additionalKeys as IEnumerable<string>);
        }

        /// <summary>
        /// Gets the specific parameters by key.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="additionalKeys">The additional keys including alias and short name.</param>
        /// <returns>A set of parameter.</returns>
        public CommandParameters Get(string key, IEnumerable<string> additionalKeys)
        {
            if (string.IsNullOrWhiteSpace(key)) return new CommandParameters(string.Empty, new List<CommandParameter>() {
                Verb
            });
            key = key.ToLower();
            var rest = additionalKeys != null ? additionalKeys.Where(item =>
            {
                return !string.IsNullOrWhiteSpace(item);
            }).Select(item =>
            {
                return CommandParameter.FormatKey(item, false);
            }) : new List<string>();
            return new CommandParameters(key, parameters.Where(item =>
            {
                return item.Key == key || rest.Contains(item.Key);
            }), rest);
        }

        /// <summary>
        /// Get the first parameter of the specific key.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <returns>A parameter matched.</returns>
        public CommandParameter GetFirst(params string[] key)
        {
            var keys = key.Where(item =>
            {
                return !string.IsNullOrWhiteSpace(item);
            }).Select(item =>
            {
                return CommandParameter.FormatKey(item, false);
            }).ToList();
            if (keys.Count == 0) return Verb;
            foreach (var p in parameters)
            {
                if (keys.IndexOf(p.Key) >= 0) return p;
            }

            return null;
        }

        /// <summary>
        /// Get the merged value of the specific key.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <returns>A string value.</returns>
        public string GetMergedValue(string key) => Get(key)?.MergedValue;

        /// <summary>
        /// Get the parameter before the specific key.
        /// </summary>
        /// <param name="key">The parameter key to get previous.</param>
        /// <returns>A parameter before the specific one.</returns>
        public CommandParameter GetPrevious(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return parameters.FirstOrDefault();
            key = CommandParameter.FormatKey(key, false);
            CommandParameter cur;
            foreach (var p in parameters)
            {
                cur = p;
                if (p.Key == key) return cur;
            }

            return null;
        }

        /// <summary>
        /// Gets the parameter before the specific one.
        /// </summary>
        /// <param name="parameter">The specific parameter to get its previous.</param>
        /// <returns>A parameter before the specific one.</returns>
        public CommandParameter GetPrevious(CommandParameter parameter)
        {
            if (parameter is null) return null;
            var i = parameters.IndexOf(parameter);
            if (i < 0) return null;
            if (i == 0) return Verb;
            return parameters[i - 1];
        }

        /// <summary>
        /// Get the next parameter of the specific key.
        /// </summary>
        /// <param name="key">The parameter key to get next.</param>
        /// <returns>A parameter after the specific one.</returns>
        public CommandParameter GetNext(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return parameters.FirstOrDefault();
            key = CommandParameter.FormatKey(key, false);
            var needReturn = false;
            foreach (var p in parameters)
            {
                if (needReturn) return p;
                if (p.Key == key) needReturn = true;
            }

            return null;
        }

        /// <summary>
        /// Gets the next parameter after the specific one.
        /// </summary>
        /// <param name="parameter">The specific parameter to get its next.</param>
        /// <returns>A parameter after the specific one.</returns>
        public CommandParameter GetNext(CommandParameter parameter)
        {
            if (parameter is null) return parameters.FirstOrDefault();
            var i = parameters.IndexOf(parameter);
            if (i < 0 || i >= (parameter.Count - 1)) return null;
            return parameters[i + 1];
        }

        /// <summary>
        /// Get the a parameter of the specific key, or the next one if it is empty.
        /// </summary>
        /// <param name="key">The parameter key to get next.</param>
        /// <param name="appendNextAsValue">true if append the next parameter as the value of the specific key; otherwise, false.</param>
        /// <param name="nextPrefix">An optional prefix character used to test the next parameter.</param>
        /// <returns>The specific parameter; or the parameter after the specific one if the specific one is empty; or the specific parameter if the next is empty; or null if non-exist.</returns>
        public CommandParameter GetFirstOrNext(string key, bool appendNextAsValue = false, char? nextPrefix = null)
        {
            if (string.IsNullOrWhiteSpace(key)) return parameters.FirstOrDefault();
            key = CommandParameter.FormatKey(key, false);
            CommandParameter cur = null;
            foreach (var p in parameters)
            {
                if (cur != null)
                {
                    if (nextPrefix.HasValue)
                    {
                        if (string.IsNullOrEmpty(p.OriginalKey))
                        {
                            if (p.IsEmpty || p.Value.FirstOrDefault() != nextPrefix.Value) return null;
                        }
                        else
                        {
                            if (p.OriginalKey.First() != nextPrefix.Value) return null;
                        }
                    }

                    if (!appendNextAsValue) return p.IsEmpty ? cur : p;
                    var list = new List<string>();
                    if (!string.IsNullOrEmpty(p.OriginalKey)) list.Add(p.OriginalKey);
                    if (!p.IsEmpty) list.AddRange(p.Values);
                    return new CommandParameter(key, list);
                }

                if (p.Key != key) continue;
                if (!p.IsEmpty) return p;
                cur = p;
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
                var str = CommandParameter.FormatKey(k, false);
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
        public bool Equals(CommandArguments other)
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

        /// <inheritdoc/>>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is CommandArguments a) return Equals(a);
            if (obj is string s) return Equals(s);
            return false;
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
                var atts = item.GetCustomAttributes(typeof(CommandArgumentAttribute), true);
                if (atts == null || atts.Length == 0) continue;
                if (!(atts[0] is CommandArgumentAttribute att) || string.IsNullOrWhiteSpace(att.Name)) continue;
                var attName = CommandParameter.FormatKey(att.Name, false);
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
                else if (itemType == typeof(HttpUri))
                {
                    var itemValue = p.ParseToHttpUri(att.Mode);
                    item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(QueryData))
                {
                    var itemValue = p.ParseToQueryData(att.Mode);
                    item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(JsonObject))
                {
                    var itemValue = p.ParseToJsonObject(att.Mode);
                    item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(JsonArray))
                {
                    var itemValue = p.ParseToJsonArray(att.Mode);
                    item.SetValue(obj, itemValue);
                }
                else if (itemType == typeof(JsonDocument))
                {
                    var itemValue = p.ParseToJsonDocument(att.Mode);
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
        private void Init(List<int> ignoreKeys = null)
        {
            Count = args.Count;
            if (Count == 0) return;
            var list = new List<int>();
            if (ignoreKeys == null) ignoreKeys = new List<int>();
            for (var i = 0; i < args.Count; i++)
            {
                if ((args[i].IndexOf('-') == 0 || args[i].IndexOf('/') == 0)
                    && !ignoreKeys.Contains(i)
                    && args[i].IndexOf(' ') < 0
                    && args[i].IndexOf('\r') < 0
                    && args[i].IndexOf('\n') < 0
                    && args[i].IndexOf('\t') < 0
                    && args[i].LastIndexOf('/') <= 0) list.Add(i);
            }

            if (args[0].IndexOf('-') != 0 && args[0].IndexOf('/') != 0)
            {
                var len = args.Count;
                if (list.Count > 0) len = list[0];
                Verb = new CommandParameter(args[0], args.Take(len).Skip(1));
            }

            for (var i = 0; i < list.Count; i++)
            {
                var index = list[i];
                IEnumerable<string> rest = null;
                var len = args.Count;
                if (i + 1 < list.Count) len = list[i + 1];
                len -= index + 1;
                if (len > 0) rest = args.Skip(index + 1).Take(len);
                parameters.Add(new CommandParameter(args[index], rest));
            }
        }
    }
}
