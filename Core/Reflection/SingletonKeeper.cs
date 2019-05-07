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
        /// </summary>
        /// <returns>The instance.</returns>
        public Task<T> GetAsync()
        {
            return GetAsync(false);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public Task<T> RenewAsync()
        {
            return GetAsync(true);
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

        private async Task<T> GetAsync(bool forceUpdate)
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
            await semaphoreSlim.WaitAsync();
            try
            {
                if (!forceUpdate && HasCache) return Cache;
                var renewTask = ResolveFromSourceAsync();
                renewTask.Wait();
                Cache = renewTask.Result;
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
    /// Thread-safe singleton refresh scheduler.
    /// </summary>
    /// <typeparam name="T">The type of singleton</typeparam>
    public class SingletonRefreshScheduler<T>
    {
        private readonly Lazy<Timer> timer;

        /// <summary>
        /// Initializes a new instance of the SingletonRefreshScheduler class.
        /// </summary>
        /// <param name="keeper">The singleton keeper instance to maintain.</param>
        /// <param name="timerFactory">The timer factory.</param>
        public SingletonRefreshScheduler(SingletonKeeper<T> keeper, Func<Timer> timerFactory)
        {
            Keeper = keeper;
            timer = new Lazy<Timer>(timerFactory, true);
        }

        /// <summary>
        /// Gets the singleton keeper source instance.
        /// </summary>
        public SingletonKeeper<T> Keeper { get; }

        /// <summary>
        /// Gets the refresh timer instance.
        /// </summary>
        public Timer Timer => timer.Value;

        /// <summary>
        /// Gets the latest refresh completed date.
        /// </summary>
        public DateTime? RefreshDate => Keeper?.RefreshDate;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public Task<T> GetAsync()
        {
            InitTimer();
            return Keeper != null ? Keeper.GetAsync() : Task.Run(() => default(T));
        }

        /// <summary>
        /// Refreshes and gets the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        public Task<T> RenewAsync()
        {
            InitTimer();
            return Keeper != null ? Keeper.RenewAsync() : Task.Run(() => default(T));
        }

        private void InitTimer()
        {
#pragma warning disable IDE0059
            var t = Timer;
#pragma warning restore IDE0059
        }
    }

    /// <summary>
    /// Thread-safe singleton refresh scheduler.
    /// </summary>
    /// <typeparam name="TKeeper">The type of keeper</typeparam>
    /// <typeparam name="TModel">The type of singleton</typeparam>
    public class SingletonRefreshScheduler<TKeeper, TModel> : SingletonRefreshScheduler<TModel>
        where TKeeper : SingletonKeeper<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the SingletonRefreshScheduler class.
        /// </summary>
        /// <param name="keeper">The singleton keeper instance to maintain.</param>
        /// <param name="timerFactory">The timer factory.</param>
        public SingletonRefreshScheduler(TKeeper keeper, Func<Timer> timerFactory) : base(keeper, timerFactory)
        {
            Keeper = keeper;
        }

        /// <summary>
        /// Gets the singleton keeper source instance.
        /// </summary>
        public new TKeeper Keeper { get; }
    }
}
