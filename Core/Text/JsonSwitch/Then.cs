using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The happy path router for JSON switch-case context.
/// </summary>
/// <typeparam name="TNode">The type of JSON node.</typeparam>
/// <typeparam name="TArgs">The type of context args.</typeparam>
/// <typeparam name="TResult">The type of result converted.</typeparam>
public abstract class BaseJsonSwitchThen<TNode, TArgs, TResult> where TNode : IJsonValueNode
{
    /// <summary>
    /// Initializes a new instance of the BaseJsonSwitchThen class.
    /// </summary>
    /// <param name="context">The JSON switch-case context instance.</param>
    internal BaseJsonSwitchThen(JsonSwitchContext<TNode, TArgs> context)
    {
        Context = context;
    }

    /// <summary>
    /// Initializes a new instance of the BaseJsonSwitchThen class.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="context">The JSON switch-case context instance.</param>
    internal BaseJsonSwitchThen(TResult result, JsonSwitchContext<TNode, TArgs> context)
    {
        IsHitted = true;
        Result = result;
        Context = context;
    }

    /// <summary>
    /// Gets a value indicating whether the switch-case route is matched.
    /// </summary>
    public bool IsHitted { get; }

    /// <summary>
    /// Gets the JSON switch-case context instance.
    /// </summary>
    protected JsonSwitchContext<TNode, TArgs> Context { get; }

    /// <summary>
    /// Gets the result; or default if fails.
    /// </summary>
    protected TResult Result { get; }

    /// <summary>
    /// Tries to get the result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>True if get succeeded; otherwise, false.</returns>
    public bool TryGet(out TResult result)
    {
        result = Result;
        return IsHitted;
    }

    /// <summary>
    /// Gets the result.
    /// </summary>
    /// <returns>The result.</returns>
    /// <exception cref="InvalidOperationException">The result does not exist because of failure.</exception>
    public TResult GetResult()
        => IsHitted ? Result : throw new InvalidOperationException("No result because of failure.");

    /// <summary>
    /// Execute the callback handler if the JSON node matches the predicate.
    /// </summary>
    /// <param name="callback">The callback handler.</param>
    /// <returns>The JSON switch context instance.</returns>
    public JsonSwitchContext<TNode, TArgs> Then(Action<TResult> callback = null)
    {
        if (IsHitted) callback?.Invoke(Result);
        return Context;
    }
}

/// <summary>
/// The happy path router for JSON switch-case context.
/// </summary>
/// <typeparam name="TNode">The type of JSON node.</typeparam>
/// <typeparam name="TArgs">The type of context args.</typeparam>
/// <typeparam name="TResult">The type of result converted.</typeparam>
public class JsonSwitchThen<TNode, TArgs, TResult> : BaseJsonSwitchThen<TNode, TArgs, TResult> where TNode : IJsonValueNode
{
    /// <summary>
    /// Initializes a new instance of the JsonSwitchThen class.
    /// </summary>
    /// <param name="context">The JSON switch-case context instance.</param>
    internal JsonSwitchThen(JsonSwitchContext<TNode, TArgs> context)
        : base(context)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonSwitchThen class.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="context">The JSON switch-case context instance.</param>
    internal JsonSwitchThen(TResult result, JsonSwitchContext<TNode, TArgs> context)
        : base(result, context)
    {
    }

    /// <summary>
    /// Execute the callback handler if the JSON node matches the predicate.
    /// </summary>
    /// <param name="callback">The callback handler.</param>
    /// <returns>The JSON switch context instance.</returns>
    public JsonSwitchContext<TNode, TArgs> Then(Action<TResult, JsonSwitchContext<TNode, TArgs>> callback)
    {
        if (IsHitted) callback?.Invoke(Result, Context);
        return Context;
    }

