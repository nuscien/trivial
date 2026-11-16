using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// Observes an asynchronous task and provides success handlers, exception handlers, and subsequent flows.
/// </summary>
/// <typeparam name="T">The type of the task result.</typeparam>
/// <remarks>
/// Observation starts during construction. Success and exception handlers do not change this flow's result or state.
/// Fluent registration methods return this flow without waiting for their handlers; use their asynchronous
/// counterparts to await handlers and observe handler exceptions. Use <c>Then</c> to create a dependent flow.
/// </remarks>
public class TaskFlow<T>
{
    private readonly Task t;

    private readonly Task<T> t2;

    private Exception exception;

    private T result;

    /// <summary>
    /// Initializes a new instance of the TaskFlow class.
    /// </summary>
    /// <param name="task">The task to observe. Supply a non-null task.</param>
    /// <remarks>Does not start a task created with the <see cref="TaskStatus.Created"/> status.</remarks>
    public TaskFlow(Task<T> task)
    {
        t2 = task;
        t = OnInit();
    }

    /// <summary>
    /// Initializes a new instance of the TaskFlow class by invoking a task factory immediately.
    /// </summary>
    /// <param name="task">The factory to invoke once. It must return a non-null task.</param>
    /// <remarks>Exceptions thrown by the factory before returning a task escape the constructor.</remarks>
    public TaskFlow(Func<Task<T>> task)
    {
        t2 = task();
        t = OnInit();
    }

    /// <summary>
    /// Gets the observed state. Canceled tasks are recorded as <see cref="TaskFlowStates.Failure"/>.
    /// </summary>
    public TaskFlowStates State { get; private set; }

    /// <summary>
    /// Gets the underlying task's status, or <see cref="TaskStatus.Created"/> if no task was supplied.
    /// </summary>
    /// <remarks>The task may complete before the flow finishes observing it. Await <see cref="WaitAsync"/> to synchronize.</remarks>
    public TaskStatus TaskStatus => t2?.Status ?? TaskStatus.Created;

    /// <summary>
    /// Gets whether the flow has observed success, failure, or cancellation, independently of registered handlers.
    /// </summary>
    public bool IsCompleted => State == TaskFlowStates.Success || State == TaskFlowStates.Failure;

    /// <summary>
    /// Gets whether the flow has observed successful completion of the underlying task.
    /// </summary>
    public bool IsSuccessful => State == TaskFlowStates.Success;

    /// <summary>
    /// Gets whether the observed exception is a <see cref="PreviousTaskException"/> whose non-null source is not this flow.
    /// </summary>
    /// <remarks>Propagated cancellation is not wrapped and does not set this flag.</remarks>
    public bool IsExceptionThrownByPrevious => exception != null && exception is PreviousTaskException ex && ex.SourceObject != null && ex.SourceObject != this;

    /// <summary>
    /// Gets whether the underlying task was canceled, rather than faulted with an <see cref="OperationCanceledException"/>.
    /// </summary>
    public bool IsCanceled => t2?.IsCanceled == true;

    /// <summary>
    /// Registers a handler to receive the result only when this flow succeeds.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    /// <returns>This flow, not a flow representing the handler.</returns>
    /// <remarks>Does not await the handler or expose its exceptions. Use <see cref="ResultToAsync(Action{T})"/> to observe completion.</remarks>
    public TaskFlow<T> ResultTo(Action<T> handler)
    {
        _ = ResultToAsync(handler);
        return this;
    }

    /// <summary>
    /// Registers an asynchronous handler to receive the result only when this flow succeeds.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    /// <returns>This flow, not a flow representing the handler.</returns>
    /// <remarks>Does not await the handler or expose its exceptions. Use <see cref="ResultToAsync(Func{T, Task})"/> to observe completion.</remarks>
    public TaskFlow<T> ResultTo(Func<T, Task> handler)
    {
        _ = ResultToAsync(handler);
        return this;
    }

    /// <summary>
    /// Registers a handler for a successful, non-null result assignable to the specified type.
    /// </summary>
    /// <typeparam name="TResult">The expected runtime result type.</typeparam>
    /// <param name="handler">The handler to register.</param>
    /// <returns>This flow.</returns>
    /// <remarks>Null and incompatible results are skipped. The handler is not awaited and its exceptions do not change this flow.</remarks>
    public virtual TaskFlow<T> ResultTo<TResult>(Action<TResult> handler)
    {
        _ = ResultToAsync(r =>
        {
            if (r is TResult arg) handler(arg);
        });
        return this;
    }

