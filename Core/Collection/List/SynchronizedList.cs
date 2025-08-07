using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using Trivial.Data;
using Trivial.Reflection;

namespace Trivial.Collection;

/// <summary>
/// Represents a thread-safe list of objects.
/// It uses reader writer lock slim to control access to the list.
/// </summary>
/// <typeparam name="T">The type of the elements to be stored in the list.</typeparam>
public class SynchronizedList<T> : IList<T>, ICloneable, INotifyPropertyChanged, INotifyCollectionChanged
{
    private readonly List<T> list;
    private readonly ReaderWriterLockSlim slim;

    /// <summary>
    /// Initializes a new instance of the ConcurrentList class.
    /// </summary>
    public SynchronizedList()
    {
        slim = new();
        list = new();
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
            list = new();
            return;
        }

        try
        {
            list = new(collection);
        }
        catch (NullReferenceException)
        {
            list = new(collection);
        }
        catch (InvalidOperationException)
        {
            list = new(collection);
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
            list = new();
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
            list = new(collection);
        }
        catch (NullReferenceException)
        {
            list = new(collection);
        }
        catch (InvalidOperationException)
        {
            list = new(collection);
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
            }
            catch (SynchronizationLockException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                if (s.IsUpgradeableReadLockHeld) s.ExitUpgradeableReadLock();
            }
            catch (SynchronizationLockException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
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
            catch (SynchronizationLockException)
            {
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
        catch (InvalidOperationException)
        {
        }
    }

    /// <summary>
    /// Occurs when an item is added, removed, changed, moved, or the entire JSON array is refreshed.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    private event PropertyChangedEventHandler notifyPropertyChanged;
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add
        {
            notifyPropertyChanged += value;
        }

        remove
        {
            notifyPropertyChanged -= value;
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

            OnPropertyChanged(true);
            Changed?.Invoke(this, new(old, value, ChangeMethods.Update, index));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Replace, value, old, index));
        }
    }

#if !NETFRAMEWORK
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
            var i = -1;
            slim.EnterWriteLock();
            try
            {
                i = index.GetOffset(list.Count);
                old = i < list.Count ? list[i] : default;
                list[i] = value;
            }
            finally
            {
                slim.ExitWriteLock();
            }

            OnPropertyChanged(true);
            Changed?.Invoke(this, new(old, value, ChangeMethods.Update, i));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Replace, value, old, i));
        }
    }
