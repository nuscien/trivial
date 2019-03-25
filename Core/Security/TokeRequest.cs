using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

using Trivial.Collection;
using Trivial.Net;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Security
{
    /// <summary>
    /// The access token resolver request.
    /// </summary>
    [DataContract]
    public abstract class TokenRequest
    {
        /// <summary>
        /// The  property name.
        /// </summary>
        public const string GrantTypeProperty = "grant_type";

        /// <summary>
        /// The  property name.
        /// </summary>
        public const string ClientIdProperty = "client_id";

        /// <summary>
        /// The  property name.
        /// </summary>
        public const string ClientSecretProperty = "client_secret";

        /// <summary>
        /// The  property name.
        /// </summary>
        public const string ScopeProperty = "scope";

        /// <summary>
        /// The  property name.
        /// </summary>
        public const string StateProperty = "state";

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="grantType">The grant type.</param>
        /// <param name="appId">The client id and secret key.</param>
        protected TokenRequest(string grantType, AppAccessingKey appId)
        {
            GrantType = grantType;
            ClientCredentials = appId;
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="grantType">The grant type.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        protected TokenRequest(string grantType, string id, string secret = null) : this(grantType, new AppAccessingKey(id, secret))
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="grantType">The grant type.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        protected TokenRequest(string grantType, string id, SecureString secret) : this(grantType, new AppAccessingKey(id, secret))
        {
        }

        /// <summary>
        /// Gets the client identifier and secret key.
        /// </summary>
        public AppAccessingKey ClientCredentials { get; private set; }

        /// <summary>
        /// Gets the grant type.
        /// </summary>
        [DataMember(Name = GrantTypeProperty)]
        public string GrantType { get; }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        [DataMember(Name = ClientIdProperty)]
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
        [DataMember(Name = ScopeProperty)]
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
        public virtual QueryData ToQueryData()
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
            if (secret != null && secret.Length > 0) data.Add(ClientSecretProperty, secret.ToUnsecureString());
            return data;
        }

        /// <summary>
        /// Gets the JSON format string.
        /// </summary>
        /// <returns>A string in JSON format.</returns>
        public virtual string ToJson()
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
            if (secret != null && secret.Length > 0) data.Add($"\"{ClientSecretProperty}\": \"{secret.ToUnsecureString()}\"");
            return "{ " + string.Join(", ", data) + " }";
        }
    }

    /// <summary>
    /// The access token resolver request with client credentials grant type.
    /// </summary>
    [DataContract]
    public class ClientTokenRequest : TokenRequest
    {
        /// <summary>
        /// The grant type value of client credentials.
        /// </summary>
        public const string ClientCredentialsGrantType = "client_credentials";

        /// <summary>
        /// Initializes a new instance of the ClientCredentialsTokenRequest class.
        /// </summary>
        /// <param name="appId">The client id and secret key.</param>
        public ClientTokenRequest(AppAccessingKey appId) : base(ClientCredentialsGrantType, appId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ClientCredentialsTokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public ClientTokenRequest(string id, string secret = null) : base(ClientCredentialsGrantType, id, secret)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ClientCredentialsTokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public ClientTokenRequest(string id, SecureString secret) : base(ClientCredentialsGrantType, id, secret)
        {
        }

        /// <summary>
        /// Parses a string to client credentials access token request.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The object parsed returned.</returns>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public static ClientTokenRequest Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s), "s should not be null, empty or consists only of white-space characters.");
            var m = TokenRequestModel.Parse(s);
            if (m == null) throw new NotSupportedException("s cannot be parsed.");
            var grantTypeExpect = ClientCredentialsGrantType;
            if (m.GrantType != grantTypeExpect) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {grantTypeExpect}.");
            return new ClientTokenRequest(new AppAccessingKey(m.ClientId, m.ClientSecret))
            {
                ScopeString = m.Scope
            };
        }
    }

    /// <summary>
    /// The access token request with authorization code grant type.
    /// </summary>
    [DataContract]
    public class CodeTokenRequest : TokenRequest
    {
        /// <summary>
        /// The grant type value of authorization code.
        /// </summary>
        public const string AuthorizationCodeGrantType = "authorization_code";

        /// <summary>
        /// The code property name.
        /// </summary>
        public const string CodeProperty = "code";

        /// <summary>
        /// The redirect URI property name.
        /// </summary>
        public const string RedirectUriProperty = "redirect_uri";

        /// <summary>
        /// The code verifier property name.
        /// </summary>
        public const string CodeVerifierProperty = "code_verifier";

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="appId">The client id and secret key.</param>
        public CodeTokenRequest(AppAccessingKey appId) : base(AuthorizationCodeGrantType, appId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public CodeTokenRequest(string id, string secret = null) : base(AuthorizationCodeGrantType, id, secret)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public CodeTokenRequest(string id, SecureString secret) : base(AuthorizationCodeGrantType, id, secret)
        {
        }

        /// <summary>
        /// Gets or sets the authorization code.
        /// </summary>
        [DataMember(Name = CodeProperty)]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        [DataMember(Name = RedirectUriProperty)]
        public Uri RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the code verifier.
        /// </summary>
        [DataMember(Name = CodeVerifierProperty)]
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

        /// <summary>
        /// Parses a string to code access token request.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The object parsed returned.</returns>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public static CodeTokenRequest Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s), "s should not be null, empty or consists only of white-space characters.");
            var m = TokenRequestModel.Parse(s);
            if (m == null) throw new NotSupportedException("s cannot be parsed.");
            var grantTypeExpect = AuthorizationCodeGrantType;
            if (m.GrantType != grantTypeExpect) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {grantTypeExpect}.");
            return new CodeTokenRequest(new AppAccessingKey(m.ClientId, m.ClientSecret))
            {
                ScopeString = m.Scope,
                Code = m.Code,
                CodeVerifier = m.CodeVerifier,
                RedirectUri = !string.IsNullOrWhiteSpace(m.RedirectUri) ? new Uri(m.RedirectUri) : null
            };
        }
    }

    /// <summary>
    /// The access token request with refresh token grant type.
    /// </summary>
    [DataContract]
    public class RefreshTokenRequest : TokenRequest
    {
        /// <summary>
        /// The grant type value of refresh token.
        /// </summary>
        public const string RefreshTokenGrantType = "refresh_token";

        /// <summary>
        /// The refresh token property name.
        /// </summary>
        public const string RefreshTokenProperty = "refresh_token";

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="appId">The client id and secret key.</param>
        public RefreshTokenRequest(AppAccessingKey appId) : base(RefreshTokenGrantType, appId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public RefreshTokenRequest(string id, string secret = null) : base(RefreshTokenGrantType, id, secret)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public RefreshTokenRequest(string id, SecureString secret) : base(RefreshTokenGrantType, id, secret)
        {
        }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        [DataMember(Name = RefreshTokenProperty)]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Parses a string to code access token request.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The object parsed returned.</returns>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public static RefreshTokenRequest Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s), "s should not be null, empty or consists only of white-space characters.");
            var m = TokenRequestModel.Parse(s);
            if (m == null) throw new NotSupportedException("s cannot be parsed.");
            var grantTypeExpect = RefreshTokenGrantType;
            if (m.GrantType != grantTypeExpect) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {grantTypeExpect}.");
            return new RefreshTokenRequest(new AppAccessingKey(m.ClientId, m.ClientSecret))
            {
                ScopeString = m.Scope,
                RefreshToken = m.RefreshToken
            };
        }
    }

    /// <summary>
    /// The access token request with password grant type.
    /// </summary>
    [DataContract]
    public class PasswordTokenRequest : TokenRequest
    {
        /// <summary>
        /// The grant type value of password.
        /// </summary>
        public const string PasswordGrantType = "password";

        /// <summary>
        /// The user name property name.
        /// </summary>
        public const string UserNameProperty = "username";

        /// <summary>
        /// The password property name.
        /// </summary>
        public const string PasswordProperty = "password";

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="appId">The client id and secret key.</param>
        public PasswordTokenRequest(AppAccessingKey appId) : base(PasswordGrantType, appId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public PasswordTokenRequest(string id, string secret = null) : base(PasswordGrantType, id, secret)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        public PasswordTokenRequest(string id, SecureString secret) : base(PasswordGrantType, id, secret)
        {
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [DataMember(Name = UserNameProperty)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [DataMember(Name = PasswordProperty)]
        public SecureString Password { get; set; }

        /// <summary>
        /// Parses a string to code access token request.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The object parsed returned.</returns>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public static PasswordTokenRequest Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s), "s should not be null, empty or consists only of white-space characters.");
            var m = TokenRequestModel.Parse(s);
            if (m == null) throw new NotSupportedException("s cannot be parsed.");
            var grantTypeExpect = PasswordGrantType;
            if (m.GrantType != grantTypeExpect) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {grantTypeExpect}.");
            return new PasswordTokenRequest(new AppAccessingKey(m.ClientId, m.ClientSecret))
            {
                ScopeString = m.Scope,
                UserName = m.UserName,
                Password = !string.IsNullOrEmpty(m.Password) ? m.Password.ToSecure() : null
            };
        }
    }

    [DataContract]
    internal class TokenRequestModel
    {
        [DataMember(Name = TokenRequest.GrantTypeProperty)]
        public string GrantType { get; set; }

        [DataMember(Name = TokenRequest.ClientIdProperty)]
        public string ClientId { get; set; }

        [DataMember(Name = TokenRequest.ClientSecretProperty)]
        public string ClientSecret { get; set; }

        [DataMember(Name = TokenRequest.ScopeProperty)]
        public string Scope { get; set; }

        [DataMember(Name = CodeTokenRequest.RedirectUriProperty)]
        public string RedirectUri { get; set; }

        [DataMember(Name = CodeTokenRequest.CodeProperty)]
        public string Code { get; set; }

        [DataMember(Name = CodeTokenRequest.CodeVerifierProperty)]
        public string CodeVerifier { get; set; }

        [DataMember(Name = RefreshTokenRequest.RefreshTokenProperty)]
        public string RefreshToken { get; set; }

        [DataMember(Name = PasswordTokenRequest.UserNameProperty)]
        public string UserName { get; set; }

        [DataMember(Name = PasswordTokenRequest.PasswordProperty)]
        public string Password { get; set; }

        public static TokenRequestModel Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim();
            if (s.IndexOf("{") == 0) return StringUtility.FromJson<TokenRequestModel>(s);
            var q = QueryData.Parse(s);
            return new TokenRequestModel
            {
                GrantType = q[TokenRequest.GrantTypeProperty],
                ClientId = q[TokenRequest.ClientIdProperty],
                ClientSecret = q[TokenRequest.ClientSecretProperty],
                Scope = q[TokenRequest.ScopeProperty],
                Code = q[CodeTokenRequest.CodeProperty],
                CodeVerifier = q[CodeTokenRequest.CodeVerifierProperty],
                RedirectUri = q[CodeTokenRequest.RedirectUriProperty],
                RefreshToken = q[RefreshTokenRequest.RefreshTokenProperty],
                UserName = q[PasswordTokenRequest.UserNameProperty],
                Password = q[PasswordTokenRequest.PasswordProperty]
            };
        }
    }
}
