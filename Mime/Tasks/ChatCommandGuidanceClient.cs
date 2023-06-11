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
using System.Net.NetworkInformation;

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
    /// <summary>
    /// Adds or removes the event on message is received.
    /// </summary>
    public event EventHandler<ChatCommandGuidanceMessageEventArgs> Received;

    /// <summary>
    /// Adds or removes the event on message is sending.
    /// </summary>
    public event EventHandler<ChatCommandGuidanceMessageEventArgs> Sending;

    /// <summary>
    /// Adds or removes the event on message is sent.
    /// </summary>
    public event EventHandler<ChatCommandGuidanceMessageEventArgs> Sent;

    /// <summary>
    /// Adds or removes the event on the new topic is created.
    /// </summary>
    public event DataEventHandler<ChatCommandGuidanceTopic> NewTopicCreated;

    /// <summary>
    /// Gets the additional information data.
    /// </summary>
    public JsonObjectNode Info { get; } = new();

    /// <summary>
    /// Gets or sets the user info.
    /// </summary>
    public UserItemInfo User { get; set; }

    /// <summary>
    /// Gets the default message kind of request.
    /// </summary>
    protected internal virtual string RequestMessageKind { get; } = "user";

    /// <summary>
    /// Creates a new topic.
    /// </summary>
    /// <returns>The topic instance.</returns>
    public ChatCommandGuidanceTopic NewTopic()
    {
        var topic = new ChatCommandGuidanceTopic(this);
        NewTopicCreated?.Invoke(this, new DataEventArgs<ChatCommandGuidanceTopic>(topic));
        return topic;
    }

    /// <summary>
    /// Occurs on request creates.
    /// </summary>
    /// <param name="request">The request message.</param>
    /// <param name="topic">The chat topic.</param>
    protected internal virtual void OnRequestCreate(ChatCommandGuidanceRequest request, ChatCommandGuidanceTopic topic)
    {
    }

    /// <summary>
    /// Occurs on response is received.
    /// </summary>
    /// <param name="response">The response to reply.</param>
    /// <param name="topic">The chat topic.</param>
    protected internal virtual void OnReceive(ChatCommandGuidanceResponse response, ChatCommandGuidanceTopic topic)
    {
    }

    /// <summary>
    /// Sends a message.
    /// </summary>
    /// <param name="request">The request message.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The response message.</returns>
    protected internal abstract Task<ChatCommandGuidanceResponse> SendAsync(ChatCommandGuidanceRequest request, CancellationToken cancellationToken = default);

    internal void NotifySending(ChatCommandGuidanceTopic topic, ChatCommandGuidanceRequest request, ChatCommandGuidanceResponse response)
        => Sending?.Invoke(this, new ChatCommandGuidanceMessageEventArgs(topic, request, response));

    internal void NotifySent(ChatCommandGuidanceTopic topic, ChatCommandGuidanceRequest request, ChatCommandGuidanceResponse response)
        => Sent?.Invoke(this, new ChatCommandGuidanceMessageEventArgs(topic, request, response));

    internal void NotifyReceive(ChatCommandGuidanceTopic topic, ChatCommandGuidanceRequest request, ChatCommandGuidanceResponse response)
        => Received?.Invoke(this, new ChatCommandGuidanceMessageEventArgs(topic, request, response));
}

/// <summary>
/// The event arguments for message of chat command guidance.
/// </summary>
public class ChatCommandGuidanceMessageEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceMessageEventArgs class.
    /// </summary>
    /// <param name="topic">The topic.</param>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    public ChatCommandGuidanceMessageEventArgs(ChatCommandGuidanceTopic topic, ChatCommandGuidanceRequest request, ChatCommandGuidanceResponse response)
    {
        Topic = topic;
        Request = request;
        Response = response;
    }

    /// <summary>
    /// Gets the chat topic.
    /// </summary>
    public ChatCommandGuidanceTopic Topic { get; }

    /// <summary>
    /// Gets the request message.
    /// </summary>
    public ChatCommandGuidanceRequest Request { get; }

    /// <summary>
    /// Gets the response message.
    /// If the event arguments is for sending, this is the response to reply.
    /// </summary>
    public ChatCommandGuidanceResponse Response { get; }
}

