using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Data;
using Trivial.Users;
using Trivial.Text;
using System.Threading;

namespace Trivial.Tasks;

/// <summary>
/// The types of command guidance for chat bot.
/// </summary>
public enum ChatCommandGuidanceProviderKinds : byte
{
    /// <summary>
    /// Invalid.
    /// </summary>
    None = 0,

    /// <summary>
    /// The command.
    /// </summary>
    Command = 1,

    /// <summary>
    /// The assistance.
    /// </summary>
    Assistance = 2,

    /// <summary>
    /// The recording operator.
    /// </summary>
    Recorder = 7,

    /// <summary>
    /// The others.
    /// </summary>
    Others = 15
}

/// <summary>
/// The arguments of the command guidance for the chat bot.
/// </summary>
public class ChatCommandGuidanceArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceArgs class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="command">The command key.</param>
    public ChatCommandGuidanceArgs(ChatCommandGuidanceContext context, string command)
    {
        Context = context;
        Command = command;
        AnswerData = context?.GetAnswerData(command) ?? new();
        Info = context?.GetInfo(command) ?? new();
        NextInfo = context?.GetNextInfo(command) ?? new();
    }

    /// <summary>
    /// Gets the additional object.
    /// </summary>
    public object AdditionalObject { get; internal set; }

    /// <summary>
    /// Gets the context.
    /// </summary>
    public ChatCommandGuidanceContext Context { get; }

    /// <summary>
    /// Gets the context information.
    /// The command guidance can access this to store useful data during this round.
    /// </summary>
    public JsonObjectNode Info { get; }

    /// <summary>
    /// Gets the shared context information.
    /// The command guidance can access this to store useful data during this round.
    /// </summary>
    public JsonObjectNode SharedInfo => Context?.Info;

    /// <summary>
    /// Gets the command key.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    public IList<string> Parameters { get; internal set; }

    /// <summary>
    /// Gets the data of the business rich output.
    /// </summary>
    public JsonObjectNode AnswerData { get; }

    /// <summary>
    /// Gets the shared data of the business rich output.
    /// </summary>
    public JsonObjectNode SharedAnswerData => Context?.AnswerData;

    /// <summary>
    /// Gets the next context information so that the command guidance can access.
    /// </summary>
    public JsonObjectNode NextInfo { get; } = new();

    /// <summary>
    /// Gets the shared next context information.
    /// </summary>
    public JsonObjectNode SharedNextInfo => Context?.NextInfo;

    /// <summary>
    /// Gets the original chat message result.
    /// </summary>
    public string OriginalAnswerMessage => Context?.OriginalAnswerMessage;

    /// <summary>
    /// Gets the chat message result.
    /// </summary>
    public string AnswerMessage => Context?.AnswerMessage;

    /// <summary>
    /// Gets the creation date and time.
    /// </summary>
    public DateTime CreationTime { get; } = DateTime.Now;
}

/// <summary>
/// The command guidance provider for chat bot.
/// </summary>
public abstract class BaseChatCommandGuidanceProvider
{
    /// <summary>
    /// Initializes a new instance of the BaseChatCommandGuidance class.
    /// </summary>
    /// <param name="command">The command key.</param>
    /// <param name="description">The command description.</param>
    /// <param name="parameterDescription">The command parameter description.</param>
    /// <param name="type">The command type.</param>
    protected BaseChatCommandGuidanceProvider(string command, string description, string parameterDescription, ChatCommandGuidanceProviderKinds type)
    {
        Command = command;
        Description = description;
        ParameterDescription = parameterDescription;
        Kind = type;
    }

    /// <summary>
    /// Gets the command key.
    /// </summary>
    public virtual string Command { get; }

    /// <summary>
    /// Gets the command description.
    /// </summary>
    public virtual string Description { get; }

    /// <summary>
    /// Gets the command parameter description.
    /// </summary>
    public virtual string ParameterDescription { get; }

    /// <summary>
    /// Gets the kind of the command.
    /// </summary>
    public virtual ChatCommandGuidanceProviderKinds Kind { get; }

    /// <summary>
    /// Occurs on initialized.
    /// </summary>
    protected internal virtual void OnInit(BaseChatCommandGuidanceEngine engine)
    {
    }

    /// <summary>
    /// Occurs on initialized.
    /// </summary>
    /// <param name="client">The client.</param>
    protected internal virtual void OnInit(BaseChatCommandGuidanceClient client)
    {
    }

    /// <summary>
    /// Creates an object used for following steps.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>The object.</returns>
    protected internal virtual object InitializeObject(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceArgs args)
        => null;

    /// <summary>
    /// Tests if the command guidance is available.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>true if the command guidance is available; otherwise, false.</returns>
    protected internal virtual bool IsAvailable(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceArgs args)
        => args != null;

    /// <summary>
    /// Generates the prompt.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The prompt segment to append.</returns>
    protected internal virtual Task<string> GeneratePromptAsync(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceArgs args, CancellationToken cancellationToken = default)
        => Task.FromResult<string>(null);

    /// <summary>
    /// Post processes.
    /// </summary>
    /// <param name="engine">The engine.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A Task that represents the work queued to execute in the ThreadPool.</returns>
    protected internal abstract Task PostProcessAsync(BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceArgs args, CancellationToken cancellationToken = default);

    /// <summary>
    /// Post processes on client side.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="args">The arguments.</param>
    protected internal virtual void ClientProcess(BaseChatCommandGuidanceClient client, ChatCommandGuidanceClientProcessingArgs args)
    {
    }

    /// <summary>
    /// Runs empty logic.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A Task that represents the work queued to execute in the ThreadPool.</returns>
    protected static Task RunEmptyAsync(CancellationToken cancellationToken = default)
        => Task.Run(ChatCommandGuidanceHelper.RunEmpty, cancellationToken);
}

internal class ChatCommandGuidanceTask
{
    private ChatCommandGuidanceTask(BaseChatCommandGuidanceEngine engine, string key, BaseChatCommandGuidanceProvider command, ChatCommandGuidanceContext context)
    {
        Command = command;
        Args = new ChatCommandGuidanceArgs(context, key);
        Args.AdditionalObject = command.InitializeObject(engine, Args);
        IsAvailable = command.IsAvailable(engine, Args);
    }

    public BaseChatCommandGuidanceProvider Command { get; }

    public ChatCommandGuidanceArgs Args { get; }

    public bool IsAvailable { get; }

    public static IEnumerable<ChatCommandGuidanceTask> Create(BaseChatCommandGuidanceEngine engine, IDictionary<string, BaseChatCommandGuidanceProvider> collection, ChatCommandGuidanceContext context)
    {
        if (collection == null || context == null) yield break;
        foreach (var kvp in collection)
        {
            var key = kvp.Key;
            var command = kvp.Value;
            if (string.IsNullOrEmpty(key) || command == null) continue;
            var task = new ChatCommandGuidanceTask(engine, key, command, context);
            if (task.IsAvailable) yield return task;
        }
    }
}
