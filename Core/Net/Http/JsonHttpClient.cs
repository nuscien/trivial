﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Reflection;
using Trivial.Tasks;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Net;

/// <summary>
/// The maker to create JSON HTTP client.
/// </summary>
public interface IJsonHttpClientMaker
{
    /// <summary>
    /// Creates a JSON HTTP client.
    /// </summary>
    /// <typeparam name="T">The type of response.</typeparam>
    /// <param name="callback">An optional callback raised on data received.</param>
    /// <returns>A new JSON HTTP client.</returns>
    JsonHttpClient<T> Create<T>(Action<ReceivedEventArgs<T>> callback = null);
}

/// <summary>
/// The event arguments on sending.
/// </summary>
/// <param name="requestMessage">The HTTP request message.</param>
public class SendingEventArgs(HttpRequestMessage requestMessage) : EventArgs
{

    /// <summary>
    /// Gets the HTTP request message.
    /// </summary>
    public HttpRequestMessage RequestMessage { get; } = requestMessage;

    /// <summary>
    /// Gets the request URI.
    /// </summary>
    public Uri RequestUri => RequestMessage?.RequestUri;

    /// <summary>
    /// Gets the HTTP request method.
    /// </summary>
    public HttpMethod Method => RequestMessage.Method ?? HttpMethod.Get;

    /// <summary>
    /// Gets the HTTP version used.
    /// </summary>
    public Version HttpVersion => RequestMessage.Version;
}

/// <summary>
/// The event arguments on received.
/// </summary>
public class ReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the ReceivedEventArgs class.
    /// </summary>
    /// <param name="ev">The event arguments.</param>
    public ReceivedEventArgs(ReceivedEventArgs ev)
    {
        if (ev == null) return;
        Header = ev.Header;
        ReasonPhrase = ev.ReasonPhrase;
        IsSuccessStatusCode = ev.IsSuccessStatusCode;
        StatusCode = ev.StatusCode;
        Content = ev.Content;
        Version = ev.Version;
        RequestMessage = ev.RequestMessage;
    }

    /// <summary>
    /// Initializes a new instance of the ReceivedEventArgs class.
    /// </summary>
    /// <param name="responseMessage">The HTTP response message.</param>
    public ReceivedEventArgs(HttpResponseMessage responseMessage)
    {
        if (responseMessage == null) return;
        Header = responseMessage.Headers;
        ReasonPhrase = responseMessage.ReasonPhrase;
        IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
        StatusCode = responseMessage.StatusCode;
        Content = responseMessage.Content;
        Version = responseMessage.Version;
        RequestMessage = responseMessage.RequestMessage;
    }

    /// <summary>
    /// Initializes a new instance of the ReceivedEventArgs class.
    /// </summary>
    /// <param name="statusCode">The status code of the HTTP response.</param>
    /// <param name="content">The content of a HTTP response message.</param>
    /// <param name="header">The collection of HTTP response headers.</param>
    public ReceivedEventArgs(HttpStatusCode statusCode, HttpContent content = null, HttpResponseHeaders header = null)
    {
        StatusCode = statusCode;
        Content = content;
        Header = header;
        var statusInt = (int)statusCode;
        IsSuccessStatusCode = statusInt >= 200 && statusInt < 300;
    }

    /// <summary>
    /// Gets the collection of HTTP response headers.
    /// </summary>
    public HttpResponseHeaders Header { get; }

    /// <summary>
    /// Gets the reason phrase which typically is sent by servers together with the status code.
    /// </summary>
    public string ReasonPhrase { get; }

    /// <summary>
    /// Gets a value that indicates if the HTTP response was successful.
    /// </summary>
    public bool IsSuccessStatusCode { get; }

    /// <summary>
    /// Gets the status code of the HTTP response.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Gets the content of a HTTP response message.
    /// </summary>
    public HttpContent Content { get; }

    /// <summary>
    /// Gets the HTTP message version.
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// Gets the request message which led to this response message.
    /// </summary>
    public HttpRequestMessage RequestMessage { get; }
}