    /// <summary>
    /// Registers an asynchronous handler for a successful, non-null result assignable to the specified type.
    /// </summary>
    /// <typeparam name="TResult">The expected runtime result type.</typeparam>
    /// <param name="handler">The handler to register.</param>
    /// <returns>This flow.</returns>
    /// <remarks>Null and incompatible results are skipped. The handler is not awaited and its exceptions do not change this flow.</remarks>
    public virtual TaskFlow<T> ResultTo<TResult>(Func<TResult, Task> handler)
    {
        _ = ResultToAsync(async r =>
        {
            if (r is TResult arg) await handler(arg);
        });
        return this;
    }

    /// <summary>
    /// Registers a handler for an observed exception, including cancellation, that matches the specified type.
    /// </summary>
    /// <typeparam name="TException">The exception type to match.</typeparam>
    /// <param name="errorHandling">The handler to register.</param>
    /// <param name="exactType">true to require the exact runtime type; false to also match derived types.</param>
    /// <returns>This flow.</returns>
    /// <remarks>Does not recover the flow or consume the exception. Use <c>CatchAsync</c> to await the handler and observe its exceptions.</remarks>
    public TaskFlow<T> Catch<TException>(Action<TException> errorHandling, bool exactType = false) where TException : Exception
    {
        _ = CatchAsync(errorHandling, exactType);
        return this;
    }

    /// <summary>
    /// Registers an asynchronous handler for an observed exception, including cancellation, that matches the specified type.
    /// </summary>
    /// <typeparam name="TException">The exception type to match.</typeparam>
    /// <param name="errorHandling">The handler to register.</param>
    /// <param name="exactType">true to require the exact runtime type; false to also match derived types.</param>
    /// <returns>This flow.</returns>
    /// <remarks>Does not recover the flow or consume the exception. Use <c>CatchAsync</c> to await the handler and observe its exceptions.</remarks>
    public TaskFlow<T> Catch<TException>(Func<TException, Task> errorHandling, bool exactType = false) where TException : Exception
    {
        _ = CatchAsync(errorHandling, exactType);
        return this;
    }

    /// <summary>
    /// Waits until this flow has observed the underlying task's completion, without waiting for registered handlers.
    /// </summary>
    /// <param name="ignoreException">true to suppress the observed failure or cancellation; false to rethrow it.</param>
    /// <returns>true on success; false on failure or cancellation when exceptions are suppressed, or if no task was supplied.</returns>
    /// <remarks>Exception handlers do not prevent this method from rethrowing the underlying exception.</remarks>
    public async Task<bool> WaitAsync(bool ignoreException = false)
    {
        await t;
        if (ignoreException || exception == null) return State == TaskFlowStates.Success;
        if (t2 != null) await t2;
        throw exception;
    }

    /// <summary>
    /// Gets the original task, preserving its result, exception, and cancellation behavior.
    /// </summary>
    /// <returns>The underlying task, or null if no task was supplied.</returns>
    /// <remarks>Awaiting this task does not wait for registered handlers or guarantee that <see cref="State"/> has been updated.</remarks>
    public Task<T> ResultAsync()
       => t2;

    /// <summary>
    /// Waits for this flow and invokes a handler only on success.
    /// </summary>
    /// <param name="handler">The handler to invoke.</param>
    /// <returns>true after the handler completes; false if this flow failed or was canceled.</returns>
    /// <remarks>Handler exceptions fault the returned task without changing this flow. Underlying task failures are not rethrown.</remarks>
    public async Task<bool> ResultToAsync(Action<T> handler)
    {
        await t;
        if (State != TaskFlowStates.Success) return false;
        handler(result);
        return true;
    }

    /// <summary>
    /// Waits for this flow and awaits an asynchronous handler only on success.
    /// </summary>
    /// <param name="handler">The handler to invoke.</param>
    /// <returns>true after the handler completes; false if this flow failed or was canceled.</returns>
    /// <remarks>Handler exceptions fault the returned task without changing this flow. Underlying task failures are not rethrown.</remarks>
    public async Task<bool> ResultToAsync(Func<T, Task> handler)
    {
        await t;
        if (State != TaskFlowStates.Success) return false;
        await handler(result);
        return true;
    }

    /// <summary>
    /// Waits for this flow and invokes a handler if the observed exception matches the specified type.
    /// </summary>
    /// <typeparam name="TException">The exception type to match.</typeparam>
    /// <param name="errorHandling">The handler to invoke.</param>
    /// <param name="exactType">true to require the exact runtime type; false to also match derived types.</param>
    /// <returns>true after the matching handler completes; false on success or an exception type mismatch.</returns>
    /// <remarks>Matches the outer exception, not inner exceptions. Handler exceptions propagate without changing this flow or consuming its failure.</remarks>
    public async Task<bool> CatchAsync<TException>(Action<TException> errorHandling, bool exactType = false) where TException : Exception
    {
        await t;
        if (exception == null || exception is not TException ex) return false;
        if (exactType && ex.GetType() != typeof(TException)) return false;
        errorHandling(ex);
        return true;
    }

