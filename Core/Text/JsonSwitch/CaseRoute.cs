using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Trivial.Tasks;

namespace Trivial.Text;

/// <summary>
/// The states of JSON switch case.
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
/// The switch-case handler for JSON node.
/// </summary>
public abstract class BaseJsonSwitchCase
{
    /// <summary>
    /// Gets a value indicating whether this instance is available.
    /// </summary>
    public bool IsAvailable => TaskState == TaskFlowStates.Unknown;

    /// <summary>
    /// Gets the result state.
    /// </summary>
    public JsonSwitchCaseStates State { get; private set; } = JsonSwitchCaseStates.Pending;

    /// <summary>
    /// Gets the task flow state.
    /// </summary>
    public TaskFlowStates TaskState { get; private set; } = TaskFlowStates.Unknown;

    /// <summary>
    /// Gets the context info bag.
    /// </summary>
    protected IJsonSwitchContextInfo ContextInfo { get; private set; }

    /// <summary>
    /// Gets the JSON node source.
    /// </summary>
    protected IJsonValueNode Source => ContextInfo?.Source;

    /// <summary>
    /// Gets the JSON value kind of the source.
    /// </summary>
    protected JsonValueKind ValueKind => ContextInfo?.Source?.ValueKind ?? JsonValueKind.Undefined;

    /// <summary>
    /// Gets the args.
    /// </summary>
    protected object Args => ContextInfo?.Args;

    /// <summary>
    /// Initializes.
    /// </summary>
    protected virtual void OnInit()
    {
    }

    /// <summary>
    /// Tests if the JSON node is matched.
    /// </summary>
    /// <returns></returns>
    protected abstract bool Test();

    /// <summary>
    /// Executes on matching.
    /// </summary>
    protected abstract void OnProcess();

    /// <summary>
    /// Executes during failure testing this time.
    /// </summary>
    protected virtual void OnFallback()
    {
    }

    /// <summary>
    /// Executes during this case is skipped to process.
    /// </summary>
    protected virtual void OnSkip()
    {
    }

    /// <summary>
    /// Cleans up.
    /// </summary>
    protected virtual void CleanUp()
    {
    }

    /// <summary>
    /// Tries to set the argument object into the context.
    /// </summary>
    /// <typeparam name="T">The type of the args.</typeparam>
    /// <param name="value">The argument value.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    protected bool TrySetArgs<T>(T value)
    {
        if (ContextInfo is not IJsonSwitchContextInfo<T> context) return false;
        context.Args = value;
        return true;
    }

    internal virtual bool ForContextArgs<T>()
        => true;

    internal void Process(bool skip, IJsonSwitchContextInfo info, Func<bool, bool> afterTest, Action block)
    {
        if (TaskState != TaskFlowStates.Unknown) return;
        TaskState = TaskFlowStates.Running;
        try
        {
            ContextInfo = info;
            if (skip)
            {
                State = JsonSwitchCaseStates.Skipped;
                try
                {
                    OnInit();
                    OnSkip();
                    CleanUp();
                    TaskState = TaskFlowStates.Success;
                }
                catch (Exception)
                {
                    TaskState = TaskFlowStates.Failure;
                    throw;
                }

                return;
            }

            try
            {
                OnInit();
            }
            catch (Exception)
            {
                State = JsonSwitchCaseStates.Fatal;
                TaskState = TaskFlowStates.Failure;
                throw;
            }

            State = JsonSwitchCaseStates.Testing;
            var b = false;
            try
            {
                b = Test();
                State = b ? JsonSwitchCaseStates.Passed : JsonSwitchCaseStates.Failed;
            }
            catch (InvalidOperationException)
            {
                State = JsonSwitchCaseStates.Failed;
            }
            catch (UnauthorizedAccessException)
            {
                State = JsonSwitchCaseStates.Failed;
            }
            catch (Exception)
            {
                State = JsonSwitchCaseStates.Fatal;
                TaskState = TaskFlowStates.Failure;
                throw;
            }

            try
            {
                afterTest?.Invoke(b);
            }
            catch (Exception)
            {
                TaskState = TaskFlowStates.Failure;
                throw;
            }

            try
            {
                if (b) OnProcess();
                else OnFallback();
                CleanUp();
                TaskState = TaskFlowStates.Success;
            }
            catch (Exception)
            {
                TaskState = TaskFlowStates.Failure;
                throw;
            }

            if (b) block?.Invoke();
            return;
        }
        finally
        {
            ContextInfo = null;
        }
    }
}

/// <summary>
/// The switch-case handler for JSON node.
/// </summary>
public abstract class BaseJsonSwitchCase<T> : BaseJsonSwitchCase
{
    /// <summary>
    /// Gets or sets the args.
    /// </summary>
    protected new T Args
    {
        get
        {
            return ContextInfo is IJsonSwitchContextInfo<T> context ? context.Args : default;
        }

        set
        {
            if (ContextInfo is not IJsonSwitchContextInfo<T> context) return;
            context.Args = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this execution is compatible the type of context args is not expected and the context args does not support to set.
    /// </summary>
    protected virtual bool IsArgsSetterCompatible => false;

    /// <summary>
    /// Gets a value indicating whether the type of context is the expected one.
    /// </summary>
    public bool IsExpectContextType => ContextInfo is IJsonSwitchContextInfo<T>;

    /// <summary>
    /// Gets the type of argument object expected.
    /// </summary>
    /// <returns>The type of args.</returns>
    public Type GetArgsType()
        => typeof(T);

    internal override bool ForContextArgs<TArgs>()
        => IsArgsSetterCompatible || typeof(TArgs) == typeof(T);
}
