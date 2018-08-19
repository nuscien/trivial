using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Trivial.Console
{
    public class Arguments: IEquatable<Arguments>, IEquatable<string>, IReadOnlyList<string>
    {
        private List<string> args;

        private List<Parameter> parameters = new List<Parameter>();

        public Arguments(IEnumerable<string> args)
        {
            this.args = args.ToList();
            Init();
        }

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
                        str = item.Substring(0, item.Length - 1);
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

        public string this[int index]
        {
            get
            {
                return args[index];
            }
        }

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

        public int Count { get; private set; }

        public Parameter Verb { get; private set; }

        public Parameters Get(string key, params string[] additionalKeys)
        {
            return Get(key, additionalKeys);
        }

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

        public bool Has(string key)
        {
            key = key.ToLower();
            if (string.IsNullOrWhiteSpace(key)) return Verb != null;
            return parameters.Count(item =>
            {
                return item.Key == key;
            }) > 0;
        }

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

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(Arguments other)
        {
            if (other == null) return false;
            return ToString() == other.ToString();
        }

        public bool Equals(string other)
        {
            return ToString() == other;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return args.GetEnumerator();
        }

        public void Deserialize<T>(T obj, bool overrideProperty = true)
        {
            foreach (var item in typeof(T).GetProperties())
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
                var p = Get(attName, restNames);
                if (p.ItemCount == 0) continue;
                var itemType = item.GetType();
                if (itemType == typeof(string))
                {
                    item.SetValue(obj, p.Value(att.Mode));
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

        public T Deserialize<T>()
        {
            var obj = Activator.CreateInstance<T>();
            Deserialize(obj);
            return obj;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return args.GetEnumerator();
        }

        private void Init()
        {
            Count = args.Count;
            if (Count == 0) return;
            var list = new List<int>();
            for (var i = 0; i < args.Count; i++)
            {
                if (args[i].IndexOf('-') == 0) list.Add(i);
            }

            if (args[0].IndexOf('-') != 0)
            {
                var len = args.Count;
                if (list.Count > 0) len = list[0];
                Verb = new Parameter(args[0], args.Take(len).Skip(1));
            }

            foreach (var i in list)
            {
                IEnumerable<string> rest = null;
                var len = args.Count;
                if (len > i + 1)
                {
                    if (list.Count > i) len = list[i + 1];
                    len -= i + 1;
                    rest = args.Skip(i + 1).Take(len);
                }

                parameters.Add(new Parameter(args[i], rest));
            }
        }
    }
}
