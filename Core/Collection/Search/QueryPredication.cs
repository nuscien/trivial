using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Net;
using Trivial.Text;

namespace Trivial.Collection;

/// <summary>
/// The query prediction.
/// </summary>
/// <typeparam name="T">The type of the queryable item.</typeparam>
public class QueryPredication<T>
{
    /// <summary>
    /// The information of the filter.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="source">The querable source.</param>
    /// <param name="key">The key.</param>
    /// <param name="q">The query data.</param>
    public class FilterInfo<TValue>(TValue value, IQueryable<T> source, string key, QueryData q)
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        public TValue Value { get; } = value;

        /// <summary>
        /// Gets the queryable source.
        /// </summary>
        public IQueryable<T> Source { get; } = source;

        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key { get; } = key;

        /// <summary>
        /// Gets the query data.
        /// </summary>
        public QueryData Q { get; } = q;

        /// <summary>
        /// Gets a value indicating whether the key is in the query data.
        /// </summary>
        public bool HasKey => ListExtensions.ContainsKey(Q, Key);

        /// <summary>
        /// Gets the first value.
        /// </summary>
        /// <returns>The string value of the first item matched; or null if non-exist.</returns>
        public string GetFirstValue() => Q.GetFirstValue(Key);

        /// <summary>
        /// Gets the last value.
        /// </summary>
        /// <returns>The string value of the first item matched; or null if non-exist.</returns>
        public string GetLastValue() => Q.GetFirstValue(Key);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="separator">The optional separator for mulitple key; or null for default separator.</param>
        /// <returns>The string value combined; or null if non-exist.</returns>
        public string GetValue(string separator = null) => Q.GetValue(Key, separator);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>The value.</returns>
        public IList<string> GetValues() => Q.GetValues(Key).ToList();

