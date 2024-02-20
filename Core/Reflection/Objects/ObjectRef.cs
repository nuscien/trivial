using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Trivial.Reflection;

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
    public ObjectRef(IObjectRef value) => reference = value;

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
    public ObjectRef(IObjectRef<T> value) => reference = value;

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

    internal static bool ReferenceEquals(T a, T b) => object.ReferenceEquals(a, b);
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
    private readonly object locker = new ();
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
    private readonly object locker = new ();
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
    /// <return>The tuple.</return>
    public static explicit operator (TParent, TItem, bool)(SelectionRelationship<TParent, TItem> value)
    {
        return value != null ? (value.Parent, value.ItemSelected, value.IsSelected) : (default(TParent), default(TItem), false);
    }
}

/// <summary>
/// The breadcrumb for node.
/// </summary>
public class NodePathBreadcrumb
{
    /// <summary>
    /// Initializes a new instance of the NodePathBreadcrumb class.
    /// </summary>
    /// <param name="parent">The parent breadcrumb.</param>
    public NodePathBreadcrumb(NodePathBreadcrumb parent)
    {
        ParentBreadcrumb = parent;
    }

    /// <summary>
    /// Initializes a new instance of the NodePathBreadcrumb class.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="parent">The parent breadcrumb.</param>
    /// <param name="property">The property name in parent.</param>
    public NodePathBreadcrumb(object obj, NodePathBreadcrumb parent, string property = null)
        : this(parent)
    {
        Current = obj;
        PropertyName = property;
    }

    /// <summary>
    /// Gets the current object.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object Current { get; }

    /// <summary>
    /// Gets the parent breadcrumb.
    /// </summary>
    [JsonPropertyName("parent")]
    public NodePathBreadcrumb ParentBreadcrumb { get; }

    /// <summary>
    /// Gets a value indicating whether has parent.
    /// </summary>
    [JsonIgnore]
    public bool HasParent => ParentBreadcrumb != null;

    /// <summary>
    /// Gets the parent object.
    /// </summary>
    [JsonIgnore]
    public object Parent => ParentBreadcrumb?.Current;

    /// <summary>
    /// Gets the property name in parent.
    /// </summary>
    [JsonPropertyName("property")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PropertyName { get; }

    /// <summary>
    /// Gets or sets the additional tag.
    /// </summary>
    [JsonPropertyName("tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object Tag { get; set; }

    /// <summary>
    /// Gets the type of the current object.
    /// </summary>
    /// <returns>The type of the current object.</returns>
    public Type GetCurrentType() => Current?.GetType();

    /// <summary>
    /// Gets the root breadcrumb;
    /// </summary>
    /// <returns>The root breadcrumb.</returns>
    public NodePathBreadcrumb GetRootBreadcrumb()
    {
        var root = this;
        while (true)
        {
            var parent = root.ParentBreadcrumb;
            if (parent is null) break;
            if (ReferenceEquals(parent, this)) return null;
            root = parent;
        }

        return root;
    }

    /// <summary>
    /// Gets the root object;
    /// </summary>
    /// <returns>The root object.</returns>
    public object GetRoot()
        => GetRootBreadcrumb()?.Current;
}

/// <summary>
/// The breadcrumb for node.
/// </summary>
/// <typeparam name="T">The type of the object.</typeparam>
public class NodePathBreadcrumb<T> : NodePathBreadcrumb
{
    /// <summary>
    /// Initializes a new instance of the NodePathBreadcrumb class.
    /// </summary>
    /// <param name="current">The object.</param>
    /// <param name="parent">The parent breadcrumb.</param>
    /// <param name="property">The property name in parent.</param>
    public NodePathBreadcrumb(T current, NodePathBreadcrumb parent, string property = null) : base(current, parent, property)
    {
        Current = current;
    }

    /// <summary>
    /// Gets the current object.
    /// </summary>
    public new T Current { get; }
}
