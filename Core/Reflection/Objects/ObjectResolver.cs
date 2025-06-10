using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trivial.Net;

namespace Trivial.Reflection;

/// <summary>
/// The object resolver.
/// </summary>
/// <typeparam name="T">The type of instance.</typeparam>
public interface IObjectResolver<T>
{
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    T GetInstance();
}

/// <summary>
/// The object resolver used to create an instance of a type using a factory method or a reference.
/// </summary>
/// <typeparam name="T">The type of the instance to create.</typeparam>
public sealed class FactoryObjectResolver<T> : IObjectResolver<T>
{
    private readonly Func<T> handler;

    /// <summary>
    /// Initializes a new instance of the FactoryObjectResolver class.
    /// </summary>
    /// <param name="create">The handler to create instance.</param>
    public FactoryObjectResolver(Func<T> create = null)
    {
        handler = create ?? Activator.CreateInstance<T>;
    }

    /// <summary>
    /// Initializes a new instance of the FactoryObjectResolver class.
    /// </summary>
    /// <param name="reference">The object reference.</param>
    public FactoryObjectResolver(IObjectRef<T> reference)
    {
        reference ??= new FactoryObjectRef<T>(null);
        handler = reference is IObjectResolver<T> r ? r.GetInstance : () => reference.Value;
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns>The instance.</returns>
    public T GetInstance()
        => handler();

    /// <summary>
    /// Creates an object reference.
    /// </summary>
    /// <returns>The object refernce.</returns>
    public static IObjectResolver<T> Create()
        => new FactoryObjectResolver<T>(Activator.CreateInstance<T>);

    /// <summary>
    /// Creates an object reference.
    /// </summary>
    /// <param name="factory">The value.</param>
    /// <returns>The object refernce.</returns>
    public static IObjectResolver<T> Create(Func<T> factory)
        => new FactoryObjectResolver<T>(factory);

    /// <summary>
    /// Creates an object reference.
    /// </summary>
    /// <param name="lazy">The lazy initialization.</param>
    /// <returns>The object refernce.</returns>
    public static IObjectResolver<T> Create(Lazy<T> lazy)
        => new LazyObjectRef<T>(lazy);

    /// <summary>
    /// Creates an object reference.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The object refernce.</returns>
    public static IObjectResolver<T> Create(T value)
        => new InstanceObjectRef<T>(value);

    /// <summary>
    /// Creates an object reference.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The object refernce.</returns>
    public static IObjectResolver<T> Create(IObjectRef<T> value)
        => value is IObjectResolver<T> r ? r : new FactoryObjectResolver<T>(value);
}