/// <summary>
/// The client processing event arguments of chat command guidance.
/// </summary>
public class ChatCommandGuidanceClientProcessingArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceClientProcessingArgs class.
    /// </summary>
    /// <param name="command">The command key.</param>
    /// <param name="value">The command value.</param>
    /// <param name="topic">The chat topic.</param>
    /// <param name="response">The response message.</param>
    public ChatCommandGuidanceClientProcessingArgs(string command, JsonObjectNode value, ChatCommandGuidanceTopic topic, ChatCommandGuidanceResponse response)
    {
        Command = command;
        Value = value;
        Topic = topic;
        Response = response;
    }

    /// <summary>
    /// Gets the command key.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// Gets the command value.
    /// </summary>
    public JsonObjectNode Value { get; }

    /// <summary>
    /// Gets the chat topic.
    /// </summary>
    public ChatCommandGuidanceTopic Topic { get; }

    /// <summary>
    /// Gets the response message.
    /// </summary>
    public ChatCommandGuidanceResponse Response { get; }
}

/// <summary>
/// The chat topic.
/// </summary>
public class ChatCommandGuidanceTopic
{
    private readonly BaseChatCommandGuidanceClient client;
    internal readonly List<SimpleChatMessage> history = new();

    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceTopic class.
    /// </summary>
    /// <param name="client">The client.</param>
    internal ChatCommandGuidanceTopic(BaseChatCommandGuidanceClient client)
    {
        this.client = client;
        History = history.AsReadOnly();
    }

    /// <summary>
    /// Adds or removes the event on message is received.
    /// </summary>
    public event DataEventHandler<ChatCommandGuidanceResponse> Received;

    /// <summary>
    /// Adds or removes the event on message is sending.
    /// </summary>
    public event DataEventHandler<ChatCommandGuidanceRequest> Sending;

    /// <summary>
    /// Adds or removes the event on message is sent.
    /// </summary>
    public event DataEventHandler<ChatCommandGuidanceRequest> Sent;

    /// <summary>
    /// Gets the additional information data.
    /// </summary>
    public JsonObjectNode Info { get; } = new();

    /// <summary>
    /// Gets the shared information data.
    /// </summary>
    public JsonObjectNode SharedInfo => client.Info;

    /// <summary>
    /// Gets the current user info.
    /// </summary>
    public UserItemInfo User => client.User;

    /// <summary>
    /// Gets the chat history.
    /// </summary>
    public IList<SimpleChatMessage> History { get; }

    /// <summary>
    /// Gets the latest request.
    /// </summary>
    public ChatCommandGuidanceRequest LatestRequest { get; private set; }

    /// <summary>
    /// Gets the latest response.
    /// </summary>
    public ChatCommandGuidanceResponse LatestResponse { get; private set; }

    /// <summary>
    /// Creates a new topic.
    /// </summary>
    /// <returns>The topic instance.</returns>
    public ChatCommandGuidanceTopic NewTopic()
        => client.NewTopic();

    /// <summary>
    /// Creates a request.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The request instance.</returns>
    public async Task<ChatCommandGuidanceResponse> SendAsync(string message, JsonObjectNode data, CancellationToken cancellationToken = default)
    {
        var user = client.User;
        var response = LatestResponse;
        var request = new ChatCommandGuidanceRequest(user, message, data, new List<SimpleChatMessage>(history), null, response);
        client.OnRequestCreate(request, this);
        Sending?.Invoke(this, new DataEventArgs<ChatCommandGuidanceRequest>(request));
        client.NotifySending(this, request, response);
        LatestRequest = request;
        history.Add(new SimpleChatMessage(user, message, DateTime.Now, client.RequestMessageKind, data));
        response = await client.SendAsync(request, cancellationToken);
        Sent?.Invoke(this, new DataEventArgs<ChatCommandGuidanceRequest>(request));
        client.NotifySent(this, request, response);
        client.OnReceive(response, this);
        LatestResponse = response;
        history.Add(new SimpleChatMessage(user, response.Message, DateTime.Now, "reply", response.Data));
        Received?.Invoke(this, new DataEventArgs<ChatCommandGuidanceResponse>(response));
        client.NotifyReceive(this, request, response);
        return response;
    }
}
