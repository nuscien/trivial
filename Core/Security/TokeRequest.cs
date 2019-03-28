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
    public class TokenRequest
    {
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
        /// Gets the scope string.
        /// </summary>
        [DataMember(Name = TokenRequestBody.ScopeProperty)]
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
            var data = Body?.ToQueryData() ?? new QueryData();
            if (!string.IsNullOrWhiteSpace(ClientId)) data.Add(TokenRequestBody.ClientIdProperty, ClientId);
            if (ClientCredentials != null && ClientCredentials.Secret != null && ClientCredentials.Secret.Length > 0) data.Add(TokenRequestBody.ClientSecretProperty, ClientCredentials.Secret.ToUnsecureString());
            if (!string.IsNullOrWhiteSpace(ScopeString)) data.Add(TokenRequestBody.ScopeProperty, ScopeString);
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
                data.Add(TokenRequestBody.ScopeProperty, $"\"{ScopeString}\"");
            return data;
        }

        /// <summary>
        /// Gets the JSON format string.
        /// </summary>
        /// <returns>A string in JSON format.</returns>
        public virtual string ToJsonString()
        {
            var data = ToJsonProperites();
            var sb = new StringBuilder("{ ");
            foreach (var kvp in data)
            {
                if (string.IsNullOrWhiteSpace(kvp.Key) || string.IsNullOrWhiteSpace(kvp.Value)) continue;
                sb.Append('"');
                sb.Append(StringUtility.ReplaceBackSlash(kvp.Key));
                sb.Append("\": ");
                sb.Append(StringUtility.ReplaceBackSlash(kvp.Value));
                sb.Append(", ");
            }

            if (sb.Length > 3) sb.Remove(sb.Length - 2, 2);
            sb.Append(" }");
            return sb.ToString();
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
        /// The scope property name.
        /// </summary>
        public const string ScopeProperty = "scope";

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
        /// <param name="clearOriginal">true if clear the properties in the current request body before filling; otherwise, false.</param>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public virtual void Fill(string s, bool clearOriginal = true)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            var m = TokenRequestModel.Parse(s);
            if (!Fill(m, clearOriginal)) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {GrantType}.");
        }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="m">The token request model.</param>
        /// <param name="clearOriginal">true if clear the properties in the current request body before filling; otherwise, false.</param>
        /// <returns>true if fill succeeded; otherwise, false.</returns>
        private bool Fill(TokenRequestModel m, bool clearOriginal)
        {
            if (m == null || m.GrantType != GrantType) return false;
            return true;
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
            var m = TokenRequestModel.Parse(s);
            if (m == null) throw new NotSupportedException("s cannot be parsed.");
            var body = new ClientTokenRequestBody();
            if (!body.Fill(m, false)) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {body.GrantType}.");
            return new TokenRequest<ClientTokenRequestBody>(body, new AppAccessingKey(m.ClientId, m.ClientSecret))
            {
                ScopeString = m.Scope
            };
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
        /// <param name="s">A string to be parsed.</param>
        /// <param name="clearOriginal">true if clear the properties in the current request body before filling; otherwise, false.</param>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public virtual void Fill(string s, bool clearOriginal = true)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            var m = TokenRequestModel.Parse(s);
            if (!Fill(m, clearOriginal)) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {GrantType}.");
        }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="m">The token request model.</param>
        /// <param name="clearOriginal">true if clear the properties in the current request body before filling; otherwise, false.</param>
        /// <returns>true if fill succeeded; otherwise, false.</returns>
        private bool Fill(TokenRequestModel m, bool clearOriginal)
        {
            if (m == null || m.GrantType != GrantType) return false;
            if (clearOriginal)
            {
                Code = null;
                CodeVerifier = null;
                RedirectUri = null;
            }

            if (!string.IsNullOrWhiteSpace(m.Code)) Code = m.Code;
            if (!string.IsNullOrWhiteSpace(m.CodeVerifier)) CodeVerifier = m.CodeVerifier;
            if (!string.IsNullOrWhiteSpace(m.RedirectUri)) RedirectUri = new Uri(m.RedirectUri);
            return true;
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
            var body = new CodeTokenRequestBody();
            if (!body.Fill(m, false)) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {body.GrantType}.");
            return new CodeTokenRequest(body, new AppAccessingKey(m.ClientId, m.ClientSecret))
            {
                ScopeString = m.Scope
            };
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
                { TokenRequestBody.ScopeProperty, ScopeString },
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
        /// The refresh token property name.
        /// </summary>
        public const string RefreshTokenProperty = "refresh_token";

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
        /// Gets or sets the refresh token.
        /// </summary>
        [DataMember(Name = RefreshTokenProperty)]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="s">A string to be parsed.</param>
        /// <param name="clearOriginal">true if clear the properties in the current request body before filling; otherwise, false.</param>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public virtual void Fill(string s, bool clearOriginal = true)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            var m = TokenRequestModel.Parse(s);
            if (!Fill(m, clearOriginal)) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {GrantType}.");
        }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="m">The token request model.</param>
        /// <param name="clearOriginal">true if clear the properties in the current request body before filling; otherwise, false.</param>
        /// <returns>true if fill succeeded; otherwise, false.</returns>
        private bool Fill(TokenRequestModel m, bool clearOriginal)
        {
            if (m == null || m.GrantType != GrantType) return false;
            if (clearOriginal)
            {
                RefreshToken = null;
            }

            if (!string.IsNullOrWhiteSpace(m.RefreshToken)) RefreshToken = m.RefreshToken;
            return true;
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
            var m = TokenRequestModel.Parse(s);
            if (m == null) throw new NotSupportedException("s cannot be parsed.");
            var body = new RefreshTokenRequestBody();
            if (!body.Fill(m, false)) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {body.GrantType}.");
            return new TokenRequest<RefreshTokenRequestBody>(body, new AppAccessingKey(m.ClientId, m.ClientSecret))
            {
                ScopeString = m.Scope
            };
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
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="s">A string to be parsed.</param>
        /// <param name="clearOriginal">true if clear the properties in the current request body before filling; otherwise, false.</param>
        /// <exception cref="ArgumentNullException">s was null, empty or consists only of white-space characters.</exception>
        /// <exception cref="ArgumentException">s was not in correct format to parse.</exception>
        /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
        /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
        public virtual void Fill(string s, bool clearOriginal = true)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            var m = TokenRequestModel.Parse(s);
            if (!Fill(m, clearOriginal)) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {GrantType}.");
        }

        /// <summary>
        /// Fills the data into the current request body.
        /// </summary>
        /// <param name="m">The token request model.</param>
        /// <param name="clearOriginal">true if clear the properties in the current request body before filling; otherwise, false.</param>
        /// <returns>true if fill succeeded; otherwise, false.</returns>
        private bool Fill(TokenRequestModel m, bool clearOriginal)
        {
            if (m == null || m.GrantType != GrantType) return false;
            if (clearOriginal)
            {
                UserName = null;
                Password = null;
            }

            if (!string.IsNullOrWhiteSpace(m.UserName)) UserName = m.UserName;
            if (!string.IsNullOrWhiteSpace(m.Password)) Password = m.Password.ToSecure();
            return true;
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
            var m = TokenRequestModel.Parse(s);
            if (m == null) throw new NotSupportedException("s cannot be parsed.");
            var body = new PasswordTokenRequestBody();
            if (!body.Fill(m, false)) throw new InvalidOperationException($"The grant type is not the expected one. Current is {m.GrantType}; but the expect is {body.GrantType}.");
            return new TokenRequest<PasswordTokenRequestBody>(body, new AppAccessingKey(m.ClientId, m.ClientSecret))
            {
                ScopeString = m.Scope
            };
        }
    }

    [DataContract]
    internal class TokenRequestModel
    {
        [DataMember(Name = TokenRequestBody.GrantTypeProperty)]
        public string GrantType { get; set; }

        [DataMember(Name = TokenRequestBody.ClientIdProperty)]
        public string ClientId { get; set; }

        [DataMember(Name = TokenRequestBody.ClientSecretProperty)]
        public string ClientSecret { get; set; }

        [DataMember(Name = TokenRequestBody.ScopeProperty)]
        public string Scope { get; set; }

        [DataMember(Name = CodeTokenRequestBody.RedirectUriProperty)]
        public string RedirectUri { get; set; }

        [DataMember(Name = CodeTokenRequestBody.CodeProperty)]
        public string Code { get; set; }

        [DataMember(Name = CodeTokenRequestBody.CodeVerifierProperty)]
        public string CodeVerifier { get; set; }

        [DataMember(Name = RefreshTokenRequestBody.RefreshTokenProperty)]
        public string RefreshToken { get; set; }

        [DataMember(Name = PasswordTokenRequestBody.UserNameProperty)]
        public string UserName { get; set; }

        [DataMember(Name = PasswordTokenRequestBody.PasswordProperty)]
        public string Password { get; set; }

        public static TokenRequestModel Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var q = QueryData.Parse(s);
            return new TokenRequestModel
            {
                GrantType = q[TokenRequestBody.GrantTypeProperty],
                ClientId = q[TokenRequestBody.ClientIdProperty],
                ClientSecret = q[TokenRequestBody.ClientSecretProperty],
                Scope = q[TokenRequestBody.ScopeProperty],
                Code = q[CodeTokenRequestBody.CodeProperty],
                CodeVerifier = q[CodeTokenRequestBody.CodeVerifierProperty],
                RedirectUri = q[CodeTokenRequestBody.RedirectUriProperty],
                RefreshToken = q[RefreshTokenRequestBody.RefreshTokenProperty],
                UserName = q[PasswordTokenRequestBody.UserNameProperty],
                Password = q[PasswordTokenRequestBody.PasswordProperty]
            };
        }
    }
}
