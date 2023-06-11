using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Data;
using Trivial.Users;
using Trivial.Text;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Trivial.Tasks;

/// <summary>
/// The engine to generate smart prompt for chat bot.
/// </summary>
public abstract class BaseChatCommandGuidanceEngine
{
    private const string FuncPrefix = "⨍";
    private const string ParameterSeperator = "⫶";
    private readonly Dictionary<string, BaseChatCommandGuidanceProvider> commands = new();

    /// <summary>
    /// Adds or removes the event on processing.
    /// </summary>
    public DataEventHandler<ChatCommandGuidanceContext> Processing;

    /// <summary>
    /// Adds or removes the event on process succeeded.
    /// </summary>
    public DataEventHandler<ChatCommandGuidanceContext> Processed;

    /// <summary>
    /// Adds or removes the event on process failed.
    /// </summary>
    public DataEventHandler<ChatCommandGuidanceContext> ProcessFailed;

    /// <summary>
    /// Adds or removes the event on source message is sending.
    /// </summary>
    public event DataEventHandler<ChatCommandGuidanceContext> Sending;

    /// <summary>
    /// Adds or removes the event on source message is failed to send or no response.
    /// </summary>
    public event EventHandler<ChatCommandGuidanceErrorEventArgs<ChatCommandGuidanceContext>> SendFailed;

    /// <summary>
    /// Adds or removes the event on source message is received.
    /// </summary>
    public event EventHandler<ChatCommandGuidanceSourceEventArgs> Received;

    /// <summary>
    /// Gets the additional information data.
    /// </summary>
    public JsonObjectNode Info { get; } = new();

    /// <summary>
    /// Gets the full name of the app service.
    /// </summary>
    public virtual string ServiceFullName { get; }

    /// <summary>
    /// Gets the command collection.
    /// </summary>
    public ICollection<BaseChatCommandGuidanceProvider> Commands => commands.Values;

    /// <summary>
    /// Gets the collection of all command key registered.
    /// </summary>
    public ICollection<string> CommandKeys => commands.Keys;

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <typeparam name="T">The type of the command guidance.</typeparam>
    public void Register<T>() where T : BaseChatCommandGuidanceProvider
        => Register(Activator.CreateInstance<T>());

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <param name="key">The command key to use.</param>
    /// <typeparam name="T">The type of the command guidance.</typeparam>
    public void Register<T>(string key) where T : BaseChatCommandGuidanceProvider
        => Register(key, Activator.CreateInstance<T>());

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <param name="command">The command.</param>
    public void Register(BaseChatCommandGuidanceProvider command)
        => Register(command.Command, command);

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <param name="key">The command key to use.</param>
    /// <param name="command">The command.</param>
    public void Register(string key, BaseChatCommandGuidanceProvider command)
    {
        key ??= command?.Command;
        if (key == null) return;
        key = key.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(key) || key.Length < 2) return;
        if (command == null)
        {
            commands.Remove(key);
            return;
        }
        
