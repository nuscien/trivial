using System;
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
/// The request of chat command guidance.
/// </summary>
public class ChatCommandGuidanceRequest
{
    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceRequest class.
    /// </summary>
    /// <param name="user">The user instance.</param>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="history">The chat history.</param>
    /// <param name="clientContextData">The context data from client.</param>
    /// <param name="response">The latest response.</param>
    public ChatCommandGuidanceRequest(UserItemInfo user, string message, JsonObjectNode data, IEnumerable<SimpleChatMessage> history, JsonObjectNode clientContextData = null, ChatCommandGuidanceResponse response = null)
    {
        User = user;
        Message = message;
        Data = data ?? new();
        History = history ?? new List<SimpleChatMessage>();
        ClientContextData = clientContextData ?? new();
        TrackingId = response?.TrackingId ?? Guid.NewGuid();
        Info = response?.Info ?? new();
    }

    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceRequest class.
    /// </summary>
    /// <param name="user">The user instance.</param>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="history">The chat history.</param>
    /// <param name="clientContextData">The context data from client.</param>
    /// <param name="trackingId">The tracking identifier.</param>
    /// <param name="info">The additional information from latest response.</param>
    internal ChatCommandGuidanceRequest(UserItemInfo user, string message, JsonObjectNode data, IEnumerable<SimpleChatMessage> history, JsonObjectNode clientContextData, Guid trackingId, JsonObjectNode info)
    {
        User = user;
        Message = message;
        Data = data ?? new();
        History = history ?? new List<SimpleChatMessage>();
        ClientContextData = clientContextData ?? new();
        TrackingId = trackingId ;
        Info = info ?? new();
    }

    /// <summary>
    /// Gets the user instance.
    /// </summary>
    internal UserItemInfo User { get; }

    /// <summary>
    /// Gets the message identifier.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets the nickname of the sender.
    /// </summary>
    public string UserNickname => User.Nickname;

    /// <summary>
    /// Gets the sender identifier.
    /// </summary>
    public string UserId => User.Id;

    /// <summary>
    /// Gets the gender of the sender.
    /// </summary>
    public Genders Gender => User.Gender;

    /// <summary>
    /// Gets the URI of the sender avatar.
    /// </summary>
    public Uri UserAvatar => User.AvatarUri;

    /// <summary>
    /// Gets the tracking identifier.
    /// </summary>
    public Guid TrackingId { get; private set; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the data.
    /// </summary>
    public JsonObjectNode Data { get; }

    /// <summary>
    /// Gets the information.
    /// </summary>
    public JsonObjectNode Info { get; private set; }

    /// <summary>
    /// Gets the chat history.
    /// </summary>
    public IEnumerable<SimpleChatMessage> History { get; }

    /// <summary>
    /// Gets the context data from client.
    /// </summary>
    public JsonObjectNode ClientContextData { get; }

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>The request instance.</returns>
    public static implicit operator ChatCommandGuidanceRequest(JsonObjectNode value)
    {
        if (value is null) return null;
        return new(value.TryGetObjectValue("sender"), value.TryGetStringValue("text") ?? value.TryGetStringValue("message"), value.TryGetObjectValue("data"), ChatCommandGuidanceHelper.DeserializeChatMessages(value.TryGetArrayValue("history")), value.TryGetObjectValue("ref"))
        {
            TrackingId = value.TryGetGuidValue("tracking") ?? Guid.NewGuid(),
            Info = value.TryGetObjectValue("info"),
        };
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator JsonObjectNode(ChatCommandGuidanceRequest value)
    {
        if (value is null) return null;
        return new()
        {
            { "sender", (JsonObjectNode)value.User },
            { "trackig", value.TrackingId },
            { "text", value.Message },
            { "data", value.Data },
            { "info", value.Info},
            { "history", ChatCommandGuidanceHelper.Serizalize(value.History) },
            { "ref", value.ClientContextData }
        };
    }
}

/// <summary>
/// The response of chat command guidance.
/// </summary>
public class ChatCommandGuidanceResponse
{
    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceResponse class.
    /// </summary>
    /// <param name="message">The answer message text.</param>
    /// <param name="data">The answer data.</param>
    /// <param name="info">The information data for context.</param>
    /// <param name="kind">The message kind.</param>
    /// <param name="request">The request message.</param>
    internal ChatCommandGuidanceResponse(string message, JsonObjectNode data, JsonObjectNode info = null, string kind = null, ChatCommandGuidanceRequest request = null)
    {
        Id = Guid.NewGuid();
        RequestId = request?.Id ?? Guid.Empty;
        TrackingId = request?.TrackingId ?? Guid.NewGuid();
        Message = message;
        Data = data;
        Info = info;
        Kind = kind;
        ClientContextData = request?.ClientContextData;
    }

    /// <summary>
    /// Gets the message identifier.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of request message.
    /// </summary>
    public Guid RequestId { get; private set; }

    /// <summary>
    /// Gets the tracking identifier.
    /// </summary>
    public Guid TrackingId { get; private set; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the response data.
    /// </summary>
    public JsonObjectNode Data { get; }

    /// <summary>
    /// Gets the details.
    /// </summary>
    public Dictionary<string, JsonObjectNode> Details { get; } = new();

    /// <summary>
    /// Gets the information.
    /// </summary>
    public JsonObjectNode Info { get; }

    /// <summary>
    /// Gets the message kind.
    /// </summary>
    public string Kind { get; }

    /// <summary>
    /// Gets the context data from client.
    /// </summary>
    public JsonObjectNode ClientContextData { get; private set; }

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>The response instance.</returns>
    public static implicit operator ChatCommandGuidanceResponse(JsonObjectNode value)
    {
        if (value is null) return null;
        var result = new ChatCommandGuidanceResponse(
            value.TryGetStringValue("text") ?? value.TryGetStringValue("message"),
            value.TryGetObjectValue("data"),
            value.TryGetObjectValue("info"),
            value.TryGetStringTrimmedValue("kind", true))
        {
            Id = value.TryGetGuidValue("id") ?? Guid.NewGuid(),
            RequestId = value.TryGetGuidValue("request") ?? Guid.Empty,
            ClientContextData = value.TryGetObjectValue("ref")
        };
        var detailsArr = value.TryGetObjectValue("details") ?? new();
        foreach (var item in detailsArr)
        {
            if (item.Value is not JsonObjectNode json) continue;
            result.Details[item.Key] = json;
        }

        return result;
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator JsonObjectNode(ChatCommandGuidanceResponse value)
    {
        if (value is null) return null;
        return new()
        {
            { "id", value.Id },
            { "trackig", value.TrackingId },
            { "text", value.Message },
            { "data", value.Data },
            { "details", ChatCommandGuidanceHelper.ToJson(value.Details) },
            { "info", value.Info},
            { "ref", value.ClientContextData }
        };
    }
}

/// <summary>
/// The result of chat command guidance.
/// </summary>
public class ChatCommandGuidanceSourceResult
{
    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceSourceResult class.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="success">A flag indicating whether the result is successful.</param>
    /// <param name="kind">The message kind.</param>
    public ChatCommandGuidanceSourceResult(string message, bool success, string kind)
    {
        Message = message;
        IsSuccessful = success;
        Kind = ChatCommandGuidanceHelper.FormatPromptName(kind);
    }

    /// <summary>
    /// Gets the message text.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets a value indicating whether the result is successful.
    /// </summary>
    public bool IsSuccessful { get; }

    /// <summary>
    /// Gets the message kind.
    /// </summary>
    public string Kind { get; }
}
