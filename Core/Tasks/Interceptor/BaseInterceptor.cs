using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// The handler interceptor to determine whether the current invoking Handler can run right now, later or never.
/// </summary>
public abstract class BaseInterceptor<T>
{
    /// <summary>
    /// The locker.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if NET9_0_OR_GREATER
    private readonly Lock locker = new();
#else
    private readonly object locker = new();
#endif

    /// <summary>
    /// The count of latest request.
    /// </summary>
    private int latestRequestCount = 0;

    /// <summary>
    /// The cache of the latest event arguments.
    /// </summary>
    private InterceptorEventArgs<T> latestArgs = null;

    /// <summary>
    /// The interceptor policy.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private InterceptorPolicy policy;

    /// <summary>
    /// The optional Handler to execute.
    /// </summary>
    private readonly Func<InterceptorEventArgs<T>, Task> action;

    /// <summary>
    /// The invoking task list.
    /// </summary>
    private readonly Collection.SynchronizedList<Task> tasks = new();

    /// <summary>
    /// Initializes a new instance of the BaseInterceptor class.
    /// </summary>
    public BaseInterceptor()
    {
        policy = new();
    }

    /// <summary>
    /// Initializes a new instance of the BaseInterceptor class.
    /// </summary>
    /// <param name="policy">The interceptor policy.</param>
    public BaseInterceptor(InterceptorPolicy policy)
    {
        this.policy = policy ?? new();
    }

    /// <summary>
    /// Initializes a new instance of the BaseInterceptor class.
    /// </summary>
    /// <param name="action">The Handler to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    public BaseInterceptor(Action<T> action, InterceptorPolicy policy)
        : this(policy)
    {
        if (action != null) Executed += (sender, ev) => action(ev.Argument);
    }

