using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// The interceptor event handler.
/// </summary>
/// <typeparam name="T">The type of argument.</typeparam>
/// <param name="sender">The sender.</param>
/// <param name="args">The arguments.</param>
public delegate void InterceptorEventHandler<T>(Interceptor<T> sender, InterceptorEventArgs<T> args);

/// <summary>
/// The interceptor event handler.
/// </summary>
/// <param name="sender">The sender.</param>
/// <param name="args">The arguments.</param>
public delegate void InterceptorEventHandler(Interceptor sender, InterceptorEventArgs<object> args);

/// <summary>
/// The event arguments of the interceptor.
/// </summary>
public class InterceptorEventArgs<T> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the InterceptorEventArgs class.
    /// </summary>
    /// <param name="count">The count of invoking times.</param>
    /// <param name="current">The current invoking date time.</param>
    /// <param name="previous">The previous date time invoked.</param>
    /// <param name="first">The first date time invoked.</param>
    /// <param name="arg">The argument.</param>
    public InterceptorEventArgs(int count, DateTime current, DateTime? previous, DateTime first, T arg)
    {
        Count = count;
        InvokingDate = current;
        PreviousInvokedDate = previous;
        FirstInvokedDate = first;
        Argument = arg;
    }

    /// <summary>
    /// Gets the count of invoking times.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the current invoking date time.
    /// </summary>
    public DateTime InvokingDate { get; }

    /// <summary>
    /// Gets the previous date time invoked.
    /// </summary>
    public DateTime? PreviousInvokedDate { get; }

    /// <summary>
    /// Gets the first date time invoked.
    /// </summary>
    public DateTime FirstInvokedDate { get; }

    /// <summary>
    /// Gets the argument.
    /// </summary>
    public T Argument { get; }
}
