using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Trivial.Data;

namespace Trivial.Collection
{
    /// <summary>
    /// Represents a thread-safe list of objects.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be stored in the list.</typeparam>
    public class ConcurrentList<T> : IList<T>, ICloneable
    {
        private readonly List<T> list;
        private readonly object locker;

        /// <summary>
        /// Initializes a new instance of the ConcurrentList class.
        /// </summary>
        public ConcurrentList()
        {
            locker = new object();
            list = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the ConcurrentList class.
        /// </summary>
        /// <param name="collection">The collection of elements used to initialize the thread-safe collection.</param>
        public ConcurrentList(IEnumerable<T> collection)
        {
            locker = new object();
            if (list is null)
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
        /// <param name="syncRoot">The object used to synchronize access the thread-safe collection.</param>
        /// <param name="collection">The collection of elements used to initialize the thread-safe collection.</param>
        /// <param name="useSource">true if set the collection as source directly instead of copying; otherwise, false.</param>
        public ConcurrentList(object syncRoot, IEnumerable<T> collection, bool useSource)
        {
            locker = syncRoot ?? new object();
            if (list is null)
            {
                list = new List<T>();
                return;
            }

            if (useSource && collection is List<T> l)
            {
                list = l;
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
        /// Adds or removes the event handler raised on item is added or removed.
        /// </summary>
        public event ChangeEventHandler<T> Changed;

        /// <summary>
        /// Adds or removes the event handler raised on items sorted.
        /// </summary>
        public event EventHandler Sorted;

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                lock (locker)
                {
                    return list[index];
                }
            }

            set
            {
                T old;
                lock (locker)
                {
                    old = index < list.Count ? list[index] : default;
                    list[index] = value;
                }

                Changed?.Invoke(this, new ChangeEventArgs<T>(old, value, ChangeMethods.Update, index));
            }
        }

#if !NETSTANDARD2_0
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
                lock (locker)
                {
                    return list[index];
                }
            }

            set
            {
                T old;
                var i = -1;
                lock (locker)
                {
                    i = index.GetOffset(list.Count);
                    old = i < list.Count ? list[i] : default;
                    list[i] = value;
                }

                Changed?.Invoke(this, new ChangeEventArgs<T>(old, value, ChangeMethods.Update, i));
            }
        }
#endif

        /// <inheritdoc />
        public int Count
        {
            get
            {
                lock (locker)
                {
                    return list.Count;
                }
            }
        }

        /// <inheritdoc />
        public bool IsReadOnly => false;

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
                lock (locker)
                {
                    list.Reverse(index, list.Count - index);
                }

                Sorted?.Invoke(this, new EventArgs());
                return;
            }

            lock (locker)
            {
                list.Reverse(index, count.Value);
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
                lock (locker)
                {
                    list.Sort(index, list.Count - index, comparer);
                }

                Sorted?.Invoke(this, new EventArgs());
                return;
            }

            lock (locker)
            {
                list.Sort(index, count.Value, comparer);
            }

            Sorted?.Invoke(this, new EventArgs());
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            var count = 0;
            lock (locker)
            {
                count = list.Count;
                list.Add(item);
            }

