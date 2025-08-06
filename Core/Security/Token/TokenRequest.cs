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
        Body = body ?? new QueryDataTokenRequestBody(clientCredentials);
        if (clientCredentials == null) return;
        var clientId = clientCredentials[TokenRequestBody.ClientIdProperty];
        var clientSecret = clientCredentials[TokenRequestBody.ClientSecretProperty];
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
    [DataMember(Name = TokenRequestBody.GrantTypeProperty)]
    [JsonPropertyName(TokenRequestBody.GrantTypeProperty)]
    public string GrantType => Body?.GrantType;

    /// <summary>
    /// Gets the client identifier.
    /// </summary>
    [DataMember(Name = TokenRequestBody.ClientIdProperty)]
    [JsonPropertyName(TokenRequestBody.ClientIdProperty)]
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
            TokenRequestBody.GrantTypeProperty => GrantType,
            TokenRequestBody.ClientIdProperty => ClientId,
            TokenRequestBody.ClientSecretProperty => ClientCredentials.Secret.ToUnsecureString(),
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
        if (!string.IsNullOrWhiteSpace(ClientId)) data.Add(TokenRequestBody.ClientIdProperty, ClientId);
        if (ClientCredentials != null && ClientCredentials.Secret != null && ClientCredentials.Secret.Length > 0) data.Add(TokenRequestBody.ClientSecretProperty, ClientCredentials.Secret.ToUnsecureString());
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
            json.SetValue(TokenRequestBody.ClientIdProperty, ClientId);
        if (ClientCredentials != null && ClientCredentials.Secret != null && ClientCredentials.Secret.Length > 0)
            json.SetValue(TokenRequestBody.ClientSecretProperty, ClientCredentials.Secret.ToUnsecureString());
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
        data.Remove(TokenRequestBody.ClientSecretProperty);
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
    public AppAccessingKey(string id, ReadOnlySpan<char> secret)
    {
        Id = id;
        Secret = secret.ToSecure();
    }

    /// <summary>
    /// Initializes a new instance of the AppAccessingKey class.
    /// </summary>
    /// <param name="id">The app id.</param>
    /// <param name="secret">The secret key.</param>
    public AppAccessingKey(string id, SecureString secret)
    {
        Id = id;
        Secret = secret?.Copy();
    }

    /// <summary>
    /// Deconstructor.
    /// </summary>
    ~AppAccessingKey()
    {
        if (Secret == null) return;
        Secret.Dispose();
        Secret = null;
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
        => Id ?? string.Empty;
}

