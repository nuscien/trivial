using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Trivial.Tasks;

namespace Trivial.Text;

/// <summary>
/// The states of JSON switch-case.
/// </summary>
public enum JsonSwitchCaseStates : byte
{
    /// <summary>
    /// Pending.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Testing.
    /// </summary>
    Testing = 1,

    /// <summary>
    /// Passed.
    /// </summary>
    Passed = 2,

    /// <summary>
    /// Failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Skipped.
    /// </summary>
    Skipped = 4,

    /// <summary>
    /// Error thrown during testing.
    /// </summary>
    Fatal = 5,
}

/// <summary>
/// The route context of JSON switch-case.
/// </summary>
public class JsonSwitchCaseContext
{
    /// <summary>
    /// The customized model.
    /// </summary>
    private object model;

    /// <summary>
    /// The type allowed to set the customized model.
    /// </summary>
    private Type modelType;

    /// <summary>
    /// Initializes a new instance of the JsonSwitchCaseContext class.
    /// </summary>
    /// <param name="info">The context info.</param>
    internal JsonSwitchCaseContext(IJsonSwitchContextInfo info)
    {
        Switch = info;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is available.
    /// </summary>
    public bool IsAvailable => TaskState == TaskFlowStates.Unknown;

    /// <summary>
    /// Gets the result state.
    /// </summary>
    public JsonSwitchCaseStates State { get; internal set; } = JsonSwitchCaseStates.Pending;

    /// <summary>
    /// Gets the task flow state.
    /// </summary>
    public TaskFlowStates TaskState { get; private set; } = TaskFlowStates.Unknown;

    /// <summary>
    /// Gets the context info bag.
    /// </summary>
    public IJsonSwitchContextInfo Switch { get; private set; }

    /// <summary>
    /// Gets the JSON node source.
    /// </summary>
    public IJsonValueNode Source => Switch?.Source;

    /// <summary>
    /// Gets the JSON value kind of the source.
    /// </summary>
    public JsonValueKind ValueKind => Switch?.Source?.ValueKind ?? JsonValueKind.Undefined;

    /// <summary>
    /// Gets the args.
    /// </summary>
    public object Args => Switch?.Args;

    /// <summary>
    /// Tries to set the argument object into the context.
    /// </summary>
    /// <typeparam name="T">The type of the args.</typeparam>
    /// <param name="value">The argument value.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    public bool TrySetArgs<T>(T value)
    {
        if (Switch is not IJsonSwitchContextInfo<T> context) return false;
        context.Args = value;
        return true;
    }

    internal virtual void Start()
    {
        if (Switch is not null) TaskState = TaskFlowStates.Running;
    }

    internal virtual void End(bool success, JsonSwitchCaseStates? state = null)
    {
        if (state.HasValue) State = state.Value;
        TaskState = success ? TaskFlowStates.Success : TaskFlowStates.Failure;
        Switch = null;
    }

    /// <summary>
    /// Gets the model from the context.
    /// </summary>
    /// <param name="value">The model.</param>
    /// <returns>true if exists; otherwise, false.</returns>
    internal bool GetModel<T>(out T value)
    {
        if (model is not T m)
        {
            value = default;
            return false;
        }

        value = m;
        return true;
    }

    /// <summary>
    /// Gets the model from the context.
    /// </summary>
    /// <param name="value">The model to set.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    internal bool SetModel<T>(T value)
    {
        if (modelType != typeof(T)) return false;
        model = value;
        return true;
    }

    /// <summary>
    /// Initializes the model type.
    /// </summary>
    /// <typeparam name="T">The type of model allowed to set.</typeparam>
    internal void SetModel<T>()
        => modelType = typeof(T);
}

/// <summary>
/// The switch-case handler for JSON node.
/// </summary>
public class JsonSwitchCaseContext<TArgs> : JsonSwitchCaseContext
{
    /// <summary>
    /// Initializes a new instance of the JsonSwitchCaseContext class.
    /// </summary>
    /// <param name="info">The context info.</param>
    internal JsonSwitchCaseContext(IJsonSwitchContextInfo<TArgs> info)
        : base(info)
    {
        Switch = info;
    }

    /// <summary>
    /// Gets the context info bag.
    /// </summary>
    public new IJsonSwitchContextInfo<TArgs> Switch { get; private set; }

