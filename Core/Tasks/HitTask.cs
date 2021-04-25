using System;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    /// <summary>
    /// The way to pick in the concurrent tasks, e.g. the first one, the last one or for all.
    /// </summary>
    public enum ConcurrencyFilters
    {
        /// <summary>
        /// Invoke all tasks without any intercept.
        /// </summary>
        All = 0,

        /// <summary>
        /// Only invoke for the first one which meet the condition.
        /// </summary>
        Mono = 1,

        /// <summary>
        /// Only invoke for the last one which meet the condition.
        /// </summary>
        Debounce = 2,

        /// <summary>
        /// Pend the first one to invoke at the end of the duration (when no more coming).
        /// </summary>
        Lock = 3
    }

    /// <summary>
    /// The hit event handler.
    /// </summary>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    public delegate void HitEventHandler<T>(BaseHitTask<T> sender, HitEventArgs<T> args);

    /// <summary>
    /// The hit event handler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    public delegate void HitEventHandler(BaseHitTask<object> sender, HitEventArgs<object> args);

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
    /// The hit task to determine whether the current invoking action can run right now, later or never..
    /// </summary>
    public abstract class BaseHitTask<T>
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private readonly object locker = new ();

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
        /// Gets or sets the counting duration. It is used to reset the processing count to zero after a specific time span.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Gets or sets the mode for multiple hitting.
        /// It determine which one invokes during the counting limitation, e.g. the first one, the last one or all.
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
        /// Gets the first processing request date time at the latest counting duration.
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
        /// Adds or removes the event to process if hit the condition.
        /// </summary>
        public event EventHandler<HitEventArgs<T>> Processed;

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg">The optional argument.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public bool Process(T arg = default)
        {
            var task = ProcessAsync(arg);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg">The optional argument.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
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

        /// <summary>
        /// Creates an event handler to register.
        /// </summary>
        /// <param name="arg">The optional argument used on this task is processing.</param>
        /// <returns>An event handler.</returns>
        public EventHandler CreateEventHandler(T arg = default)
        {
            return (object sender, EventArgs ev) =>
            {
                var task = ProcessAsync(arg);
            };
        }

        /// <summary>
        /// Creates an event handler to register.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event argument.</typeparam>
        /// <param name="arg">The optional argument used on this task is processing.</param>
        /// <returns>An event handler.</returns>
        public EventHandler<TEventArgs> CreateEventHandler<TEventArgs>(T arg = default) where TEventArgs : EventArgs
        {
            return (object sender, TEventArgs ev) =>
            {
                var task = ProcessAsync(arg);
            };
        }
    }

    /// <summary>
    /// The arguments of hit event.
    /// </summary>
    /// <typeparam name="T">The type of argument.</typeparam>
    public class HitTask<T> : BaseHitTask<T>
    {
        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public void ProcessBegin(T arg) => _ = ProcessAsync(arg);

        /// <summary>
        /// Creates a debounce task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// Maybe a handler will be asked to process several times in a short time
        /// but you just want to process once at the last time because the previous ones are obsolete.
        /// A sample scenario is real-time search.
        /// </remarks>
        public static HitTask<T> Debounce(Action<T> action, TimeSpan delay) => HitTask.Debounce(action, delay);

        /// <summary>
        /// Creates a debounce task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// Maybe a handler will be asked to process several times in a short time
        /// but you just want to process once at the last time because the previous ones are obsolete.
        /// A sample scenario is real-time search.
        /// </remarks>
        public static HitTask<T> Debounce(HitEventHandler<T> action, TimeSpan delay) => HitTask.Debounce(action, delay);

        /// <summary>
        /// Creates a throttle task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// You may want to request to call an action only once in a short time
        /// even if you request to call several times.
        /// The rest will be ignored.
        /// So the handler will be frozen for a while after it has processed.
        /// </remarks>
        public static HitTask<T> Throttle(Action<T> action, TimeSpan duration) => HitTask.Throttle(action, duration);

        /// <summary>
        /// Creates a throttle task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// You may want to request to call an action only once in a short time
        /// even if you request to call several times.
        /// The rest will be ignored.
        /// So the handler will be frozen for a while after it has processed.
        /// </remarks>
        public static HitTask<T> Throttle(HitEventHandler<T> action, TimeSpan duration) => HitTask.Throttle(action, duration);

        /// <summary>
        /// Creates a multi-hit task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remark>
        /// The handler to process for the specific times and it will be reset after a while.
        /// </remark>
        public static HitTask<T> Mutliple(Action<T> action, int min, int? max, TimeSpan timeout) => HitTask.Mutliple(action, min, max, timeout);

        /// <summary>
        /// Creates a multi-hit task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remark>
        /// The handler to process for the specific times and it will be reset after a while.
        /// </remark>
        public static HitTask<T> Mutliple(HitEventHandler<T> action, int min, int? max, TimeSpan timeout) => HitTask.Mutliple(action, min, max, timeout);

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T> Times(Action<T> action, int min, int? max, TimeSpan timeout) => HitTask.Times(action, min, max, timeout);

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T> Times(HitEventHandler<T> action, int min, int? max, TimeSpan timeout) => HitTask.Times(action, min, max, timeout);

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="count">The hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T> Times(Action<T> action, int count, TimeSpan timeout) => HitTask.Times(action, count, timeout);

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="count">The hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T> Times(HitEventHandler<T> action, int count, TimeSpan timeout) => HitTask.Times(action, count, timeout);
    }

    /// <summary>
    /// The hit task.
    /// </summary>
    public class HitTask : BaseHitTask<object>
    {
        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public void ProcessBegin() => _ = ProcessAsync();

        /// <summary>
        /// Creates a debounce task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// Maybe a handler will be asked to process several times in a short time
        /// but you just want to process once at the last time because the previous ones are obsolete.
        /// A sample scenario is real-time search.
        /// </remarks>
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
        /// <remarks>
        /// Maybe a handler will be asked to process several times in a short time
        /// but you just want to process once at the last time because the previous ones are obsolete.
        /// A sample scenario is real-time search.
        /// </remarks>
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
        /// <remarks>
        /// Maybe a handler will be asked to process several times in a short time
        /// but you just want to process once at the last time because the previous ones are obsolete.
        /// A sample scenario is real-time search.
        /// </remarks>
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
        /// <remarks>
        /// Maybe a handler will be asked to process several times in a short time
        /// but you just want to process once at the last time because the previous ones are obsolete.
        /// A sample scenario is real-time search.
        /// </remarks>
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
        /// Creates a debounce task.
        /// </summary>
        /// <typeparam name="T1">The type of argument 1.</typeparam>
        /// <typeparam name="T2">The type of argument 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// Maybe a handler will be asked to process several times in a short time
        /// but you just want to process once at the last time because the previous ones are obsolete.
        /// A sample scenario is real-time search.
        /// </remarks>
        public static HitTask<T1, T2> Debounce<T1, T2>(Action<T1, T2> action, TimeSpan delay)
        {
            var task = new HitTask<T1, T2>
            {
                Mode = ConcurrencyFilters.Debounce,
                Delay = delay
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(ev.Argument.Item1, ev.Argument.Item2);
            };
            return task;
        }

        /// <summary>
        /// Creates a debounce task.
        /// </summary>
        /// <typeparam name="T1">The type of argument 1.</typeparam>
        /// <typeparam name="T2">The type of argument 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// Maybe a handler will be asked to process several times in a short time
        /// but you just want to process once at the last time because the previous ones are obsolete.
        /// A sample scenario is real-time search.
        /// </remarks>
        public static HitTask<T1, T2> Debounce<T1, T2>(HitEventHandler<Tuple<T1, T2>> action, TimeSpan delay)
        {
            var task = new HitTask<T1, T2>
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
        /// <remarks>
        /// You may want to request to call an action only once in a short time
        /// even if you request to call several times.
        /// The rest will be ignored.
        /// So the handler will be frozen for a while after it has processed.
        /// </remarks>
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
        /// <remarks>
        /// You may want to request to call an action only once in a short time
        /// even if you request to call several times.
        /// The rest will be ignored.
        /// So the handler will be frozen for a while after it has processed.
        /// </remarks>
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
        /// <remarks>
        /// You may want to request to call an action only once in a short time
        /// even if you request to call several times.
        /// The rest will be ignored.
        /// So the handler will be frozen for a while after it has processed.
        /// </remarks>
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
        /// <remarks>
        /// You may want to request to call an action only once in a short time
        /// even if you request to call several times.
        /// The rest will be ignored.
        /// So the handler will be frozen for a while after it has processed.
        /// </remarks>
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
        /// Creates a throttle task.
        /// </summary>
        /// <typeparam name="T1">The type of argument 1.</typeparam>
        /// <typeparam name="T2">The type of argument 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// You may want to request to call an action only once in a short time
        /// even if you request to call several times.
        /// The rest will be ignored.
        /// So the handler will be frozen for a while after it has processed.
        /// </remarks>
        public static HitTask<T1, T2> Throttle<T1, T2>(Action<T1, T2> action, TimeSpan duration)
        {
            var task = new HitTask<T1, T2>
            {
                Mode = ConcurrencyFilters.Mono,
                Timeout = duration,
                Duration = duration,
                MaxCount = 1
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(ev.Argument.Item1, ev.Argument.Item2);
            };
            return task;
        }

        /// <summary>
        /// Creates a throttle task.
        /// </summary>
        /// <typeparam name="T1">The type of argument 1.</typeparam>
        /// <typeparam name="T2">The type of argument 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// You may want to request to call an action only once in a short time
        /// even if you request to call several times.
        /// The rest will be ignored.
        /// So the handler will be frozen for a while after it has processed.
        /// </remarks>
        public static HitTask<T1, T2> Throttle<T1, T2>(HitEventHandler<Tuple<T1, T2>> action, TimeSpan duration)
        {
            var task = new HitTask<T1, T2>
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
        /// <remark>
        /// The handler to process for the specific times and it will be reset after a while.
        /// </remark>
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
        /// <remark>
        /// The handler to process for the specific times and it will be reset after a while.
        /// </remark>
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
        /// <remark>
        /// The handler to process for the specific times and it will be reset after a while.
        /// </remark>
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
        /// <remark>
        /// The handler to process for the specific times and it will be reset after a while.
        /// </remark>
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
        /// Creates a multi-hit task.
        /// </summary>
        /// <typeparam name="T1">The type of argument 1.</typeparam>
        /// <typeparam name="T2">The type of argument 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remark>
        /// The handler to process for the specific times and it will be reset after a while.
        /// </remark>
        public static HitTask<T1, T2> Mutliple<T1, T2>(Action<T1, T2> action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask<T1, T2>
            {
                Timeout = timeout,
                MinCount = min,
                MaxCount = max
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(ev.Argument.Item1, ev.Argument.Item2);
            };
            return task;
        }

        /// <summary>
        /// Creates a multi-hit task.
        /// </summary>
        /// <typeparam name="T1">The type of argument 1.</typeparam>
        /// <typeparam name="T2">The type of argument 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remark>
        /// The handler to process for the specific times and it will be reset after a while.
        /// </remark>
        public static HitTask<T1, T2> Mutliple<T1, T2>(HitEventHandler<Tuple<T1, T2>> action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask<T1, T2>
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
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
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
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
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
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
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
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
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

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <typeparam name="T1">The type of argument 1.</typeparam>
        /// <typeparam name="T2">The type of argument 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T1, T2> Times<T1, T2>(Action<T1, T2> action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask<T1, T2>
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = min,
                MaxCount = max,
                Mode = ConcurrencyFilters.Debounce
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(ev.Argument.Item1, ev.Argument.Item2);
            };
            return task;
        }

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <typeparam name="T1">The type of argument 1.</typeparam>
        /// <typeparam name="T2">The type of argument 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T1, T2> Times<T1, T2>(HitEventHandler<Tuple<T1, T2>> action, int min, int? max, TimeSpan timeout)
        {
            var task = new HitTask<T1, T2>
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
        /// <param name="action">The action.</param>
        /// <param name="count">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask Times(Action action, int count, TimeSpan timeout)
        {
            var task = new HitTask
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = count,
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
        /// <param name="count">The hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask Times(HitEventHandler action, int count, TimeSpan timeout)
        {
            var task = new HitTask
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = count,
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
        /// <param name="count">The hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T> Times<T>(Action<T> action, int count, TimeSpan timeout)
        {
            var task = new HitTask<T>
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = count,
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
        /// <param name="count">The hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T> Times<T>(HitEventHandler<T> action, int count, TimeSpan timeout)
        {
            var task = new HitTask<T>
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = count,
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
        /// <typeparam name="T1">The type of argument 1.</typeparam>
        /// <typeparam name="T2">The type of argument 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="count">The hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T1, T2> Times<T1, T2>(Action<T1, T2> action, int count, TimeSpan timeout)
        {
            var task = new HitTask<T1, T2>
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = count,
                Mode = ConcurrencyFilters.Debounce
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(ev.Argument.Item1, ev.Argument.Item2);
            };
            return task;
        }

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <typeparam name="T1">The type of argument 1.</typeparam>
        /// <typeparam name="T2">The type of argument 2.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="count">The hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T1, T2> Times<T1, T2>(HitEventHandler<Tuple<T1, T2>> action, int count, TimeSpan timeout)
        {
            var task = new HitTask<T1, T2>
            {
                Delay = timeout,
                Timeout = timeout,
                MinCount = count,
                Mode = ConcurrencyFilters.Debounce
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(task, ev);
            };
            return task;
        }
    }

    /// <summary>
    /// The arguments of hit event.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    public class HitTask<T1, T2> : BaseHitTask<Tuple<T1, T2>>
    {
        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public bool Process(T1 arg1, T2 arg2) => Process(new Tuple<T1, T2>(arg1, arg2));

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> ProcessAsync(T1 arg1, T2 arg2) => ProcessAsync(new Tuple<T1, T2>(arg1, arg2));

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public void ProcessBegin(T1 arg1, T2 arg2) => _ = ProcessAsync(new Tuple<T1, T2>(arg1, arg2));

        /// <summary>
        /// Creates a debounce task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="delay">Delay time span.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// Maybe a handler will be asked to process several times in a short time
        /// but you just want to process once at the last time because the previous ones are obsolete.
        /// A sample scenario is real-time search.
        /// </remarks>
        public static HitTask<T1, T2> Debounce(Action<T1, T2> action, TimeSpan delay) => HitTask.Debounce(action, delay);

        /// <summary>
        /// Creates a throttle task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// You may want to request to call an action only once in a short time
        /// even if you request to call several times.
        /// The rest will be ignored.
        /// So the handler will be frozen for a while after it has processed.
        /// </remarks>
        public static HitTask<T1, T2> Throttle(Action<T1, T2> action, TimeSpan duration) => HitTask.Throttle(action, duration);

        /// <summary>
        /// Creates a multi-hit task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remark>
        /// The handler to process for the specific times and it will be reset after a while.
        /// </remark>
        public static HitTask<T1, T2> Mutliple(Action<T1, T2> action, int min, int? max, TimeSpan timeout) => HitTask.Mutliple(action, min, max, timeout);

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="min">The minmum hit count.</param>
        /// <param name="max">The maxmum hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T1, T2> Times(Action<T1, T2> action, int min, int? max, TimeSpan timeout) => HitTask.Times(action, min, max, timeout);

        /// <summary>
        /// Creates a hit task responded at a specific times.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="count">The hit count.</param>
        /// <param name="timeout">The time span between each hit.</param>
        /// <returns>The hit task instance.</returns>
        /// <remarks>
        /// A handler to process at last only when request to call in the specific times range.
        /// A sample scenario is double click.
        /// </remarks>
        public static HitTask<T1, T2> Times(Action<T1, T2> action, int count, TimeSpan timeout) => HitTask.Times(action, count, timeout);
    }

    /// <summary>
    /// The arguments of hit event.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 1.</typeparam>
    public class HitTask<T1, T2, T3> : BaseHitTask<Tuple<T1, T2, T3>>
    {
        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 2.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public bool Process(T1 arg1, T2 arg2, T3 arg3) => Process(new Tuple<T1, T2, T3>(arg1, arg2, arg3));

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 2.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> ProcessAsync(T1 arg1, T2 arg2, T3 arg3) => ProcessAsync(new Tuple<T1, T2, T3>(arg1, arg2, arg3));

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public void ProcessBegin(T1 arg1, T2 arg2, T3 arg3) => _ = ProcessAsync(new Tuple<T1, T2, T3>(arg1, arg2, arg3));
    }

    /// <summary>
    /// The arguments of hit event.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    public class HitTask<T1, T2, T3, T4> : BaseHitTask<Tuple<T1, T2, T3, T4>>
    {
        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public bool Process(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => Process(new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4));

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> ProcessAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => ProcessAsync(new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4));

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public void ProcessBegin(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => _ = ProcessAsync(new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4));
    }

    /// <summary>
    /// The arguments of hit event.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    public class HitTask<T1, T2, T3, T4, T5> : BaseHitTask<Tuple<T1, T2, T3, T4, T5>>
    {
        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public bool Process(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => Process(new Tuple<T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5));

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> ProcessAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => ProcessAsync(new Tuple<T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5));
    }

    /// <summary>
    /// The arguments of hit event.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    public class HitTask<T1, T2, T3, T4, T5, T6> : BaseHitTask<Tuple<T1, T2, T3, T4, T5, T6>>
    {
        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public bool Process(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => Process(new Tuple<T1, T2, T3, T4, T5, T6>(arg1, arg2, arg3, arg4, arg5, arg6));

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> ProcessAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => ProcessAsync(new Tuple<T1, T2, T3, T4, T5, T6>(arg1, arg2, arg3, arg4, arg5, arg6));
    }

    /// <summary>
    /// The arguments of hit event.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    public class HitTask<T1, T2, T3, T4, T5, T6, T7> : BaseHitTask<Tuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public bool Process(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => Process(new Tuple<T1, T2, T3, T4, T5, T6, T7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7));

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> ProcessAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => ProcessAsync(new Tuple<T1, T2, T3, T4, T5, T6, T7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7));
    }

    /// <summary>
    /// The arguments of hit event.
    /// </summary>
    /// <typeparam name="T1">The type of argument 1.</typeparam>
    /// <typeparam name="T2">The type of argument 2.</typeparam>
    /// <typeparam name="T3">The type of argument 3.</typeparam>
    /// <typeparam name="T4">The type of argument 4.</typeparam>
    /// <typeparam name="T5">The type of argument 5.</typeparam>
    /// <typeparam name="T6">The type of argument 6.</typeparam>
    /// <typeparam name="T7">The type of argument 7.</typeparam>
    /// <typeparam name="T8">The type of argument 8.</typeparam>
    public class HitTask<T1, T2, T3, T4, T5, T6, T7, T8> : BaseHitTask<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        /// <param name="arg8">The argument 8.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public bool Process(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => Process(new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        /// <param name="arg8">The argument 8.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> ProcessAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => ProcessAsync(new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));
    }
}
