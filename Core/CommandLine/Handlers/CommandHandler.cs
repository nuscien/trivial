using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Reflection;

namespace Trivial.CommandLine;

/// <summary>
/// The command handler interface.
/// It can be used for sub-application.
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Gets the description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets a value indicating whether the command is disabled.
    /// </summary>
    bool IsDisabled { get; }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="args">The command arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>true if process succeeded; otherwise, false.</returns>
    Task ProcessAsync(CommandArguments args, CommandConversationContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets help information.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    void GetHelp(CommandArguments args, CommandConversationContext context);
}

/// <summary>
/// The base command handler.
/// It can be used for sub-application.
/// </summary>
public abstract class BaseCommandHandler : ICommandHandler
{
    /// <summary>
    /// Gets the description.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Gets a value indicating whether the command is disabled.
    /// </summary>
    public bool IsDisabled { get; protected set; }

    /// <summary>
    /// Gets the exception handler.
    /// </summary>
    protected ExceptionHandler ExceptionHandler { get; } = new ExceptionHandler();

    /// <summary>
    /// Occurs on initializating the handler.
    /// </summary>
    protected virtual void OnInit()
    {
    }

    /// <summary>
    /// Occurs on processing.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    protected abstract void OnProcess(CommandArguments args, CommandConversationContext context);

    /// <summary>
    /// Occurs on getting help of the handler.
    /// </summary>
    /// <param name="args">The arguments data.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    protected virtual void OnGetHelp(CommandArguments args, CommandConversationContext context)
    {
        Console.WriteLine(Description);
    }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="args">The command arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>true if process succeeded; otherwise, false.</returns>
    async Task ICommandHandler.ProcessAsync(CommandArguments args, CommandConversationContext context, CancellationToken cancellationToken)
    {
        if (IsDisabled) return;
        await Task.Run(() => { }, cancellationToken);
        try
        {
            OnProcess(args, context);
        }
        catch (Exception ex)
        {
            var exConverted = ExceptionHandler.GetException(ex);
            if (exConverted == null) return;
            if (exConverted == ex) throw;
            throw exConverted;
        }
    }

    /// <summary>
    /// Gets help information.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    void ICommandHandler.GetHelp(CommandArguments args, CommandConversationContext context)
        => OnGetHelp(args, context);
}

/// <summary>
/// The base command handler.
/// It can be used for sub-application.
/// </summary>
public abstract class AsyncCommandHandler : ICommandHandler
{
    /// <summary>
    /// Gets the description.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Gets a value indicating whether the command is disabled.
    /// </summary>
    public bool IsDisabled { get; protected set; }

    /// <summary>
    /// Gets the exception handler.
    /// </summary>
    protected ExceptionHandler ExceptionHandler { get; } = new ExceptionHandler();

    /// <summary>
    /// Occurs on initializating the handler.
    /// </summary>
    protected virtual void OnInit()
    {
    }

    /// <summary>
    /// Occurs on processing.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A task that represents the completion of all of the invoking action tasks.</returns>
    protected abstract Task OnProcessAsync(CommandArguments args, CommandConversationContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Occurs on getting help of the handler.
    /// </summary>
    /// <param name="args">The arguments data.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    protected virtual void OnGetHelp(CommandArguments args, CommandConversationContext context)
    {
    }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="args">The command arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>true if process succeeded; otherwise, false.</returns>
    async Task ICommandHandler.ProcessAsync(CommandArguments args, CommandConversationContext context, CancellationToken cancellationToken)
    {
        if (IsDisabled) return;
        try
        {
            await OnProcessAsync(args, context, cancellationToken);
        }
        catch (Exception ex)
        {
            var exConverted = ExceptionHandler.GetException(ex);
            if (exConverted == null) return;
            if (exConverted == ex) throw;
            throw exConverted;
        }
    }

    /// <summary>
    /// Gets help information.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    void ICommandHandler.GetHelp(CommandArguments args, CommandConversationContext context)
        => OnGetHelp(args, context);
}

/// <summary>
/// The base command verb.
/// </summary>
public abstract class BaseCommandVerb
{
    /// <summary>
    /// Gets the command arguments.
    /// </summary>
    protected CommandArguments Arguments { get; private set; }

    /// <summary>
    /// Gets the conversation context during the command processing.
    /// </summary>
    protected CommandConversationContext Context { get; private set; }

    /// <summary>
    /// Tests if the arguments are valid.
    /// </summary>
    /// <returns>true if it is valid; otherwise, false.</returns>
    protected virtual bool IsValid() => true;

    /// <summary>
    /// Occurs on processing.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A task that represents the completion of all of the invoking action tasks.</returns>
    protected abstract Task OnProcessAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Occurs on getting help of the handler.
    /// </summary>
    protected virtual void OnGetHelp()
    {
    }