    /// <summary>
    /// Execute the callback handler if the JSON node matches the predicate.
    /// </summary>
    /// <param name="callback">The callback handler.</param>
    /// <returns>The next step with new result or failure.</returns>
    public JsonSwitchThen<TNode, TArgs, TNextResult> Then<TNextResult>(Func<TResult, TNextResult> callback)
    {
        if (IsHitted && callback is not null)
        {
            try
            {
                var result = callback.Invoke(Result);
                return new(result, Context);
            }
            catch (InvalidOperationException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (ArgumentException)
            {
            }

            Context?.KeepAvailable();
        }

        return new(Context);
    }

    /// <summary>
    /// Execute the callback handler if the JSON node matches the predicate.
    /// </summary>
    /// <param name="callback">The callback handler.</param>
    /// <returns>The next step with new result or failure.</returns>
    public JsonSwitchThen<TNode, TArgs, TNextResult> Then<TNextResult>(Func<TResult, JsonSwitchContext<TNode, TArgs>, TNextResult> callback)
    {
        if (IsHitted && callback is not null)
        {
            try
            {
                var result = callback.Invoke(Result, Context);
                return new(result, Context);
            }
            catch (InvalidOperationException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (ArgumentException)
            {
            }

            Context?.KeepAvailable();
        }

        return new(Context);
    }
}

/// <summary>
/// The happy path router for JSON switch-case context.
/// </summary>
/// <typeparam name="TNode">The type of JSON node.</typeparam>
/// <typeparam name="TArgs">The type of context args.</typeparam>
/// <typeparam name="TResult">The type of result converted.</typeparam>
/// <typeparam name="TInfo">The type of additional info to return.</typeparam>
public class JsonSwitchThen<TNode, TArgs, TResult, TInfo> : BaseJsonSwitchThen<TNode, TArgs, TResult> where TNode : IJsonValueNode
{
    /// <summary>
    /// Initializes a new instance of the JsonSwitchThen class.
    /// </summary>
    /// <param name="context">The JSON switch-case context instance.</param>
    internal JsonSwitchThen(JsonSwitchContext<TNode, TArgs> context)
        : base(context)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonSwitchThen class.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="info">The additional info to return.</param>
    /// <param name="context">The JSON switch-case context instance.</param>
    internal JsonSwitchThen(TResult result, TInfo info, JsonSwitchContext<TNode, TArgs> context)
        : base(result, context)
    {
        Info = info;
    }

    /// <summary>
    /// Gets the additional info to return.
    /// </summary>
    protected TInfo Info { get; }

    /// <summary>
    /// Tries to get the result and info.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="info">The additional info to return.</param>
    /// <returns>True if get succeeded; otherwise, false.</returns>
    public bool TryGet(out TResult result, out TInfo info)
    {
        result = Result;
        info = Info;
        return IsHitted;
    }

    /// <summary>
    /// Gets the additional info to return.
    /// </summary>
    /// <returns>The additional info to return.</returns>
    /// <exception cref="InvalidOperationException">The info does not exist because of failure.</exception>
    public TInfo GetInfo()
        => IsHitted ? Info : throw new InvalidOperationException("No info because of failure.");

    /// <summary>
    /// Execute the callback handler if the JSON node matches the predicate.
    /// </summary>
    /// <param name="callback">The callback handler.</param>
    /// <returns>The JSON switch context instance.</returns>
    public JsonSwitchContext<TNode, TArgs> Then(Action<TResult, TInfo> callback)
    {
        if (IsHitted) callback?.Invoke(Result, Info);
        return Context;
    }

    /// <summary>
    /// Execute the callback handler if the JSON node matches the predicate.
    /// </summary>
    /// <param name="callback">The callback handler.</param>
    /// <returns>The JSON switch context instance.</returns>
    public JsonSwitchContext<TNode, TArgs> Then(Action<TResult, TInfo, JsonSwitchContext<TNode, TArgs>> callback)
    {
        if (IsHitted) callback?.Invoke(Result, Info, Context);
        return Context;
    }

    /// <summary>
    /// Execute the callback handler if the JSON node matches the predicate.
    /// </summary>
    /// <param name="callback">The callback handler.</param>
    /// <returns>The next step with new result or failure.</returns>
    public JsonSwitchThen<TNode, TArgs, TNextResult> Then<TNextResult>(Func<TResult, TInfo, TNextResult> callback)
    {
        if (IsHitted && callback is not null)
        {
            try
            {
                var result = callback.Invoke(Result, Info);
                return new(result, Context);
            }
            catch (InvalidOperationException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (ArgumentException)
            {
            }

            Context?.KeepAvailable();
        }

        return new(Context);
    }

    /// <summary>
    /// Execute the callback handler if the JSON node matches the predicate.
    /// </summary>
    /// <param name="callback">The callback handler.</param>
    /// <returns>The next step with new result or failure.</returns>
    public JsonSwitchThen<TNode, TArgs, TNextResult> Then<TNextResult>(Func<TResult, TInfo, JsonSwitchContext<TNode, TArgs>, TNextResult> callback)
    {
        if (IsHitted && callback is not null)
        {
            try
            {
                var result = callback.Invoke(Result, Info, Context);
                return new(result, Context);
            }
            catch (InvalidOperationException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (ArgumentException)
            {
            }

            Context?.KeepAvailable();
        }

        return new(Context);
    }
}