        commands[key] = command;
        command.OnInit(this);
    }

    /// <summary>
    /// Tries to get the specific command.
    /// </summary>
    /// <param name="key">The command key.</param>
    /// <returns>The command guidance</returns>
    public BaseChatCommandGuidanceProvider GetCommand(string key)
    {
        key = key?.Trim()?.ToLowerInvariant();
        if (string.IsNullOrEmpty(key)) return null;
        return commands.TryGetValue(key, out var result) ? result : null;
    }

    /// <summary>
    /// Removes a specific command.
    /// </summary>
    /// <param name="key">The command key.</param>
    /// <returns>true if remove succeeded; otherwise, false.</returns>
    public bool RemoveCommand(string key)
        => commands.Remove(key);

    /// <summary>
    /// Projects each element of a sequence into a new form by incorporating the element's key.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by selector.</typeparam>
    /// <param name="select">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
    /// <returns>A collection whose elements are the result of invoking the transform function on each element of source.</returns>
    public IEnumerable<T> SelectCommand<T>(Func<string, BaseChatCommandGuidanceProvider, T> select)
    {
        if (select == null) yield break;
        foreach (var kvp in commands)
        {
            if (kvp.Value == null) continue;
            yield return select(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Projects each element of a sequence into a new form by incorporating the element's key.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by selector.</typeparam>
    /// <param name="kind">A filter by command guidance kind.</param>
    /// <param name="select">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
    /// <returns>A collection whose elements are the result of invoking the transform function on each element of source.</returns>
    public IEnumerable<T> SelectCommand<T>(ChatCommandGuidanceProviderKinds kind, Func<string, BaseChatCommandGuidanceProvider, T> select)
    {
        if (select == null) yield break;
        foreach (var kvp in commands)
        {
            if (kvp.Value?.Kind != kind) continue;
            yield return select(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="request">The command guidance request.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The response.</returns>
    public async Task<ChatCommandGuidanceResponse> ProcessAsync(ChatCommandGuidanceRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) return null;
        var context = new ChatCommandGuidanceContext(request);
        var list = ChatCommandGuidanceHelper.Create(commands, context);
        var args = new DataEventArgs<ChatCommandGuidanceContext>(context);
        Processing?.Invoke(this, args);
        Task.WaitAll(list.Select(ChatCommandGuidanceHelper.GeneratePromptAsync).ToArray(), cancellationToken);
        var prompt = GenerateDefaultPrompt(context);
        prompt = ChatCommandGuidanceHelper.JoinWithEmptyLine(prompt, context.PromptCollection.Where(ChatCommandGuidanceHelper.IsNotEmpty));
        Sending?.Invoke(this, new DataEventArgs<ChatCommandGuidanceContext>(context));
        ChatCommandGuidanceSourceResult answer;
        try
        {
            answer = await SendAsync(context, prompt, cancellationToken);
            answer ??= new(null, false, null);
        }
        catch (Exception ex)
        {
            SendFailed?.Invoke(this, new ChatCommandGuidanceErrorEventArgs<ChatCommandGuidanceContext>(ex, context));
            throw;
        }

        Received?.Invoke(this, new ChatCommandGuidanceSourceEventArgs(context, answer));
        context.SetAnswerMessage(answer.Message, answer.Kind);
        if (answer.IsSuccessful)
        {
            var result = ChatCommandGuidanceHelper.ParseCommands(answer.Message, list, FuncPrefix).ToList();
            Task.WaitAll(result.Select(ChatCommandGuidanceHelper.PostProcessAsync).ToArray(), cancellationToken);
            OnCommandProccessed(result.Select(ele => ele.Args).ToList());
            Processed?.Invoke(this, args);
        }
        else
        {
            ProcessFailed?.Invoke(this, args);
        }

        return context.GetResponse();
    }

    /// <summary>
    /// Occurs on the command is processed.
    /// </summary>
    /// <param name="args">The arguments.</param>
    protected virtual void OnCommandProccessed(IList<ChatCommandGuidanceArgs> args)
    {
    }

    /// <summary>
    /// Generates default prompt.
    /// </summary>
    /// <param name="context">The command guidance context.</param>
    /// <returns>The prompt.</returns>
    protected virtual string GenerateDefaultPrompt(ChatCommandGuidanceContext context)
    {
        var sb = new StringBuilder();
        sb.Append("You are a helpful assistant. ");
        var nickname = ChatCommandGuidanceHelper.FormatPromptName(context.UserNickname);
        if (!string.IsNullOrWhiteSpace(nickname))
        {
            sb.Append("User's name is ");
            sb.Append(nickname);
            sb.Append(context.Gender switch
            {
                Genders.Male => ". He",
                Genders.Female => ". She",
                _ => ". The user",
            });
        }
        else
        {
            sb.Append("The user");
        }

        sb.Append(" is using ");
        sb.Append(ChatCommandGuidanceHelper.FormatPromptName(ServiceFullName) ?? "the service");
        sb.Append(" which also supports following commands if user asks for or is talking about. The commands are only available for you by adding a new line at the end of answer with character prefix in ");
        sb.Append(FuncPrefix);
        sb.Append(", command key, a seperator character and additional parameter. Each parameters are separeted by the seperator character. The seperator character is ");
        sb.Append(ParameterSeperator);
        sb.Append(". Don't let user send command directly.");
        var col = commands.Where(ChatCommandGuidanceHelper.IsSupportedCommand).ToList();
        sb.AppendLine("|Command key|Command description|Parameter description|Command kind|");
        sb.AppendLine("|----------|------------------------|------------------------|------|");
        if (col.Count > 0)
        {
            foreach (var kvp in col)
            {
                var command = kvp.Value;
                sb.Append('|');
                sb.Append(command.Command);
                sb.Append('|');
                sb.Append(command.Description);
                sb.Append('|');
                sb.Append(command.ParameterDescription);
                sb.Append('|');
                sb.Append(command.Kind);
                sb.Append('|');
            }
        }
        else
        {
            sb.Append("|help|Get help.|None.|Command|");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Sends the chat to get response.
    /// </summary>
    /// <param name="context">The command guidance context.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The answer text.</returns>
    protected abstract Task<ChatCommandGuidanceSourceResult> SendAsync(ChatCommandGuidanceContext context, string prompt, CancellationToken cancellationToken = default);
}
