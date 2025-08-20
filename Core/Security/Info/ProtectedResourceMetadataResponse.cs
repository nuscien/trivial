using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Net;
using Trivial.Text;

namespace Trivial.Security;

/// <summary>
/// The base response model of OAuth 2.0 Protected Resource Metadata (RFC-9728).
/// </summary>
[DataContract]
public class ProtectedResourceMetadataResponse
{
    /// <summary>
    /// Gets or sets the identifier of the protected resource, which is a URL that uses the https scheme and has no fragment component.
    /// </summary>
    [DataMember(Name = "resource", EmitDefaultValue = false)]
    [JsonPropertyName("resource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The identifier of the protected resource, which is a URL that uses the https scheme and has no fragment component.")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets a list of OAuth authorization server issuer identifiers, for authorization servers that can be used with this protected resource.
    /// </summary>
    [DataMember(Name = "authorization_servers", EmitDefaultValue = false)]
    [JsonPropertyName("authorization_servers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of OAuth authorization server issuer identifiers, for authorization servers that can be used with this protected resource.")]
    public List<string> AuthorizationServers { get; set; }

    /// <summary>
    /// Gets or sets the document URL of the JSON Web Key Set.
    /// This contains public keys belonging to the protected resource, such as signing key(s) that the resource server uses to sign resource responses.
    /// </summary>
    [DataMember(Name = "jwks_uri", EmitDefaultValue = false)]
    [JsonPropertyName("jwks_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The document URL of the JSON Web Key Set.")]
    public string JsonWebKeysUrl { get; set; }

    /// <summary>
    /// Gets or sets a list of scope values, as defined in OAuth 2.0, that are used in authorization requests to request access to this protected resource.
    /// </summary>
    [DataMember(Name = "scopes_supported", EmitDefaultValue = false)]
    [JsonPropertyName("scopes_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of scope values used in authorization requests to request access to this protected resource.")]
    public List<string> Scopes { get; set; }

    /// <summary>
    /// Gets or sets a list of the supported methods of sending an OAuth 2.0 bearer token to the protected resource.
    /// Defined values are "header", "body" and "query".
    /// </summary>
    [DataMember(Name = "bearer_methods_supported", EmitDefaultValue = false)]
    [JsonPropertyName("bearer_methods_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of the supported methods of sending an OAuth 2.0 bearer token to the protected resource. Defined values are header, body and query.")]
    public List<string> BearerMethods { get; set; }

    /// <summary>
    /// Gets or sets a list of the JSON Web Signature and Encryption Algorithms supported by the protected resource for signing resource responses.
    /// </summary>
    [DataMember(Name = "resource_signing_alg_values_supported", EmitDefaultValue = false)]
    [JsonPropertyName("resource_signing_alg_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of the JSON Web Signature and Encryption Algorithms supported by the protected resource for signing resource responses.")]
    public List<string> SigningAlgorithms { get; set; }

    /// <summary>
    /// Gets or sets human-readable name of the protected resource intended for display to the end user.
    /// </summary>
    [DataMember(Name = "resource_name", EmitDefaultValue = false)]
    [JsonPropertyName("resource_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("Human-readable name of the protected resource intended for display to the end user.")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the URL of a page containing human-readable information that developers might want or need to know when using the protected resource.
    /// </summary>
    [DataMember(Name = "resource_documentation", EmitDefaultValue = false)]
    [JsonPropertyName("resource_documentation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The URL of a page containing human-readable information that developers might want or need to know when using the protected resource.")]
    public string DocumentationUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL of a page containing human-readable information about the protected resource's requirements on how the client can use the data provided by the protected resource.
    /// </summary>
    [DataMember(Name = "resource_policy_uri", EmitDefaultValue = false)]
    [JsonPropertyName("resource_policy_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The URL of a page containing human-readable information about the protected resource's requirements on how the client can use the data provided by the protected resource.")]
    public string PolicyUrL { get; set; }

    /// <summary>
    /// Gets or sets the URL of a page containing human-readable information about the protected resource's terms of service.
    /// </summary>
    [DataMember(Name = "resource_tos_uri", EmitDefaultValue = false)]
    [JsonPropertyName("resource_tos_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The URL of a page containing human-readable information about the protected resource's terms of service.")]
    public string TermsOfServiceUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating protected resource support for mutual-TLS client certificate-bound access tokens.
    /// If omitted, the default value is false.
    /// </summary>
    [DataMember(Name = "tls_client_certificate_bound_access_tokens", EmitDefaultValue = false)]
    [JsonPropertyName("tls_client_certificate_bound_access_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A value indicating protected resource support for mutual-TLS client certificate-bound access tokens.")]
    public bool TlsClientCertificateBoundAccessTokens { get; set; }

    /// <summary>
    /// Gets or sets a list of the authorization details type values supported by the resource server when the authorization_details request parameter is used.
    /// </summary>
    [DataMember(Name = "authorization_details_types_supported", EmitDefaultValue = false)]
    [JsonPropertyName("authorization_details_types_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of the authorization details type values supported by the resource server when the authorization_details request parameter is used.")]
    public List<string> AuthorizationDetailsTypes { get; set; }

    /// <summary>
    /// Gets or sets a list of the JSON Web Signature and Encryption Algorithms supported by the resource server for validating Demonstrating Proof of Possession proof JSON Web Tokens.
    /// </summary>
    [DataMember(Name = "dpop_signing_alg_values_supported", EmitDefaultValue = false)]
    [JsonPropertyName("dpop_signing_alg_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description(".")]
    public List<string> DPoPSigningAlgorithms { get; set; }

    /// <summary>
    /// Gets or sets a value specifying whether the protected resource always requires the use of DPoP-bound access tokens.
    /// If omitted, the default value is false.
    /// </summary>
    [DataMember(Name = "dpop_bound_access_tokens_required", EmitDefaultValue = false)]
    [JsonPropertyName("dpop_bound_access_tokens_required")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A value specifying whether the protected resource always requires the use of DPoP-bound access tokens.")]
    public bool DPoPBoundAccessTokensRequired { get; set; }

    /// <summary>
    /// Gets or sets a list about the formats of token supported, e.g. jwt.
    /// </summary>
    [DataMember(Name = "token_formats_supported", EmitDefaultValue = false)]
    [JsonPropertyName("token_formats_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list about the formats of token supported, e.g. jwt.")]
    public List<string> TokenFormats { get; set; }

    /// <summary>
    /// Gets or sets the endpoint URL for token validation.
    /// </summary>
    [DataMember(Name = "token_introspection_endpoint", EmitDefaultValue = false)]
    [JsonPropertyName("token_introspection_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The endpoint URL for token validation.")]
    public string TokenIntrospectionEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the JSON Web Token that asserts metadata values about the protected resource as a bundle.
    /// </summary>
    [DataMember(Name = "signed_metadata", EmitDefaultValue = false)]
    [JsonPropertyName("signed_metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A JSON Web Token that asserts metadata values about the protected resource as a bundle.")]
    public string Signed { get; set; }

    /// <summary>
    /// Adds a bearer method "header".
    /// </summary>
    public void AddBearerMethodHeader()
        => AddBearerMethod("header");

    /// <summary>
    /// Adds a bearer method "body".
    /// </summary>
    public void AddBearerMethodBody()
        => AddBearerMethod("body");

    /// <summary>
    /// Adds a bearer method "query".
    /// </summary>
    public void AddBearerMethodQuery()
        => AddBearerMethod("query");

    /// <summary>
    /// Adds a bearer method supported.
    /// </summary>
    /// <param name="value">The method to add.</param>
    public void AddBearerMethod(string value)
    {
        var col = BearerMethods;
        if (col is null)
        {
            col = new();
            if (BearerMethods is null)
            {
                BearerMethods = col;
            }
            else
            {
                var col2 = BearerMethods;
                if (col2 is null) BearerMethods = col;
            }
        }

        if (!string.IsNullOrWhiteSpace(value) && !col.Contains(value)) col.Add(value);
    }

    /// <summary>
    /// Adds the JSON Web Signature and Encryption Algorithm supported by the resource server for validating Demonstrating Proof of Possession proof JSON Web Tokens.
    /// </summary>
    /// <param name="algorithmName">The name of the algorithm to add.</param>
    public void AddDPoPSigningAlgorithm(string algorithmName)
    {
        var col = DPoPSigningAlgorithms;
        if (col is null)
        {
            col = new();
            if (DPoPSigningAlgorithms is null)
            {
                DPoPSigningAlgorithms = col;
            }
            else
            {
                var col2 = DPoPSigningAlgorithms;
                if (col2 is null) DPoPSigningAlgorithms = col;
            }
        }

        if (!string.IsNullOrWhiteSpace(algorithmName) && !col.Contains(algorithmName)) col.Add(algorithmName);
    }

    /// <summary>
    /// Adds the JSON Web Signature and Encryption Algorithm supported by the protected resource for signing resource responses.
    /// </summary>
    /// <param name="algorithmName">The name of the algorithm to add.</param>
    public void AddSigningAlgorithm(string algorithmName)
    {
        var col = SigningAlgorithms;
        if (col is null)
        {
            col = new();
            if (SigningAlgorithms is null)
            {
                SigningAlgorithms = col;
            }
            else
            {
                var col2 = SigningAlgorithms;
                if (col2 is null) SigningAlgorithms = col;
            }
        }

        if (!string.IsNullOrWhiteSpace(algorithmName) && !col.Contains(algorithmName)) col.Add(algorithmName);
    }

    /// <summary>
    /// Adds JSON Web Token to the formats.
    /// </summary>
    public void AddJwtFormat()
    {
        var col = TokenFormats;
        if (col is null)
        {
            col = new();
            if (TokenFormats is null)
            {
                TokenFormats = col;
            }
            else
            {
                var col2 = TokenFormats;
                if (col2 is null) TokenFormats = col;
            }
        }

        if (!col.Contains("jwt")) col.Add("jwt");
    }

    /// <summary>
    /// Sets the signed metedata.
    /// </summary>
    /// <param name="jwt">The JSON Web Token with the metadata.</param>
    public void SetSignedMetadata<T>(JsonWebToken<T> jwt)
        => Signed = jwt?.ToEncodedString();

    /// <summary>
    /// Sets the signed metedata.
    /// </summary>
    /// <param name="issuer">The issuer.</param>
    /// <param name="sign">The signature provider.</param>
    /// <returns>The JSON Web Token.</returns>
    public JsonWebToken<JsonObjectNode> SetSignedMetadata(string issuer, ISignatureProvider sign)
    {
        var json = JsonObjectNode.ConvertFrom(this);
        json.Remove("signed_metadata");
        json.SetValueIfNotEmpty("iss", issuer);
        var jwt = new JsonWebToken<JsonObjectNode>(json, sign);
        SetSignedMetadata(jwt);
        return jwt;
    }

    /// <summary>
    /// Sets the signed metedata.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="sign">The signature provider.</param>
    public void SetSignedMetadata<T>(T payload, ISignatureProvider sign)
        => SetSignedMetadata(new JsonWebToken<T>(payload, sign));
}

/// <summary>
/// The link and other information of protected resource metadata.
/// </summary>
public class ProtectedResourceMetadataLink
{
    /// <summary>
    /// Initializes a new instance of the ProtectedResourceMetadataLink class.
    /// </summary>
    public ProtectedResourceMetadataLink()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ProtectedResourceMetadataLink class.
    /// </summary>
    /// <param name="url">The metadata request URL.</param>
    /// <param name="realm">The realm of the resource.</param>
    public ProtectedResourceMetadataLink(string url, string realm = null)
    {
        Url = url;
        Realm = realm;
    }

    /// <summary>
    /// Initializes a new instance of the ProtectedResourceMetadataLink class.
    /// </summary>
    /// <param name="uri">The metadata request URI.</param>
    /// <param name="realm">The realm of the resource.</param>
    public ProtectedResourceMetadataLink(Uri uri, string realm = null)
    {
        Uri = uri;
        Realm = realm;
    }

    /// <summary>
    /// Gets or sets the realm of the resource.
    /// </summary>
    [DataMember(Name = "realm", EmitDefaultValue = false)]
    [JsonPropertyName("realm")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The realm of the resource.")]
    public string Realm { get; set; }

    /// <summary>
    /// Gets or sets the URL of the protected resource metadata.
    /// </summary>
    [DataMember(Name = "resource_metadata_url", EmitDefaultValue = false)]
    [JsonPropertyName("token_formats_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The URL of the protected resource metadata.")]
    public string Url
    {
        get => Uri?.OriginalString;
        set => Uri = StringExtensions.TryCreateUri(value);
    }

    /// <summary>
    /// Gets or sets the URI of the protected resource metadata.
    /// </summary>
    [JsonIgnore]
    public Uri Uri { get; set; }

    /// <summary>
    /// Gets or sets a list of scope values, as defined in OAuth 2.0, that are used in authorization requests to request access to this protected resource.
    /// </summary>
    [DataMember(Name = "scope", EmitDefaultValue = false)]
    [JsonPropertyName("scope")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of scope values used in authorization requests to request access to this protected resource.")]
    public List<string> Scopes { get; set; }

    /// <summary>
    /// Gets the metadata.
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The protected resource metadata.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<ProtectedResourceMetadataResponse> GetMetadataAsync(IJsonHttpClientMaker client, CancellationToken cancellationToken = default)
    {
        var http = client?.Create<ProtectedResourceMetadataResponse>();
        return GetMetadataAsync(http, cancellationToken);
    }

    /// <summary>
    /// Gets the metadata.
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The protected resource metadata.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public Task<ProtectedResourceMetadataResponse> GetMetadataAsync(JsonHttpClient<ProtectedResourceMetadataResponse> client, CancellationToken cancellationToken = default)
        => (client ?? new()).GetAsync(Uri, cancellationToken);

    /// <summary>
    /// Gets the metadata.
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The protected resource metadata.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">The task is cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The inner HTTP web client instance has been disposed.</exception>
    public async Task<ProtectedResourceMetadataResponse> GetMetadataAsync(JsonHttpClient<JsonObjectNode> client, CancellationToken cancellationToken = default)
    {
        if (client == null) return await GetMetadataAsync(new JsonHttpClient<ProtectedResourceMetadataResponse>(), cancellationToken);
        var json = await client.GetAsync(Uri, cancellationToken);
        return json.Deserialize<ProtectedResourceMetadataResponse>();
    }

    /// <summary>
    /// Returns a string in WWW-Authenticate that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(TokenInfo.BearerTokenType);
        sb.Append(' ');
        var count = sb.Length;
        FillParameter(sb);
        return count != sb.Length ? sb.ToString() : string.Empty;
    }

    /// <summary>
    /// Returns a string in WWW-Authenticate that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    protected virtual void FillParameter(StringBuilder sb)
    {
        AppendComponent(sb, "realm", Realm);
        var scope = Scopes;
        if (scope != null) AppendComponent(sb, "scope", string.Join(" ", scope.Where(ele => !string.IsNullOrWhiteSpace(ele))));
        AppendComponent(sb, "resource_metadata_url", Url);
    }

    /// <summary>
    /// Converts to the authentication header value.
    /// </summary>
    /// <returns>The authentication header value.</returns>
    public AuthenticationHeaderValue ToAuthenticationHeader()
    {
        var sb = new StringBuilder();
        FillParameter(sb);
        return new(TokenInfo.BearerTokenType, sb.ToString());
    }

    /// <summary>
    /// Converts to the authentication header value.
    /// </summary>
    /// <returns>The authentication header value.</returns>
    public AuthenticationHeaderValue ToAuthenticationHeader(HttpResponseHeaders header)
    {
        var v = ToAuthenticationHeader();
        header.WwwAuthenticate.Add(v);
        return v;
    }

    /// <summary>
    /// Tries to parse the authentication header value of protected resource metadata.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The request info of protected resource metadata.</returns>
    public static ProtectedResourceMetadataLink TryParse(string s)
    {
        s = s?.Trim();
        if (string.IsNullOrEmpty(s)) return null;
        if (s.StartsWith('{')) return JsonSerializer.Deserialize<ProtectedResourceMetadataLink>(s);
        if (s.StartsWith(TokenInfo.BearerTokenType)) s = s.Substring(6).TrimStart();
        if (s.StartsWith(TokenInfo.BearerTokenType)) s = s.Substring(6).TrimStart();
        if (s.Length < 2) return null;
        var col = StringExtensions.TryParseJsonStrings(s);
        var r = new ProtectedResourceMetadataLink();
        if (col.TryGetValue("realm", out s) && s != null) r.Realm = s;
        if (col.TryGetValue("resource_metadata_url", out s) && s != null) r.Url = s;
        if (col.TryGetValue("scope", out s) && s != null) r.Scopes = s.Split(' ').ToList();
        return r;
    }

    /// <summary>
    /// Tries to parse the authentication header value of protected resource metadata.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <returns>The request info of protected resource metadata.</returns>
    public static ProtectedResourceMetadataLink TryParse(HttpResponseHeaders response)
    {
        var col = response?.WwwAuthenticate;
        if (col is null) return null;
        foreach (var item in col)
        {
            if (item.Scheme != TokenInfo.BearerTokenType || string.IsNullOrWhiteSpace(item.Parameter)) continue;
            return TryParse(item.Parameter);
        }

        return null;
    }

    /// <summary>
    /// Tries to parse the authentication header value of protected resource metadata.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <returns>The request info of protected resource metadata.</returns>
    public static ProtectedResourceMetadataLink TryParse(HttpResponseMessage response)
        => TryParse(response?.Headers);

    private static bool AppendComponent(StringBuilder sb, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        sb.Append(key);
        sb.Append('=');
        sb.Append(JsonStringNode.ToJson(value, false));
        sb.Append(',');
        return true;
    }
}

/// <summary>
/// The base response model of OAuth 2.0 Authorization Server Model (RFC-8414).
/// </summary>
public class AuthorizationServerMetadataResponse
{
    /// <summary>
    /// Gets or sets the authorization server's issuer identifier, which is a URL that uses the "https" scheme and has no query or fragment components.
    /// </summary>
    [DataMember(Name = "issuer", EmitDefaultValue = false)]
    [JsonPropertyName("issuer")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The authorization server's issuer identifier, which is a URL that uses the https scheme and has no query or fragment components.")]
    public string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the URL of the authorization server's authorization endpoint.
    /// </summary>
    [DataMember(Name = "authorization_endpoint", EmitDefaultValue = false)]
    [JsonPropertyName("authorization_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A URL of the authorization server's authorization endpoint.")]
    public string AuthorizationEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the URL of the authorization server's token endpoint.
    /// </summary>
    [DataMember(Name = "token_endpoint", EmitDefaultValue = false)]
    [JsonPropertyName("token_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A URL of the authorization server's token endpoint.")]
    public string TokenEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the document URL of the JSON Web Key Set.
    /// The referenced document contains the signing key(s) the client uses to validate signatures from the authorization server.
    /// It may also contain the server's encryption key or keys, which are used by clients to encrypt requests to the server.
    /// </summary>
    [DataMember(Name = "jwks_uri", EmitDefaultValue = false)]
    [JsonPropertyName("jwks_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The document URL of the JSON Web Key Set.")]
    public string JsonWebKeysUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL of the authorization server's OAuth 2.0 Dynamic Client Registration endpoint.
    /// </summary>
    [DataMember(Name = "registration_endpoint", EmitDefaultValue = false)]
    [JsonPropertyName("registration_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A URL of the authorization server's OAuth 2.0 Dynamic Client Registration endpoint.")]
    public string RegistrationEndpoint { get; set; }

    /// <summary>
    /// Gets or sets a list of scope values, as defined in OAuth 2.0, that this authorization server supports.
    /// </summary>
    [DataMember(Name = "scopes_supported", EmitDefaultValue = false)]
    [JsonPropertyName("scopes_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of scope values that this authorization server supports.")]
    public List<string> Scopes { get; set; }


    /// <summary>
    /// Gets or sets the endpoint URL for token validation.
    /// </summary>
    [DataMember(Name = "introspection_endpoint", EmitDefaultValue = false)]
    [JsonPropertyName("introspection_endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The endpoint URL for token validation.")]
    public string TokenIntrospectionEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the list of client authentication methods supported by this introspection endpoint.
    /// </summary>
    [DataMember(Name = "introspection_endpoint_auth_methods_supported", EmitDefaultValue = false)]
    [JsonPropertyName("introspection_endpoint_auth_methods_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of client authentication methods supported by this introspection endpoint.")]
    public List<string> IntrospectionEndpointAuthMethods { get; set; }

    /// <summary>
    /// Gets or sets the list of the JWS signing algorithms supported by the introspection endpoint for the signature on the JSON Web Token used to authenticate the client at the introspection endpoint for the "private_key_jwt" and "client_secret_jwt" authentication methods.
    /// </summary>
    [DataMember(Name = "introspection_endpoint_auth_signing_alg_values_supported", EmitDefaultValue = false)]
    [JsonPropertyName("introspection_endpoint_auth_signing_alg_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of the JWS signing algorithms supported by the introspection endpoint for the signature on the JSON Web Token used to authenticate the client at the introspection endpoint for the private_key_jwt and client_secret_jwt authentication methods.")]
    public List<string> IntrospectionEndpointAuthSigningAlgorithms { get; set; }

    /// <summary>
    /// Gets or sets the list of Proof Key for Code Exchange code challenge methods supported by this authorization server.
    /// </summary>
    [DataMember(Name = "code_challenge_methods_supported", EmitDefaultValue = false)]
    [JsonPropertyName("code_challenge_methods_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of Proof Key for Code Exchange code challenge methods supported by this authorization server.")]
    public List<string> CodeChallengeMethods { get; set; }

    /// <summary>
    /// Gets or sets the JSON Web Token that asserts metadata values about the authorization server as a bundle.
    /// </summary>
    [DataMember(Name = "signed_metadata", EmitDefaultValue = false)]
    [JsonPropertyName("signed_metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A JSON Web Token that asserts metadata values about the authorization server as a bundle.")]
    public string Signed { get; set; }

    /// <summary>
    /// Sets the signed metedata.
    /// </summary>
    /// <param name="jwt">The JSON Web Token with the metadata.</param>
    public void SetSignedMetadata<T>(JsonWebToken<T> jwt)
        => Signed = jwt?.ToEncodedString();

    /// <summary>
    /// Sets the signed metedata.
    /// </summary>
    /// <param name="issuer">The issuer.</param>
    /// <param name="sign">The signature provider.</param>
    /// <returns>The JSON Web Token.</returns>
    public JsonWebToken<JsonObjectNode> SetSignedMetadata(string issuer, ISignatureProvider sign)
    {
        var json = JsonObjectNode.ConvertFrom(this);
        json.Remove("signed_metadata");
        json.SetValueIfNotEmpty("iss", issuer);
        var jwt = new JsonWebToken<JsonObjectNode>(json, sign);
        SetSignedMetadata(jwt);
        return jwt;
    }

    /// <summary>
    /// Sets the signed metedata.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="sign">The signature provider.</param>
    public void SetSignedMetadata<T>(T payload, ISignatureProvider sign)
        => SetSignedMetadata(new JsonWebToken<T>(payload, sign));
}
