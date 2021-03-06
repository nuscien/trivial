﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Data
{
    /// <summary>
    /// The collection for data cache.
    /// </summary>
    /// <typeparam name="T">The type of data model.</typeparam>
    public class DataCacheCollection<T> : ICollection<DataCacheCollection<T>.ItemInfo>, IReadOnlyList<DataCacheCollection<T>.ItemInfo>
    {
        private class FactoryInfo : ConcurrentDictionary<string, SemaphoreSlim>
        {
            /// <summary>
            /// Gets or sets the factory.
            /// </summary>
            public Func<string, Task<T>> Factory { get; set; }

            /// <summary>
            /// Gets or sets the optional time span that represents the number of milliseconds to wait.
            /// </summary>
            public TimeSpan? Timeout { get; set; }
        }

        /// <summary>
        /// The cache item info.
        /// </summary>
        public class ItemInfo
        {
            /// <summary>
            /// Initializes a new instance of the DataCacheCollection.ItemInfo class.
            /// </summary>
            /// <param name="id">The identifier.</param>
            /// <param name="value">The value.</param>
            /// <param name="expiration">The optional expiration to override current policy.</param>
            public ItemInfo(string id, T value, TimeSpan? expiration = null)
            {
                Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
                Value = value;
                Expiration = expiration;
            }

            /// <summary>
            /// Initializes a new instance of the DataCacheCollection.ItemInfo class.
            /// </summary>
            /// <param name="idPrefix">The prefix of the identifier for resource group.</param>
            /// <param name="id">The identifier in the resource group.</param>
            /// <param name="value">The value.</param>
            /// <param name="expiration">The optional expiration to override current policy.</param>
            public ItemInfo(string idPrefix, string id, T value, TimeSpan? expiration = null) : this(id, value, expiration)
            {
                Prefix = !string.IsNullOrWhiteSpace(idPrefix) ? idPrefix : null;
            }

            /// <summary>
            /// Gets or sets the prefix.
            /// </summary>
            public string Prefix { get; }

            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// Gets or sets the creation date.
            /// </summary>
            public DateTime CreationDate { get; internal set; } = DateTime.Now;

            /// <summary>
            /// Gets or sets the update date.
            /// </summary>
            public DateTime UpdateDate { get; } = DateTime.Now;

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public T Value { get; }

            /// <summary>
            /// Gets the optional expiration.
            /// </summary>
            public TimeSpan? Expiration { get; }

            /// <summary>
            /// Gets or sets the expired date.
            /// </summary>
            /// <param name="expiration">The expiration time span.</param>
            /// <returns>The expired date.</returns>
            public DateTime GetExpiredDate(TimeSpan expiration)
            {
                return UpdateDate + (Expiration ?? expiration);
            }

            /// <summary>
            /// Tests if the item is expired.
            /// </summary>
            /// <param name="expiration">The expiration time span.</param>
            /// <returns>true if expired; otherwise, false.</returns>
            public bool IsExpired(TimeSpan? expiration)
            {
                if (!expiration.HasValue && !Expiration.HasValue) return false;
                return DateTime.Now >= (UpdateDate + (Expiration ?? expiration).Value);
            }
        }

        /// <summary>
        /// The locker for member initialzation.
        /// </summary>
        private readonly object locker = new();

        /// <summary>
        /// The date and time excuted cleaning up.
        /// </summary>
        private DateTime cleanUpTime = DateTime.Now;

        /// <summary>
        /// The cache data list.
        /// </summary>
        private readonly ConcurrentDictionary<string, ItemInfo> items = new();

        /// <summary>
        /// The cache data list for prefix ones.
        /// </summary>
        private ConcurrentDictionary<string, ItemInfo> items2;

        /// <summary>
        /// The cache data list for prefix ones.
        /// </summary>
        private ConcurrentDictionary<string, FactoryInfo> items3;

        /// <summary>
        /// Gets the maxinum count of the elements contained in the cache item collection.
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
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count => items.Count + (items2 != null ? items2.Count : 0);

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
        ItemInfo IReadOnlyList<ItemInfo>.this[int index] => items[items.Keys.ToList()[index]];

        /// <summary>
        /// Gets or sets the specific element.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">id was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="KeyNotFoundException">The identifier does not exist.</exception>
        public T this[string id]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id), "id should not be null, empty or consists only of white-space characters.");
                var info = GetInfo(null, id);
                if (info == null) throw new KeyNotFoundException("The identifier does not exist.");
                return info.Value;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id), "id should not be null, empty or consists only of white-space characters.");
                items[id] = new ItemInfo(id, value);
                RemoveExpiredAuto();
            }
        }

        /// <summary>
        /// Gets or sets the specific element.
        /// </summary>
        /// <param name="idPrefix">The prefix of the identifier for resource group.</param>
        /// <param name="id">The identifier in the resource group.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">id was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="KeyNotFoundException">The identifier does not exist.</exception>
        public T this[string idPrefix, string id]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id), "id should not be null, empty or consists only of white-space characters.");
                var info = GetInfo(idPrefix, id);
                if (info == null) throw new KeyNotFoundException("The identifier does not exist.");
                return info.Value;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id), "id should not be null, empty or consists only of white-space characters.");
                if (string.IsNullOrEmpty(idPrefix)) items[id] = new ItemInfo(id, value);
                else GetItemsForPrefix()[GetIdWithPrefix(idPrefix, id)] = new ItemInfo(idPrefix, id, value);
                RemoveExpiredAuto();
            }
        }

        /// <summary>
        /// Gets the cache item info.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="initialization">The value initialization. It will be called to generate a new value when no cache.</param>
        /// <param name="expiration">The expiration for initialization.</param>
        /// <returns>The cache item info.</returns>
        public ItemInfo GetInfo(string id, Func<T> initialization = null, TimeSpan? expiration = null)
        {
            return GetInfo(null, id, initialization, expiration);
        }

        /// <summary>
        /// Gets the cache item info.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="id">The identifier in the resource group.</param>
        /// <param name="initialization">The value initialization. It will be called to generate a new value when no cache.</param>
        /// <param name="expiration">The expiration for initialization.</param>
        /// <returns>The cache item info.</returns>
        public ItemInfo GetInfo(string prefix, string id, Func<T> initialization = null, TimeSpan? expiration = null)
        {
            if (string.IsNullOrEmpty(id)) return null;
            ItemInfo info = null;
            if (string.IsNullOrEmpty(prefix))
            {
                items.TryGetValue(id, out info);
            }
            else if (items2 != null)
            {
                items2.TryGetValue(GetIdWithPrefix(prefix, id), out info);
            }

            if (info == null)
            {
                return Add(prefix, id, initialization, expiration);
            }

            if (info.IsExpired(Expiration))
            {
                if (string.IsNullOrEmpty(prefix)) items.TryRemove(id, out _);
                return Add(prefix, id, initialization, expiration);
            }

            return info;
        }

        /// <summary>
        /// Gets the cache item info.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The cache item info.</returns>
        public ItemInfo GetInfo(Func<ItemInfo, bool> predicate)
        {
            if (predicate is null) predicate = ele => true;
            try
            {
                return items.Select(ele => ele.Value).LastOrDefault(predicate);
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }

            try
            {
                return items.Select(ele => ele.Value).LastOrDefault(predicate);
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }

            return items.Select(ele => ele.Value).LastOrDefault(predicate);
        }

        /// <summary>
        /// Gets the cache item info.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="initialization">The value initialization. It will be called to generate a new value when no cache.</param>
        /// <param name="expiration">The expiration for initialization.</param>
        /// <returns>The cache item info.</returns>
        public Task<ItemInfo> GetInfoAsync(string id, Func<Task<T>> initialization = null, TimeSpan? expiration = null)
        {
            return GetInfoAsync(null, id, initialization, expiration);
        }

        /// <summary>
        /// Gets the cache item info.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="id">The identifier in the resource group.</param>
        /// <param name="initialization">The value initialization. It will be called to generate a new value when no cache.</param>
        /// <param name="expiration">The expiration for initialization.</param>
        /// <returns>The cache item info.</returns>
        public async Task<ItemInfo> GetInfoAsync(string prefix, string id, Func<Task<T>> initialization = null, TimeSpan? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            if (string.IsNullOrEmpty(prefix))
            {
                return items.TryGetValue(id, out var r) && !r.IsExpired(Expiration)
                    ? r
                    : await AddAsync(prefix, id, initialization, expiration);
            }

            return GetItemsForPrefix().TryGetValue(GetIdWithPrefix(prefix, id), out var info) && !info.IsExpired(Expiration)
                ? info
                : await AddAsync(prefix, id, initialization, expiration);
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
            var result = await GetInfoAsync(null, id, initialization, expiration);
            if (result is null) throw new KeyNotFoundException("The identifier does not exist.");
            return result.Value;
        }

        /// <summary>
        /// Gets the cache item info.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="initialization">The value initialization. It will be called to generate a new value when no cache.</param>
        /// <param name="expiration">The expiration for initialization.</param>
        /// <returns>The cache item info.</returns>
        public async Task<T> GetAsync(string prefix, string id, Func<Task<T>> initialization = null, TimeSpan? expiration = null)
        {
            var result = await GetInfoAsync(prefix, id, initialization, expiration);
            if (result is null) throw new KeyNotFoundException("The identifier does not exist.");
            return result.Value;
        }

        /// <summary>
        /// Tries to get the cache item info.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="result">The output result.</param>
        /// <returns>true if has the info and it is not expired; otherwise, false.</returns>
        public bool TryGetInfo(string id, out ItemInfo result)
        {
            return TryGetInfo(null, id, out result);
        }

        /// <summary>
        /// Tries to get the cache item info.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="id">The identifier in the resource group.</param>
        /// <param name="result">The output result.</param>
        /// <returns>true if has the info and it is not expired; otherwise, false.</returns>
        public bool TryGetInfo(string prefix, string id, out ItemInfo result)
        {
            result = GetInfo(prefix, id);
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
            return TryGet(null, id, out data);
        }

        /// <summary>
        /// Tries to get the cache item data.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="id">The identifier in the resource group.</param>
        /// <param name="data">The output data.</param>
        /// <returns>true if has the info and it is not expired; otherwise, false.</returns>
        public bool TryGet(string prefix, string id, out T data)
        {
            var result = GetInfo(prefix, id);
            if (result is null)
            {
                data = default;
                return false;
            }

            data = result.Value;
            return true;
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="item">The object to be added to the end of the collection.</param>
        public void Add(ItemInfo item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id) || item.IsExpired(Expiration)) return;
            if (string.IsNullOrEmpty(item.Prefix)) items[item.Id] = item;
            else GetItemsForPrefix()[$"{item.Prefix}\t{item.Id}"] = item;
            RemoveExpiredAuto();
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiration">The optional expiration to override current policy.</param>
        public void Add(string id, T value, TimeSpan? expiration = null)
        {
            Add(new ItemInfo(null, id, value, expiration));
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="idPrefix">The prefix of the identifier for resource group.</param>
        /// <param name="id">The identifier in the resource group.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiration">The optional expiration to override current policy.</param>
        public void Add(string idPrefix, string id, T value, TimeSpan? expiration = null)
        {
            Add(new ItemInfo(idPrefix, id, value, expiration));
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            items.Clear();
            if (items2 != null) items2.Clear();
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        /// <param name="onlyNoPrefix">true if clear the items with no prefix only; otherwise, false.</param>
        public void Clear(bool onlyNoPrefix)
        {
            items.Clear();
            if (!onlyNoPrefix && items2 != null) items2.Clear();
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        public void Clear(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                items.Clear();
                return;
            }

            if (items2 is null) return;
            var col = RemoveAll(ele => ele.Prefix == prefix);
        }

        /// <summary>
        /// Determines whether an element is in the collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise, false.</returns>
        public bool Contains(ItemInfo item)
        {
            if (string.IsNullOrEmpty(item.Id)) return false;
            return string.IsNullOrEmpty(item.Prefix)
                ? Contains(item.Id)
                : Contains(item.Prefix, item.Id);
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
        /// <param name="idPrefix">The prefix of the identifier for resource group.</param>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise, false.</returns>
        public bool ContainsValue(string idPrefix, T item)
        {
            if (string.IsNullOrEmpty(idPrefix)) return ContainsValue(item);
            if (items2 == null) return false;
            if (item is null)
            {
                foreach (var ele in items2)
                {
                    if (!ele.Key.StartsWith(idPrefix)) continue;
                    if (ele.Value is null && !ele.Value.IsExpired(Expiration)) return true;
                }

                return false;
            }

            foreach (var ele in items2)
            {
                if (!ele.Key.StartsWith(idPrefix)) continue;
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
        /// Determines whether an element is in the collection.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="id">The identifier in the resource group.</param>
        /// <returns>true if item is found in the collection; otherwise, false.</returns>
        public bool Contains(string prefix, string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            if (string.IsNullOrEmpty(prefix)) return Contains(id);
            if (items2 == null) return false;
            var key = GetIdWithPrefix(prefix, id);
            if (!items2.TryGetValue(key, out var info) || info == null) return false;
            if (!info.IsExpired(Expiration)) return true;
            items2.TryRemove(key, out _);
            return false;
        }

        /// <summary>
        /// Registers a value resolving factory.
        /// </summary>
        /// <param name="factory">The value resovling factory.</param>
        /// <param name="timeout">An optional time span that represents the number of milliseconds to wait</param>
        public void Register(Func<string, Task<T>> factory, TimeSpan? timeout = null)
        {
            Register(string.Empty, factory, timeout);
        }

        /// <summary>
        /// Registers a value resolving factory.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="factory">The value resovling factory.</param>
        /// <param name="timeout">An optional time span that represents the number of milliseconds to wait</param>
        public void Register(string prefix, Func<string, Task<T>> factory, TimeSpan? timeout = null)
        {
            if (items3 == null)
            {
                lock (locker)
                {
                    if (items3 == null) items3 = new ConcurrentDictionary<string, FactoryInfo>();
                }
            }

            if (factory is null) items3.TryRemove(prefix, out _);
            else items3[prefix] = new FactoryInfo() { Factory = factory, Timeout = timeout };
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from System.Collections.Generic.List`1. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex was less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(ItemInfo[] array, int arrayIndex)
        {
            items.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from System.Collections.Generic.List`1. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex was less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            items.Select(ele => ele.Value.Value).ToList().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from System.Collections.Generic.List`1. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex was less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(string prefix, ItemInfo[] array, int arrayIndex)
        {
            GetItemsForPrefix().Where(ele => ele.Key.StartsWith(prefix)).Select(ele => ele.Value).ToList().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from System.Collections.Generic.List`1. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">array was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex was less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(string prefix, T[] array, int arrayIndex)
        {
            GetItemsForPrefix().Where(ele => ele.Key.StartsWith(prefix)).Select(ele => ele.Value.Value).ToList().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
        public bool Remove(ItemInfo item)
        {
            if (item?.Id == null) return false;
            if (string.IsNullOrEmpty(item.Prefix))
            {
                return items.TryRemove(item.Id, out _);
            }

            return GetItemsForPrefix().TryRemove(GetIdWithPrefix(item), out _);
        }

        /// <summary>
        /// Removes the occurrence of a specific value from the collection.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
        public int RemoveAll(Predicate<ItemInfo> predicate)
        {
            return RemoveAll(false, predicate);
        }

        /// <summary>
        /// Removes the occurrence of a specific value from the collection.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
        public int RemoveAll(string prefix, Predicate<ItemInfo> predicate)
        {
            if (string.IsNullOrEmpty(prefix)) return RemoveAll(true, predicate);
            if (items2 is null) return 0;
            var i = 0;
            var col = items2.Where(ele => predicate(ele.Value)).Select(ele => ele.Key).ToList();
            foreach (var item in col)
            {
                if (items2.TryRemove(item, out _)) i++;
            }

            return i;
        }

        /// <summary>
        /// Removes the occurrence of a specific value from the collection.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="onlyNoPrefix">true if clear the items with no prefix only; otherwise, false.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
        public int RemoveAll(bool onlyNoPrefix, Predicate<ItemInfo> predicate)
        {
            if (predicate == null) return 0;
            var i = 0;
            var col = items.Where(ele => predicate(ele.Value)).Select(ele => ele.Key).ToList();
            foreach (var item in col)
            {
                if (items.TryRemove(item, out _)) i++;
            }

            if (!onlyNoPrefix && items2 != null)
            {
                col = items2.Where(ele => predicate(ele.Value)).Select(ele => ele.Key).ToList();
                foreach (var item in col)
                {
                    if (items2.TryRemove(item, out _)) i++;
                }
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
        {
            return Remove(id, out _);
        }

        /// <summary>
        /// Removes the occurrence with the specific identifier from the collection.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="result">The result deleted.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
        public bool Remove(string id, out ItemInfo result)
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
        /// Removes the occurrence with the specific identifier from the collection.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="id">The identifier in the resource group.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
        public bool Remove(string prefix, string id)
        {
            return Remove(prefix, id, out _);
        }

        /// <summary>
        /// Removes the occurrence with the specific identifier from the collection.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <param name="id">The identifier in the resource group.</param>
        /// <param name="result">The result deleted.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the collection.</returns>
        public bool Remove(string prefix, string id, out ItemInfo result)
        {
            if (id == null)
            {
                result = null;
                return false;
            }

            RemoveExpiredAuto();
            return string.IsNullOrEmpty(prefix)
                ? items.TryRemove(id, out result)
                : GetItemsForPrefix().TryRemove(GetIdWithPrefix(prefix, id), out result);
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
                if (items2 != null) items2.Clear();
            }
            else
            {
                while (Count > maxCount) RemoveEarliest();
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
                if (items2 != null) items2.Clear();
            }
            else
            {
                while (Count > maxCount) RemoveEarliest();
            }
        }

        /// <summary>
        /// Removes the earliest elements.
        /// </summary>
        /// <param name="result">The item removed.</param>
        public void RemoveEarliest(out ItemInfo result)
        {
            try
            {
                ItemInfo r = null;
                var is2 = false;
                foreach (var item in items)
                {
                    if (r == null || item.Value.CreationDate < r.CreationDate) r = item.Value;
                }

                if (items2 != null)
                {
                    foreach (var item in items2)
                    {
                        if (r == null || item.Value.CreationDate < r.CreationDate)
                        {
                            r = item.Value;
                            is2 = true;
                        }
                    }

                    if (is2)
                    {
                        items2.TryRemove(GetIdWithPrefix(r), out result);
                        return;
                    }
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
        {
            RemoveEarliest(out _);
        }

        /// <summary>
        /// Tests if the item is expired.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>true if expired; otherwise, false.</returns>
        public bool IsExpired(ItemInfo item)
        {
            return item.IsExpired(Expiration);
        }

        /// <summary>
        /// Converts to a list.
        /// </summary>
        /// <returns>A list copied.</returns>
        public List<ItemInfo> ToList()
        {
            return ToList(false);
        }

        /// <summary>
        /// Converts to a list.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <returns>A list copied.</returns>
        public List<ItemInfo> ToList(string prefix)
        {
            return (string.IsNullOrEmpty(prefix) ? items : GetItemsForPrefix().Where(ele => ele.Value?.Prefix == prefix && !ele.Value.IsExpired(Expiration))).Select(ele => ele.Value).ToList();
        }

        /// <summary>
        /// Converts to a list.
        /// </summary>
        /// <param name="onlyNoPrefix">true if clear the items with no prefix only; otherwise, false.</param>
        /// <returns>A list copied.</returns>
        public List<ItemInfo> ToList(bool onlyNoPrefix)
        {
            var col = items.Select(ele => ele.Value).Where(ele => ele?.IsExpired(Expiration) == false);
            if (!onlyNoPrefix && items2 != null) col = col.Union(items2.Select(ele => ele.Value).Where(ele => ele?.IsExpired(Expiration) == false));
            return col.ToList();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ItemInfo> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <param name="prefix">The prefix of the identifier for resource group.</param>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ItemInfo> GetEnumerator(string prefix)
        {
            return ToList(prefix).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <param name="onlyNoPrefix">true if clear the items with no prefix only; otherwise, false.</param>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ItemInfo> GetEnumerator(bool onlyNoPrefix)
        {
            return ToList(onlyNoPrefix).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static string GetIdWithPrefix(ItemInfo info)
        {
            if (string.IsNullOrEmpty(info?.Prefix)) return null;
            return $"{info.Prefix}\t{info.Id}";
        }

        private static string GetIdWithPrefix(string prefix, string id)
        {
            return $"{prefix}\t{id}";
        }

        private ConcurrentDictionary<string, ItemInfo> GetItemsForPrefix()
        {
            if (items2 != null) return items2;
            lock (locker)
            {
                if (items2 == null) items2 = new ConcurrentDictionary<string, ItemInfo>();
                return items2;
            }
        }

        private ItemInfo Add(string prefix, string id, Func<T> initialization = null, TimeSpan? expiration = null)
        {
            if (initialization == null)
            {
                _ = GetByFactoryAsync(prefix, id);
                return null;
            }

            var obj = initialization();
            var info = new ItemInfo(prefix, id, obj, expiration);
            if (info.IsExpired(Expiration)) return null;
            Add(info);
            return info;
        }

        private async Task<ItemInfo> AddAsync(string prefix, string id, Func<Task<T>> initialization = null, TimeSpan? expiration = null)
        {
            if (initialization == null)
            {
                if (string.IsNullOrEmpty(prefix)) items.TryRemove(id, out _);
                else if (items2 != null) items2.TryRemove(GetIdWithPrefix(prefix, id), out _);
                return await GetByFactoryAsync(prefix, id);
            }

            var obj = await initialization();
            var info = new ItemInfo(prefix, id, obj, expiration);
            if (info.IsExpired(Expiration))
            {
                if (string.IsNullOrEmpty(prefix)) items.TryGetValue(id, out info);
                else if (items2 != null) items2.TryGetValue(GetIdWithPrefix(prefix, id), out info);
                if (info == null) return null;
                return null;
            }

            Add(info);
            return info;
        }

        private async Task<ItemInfo> GetByFactoryAsync(string prefix, string id)
        {
            if (string.IsNullOrEmpty(id) || items3 == null || !items3.TryGetValue(prefix ?? string.Empty, out var factory)) return null;
            if (!string.IsNullOrEmpty(prefix)) GetItemsForPrefix();
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
            ItemInfo r;
            #pragma warning restore IDE0018
            if (string.IsNullOrEmpty(prefix) ? items.TryGetValue(id, out r) : items2.TryGetValue(GetIdWithPrefix(prefix, id), out r))
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
                if (string.IsNullOrEmpty(prefix))
                {
                    r = new ItemInfo(id, v);
                    items[id] = r;
                }
                else
                {
                    r = new ItemInfo(prefix, id, v);
                    items2[GetIdWithPrefix(prefix, id)] = r;
                }

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
                greater = Count > MaxCount.Value;
            }
            catch (InvalidOperationException)
            {
            }

            if (!greater && DateTime.Now < (cleanUpTime + (Expiration ?? TimeSpan.Zero))) return;
            RemoveExpired();
        }
    }
}
