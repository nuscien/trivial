using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.Users;

namespace Trivial.Text;

/// <summary>
/// The states of send result for chat message.
/// </summary>
public enum ExtendedChatMessageSendResultStates : byte
{
    /// <summary>
    /// Not send.
    /// </summary>
    NotSend = 0,

    /// <summary>
    /// Success.
    /// </summary>
    Success = 1,

    /// <summary>
    /// Network issue or timeout.
    /// </summary>
    NetworkIssue = 2,

    /// <summary>
    /// Unauthorized or fobidden to send.
    /// </summary>
    Forbidden = 3,

    /// <summary>
    /// Traffic limitation for connection.
    /// </summary>
    Throttle = 4,

    /// <summary>
    /// Format error or invalid request.
    /// </summary>
    RequestError = 5,

    /// <summary>
    /// Client error.
    /// </summary>
    ClientError = 6,

    /// <summary>
    /// Unknown server-side error.
    /// </summary>
    ServerError = 7,

    /// <summary>
    /// Other error.
    /// </summary>
    OtherError = 10
}

/// <summary>
/// The provider for chat client.
/// </summary>
public interface IExtendedChatClientProvider
{
    /// <summary>
    /// Adds or removes the event occurs on message received.
    /// </summary>
    event DataEventHandler<ExtendedChatMessage> MessageReceived;

    /// <summary>
    /// Gets the current user.
    /// </summary>
    UserItemInfo User { get; }

    /// <summary>
    /// Sends the message.
    /// </summary>
    /// <param name="to">The thread to send message.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The result state and other information.</returns>
    Task<ExtendedChatMessageSendResult> SendAsync(IExtendedChatThread to, ExtendedChatMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tries to get the user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The user information instance; or null, if not exists.</returns>
    Task<UserItemInfo> TryGetUserAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets latest threads.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The thread list.</returns>
    Task<List<BaseExtendedChatThread>> ListThreadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches threads.
    /// </summary>
    /// <param name="q">The search query.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The thread list.</returns>
    Task<List<BaseExtendedChatThread>> ListThreadAsync(string q, CancellationToken cancellationToken = default);
}

/// <summary>
/// The chat client.
/// </summary>
public class ExtendedChatClient
{
    /// <summary>
    /// Initializes a new instance of the ExtendedChatClient class.
    /// </summary>
    /// <param name="provider">The chat client provider.</param>
    public ExtendedChatClient(IExtendedChatClientProvider provider)
    {
        Provider = provider;
    }

    /// <summary>
    /// Gets or sets the user info.
    /// </summary>
    public UserItemInfo User => Provider.User;

    /// <summary>
    /// Gets the i.
    /// </summary>
    protected IExtendedChatClientProvider Provider { get; }

    /// <summary>
    /// Sends the message.
    /// </summary>
    /// <param name="to">The thread to send message.</param>
    /// <param name="message">The message text to send.</param>
    /// <param name="info">The additional data to send.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The message request.</returns>
    public ExtendedChatMessageRequest Send(IExtendedChatThread to, string message, JsonObjectNode info, CancellationToken cancellationToken = default)
    {
        var req = new ExtendedChatMessageRequest(Provider, to, new ExtendedChatMessage(User, message, null, info));
        _ = req.GetResultAsync(cancellationToken);
        return req;
    }

    /// <summary>
    /// Tries to get the user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The user information instance; or null, if not exists.</returns>
    public Task<UserItemInfo> TryGetUserAsync(string id, CancellationToken cancellationToken = default)
        => Provider?.TryGetUserAsync(id, cancellationToken) ?? Task.FromResult<UserItemInfo>(null);

    /// <summary>
    /// Gets latest threads.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The thread list.</returns>
    public async Task<List<BaseExtendedChatThread>> ListThreadAsync(CancellationToken cancellationToken = default)
    {
        if (Provider == null) return new();
        return await Provider.ListThreadAsync(cancellationToken) ?? new();
    }

