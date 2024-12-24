using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Trivial.Data;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Security;

/// <summary>
/// <para>The OAuth HTTP web client (for RFC-6749).</para>
/// <para>You can use this to login and then create the JSON HTTP web clients with the authentication information.</para>
/// </summary>
/// <example>
/// <code>
/// // Inialize a new instance of OAuth client
/// // with client identifier, client secret, authorization URI and scope.
/// var oauth = new OAuthClient(
///     CLIENT_ID,
///     CLIENT_SECRET,
///     new Uri("https://login.live.com/accesstoken.srf"),
///     "notify.windows.com");
///
/// // Get access token.
/// var token = await oauth.ResolveTokenAsync(new ClientTokenRequestBody());
///
/// // Then you can create the JSON HTTP web client when you need,
/// // And it will set the access token and its type into the authorization header of HTTP request.
/// var httpClient = oauth.Create&lt;ResponseBody&gt;();
/// 
/// // Get response with authentication.
/// var resp = await httpClient.PostAsync(A-URL-FOR-BUSINESS, payload);
/// </code>
/// </example>
public class OAuthClient : TokenContainer, IJsonHttpClientMaker
{
    /// <summary>
    /// The app accessing key instance.
    /// </summary>
    private readonly AppAccessingKey appInfo;

    /// <summary>
    /// The locker.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private SemaphoreSlim slim;

    /// <summary>
    /// Initializes a new instance of the OAuthClient class.
    /// </summary>
    /// <param name="tokenCached">The token information instance cached.</param>
    public OAuthClient(TokenInfo tokenCached) : base(tokenCached)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OAuthHttpClient class.
    /// </summary>
    /// <param name="appKey">The app accessing key.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    public OAuthClient(AppAccessingKey appKey, TokenInfo tokenCached = null) : base(tokenCached)
    {
        appInfo = appKey;
    }

    /// <summary>
    /// Initializes a new instance of the OAuthHttpClient class.
    /// </summary>
    /// <param name="appId">The app id.</param>
    /// <param name="secretKey">The secret key.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    public OAuthClient(string appId, string secretKey = null, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OAuthHttpClient class.
    /// </summary>
    /// <param name="appId">The app id.</param>
    /// <param name="secretKey">The secret key.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    public OAuthClient(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OAuthHttpClient class.
    /// </summary>
    /// <param name="appKey">The app accessing key.</param>
    /// <param name="tokenResolveUri">The token resolve URI.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    public OAuthClient(AppAccessingKey appKey, Uri tokenResolveUri, IEnumerable<string> scope, TokenInfo tokenCached = null) : this(appKey, tokenCached)
    {
        TokenResolverUri = tokenResolveUri;
        if (scope == null) return;
        foreach (var ele in scope)
        {
            Scope.Add(ele);
        }
    }

    /// <summary>
    /// Initializes a new instance of the OAuthHttpClient class.
    /// </summary>
    /// <param name="appId">The app id.</param>
    /// <param name="secretKey">The secret key.</param>
    /// <param name="tokenResolveUri">The token resolve URI.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    public OAuthClient(string appId, string secretKey, Uri tokenResolveUri, IEnumerable<string> scope, TokenInfo tokenCached = null) : this(appId, secretKey, tokenCached)
    {
        TokenResolverUri = tokenResolveUri;
        if (scope == null) return;
        foreach (var ele in scope)
        {
            Scope.Add(ele);
        }
    }

    /// <summary>
    /// Initializes a new instance of the OAuthHttpClient class.
    /// </summary>
    /// <param name="appId">The app id.</param>
    /// <param name="secretKey">The secret key.</param>
    /// <param name="tokenResolveUri">The token resolve URI.</param>
    /// <param name="scope">The scope.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    public OAuthClient(string appId, SecureString secretKey, Uri tokenResolveUri, IEnumerable<string> scope, TokenInfo tokenCached = null) : this(appId, secretKey, tokenCached)
    {
        TokenResolverUri = tokenResolveUri;
        if (scope == null) return;
        foreach (var ele in scope)
        {
            Scope.Add(ele);
        }
    }

