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
using Trivial.Security;
using Trivial.Net;
using System.Net.Http;

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
    private readonly Dictionary<string, BaseChatCommandGuidanceProvider> commands = new();

    /// <summary>
    /// Initializes a new instance of the BaseChatCommandGuidanceClient class.
    /// </summary>
    /// <param name="user">The user info.</param>
    protected BaseChatCommandGuidanceClient(UserItemInfo user)
    {
        User = user ?? new();
    }

    /// <summary>
    /// Adds or removes the event on message is sending.
    /// </summary>
    public event EventHandler<ChatCommandGuidanceMessageEventArgs> Sending;

    /// <summary>
    /// Adds or removes the event on message is failed to send or get response.
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
    public UserItemInfo User { get; }

    /// <summary>
    /// Gets the default message kind of request.
    /// </summary>
    protected internal virtual string RequestMessageKind { get; } = "user";

    /// <summary>
    /// Gets the command collection.
    /// </summary>
    public ICollection<BaseChatCommandGuidanceProvider> Commands => commands.Values;

    /// <summary>
    /// Gets the collection of all command key registered.
    /// </summary>
    public ICollection<string> CommandKeys => commands.Keys;

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <typeparam name="T">The type of the command guidance.</typeparam>
    public void Register<T>() where T : BaseChatCommandGuidanceProvider
        => Register(Activator.CreateInstance<T>());

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <param name="key">The command key to use.</param>
    /// <typeparam name="T">The type of the command guidance.</typeparam>
    public void Register<T>(string key) where T : BaseChatCommandGuidanceProvider
        => Register(key, Activator.CreateInstance<T>());

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <param name="command">The command.</param>
    public void Register(BaseChatCommandGuidanceProvider command)
        => Register(command.Command, command);

    /// <summary>
    /// Registers a command.
    /// </summary>
    /// <param name="key">The command key to use.</param>
    /// <param name="command">The command.</param>
    public void Register(string key, BaseChatCommandGuidanceProvider command)
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
    public BaseChatCommandGuidanceProvider GetCommand(string key)
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
    /// Projects each element of a sequence into a new form by incorporating the element's key.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by selector.</typeparam>
    /// <param name="select">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
    /// <returns>A collection whose elements are the result of invoking the transform function on each element of source.</returns>
    public IEnumerable<T> SelectCommand<T>(Func<string, BaseChatCommandGuidanceProvider, T> select)
    {
        if (select == null) yield break;
        foreach (var kvp in commands)
        {
            if (kvp.Value == null) continue;
            yield return select(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Projects each element of a sequence into a new form by incorporating the element's key.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by selector.</typeparam>
    /// <param name="kind">A filter by command guidance kind.</param>
    /// <param name="select">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
    /// <returns>A collection whose elements are the result of invoking the transform function on each element of source.</returns>
    public IEnumerable<T> SelectCommand<T>(ChatCommandGuidanceProviderKinds kind, Func<string, BaseChatCommandGuidanceProvider, T> select)
    {
        if (select == null) yield break;
        foreach (var kvp in commands)
        {
            if (kvp.Value?.Kind != kind) continue;
            yield return select(kvp.Key, kvp.Value);
        }
    }

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
    /// <param name="args">The arguments.</param>
    protected internal virtual void OnRequestCreate(ChatCommandGuidanceMessageEventArgs args)
    {
    }

    /// <summary>
    /// Occurs on request is sending.
    /// </summary>
    /// <param name="args">The arguments.</param>
    protected internal virtual void OnSend(ChatCommandGuidanceMessageEventArgs args)
    {
    }

    /// <summary>
    /// Occurs on response is received.
    /// </summary>
    /// <param name="args">The arguments.</param>
    protected internal virtual void OnReceive(ChatCommandGuidanceMessageEventArgs args)
    {
    }

    /// <summary>
    /// Occurs on response is processed.
    /// </summary>
    /// <param name="args">The arguments.</param>
    protected internal virtual void OnProcessed(ChatCommandGuidanceMessageEventArgs args)
    {
    }

    /// <summary>
    /// Sends a message.
    /// </summary>
    /// <param name="request">The request message.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The response message.</returns>
    protected internal abstract Task<ChatCommandGuidanceResponse> SendAsync(ChatCommandGuidanceRequest request, CancellationToken cancellationToken = default);

    internal void NotifySending(ChatCommandGuidanceMessageEventArgs args)
        => Sending?.Invoke(this, args);

    internal void NotifySendFailed(ChatCommandGuidanceErrorEventArgs<ChatCommandGuidanceMessageEventArgs> args)
        => SendFailed?.Invoke(this, args);

    internal void NotifyReceived(ChatCommandGuidanceMessageEventArgs args)
        => Received?.Invoke(this, args);

    internal void NotifyProcessed(ChatCommandGuidanceMessageEventArgs args)
        => Processed?.Invoke(this, args);

    internal void ProcessCommands(ChatCommandGuidanceTopic topic, ChatCommandGuidanceResponse response)
    {
        var details = response?.Details;
        if (details == null || topic == null) return;
        foreach (var item in details)
        {
            if (string.IsNullOrWhiteSpace(item.Key) || item.Value == null || !commands.TryGetValue(item.Key, out var command)) continue;
            command.ClientProcess(this, new ChatCommandGuidanceClientProcessingArgs(item.Key, item.Value, topic, response));
        }
    }
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
    /// <param name="response">The response message.</param>
    /// <param name="request">The request message.</param>
    /// <param name="reply">The response which the request is to reply.</param>
    public ChatCommandGuidanceMessageEventArgs(ChatCommandGuidanceTopic topic, ChatCommandGuidanceResponse response, ChatCommandGuidanceRequest request, ChatCommandGuidanceResponse reply)
    {
        Topic = topic;
        Response = response;
        Request = request;
        Previous = reply;
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
    /// The response which the request is to reply.
    /// </summary>
    public ChatCommandGuidanceResponse Previous { get; }

    /// <summary>
    /// Gets the response message.
    /// </summary>
    public ChatCommandGuidanceResponse Response { get; internal set; }
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
/// The command guidance client for chat bot.
/// </summary>
public class LocalChatCommandGuidanceClient : BaseChatCommandGuidanceClient
{
    /// <summary>
    /// Initializes a new instance of the LocalChatCommandGuidanceClient class.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="engine">The engine.</param>
    public LocalChatCommandGuidanceClient(UserItemInfo user, BaseChatCommandGuidanceEngine engine)
        : base(user)
    {
        Engine = engine ?? new EmptyChatCommandGuidanceEngine();
    }

    /// <summary>
    /// Gets the engine.
    /// </summary>
    protected BaseChatCommandGuidanceEngine Engine { get; }

    /// <inheritdoc />
    protected internal override Task<ChatCommandGuidanceResponse> SendAsync(ChatCommandGuidanceRequest request, CancellationToken cancellationToken = default)
        => Engine.ProcessAsync(request, cancellationToken);
}

/// <summary>
/// The command guidance client for chat bot.
/// </summary>
public class OnlineChatCommandGuidanceClient : BaseChatCommandGuidanceClient
{
    /// <summary>
    /// Initializes a new instance of the OnlineChatCommandGuidanceClient class.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="uri">The URI of the web API.</param>
    /// <param name="client">The web client.</param>
    public OnlineChatCommandGuidanceClient(UserItemInfo user, Uri uri, OAuthClient client = null)
        : base(user)
    {
        Uri = uri;
        Client = client;
    }

    /// <summary>
    /// Gets the web client.
    /// </summary>
    public OAuthClient Client { get; }

    /// <summary>
    /// Gets the URI of the web API.
    /// </summary>
    protected Uri Uri { get; }

    /// <inheritdoc />
    protected internal override async Task<ChatCommandGuidanceResponse> SendAsync(ChatCommandGuidanceRequest request, CancellationToken cancellationToken = default)
    {
        if (Uri == null) return null;
        var client = CreateHttpClient<JsonObjectNode>();
        var resp = await client.PostAsync(Uri, (JsonObjectNode)request, cancellationToken);
        return (ChatCommandGuidanceResponse)resp;
    }

    /// <summary>
    /// Creates a JSON HTTP client.
    /// </summary>
    /// <typeparam name="T">The type of response.</typeparam>
    /// <param name="callback">An optional callback raised on data received.</param>
    /// <returns>A new JSON HTTP client.</returns>
    public virtual JsonHttpClient<T> CreateHttpClient<T>(Action<ReceivedEventArgs<T>> callback = null)
    {
        if (Client != null) return Client.Create(callback);
        var client = new JsonHttpClient<T>();
        if (callback != null) client.Received += (sender, ev) =>
        {
            callback(ev);
        };
        return client;
    }
}
