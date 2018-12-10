using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Web;

namespace Trivial.Security
{
    /// <summary>
    /// The open id token response.
    /// </summary>
    public class OpenIdTokenResponse
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public SecureString AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        public SecureString RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the resource identifier.
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// Gets or sets the time span of expiration.
        /// </summary>
        public TimeSpan ExpiresIn { get; set; }

        /// <summary>
        /// Gets the permission scope.
        /// </summary>
        public IList<string> Scope { get; } = new List<string>();

        /// <summary>
        /// Tests if the access token is set.
        /// </summary>
        /// <returns>true if it is null or empty; otherwise, false.</returns>
        public bool IsEmpty()
        {
            try
            {
                return AccessToken == null || AccessToken.Length == 0;
            }
            catch (ObjectDisposedException)
            {
            }

            return true;
        }
    }

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
    public abstract class OpenIdTokenClient
    {
        private AppAccessingKey appInfo;

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
        public OpenIdTokenResponse Token { get; protected set; }

        /// <summary>
        /// Gets the latest visited date.
        /// </summary>
        public DateTime LatestVisitDate { get; private set; }

        /// <summary>
        /// Gets the login URI.
        /// </summary>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="scope">The permission scope to request.</param>
        /// <param name="state">A state code.</param>
        /// <returns>A URI for login.</returns>
        public abstract Uri GetLoginUri(Uri redirectUri, string scope, string state);

        /// <summary>
        /// Validates the code.
        /// </summary>
        /// <param name="code">The code to validate.</param>
        /// <returns>A new open id; or null, if failed.</returns>
        public OpenIdTokenResponse ValidateCode(string code)
        {
            if (!AppAccessingKey.IsNullOrEmpty(appInfo) || !string.IsNullOrWhiteSpace(code)) return null;
            var url = GetValidationUri(appInfo.Key, code);
            if (url == null) return null;
            var webClient = CreateWebClient();
            var now = DateTime.UtcNow;
            var token = webClient.DownloadString(url);
            var response = Deserialize(token);
            Token = response;
            LatestVisitDate = now;
            return response;
        }

        /// <summary>
        /// Refreshes the token.
        /// </summary>
        /// <returns>A new open id; or null, if failed.</returns>
        public OpenIdTokenResponse Refresh()
        {
            if (!AppAccessingKey.IsNullOrEmpty(this.appInfo)) return null;
            var url = GetRefreshingUri();
            if (url == null) return null;
            var webClient = CreateWebClient();
            var now = DateTime.UtcNow;
            var token = webClient.DownloadString(url);
            var response = Deserialize(token);
            Token = response;
            LatestVisitDate = now;
            return response;
        }

        /// <summary>
        /// Creates a web client.
        /// </summary>
        /// <returns>A web client to use access network resource.</returns>
        protected virtual WebClient CreateWebClient()
        {
            return new WebClient
            {
                UseDefaultCredentials = true
            };
        }

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
        /// Deserializes the text to the open id response instance.
        /// </summary>
        /// <param name="response">The response string to deserialize.</param>
        /// <returns>The open id response instance.</returns>
        protected abstract OpenIdTokenResponse Deserialize(string response);
    }
}
