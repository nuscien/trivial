using System;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    /// <summary>
    /// The way to pick in the concurrent tasks.
    /// </summary>
    public enum ConcurrencyFilters
    {
        /// <summary>
        /// Processes all tasks.
        /// </summary>
        All = 0,

        /// <summary>
        /// Only process the first one.
        /// </summary>
        Mono = 1,

        /// <summary>
        /// Only process the last one.
        /// </summary>
        Debounce = 2,

        /// <summary>
        /// Only process the first one after no more coming.
        /// </summary>
        Lock = 3
    }

    /// <summary>
    /// The hit event handler.
    /// </summary>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    public delegate void HitEventHandler<T>(HitTask<T> sender, HitEventArgs<T> args);

    /// <summary>
    /// The hit event handler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    public delegate void HitEventHandler(HitTask<object> sender, HitEventArgs<object> args);

    /// <summary>
    /// The arguments of hit event.
    /// </summary>
    public class HitEventArgs<T>: EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the HitEventArgs class.
        /// </summary>
        /// <param name="count">The hit count.</param>
        /// <param name="request">The current request sending date time.</param>
        /// <param name="previous">The previous request sending date time.</param>
        /// <param name="first">The first request sending date time.</param>
        /// <param name="arg">The argument.</param>
        public HitEventArgs(int count, DateTime request, DateTime? previous, DateTime first, T arg)
        {
            Count = count;
            RequestDate = request;
            PreviousRequestDate = previous;
            FirstRequestDate = first;
            Argument = arg;
        }

        /// <summary>
        /// Gets the hit count.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the current request sending date time.
        /// </summary>
        public DateTime RequestDate { get; }

        /// <summary>
        /// Gets the previous occur date time.
        /// </summary>
        public DateTime? PreviousRequestDate { get; }

        /// <summary>
        /// Gets the first request sending date time.
        /// </summary>
        public DateTime FirstRequestDate { get; }

        /// <summary>
        /// Gets the argument.
        /// </summary>
        public T Argument { get; }
    }

    /// <summary>
    /// The hit task.
    /// </summary>
    public class HitTask<T>
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The count of latest request.
        /// </summary>
        private int latestRequestCount = 0;

        /// <summary>
        /// The cache of the latest event arguments.
        /// </summary>
        private HitEventArgs<T> latestArgs = null;

        /// <summary>
        /// Gets the total count.
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Gets or sets the maximum hit count.
        /// </summary>
        public int? MaxCount { get; set; }

        /// <summary>
        /// Gets or sets the minimum hit count.
        /// </summary>
        public int MinCount { get; set; }

        /// <summary>
        /// Gets or sets the delay time span to process the task.
        /// </summary>
        public TimeSpan? Delay { get; set; }

        /// <summary>
        /// Gets or sets the timeout a set of hit.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Gets or sets the mode for multiple hitting.
        /// </summary>
        public ConcurrencyFilters Mode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether need ignore all hit request.
        /// </summary>
        public bool IsIgnoring { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pause to process.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Gets the first processing request date time at the latest duration.
        /// </summary>
        public DateTime? FirstDurationRequestDate { get; private set; }

        /// <summary>
        /// Gets the latest processing request date time.
        /// </summary>
        public DateTime? LatestRequestDate { get; private set; }

        /// <summary>
        /// Gets the latest hit processing date time.
        /// </summary>
        public DateTime? LatestProcessDate { get; private set; }

        /// <summary>
        /// Adds or removes the event to process when hit the hit condition.
        /// </summary>
        public event EventHandler<HitEventArgs<T>> Processed;

        /// <summary>
        /// Sends request to process.
        /// </summary>
        public bool Process(T arg = default)
        {
            var task = ProcessAsync(arg);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <returns>The processing task instance.</returns>
        public async Task<bool> ProcessAsync(T arg = default)
        {
            if (IsIgnoring) return false;
            var now = DateTime.Now;
            var mode = Mode;
            HitEventArgs<T> args;
            lock (locker)
            {
                var latestReqDate = LatestRequestDate;
                var timeout = Timeout;
                var firstReq = FirstDurationRequestDate ?? latestReqDate ?? now;

                if (Duration.HasValue && Duration.Value < now - firstReq)
                {
                    latestRequestCount = 0;
                    FirstDurationRequestDate = now;
                }
                else if (!timeout.HasValue || !latestReqDate.HasValue || timeout.Value < now - latestReqDate.Value)
                {
                    latestRequestCount = 0;
                    FirstDurationRequestDate = now;
                }

                LatestRequestDate = now;
                var count = ++latestRequestCount;
                var minCount = MinCount;
                if (minCount > count)
                {
                    latestArgs = null;
                    return false;
                }

                var maxCount = MaxCount;
                if (maxCount.HasValue && maxCount.Value < count)
                {
                    latestArgs = null;
                    return false;
                }

                if (count > 1 && mode == ConcurrencyFilters.Mono) return false;
                var lArgs = latestArgs;
                if (mode == ConcurrencyFilters.Lock && lArgs != null && count > 1)
                {
                    args = new HitEventArgs<T>(lArgs.Count, lArgs.RequestDate, lArgs.PreviousRequestDate, lArgs.FirstRequestDate, lArgs.Argument);
                }
                else
                {
                    args = new HitEventArgs<T>(count, now, latestReqDate, firstReq, arg);
                }

                latestArgs = args;
            }

            if (Delay.HasValue) await Task.Delay(Delay.Value);
            if (mode != ConcurrencyFilters.All)
            {
                if (latestArgs != args) return false;
            }

            if (IsPaused) return false;
            LatestProcessDate = DateTime.Now;
            TotalCount++;
            Processed?.Invoke(this, args);
            return true;
        }
    }

    /// <summary>
    /// The hit task.
    /// </summary>
    public class HitTask : HitTask<object>
    {
        /// <summary>
        /// Creates a debounce task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Debounce(Action action, TimeSpan delay)
        {
            var task = new HitTask
            {
                Mode = ConcurrencyFilters.Debounce,
                Delay = delay
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action();
            };
            return task;
        }

        /// <summary>
        /// Creates a debounce task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Debounce(HitEventHandler action, TimeSpan delay)
        {
            var task = new HitTask
            {
                Mode = ConcurrencyFilters.Debounce,
                Delay = delay
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(task, ev);
            };
            return task;
        }

        /// <summary>
        /// Creates a debounce task.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Debounce<T>(Action<T> action, TimeSpan delay)
        {
            var task = new HitTask<T>
            {
                Mode = ConcurrencyFilters.Debounce,
                Delay = delay
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(ev.Argument);
            };
            return task;
        }

        /// <summary>
        /// Creates a debounce task.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Debounce<T>(HitEventHandler<T> action, TimeSpan delay)
        {
            var task = new HitTask<T>
            {
                Mode = ConcurrencyFilters.Debounce,
                Delay = delay
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(task, ev);
            };
            return task;
        }

        /// <summary>
        /// Creates a throttle task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Throttle(Action action, TimeSpan duration)
        {
            var task = new HitTask
            {
                Mode = ConcurrencyFilters.Mono,
                Timeout = duration,
                Duration = duration,
                MaxCount = 1
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action();
            };
            return task;
        }

        /// <summary>
        /// Creates a throttle task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Throttle(HitEventHandler action, TimeSpan duration)
        {
            var task = new HitTask
            {
                Mode = ConcurrencyFilters.Mono,
                Timeout = duration,
                Duration = duration,
                MaxCount = 1
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(task, ev);
            };
            return task;
        }

        /// <summary>
        /// Creates a throttle task.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Throttle<T>(Action<T> action, TimeSpan duration)
        {
            var task = new HitTask<T>
            {
                Mode = ConcurrencyFilters.Mono,
                Timeout = duration,
                Duration = duration,
                MaxCount = 1
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(ev.Argument);
            };
            return task;
        }

        /// <summary>
        /// Creates a throttle task.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Throttle<T>(HitEventHandler<T> action, TimeSpan duration)
        {
            var task = new HitTask<T>
            {
                Mode = ConcurrencyFilters.Mono,
                Timeout = duration,
                Duration = duration,
                MaxCount = 1
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(task, ev);
            };
            return task;
        }

        /// <summary>
        /// Creates a multi-hit task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Mutliple(Action action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask
            {
                Timeout = timeout,
                MinCount = min,
                MaxCount = max
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action();
            };
            return task;
        }

        /// <summary>
        /// Creates a multi-hit task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Mutliple(HitEventHandler action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask
            {
                Timeout = timeout,
                MinCount = min,
                MaxCount = max
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(task, ev);
            };
            return task;
        }

        /// <summary>
        /// Creates a multi-hit task.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Mutliple<T>(Action<T> action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask<T>
            {
                Timeout = timeout,
                MinCount = min,
                MaxCount = max
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(ev.Argument);
            };
            return task;
        }

        /// <summary>
        /// Creates a multi-hit task.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Mutliple<T>(HitEventHandler<T> action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask<T>
            {
                Timeout = timeout,
                MinCount = min,
                MaxCount = max
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(task, ev);
            };
            return task;
        }


        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Times(Action action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = min,
                MaxCount = max,
                Mode = ConcurrencyFilters.Debounce
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action();
            };
            return task;
        }

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Times(HitEventHandler action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = min,
                MaxCount = max,
                Mode = ConcurrencyFilters.Debounce
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(task, ev);
            };
            return task;
        }

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Times<T>(Action<T> action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask<T>
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = min,
                MaxCount = max,
                Mode = ConcurrencyFilters.Debounce
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(ev.Argument);
            };
            return task;
        }

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Times<T>(HitEventHandler<T> action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask<T>
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = min,
                MaxCount = max,
                Mode = ConcurrencyFilters.Debounce
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(task, ev);
            };
            return task;
        }
    }
}
