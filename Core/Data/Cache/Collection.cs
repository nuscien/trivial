using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Text;

namespace Trivial.Data;

/// <summary>
/// The thread-safe cache for keyed data.
/// </summary>
/// <typeparam name="T">The type of data model.</typeparam>
/// <example>
/// <code>
/// // Initializes an instance as a singleton.
/// var cache = new DataCacheCollection&lt;DataModel&gt;();
/// 
/// // Set data with an identifier.
/// cache["id-to-reference"] = new();
/// 
/// // Get data on demand.
/// var m = cache.TryGet("id-to-reference", default);
/// </code>
/// </example>
public class DataCacheCollection<T> : ICollection<DataCacheItemInfo<T>>, IReadOnlyList<DataCacheItemInfo<T>>
{
    /// <summary>
    /// The date and time excuted cleaning up.
    /// </summary>
    private DateTime cleanUpTime = DateTime.Now;

    /// <summary>
    /// The cache data list.
    /// </summary>
    private readonly ConcurrentDictionary<string, DataCacheItemInfo<T>> items = new();

    /// <summary>
    /// The cache data factory.
    /// </summary>
    private DataCacheFactoryInfo<T> factoryInfo;

    /// <summary>
    /// Gets the maxinum count of the elements contained in the cache item collection; or null, if no limitation.
    /// </summary>
    public int? MaxCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether need keep only one instance.
    /// </summary>
    public bool SingleCacheInstance { get; set; }

    /// <summary>
    /// Gets the optional expiration.
    /// </summary>
    public TimeSpan? Expiration { get; set; }

    /// <summary>
    /// Gets the count of elements contained in the collection.
    /// </summary>
    int ICollection<DataCacheItemInfo<T>>.Count => AsEnumerable().Count();

    /// <summary>
    /// Gets the count of elements contained in the collection.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int IReadOnlyCollection<DataCacheItemInfo<T>>.Count => AsEnumerable().Count();

    /// <summary>
    /// Gets the count of elements contained in the collection cache.
    /// </summary>
    public int CacheCount => items.Count;

    /// <summary>
    /// Gets a value indicating whether the collection is readonly.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- index is equal to or greater than the count.</exception>
    DataCacheItemInfo<T> IReadOnlyList<DataCacheItemInfo<T>>.this[int index] => items[items.Keys.ToList()[index]];

    /// <summary>
    /// Gets or sets the specific element.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">id was null.</exception>
    /// <exception cref="ArgumentException">id was empty or consists only of white-space characters.</exception>
    /// <exception cref="KeyNotFoundException">The identifier does not exist.</exception>
    public T this[string id]
    {
        get
        {
            StringExtensions.AssertNotWhiteSpace(nameof(id), id);
            var info = GetInfo(id);
            if (info == null) throw new KeyNotFoundException("The identifier does not exist.");
            return info.Value;
        }

        set
        {
            StringExtensions.AssertNotWhiteSpace(nameof(id), id);
            items[id] = new DataCacheItemInfo<T>(id, value);
            RemoveExpiredAuto();
        }
    }

    /// <summary>
    /// Gets the cache item info.
    /// </summary>
    /// <param name="id">The identifier in the resource group.</param>
    /// <param name="initialization">The value initialization. It will be called to generate a new value when no cache.</param>
    /// <param name="expiration">The expiration for initialization.</param>
    /// <returns>The cache item info.</returns>
    public DataCacheItemInfo<T> GetInfo(string id, Func<T> initialization = null, TimeSpan? expiration = null)
    {
        if (string.IsNullOrEmpty(id)) return null;
        items.TryGetValue(id, out var info);
        if (info == null)
            return Add(id, initialization, expiration);
        if (!info.IsExpired(Expiration)) return info;
        items.TryRemove(id, out _);
        return null;
    }

    /// <summary>
    /// Gets the cache item info.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>The cache item info.</returns>
    public DataCacheItemInfo<T> GetInfo(Func<DataCacheItemInfo<T>, bool> predicate)
        => predicate == null
        ? items.ToList().Select(ele => ele.Value).LastOrDefault()
        : items.ToList().Select(ele => ele.Value).LastOrDefault(predicate);

