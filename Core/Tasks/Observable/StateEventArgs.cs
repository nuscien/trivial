using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Trivial.Tasks;

/// <summary>
/// The state event handler.
/// </summary>
/// <typeparam name="T">The type of the state.</typeparam>
/// <param name="sender">The sender.</param>
/// <param name="e">The event arguments.</param>
public delegate void StateEventHandler<T>(object sender, StateEventArgs<T> e);

/// <summary>
/// The result event handler.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
/// <param name="sender">The sender.</param>
/// <param name="e">The event arguments.</param>
public delegate void ResultEventHandler<T>(object sender, ResultEventArgs<T> e);

/// <summary>
/// The event arguments with message.
/// </summary>
/// <param name="message">The additional message.</param>
[DebuggerDisplay("{Message}")]
public class MessageEventArgs(string message = null) : EventArgs
{

    /// <summary>
    /// Gets the additional message.
    /// </summary>
    public string Message { get; } = message;
}

/// <summary>
/// The event arguments with state.
/// </summary>
/// <typeparam name="T">The type of the state.</typeparam>
[DebuggerDisplay("{State}")]
public class StateEventArgs<T> : MessageEventArgs
{
    /// <summary>
    /// Initializes a new instance of the StateEventArgs class.
    /// </summary>
    /// <param name="state">The state.</param>
    public StateEventArgs(T state) => State = state;

    /// <summary>
    /// Initializes a new instance of the StateEventArgs class.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="message">The additional message.</param>
    public StateEventArgs(T state, string message) : base(message) => State = state;

    /// <summary>
    /// Gets the state.
    /// </summary>
    public T State { get; }
}

/// <summary>
/// The event arguments with result.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public class ResultEventArgs<T> : StateEventArgs<TaskStates>
{
    /// <summary>
    /// Initializes a new instance of the ResultEventArgs class.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="state">The state.</param>
    /// <param name="exception">The exception.</param>
    public ResultEventArgs(T result, TaskStates state = TaskStates.Done, Exception exception = null) : base(state)
    {
        Result = result;
        Exception = exception;
    }

    /// <summary>
    /// Initializes a new instance of the ResultEventArgs class.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="state">The state.</param>
    public ResultEventArgs(Exception exception, TaskStates state = TaskStates.Faulted) : base(state) => Exception = exception;

    /// <summary>
    /// Initializes a new instance of the ResultEventArgs class.
    /// </summary>
    /// <param name="message">The additional message.</param>
    /// <param name="result">The result.</param>
    /// <param name="state">The state.</param>
    /// <param name="exception">The exception.</param>
    public ResultEventArgs(string message, T result, TaskStates state = TaskStates.Done, Exception exception = null) : base(state, message)
    {
        Result = result;
        Exception = exception;
    }

    /// <summary>
    /// Initializes a new instance of the ResultEventArgs class.
    /// </summary>
    /// <param name="message">The additional message.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="state">The state.</param>
    public ResultEventArgs(string message, Exception exception, TaskStates state = TaskStates.Faulted) : base(state, message) => Exception = exception;

    /// <summary>
    /// Gets the result.
    /// </summary>
    public T Result { get; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    public Exception Exception { get; }
}
