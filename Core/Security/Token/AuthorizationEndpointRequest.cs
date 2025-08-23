using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Trivial.Net;
using Trivial.Text;

namespace Trivial.Security;

/// <summary>
/// The parameters to generate authorization URI.
/// </summary>
[DataContract]
[Guid("69BF4CCB-4F52-441E-9ED4-77EFE7B4A9CF")]
public class AuthorizationEndpointRequest
{
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    [DataMember(Name = "client_id", EmitDefaultValue = false)]
    [JsonPropertyName("client_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The client identifier.")]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the response responseTypes, e.g. code, token, id_token.
    /// </summary>
    [DataMember(Name = CodeTokenRequestBody.ResponseType, EmitDefaultValue = false)]
    [JsonPropertyName(CodeTokenRequestBody.ResponseType)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The response types, e.g. code, token, id_token.")]
    public List<string> ResponseTypes { get; set; }

    /// <summary>
    /// Gets or sets the redirect URI where authentication responses can be sent and received by client.
    /// </summary>
    [DataMember(Name = CodeTokenRequestBody.RedirectUriProperty, EmitDefaultValue = false)]
    [JsonPropertyName("redirect_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The redirect URI where authentication responses can be sent and received by client.")]
    public Uri RedirectUri { get; set; }

    /// <summary>
    /// Gets or sets the collection of scopes.
    /// </summary>
    [DataMember(Name = TokenInfo.ScopeProperty, EmitDefaultValue = false)]
    [JsonPropertyName(TokenInfo.ScopeProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The scopes.")]
    public List<string> Scopes { get; set; }

    /// <summary>
    /// Gets or sets the value generated and sent by client in its request for an ID token. The same nonce value is included in the ID token returned to client by the authorization endpoint.
    /// </summary>
    [DataMember(Name = "nonce", EmitDefaultValue = false)]
    [JsonPropertyName("nonce")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A value generated and sent by client in its request for an ID token. The same nonce value is included in the ID token returned to client by the authorization endpoint.")]
    public string Nonce { get; set; }

    /// <summary>
    /// Gets or sets the method (e.g. form_post) that should be used to send the resulting authorization code back to client.
    /// </summary>
    [DataMember(Name = "response_mode", EmitDefaultValue = false)]
    [JsonPropertyName("response_mode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The method (e.g. form_post) that should be used to send the resulting authorization code back to client.")]
    public string ResponseMode { get; set; }

    /// <summary>
    /// Gets or sets the value included in the request that is also returned in the token response. It can be a string of any content.
    /// </summary>
    [DataMember(Name = CodeTokenRequestBody.StateProperty, EmitDefaultValue = false)]
    [JsonPropertyName(CodeTokenRequestBody.StateProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The value included in the request that is also returned in the token response.")]
    public string State { get; set; }

    /// <summary>
    /// Gets or sets the type of user interaction that is required, e.g. login, none, consent, select_account..
    /// </summary>
    [DataMember(Name = "prompt", EmitDefaultValue = false)]
    [JsonPropertyName("prompt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The type of user interaction that is required.")]
    public string Prompt { get; set; }

    /// <summary>
    /// Gets or sets the username or email address to prefill the field of the sign-in page for the user.
    /// </summary>
    [DataMember(Name = "login_hint", EmitDefaultValue = false)]
    [JsonPropertyName("login_hint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The username or email address to prefill the field of the sign-in page for the user.")]
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the realm of the user in a federated directory. This skips the email-based discovery process that the user goes through on the sign-in page, for a slightly more streamlined user experience.
    /// </summary>
    [DataMember(Name = "domain_hint", EmitDefaultValue = false)]
    [JsonPropertyName("domain_hint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The realm of the user in a federated directory.")]
    public string DomainHint { get; set; }

    /// <summary>
    /// Adds the item of response type.
    /// </summary>
    /// <param name="type">The response type.</param>
    public void AddResponseType(string type)
    {
        var col = ResponseTypes;
        if (col is null)
        {
            col = new();
            if (ResponseTypes is null)
            {
                ResponseTypes = col;
            }
            else
            {
                var col2 = ResponseTypes;
                if (col2 is null) ResponseTypes = col;
                else col = col2;
            }
        }

        if (!string.IsNullOrWhiteSpace(type) && !col.Contains(type)) col.Add(type);
    }

    /// <summary>
    /// Adds the item of response type: token.
    /// </summary>
    public void AddResponseTypeToken()
        => AddResponseType(TokenInfo.TokenKey);

    /// <summary>
    /// Adds the item of response type: id_token.
    /// </summary>
    public void AddResponseTypeIdToken()
        => AddResponseType(TokenInfo.IdTokenProperty);

    /// <summary>
    /// Adds the item of response type: code.
    /// </summary>
    public void AddResponseTypeCode()
        => AddResponseType(CodeTokenRequestBody.CodeProperty);

    /// <summary>
    /// Adds the item of scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    public void AddScope(string scope)
    {
        var col = Scopes;
        if (col is null)
        {
            col = new();
            if (Scopes is null)
            {
                Scopes = col;
            }
            else
            {
                var col2 = Scopes;
                if (col2 is null) Scopes = col;
                else col = col2;
            }
        }

        if (!string.IsNullOrWhiteSpace(scope) && !col.Contains(scope)) col.Add(scope);
    }

    /// <summary>
    /// Gets the query string of the value.
    /// </summary>
    /// <returns>The query format string.</returns>
    public override string ToString()
        => ToQueryData().ToString();

    /// <summary>
    /// Gets the query string of the value.
    /// </summary>
    /// <param name="encoding">The encoding.</param>
    /// <returns>The query format string.</returns>
    public string ToString(Encoding encoding = null)
        => ToQueryData().ToString(encoding);

    /// <summary>
    /// Gets the query string of the value.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>The query format string.</returns>
    public string ToString(Uri uri, Encoding encoding = null)
        => ToQueryData().ToString(uri, encoding);

    /// <summary>
    /// Gets the query string of the value.
    /// </summary>
    /// <param name="metadata">The Authorization Server Metadata.</param>
    /// <returns>The query format string.</returns>
    public string ToString(AuthorizationServerMetadataResponse metadata)
        => ToQueryData().ToString(metadata?.AuthorizationEndpoint);

    /// <summary>
    /// Returns a string HTTP request content.
    /// </summary>
    /// <param name="encoding">The optional encoding to use for the content. Or null for default.</param>
    /// <returns>A string HTTP request content.</returns>
    public StringContent ToStringContent(Encoding encoding = null)
        => ToQueryData().ToStringContent(encoding);

    /// <summary>
    /// Converts to query data.
    /// </summary>
    /// <returns>The query data.</returns>
    protected virtual QueryData ToQueryData()
    {
        var q = new QueryData();
        q.SetIfNotEmpty("client_id", ClientId);
        q.SetIfNotEmpty(CodeTokenRequestBody.ResponseType, StringExtensions.Join(' ', ResponseTypes));
        q.SetIfNotEmpty(CodeTokenRequestBody.RedirectUriProperty, RedirectUri?.OriginalString);
        q.SetIfNotEmpty(TokenInfo.ScopeProperty, StringExtensions.Join(' ', Scopes));
        q.SetIfNotEmpty("nonce", Nonce);
        q.SetIfNotEmpty("response_mode", ResponseMode);
        q.SetIfNotEmpty(CodeTokenRequestBody.StateProperty, State);
        q.SetIfNotEmpty("prompt", Prompt);
        q.SetIfNotEmpty("login_hint", UserName);
        q.SetIfNotEmpty("domain_hint", DomainHint);
        return q;
    }

    /// <summary>
    /// Converts to query data.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The query data.</returns>
    public static explicit operator QueryData(AuthorizationEndpointRequest value)
        => value?.ToQueryData();
}

/// <summary>
/// The parameters to generate authorization URI.
/// </summary>
[DataContract]
[Guid("E75455F7-0A6E-4804-A728-581EC42449A7")]
public class AuthorizationRedirectRequest
{
    /// <summary>
    /// Initializes a new instance of the AuthorizationRedirectRequest class.
    /// </summary>
    public AuthorizationRedirectRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the AuthorizationRedirectRequest class.
    /// </summary>
    /// <param name="request">The authorization endpoint request.</param>
    public AuthorizationRedirectRequest(AuthorizationEndpointRequest request)
    {
        if (request is null) return;
        if (request.Scopes is not null) Scopes = new(request.Scopes);
        Nonce = request.Nonce;
        State = request.State;
    }

    /// <summary>
    /// Initializes a new instance of the AuthorizationRedirectRequest class.
    /// </summary>
    /// <param name="request">The authorization endpoint request.</param>
    /// <param name="token">The token information.</param>
    public AuthorizationRedirectRequest(AuthorizationEndpointRequest request, TokenInfo token)
        : this(request)
    {
        if (token is null) return;
        TokenType = token.TokenType;
        var types = request?.ResponseTypes;
        SetToken(token, request?.ResponseTypes);
    }

    /// <summary>
    /// Initializes a new instance of the AuthorizationRedirectRequest class.
    /// </summary>
    /// <param name="request">The authorization endpoint request.</param>
    /// <param name="code">The code.</param>
    public AuthorizationRedirectRequest(AuthorizationEndpointRequest request, string code)
        : this(request)
    {
        Code = code;
    }

    /// <summary>
    /// Gets or sets the authorization code.
    /// </summary>
    [DataMember(Name = CodeTokenRequestBody.CodeProperty, EmitDefaultValue = false)]
    [JsonPropertyName(CodeTokenRequestBody.CodeProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The authorization code.")]
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    [DataMember(Name = TokenInfo.AccessTokenProperty, EmitDefaultValue = false)]
    [JsonPropertyName(TokenInfo.AccessTokenProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The access token.")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    [DataMember(Name = TokenInfo.RefreshTokenProperty, EmitDefaultValue = false)]
    [JsonPropertyName(TokenInfo.RefreshTokenProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The refresh token.")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the token type.
    /// </summary>
    [DataMember(Name = TokenInfo.TokenTypeProperty, EmitDefaultValue = false)]
    [JsonPropertyName(TokenInfo.TokenTypeProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The token type.")]
    public string TokenType { get; set; }

    /// <summary>
    /// Gets or sets the token expiration seconds.
    /// </summary>
    [DataMember(Name = TokenInfo.ExpiresInProperty, EmitDefaultValue = false)]
    [JsonPropertyName(TokenInfo.ExpiresInProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The expiration seconds..")]
    public int? ExpiredInSecond
    {
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
    /// Gets or sets the token expiration time span.
    /// </summary>
    [JsonIgnore]
    public TimeSpan? ExpiredAfter { get; set; }

    /// <summary>
    /// Gets or sets the collection of scopes.
    /// </summary>
    [DataMember(Name = TokenInfo.ScopeProperty, EmitDefaultValue = false)]
    [JsonPropertyName(TokenInfo.ScopeProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The scope.")]
    public List<string> Scopes { get; set; }

    /// <summary>
    /// Gets or sets the ID token.
    /// </summary>
    [DataMember(Name = TokenInfo.IdTokenProperty, EmitDefaultValue = false)]
    [JsonPropertyName(TokenInfo.IdTokenProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The ID token.")]
    public string IdToken { get; set; }

    /// <summary>
    /// Gets or sets the value which is from the request of authorization. It can be a string of any content.
    /// </summary>
    [DataMember(Name = CodeTokenRequestBody.StateProperty, EmitDefaultValue = false)]
    [JsonPropertyName(CodeTokenRequestBody.StateProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The value which is from the request of authorization.")]
    public string State { get; set; }

    /// <summary>
    /// Gets or sets the value generated and sent by client in its request for an ID token. The same nonce value is included in the ID token returned to client by the authorization endpoint.
    /// </summary>
    [DataMember(Name = "nonce", EmitDefaultValue = false)]
    [JsonPropertyName("nonce")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A value generated and sent by client in its request for an ID token. The same nonce value is included in the ID token returned to client by the authorization endpoint.")]
    public string Nonce { get; set; }

    /// <summary>
    /// Sets the access token, refresh token and ID token.
    /// These tokens and the token type will be overrided by the specific ones.
    /// </summary>
    /// <param name="token">The token information.</param>
    public void SetToken(TokenInfo token)
    {
        if (token is null) return;
        TokenType = token.TokenType;
        ExpiredAfter = token.ExpiredAfter;
        AccessToken = token.AccessToken;
        RefreshToken = token.RefreshToken;
        IdToken = token.IdToken;
    }

    /// <summary>
    /// Sets the access token, refresh token and ID token.
    /// These tokens and the token type will be overrided by the specific ones.
    /// </summary>
    /// <param name="token">The token information.</param>
    /// <param name="responseTypes">The response types from authorization request.</param>
    public void SetToken(TokenInfo token, ICollection<string> responseTypes)
    {
        if (token is null || responseTypes is null) return;
        var has = false;
        if (responseTypes.Contains("token"))
        {
            AccessToken = token.AccessToken;
            RefreshToken = token.RefreshToken;
            has = true;
        }

        if (responseTypes.Contains(TokenInfo.IdTokenProperty))
        {
            IdToken = token.IdToken;
            has = true;
        }

        if (!has) return;
        ExpiredAfter = token.ExpiredAfter;
        TokenType = token.TokenType;
    }

    /// <summary>
    /// Sets the access token.
    /// </summary>
    /// <param name="value">The access token.</param>
    public void SetAccessToken(string value)
    {
        TokenType = TokenInfo.BearerTokenType;
        AccessToken = value;
    }

    /// <summary>
    /// Sets the access token.
    /// </summary>
    /// <typeparam name="T">The type of JSON web token payload.</typeparam>
    /// <param name="value">The access token.</param>
    public void SetAccessToken<T>(JsonWebToken<T> value)
        => SetAccessToken(value?.ToEncodedString());

    /// <summary>
    /// Sets the access token.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="sign">The signature provider.</param>
    public void SetAccessToken(JsonWebTokenPayload payload, ISignatureProvider sign)
        => SetAccessToken(payload is null ? null : new JsonWebToken<JsonWebTokenPayload>(payload, sign).ToEncodedString());

    /// <summary>
    /// Sets the access token.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="sign">The signature provider.</param>
    public void SetAccessToken(JsonObjectNode payload, ISignatureProvider sign)
        => SetAccessToken(payload is null ? null : new JsonWebToken<JsonObjectNode>(payload, sign).ToEncodedString());

    /// <summary>
    /// Sets the ID token.
    /// </summary>
    /// <param name="value">The ID token.</param>
    public void SetIdToken(string value)
    {
        TokenType = TokenInfo.BearerTokenType;
        IdToken = value;
    }

    /// <summary>
    /// Sets the ID token.
    /// </summary>
    /// <typeparam name="T">The type of JSON web token payload.</typeparam>
    /// <param name="value">The ID token.</param>
    public void SetIdToken<T>(JsonWebToken<T> value)
        => SetIdToken(value?.ToEncodedString());

    /// <summary>
    /// Sets the ID token.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="sign">The signature provider.</param>
    public void SetIdToken(JsonWebTokenPayload payload, ISignatureProvider sign)
        => SetIdToken(payload is null ? null : new JsonWebToken<JsonWebTokenPayload>(payload, sign).ToEncodedString());

    /// <summary>
    /// Sets the ID token.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="sign">The signature provider.</param>
    public void SetIdToken(JsonObjectNode payload, ISignatureProvider sign)
        => SetIdToken(IdToken = payload is null ? null : new JsonWebToken<JsonObjectNode>(payload, sign).ToEncodedString());

    /// <summary>
    /// Adds the item of scope.
    /// </summary>
    /// <param name="scope">The scope.</param>
    public void AddScope(string scope)
    {
        var col = Scopes;
        if (col is null)
        {
            col = new();
            if (Scopes is null)
            {
                Scopes = col;
            }
            else
            {
                var col2 = Scopes;
                if (col2 is null) Scopes = col;
                else col = col2;
            }
        }

        if (!string.IsNullOrWhiteSpace(scope) && !col.Contains(scope)) col.Add(scope);
    }

    /// <summary>
    /// Gets the query string of the value.
    /// </summary>
    /// <returns>The query format string.</returns>
    public override string ToString()
        => ToQueryData().ToString();

    /// <summary>
    /// Gets the query string of the value.
    /// </summary>
    /// <param name="encoding">The encoding.</param>
    /// <returns>The query format string.</returns>
    public string ToString(Encoding encoding = null)
        => ToQueryData().ToString(encoding);

    /// <summary>
    /// Gets the query string of the value.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>The query format string.</returns>
    public string ToString(Uri uri, Encoding encoding = null)
        => ToQueryData().ToString(uri, encoding);

    /// <summary>
    /// Gets the query string of the value.
    /// </summary>
    /// <param name="metadata">The Authorization Server Metadata.</param>
    /// <returns>The query format string.</returns>
    public string ToString(AuthorizationServerMetadataResponse metadata)
        => ToQueryData().ToString(metadata?.AuthorizationEndpoint);

    /// <summary>
    /// Returns a string HTTP request content.
    /// </summary>
    /// <param name="encoding">The optional encoding to use for the content. Or null for default.</param>
    /// <returns>A string HTTP request content.</returns>
    public StringContent ToStringContent(Encoding encoding = null)
        => ToQueryData().ToStringContent(encoding);

    /// <summary>
    /// Converts to query data.
    /// </summary>
    /// <returns>The query data.</returns>
    protected virtual QueryData ToQueryData()
    {
        var q = new QueryData();
        q.SetIfNotEmpty(CodeTokenRequestBody.CodeProperty, Code);
        q.SetIfNotEmpty(TokenInfo.AccessTokenProperty, AccessToken);
        q.SetIfNotEmpty(TokenInfo.RefreshTokenProperty, RefreshToken);
        q.SetIfNotEmpty(TokenInfo.TokenTypeProperty, TokenType);
        if (ExpiredInSecond.HasValue) q.Add(TokenInfo.ExpiresInProperty, ExpiredInSecond.Value);
        q.SetIfNotEmpty(TokenInfo.ScopeProperty, StringExtensions.Join(' ', Scopes));
        q.SetIfNotEmpty(TokenInfo.IdTokenProperty, IdToken);
        q.SetIfNotEmpty(CodeTokenRequestBody.StateProperty, State);
        q.SetIfNotEmpty("nonce", Nonce);
        return q;
    }

    /// <summary>
    /// Converts to query data.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The query data.</returns>
    public static explicit operator QueryData(AuthorizationRedirectRequest value)
        => value?.ToQueryData();
}
