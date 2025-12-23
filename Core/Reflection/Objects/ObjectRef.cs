using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using Trivial.Tasks;

namespace Trivial.Reflection;

/// <summary>
/// Object reference interface.
/// </summary>
public interface IObjectRef
{
    /// <summary>
    /// Gets the source value.
    /// </summary>
    object Value { get; }

    /// <summary>
    /// Gets a value that indicates whether a value has been created for this instance.
    /// </summary>
    bool IsValueCreated { get; }
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
/// Object reference to maintain a singleton or a factory instance.
/// It can be used to create an instance lazily or to hold a reference to an existing instance.
/// </summary>
/// <example>
/// <code>
/// var ref1 = new ObjectRef(() => new object());
/// var ref2 = new ObjectRef(new object());
/// var ref3 = new ObjectRef(ref1);
/// </code>
/// </example>
[DebuggerDisplay("{ValueToDisplay}")]
public sealed class ObjectRef : IObjectRef
{
    private readonly IObjectRef reference;

    /// <summary>
    /// Initializes a new instance of the ObjectRef class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ObjectRef(IObjectRef value) => reference = value ?? new InstanceObjectRef(null);

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
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public object Value => reference.Value;

    /// <summary>
    /// Gets a value that indicates whether a value has been created for this instance.
    /// </summary>
    public bool IsValueCreated => reference.IsValueCreated;

    /// <summary>
    /// Gets the value in Collection to debugging display; or TaskStates.Pending if the value has not ready.
    /// </summary>
    internal object ValueToDisplay => reference.IsValueCreated ? reference.Value : TaskStates.Pending;

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
[DebuggerDisplay("{ValueToDisplay}")]
public sealed class ObjectRef<T> : IObjectRef<T>, IObjectResolver<T>
{
    private readonly IObjectRef<T> reference;

    /// <summary>
    /// Initializes a new instance of the ObjectRef class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ObjectRef(IObjectRef<T> value) => reference = value ?? new InstanceObjectRef<T>(default);

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
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T Value => reference.Value;

    /// <summary>
    /// Gets the value.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object IObjectRef.Value => reference.Value;

    /// <summary>
    /// Gets a value that indicates whether a value has been created for this instance.
    /// </summary>
    public bool IsValueCreated => reference.IsValueCreated;

    /// <summary>
    /// Gets the value in Collection to debugging display; or TaskStates.Pending if the value has not ready.
    /// </summary>
    internal object ValueToDisplay => reference.IsValueCreated ? reference.Value : TaskStates.Pending;

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
[DebuggerDisplay("{Value}")]
internal class InstanceObjectRef(object value) : IObjectRef
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public object Value { get; } = value;

    /// <summary>
    /// Gets a value that indicates whether a value has been created for this instance.
    /// </summary>
    public bool IsValueCreated => true;
}

/// <summary>
/// Instance object reference.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="value">The value.</param>
[DebuggerDisplay("{Value}")]
internal class InstanceObjectRef<T>(T value) : InstanceObjectRef(value), IObjectRef<T>, IObjectResolver<T>
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
[DebuggerDisplay("{ValueToDisplay}")]
internal class LazyObjectRef<T>(Lazy<T> lazy) : IObjectRef<T>, IObjectResolver<T>
{
    private readonly Lazy<T> value = lazy;

    /// <summary>
    /// Gets the value.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T Value => value.Value;

    /// <summary>
    /// Gets the value.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object IObjectRef.Value => value.Value;

    /// <summary>
    /// Gets a value that indicates whether a value has been created for this instance.
    /// </summary>
    public bool IsValueCreated => value.IsValueCreated;

    /// <summary>
    /// Gets the value in Collection to debugging display; or TaskStates.Pending if the value has not ready.
    /// </summary>
    internal object ValueToDisplay => value.IsValueCreated ? value.Value : TaskStates.Pending;

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
[DebuggerDisplay("{ValueToDisplay}")]
internal class FactoryObjectRef : IObjectRef
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if NETCOREAPP
    private readonly Lock locker = new();
#else
    private readonly object locker = new();
#endif

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Func<object> f;

    private object value;

    /// <summary>
    /// Initializes a new instance of the FactoryObjectRef class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    public FactoryObjectRef(Func<object> factory) => f = factory;

    /// <summary>
    /// Gets a value that indicates whether a value has been created for this instance.
    /// </summary>
    public bool IsValueCreated { get; private set; }

    /// <summary>
    /// Gets the value in Collection to debugging display; or TaskStates.Pending if the value has not ready.
    /// </summary>
    internal object ValueToDisplay => IsValueCreated ? Value : TaskStates.Pending;

    /// <summary>
    /// Gets the value.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public object Value
    {
        get
        {
            if (IsValueCreated) return value;
            lock (locker)
            {
                if (IsValueCreated) return value;
                var obj = f();
                if (IsValueCreated) return value;
                IsValueCreated = true;
                return value = obj;
            }
        }
    }
}

/// <summary>
/// Object reference for thread safe factory.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
[DebuggerDisplay("{ValueToDisplay}")]
internal class FactoryObjectRef<T> : IObjectRef<T>, IObjectResolver<T>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if NETCOREAPP
    private readonly Lock locker = new();
#else
    private readonly object locker = new();
#endif
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Func<T> f;

    private T value;

    /// <summary>
    /// Initializes a new instance of the FactoryObjectRef class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    public FactoryObjectRef(Func<T> factory) => f = factory ?? Activator.CreateInstance<T>;

    /// <summary>
    /// Gets the value.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T Value
    {
        get
        {
            if (IsValueCreated) return value;
            lock (locker)
            {
                if (IsValueCreated) return value;
                var obj = f();
                if (IsValueCreated) return value;
                IsValueCreated = true;
                return value = f();
            }
        }
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object IObjectRef.Value => Value;

    /// <summary>
    /// Gets a value that indicates whether a value has been created for this instance.
    /// </summary>
    public bool IsValueCreated { get; private set; }

    /// <summary>
    /// Gets the value in Collection to debugging display; or TaskStates.Pending if the value has not ready.
    /// </summary>
    internal object ValueToDisplay => IsValueCreated ? Value : TaskStates.Pending;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    T IObjectResolver<T>.GetInstance()
        => Value;
}