        /// <summary>
        /// Converts an instance to its value.
        /// </summary>
        /// <param name="info">The instance.</param>
        /// <returns>The value.</returns>
        public static explicit operator TValue(FilterInfo<TValue> info)
        {
            return info is null ? default : info.Value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the QueryPredication class.
    /// </summary>
    /// <param name="source">The queryable source.</param>
    /// <param name="q">The query data.</param>
    public QueryPredication(IQueryable<T> source, QueryData q)
    {
        Data = source;
        OriginalSource = source;
        Q = q ?? new();
    }

    /// <summary>
    /// Gets the original queryable source.
    /// </summary>
    public IQueryable<T> OriginalSource { get; private set; }

    /// <summary>
    /// Gets the queryable source as result.
    /// </summary>
    public IQueryable<T> Data { get; private set; }

    /// <summary>
    /// Gets the query data.
    /// </summary>
    public QueryData Q { get; }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="filter">The filter.</param>
    public void Add(Func<IQueryable<T>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        Data = filter(Data);
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    public void Add(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        if (predicate == null || Data == null) return;
        Data = Data.Where(predicate);
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    public void Add(System.Linq.Expressions.Expression<Func<T, int, bool>> predicate)
    {
        if (predicate == null || Data == null) return;
        Data = Data.Where(predicate);
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="checkEmpty">true if pass for null or empty result; otherwise, false.</param>
    public void Add(string key, Func<FilterInfo<string>, IQueryable<T>> filter, bool checkEmpty = false)
    {
        if (filter == null || Data == null) return;
        var s = Q[key];
        if (!checkEmpty || !string.IsNullOrWhiteSpace(s)) Data = filter(new FilterInfo<string>(s, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="checkEmpty">true if pass for null or empty result; otherwise, false.</param>
    public void AddForString(string key, Func<FilterInfo<string>, IQueryable<T>> filter, bool checkEmpty = false) => Add(key, filter, checkEmpty);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="separator">The separator for multiple values.</param>
    /// <param name="checkEmpty">true if pass for null or empty result; otherwise, false.</param>
    public void Add(string key, Func<FilterInfo<string>, IQueryable<T>> filter, string separator, bool checkEmpty = false)
    {
        if (filter == null || Data == null) return;
        var s = Q.GetValue(key, separator);
        if (!checkEmpty || !string.IsNullOrWhiteSpace(s)) Data = filter(new FilterInfo<string>(s, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="separator">The separator for multiple values.</param>
    /// <param name="checkEmpty">true if pass for null or empty result; otherwise, false.</param>
    public void AddForString(string key, Func<FilterInfo<string>, IQueryable<T>> filter, string separator, bool checkEmpty = false) => Add(key, filter, separator, checkEmpty);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void Add(string key, Func<FilterInfo<int>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetInt32Value(key);
        if (s.HasValue) Data = filter(new FilterInfo<int>(s.Value, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void AddForInt32(string key, Func<FilterInfo<int>, IQueryable<T>> filter) => Add(key, filter);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void Add(string key, Func<FilterInfo<int?>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetInt32Value(key);
        Data = filter(new FilterInfo<int?>(s, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void AddForNullableInt32(string key, Func<FilterInfo<int?>, IQueryable<T>> filter) => Add(key, filter);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void Add(string key, Func<FilterInfo<long>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetInt64Value(key);
        if (s.HasValue) Data = filter(new FilterInfo<long>(s.Value, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void AddForInt64(string key, Func<FilterInfo<long>, IQueryable<T>> filter) => Add(key, filter);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void Add(string key, Func<FilterInfo<long?>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetInt64Value(key);
        Data = filter(new FilterInfo<long?>(s, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void AddForNullableInt64(string key, Func<FilterInfo<long?>, IQueryable<T>> filter) => Add(key, filter);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void Add(string key, Func<FilterInfo<float>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetSingleValue(key);
        if (s.HasValue) Data = filter(new FilterInfo<float>(s.Value, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void AddForSingle(string key, Func<FilterInfo<float>, IQueryable<T>> filter) => Add(key, filter);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void Add(string key, Func<FilterInfo<float?>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetSingleValue(key);
        Data = filter(new FilterInfo<float?>(s, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void AddForNullableSingle(string key, Func<FilterInfo<float?>, IQueryable<T>> filter) => Add(key, filter);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void Add(string key, Func<FilterInfo<double>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetDoubleValue(key);
        if (s.HasValue) Data = filter(new FilterInfo<double>(s.Value, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void AddForDouble(string key, Func<FilterInfo<double>, IQueryable<T>> filter) => Add(key, filter);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void Add(string key, Func<FilterInfo<double?>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetDoubleValue(key);
        Data = filter(new FilterInfo<double?>(s, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void AddForNullableDouble(string key, Func<FilterInfo<double>, IQueryable<T>> filter) => Add(key, filter);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void Add(string key, Func<FilterInfo<bool>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        var s = Q.GetValues(key).FirstOrDefault(ele => !string.IsNullOrWhiteSpace(ele));
        var b = JsonBooleanNode.TryParse(s);
        if (b == null) return;
        Data = filter(new FilterInfo<bool>(b.Value, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void AddForBoolean(string key, Func<FilterInfo<bool>, IQueryable<T>> filter) => Add(key, filter);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void Add(string key, Func<FilterInfo<bool?>, IQueryable<T>> filter)
    {
        if (filter == null || Data == null) return;
        var s = Q.GetValues(key).FirstOrDefault(ele => !string.IsNullOrWhiteSpace(ele));
        var b = JsonBooleanNode.TryParse(s);
        Data = filter(new FilterInfo<bool?>(b?.Value, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    public void AddForNullableBoolean(string key, Func<FilterInfo<bool>, IQueryable<T>> filter) => Add(key, filter);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="useUnixTimestamp">true if the value is unix timestamp; otherwise, false.</param>
    public void Add(string key, Func<FilterInfo<DateTime>, IQueryable<T>> filter, bool useUnixTimestamp = false)
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetDateTimeValue(key, useUnixTimestamp);
        if (s.HasValue) Data = filter(new FilterInfo<DateTime>(s.Value, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="useUnixTimestamp">true if the value is unix timestamp; otherwise, false.</param>
    public void AddForDateTime(string key, Func<FilterInfo<DateTime>, IQueryable<T>> filter, bool useUnixTimestamp = false) => Add(key, filter, useUnixTimestamp);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="useUnixTimestamp">true if the value is unix timestamp; otherwise, false.</param>
    public void Add(string key, Func<FilterInfo<DateTime?>, IQueryable<T>> filter, bool useUnixTimestamp = false)
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetDateTimeValue(key, useUnixTimestamp);
        Data = filter(new FilterInfo<DateTime?>(s, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="useUnixTimestamp">true if the value is unix timestamp; otherwise, false.</param>
    public void AddForNullableDateTime(string key, Func<FilterInfo<DateTime?>, IQueryable<T>> filter, bool useUnixTimestamp = false) => Add(key, filter, useUnixTimestamp);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="ignoreCase">true to ignore case; false to regard case; null to use default settings.</param>
    public void Add<TEnum>(string key, Func<FilterInfo<TEnum>, IQueryable<T>> filter, bool? ignoreCase = null) where TEnum : struct
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetEnumValue<TEnum>(key, ignoreCase);
        if (s.HasValue) Data = filter(new FilterInfo<TEnum>(s.Value, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="ignoreCase">true to ignore case; false to regard case; null to use default settings.</param>
    public void AddForEnum<TEnum>(string key, Func<FilterInfo<TEnum>, IQueryable<T>> filter, bool? ignoreCase = null) where TEnum : struct => Add(key, filter, ignoreCase);

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="ignoreCase">true to ignore case; false to regard case; null to use default settings.</param>
    public void Add<TEnum>(string key, Func<FilterInfo<TEnum?>, IQueryable<T>> filter, bool? ignoreCase = null) where TEnum : struct
    {
        if (filter == null || Data == null) return;
        var s = Q.TryGetEnumValue<TEnum>(key, ignoreCase);
        Data = filter(new FilterInfo<TEnum?>(s, Data, key, Q)) ?? Data;
    }

    /// <summary>
    /// Adds a predication.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="key">The key in query data.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="ignoreCase">true to ignore case; false to regard case; null to use default settings.</param>
    public void AddForNullableEnum<TEnum>(string key, Func<FilterInfo<TEnum?>, IQueryable<T>> filter, bool? ignoreCase = null) where TEnum : struct => Add(key, filter, ignoreCase);
}
