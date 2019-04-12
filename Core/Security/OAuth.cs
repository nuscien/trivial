using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Trivial.Data;
using Trivial.Net;
using Trivial.Text;

namespace Trivial.Security
{
    /// <summary>
    /// <para>The OAuth HTTP web client (for RFC-6749).</para>
    /// <para>You can use this to login and then create the JSON HTTP web clients with the authentication information.</para>
    /// </summary>
    public class OAuthClient : TokenContainer, IDisposable
    {
        /// <summary>
        /// A value indiciating whether need dispose the app accessing key automatically.
        /// </summary>
        private readonly bool needDisposeAppInfo;

        /// <summary>
        /// The app accessing key instance.
        /// </summary>
        private readonly AppAccessingKey appInfo;

        /// <summary>
        /// Initializes a new instance of the OAuthClient class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(TokenInfo tokenCached) : base(tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthHttpClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(AppAccessingKey appKey, TokenInfo tokenCached = null) : base(tokenCached)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Initializes a new instance of the OAuthHttpClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(string appId, string secretKey = null, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
            needDisposeAppInfo = true;
        }

        /// <summary>
        /// Initializes a new instance of the OAuthHttpClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
            needDisposeAppInfo = true;
        }

        /// <summary>
        /// Initializes a new instance of the OAuthHttpClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenResolveUri">The token resolve URI.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(AppAccessingKey appKey, Uri tokenResolveUri, IEnumerable<string> scope, TokenInfo tokenCached = null) : this(appKey, tokenCached)
        {
            TokenResolverUri = tokenResolveUri;
            if (scope == null) return;
            foreach (var ele in scope)
            {
                Scope.Add(ele);
            }
        }

        /// <summary>
        /// Initializes a new instance of the OAuthHttpClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenResolveUri">The token resolve URI.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(string appId, string secretKey, Uri tokenResolveUri, IEnumerable<string> scope, TokenInfo tokenCached = null) : this(appId, secretKey, tokenCached)
        {
            TokenResolverUri = tokenResolveUri;
            if (scope == null) return;
            foreach (var ele in scope)
            {
                Scope.Add(ele);
            }
        }

        /// <summary>
        /// Initializes a new instance of the OAuthHttpClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenResolveUri">The token resolve URI.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(string appId, SecureString secretKey, Uri tokenResolveUri, IEnumerable<string> scope, TokenInfo tokenCached = null) : this(appId, secretKey, tokenCached)
        {
            TokenResolverUri = tokenResolveUri;
            if (scope == null) return;
            foreach (var ele in scope)
            {
                Scope.Add(ele);
            }
        }

        /// <summary>
        /// Adds or removes a handler raised on sending.
        /// </summary>
        public event EventHandler<SendingEventArgs> Sending;

        /// <summary>
        /// Adds or removes a handler raised on token is resolving.
        /// </summary>
        public event EventHandler<SendingEventArgs> TokenResolving;

        /// <summary>
        /// Adds or removes a handler raised on token has resolved.
        /// </summary>
        public event EventHandler<ReceivedEventArgs<TokenInfo>> TokenResolved;

        /// <summary>
        /// Gets the app identifier.
        /// </summary>
        public string AppId => appInfo.Id;

        /// <summary>
        /// Gets or sets the token resolver URI.
        /// </summary>
        public Uri TokenResolverUri { get; set; }

        /// <summary>
        /// Gets the scope to use.
        /// </summary>
        public IList<string> Scope { get; private set; } = new List<string>();

        /// <summary>
        /// Gets the scope string.
        /// </summary>
        public string ScopeString
        {
            get
            {
                return string.Join(" ", Scope);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Scope.Clear();
                    return;
                }

                Scope = value.Split(' ');
            }
        }

        /// <summary>
        /// Gets or sets the JSON deserializer.
        /// </summary>
        public Func<string, Type, object> Deserializer { get; set; }

        /// <summary>
        /// Gets or sets a handler to indicate whether the token request is valid and fill additional information if needed.
        /// </summary>
        public Func<QueryData, bool> TokenRequestInfoValidator { get; set; }

        /// <summary>
        /// Gets or sets the HTTP web client handler for sending message.
        /// But token resolving will not use this.
        /// </summary>
        public HttpClientHandler HttpClientHandler { get; set; }

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of bytes to buffer when reading the response content.
        /// The default value for this property is 2 gigabytes.
        /// </summary>
        public long? MaxResponseContentBufferSize { get; set; }

        /// <summary>
        /// Creates a JSON HTTP client.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <param name="callback">An optional callback raised on data received.</param>
        /// <returns>A new JSON HTTP client.</returns>
        public virtual JsonHttpClient<T> Create<T>(Action<ReceivedEventArgs<T>> callback = null)
        {
            var httpClient = new JsonHttpClient<T>();
            if (HttpClientHandler != null) httpClient.Client = new HttpClient(HttpClientHandler, false);
            httpClient.Timeout = Timeout;
            httpClient.MaxResponseContentBufferSize = MaxResponseContentBufferSize;
            httpClient.Sending += (sender, ev) =>
            {
                WriteAuthenticationHeaderValue(ev.RequestMessage.Headers);
                Sending?.Invoke(sender, ev);
            };
            if (Deserializer != null) httpClient.Deserializer = json => (T)Deserializer(json, typeof(T));
            if (callback != null) httpClient.Received += (sender, ev) =>
            {
                callback(ev);
            };
            return httpClient;
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request), "request should not be null");
            var client = CreateHttpClient();
            WriteAuthenticationHeaderValue(request.Headers);
            Sending?.Invoke(this, new SendingEventArgs(request));
            return client.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request), "request should not be null");
            var client = CreateHttpClient();
            WriteAuthenticationHeaderValue(request.Headers);
            Sending?.Invoke(this, new SendingEventArgs(request));
            return client.SendAsync(request, completionOption, cancellationToken);
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return Create<T>().SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="callback">An optional callback raised on data received.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public Task<T> SendAsync<T>(HttpRequestMessage request, Action<ReceivedEventArgs<T>> callback, CancellationToken cancellationToken = default)
        {
            return Create<T>().SendAsync(request, callback, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">content was null.</exception>
        /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        public Task<TokenInfo> ResolveTokenAsync(TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            if (TokenResolverUri == null) throw new InvalidOperationException("TokenResolverUri property should not be null.");
            return ResolveTokenAsync(TokenResolverUri, content, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <typeparam name="T">The token info type.</typeparam>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">content was null.</exception>
        /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        public Task<T> ResolveTokenAsync<T>(TokenRequestBody content, CancellationToken cancellationToken = default) where T : TokenInfo
        {
            if (TokenResolverUri == null) throw new InvalidOperationException("TokenResolverUri property should not be null.");
            return ResolveTokenAsync<T>(TokenResolverUri, content, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">requestUri or content was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        public Task<TokenInfo> ResolveTokenAsync(Uri requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            var uri = requestUri ?? TokenResolverUri;
            if (uri == null) throw new ArgumentNullException(nameof(uri), "requestUri should not be null.");
            if (content == null) throw new ArgumentNullException(nameof(content), "content should not be null.");
            var c = new TokenRequest<TokenRequestBody>(content, appInfo, Scope);
            var q = c.ToQueryData();
            if (TokenRequestInfoValidator != null && !TokenRequestInfoValidator(q)) throw new InvalidOperationException("Cannot pass the request because it is not valid.");
            return CreateTokenResolveHttpClient().SendAsync(HttpMethod.Post, uri, q, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">requestUri was null, empty, or consists only of white-space characters; or, content was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        public Task<TokenInfo> ResolveTokenAsync(string requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            var uri = requestUri ?? TokenResolverUri?.OriginalString;
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException(nameof(uri), "requestUri should not be null.");
            if (content == null) throw new ArgumentNullException(nameof(content), "content should not be null.");
            var c = new TokenRequest<TokenRequestBody>(content, appInfo, Scope);
            var q = c.ToQueryData();
            if (TokenRequestInfoValidator != null && !TokenRequestInfoValidator(q)) throw new InvalidOperationException("Cannot pass the request because it is not valid.");
            return CreateTokenResolveHttpClient().SendAsync(HttpMethod.Post, uri, q, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <typeparam name="T">The token info type.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">requestUri or content was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        public Task<T> ResolveTokenAsync<T>(Uri requestUri, TokenRequestBody content, CancellationToken cancellationToken = default) where T : TokenInfo
        {
            var uri = requestUri ?? TokenResolverUri;
            if (uri == null) throw new ArgumentNullException(nameof(uri), "requestUri should not be null.");
            if (content == null) throw new ArgumentNullException(nameof(content), "content should not be null.");
            var c = new TokenRequest(content, appInfo, Scope);
            var q = c.ToQueryData();
            if (TokenRequestInfoValidator != null && !TokenRequestInfoValidator(q)) throw new InvalidOperationException("Cannot pass the request because it is not valid.");
            return CreateTokenResolveHttpClient<T>().SendAsync(HttpMethod.Post, uri, q, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <typeparam name="T">The token info type.</typeparam>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">requestUri was null, empty, or consists only of white-space characters; or, content was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        public Task<T> ResolveTokenAsync<T>(string requestUri, TokenRequestBody content, CancellationToken cancellationToken = default) where T : TokenInfo
        {
            var uri = requestUri ?? TokenResolverUri?.OriginalString;
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException(nameof(uri), "requestUri should not be null.");
            if (content == null) throw new ArgumentNullException(nameof(content), "content should not be null.");
            var c = new TokenRequest(content, appInfo, Scope);
            var q = c.ToQueryData();
            if (TokenRequestInfoValidator != null && !TokenRequestInfoValidator(q)) throw new InvalidOperationException("Cannot pass the request because it is not valid.");
            return CreateTokenResolveHttpClient<T>().SendAsync(HttpMethod.Post, uri, q, cancellationToken);
        }

        /// <summary>
        /// Creates the JSON HTTP web client for resolving token.
        /// </summary>
        /// <returns>A result serialized.</returns>
        private JsonHttpClient<TokenInfo> CreateTokenResolveHttpClient()
        {
            var httpClient = new JsonHttpClient<TokenInfo>
            {
                SerializeEvenIfFailed = true,
                Timeout = Timeout,
                MaxResponseContentBufferSize = MaxResponseContentBufferSize
            };
            httpClient.Received += (sender, ev) =>
            {
                if (!ev.IsSuccessStatusCode)
                {
                    TokenResolved?.Invoke(sender, ev);
                    return;
                }

                Token = ev.Result;
                TokenResolved?.Invoke(sender, ev);
            };
            if (Deserializer != null) httpClient.Deserializer = json => (TokenInfo)Deserializer(json, typeof(TokenInfo));
            if (TokenResolving != null) httpClient.Sending += (sender, ev) =>
            {
                TokenResolving?.Invoke(sender, ev);
            };
            return httpClient;
        }

        /// <summary>
        /// Creates the JSON HTTP web client for resolving token.
        /// </summary>
        /// <returns>A result serialized.</returns>
        private JsonHttpClient<T> CreateTokenResolveHttpClient<T>() where T : TokenInfo
        {
            var httpClient = new JsonHttpClient<T>
            {
                SerializeEvenIfFailed = true,
                Timeout = Timeout,
                MaxResponseContentBufferSize = MaxResponseContentBufferSize
            };
            httpClient.Received += (sender, ev) =>
            {
                if (!ev.IsSuccessStatusCode)
                {
                    TokenResolved?.Invoke(sender, ev.ConvertTo<TokenInfo>());
                    return;
                }

                Token = ev.Result;
                TokenResolved?.Invoke(sender, ev.ConvertTo<TokenInfo>());
            };
            if (Deserializer != null) httpClient.Deserializer = json => (T)Deserializer(json, typeof(T));
            if (TokenResolving != null) httpClient.Sending += (sender, ev) =>
            {
                TokenResolving?.Invoke(sender, ev);
            };
            return httpClient;
        }

        /// <summary>
        /// Creats an HTTP web client.
        /// </summary>
        /// <returns>The HTTP web client.</returns>
        private HttpClient CreateHttpClient()
        {
            var client = HttpClientHandler != null ? new HttpClient(HttpClientHandler, false) : new HttpClient();
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

            return client;
        }

        /// <summary>
        /// Releases all resources used by the current OAuth client object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (needDisposeAppInfo && appInfo != null) appInfo.Dispose();
        }
    }

    /// <summary>
    /// The OAuth based JSON HTTP web client.
    /// </summary>
    public abstract class OAuthBasedClient : TokenContainer, IDisposable
    {
        /// <summary>
        /// The OAuth client.
        /// </summary>
        internal readonly OAuthClient oauth;

        /// <summary>
        /// Initializes a new instance of the OAuthBasedClient class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthBasedClient(TokenInfo tokenCached) : base(tokenCached)
        {
            oauth = new OAuthClient(tokenCached);
            oauth.TokenChanged += (sender, ev) =>
            {
                if (Token == ev.NewValue) return;
                Token = ev.NewValue;
            };
            TokenChanged += (sender, ev) =>
            {
                if (oauth.Token == ev.NewValue) return;
                oauth.Token = ev.NewValue;
            };
        }

        /// <summary>
        /// Initializes a new instance of the OAuthBasedClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthBasedClient(AppAccessingKey appKey, TokenInfo tokenCached = null) : base(tokenCached)
        {
            oauth = new OAuthClient(appKey, tokenCached);
            oauth.TokenChanged += (sender, ev) =>
            {
                if (Token == ev.NewValue) return;
                Token = ev.NewValue;
            };
            TokenChanged += (sender, ev) =>
            {
                if (Token == ev.NewValue) return;
                oauth.Token = ev.NewValue;
            };
        }

        /// <summary>
        /// Initializes a new instance of the OAuthBasedClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthBasedClient(string appId, string secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthBasedClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthBasedClient(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Adds or removes a handler raised on sending.
        /// </summary>
        public event EventHandler<SendingEventArgs> Sending
        {
            add
            {
                oauth.Sending += value;
            }

            remove
            {
                oauth.Sending -= value;
            }
        }

        /// <summary>
        /// Adds or removes a handler raised on token is resolving.
        /// </summary>
        protected event EventHandler<SendingEventArgs> TokenResolving
        {
            add
            {
                oauth.TokenResolving += value;
            }

            remove
            {
                oauth.TokenResolving -= value;
            }
        }

        /// <summary>
        /// Adds or removes a handler raised on token has resolved.
        /// </summary>
        protected event EventHandler<ReceivedEventArgs<TokenInfo>> TokenResolved
        {
            add
            {
                oauth.TokenResolved += value;
            }

            remove
            {
                oauth.TokenResolved -= value;
            }
        }

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// </summary>
        public TimeSpan? Timeout
        {
            get => oauth.Timeout;
            set => oauth.Timeout = value;
        }

        /// <summary>
        /// Gets or sets the maximum number of bytes to buffer when reading the response content.
        /// The default value for this property is 2 gigabytes.
        /// </summary>
        public long? MaxResponseContentBufferSize
        {
            get => oauth.MaxResponseContentBufferSize;
            set => oauth.MaxResponseContentBufferSize = value;
        }

        /// <summary>
        /// Gets or sets the token resolver URI.
        /// </summary>
        protected Uri TokenResolverUri
        {
            get => oauth.TokenResolverUri;
            set => oauth.TokenResolverUri = value;
        }

        /// <summary>
        /// Gets the scope to use.
        /// </summary>
        protected IList<string> Scope => oauth.Scope;

        /// <summary>
        /// Gets the scope string.
        /// </summary>
        protected string ScopeString
        {
            get => oauth.ScopeString;
            set => oauth.ScopeString = value;
        }

        /// <summary>
        /// Gets or sets a handler to indicate whether the token request is valid and fill additional information if needed.
        /// </summary>
        public Func<QueryData, bool> TokenRequestInfoValidator
        {
            get => oauth.TokenRequestInfoValidator;
            set => oauth.TokenRequestInfoValidator = value;
        }

        /// <summary>
        /// Gets or sets the case of authenticiation scheme.
        /// </summary>
        public override Cases AuthenticationSchemeCase
        {
            get => oauth.AuthenticationSchemeCase;
            set => oauth.AuthenticationSchemeCase = value;
        }

        /// <summary>
        /// Gets or sets the converter of authentication header value.
        /// </summary>
        public override Func<TokenInfo, Cases, AuthenticationHeaderValue> ConvertToAuthenticationHeaderValue
        {
            get => oauth.ConvertToAuthenticationHeaderValue;
            set => oauth.ConvertToAuthenticationHeaderValue = value;
        }

        /// <summary>
        /// Gets the app identifier.
        /// </summary>
        public string AppId => oauth.AppId;

        /// <summary>
        /// Gets or sets the JSON deserializer.
        /// </summary>
        public Func<string, Type, object> Deserializer
        {
            get => oauth.Deserializer;
            set => oauth.Deserializer = value;
        }

        /// <summary>
        /// Creates a JSON HTTP client.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <returns>A new JSON HTTP client.</returns>
        public virtual JsonHttpClient<T> Create<T>(Action<ReceivedEventArgs<T>> callback = null)
        {
            return oauth.Create(callback);
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return oauth.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
        {
            return oauth.SendAsync(request, completionOption, cancellationToken);
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return Create<T>().SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="callback">An optional callback raised on data received.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The request was null.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public Task<T> SendAsync<T>(HttpRequestMessage request, Action<ReceivedEventArgs<T>> callback, CancellationToken cancellationToken = default)
        {
            return Create<T>().SendAsync(request, callback, cancellationToken);
        }

        /// <summary>
        /// Releases all resources used by the current OAuth client object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (oauth != null) oauth.Dispose();
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">content was null.</exception>
        /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        protected Task<TokenInfo> ResolveTokenAsync(TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            return oauth.ResolveTokenAsync(content, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">content was null.</exception>
        /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        protected Task<TokenInfo> ResolveTokenAsync(Uri requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            return oauth.ResolveTokenAsync(requestUri, content, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">content was null.</exception>
        /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        protected Task<TokenInfo> ResolveTokenAsync(string requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            return oauth.ResolveTokenAsync(requestUri, content, cancellationToken);
        }
    }

    /// <summary>
    /// The OAuth based JSON HTTP web client.
    /// </summary>
    public abstract class OAuthBasedClient<T> : OAuthBasedClient where T : TokenInfo
    {
        /// <summary>
        /// Initializes a new instance of the OAuthBasedClient class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthBasedClient(T tokenCached) : base(tokenCached)
        {
            oauth.TokenResolved += (sender, ev) =>
            {
                TokenResolved?.Invoke(sender, ev.ConvertTo<T>());
            };
        }

        /// <summary>
        /// Initializes a new instance of the OAuthBasedClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthBasedClient(AppAccessingKey appKey, T tokenCached = null) : base(appKey, tokenCached)
        {
            oauth.TokenResolved += (sender, ev) =>
            {
                TokenResolved?.Invoke(sender, ev.ConvertTo<T>());
            };
        }

        /// <summary>
        /// Initializes a new instance of the OAuthBasedClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthBasedClient(string appId, string secretKey, T tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthBasedClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthBasedClient(string appId, SecureString secretKey, T tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Adds or removes a handler raised on token has resolved.
        /// </summary>
        protected new event EventHandler<ReceivedEventArgs<T>> TokenResolved;

        /// <summary>
        /// Gets or sets the converter of authentication header value.
        /// </summary>
        protected new Func<T, Cases, AuthenticationHeaderValue> ConvertToAuthenticationHeaderValue
        {
            get
            {
                return oauth.ConvertToAuthenticationHeaderValue;
            }

            set
            {
                if (value == null)
                {
                    oauth.ConvertToAuthenticationHeaderValue = null;
                    return;
                }

                oauth.ConvertToAuthenticationHeaderValue = (tokenInfo, authCase) =>
                {
                    var ti = Token;
                    if (tokenInfo != ti)
                    {
                        ti = tokenInfo as T;
                        if (ti == null) return null;
                    }

                    return value(ti, authCase);
                };
            }
        }

        /// <summary>
        /// Gets the token information instance saved in this container.
        /// </summary>
        public new T Token
        {
            get
            {
                return (T)base.Token;
            }

            protected set
            {
                base.Token = value;
            }
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">content was null.</exception>
        /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        protected new Task<T> ResolveTokenAsync(TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            return oauth.ResolveTokenAsync<T>(content, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">content was null.</exception>
        /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        protected new Task<T> ResolveTokenAsync(Uri requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            return oauth.ResolveTokenAsync<T>(requestUri, content, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request and gets the result serialized by JSON.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">content was null.</exception>
        /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        protected new Task<T> ResolveTokenAsync(string requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            return oauth.ResolveTokenAsync<T>(requestUri, content, cancellationToken);
        }
    }
}