    /// <summary>
    /// Gets the console instance from context.
    /// </summary>
    /// <returns>The console instance.</returns>
    protected StyleConsole GetConsole()
        => Context?.Console ?? StyleConsole.Default;

    /// <summary>
    /// Tests if the exception on processing need be thrown.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <returns>true if need; otherwise, false.</returns>
    protected virtual bool NeedThrowException(Exception ex)
        => true;

    /// <summary>
    /// Queues the specified work to run on the thread pool.
    /// </summary>
    /// <param name="action">The work to execute asynchronously.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A task that represents the work queued to execute in the thread pool.</returns>
    protected static Task RunAsync(Action action = null, CancellationToken cancellationToken = default)
        => Task.Run(action is null ? RunEmpty : action, cancellationToken);

    /// <summary>
    /// Queues the specified work to run on the thread pool.
    /// </summary>
    /// <param name="action">The work to execute asynchronously.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A task that represents the work queued to execute in the thread pool.</returns>
    protected static Task<T> RunAsync<T>(Func<T> action, CancellationToken cancellationToken = default)
        => Task.Run(action, cancellationToken);

    /// <summary>
    /// Creates a cancellable task that completes after a specified number of milliseconds.
    /// </summary>
    /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned task, or -1 to wait indefinitely.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the time delay.</returns>
    protected static Task Delay(int millisecondsDelay, CancellationToken cancellationToken = default)
        => Task.Delay(millisecondsDelay, cancellationToken);

    /// <summary>
    /// Creates a cancellable task that completes after a specified number of milliseconds.
    /// </summary>
    /// <param name="delay">The time span to wait before completing the returned task, or TimeSpan.FromMilliseconds(-1) to wait indefinitely.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the time delay.</returns>
    protected static Task Delay(TimeSpan delay, CancellationToken cancellationToken = default)
        => Task.Delay(delay, cancellationToken);

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="args">The command arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>true if process succeeded; otherwise, false.</returns>
    internal async Task ProcessAsync(CommandArguments args, CommandConversationContext context, CancellationToken cancellationToken = default)
    {
        Arguments = args;
        Context = context;
        if (!IsValid()) return;
        try
        {
            await OnProcessAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            if (cancellationToken.IsCancellationRequested) throw;
            if (NeedThrowException(ex)) throw;
        }
    }

    /// <summary>
    /// Gets help information.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    internal void GetHelp(CommandArguments args, CommandConversationContext context)
    {
        Arguments = new CommandArguments(args);
        Context = context;
        OnGetHelp();
    }

    private static void RunEmpty()
    {
    }
}

/// <summary>
/// The base command handler.
/// It can be used for sub-application.
/// </summary>
public class CommandVerbHandler<T> : ICommandHandler
    where T : BaseCommandVerb
{
    private readonly Func<T> factory;

    /// <summary>
    /// Initializes a new instance of the CommandVerbHandler class.
    /// </summary>
    public CommandVerbHandler() : this(null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the CommandVerbHandler class.
    /// </summary>
    /// <param name="description">The optional description.</param>
    public CommandVerbHandler(string description) : this(null, description)
    {
    }

    /// <summary>
    /// Initializes a new instance of the CommandVerbHandler class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="description">The optional description.</param>
    public CommandVerbHandler(Func<T> factory, string description = null)
    {
        this.factory = factory ?? Activator.CreateInstance<T>;
        Description = description = description?.Trim();
        if (!string.IsNullOrEmpty(description)) return;
        try
        {
            var desc = typeof(T).GetProperty(nameof(Description), BindingFlags.Static | BindingFlags.Public);
            if (desc == null)
            {
                desc = typeof(T).GetProperty(nameof(Description));
                if (desc == null || !desc.CanRead) return;
                Description = (string)desc.GetValue(this.factory());
                return;
            }

            if (!desc.CanRead) return;
            description = (string)desc.GetValue(null);
        }
        catch (AmbiguousMatchException)
        {
        }
        catch (InvalidCastException)
        {
        }
        catch (InvalidOperationException)
        {
        }
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets a value indicating whether the command is disabled.
    /// </summary>
    public bool IsDisabled { get; protected set; }

    /// <summary>
    /// Gets the type of the command verb.
    /// </summary>
    public Type VerbType => typeof(T);

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="args">The command arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>true if process succeeded; otherwise, false.</returns>
    Task ICommandHandler.ProcessAsync(CommandArguments args, CommandConversationContext context, CancellationToken cancellationToken)
    {
        var instance = factory();
        if (instance is null) return Task.Run(() => { }, cancellationToken);
        return instance.ProcessAsync(args ?? new CommandArguments(string.Empty), context, cancellationToken);
    }

    /// <summary>
    /// Gets help information.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="context">The conversation context during the command processing.</param>
    void ICommandHandler.GetHelp(CommandArguments args, CommandConversationContext context)
    {
        var instance = factory();
        if (instance is null) return;
        instance.GetHelp(args ?? new CommandArguments(string.Empty), context);
    }
}
