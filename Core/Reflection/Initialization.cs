using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Reflection
{
    /// <summary>
    /// The initialization in thread-safe mode.
    /// </summary>
    public class Initialization
    {
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly Func<Task> init;

        /// <summary>
        /// Initializes a new instance of the Initialization class.
        /// </summary>
        protected Initialization()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Initialization class.
        /// </summary>
        /// <param name="initialization">The initialization handler.</param>
        public Initialization(Func<Task> initialization)
        {
            init = initialization;
        }

        /// <summary>
        /// Gets a System.Threading.WaitHandle that can be used to wait on the semaphore.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The System.Threading.SemaphoreSlim has been disposed.</exception>
        protected WaitHandle SemaphoreSlimAvailableWaitHandle => semaphoreSlim.AvailableWaitHandle;

        /// <summary>
        /// Gets a value indicating whether it has initialized.
        /// </summary>
        public bool HasInit { get; private set; }

        /// <summary>
        /// Ensures it has initialized.
        /// </summary>
        /// <returns>The async task returned.</returns>
        public async Task EnsureInitAsync()
        {
            if (HasInit || semaphoreSlim == null) return;
            try
            {
                await semaphoreSlim.WaitAsync();
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (NullReferenceException)
            {
                return;
            }

            try
            {
                if (HasInit) return;
                await OnInitAsync();
                HasInit = true;
            }
            finally
            {
                try
                {
                    if (semaphoreSlim != null) semaphoreSlim.Release();
                }
                catch (ObjectDisposedException)
                {
                }
                catch (NullReferenceException)
                {
                }
            }

            AfterInit();
        }

        /// <summary>
        /// Processes the initialization handler.
        /// </summary>
        /// <returns>The async task returned.</returns>
        protected virtual Task OnInitAsync()
        {
            return init != null ? init() : Task.Run(() => { });
        }

        /// <summary>
        /// Processes on initialization has finished.
        /// </summary>
        protected virtual void AfterInit()
        {
        }

        /// <summary>
        /// Waits for initialization.
        /// </summary>
        /// <returns>The async task returned.</returns>
        protected async Task WaitInitAsync()
        {
            if (semaphoreSlim == null) return;
            try
            {
                await semaphoreSlim.WaitAsync();
                if (semaphoreSlim != null) semaphoreSlim.Release();
            }
            catch (NullReferenceException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Waits for initialization.
        /// </summary>
        /// <param name="timeout">A System.TimeSpan that represents the number of milliseconds to wait.</param>
        /// <returns>true if initialization completes; otherwise, false..</returns>
        protected async Task<bool> WaitInitAsync(TimeSpan timeout)
        {
            var result = true;
            if (semaphoreSlim == null) return result;
            try
            {
                result = await semaphoreSlim.WaitAsync(timeout);
                if (result && semaphoreSlim != null) semaphoreSlim.Release();
            }
            catch (NullReferenceException)
            {
            }
            catch (ObjectDisposedException)
            {
            }

            return result;
        }

        /// <summary>
        /// Disposes the semaphore slim.
        /// </summary>
        protected void DisposeSemaphoreSlim()
        {
            if (semaphoreSlim == null) return;
            try
            {
                semaphoreSlim.Dispose();
                semaphoreSlim = null;
            }
            catch (NullReferenceException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Disposes the semaphore slim.
        /// </summary>
        /// <param name="delay">The time span to wait before completing the returned task.</param>
        /// <exception cref="ArgumentOutOfRangeException">delay represents a negative time interval other than TimeSpan.FromMilliseconds(-1). -or- The delay argument's System.TimeSpan.TotalMilliseconds property is greater than System.Int32.MaxValue.</exception>
        protected async Task DisposeSemaphoreSlimAsync(TimeSpan delay)
        {
            if (semaphoreSlim == null) return;
            try
            {
                if (semaphoreSlim.CurrentCount == 1) semaphoreSlim.Dispose();
            }
            catch (NullReferenceException)
            {
            }
            catch (ObjectDisposedException)
            {
            }

            await Task.Delay(delay);
            DisposeSemaphoreSlim();
        }
    }
}
