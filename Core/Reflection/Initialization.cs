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
        /// Gets a System.Threading.WaitHandle that can be used to wait on the semaphore.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The System.Threading.SemaphoreSlim has been disposed.</exception>
        protected WaitHandle SemaphoreSlimAvailableWaitHandle => semaphoreSlim.AvailableWaitHandle;

        /// <summary>
        /// Initializes a new instance of the Initialization class.
        /// </summary>
        /// <param name="initialization">The initialization handler.</param>
        public Initialization(Func<Task> initialization)
        {
            init = initialization;
        }

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
                    semaphoreSlim.Release();
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
        /// Disposes the semaphore slim.
        /// </summary>
        protected void DisposeSemaphoreSlim()
        {
            semaphoreSlim.Dispose();
            semaphoreSlim = null;
        }
    }
}
