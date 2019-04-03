using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
    public class TokenRequest
    {
        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="tokenRequest">The token request.</param>
        public TokenRequest(TokenRequest tokenRequest)
        {
            if (tokenRequest == null) return;
            Body = tokenRequest.Body;
            ClientCredentials = tokenRequest.ClientCredentials;
            ScopeString = tokenRequest.ScopeString;
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="appId">The client id and secret key.</param>
        /// <param name="scope">The scope.</param>
        public TokenRequest(TokenRequestBody body, AppAccessingKey appId, IEnumerable<string> scope = null)
        {
            Body = body;
            ClientCredentials = appId;
            if (scope != null) Scope = scope.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        /// <param name="scope">The scope.</param>
        public TokenRequest(TokenRequestBody body, string id, string secret = null, IEnumerable<string> scope = null) : this(body, new AppAccessingKey(id, secret), scope)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        /// <param name="scope">The scope.</param>
        public TokenRequest(TokenRequestBody body, string id, SecureString secret, IEnumerable<string> scope = null) : this(body, new AppAccessingKey(id, secret), scope)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="clientCredentials">The client credentials and scope query data.</param>
        public TokenRequest(TokenRequestBody body, QueryData clientCredentials)
        {
            Body = body;
            if (clientCredentials == null) return;
            var clientId = clientCredentials[TokenRequestBody.ClientIdProperty];
            var clientSecret = clientCredentials[TokenRequestBody.ClientSecretProperty];
            if (!string.IsNullOrEmpty(clientId) || !string.IsNullOrEmpty(clientSecret)) ClientCredentials = new AppAccessingKey(clientId, clientSecret);
            ScopeString = clientCredentials[TokenInfo.ScopeProperty];
        }

        /// <summary>
        /// Gets the client identifier and secret key.
        /// </summary>
        public AppAccessingKey ClientCredentials { get; private set; }

        /// <summary>
        /// Gets the body.
        /// </summary>
        [DataMember(Name = "data")]
        public TokenRequestBody Body { get; }

        /// <summary>
        /// Gets the grant type.
        /// </summary>
        [DataMember(Name = TokenRequestBody.GrantTypeProperty)]
        public string GrantType => Body?.GrantType;

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        [DataMember(Name = TokenRequestBody.ClientIdProperty)]
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
        /// Gets or sets the scope string.
        /// </summary>
        [DataMember(Name = TokenInfo.ScopeProperty)]
        public string ScopeString
        {
            get
            {
                return Scope.Count > 0 ? string.Join(" ", Scope) : null;
            }

            set
            {
                Scope.Clear();
                if (string.IsNullOrWhiteSpace(value)) return;
                foreach (var ele in value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Scope.Add(ele);
                }
            }
        }

        /// <summary>
        /// Gets the query data.
        /// </summary>
        /// <returns>A query data.</returns>
        public virtual QueryData ToQueryData()
        {
            var data = Body?.ToQueryData() ?? new QueryData();
            if (!string.IsNullOrWhiteSpace(ClientId)) data.Add(TokenRequestBody.ClientIdProperty, ClientId);
            if (ClientCredentials != null && ClientCredentials.Secret != null && ClientCredentials.Secret.Length > 0) data.Add(TokenRequestBody.ClientSecretProperty, ClientCredentials.Secret.ToUnsecureString());
            if (!string.IsNullOrWhiteSpace(ScopeString)) data.Add(TokenInfo.ScopeProperty, ScopeString);
            return data;
        }

        /// <summary>
        /// Gets the JSON format string.
        /// </summary>
        /// <returns>A string in JSON format.</returns>
        protected virtual IDictionary<string, string> ToJsonProperites()
        {
            var data = Body?.ToJsonProperites() ?? new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(ClientId))
                data.Add(TokenRequestBody.ClientIdProperty, $"\"{ClientId}\"");
            if (ClientCredentials != null && ClientCredentials.Secret != null && ClientCredentials.Secret.Length > 0)
                data.Add(TokenRequestBody.ClientSecretProperty, $"\"{ClientCredentials.Secret.ToUnsecureString()}\"");
            if (!string.IsNullOrWhiteSpace(ScopeString))
                data.Add(TokenInfo.ScopeProperty, $"\"{ScopeString}\"");
            return data;
        }

        /// <summary>
        /// Gets the JSON format string.
        /// </summary>
        /// <returns>A string in JSON format.</returns>
        public virtual string ToJsonString()
        {
            var data = ToJsonProperites();
            var sb = new StringBuilder("{");
            foreach (var kvp in data)
            {
                if (string.IsNullOrWhiteSpace(kvp.Key) || string.IsNullOrWhiteSpace(kvp.Value)) continue;
                sb.Append('"');
                sb.Append(StringUtility.ReplaceBackSlash(kvp.Key));
                sb.Append("\":");
                sb.Append(StringUtility.ReplaceBackSlash(kvp.Value));
                sb.Append(",");
            }

            if (sb.Length > 3) sb.Remove(sb.Length - 2, 2);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A query string of the token request but without client secret.</returns>
        public override string ToString()
        {
            var data = ToQueryData();
            data.Remove(TokenRequestBody.ClientSecretProperty);
            return data.ToString();
        }
    }

    /// <summary>
    /// The access token resolver request.
    /// </summary>
    [DataContract]
    public class TokenRequest<T> : TokenRequest where T : TokenRequestBody
    {
        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="tokenRequest">The token request.</param>
        public TokenRequest(TokenRequest<T> tokenRequest) : base(tokenRequest)
        {
            if (tokenRequest == null) return;
            Body = tokenRequest.Body;
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="appId">The client id and secret key.</param>
        /// <param name="scope">The scope.</param>
        public TokenRequest(T body, AppAccessingKey appId, IEnumerable<string> scope = null) : base(body, appId, scope)
        {
            Body = body;
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        /// <param name="scope">The scope.</param>
        public TokenRequest(T body, string id, string secret = null, IEnumerable<string> scope = null) : base(body, id, secret, scope)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        /// <param name="scope">The scope.</param>
        public TokenRequest(T body, string id, SecureString secret, IEnumerable<string> scope = null) : base(body, id, secret, scope)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="clientCredentials">The client credentials and scope query data.</param>
        public TokenRequest(T body, QueryData clientCredentials) : base(body, clientCredentials)
        {
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public new T Body { get; }
    }

    /// <summary>
    /// The app secret key for accessing api.
    /// </summary>
    public class AppAccessingKey
    {
        /// <summary>
        /// Initializes a new instance of the AppAccessingKey class.
        /// </summary>
        public AppAccessingKey()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AppAccessingKey class.
        /// </summary>
        /// <param name="id">The app id.</param>
        /// <param name="secret">The secret key.</param>
        public AppAccessingKey(string id, string secret = null)
        {
            Id = id;
            if (secret != null) Secret = secret.ToSecure();
        }

        /// <summary>
        /// Initializes a new instance of the AppAccessingKey class.
        /// </summary>
        /// <param name="id">The app id.</param>
        /// <param name="secret">The secret key.</param>
        public AppAccessingKey(string id, SecureString secret)
        {
            Id = id;
            Secret = secret;
        }

        /// <summary>
        /// The app id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The secret key.
        /// </summary>
        public SecureString Secret { get; set; }

        /// <summary>
        /// Gets additional string bag.
        /// </summary>
        public IDictionary<string, string> Bag { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Tests if the app accessing key is null or empty.
        /// </summary>
        /// <param name="appKey">The app accessing key instance.</param>
        /// <returns>true if it is null or empty; otherwise, false.</returns>
        public static bool IsNullOrEmpty(AppAccessingKey appKey)
        {
            try
            {
                return appKey == null || string.IsNullOrWhiteSpace(appKey.Id) || appKey.Secret == null || appKey.Secret.Length == 0;
            }
            catch (ObjectDisposedException)
            {
            }

            return true;
        }

        /// <summary>
        /// Returns a System.String that represents the current AppAccessingKey.
        /// </summary>
        /// <returns>A System.String that represents the current AppAccessingKey.</returns>
        public override string ToString()
        {
            return Id ?? string.Empty;
        }
    }

    /// <summary>
    /// The access token resolver request body.
    /// </summary>
    [DataContract]
    public abstract class TokenRequestBody
    {
        /// <summary>
        /// The grant type property name.
        /// </summary>
        public const string GrantTypeProperty = "grant_type";

        /// <summary>
        /// The client identifier property name.
        /// </summary>
        public const string ClientIdProperty = "client_id";

        /// <summary>
        /// The client secret property name.
        /// </summary>
        public const string ClientSecretProperty = "client_secret";

        /// <summary>
        /// Initializes a new instance of the TokenRequestBody class.
        /// </summary>
        /// <param name="grantType">The grant type.</param>
        protected TokenRequestBody(string grantType)
        {
            GrantType = grantType;
        }

        /// <summary>
        /// Gets the grant type.
        /// </summary>
        [DataMember(Name = GrantTypeProperty)]
        public string GrantType { get; }

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
                else if (!string.IsNullOrWhiteSpace(propValue.ToString())) data.Add(attr.Name, propValue.ToString());
            }

            return data;
        }

        /// <summary>
        /// Gets the JSON format string.
        /// </summary>
        /// <returns>A string in JSON format.</returns>
        internal protected virtual IDictionary<string, string> ToJsonProperites()
        {
            var data = new Dictionary<string, string>();
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
                if (propType == typeof(Uri)) data.Add(attr.Name, $"\"{((Uri)propValue).OriginalString}\"");
                else if (propType == typeof(DateTime)) data.Add(attr.Name, $"{WebUtility.ParseDate((DateTime)propValue).ToString()}");
                else if (propType == typeof(DateTimeOffset)) data.Add(attr.Name, $"{WebUtility.ParseDate((DateTimeOffset)propValue).ToString()}");
                else if (propType == typeof(int) || propType == typeof(long) || propType == typeof(uint) || propType == typeof(ulong) || propType == typeof(float) || propType == typeof(double) || propType == typeof(short) || propType == typeof(bool)) data.Add(attr.Name, $"{propValue.ToString()}");
                else if (propType == typeof(SecureString)) data.Add(attr.Name, $"\"{((SecureString)propValue).ToUnsecureString()}\"");
                else if (!string.IsNullOrWhiteSpace(propValue.ToString())) data.Add(attr.Name, $"\"{propValue.ToString()}\"");
            }

            return data;
        }
    }

    /// <summary>
    /// The access token resolver request with client credentials grant type.
    /// </summary>
    [DataContract]
    public class ClientTokenRequestBody : TokenRequestBody
    {
        /// <summary>
        /// The grant type value of client credentials.
        /// </summary>
        public const string ClientCredentialsGrantType = "client_credentials";

        /// <summary>
        /// Initializes a new instance of the ClientCredentialsTokenRequest class.
        /// </summary>
        public ClientTokenRequestBody() : base(ClientCredentialsGrantType)
        {
        }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="s">A string to be parsed.</param>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public virtual void Fill(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            var q = QueryData.Parse(s);
            Fill(q);
        }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="q">The query data.</param>
        /// <returns>true if fill succeeded; otherwise, false.</returns>
        private void Fill(QueryData q)
        {
            if (q == null) return;
            var grantType = q[GrantTypeProperty];
            if (!string.IsNullOrEmpty(grantType) && grantType != GrantType) throw new InvalidOperationException($"The grant type is not the expected one. Current is {grantType}; but the expect is {GrantType}.");
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
        public static TokenRequest<ClientTokenRequestBody> Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s), "s should not be null, empty or consists only of white-space characters.");
            var q = QueryData.Parse(s);
            var body = new ClientTokenRequestBody();
            body.Fill(q);
            return new TokenRequest<ClientTokenRequestBody>(body, q);
        }
    }

    /// <summary>
    /// The access token request with authorization code grant type.
    /// </summary>
    [DataContract]
    public class CodeTokenRequestBody : TokenRequestBody
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
        /// The state property name.
        /// </summary>
        public const string StateProperty = "state";

        /// <summary>
        /// The response type property name.
        /// </summary>
        public const string ResponseType = "response_type";

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        public CodeTokenRequestBody() : base(AuthorizationCodeGrantType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="code">The authorization code.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="codeVerifier">The code verifier.</param>
        public CodeTokenRequestBody(string code, Uri redirectUri = null, string codeVerifier = null) : this()
        {
            Code = code;
            RedirectUri = redirectUri;
            CodeVerifier = codeVerifier;
        }

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="tokenRequestBody">Another token request body to copy.</param>
        public CodeTokenRequestBody(CodeTokenRequestBody tokenRequestBody) : this(tokenRequestBody.Code, tokenRequestBody.RedirectUri, tokenRequestBody.CodeVerifier)
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
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="s">A string to parse.</param>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public virtual void Fill(string s)
        {
            var q = QueryData.Parse(s);
            Fill(q);
        }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="q">The query data.</param>
        /// <returns>true if fill succeeded; otherwise, false.</returns>
        private void Fill(QueryData q)
        {
            if (q == null) return;
            var grantType = q[GrantTypeProperty];
            if (!string.IsNullOrEmpty(grantType) && grantType != GrantType) throw new InvalidOperationException($"The grant type is not the expected one. Current is {grantType}; but the expect is {GrantType}.");
            var code = q[CodeProperty];
            if (code != null) Code = q[CodeProperty];
            var codeVerifier = q[CodeVerifierProperty];
            if (codeVerifier != null) CodeVerifier = codeVerifier;
            var redirectUri = q[RedirectUriProperty];
            if (redirectUri != null) RedirectUri = redirectUri == string.Empty ? null : new Uri(redirectUri);
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
            var q = QueryData.Parse(s);
            var body = new CodeTokenRequestBody();
            body.Fill(q);
            return new CodeTokenRequest(body, q);
        }
    }

    /// <summary>
    /// The access token resolver request.
    /// </summary>
    [DataContract]
    public class CodeTokenRequest : TokenRequest<CodeTokenRequestBody>
    {
        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="tokenRequest">The token request.</param>
        public CodeTokenRequest(TokenRequest<CodeTokenRequestBody> tokenRequest) : base(tokenRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="appId">The client id and secret key.</param>
        /// <param name="scope">The scope.</param>
        public CodeTokenRequest(CodeTokenRequestBody body, AppAccessingKey appId, IEnumerable<string> scope = null) : base(body, appId, scope)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        /// <param name="scope">The scope.</param>
        public CodeTokenRequest(CodeTokenRequestBody body, string id, string secret = null, IEnumerable<string> scope = null) : base(body, id, secret, scope)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        /// <param name="scope">The scope.</param>
        public CodeTokenRequest(CodeTokenRequestBody body, string id, SecureString secret, IEnumerable<string> scope = null) : base(body, id, secret, scope)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="body">The request body.</param>
        /// <param name="clientCredentials">The client credentials and scope query data.</param>
        public CodeTokenRequest(CodeTokenRequestBody body, QueryData clientCredentials) : base(body, clientCredentials)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="code">The authorization code.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        /// <param name="scope">The scope.</param>
        public CodeTokenRequest(string code, Uri redirectUri, string id, string secret = null, IEnumerable<string> scope = null) : base(new CodeTokenRequestBody
        {
            Code = code,
            RedirectUri = redirectUri
        }, id, secret, scope)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodeTokenRequest class.
        /// </summary>
        /// <param name="code">The authorization code.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="id">The client id.</param>
        /// <param name="secret">The client secret key.</param>
        /// <param name="scope">The scope.</param>
        public CodeTokenRequest(string code, Uri redirectUri, string id, SecureString secret, IEnumerable<string> scope = null) : base(new CodeTokenRequestBody
        {
            Code = code,
            RedirectUri = redirectUri
        }, id, secret, scope)
        {
        }

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
                { CodeTokenRequestBody.ResponseType, responseType },
                { TokenRequestBody.ClientIdProperty, ClientId },
                { CodeTokenRequestBody.RedirectUriProperty, Body?.RedirectUri?.OriginalString },
                { TokenInfo.ScopeProperty, ScopeString },
                { CodeTokenRequestBody.StateProperty, state }
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
            return CodeTokenRequestBody.Parse(s);
        }
    }

    /// <summary>
    /// The access token request with refresh token grant type.
    /// </summary>
    [DataContract]
    public class RefreshTokenRequestBody : TokenRequestBody
    {
        /// <summary>
        /// The grant type value of refresh token.
        /// </summary>
        public const string RefreshTokenGrantType = "refresh_token";

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        public RefreshTokenRequestBody() : base(RefreshTokenGrantType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        public RefreshTokenRequestBody(string refreshToken) : this()
        {
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Initializes a new instance of the RereshTokenRequest class.
        /// </summary>
        /// <param name="tokenRequestBody">Another token request body to copy.</param>
        public RefreshTokenRequestBody(RefreshTokenRequestBody tokenRequestBody) : this(tokenRequestBody.RefreshToken)
        {
        }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        [DataMember(Name = TokenInfo.RefreshTokenProperty)]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="s">A string to be parsed.</param>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public virtual void Fill(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            var q = QueryData.Parse(s);
            Fill(q);
        }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="q">The query data.</param>
        /// <returns>true if fill succeeded; otherwise, false.</returns>
        private void Fill(QueryData q)
        {
            if (q == null) return;
            var grantType = q[GrantTypeProperty];
            if (!string.IsNullOrEmpty(grantType) && grantType != GrantType) throw new InvalidOperationException($"The grant type is not the expected one. Current is {grantType}; but the expect is {GrantType}.");
            var refreshToken = q[TokenInfo.RefreshTokenProperty];
            if (refreshToken != null) RefreshToken = refreshToken;
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
        public static TokenRequest<RefreshTokenRequestBody> Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s), "s should not be null, empty or consists only of white-space characters.");
            var q = QueryData.Parse(s);
            var body = new RefreshTokenRequestBody();
            body.Fill(q);
            return new TokenRequest<RefreshTokenRequestBody>(body, q);
        }
    }

    /// <summary>
    /// The access token request with password grant type.
    /// </summary>
    [DataContract]
    public class PasswordTokenRequestBody : TokenRequestBody
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
        /// The token type.
        /// </summary>
        public const string BasicTokenType = "Basic";

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        public PasswordTokenRequestBody() : base(PasswordGrantType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public PasswordTokenRequestBody(string userName, string password) : this()
        {
            UserName = userName;
            Password = password.ToSecure();
        }

        /// <summary>
        /// Initializes a new instance of the TokenRequest class.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public PasswordTokenRequestBody(string userName, SecureString password) : this()
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Initializes a new instance of the PasswordTokenRequest class.
        /// </summary>
        /// <param name="tokenRequestBody">Another token request body to copy.</param>
        public PasswordTokenRequestBody(PasswordTokenRequestBody tokenRequestBody) : this(tokenRequestBody.UserName, tokenRequestBody.Password)
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
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current user name and password information.
        /// </summary>
        /// <param name="schemeCase">The scheme case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current user name and password information.</returns>
        public AuthenticationHeaderValue ToAuthenticationHeaderValue(Cases schemeCase = Cases.Original)
        {
            return ToAuthenticationHeaderValue(null, schemeCase);
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.
        /// </summary>
        /// <param name="encoding">The text encoding.</param>
        /// <param name="schemeCase">The scheme case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.</returns>
        public AuthenticationHeaderValue ToAuthenticationHeaderValue(Encoding encoding, Cases schemeCase = Cases.Original)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(UserName)) sb.Append(UserName);
            sb.Append(':');
            if (Password != null && Password.Length > 0) sb.Append(Password.ToUnsecureString());
            return new AuthenticationHeaderValue(
                StringUtility.ToSpecificCaseInvariant(BasicTokenType, schemeCase),
                Convert.ToBase64String((encoding ?? Encoding.UTF8).GetBytes(sb.ToString())));
        }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="s">A string to be parsed.</param>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public virtual void Fill(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            var q = QueryData.Parse(s);
            Fill(q);
        }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="q">The query data.</param>
        /// <returns>true if fill succeeded; otherwise, false.</returns>
        private void Fill(QueryData q)
        {
            if (q == null) return;
            var grantType = q[GrantTypeProperty];
            if (!string.IsNullOrEmpty(grantType) && grantType != GrantType) throw new InvalidOperationException($"The grant type is not the expected one. Current is {grantType}; but the expect is {GrantType}.");
            var userName = q[UserNameProperty];
            if (userName != null) UserName = userName;
            var password = q[PasswordProperty];
            if (password != null) Password = password == string.Empty ? null : password.ToSecure();
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
        public static TokenRequest<PasswordTokenRequestBody> Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s), "s should not be null, empty or consists only of white-space characters.");
            var q = QueryData.Parse(s);
            var body = new PasswordTokenRequestBody();
            body.Fill(q);
            return new TokenRequest<PasswordTokenRequestBody>(body, q);
        }
    }
}
