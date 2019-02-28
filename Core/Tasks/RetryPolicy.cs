using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    /// <summary>
    /// Retry policy.
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// Creates an instance of action retry.
        /// </summary>
        /// <returns>A processing retry instance.</returns>
        RetryInstance CreateInstance();
    }

    /// <summary>
    /// The record instance for action retry.
    /// </summary>
    internal class InternalRetryInstance : RetryInstance
    {
        internal Func<TimeSpan?> next;

        /// <summary>
        /// Gets the waiting time span for next retry; or null, if no more retry.
        /// </summary>
        /// <returns>A time span for next retry; or null, if no more retry.</returns>
        protected override TimeSpan? GetNextSpan()
        {
            return next?.Invoke();
        }
    }

    /// <summary>
    /// The retry result.
    /// </summary>
    public class RetryResult
    {
        /// <summary>
        /// Gets if it has processed.
        /// </summary>
        public bool HasProcessed { get; private set; }

        /// <summary>
        /// Gets if it processes succeeded.
        /// </summary>
        public bool? IsSuccessful { get; private set; }

        /// <summary>
        /// Gets the count of retry.
        /// </summary>
        public int RetryCount => HasProcessed ? (Exceptions.Count + (IsSuccessful == true ? 0 : -1)) : 0;

        /// <summary>
        /// Gets the exceptions catched to retry during processing.
        /// </summary>
        public List<Exception> Exceptions { get; } = new List<Exception>();

        /// <summary>
        /// Gets the exception catched during last processing if failed finally.
        /// </summary>
        public Exception LastException => Exceptions.Count > 0 ? Exceptions[Exceptions.Count - 1] : null;

        /// <summary>
        /// Failures once.
        /// </summary>
        /// <param name="exception">The exception thrown.</param>
        /// <param name="end">true if final; otherwise, false.</param>
        public void Fail(Exception exception = null, bool end = false)
        {
            if (IsSuccessful.HasValue) return;
            HasProcessed = true;
            if (end) IsSuccessful = false;
            Exceptions.Add(exception);
        }

        /// <summary>
        /// Succeeds.
        /// </summary>
        public virtual void Success()
        {
            if (IsSuccessful.HasValue) return;
            HasProcessed = true;
            IsSuccessful = true;
        }

        /// <summary>
        /// Ends.
        /// </summary>
        public void End()
        {
            if (!HasProcessed && IsSuccessful.HasValue) return;
            IsSuccessful = false;
        }
    }

    /// <summary>
    /// The retry result.
    /// </summary>
    public class RetryResult<T> : RetryResult
    {
        /// <summary>
        /// Gets the result without exception.
        /// </summary>
        public T TryResult { get; private set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public T Result
        {
            get
            {
                if (IsSuccessful == true) return TryResult;
                var ex = LastException;
                if (ex != null) throw LastException;
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Succeeds.
        /// </summary>
        /// <param name="value">The result value.</param>
        public virtual void Success(T value)
        {
            TryResult = value;
            base.Success();
        }
    }

    /// <summary>
    /// Retry status.
    /// </summary>
    public class RetryStatus
    {
        /// <summary>
        /// Initializes a new instance of the RetryStatus class.
        /// </summary>
        /// <param name="processTime">The processing date time list.</param>
        public RetryStatus(IReadOnlyList<DateTime> processTime)
        {
            ProcessTime = processTime;
        }

        /// <summary>
        /// Gets the processing date time list.
        /// </summary>
        public IReadOnlyList<DateTime> ProcessTime { get; }

        /// <summary>
        /// Gets or sets a value indicating whether enable the retry policy.
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Retry event argument.
    /// </summary>
    public class RetryEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the RetryEventArgs class.
        /// </summary>
        /// <param name="status">The retry status.</param>
        public RetryEventArgs(RetryStatus status) => Status = status;

        /// <summary>
        /// Gets the retry status.
        /// </summary>
        public RetryStatus Status { get; }
    }

    /// <summary>
    /// The retry task.
    /// </summary>
    /// <typeparam name="T">The type of retry policy.</typeparam>
    public class RetryTask<T> where T : IRetryPolicy
    {
        /// <summary>
        /// Initializes a new instance of the RetryTask class.
        /// </summary>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="action">The action to process.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        public RetryTask(T retryPolicy, Action<RetryStatus> action = null, Reflection.ExceptionHandler exceptionHandler = null)
        {
            RetryPolicy = retryPolicy;
            if (action != null) Processing += (sender, ev) => action(ev.Status);
            ExceptionHandler = exceptionHandler ?? new Reflection.ExceptionHandler();
        }

        /// <summary>
        /// Gets the retry policy.
        /// </summary>
        public T RetryPolicy { get; }

        /// <summary>
        /// Gets the exception handler.
        /// </summary>
        public Reflection.ExceptionHandler ExceptionHandler { get; }

        /// <summary>
        /// Adds or removes an event handler on processing.
        /// </summary>
        public event EventHandler<RetryEventArgs> Processing;

        /// <summary>
        /// Enables retry policy to process.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public async Task<RetryResult> Process(CancellationToken cancellationToken = default)
        {
            var result = new RetryResult();
            var retry = RetryPolicy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    Processing?.Invoke(this, new RetryEventArgs(retry.Status));
                    result.Success();
                    return result;
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    ex = ExceptionHandler.GetException(ex);
                    if (ex != null) throw ex;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value);
            }
        }
    }

    /// <summary>
    /// The instance for action retry.
    /// </summary>
    public static class RetryExtension
    {
        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The action to process.</param>
        /// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public static async Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Action action, Func<Exception, Exception> needThrow, CancellationToken cancellationToken = default)
        {
            var result = new RetryResult();
            if (action == null) return result;
            if (needThrow == null) needThrow = ex => ex;
            var retry = policy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    action();
                    result.Success();
                    return result;
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    ex = needThrow(ex);
                    if (ex != null) throw ex;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value);
            }
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The action to process.</param>
        /// <param name="exceptionTypesForRetry">The exception types for retry.</param>
        /// <returns>The processing retry result.</returns>
        public static Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Action action, params Type[] exceptionTypesForRetry)
        {
            return ProcessAsync(policy, action, HitException(exceptionTypesForRetry));
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action to process.</param>
        /// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public static async Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Func<CancellationToken, Task> action, Func<Exception, Exception> needThrow, CancellationToken cancellationToken = default)
        {
            var result = new RetryResult();
            if (action == null) return result;
            if (needThrow == null) needThrow = ex => ex;
            var retry = policy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                try
                {
                    await action(cancellationToken);
                    result.Success();
                    return result;
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    ex = needThrow(ex);
                    if (ex != null) throw ex;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value);
            }
        }

        ///// <summary>
        ///// Processes an action with this retry policy.
        ///// </summary>
        ///// <param name="policy">The retry policy.</param>
        ///// <param name="action">The async action to process.</param>
        ///// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
        ///// <param name="cancellationToken">The optional cancellation token.</param>
        ///// <returns>The processing retry result.</returns>
        //public static Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Func<CancellationToken, Task> action, Func<Exception, bool> needThrow, CancellationToken cancellationToken = default)
        //{
        //    return ProcessAsync(policy, action, ex =>
        //    {
        //        return needThrow(ex) ? ex : null;
        //    }, cancellationToken);
        //}

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action to process.</param>
        /// <param name="exceptionTypesForRetry">The exception types for retry.</param>
        /// <returns>The processing retry result.</returns>
        public static Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Func<CancellationToken, Task> action, params Type[] exceptionTypesForRetry)
        {
            return ProcessAsync(policy, action, HitException(exceptionTypesForRetry));
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action to process.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <param name="exceptionTypesForRetry">The exception types for retry.</param>
        /// <returns>The processing retry result.</returns>
        public static Task<RetryResult> ProcessAsync(this IRetryPolicy policy, Func<CancellationToken, Task> action, CancellationToken cancellationToken, params Type[] exceptionTypesForRetry)
        {
            return ProcessAsync(policy, action, HitException(exceptionTypesForRetry), cancellationToken);
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action which will return a result to process.</param>
        /// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public static async Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<T>> action, Func<Exception, Exception> needThrow, CancellationToken cancellationToken = default)
        {
            var result = new RetryResult<T>();
            if (action == null) return result;
            if (needThrow == null) needThrow = ex => ex;
            var retry = policy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var r = await action(cancellationToken);
                    result.Success(r);
                    return result;
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    ex = needThrow(ex);
                    if (ex != null) throw ex;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value);
            }
        }

        ///// <summary>
        ///// Processes an action with this retry policy.
        ///// </summary>
        ///// <param name="policy">The retry policy.</param>
        ///// <param name="action">The async action which will return a result to process.</param>
        ///// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
        ///// <param name="cancellationToken">The optional cancellation token.</param>
        ///// <returns>The processing retry result.</returns>
        //public static Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<T>> action, Func<Exception, bool> needThrow, CancellationToken cancellationToken = default)
        //{
        //    return ProcessAsync(policy, action, ex =>
        //    {
        //        return needThrow(ex) ? ex : null;
        //    }, cancellationToken);
        //}

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action which will return a result to process.</param>
        /// <param name="exceptionTypesForRetry">The exception types for retry.</param>
        /// <returns>The processing retry result.</returns>
        public static Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<T>> action, params Type[] exceptionTypesForRetry)
        {
            return ProcessAsync(policy, action, HitException(exceptionTypesForRetry));
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action which will return a result to process.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <param name="exceptionTypesForRetry">The exception types for retry.</param>
        /// <returns>The processing retry result.</returns>
        public static Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken, params Type[] exceptionTypesForRetry)
        {
            return ProcessAsync(policy, action, HitException(exceptionTypesForRetry), cancellationToken);
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action to process to return a value indicating whether success and a result if success.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public static async Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<(bool, T)>> action, CancellationToken cancellationToken)
        {
            var result = new RetryResult<T>();
            if (action == null) return result;
            var retry = policy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                try
                {
                    var (succ, r) = await action(cancellationToken);
                    if (succ)
                    {
                        result.Success(r);
                        return result;
                    }

                    result.Fail();
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    throw;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value);
            }
        }

        /// <summary>
        /// Processes an action with this retry policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="action">The async action to process to return an exception if failed and a result if success.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>The processing retry result.</returns>
        public static async Task<RetryResult<T>> ProcessAsync<T>(this IRetryPolicy policy, Func<CancellationToken, Task<(Exception, T)>> action, CancellationToken cancellationToken)
        {
            var result = new RetryResult<T>();
            if (action == null) return result;
            var retry = policy?.CreateInstance() ?? new InternalRetryInstance();
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var (ex, r) = await action(cancellationToken);
                    if (ex == null)
                    {
                        result.Success(r);
                        return result;
                    }

                    result.Fail(ex);
                }
                catch (Exception ex)
                {
                    result.Fail(ex);
                    throw;
                }

                var span = retry.Next();
                if (!span.HasValue)
                {
                    result.End();
                    return result;
                }

                await Task.Delay(span.Value);
            }
        }

        /// <summary>
        /// Creates a retry record instance.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <returns>A retry record instance.</returns>
        public static RetryInstance CreateInstance(IRetryPolicy policy)
        {
            return policy?.CreateInstance() ?? new InternalRetryInstance();
        }

        private static Func<Exception, Exception> HitException(params Type[] exceptionTypesForRetry)
        {
            return ex =>
            {
                var exType = ex.GetType();
                foreach (var item in exceptionTypesForRetry)
                {
                    if (exType == item || exType.IsSubclassOf(item)) return null;
                }

                return ex;
            };
        }
    }

    /// <summary>
    /// The record instance for action retry.
    /// </summary>
    public abstract class RetryInstance
    {
        private readonly List<DateTime> list = new List<DateTime>();

        /// <summary>
        /// Gets the retry status.
        /// </summary>
        public RetryStatus Status => new RetryStatus(list.AsReadOnly());

        /// <summary>
        /// Gets the processing date time list.
        /// </summary>
        public IReadOnlyList<DateTime> ProcessTime => Status.ProcessTime;

        /// <summary>
        /// Gets or sets a value indicating whether enable the retry policy.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return Status.IsEnabled;
            }

            set
            {
                Status.IsEnabled = value;
            }
        }

        /// <summary>
        /// Tests whether need retry.
        /// </summary>
        /// <returns>true if need retry; otherwise, false.</returns>
        /// <remarks>Strongly suggest call Next() member method directly and test if the value returned has value.</remarks>
        public bool Need()
        {
            if (!IsEnabled) return false;
            try
            {
                return GetNextSpan().HasValue;
            }
            catch (InvalidOperationException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (IndexOutOfRangeException)
            {
            }

            return false;
        }

        /// <summary>
        /// Gets the waiting time span for next retry; or null, if no more retry.
        /// </summary>
        /// <returns>A time span for next retry; or null, if no more retry.</returns>
        public TimeSpan? Next()
        {
            if (!IsEnabled) return null;
            try
            {
                var span = GetNextSpan();
                if (span == null) return null;
                list.Add(DateTime.Now);
                return span;
            }
            catch (InvalidOperationException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (IndexOutOfRangeException)
            {
            }

            return null;
        }

        /// <summary>
        /// Gets the time span for next retry; or null, for no more retry.
        /// </summary>
        /// <returns>The time span for next retry; or null, for no more retry.</returns>
        protected abstract TimeSpan? GetNextSpan();
    }

    /// <summary>
    /// The linear retry policy.
    /// </summary>
    public class LinearRetryPolicy : IRetryPolicy
    {
        /// <summary>
        /// Initializes a new instance of the CustomizedRetryPolicy class.
        /// </summary>
        public LinearRetryPolicy()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomizedRetryPolicy class.
        /// </summary>
        /// <param name="count">The retry count.</param>
        public LinearRetryPolicy(int count) => Count = count;

        /// <summary>
        /// Initializes a new instance of the CustomizedRetryPolicy class.
        /// </summary>
        /// <param name="count">The retry count.</param>
        /// <param name="interval">The interval between two actions.</param>
        public LinearRetryPolicy(int count, TimeSpan interval) : this(count) => Interval = interval;

        /// <summary>
        /// Initializes a new instance of the CustomizedRetryPolicy class.
        /// </summary>
        /// <param name="count">The retry count.</param>
        /// <param name="interval">The interval between two actions.</param>
        /// <param name="increase">The increase per action.</param>
        public LinearRetryPolicy(int count, TimeSpan interval, TimeSpan increase) : this(count, interval) => Increase = increase;

        /// <summary>
        /// The retry count.
        /// </summary>
        public int Count { get; set; } = 0;

        /// <summary>
        /// The interval between two actions.
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The increase per action.
        /// </summary>
        public TimeSpan Increase { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Creates an instance of action retry.
        /// </summary>
        /// <returns>A processing retry instance.</returns>
        public RetryInstance CreateInstance()
        {
            var retry = new InternalRetryInstance();
            retry.next = () =>
            {
                var len = retry.ProcessTime.Count;
                if (len >= Count) return null;
                return TimeSpan.FromTicks(Increase.Ticks * len + Interval.Ticks);
            };
            return retry;
        }
    }

    /// <summary>
    /// The function customized retry policy.
    /// </summary>
    public class CustomizedRetryPolicy : IRetryPolicy
    {
        private readonly Func<IReadOnlyList<DateTime>, TimeSpan?> next;

        /// <summary>
        /// Initializes a new instance of the CustomizedRetryPolicy class.
        /// </summary>
        /// <param name="nextHandler">The handler to gets the waiting time span for next retry or null for no retry.</param>
        public CustomizedRetryPolicy(Func<IReadOnlyList<DateTime>, TimeSpan?> nextHandler) => next = nextHandler;

        /// <summary>
        /// Creates an instance of action retry.
        /// </summary>
        /// <returns>A processing retry instance.</returns>
        public RetryInstance CreateInstance()
        {
            var retry = new InternalRetryInstance();
            retry.next = () =>
            {
                return next?.Invoke(retry.ProcessTime);
            };
            return retry;
        }
    }
}