    /// <summary>
    /// Waits for this flow and awaits a handler if the observed exception matches the specified type.
    /// </summary>
    /// <typeparam name="TException">The exception type to match.</typeparam>
    /// <param name="errorHandling">The handler to invoke.</param>
    /// <param name="exactType">true to require the exact runtime type; false to also match derived types.</param>
    /// <returns>true after the matching handler completes; false on success or an exception type mismatch.</returns>
    /// <remarks>Matches the outer exception, not inner exceptions. Handler exceptions propagate without changing this flow or consuming its failure.</remarks>
    public async Task<bool> CatchAsync<TException>(Func<TException, Task> errorHandling, bool exactType = false) where TException : Exception
    {
        await t;
        if (exception == null || exception is not TException ex) return false;
        if (exactType && ex.GetType() != typeof(TException)) return false;
        await errorHandling(ex);
        return true;
    }

    /// <summary>
    /// Creates a dependent flow by transforming this flow's successful result.
    /// </summary>
    /// <typeparam name="TResult">The result type of the new flow.</typeparam>
    /// <param name="handler">The transformation to invoke only on success.</param>
    /// <returns>A new flow representing the transformation.</returns>
    /// <remarks>Predecessor failures are wrapped in <see cref="PreviousTaskException"/>; cancellation propagates without wrapping.
    /// Exceptions from the handler itself belong to the new flow and are not wrapped.</remarks>
    public TaskFlow<TResult> Then<TResult>(Func<T, TResult> handler)
        => new(ThenAsync(handler));

    /// <summary>
    /// Creates a dependent flow by asynchronously transforming this flow's successful result.
    /// </summary>
    /// <typeparam name="TResult">The result type of the new flow.</typeparam>
    /// <param name="handler">The transformation to invoke and await only on success.</param>
    /// <returns>A new flow representing completion of the asynchronous transformation.</returns>
    /// <remarks>Predecessor failures are wrapped in <see cref="PreviousTaskException"/>; cancellation propagates without wrapping.
    /// Exceptions from the handler itself belong to the new flow and are not wrapped.</remarks>
    public TaskFlow<TResult> Then<TResult>(Func<T, Task<TResult>> handler)
        => new(ThenAsync(handler));

    /// <summary>
    /// Transforms the successful result for a dependent flow.
    /// </summary>
    /// <typeparam name="TResult">The transformed result type.</typeparam>
    /// <param name="handler">The transformation to invoke only on success.</param>
    /// <returns>The transformed result.</returns>
    private async Task<TResult> ThenAsync<TResult>(Func<T, TResult> handler)
    {
        try
        {
            await WaitAsync();
        }
        catch (OperationCanceledException ex)
        {
            if (t2 != null && t2.IsCanceled) throw;
            throw new PreviousTaskException(this, "The previous task throws an exception.", ex);
        }
        catch (Exception ex)
        {
            throw new PreviousTaskException(this, "The previous task throws an exception.", ex);
        }

        return handler(result);
    }

    /// <summary>
    /// Waits for this flow and asynchronously transforms its successful result.
    /// </summary>
    /// <typeparam name="TResult">The transformed result type.</typeparam>
    /// <param name="handler">The transformation to invoke and await only on success.</param>
    /// <returns>A task representing the transformed result, rather than a new flow.</returns>
    /// <exception cref="PreviousTaskException">This flow failed without its underlying task being canceled.</exception>
    /// <exception cref="OperationCanceledException">The underlying task was canceled.</exception>
    /// <remarks>Handler exceptions propagate directly. A predecessor failure identifies this flow in <see cref="PreviousTaskException.SourceObject"/>.</remarks>
    public async Task<TResult> ThenAsync<TResult>(Func<T, Task<TResult>> handler)
    {
        try
        {
            await WaitAsync();
        }
        catch (OperationCanceledException ex)
        {
            if (t2 != null && t2.IsCanceled) throw;
            throw new PreviousTaskException(this, "The previous task throws an exception.", ex);
        }
        catch (Exception ex)
        {
            throw new PreviousTaskException(this, "The previous task throws an exception.", ex);
        }

        return await handler(result);
    }

    private async Task OnInit()
    {
        State = TaskFlowStates.Running;
        if (t2 == null)
        {
            State = TaskFlowStates.Failure;
            return;
        }

        try
        {
            result = await t2;
            State = TaskFlowStates.Success;
        }
        catch (Exception ex)
        {
            exception = ex;
            State = TaskFlowStates.Failure;
        }
    }
}

