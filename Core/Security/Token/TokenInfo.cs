using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.Data;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Security;

/// <summary>
/// The access token response information.
/// </summary>
[DataContract]
public class TokenInfo
{
    /// <summary>
    /// The error codes of token response.
    /// </summary>
    public static class ErrorCodeConstants
    {
        /// <summary>
        /// The unknown error
        /// </summary>
        public const string Unknown = "unknown";

        /// <summary>
        /// The request is missing a required parameter,
        /// includes an invalid parameter value,
        /// includes a parameter more than once,
        /// or is otherwise malformed.
        /// </summary>
        public const string InvalidRequest = "invalid_request";

        /// <summary>
        /// Client authentication failed.
        /// </summary>
        public const string InvalidClient = "invalid_client";

        /// <summary>
        /// The provided authorization grant or refresh token is
        /// invalid, expired, revoked,
        /// does not match the redirection URI used in the authorization request,
        /// or was issued to another client.
        /// </summary>
        public const string InvalidGrant = "invalid_grant";

        /// <summary>
        /// The client is not authorized to request an access token using this method.
        /// </summary>
        public const string UnauthorizedClient = "UnauthorizedClient";

        /// <summary>
        /// The resource owner or authorization server denied the request.
        /// </summary>
        public const string AccessDenied = "access_denied";

        /// <summary>
        /// The authorization server does not support obtaining an access token using this method.
        /// </summary>
        public const string UnsupportedResponseType = "unsupported_response_type";

        /// <summary>
        /// The authorization grant type is not supported by the authorization server.
        /// </summary>
        public const string UnsupportedGrantType = "unsupported_grant_type";

        /// <summary>
        /// The requested scope is invalid, unknown, or malformed.
        /// </summary>
        public const string InvalidScope = "invalid_scope";

        /// <summary>
        /// The authorization server encountered an unexpected condition
        /// that prevented it from fulfilling the request.
        /// </summary>
        public const string ServerError = "server_error";

        /// <summary>
        /// The authorization server is currently unable to handle the request
        /// due to a temporary overloading or maintenance of the server.
        /// </summary>
        public const string TemporarilyUnavailable = "temporarily_unavailable";
    }

    /// <summary>
    /// The user identifier property name.
    /// </summary>
    public const string TokenKey = "token";

    /// <summary>
    /// The user identifier property name.
    /// </summary>
    public const string UserIdProperty = "user_id";

    /// <summary>
    /// The resource identifier property name.
    /// </summary>
    public const string ResourceIdProperty = "res_id";

    /// <summary>
    /// The user name property name.
    /// </summary>
    public const string UserNameProperty = "username";

    /// <summary>
    /// The token reference identifier property name.
    /// </summary>
    public const string TokenIdProperty = "token_id";

    /// <summary>
    /// The access token property name.
    /// </summary>
    public const string AccessTokenProperty = "access_token";

    /// <summary>
    /// The token type property name.
    /// </summary>
    public const string TokenTypeProperty = "token_type";

    /// <summary>
    /// The refresh token property name.
    /// </summary>
    public const string RefreshTokenProperty = "refresh_token";

    /// <summary>
    /// The expires in property name.
    /// </summary>
    public const string ExpiresInProperty = "expires_in";

    /// <summary>
    /// The error code property name.
    /// </summary>
    public const string ErrorCodeProperty = "error";

    /// <summary>
    /// The error description property name.
    /// </summary>
    public const string ErrorDescriptionProperty = "error_description";

    /// <summary>
    /// The error description property name.
    /// </summary>
    public const string ErrorUriProperty = "error_uri";

    /// <summary>
    /// The state property name.
    /// </summary>
    public const string StateProperty = "state";

    /// <summary>
    /// The scope property name.
    /// </summary>
    public const string ScopeProperty = "scope";

