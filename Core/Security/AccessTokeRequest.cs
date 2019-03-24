using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

using Trivial.Collection;
using Trivial.Net;
using Trivial.Web;

namespace Trivial.Security
{
    /// <summary>
    /// The access token resolver request.
    /// </summary>
    [DataContract]
    public class AccessTokenRequest
    {
        /// <summary>
        /// The grant type value.
        /// </summary>
        public const string GrantTypeValue = "client_credentials";

        /// <summary>
        /// Initializes a new instance of the AccessTokenRequest class.
        /// </summary>
        /// <param name="appId">The client id and secret key.</param>
        protected AccessTokenRequest(AppAccessingKey appId)
        {
            GrantType = GrantTypeValue;
            ClientCredentials = appId;
        }

        /// <summary>
        /// Initializes a new instance of the AccessTokenRequest class.
        /// </summary>
        /// <param name="grantType">The grant type.</param>
        /// <param name="appId">The client id and secret key.</param>
        protected AccessTokenRequest(string grantType, AppAccessingKey appId)
        {
            GrantType = grantType;
            ClientCredentials = appId;
        }

        /// <summary>
        /// Initializes a new instance of the AccessTokenRequest class.
        /// </summary>
        /// <param name="grantType">The grant type.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public AccessTokenRequest(string grantType, string id, string secret = null) : this(grantType, new AppAccessingKey(id, secret))
        {
        }

        /// <summary>
        /// Initializes a new instance of the AccessTokenRequest class.
        /// </summary>
        /// <param name="grantType">The grant type.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public AccessTokenRequest(string grantType, string id, SecureString secret) : this(grantType, new AppAccessingKey(id, secret))
        {
        }

        /// <summary>
        /// Gets the client identifier and secret key.
        /// </summary>
        public AppAccessingKey ClientCredentials { get; private set; }

        /// <summary>
        /// Gets the grant type.
        /// </summary>
        [DataMember(Name = "grant_type")]
        public string GrantType { get; }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        [DataMember(Name = "client_id")]
        public string ClientId
        {
            get
            {
                return ClientCredentials?.Id;
            }

            set
            {
                if (ClientCredentials == null)
                {
                    if (string.IsNullOrWhiteSpace(value)) return;
                    ClientCredentials = new AppAccessingKey(value);
                }
                else
                {
                    ClientCredentials.Id = value;
                }
            }
        }

        /// <summary>
        /// Gets the scope to use.
        /// </summary>
        public IList<string> Scope { get; private set; } = new List<string>();

        /// <summary>
        /// Gets the scope string.
        /// </summary>
        [DataMember(Name = "scope")]
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
        /// Gets the query data.
        /// </summary>
        /// <returns>A query data.</returns>
        public QueryData ToQueryData()
        {
            var data = new QueryData();
            var props = GetType().GetProperties();
            foreach (var item in props)
            {
                var attributes = item.GetCustomAttributes<DataMemberAttribute>(true);
                if (attributes == null) continue;
                var attr = attributes.FirstOrDefault();
                if (attr == null) continue;
                var propValue = item.GetValue(this);
                if (propValue == null) continue;
                var propType = propValue.GetType();
                if (propType == typeof(Uri)) data.Add(attr.Name, ((Uri)propValue).OriginalString);
                else if (propType == typeof(DateTime)) data.Add(attr.Name, WebUtility.ParseDate((DateTime)propValue).ToString());
                else if (propType == typeof(DateTimeOffset)) data.Add(attr.Name, WebUtility.ParseDate((DateTimeOffset)propValue).ToString());
                else if (propType == typeof(SecureString)) data.Add(attr.Name, ((SecureString)propValue).ToUnsecureString());
                else data.Add(attr.Name, propValue.ToString());
            }

            var secret = ClientCredentials?.Secret;
            if (secret != null && secret.Length > 0) data.Add("client_secret", secret.ToUnsecureString());
            return data;
        }

