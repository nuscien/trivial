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
internal static class ChatCommandGuidanceHelper
{
    /// <summary>
    /// Tests if the command is valid.
    /// </summary>
    /// <param name="kvp">The key value pair.</param>
    /// <returns>true if it is supported; otherwise, false.</returns>
    public static bool IsSupportedCommand(KeyValuePair<string, BaseChatCommandGuidanceProvider> kvp)
    {
        if (kvp.Value == null) return false;
        return kvp.Value.Kind switch
        {
            ChatCommandGuidanceProviderKinds.Command or ChatCommandGuidanceProviderKinds.Assistance or ChatCommandGuidanceProviderKinds.Recorder or ChatCommandGuidanceProviderKinds.Others => true,
            _ => false
        };
    }

    public static async Task<string> GeneratePromptAsync(this ChatCommandGuidanceTask instance, BaseChatCommandGuidanceEngine engine, CancellationToken cancellationToken = default)
    {
        var prompt = await instance.Command.GeneratePromptAsync(engine, instance.Args, cancellationToken);
        prompt = prompt?.Trim();
        if (!string.IsNullOrEmpty(prompt)) instance.Args.Context.PromptCollection.Add(prompt);
        return prompt;
    }

    public static Task<string[]> GeneratePromptAsync(this IEnumerable<ChatCommandGuidanceTask> col, BaseChatCommandGuidanceEngine engine, CancellationToken cancellationToken = default)
    {
        var list = new List<Task<string>>();
        foreach (var item in col)
        {
            var task = item.GeneratePromptAsync(engine, cancellationToken);
            if (task != null) list.Add(task);
        }

        return Task.WhenAll(list.ToArray());
    }

    public static Task PostProcessAsync(this ChatCommandGuidanceTask instance, BaseChatCommandGuidanceEngine engine, CancellationToken cancellationToken = default)
        => instance.Command.PostProcessAsync(engine, instance.Args, cancellationToken);

    public static Task PostProcessAsync(this IEnumerable<ChatCommandGuidanceTask> col, BaseChatCommandGuidanceEngine engine, CancellationToken cancellationToken = default)
    {
        var list = new List<Task>();
        foreach (var item in col)
        {
            var task = item.PostProcessAsync(engine, cancellationToken);
            if (task != null) list.Add(task);
        }

        return Task.WhenAll(list.ToArray());
    }

    public static IEnumerable<ChatCommandGuidanceTask> ParseCommands(string answer, IEnumerable<ChatCommandGuidanceTask> list, string funcPrefix)
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

    /// <summary>
    /// Occurs on request.
    /// </summary>
    /// <param name="col">The collection.</param>
    /// <param name="engine">The engine.</param>
    /// <param name="userDetails">The user information in details.</param>
    public static void OnRequest(this IEnumerable<ChatCommandGuidanceEngineMonitorTask> col, BaseChatCommandGuidanceEngine engine, JsonObjectNode userDetails)
    {
        foreach (var item in col)
        {
            item.Monitor.OnRequest(engine, item.Context, userDetails, item.Ref);
        }
    }

    /// <summary>
    /// Generates the prompt.
    /// </summary>
    /// <param name="col">The collection.</param>
    /// <param name="engine">The engine.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The prompt segment to append.</returns>
    public static async Task<List<string>> GeneratePromptAsync(this IEnumerable<ChatCommandGuidanceEngineMonitorTask> col, BaseChatCommandGuidanceEngine engine, CancellationToken cancellationToken = default)
    {
        var list = new List<string>();
        foreach (var item in col)
        {
            var s = await item.Monitor.GeneratePromptAsync(engine, item.Context, item.Ref, cancellationToken);
            list.Add(s);
        }

        return list;
    }

    /// <summary>
    /// Occurs on sending failure.
    /// </summary>
    /// <param name="col">The collection.</param>
    /// <param name="engine">The engine.</param>
    /// <param name="ex">The exception.</param>
    public static void OnSendError(this IEnumerable<ChatCommandGuidanceEngineMonitorTask> col, BaseChatCommandGuidanceEngine engine, Exception ex)
    {
        foreach (var item in col)
        {
            item.Monitor.OnSendError(engine, item.Context, ex, item.Ref);
        }
    }

    /// <summary>
    /// Occurs on answer receive.
    /// </summary>
    /// <param name="col">The collection.</param>
    /// <param name="engine">The engine.</param>
    /// <param name="answer">The answer.</param>
    public static void OnReceive(this IEnumerable<ChatCommandGuidanceEngineMonitorTask> col, BaseChatCommandGuidanceEngine engine, ChatCommandGuidanceSourceResult answer)
    {
        foreach (var item in col)
        {
            item.Monitor.OnReceive(engine, item.Context, answer, item.Ref);
        }
    }

    /// <summary>
    /// Occurs on response returning.
    /// </summary>
    /// <param name="col">The collection.</param>
    /// <param name="engine">The engine.</param>
    public static void OnResponse(this IEnumerable<ChatCommandGuidanceEngineMonitorTask> col, BaseChatCommandGuidanceEngine engine)
    {
        foreach (var item in col)
        {
            item.Monitor.OnResponse(engine, item.Context, item.Ref);
        }
    }

    public static JsonObjectNode ToJson(Dictionary<string, JsonObjectNode> col)
    {
        if (col == null) return null;
        var json = new JsonObjectNode();
        foreach (var item in col)
        {
            json[item.Key] = item.Value;
        }

        return json;
    }

    public static IEnumerable<SimpleChatMessage> DeserializeChatMessages(JsonArrayNode arr)
    {
        if (arr == null) yield break;
        foreach (var item in arr)
        {
            if (item is not JsonObjectNode json) continue;
            yield return json;
        }
    }

    public static JsonArrayNode Serizalize(IEnumerable<SimpleChatMessage> value)
    {
        if (value == null) return null;
        var col = new JsonArrayNode();
        foreach (var item in value)
        {
            col.Add((JsonObjectNode)item);
        }

        return col;
    }

    public static bool IsNotEmpty(string s)
        => !string.IsNullOrWhiteSpace(s);

    /// <summary>
    /// Generates prompt.
    /// </summary>
    /// <param name="line">The default prompt.</param>
    /// <param name="lines">The prompt segment generated by each command guidance.</param>
    /// <returns>The prompt.</returns>
    public static string JoinWithEmptyLine(string line, IEnumerable<string> lines)
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

    public static string FormatPromptName(string s)
        => string.IsNullOrWhiteSpace(s) ? null : s.Trim().Replace(".", string.Empty).Replace(",", string.Empty).Replace(";", string.Empty).Replace("!", string.Empty).Replace("?", string.Empty).Replace("|", string.Empty).Replace("\"", string.Empty).Replace("\'", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("…", string.Empty).Replace("。", string.Empty).Replace("，", string.Empty).Replace("！", string.Empty).Replace("？", string.Empty);

    public static void RunEmpty()
    {
    }
}
