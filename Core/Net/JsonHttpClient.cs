// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonHttpClient.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The JSON HTTP web client and its event arguments.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
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

using Trivial.Web;

namespace Trivial.Net
{
    /// <summary>
    /// The event arguments on sending.
    /// </summary>
    public class SendingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the SendingEventArgs class.
        /// </summary>
        /// <param name="requestMessage">The HTTP request message.</param>
        public SendingEventArgs(HttpRequestMessage requestMessage)
        {
            RequestMessage = requestMessage;
        }

        /// <summary>
        /// Gets the HTTP request message.
        /// </summary>
        public HttpRequestMessage RequestMessage { get; }
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
        {
            return new ReceivedEventArgs<U>(Result, this);
        }
    }

    /// <summary>
    /// JSON format serialization HTTP client.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class JsonHttpClient<T>
    {
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
        public Tasks.IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Gets or sets the JSON deserializer.
        /// </summary>
        public Func<string, T> Deserializer { get; set; }

        /// <summary>
        /// Gets or sets the HTTP client.
        /// </summary>
        public HttpClient Client { get; set; }

        /// <summary>
        /// Gets or sets a value inidcating whether need create new HTTP client instance per request when the property Client is null.
        /// </summary>
        public bool IsNewHttpClientByDefault { get; set; }

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Gets or sets The maximum number of bytes to buffer when reading the response content.
        /// The default value for this property is 2 gigabytes.
        /// </summary>
        public long? MaxResponseContentBufferSize { get; set; }

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
        /// Initializes a new instance of the JsonHttpClient class.
        /// </summary>
        public JsonHttpClient()
        {
            var d = WebFormat.GetJsonDeserializer<T>(true);
            if (d != null) Deserializer = d;
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="callback">An optional callback raised on data received.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public async Task<T> SendAsync(HttpRequestMessage request, Action<ReceivedEventArgs<T>> callback, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request), "request should not be null.");
            if (Client == null && !IsNewHttpClientByDefault) Client = new HttpClient();
            var client = Client ?? new HttpClient();
            var timeout = Timeout;
            if (timeout.HasValue)
            {
                try
                {
                    client.Timeout = timeout.Value;
                }
                catch (ArgumentException)
                {
                }
            }

            var maxBufferSize = MaxResponseContentBufferSize;
            if (maxBufferSize.HasValue)
            {
                try
                {
                    client.MaxResponseContentBufferSize = maxBufferSize.Value;
                }
                catch (ArgumentException)
                {
                }
            }

            Sending?.Invoke(this, new SendingEventArgs(request));
            cancellationToken.ThrowIfCancellationRequested();
            OnSend(request);
            HttpResponseMessage resp = null;
            T valueResult;
            try
            {
                var result = await Tasks.RetryExtensions.ProcessAsync(RetryPolicy, async (CancellationToken cancellation) =>
                {
                    resp = await client.SendAsync(request, cancellationToken);
                    if (!SerializeEvenIfFailed && !resp.IsSuccessStatusCode)
                        throw FailedHttpException.Create(resp, "Failed to send JSON HTTP web request because of unsuccess status code.");
                    var obj = Deserializer != null
#if NET5_0_OR_GREATER
                        ? await HttpClientExtensions.DeserializeAsync(resp.Content, Deserializer, cancellationToken)
#else
                        ? await HttpClientExtensions.DeserializeAsync(resp.Content, Deserializer)
#endif
                        : await HttpClientExtensions.DeserializeJsonAsync<T>(resp.Content, cancellationToken);
                    return obj;
                }, GetExceptionInternal, cancellationToken);
                valueResult = result.Result;
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
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, null, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendAsync(HttpMethod method, string requestUri, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(method, requestUri), cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendAsync(HttpMethod method, Uri requestUri, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(method, requestUri), cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendAsync(HttpMethod method, string requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = content
            }, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendAsync(HttpMethod method, Uri requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = content
            }, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendAsync(HttpMethod method, string requestUri, QueryData content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(content.ToString(), Encoding.UTF8, Web.WebFormat.FormUrlMIME)
            }, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendAsync(HttpMethod method, Uri requestUri, QueryData content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(content.ToString(), Encoding.UTF8, Web.WebFormat.FormUrlMIME)
            }, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="options">The options for serialization.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public async Task<T> SendJsonAsync(HttpMethod method, string requestUri, object content, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            using var body = HttpClientExtensions.CreateJsonContent(content, options);
            return await SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = body
            }, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="options">The options for serialization.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public async Task<T> SendJsonAsync(HttpMethod method, Uri requestUri, object content, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            using var body = HttpClientExtensions.CreateJsonContent(content, options);
            return await SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = body
            }, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="options">The options for serialization.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public async Task<T> SendJsonAsync(HttpMethod method, string requestUri, object content, DataContractJsonSerializerSettings options, CancellationToken cancellationToken = default)
        {
            var json = Text.StringExtensions.ToJson(content, options) ?? string.Empty;
            using var str = new StringContent(json, Encoding.UTF8, WebFormat.JsonMIME);
            return await SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = str
            }, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="options">The options for serialization.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public async Task<T> SendJsonAsync(HttpMethod method, Uri requestUri, object content, DataContractJsonSerializerSettings options, CancellationToken cancellationToken = default)
        {
            var json = Text.StringExtensions.ToJson(content, options) ?? string.Empty;
            using var str = new StringContent(json, Encoding.UTF8, WebFormat.JsonMIME);
            return await SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = str
            }, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendJsonAsync(HttpMethod method, string requestUri, object content, CancellationToken cancellationToken = default)
        {
            return SendJsonAsync(method, requestUri, content, null as JsonSerializerOptions, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendJsonAsync(HttpMethod method, Uri requestUri, object content, CancellationToken cancellationToken = default)
        {
            return SendJsonAsync(method, requestUri, content, null as JsonSerializerOptions, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <typeparam name="TRequestBody">The type of request body to serialize and send.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="deserializer">The JSON deserializer.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendJsonAsync<TRequestBody>(HttpMethod method, string requestUri, TRequestBody content, Func<TRequestBody, string> deserializer, CancellationToken cancellationToken = default)
        {
            return deserializer != null ? SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(deserializer(content), Encoding.UTF8, Trivial.Web.WebFormat.JsonMIME)
            }, cancellationToken) : SendJsonAsync(method, requestUri, content, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <typeparam name="TRequestBody">The type of request body to serialize and send.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="deserializer">The JSON deserializer.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> SendJsonAsync<TRequestBody>(HttpMethod method, Uri requestUri, TRequestBody content, Func<TRequestBody, string> deserializer, CancellationToken cancellationToken = default)
        {
            return deserializer != null ? SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(deserializer(content), Encoding.UTF8, Trivial.Web.WebFormat.JsonMIME)
            }, cancellationToken) : SendJsonAsync(method, requestUri, content, cancellationToken);
        }

        /// <summary>
        /// Sends a GET request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> GetAsync(string requestUri, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
        }

        /// <summary>
        /// Sends a GET request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> GetAsync(Uri requestUri, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content
            }, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content
            }, cancellationToken);
        }

        /// <summary>
        /// Sends a PUT request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = content
            }, cancellationToken);
        }

        /// <summary>
        /// Sends a PUT request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
        public Task<T> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = content
            }, cancellationToken);
        }

        /// <summary>
        /// Gets the exception need throw.
        /// </summary>
        /// <param name="exception">The exception thrown to test.</param>
        /// <returns>true if need retry; otherwise, false.</returns>
        protected virtual Exception GetException(Exception exception)
        {
            return exception;
        }

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
        {
            return GetExceptionHandler?.Invoke(exception) ?? GetException(exception);
        }
    }
}
