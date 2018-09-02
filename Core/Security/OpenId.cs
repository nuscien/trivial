using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Trivial.Security
{
    public class OpenIdTokenResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string OpenId { get; set; }

        public TimeSpan ExpiresIn { get; set; }

        public IList<string> Scope { get; } = new List<string>();
    }

    public class AppAccessingKey
    {
        public string Id { get; set; }

        public string Key { get; set; }

        public static bool IsValid(AppAccessingKey appKey)
        {
            return appKey != null && !string.IsNullOrWhiteSpace(appKey.Id) && !string.IsNullOrWhiteSpace(appKey.Key);
        }
    }

    public abstract class OpenIdTokenClient
    {
        private AppAccessingKey _app;

        public OpenIdTokenClient(AppAccessingKey appKey)
        {
            this._app = appKey;
        }

        public string AppId { get { return AppAccessingKey.IsValid(this._app) ? this._app.Id : null; } }

        public OpenIdTokenResponse Token { get; set; }

        public DateTime Visited { get; set; }

        public abstract Uri GetLoginUri(Uri redirectUri, string scope, string state);

        public OpenIdTokenResponse ValidateCode(string code)
        {
            if (!AppAccessingKey.IsValid(this._app) || !string.IsNullOrWhiteSpace(code)) return null;
            var url = this.GetValidationUri(this._app.Key, code);
            if (url == null) return null;
            var webClient = new WebClient();
            var now = DateTime.UtcNow;
            var token = webClient.DownloadString(url);
            var response = this.Deserialize(token);
            Token = response;
            Visited = now;
            return response;
        }

        public OpenIdTokenResponse Refresh()
        {
            if (!AppAccessingKey.IsValid(this._app)) return null;
            var url = GetRefreshingUri();
            if (url == null) return null;
            var webClient = new WebClient();
            var now = DateTime.UtcNow;
            var token = webClient.DownloadString(url);
            var response = this.Deserialize(token);
            this.Token = response;
            this.Visited = now;
            return response;
        }

        protected abstract Uri GetValidationUri(string appKey, string code);

        protected abstract Uri GetRefreshingUri();

        protected abstract OpenIdTokenResponse Deserialize(string token);
    }
}
