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
    public ChatCommandGuidanceArgs(ChatCommandGuidanceContext context, string command)
    {
        Context = context;
        Command = command;
        AnswerData = context?.GetAnswerData(command) ?? new();
        Info = context?.GetInfo(command) ?? new();
        NextInfo = context?.GetNextInfo(command) ?? new();
    }

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
    /// Tests if the command guidance is available.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>true if the command guidance is available; otherwise, false.</returns>
    protected internal virtual bool IsAvailable(ChatCommandGuidanceArgs args)
        => args != null;

    /// <summary>
    /// Generates the prompt.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>The prompt segment to append.</returns>
    protected internal virtual Task<string> GeneratePromptAsync(ChatCommandGuidanceArgs args)
        => Task.FromResult<string>(null);

    /// <summary>
    /// Post processes.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>A Task that represents the work queued to execute in the ThreadPool.</returns>
    protected internal abstract Task PostProcessAsync(ChatCommandGuidanceArgs args);

    /// <summary>
    /// Processes on client side.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>A Task that represents the work queued to execute in the ThreadPool.</returns>
    protected internal virtual Task ClientProcessAsync(ChatCommandGuidanceClientProcessingArgs args)
        => RunEmptyAsync();

    /// <summary>
    /// Runs empty logic.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A Task that represents the work queued to execute in the ThreadPool.</returns>
    protected static Task RunEmptyAsync(CancellationToken cancellationToken = default)
        => Task.Run(ChatCommandGuidanceHelper.RunEmpty, cancellationToken);
}