    /// <summary>
    /// Initializes a new instance of the BaseInterceptor class.
    /// </summary>
    /// <param name="action">The Handler to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the Handler result; otherwise, false.</param>
    public BaseInterceptor(Func<T, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        : this(policy)
    {
        if (action is null) return;
        if (doNotWait) Executed += (sender, ev) => _ = action(ev.Argument);
        else this.action = ev => action(ev.Argument);
    }

    /// <summary>
    /// Initializes a new instance of the BaseInterceptor class.
    /// </summary>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="action">The Handler to register to execute.</param>
    /// <param name="doNotWait">true if do not wait for the Handler result; otherwise, false.</param>
    public BaseInterceptor(InterceptorPolicy policy, Func<InterceptorEventArgs<T>, Task> action, bool doNotWait = false)
        : this(policy)
    {
        if (doNotWait && this.action != null) Executed += (sender, ev) => _ = action(ev);
        else this.action = action;
    }

    /// <summary>
    /// Adds or removes the event that the actions registered is executed.
    /// </summary>
    public event EventHandler<InterceptorEventArgs<T>> Executed;

    /// <summary>
    /// Gets or sets the interceptor policy.
    /// </summary>
    public InterceptorPolicy Policy
    {
        get => policy;
        set => policy = value ?? new();
    }

    /// <summary>
    /// Gets or sets a value indicating whether need ignore all invoking.
    /// </summary>
    public bool IsIgnoring { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether pause to execute.
    /// </summary>
    public bool IsPaused { get; set; }

    /// <summary>
    /// Gets the total count of execution.
    /// </summary>
    public int ExecutionCount { get; private set; }

    /// <summary>
    /// Gets the first invoking date time in the current duration.
    /// </summary>
    public DateTime? InitiationInvokingDate { get; private set; }

    /// <summary>
    /// Gets the last invoking date time in the current duration.
    /// </summary>
    public DateTime? LatestInvokingDate { get; private set; }

    /// <summary>
    /// Gets the latest execution date time.
    /// </summary>
    public DateTime? LatestExecutionDate { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the task is working.
    /// </summary>
    public bool IsWorking => tasks.Count > 0;

    /// <summary>
    /// Resets the duration information.
    /// </summary>
    public virtual void ResetDuration()
    {
        lock (locker)
        {
            InitiationInvokingDate = null;
            LatestExecutionDate = null;
            latestArgs = null;
            latestRequestCount = 0;
        }
    }

    /// <summary>
    /// Invokes. But executes only when matches the policy.
    /// </summary>
    /// <param name="arg">The optional argument.</param>
    /// <returns>true if executes at current time; otherwise, false.</returns>
    public Task<bool> InvokeAsync(T arg)
    {
        var task = InvokeInternalAsync(arg);
        tasks.Add(task);
        if (tasks.Count == 1) _ = WaitAsync();
        return task;
    }

    /// <summary>
    /// Waits for all invoking Handler tasks.
    /// </summary>
    /// <returns>A task that represents the completion of all of the invoking Handler tasks.</returns>
    public async Task WaitAsync()
    {
        while (tasks.Count > 0)
        {
            await Task.WhenAll(tasks.ToList());
            tasks.RemoveAll(ele => {
                try
                {
                    return ele.IsCompleted;
                }
                catch (InvalidOperationException)
                {
                    return true;
                }
                catch (NullReferenceException)
                {
                    return true;
                }
            });
        }
    }

    /// <summary>
    /// Creates a function with fixed argument.
    /// </summary>
    /// <param name="arg">The argument</param>
    /// <returns>A function to invoke.</returns>
    public Func<Task<bool>> FixArgument(T arg)
        => () => InvokeAsync(arg);

    /// <summary>
    /// Creates an Handler with fixed argument.
    /// </summary>
    /// <param name="arg">The argument</param>
    /// <returns>An Handler to invoke.</returns>
    public Action FixArgumentAction(T arg)
        => () => _ = InvokeAsync(arg);

    /// <summary>
    /// Raises on executing.
    /// </summary>
    /// <param name="args">The argument.</param>
    protected virtual void OnExecute(InterceptorEventArgs<T> args)
    {
    }

    /// <summary>
    /// Tests if the argument is valid.
    /// </summary>
    /// <param name="arg">The argument to test.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    protected virtual bool ValidateArgument(T arg) => true;

    /// <summary>
    /// Invokes. But executes only when matches the policy.
    /// </summary>
    /// <param name="arg">The optional argument.</param>
    /// <returns>true if executes at current time; otherwise, false.</returns>
    private async Task<bool> InvokeInternalAsync(T arg)
    {
        if (IsIgnoring || !ValidateArgument(arg)) return false;
        var now = DateTime.Now;
        var mode = policy.Mode;
        InterceptorEventArgs<T> args;
        lock (locker)
        {
            var latestReqDate = LatestInvokingDate;
            var timeout = policy.Timeout;
            var firstReq = InitiationInvokingDate ?? latestReqDate ?? now;
            var policyDuration = policy.Duration;

            if (policyDuration.HasValue && policyDuration.Value < now - firstReq)
            {
                latestRequestCount = 0;
                InitiationInvokingDate = now;
            }
            else if (!timeout.HasValue || !latestReqDate.HasValue || timeout.Value < now - latestReqDate.Value)
            {
                latestRequestCount = 0;
                InitiationInvokingDate = now;
            }

            LatestInvokingDate = now;
            var count = ++latestRequestCount;
            var minCount = policy.MinCount;
            if (minCount > count)
            {
                latestArgs = null;
                return false;
            }

            var maxCount = policy.MaxCount;
            if (maxCount.HasValue && maxCount.Value < count)
            {
                latestArgs = null;
                return false;
            }

            if (count > 1 && mode == InterceptorModes.Mono) return false;
            var lArgs = latestArgs;
            if (mode == InterceptorModes.Lock && lArgs != null && count > 1)
            {
                args = new InterceptorEventArgs<T>(lArgs.Count, lArgs.InvokingDate, lArgs.PreviousInvokedDate, lArgs.FirstInvokedDate, lArgs.Argument);
            }
            else
            {
                args = new InterceptorEventArgs<T>(count, now, latestReqDate, firstReq, arg);
            }

            latestArgs = args;
        }

        var delayValue = policy.Delay;
        if (delayValue.HasValue) await Task.Delay(delayValue.Value);
        if (mode != InterceptorModes.Pass)
        {
            if (latestArgs != args) return false;
        }

        if (IsPaused) return false;
        LatestExecutionDate = DateTime.Now;
        ExecutionCount++;
        OnExecute(args);
        if (action != null) await action(args);
        Executed?.Invoke(this, args);
        return true;
    }
}
