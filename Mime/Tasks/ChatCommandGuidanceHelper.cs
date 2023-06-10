using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Text;

namespace Trivial.Tasks;

internal static class ChatCommandGuidanceHelper
{
    public static IEnumerable<ChatCommandGuidanceTask> Create(IDictionary<string, BaseChatCommandGuidance> collection, ChatCommandGuidanceContext context)
    {
        if (collection == null || context == null) yield break;
        foreach (var kvp in collection)
        {
            var key = kvp.Key;
            var command = kvp.Value;
            if (string.IsNullOrEmpty(key) || command == null) continue;
            var task = new ChatCommandGuidanceTask(key, command, context);
            if (task.IsAvailable()) yield return task;
        }
    }

    public static bool IsAvailable(this ChatCommandGuidanceTask instance)
        => instance.Command.IsAvailable(instance.Args);

    public static async Task<string> GeneratePromptAsync(this ChatCommandGuidanceTask instance)
    {
        var prompt = await instance.Command.GeneratePromptAsync(instance.Args);
        prompt = prompt?.Trim();
        if (!string.IsNullOrEmpty(prompt)) instance.Args.Context.PromptCollection.Add(prompt);
        return prompt;
    }

    public static Task PostProcessAsync(this ChatCommandGuidanceTask instance)
        => instance.Command.PostProcessAsync(instance.Args);

    public static IEnumerable<string> ParseCommands(string answer, IEnumerable<ChatCommandGuidanceTask> list, string funcPrefix)
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
            yield return s;
            var parameters = v.Skip(1).ToList();
            if (parameters.Count < 1) continue;
            foreach (var command in list)
            {
                if (command.Args.Command == key) command.Args.Parameters = parameters.AsReadOnly();
            }
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
    public static string FormatPromptName(string s)
        => string.IsNullOrWhiteSpace(s) ? null : s.Trim().Replace(".", string.Empty).Replace(",", string.Empty).Replace(";", string.Empty).Replace("!", string.Empty).Replace("?", string.Empty).Replace("|", string.Empty).Replace("\"", string.Empty).Replace("\'", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("…", string.Empty).Replace("。", string.Empty).Replace("，", string.Empty).Replace("！", string.Empty).Replace("？", string.Empty);

    public static void RunEmpty()
    {
    }
}

internal class ChatCommandGuidanceTask
{
    public ChatCommandGuidanceTask(string key, BaseChatCommandGuidance command, ChatCommandGuidanceContext context)
    {
        Command = command;
        Args = new ChatCommandGuidanceArgs(context, key);
    }

    public BaseChatCommandGuidance Command { get; }

    public ChatCommandGuidanceArgs Args { get; }
}