            Changed?.Invoke(this, new ChangeEventArgs<T>(default, item, ChangeMethods.Add, count));
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the list.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection is null) return;
            if (Changed == null)
            {
                lock (locker)
                {
                    list.AddRange(collection);
                }

                return;
            }

            var copied = new List<T>(collection);
            var count = 0;
            lock (locker)
            {
                count = list.Count;
                list.AddRange(copied);
            }

            for (var i = 0; i < copied.Count; i++)
            {
                Changed?.Invoke(this, new ChangeEventArgs<T>(default, copied[i], ChangeMethods.Add, i + count));
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            List<T> copied = null;
            lock (locker)
            {
                if (Changed != null) copied = new List<T>(list);
                list.Clear();
            }

            if (copied == null || Changed == null) return;
            for (var i = 0; i < copied.Count; i++)
            {
                Changed?.Invoke(this, new ChangeEventArgs<T>(copied[i], default, ChangeMethods.Remove, i));
            }
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            lock (locker)
            {
                return list.Contains(item);
            }
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (locker)
            {
                list.CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            List<T> col;
            lock (locker)
            {
                col = new List<T>(list);
            }

            return col.GetEnumerator();
        }

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            lock (locker)
            {
                return list.IndexOf(item);
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
                    lock (locker)
                    {
                        return list.IndexOf(item, index);
                    }
                }

                lock (locker)
                {
                    return list.IndexOf(item, index, count.Value);
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
            lock (locker)
            {
                return list.LastIndexOf(item);
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
            lock (locker)
            {
                return list.LastIndexOf(item, index);
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
            lock (locker)
            {
                return list.LastIndexOf(item, index, count);
            }
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            lock (locker)
            {
                list.Insert(index, item);
            }

            Changed?.Invoke(this, new ChangeEventArgs<T>(default, item, ChangeMethods.Add, index));
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
            lock (locker)
            {
                list.InsertRange(index, collection);
            }

            if (Changed == null) return;
            var copied = new List<T>(collection);
            foreach (var item in copied)
            {
                Changed?.Invoke(this, new ChangeEventArgs<T>(default, item, ChangeMethods.Add, index));
            }
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            var i = -1;
            lock (locker)
            {
                if (Changed != null) i = list.IndexOf(item);
                if (!list.Remove(item)) return false;
            }

            if (i >= 0) Changed?.Invoke(this, new ChangeEventArgs<T>(item, default, ChangeMethods.Remove, i));
            return true;
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            if (Changed == null)
            {
                lock (locker)
                {
                    list.RemoveAt(index);
                }

                return;
            }

            T item = default;
            lock (locker)
            {
                item = list[index];
                list.RemoveAt(index);
            }

            Changed?.Invoke(this, new ChangeEventArgs<T>(item, default, ChangeMethods.Remove, index));
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The predicate delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the list.</returns>
        public int RemoveAll(Predicate<T> match)
        {
            if (match is null) return 0;
            List<T> copied = null;
            var result = 0;
            lock (locker)
            {
                if (Changed != null) copied = list.Where(ele => match(ele)).ToList();
                result = list.RemoveAll(match);
            }

            if (Changed != null && copied != null)
            {
                for (var i = 0; i < copied.Count; i++)
                {
                    Changed?.Invoke(this, new ChangeEventArgs<T>(copied[i], default, ChangeMethods.Remove, i));
                }
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
            List<T> copied = null;
            lock (locker)
            {
                if (Changed != null) copied = list.Skip(index).Take(count).ToList();
                list.RemoveRange(index, count);
            }

            if (Changed == null || copied == null) return;
            for (var i = 0; i < copied.Count; i++)
            {
                Changed?.Invoke(this, new ChangeEventArgs<T>(copied[i], default, ChangeMethods.Remove, i));
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
        public ConcurrentList<T> Clone()
        {
            lock (locker)
            {
                return new ConcurrentList<T>(list);
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
        public ConcurrentList<T> Clone(int index, int? count = null)
        {
            lock (locker)
            {
                var col = list.Skip(index);
                if (count.HasValue) col = col.Take(count.Value);
                return new ConcurrentList<T>(col);
            }
        }

        /// <summary>
        /// Gets the rage at the specified index.
        /// </summary>
        /// <param name="indexes">The zero-based starting indexex to get.</param>
        /// <returns>The sub list.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- count is less than 0.</exception>
        /// <exception cref="ArgumentException">index and count do not denote a valid range of elements in the list.</exception>
        public ConcurrentList<T> Clone(IEnumerable<int> indexes)
        {
            var col = new ConcurrentList<T>();
            lock (locker)
            {
                col.AddRange(indexes.Select(ele => list[ele]));
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
