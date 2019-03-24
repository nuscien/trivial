using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Security;

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
        /// <param name="responseMessage">The HTTP response message.</param>
        public ReceivedEventArgs(T result, HttpResponseMessage responseMessage) : base(responseMessage)
        {
            Result = result;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public T Result { get; }
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
        /// Gets or sets the JSON serializer.
        /// </summary>
        public Func<string, T> Serializer { get; set; }

        /// <summary>
        /// Gets or sets the HTTP client.
        /// </summary>
        public HttpClient Client { get; set; }

        /// <summary>
        /// Gets or sets a value inidcating whether need create new HTTP client instance per request by default.
        /// </summary>
        public bool IsNewHttpClientByDefault { get; set; }

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
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        public async Task<T> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request), "request should not be null.");
            if (Client == null && !IsNewHttpClientByDefault) Client = new HttpClient();
            var client = Client ?? new HttpClient();
            Sending?.Invoke(this, new SendingEventArgs(request));
            cancellationToken.ThrowIfCancellationRequested();
            OnSend(request);
            HttpResponseMessage resp = null;
            try
            {
                var result = await Tasks.RetryExtension.ProcessAsync(RetryPolicy, async (CancellationToken cancellation) =>
                {
                    resp = await client.SendAsync(request, cancellationToken);
                    if (!SerializeEvenIfFailed && !resp.IsSuccessStatusCode)
                        throw new FailedHttpException(resp);
                    var obj = Serializer != null
                        ? await HttpClientUtility.SerializeAsync(resp.Content, Serializer)
                        : await HttpClientUtility.SerializeJsonAsync<T>(resp.Content);
                    return obj;
                }, GetExceptionInternal, cancellationToken);
                OnReceive(result.Result, resp);
                Received?.Invoke(this, new ReceivedEventArgs<T>(result.Result, resp));
                return result.Result;
            }
            catch (FailedHttpException)
            {
                OnReceive(default, resp);
                Received?.Invoke(this, new ReceivedEventArgs<T>(default, resp));
                throw;
            }
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
        public Task<T> SendAsync(HttpMethod method, string requestUri, QueryData content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(content.ToString(), Encoding.UTF8, Web.WebUtility.FormUrlMIME)
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
        public Task<T> SendAsync(HttpMethod method, Uri requestUri, QueryData content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(content.ToString(), Encoding.UTF8, Web.WebUtility.FormUrlMIME)
            }, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="settings">The options for serialization.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        public async Task<T> SendJsonAsync(HttpMethod method, string requestUri, object content, DataContractJsonSerializerSettings settings, CancellationToken cancellationToken = default)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = settings != null ? new DataContractJsonSerializer(content.GetType(), settings) : new DataContractJsonSerializer(content.GetType());
                serializer.WriteObject(stream, content);
                return await SendAsync(new HttpRequestMessage(method, requestUri)
                {
                    Content = new StreamContent(stream)
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="settings">The options for serialization.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        public async Task<T> SendJsonAsync(HttpMethod method, Uri requestUri, object content, DataContractJsonSerializerSettings settings, CancellationToken cancellationToken = default)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = settings != null ? new DataContractJsonSerializer(content.GetType(), settings) : new DataContractJsonSerializer(content.GetType());
                serializer.WriteObject(stream, content);
                return await SendAsync(new HttpRequestMessage(method, requestUri)
                {
                    Content = new StreamContent(stream)
                }, cancellationToken);
            }
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
        public Task<T> SendJsonAsync(HttpMethod method, string requestUri, object content, CancellationToken cancellationToken = default)
        {
            return SendJsonAsync(method, requestUri, content, null as DataContractJsonSerializerSettings, cancellationToken);
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
        public Task<T> SendJsonAsync(HttpMethod method, Uri requestUri, object content, CancellationToken cancellationToken = default)
        {
            return SendJsonAsync(method, requestUri, content, null as DataContractJsonSerializerSettings, cancellationToken);
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
        public Task<T> SendJsonAsync<TRequestBody>(HttpMethod method, string requestUri, TRequestBody content, Func<TRequestBody, string> deserializer, CancellationToken cancellationToken = default)
        {
            return deserializer != null ? SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(deserializer(content), Encoding.UTF8, Trivial.Web.WebUtility.JsonMIME)
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
        public Task<T> SendJsonAsync<TRequestBody>(HttpMethod method, Uri requestUri, TRequestBody content, Func<TRequestBody, string> deserializer, CancellationToken cancellationToken = default)
        {
            return deserializer != null ? SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(deserializer(content), Encoding.UTF8, Trivial.Web.WebUtility.JsonMIME)
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
