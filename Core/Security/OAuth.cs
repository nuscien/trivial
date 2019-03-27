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
        /// Gets or sets the case of authenticiation scheme.
        /// </summary>
        public Cases AuthenticationSchemeCase { get; set; } = Cases.Original;

        /// <summary>
        /// Gets or sets the JSON serializer.
        /// </summary>
        public Func<string, Type, object> Serializer { get; set; }

        /// <summary>
        /// Gets or sets the converter of authentication header value.
        /// </summary>
        public Func<TokenInfo, Cases, AuthenticationHeaderValue> ConvertToAuthenticationHeaderValue { get; set; }

        /// <summary>
        /// Creates a JSON HTTP client.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <returns>A new JSON HTTP client.</returns>
        public virtual JsonHttpClient<T> Create<T>(Action<ReceivedEventArgs<T>> callback = null)
        {
            var httpClient = new JsonHttpClient<T>();
            httpClient.Sending += (sender, ev) =>
            {
                if (Token != null && ev.RequestMessage.Headers.Authorization == null)
                    ev.RequestMessage.Headers.Authorization = ConvertToAuthenticationHeaderValue != null
                        ? ConvertToAuthenticationHeaderValue(Token, AuthenticationSchemeCase)
                        : Token.ToAuthenticationHeaderValue(AuthenticationSchemeCase);
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
        /// <typeparam name="T">The token info type.</typeparam>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="ArgumentNullException">content was null.</exception>
        /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
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
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        public Task<T> ResolveTokenAsync<T>(Uri requestUri, TokenRequestBody content, CancellationToken cancellationToken = default) where T : TokenInfo
        {
            var uri = requestUri ?? TokenResolverUri;
            if (uri == null) throw new ArgumentNullException(nameof(uri), "requestUri should not be null.");
            if (content == null) throw new ArgumentNullException(nameof(content), "content should not be null.");
            var c = new TokenRequest<TokenRequestBody>(content, appInfo, Scope);
            return CreateTokenResolveHttpClient<T>().SendAsync(HttpMethod.Post, uri, c.ToQueryData(), cancellationToken);
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
        /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
        public Task<T> ResolveTokenAsync<T>(string requestUri, TokenRequestBody content, CancellationToken cancellationToken = default) where T : TokenInfo
        {
            var uri = requestUri ?? TokenResolverUri?.OriginalString;
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException(nameof(uri), "requestUri should not be null.");
            if (content == null) throw new ArgumentNullException(nameof(content), "content should not be null.");
            var c = new TokenRequest<TokenRequestBody>(content, appInfo, Scope);
            return CreateTokenResolveHttpClient<T>().SendAsync(HttpMethod.Post, uri, c.ToQueryData(), cancellationToken);
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

        /// <summary>
        /// Creates the JSON HTTP web client for resolving token.
        /// </summary>
        /// <returns>A result serialized.</returns>
        private JsonHttpClient<T> CreateTokenResolveHttpClient<T>() where T : TokenInfo
        {
            var httpClient = new JsonHttpClient<T>
            {
                SerializeEvenIfFailed = true
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
            if (Serializer != null) httpClient.Serializer = json => (T)Serializer(json, typeof(TokenInfo));
            if (TokenResolving != null) httpClient.Sending += (sender, ev) =>
            {
                TokenResolving?.Invoke(sender, ev);
            };
            return httpClient;
        }
    }

    /// <summary>
    /// The OAuth based JSON HTTP web client.
    /// </summary>
    public abstract class OAuthBasedClient : TokenContainer
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
        /// Gets or sets the case of authenticiation scheme.
        /// </summary>
        protected Cases AuthenticationSchemeCase
        {
            get => oauth.AuthenticationSchemeCase;
            set => oauth.AuthenticationSchemeCase = value;
        }

        /// <summary>
        /// Gets or sets the converter of authentication header value.
        /// </summary>
        protected Func<TokenInfo, Cases, AuthenticationHeaderValue> ConvertToAuthenticationHeaderValue
        {
            get => oauth.ConvertToAuthenticationHeaderValue;
            set => oauth.ConvertToAuthenticationHeaderValue = value;
        }

        /// <summary>
        /// Gets the app identifier.
        /// </summary>
        public string AppId => oauth.AppId;

        /// <summary>
        /// Gets or sets the JSON serializer.
        /// </summary>
        public Func<string, Type, object> Serializer
        {
            get => oauth.Serializer;
            set => oauth.Serializer = value;
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
