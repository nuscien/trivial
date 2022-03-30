using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Reflection;

/// <summary>
/// Factory set.
/// </summary>
public class FactorySet
{
    /// <summary>
    /// Reader writer lcok slim.
    /// </summary>
    private readonly ReaderWriterLockSlim slim = new ();

    /// <summary>
    /// Cache.
    /// </summary>
    private readonly Dictionary<Type, object> factories = new ();

    /// <summary>
    /// Deconstructor.
    /// </summary>
    ~FactorySet()
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
    /// Registers a factory.
    /// </summary>
    /// <typeparam name="T">The type of instance.</typeparam>
    /// <param name="factory">The factory to register.</param>
    public void Register<T>(Func<T> factory)
    {
        slim.EnterWriteLock();
        try
        {
            if (factories == null) factories.Remove(typeof(T));
            else factories[typeof(T)] = factory;
        }
        finally
        {
            slim.ExitWriteLock();
        }
    }

    /// <summary>
    /// Removes the specific factory.
    /// </summary>
    /// <typeparam name="T">The type of instance.</typeparam>
    public void Remove<T>() => Register<T>(null);

    /// <summary>
    /// Clears all the factories.
    /// </summary>
    public void Clear()
    {
        slim.EnterWriteLock();
        try
        {
            factories.Clear();
        }
        finally
        {
            slim.ExitWriteLock();
        }
    }

    /// <summary>
    /// Gets all the types registered.
    /// </summary>
    /// <returns>The type list.</returns>
    public IList<Type> GetAllTypes()
    {
        slim.EnterReadLock();
        try
        {
            return factories.Keys.ToList();
        }
        finally
        {
            slim.ExitReadLock();
        }
    }

    /// <summary>
    /// Register a singleton instance.
    /// </summary>
    /// <typeparam name="T">The type of the singleton.</typeparam>
    /// <param name="singleton">The singleton to register.</param>
    public void Singleton<T>(T singleton) => Register(() => singleton);

    /// <summary>
    /// Tests if there is a factory of the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <returns>true if contains; otherwise, false.</returns>
    public bool Has<T>() => !(GetFactory<T>() is null);

    /// <summary>
    /// Creates an instance of the specific type using the factory registered.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <returns>The instance.</returns>
    /// <exception cref="NotSupportedException">The factory of the instance was not registered yet.</exception>
    public T Create<T>()
    {
        var h = GetFactory<T>();
        if (h is null)
            throw new NotSupportedException("The factory of the instance has not been registered yet.");
        return h();
    }

    /// <summary>
    /// Creates an instance of the specific type using the async factory registered.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <exception cref="NotSupportedException">The factory of the instance was not registered yet.</exception>
    /// <returns>The instance in task.</returns>
    public Task<T> CreateAsync<T>() => Create<Task<T>>();

    /// <summary>
    /// Gets the factory.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <returns>The factory registered; or null, if non-exist.</returns>
    public Func<T> GetFactory<T>()
    {
        slim.EnterReadLock();
        try
        {
            if (factories.TryGetValue(typeof(T), out var h) && h is Func<T> r)
                return r;
        }
        finally
        {
            slim.ExitReadLock();
        }

        return null;
    }

    /// <summary>
    /// Gets the factory set instance.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>A factory set instance resolved.</returns>
    public static FactorySet Instance(string key = null)
    {
        return SingletonResolver.Instance.EnsureResolve<FactorySet>(key);
    }
}

/// <summary>
/// Routed factory.
/// </summary>
public class RoutedFactory<T>
{
    /// <summary>
    /// Reader writer lcok slim.
    /// </summary>
    private readonly ReaderWriterLockSlim slim = new ();

    /// <summary>
    /// Cache.
    /// </summary>
    private readonly Dictionary<string, Func<T>> factories = new();

    /// <summary>
    /// Backup factory.
    /// </summary>
    private Func<string, T> backup1;

    /// <summary>
    /// Backup factory.
    /// </summary>
    private Func<T> backup2;

    /// <summary>
    /// Gets the routed factory singleton.
    /// </summary>
    /// <returns>A routed factory instance resolved.</returns>
    public static RoutedFactory<T> Instance => SingletonResolver.Instance.EnsureResolve<RoutedFactory<T>>();

    /// <summary>
    /// Deconstructor.
    /// </summary>
    ~RoutedFactory()
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
    /// Gets a value indicating whether contains the backup factory.
    /// </summary>
    public bool HasBackup => backup1 != null || backup2 != null;

    /// <summary>
    /// Registers a factory.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="factory">The factory to register.</param>
    public void Register(string key, Func<T> factory)
    {
        if (key == null) key = string.Empty;
        slim.EnterWriteLock();
        try
        {
            if (factories == null) factories.Remove(key);
            else factories[key] = factory;
        }
        finally
        {
            slim.ExitWriteLock();
        }
    }

    /// <summary>
    /// Registers a factory.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="factory">The factory to register.</param>
    public void Register(string key, Func<string, T> factory) => Register(key, () => factory(key));

    /// <summary>
    /// Removes the specific factory.
    /// </summary>
    /// <param name="key">The key.</param>
    public void Remove(string key) => Register(key ?? string.Empty, null as Func<T>);

    /// <summary>
    /// Clears all the factories.
    /// </summary>
    public void Clear()
    {
        slim.EnterWriteLock();
        try
        {
            factories.Clear();
        }
        finally
        {
            slim.ExitWriteLock();
        }
    }

    /// <summary>
    /// Sets backup factory.
    /// </summary>
    /// <param name="factory">The factory.</param>
    public void SetBackup(Func<T> factory)
    {
        backup1 = null;
        backup2 = factory;
    }

    /// <summary>
    /// Sets backup factory.
    /// </summary>
    /// <param name="factory">The factory.</param>
    public void SetBackup(Func<string, T> factory)
    {
        backup2 = null;
        backup1 = factory;
    }

    /// <summary>
    /// Removes backup factory.
    /// </summary>
    public void RemoveBackup()
    {
        backup1 = null;
        backup2 = null;
    }

    /// <summary>
    /// Gets all the types registered.
    /// </summary>
    /// <returns>The type list.</returns>
    public IList<string> GetKeys()
    {
        slim.EnterReadLock();
        try
        {
            return factories.Keys.ToList();
        }
        finally
        {
            slim.ExitReadLock();
        }
    }

    /// <summary>
    /// Register a singleton instance.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="singleton">The singleton to register.</param>
    public void Singleton(string key, T singleton) => Register(key, () => singleton);

    /// <summary>
    /// Tests if there is a factory of the specific type.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public bool Has(string key) => !(GetFactory(key, true) is null);

    /// <summary>
    /// Creates an instance of the specific type using the factory registered.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The instance.</returns>
    /// <exception cref="NotSupportedException">The factory of the instance was not registered yet.</exception>
    public T Create(string key)
    {
        var h = GetFactory(key);
        if (h is null)
            throw new NotSupportedException("The factory of the instance has not been registered yet.");
        return h();
    }

    /// <summary>
    /// Gets the factory.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="disableBackup">true if disable backup; otherwise, false.</param>
    /// <returns>The factory registered; or null, if non-exist.</returns>
    public Func<T> GetFactory(string key = null, bool disableBackup = false)
    {
        slim.EnterReadLock();
        try
        {
            if (factories.TryGetValue(key ?? string.Empty, out var h) && h is Func<T> r)
                return r;
        }
        finally
        {
            slim.ExitReadLock();
        }

        if (!disableBackup) return null;
        var b = backup1;
        if (b != null) return () => b(key);
        return backup2;
    }
}
