﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Data;
using Trivial.Users;
using Trivial.Text;

namespace Trivial.Tasks;

/// <summary>
/// The context of the command guidance for the chat bot.
/// </summary>
public class ChatCommandGuidanceContext
{
    private readonly JsonObjectNode infos;
    private readonly Dictionary<string, JsonObjectNode> nextData = new();
    private readonly Dictionary<string, JsonObjectNode> nextInfo = new();
    private readonly ChatCommandGuidanceRequest request;

    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceContext class.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="additionalRequestInfo">The additional request information.</param>
    public ChatCommandGuidanceContext(ChatCommandGuidanceRequest request, object additionalRequestInfo = null)
    {
        NextInfo = new();
        nextInfo["_"] = NextInfo;
        this.request = request ?? new(null, null, null, null);
        UserMessageData = request.Data;
        var history = request.History;
        History = history == null ? new() : new(history);
        Info = request.Info?.TryGetObjectValue("_");
        infos = request.Info;
        ClientInfo = request.ClientInfo;
        AdditionalRequestInfo = additionalRequestInfo;
    }

    /// <summary>
    /// Gets the instance identifier.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets the tracking identifier.
    /// </summary>
    public Guid TrackingId => request.TrackingId;

    /// <summary>
    /// Gets the creation date time.
    /// </summary>
    public DateTime CreationTime { get; } = DateTime.Now;

    /// <summary>
    /// Gets the replied date time.
    /// </summary>
    public DateTime? RepliedTime { get; private set; }

    /// <summary>
    /// Gets the chat message from sender.
    /// </summary>
    public string UserMessage => request.Message;

    /// <summary>
    /// Gets the chat message data from sender.
    /// </summary>
    public JsonObjectNode UserMessageData { get; }

    /// <summary>
    /// Gets the original chat message result.
    /// </summary>
    public string OriginalAnswerMessage { get; private set; }

    /// <summary>
    /// Gets the chat message result.
    /// </summary>
    public string AnswerMessage { get; private set; }

    /// <summary>
    /// Gets the message kind.
    /// </summary>
    public string MessageKind { get; private set; }

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public string UserId => request.User?.Id;

    /// <summary>
    /// Gets the user nickname.
    /// </summary>
    public string UserNickname => request.User?.Nickname;

    /// <summary>
    /// Gets the user gender.
    /// </summary>
    public Genders Gender => request.User?.Gender ?? Genders.Unknown;

    /// <summary>
    /// Gets the URI of the user avatar.
    /// </summary>
    public Uri UserAvatar => request.User?.AvatarUri;

    /// <summary>
    /// Gets the history.
    /// </summary>
    public List<SimpleChatMessage> History { get; }

    /// <summary>
    /// Gets the context information.
    /// The command guidance can access this to store useful data during this round.
    /// </summary>
    public JsonObjectNode Info { get; }

    /// <summary>
    /// Gets next context information for context.
    /// </summary>
    public JsonObjectNode NextInfo { get; }

    /// <summary>
    /// Gets the data of the business rich output.
    /// </summary>
    public JsonObjectNode AnswerData { get; } = new();

    /// <summary>
    /// Gets the client information.
    /// </summary>
    public JsonObjectNode ClientInfo { get; }

    /// <summary>
    /// Gets the additional request information.
    /// </summary>
    public object AdditionalRequestInfo { get; }

    /// <summary>
    /// Gets or set the tag object.
    /// </summary>
    public object Tag { get; set; }

    /// <summary>
    /// Gets the prompt collection generated by each command guidance.
    /// </summary>
    internal SynchronizedList<string> PromptCollection { get; } = new();

    /// <summary>
    /// Rewrites the answer message.
    /// </summary>
    /// <param name="value">The message.</param>
    public void RewriteAnswerMessage(string value)
        => AnswerMessage = value;

    /// <summary>
    /// Rewrites the answer message.
    /// </summary>
    /// <param name="value">The message.</param>
    /// <param name="additionalNewLine">true if append an empty line before original message.</param>
    public void AppendAnswerMessage(string value, bool additionalNewLine = false)
        => AnswerMessage = string.IsNullOrWhiteSpace(OriginalAnswerMessage) ? value : string.Concat(AnswerMessage, Environment.NewLine, additionalNewLine ? Environment.NewLine : string.Empty, value);

    /// <summary>
    /// Gets the JSON object of response.
    /// </summary>
    /// <returns>The response JSON object.</returns>
    public ChatCommandGuidanceResponse GetResponse()
    {
        var resp = new ChatCommandGuidanceResponse(AnswerMessage, AnswerData, Info, MessageKind, request);
        foreach (var item in nextData)
        {
            resp.Details[item.Key] = item.Value;
        }

        return resp;
    }

    /// <summary>
    /// Sets the answer message.
    /// </summary>
    /// <param name="value">The message text.</param>
    /// <param name="kind">The message kind.</param>
    internal void SetAnswerMessage(string value, string kind)
    {
        RepliedTime = DateTime.Now;
        AnswerMessage = value;
        OriginalAnswerMessage = value;
        MessageKind = kind;
    }

    /// <summary>
    /// Gets the information data.
    /// </summary>
    /// <param name="key">The command key.</param>
    /// <returns>The information data.</returns>
    internal JsonObjectNode GetInfo(string key)
        => key == null ? Info : infos?.TryGetObjectValue(key);

    /// <summary>
    /// Gets the next information data of a specific command.
    /// </summary>
    /// <param name="command">The command key.</param>
    /// <param name="init">true if ensures; otherwise, false.</param>
    /// <returns>The next information data for context.</returns>
    internal JsonObjectNode GetNextInfo(string command, bool init = false)
    {
        if (command == null) return NextInfo;
        command = command.Trim().ToLowerInvariant();
        if (nextInfo.TryGetValue(command, out var result)) return result;
        if (!init) return null;
        result = new();
        nextInfo[command] = result;
        return result;
    }

    /// <summary>
    /// Gets the answer data of a specific command.
    /// </summary>
    /// <param name="command">The command key.</param>
    /// <param name="init">true if ensures; otherwise, false.</param>
    /// <returns>The data for rich output.</returns>
    internal JsonObjectNode GetAnswerData(string command, bool init = false)
    {
        if (command == null) return AnswerData;
        command = command.Trim().ToLowerInvariant();
        if (nextData.TryGetValue(command, out var result)) return result;
        if (!init) return null;
        result = new();
        nextData[command] = result;
        return result;
    }
}