    /// <summary>
    /// Gets the cache item info.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="initialization">The value initialization. It will be called to generate a new value when no cache.</param>
    /// <param name="expiration">The expiration for initialization.</param>
    /// <returns>The cache item info.</returns>
    public async Task<DataCacheItemInfo<T>> GetInfoAsync(string id, Func<Task<T>> initialization = null, TimeSpan? expiration = null)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        return items.TryGetValue(id, out var r) && !r.IsExpired(Expiration)
            ? r
            : await AddAsync(id, initialization, expiration);
    }

    /// <summary>
    /// Gets the cache item info.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="initialization">The value initialization. It will be called to generate a new value when no cache.</param>
    /// <param name="expiration">The expiration for initialization.</param>
    /// <returns>The cache item info.</returns>
    public async Task<T> GetAsync(string id, Func<Task<T>> initialization = null, TimeSpan? expiration = null)
    {
        var result = await GetInfoAsync(id, initialization, expiration);
        if (result is null) throw new KeyNotFoundException("The identifier does not exist.");
        return result.Value;
    }

    /// <summary>
    /// Tries to get the cache item info.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="result">The output result.</param>
    /// <returns>true if has the info and it is not expired; otherwise, false.</returns>
    public bool TryGetInfo(string id, out DataCacheItemInfo<T> result)
    {
        result = GetInfo(id);
        return !(result is null);
    }

    /// <summary>
    /// Tries to get the cache item data.
    /// </summary>
    /// <param name="id">The identifier in the resource group.</param>
    /// <param name="data">The output data.</param>
    /// <returns>true if has the info and it is not expired; otherwise, false.</returns>
    public bool TryGet(string id, out T data)
    {
        var result = GetInfo(id);
        if (result is null)
        {
            data = default;
            return false;
        }

        data = result.Value;
        return true;
    }

    /// <summary>
    /// Tries to get the cache item data.
    /// </summary>
    /// <param name="id">The identifier in the resource group.</param>
    /// <param name="defaultValue">The default data to return if non-exists.</param>
    /// <param name="setIfNonExist">true if set the default data back if non-exists; otherwise, false.</param>
    /// <returns>true if has the info and it is not expired; otherwise, false.</returns>
    public T TryGet(string id, T defaultValue, bool setIfNonExist = false)
    {
        if (string.IsNullOrEmpty(id)) return defaultValue;
        if (TryGet(id, out var result)) return result;
        if (setIfNonExist) this[id] = defaultValue;
        return defaultValue;
    }

    /// <summary>
    /// Adds an object to the end of the collection.
    /// </summary>
    /// <param name="item">The object to be added to the end of the collection.</param>
    public void Add(DataCacheItemInfo<T> item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Id) || item.IsExpired(Expiration)) return;
        items[item.Id] = item;
        RemoveExpiredAuto();
    }

    /// <summary>
    /// Adds an object to the end of the collection.
    /// </summary>
    /// <param name="id">The identifier in the resource group.</param>
    /// <param name="value">The value.</param>
    /// <param name="expiration">The optional expiration to override current policy.</param>
    public void Add(string id, T value, TimeSpan? expiration = null)
        => Add(items.TryGetValue(id, out var item) && item != null
            ? new DataCacheItemInfo<T>(id, value, expiration) { CreationDate = item.CreationDate }
            : new DataCacheItemInfo<T>(id, value, expiration));

    /// <summary>
    /// Removes all elements from the collection.
    /// </summary>
    public void Clear()
        => items.Clear();

    /// <summary>
    /// Gets the count of elements contained in the collection.
    /// </summary>
    /// <returns>The count of elements.</returns>
    public int Count() 
        => AsEnumerable().Count();

    /// <summary>
    /// Determines whether an element is in the collection.
    /// </summary>
    /// <param name="item">The object to locate in the collection.</param>
    /// <returns>true if item is found in the collection; otherwise, false.</returns>
    public bool Contains(DataCacheItemInfo<T> item)
    {
        if (string.IsNullOrEmpty(item.Id)) return false;
        return Contains(item.Id);
    }

    /// <summary>
    /// Determines whether an element is in the collection.
    /// </summary>
    /// <param name="item">The object to locate in the collection.</param>
    /// <returns>true if item is found in the collection; otherwise, false.</returns>
    public bool ContainsValue(T item)
    {
        if (item is null)
        {
            foreach (var ele in items)
            {
                if (ele.Value is null && !ele.Value.IsExpired(Expiration)) return true;
            }

            return false;
        }

        foreach (var ele in items)
        {
            if (item.Equals(ele.Value) && !ele.Value.IsExpired(Expiration)) return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether an element is in the collection.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>true if item is found in the collection; otherwise, false.</returns>
    public bool Contains(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        if (!items.TryGetValue(id, out var info) || info == null) return false;
        if (!info.IsExpired(Expiration)) return true;
        items.TryRemove(id, out _);
        return false;
    }

    /// <summary>
    /// Registers a value resolving factory.
    /// </summary>
    /// <param name="factory">The value resovling factory.</param>
    /// <param name="timeout">An optional time span that represents the number of milliseconds to wait</param>
    public void Register(Func<string, Task<T>> factory, TimeSpan? timeout = null)
        => factoryInfo = factory == null
            ? null
            : new DataCacheFactoryInfo<T>() { Factory = factory, Timeout = timeout };

    /// <summary>
    /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from System.Collections.Generic.List`1. The System.Array must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentNullException">array was null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">arrayIndex was less than 0.</exception>
    /// <exception cref="ArgumentException">The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(DataCacheItemInfo<T>[] array, int arrayIndex)
        => items.Values.CopyTo(array, arrayIndex);

    /// <summary>
    /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from System.Collections.Generic.List`1. The System.Array must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    /// <exception cref="ArgumentNullException">array was null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">arrayIndex was less than 0.</exception>
    /// <exception cref="ArgumentException">The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.</exception>
    public void CopyTo(T[] array, int arrayIndex)
        => items.Select(ele => ele.Value.Value).ToList().CopyTo(array, arrayIndex);

    /// <summary>
    /// Removes the occurrence of a specific object from the collection.
    /// </summary>
    /// <param name="item">The object to remove from the collection.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
    public bool Remove(DataCacheItemInfo<T> item)
    {
        if (item?.Id == null) return false;
        return items.TryRemove(item.Id, out _);
    }

    /// <summary>
    /// Removes the occurrence of a specific value from the collection.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
    public int RemoveAll(Predicate<DataCacheItemInfo<T>> predicate)
    {
        if (predicate == null) return 0;
        var i = 0;
        var col = items.ToList().Where(ele => predicate(ele.Value)).Select(ele => ele.Key);
        foreach (var item in col)
        {
            if (items.TryRemove(item, out _)) i++;
        }

        return i;
    }

    /// <summary>
    /// Removes the occurrence of a specific value from the collection.
    /// </summary>
    /// <param name="item">The object to remove from the collection.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
    public bool RemoveValue(T item)
    {
        if (item == null) return false;
        var result = RemoveAll(ele => item.Equals(ele.Value)) > 0;
        RemoveExpiredAuto();
        return result;
    }

    /// <summary>
    /// Removes the occurrence with the specific identifier from the collection.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
    public bool Remove(string id)
        => Remove(id, out _);

    /// <summary>
    /// Removes the occurrence with the specific identifier from the collection.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="result">The result deleted.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
    public bool Remove(string id, out DataCacheItemInfo<T> result)
    {
        if (id == null)
        {
            result = null;
            return false;
        }

        RemoveExpiredAuto();
        return items.TryRemove(id, out result);
    }

    /// <summary>
    /// Removes the elements expired.
    /// </summary>
    /// <param name="expiration">An optional expiration time span.</param>
    public void RemoveExpired(TimeSpan? expiration = null)
    {
        try
        {
            RemoveAll(ele => ele.IsExpired(expiration ?? Expiration));
        }
        catch (NullReferenceException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        if (!MaxCount.HasValue) return;
        var maxCount = 0;
        try
        {
            maxCount = MaxCount.Value;
        }
        catch (InvalidOperationException)
        {
            cleanUpTime = DateTime.Now;
            return;
        }

        if (maxCount < 1)
        {
            items.Clear();
        }
        else
        {
            while (CacheCount > maxCount) RemoveEarliest();
        }

        cleanUpTime = DateTime.Now;
    }

    /// <summary>
    /// Removes all the elements before the specific date time.
    /// </summary>
    /// <param name="date">The date time to compare with the update date of each item.</param>
    public void RemoveBefore(DateTime date)
    {
        RemoveAll(ele => ele.UpdateDate < date || ele.IsExpired(Expiration));
        if (!MaxCount.HasValue) return;
        var maxCount = MaxCount.Value;
        if (maxCount < 1)
        {
            items.Clear();
        }
        else
        {
            while (CacheCount > maxCount) RemoveEarliest();
        }
    }

    /// <summary>
    /// Removes the earliest elements.
    /// </summary>
    /// <param name="result">The item removed.</param>
    public void RemoveEarliest(out DataCacheItemInfo<T> result)
    {
        try
        {
            DataCacheItemInfo<T> r = null;
            foreach (var item in items)
            {
                if (r == null || item.Value.CreationDate < r.CreationDate) r = item.Value;
            }

            items.TryRemove(r.Id, out result);
            return;
        }
        catch (NullReferenceException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        result = null;
    }

    /// <summary>
    /// Removes the earliest elements.
    /// </summary>
    public void RemoveEarliest()
        => RemoveEarliest(out _);

    /// <summary>
    /// Tests if the item is expired.
    /// </summary>
    /// <param name="item">The item to test.</param>
    /// <returns>true if expired; otherwise, false.</returns>
    public bool IsExpired(DataCacheItemInfo<T> item)
        => item == null || item.IsExpired(Expiration);

    /// <summary>
    /// Tests if there is any item.
    /// </summary>
    /// <returns>true if contains one or more items; otherwise, false.</returns>
    public bool Any()
        => AsEnumerable().Any();

    /// <summary>
    /// Converts to a list.
    /// </summary>
    /// <returns>A list copied.</returns>
    public List<DataCacheItemInfo<T>> ToList()
        => AsEnumerable().ToList();

    /// <summary>
    /// Creates a dictionary from the key value pairs.
    /// </summary>
    /// <returns>A dictionary with key and the value collection.</returns>
    public Dictionary<string, T> ToDictionary()
        => AsEnumerable().ToDictionary(item => item.Id, item => item.Value);

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<DataCacheItemInfo<T>> GetEnumerator()
        => ToList().GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Converts to a collection.
    /// </summary>
    /// <returns>A collection.</returns>
    private IEnumerable<DataCacheItemInfo<T>> AsEnumerable()
        => items.Select(ele => ele.Value).Where(ele => ele?.IsExpired(Expiration) == false);

    private DataCacheItemInfo<T> Add(string id, Func<T> initialization = null, TimeSpan? expiration = null)
    {
        if (initialization == null)
        {
            _ = GetByFactoryAsync(id);
            return null;
        }

        var obj = initialization();
        var info = new DataCacheItemInfo<T>(id, obj, expiration);
        if (info.IsExpired(Expiration)) return null;
        Add(info);
        return info;
    }

    private async Task<DataCacheItemInfo<T>> AddAsync(string id, Func<Task<T>> initialization = null, TimeSpan? expiration = null)
    {
        if (initialization == null)
        {
            items.TryRemove(id, out _);
            return await GetByFactoryAsync(id);
        }

        var obj = await initialization();
        var info = new DataCacheItemInfo<T>(id, obj, expiration);
        if (info.IsExpired(Expiration))
        {
            items.TryGetValue(id, out _);
            return null;
        }

        Add(info);
        return info;
    }

    private async Task<DataCacheItemInfo<T>> GetByFactoryAsync(string id)
    {
        var factory = factoryInfo;
        if (string.IsNullOrEmpty(id) || factory == null) return null;
        factory.TryGetValue(id, out var slim);
        if (slim is null)
        {
            var newSlim = new SemaphoreSlim(1);
            if (factory.TryGetValue(id, out slim))
            {
                if (factory.TryGetValue(id, out slim))
                {
                    try
                    {
                        newSlim.Dispose();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (NullReferenceException)
                    {
                    }
                }
                else
                {
                    factory[id] = newSlim;
                    slim = newSlim;
                }
            }
            else
            {
                factory[id] = newSlim;
                slim = newSlim;
            }
        }

        try
        {
            var timeout = factory.Timeout;
            if (timeout.HasValue)
            {
                await slim.WaitAsync(timeout.Value);
            }
            else
            {
                await slim.WaitAsync();
            }
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }

        #pragma warning disable IDE0018
        DataCacheItemInfo<T> r;
        #pragma warning restore IDE0018
        if (items.TryGetValue(id, out r))
        {
            try
            {
                slim.Release();
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (SemaphoreFullException)
            {
            }

            return r;
        }

        try
        {
            var v = await factory.Factory(id);
            r = new DataCacheItemInfo<T>(id, v);
            items[id] = r;
            return r;
        }
        finally
        {
            try
            {
                factory.TryRemove(id, out _);
                slim.Release();
                if (slim.CurrentCount > 0)
                {
                    slim.Dispose();
                }
                else
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(10);
                            if (slim.CurrentCount > 0)
                            {
                                slim.Dispose();
                                return;
                            }

                            await Task.Delay(30);
                            if (slim.CurrentCount > 0)
                            {
                                slim.Dispose();
                                return;
                            }

                            await Task.Delay(60);
                            if (slim.CurrentCount > 0)
                            {
                                slim.Dispose();
                                return;
                            }

                            await Task.Delay(100);
                            slim.Dispose();
                        }
                        catch (InvalidOperationException)
                        {
                        }
                        catch (NullReferenceException)
                        {
                        }
                    });
                }

                RemoveExpiredAuto();
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (SemaphoreFullException)
            {
            }
        }
    }

    /// <summary>
    /// Removes the elements expired.
    /// </summary>
    private void RemoveExpiredAuto()
    {
        var greater = false;
        try
        {
            greater = CacheCount > MaxCount.Value;
        }
        catch (InvalidOperationException)
        {
        }

        if (!greater && DateTime.Now < (cleanUpTime + (Expiration ?? TimeSpan.Zero))) return;
        RemoveExpired();
    }
}