    /// <summary>
    /// Gets or sets the args.
    /// </summary>
    public new TArgs Args
    {
        get
        {
            var context = Switch;
            if (context is null) return default;
            return context.Args;
        }

        set
        {
            var context = Switch;
            if (context is null) return;
            context.Args = value;
        }
    }

    internal override void End(bool success, JsonSwitchCaseStates? state = null)
    {
        base.End(success, state);
        Switch = null;
    }
}

/// <summary>
/// The switch-case handler for JSON node.
/// </summary>
public abstract class JsonSwitchCaseContextHandler<TCase> where TCase : JsonSwitchCaseContext
{
    /// <summary>
    /// Initializes a new instance of the InternalJsonSwitchCase class.
    /// </summary>
    internal JsonSwitchCaseContextHandler()
    {
    }

    /// <summary>
    /// Gets a value indicating whether skip the common processing on the condition that the input is matched and is a JSON object.
    /// </summary>
    protected virtual bool SkipCommonProcessForJsonObject => false;

    /// <summary>
    /// Initializes.
    /// </summary>
    /// <param name="context">The route context of JSON switch-case.</param>
    protected virtual void OnInit(TCase context)
    {
    }

    /// <summary>
    /// Tests if the JSON node is matched.
    /// </summary>
    /// <returns>true if passes; otherwise, false.</returns>
    /// <param name="context">The route context of JSON switch-case.</param>
    protected abstract bool Test(TCase context);

    /// <summary>
    /// Executes on matching.
    /// </summary>
    /// <param name="context">The route context of JSON switch-case.</param>
    protected abstract void OnProcess(TCase context);

    /// <summary>
    /// Executes on matching.
    /// </summary>
    /// <param name="json">The JSON object input.</param>
    /// <param name="context">The route context of JSON switch-case.</param>
    protected virtual void OnProcess(JsonObjectNode json, TCase context)
    {
    }

    /// <summary>
    /// Executes during failure testing this time.
    /// </summary>
    /// <param name="context">The route context of JSON switch-case.</param>
    protected virtual void OnFallback(TCase context)
    {
    }

    /// <summary>
    /// Executes during this case is skipped to process.
    /// </summary>
    /// <param name="context">The route context of JSON switch-case.</param>
    protected virtual void OnSkip(TCase context)
    {
    }

    /// <summary>
    /// Cleans up.
    /// </summary>
    /// <param name="context">The route context of JSON switch-case.</param>
    protected virtual void CleanUp(TCase context)
    {
    }

    /// <summary>
    /// Runs before process.
    /// </summary>
    /// <param name="context">The route context of JSON switch-case.</param>
    internal virtual void Prepare(TCase context)
    {
    }

    internal void Process(TCase context, Func<bool, bool> afterTest, Action block)
    {
        if (context?.Switch is null || context.TaskState != TaskFlowStates.Unknown) return;
        context.Start();
        Prepare(context);
        if (context.Switch.IsPassed)
        {
            context.State = JsonSwitchCaseStates.Skipped;
            try
            {
                OnInit(context);
                OnSkip(context);
                CleanUp(context);
                context.End(true);
                return;
            }
            catch (Exception)
            {
                context.End(false);
                throw;
            }
        }

        try
        {
            OnInit(context);
        }
        catch (Exception)
        {
            context.End(false, JsonSwitchCaseStates.Fatal);
            throw;
        }

        context.State = JsonSwitchCaseStates.Testing;
        var b = false;
        try
        {
            b = Test(context);
            context.State = b ? JsonSwitchCaseStates.Passed : JsonSwitchCaseStates.Failed;
        }
        catch (InvalidOperationException)
        {
            context.State = JsonSwitchCaseStates.Failed;
        }
        catch (UnauthorizedAccessException)
        {
            context.State = JsonSwitchCaseStates.Failed;
        }
        catch (NotSupportedException)
        {
            context.State = JsonSwitchCaseStates.Failed;
        }
        catch (ArgumentException)
        {
            context.State = JsonSwitchCaseStates.Failed;
        }
        catch (Exception)
        {
            context.End(false, JsonSwitchCaseStates.Fatal);
            throw;
        }

        try
        {
            afterTest?.Invoke(b);
        }
        catch (Exception)
        {
            context.End(false);
            throw;
        }

        try
        {
            if (b)
            {
                if (context.Source is JsonObjectNode json)
                {
                    if (!SkipCommonProcessForJsonObject) OnProcess(context);
                    OnProcess(json, context);
                }
                else
                {
                    OnProcess(context);
                }
            }
            else
            {
                OnFallback(context);
            }

            CleanUp(context);
            context.End(false);
        }
        catch (Exception)
        {
            context.End(false);
            throw;
        }

        if (b) block?.Invoke();
        return;
    }
}

/// <summary>
/// The switch-case handler for JSON node.
/// </summary>
public abstract class BaseJsonSwitchCase : JsonSwitchCaseContextHandler<JsonSwitchCaseContext>
{
}

/// <summary>
/// The switch-case handler for JSON node.
/// </summary>
public abstract class BaseJsonSwitchCase<TArgs> : JsonSwitchCaseContextHandler<JsonSwitchCaseContext<TArgs>>
{
    /// <summary>
    /// Gets the type of argument object expected.
    /// </summary>
    /// <returns>The type of args.</returns>
    public Type GetArgsType()
        => typeof(TArgs);
}

/// <summary>
/// The switch-case handler for JSON node.
/// </summary>
public abstract class ModelJsonSwitchCase<TModel> : JsonSwitchCaseContextHandler<JsonSwitchCaseContext>
{
    /// <summary>
    /// Gets the model from the context.
    /// </summary>
    /// <param name="context">The current JSON switch-case route context.</param>
    /// <returns>The model.</returns>
    protected TModel GetModel(JsonSwitchCaseContext context)
        => GetModel(context, out var result) ? result : default;