/// <summary>
/// The access token resolver request body.
/// </summary>
/// <param name="grantType">The grant type.</param>
[DataContract]
public abstract class TokenRequestBody(string grantType)
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
    /// Gets the grant type.
    /// </summary>
    [DataMember(Name = GrantTypeProperty)]
    [JsonPropertyName(GrantTypeProperty)]
    public string GrantType { get; } = grantType;

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <returns>The value of the specific property.</returns>
    public virtual string Property(string name)
        => ToQueryData()?[name];

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
            (var propStr, _) = GetNumberValueString(propValue, propValue.GetType());
            if (propStr != null) data.Add(attr.Name, propStr);
        }

        return data;
    }

    /// <summary>
    /// Gets the JSON object.
    /// </summary>
    /// <returns>A JSON object instance.</returns>
    internal protected virtual JsonObjectNode ToJsonObject()
    {
        var json = JsonSerializer.Serialize(this, GetType());
        return JsonObjectNode.Parse(json);
    }

    private static (string, bool) GetNumberValueString(object obj, Type type)
    {
        if (obj == null) return (null, false);
        if (type == typeof(string) || type == typeof(StringBuilder)) return (obj.ToString(), true);
        if (type == typeof(DateTime)) return (WebFormat.ParseDate((DateTime)obj).ToString(CultureInfo.InvariantCulture), false);
        if (type == typeof(DateTimeOffset)) return (WebFormat.ParseDate((DateTimeOffset)obj).ToString(CultureInfo.InvariantCulture), false);
        if (type == typeof(int)) return (((int)obj).ToString("g", CultureInfo.InvariantCulture), false);
        if (type == typeof(long)) return (((long)obj).ToString("g", CultureInfo.InvariantCulture), false);
        if (type == typeof(uint)) return (((uint)obj).ToString("g", CultureInfo.InvariantCulture), false);
        if (type == typeof(ulong)) return (((ulong)obj).ToString("g", CultureInfo.InvariantCulture), false);
        if (type == typeof(float)) return (((float)obj).ToString("g", CultureInfo.InvariantCulture), false);
        if (type == typeof(short)) return (((short)obj).ToString("g", CultureInfo.InvariantCulture), false);
        if (type == typeof(ushort)) return (((short)obj).ToString("g", CultureInfo.InvariantCulture), false);
        if (type == typeof(bool)) return (((bool)obj).ToString(CultureInfo.InvariantCulture), false);
        if (type == typeof(SecureString)) return (((SecureString)obj).ToUnsecureString(), true);
        if (type == typeof(Uri))
        {
            try
            {
                return (((Uri)obj).OriginalString, true);
            }
            catch (InvalidOperationException)
            {
                return (((Uri)obj).ToString(), true);
            }
        }

        if (type == typeof(double))
        {
            var num = (double)obj;
            var nStr = num.ToString("g", CultureInfo.InvariantCulture);
            if (nStr.IndexOf('e') < 0) return (nStr, false);
            return (nStr, true);
        }

        if (type == typeof(TimeSpan))
        {
            var num = ((TimeSpan)obj).TotalSeconds;
            var nStr = num.ToString("g", CultureInfo.InvariantCulture);
            if (nStr.IndexOf('e') < 0) return (nStr, false);
            return (nStr, true);
        }

        var str = obj.ToString();
        if (!string.IsNullOrWhiteSpace(str)) return (obj.ToString(), true);
        return (null, false);
    }
}

/// <summary>
/// The access token resolver request body.
/// </summary>
internal sealed class QueryDataTokenRequestBody : TokenRequestBody
{
    private readonly QueryData query;

    /// <summary>
    /// Initializes a new instance of the QueryDataTokenRequestBody class.
    /// </summary>
    /// <param name="q">The query data.</param>
    public QueryDataTokenRequestBody(QueryData q) : base(q[GrantTypeProperty])
    {
        query = q ?? new();
    }

    /// <summary>
    /// Gets the query data.
    /// </summary>
    /// <returns>A query data.</returns>
    public override QueryData ToQueryData()
        => query;