    /// <summary>
    /// Deconstructor.
    /// </summary>
    ~OAuthClient()
    {
        try
        {
            slim?.Dispose();
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
    }

    /// <summary>
    /// Adds or removes a handler raised on sending.
    /// </summary>
    public event EventHandler<SendingEventArgs> Sending;

    /// <summary>
    /// Adds or removes a handler raised on token is resolving.
    /// </summary>
    public event EventHandler<SendingEventArgs> TokenResolving;

    /// <summary>
    /// Adds or removes a handler raised on token has resolved.
    /// </summary>
    public event EventHandler<ReceivedEventArgs<TokenInfo>> TokenResolved;

    /// <summary>
    /// Gets the app identifier.
    /// </summary>
    [JsonIgnore]
    public string AppId => appInfo?.Id;

    /// <summary>
    /// Gets or sets the token resolver URI.
    /// </summary>
    [JsonIgnore]
    public Uri TokenResolverUri { get; set; }

    /// <summary>
    /// Gets the scope to use.
    /// </summary>
    [JsonIgnore]
    public IList<string> Scope { get; private set; } = new List<string>();

    /// <summary>
    /// Gets the scope string.
    /// </summary>
    [JsonIgnore]
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
    /// Gets or sets the JSON deserializer.
    /// </summary>
    [JsonIgnore]
    public Func<string, Type, object> Deserializer { get; set; }

    /// <summary>
    /// Gets or sets a handler to indicate whether the token request is valid and fill additional information if needed.
    /// </summary>
    [JsonIgnore]
    public Func<QueryData, bool> TokenRequestInfoValidator { get; set; }

    /// <summary>
    /// Gets or sets the HTTP client resolver.
    /// </summary>
    [JsonIgnore]
    public IObjectResolver<HttpClient> HttpClientResolver { get; set; }

    /// <summary>
    /// Gets or sets the HTTP client resolver for authorization logic; or null, to use HttpClientResolver property.
    /// </summary>
    [JsonIgnore]
    public IObjectResolver<HttpClient> AuthorizationHttpClientResolver { get; set; }

    /// <summary>
    /// Gets the date and time of token resolved.
    /// </summary>
    [JsonIgnore]
    public DateTime TokenResolvedTime { get; private set; } = DateTime.Now;

    /// <summary>
    /// Creates a JSON HTTP client.
    /// </summary>
    /// <typeparam name="T">The type of response.</typeparam>
    /// <param name="callback">An optional callback raised on data received.</param>
    /// <returns>A new JSON HTTP client.</returns>
    public virtual JsonHttpClient<T> Create<T>(Action<ReceivedEventArgs<T>> callback = null)
    {
        var httpClient = new JsonHttpClient<T>
        {
            HttpClientResolver = HttpClientResolver,
        };
        httpClient.Sending += (sender, ev) =>
        {
            WriteAuthenticationHeaderValue(ev.RequestMessage.Headers);
            Sending?.Invoke(sender, ev);
        };
        if (Deserializer != null) httpClient.Deserializer = json => (T)Deserializer(json, typeof(T));
        if (callback != null) httpClient.Received += (sender, ev) =>
        {
            callback(ev);
        };
        return httpClient;
    }

    /// <summary>
    /// Creates a token request instance by a specific body.
    /// The current app secret information will be filled into the instance.
    /// </summary>
    /// <typeparam name="T">The type of the body.</typeparam>
    /// <param name="body">The token request body.</param>
    /// <param name="scope">The additional scope.</param>
    /// <returns>A token request instance with given body and the current app secret information.</returns>
    public TokenRequest<T> CreateTokenRequest<T>(T body, IEnumerable<string> scope = null) where T : TokenRequestBody
        => new(body, appInfo, scope);

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw ObjectConvert.ArgumentNull(nameof(request));
        var client = GetHttpClient();
        WriteAuthenticationHeaderValue(request.Headers);
        Sending?.Invoke(this, new SendingEventArgs(request));
        return client.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
    {
        if (request == null) throw ObjectConvert.ArgumentNull(nameof(request));
        var client = GetHttpClient();
        WriteAuthenticationHeaderValue(request.Headers);
        Sending?.Invoke(this, new SendingEventArgs(request));
        return client.SendAsync(request, completionOption, cancellationToken);
    }

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    public Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => Create<T>().SendAsync(request, cancellationToken);

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="callback">An optional callback raised on data received.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    public Task<T> SendAsync<T>(HttpRequestMessage request, Action<ReceivedEventArgs<T>> callback, CancellationToken cancellationToken = default)
        => Create<T>().SendAsync(request, callback, cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    public Task<TokenInfo> ResolveTokenAsync(TokenRequestBody content, CancellationToken cancellationToken = default)
    {
        if (TokenResolverUri == null) throw new InvalidOperationException("TokenResolverUri property should not be null.");
        return ResolveTokenAsync(TokenResolverUri, content, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <typeparam name="T">The token info type.</typeparam>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    public Task<T> ResolveTokenAsync<T>(TokenRequestBody content, CancellationToken cancellationToken = default) where T : TokenInfo
    {
        if (TokenResolverUri == null) throw new InvalidOperationException("TokenResolverUri property should not be null.");
        return ResolveTokenAsync<T>(TokenResolverUri, content, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="oldTokenValidation">The validation of old token. The resolving will be in thread-safe mode.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    public async Task<TokenInfo> ResolveTokenAsync(TokenRequestBody content, Func<TokenInfo, bool> oldTokenValidation, CancellationToken cancellationToken = default)
    {
        if (TokenResolverUri == null) throw new InvalidOperationException("TokenResolverUri property should not be null.");
        var token = Token;
        if (oldTokenValidation is null) oldTokenValidation = t => t?.IsEmpty == false;
        if (token?.IsEmpty == false && oldTokenValidation(token)) return token;
        var locker = slim;
        if (locker is null)
        {
            locker = new SemaphoreSlim(1);
            if (slim is null)
            {
                slim = locker;
            }
            else
            {
                try
                {
                    locker.Dispose();
                }
                catch (InvalidOperationException)
                {
                }
                catch (NullReferenceException)
                {
                }

                locker = slim;
            }
        }

        await locker.WaitAsync(cancellationToken);
        token = Token;
        if (token?.IsEmpty == false && oldTokenValidation(token)) return token;
        try
        {
            return await ResolveTokenAsync(TokenResolverUri, content, cancellationToken);
        }
        finally
        {
            try
            {
                locker.Release();
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (SemaphoreFullException)
            {
            }
        }
    }

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <typeparam name="T">The token info type.</typeparam>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="oldTokenValidation">The validation of old token. The resolving will be in thread-safe mode.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    public async Task<T> ResolveTokenAsync<T>(TokenRequestBody content, Func<TokenInfo, bool> oldTokenValidation, CancellationToken cancellationToken = default) where T : TokenInfo
    {
        if (TokenResolverUri == null) throw new InvalidOperationException("TokenResolverUri property should not be null.");
        var token = Token as T;
        if (oldTokenValidation is null) oldTokenValidation = t => t?.IsEmpty == false;
        if (token?.IsEmpty == false && oldTokenValidation(token)) return token;
        var locker = slim;
        if (locker is null)
        {
            locker = new SemaphoreSlim(1);
            if (slim is null)
            {
                slim = locker;
            }
            else
            {
                try
                {
                    locker.Dispose();
                }
                catch (InvalidOperationException)
                {
                }
                catch (NullReferenceException)
                {
                }

                locker = slim;
            }
        }

        await locker.WaitAsync(cancellationToken);
        token = Token as T;
        if (token?.IsEmpty == false && oldTokenValidation(token)) return token;
        try
        {
            return await ResolveTokenAsync<T>(TokenResolverUri, content, cancellationToken);
        }
        finally
        {
            try
            {
                locker.Release();
            }
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }
            catch (SemaphoreFullException)
            {
            }
        }
    }

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">requestUri or content was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    public Task<TokenInfo> ResolveTokenAsync(Uri requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
    {
        var uri = requestUri ?? TokenResolverUri;
        if (uri == null) throw ObjectConvert.ArgumentNull(nameof(uri));
        if (content == null) throw ObjectConvert.ArgumentNull(nameof(content));
        var c = new TokenRequest<TokenRequestBody>(content, appInfo, Scope);
        var q = c.ToQueryData();
        if (TokenRequestInfoValidator != null && !TokenRequestInfoValidator(q)) throw new InvalidOperationException("Cannot pass the request because it is not valid.");
        return CreateTokenResolveHttpClient().SendAsync(HttpMethod.Post, uri, q, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">requestUri was null, empty, or consists only of white-space characters; or, content was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    public Task<TokenInfo> ResolveTokenAsync(string requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
    {
        var uri = requestUri ?? TokenResolverUri?.OriginalString;
        if (string.IsNullOrWhiteSpace(uri)) throw ObjectConvert.ArgumentNull(nameof(uri));
        if (content == null) throw ObjectConvert.ArgumentNull(nameof(content));
        var c = new TokenRequest<TokenRequestBody>(content, appInfo, Scope);
        var q = c.ToQueryData();
        if (TokenRequestInfoValidator != null && !TokenRequestInfoValidator(q)) throw new InvalidOperationException("Cannot pass the request because it is not valid.");
        return CreateTokenResolveHttpClient().SendAsync(HttpMethod.Post, uri, q, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <typeparam name="T">The token info type.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">requestUri or content was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    public Task<T> ResolveTokenAsync<T>(Uri requestUri, TokenRequestBody content, CancellationToken cancellationToken = default) where T : TokenInfo
    {
        var uri = requestUri ?? TokenResolverUri;
        if (uri == null) throw ObjectConvert.ArgumentNull(nameof(uri));
        if (content == null) throw ObjectConvert.ArgumentNull(nameof(content));
        var c = new TokenRequest(content, appInfo, Scope);
        var q = c.ToQueryData();
        if (TokenRequestInfoValidator != null && !TokenRequestInfoValidator(q)) throw new InvalidOperationException("Cannot pass the request because it is not valid.");
        return CreateTokenResolveHttpClient<T>().SendAsync(HttpMethod.Post, uri, q, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <typeparam name="T">The token info type.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">requestUri was null, empty, or consists only of white-space characters; or, content was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="InvalidOperationException">Cannot pass the request by the TokenRequestInfoValidator because it is not valid.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    public Task<T> ResolveTokenAsync<T>(string requestUri, TokenRequestBody content, CancellationToken cancellationToken = default) where T : TokenInfo
    {
        var uri = requestUri ?? TokenResolverUri?.OriginalString;
        if (string.IsNullOrWhiteSpace(uri)) throw ObjectConvert.ArgumentNull(nameof(uri));
        if (content == null) throw ObjectConvert.ArgumentNull(nameof(content));
        var c = new TokenRequest(content, appInfo, Scope);
        var q = c.ToQueryData();
        if (TokenRequestInfoValidator != null && !TokenRequestInfoValidator(q)) throw new InvalidOperationException("Cannot pass the request because it is not valid.");
        return CreateTokenResolveHttpClient<T>().SendAsync(HttpMethod.Post, uri, q, cancellationToken);
    }

    /// <summary>
    /// Creates the JSON HTTP web client for resolving token.
    /// </summary>
    /// <returns>A result serialized.</returns>
    private JsonHttpClient<TokenInfo> CreateTokenResolveHttpClient()
    {
        var httpClient = new JsonHttpClient<TokenInfo>
        {
            SerializeEvenIfFailed = true,
            HttpClientResolver = AuthorizationHttpClientResolver ?? HttpClientResolver
        };
        httpClient.Received += (sender, ev) =>
        {
            if (!ev.IsSuccessStatusCode)
            {
                TokenResolved?.Invoke(sender, ev);
                return;
            }

            TokenResolvedTime = DateTime.Now;
            Token = ev.Result;
            TokenResolved?.Invoke(sender, ev);
        };
        if (Deserializer != null) httpClient.Deserializer = json => (TokenInfo)Deserializer(json, typeof(TokenInfo));
        if (TokenResolving != null) httpClient.Sending += (sender, ev) =>
        {
            TokenResolving?.Invoke(sender, ev);
        };
        return httpClient;
    }

    /// <summary>
    /// Creates the JSON HTTP web client for resolving token.
    /// </summary>
    /// <returns>A result serialized.</returns>
    private JsonHttpClient<T> CreateTokenResolveHttpClient<T>() where T : TokenInfo
    {
        var httpClient = new JsonHttpClient<T>
        {
            SerializeEvenIfFailed = true,
            HttpClientResolver = AuthorizationHttpClientResolver ?? HttpClientResolver
        };
        httpClient.Received += (sender, ev) =>
        {
            if (!ev.IsSuccessStatusCode)
            {
                TokenResolved?.Invoke(sender, ev.ConvertTo<TokenInfo>());
                return;
            }

            TokenResolvedTime = DateTime.Now;
            Token = ev.Result;
            TokenResolved?.Invoke(sender, ev.ConvertTo<TokenInfo>());
        };
        if (Deserializer != null) httpClient.Deserializer = json => (T)Deserializer(json, typeof(T));
        if (TokenResolving != null) httpClient.Sending += (sender, ev) =>
        {
            TokenResolving?.Invoke(sender, ev);
        };
        return httpClient;
    }

    /// <summary>
    /// Creats an HTTP web client.
    /// </summary>
    /// <returns>The HTTP web client.</returns>
    private HttpClient GetHttpClient()
    {
        var resolver = HttpClientResolver;
        if (resolver is null)
        {
            var ready = new InstanceObjectRef<HttpClient>(new());
            if (HttpClientResolver is null)
            {
                HttpClientResolver = ready;
                resolver = ready;
            }
            else
            {
                resolver = HttpClientResolver;
                if (resolver is null)
                {
                    HttpClientResolver = ready;
                    resolver = ready;
                }
            }
        }

        return resolver.GetInstance() ?? new();
    }
}

/// <summary>
/// The OAuth based JSON HTTP web client.
/// </summary>
public abstract class OAuthBasedClient : TokenContainer, IJsonHttpClientMaker
{
    /// <summary>
    /// The OAuth client.
    /// </summary>
    internal readonly OAuthClient oauth;

    /// <summary>
    /// Initializes a new instance of the OAuthBasedClient class.
    /// </summary>
    /// <param name="tokenCached">The token information instance cached.</param>
    protected OAuthBasedClient(TokenInfo tokenCached) : base(tokenCached)
    {
        oauth = new OAuthClient(tokenCached);
        oauth.TokenChanged += (sender, ev) =>
        {
            if (Token == ev.NewValue) return;
            Token = ev.NewValue;
        };
        TokenChanged += (sender, ev) =>
        {
            if (oauth.Token == ev.NewValue) return;
            oauth.Token = ev.NewValue;
        };
    }

    /// <summary>
    /// Initializes a new instance of the OAuthBasedClient class.
    /// </summary>
    /// <param name="appKey">The app accessing key.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    protected OAuthBasedClient(AppAccessingKey appKey, TokenInfo tokenCached = null) : base(tokenCached)
    {
        oauth = new OAuthClient(appKey, tokenCached);
        oauth.TokenChanged += (sender, ev) =>
        {
            if (Token == ev.NewValue) return;
            Token = ev.NewValue;
        };
        TokenChanged += (sender, ev) =>
        {
            if (Token == ev.NewValue) return;
            oauth.Token = ev.NewValue;
        };
    }

    /// <summary>
    /// Initializes a new instance of the OAuthBasedClient class.
    /// </summary>
    /// <param name="appId">The app id.</param>
    /// <param name="secretKey">The secret key.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    protected OAuthBasedClient(string appId, string secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OAuthBasedClient class.
    /// </summary>
    /// <param name="appId">The app id.</param>
    /// <param name="secretKey">The secret key.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    protected OAuthBasedClient(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
    {
    }

    /// <summary>
    /// Adds or removes a handler raised on sending.
    /// </summary>
    public event EventHandler<SendingEventArgs> Sending
    {
        add
        {
            oauth.Sending += value;
        }

        remove
        {
            oauth.Sending -= value;
        }
    }

    /// <summary>
    /// Adds or removes a handler raised on token is resolving.
    /// </summary>
    protected event EventHandler<SendingEventArgs> TokenResolving
    {
        add
        {
            oauth.TokenResolving += value;
        }

        remove
        {
            oauth.TokenResolving -= value;
        }
    }

    /// <summary>
    /// Adds or removes a handler raised on token has resolved.
    /// </summary>
    protected event EventHandler<ReceivedEventArgs<TokenInfo>> TokenResolved
    {
        add
        {
            oauth.TokenResolved += value;
        }

        remove
        {
            oauth.TokenResolved -= value;
        }
    }

    /// <summary>
    /// Gets or sets the token resolver URI.
    /// </summary>
    [JsonIgnore]
    protected Uri TokenResolverUri
    {
        get => oauth.TokenResolverUri;
        set => oauth.TokenResolverUri = value;
    }

    /// <summary>
    /// Gets the scope to use.
    /// </summary>
    [JsonIgnore]
    protected IList<string> Scope => oauth.Scope;

    /// <summary>
    /// Gets the scope string.
    /// </summary>
    [JsonIgnore]
    protected string ScopeString
    {
        get => oauth.ScopeString;
        set => oauth.ScopeString = value;
    }

    /// <summary>
    /// Gets or sets a handler to indicate whether the token request is valid and fill additional information if needed.
    /// </summary>
    [JsonIgnore]
    public Func<QueryData, bool> TokenRequestInfoValidator
    {
        get => oauth.TokenRequestInfoValidator;
        set => oauth.TokenRequestInfoValidator = value;
    }

    /// <summary>
    /// Gets or sets the case of authenticiation scheme.
    /// </summary>
    [JsonIgnore]
    public override Cases AuthenticationSchemeCase
    {
        get => oauth.AuthenticationSchemeCase;
        set => oauth.AuthenticationSchemeCase = value;
    }

    /// <summary>
    /// Gets or sets the converter of authentication header value.
    /// </summary>
    [JsonIgnore]
    public override Func<TokenInfo, Cases, AuthenticationHeaderValue> ConvertToAuthenticationHeaderValue
    {
        get => oauth.ConvertToAuthenticationHeaderValue;
        set => oauth.ConvertToAuthenticationHeaderValue = value;
    }

    /// <summary>
    /// Gets the app identifier.
    /// </summary>
    [JsonIgnore]
    public string AppId => oauth.AppId;

    /// <summary>
    /// Gets or sets the JSON deserializer.
    /// </summary>
    [JsonIgnore]
    public Func<string, Type, object> Deserializer
    {
        get => oauth.Deserializer;
        set => oauth.Deserializer = value;
    }

    /// <summary>
    /// Creates a JSON HTTP client.
    /// </summary>
    /// <typeparam name="T">The type of response.</typeparam>
    /// <returns>A new JSON HTTP client.</returns>
    public virtual JsonHttpClient<T> Create<T>(Action<ReceivedEventArgs<T>> callback = null)
        => oauth.Create(callback);

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => oauth.SendAsync(request, cancellationToken);

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
        => oauth.SendAsync(request, completionOption, cancellationToken);

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    public Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
        => Create<T>().SendAsync(request, cancellationToken);

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="callback">An optional callback raised on data received.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The request was null.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    public Task<T> SendAsync<T>(HttpRequestMessage request, Action<ReceivedEventArgs<T>> callback, CancellationToken cancellationToken = default)
        => Create<T>().SendAsync(request, callback, cancellationToken);

    /// <summary>
    /// Creates a token request instance by a specific body.
    /// The current app secret information will be filled into the instance.
    /// </summary>
    /// <typeparam name="T">The type of the body.</typeparam>
    /// <param name="body">The token request body.</param>
    /// <param name="scope">The additional scope.</param>
    /// <returns>A token request instance with given body and the current app secret information.</returns>
    protected TokenRequest<T> CreateTokenRequest<T>(T body, IEnumerable<string> scope = null) where T : TokenRequestBody
        => oauth?.CreateTokenRequest(body, scope) ?? new TokenRequest<T>(body, new AppAccessingKey(), scope);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    protected Task<TokenInfo> ResolveTokenAsync(TokenRequestBody content, CancellationToken cancellationToken = default)
        => oauth.ResolveTokenAsync(content, cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="oldTokenValidation">The validation of old token. The resolving will be in thread-safe mode.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    protected Task<TokenInfo> ResolveTokenAsync(TokenRequestBody content, Func<TokenInfo, bool> oldTokenValidation, CancellationToken cancellationToken = default)
        => oauth.ResolveTokenAsync(content, oldTokenValidation, cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    protected Task<TokenInfo> ResolveTokenAsync(Uri requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        => oauth.ResolveTokenAsync(requestUri, content, cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    protected Task<TokenInfo> ResolveTokenAsync(string requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        => oauth.ResolveTokenAsync(requestUri, content, cancellationToken);
}

/// <summary>
/// The OAuth based JSON HTTP web client.
/// </summary>
/// <typeparam name="T">The type of token info.</typeparam>
public abstract class OAuthBasedClient<T> : OAuthBasedClient where T : TokenInfo
{
    /// <summary>
    /// Initializes a new instance of the OAuthBasedClient class.
    /// </summary>
    /// <param name="tokenCached">The token information instance cached.</param>
    protected OAuthBasedClient(T tokenCached) : base(tokenCached)
    {
        oauth.TokenResolved += (sender, ev) =>
        {
            TokenResolved?.Invoke(sender, ev.ConvertTo<T>());
        };
    }

    /// <summary>
    /// Initializes a new instance of the OAuthBasedClient class.
    /// </summary>
    /// <param name="appKey">The app accessing key.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    protected OAuthBasedClient(AppAccessingKey appKey, T tokenCached = null) : base(appKey, tokenCached)
    {
        oauth.TokenResolved += (sender, ev) =>
        {
            TokenResolved?.Invoke(sender, ev.ConvertTo<T>());
        };
    }

    /// <summary>
    /// Initializes a new instance of the OAuthBasedClient class.
    /// </summary>
    /// <param name="appId">The app id.</param>
    /// <param name="secretKey">The secret key.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    protected OAuthBasedClient(string appId, string secretKey, T tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OAuthBasedClient class.
    /// </summary>
    /// <param name="appId">The app id.</param>
    /// <param name="secretKey">The secret key.</param>
    /// <param name="tokenCached">The token information instance cached.</param>
    protected OAuthBasedClient(string appId, SecureString secretKey, T tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
    {
    }

    /// <summary>
    /// Adds or removes a handler raised on token has resolved.
    /// </summary>
    protected new event EventHandler<ReceivedEventArgs<T>> TokenResolved;

    /// <summary>
    /// Gets the type of token info.
    /// </summary>
    /// <returns>The type of token info.</returns>
    public Type GetTokenInfoType()
        => Token?.GetType() ?? typeof(T);

    /// <summary>
    /// Gets or sets the converter of authentication header value.
    /// </summary>
    protected new Func<T, Cases, AuthenticationHeaderValue> ConvertToAuthenticationHeaderValue
    {
        get
        {
            return oauth.ConvertToAuthenticationHeaderValue;
        }

        set
        {
            if (value == null)
            {
                oauth.ConvertToAuthenticationHeaderValue = null;
                return;
            }

            oauth.ConvertToAuthenticationHeaderValue = (tokenInfo, authCase) =>
            {
                var ti = Token;
                if (tokenInfo != ti)
                {
                    ti = tokenInfo as T;
                    if (ti == null) return null;
                }

                return value(ti, authCase);
            };
        }
    }

    /// <summary>
    /// Gets the token information instance saved in this container.
    /// </summary>
    [JsonIgnore]
    public new T Token
    {
        get
        {
            return (T)base.Token;
        }

        protected set
        {
            base.Token = value;
        }
    }

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    protected new Task<T> ResolveTokenAsync(TokenRequestBody content, CancellationToken cancellationToken = default)
        => oauth.ResolveTokenAsync<T>(content, cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="oldTokenValidation">The validation of old token. The resolving will be in thread-safe mode.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    protected new Task<T> ResolveTokenAsync(TokenRequestBody content, Func<TokenInfo, bool> oldTokenValidation, CancellationToken cancellationToken = default)
        => oauth.ResolveTokenAsync<T>(content, oldTokenValidation, cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    protected new Task<T> ResolveTokenAsync(Uri requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        => oauth.ResolveTokenAsync<T>(requestUri, content, cancellationToken);

    /// <summary>
    /// Sends a POST request and gets the result serialized by JSON.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    /// <returns>A result serialized.</returns>
    /// <exception cref="ArgumentNullException">content was null.</exception>
    /// <exception cref="InvalidOperationException">TokenResolverUri property was null.</exception>
    /// <exception cref="FailedHttpException">HTTP response contains failure status code.</exception>
    /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
    /// <exception cref="OperationCanceledException">The task is cancelled.</exception>
    protected new Task<T> ResolveTokenAsync(string requestUri, TokenRequestBody content, CancellationToken cancellationToken = default)
        => oauth.ResolveTokenAsync<T>(requestUri, content, cancellationToken);
}