/// <summary>
/// The event arguments on received.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public class ReceivedEventArgs<T> : ReceivedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the ReceivedEventArgs class.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="ev">The event arguments.</param>
    public ReceivedEventArgs(object result, ReceivedEventArgs ev) : base(ev)
    {
        if (result == null) return;
        try
        {
            Result = (T)result;
        }
        catch (InvalidCastException)
        {
        }
    }

    /// <summary>
    /// Initializes a new instance of the ReceivedEventArgs class.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="responseMessage">The HTTP response message.</param>
    public ReceivedEventArgs(T result, HttpResponseMessage responseMessage) : base(responseMessage)
    {
        Result = result;
    }

    /// <summary>
    /// Gets the result.
    /// </summary>
    public T Result { get; }

    /// <summary>
    /// Converts to a specific received event arguments.
    /// </summary>
    /// <typeparam name="U">The type of result.</typeparam>
    /// <returns>A received data event arguments.</returns>
    public ReceivedEventArgs<U> ConvertTo<U>()
        => new(Result, this);
}

/// <summary>
/// JSON format serialization HTTP client with advanced options.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
/// <example>
/// <code>
/// var http1 = new JsonHttpClient&lt;JsonObjectNode&gt;()
/// var json = await http1.GetAsync(A-URL-TO-GET-JSON);
/// </code>
/// <code>
/// var http2 = new JsonHttpClient&lt;IAsyncEnumerable&lt;ServerSentEventInfo&gt;&gt;()
/// var sse = await http2.GetAsync(A-URL-TO-STREAM-MESSAGE);
/// </code>
/// <code>
/// var http3 = new JsonHttpClient&lt;SerializableModel&gt;()
/// var model = await http3.GetAsync(A-URL-TO-GET-JSON);
/// </code>
/// <code>
/// var http4 = new JsonHttpClient&lt;IAsyncEnumerable&lt;JsonObjectNode&gt;&gt;()
/// var sse = await http4.GetAsync(A-URL-TO-GET-JSONL);
/// </code>
/// </example>
public class JsonHttpClient<T>
{
    /// <summary>
    /// Initializes a new instance of the JsonHttpClient class.
    /// </summary>
    public JsonHttpClient()
    {
        var d = WebFormat.GetJsonDeserializer<T>(true);
        if (d != null) Deserializer = new JsonTypedDeserializer<T>(d);
    }

    /// <summary>
    /// Initializes a new instance of the JsonHttpClient class.
    /// </summary>
    /// <param name="httpClientResolver">The HTTP client resolver.</param>
    /// <param name="serializeEvenIfFailed">true if still need serialize the result even if it fails to send request; otherwise, false.</param>
    /// <param name="retryPolicy">The retry policy.</param>
    public JsonHttpClient(IObjectResolver<HttpClient> httpClientResolver, bool serializeEvenIfFailed = false, IRetryPolicy retryPolicy = null)
        : this()
    {
        HttpClientResolver = httpClientResolver;
        SerializeEvenIfFailed = serializeEvenIfFailed;
        RetryPolicy = retryPolicy;
    }

