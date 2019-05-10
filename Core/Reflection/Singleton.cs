using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Data;

namespace Trivial.Reflection
{
    /// <summary>
    /// Resolves an instance by key.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="value">The value output.</param>
    /// <returns>true if resolve succeeded; otherwise, false.</returns>
    public delegate bool KeyedInstanceResolver<T>(string key, out T value);

    /// <summary>
    /// Singleton resolver interface.
    /// </summary>
    public interface ISingletonResolver
    {
        /// <summary>
        /// Tries to resolves a singleton instance.
        /// </summary>
        /// <typeparam name="T">The type of instance.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>An instance resolved.</returns>
        /// <exception cref="NotSupportedException">The type of instance was not support to resolve.</exception>
        /// <exception cref="KeyNotFoundException">The key was not supported for this type.</exception>
        T Resolve<T>(string key);

        /// <summary>
        /// Tries to resolves a singleton instance.
        /// </summary>
        /// <typeparam name="T">The type of instance.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="result">An instance resolved.</param>
        /// <returns>true if resolve succeeded; otherwise, false.</returns>
        bool TryResolve<T>(string key, out T result);
    }

    /// <summary>
    /// The base singleton resolver.
    /// </summary>
    public abstract class BaseSingletonResolver : ISingletonResolver
    {
        /// <summary>
        /// Cache.
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, object>> cache = new Dictionary<Type, Dictionary<string, object>>();

        /// <summary>
        /// Resolves a singleton instance.
        /// </summary>
        /// <typeparam name="T">The type of instance.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>An instance resolved.</returns>
        /// <exception cref="NotSupportedException">The type of instance was not support to resolve.</exception>
        /// <exception cref="KeyNotFoundException">The key was not supported for this type.</exception>
        public T Resolve<T>(string key)
        {
            if (key == null) key = string.Empty;
            KeyedInstanceResolver<T> resolver = null;
            Exception exception = null;
            try
            {
                resolver = GetResolver<T>();
            }
            catch (ArgumentException ex)
            {
                exception = ex;
            }
            catch (NullReferenceException ex)
            {
                exception = ex;
            }
            catch (KeyNotFoundException ex)
            {
                exception = ex;
            }
            catch (InvalidCastException ex)
            {
                exception = ex;
            }
            catch (NotSupportedException ex)
            {
                exception = ex;
            }
            catch (InvalidOperationException ex)
            {
                exception = ex;
            }

            if (exception != null) throw new NotSupportedException("The type of the instance has not been registered yet.", exception);
            if (resolver == null) throw new NotSupportedException("The type of the instance has not been registered yet.");

            try
            {
                if (resolver(key, out T result)) return result;
                if (string.IsNullOrEmpty(key)) throw new NotSupportedException("There is no default instance of the type registered.");
                throw new KeyNotFoundException("The key is not supported in the given type registered.");
            }
            catch (ArgumentException ex)
            {
                exception = ex;
            }
            catch (NullReferenceException ex)
            {
                exception = ex;
            }
            catch (KeyNotFoundException ex)
            {
                exception = ex;
            }
            catch (InvalidCastException ex)
            {
                exception = ex;
            }
            catch (NotSupportedException ex)
            {
                exception = ex;
            }
            catch (InvalidOperationException ex)
            {
                exception = ex;
            }

            if (string.IsNullOrEmpty(key)) throw new NotSupportedException("There is no default instance of the type registered.", exception);
            throw new KeyNotFoundException("The key is not supported in the given type registered.", exception);
        }

