using System;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    /// <summary>
    /// The task states.
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// Ready for processing.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Processing.
        /// </summary>
        Working = 1,

        /// <summary>
        /// Failed to process.
        /// </summary>
        Faulted = 2,

        /// <summary>
        /// Cancelled.
        /// </summary>
        Canceled = 3,

        /// <summary>
        /// Process succeeded.
        /// </summary>
        Done = 4
    }

    /// <summary>
    /// The cancellable task.
    /// </summary>
    public abstract class CancellableTask<T>
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The result cache.
        /// </summary>
        private T result = default(T);

        /// <summary>
        /// The exception.
        /// </summary>
        private Exception exception = null;

        /// <summary>
        /// Gets the task state.
        /// </summary>
        public TaskState State { get; private set; } = TaskState.Pending;

        /// <summary>
        /// Gets a value indicating whether the task is cancelled.
        /// </summary>
        public bool IsCanceled => State == TaskState.Canceled;

        /// <summary>
        /// Gets a value indicating whether the task is pending to process.
        /// </summary>
        public bool IsPending => State == TaskState.Pending;

        /// <summary>
        /// Gets a value indicating whether the task is processing.
        /// </summary>
        public bool IsWorking => State == TaskState.Working;

        /// <summary>
        /// Gets a value indicating whether the task is not finished.
        /// </summary>
        public bool IsPendingOrWorking => State == TaskState.Pending || State == TaskState.Working;

        /// <summary>
        /// Gets a value indicating whether the task is done.
        /// </summary>
        public bool IsDone => State == TaskState.Done;

        /// <summary>
        /// Gets a value indicating whether the task is failed.
        /// </summary>
        public bool IsFailed => State == TaskState.Faulted;

        /// <summary>
        /// Gets whether cancellation has been requested for this System.Threading.CancellationTokenSource.
        /// </summary>
        public bool IsCancellationRequested { get; set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public T Result
        {
            get
            {
                if (State == TaskState.Pending || State == TaskState.Working) throw new InvalidOperationException();
                else if (State == TaskState.Faulted) throw exception;
                ThrowIfCancellationRequested();
                return result;
            }
        }

        /// <summary>
        /// Processes.
        /// </summary>
        public void Process()
        {
            ProcessAsync().Wait();
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <returns>The processing task instance.</returns>
        public async Task<T> ProcessAsync()
        {
            lock (locker)
            {
                if (State != TaskState.Pending) throw new InvalidOperationException();
                State = TaskState.Working;
            }

            ThrowIfCancellationRequested();
            try
            {
                OnInitAsync();
            }
            catch (OperationCanceledException ex)
            {
                State = TaskState.Canceled;
                exception = ex;
                throw;
            }
            catch (Exception ex)
            {
                State = TaskState.Faulted;
                exception = ex;
                throw;
            }

            ThrowIfCancellationRequested();
            try
            {
                return result = await OnProcessAsync();
            }
            catch (OperationCanceledException ex)
            {
                State = TaskState.Canceled;
                exception = ex;
                throw;
            }
            catch (Exception ex)
            {
                State = TaskState.Faulted;
                exception = ex;
                throw;
            }
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel()
        {
            lock (locker)
            {
                if (State == TaskState.Canceled || IsCancellationRequested) return;
                IsCancellationRequested = true;
            }

            try
            {
                OnCancel();
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ThrowIfCancellationRequested()
        {
            if (State != TaskState.Canceled) return;
            if (exception != null && exception is OperationCanceledException) throw exception;
            throw new OperationCanceledException();
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        protected virtual void OnCancel()
        {
        }

        /// <summary>
        /// Initializes before processing.
        /// </summary>
        protected virtual void OnInitAsync()
        {
            return;
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <returns>The processing task result.</returns>
        protected abstract Task<T> OnProcessAsync();
    }
}