    /// <summary>
    /// Gets the model from the context.
    /// </summary>
    /// <param name="context">The current JSON switch-case route context.</param>
    /// <returns>The model.</returns>
    /// <param name="defaultValue">The default value.</param>
    protected TModel GetModel(JsonSwitchCaseContext context, TModel defaultValue)
        => GetModel(context, out var result) ? result : defaultValue;

    /// <summary>
    /// Gets the model from the context.
    /// </summary>
    /// <param name="context">The current JSON switch-case route context.</param>
    /// <param name="value">The model.</param>
    /// <returns>true if exists; otherwise, false.</returns>
    protected bool GetModel(JsonSwitchCaseContext context, out TModel value)
    {
        if (context is null)
        {
            value = default;
            return false;
        }

        return context.GetModel(out value);
    }

    /// <summary>
    /// Gets the model from the context.
    /// </summary>
    /// <param name="context">The current JSON switch-case route context.</param>
    /// <param name="model">The model to set.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    protected bool SetModel(JsonSwitchCaseContext context, TModel model)
        => context?.SetModel(model) ?? false;

    /// <inheritdoc />
    internal override void Prepare(JsonSwitchCaseContext context)
    {
        context.SetModel<TModel>();
        base.Prepare(context);
    }
}

/// <summary>
/// The switch-case handler for JSON node.
/// </summary>
public abstract class ModelJsonSwitchCase<TArgs, TModel> : JsonSwitchCaseContextHandler<JsonSwitchCaseContext<TArgs>>
{
    /// <summary>
    /// Gets the model from the context.
    /// </summary>
    /// <param name="context">The current JSON switch-case route context.</param>
    /// <returns>The model.</returns>
    protected TModel GetModel(JsonSwitchCaseContext<TArgs> context)
        => GetModel(context, out var result) ? result : default;

    /// <summary>
    /// Gets the model from the context.
    /// </summary>
    /// <param name="context">The current JSON switch-case route context.</param>
    /// <returns>The model.</returns>
    /// <param name="defaultValue">The default value.</param>
    protected TModel GetModel(JsonSwitchCaseContext<TArgs> context, TModel defaultValue)
        => GetModel(context, out var result) ? result : defaultValue;

    /// <summary>
    /// Gets the model from the context.
    /// </summary>
    /// <param name="context">The current JSON switch-case route context.</param>
    /// <param name="value">The model.</param>
    /// <returns>true if exists; otherwise, false.</returns>
    protected bool GetModel(JsonSwitchCaseContext<TArgs> context, out TModel value)
    {
        if (context is null)
        {
            value = default;
            return false;
        }

        return context.GetModel(out value);
    }

    /// <summary>
    /// Gets the model from the context.
    /// </summary>
    /// <param name="context">The current JSON switch-case route context.</param>
    /// <param name="model">The model to set.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    protected bool SetModel(JsonSwitchCaseContext<TArgs> context, TModel model)
        => context?.SetModel(model) ?? false;

    /// <inheritdoc />
    internal override void Prepare(JsonSwitchCaseContext<TArgs> context)
    {
        context.SetModel<TModel>();
        base.Prepare(context);
    }
}