    /// <summary>
    /// Gets the JSON format string.
    /// </summary>
    /// <returns>A string in JSON format.</returns>
    internal protected override JsonObjectNode ToJsonObject()
    {
        var json = new JsonObjectNode();
        json.SetRange(query);
        return json;
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
    /// <exception cref="InvalidOperationException">The grant type is not expected.</exception>
    private void Fill(QueryData q)
    {
        if (q == null) return;
        var grantType = q[GrantTypeProperty];
        if (!string.IsNullOrEmpty(grantType) && grantType != GrantType) throw new InvalidOperationException($"The grant type is not the expected one. Current is {grantType}; but the expect is {GrantType}.");
    }

    /// <summary>
    /// Creates the token request body.
    /// </summary>
    /// <param name="q">The query data used to fill properties.</param>
    /// <returns>The object parsed returned.</returns>
    /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
    internal static ClientTokenRequestBody Create(QueryData q)
    {
        var body = new ClientTokenRequestBody();
        body.Fill(q);
        return body;
    }

    /// <summary>
    /// Parses a string to client credentials access token request.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The object parsed returned.</returns>
    /// <exception cref="ArgumentNullException">s was null.</exception>
    /// <exception cref="ArgumentException">s was empty or consists only of white-space characters; or s was not in correct format to parse.</exception>
    /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
    /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
    public static TokenRequest<ClientTokenRequestBody> Parse(string s)
    {
        StringExtensions.AssertNotWhiteSpace(nameof(s), s);
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
    /// The service provider property name.
    /// </summary>
    public const string ServiceProviderProperty = "provider";

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
    [JsonPropertyName(CodeProperty)]
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the redirect URI.
    /// </summary>
    [DataMember(Name = RedirectUriProperty, EmitDefaultValue = false)]
    [JsonPropertyName(RedirectUriProperty)]
    public Uri RedirectUri { get; set; }

    /// <summary>
    /// Gets or sets the code verifier.
    /// </summary>
    [DataMember(Name = CodeVerifierProperty, EmitDefaultValue = false)]
    [JsonPropertyName(CodeVerifierProperty)]
    public string CodeVerifier { get; set; }

    /// <summary>
    /// Gets or sets the service provider.
    /// </summary>
    [DataMember(Name = ServiceProviderProperty, EmitDefaultValue = false)]
    [JsonPropertyName(ServiceProviderProperty)]
    public string ServiceProvider { get; set; }

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
    /// <exception cref="InvalidOperationException">The grant type is not expected.</exception>
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
        if (redirectUri != null) RedirectUri = redirectUri == string.Empty ? null : new Uri(redirectUri, UriKind.RelativeOrAbsolute);
        var provider = q[ServiceProviderProperty];
        if (provider != null) ServiceProvider = q[ServiceProviderProperty];
    }

    /// <summary>
    /// Creates the token request body.
    /// </summary>
    /// <param name="q">The query data used to fill properties.</param>
    /// <returns>The object parsed returned.</returns>
    /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
    internal static CodeTokenRequestBody Create(QueryData q)
    {
        var body = new CodeTokenRequestBody();
        body.Fill(q);
        return body;
    }

    /// <summary>
    /// Parses a string to code access token request.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The object parsed returned.</returns>
    /// <exception cref="ArgumentNullException">s was null.</exception>
    /// <exception cref="ArgumentException">s was empty or consists only of white-space characters; or s was not in correct format to parse.</exception>
    /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
    /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
    public static TokenRequest<CodeTokenRequestBody> Parse(string s)
    {
        StringExtensions.AssertNotWhiteSpace(nameof(s), s);
        var q = QueryData.Parse(s);
        var body = new CodeTokenRequestBody();
        body.Fill(q);
        return new TokenRequest<CodeTokenRequestBody>(body, q);
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
    [JsonPropertyName(TokenInfo.RefreshTokenProperty)]
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
    /// <exception cref="InvalidOperationException">The grant type is not expected.</exception>
    private void Fill(QueryData q)
    {
        if (q == null) return;
        var grantType = q[GrantTypeProperty];
        if (!string.IsNullOrEmpty(grantType) && grantType != GrantType) throw new InvalidOperationException($"The grant type is not the expected one. Current is {grantType}; but the expect is {GrantType}.");
        var refreshToken = q[TokenInfo.RefreshTokenProperty];
        if (refreshToken != null) RefreshToken = refreshToken;
    }

    /// <summary>
    /// Creates the token request body.
    /// </summary>
    /// <param name="q">The query data used to fill properties.</param>
    /// <returns>The object parsed returned.</returns>
    /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
    internal static RefreshTokenRequestBody Create(QueryData q)
    {
        var body = new RefreshTokenRequestBody();
        body.Fill(q);
        return body;
    }

    /// <summary>
    /// Parses a string to code access token request.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The object parsed returned.</returns>
    /// <exception cref="ArgumentNullException">s was null.</exception>
    /// <exception cref="ArgumentException">s was empty or consists only of white-space characters; or s was not in correct format to parse.</exception>
    /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
    /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
    public static TokenRequest<RefreshTokenRequestBody> Parse(string s)
    {
        StringExtensions.AssertNotWhiteSpace(nameof(s), s);
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
public class PasswordTokenRequestBody : TokenRequestBody, ICredentials, ICredentialsByHost
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
    /// The LDAP (Lightweight Directory Access Protocol) property name.
    /// </summary>
    public const string LdapProperty = "ldap";

    /// <summary>
    /// The token type.
    /// </summary>
    public const string BasicTokenType = "Basic";

    /// <summary>
    /// The network credential.
    /// </summary>
    private NetworkCredential credential;

    /// <summary>
    /// Initializes a new instance of the TokenRequest class.
    /// </summary>
    public PasswordTokenRequestBody() : base(PasswordGrantType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the TokenRequest class.
    /// </summary>
    /// <param name="credential">A network credential instance.</param>
    /// <param name="ignoreDomain">true if ignore the domain; otherwise, set user name as domain and user name.</param>
    public PasswordTokenRequestBody(NetworkCredential credential, bool ignoreDomain = false) : base(PasswordGrantType)
    {
        if (string.IsNullOrWhiteSpace(credential.Domain) || ignoreDomain || credential.UserName.Contains('@'))
        {
            UserName = credential.Domain;
        }
        else
        {
            var domain = credential.Domain + '\\';
            var username = credential.UserName ?? string.Empty;
            UserName = credential.UserName.StartsWith(domain) ? username : (domain + username);
        }

        Password = credential.SecurePassword;
    }

    /// <summary>
    /// Initializes a new instance of the TokenRequest class.
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <param name="password">The password.</param>
    /// <param name="ldap">The optional LDAP.</param>
    public PasswordTokenRequestBody(string userName, string password, string ldap = null) : this()
    {
        UserName = userName;
        Password = password?.ToSecure();
        Ldap = ldap;
    }

    /// <summary>
    /// Initializes a new instance of the TokenRequest class.
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <param name="password">The password.</param>
    /// <param name="ldap">The optional LDAP.</param>
    public PasswordTokenRequestBody(string userName, SecureString password, string ldap = null) : this()
    {
        UserName = userName;
        Password = password;
        Ldap = ldap;
    }

    /// <summary>
    /// Initializes a new instance of the PasswordTokenRequest class.
    /// </summary>
    /// <param name="tokenRequestBody">Another token request body to copy.</param>
    public PasswordTokenRequestBody(PasswordTokenRequestBody tokenRequestBody) : this(tokenRequestBody?.UserName, tokenRequestBody?.Password, tokenRequestBody?.Ldap)
    {
    }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    [DataMember(Name = UserNameProperty)]
    [JsonPropertyName(UserNameProperty)]
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [JsonIgnore]
    public SecureString Password { get; set; }

    /// <summary>
    /// Gets or sets the address of Lightweight Directory Access Protocol.
    /// </summary>
    [DataMember(Name = LdapProperty, EmitDefaultValue = false)]
    [JsonPropertyName(LdapProperty)]
    public string Ldap { get; set; }

    /// <summary>
    /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current user name and password information.
    /// </summary>
    /// <param name="schemeCase">The scheme case.</param>
    /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current user name and password information.</returns>
    public AuthenticationHeaderValue ToAuthenticationHeaderValue(Cases schemeCase = Cases.Original)
        => ToAuthenticationHeaderValue(null, schemeCase);

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
            StringExtensions.ToSpecificCaseInvariant(BasicTokenType, schemeCase),
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
    /// Gets the query data.
    /// </summary>
    /// <returns>A query data.</returns>
    public override QueryData ToQueryData()
    {
        var query = base.ToQueryData() ?? new();
        query[PasswordProperty] = Password.ToUnsecureString();
        return query;
    }

    /// <summary>
    /// Gets a network credential instance for the specified Uniform Resource Identifier (URI) and authentication type.
    /// </summary>
    /// <param name="uri">The URI that the client provides authentication for.</param>
    /// <param name="authType">The type of authentication requested, as defined in the <seealso cref="System.Net.IAuthenticationModule.AuthenticationType" /> property.</param>
    /// <returns>A network credential object.</returns>
    public NetworkCredential GetCredential(Uri uri, string authType)
    {
        credential ??= new NetworkCredential(UserName, Password);
        return credential.GetCredential(uri, authType);
    }

    /// <summary>
    /// Gets a network credential instance for the specified Uniform Resource Identifier (URI) and authentication type.
    /// </summary>
    /// <param name="host">The host computer that authenticates the client.</param>
    /// <param name="port">The port on the host that the client communicates with.</param>
    /// <param name="authType">The type of authentication requested, as defined in the <seealso cref="System.Net.IAuthenticationModule.AuthenticationType" /> property.</param>
    /// <returns>A network credential object.</returns>
    public NetworkCredential GetCredential(string host, int port, string authType)
    {
        credential ??= new NetworkCredential(UserName, Password);
        return credential.GetCredential(host, port, authType);
    }

    /// <summary>
    /// Gets the JSON format string.
    /// </summary>
    /// <returns>A string in JSON format.</returns>
    internal protected override JsonObjectNode ToJsonObject()
    {
        var json = base.ToJsonObject() ?? new();
        json.SetValue(PasswordProperty, Password.ToUnsecureString());
        return json;
    }

    /// <summary>
    /// Fills the data into the current request body.
    /// </summary>
    /// <param name="q">The query data.</param>
    /// <returns>true if fill succeeded; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">The grant type is not expected.</exception>
    private void Fill(QueryData q)
    {
        if (q == null) return;
        var grantType = q[GrantTypeProperty];
        if (!string.IsNullOrEmpty(grantType) && grantType != GrantType) throw new InvalidOperationException($"The grant type is not the expected one. Current is {grantType}; but the expect is {GrantType}.");
        var userName = q[UserNameProperty];
        if (userName != null) UserName = userName;
        var password = q[PasswordProperty];
        if (password != null) Password = password == string.Empty ? null : password.ToSecure();
        var ldap = q[LdapProperty];
        if (ldap != null) Ldap = ldap;
    }

    /// <summary>
    /// Creates the token request body.
    /// </summary>
    /// <param name="q">The query data used to fill properties.</param>
    /// <returns>The object parsed returned.</returns>
    /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
    internal static PasswordTokenRequestBody Create(QueryData q)
    {
        var body = new PasswordTokenRequestBody();
        body.Fill(q);
        return body;
    }

    /// <summary>
    /// Creates the password token request body from basic token authentication value.
    /// </summary>
    /// <param name="token">The token value.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>The password token request.</returns>
    /// <exception cref="FormatException">The token value is invalid.</exception>
    public static PasswordTokenRequestBody CreateByBasicToken(string token, Encoding encoding = null)
    {
        token = token?.Trim();
        if (string.IsNullOrEmpty(token)) return null;
        if (token.ToLower().StartsWith("basic ")) token = token.Substring(6).Trim();
        if (token.ToLower().StartsWith("basic ")) token = token.Substring(6).Trim();
        if (token.IndexOf(' ') > 0 || token.Length < 4)
            throw new FormatException("The token value is invalid.");
        try
        {
            var bytes = Convert.FromBase64String(token);
            token = (encoding ?? Encoding.UTF8).GetString(bytes);
            var arr = token.Split(':');
            if (arr.Length == 0) return null;
            return new PasswordTokenRequestBody(arr[0], arr.Length > 1 ? arr[1] : null);
        }
        catch (ArgumentException ex)
        {
            throw new FormatException("The token value is invalid.", ex);
        }
        catch (FormatException ex)
        {
            throw new FormatException("The token value is invalid.", ex);
        }
    }

    /// <summary>
    /// Creates the password token request body from basic token authentication value.
    /// </summary>
    /// <param name="token">The token value.</param>
    /// <param name="encoding">The optional encoding.</param>
    /// <returns>The password token request.</returns>
    /// <exception cref="FormatException">The token value is invalid.</exception>
    public static PasswordTokenRequestBody CreateByBasicToken(AuthenticationHeaderValue token, Encoding encoding = null)
        => CreateByBasicToken(token?.Parameter, encoding);

    /// <summary>
    /// Parses a string to code access token request.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The object parsed returned.</returns>
    /// <exception cref="ArgumentNullException">s was null.</exception>
    /// <exception cref="ArgumentException">s was empty or consists only of white-space characters; or s was not in correct format to parse.</exception>
    /// <exception cref="NotSupportedException">s was not in correct format to parse.</exception>
    /// <exception cref="InvalidOperationException">The grant type was not the expected one.</exception>
    public static TokenRequest<PasswordTokenRequestBody> Parse(string s)
    {
        StringExtensions.AssertNotWhiteSpace(nameof(s), s);
        var q = QueryData.Parse(s);
        var body = new PasswordTokenRequestBody();
        body.Fill(q);
        return new TokenRequest<PasswordTokenRequestBody>(body, q);
    }
}