        /// <summary>
        /// Resolves a singleton instance.
        /// </summary>
        /// <typeparam name="T">The type of instance.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="result">An instance resolved.</param>
        /// <returns>true if resolve succeeded; otherwise, false.</returns>
        /// <exception cref="NotSupportedException">The type of instance was not support to resolve.</exception>
        /// <exception cref="KeyNotFoundException">The key was not supported for this type.</exception>
        public bool TryResolve<T>(string key, out T result)
        {
            if (key == null) key = string.Empty;
            try
            {
                var resolver = GetResolver<T>();
                if (resolver != null) return resolver(key, out result);
                result = default;
                return false;
            }
            catch (ArgumentException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (KeyNotFoundException)
            {
            }
            catch (InvalidCastException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Gets the keyed instance resolver of the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <returns>The resolver; or null, if so such type supported.</returns>
        protected abstract KeyedInstanceResolver<T> GetResolver<T>();
    }

    /// <summary>
    /// In-memory singleton resolver.
    /// </summary>
    public class SingletonResolver : BaseSingletonResolver
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// Cache.
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, object>> cache = new Dictionary<Type, Dictionary<string, object>>();

        /// <summary>
        /// Registers an instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance to register.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="obj">The instance.</param>
        public void Register<T>(string key, T obj)
        {
            if (!cache.ContainsKey(typeof(T)))
            {
                lock (locker)
                {
                    if (!cache.ContainsKey(typeof(T)))
                    {
                        cache[typeof(T)] = new Dictionary<string, object>();
                    }
                }
            }

            var set = cache[typeof(T)];
            set[key ?? string.Empty] = obj;
        }

        /// <summary>
        /// Registers an instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance to register.</typeparam>
        /// <param name="obj">The instance.</param>
        public void Register<T>(T obj)
        {
            Register(null, obj);
        }

        /// <summary>
        /// Registers an instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance to register.</typeparam>
        /// <param name="key">The key.</param>
        public void Register<T>(string key = null)
        {
            Register(key, Activator.CreateInstance<T>());
        }

        /// <summary>
        /// Gets the keyed instance resolver of the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <returns>The resolver; or null, if so such type supported.</returns>
        protected override KeyedInstanceResolver<T> GetResolver<T>()
        {
            if (!cache.TryGetValue(typeof(T), out var set)) return null;
            return (string key, out T result) =>
            {
                if (key == null) key = string.Empty;
                if (!set.TryGetValue(key, out var value))
                {
                    result = default;
                    return false;
                }

                try
                {
                    result = (T)value;
                    return true;
                }
                catch (InvalidCastException)
                {
                    result = default;
                    return false;
                }
            };
        }
    }

    /// <summary>
    /// Singleton keeper with optional renew ability in thread-safe mode.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    public class SingletonKeeper<T>
    {
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly Func<Task<T>> renew;

        /// <summary>
        /// Initializes a new instance of the SingletonKeeper class.
        /// </summary>
        protected SingletonKeeper()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SingletonKeeper class.
        /// </summary>
        /// <param name="resolveHandler">The resovle handler.</param>
        public SingletonKeeper(Func<Task<T>> resolveHandler)
        {
            renew = resolveHandler;
        }

        /// <summary>
        /// Initializes a new instance of the SingletonKeeper class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="refreshDate">The latest refresh succeeded date time of cache.</param>
        public SingletonKeeper(T cache, DateTime? refreshDate = null)
        {
            HasCache = true;
            Cache = cache;
            RefreshDate = refreshDate;
        }

        /// <summary>
        /// Initializes a new instance of the SingletonKeeper class.
        /// </summary>
        /// <param name="resolveHandler">The resovle handler.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="refreshDate">The latest refresh succeeded date time of cache.</param>
        public SingletonKeeper(Func<Task<T>> resolveHandler, T cache, DateTime? refreshDate = null) : this(cache, refreshDate)
        {
            renew = resolveHandler;
        }

        /// <summary>
        /// Adds or removes after the cache is updated.
        /// </summary>
        public event ChangeEventHandler<T> Renewed;

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public T Cache { get; private set; }

        /// <summary>
        /// Gets a value indicating whether has cache.
        /// </summary>
        public bool HasCache { get; private set; }

        /// <summary>
        /// Gets the latest refresh completed date.
        /// </summary>
        public DateTime? RefreshDate { get; private set; }

        /// <summary>
        /// Gets the instance.
        /// It will load from cache if it does not expired; otherwise, renew one, and then return.
        /// </summary>
        /// <returns>The instance.</returns>
        public Task<T> GetAsync()
        {
            return GetAsync(false);
        }

        /// <summary>
        /// Renews and gets the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public Task<T> RenewAsync()
        {
            return GetAsync(true);
        }

        /// <summary>
        /// Sets the cache flag as false.
        /// </summary>
        public void ClearCache()
        {
            HasCache = false;
        }

        /// <summary>
        /// Creates a timer to renew the singleton.
        /// </summary>
        /// <param name="dueTime">
        /// The amount of time to delay before the callback parameter invokes its methods.
        /// Specify negative one (-1) milliseconds to prevent the timer from starting.
        /// Specify zero (0) to start the timer immediately.
        /// </param>
        /// <param name="period">
        /// The time interval between invocations of the methods referenced by callback.
        /// Specify negative one (-1) milliseconds to disable periodic signaling.
        /// </param>
        /// <param name="isPaused">A handler to let the timer know if the renew action is paused.</param>
        /// <returns>The timer.</returns>
        public Timer CreateRenewTimer(TimeSpan dueTime, TimeSpan period, Func<bool> isPaused = null)
        {
            if (isPaused == null) isPaused = () => false;
            return new Timer(state =>
            {
                if (!isPaused()) RenewAsync();
            }, null, dueTime, period);
        }

        /// <summary>
        /// Forces to resolve a new instance.
        /// </summary>
        /// <returns>The instance.</returns>
        protected virtual async Task<T> ResolveFromSourceAsync()
        {
            if (renew == null) return Cache;
            return await renew();
        }

        /// <summary>
        /// Tests if the cache is valid.
        /// </summary>
        /// <returns>true if valid; otherwise, false.</returns>
        protected virtual Task<bool> NeedRenewAsync()
        {
            return Task.FromResult(!HasCache);
        }

        private async Task<T> GetAsync(bool forceUpdate)
        {
            var hasThread = semaphoreSlim.CurrentCount == 0;
            if (!hasThread && !forceUpdate && HasCache)
            {
                try
                {
                    HasCache = !await NeedRenewAsync();
                    if (HasCache) return Cache;
                }
                catch (AggregateException)
                {
                }
                catch (NullReferenceException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (OperationCanceledException)
                {
                }
                catch (NotSupportedException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }

                hasThread = semaphoreSlim.CurrentCount == 0;
            }

            var cache = Cache;
            await semaphoreSlim.WaitAsync();
            try
            {
                if ((!forceUpdate || hasThread) && HasCache) return Cache;
                Cache = await ResolveFromSourceAsync();
                RefreshDate = DateTime.Now;
                HasCache = true;
            }
            finally
            {
                semaphoreSlim.Release();
            }

            Renewed?.Invoke(this, new ChangeEventArgs<T>(cache, Cache, nameof(Cache), true));
            return Cache;
        }
    }

    /// <summary>
    /// Thread-safe singleton renew scheduler.
    /// </summary>
    /// <typeparam name="T">The type of singleton</typeparam>
    public class SingletonRenewScheduler<T> : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the SingletonRenewScheduler class.
        /// </summary>
        /// <param name="keeper">The singleton keeper instance to maintain.</param>
        /// <param name="dueTime">
        /// The amount of time to delay before the callback parameter invokes its methods.
        /// Specify negative one (-1) milliseconds to prevent the timer from starting.
        /// Specify zero (0) to start the timer immediately.
        /// </param>
        /// <param name="period">
        /// The time interval between invocations of the methods referenced by callback.
        /// Specify negative one (-1) milliseconds to disable periodic signaling.
        /// </param>
        public SingletonRenewScheduler(SingletonKeeper<T> keeper, TimeSpan dueTime, TimeSpan period)
        {
            Keeper = keeper ?? new SingletonKeeper<T>(default(T));
            Timer = Keeper.CreateRenewTimer(dueTime, period, () => IsPaused);
        }

        /// <summary>
        /// Initializes a new instance of the SingletonRenewScheduler class.
        /// </summary>
        /// <param name="resolveHandler">The resovle handler.</param>
        /// <param name="dueTime">
        /// The amount of time to delay before the callback parameter invokes its methods.
        /// Specify negative one (-1) milliseconds to prevent the timer from starting.
        /// Specify zero (0) to start the timer immediately.
        /// </param>
        /// <param name="period">
        /// The time interval between invocations of the methods referenced by callback.
        /// Specify negative one (-1) milliseconds to disable periodic signaling.
        /// </param>
        public SingletonRenewScheduler(Func<Task<T>> resolveHandler, TimeSpan dueTime, TimeSpan period)
            : this(new SingletonKeeper<T>(resolveHandler), dueTime, period)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SingletonRenewScheduler class.
        /// </summary>
        /// <param name="resolveHandler">The resovle handler.</param>
        /// <param name="dueTime">
        /// The amount of time to delay before the callback parameter invokes its methods.
        /// Specify negative one (-1) milliseconds to prevent the timer from starting.
        /// Specify zero (0) to start the timer immediately.
        /// </param>
        /// <param name="period">
        /// The time interval between invocations of the methods referenced by callback.
        /// Specify negative one (-1) milliseconds to disable periodic signaling.
        /// </param>
        /// <param name="cache">The cache.</param>
        /// <param name="refreshDate">The latest refresh succeeded date time of cache.</param>
        public SingletonRenewScheduler(Func<Task<T>> resolveHandler, TimeSpan dueTime, TimeSpan period, T cache, DateTime? refreshDate = null)
            : this(new SingletonKeeper<T>(resolveHandler, cache, refreshDate), dueTime, period)
        {
        }

        /// <summary>
        /// Adds or removes after the cache is updated.
        /// </summary>
        public event ChangeEventHandler<T> Renewed
        {
            add => Keeper.Renewed += value;
            remove => Keeper.Renewed -= value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the timer should pause to renew.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Gets the singleton keeper source instance.
        /// </summary>
        public SingletonKeeper<T> Keeper { get; }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public T Cache => Keeper.Cache;

        /// <summary>
        /// Gets a value indicating whether has cache.
        /// </summary>
        public bool HasCache => Keeper.HasCache;

        /// <summary>
        /// Gets the latest refresh completed date.
        /// </summary>
        public DateTime? RefreshDate => Keeper.RefreshDate;

        /// <summary>
        /// Gets the refresh timer instance.
        /// </summary>
        public Timer Timer { get; }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public Task<T> GetAsync() => Keeper.GetAsync();

        /// <summary>
        /// Refreshes and gets the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public Task<T> RenewAsync() => Keeper.RenewAsync();

        /// <summary>
        /// Sets the cache flag as false.
        /// </summary>
        public void ClearCache() => Keeper.ClearCache();

        /// <summary>
        /// Releases all resources used by the current secret exchange object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            Timer.Dispose();
        }
    }

    /// <summary>
    /// Thread-safe singleton renew scheduler.
    /// </summary>
    /// <typeparam name="TKeeper">The type of keeper</typeparam>
    /// <typeparam name="TModel">The type of singleton</typeparam>
    public class SingletonRenewScheduler<TKeeper, TModel> : SingletonRenewScheduler<TModel>
        where TKeeper : SingletonKeeper<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the SingletonRenewScheduler class.
        /// </summary>
        /// <param name="keeper">The singleton keeper instance to maintain.</param>
        /// <param name="dueTime">
        /// The amount of time to delay before the callback parameter invokes its methods.
        /// Specify negative one (-1) milliseconds to prevent the timer from starting.
        /// Specify zero (0) to start the timer immediately.
        /// </param>
        /// <param name="period">
        /// The time interval between invocations of the methods referenced by callback.
        /// Specify negative one (-1) milliseconds to disable periodic signaling.
        /// </param>
        public SingletonRenewScheduler(TKeeper keeper, TimeSpan dueTime, TimeSpan period) : base(keeper, dueTime, period)
        {
            Keeper = keeper;
        }

        /// <summary>
        /// Gets the singleton keeper source instance.
        /// </summary>
        public new TKeeper Keeper { get; }
    }
}
