using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Tasks;

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
    /// Gets a value indicating whether it has processed.
    /// </summary>
    public bool HasProcessed { get; private set; }

    /// <summary>
    /// Gets a value indicating whether it processes succeeded.
    /// </summary>
    public bool? IsSuccessful { get; private set; }

    /// <summary>
    /// Gets the count of retry.
    /// </summary>
    public int RetryCount => HasProcessed ? (Exceptions.Count + (IsSuccessful == true ? 0 : -1)) : 0;

    /// <summary>
    /// Gets the exceptions catched to retry during processing.
    /// </summary>
    public List<Exception> Exceptions { get; } = new();

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
/// Retry event argument.
/// </summary>
public class RetryEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the RetryEventArgs class.
    /// </summary>
    /// <param name="processTime">The processing date time list.</param>
    public RetryEventArgs(IReadOnlyList<DateTime> processTime) => ProcessTime = processTime;

    /// <summary>
    /// Gets the processing date time list.
    /// </summary>
    public IReadOnlyList<DateTime> ProcessTime { get; }
}

/// <summary>
/// The retry task.
/// </summary>
/// <typeparam name="T">The type of retry policy.</typeparam>
/// <remarks>
/// You can set up a task and a retry policy
/// so that the task will process and retry automatically expected when it fails.
/// </remarks>
public class RetryTask<T> where T : IRetryPolicy
{
    /// <summary>
    /// Initializes a new instance of the RetryTask class.
    /// </summary>
    /// <param name="retryPolicy">The retry policy.</param>
    /// <param name="action">The action to process.</param>
    /// <param name="exceptionHandler">The exception handler.</param>
    public RetryTask(T retryPolicy, Action<RetryEventArgs> action = null, Reflection.ExceptionHandler exceptionHandler = null)
    {
        RetryPolicy = retryPolicy;
        if (action != null) Processing += (sender, ev) => action(ev);
        ExceptionHandler = exceptionHandler ?? new Reflection.ExceptionHandler();
    }

    /// <summary>
    /// Initializes a new instance of the RetryTask class.
    /// </summary>
    /// <param name="retryPolicy">The retry policy.</param>
    /// <param name="action">The action to process.</param>
    /// <param name="exceptionHandler">The exception handler.</param>
    public RetryTask(T retryPolicy, Action action, Reflection.ExceptionHandler exceptionHandler = null)
        : this(retryPolicy, action is null ? null : ev => action(), exceptionHandler)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RetryTask class.
    /// </summary>
    /// <param name="retryPolicy">The retry policy.</param>
    /// <param name="exceptionHandler">The exception handler.</param>
    public RetryTask(T retryPolicy, Reflection.ExceptionHandler exceptionHandler)
        : this(retryPolicy, null as Action<RetryEventArgs>, exceptionHandler)
    {
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
    /// Gets the task state.
    /// </summary>
    public TaskStates State { get; private set; } = TaskStates.Pending;

    /// <summary>
    /// Adds or removes an event handler on processing.
    /// </summary>
    public event EventHandler<RetryEventArgs> Processing;

    /// <summary>
    /// Enables retry policy to process.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The processing retry result.</returns>
    public async Task<RetryResult> ProcessAsync(CancellationToken cancellationToken = default)
    {
        State = TaskStates.Initializing;
        var result = new RetryResult();
        var retry = RetryPolicy?.CreateInstance() ?? new InternalRetryInstance();
        State = TaskStates.Working;
        while (true)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                State = TaskStates.Canceled;
                throw;
            }
            catch (ObjectDisposedException)
            {
                State = TaskStates.Canceled;
                throw;
            }

            try
            {
                await OnProcessAsync(cancellationToken);
                Processing?.Invoke(this, new RetryEventArgs(retry.ProcessTime));
                State = TaskStates.Done;
                result.Success();
                return result;
            }
            catch (Exception ex)
            {
                State = TaskStates.WaitingToRetry;
                result.Fail(ex);
                try
                {
                    ex = ExceptionHandler.GetException(ex);
                }
                catch (Exception)
                {
                    State = TaskStates.Faulted;
                    throw;
                }

                if (ex != null)
                {
                    State = TaskStates.Faulted;
                    throw ex;
                }
            }

            var span = retry.Next();
            if (!span.HasValue)
            {
                State = TaskStates.Faulted;
                result.End();
                return result;
            }

            await Task.Delay(span.Value, cancellationToken);
            State = TaskStates.Retrying;
        }
    }

    /// <summary>
    /// Raises on processing.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>An asynchronous operation instance.</returns>
    protected virtual Task OnProcessAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() => { }, cancellationToken);
    }
}

/// <summary>
/// The record instance for action retry.
/// </summary>
public abstract class RetryInstance
{
    private readonly List<DateTime> list = new();

    /// <summary>
    /// Gets the processing date time list.
    /// </summary>
    public IReadOnlyList<DateTime> ProcessTime => list.AsReadOnly();

    /// <summary>
    /// Gets or sets a value indicating whether disable the retry policy.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Tests whether need retry.
    /// </summary>
    /// <returns>true if need retry; otherwise, false.</returns>
    /// <remarks>Strongly suggest call Next() member method directly and test if the value returned has value.</remarks>
    public bool Need()
    {
        if (IsDisabled) return false;
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
        if (IsDisabled) return null;
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
