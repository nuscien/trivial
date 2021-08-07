using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    /// <summary>
    /// The task flow.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    public class TaskFlow<T>
    {
        private readonly Task t;

        private readonly Task<T> t2;

        private Exception exception;

        private T result;

        /// <summary>
        /// Initializes a new instance of the TaskFlow class.
        /// </summary>
        /// <param name="task">The task to monitor.</param>
        public TaskFlow(Task<T> task)
        {
            t2 = task;
            t = OnInit();
        }

        /// <summary>
        /// Initializes a new instance of the TaskFlow class.
        /// </summary>
        /// <param name="task">The task to monitor.</param>
        public TaskFlow(Func<Task<T>> task)
        {
            t2 = task();
            t = OnInit();
        }

        /// <summary>
        /// The state of the task flow.
        /// </summary>
        public TaskFlowStates State { get; private set; }

        /// <summary>
        /// Gets the status of the task.
        /// </summary>
        public TaskStatus TaskStatus => t2?.Status ?? TaskStatus.Created;

        /// <summary>
        /// Gets a value that indicates whether the task has completed.
        /// </summary>
        public bool IsCompleted => State == TaskFlowStates.Success || State == TaskFlowStates.Failure;

        /// <summary>
        /// Gets whether the task ran to completion.
        /// </summary>
        public bool IsSuccessful => State == TaskFlowStates.Success;

        /// <summary>
        /// 
        /// </summary>
        public bool IsExceptionThrownByPrevious => exception != null && exception is PreviousTaskException ex && ex.SourceObject != null && ex.SourceObject != this;

        /// <summary>
        /// Gets whether the task instance has completed execution due to being canceled.
        /// </summary>
        public bool IsCanceled => t2?.IsCanceled == true;

        /// <summary>
        /// Appends a subsequent handler on success.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        /// <returns>The flow itself.</returns>
        public TaskFlow<T> ResultTo(Action<T> handler)
        {
            _ = ResultToAsync(handler);
            return this;
        }

        /// <summary>
        /// Appends a subsequent handler on success.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        /// <returns>The flow itself.</returns>
        public TaskFlow<T> ResultTo(Func<T, Task> handler)
        {
            _ = ResultToAsync(handler);
            return this;
        }

        /// <summary>
        /// Appends a subsequent handler on success with a specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of task result expected.</typeparam>
        /// <param name="handler">The handler to register.</param>
        /// <returns>The flow itself.</returns>
        public virtual TaskFlow<T> ResultTo<TResult>(Action<TResult> handler)
        {
            _ = ResultToAsync(r =>
            {
                if (r is TResult arg) handler(arg);
            });
            return this;
        }

        /// <summary>
        /// Appends a subsequent handler on success with a specific type.
        /// </summary>
        /// <typeparam name="TResult">The type of task result expected.</typeparam>
        /// <param name="handler">The handler to register.</param>
        /// <returns>The flow itself.</returns>
        public virtual TaskFlow<T> ResultTo<TResult>(Func<TResult, Task> handler)
        {
            _ = ResultToAsync(async r =>
            {
                if (r is TResult arg) await handler(arg);
            });
            return this;
        }

        /// <summary>
        /// Appends a handler on exception thrown.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="errorHandling">The error handling to register.</param>
        /// <param name="exactType">true if the exception type is the one expected exactly; otherwise, false.</param>
        /// <returns>The flow itself.</returns>
        public TaskFlow<T> Catch<TException>(Action<TException> errorHandling, bool exactType = false) where TException : Exception
        {
            _ = CatchAsync(errorHandling, exactType);
            return this;
        }

        /// <summary>
        /// Appends a handler on exception thrown.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="errorHandling">The error handling to register.</param>
        /// <param name="exactType">true if the exception type is the one expected exactly; otherwise, false.</param>
        /// <returns>The flow itself.</returns>
        public TaskFlow<T> Catch<TException>(Func<TException, Task> errorHandling, bool exactType = false) where TException : Exception
        {
            _ = CatchAsync(errorHandling, exactType);
            return this;
        }

        /// <summary>
        /// Waits the result.
        /// </summary>
        /// <param name="ignoreException">true if do not throw exception even if the task is failed; otherwise, false.</param>
        /// <returns>true if the task is completed successfully; otherwise, false.</returns>
        public async Task<bool> WaitAsync(bool ignoreException = false)
        {
            await t;
            if (ignoreException || exception == null) return State == TaskFlowStates.Success;
            if (t2 != null) await t2;
            throw exception;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns>The result.</returns>
        public Task<T> ResultAsync()
           => t2;

        /// <summary>
        /// Appends a subsequent handler on success.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        /// <returns>true if the state matches the condition; otherwise, false.</returns>
        public async Task<bool> ResultToAsync(Action<T> handler)
        {
            await t;
            if (State != TaskFlowStates.Success) return false;
            handler(result);
            return true;
        }

        /// <summary>
        /// Appends a subsequent handler on success.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        /// <returns>true if the state matches the condition; otherwise, false.</returns>
        public async Task<bool> ResultToAsync(Func<T, Task> handler)
        {
            await t;
            if (State != TaskFlowStates.Success) return false;
            await handler(result);
            return true;
        }

        /// <summary>
        /// Appends a handler on exception thrown.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="errorHandling">The error handling to register.</param>
        /// <param name="exactType">true if the exception type is the one expected exactly; otherwise, false.</param>
        /// <returns>true if the state matches the condition; otherwise, false.</returns>
        public async Task<bool> CatchAsync<TException>(Action<TException> errorHandling, bool exactType = false) where TException : Exception
        {
            await t;
            if (exception == null || exception is not TException ex) return false;
            if (exactType && ex.GetType() != typeof(TException)) return false;
            errorHandling(ex);
            return true;
        }

        /// <summary>
        /// Appends a handler on exception thrown.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="errorHandling">The error handling to register.</param>
        /// <param name="exactType">true if the exception type is the one expected exactly; otherwise, false.</param>
        /// <returns>true if the state matches the condition; otherwise, false.</returns>
        public async Task<bool> CatchAsync<TException>(Func<TException, Task> errorHandling, bool exactType = false) where TException : Exception
        {
            await t;
            if (exception == null || exception is not TException ex) return false;
            if (exactType && ex.GetType() != typeof(TException)) return false;
            await errorHandling(ex);
            return true;
        }

        /// <summary>
        /// Follows a subsequent handler on success.
        /// </summary>
        /// <param name="handler">The handler to run after the current task completed.</param>
        /// <returns>A new flow created for the handler.</returns>
        public TaskFlow<TResult> Then<TResult>(Func<T, TResult> handler)
        {
            return new TaskFlow<TResult>(ThenAsync(handler));
        }

        /// <summary>
        /// Follows a subsequent handler on success.
        /// </summary>
        /// <param name="handler">The handler to run after the current task completed.</param>
        /// <returns>A new flow created for the handler.</returns>
        public TaskFlow<TResult> Then<TResult>(Func<T, Task<TResult>> handler)
        {
            return new TaskFlow<TResult>(ThenAsync(handler));
        }

        /// <summary>
        /// Follows a subsequent handler on success.
        /// </summary>
        /// <param name="handler">The handler to run after the current task completed.</param>
        /// <returns>A new flow created for the handler.</returns>
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
        /// Follows a subsequent handler on success.
        /// </summary>
        /// <param name="handler">The handler to run after the current task completed.</param>
        /// <returns>A new flow created for the handler.</returns>
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
}

