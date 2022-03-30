using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// The handler interceptor to determine whether the current invoking action can run right now, later or never..
/// </summary>
/// <typeparam name="T">The type of action handler argument.</typeparam>
public class Interceptor<T> : BaseInterceptor<T>
{
    /// <summary>
    /// Initializes a new instance of the Interceptor class.
    /// </summary>
    public Interceptor()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Interceptor class.
    /// </summary>
    /// <param name="policy">The interceptor policy.</param>
    public Interceptor(InterceptorPolicy policy)
        : base(policy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Interceptor class.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    public Interceptor(Action<T> action, InterceptorPolicy policy)
        : base(action, policy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Interceptor class.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    public Interceptor(Func<T, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        : base(action, policy, doNotWait)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Interceptor class.
    /// </summary>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    public Interceptor(InterceptorPolicy policy, Func<InterceptorEventArgs<T>, Task> action, bool doNotWait = false)
        : base(policy, action, doNotWait)
    {
    }

    /// <summary>
    /// Starts to invokes. But executes only when matches the policy.
    /// </summary>
    /// <param name="arg">The argument.</param>
    public void InvokeBegin(T arg)
        => _ = InvokeAsync(arg);

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Action<T> Action(Action<T> action, InterceptorPolicy policy)
        => new Interceptor<T>(action, policy).InvokeBegin;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Func<T, Task> Action(Func<T, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        => new Interceptor<T>(action, policy, doNotWait).InvokeAsync;

    /// <summary>
    /// Creates a debounce interceptor policy.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="delay">Delay time span.</param>
    /// <returns>The interceptor policy.</returns>
    /// <remarks>
    /// Maybe a handler will be asked to process several times in a short time
    /// but you just want to process once at the last time because the previous ones are obsolete.
    /// A sample scenario is real-time search.
    /// </remarks>
    public static Action<T> Debounce(Action<T> action, TimeSpan delay)
        => Action(action, InterceptorPolicy.Debounce(delay));

    /// <summary>
    /// Creates a throttle interceptor policy.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="duration">The duration.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// You may want to request to call an action only once in a short time
    /// even if you request to call several times.
    /// The rest will be ignored.
    /// So the handler will be frozen for a while after it has processed.
    /// </remarks>
    public static Action<T> Throttle(Action<T> action, TimeSpan duration)
        => Action(action, InterceptorPolicy.Throttle(duration));

    /// <summary>
    /// Creates a multi-hit interceptor policy.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remark>
    /// The handler to process for the specific times and it will be reset after a while.
    /// </remark>
    public static Action<T> Mutliple(Action<T> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Mutliple(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Action<T> Times(Action<T> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="count">The invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Action<T> Times(Action<T> action, int count, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(count, timeout));

    /// <summary>
    /// Creates a debounce interceptor policy.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="delay">Delay time span.</param>
    /// <returns>The interceptor policy.</returns>
    /// <remarks>
    /// Maybe a handler will be asked to process several times in a short time
    /// but you just want to process once at the last time because the previous ones are obsolete.
    /// A sample scenario is real-time search.
    /// </remarks>
    public static Func<T, Task> Debounce(Func<T, Task> action, TimeSpan delay)
        => Action(action, InterceptorPolicy.Debounce(delay));

    /// <summary>
    /// Creates a throttle interceptor policy.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="duration">The duration.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// You may want to request to call an action only once in a short time
    /// even if you request to call several times.
    /// The rest will be ignored.
    /// So the handler will be frozen for a while after it has processed.
    /// </remarks>
    public static Func<T, Task> Throttle(Func<T, Task> action, TimeSpan duration)
        => Action(action, InterceptorPolicy.Throttle(duration));

    /// <summary>
    /// Creates a multi-hit interceptor policy.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remark>
    /// The handler to process for the specific times and it will be reset after a while.
    /// </remark>
    public static Func<T, Task> Mutliple(Func<T, Task> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Mutliple(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Func<T, Task> Times(Func<T, Task> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="count">The invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Func<T, Task> Times(Func<T, Task> action, int count, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(count, timeout));
}
