using System;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    /// <summary>
    /// The task states.
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// Ready for processing.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Processing.
        /// </summary>
        Working = 1,

        /// <summary>
        /// Failed to process.
        /// </summary>
        Faulted = 2,

        /// <summary>
        /// Cancelled.
        /// </summary>
        Canceled = 3,

        /// <summary>
        /// Process succeeded.
        /// </summary>
        Done = 4
    }

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
        //Lock = 3
    }

    /// <summary>
    /// The cancellable task.
    /// </summary>
    public abstract class CancellableTask<T>
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The result cache.
        /// </summary>
        private T result = default(T);

        /// <summary>
        /// The exception.
        /// </summary>
        private Exception exception = null;

        /// <summary>
        /// Gets the task state.
        /// </summary>
        public TaskState State { get; private set; } = TaskState.Pending;

        /// <summary>
        /// Gets a value indicating whether the task is cancelled.
        /// </summary>
        public bool IsCanceled => State == TaskState.Canceled;

        /// <summary>
        /// Gets a value indicating whether the task is pending to process.
        /// </summary>
        public bool IsPending => State == TaskState.Pending;

        /// <summary>
        /// Gets a value indicating whether the task is processing.
        /// </summary>
        public bool IsWorking => State == TaskState.Working;

        /// <summary>
        /// Gets a value indicating whether the task is not finished.
        /// </summary>
        public bool IsPendingOrWorking => State == TaskState.Pending || State == TaskState.Working;

        /// <summary>
        /// Gets a value indicating whether the task is done.
        /// </summary>
        public bool IsDone => State == TaskState.Done;

        /// <summary>
        /// Gets a value indicating whether the task is failed.
        /// </summary>
        public bool IsFailed => State == TaskState.Faulted;

        /// <summary>
        /// Gets whether cancellation has been requested for this System.Threading.CancellationTokenSource.
        /// </summary>
        public bool IsCancellationRequested { get; set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public T Result
        {
            get
            {
                if (State == TaskState.Pending || State == TaskState.Working) throw new InvalidOperationException();
                else if (State == TaskState.Faulted) throw exception;
                ThrowIfCancellationRequested();
                return result;
            }
        }

        /// <summary>
        /// Processes.
        /// </summary>
        public void Process()
        {
            ProcessAsync().Wait();
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <returns>The processing task instance.</returns>
        public async Task<T> ProcessAsync()
        {
            lock (locker)
            {
                if (State != TaskState.Pending) throw new InvalidOperationException();
                State = TaskState.Working;
            }

            ThrowIfCancellationRequested();
            try
            {
                OnInitAsync();
            }
            catch (OperationCanceledException ex)
            {
                State = TaskState.Canceled;
                exception = ex;
                throw;
            }
            catch (Exception ex)
            {
                State = TaskState.Faulted;
                exception = ex;
                throw;
            }

            ThrowIfCancellationRequested();
            try
            {
                return result = await OnProcessAsync();
            }
            catch (OperationCanceledException ex)
            {
                State = TaskState.Canceled;
                exception = ex;
                throw;
            }
            catch (Exception ex)
            {
                State = TaskState.Faulted;
                exception = ex;
                throw;
            }
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel()
        {
            lock (locker)
            {
                if (State == TaskState.Canceled || IsCancellationRequested) return;
                IsCancellationRequested = true;
            }

            try
            {
                OnCancel();
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ThrowIfCancellationRequested()
        {
            if (State != TaskState.Canceled) return;
            if (exception != null && exception is OperationCanceledException) throw exception;
            throw new OperationCanceledException();
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        protected virtual void OnCancel()
        {
        }

        /// <summary>
        /// Initializes before processing.
        /// </summary>
        protected virtual void OnInitAsync()
        {
            return;
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <returns>The processing task result.</returns>
        protected abstract Task<T> OnProcessAsync();
    }

    /// <summary>
    /// The arguments of hit event.
    /// </summary>
    public class HitEventArgs<T>: EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the HitEventArgs class.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="occur"></param>
        /// <param name="previous"></param>
        /// <param name="total"></param>
        /// <param name="arg"></param>
        public HitEventArgs(int count, DateTime request, DateTime? previous, T arg)
        {
            Count = count;
            RequestDate = request;
            PreviousRequestDate = previous;
            Argument = arg;
        }

        /// <summary>
        /// Gets the hit count.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the occur date time.
        /// </summary>
        public DateTime RequestDate { get; }

        /// <summary>
        /// Gets the previous occur date time.
        /// </summary>
        public DateTime? PreviousRequestDate { get; }

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
        public int? MinCount { get; set; }

        /// <summary>
        /// Gets or sets the delay time span to process the task.
        /// </summary>
        public TimeSpan? Delay { get; set; }

        /// <summary>
        /// Gets or sets the timeout a set of hit.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Gets or sets the lock time span after timeout.
        /// </summary>
        public TimeSpan? Lock { get; set; }

        /// <summary>
        /// Gets or sets the mode for multiple hitting.
        /// </summary>
        public ConcurrencyFilters Mode { get; set; }

        /// <summary>
        /// Gets or sets a valueindicating whether need to avoid all hit request.
        /// </summary>
        public bool Avoid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pause to process.
        /// </summary>
        public bool IsPaused { get; set; }

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
        public bool Process(T arg = default(T))
        {
            var task = ProcessAsync(arg);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Sends request to process.
        /// </summary>
        /// <returns>The processing task instance.</returns>
        public async Task<bool> ProcessAsync(T arg = default(T))
        {
            if (Avoid) return false;
            var now = DateTime.Now;
            var mode = Mode;
            HitEventArgs<T> args;
            lock (locker)
            {
                var latestReqDate = LatestRequestDate;
                var timeout = Timeout;
                if (timeout.HasValue && (!latestReqDate.HasValue || now - latestReqDate.Value < timeout.Value))
                {
                    var lockSpan = Lock;
                    if (lockSpan.HasValue && now - latestReqDate.Value < timeout.Value + lockSpan.Value)
                    {
                        latestRequestCount = 0;
                        return false;
                    }

                    latestRequestCount = 1;
                }

                var count = ++latestRequestCount;
                var minCount = MinCount;
                if (minCount.HasValue || minCount.Value > count) return false;
                var maxCount = MaxCount;
                if (maxCount.HasValue || maxCount.Value < count) return false;
                if (count > 1 && mode == ConcurrencyFilters.Mono) return false;
                args = new HitEventArgs<T>(count, now, latestReqDate, arg);
                latestArgs = args;
            }

            if (Delay.HasValue) await Task.Delay(Delay.Value);
            if (mode == ConcurrencyFilters.All || latestArgs != args || IsPaused) return false;
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
        public static HitTask Debounce(Action<HitTask, HitEventArgs<object>> action, TimeSpan delay)
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
        public static HitTask<T> Debounce<T>(Action<HitTask<T>, HitEventArgs<T>> action, TimeSpan delay)
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
        /// <param name="timeout">The timeout.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Throttle(Action action, TimeSpan timeout)
        {
            var task = new HitTask
            {
                Mode = ConcurrencyFilters.Debounce,
                Timeout = timeout,
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
        /// <param name="timeout">The timeout.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Throttle(Action<HitTask, HitEventArgs<object>> action, TimeSpan timeout)
        {
            var task = new HitTask
            {
                Mode = ConcurrencyFilters.Debounce,
                Timeout = timeout,
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
        /// <param name="timeout">The timeout.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Throttle<T>(Action<T> action, TimeSpan timeout)
        {
            var task = new HitTask<T>
            {
                Mode = ConcurrencyFilters.Debounce,
                Timeout = timeout,
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
        /// <param name="timeout">The timeout.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Throttle<T>(Action<HitTask<T>, HitEventArgs<T>> action, TimeSpan timeout)
        {
            var task = new HitTask<T>
            {
                Mode = ConcurrencyFilters.Debounce,
                Timeout = timeout,
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
        /// <param name="timeout">The timeout.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Mutliple(Action action, int min, int max, TimeSpan duration)
        {
            var task = new HitTask
            {
                Timeout = duration,
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
        /// <param name="timeout">The timeout.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask Mutliple(Action<HitTask, HitEventArgs<object>> action, int min, int max, TimeSpan duration)
        {
            var task = new HitTask
            {
                Timeout = duration,
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
        /// <param name="timeout">The timeout.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Mutliple<T>(Action<T> action, int min, int max, TimeSpan duration)
        {
            var task = new HitTask<T>
            {
                Timeout = duration,
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
        /// <param name="timeout">The timeout.</param>
        /// <returns>The hit task instance.</returns>
        public static HitTask<T> Mutliple<T>(Action<HitTask<T>, HitEventArgs<T>> action, int min, int max, TimeSpan duration)
        {
            var task = new HitTask<T>
            {
                Timeout = duration,
                MinCount = min,
                MaxCount = max
            };
            if (action != null) task.Processed += (sender, ev) =>
            {
                action(task, ev);
            };
            return task;
        }
    }
}
