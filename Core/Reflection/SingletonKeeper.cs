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
    /// Singleton keeper.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    public class SingletonKeeper<T>
    {
        private readonly object locker = new object();
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
        public SingletonKeeper(T cache)
        {
            HasCache = true;
            Cache = cache;
        }

        /// <summary>
        /// Initializes a new instance of the SingletonKeeper class.
        /// </summary>
        /// <param name="resolveHandler">The resovle handler.</param>
        /// <param name="cache">The cache.</param>
        public SingletonKeeper(Func<Task<T>> resolveHandler, T cache) : this(cache)
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
        /// Gets the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public T Get()
        {
            return Get(false);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public Task<T> GetAsync()
        {
            return Task.Run(() =>
            {
                return Get(false);
            });
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public Task<T> RenewAsync()
        {
            return Task.Run(() =>
            {
                return Get(true);
            });
        }

        /// <summary>
        /// Clears the cache.
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
        /// <returns>The timer.</returns>
        public Timer CreateRenewTimer(TimeSpan dueTime, TimeSpan period)
        {
            return new Timer(state =>
            {
                RenewAsync();
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
        /// Validates if the cache is valid.
        /// </summary>
        /// <returns>true if valid; otherwise, false.</returns>
        protected virtual Task<bool> NeedRenewAsync()
        {
            return Task.Run(() => !HasCache);
        }

        private T Get(bool forceUpdate)
        {
            if (!forceUpdate && HasCache)
            {
                try
                {
                    var needTask = NeedRenewAsync();
                    needTask.Wait();
                    HasCache = needTask.IsCompleted && needTask.Result == false;
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
            }

            var cache = Cache;
            lock (locker)
            {
                if (!forceUpdate && HasCache) return Cache;
                var renewTask = ResolveFromSourceAsync();
                renewTask.Wait();
                Cache = renewTask.Result;
                HasCache = true;
            }

            Renewed?.Invoke(this, new ChangeEventArgs<T>(cache, Cache, nameof(Cache), true));
            return Cache;
        }
    }
}
