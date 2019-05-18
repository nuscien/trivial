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
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
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
        /// Gets a value indicating whether it has initialized.
        /// </summary>
        public bool HasInit { get; private set; }

        /// <summary>
        /// Ensures it has initialized.
        /// </summary>
        /// <returns>The async task returned.</returns>
        public async Task EnsureInitAsync()
        {
            if (HasInit) return;
            try
            {
                await semaphoreSlim.WaitAsync();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (NullReferenceException)
            {
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
        }

        /// <summary>
        /// Processes the initialization handler.
        /// </summary>
        /// <returns>The async task returned.</returns>
        protected virtual Task OnInitAsync()
        {
            return init != null ? init() : Task.Run(() => { });
        }
    }
}
