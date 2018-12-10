using System;
using System.Collections.Generic;
using System.Threading;
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
        /// Initializing.
        /// </summary>
        Initializing = 1,

        /// <summary>
        /// Processing normally.
        /// </summary>
        Working = 2,

        /// <summary>
        /// Retrying.
        /// </summary>
        Retrying = 3,

        /// <summary>
        /// Cancelling.
        /// </summary>
        Cancelling = 4,

        /// <summary>
        /// Cancelled.
        /// </summary>
        Canceled = 5,

        /// <summary>
        /// Process succeeded.
        /// </summary>
        Done = 6,

        /// <summary>
        /// Failed to process.
        /// </summary>
        Faulted = 7
    }

    /// <summary>
    /// The observable task.
    /// </summary>
    public abstract class ObservableTask<T>
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// The processing task instance.
        /// </summary>
        private Task<T> task;

        /// <summary>
        /// Retry times.
        /// </summary>
        private List<DateTime> retryTimes = new List<DateTime>();

        /// <summary>
        /// The result cache.
        /// </summary>
        private T result = default(T);

        /// <summary>
        /// The progress in percent.
        /// </summary>
        private double progress = 0;

        /// <summary>
        /// The exception.
        /// </summary>
        private Exception exception = null;

        /// <summary>
        /// Adds or removes an event handler after cancellation has requested.
        /// </summary>
        public event EventHandler CancellationRequested;

        /// <summary>
        /// Adds or removes an event handler after the action is finished.
        /// </summary>
        public event ResultEventHandler<T> Finished;

        /// <summary>
        /// Gets the task state.
        /// </summary>
        public TaskState State { get; private set; } = TaskState.Pending;

        /// <summary>
        /// Gets a value indicating whether the task is pending to process.
        /// </summary>
        public bool IsPending => State == TaskState.Pending;

        /// <summary>
        /// Gets a value indicating whether the task is processing.
        /// </summary>
        public bool IsWorking => State == TaskState.Working || State == TaskState.Retrying;

        /// <summary>
        /// Gets a value indicating whether the task is not finished.
        /// </summary>
        public bool IsPendingOrWorking => State == TaskState.Pending || State == TaskState.Working || State == TaskState.Retrying || State == TaskState.Initializing || State == TaskState.Cancelling;

        /// <summary>
        /// Gets a value indicating whether the task is cancelled.
        /// </summary>
        public bool IsCanceled => State == TaskState.Canceled;

        /// <summary>
        /// Gets a value indicating whether the task is done.
        /// </summary>
        public bool IsDone => State == TaskState.Done;

        /// <summary>
        /// Gets a value indicating whether the task is failed.
        /// </summary>
        public bool IsFailed => State == TaskState.Faulted;

        /// <summary>
        /// Gets the progress in percent.
        /// </summary>
        public double Progress
        {
            get
            {
                return progress;
            }

            protected set
            {
                if (value < 0) progress = 0;
                else if (value > 100) progress = 100;
                else progress = value;
            }
        }

        /// <summary>
        /// Gets whether cancellation has been requested for this System.Threading.CancellationTokenSource.
        /// </summary>
        public bool IsCancellationRequested => State == TaskState.Cancelling || State == TaskState.Canceled;

        /// <summary>
        /// Gets the result.
        /// </summary>
        public T Result
        {
            get
            {
                if (IsPendingOrWorking) throw new InvalidOperationException();
                else if (State == TaskState.Faulted) throw exception;
                ThrowIfCancellationRequested();
                return result;
            }
        }

        /// <summary>
        /// Gets or sets the retry policy.
        /// </summary>
        public IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Processes.
        /// </summary>
        public T Process()
        {
            var task = ProcessAsync();
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Processes and gets the result.
        /// </summary>
        /// <returns>The processing task instance.</returns>
        public async Task<T> ProcessAsync()
        {
            lock (locker)
            {
                if (State != TaskState.Pending && State != TaskState.Canceled) throw new InvalidOperationException();
                State = TaskState.Initializing;
            }

            task = ProcessImplAsync();
            return await task;
        }

        /// <summary>
        /// Processes or waits.
        /// </summary>
        /// <returns>The processing task instance.</returns>
        public Task ProcessOrWaitAsync()
        {
            return ProcessOrWaitAsync(CancellationToken.None);
        }

        /// <summary>
        /// Processes or waits.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for current waiting.</param>
        /// <returns>The processing task instance.</returns>
        public async Task ProcessOrWaitAsync(CancellationToken cancellationToken)
        {
            var canWork = false;
            lock (locker)
            {
                canWork = State != TaskState.Pending && State != TaskState.Canceled;
                if (canWork) State = TaskState.Initializing;
            }

            if (canWork) task = ProcessImplAsync();
            else if (task == null) await Task.Delay(100);
            await Task.WhenAny(task, Task.FromCanceled(cancellationToken));
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel()
        {
            lock (locker)
            {
                if (IsCancellationRequested || !IsPendingOrWorking) return;
                State = TaskState.Cancelling;
            }

            try
            {
                OnCancel();
            }
            catch (OperationCanceledException)
            {
            }

            CancellationRequested?.Invoke(this, new EventArgs());
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
        /// Checks whether need retry.
        /// </summary>
        protected virtual bool NeedRetry(Exception ex)
        {
            return false;
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
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <returns>The processing task result.</returns>
        protected abstract Task<T> OnProcessAsync();

        /// <summary>
        /// Processes.
        /// </summary>
        /// <returns>The processing task instance.</returns>
        private async Task<T> ProcessImplAsync()
        {
            ThrowIfCancellationRequested();
            try
            {
                OnInitAsync();
            }
            catch (OperationCanceledException ex)
            {
                ErrorHandle(TaskState.Canceled, ex);
                throw;
            }
            catch (Exception ex)
            {
                ErrorHandle(TaskState.Faulted, ex);
                throw;
            }

            var retry = RetryPolicy?.CreateInstance();
            ThrowIfCancellationRequested();
            State = TaskState.Working;
            while (true)
            {
                try
                {
                    result = await OnProcessAsync();
                    break;
                }
                catch (OperationCanceledException ex)
                {
                    ErrorHandle(TaskState.Canceled, ex);
                    throw;
                }
                catch (Exception ex)
                {
                    var sleep = NeedRetry(ex) ? retry?.Next() : null;
                    if (sleep.HasValue)
                    {
                        try
                        {
                            await Task.Delay(sleep.Value);
                        }
                        catch (ArgumentException)
                        {
                        }

                        retryTimes.Add(DateTime.Now);
                        ErrorHandle(TaskState.Retrying, ex);
                        continue;
                    }

                    ErrorHandle(TaskState.Faulted, ex);
                    throw;
                }
            }

            State = TaskState.Done;
            progress = 100;
            Finished?.Invoke(this, new ResultEventArgs<T>(result));
            return result;
        }

        private void ErrorHandle(TaskState state, Exception ex)
        {
            State = state;
            exception = ex;
            Finished?.Invoke(this, new ResultEventArgs<T>(ex, state));
        }
    }
}