    /// <summary>
    /// Initializes a new instance of the JsonHttpClient class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to re-use.</param>
    /// <param name="serializeEvenIfFailed">true if still need serialize the result even if it fails to send request; otherwise, false.</param>
    /// <param name="retryPolicy">The retry policy.</param>
    public JsonHttpClient(HttpClient httpClient, bool serializeEvenIfFailed = false, IRetryPolicy retryPolicy = null)
        : this(new InstanceObjectRef<HttpClient>(httpClient ?? new()), serializeEvenIfFailed, retryPolicy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonHttpClient class.
    /// </summary>
    /// <param name="httpClientHandler">The HTTP client handler to re-use.</param>
    /// <param name="disposeHandler">true if the inner handler should be disposed when the HTTP client is disposed; otherwise, false, to reuse the inner handler.</param>
    /// <param name="serializeEvenIfFailed">true if still need serialize the result even if it fails to send request; otherwise, false.</param>
    /// <param name="retryPolicy">The retry policy.</param>
    public JsonHttpClient(HttpClientHandler httpClientHandler, bool disposeHandler, bool serializeEvenIfFailed = false, IRetryPolicy retryPolicy = null)
        : this(new InstanceObjectRef<HttpClient>(httpClientHandler is null ? new() : new(httpClientHandler, disposeHandler)), serializeEvenIfFailed, retryPolicy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonHttpClient class.
    /// </summary>
    /// <param name="createHttpClientEvertime">true if create a new HTTP client everytime; otherwise, false.</param>
    /// <param name="serializeEvenIfFailed">true if still need serialize the result even if it fails to send request; otherwise, false.</param>
    /// <param name="retryPolicy">The retry policy.</param>
    public JsonHttpClient(bool createHttpClientEvertime, bool serializeEvenIfFailed = false, IRetryPolicy retryPolicy = null)
        : this(createHttpClientEvertime ? FactoryObjectResolver<HttpClient>.Create() : new InstanceObjectRef<HttpClient>(new()), serializeEvenIfFailed, retryPolicy)
    {
    }

    /// <summary>
    /// Adds or removes a handler raised on sending.
    /// </summary>
    public event EventHandler<SendingEventArgs> Sending;

    /// <summary>
    /// Adds or removes a handler raised on received.
    /// </summary>
    public event EventHandler<ReceivedEventArgs<T>> Received;

    /// <summary>
    /// Gets the type of response expected.
    /// </summary>
    public Type ResponseType => typeof(T);

    /// <summary>
    /// Gets or sets the retry policy.
    /// </summary>
    public IRetryPolicy RetryPolicy { get; set; }

    /// <summary>
    /// Gets or sets the JSON deserializer.
    /// </summary>
    public JsonTypedDeserializer<T> Deserializer { get; set; }

    /// <summary>
    /// Gets or sets the HTTP client resolver.
    /// </summary>
    public IObjectResolver<HttpClient> HttpClientResolver { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether still need serialize the result even if it fails to send request.
    /// </summary>
    public bool SerializeEvenIfFailed { get; set; }

    /// <summary>
    /// Gets or sets a handler to catch the exception and return if need throw.
    /// </summary>
    public Func<Exception, Exception> GetExceptionHandler { get; set; }

    /// <summary>
    /// Gets additional string bag.
    /// </summary>
    public IDictionary<string, string> Bag { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets HTTP client instance.
    /// </summary>
    /// <returns>The HTTP client instance.</returns>
    public HttpClient GetHttpClient()
    {
        var resolver = HttpClientResolver;
        if (resolver is null)
        {
            var ready = new InstanceObjectRef<HttpClient>(new());
            if (HttpClientResolver is null)
            {
                HttpClientResolver = ready;
                resolver = ready;
            }
            else
            {
                resolver = HttpClientResolver;
                if (resolver is null)
                {
                    HttpClientResolver = ready;
                    resolver = ready;
                }
            }
        }

        return resolver.GetInstance() ?? new();
    }

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="callback">An optional callback raised on data received.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public async Task<T> SendAsync(HttpRequestMessage request, Action<ReceivedEventArgs<T>> callback, CancellationToken cancellationToken = default)
    {
        if (request == null) throw ObjectConvert.ArgumentNull(nameof(request));
        var client = GetHttpClient();
        Sending?.Invoke(this, new SendingEventArgs(request));
        cancellationToken.ThrowIfCancellationRequested();
        OnSend(request);
        HttpResponseMessage resp = null;
        T valueResult;
        try
        {
            valueResult = await SendAsync(client, request, cancellationToken);
        }
        catch (HttpRequestException)
        {
            OnReceive(default, resp);
            Received?.Invoke(this, new ReceivedEventArgs<T>(default, resp));
            callback?.Invoke(new ReceivedEventArgs<T>(default, resp));
            throw;
        }
        catch (FailedHttpException)
        {
            OnReceive(default, resp);
            Received?.Invoke(this, new ReceivedEventArgs<T>(default, resp));
            callback?.Invoke(new ReceivedEventArgs<T>(default, resp));
            throw;
        }
        catch (ArgumentException)
        {
            OnReceive(default, resp);
            Received?.Invoke(this, new ReceivedEventArgs<T>(default, resp));
            callback?.Invoke(new ReceivedEventArgs<T>(default, resp));
            throw;
        }
        catch (InvalidOperationException)
        {
            OnReceive(default, resp);
            Received?.Invoke(this, new ReceivedEventArgs<T>(default, resp));
            callback?.Invoke(new ReceivedEventArgs<T>(default, resp));
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            OnReceive(default, resp);
            Received?.Invoke(this, new ReceivedEventArgs<T>(default, resp));
            callback?.Invoke(new ReceivedEventArgs<T>(default, resp));
            throw;
        }
        catch (NullReferenceException)
        {
            OnReceive(default, resp);
            Received?.Invoke(this, new ReceivedEventArgs<T>(default, resp));
            callback?.Invoke(new ReceivedEventArgs<T>(default, resp));
            throw;
        }

        OnReceive(valueResult, resp);
        Received?.Invoke(this, new ReceivedEventArgs<T>(valueResult, resp));
        callback?.Invoke(new ReceivedEventArgs<T>(valueResult, resp));
        return valueResult;
    }

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => SendAsync(request, null, cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendAsync(HttpMethod method, string requestUri, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Get, requestUri), cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendAsync(HttpMethod method, Uri requestUri, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Get, requestUri), cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendAsync(HttpMethod method, string requestUri, HttpContent content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Post, requestUri, content), cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendAsync(HttpMethod method, Uri requestUri, HttpContent content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Post, requestUri, content), cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendAsync(HttpMethod method, string requestUri, QueryData content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Post, requestUri, new StringContent(content.ToString(), Encoding.UTF8, WebFormat.FormUrlMIME)), cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendAsync(HttpMethod method, Uri requestUri, QueryData content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Post, requestUri, new StringContent(content.ToString(), Encoding.UTF8, WebFormat.FormUrlMIME)), cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="options">The options for serialization.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public async Task<T> SendJsonAsync(HttpMethod method, string requestUri, object content, JsonSerializerOptions options, CancellationToken cancellationToken = default)
    {
        using var body = HttpClientExtensions.CreateJsonContent(content, options);
        return await SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Post, requestUri, body), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="options">The options for serialization.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public async Task<T> SendJsonAsync(HttpMethod method, Uri requestUri, object content, JsonSerializerOptions options, CancellationToken cancellationToken = default)
    {
        using var body = HttpClientExtensions.CreateJsonContent(content, options);
        return await SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Post, requestUri, body), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="options">The options for serialization.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendJsonAsync(HttpMethod method, string requestUri, object content, DataContractJsonSerializerSettings options, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Post, requestUri, HttpClientExtensions.CreateJsonContent(content, options)), cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="options">The options for serialization.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendJsonAsync(HttpMethod method, Uri requestUri, object content, DataContractJsonSerializerSettings options, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Post, requestUri, HttpClientExtensions.CreateJsonContent(content, options)), cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendJsonAsync(HttpMethod method, string requestUri, object content, CancellationToken cancellationToken = default)
        => SendJsonAsync(method, requestUri, content, null as JsonSerializerOptions, cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendJsonAsync(HttpMethod method, Uri requestUri, object content, CancellationToken cancellationToken = default)
        => SendJsonAsync(method, requestUri, content, null as JsonSerializerOptions, cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of request body to serialize and send.</typeparam>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="deserializer">The JSON deserializer.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendJsonAsync<TRequestBody>(HttpMethod method, string requestUri, TRequestBody content, Func<TRequestBody, string> deserializer, CancellationToken cancellationToken = default)
        => deserializer != null ? SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Post, requestUri, new StringContent(deserializer(content), Encoding.UTF8, Text.JsonValues.JsonMIME)), cancellationToken) : SendJsonAsync(method, requestUri, content, cancellationToken);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of request body to serialize and send.</typeparam>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="deserializer">The JSON deserializer.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> SendJsonAsync<TRequestBody>(HttpMethod method, Uri requestUri, TRequestBody content, Func<TRequestBody, string> deserializer, CancellationToken cancellationToken = default)
        => deserializer != null ? SendAsync(HttpClientExtensions.CreateRequestMessage(method ?? HttpMethod.Post, requestUri, new StringContent(deserializer(content), Encoding.UTF8, Text.JsonValues.JsonMIME)), cancellationToken) : SendJsonAsync(method, requestUri, content, cancellationToken);

    /// <summary>
    /// Sends an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer as an asynchronous operation.
    /// </summary>
    /// <param name="hostNameOrAddress">The computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
    /// <param name="timeout">The maximum time span to wait for the ICMP echo reply message.</param>
    /// <returns>The ping reply instance.</returns>
    public async Task<System.Net.NetworkInformation.PingReply> SendPingAsync(string hostNameOrAddress, TimeSpan timeout)
    {
        using var ping = new System.Net.NetworkInformation.Ping();
        return await ping.SendPingAsync(hostNameOrAddress, (int)timeout.TotalMilliseconds);
    }

    /// <summary>
    /// Sends an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer as an asynchronous operation.
    /// </summary>
    /// <param name="hostNameOrAddress">The computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
    /// <param name="timeout">The maximum time span to wait for the ICMP echo reply message.</param>
    /// <param name="buffer">A byte array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
    /// <param name="options">An options object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
    /// <returns>The ping reply instance.</returns>
    public async Task<System.Net.NetworkInformation.PingReply> SendPingAsync(string hostNameOrAddress, TimeSpan timeout, byte[] buffer, System.Net.NetworkInformation.PingOptions options = null)
    {
        using var ping = new System.Net.NetworkInformation.Ping();
        return await ping.SendPingAsync(hostNameOrAddress, (int)timeout.TotalMilliseconds, buffer, options);
    }

    /// <summary>
    /// Sends a GET request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> GetAsync(string requestUri, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(HttpMethod.Get, requestUri), cancellationToken);

    /// <summary>
    /// Sends a GET request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> GetAsync(Uri requestUri, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(HttpMethod.Get, requestUri), cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(HttpMethod.Post, requestUri, content), cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(HttpMethod.Post, requestUri, content), cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> PostAsync(string requestUri, Text.JsonObjectNode content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(HttpMethod.Post, requestUri, HttpClientExtensions.CreateJsonContent(content)), cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> PostAsync(Uri requestUri, Text.JsonObjectNode content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(HttpMethod.Post, requestUri, HttpClientExtensions.CreateJsonContent(content)), cancellationToken);

    /// <summary>
    /// Sends a PUT request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(HttpMethod.Put, requestUri, content), cancellationToken);

    /// <summary>
    /// Sends a PUT request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(HttpMethod.Put, requestUri, content), cancellationToken);

    /// <summary>
    /// Sends a PUT request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> PutAsync(string requestUri, Text.JsonObjectNode content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(HttpMethod.Put, requestUri, HttpClientExtensions.CreateJsonContent(content)), cancellationToken);

    /// <summary>
    /// Sends a PUT request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<T> PutAsync(Uri requestUri, Text.JsonObjectNode content, CancellationToken cancellationToken = default)
        => SendAsync(HttpClientExtensions.CreateRequestMessage(HttpMethod.Put, requestUri, HttpClientExtensions.CreateJsonContent(content)), cancellationToken);

    /// <summary>
    /// Gets the exception need throw.
    /// </summary>
    /// <param name="exception">The exception thrown to test.</param>
    /// <returns>true if need retry; otherwise, false.</returns>
    protected virtual Exception GetException(Exception exception)
        => exception;

    /// <summary>
    /// Processes before sending the request message.
    /// </summary>
    /// <param name="requestMessage">The HTTP request message pending to send.</param>
    protected virtual void OnSend(HttpRequestMessage requestMessage)
    {
    }

    /// <summary>
    /// Processes before sending the request message.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="responseMessage">The HTTP response message pending to send.</param>
    protected virtual void OnReceive(T result, HttpResponseMessage responseMessage)
    {
    }

    /// <summary>
    /// Tests if need retry for the exception catched.
    /// </summary>
    /// <param name="exception">The exception thrown to test.</param>
    /// <returns>true if need retry; otherwise, false.</returns>
    private Exception GetExceptionInternal(Exception exception)
        => GetExceptionHandler?.Invoke(exception) ?? GetException(exception);

    /// <summary>
    /// Sends an HTTP request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    protected virtual async Task<T> SendAsync(HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var result = await RetryExtensions.ProcessAsync(RetryPolicy, async (CancellationToken cancellation) =>
        {
            var resp = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!SerializeEvenIfFailed && !resp.IsSuccessStatusCode)
                throw FailedHttpException.Create(resp);
            var obj = Deserializer != null
#if NET6_0_OR_GREATER
                    ? await HttpClientExtensions.DeserializeAsync(resp.Content, Deserializer, cancellationToken)
#else
                ? await HttpClientExtensions.DeserializeAsync(resp.Content, Deserializer)
#endif
                : await HttpClientExtensions.DeserializeJsonAsync<T>(resp.Content, cancellationToken);
            return obj;
        }, GetExceptionInternal, cancellationToken);
        return result.Result;
    }
}
