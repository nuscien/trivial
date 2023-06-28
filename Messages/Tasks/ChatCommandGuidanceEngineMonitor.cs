using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Text;

namespace Trivial.Tasks;

/// <summary>
/// The monitor of chat command guidance engine.
/// </summary>
public interface IChatCommandGuidanceEngineMonitor
{
    /// <summary>
    /// Creates an object used for following steps.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <returns>The object.</returns>
    object InitializeObject(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context);

    /// <summary>
    /// Occurs on request.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="userDetails">The user information in details.</param>
    /// <param name="obj">The object.</param>
    void OnRequest(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, JsonObjectNode userDetails, object obj);

    /// <summary>
    /// Generates the prompt.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="obj">The object.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The prompt segment to append.</returns>
    Task<string> GeneratePromptAsync(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, object obj, CancellationToken cancellationToken = default);

    /// <summary>
    /// Occurs on sending error.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="ex">The exception</param>
    /// <param name="obj">The object.</param>
    void OnSendError(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, Exception ex, object obj);

    /// <summary>
    /// Occurs on answer received.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="answer">The answer.</param>
    /// <param name="obj">The object.</param>
    void OnReceive(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, ChatCommandGuidanceSourceResult answer, object obj);

    /// <summary>
    /// Occurs on response returning.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="obj">The object.</param>
    void OnResponse(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, object obj);
}

/// <summary>
/// The monitor of chat command guidance engine.
/// </summary>
public abstract class BaseChatCommandGuidanceEngineMonitor<T> : IChatCommandGuidanceEngineMonitor where T : class
{
    /// <summary>
    /// Creates an object used for following steps.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <returns>The object.</returns>
    protected virtual T InitializeObject(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context)
        => null;

    /// <summary>
    /// Creates an object used for following steps.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <returns>The object.</returns>
    object IChatCommandGuidanceEngineMonitor.InitializeObject(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context)
        => InitializeObject(engine, context);

    /// <summary>
    /// Occurs on request.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="userDetails">The user information in details.</param>
    /// <param name="obj">The object.</param>
    protected virtual void OnRequest(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, JsonObjectNode userDetails, T obj)
    {
    }

    /// <summary>
    /// Occurs on request.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="userDetails">The user information in details.</param>
    /// <param name="obj">The object.</param>
    void IChatCommandGuidanceEngineMonitor.OnRequest(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, JsonObjectNode userDetails, object obj)
        => OnRequest(engine, context, userDetails, obj as T);

    /// <summary>
    /// Generates the prompt.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="obj">The object.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The prompt segment to append.</returns>
    protected virtual Task<string> GeneratePromptAsync(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, T obj, CancellationToken cancellationToken = default)
        => Task.FromResult<string>(null);

    /// <summary>
    /// Generates the prompt.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="obj">The object.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The prompt segment to append.</returns>
    Task<string> IChatCommandGuidanceEngineMonitor.GeneratePromptAsync(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, object obj, CancellationToken cancellationToken)
        => GeneratePromptAsync(engine, context, obj as T, cancellationToken);

    /// <summary>
    /// Occurs on sending error.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="ex">The exception</param>
    /// <param name="obj">The object.</param>
    protected virtual void OnSendError(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, Exception ex, T obj)
    {
    }

    /// <summary>
    /// Occurs on sending error.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="ex">The exception</param>
    /// <param name="obj">The object.</param>
    void IChatCommandGuidanceEngineMonitor.OnSendError(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, Exception ex, object obj)
        => OnSendError(engine, context, ex, obj as T);

    /// <summary>
    /// Occurs on answer received.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="answer">The answer.</param>
    /// <param name="obj">The object.</param>
    protected virtual void OnReceive(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, ChatCommandGuidanceSourceResult answer, T obj)
    {
    }

    /// <summary>
    /// Occurs on answer received.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="answer">The answer.</param>
    /// <param name="obj">The object.</param>
    void IChatCommandGuidanceEngineMonitor.OnReceive(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, ChatCommandGuidanceSourceResult answer, object obj)
        => OnReceive(engine, context, answer, obj as T);

    /// <summary>
    /// Occurs on response returning.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="obj">The object.</param>
    protected virtual void OnResponse(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, T obj)
    {
    }

    /// <summary>
    /// Occurs on response returning.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="context">The context.</param>
    /// <param name="obj">The object.</param>
    void IChatCommandGuidanceEngineMonitor.OnResponse(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context, object obj)
        => OnResponse(engine, context, obj as T);
}

internal class ChatCommandGuidanceEngineMonitorTask
{
    private ChatCommandGuidanceEngineMonitorTask(IChatCommandGuidanceEngineMonitor monitor, BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context)
    {
        if (monitor == null) return;
        Monitor = monitor;
        Context = context;
        Ref = monitor.InitializeObject(engine, context);
    }

    public ChatCommandGuidanceContext Context { get; }

    public IChatCommandGuidanceEngineMonitor Monitor { get; }

    public object Ref { get; }

    public static IEnumerable<ChatCommandGuidanceEngineMonitorTask> Create(IEnumerable<IChatCommandGuidanceEngineMonitor> monitors, BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceContext context)
    {
        if (monitors == null) yield break;
        foreach (var monitor in monitors)
        {
            yield return new ChatCommandGuidanceEngineMonitorTask(monitor, engine, context);
        }
    }
}
