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
        var clientId = clientCredentials[TokenPropertyKeys.ClientId];
        var clientSecret = clientCredentials[TokenPropertyKeys.ClientSecret];
        if (!string.IsNullOrEmpty(clientId) || !string.IsNullOrEmpty(clientSecret)) ClientCredentials = new AppAccessingKey(clientId, clientSecret);
        ScopeString = clientCredentials[TokenPropertyKeys.Scope];
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
    [DataMember(Name = TokenPropertyKeys.GrantType)]
    [JsonPropertyName(TokenPropertyKeys.GrantType)]
    public string GrantType => Body?.GrantType;

    /// <summary>
    /// Gets the client identifier.
    /// </summary>
    [DataMember(Name = TokenPropertyKeys.ClientId)]
    [JsonPropertyName(TokenPropertyKeys.ClientId)]
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
    [DataMember(Name = TokenPropertyKeys.Scope, EmitDefaultValue = false)]
    [JsonPropertyName(TokenPropertyKeys.Scope)]
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
            TokenPropertyKeys.GrantType => GrantType,
            TokenPropertyKeys.ClientId => ClientId,
            TokenPropertyKeys.ClientSecret => ClientCredentials.Secret.ToUnsecureString(),
            TokenPropertyKeys.Scope => ScopeString,
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
        if (!string.IsNullOrWhiteSpace(ClientId)) data.Add(TokenPropertyKeys.ClientId, ClientId);
        if (ClientCredentials != null && ClientCredentials.Secret != null && ClientCredentials.Secret.Length > 0) data.Add(TokenPropertyKeys.ClientSecret, ClientCredentials.Secret.ToUnsecureString());
        if (!string.IsNullOrWhiteSpace(ScopeString)) data.Add(TokenPropertyKeys.Scope, ScopeString);
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
            json.SetValue(TokenPropertyKeys.ClientId, ClientId);
        if (ClientCredentials != null && ClientCredentials.Secret != null && ClientCredentials.Secret.Length > 0)
            json.SetValue(TokenPropertyKeys.ClientSecret, ClientCredentials.Secret.ToUnsecureString());
        if (!string.IsNullOrWhiteSpace(ScopeString))
            json.SetValue(TokenPropertyKeys.Scope, ScopeString);
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
        data.Remove(TokenPropertyKeys.ClientSecret);
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
