using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Trivial.Console
{
    public class Parameter: IEquatable<Parameter>, IEquatable<Parameters>, IEquatable<string>, IReadOnlyList<string>
    {
        public Parameter(string key, IEnumerable<string> rest)
        {
            OriginalKey = key;
            Key = FormatKey(key);
            Values = (rest != null ? rest.Where(item =>
            {
                return item != null;
            }).ToList() : new List<string>()).AsReadOnly();
            Count = Values.Count;
            IsEmpty = Count == 0;
            if (IsEmpty)
            {
                Value = string.Empty;
                return;
            }

            var str = new StringBuilder(Values[0]);
            for (var i = 1; i < Values.Count; i++)
            {
                str.Append(' ');
                str.Append(Values[i]);
            }

            Value = str.ToString();
        }

        public string Key { get; }

        public string OriginalKey { get; }

        public string Value { get; }

        public bool IsEmpty { get; }

        public IReadOnlyList<string> Values { get; }

        public int Count { get; }

        public string this[int index]
        {
            get
            {
                return Values[index];
            }
        }

        public bool TryToParse(out int result)
        {
            return int.TryParse(Value, out result);
        }

        public bool TryToParse(out long result)
        {
            return long.TryParse(Value, out result);
        }

        public bool TryToParse(out float result)
        {
            return float.TryParse(Value, out result);
        }

        public bool TryToParse(out double result)
        {
            return double.TryParse(Value, out result);
        }

        public bool TryToParse(out Guid result)
        {
            return Guid.TryParse(Value, out result);
        }

        public bool TryToParse(out DateTime result)
        {
            return DateTime.TryParse(Value, out result);
        }

        public bool TryToParse(out DateTimeOffset result)
        {
            return DateTimeOffset.TryParse(Value, out result);
        }

        public Uri ParseToUri()
        {
            return new Uri(Value);
        }

        public FileInfo ParseToFileInfo()
        {
            return new FileInfo(Value);
        }

        public DirectoryInfo ParseToDirectoryInfo()
        {
            return new DirectoryInfo(Value);
        }

        public override string ToString()
        {
            var str = new StringBuilder(OriginalKey);
            if (string.IsNullOrEmpty(Value)) return str.ToString();
            if (str.Length > 0) str.Append(' ');
            str.Append(Value);
            return str.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(Parameter other)
        {
            if (other == null) return false;
            return ToString() == other.ToString();
        }

        public bool Equals(Parameters other)
        {
            if (other == null) return false;
            return ToString() == other.ToString();
        }

        public bool Equals(string other)
        {
            return ToString() == other;
        }

        internal static string FormatKey(string key, bool removePrefix = true)
        {
            if (key == null) return null;
            key = key.Trim();
            if (removePrefix)
            {
                if (key.IndexOf("--") == 0) return key.Substring(2).ToLower();
                if (key.IndexOf('-') == 0) return key.Substring(1).ToLower();
            }

            return key.ToLower();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }

    public class Parameters: IEquatable<Parameters>, IEquatable<Parameter>, IEquatable<string>, IEnumerable<Parameter>
    {
        public Parameters(string key, IEnumerable<Parameter> value, IEnumerable<string> additionalKeys = null)
        {
            Key = key;
            Items = (value != null ? value.Where(item =>
            {
                return item != null;
            }).ToList() : new List<Parameter>()).AsReadOnly();
            ItemCount = Items.Count;
            if (ItemCount > 0) FirstItem = Items[0];
            AdditionalKeys = (additionalKeys != null ? additionalKeys.ToList() : new List<string>()).AsReadOnly();
            var keys = new List<string>
            {
                Key
            };
            keys.AddRange(AdditionalKeys);
            AllKeys = keys.AsReadOnly();
            IsEmpty = Items.Count == 0;
            if (IsEmpty)
            {
                FirstValues = (new List<string>()).AsReadOnly();
                FirstValue = string.Empty;
                MergedValues = (new List<string>()).AsReadOnly();
                MergedValue = string.Empty;
            }

            FirstValues = Items[0].Values;
            FirstValue = Items[0].Value;
            var mergedList = new List<string>();
            var mergedStr = new StringBuilder();
            foreach (var item in Items)
            {
                mergedList.AddRange(item.Values);
                if (mergedStr.Length > 0) mergedStr.Append(' ');
                mergedStr.Append(item.Value);
            }
        }

        public string Key { get; }

        public IReadOnlyList<string> AllKeys { get; }

        public bool IsEmpty { get; }

        public IReadOnlyList<string> AdditionalKeys { get; }

        public IReadOnlyList<Parameter> Items { get; }

        public Parameter FirstItem { get; }

        public int ItemCount { get; }

        public string FirstValue { get; }

        public IReadOnlyList<string> FirstValues { get; }

        public string MergedValue { get; }

        public IReadOnlyList<string> MergedValues { get; }

        public string Value(ParameterMode mode = ParameterMode.First)
        {
            switch (mode)
            {
                case ParameterMode.All:
                    return MergedValue;
                case ParameterMode.Last:
                    if (ItemCount == 0) return string.Empty;
                    return Items[ItemCount - 1].Value;
                default:
                    return FirstValue;
            }
        }

        public bool TryToParse(out int result, ParameterMode mode = ParameterMode.First)
        {
            return int.TryParse(Value(mode), out result);
        }

        public bool TryToParse(out long result, ParameterMode mode = ParameterMode.First)
        {
            return long.TryParse(Value(mode), out result);
        }

        public bool TryToParse(out float result, ParameterMode mode = ParameterMode.First)
        {
            return float.TryParse(Value(mode), out result);
        }

        public bool TryToParse(out double result, ParameterMode mode = ParameterMode.First)
        {
            return double.TryParse(Value(mode), out result);
        }

        public bool TryToParse(out Guid result, ParameterMode mode = ParameterMode.First)
        {
            return Guid.TryParse(Value(mode), out result);
        }

        public bool TryToParse(out DateTime result, ParameterMode mode = ParameterMode.First)
        {
            return DateTime.TryParse(Value(mode), out result);
        }

        public bool TryToParse(out DateTimeOffset result, ParameterMode mode = ParameterMode.First)
        {
            return DateTimeOffset.TryParse(Value(mode), out result);
        }

        public Uri ParseToUri(ParameterMode mode = ParameterMode.First)
        {
            return new Uri(Value(mode));
        }

        public FileInfo ParseToFileInfo(ParameterMode mode = ParameterMode.First)
        {
            return new FileInfo(Value(mode));
        }

        public DirectoryInfo ParseToDirectoryInfo(ParameterMode mode = ParameterMode.First)
        {
            return new DirectoryInfo(Value(mode));
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var item in Items)
            {
                if (str.Length > 0) str.Append(' ');
                str.Append(item.ToString());
            }

            return str.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(Parameters other)
        {
            if (other == null) return false;
            return ToString() == other.ToString();
        }

        public bool Equals(Parameter other)
        {
            if (other == null) return false;
            return ToString() == other.ToString();
        }

        public bool Equals(string other)
        {
            return ToString() == other;
        }

        public IEnumerator<Parameter> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
