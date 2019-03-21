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
        public TokenInfo Token { get; protected set; }

        /// <summary>
        /// Gets the access token saved in this container.
        /// </summary>
        public string AccessToken => Token?.AccessToken;

        /// <summary>
        /// Gets a value indicating whether the access token is null, empty or consists only of white-space characters.
        /// </summary>
        public bool IsTokenNullOrEmpty => Token?.IsEmpty ?? false;

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
    /// The token resolver.
    /// </summary>
    public abstract class TokenResolver : TokenContainer
    {
        private readonly AppAccessingKey appInfo;
        private Task<TokenInfo> task;

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(TokenInfo tokenCached) : base(tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(AppAccessingKey appKey, TokenInfo tokenCached = null) : this(tokenCached)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(string appId, string secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
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
        /// Gets the JSON HTTP web client for resolving access token information instance.
        /// </summary>
        protected virtual JsonHttpClient<TokenInfo> WebClient { get; } = new JsonHttpClient<TokenInfo>();

        /// <summary>
        /// Adds or removes the event raised after token changed.
        /// </summary>
        public event ChangeEventHandler<TokenInfo> TokenChanged;

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
        /// Gets the token resolve request message.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <returns>A URI for login.</returns>
        protected abstract HttpRequestMessage CreateResolveMessage(SecureString appSecretKey);

        /// <summary>
        /// Updates the access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token information instance updated.</returns>
        private async Task<TokenInfo> UpdateUnlockedAsync(CancellationToken cancellationToken = default)
        {
            if (IsAppIdRequired && AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            var wc = WebClient;
            var oldToken = Token;
            using (var request = CreateResolveMessage(appInfo?.Secret))
            {
                if (request == null) return null;
                Token = await wc.SendAsync(request, cancellationToken);
                LatestResolveDate = DateTime.Now;
            }

            TokenChanged?.Invoke(this, new ChangeEventArgs<TokenInfo>(oldToken, Token, nameof(Token), true));
            return Token;
        }
    }

    /// <summary>
    /// The open id token client.
    /// </summary>
    public abstract class OpenIdTokenClient : TokenContainer
    {
        private readonly AppAccessingKey appInfo;
        private Task<TokenInfo> task;

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(TokenInfo tokenCached) : base(tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(AppAccessingKey appKey, TokenInfo tokenCached = null) : this(tokenCached)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(string appId, string secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
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
        /// Gets the JSON HTTP web client for resolving access token information instance.
        /// </summary>
        protected virtual JsonHttpClient<TokenInfo> WebClient { get; } = new JsonHttpClient<TokenInfo>();

        /// <summary>
        /// Adds or removes the event raised after token changed.
        /// </summary>
        public event ChangeEventHandler<TokenInfo> TokenChanged;

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
        /// Gets the login URI.
        /// </summary>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="scope">The permission scope to request.</param>
        /// <param name="state">A state code.</param>
        /// <returns>A URI for login.</returns>
        public abstract Uri GetLoginUri(Uri redirectUri, string scope, string state);

        /// <summary>
        /// Creates the validation request message.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <param name="code">The code to validate.</param>
        /// <returns>A URI for login.</returns>
        protected abstract HttpRequestMessage CreateValidationMessage(SecureString appSecretKey, string code);

        /// <summary>
        /// Creates the token refresh request message.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <returns>A URI for login.</returns>
        protected abstract HttpRequestMessage CreateRefreshingMessage(SecureString appSecretKey);

        private async Task<TokenInfo> ProcessAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null) return null;
            if (IsAppIdRequired && AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            var wc = WebClient;
            var oldToken = Token;
            Token = await wc.SendAsync(request, cancellationToken);
            LatestVisitDate = DateTime.Now;
            TokenChanged?.Invoke(this, new ChangeEventArgs<TokenInfo>(oldToken, Token, nameof(Token), true));
            return Token;
        }
    }
}
