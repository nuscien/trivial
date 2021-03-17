using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Trivial.Console
{
    /// <summary>
    /// The arguments parameter.
    /// </summary>
    public class Parameter: IEquatable<Parameter>, IEquatable<Parameters>, IEquatable<string>, IReadOnlyList<string>
    {
        /// <summary>
        /// Initializes a new instance of the Parameter class.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="values">The words of value.</param>
        public Parameter(string key, IEnumerable<string> values)
        {
            OriginalKey = key;
            Key = FormatKey(key);
            Values = (values != null ? values.Where(item =>
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

        /// <summary>
        /// Gets the parameter key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the original parameter key.
        /// </summary>
        public string OriginalKey { get; }

        /// <summary>
        /// Gets the value string.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Get a value indicating whether the value is empty.
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// Gets the words of the value.
        /// </summary>
        public IReadOnlyList<string> Values { get; }

        /// <summary>
        /// Gets the count of words of the value.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the specific word of the value.
        /// </summary>
        /// <param name="index">The index of the word.</param>
        /// <returns>A word in the value.</returns>
        public string this[int index]
        {
            get
            {
                return Values[index];
            }
        }

        /// <summary>
        /// Converts the value to its boolean equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryParse(out bool result)
        {
            if (!string.IsNullOrWhiteSpace(Value)) return bool.TryParse(Value, out result);
            result = true;
            return true;
        }

        /// <summary>
        /// Converts the value to its 32-bit signed integer equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryParse(out int result)
        {
            return int.TryParse(Value, out result);
        }

        /// <summary>
        /// Converts the value to its 64-bit signed integer equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryParse(out long result)
        {
            return long.TryParse(Value, out result);
        }

        /// <summary>
        /// Converts the value to its single-precision floating-point number equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryParse(out float result)
        {
            return float.TryParse(Value, out result);
        }

        /// <summary>
        /// Converts the value to its double-precision floating-point number equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryParse(out double result)
        {
            return double.TryParse(Value, out result);
        }

        /// <summary>
        /// Converts the value to its GUID equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryParse(out Guid result)
        {
            return Guid.TryParse(Value, out result);
        }

        /// <summary>
        /// Converts the value to its date and time equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryParse(out DateTime result)
        {
            return DateTime.TryParse(Value, out result);
        }

        /// <summary>
        /// Converts the value to its date and time with offset relative to UTC equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryParse(out DateTimeOffset result)
        {
            return DateTimeOffset.TryParse(Value, out result);
        }

        /// <summary>
        /// Converts the value to its time span equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryParse(out TimeSpan result)
        {
            return TimeSpan.TryParse(Value, out result);
        }

        /// <summary>
        /// Converts the value to its URI equivalent.
        /// </summary>
        /// <returns>The result value converted.</returns>
        public Uri ParseToUri()
        {
            return !string.IsNullOrWhiteSpace(Value) ? new Uri(Value) : null;
        }

        /// <summary>
        /// Converts the value to its file information object equivalent.
        /// </summary>
        /// <returns>The result value converted.</returns>
        public FileInfo ParseToFileInfo()
        {
            if (string.IsNullOrWhiteSpace(Value)) return null;
            try
            {
                return new FileInfo(Value);
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (PathTooLongException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (FileNotFoundException)
            {
            }

            return null;
        }

        /// <summary>
        /// Converts the value to its directory information object equivalent.
        /// </summary>
        /// <returns>The result value converted.</returns>
        public DirectoryInfo ParseToDirectoryInfo()
        {
            if (string.IsNullOrWhiteSpace(Value)) return null;
            try
            {
                return new DirectoryInfo(Value);
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (PathTooLongException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }

            return null;
        }

        /// <summary>
        /// Converts the value of this instance to the parameter string.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public override string ToString()
        {
            var str = new StringBuilder(OriginalKey);
            if (string.IsNullOrEmpty(Value)) return str.ToString();
            if (str.Length > 0) str.Append(' ');
            str.Append(Value);
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
        public bool Equals(Parameter other)
        {
            if (other == null) return false;
            return ToString() == other.ToString();
        }

        /// <summary>
        /// Determines whether the value of this instance and the specified one have the same value.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
        public bool Equals(Parameters other)
        {
            if (other == null) return false;
            return ToString() == other.ToString();
        }

        /// <summary>
        /// Determines whether the value of this instance and the specified one have the same value.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
        public bool Equals(string other)
        {
            return ToString() == other;
        }

        /// <summary>
        /// Determines whether the value of this instance and the specified one have the same value.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is Parameters parameters) return Equals(parameters);
            if (other is Parameter parameter) return Equals(parameter);
            if (other is string str) return Equals(str);
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>A enumerator.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>A enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        /// <summary>
        /// Formats the parameter key.
        /// </summary>
        /// <param name="key">The parameter key to format.</param>
        /// <param name="removePrefix">true if remove the prefix; otherwise, false.</param>
        /// <returns>The parameter key formatted.</returns>
        internal static string FormatKey(string key, bool removePrefix = true)
        {
            if (key == null) return null;
            key = key.Trim();
            if (removePrefix)
            {
                if (key.IndexOf("--") == 0) return key.Substring(2).ToLower();
                if (key.IndexOf('-') == 0) return key.Substring(1).ToLower();
                if (key.IndexOf('/') == 0) return key.Substring(1).ToLower();
            }

            return key.ToLower();
        }
    }

    /// <summary>
    /// A set of parameters with the same key or the related keys.
    /// </summary>
    public class Parameters: IEquatable<Parameters>, IEquatable<Parameter>, IEquatable<string>, IEnumerable<Parameter>
    {
        /// <summary>
        /// Initializes a new instance of the Parameters instance.
        /// </summary>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The collection of parameter.</param>
        /// <param name="additionalKeys">The additional keys.</param>
        public Parameters(string key, IEnumerable<Parameter> value, IEnumerable<string> additionalKeys = null)
        {
            Key = key;
            Items = (value != null ? value.Where(item =>
            {
                return item != null;
            }).ToList() : new List<Parameter>()).AsReadOnly();
            ItemCount = Items.Count;
            AdditionalKeys = (additionalKeys != null ? additionalKeys.ToList() : new List<string>()).AsReadOnly();
            var keys = new List<string>
            {
                Key
            };
            keys.AddRange(AdditionalKeys);
            AllKeys = keys.AsReadOnly();
            IsEmpty = ItemCount == 0;
            if (IsEmpty)
            {
                FirstValues = new List<string>().AsReadOnly();
                FirstValue = string.Empty;
                LastValues = new List<string>().AsReadOnly();
                LastValue = string.Empty;
                MergedValues = new List<string>().AsReadOnly();
                MergedValue = string.Empty;
                return;
            }

            FirstItem = Items[0];
            FirstValues = FirstItem.Values;
            FirstValue = FirstItem.Value;
            LastItem = Items[ItemCount - 1];
            LastValues = LastItem.Values;
            LastValue = LastItem.Value;
            var mergedList = new List<string>();
            var mergedStr = new StringBuilder();
            foreach (var item in Items)
            {
                mergedList.AddRange(item.Values);
                if (mergedStr.Length > 0) mergedStr.Append(' ');
                mergedStr.Append(item.Value);
            }

            MergedValues = mergedList.AsReadOnly();
            MergedValue = mergedStr.ToString();
        }

        /// <summary>
        /// Gets the primary parameter key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets all of the keys matched.
        /// </summary>
        public IReadOnlyList<string> AllKeys { get; }

        /// <summary>
        /// Gets the addtional keys.
        /// </summary>
        public IReadOnlyList<string> AdditionalKeys { get; }

        /// <summary>
        /// Gets a value indicating whether there is any parameter matched.
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// Gets a readonly list of the parameter matched the key.
        /// </summary>
        public IReadOnlyList<Parameter> Items { get; }

        /// <summary>
        /// Gets the first parameter matched the key.
        /// </summary>
        public Parameter FirstItem { get; }

        /// <summary>
        /// Gets the first parameter matched the key.
        /// </summary>
        public Parameter LastItem { get; }

        /// <summary>
        /// Gets the count of the parameter matched the key.
        /// </summary>
        public int ItemCount { get; }

        /// <summary>
        /// Gets the string value of the first parameter matched the key.
        /// </summary>
        public string FirstValue { get; }

        /// <summary>
        /// Gets the words of the value of the first parameter matched the key.
        /// </summary>
        public IReadOnlyList<string> FirstValues { get; }

        /// <summary>
        /// Gets the string value of the last parameter matched the key.
        /// </summary>
        public string LastValue { get; }

        /// <summary>
        /// Gets the words of the value of the last parameter matched the key.
        /// </summary>
        public IReadOnlyList<string> LastValues { get; }

        /// <summary>
        /// Gets the string value merged by all the parameter matched the key.
        /// </summary>
        public string MergedValue { get; }

        /// <summary>
        /// Gets the words of the value merged by all the parameter matched the key.
        /// </summary>
        public IReadOnlyList<string> MergedValues { get; }

        /// <summary>
        /// Gets the string value of a specific parameter matched the key.
        /// </summary>
        /// <param name="mode">The parameter getting mode.</param>
        /// <returns>A string value of a specific parameter.</returns>
        public string Value(ParameterModes mode = ParameterModes.First)
        {
            return mode switch
            {
                ParameterModes.All => MergedValue,
                ParameterModes.Last => LastValue,
                _ => FirstValue,
            };
        }

        /// <summary>
        /// Gets the string value of a specific parameter matched the key.
        /// </summary>
        /// <param name="index">The parameter index.</param>
        /// <returns>A string value of a specific parameter.</returns>
        public string Value(int index)
        {
            return Items[index].Value;
        }

        /// <summary>
        /// Gets the words of the value of a specific parameter matched the key.
        /// </summary>
        /// <param name="mode">The parameter getting mode.</param>
        /// <returns>The words of the value of a specific parameter.</returns>
        public IReadOnlyList<string> Values(ParameterModes mode = ParameterModes.First)
        {
            return mode switch
            {
                ParameterModes.All => MergedValues,
                ParameterModes.Last => LastValues,
                _ => FirstValues,
            };
        }

        /// <summary>
        /// Gets the words of the string value of a specific parameter matched the key.
        /// </summary>
        /// <param name="index">The parameter index.</param>
        /// <returns>The words of the value of a specific parameter.</returns>
        public IReadOnlyList<string> Values(int index)
        {
            return Items[index].Values;
        }

#if !NETOLDVER
        /// <summary>
        /// Gets the string value of a specific parameter matched the key.
        /// </summary>
        /// <param name="index">The parameter index.</param>
        /// <returns>A string value of a specific parameter.</returns>
        public string Value(Index index)
        {
            return Items[index].Value;
        }

        /// <summary>
        /// Gets the words of the string value of a specific parameter matched the key.
        /// </summary>
        /// <param name="index">The parameter index.</param>
        /// <returns>The words of the value of a specific parameter.</returns>
        public IReadOnlyList<string> Values(Index index)
        {
            return Items[index].Values;
        }
#endif

        /// <summary>
        /// Converts the value to its boolean equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryToParse(out bool result, ParameterModes mode = ParameterModes.First)
        {
            var str = Value(mode);
            if (!string.IsNullOrWhiteSpace(str)) return bool.TryParse(str, out result);
            result = true;
            return true;
        }

        /// <summary>
        /// Converts the value to its 32-bit signed integer equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryToParse(out int result, ParameterModes mode = ParameterModes.First)
        {
            return int.TryParse(Value(mode), out result);
        }

        /// <summary>
        /// Converts the value to its 64-bit signed integer equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryToParse(out long result, ParameterModes mode = ParameterModes.First)
        {
            return long.TryParse(Value(mode), out result);
        }

        /// <summary>
        /// Converts the value to its single-precision floating-point number equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryToParse(out float result, ParameterModes mode = ParameterModes.First)
        {
            return float.TryParse(Value(mode), out result);
        }

        /// <summary>
        /// Converts the value to its double-precision floating-point number equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryToParse(out double result, ParameterModes mode = ParameterModes.First)
        {
            return double.TryParse(Value(mode), out result);
        }

        /// <summary>
        /// Converts the value to its GUID equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryToParse(out Guid result, ParameterModes mode = ParameterModes.First)
        {
            return Guid.TryParse(Value(mode), out result);
        }

        /// <summary>
        /// Converts the value to its date and time equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryToParse(out DateTime result, ParameterModes mode = ParameterModes.First)
        {
            return DateTime.TryParse(Value(mode), out result);
        }

        /// <summary>
        /// Converts the value to its date and time with offset relative to UTC equivalent.
        /// </summary>
        /// <param name="result">The result value converted when this method returns.</param>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>true if the value was converted successfully; otherwise, false.</returns>
        public bool TryToParse(out DateTimeOffset result, ParameterModes mode = ParameterModes.First)
        {
            return DateTimeOffset.TryParse(Value(mode), out result);
        }

        /// <summary>
        /// Converts the value to its URI equivalent.
        /// </summary>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>The result value converted.</returns>
        public Uri ParseToUri(ParameterModes mode = ParameterModes.First)
        {
            var v = Value(mode);
            if (string.IsNullOrWhiteSpace(v)) return null;
            return new Uri(v);
        }

        /// <summary>
        /// Converts the value to its file information object equivalent.
        /// </summary>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>The result value converted.</returns>
        public FileInfo ParseToFileInfo(ParameterModes mode = ParameterModes.First)
        {
            var v = Value(mode);
            if (string.IsNullOrWhiteSpace(v)) return null;
            try
            {
                return new FileInfo(v);
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (PathTooLongException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (FileNotFoundException)
            {
            }

            return null;
        }

        /// <summary>
        /// Converts the value to its directory information object equivalent.
        /// </summary>
        /// <param name="mode">The parameter resolving mode.</param>
        /// <returns>The result value converted.</returns>
        public DirectoryInfo ParseToDirectoryInfo(ParameterModes mode = ParameterModes.First)
        {
            var v = Value(mode);
            if (string.IsNullOrWhiteSpace(v)) return null;
            try
            {
                return new DirectoryInfo(v);
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (PathTooLongException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }

            return null;
        }

        /// <summary>
        /// Converts the value of this instance to the parameters string.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
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
        public bool Equals(Parameters other)
        {
            if (other == null) return false;
            return ToString() == other.ToString();
        }

        /// <summary>
        /// Determines whether the value of this instance and the specified one have the same value.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
        public bool Equals(Parameter other)
        {
            if (other == null) return false;
            return ToString() == other.ToString();
        }

        /// <summary>
        /// Determines whether the value of this instance and the specified one have the same value.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
        public bool Equals(string other)
        {
            return ToString() == other;
        }

        /// <summary>
        /// Determines whether the value of this instance and the specified one have the same value.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>true if this instance is the value of the same as the specific one; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is Parameters parameters) return Equals(parameters);
            if (other is Parameter parameter) return Equals(parameter);
            if (other is string str) return Equals(str);
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>A enumerator.</returns>
        public IEnumerator<Parameter> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>A enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
