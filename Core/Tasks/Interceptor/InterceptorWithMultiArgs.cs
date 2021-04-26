using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    /// <summary>
    /// The handler interceptor to determine whether the current invoking action can run right now, later or never..
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    public class Interceptor<T1, T2> : BaseInterceptor<Tuple<T1, T2>>
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
        public Interceptor(Action<T1, T2> action, InterceptorPolicy policy)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2), policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        public Interceptor(Action<Tuple<T1, T2>> action, InterceptorPolicy policy)
            : base(action, policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<T1, T2, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2), policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<Tuple<T1, T2>, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action, policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(InterceptorPolicy policy, Func<InterceptorEventArgs<Tuple<T1, T2>>, Task> action, bool doNotWait = false)
            : base(policy, action, doNotWait)
        {
        }

        /// <summary>
        /// Invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> InvokeAsync(T1 arg1, T2 arg2)
            => InvokeAsync(new Tuple<T1, T2>(arg1, arg2));

        /// <summary>
        /// Starts to invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        public void InvokeBegin(T1 arg1, T2 arg2)
            => _ = InvokeAsync(new Tuple<T1, T2>(arg1, arg2));

        /// <summary>
        /// Creates a function with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <returns>A function to invoke.</returns>
        public Func<Task<bool>> FixArgument(T1 arg1, T2 arg2)
            => () => InvokeAsync(new Tuple<T1, T2>(arg1, arg2));

        /// <summary>
        /// Creates an action with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <returns>An action to invoke.</returns>
        public Action FixArgumentAction(T1 arg1, T2 arg2)
            => () => _ = InvokeAsync(new Tuple<T1, T2>(arg1, arg2));

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<Tuple<T1, T2>> Action(Action<Tuple<T1, T2>> action, InterceptorPolicy policy)
        {
            var interceptor = new Interceptor<T1, T2>(action, policy);
            return arg => _ = interceptor.InvokeAsync(arg);
        }

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<T1, T2> Action(Action<T1, T2> action, InterceptorPolicy policy)
            => new Interceptor<T1, T2>(action, policy).InvokeBegin;

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Func<T1, T2, Task> Action(Func<T1, T2, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            => new Interceptor<T1, T2>(action, policy, doNotWait).InvokeAsync;

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
        public static Action<T1, T2> Debounce(Action<T1, T2> action, TimeSpan delay)
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
        public static Action<T1, T2> Throttle(Action<T1, T2> action, TimeSpan duration)
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
        public static Action<T1, T2> Mutliple(Action<T1, T2> action, int min, int? max, TimeSpan timeout)
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
        public static Action<T1, T2> Times(Action<T1, T2> action, int min, int? max, TimeSpan timeout)
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
        public static Action<T1, T2> Times(Action<T1, T2> action, int count, TimeSpan timeout)
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
        public static Func<T1, T2, Task> Debounce(Func<T1, T2, Task> action, TimeSpan delay)
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
        public static Func<T1, T2, Task> Throttle(Func<T1, T2, Task> action, TimeSpan duration)
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
        public static Func<T1, T2, Task> Mutliple(Func<T1, T2, Task> action, int min, int? max, TimeSpan timeout)
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
        public static Func<T1, T2, Task> Times(Func<T1, T2, Task> action, int min, int? max, TimeSpan timeout)
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
        public static Func<T1, T2, Task> Times(Func<T1, T2, Task> action, int count, TimeSpan timeout)
            => Action(action, InterceptorPolicy.Times(count, timeout));
    }

    /// <summary>
    /// The handler interceptor to determine whether the current invoking action can run right now, later or never..
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    public class Interceptor<T1, T2, T3> : BaseInterceptor<Tuple<T1, T2, T3>>
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
        public Interceptor(Action<T1, T2, T3> action, InterceptorPolicy policy)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3), policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        public Interceptor(Action<Tuple<T1, T2, T3>> action, InterceptorPolicy policy)
            : base(action, policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<T1, T2, T3, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3), policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<Tuple<T1, T2, T3>, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action, policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(InterceptorPolicy policy, Func<InterceptorEventArgs<Tuple<T1, T2, T3>>, Task> action, bool doNotWait = false)
            : base(policy, action, doNotWait)
        {
        }

        /// <summary>
        /// Invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> InvokeAsync(T1 arg1, T2 arg2, T3 arg3)
            => InvokeAsync(new Tuple<T1, T2, T3>(arg1, arg2, arg3));

        /// <summary>
        /// Starts to invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        public void InvokeBegin(T1 arg1, T2 arg2, T3 arg3)
            => _ = InvokeAsync(new Tuple<T1, T2, T3>(arg1, arg2, arg3));

        /// <summary>
        /// Creates a function with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <returns>A function to invoke.</returns>
        public Func<Task<bool>> FixArgument(T1 arg1, T2 arg2, T3 arg3)
            => () => InvokeAsync(new Tuple<T1, T2, T3>(arg1, arg2, arg3));

        /// <summary>
        /// Creates an action with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <returns>An action to invoke.</returns>
        public Action FixArgumentAction(T1 arg1, T2 arg2, T3 arg3)
            => () => _ = InvokeAsync(new Tuple<T1, T2, T3>(arg1, arg2, arg3));

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<Tuple<T1, T2, T3>> Action(Action<Tuple<T1, T2, T3>> action, InterceptorPolicy policy)
        {
            var interceptor = new Interceptor<T1, T2, T3>(action, policy);
            return arg => _ = interceptor.InvokeAsync(arg);
        }

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<T1, T2, T3> Action(Action<T1, T2, T3> action, InterceptorPolicy policy)
            => new Interceptor<T1, T2, T3>(action, policy).InvokeBegin;

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Func<T1, T2, T3, Task> Action(Func<T1, T2, T3, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            => new Interceptor<T1, T2, T3>(action, policy, doNotWait).InvokeAsync;

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
        public static Action<T1, T2, T3> Debounce(Action<T1, T2, T3> action, TimeSpan delay)
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
        public static Action<T1, T2, T3> Throttle(Action<T1, T2, T3> action, TimeSpan duration)
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
        public static Action<T1, T2, T3> Mutliple(Action<T1, T2, T3> action, int min, int? max, TimeSpan timeout)
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
        public static Action<T1, T2, T3> Times(Action<T1, T2, T3> action, int min, int? max, TimeSpan timeout)
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
        public static Action<T1, T2, T3> Times(Action<T1, T2, T3> action, int count, TimeSpan timeout)
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
        public static Func<T1, T2, T3, Task> Debounce(Func<T1, T2, T3, Task> action, TimeSpan delay)
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
        public static Func<T1, T2, T3, Task> Throttle(Func<T1, T2, T3, Task> action, TimeSpan duration)
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
        public static Func<T1, T2, T3, Task> Mutliple(Func<T1, T2, T3, Task> action, int min, int? max, TimeSpan timeout)
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
        public static Func<T1, T2, T3, Task> Times(Func<T1, T2, T3, Task> action, int min, int? max, TimeSpan timeout)
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
        public static Func<T1, T2, T3, Task> Times(Func<T1, T2, T3, Task> action, int count, TimeSpan timeout)
            => Action(action, InterceptorPolicy.Times(count, timeout));
    }

    /// <summary>
    /// The handler interceptor to determine whether the current invoking action can run right now, later or never..
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    public class Interceptor<T1, T2, T3, T4> : BaseInterceptor<Tuple<T1, T2, T3, T4>>
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
        public Interceptor(Action<T1, T2, T3, T4> action, InterceptorPolicy policy)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3, arg.Item4), policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        public Interceptor(Action<Tuple<T1, T2, T3, T4>> action, InterceptorPolicy policy)
            : base(action, policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<T1, T2, T3, T4, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3, arg.Item4), policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<Tuple<T1, T2, T3, T4>, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action, policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(InterceptorPolicy policy, Func<InterceptorEventArgs<Tuple<T1, T2, T3, T4>>, Task> action, bool doNotWait = false)
            : base(policy, action, doNotWait)
        {
        }

        /// <summary>
        /// Invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> InvokeAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            => InvokeAsync(new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4));

        /// <summary>
        /// Starts to invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        public void InvokeBegin(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            => _ = InvokeAsync(new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4));

        /// <summary>
        /// Creates a function with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <returns>A function to invoke.</returns>
        public Func<Task<bool>> FixArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            => () => InvokeAsync(new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4));

        /// <summary>
        /// Creates an action with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <returns>An action to invoke.</returns>
        public Action FixArgumentAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            => () => _ = InvokeAsync(new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4));

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<Tuple<T1, T2, T3, T4>> Action(Action<Tuple<T1, T2, T3, T4>> action, InterceptorPolicy policy)
        {
            var interceptor = new Interceptor<T1, T2, T3, T4>(action, policy);
            return arg => _ = interceptor.InvokeAsync(arg);
        }

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<T1, T2, T3, T4> Action(Action<T1, T2, T3, T4> action, InterceptorPolicy policy)
            => new Interceptor<T1, T2, T3, T4>(action, policy).InvokeBegin;

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Func<T1, T2, T3, T4, Task> Action(Func<T1, T2, T3, T4, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            => new Interceptor<T1, T2, T3, T4>(action, policy, doNotWait).InvokeAsync;

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
        public static Action<T1, T2, T3, T4> Debounce(Action<T1, T2, T3, T4> action, TimeSpan delay)
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
        public static Action<T1, T2, T3, T4> Throttle(Action<T1, T2, T3, T4> action, TimeSpan duration)
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
        public static Action<T1, T2, T3, T4> Mutliple(Action<T1, T2, T3, T4> action, int min, int? max, TimeSpan timeout)
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
        public static Action<T1, T2, T3, T4> Times(Action<T1, T2, T3, T4> action, int min, int? max, TimeSpan timeout)
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
        public static Action<T1, T2, T3, T4> Times(Action<T1, T2, T3, T4> action, int count, TimeSpan timeout)
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
        public static Func<T1, T2, T3, T4, Task> Debounce(Func<T1, T2, T3, T4, Task> action, TimeSpan delay)
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
        public static Func<T1, T2, T3, T4, Task> Throttle(Func<T1, T2, T3, T4, Task> action, TimeSpan duration)
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
        public static Func<T1, T2, T3, T4, Task> Mutliple(Func<T1, T2, T3, T4, Task> action, int min, int? max, TimeSpan timeout)
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
        public static Func<T1, T2, T3, T4, Task> Times(Func<T1, T2, T3, T4, Task> action, int min, int? max, TimeSpan timeout)
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
        public static Func<T1, T2, T3, T4, Task> Times(Func<T1, T2, T3, T4, Task> action, int count, TimeSpan timeout)
            => Action(action, InterceptorPolicy.Times(count, timeout));
    }

    /// <summary>
    /// The handler interceptor to determine whether the current invoking action can run right now, later or never..
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    public class Interceptor<T1, T2, T3, T4, T5> : BaseInterceptor<Tuple<T1, T2, T3, T4, T5>>
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
        public Interceptor(Action<T1, T2, T3, T4, T5> action, InterceptorPolicy policy)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3, arg.Item4, arg.Item5), policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        public Interceptor(Action<Tuple<T1, T2, T3, T4, T5>> action, InterceptorPolicy policy)
            : base(action, policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<T1, T2, T3, T4, T5, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3, arg.Item4, arg.Item5), policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<Tuple<T1, T2, T3, T4, T5>, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action, policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(InterceptorPolicy policy, Func<InterceptorEventArgs<Tuple<T1, T2, T3, T4, T5>>, Task> action, bool doNotWait = false)
            : base(policy, action, doNotWait)
        {
        }

        /// <summary>
        /// Invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> InvokeAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            => InvokeAsync(new Tuple<T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5));

        /// <summary>
        /// Starts to invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        public void InvokeBegin(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            => _ = InvokeAsync(new Tuple<T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5));

        /// <summary>
        /// Creates a function with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <returns>A function to invoke.</returns>
        public Func<Task<bool>> FixArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            => () => InvokeAsync(new Tuple<T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5));

        /// <summary>
        /// Creates an action with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <returns>An action to invoke.</returns>
        public Action FixArgumentAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            => () => _ = InvokeAsync(new Tuple<T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5));

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<Tuple<T1, T2, T3, T4, T5>> Action(Action<Tuple<T1, T2, T3, T4, T5>> action, InterceptorPolicy policy)
        {
            var interceptor = new Interceptor<T1, T2, T3, T4, T5>(action, policy);
            return arg => _ = interceptor.InvokeAsync(arg);
        }

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<T1, T2, T3, T4, T5> Action(Action<T1, T2, T3, T4, T5> action, InterceptorPolicy policy)
            => new Interceptor<T1, T2, T3, T4, T5>(action, policy).InvokeBegin;

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Func<T1, T2, T3, T4, T5, Task> Action(Func<T1, T2, T3, T4, T5, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            => new Interceptor<T1, T2, T3, T4, T5>(action, policy, doNotWait).InvokeAsync;
    }

    /// <summary>
    /// The handler interceptor to determine whether the current invoking action can run right now, later or never..
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <typeparam name="T6">The type of action handler argument 6.</typeparam>
    public class Interceptor<T1, T2, T3, T4, T5, T6> : BaseInterceptor<Tuple<T1, T2, T3, T4, T5, T6>>
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
        public Interceptor(Action<T1, T2, T3, T4, T5, T6> action, InterceptorPolicy policy)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3, arg.Item4, arg.Item5, arg.Item6), policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        public Interceptor(Action<Tuple<T1, T2, T3, T4, T5, T6>> action, InterceptorPolicy policy)
            : base(action, policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<T1, T2, T3, T4, T5, T6, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3, arg.Item4, arg.Item5, arg.Item6), policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<Tuple<T1, T2, T3, T4, T5, T6>, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action, policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(InterceptorPolicy policy, Func<InterceptorEventArgs<Tuple<T1, T2, T3, T4, T5, T6>>, Task> action, bool doNotWait = false)
            : base(policy, action, doNotWait)
        {
        }

        /// <summary>
        /// Invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> InvokeAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            => InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6>(arg1, arg2, arg3, arg4, arg5, arg6));

        /// <summary>
        /// Starts to invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        public void InvokeBegin(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            => _ = InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6>(arg1, arg2, arg3, arg4, arg5, arg6));

        /// <summary>
        /// Creates a function with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <returns>A function to invoke.</returns>
        public Func<Task<bool>> FixArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            => () => InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6>(arg1, arg2, arg3, arg4, arg5, arg6));

        /// <summary>
        /// Creates an action with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <returns>An action to invoke.</returns>
        public Action FixArgumentAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            => () => _ = InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6>(arg1, arg2, arg3, arg4, arg5, arg6));

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<Tuple<T1, T2, T3, T4, T5, T6>> Action(Action<Tuple<T1, T2, T3, T4, T5, T6>> action, InterceptorPolicy policy)
        {
            var interceptor = new Interceptor<T1, T2, T3, T4, T5, T6>(action, policy);
            return arg => _ = interceptor.InvokeAsync(arg);
        }

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<T1, T2, T3, T4, T5, T6> Action(Action<T1, T2, T3, T4, T5, T6> action, InterceptorPolicy policy)
            => new Interceptor<T1, T2, T3, T4, T5, T6>(action, policy).InvokeBegin;

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, Task> Action(Func<T1, T2, T3, T4, T5, T6, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            => new Interceptor<T1, T2, T3, T4, T5, T6>(action, policy, doNotWait).InvokeAsync;
    }

    /// <summary>
    /// The handler interceptor to determine whether the current invoking action can run right now, later or never..
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <typeparam name="T6">The type of action handler argument 6.</typeparam>
    /// <typeparam name="T7">The type of action handler argument 7.</typeparam>
    public class Interceptor<T1, T2, T3, T4, T5, T6, T7> : BaseInterceptor<Tuple<T1, T2, T3, T4, T5, T6, T7>>
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
        public Interceptor(Action<T1, T2, T3, T4, T5, T6, T7> action, InterceptorPolicy policy)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3, arg.Item4, arg.Item5, arg.Item6, arg.Item7), policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        public Interceptor(Action<Tuple<T1, T2, T3, T4, T5, T6, T7>> action, InterceptorPolicy policy)
            : base(action, policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<T1, T2, T3, T4, T5, T6, T7, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3, arg.Item4, arg.Item5, arg.Item6, arg.Item7), policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<Tuple<T1, T2, T3, T4, T5, T6, T7>, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action, policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(InterceptorPolicy policy, Func<InterceptorEventArgs<Tuple<T1, T2, T3, T4, T5, T6, T7>>, Task> action, bool doNotWait = false)
            : base(policy, action, doNotWait)
        {
        }

        /// <summary>
        /// Invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task<bool> InvokeAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            => InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6, T7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7));

        /// <summary>
        /// Starts to invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        public void InvokeBegin(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            => _ = InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6, T7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7));

        /// <summary>
        /// Creates a function with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        /// <returns>A function to invoke.</returns>
        public Func<Task<bool>> FixArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            => () => InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6, T7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7));

        /// <summary>
        /// Creates an action with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        /// <returns>An action to invoke.</returns>
        public Action FixArgumentAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            => () => _ = InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6, T7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7));

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<Tuple<T1, T2, T3, T4, T5, T6, T7>> Action(Action<Tuple<T1, T2, T3, T4, T5, T6, T7>> action, InterceptorPolicy policy)
        {
            var interceptor = new Interceptor<T1, T2, T3, T4, T5, T6, T7>(action, policy);
            return arg => _ = interceptor.InvokeAsync(arg);
        }

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7> Action(Action<T1, T2, T3, T4, T5, T6, T7> action, InterceptorPolicy policy)
            => new Interceptor<T1, T2, T3, T4, T5, T6, T7>(action, policy).InvokeBegin;

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, Task> Action(Func<T1, T2, T3, T4, T5, T6, T7, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            => new Interceptor<T1, T2, T3, T4, T5, T6, T7>(action, policy, doNotWait).InvokeAsync;
    }

    /// <summary>
    /// The handler interceptor to determine whether the current invoking action can run right now, later or never..
    /// </summary>
    /// <typeparam name="T1">The type of action handler argument 1.</typeparam>
    /// <typeparam name="T2">The type of action handler argument 2.</typeparam>
    /// <typeparam name="T3">The type of action handler argument 3.</typeparam>
    /// <typeparam name="T4">The type of action handler argument 4.</typeparam>
    /// <typeparam name="T5">The type of action handler argument 5.</typeparam>
    /// <typeparam name="T6">The type of action handler argument 6.</typeparam>
    /// <typeparam name="T7">The type of action handler argument 7.</typeparam>
    /// <typeparam name="T8">The type of action handler argument 8.</typeparam>
    public class Interceptor<T1, T2, T3, T4, T5, T6, T7, T8> : BaseInterceptor<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>>
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
        public Interceptor(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, InterceptorPolicy policy)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3, arg.Item4, arg.Item5, arg.Item6, arg.Item7, arg.Rest), policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        public Interceptor(Action<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> action, InterceptorPolicy policy)
            : base(action, policy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action is null ? null : arg => action(arg.Item1, arg.Item2, arg.Item3, arg.Item4, arg.Item5, arg.Item6, arg.Item7, arg.Rest), policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(Func<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            : base(action, policy, doNotWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Interceptor class.
        /// </summary>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        public Interceptor(InterceptorPolicy policy, Func<InterceptorEventArgs<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>>, Task> action, bool doNotWait = false)
            : base(policy, action, doNotWait)
        {
        }

        /// <summary>
        /// Invokes. But executes only when matches the policy.
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
        public Task<bool> InvokeAsync(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
            => InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));

        /// <summary>
        /// Starts to invokes. But executes only when matches the policy.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        /// <param name="arg8">The argument 8.</param>
        public void InvokeBegin(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
            => _ = InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));

        /// <summary>
        /// Creates a function with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        /// <param name="arg8">The argument 8.</param>
        /// <returns>A function to invoke.</returns>
        public Func<Task<bool>> FixArgument(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
            => () => InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));

        /// <summary>
        /// Creates an action with fixed argument.
        /// </summary>
        /// <param name="arg1">The argument 1.</param>
        /// <param name="arg2">The argument 2.</param>
        /// <param name="arg3">The argument 3.</param>
        /// <param name="arg4">The argument 4.</param>
        /// <param name="arg5">The argument 5.</param>
        /// <param name="arg6">The argument 6.</param>
        /// <param name="arg7">The argument 7.</param>
        /// <param name="arg8">The argument 8.</param>
        /// <returns>An action to invoke.</returns>
        public Action FixArgumentAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
            => () => _ = InvokeAsync(new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> Action(Action<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> action, InterceptorPolicy policy)
        {
            var interceptor = new Interceptor<T1, T2, T3, T4, T5, T6, T7, T8>(action, policy);
            return arg => _ = interceptor.InvokeAsync(arg);
        }

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Action(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, InterceptorPolicy policy)
            => new Interceptor<T1, T2, T3, T4, T5, T6, T7, T8>(action, policy).InvokeBegin;

        /// <summary>
        /// Creates an action with interceptor policy.
        /// </summary>
        /// <param name="action">The action to register to execute.</param>
        /// <param name="policy">The interceptor policy.</param>
        /// <param name="doNotWait">true if do not wait for the action result; otherwise, false.</param>
        /// <returns>The action with interceptor policy integration.</returns>
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> Action(Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> action, InterceptorPolicy policy, bool doNotWait = false)
            => new Interceptor<T1, T2, T3, T4, T5, T6, T7, T8>(action, policy, doNotWait).InvokeAsync;
    }
}