    /// <summary>
    /// Searches threads.
    /// </summary>
    /// <param name="q">The search query.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The thread list.</returns>
    public async Task<List<BaseExtendedChatThread>> ListThreadAsync(string q, CancellationToken cancellationToken = default)
    {
        if (Provider == null) return new();
        return await Provider.ListThreadAsync(q, cancellationToken) ?? new();
    }
}

/// <summary>
/// The sending result of chat message.
/// </summary>
public class ExtendedChatMessageSendResult
{
    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessageSendResult class.
    /// </summary>
    /// <param name="state">The result state.</param>
    public ExtendedChatMessageSendResult(ExtendedChatMessageSendResultStates state)
    {
        State = state;
        Info = new();
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessageSendResult class.
    /// </summary>
    /// <param name="state">The result state.</param>
    /// <param name="message"></param>
    /// <param name="info"></param>
    public ExtendedChatMessageSendResult(ExtendedChatMessageSendResultStates state, string message, JsonObjectNode info = null)
    {
        State = state;
        Message = message;
        Info = info ?? new();
    }

    /// <summary>
    /// Gets the result state.
    /// </summary>
    public ExtendedChatMessageSendResultStates State { get; }

    /// <summary>
    /// Gets the additional response message, such like error message or result notes.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the additional information.
    /// </summary>
    public JsonObjectNode Info { get; }

    /// <summary>
    /// Gets a value indicating whether the state is a retry-able one.
    /// </summary>
    internal bool CanRetry => State == ExtendedChatMessageSendResultStates.NotSend || State == ExtendedChatMessageSendResultStates.Throttle || State == ExtendedChatMessageSendResultStates.NetworkIssue;
}

/// <summary>
/// The chat message request.
/// </summary>
public class ExtendedChatMessageRequest
{
    private Task<ExtendedChatMessageSendResult> task;
    private readonly IExtendedChatClientProvider client;

    /// <summary>
    /// Initializies a new instance of the ExtendedChatMessageRequest class.
    /// </summary>
    /// <param name="client">The client provider.</param>
    /// <param name="to">The thread to send message.</param>
    /// <param name="message">The message to send.</param>
    public ExtendedChatMessageRequest(IExtendedChatClientProvider client, IExtendedChatThread to, ExtendedChatMessage message)
    {
        this.client = client;
        Thread = to;
        Message = message;
    }

    /// <summary>
    /// Gets the message sent.
    /// </summary>
    public ExtendedChatMessage Message { get; }

    /// <summary>
    /// Gets the thread to send message.
    /// </summary>
    public IExtendedChatThread Thread { get; }

    /// <summary>
    /// Gets the result.
    /// </summary>
    public ExtendedChatMessageSendResult Result { get; private set; } = new(ExtendedChatMessageSendResultStates.NotSend);

    /// <summary>
    /// Gets a value indicating whether the message is sending.
    /// </summary>
    public bool IsSending => task != null;

    /// <summary>
    /// Gets the result.
    /// </summary>
    /// <returns>The result state and other information.</returns>
    public Task<ExtendedChatMessageSendResult> GetResultAsync()
        => GetResultAsync(false);

    /// <summary>
    /// Gets the result.
    /// </summary>
    /// <param name="retry">true if resend for networking issue; otherwise, false.</param>
    /// <returns>The result state and other information.</returns>
    public async Task<ExtendedChatMessageSendResult> GetResultAsync(bool retry)
    {
        var t = task;
        if (t != null)
        {
            try
            {
                Result = await t;
            }
            finally
            {
                task = null;
                Result ??= new(ExtendedChatMessageSendResultStates.OtherError);
            }

            return Result;
        }

        if (!retry || Result == null || !Result.CanRetry) return Result ?? new(ExtendedChatMessageSendResultStates.OtherError);
        await GetResultAsync(default(CancellationToken));
        return Result;
    }

    /// <summary>
    /// Gets the result.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>The result state and other information.</returns>
    internal async Task<ExtendedChatMessageSendResult> GetResultAsync(CancellationToken cancellationToken)
    {
        if (client == null || Thread == null) return Result = new(ExtendedChatMessageSendResultStates.ClientError);
        task = client.SendAsync(Thread, Message, cancellationToken);
        try
        {
            Result = await task;
        }
        finally
        {
            Result ??= new(ExtendedChatMessageSendResultStates.OtherError);
            task = null;
        }

        return Result;
    }
}