#endif

    /// <summary>
    /// Gets the revision token of member-wised property updated.
    /// </summary>
    public object RevisionToken { get; private set; } = new();

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

            OnPropertyChanged(true);
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

        OnPropertyChanged(true);
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

            OnPropertyChanged(true);
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

        OnPropertyChanged(true);
        Sorted?.Invoke(this, new EventArgs());
    }

    /// <inheritdoc />
    public void Add(T item)
    {
        var count = 0;
        slim.EnterWriteLock();
        try
        {
            count = list.Count;
            list.Add(item);
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged();
        Changed?.Invoke(this, new(default, item, ChangeMethods.Add, count));
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, item, count));
    }

    /// <summary>
    /// Adds the elements of the specified collection to the end of the list.
    /// </summary>
    /// <param name="collection">The collection whose elements should be added to the end of the list.</param>
    public void AddRange(IEnumerable<T> collection)
    {
        if (collection is null) return;
        if (Changed == null && CollectionChanged == null)
        {
            slim.EnterWriteLock();
            try
            {
                list.AddRange(collection);
            }
            finally
            {
                slim.ExitWriteLock();
            }

            OnPropertyChanged();
            return;
        }

        var copied = new List<T>(collection);
        var count = 0;
        slim.EnterWriteLock();
        try
        {
            count = list.Count;
            list.AddRange(copied);
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged();
        for (var i = 0; i < copied.Count; i++)
        {
            var j = i + count;
            Changed?.Invoke(this, new(default, copied[i], ChangeMethods.Add, j));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, copied[i], j));
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        List<T> copied = null;
        slim.EnterWriteLock();
        try
        {
            if (Changed != null) copied = new List<T>(list);
            list.Clear();
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged();
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        if (copied == null || Changed == null) return;
        for (var i = 0; i < copied.Count; i++)
        {
            Changed?.Invoke(this, new(copied[i], default, ChangeMethods.Remove, i));
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

        OnPropertyChanged();
        Changed?.Invoke(this, new(default, item, ChangeMethods.Add, index));
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, item, index));
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

        OnPropertyChanged();
        if (Changed == null && CollectionChanged == null) return;
        var copied = new List<T>(collection);
        foreach (var item in copied)
        {
            Changed?.Invoke(this, new(default, item, ChangeMethods.Add, index));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, item, index));
        }
    }

    /// <summary>
    /// Updates an element.
    /// </summary>
    /// <param name="index">The zero-based index at which the new elements should be updated.</param>
    /// <param name="value">The new value of the element to update.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- index is greater than count of the list.</exception>
    public void Update(int index, T value)
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

        OnPropertyChanged(true);
        Changed?.Invoke(this, new(old, value, ChangeMethods.Update, index));
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Replace, value, old, index));
    }

    /// <summary>
    /// Updates an element.
    /// </summary>
    /// <param name="index">The zero-based index at which the new elements should be updated.</param>
    /// <param name="action">An Handler to update the value of the specific element.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- index is greater than count of the list.</exception>
    public void Update(int index, Func<T, T> action)
    {
        T old;
        T value;
        slim.EnterWriteLock();
        try
        {
            old = index < list.Count ? list[index] : default;
            value = action(old);
            list[index] = value;
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged(true);
        Changed?.Invoke(this, new(old, value, ChangeMethods.Update, index));
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Replace, value, old, index));
    }

    /// <summary>
    /// Updates an element.
    /// </summary>
    /// <param name="index">The zero-based index at which the new elements should be updated.</param>
    /// <param name="action">An Handler to update the value of the specific element.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- index is greater than count of the list.</exception>
    public void Update(int index, Func<T, int, T> action)
    {
        T old;
        T value;
        slim.EnterWriteLock();
        try
        {
            old = index < list.Count ? list[index] : default;
            value = action(old, index);
            list[index] = value;
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged(true);
        Changed?.Invoke(this, new(old, value, ChangeMethods.Update, index));
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Replace, value, old, index));
    }

    /// <summary>
    /// Replaces an item to a new one. Only the first one will be replaced. No Handler if the old item does not exist.
    /// </summary>
    /// <param name="oldItem">The old item to remove.</param>
    /// <param name="newItem">The new item to update.</param>
    /// <returns>true if replace succeeded; otherwise, false, e.g. the old item does not exist.</returns>
    public bool ReplaceFirst(T oldItem, T newItem)
    {
        if (ReferenceEquals(oldItem, newItem)) return true;
        var i = -1;
        slim.EnterWriteLock();
        try
        {
            if (Changed != null || CollectionChanged != null) i = list.IndexOf(oldItem);
            if (!list.Remove(oldItem)) return false;
            list.Insert(i, newItem);
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged(true);
        if (i >= 0)
        {
            Changed?.Invoke(this, new(oldItem, newItem, ChangeMethods.Update, i));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Replace, newItem, oldItem, i));
        }

        return true;
    }

    /// <summary>
    /// Replaces an item to a new one for all.
    /// </summary>
    /// <param name="oldItem">The old item to remove.</param>
    /// <param name="newItem">The new item to update.</param>
    /// <param name="compare">The optional compare handler. Or null to test by reference equaling.</param>
    /// <returns>The count of item to replace.</returns>
    public int Replace(T oldItem, T newItem, Func<T, T, bool> compare = null)
    {
        if (ReferenceEquals(oldItem, newItem)) return 0;
        var count = 0;
        compare ??= ObjectRef<T>.ReferenceEquals;
        var args = new List<ChangeEventArgs<T>>();
        slim.EnterWriteLock();
        try
        {
            for (var i = 0; i < list.Count; i++)
            {
                var test = list[i];
                if (!compare(test, oldItem)) continue;
                list[i] = newItem;
                count++;
                args.Add(new ChangeEventArgs<T>(test, newItem, ChangeMethods.Update, i));
            }
        }
        catch (ArgumentOutOfRangeException)
        {
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged(true);
        if (Changed != null || CollectionChanged != null)
        {
            foreach (var argsItem in args)
            {
                Changed?.Invoke(this, argsItem);
                if (int.TryParse(argsItem.Key, out var i)) CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, argsItem.NewValue, argsItem.OldValue, i));
            }
        }

        return count;
    }

    /// <summary>
    /// Replaces an item to a new one for all.
    /// </summary>
    /// <param name="oldItem">The old item to remove.</param>
    /// <param name="newItem">The new item to update.</param>
    /// <param name="maxCount">The maximum count of item to replace.</param>
    /// <param name="compare">The optional compare handler. Or null to test by reference equaling.</param>
    /// <returns>The count of item to replace.</returns>
    public int Replace(T oldItem, T newItem, int maxCount, Func<T, T, bool> compare = null)
    {
        if (ReferenceEquals(oldItem, newItem)) return 0;
        var count = 0;
        compare ??= ObjectRef<T>.ReferenceEquals;
        var args = new List<ChangeEventArgs<T>>();
        slim.EnterWriteLock();
        try
        {
            for (var i = 0; i < list.Count; i++)
            {
                var test = list[i];
                if (!compare(test, oldItem)) continue;
                list[i] = newItem;
                count++;
                args.Add(new ChangeEventArgs<T>(test, newItem, ChangeMethods.Update, i));
                if (count >= maxCount) break;
            }
        }
        catch (ArgumentOutOfRangeException)
        {
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged(true);
        if (Changed != null || CollectionChanged != null)
        {
            foreach (var argsItem in args)
            {
                Changed?.Invoke(this, argsItem);
                if (int.TryParse(argsItem.Key, out var i)) CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, argsItem.NewValue, argsItem.OldValue, i));
            }
        }

        return count;
    }

    /// <summary>
    /// Replaces by given test handler.
    /// </summary>
    /// <param name="replace">The handler to compare old item to return the new one.</param>
    /// <returns>The count of item to replace.</returns>
    public int Replace(Func<T, int, T> replace)
    {
        if (replace == null) return 0;
        var count = 0;
        var args = new List<ChangeEventArgs<T>>();
        slim.EnterWriteLock();
        try
        {
            for (var i = 0; i < list.Count; i++)
            {
                var test = list[i];
                var item = replace(test, i);
                if (ReferenceEquals(test, item)) continue;
                list[i] = item;
                count++;
                args.Add(new ChangeEventArgs<T>(test, item, ChangeMethods.Update, i));
            }
        }
        catch (ArgumentOutOfRangeException)
        {
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged(true);
        if (Changed != null || CollectionChanged != null)
        {
            foreach (var argsItem in args)
            {
                Changed?.Invoke(this, argsItem);
                if (int.TryParse(argsItem.Key, out var i)) CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, argsItem.NewValue, argsItem.OldValue, i));
            }
        }

        return count;
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        var i = -1;
        slim.EnterWriteLock();
        try
        {
            if (Changed != null && CollectionChanged != null) i = list.IndexOf(item);
            if (!list.Remove(item)) return false;
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged();
        if (i >= 0)
        {
            Changed?.Invoke(this, new(item, default, ChangeMethods.Remove, i));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, item, i));
        }

        return true;
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        if (Changed == null && CollectionChanged == null)
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

            OnPropertyChanged();
            return;
        }

        T item = default;
        slim.EnterWriteLock();
        try
        {
            item = list[index];
            list.RemoveAt(index);
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged();
        Changed?.Invoke(this, new(item, default, ChangeMethods.Remove, index));
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, item, index));
    }

    /// <summary>
    /// Removes the System.Collections.Generic.IList`1 item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    /// <param name="itemRemoved">The item removed.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the list.</exception>
    public void RemoveAt(int index, out T itemRemoved)
    {
        slim.EnterWriteLock();
        try
        {
            itemRemoved = list[index];
            list.RemoveAt(index);
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged();
        Changed?.Invoke(this, new(itemRemoved, default, ChangeMethods.Remove, index));
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, itemRemoved, index));
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
        slim.EnterWriteLock();
        try
        {
            if (Changed != null || CollectionChanged != null) copied = list.Where(ele => match(ele)).ToList();
            result = list.RemoveAll(match);
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged();
        if ((Changed != null || CollectionChanged != null) && copied != null)
        {
            for (var i = 0; i < copied.Count; i++)
            {
                Changed?.Invoke(this, new(copied[i], default, ChangeMethods.Remove, i));
                CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, copied[i], i));
            }
        }

        return result;
    }

    /// <summary>
    /// Removes all the elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// <param name="match">The predicate delegate that defines the conditions of the elements to remove.</param>
    /// <param name="itemsRemoved">The items removed.</param>
    /// <returns>The number of elements removed from the list.</returns>
    public void RemoveAll(Predicate<T> match, out IList<T> itemsRemoved)
    {
        if (match is null)
        {
            itemsRemoved = new List<T>();
            return;
        }

        List<T> copied = null;
        slim.EnterWriteLock();
        try
        {
            copied = list.Where(ele => match(ele)).ToList();
            list.RemoveAll(match);
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged();
        itemsRemoved = copied;
        if ((Changed == null && CollectionChanged == null) || copied == null) return;
        for (var i = 0; i < copied.Count; i++)
        {
            Changed?.Invoke(this, new(copied[i], default, ChangeMethods.Remove, i));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, copied[i], i));
        }
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
        slim.EnterWriteLock();
        try
        {
            if (Changed != null || CollectionChanged != null) copied = list.Skip(index).Take(count).ToList();
            list.RemoveRange(index, count);
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged();
        if ((Changed == null && CollectionChanged == null) || copied == null) return;
        for (var i = 0; i < copied.Count; i++)
        {
            Changed?.Invoke(this, new(copied[i], default, ChangeMethods.Remove, i));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, copied[i], i));
        }
    }

    /// <summary>
    /// Removes a range of elements from the list.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
    /// <param name="count">The number of elements to remove.</param>
    /// <param name="itemsRemoved">The items removed.</param>
    /// <exception cref="ArgumentNullException">index is less than 0. -or- count is less than 0.</exception>
    /// <exception cref="ArgumentException">index and count do not denote a valid range of elements in the list.</exception>
    public void RemoveRange(int index, int count, out IList<T> itemsRemoved)
    {
        List<T> copied = null;
        slim.EnterWriteLock();
        try
        {
            copied = list.Skip(index).Take(count).ToList();
            list.RemoveRange(index, count);
        }
        finally
        {
            slim.ExitWriteLock();
        }

        OnPropertyChanged();
        itemsRemoved = copied;
        if ((Changed == null && CollectionChanged == null) || copied == null) return;
        for (var i = 0; i < copied.Count; i++)
        {
            Changed?.Invoke(this, new(copied[i], default, ChangeMethods.Remove, i));
            CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, copied[i], i));
        }
    }

    /// <summary>
    /// Copies the elements of the list to a new array.
    /// </summary>
    /// <returns>An array containing copies of the elements of the list.</returns>
    public T[] ToArray()
    {
        slim.EnterReadLock();
        try
        {
            return list.ToArray();
        }
        finally
        {
            slim.ExitReadLock();
        }
    }

    /// <summary>
    /// Copies the elements of the list to a new array.
    /// </summary>
    /// <returns>An array containing copies of the elements of the list.</returns>
    /// <exception cref="ArgumentOutOfRangeException">index is less than 0. -or- count is less than 0.</exception>
    /// <exception cref="ArgumentException">index and count do not denote a valid range of elements in the list.</exception>
    public T[] ToArray(int start, int? length = null)
    {
        slim.EnterReadLock();
        try
        {
            var col = list.Skip(start);
            if (length.HasValue) col = col.Take(length.Value);
            return col.ToArray();
        }
        finally
        {
            slim.ExitReadLock();
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Creates a new concurrent list that is a copy of the current instance.
    /// </summary>
    /// <returns>A new concurrent list that is a copy of this instance.</returns>
    public SynchronizedList<T> Clone()
    {
        slim.EnterReadLock();
        try
        {
            return new(list);
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
            return new(col);
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
        => Clone();

    private void OnPropertyChanged(bool onlyItemUpdate = false)
    {
        RevisionToken = new();
        if (!onlyItemUpdate) notifyPropertyChanged?.Invoke(this, new(nameof(Count)));
        notifyPropertyChanged?.Invoke(this, new("Item[]"));
    }
}
