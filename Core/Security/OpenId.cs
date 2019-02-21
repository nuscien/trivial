using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Trivial.Net;

namespace Trivial.Security
{
    /// <summary>
    /// Gets the app secret key for accessing api.
    /// </summary>
    public class AppAccessingKey
    {
        /// <summary>
        /// The app id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The secret key.
        /// </summary>
        public SecureString Key { get; set; }

        /// <summary>
        /// Tests if the app accessing key is null or empty.
        /// </summary>
        /// <param name="appKey">The app accessing key instance.</param>
        /// <returns>true if it is null or empty; otherwise, false.</returns>
        public static bool IsNullOrEmpty(AppAccessingKey appKey)
        {
            try
            {
                return appKey == null || string.IsNullOrWhiteSpace(appKey.Id) || appKey.Key == null || appKey.Key.Length == 0;
            }
            catch (ObjectDisposedException)
            {
            }

            return true;
        }
    }

    /// <summary>
    /// The open id token client.
    /// </summary>
    public abstract class TokenResolver
    {
        private AppAccessingKey appInfo;

        /// <summary>
        /// Gets the JSON HTTP web client for resolving access token information instance.
        /// </summary>
        private JsonHttpClient<TokenInfo> webClient;

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        public TokenResolver(AppAccessingKey appKey)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Gets the app id.
        /// </summary>
        public string AppId => AppAccessingKey.IsNullOrEmpty(appInfo) ? appInfo.Id : null;

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
        /// Gets a value indicating whether need dispose request content after receiving response.
        /// </summary>
        protected virtual bool NeedDisposeRequestContent => true;

        /// <summary>
        /// Updates the access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token information instance updated.</returns>
        public async Task<TokenInfo> Update(CancellationToken cancellationToken = default)
        {
            if (AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            if (webClient == null) webClient = new JsonHttpClient<TokenInfo>();
            PrepareWebClient(webClient, appInfo);
            Token = await webClient.Process(cancellationToken);
            LatestResolveDate = DateTime.Now;
            if (NeedDisposeRequestContent && webClient.RequestContent != null)
            {
                webClient.RequestContent.Dispose();
                webClient.RequestContent = null;
            }

            return Token;
        }

        /// <summary>
        /// Prepared web client.
        /// </summary>
        /// <param name="webClient">The JSON HTTP client instance.</param>
        /// <param name="appInfo">The app id and key.</param>
        protected abstract void PrepareWebClient(JsonHttpClient<TokenInfo> webClient, AppAccessingKey appInfo);
    }

    /// <summary>
    /// The open id token client.
    /// </summary>
    public abstract class OpenIdTokenClient
    {
        private AppAccessingKey appInfo;

        /// <summary>
        /// Gets the JSON HTTP web client for resolving access token information instance.
        /// </summary>
        private readonly JsonHttpClient<TokenInfo> webClient = new JsonHttpClient<TokenInfo>();

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        public OpenIdTokenClient(AppAccessingKey appKey)
        {
            appInfo = appKey;
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
        /// Gets a value indicating whether need dispose request content after receiving response.
        /// </summary>
        protected virtual bool NeedDisposeRequestContent => true;

        /// <summary>
        /// Validates the code.
        /// </summary>
        /// <param name="code">The code to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A new open id; or null, if failed.</returns>
        public async Task<TokenInfo> ValidateCode(string code, CancellationToken cancellationToken = default)
        {
            if (!AppAccessingKey.IsNullOrEmpty(appInfo) || !string.IsNullOrWhiteSpace(code)) return null;
            var url = GetValidationUri(appInfo.Key, code);
            if (url == null) return null;
            PrepareWebClient(webClient, appInfo);
            webClient.Uri = url;
            Token = await webClient.Process(cancellationToken);
            LatestVisitDate = DateTime.Now;
            if (NeedDisposeRequestContent && webClient.RequestContent != null)
            {
                webClient.RequestContent.Dispose();
                webClient.RequestContent = null;
            }

            return Token;
        }

        /// <summary>
        /// Refreshes the token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A new open id; or null, if failed.</returns>
        public async Task<TokenInfo> Refresh(CancellationToken cancellationToken = default)
        {
            if (!AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            var url = GetRefreshingUri();
            if (url == null) return null;
            PrepareWebClient(webClient, appInfo);
            webClient.Uri = url;
            Token = await webClient.Process(cancellationToken);
            LatestVisitDate = DateTime.Now;
            if (NeedDisposeRequestContent && webClient.RequestContent != null)
            {
                webClient.RequestContent.Dispose();
                webClient.RequestContent = null;
            }

            return Token;
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
        /// <param name="appKey">The app secret string.</param>
        /// <param name="code">The code to validate.</param>
        /// <returns>A URI for login.</returns>
        protected abstract Uri GetValidationUri(SecureString appKey, string code);

        /// <summary>
        /// Gets the token refresh URI.
        /// </summary>
        /// <returns>A URI for login.</returns>
        protected abstract Uri GetRefreshingUri();

        /// <summary>
        /// Prepared web client.
        /// </summary>
        /// <param name="webClient">The JSON HTTP client instance.</param>
        /// <param name="appInfo">The app id and key.</param>
        protected virtual void PrepareWebClient(JsonHttpClient<TokenInfo> webClient, AppAccessingKey appInfo)
        {
        }
    }
}
