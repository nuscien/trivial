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
    /// Gets the command collection.
    /// </summary>
    public ICollection<BaseChatCommandGuidance> Commands => commands.Values;

    /// <summary>
    /// Gets the collection of all command key registered.
    /// </summary>
    public ICollection<string> CommandKeys => commands.Keys;

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="request">The command guidance request.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns></returns>
    public Task<ChatCommandGuidanceResponse> ProcessAsync(ChatCommandGuidanceRequest request, CancellationToken cancellationToken = default)
        => ProcessAsync(new ChatCommandGuidanceContext(request), cancellationToken);

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="context">The command guidance context.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns></returns>
    public async Task<ChatCommandGuidanceResponse> ProcessAsync(ChatCommandGuidanceContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) return null;
        var list = ChatCommandGuidanceHelper.Create(commands, context);
        Task.WaitAll(list.Select(ChatCommandGuidanceHelper.GeneratePromptAsync).ToArray(), cancellationToken);
        var answer = await SendAsync(context, cancellationToken);
        if (answer != null)
        {
            context.SetAnswerMessage(answer);
            var lines = answer.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            foreach (var line in lines)
            {
                var s = line?.Trim() ?? string.Empty;
                if (s.Length < 4 || !s.StartsWith(FuncPrefix)) continue;
                s = s.Substring(FuncPrefix.Length).TrimStart();
                var v = s.Split(new[] { '|' }, StringSplitOptions.None);
                if (v == null || v.Length < 1) continue;
                var key = v[0]?.Trim()?.ToLowerInvariant();
                if (string.IsNullOrEmpty(key)) continue;
                var parameters = v.Skip(1).ToList();
                if (parameters.Count < 1) continue;
                foreach (var command in list)
                {
                    if (command.Args.Command == key) command.Args.Parameters = parameters.AsReadOnly();
                }
            }
        }

        Task.WaitAll(list.Select(ChatCommandGuidanceHelper.PostProcessAsync).ToArray(), cancellationToken);
        return context.GetResponse();
    }

    /// <summary>
    /// Sends the chat to get response.
    /// </summary>
    /// <param name="context">The command guidance context.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns></returns>
    protected abstract Task<string> SendAsync(ChatCommandGuidanceContext context, CancellationToken cancellationToken = default);
}
