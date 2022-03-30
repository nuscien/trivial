using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// The handler interceptor to determine whether the current invoking action can run right now, later or never..
/// </summary>
public class Interceptor : BaseInterceptor<object>
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
    public Interceptor(Action action, InterceptorPolicy policy)
        : base(action is null ? null : arg => action(), policy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Interceptor class.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    public Interceptor(Func<Task> action, InterceptorPolicy policy, bool doNotWait = false)
        : base(action is null ? null : arg => action(), policy, doNotWait)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Interceptor class.
    /// </summary>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    public Interceptor(InterceptorPolicy policy, Func<InterceptorEventArgs<object>, Task> action, bool doNotWait = false)
        : base(policy, action, doNotWait)
    {
    }

    /// <summary>
    /// Invokes. But executes only when matches the policy.
    /// </summary>
    /// <returns>true if process succeeded; otherwise, false.</returns>
    public Task<bool> InvokeAsync()
        => InvokeAsync(null);

    /// <summary>
    /// Starts to invokes. But executes only when matches the policy.
    /// </summary>
    public void InvokeBegin()
        => _ = InvokeAsync(null);

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Action Action(Action action, InterceptorPolicy policy)
        => new Interceptor(action, policy).InvokeBegin;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Func<Task> Action(Func<Task> action, InterceptorPolicy policy, bool doNotWait = false)
        => new Interceptor(action, policy, doNotWait).InvokeAsync;

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
    public static Action Debounce(Action action, TimeSpan delay)
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
    public static Action Throttle(Action action, TimeSpan duration)
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
    public static Action Mutliple(Action action, int min, int? max, TimeSpan timeout)
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
    public static Action Times(Action action, int min, int? max, TimeSpan timeout)
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
    public static Action Times(Action action, int count, TimeSpan timeout)
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
    public static Func<Task> Debounce(Func<Task> action, TimeSpan delay)
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
    public static Func<Task> Throttle(Func<Task> action, TimeSpan duration)
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
    public static Func<Task> Mutliple(Func<Task> action, int min, int? max, TimeSpan timeout)
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
    public static Func<Task> Times(Func<Task> action, int min, int? max, TimeSpan timeout)
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
    public static Func<Task> Times(Func<Task> action, int count, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(count, timeout));

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Action<T> Action<T>(Action<T> action, InterceptorPolicy policy)
        => new Interceptor<T>(action, policy).InvokeBegin;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Func<T, Task> Action<T>(Func<T, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        => new Interceptor<T>(action, policy, doNotWait).InvokeAsync;

    /// <summary>
    /// Creates a debounce interceptor policy.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="delay">Delay time span.</param>
    /// <returns>The interceptor policy.</returns>
    /// <remarks>
    /// Maybe a handler will be asked to process several times in a short time
    /// but you just want to process once at the last time because the previous ones are obsolete.
    /// A sample scenario is real-time search.
    /// </remarks>
    public static Action<T> Debounce<T>(Action<T> action, TimeSpan delay)
        => Action(action, InterceptorPolicy.Debounce(delay));

    /// <summary>
    /// Creates a throttle interceptor policy.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="duration">The duration.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// You may want to request to call an action only once in a short time
    /// even if you request to call several times.
    /// The rest will be ignored.
    /// So the handler will be frozen for a while after it has processed.
    /// </remarks>
    public static Action<T> Throttle<T>(Action<T> action, TimeSpan duration)
        => Action(action, InterceptorPolicy.Throttle(duration));

    /// <summary>
    /// Creates a multi-hit interceptor policy.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remark>
    /// The handler to process for the specific times and it will be reset after a while.
    /// </remark>
    public static Action<T> Mutliple<T>(Action<T> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Mutliple(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Action<T> Times<T>(Action<T> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="count">The invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Action<T> Times<T>(Action<T> action, int count, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(count, timeout));

    /// <summary>
    /// Creates a debounce interceptor policy.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="delay">Delay time span.</param>
    /// <returns>The interceptor policy.</returns>
    /// <remarks>
    /// Maybe a handler will be asked to process several times in a short time
    /// but you just want to process once at the last time because the previous ones are obsolete.
    /// A sample scenario is real-time search.
    /// </remarks>
    public static Func<T, Task> Debounce<T>(Func<T, Task> action, TimeSpan delay)
        => Action(action, InterceptorPolicy.Debounce(delay));

    /// <summary>
    /// Creates a throttle interceptor policy.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="duration">The duration.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// You may want to request to call an action only once in a short time
    /// even if you request to call several times.
    /// The rest will be ignored.
    /// So the handler will be frozen for a while after it has processed.
    /// </remarks>
    public static Func<T, Task> Throttle<T>(Func<T, Task> action, TimeSpan duration)
        => Action(action, InterceptorPolicy.Throttle(duration));

    /// <summary>
    /// Creates a multi-hit interceptor policy.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remark>
    /// The handler to process for the specific times and it will be reset after a while.
    /// </remark>
    public static Func<T, Task> Mutliple<T>(Func<T, Task> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Mutliple(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Func<T, Task> Times<T>(Func<T, Task> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <typeparam name="T">The type of action handler argument.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="count">The invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Func<T, Task> Times<T>(Func<T, Task> action, int count, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(count, timeout));

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Action<T1, T2> Action<T1, T2>(Action<T1, T2> action, InterceptorPolicy policy)
        => new Interceptor<T1, T2>(action, policy).InvokeBegin;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Func<T1, T2, Task> Action<T1, T2>(Func<T1, T2, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        => new Interceptor<T1, T2>(action, policy, doNotWait).InvokeAsync;

    /// <summary>
    /// Creates a debounce interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="delay">Delay time span.</param>
    /// <returns>The interceptor policy.</returns>
    /// <remarks>
    /// Maybe a handler will be asked to process several times in a short time
    /// but you just want to process once at the last time because the previous ones are obsolete.
    /// A sample scenario is real-time search.
    /// </remarks>
    public static Action<T1, T2> Debounce<T1, T2>(Action<T1, T2> action, TimeSpan delay)
        => Action(action, InterceptorPolicy.Debounce(delay));

    /// <summary>
    /// Creates a throttle interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="duration">The duration.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// You may want to request to call an action only once in a short time
    /// even if you request to call several times.
    /// The rest will be ignored.
    /// So the handler will be frozen for a while after it has processed.
    /// </remarks>
    public static Action<T1, T2> Throttle<T1, T2>(Action<T1, T2> action, TimeSpan duration)
        => Action(action, InterceptorPolicy.Throttle(duration));

    /// <summary>
    /// Creates a multi-hit interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remark>
    /// The handler to process for the specific times and it will be reset after a while.
    /// </remark>
    public static Action<T1, T2> Mutliple<T1, T2>(Action<T1, T2> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Mutliple(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Action<T1, T2> Times<T1, T2>(Action<T1, T2> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="count">The invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Action<T1, T2> Times<T1, T2>(Action<T1, T2> action, int count, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(count, timeout));

    /// <summary>
    /// Creates a debounce interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="delay">Delay time span.</param>
    /// <returns>The interceptor policy.</returns>
    /// <remarks>
    /// Maybe a handler will be asked to process several times in a short time
    /// but you just want to process once at the last time because the previous ones are obsolete.
    /// A sample scenario is real-time search.
    /// </remarks>
    public static Func<T1, T2, Task> Debounce<T1, T2>(Func<T1, T2, Task> action, TimeSpan delay)
        => Action(action, InterceptorPolicy.Debounce(delay));

    /// <summary>
    /// Creates a throttle interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="duration">The duration.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// You may want to request to call an action only once in a short time
    /// even if you request to call several times.
    /// The rest will be ignored.
    /// So the handler will be frozen for a while after it has processed.
    /// </remarks>
    public static Func<T1, T2, Task> Throttle<T1, T2>(Func<T1, T2, Task> action, TimeSpan duration)
        => Action(action, InterceptorPolicy.Throttle(duration));

    /// <summary>
    /// Creates a multi-hit interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remark>
    /// The handler to process for the specific times and it will be reset after a while.
    /// </remark>
    public static Func<T1, T2, Task> Mutliple<T1, T2>(Func<T1, T2, Task> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Mutliple(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="min">The minmum invoking count.</param>
    /// <param name="max">The maxmum invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Func<T1, T2, Task> Times<T1, T2>(Func<T1, T2, Task> action, int min, int? max, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(min, max, timeout));

    /// <summary>
    /// Creates an interceptor policy responded at a specific times.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="count">The invoking count.</param>
    /// <param name="timeout">The time span between each invoking.</param>
    /// <returns>The action with the specific interceptor policy integration.</returns>
    /// <remarks>
    /// A handler to process at last only when request to call in the specific times range.
    /// A sample scenario is double click.
    /// </remarks>
    public static Func<T1, T2, Task> Times<T1, T2>(Func<T1, T2, Task> action, int count, TimeSpan timeout)
        => Action(action, InterceptorPolicy.Times(count, timeout));

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Action<T1, T2, T3> Action<T1, T2, T3>(Action<T1, T2, T3> action, InterceptorPolicy policy)
        => new Interceptor<T1, T2, T3>(action, policy).InvokeBegin;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Func<T1, T2, T3, Task> Action<T1, T2, T3>(Func<T1, T2, T3, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        => new Interceptor<T1, T2, T3>(action, policy, doNotWait).InvokeAsync;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Action<T1, T2, T3, T4> Action<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, InterceptorPolicy policy)
        => new Interceptor<T1, T2, T3, T4>(action, policy).InvokeBegin;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Func<T1, T2, T3, T4, Task> Action<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        => new Interceptor<T1, T2, T3, T4>(action, policy, doNotWait).InvokeAsync;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Action<T1, T2, T3, T4, T5> Action<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, InterceptorPolicy policy)
        => new Interceptor<T1, T2, T3, T4, T5>(action, policy).InvokeBegin;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Func<T1, T2, T3, T4, T5, Task> Action<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        => new Interceptor<T1, T2, T3, T4, T5>(action, policy, doNotWait).InvokeAsync;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <typeparam name="T6">The type of action handler argument 6.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Action<T1, T2, T3, T4, T5, T6> Action<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, InterceptorPolicy policy)
        => new Interceptor<T1, T2, T3, T4, T5, T6>(action, policy).InvokeBegin;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <typeparam name="T6">The type of action handler argument 6.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Func<T1, T2, T3, T4, T5, T6, Task> Action<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        => new Interceptor<T1, T2, T3, T4, T5, T6>(action, policy, doNotWait).InvokeAsync;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <typeparam name="T6">The type of action handler argument 6.</typeparam>
    /// <typeparam name="T7">The type of action handler argument 7.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Action<T1, T2, T3, T4, T5, T6, T7> Action<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, InterceptorPolicy policy)
        => new Interceptor<T1, T2, T3, T4, T5, T6, T7>(action, policy).InvokeBegin;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <typeparam name="T6">The type of action handler argument 6.</typeparam>
    /// <typeparam name="T7">The type of action handler argument 7.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, Task> Action<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        => new Interceptor<T1, T2, T3, T4, T5, T6, T7>(action, policy, doNotWait).InvokeAsync;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <typeparam name="T6">The type of action handler argument 6.</typeparam>
    /// <typeparam name="T7">The type of action handler argument 7.</typeparam>
    /// <typeparam name="T8">The type of action handler argument 8.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Action<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, InterceptorPolicy policy)
        => new Interceptor<T1, T2, T3, T4, T5, T6, T7, T8>(action, policy).InvokeBegin;

    /// <summary>
    /// Creates an action with interceptor policy.
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <typeparam name="T6">The type of action handler argument 6.</typeparam>
    /// <typeparam name="T7">The type of action handler argument 7.</typeparam>
    /// <typeparam name="T8">The type of action handler argument 8.</typeparam>
    /// <param name="action">The action to register to execute.</param>
    /// <param name="policy">The interceptor policy.</param>
    /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
    /// <returns>The action with interceptor policy integration.</returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> Action<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> action, InterceptorPolicy policy, bool doNotWait = false)
        => new Interceptor<T1, T2, T3, T4, T5, T6, T7, T8>(action, policy, doNotWait).InvokeAsync;
}
