using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Text;
using Trivial.Users;

namespace Trivial.Tasks;

/// <summary>
/// The helper of chat command guidance.
/// </summary>
public static class ChatCommandGuidanceHelper
{
    /// <summary>
    /// Creates a client.
    /// </summary>
    /// <param name="user">The user instance.</param>
    /// <param name="engine">The engine.</param>
    /// <returns>The client.</returns>
    public static BaseChatCommandGuidanceClient CreateClient(UserItemInfo user, BaseChatCommandGuidanceEngine engine)
        => new LocalChatCommandGuidanceClient(user, engine);

    /// <summary>
    /// Tests if the command is valid.
    /// </summary>
    /// <param name="kvp">The key value pair.</param>
    /// <returns>true if it is supported; otherwise, false.</returns>
    internal static bool IsSupportedCommand(KeyValuePair<string, BaseChatCommandGuidanceProvider> kvp)
    {
        if (kvp.Value == null) return false;
        return kvp.Value.Kind switch
        {
            ChatCommandGuidanceProviderKinds.Command or ChatCommandGuidanceProviderKinds.Assistance or ChatCommandGuidanceProviderKinds.Recorder or ChatCommandGuidanceProviderKinds.Others => true,
            _ => false
        };
    }

    internal static IEnumerable<ChatCommandGuidanceTask> Create(BaseChatCommandGuidanceEngine engine, IDictionary<string, BaseChatCommandGuidanceProvider> collection, ChatCommandGuidanceContext context)
    {
        if (collection == null || context == null) yield break;
        foreach (var kvp in collection)
        {
            var key = kvp.Key;
            var command = kvp.Value;
            if (string.IsNullOrEmpty(key) || command == null) continue;
            var task = new ChatCommandGuidanceTask(key, command, context);
            if (task.IsAvailable(engine)) yield return task;
        }
    }

    internal static bool IsAvailable(this ChatCommandGuidanceTask instance, BaseChatCommandGuidanceEngine engine)
        => instance.Command.IsAvailable(engine, instance.Args);

    internal static async Task<string> GeneratePromptAsync(this ChatCommandGuidanceTask instance, BaseChatCommandGuidanceEngine engine, CancellationToken cancellationToken = default)
    {
        var prompt = await instance.Command.GeneratePromptAsync(engine, instance.Args, cancellationToken);
        prompt = prompt?.Trim();
        if (!string.IsNullOrEmpty(prompt)) instance.Args.Context.PromptCollection.Add(prompt);
        return prompt;
    }

    internal static Task<string[]> GeneratePromptAsync(this IEnumerable<ChatCommandGuidanceTask> col, BaseChatCommandGuidanceEngine engine, CancellationToken cancellationToken = default)
    {
        var list = new List<Task<string>>();
        foreach (var item in col)
        {
            var task = item.GeneratePromptAsync(engine, cancellationToken);
            if (task != null) list.Add(task);
        }

        return Task.WhenAll(list.ToArray());
    }

    internal static Task PostProcessAsync(this ChatCommandGuidanceTask instance, BaseChatCommandGuidanceEngine engine, CancellationToken cancellationToken = default)
        => instance.Command.PostProcessAsync(engine, instance.Args, cancellationToken);

    internal static Task PostProcessAsync(this IEnumerable<ChatCommandGuidanceTask> col, BaseChatCommandGuidanceEngine engine, CancellationToken cancellationToken = default)
    {
        var list = new List<Task>();
        foreach (var item in col)
        {
            var task = item.PostProcessAsync(engine, cancellationToken);
            if (task != null) list.Add(task);
        }

        return Task.WhenAll(list.ToArray());
    }

    internal static IEnumerable<ChatCommandGuidanceTask> ParseCommands(string answer, IEnumerable<ChatCommandGuidanceTask> list, string funcPrefix)
    {
        if (answer == null || list == null) yield break;
        var lines = answer.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        foreach (var line in lines)
        {
            var s = line?.Trim() ?? string.Empty;
            if (s.Length < 4 || !s.StartsWith(funcPrefix)) continue;
            s = s.Substring(funcPrefix.Length).TrimStart();
            var v = s.Split(new[] { '|' }, StringSplitOptions.None);
            if (v == null || v.Length < 1) continue;
            var key = v[0]?.Trim()?.ToLowerInvariant();
            if (string.IsNullOrEmpty(key)) continue;
            var parameters = v.Skip(1).ToList();
            foreach (var command in list)
            {
                if (command.Args.Command != key) continue;
                command.Args.Parameters = parameters.AsReadOnly();
                yield return command;
            }
        }
    }

    internal static JsonObjectNode ToJson(Dictionary<string, JsonObjectNode> col)
    {
        if (col == null) return null;
        var json = new JsonObjectNode();
        foreach (var item in col)
        {
            json[item.Key] = item.Value;
        }

        return json;
    }

    internal static IEnumerable<SimpleChatMessage> DeserializeChatMessages(JsonArrayNode arr)
    {
        if (arr == null) yield break;
        foreach (var item in arr)
        {
            if (item is not JsonObjectNode json) continue;
            yield return json;
        }
    }

    internal static JsonArrayNode Serizalize(IEnumerable<SimpleChatMessage> value)
    {
        if (value == null) return null;
        var col = new JsonArrayNode();
        foreach (var item in value)
        {
            col.Add((JsonObjectNode)item);
        }

        return col;
    }

    internal static bool IsNotEmpty(string s)
        => !string.IsNullOrWhiteSpace(s);

    /// <summary>
    /// Generates prompt.
    /// </summary>
    /// <param name="line">The default prompt.</param>
    /// <param name="lines">The prompt segment generated by each command guidance.</param>
    /// <returns>The prompt.</returns>
    internal static string JoinWithEmptyLine(string line, IEnumerable<string> lines)
    {
        var sb = new StringBuilder(line);
        sb.AppendLine();
        sb.AppendLine();
        lines ??= new List<string>();
        foreach (var item in lines)
        {
            sb.AppendLine(item);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    internal static string FormatPromptName(string s)
        => string.IsNullOrWhiteSpace(s) ? null : s.Trim().Replace(".", string.Empty).Replace(",", string.Empty).Replace(";", string.Empty).Replace("!", string.Empty).Replace("?", string.Empty).Replace("|", string.Empty).Replace("\"", string.Empty).Replace("\'", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("…", string.Empty).Replace("。", string.Empty).Replace("，", string.Empty).Replace("！", string.Empty).Replace("？", string.Empty);

    internal static void RunEmpty()
    {
    }
}

internal class ChatCommandGuidanceTask
{
    public ChatCommandGuidanceTask(string key, BaseChatCommandGuidanceProvider command, ChatCommandGuidanceContext context)
    {
        Command = command;
        Args = new ChatCommandGuidanceArgs(context, key);
    }

    public BaseChatCommandGuidanceProvider Command { get; }

    public ChatCommandGuidanceArgs Args { get; }
}
