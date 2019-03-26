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
    /// The OAuth client.
    /// </summary>
    public class OAuthClient : TokenContainer
    {
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
        /// Initializes a new instance of the OAuthClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(AppAccessingKey appKey, TokenInfo tokenCached = null) : base(tokenCached)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Initializes a new instance of the OAuthClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(string appId, string secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OAuthClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

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
        /// Gets or sets the JSON serializer.
        /// </summary>
        public Func<string, Type, object> Serializer { get; set; }

        /// <summary>
        /// Gets or sets the case of authenticiation scheme.
        /// </summary>
        public Cases AuthenticationSchemeCase { get; set; } = Cases.Original;

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
        /// Creates a JSON HTTP client.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <returns>A new JSON HTTP client.</returns>
        public JsonHttpClient<T> Create<T>(Action<ReceivedEventArgs<T>> callback = null)
        {
            var httpClient = new JsonHttpClient<T>();
            httpClient.Sending += (sender, ev) =>
            {
                if (Token == null || ev.RequestMessage.Headers.Authorization != null)
                {
                    Sending?.Invoke(sender, ev);
                    return;
                }

                ev.RequestMessage.Headers.Authorization = Token.ToAuthenticationHeaderValue(AuthenticationSchemeCase);
                Sending?.Invoke(sender, ev);
            };
            if (Serializer != null) httpClient.Serializer = json => (T)Serializer(json, typeof(T));
            if (callback != null) httpClient.Received += (sender, ev) =>
            {
                callback(ev);
            };
            return httpClient;
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
        public Task<TokenInfo> ResolveTokenAsync(TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            if (TokenResolverUri == null) throw new InvalidOperationException("TokenResolverUri property should not be null.");
            return ResolveTokenAsync(TokenResolverUri, content, cancellationToken);
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
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        public Task<TokenInfo> ResolveTokenAsync(Uri requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            var uri = requestUri ?? TokenResolverUri;
            if (uri == null) throw new ArgumentNullException(nameof(uri), "requestUri should not be null.");
            if (content == null) throw new ArgumentNullException(nameof(content), "content should not be null.");
            var c = new TokenRequest<TokenRequestBody>(content, appInfo, Scope);
            return CreateTokenResolveHttpClient().SendAsync(HttpMethod.Post, uri, c.ToQueryData(), cancellationToken);
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
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        public Task<TokenInfo> ResolveTokenAsync(string requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        {
            var uri = requestUri ?? TokenResolverUri?.OriginalString;
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException(nameof(uri), "requestUri should not be null.");
            if (content == null) throw new ArgumentNullException(nameof(content), "content should not be null.");
            var c = new TokenRequest<TokenRequestBody>(content, appInfo, Scope);
            return CreateTokenResolveHttpClient().SendAsync(HttpMethod.Post, uri, c.ToQueryData(), cancellationToken);
        }

        /// <summary>
        /// Creates the JSON HTTP web client for resolving token.
        /// </summary>
        /// <returns>A result serialized.</returns>
        private JsonHttpClient<TokenInfo> CreateTokenResolveHttpClient()
        {
            var httpClient = new JsonHttpClient<TokenInfo>
            {
                SerializeEvenIfFailed = true
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
            if (Serializer != null) httpClient.Serializer = json => (TokenInfo)Serializer(json, typeof(TokenInfo));
            if (TokenResolving != null) httpClient.Sending += (sender, ev) =>
            {
                TokenResolving?.Invoke(sender, ev);
            };
            return httpClient;
        }
    }
}
