using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Data;
using Trivial.Users;
using Trivial.Text;
using System.Threading;

namespace Trivial.Tasks;

/// <summary>
/// Creates a request.
/// </summary>
/// <param name="message">The message text.</param>
/// <param name="data">The message data.</param>
/// <param name="response">The response to reply.</param>
/// <param name="cancellationToken">The optional cancellation token.</param>
/// <returns>The request instance.</returns>
public delegate Task<ChatCommandGuidanceRequest> ChatCommandGuidanceRequestSend(string message, JsonObjectNode data, ChatCommandGuidanceResponse response, CancellationToken cancellationToken);

/// <summary>
/// The command guidance client for chat bot.
/// </summary>
public abstract class BaseChatCommandGuidanceClient
{
    internal readonly List<SimpleChatMessage> history = new();

    /// <summary>
    /// Adds or removes the event on message is received.
    /// </summary>
    public event DataEventHandler<ChatCommandGuidanceResponse> MessageReceived;

    /// <summary>
    /// Gets the additional information data.
    /// </summary>
    public JsonObjectNode Info { get; } = new();

    /// <summary>
    /// Gets or sets the user info.
    /// </summary>
    public UserItemInfo User { get; set; }

    /// <summary>
    /// Gets the latest response.
    /// </summary>
    public ChatCommandGuidanceResponse LatestResponse { get; private set; }

    /// <summary>
    /// Creates a request.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <returns>The request instance.</returns>
    public Task<ChatCommandGuidanceRequest> SendAsync(string message, JsonObjectNode data)
        => SendAsync(message, data, null);

    /// <summary>
    /// Receives a response.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <returns>An agent to reply the response.</returns>
    public ChatCommandGuidanceReplyAgent Receive(ChatCommandGuidanceResponse response)
    {
        OnReceive(response);
        LatestResponse = response;
        history.Add(new SimpleChatMessage(User.Nickname, response.Message, DateTime.Now, "reply", response.Data));
        MessageReceived?.Invoke(this, new DataEventArgs<ChatCommandGuidanceResponse>(response));
        return new(response, SendAsync);
    }

    /// <summary>
    /// Occurs on request creates.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="response">The response to reply.</param>
    /// <param name="context">The context.</param>
    protected virtual void OnCreateRequest(string message, JsonObjectNode data, ChatCommandGuidanceResponse response, JsonObjectNode context)
    {
    }

    /// <summary>
    /// Occurs on response is received.
    /// </summary>
    /// <param name="response">The response to reply.</param>
    protected virtual void OnReceive(ChatCommandGuidanceResponse response)
    {
    }

    /// <summary>
    /// Sends the message.
    /// </summary>
    /// <param name="request">The request message.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A Task that represents the work queued to execute in the ThreadPool.</returns>
    protected abstract Task SendAsync(ChatCommandGuidanceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a request.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="response">The response to reply.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The request instance.</returns>
    internal async Task<ChatCommandGuidanceRequest> SendAsync(string message, JsonObjectNode data, ChatCommandGuidanceResponse response, CancellationToken cancellationToken = default)
    {
        var context = new JsonObject();
        OnCreateRequest(message, data, response, context);
        var request = new ChatCommandGuidanceRequest(User, message, data, new List<SimpleChatMessage>(history), context, response);
        history.Add(new SimpleChatMessage(User.Nickname, message, DateTime.Now, "user", data));
        await SendAsync(request, cancellationToken);
        return request;
    }
}

/// <summary>
/// The agent to reply the response.
/// </summary>
public class ChatCommandGuidanceReplyAgent
{
    private readonly ChatCommandGuidanceRequestSend handler;

    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceReplyAgent class.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="reply">The reply handler.</param>
    internal ChatCommandGuidanceReplyAgent(ChatCommandGuidanceResponse response, ChatCommandGuidanceRequestSend reply)
    {
        Response = response;
        handler = reply;
    }

    /// <summary>
    /// Gets the response.
    /// </summary>
    public ChatCommandGuidanceResponse Response { get; }

    /// <summary>
    /// Replies.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The request instance.</returns>
    public Task<ChatCommandGuidanceRequest> ReplyAsync(string message, JsonObjectNode data, CancellationToken cancellationToken = default)
        => handler(message, data, Response, cancellationToken);
}
