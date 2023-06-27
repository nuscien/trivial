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
    /// Adds or removes the event on message is sending.
    /// </summary>
    public event EventHandler<ChatCommandGuidanceMessageEventArgs> Sending;

    /// <summary>
    /// Adds or removes the event on message is failed to send or no response.
    /// </summary>
    public event EventHandler<ChatCommandGuidanceErrorEventArgs<ChatCommandGuidanceMessageEventArgs>> SendFailed;

    /// <summary>
    /// Adds or removes the event on message is received.
    /// </summary>
    public event EventHandler<ChatCommandGuidanceMessageEventArgs> Received;

    /// <summary>
    /// Adds or removes the event on message is post processed.
    /// </summary>
    public event EventHandler<ChatCommandGuidanceMessageEventArgs> Processed;

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
    /// Sends a request to get response.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The response instance.</returns>
    public async Task<ChatCommandGuidanceResponse> SendAsync(string message, JsonObjectNode data, CancellationToken cancellationToken = default)
    {
        var result = await SendAsync(message, data, LatestResponse, cancellationToken);
        return result?.Response;
    }

    /// <summary>
    /// Sends a request to get response.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="callback">The callback.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The reply instance with response.</returns>
    public async Task<ChatCommandGuidanceResponse> SendAsync(string message, JsonObjectNode data, Action<ChatCommandGuidanceReply> callback, CancellationToken cancellationToken = default)
    {
        var result = await SendAsync(message, data, LatestResponse, cancellationToken);
        callback?.Invoke(result);
        return result?.Response;
    }

    /// <summary>
    /// Sends a request to get response.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="reply">The response message to reply; or null, if starts a new topic.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The response instance.</returns>
    internal async Task<ChatCommandGuidanceReply> SendAsync(string message, JsonObjectNode data, ChatCommandGuidanceResponse reply, CancellationToken cancellationToken = default)
    {
        var user = client.User;
        var request = new ChatCommandGuidanceRequest(user, message, data, new List<SimpleChatMessage>(history), null, reply);
        var args = new ChatCommandGuidanceMessageEventArgs(this, null, request, reply);
        client.OnRequestCreate(args);
        Sending?.Invoke(this, args);
        client.NotifySending(args);
        LatestRequest = request;
        var record = new SimpleChatMessage(request.Id, user, message, DateTime.Now, client.RequestMessageKind, data);
        history.Add(record);
        client.AddHistory(record);
        client.OnSend(args);
        ChatCommandGuidanceResponse response;
        try
        {
            response = await client.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            var exArgs = new ChatCommandGuidanceErrorEventArgs<ChatCommandGuidanceMessageEventArgs>(ex, args);
            SendFailed?.Invoke(this, exArgs);
            client.NotifySendFailed(exArgs);
            throw;
        }

        args.Response = response;
        Received?.Invoke(this, args);
        client.NotifyReceived(args);
        LatestResponse = response;
        record = new SimpleChatMessage(response.Id, user, response.Message, DateTime.Now, response.Kind, response.Data);
        history.Add(record);
        client.AddHistory(record);
        client.OnReceive(args);
        client.ProcessCommands(this, response);
        client.OnProcessed(args);
        Processed?.Invoke(this, args);
        client.NotifyProcessed(args);
        return new(this, response);
    }
}

/// <summary>
/// The reply information for chat command guidance topic.
/// </summary>
public class ChatCommandGuidanceReply
{
    private readonly ChatCommandGuidanceTopic topic;

    /// <summary>
    /// Initializes a new instance of the ChatCommandGuidanceReply class.
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="response"></param>
    internal ChatCommandGuidanceReply(ChatCommandGuidanceTopic topic, ChatCommandGuidanceResponse response)
    {
        this.topic = topic;
        Response = response;
    }

    /// <summary>
    /// Gets the response.
    /// </summary>
    public ChatCommandGuidanceResponse Response { get; }

    /// <summary>
    /// Gets a value indicating whether the response has replied.
    /// </summary>
    public bool HasReplied { get; private set; }

    /// <summary>
    /// Gets the creation date and time.
    /// </summary>
    public DateTime CreationTime { get; } = DateTime.Now;

    /// <summary>
    /// Creates a request.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The request instance.</returns>
    public async Task<ChatCommandGuidanceResponse> SendAsync(string message, JsonObjectNode data, CancellationToken cancellationToken = default)
    {
        HasReplied = true;
        var result = await topic.SendAsync(message, data, Response, cancellationToken);
        return result.Response;
    }

    /// <summary>
    /// Creates a request.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="data">The message data.</param>
    /// <param name="callback">The callback.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The request instance.</returns>
    public async Task<ChatCommandGuidanceResponse> SendAsync(string message, JsonObjectNode data, Action<ChatCommandGuidanceReply> callback, CancellationToken cancellationToken = default)
    {
        HasReplied = true;
        var result = await topic.SendAsync(message, data, Response, cancellationToken);
        callback?.Invoke(result);
        return result.Response;
    }
}
