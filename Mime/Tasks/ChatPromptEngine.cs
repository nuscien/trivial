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
public abstract class BaseChatPromptEngine
{
    private const string FuncPrefix = "⨍";
    private readonly Dictionary<string, BaseChatCommandGuidance> commands = new();

    /// <summary>
    /// Gets the additional information data.
    /// </summary>
    public JsonObjectNode Info { get; } = new();

    /// <summary>
    /// Gets the app service name.
    /// </summary>
    public virtual string ServiceName { get; }

    /// <summary>
    /// Gets the command collection.
    /// </summary>
    public ICollection<BaseChatCommandGuidance> Commands => commands.Values;

    /// <summary>
    /// Gets the collection of all command key registered.
    /// </summary>
    public ICollection<string> CommandKeys => commands.Keys;

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <typeparam name="T">The type of the command guidance.</typeparam>
    public void Register<T>() where T : BaseChatCommandGuidance
        => Register(Activator.CreateInstance<T>());

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <param name="key">The command key to use.</param>
    /// <typeparam name="T">The type of the command guidance.</typeparam>
    public void Register<T>(string key) where T : BaseChatCommandGuidance
        => Register(key, Activator.CreateInstance<T>());

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <param name="command">The command.</param>
    public void Register(BaseChatCommandGuidance command)
        => Register(command.Command, command);

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <param name="key">The command key to use.</param>
    /// <param name="command">The command.</param>
    public void Register(string key, BaseChatCommandGuidance command)
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
    public BaseChatCommandGuidance GetCommand(string key)
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
    /// Processes.
    /// </summary>
    /// <param name="request">The command guidance request.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The response.</returns>
    public Task<ChatCommandGuidanceResponse> ProcessAsync(ChatCommandGuidanceRequest request, CancellationToken cancellationToken = default)
        => ProcessAsync(new ChatCommandGuidanceContext(request), cancellationToken);

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="context">The command guidance context.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The response.</returns>
    public async Task<ChatCommandGuidanceResponse> ProcessAsync(ChatCommandGuidanceContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) return null;
        var list = ChatCommandGuidanceHelper.Create(commands, context);
        Task.WaitAll(list.Select(ChatCommandGuidanceHelper.GeneratePromptAsync).ToArray(), cancellationToken);
        var prompt = GeneratePrompt(context, context.PromptCollection.ToList());
        var answer = await SendAsync(context, prompt, cancellationToken);
        context.SetAnswerMessage(answer);
        _ = ChatCommandGuidanceHelper.ParseCommands(answer, list, FuncPrefix).Count();
        Task.WaitAll(list.Select(ChatCommandGuidanceHelper.PostProcessAsync).ToArray(), cancellationToken);
        return context.GetResponse();
    }

    /// <summary>
    /// Generates prompt.
    /// </summary>
    /// <param name="context">The command guidance context.</param>
    /// <param name="promptSegments">The prompt segment generated by each command guidance.</param>
    /// <returns>The prompt.</returns>
    protected abstract string GeneratePrompt(ChatCommandGuidanceContext context, IList<string> promptSegments);
    //protected virtual string GeneratePrompt(ChatCommandGuidanceContext context, IList<string> promptSegments)
    //{
    //    var sb = new StringBuilder();
    //    sb.Append("You are a helpful assistant. ");
    //    var nickname = ChatCommandGuidanceHelper.FormatPromptName(context.UserNickname);
    //    if (!string.IsNullOrWhiteSpace(nickname))
    //    {
    //        sb.Append("User's name is ");
    //        sb.Append(nickname);
    //        sb.Append(context.Gender switch
    //        {
    //            Genders.Male => ". He",
    //            Genders.Female => ". She",
    //            _ => ". The user",
    //        });
    //    }
    //    else
    //    {
    //        sb.Append("The user");
    //    }

    //    sb.Append(" is using ");
    //    sb.Append(ChatCommandGuidanceHelper.FormatPromptName(ServiceName));
    //    sb.AppendLine(" which also supports following commands if user asks. The commands are only available for you by adding a new line at the end with character prefix in ⨍, command key, a white space and additional parameter. Don't let user send command directly.");
    //    //var col = null;
    //    sb.AppendLine("|Command key|Command description|");
    //    sb.AppendLine("|----------|------------------------|");
    //    return sb.ToString();
    //}

    /// <summary>
    /// Sends the chat to get response.
    /// </summary>
    /// <param name="context">The command guidance context.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The answer text.</returns>
    protected abstract Task<string> SendAsync(ChatCommandGuidanceContext context, string prompt, CancellationToken cancellationToken = default);
}
