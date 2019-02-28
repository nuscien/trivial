using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Net
{
    /// <summary>
    /// JSON format serialization HTTP client.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class JsonHttpClient<T>
    {
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
        public async Task<T> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (Client == null && !IsNewHttpClientByDefault) Client = new HttpClient();
            var client = Client ?? new HttpClient();
            var result = await Tasks.RetryExtension.ProcessAsync(RetryPolicy, async (CancellationToken cancellation) =>
            {
                var resp = await client.SendAsync(request, cancellationToken);
                if (!SerializeEvenIfFailed && !resp.IsSuccessStatusCode) throw new FailedHttpException(resp);
                var obj = Serializer != null
                    ? await HttpClientUtility.SerializeAsync(resp.Content, Serializer)
                    : await HttpClientUtility.SerializeJsonAsync<T>(resp.Content);
                return obj;
            }, GetExceptionInternal, cancellationToken);
            return result.Result;
        }

        /// <summary>
        /// Sends an HTTP request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
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
        public Task<T> SendAsync(HttpMethod method, Uri requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = content
            }, cancellationToken);
        }

        /// <summary>
        /// Sends a GET request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
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
        public Task<T> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken = default)
        {
            return SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
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
