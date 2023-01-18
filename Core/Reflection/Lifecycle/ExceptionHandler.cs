using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Reflection;

/// <summary>
/// The exception handler instance.
/// </summary>
public class ExceptionHandler
{
    /// <summary>
    /// The item handler.
    /// </summary>
    /// <param name="ex">The exception to test.</param>
    /// <param name="handled">true if handled; otherwise, false.</param>
    /// <returns>The exception need throw; or null, if ignore.</returns>
    public delegate Exception ItemHandler(Exception ex, out bool handled);

    /// <summary>
    /// The item of exception handlers registered.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Initializes a new instance of the ExceptionHandler.Item class.
        /// </summary>
        /// <param name="type">The exception type.</param>
        /// <param name="handler">The catch handler.</param>
        public Item(Type type, ItemHandler handler)
        {
            Type = type;
            Handler = handler;
        }

        /// <summary>
        /// Gets the exception type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the catch handler.
        /// </summary>
        public ItemHandler Handler { get; private set; }
    }

    /// <summary>
    /// The item of exception handlers registered.
    /// </summary>
    public class Item<T> : Item, IEquatable<object>, IEquatable<Item>, IEquatable<Item<T>> where T : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ExceptionHandler.Item class.
        /// </summary>
        /// <param name="handler">The catch handler.</param>
        public Item(Func<T, Exception> handler) : base(typeof(T), (Exception ex, out bool handled) =>
        {
            if (ex is T exConverted)
            {
                handled = true;
                return handler?.Invoke(exConverted);
            }

            handled = false;
            return ex;
        })
        {
            Handler = handler;
        }

        /// <summary>
        /// Gets the catch handler.
        /// </summary>
        public new Func<T, Exception> Handler { get; private set; }

        /// <summary>
        /// Tests if the given item equals the current one.
        /// </summary>
        /// <param name="other">The given item.</param>
        /// <returns>true if the given item equals the current one; otherwise, false.</returns>
        public bool Equals(Item<T> other)
            => other is not null && other.Handler == Handler;

        /// <summary>
        /// Tests if the given item equals the current one.
        /// </summary>
        /// <param name="other">The given item.</param>
        /// <returns>true if the given item equals the current one; otherwise, false.</returns>
        public bool Equals(Item other)
        {
            if (other is null) return false;
            if (other is Item<T> item) return item.Handler == Handler;
            return other.Handler == base.Handler && other.Type == Type;
        }

        /// <summary>
        /// Tests if the given item equals the current one.
        /// </summary>
        /// <param name="other">The given item.</param>
        /// <returns>true if the given item equals the current one; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is Item item) return Equals(item);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
            => Handler != null ? Handler.GetHashCode() : 0.GetHashCode();
    }

    /// <summary>
    /// The catch handler list.
    /// </summary>
    private readonly List<Item> list = new();

    /// <summary>
    /// Gets a value indicating whether need test all catch handler until an exception returned rather than try-catch logic.
    /// </summary>
    public bool TestAll { get; set; }

    /// <summary>
    /// Gets the count of the catch handler.
    /// </summary>
    public int Count => list.Count;

    /// <summary>
    /// Tests if need throw an exception.
    /// </summary>
    /// <param name="ex">The exception catched.</param>
    /// <returns>The exception needed to throw.</returns>
    public Exception GetException(Exception ex)
    {
        if (ex == null) return null;
        var has = false;
        foreach (var item in list)
        {
            var result = item.Handler(ex, out bool handled);
            if (handled) has = true;
            if (handled && (!TestAll || result != null)) return result;
        }

        return has ? null : ex;
    }

    /// <summary>
    /// Tests if need throw an exception.
    /// </summary>
    /// <param name="ex">The exception catched.</param>
    /// <returns>The exception needed to throw.</returns>
    public void ThrowIfUnhandled(Exception ex)
    {
        ex = GetException(ex);
        if (ex != null) throw ex;
    }

    /// <summary>
    /// Checks whether has a given catch handler.
    /// </summary>
    /// <typeparam name="T">The type of exception to try to catch.</typeparam>
    /// <param name="catchHandler">The handler to return if need throw an exception.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public bool Contains<T>(Func<T, Exception> catchHandler = null) where T : Exception
    {
        foreach (var item in list)
        {
            if (item is Item<T> itemConverted && itemConverted.Handler == catchHandler) return true;
        }

        return false;
    }

    /// <summary>
    /// Inserts a catch handler at the specified index.
    /// </summary>
    /// <typeparam name="T">The type of exception to try to catch.</typeparam>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="catchHandler">The handler to return if need throw an exception.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the list of catch handler registered.</exception>
    public void Insert<T>(int index, Func<T, Exception> catchHandler = null) where T : Exception
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} should be a natural number.");
        if (index >= list.Count) throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} should be less than {list.Count}.");
        Remove(catchHandler);
        list.Insert(index, new Item<T>(catchHandler));
    }

    /// <summary>
    /// Adds a catch handler.
    /// </summary>
    /// <typeparam name="T">The type of exception to try to catch.</typeparam>
    /// <param name="catchHandler">The handler to return if need throw an exception.</param>
    public void Add<T>(Func<T, Exception> catchHandler = null) where T : Exception
    {
        if (Contains(catchHandler)) return;
        list.Add(new Item<T>(catchHandler));
    }

    /// <summary>
    /// Removes a catch handler.
    /// </summary>
    /// <typeparam name="T">The type of exception to try to catch.</typeparam>
    /// <param name="catchHandler">The handler to return if need throw an exception.</param>
    public bool Remove<T>(Func<T, Exception> catchHandler) where T : Exception
    {
        var removing = new List<Item>();
        foreach (var item in list)
        {
            if (item is Item<T> itemConverted && itemConverted.Handler == catchHandler) removing.Add(item);
        }

        var count = removing.Count;
        foreach (var item in removing)
        {
            list.Remove(item);
        }

        return count > 0;
    }

    /// <summary>
    /// Removes the catch handler at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">index is not a valid index in the list of catch handler registered.</exception>
    public void RemoveAt(int index)
        => list.RemoveAt(index);

    /// <summary>
    /// Removes a catch handler.
    /// </summary>
    /// <typeparam name="T">The type of exception to try to catch.</typeparam>
    public bool Remove<T>() where T : Exception
    {
        var removing = new List<Item>();
        foreach (var item in list)
        {
            if (item is Item<T>) removing.Add(item);
        }

        var count = removing.Count;
        foreach (var item in removing)
        {
            list.Remove(item);
        }

        return count > 0;
    }

    /// <summary>
    /// Clears all catch handlers.
    /// </summary>
    public void Clear()
        => list.Clear();
}
