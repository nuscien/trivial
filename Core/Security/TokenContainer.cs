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
    /// The token container.
    /// </summary>
    public class TokenContainer
    {
        private TokenInfo token;

        /// <summary>
        /// Initializes a new instance of the TokenContainer class.
        /// </summary>
        public TokenContainer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenContainer class.
        /// </summary>
        /// <param name="token">The token information instance.</param>
        public TokenContainer(TokenInfo token)
        {
            Token = token;
        }

        /// <summary>
        /// Gets the token information instance saved in this container.
        /// </summary>
        public TokenInfo Token
        {
            get
            {
                return token;
            }

            protected set
            {
                var oldValue = token;
                token = value;
                TokenChanged?.Invoke(this, new ChangeEventArgs<TokenInfo>(oldValue, value, nameof(Token), true));
            }
        }

        /// <summary>
        /// Gets the access token saved in this container.
        /// </summary>
        public string AccessToken => Token?.AccessToken;

        /// <summary>
        /// Gets a value indicating whether the access token is null, empty or consists only of white-space characters.
        /// </summary>
        public bool IsTokenNullOrEmpty => Token?.IsEmpty ?? false;

        /// <summary>
        /// Adds or removes the event raised after token changed.
        /// </summary>
        public event ChangeEventHandler<TokenInfo> TokenChanged;

        /// <summary>
        /// Returns a System.String that represents the current token container instance.
        /// </summary>
        /// <returns>A System.String that represents the current token container instance.</returns>
        public override string ToString()
        {
            return Token?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.
        /// </summary>
        /// <param name="schemeCase">The scheme case.</param>
        /// <param name="parameterCase">The parameter case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.</returns>
        public virtual AuthenticationHeaderValue ToAuthenticationHeaderValue(Cases schemeCase = Cases.Original, Cases parameterCase = Cases.Original)
        {
            return Token?.ToAuthenticationHeaderValue(schemeCase, parameterCase);
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.
        /// </summary>
        /// <param name="scheme">The scheme to use for authorization.</param>
        /// <param name="parameterCase">The parameter case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.</returns>
        public virtual AuthenticationHeaderValue ToAuthenticationHeaderValue(string scheme, Cases parameterCase = Cases.Original)
        {
            return Token?.ToAuthenticationHeaderValue(scheme, parameterCase);
        }
    }

    /// <summary>
    /// The client credentials token container.
    /// </summary>
    public class ClientTokenContainer : TokenContainer
    {
        private readonly AppAccessingKey appInfo;
        private Task<TokenInfo> task;

        /// <summary>
        /// Initializes a new instance of the ClientTokenContainer class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public ClientTokenContainer(TokenInfo tokenCached) : base(tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ClientTokenContainer class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public ClientTokenContainer(AppAccessingKey appKey, TokenInfo tokenCached = null) : this(tokenCached)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Initializes a new instance of the ClientTokenContainer class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public ClientTokenContainer(string appId, string secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ClientTokenContainer class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public ClientTokenContainer(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Gets the app id.
        /// </summary>
        public string AppId => !AppAccessingKey.IsNullOrEmpty(appInfo) ? appInfo.Id : null;

        /// <summary>
        /// Gets or sets a value indicating whether it requires an app identifier.
        /// </summary>
        public bool IsAppIdRequired { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a token cached.
        /// </summary>
        public bool HasCache => !string.IsNullOrWhiteSpace(Token?.AccessToken);

        /// <summary>
        /// Gets authorization value.
        /// </summary>
        public string Authorization => Token?.ToString();

        /// <summary>
        /// Gets the latest resolved date.
        /// </summary>
        public DateTime LatestResolveDate { get; private set; }

        /// <summary>
        /// Gets or sets the token resolve URL.
        /// </summary>
        public Uri TokenResolveUri { get; set; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public IList<string> Scope { get; } = new List<string>();

        /// <summary>
        /// Gets the JSON HTTP web client for resolving access token information instance.
        /// </summary>
        protected virtual JsonHttpClient<TokenInfo> WebClient { get; } = new JsonHttpClient<TokenInfo>();

        /// <summary>
        /// Updates the access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token information instance updated.</returns>
        public async Task<TokenInfo> UpdateAsync(CancellationToken cancellationToken = default)
        {
            var t = task;
            if (t != null && t.Status == TaskStatus.Running)
            {
                try
                {
                    return await Task.Run(async () =>
                    {
                        return await t;
                    }, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested) throw;
                }
                catch (ObjectDisposedException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }

            task = t = UpdateUnlockedAsync(cancellationToken);
            var result = await t;
            task = null;
            return result;
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token information instance.</returns>
        public Task<TokenInfo> GetAsync(CancellationToken cancellationToken = default)
        {
            return HasCache ? Task.Run(() => Token) : UpdateAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the client token request.
        /// </summary>
        /// <returns>The client token request instance.</returns>
        public virtual TokenRequest<ClientTokenRequestBody> ToClientTokenRequest()
        {
            return new TokenRequest<ClientTokenRequestBody>(new ClientTokenRequestBody(), appInfo, Scope);
        }

        /// <summary>
        /// Gets the token resolve request message.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <returns>A URI for login.</returns>
        protected virtual HttpRequestMessage CreateResolveMessage(SecureString appSecretKey)
        {
            var p = ToClientTokenRequest().ToQueryData().ToString();
            return new HttpRequestMessage(HttpMethod.Post, TokenResolveUri)
            {
                Content = new StringContent(p, Encoding.UTF8, Web.WebUtility.FormUrlMIME)
            };
        }

        /// <summary>
        /// Updates the access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token information instance updated.</returns>
        private async Task<TokenInfo> UpdateUnlockedAsync(CancellationToken cancellationToken = default)
        {
            if (IsAppIdRequired && AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            var wc = WebClient;
            using (var request = CreateResolveMessage(appInfo?.Secret))
            {
                if (request == null) return null;
                Token = await wc.SendAsync(request, cancellationToken);
                LatestResolveDate = DateTime.Now;
            }

            return Token;
        }
    }

    /// <summary>
    /// The code token container.
    /// </summary>
    public class CodeTokenContainer : TokenContainer
    {
        private readonly AppAccessingKey appInfo;
        private Task<TokenInfo> task;

        /// <summary>
        /// Initializes a new instance of the AuthorizationCodeTokenContainer class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public CodeTokenContainer(TokenInfo tokenCached) : base(tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AuthorizationCodeTokenContainer class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public CodeTokenContainer(AppAccessingKey appKey, TokenInfo tokenCached = null) : this(tokenCached)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Initializes a new instance of the AuthorizationCodeTokenContainer class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public CodeTokenContainer(string appId, string secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AuthorizationCodeTokenContainer class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public CodeTokenContainer(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Gets the app id.
        /// </summary>
        public string AppId => !AppAccessingKey.IsNullOrEmpty(appInfo) ? appInfo.Id : null;

        /// <summary>
        /// Gets or sets a value indicating whether it requires an app identifier.
        /// </summary>
        public bool IsAppIdRequired { get; set; }

        /// <summary>
        /// Gets the latest visited date.
        /// </summary>
        public DateTime LatestVisitDate { get; private set; }

        /// <summary>
        /// Gets the latest refreshed date.
        /// </summary>
        public DateTime LatestRefreshDate { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether need refresh token before valdate the code when expired.
        /// </summary>
        public bool IsAutoRefresh { get; set; }

        /// <summary>
        /// Gets or sets the token resolve URL.
        /// </summary>
        public Uri TokenResolveUri { get; set; }

        /// <summary>
        /// Gets or sets the login base URI.
        /// </summary>
        public Uri LoginBaseUri { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        public Uri RedirectUri { get; set; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public IList<string> Scope { get; } = new List<string>();

        /// <summary>
        /// Gets the JSON HTTP web client for resolving access token information instance.
        /// </summary>
        protected virtual JsonHttpClient<TokenInfo> WebClient { get; } = new JsonHttpClient<TokenInfo>();

        /// <summary>
        /// Validates the code.
        /// </summary>
        /// <param name="code">The code to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A new open id; or null, if failed.</returns>
        public async Task<TokenInfo> ValidateCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(code)) return null;
            if (IsAppIdRequired && AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            using (var request = CreateValidationMessage(appInfo?.Secret, code))
            {
                if (request == null) return null;
                try
                {
                    return await ProcessAsync(request, cancellationToken);
                }
                catch (FailedHttpException ex)
                {
                    if (!IsAutoRefresh || ex.Response == null || ex.Response.StatusCode != HttpStatusCode.Unauthorized) throw;
                    var token = await RefreshAsync(cancellationToken);
                    if (token == null || token.IsEmpty) throw;
                    using (var request2 = CreateValidationMessage(appInfo?.Secret, code))
                    {
                        if (request2 == null) return null;
                        return await ProcessAsync(request2, cancellationToken);
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A new open id; or null, if failed.</returns>
        public async Task<TokenInfo> RefreshAsync(CancellationToken cancellationToken = default)
        {
            var t = task;
            if (t != null && t.Status == TaskStatus.Running)
            {
                try
                {
                    return await Task.Run(async () =>
                    {
                        return await t;
                    }, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested) throw;
                }
                catch (ObjectDisposedException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }

            if (IsAppIdRequired && AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            using (var request = CreateRefreshingMessage(appInfo?.Secret))
            {
                if (request == null) return null;
                task = t = ProcessAsync(request, cancellationToken);
                var result = await t;
                LatestRefreshDate = DateTime.Now;
                task = null;
                return result;
            }
        }

        /// <summary>
        /// Gets the code token request.
        /// </summary>
        /// <param name="code">The authorization code.</param>
        /// <returns>The code token request instance.</returns>
        public virtual CodeTokenRequest ToCodeTokenRequest(string code)
        {
            return new CodeTokenRequest(new CodeTokenRequestBody
            {
                Code = code,
                RedirectUri = RedirectUri
            }, appInfo, Scope);
        }

        /// <summary>
        /// Gets the refresh token request.
        /// </summary>
        /// <returns>The refresh token request instance.</returns>
        public virtual TokenRequest<RefreshTokenRequestBody> ToRefreshTokenRequest()
        {
            if (Token == null || string.IsNullOrWhiteSpace(Token.RefreshToken)) return null;
            return new TokenRequest<RefreshTokenRequestBody>(new RefreshTokenRequestBody
            {
                RefreshToken = Token.RefreshToken
            }, appInfo, Scope);
        }

        /// <summary>
        /// Gets the login URI.
        /// </summary>
        /// <param name="state">A state code.</param>
        /// <param name="responseType">The response type.</param>
        /// <returns>A URI for login.</returns>
        public virtual Uri GetLoginUri(string state, string responseType = null)
        {
            return ToCodeTokenRequest(null).GetLoginUri(LoginBaseUri, responseType ?? "code", state);
        }

        /// <summary>
        /// Creates the validation request message.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <param name="code">The code to validate.</param>
        /// <returns>A URI for login.</returns>
        protected virtual HttpRequestMessage CreateValidationMessage(SecureString appSecretKey, string code)
        {
            var p = ToCodeTokenRequest(code).ToQueryData().ToString();
            return new HttpRequestMessage(HttpMethod.Post, TokenResolveUri)
            {
                Content = new StringContent(p, Encoding.UTF8, Web.WebUtility.FormUrlMIME)
            };
        }

        /// <summary>
        /// Creates the token refresh request message.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <returns>A URI for login.</returns>
        protected virtual HttpRequestMessage CreateRefreshingMessage(SecureString appSecretKey)
        {
            var p = ToRefreshTokenRequest()?.ToQueryData()?.ToString();
            if (string.IsNullOrWhiteSpace(p)) return null;
            return new HttpRequestMessage(HttpMethod.Post, TokenResolveUri)
            {
                Content = new StringContent(p, Encoding.UTF8, Web.WebUtility.FormUrlMIME)
            };
        }

        private async Task<TokenInfo> ProcessAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null) return null;
            if (IsAppIdRequired && AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            var wc = WebClient;
            Token = await wc.SendAsync(request, cancellationToken);
            LatestVisitDate = DateTime.Now;
            return Token;
        }
    }

    /// <summary>
    /// The OAuth client.
    /// </summary>
    public class OAuthClient : TokenContainer
    {
        /// <summary>
        /// Initializes a new instance of the OAuthClient class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OAuthClient(TokenInfo tokenCached) : base(tokenCached)
        {
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
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
        public Task<TokenInfo> ResolveTokenAsync<T>(Uri requestUri, TokenRequest<T> content, CancellationToken cancellationToken = default) where T : TokenRequestBody
        {
            var httpClient = new JsonHttpClient<TokenInfo>
            {
                SerializeEvenIfFailed = true
            };
            if (Serializer != null) httpClient.Serializer = json => (TokenInfo)Serializer(json, typeof(TokenInfo));
            httpClient.Sending += (sender, ev) =>
            {
                TokenResolving?.Invoke(sender, ev);
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
            return httpClient.SendAsync(HttpMethod.Post, requestUri, content.ToQueryData(), cancellationToken);
        }
    }
}
