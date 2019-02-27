using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.Net;

namespace Trivial.Security
{
    /// <summary>
    /// The token resolver.
    /// </summary>
    public abstract class TokenResolver
    {
        private AppAccessingKey appInfo;
        private Task<TokenInfo> task;

        /// <summary>
        /// The JSON HTTP web client for resolving access token information instance.
        /// </summary>
        private JsonHttpClient<TokenInfo> webClient;

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(AppAccessingKey appKey, TokenInfo tokenCached = null)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(string appId, string secretKey, TokenInfo tokenCached = null)
        {
            appInfo = new AppAccessingKey(appId, secretKey);
        }

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(string appId, SecureString secretKey, TokenInfo tokenCached = null)
        {
            appInfo = new AppAccessingKey(appId, secretKey);
        }

        /// <summary>
        /// Gets the app id.
        /// </summary>
        public string AppId => !AppAccessingKey.IsNullOrEmpty(appInfo) ? appInfo.Id : null;

        /// <summary>
        /// Gets the open id info cached.
        /// </summary>
        public TokenInfo Token { get; protected set; }

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
        /// Adds or removes the event raised after token changed.
        /// </summary>
        public event ChangeEventHandler<TokenInfo> TokenChanged;

        /// <summary>
        /// Gets a value indicating whether need dispose request content after receiving response.
        /// </summary>
        protected virtual bool NeedDisposeRequestContent => true;

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
        /// Gets the token resolve URI.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <returns>A URI for login.</returns>
        protected abstract Uri GetResolveUri(SecureString appSecretKey);

        /// <summary>
        /// Creates a web client.
        /// </summary>
        /// <param name="appSecretKey">The app secret key.</param>
        protected virtual JsonHttpClient<TokenInfo> CreateWebClient(SecureString appSecretKey)
        {
            if (webClient == null) webClient = new JsonHttpClient<TokenInfo>();
            return webClient;
        }

        /// <summary>
        /// Updates the access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token information instance updated.</returns>
        private async Task<TokenInfo> UpdateUnlockedAsync(CancellationToken cancellationToken = default)
        {
            if (AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            var wc = CreateWebClient(appInfo.Secret);
            wc.Uri = GetResolveUri(appInfo.Secret);
            var oldToken = Token;
            Token = await wc.Process(cancellationToken);
            LatestResolveDate = DateTime.Now;
            if (NeedDisposeRequestContent && wc.RequestContent != null)
            {
                wc.RequestContent.Dispose();
                wc.RequestContent = null;
            }

            TokenChanged?.Invoke(this, new ChangeEventArgs<TokenInfo>(oldToken, Token, nameof(Token), true));
            return Token;
        }
    }

    /// <summary>
    /// The open id token client.
    /// </summary>
    public abstract class OpenIdTokenClient
    {
        private readonly AppAccessingKey appInfo;
        private Task<TokenInfo> task;

        /// <summary>
        /// The JSON HTTP web client for resolving access token information instance.
        /// </summary>
        private JsonHttpClient<TokenInfo> webClient;

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(AppAccessingKey appKey, TokenInfo tokenCached = null)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(string appId, string secretKey, TokenInfo tokenCached = null)
        {
            appInfo = new AppAccessingKey(appId, secretKey);
        }

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(string appId, SecureString secretKey, TokenInfo tokenCached = null)
        {
            appInfo = new AppAccessingKey(appId, secretKey);
        }

        /// <summary>
        /// Gets the app id.
        /// </summary>
        public string AppId => AppAccessingKey.IsNullOrEmpty(appInfo) ? appInfo.Id : null;

        /// <summary>
        /// Gets the open id info.
        /// </summary>
        public TokenInfo Token { get; protected set; }

        /// <summary>
        /// Gets the latest visited date.
        /// </summary>
        public DateTime LatestVisitDate { get; private set; }

        /// <summary>
        /// Adds or removes the event raised after token changed.
        /// </summary>
        public event ChangeEventHandler<TokenInfo> TokenChanged;

        /// <summary>
        /// Gets a value indicating whether need dispose request content after receiving response.
        /// </summary>
        protected virtual bool NeedDisposeRequestContent => true;

        /// <summary>
        /// Validates the code.
        /// </summary>
        /// <param name="code">The code to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A new open id; or null, if failed.</returns>
        public async Task<TokenInfo> ValidateCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(code)) return null;
            return await ProcessAsync(GetValidationUri(appInfo.Secret, code), cancellationToken);
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

            task = t = ProcessAsync(GetRefreshingUri(appInfo.Secret), cancellationToken);
            var result = await t;
            task = null;
            return result;
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
        /// Gets the validation URI.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <param name="code">The code to validate.</param>
        /// <returns>A URI for login.</returns>
        protected abstract Uri GetValidationUri(SecureString appSecretKey, string code);

        /// <summary>
        /// Gets the token refresh URI.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <returns>A URI for login.</returns>
        protected abstract Uri GetRefreshingUri(SecureString appSecretKey);

        /// <summary>
        /// Creates a web client.
        /// </summary>
        /// <param name="appSecretKey">The app secret key.</param>
        protected virtual JsonHttpClient<TokenInfo> CreateWebClient(SecureString appSecretKey)
        {
            if (webClient == null) webClient = new JsonHttpClient<TokenInfo>();
            return webClient;
        }

        private async Task<TokenInfo> ProcessAsync(Uri uri, CancellationToken cancellationToken)
        {
            if (!AppAccessingKey.IsNullOrEmpty(appInfo) || uri == null) return null;
            var wc = CreateWebClient(appInfo.Secret);
            wc.Uri = uri;
            var oldToken = Token;
            Token = await wc.Process(cancellationToken);
            LatestVisitDate = DateTime.Now;
            if (NeedDisposeRequestContent && wc.RequestContent != null)
            {
                wc.RequestContent.Dispose();
                wc.RequestContent = null;
            }

            TokenChanged?.Invoke(this, new ChangeEventArgs<TokenInfo>(oldToken, Token, nameof(Token), true));
            return Token;
        }
    }
}