    /// <summary>
    /// The token type.
    /// </summary>
    public const string BearerTokenType = "Bearer";

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    public TokenInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="tokenType">The token type.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="accessToken">The access token.</param>
    /// <param name="scope">The permission scope.</param>
    public TokenInfo(string tokenType, string userId, string accessToken, IEnumerable<string> scope = null)
    {
        TokenType = tokenType;
        UserId = userId;
        AccessToken = accessToken;
        Scope = ListExtensions.ToList(scope, false);
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    public TokenInfo(JsonWebToken<JsonWebTokenPayload> jwt, string refreshToken, IEnumerable<string> scope = null)
    {
        Scope = ListExtensions.ToList(scope, false);
        TokenType = BearerTokenType;
        RefreshToken = refreshToken;
        if (jwt?.Payload == null) return;
        AccessToken = jwt.ToEncodedString();
        UserId = jwt.Payload.Subject;
        var expiration = jwt.Payload.Expiration;
        if (expiration.HasValue) ExpiredAfter = expiration - DateTime.Now;
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="scope">The permission scope.</param>
    public TokenInfo(JsonWebToken<JsonWebTokenPayload> jwt, IEnumerable<string> scope = null)
        : this(jwt, null, scope)
    {
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    public TokenInfo(JsonWebToken<RSASecretExchange.JsonWebTokenPayload> jwt, string refreshToken, IEnumerable<string> scope = null)
    {
        Scope = ListExtensions.ToList(scope, false);
        TokenType = BearerTokenType;
        RefreshToken = refreshToken;
        if (jwt?.Payload == null) return;
        AccessToken = jwt.ToEncodedString();
        UserId = jwt.Payload.Subject;
        var expiration = jwt.Payload.Expiration;
        if (expiration.HasValue) ExpiredAfter = expiration - DateTime.Now;
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="scope">The permission scope.</param>
    public TokenInfo(JsonWebToken<RSASecretExchange.JsonWebTokenPayload> jwt, IEnumerable<string> scope = null)
        : this(jwt, null, scope)
    {
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    public TokenInfo(JsonWebToken<JsonObjectNode> jwt, string refreshToken, IEnumerable<string> scope = null)
    {
        Scope = ListExtensions.ToList(scope, false);
        TokenType = BearerTokenType;
        RefreshToken = refreshToken;
        if (jwt?.Payload == null) return;
        AccessToken = jwt.ToEncodedString();
        UserId = jwt.Payload.TryGetStringTrimmedValue("sub", true);
        var expiration = jwt.Payload.TryGetDateTimeValue("exp", true);
        if (expiration.HasValue) ExpiredAfter = expiration - DateTime.Now;
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="scope">The permission scope.</param>
    public TokenInfo(JsonWebToken<JsonObjectNode> jwt, IEnumerable<string> scope = null)
        : this(jwt, null, scope)
    {
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="tokenType">The token type.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="accessToken">The access token.</param>
    /// <param name="expires">The expiration in second.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    public TokenInfo(string tokenType, string userId, string accessToken, int expires, string refreshToken = null, IEnumerable<string> scope = null)
        : this(tokenType, userId, accessToken, scope)
    {
        ExpiredInSecond = expires;
        RefreshToken = refreshToken;
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="tokenType">The token type.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="accessToken">The access token.</param>
    /// <param name="expires">The expiration time span.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    public TokenInfo(string tokenType, string userId, string accessToken, TimeSpan expires, string refreshToken = null, IEnumerable<string> scope = null)
        : this(tokenType, userId, accessToken, scope)
    {
        ExpiredAfter = expires;
        RefreshToken = refreshToken;
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="errorUri">The URI of error information.</param>
    /// <param name="errorDescription">The description of the error.</param>
    public TokenInfo(string errorCode, Uri errorUri, string errorDescription = null)
    {
        ErrorCode = errorCode;
        ErrorUri = errorUri;
        ErrorDescription = errorDescription;
    }

    /// <summary>
    /// Initializes a new instance of the TokenInfo class.
    /// </summary>
    /// <param name="tokenType">The token type.</param>
    /// <param name="accessToken">The access token.</param>
    /// <param name="expires">The expiration time span.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    protected TokenInfo(string tokenType, string accessToken, TimeSpan expires, string refreshToken = null, IEnumerable<string> scope = null)
    {
        TokenType = TokenType;
        AccessToken = accessToken;
        ExpiredAfter = expires;
        RefreshToken = refreshToken;
        Scope = ListExtensions.ToList(scope, false);
    }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    [DataMember(Name = AccessTokenProperty)]
    [JsonPropertyName(AccessTokenProperty)]
    public string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the token type.
    /// </summary>
    [DataMember(Name = TokenTypeProperty)]
    [JsonPropertyName(TokenTypeProperty)]
    public string TokenType { get; set; }

    /// <summary>
    /// Gets or sets the expiration seconds.
    /// </summary>
    [DataMember(Name = ExpiresInProperty, EmitDefaultValue = false)]
    [JsonPropertyName(ExpiresInProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ExpiredInSecond {
        get
        {
            var expiredAfter = ExpiredAfter;
            if (!expiredAfter.HasValue) return null;
            return (int)expiredAfter.Value.TotalSeconds;
        }

        set
        {
            if (value.HasValue) ExpiredAfter = TimeSpan.FromSeconds(value.Value);
            else ExpiredAfter = null;
        }
    }

    /// <summary>
    /// Gets or sets the expiration time span.
    /// </summary>
    [JsonIgnore]
    public TimeSpan? ExpiredAfter { get; set; }

    /// <summary>
    /// Gets or sets the refresh token used to re-generate an access token.
    /// </summary>
    [DataMember(Name = RefreshTokenProperty, EmitDefaultValue = false)]
    [JsonPropertyName(RefreshTokenProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    [DataMember(Name = ErrorCodeProperty, EmitDefaultValue = false)]
    [JsonPropertyName(ErrorCodeProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the error description.
    /// Human-readable text providing additional information,
    /// used to assist the client developer in understanding the error that occurred.
    /// </summary>
    [DataMember(Name = ErrorDescriptionProperty, EmitDefaultValue = false)]
    [JsonPropertyName(ErrorDescriptionProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorDescription { get; set; }

    /// <summary>
    /// Gets or sets the error URI in string.
    /// A URI identifying a human-readable web page with information about the error,
    /// used to provide the client developer with additional information about the error.
    /// </summary>
    [DataMember(Name = ErrorUriProperty, EmitDefaultValue = false)]
    [JsonPropertyName(ErrorUriProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorUrl
    {
        get
        {
            try
            {
                return ErrorUri?.OriginalString;
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        set
        {
            try
            {
                ErrorUri = !string.IsNullOrWhiteSpace(value) ? new Uri(value, UriKind.RelativeOrAbsolute) : null;
            }
            catch (FormatException)
            {
                ErrorUri = null;
            }
            catch (ArgumentException)
            {
                ErrorUri = null;
            }
            catch (NotSupportedException)
            {
                ErrorUri = null;
            }
            catch (InvalidCastException)
            {
                ErrorUri = null;
            }
            catch (InvalidDataContractException)
            {
                ErrorUri = null;
            }
            catch (InvalidOperationException)
            {
                ErrorUri = null;
            }
            catch (NullReferenceException)
            {
                ErrorUri = null;
            }
        }
    }

    /// <summary>
    /// Gets or sets the error URI.
    /// A URI identifying a human-readable web page with information about the error,
    /// used to provide the client developer with additional information about the error.
    /// </summary>
    [JsonIgnore]
    public Uri ErrorUri { get; set; }

    /// <summary>
    /// Gets or sets the state sent by client authorization request.
    /// </summary>
    [DataMember(Name = StateProperty, EmitDefaultValue = false)]
    [JsonPropertyName(StateProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual string State { get; set; }

    /// <summary>
    /// Gets or sets the resource identifier.
    /// </summary>
    [DataMember(Name = ResourceIdProperty, EmitDefaultValue = false)]
    [JsonPropertyName(ResourceIdProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual string ResourceId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    [DataMember(Name = UserIdProperty, EmitDefaultValue = false)]
    [JsonPropertyName(UserIdProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual string UserId { get; set; }

    /// <summary>
    /// Gets the permission scope.
    /// </summary>
    [JsonIgnore]
    public IList<string> Scope { get; private set; } = new List<string>();

    /// <summary>
    /// Gets or sets the scope string.
    /// </summary>
    [DataMember(Name = ScopeProperty, EmitDefaultValue = false)]
    [JsonPropertyName(ScopeProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ScopeString
    {
        get
        {
            return Scope != null && Scope.Count > 0 ? string.Join(" ", Scope) : null;
        }

        set
        {
            if (Scope == null) Scope = new List<string>();
            else Scope.Clear();
            if (string.IsNullOrWhiteSpace(value)) return;
            foreach (var ele in value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Scope.Add(ele);
            }
        }
    }

    /// <summary>
    /// Gets the additional data.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, string> AdditionalData { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets a value indicating whether the access token is null, empty or consists only of white-space characters.
    /// </summary>
    [JsonIgnore]
    public bool IsEmpty => string.IsNullOrWhiteSpace(AccessToken);

    /// <summary>
    /// Tries to get string value of additional data.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>A string value of additional data; or null, if no such property.</returns>
    public string TryGetAdditionalValue(string key)
        => AdditionalData.TryGetValue(key, out string resultStr) ? resultStr : null;

    /// <summary>
    /// Returns a System.String that represents the current TokenInfo.
    /// </summary>
    /// <returns>A System.String that represents the current TokenInfo.</returns>
    public override string ToString()
        => $"{TokenType} {AccessToken}".Trim();

    /// <summary>
    /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.
    /// </summary>
    /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.</returns>
    public AuthenticationHeaderValue ToAuthenticationHeaderValue()
        => ToAuthenticationHeaderValue(Cases.Original, Cases.Original);

    /// <summary>
    /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.
    /// </summary>
    /// <param name="schemeCase">The scheme case.</param>
    /// <param name="parameterCase">The parameter case.</param>
    /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.</returns>
    public AuthenticationHeaderValue ToAuthenticationHeaderValue(Cases schemeCase, Cases parameterCase = Cases.Original)
    {
        if (string.IsNullOrEmpty(TokenType)) return null;
        var scheme = StringExtensions.ToSpecificCaseInvariant(TokenType, schemeCase);
        return AccessToken == null ? new(scheme) : new(scheme, StringExtensions.ToSpecificCaseInvariant(AccessToken, parameterCase));
    }

    /// <summary>
    /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.
    /// </summary>
    /// <param name="scheme">The scheme to use for authorization.</param>
    /// <param name="parameterCase">The parameter case.</param>
    /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.</returns>
    public AuthenticationHeaderValue ToAuthenticationHeaderValue(string scheme, Cases parameterCase = Cases.Original)
        => AccessToken != null
            ? new AuthenticationHeaderValue(
                scheme ?? TokenType,
                StringExtensions.ToSpecificCaseInvariant(AccessToken, parameterCase))
            : new AuthenticationHeaderValue(scheme ?? TokenType);

    /// <summary>
    /// Converts to the authentication header value.
    /// </summary>
    /// <param name="token">The token info.</param>
    /// <returns>An instance of the authentication header value.</returns>
    public static explicit operator AuthenticationHeaderValue(TokenInfo token)
        => token?.ToAuthenticationHeaderValue();

    /// <summary>
    /// Converts to the token container.
    /// </summary>
    /// <param name="token">The token info.</param>
    /// <returns>An instance of the token container.</returns>
    public static explicit operator TokenContainer(TokenInfo token)
        => token is null ? null : new(token);

    /// <summary>
    /// Converts to the JSON string.
    /// </summary>
    /// <param name="token">The token info.</param>
    /// <returns>An instance of the JSON string.</returns>
    public static explicit operator JsonStringNode(TokenInfo token)
    {
        var s = token?.AccessToken?.Trim();
        if (string.IsNullOrWhiteSpace(s)) return null;
        return new($"{token.TokenType} {s}".TrimStart());
    }
}

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
    /// Adds or removes the event raised after token changed.
    /// </summary>
    public event ChangeEventHandler<TokenInfo> TokenChanged;

    /// <summary>
    /// Gets the token information instance saved in this container.
    /// </summary>
    [JsonIgnore]
    public TokenInfo Token
    {
        get
        {
            return token;
        }

        internal protected set
        {
            if (token == value) return;
            var oldValue = token;
            token = value;
            TokenChanged?.Invoke(this, new ChangeEventArgs<TokenInfo>(oldValue, value, nameof(Token), true));
        }
    }

    /// <summary>
    /// Gets or sets an additional tag object.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }

    /// <summary>
    /// Gets the access token saved in this container.
    /// </summary>
    [JsonIgnore]
    public string AccessToken => Token?.AccessToken;

    /// <summary>
    /// Gets a value indicating whether the access token is null, empty or consists only of white-space characters.
    /// </summary>
    [JsonIgnore]
    public bool IsTokenNullOrEmpty => Token?.IsEmpty ?? true;

    /// <summary>
    /// Gets or sets the case of authenticiation scheme.
    /// </summary>
    [JsonIgnore]
    public virtual Cases AuthenticationSchemeCase { get; set; } = Cases.Original;

    /// <summary>
    /// Gets or sets the converter of authentication header value.
    /// </summary>
    [JsonIgnore]
    public virtual Func<TokenInfo, Cases, AuthenticationHeaderValue> ConvertToAuthenticationHeaderValue { get; set; }

    /// <summary>
    /// Returns a System.String that represents the current token container instance.
    /// </summary>
    /// <returns>A System.String that represents the current token container instance.</returns>
    public override string ToString()
        => Token?.ToString() ?? string.Empty;

    /// <summary>
    /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.
    /// </summary>
    /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.</returns>
    public virtual AuthenticationHeaderValue ToAuthenticationHeaderValue()
        => Token?.ToAuthenticationHeaderValue(AuthenticationSchemeCase);

    /// <summary>
    /// Sets the headers with current authorization information.
    /// </summary>
    /// <param name="headers">The headers to fill.</param>
    /// <param name="forceToSet">true if force to set even if it has one; otherwise, false.</param>
    /// <returns>true if write succeeded; otherwise, false.</returns>
    public bool WriteAuthenticationHeaderValue(HttpRequestHeaders headers, bool forceToSet = false)
    {
        if (headers == null || Token == null || (headers.Authorization != null && !forceToSet)) return false;
        headers.Authorization = ConvertToAuthenticationHeaderValue != null
            ? ConvertToAuthenticationHeaderValue(Token, AuthenticationSchemeCase)
            : Token.ToAuthenticationHeaderValue(AuthenticationSchemeCase);
        return true;
    }

    /// <summary>
    /// Creates a JSON HTTP client.
    /// </summary>
    /// <typeparam name="T">The type of response.</typeparam>
    /// <param name="httpClient">The JSON HTTP client to set token.</param>
    /// <param name="callback">An optional callback raised on data received.</param>
    public void ApplyToken<T>(JsonHttpClient<T> httpClient, Action<ReceivedEventArgs<T>> callback = null)
    {
        if (httpClient == null) return;
        httpClient.Sending += OnSend;
        if (callback != null) httpClient.Received += (sender, ev) =>
        {
            callback(ev);
        };
    }

    /// <summary>
    /// Occurs on the JSON HTTP client is sending.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="ev">The event arguments.</param>
    protected virtual void OnSend(object sender, SendingEventArgs ev)
        => WriteAuthenticationHeaderValue(ev.RequestMessage.Headers);

    /// <summary>
    /// Converts to the token info.
    /// </summary>
    /// <param name="container">The token container.</param>
    /// <returns>An instance of the token info.</returns>
    public static explicit operator TokenInfo(TokenContainer container)
        => container?.Token;

    /// <summary>
    /// Converts to the authentication header value.
    /// </summary>
    /// <param name="container">The token container.</param>
    /// <returns>An instance of the authentication header value.</returns>
    public static explicit operator AuthenticationHeaderValue(TokenContainer container)
        => container?.ToAuthenticationHeaderValue();
}

/// <summary>
/// The access token response information with user model.
/// </summary>
/// <typeparam name="T">The type of user model.</typeparam>
public abstract class BaseAccountTokenInfo<T> : TokenInfo
{
    private T userCache;

    /// <summary>
    /// Initializes a new instance of the BaseAccountTokenInfo class.
    /// </summary>
    protected BaseAccountTokenInfo()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseAccountTokenInfo class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="errorUri">The URI of error information.</param>
    /// <param name="errorDescription">The description of the error.</param>
    protected BaseAccountTokenInfo(string errorCode, Uri errorUri, string errorDescription = null)
        : base(errorCode, errorUri, errorDescription)
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseAccountTokenInfo class.
    /// </summary>
    /// <param name="user">The user entity.</param>
    /// <param name="accessToken">The access token.</param>
    /// <param name="expires">The expiration time span.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    protected BaseAccountTokenInfo(T user, string accessToken, TimeSpan expires, string refreshToken = null, IEnumerable<string> scope = null)
        : base(BearerTokenType, accessToken, expires, refreshToken, scope)
    {
        User = user;
    }

    /// <summary>
    /// Initializes a new instance of the BaseAccountTokenInfo class.
    /// </summary>
    /// <param name="tokenType">The token type.</param>
    /// <param name="user">The user entity.</param>
    /// <param name="accessToken">The access token.</param>
    /// <param name="expires">The expiration time span.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    protected BaseAccountTokenInfo(string tokenType, T user, string accessToken, TimeSpan expires, string refreshToken = null, IEnumerable<string> scope = null)
        : base(tokenType, accessToken, expires, refreshToken, scope)
    {
        User = user;
    }

    /// <summary>
    /// Initializes a new instance of the BaseAccountTokenInfo class.
    /// </summary>
    /// <param name="user">The user entity.</param>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    protected BaseAccountTokenInfo(T user, JsonWebToken<JsonWebTokenPayload> jwt, string refreshToken, IEnumerable<string> scope = null)
        : base(jwt, refreshToken, scope)
    {
        User = user;
    }

    /// <summary>
    /// Initializes a new instance of the BaseAccountTokenInfo class.
    /// </summary>
    /// <param name="user">The user entity.</param>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="scope">The permission scope.</param>
    protected BaseAccountTokenInfo(T user, JsonWebToken<JsonWebTokenPayload> jwt, IEnumerable<string> scope = null)
        : base(jwt, scope)
    {
        User = user;
    }

    /// <summary>
    /// Initializes a new instance of the BaseAccountTokenInfo class.
    /// </summary>
    /// <param name="user">The user entity.</param>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    protected BaseAccountTokenInfo(T user, JsonWebToken<RSASecretExchange.JsonWebTokenPayload> jwt, string refreshToken, IEnumerable<string> scope = null)
        : base(jwt, refreshToken, scope)
    {
        User = user;
    }

    /// <summary>
    /// Initializes a new instance of the BaseAccountTokenInfo class.
    /// </summary>
    /// <param name="user">The user entity.</param>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="scope">The permission scope.</param>
    protected BaseAccountTokenInfo(T user, JsonWebToken<RSASecretExchange.JsonWebTokenPayload> jwt, IEnumerable<string> scope = null)
        : base(jwt, scope)
    {
        User = user;
    }

    /// <summary>
    /// Initializes a new instance of the BaseAccountTokenInfo class.
    /// </summary>
    /// <param name="user">The user entity.</param>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="scope">The permission scope.</param>
    protected BaseAccountTokenInfo(T user, JsonWebToken<JsonObjectNode> jwt, string refreshToken, IEnumerable<string> scope = null)
        : base(jwt, refreshToken, scope)
    {
        User = user;
    }

    /// <summary>
    /// Initializes a new instance of the BaseAccountTokenInfo class.
    /// </summary>
    /// <param name="user">The user entity.</param>
    /// <param name="jwt">The JSON web token used to generate access token.</param>
    /// <param name="scope">The permission scope.</param>
    protected BaseAccountTokenInfo(T user, JsonWebToken<JsonObjectNode> jwt, IEnumerable<string> scope = null)
        : base(jwt, scope)
    {
        User = user;
    }

    /// <summary>
    /// Gets or sets the current user entity.
    /// </summary>
    [DataMember(Name = "user", EmitDefaultValue = false)]
    [JsonPropertyName("user")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T User
    {
        get
        {
            return userCache;
        }

        set
        {
            var id = value is null ? null : GetUserId(value);
            userCache = value;
            base.UserId = id;
        }
    }

    /// <summary>
    /// Gets or sets the user identifer.
    /// </summary>
    public override string UserId
    {
        get
        {
            return base.UserId;
        }

        set
        {
            if (value == base.UserId) return;
            userCache = default;
            base.UserId = value;
        }
    }

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    [DataMember(Name = TokenRequestBody.ClientIdProperty, EmitDefaultValue = true)]
    [JsonPropertyName(TokenRequestBody.ClientIdProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets the user identifier from the entity.
    /// </summary>
    /// <param name="user">The user entity to get the identifier.</param>
    /// <returns>The user identifier.</returns>
    protected abstract string GetUserId(T user);

    /// <summary>
    /// Sets a new user identifier without validating user model.
    /// </summary>
    /// <param name="id">The new user identifier without validation.</param>
    protected void ForceUpdateUserId(string id)
        => base.UserId = id;

    /// <summary>
    /// Converts to the token info and user entity.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>An instance of the relationship.</returns>
    public static explicit operator SelectionRelationship<T, TokenInfo>(BaseAccountTokenInfo<T> token)
        => token is null ? new() : new(token.User, token);
}
