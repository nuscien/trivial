using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Data;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Security;

/// <summary>
/// The access token resolver request.
/// </summary>
[DataContract]
public class TokenRequest : IQueryDataGenerator
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
        if (scope is not null) Scope = scope.ToList();
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
        Body = body ?? new QueryDataTokenRequestBody(clientCredentials);
        if (clientCredentials == null) return;
        var clientId = clientCredentials[TokenRequestProperties.ClientId];
        var clientSecret = clientCredentials[TokenRequestProperties.ClientSecret];
        if (!string.IsNullOrEmpty(clientId) || !string.IsNullOrEmpty(clientSecret)) ClientCredentials = new AppAccessingKey(clientId, clientSecret);
        ScopeString = clientCredentials[TokenInfo.ScopeProperty];
    }

    /// <summary>
    /// Initializes a new instance of the TokenRequest class.
    /// </summary>
    /// <param name="q">The query data.</param>
    public TokenRequest(QueryData q) : this(null, q)
    {
    }

    /// <summary>
    /// Gets the client identifier and secret key.
    /// </summary>
    [JsonIgnore]
    public AppAccessingKey ClientCredentials { get; private set; }

    /// <summary>
    /// Gets the body.
    /// </summary>
    [DataMember(Name = "data")]
    [JsonPropertyName("data")]
    public TokenRequestBody Body { get; }

    /// <summary>
    /// Gets the grant type.
    /// </summary>
    [DataMember(Name = TokenRequestProperties.GrantType)]
    [JsonPropertyName(TokenRequestProperties.GrantType)]
    public string GrantType => Body?.GrantType;

    /// <summary>
    /// Gets the client identifier.
    /// </summary>
    [DataMember(Name = TokenRequestProperties.ClientId)]
    [JsonPropertyName(TokenRequestProperties.ClientId)]
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
    [JsonIgnore]
    public IList<string> Scope { get; private set; } = new List<string>();

    /// <summary>
    /// Gets or sets the scope string.
    /// </summary>
    [DataMember(Name = TokenInfo.ScopeProperty, EmitDefaultValue = false)]
    [JsonPropertyName(TokenInfo.ScopeProperty)]
    public string ScopeString
    {
        get
        {
            return Scope != null && Scope.Count > 0 ? string.Join(" ", Scope) : null;
        }

        set
        {
            if (Scope == null) Scope = new List<string>();
            Scope.Clear();
            if (string.IsNullOrWhiteSpace(value)) return;
            foreach (var ele in value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Scope.Add(ele);
            }
        }
    }

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <returns>The value of the specific property.</returns>
    public string Property(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return name switch
        {
            TokenRequestProperties.GrantType => GrantType,
            TokenRequestProperties.ClientId => ClientId,
            TokenRequestProperties.ClientSecret => ClientCredentials.Secret.ToUnsecureString(),
            TokenInfo.ScopeProperty => ScopeString,
            _ => Body?.Property(name),
        };
    }

    /// <summary>
    /// Gets the query data.
    /// </summary>
    /// <returns>A query data.</returns>
    public virtual QueryData ToQueryData()
    {
        var data = Body?.ToQueryData() ?? new QueryData();
        if (!string.IsNullOrWhiteSpace(ClientId)) data.Add(TokenRequestProperties.ClientId, ClientId);
        if (ClientCredentials != null && ClientCredentials.Secret != null && ClientCredentials.Secret.Length > 0) data.Add(TokenRequestProperties.ClientSecret, ClientCredentials.Secret.ToUnsecureString());
        if (!string.IsNullOrWhiteSpace(ScopeString)) data.Add(TokenInfo.ScopeProperty, ScopeString);
        return data;
    }

    /// <summary>
    /// Gets the JSON format string.
    /// </summary>
    /// <returns>A string in JSON format.</returns>
    protected virtual JsonObjectNode ToJsonObject()
    {
        var json = Body?.ToJsonObject() ?? new JsonObjectNode();
        if (!string.IsNullOrWhiteSpace(ClientId))
            json.SetValue(TokenRequestProperties.ClientId, ClientId);
        if (ClientCredentials != null && ClientCredentials.Secret != null && ClientCredentials.Secret.Length > 0)
            json.SetValue(TokenRequestProperties.ClientSecret, ClientCredentials.Secret.ToUnsecureString());
        if (!string.IsNullOrWhiteSpace(ScopeString))
            json.SetValue(TokenInfo.ScopeProperty, ScopeString);
        return json;
    }

    /// <summary>
    /// Gets the JSON format string.
    /// </summary>
    /// <returns>A string in JSON format.</returns>
    public virtual string ToJsonString()
        => ToJsonObject()?.ToString() ?? "{}";

    /// <summary>
    /// Writes this instance to the specified writer as a JSON value.
    /// </summary>
    /// <param name="writer">The writer to which to write this instance.</param>
    public void WriteTo(Utf8JsonWriter writer)
        => ToJsonObject().WriteTo(writer);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A query string of the token request but without client secret.</returns>
    public override string ToString()
    {
        var data = ToQueryData() ?? new();
        data.Remove(TokenRequestProperties.ClientSecret);
        return data.ToString();
    }

    internal JsonObjectNode ToJsonObjectNode()
        => ToJsonObject();
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
        Body = body;
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
        Body = body;
    }

    /// <summary>
    /// Initializes a new instance of the TokenRequest class.
    /// </summary>
    /// <param name="body">The request body.</param>
    /// <param name="clientCredentials">The client credentials and scope query data.</param>
    public TokenRequest(T body, QueryData clientCredentials) : base(body, clientCredentials)
    {
        Body = body;
    }

    /// <summary>
    /// Gets the body.
    /// </summary>
    public new T Body { get; }
}

/// <summary>
/// The property key and value consts.
/// </summary>
public static class TokenRequestProperties
{
    /// <summary>
    /// The grant type property name.
    /// </summary>
    public const string GrantType = "grant_type";

    /// <summary>
    /// The client identifier property name.
    /// </summary>
    public const string ClientId = "client_id";

    /// <summary>
    /// The client secret property name.
    /// </summary>
    public const string ClientSecret = "client_secret";

    /// <summary>
    /// The grant type value of client credentials.
    /// </summary>
    public const string ClientCredentials = "client_credentials";

    /// <summary>
    /// The grant type value of authorization code.
    /// </summary>
    public const string AuthorizationCode = "authorization_code";

    /// <summary>
    /// The code property name.
    /// </summary>
    public const string Code = "code";

    /// <summary>
    /// The redirect URI property name.
    /// </summary>
    public const string RedirectUri = "redirect_uri";

    /// <summary>
    /// The code verifier property name.
    /// </summary>
    public const string CodeVerifier = "code_verifier";

    /// <summary>
    /// The service provider property name.
    /// </summary>
    public const string ServiceProvider = "provider";

    /// <summary>
    /// The state property name.
    /// </summary>
    public const string State = "state";

    /// <summary>
    /// The response type property name.
    /// </summary>
    public const string ResponseType = "response_type";

    /// <summary>
    /// The grant type value of refresh token.
    /// </summary>
    public const string RefreshToken = "refresh_token";

    /// <summary>
    /// The grant type value of password.
    /// </summary>
    public const string Password = "password";

    /// <summary>
    /// The user name property name.
    /// </summary>
    public const string UserName = "username";

    /// <summary>
    /// The LDAP (Lightweight Directory Access Protocol) property name.
    /// </summary>
    public const string Ldap = "ldap";

    /// <summary>
    /// The token type of basic.
    /// </summary>
    public const string Basic = "Basic";
}
