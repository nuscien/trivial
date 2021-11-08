﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Trivial.Text
{
    /// <summary>
    /// Represents a thread-safe list of objects.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be stored in the list.</typeparam>
    internal class SynchronizedList<T> : IList<T>, ICloneable
    {
        private readonly List<T> list;
        private readonly ReaderWriterLockSlim slim;

        /// <summary>
        /// Initializes a new instance of the ConcurrentList class.
        /// </summary>
        public SynchronizedList()
        {
            slim = new ReaderWriterLockSlim();
            list = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the ConcurrentList class.
        /// </summary>
        /// <param name="collection">The collection of elements used to initialize the thread-safe collection.</param>
        public SynchronizedList(IEnumerable<T> collection)
        {
            slim = new ReaderWriterLockSlim();
            if (collection is null)
            {
                list = new List<T>();
                return;
            }

            try
            {
                list = new List<T>(collection);
            }
            catch (NullReferenceException)
            {
                list = new List<T>(collection);
            }
            catch (InvalidOperationException)
            {
                list = new List<T>(collection);
            }
        }

        /// <summary>
        /// Initializes a new instance of the ConcurrentList class.
        /// </summary>
        /// <param name="recursionPolicy">One of the enumeration values that specifies the lock recursion policy.</param>
        /// <param name="collection">The collection of elements used to initialize the thread-safe collection.</param>
        /// <param name="useSource">true if set the collection as source directly instead of copying; otherwise, false.</param>
        internal SynchronizedList(LockRecursionPolicy recursionPolicy, IEnumerable<T> collection = null, bool useSource = false)
        {
            slim = new ReaderWriterLockSlim(recursionPolicy);
            if (collection is null)
            {
                list = new List<T>();
                return;
            }

            if (useSource)
            {
                if (collection is List<T> l)
                {
                    list = l;
                    return;
                }
                else if (collection is SynchronizedList<T> sl)
                {
                    list = sl.list;
                    return;
                }
            }

            try
            {
                list = new List<T>(collection);
            }
            catch (NullReferenceException)
            {
                list = new List<T>(collection);
            }
            catch (InvalidOperationException)
            {
                list = new List<T>(collection);
            }
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~SynchronizedList()
        {
            if (slim == null) return;
            try
            {
                slim.Dispose();
            }
            catch (SynchronizationLockException)
            {
                var s = slim;
                try
                {
                    if (s.IsWriteLockHeld) s.ExitWriteLock();
                    if (s.IsUpgradeableReadLockHeld) s.ExitUpgradeableReadLock();
                    if (s.IsReadLockHeld) s.ExitReadLock();
                }
                catch (SynchronizationLockException)
                {
                }
                catch (InvalidOperationException)
                {
                }

                try
                {
                    s.Dispose();
                }
                catch (InvalidOperationException)
                {
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Adds or removes the event handler raised on items sorted.
        /// </summary>
        public event EventHandler Sorted;

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                slim.EnterReadLock();
                try
                {
                    return list[index];
                }
                finally
                {
                    slim.ExitReadLock();
                }
            }

            set
            {
                T old;
                slim.EnterWriteLock();
                try
                {
                    old = index < list.Count ? list[index] : default;
                    list[index] = value;
                }
                finally
                {
                    slim.ExitWriteLock();
                }
            }
        }

#if !NETOLDVER
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- index is equal to or greater than count of the list.</exception>
        public T this[Index index]
        {
            get
            {
                slim.EnterReadLock();
                try
                {
                    return list[index];
                }
                finally
                {
                    slim.ExitReadLock();
                }
            }

            set
            {
                T old;
                slim.EnterWriteLock();
                try
                {
                    var i = index.GetOffset(list.Count);
                    old = i < list.Count ? list[i] : default;
                    list[i] = value;
                }
                finally
                {
                    slim.ExitWriteLock();
                }
            }
        }
#endif

        /// <inheritdoc />
        public int Count
        {
            get
            {
                slim.EnterReadLock();
                try
                {
                    return list.Count;
                }
                finally
                {
                    slim.ExitReadLock();
                }
            }
        }

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the total number of threads that are waiting to enter the lock in read mode.
        /// </summary>
        public int WaitingReadCount => slim.WaitingReadCount;

        /// <summary>
        /// Gets a value that indicates whether the current thread has entered the lock in write mode.
        /// </summary>
        public bool IsWriteLockHeld => slim.IsWriteLockHeld;

        /// <summary>
        /// Gets a value that indicates whether the current thread has entered the lock in upgradeable mode.
        /// </summary>
        public bool IsUpgradeableReadLockHeld => slim.IsUpgradeableReadLockHeld;

        /// <summary>
        /// Gets a value that indicates whether the current thread has entered the lock in read mode.
        /// </summary>
        public bool IsReadLockHeld => slim.IsReadLockHeld;

        /// <summary>
        /// Gets the total number of unique threads that have entered the lock in read mode.
        /// </summary>
        public int CurrentReadCount => slim.CurrentReadCount;

        /// <summary>
        /// Gets the total number of threads that are waiting to enter the lock in upgradeable mode.
        /// </summary>
        public int WaitingUpgradeCount => slim.WaitingUpgradeCount;

        /// <summary>
        /// Gets the total number of threads that are waiting to enter the lock in write mode.
        /// </summary>
        public int WaitingWriteCount => slim.WaitingWriteCount;

        /// <summary>
        /// <para>
        /// Gets the number of times the current thread has entered the lock in write mode, as an indication of recursion.
        /// </para>
        /// <para>
        /// 0 if the current thread has not entered write mode,
        /// 1 if the thread has entered write mode but has not entered it recursively,
        /// or n if the thread has entered write mode recursively n - 1 times.
        /// </para>
        /// </summary>
        internal int RecursiveWriteCount => slim.RecursiveWriteCount;

        /// <summary>
        /// <para>
        /// Gets the number of times the current thread has entered the lock in upgradeable mode, as an indication of recursion.
        /// </para>
        /// <para>
        /// 0 if the current thread has not entered upgradeable mode,
        /// 1 if the thread has entered upgradeable mode but has not entered it recursively,
        /// or n if the thread has entered upgradeable mode recursively n - 1 times.
        /// </para>
        /// </summary>
        internal int RecursiveUpgradeCount => slim.RecursiveUpgradeCount;

        /// <summary>
        /// <para>
        /// Gets the number of times the current thread has entered the lock in read mode, as an indication of recursion.
        /// </para>
        /// 0 if the current thread has not entered read mode,
        /// 1 if the thread has entered read mode but has not entered it recursively,
        /// or n if the thread has entered the lock recursively n - 1 times.
        /// </summary>
        internal int RecursiveReadCount => slim.RecursiveReadCount;

        /// <summary>
        /// Tries to get the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="result">The result output.</param>
        /// <returns>true if contains; otherwise, false.</returns>
        public bool TryGet(int index, out T result)
        {
            if (index >= 0)
            {
                slim.EnterReadLock();
                try
                {
                    if (index < list.Count)
                    {
                        result = list[index];
                        return true;
                    }
                }
                catch (ArgumentException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                finally
                {
                    slim.ExitReadLock();
                }
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- count is less than 0.</exception>
        /// <exception cref="ArgumentException">index and count do not denote a valid range of elements in the list.</exception>
        public void Reverse(int index = 0, int? count = null)
        {
            if (!count.HasValue)
            {
                slim.EnterWriteLock();
                try
                {
                    list.Reverse(index, list.Count - index);
                }
                finally
                {
                    slim.ExitWriteLock();
                }

                Sorted?.Invoke(this, new EventArgs());
                return;
            }

            slim.EnterWriteLock();
            try
            {
                list.Reverse(index, count.Value);
            }
            finally
            {
                slim.ExitWriteLock();
            }

            Sorted?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Sorts the elements in a range of elements in list.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The optional comparer implementation to use when comparing elements.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- count is less than 0.</exception>
        /// <exception cref="ArgumentException">index and count do not denote a valid range of elements in the list. -or- The implementation of comparer caused an error during the sort.</exception>
        /// <exception cref="InvalidOperationException">comparer is null, and the default comparer cannot find implementation.</exception>
        public void Sort(int index = 0, int? count = null, IComparer<T> comparer = null)
        {
            if (!count.HasValue)
            {
                slim.EnterWriteLock();
                try
                {
                    list.Sort(index, list.Count - index, comparer);
                }
                finally
                {
                    slim.ExitWriteLock();
                }

                Sorted?.Invoke(this, new EventArgs());
                return;
            }

            slim.EnterWriteLock();
            try
            {
                list.Sort(index, count.Value, comparer);
            }
            finally
            {
                slim.ExitWriteLock();
            }

            Sorted?.Invoke(this, new EventArgs());
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            slim.EnterWriteLock();
            try
            {
                list.Add(item);
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the list.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection is null) return;
            slim.EnterWriteLock();
            try
            {
                list.AddRange(collection);
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            slim.EnterWriteLock();
            try
            {
                list.Clear();
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            slim.EnterReadLock();
            try
            {
                return list.Contains(item);
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            slim.EnterReadLock();
            try
            {
                list.CopyTo(array, arrayIndex);
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            List<T> col;
            slim.EnterReadLock();
            try
            {
                col = new List<T>(list);
            }
            finally
            {
                slim.ExitReadLock();
            }

            return col.GetEnumerator();
        }

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            slim.EnterReadLock();
            try
            {
                return list.IndexOf(item);
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the list that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the first occurrence of item within the range of elements in the list that starts at index and contains count number of elements, if found; otherwise, -1.</returns>
        public int IndexOf(T item, int index, int? count = null)
        {
            if (index < 0) index = 0;
            try
            {
                if (!count.HasValue)
                {
                    slim.EnterReadLock();
                    try
                    {
                        return list.IndexOf(item, index);
                    }
                    finally
                    {
                        slim.ExitReadLock();
                    }
                }

                slim.EnterReadLock();
                try
                {
                    return list.IndexOf(item, index, count.Value);
                }
                finally
                {
                    slim.ExitReadLock();
                }
            }
            catch (ArgumentException)
            {
                return -1;
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the entire list.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <returns>The zero-based index of the last occurrence of item within the entire the list, if found; otherwise, -1..</returns>
        public int LastIndexOf(T item)
        {
            slim.EnterReadLock();
            try
            {
                return list.LastIndexOf(item);
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the list that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <param name="index">The zero-based starting index of the backward search. 0 (zero) is valid in an empty list.</param>
        /// <returns>The zero-based index of the last occurrence of item within the range of elements in the list that extends from the first element to index, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is outside the range of valid indexes for the list.</exception>
        public int LastIndexOf(T item, int index)
        {
            slim.EnterReadLock();
            try
            {
                return list.LastIndexOf(item, index);
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the list that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <param name="index">The zero-based starting index of the backward search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>The zero-based index of the last occurrence of item within the range of elements in the list that extends from the first element to index, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is outside the range of valid indexes for the list. -or- count is less than 0. -or- index and count do not specify a valid section in the list.</exception>
        public int LastIndexOf(T item, int index, int count)
        {
            slim.EnterReadLock();
            try
            {
                return list.LastIndexOf(item, index, count);
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            slim.EnterWriteLock();
            try
            {
                list.Insert(index, item);
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// Inserts the elements of a collection into the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the list.</param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- index is greater than count of the list.</exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection is null) return;
            slim.EnterWriteLock();
            try
            {
                list.InsertRange(index, collection);
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            slim.EnterWriteLock();
            try
            {
                if (!list.Remove(item)) return false;
            }
            finally
            {
                slim.ExitWriteLock();
            }

            return true;
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            slim.EnterWriteLock();
            try
            {
                list.RemoveAt(index);
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The predicate delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the list.</returns>
        public int RemoveAll(Predicate<T> match)
        {
            if (match is null) return 0;
            var result = 0;
            slim.EnterWriteLock();
            try
            {
                result = list.RemoveAll(match);
            }
            finally
            {
                slim.ExitWriteLock();
            }

            return result;
        }

        /// <summary>
        /// Removes a range of elements from the list.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentNullException">index is less than 0. -or- count is less than 0.</exception>
        /// <exception cref="ArgumentException">index and count do not denote a valid range of elements in the list.</exception>
        public void RemoveRange(int index, int count)
        {
            slim.EnterWriteLock();
            try
            {
                list.RemoveRange(index, count);
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates a new concurrent list that is a copy of the current instance.
        /// </summary>
        /// <returns>A new concurrent list that is a copy of this instance.</returns>
        public SynchronizedList<T> Clone()
        {
            slim.EnterReadLock();
            try
            {
                return new SynchronizedList<T>(list);
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets the rage at the specified index.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to get.</param>
        /// <param name="count">The number of elements in the range to get.</param>
        /// <returns>The sub list.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- count is less than 0.</exception>
        /// <exception cref="ArgumentException">index and count do not denote a valid range of elements in the list.</exception>
        public SynchronizedList<T> Clone(int index, int? count = null)
        {
            slim.EnterReadLock();
            try
            {
                var col = list.Skip(index);
                if (count.HasValue) col = col.Take(count.Value);
                return new SynchronizedList<T>(col);
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets the rage at the specified index.
        /// </summary>
        /// <param name="indexes">The zero-based starting indexex to get.</param>
        /// <returns>The sub list.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- count is less than 0.</exception>
        /// <exception cref="ArgumentException">index and count do not denote a valid range of elements in the list.</exception>
        public SynchronizedList<T> Clone(IEnumerable<int> indexes)
        {
            var col = new SynchronizedList<T>();
            slim.EnterReadLock();
            try
            {
                col.AddRange(indexes.Select(ele => list[ele]));
            }
            finally
            {
                slim.ExitReadLock();
            }

            return col;
        }

        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}