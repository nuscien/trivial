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
/// The object reference to maintain a singleton.
/// </summary>
public sealed class ObjectRef<T> : IObjectRef<T>, IObjectRef, IObjectResolver<T>
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
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    T IObjectResolver<T>.GetInstance()
        => Value;

    /// <summary>
    /// Creates an object reference.
    /// </summary>
    /// <returns>The object refernce.</returns>
    public static IObjectRef<T> Create() => new InstanceObjectRef<T>(Activator.CreateInstance<T>());

    /// <summary>
    /// Creates an object reference.
    /// </summary>
    /// <param name="factory">The value.</param>
    /// <returns>The object refernce.</returns>
    public static IObjectRef<T> Create(Func<T> factory) => new FactoryObjectRef<T>(factory);

    /// <summary>
    /// Creates an object reference.
    /// </summary>
    /// <param name="lazy">The lazy initialization.</param>
    /// <returns>The object refernce.</returns>
    public static IObjectRef<T> Create(Lazy<T> lazy) => new LazyObjectRef<T>(lazy);

    /// <summary>
    /// Creates an object reference.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The object refernce.</returns>
    public static IObjectRef<T> Create(T value) => new InstanceObjectRef<T>(value);

    internal static bool ReferenceEquals(T a, T b) => object.ReferenceEquals(a, b);
}

/// <summary>
/// Instance object reference.
/// </summary>
/// <param name="value">The value.</param>
internal class InstanceObjectRef(object value) : IObjectRef
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public object Value { get; } = value;
}

/// <summary>
/// Instance object reference.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="value">The value.</param>
internal class InstanceObjectRef<T>(T value) : InstanceObjectRef(value), IObjectRef<T>, IObjectResolver<T>
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public new T Value { get; } = value;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    T IObjectResolver<T>.GetInstance()
        => Value;
}

/// <summary>
/// Object reference for lazy initialization.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="lazy">The lazy initialization.</param>
internal class LazyObjectRef<T>(Lazy<T> lazy) : IObjectRef, IObjectRef<T>, IObjectResolver<T>
{
    private readonly Lazy<T> value = lazy;

    /// <summary>
    /// Gets the value.
    /// </summary>
    public T Value => value.Value;

    /// <summary>
    /// Gets the value.
    /// </summary>
    object IObjectRef.Value => value.Value;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    T IObjectResolver<T>.GetInstance()
        => Value;
}

/// <summary>
/// Object reference for thread safe factory.
/// </summary>
internal class FactoryObjectRef : IObjectRef
{
    private readonly object locker = new();
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
internal class FactoryObjectRef<T> : IObjectRef, IObjectRef<T>, IObjectResolver<T>
{
    private readonly object locker = new ();
    private readonly Func<T> f;
    private bool isInit;
    private T value;

    /// <summary>
    /// Initializes a new instance of the FactoryObjectRef class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    public FactoryObjectRef(Func<T> factory) => f = factory ?? Activator.CreateInstance<T>;

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

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    T IObjectResolver<T>.GetInstance()
        => Value;
}