        /// <summary>
        /// Gets the JSON format string.
        /// </summary>
        /// <returns>A string in JSON format.</returns>
        public string ToJson()
        {
            var data = new List<string>();
            var props = GetType().GetProperties();
            foreach (var item in props)
            {
                var attributes = item.GetCustomAttributes<DataMemberAttribute>(true);
                if (attributes == null) continue;
                var attr = attributes.FirstOrDefault();
                if (attr == null) continue;
                var propValue = item.GetValue(this);
                if (propValue == null) continue;
                var propType = propValue.GetType();
                if (propType == typeof(Uri)) data.Add($"\"{attr.Name}\": \"{((Uri)propValue).OriginalString}\"");
                else if (propType == typeof(DateTime)) data.Add($"\"{attr.Name}\": {WebUtility.ParseDate((DateTime)propValue).ToString()}");
                else if (propType == typeof(DateTimeOffset)) data.Add($"\"{attr.Name}\": {WebUtility.ParseDate((DateTimeOffset)propValue).ToString()}");
                else if (propType == typeof(int) || propType == typeof(long) || propType == typeof(uint) || propType == typeof(ulong) || propType == typeof(float) || propType == typeof(double) || propType == typeof(short) || propType == typeof(bool)) data.Add($"\"{attr.Name}\": {propValue.ToString()}");
                else if (propType == typeof(SecureString)) data.Add($"\"{attr.Name}\": \"{((SecureString)propValue).ToUnsecureString()}\"");
                else data.Add($"\"{attr.Name}\": \"{propValue.ToString()}\"");
            }

            var secret = ClientCredentials?.Secret;
            if (secret != null && secret.Length > 0) data.Add($"\"client_secret\": \"{secret.ToUnsecureString()}\"");
            return "{ " + string.Join(", ", data) + " }";
        }
    }

    /// <summary>
    /// The access token request with authorization code grant type.
    /// </summary>
    [DataContract]
    public class CodeAccessTokenRequest : AccessTokenRequest
    {
        /// <summary>
        /// The grant type value.
        /// </summary>
        public new const string GrantTypeValue = "authorization_code";

        /// <summary>
        /// Initializes a new instance of the CodeAccessTokenRequest class.
        /// </summary>
        /// <param name="appId">The client id and secret key.</param>
        protected CodeAccessTokenRequest(AppAccessingKey appId) : base(GrantTypeValue, appId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeAccessTokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public CodeAccessTokenRequest(string id, string secret = null) : base(GrantTypeValue, id, secret)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeAccessTokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public CodeAccessTokenRequest(string id, SecureString secret) : base(GrantTypeValue, id, secret)
        {
        }

        /// <summary>
        /// Gets or sets the authorization code.
        /// </summary>
        [DataMember(Name = "code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        [DataMember(Name = "redirect_uri")]
        public Uri RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the code verifier.
        /// </summary>
        [DataMember(Name = "code_verifier")]
        public string CodeVerifier { get; set; }

        /// <summary>
        /// Gets the login URI.
        /// </summary>
        /// <param name="uri">The base URI.</param>
        /// <param name="responseType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Uri GetLoginUri(Uri uri, string responseType, string state)
        {
            var data = new QueryData
            {
                { "response_type", responseType },
                { "client_id", ClientId },
                { "redirect_uri", RedirectUri.OriginalString },
                { "scope", ScopeString },
                { "state", state }
            };
            return new Uri(data.ToString(uri.OriginalString));
        }
    }

    /// <summary>
    /// The access token request with refresh token grant type.
    /// </summary>
    [DataContract]
    public class RefreshAccessTokenRequest : AccessTokenRequest
    {
        /// <summary>
        /// The grant type value.
        /// </summary>
        public new const string GrantTypeValue = "refresh_token";

        /// <summary>
        /// Initializes a new instance of the AccessTokenRequest class.
        /// </summary>
        /// <param name="appId">The client id and secret key.</param>
        protected RefreshAccessTokenRequest(AppAccessingKey appId) : base(GrantTypeValue, appId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AccessTokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public RefreshAccessTokenRequest(string id, string secret = null) : base(GrantTypeValue, id, secret)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AccessTokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public RefreshAccessTokenRequest(string id, SecureString secret) : base(GrantTypeValue, id, secret)
        {
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [DataMember(Name = "username")]
        public string UserName { get; set; }
    }

    /// <summary>
    /// The access token request with password grant type.
    /// </summary>
    [DataContract]
    public class PasswordAccessTokenRequest : AccessTokenRequest
    {
        /// <summary>
        /// The grant type value.
        /// </summary>
        public new const string GrantTypeValue = "password";

        /// <summary>
        /// Initializes a new instance of the AccessTokenRequest class.
        /// </summary>
        /// <param name="appId">The client id and secret key.</param>
        protected PasswordAccessTokenRequest(AppAccessingKey appId) : base(GrantTypeValue, appId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AccessTokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public PasswordAccessTokenRequest(string id, string secret = null) : base(GrantTypeValue, id, secret)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AccessTokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public PasswordAccessTokenRequest(string id, SecureString secret) : base(GrantTypeValue, id, secret)
        {
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [DataMember(Name = "username")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DataMember(Name = "password")]
        public SecureString Password { get; set; }
    }
}
