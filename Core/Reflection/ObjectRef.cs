using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Reflection
{
    /// <summary>
    /// Object reference interface.
    /// </summary>
    public interface IObjectRef
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        object Value { get; }
    }

    /// <summary>
    /// Object reference interface.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public interface IObjectRef<T> : IObjectRef
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        new T Value { get; }
    }

    /// <summary>
    /// Object reference.
    /// </summary>
    public sealed class ObjectRef : IObjectRef
    {
        private readonly IObjectRef reference;

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ObjectRef(IObjectRef value) => reference = value != reference ? value : new InstanceObjectRef(null);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="factory">The value.</param>
        public ObjectRef(Func<object> factory) => reference = new FactoryObjectRef(factory);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ObjectRef(object value) => reference = new InstanceObjectRef(value);

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value => reference.Value;

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="factory">The value.</param>
        public static IObjectRef Create(Func<object> factory) => new FactoryObjectRef(factory);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public static IObjectRef Create(object value) => new InstanceObjectRef(value);
    }

    /// <summary>
    /// Object reference.
    /// </summary>
    public sealed class ObjectRef<T> : IObjectRef<T>, IObjectRef
    {
        private readonly IObjectRef<T> reference;

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ObjectRef(IObjectRef<T> value) => reference = value != reference ? value : new InstanceObjectRef<T>(default);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="factory">The value.</param>
        public ObjectRef(Func<T> factory) => reference = new FactoryObjectRef<T>(factory);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="lazy">The lazy initialization.</param>
        public ObjectRef(Lazy<T> lazy) => reference = new LazyObjectRef<T>(lazy);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ObjectRef(T value) => reference = new InstanceObjectRef<T>(value);

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value => reference.Value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        object IObjectRef.Value => reference.Value;

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="factory">The value.</param>
        public static IObjectRef<T> Create(Func<T> factory) => new FactoryObjectRef<T>(factory);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="lazy">The lazy initialization.</param>
        public static IObjectRef<T> Create(Lazy<T> lazy) => new LazyObjectRef<T>(lazy);

        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public static IObjectRef<T> Create(T value) => new InstanceObjectRef<T>(value);
    }

    /// <summary>
    /// Instance object reference.
    /// </summary>
    internal class InstanceObjectRef : IObjectRef
    {
        /// <summary>
        /// Initializes a new instance of the InstanceObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public InstanceObjectRef(object value) => Value = value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; }
    }

    /// <summary>
    /// Instance object reference.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    internal class InstanceObjectRef<T> : InstanceObjectRef, IObjectRef<T>
    {
        /// <summary>
        /// Initializes a new instance of the ObjectRef class.
        /// </summary>
        /// <param name="value">The value.</param>
        public InstanceObjectRef(T value) : base(value) => Value = value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public new T Value { get; }
    }

    /// <summary>
    /// Object reference for lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    internal class LazyObjectRef<T> : IObjectRef, IObjectRef<T>
    {
        private readonly Lazy<T> value;

        /// <summary>
        /// Initializes a new instance of the LazyObjectRef class.
        /// </summary>
        /// <param name="lazy">The lazy initialization.</param>
        public LazyObjectRef(Lazy<T> lazy) => value = lazy;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value => value.Value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        object IObjectRef.Value => value.Value;
    }

    /// <summary>
    /// Object reference for thread safe factory.
    /// </summary>
    internal class FactoryObjectRef : IObjectRef
    {
        private readonly object locker = new object();
        private readonly Func<object> f;
        private bool isInit;
        private object value;

        /// <summary>
        /// Initializes a new instance of the FactoryObjectRef class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public FactoryObjectRef(Func<object> factory) => f = factory;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value
        {
            get
            {
                if (isInit) return value;
                lock (locker)
                {
                    if (isInit) return value;
                    isInit = true;
                    return value = f();
                }
            }
        }
    }

    /// <summary>
    /// Object reference for thread safe factory.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    internal class FactoryObjectRef<T> : IObjectRef, IObjectRef<T>
    {
        private readonly object locker = new object();
        private readonly Func<T> f;
        private bool isInit;
        private T value;

        /// <summary>
        /// Initializes a new instance of the FactoryObjectRef class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public FactoryObjectRef(Func<T> factory) => f = factory;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value
        {
            get
            {
                if (isInit) return value;
                lock (locker)
                {
                    if (isInit) return value;
                    isInit = true;
                    return value = f();
                }
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        object IObjectRef.Value => Value;
    }

    /// <summary>
    /// A model with the relationship between the item selected and its parent.
    /// </summary>
    public class SelectionRelationship<TParent, TItem>
    {
        /// <summary>
        /// Initializes a new instance of the SelectionRelationship class.
        /// </summary>
        public SelectionRelationship()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SelectionRelationship class.
        /// </summary>
        /// <param name="tuple">The parent and item selected.</param>
        public SelectionRelationship((TParent, TItem) tuple)
        {
            IsSelected = true;
            Parent = tuple.Item1;
            ItemSelected = tuple.Item2;
        }

        /// <summary>
        /// Initializes a new instance of the SelectionRelationship class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public SelectionRelationship(TParent parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the SelectionRelationship class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="itemSelected">The item selected.</param>
        public SelectionRelationship(TParent parent, TItem itemSelected)
        {
            IsSelected = true;
            Parent = parent;
            ItemSelected = itemSelected;
        }

        /// <summary>
        /// Gets a value indicating whether there is an item selected.
        /// </summary>
        public bool IsSelected { get; }

        /// <summary>
        /// Gets the parent which contains the item.
        /// </summary>
        public TParent Parent { get; }

        /// <summary>
        /// Gets the item selected.
        /// </summary>
        public TItem ItemSelected { get; }

        /// <summary>
        /// Converts to tuple.
        /// </summary>
        /// <param name="value">The value.</param>
        public static explicit operator (TParent, TItem, bool)(SelectionRelationship<TParent, TItem> value)
        {
            return value != null ? (value.Parent, value.ItemSelected, value.IsSelected) : (default(TParent), default(TItem), false);
        }
    }
}
