﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Reflection;

/// <summary>
/// The state management module used to ensure that a certain initialization logic is executed only once when need in a thread-safe manner.
/// </summary>
/// <example>
/// <code>
/// var count = 0;
/// var ini = new Initialization(async () => {
///   // Do some initialization logic here.
///   await Task.Delay(1000);
///   count++;
/// });
/// 
/// // Test.
/// var task = init.EnsureInitAsync();
/// await init.EnsureInitAsync();
/// await task;
/// await init.EnsureInitAsync();
/// Console.WriteLine(count); // Output: 1
/// </code>
/// </example>
[Guid("EF30759E-EF34-4C68-9994-5769A147BE4E")]
public class Initialization
{
    private SemaphoreSlim semaphoreSlim = new(1, 1);
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
    /// Deconstructor.
    /// </summary>
    ~Initialization()
    {
        if (semaphoreSlim == null) return;
        try
        {
            semaphoreSlim.Dispose();
            semaphoreSlim = null;
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
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
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task EnsureInitAsync()
    {
        if (HasInit || semaphoreSlim == null) return;
        try
        {
            await semaphoreSlim.WaitAsync();
        }
        catch (InvalidOperationException)
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
            await Task.CompletedTask;
            HasInit = true;
        }
        finally
        {
            try
            {
                semaphoreSlim?.Release();
            }
            catch (InvalidOperationException)
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
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task OnInitAsync()
        => init != null ? init() : Task.CompletedTask;

    /// <summary>
    /// Processes on initialization has finished.
    /// </summary>
    protected virtual void AfterInit()
    {
    }

    /// <summary>
    /// Waits for initialization.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task WaitInitAsync()
    {
        if (semaphoreSlim == null) return;
        try
        {
            await semaphoreSlim.WaitAsync();
            semaphoreSlim?.Release();
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
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
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
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
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
    }

    /// <summary>
    /// Disposes the semaphore slim.
    /// </summary>
    /// <param name="delay">The time span to wait before completing the returned task.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">delay represents a negative time interval other than TimeSpan.FromMilliseconds(-1). -or- The delay argument's System.TimeSpan.TotalMilliseconds property is greater than System.Int32.MaxValue.</exception>
    protected async Task DisposeSemaphoreSlimAsync(TimeSpan delay)
    {
        if (semaphoreSlim == null) return;
        try
        {
            if (semaphoreSlim.CurrentCount == 1) semaphoreSlim.Dispose();
        }
        catch (InvalidOperationException)
        {
            if (semaphoreSlim == null) return;
        }
        catch (NullReferenceException)
        {
            if (semaphoreSlim == null) return;
        }

        await Task.Delay(delay);
        DisposeSemaphoreSlim();
    }
}
