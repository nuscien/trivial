using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Data;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Security;

/// <summary>
/// The server route of token request handler.
/// </summary>
/// <typeparam name="T">The type of account information.</typeparam>
/// <example>
/// <code>
/// // Create a route and register the handlers.
/// var route = new TokenRequestRoute&lt;UserInfo&gt;();
/// route.OnRegisterInternal((PasswordTokenRequestBody req, CancellationToken cancellationToken)
///     => UserManager.LoginByPasswordAsync(req.UserName, req.Password, cancellationToken));
/// route.OnRegisterInternal((RefreshTokenRequestBody req, CancellationToken cancellationToken)
///     => UserManager.LoginByRefreshTokenAsync(req.RefreshToken, cancellationToken));
///
/// // Then you can handle following login request.
/// var resp = await route.SignInAsync(tokenReq);
/// </code>
/// </example>
[Guid("1E9E554C-82DB-4D5C-8C5A-A0D054D17426")]
public class TokenRequestRoute<T>
{
    /// <summary>
    /// The handlers.
    /// </summary>
    private readonly Dictionary<string, Func<QueryData, CancellationToken, Task<SelectionRelationship<T, TokenInfo>>>> handlers = new();

    /// <summary>
    /// Adds or removes the event handler after signing in.
    /// </summary>
    public event DataEventHandler<SelectionRelationship<T, TokenInfo>> SignedIn;

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="h">The handler.</param>
    public void Register(string grantType, Func<QueryData, CancellationToken, Task<SelectionRelationship<T, TokenInfo>>> h)
        => handlers[grantType] = h;

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="h">The handler.</param>
    public void Register(string grantType, Func<TokenRequest, CancellationToken, Task<SelectionRelationship<T, TokenInfo>>> h)
        => handlers[grantType] = (q, cancellationToken) =>
        {
            var info = new TokenRequest(q);
            return h(info, cancellationToken);
        };

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="tokenRequestFactory">The token request factory.</param>
    /// <param name="h">The handler.</param>
    public void Register<TTokenRequest>(string grantType, Func<QueryData, TTokenRequest> tokenRequestFactory, Func<TTokenRequest, CancellationToken, Task<SelectionRelationship<T, TokenInfo>>> h) where TTokenRequest : TokenRequest
        => handlers[grantType] = (q, cancellationToken) =>
        {
            var info = tokenRequestFactory(q);
            if (info == null) return Task.FromResult<SelectionRelationship<T, TokenInfo>>(new());
            return h(info, cancellationToken);
        };

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<PasswordTokenRequestBody>, CancellationToken, Task<SelectionRelationship<T, TokenInfo>>> h)
        => Register(PasswordTokenRequestBody.PasswordGrantType, PasswordTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<ClientTokenRequestBody>, CancellationToken, Task<SelectionRelationship<T, TokenInfo>>> h)
        => Register(ClientTokenRequestBody.ClientCredentialsGrantType, ClientTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<CodeTokenRequestBody>, CancellationToken, Task<SelectionRelationship<T, TokenInfo>>> h)
        => Register(CodeTokenRequestBody.AuthorizationCodeGrantType, CodeTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<RefreshTokenRequestBody>, CancellationToken, Task<SelectionRelationship<T, TokenInfo>>> h)
        => Register(RefreshTokenRequestBody.RefreshTokenGrantType, RefreshTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<PasswordTokenRequestBody>, CancellationToken, Task<BaseAccountTokenInfo<T>>> h)
        => Register(PasswordTokenRequestBody.PasswordGrantType, PasswordTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<ClientTokenRequestBody>, CancellationToken, Task<BaseAccountTokenInfo<T>>> h)
        => Register(ClientTokenRequestBody.ClientCredentialsGrantType, ClientTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<CodeTokenRequestBody>, CancellationToken, Task<BaseAccountTokenInfo<T>>> h)
        => Register(CodeTokenRequestBody.AuthorizationCodeGrantType, CodeTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<RefreshTokenRequestBody>, CancellationToken, Task<BaseAccountTokenInfo<T>>> h)
        => Register(RefreshTokenRequestBody.RefreshTokenGrantType, RefreshTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="h">The handler.</param>
    public void Register(string grantType, Func<QueryData, CancellationToken, Task<BaseAccountTokenInfo<T>>> h)
        => handlers[grantType] = async (q, cancellationToken) =>
        {
            var resp = await h(q, cancellationToken);
            return new(resp.User, resp);
        };

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="h">The handler.</param>
    public void Register(string grantType, Func<TokenRequest, Task<BaseAccountTokenInfo<T>>> h)
        => handlers[grantType] = async (q, cancellationToken) =>
        {
            var info = new TokenRequest(q);
            var resp = await h(info);
            return new(resp.User, resp);
        };

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="tokenRequestFactory">The token request factory.</param>
    /// <param name="h">The handler.</param>
    public void Register<TTokenRequest>(string grantType, Func<QueryData, TTokenRequest> tokenRequestFactory, Func<TTokenRequest, Task<BaseAccountTokenInfo<T>>> h) where TTokenRequest : TokenRequest
        => handlers[grantType] = async (q, cancellationToken) =>
        {
            var info = tokenRequestFactory(q);
            if (info == null) return new();
            var resp = await h(info);
            return new(resp.User, resp);
        };

    /// <summary>
    /// Removes a handler.
    /// </summary>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(string method)
        => method != null && handlers.Remove(method);

    /// <summary>
    /// Removes a set of handler.
    /// </summary>
    /// <param name="methods">The method name.</param>
    /// <returns>The count of items removed.</returns>
    public int Remove(IEnumerable<string> methods)
    {
        var i = 0;
        foreach (var method in methods)
        {
            if (Remove(method)) i++;
        }

        return i;
    }

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="tokenRequest">The token request information.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A token information.</returns>
    public Task<SelectionRelationship<T, TokenInfo>> SignInAsync(TokenRequest tokenRequest, CancellationToken cancellationToken = default)
    {
        var q = tokenRequest?.ToQueryData();
        return SignInAsync(q, cancellationToken);
    }

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="s">The token request information.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A token information.</returns>
    public Task<SelectionRelationship<T, TokenInfo>> SignInAsync(string s, CancellationToken cancellationToken = default)
    {
        var q = QueryData.Parse(s);
        return SignInAsync(q, cancellationToken);
    }

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="s">The token request information.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A token information.</returns>
    public Task<SelectionRelationship<T, TokenInfo>> SignInAsync(ReadOnlySpan<char> s, CancellationToken cancellationToken = default)
    {
        var q = QueryData.Parse(s);
        return SignInAsync(q, cancellationToken);
    }

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="q">The token request information.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A token information.</returns>
    public async Task<SelectionRelationship<T, TokenInfo>> SignInAsync(QueryData q, CancellationToken cancellationToken = default)
    {
        if (q == null) return null;
        var grantType = q[TokenRequestBody.GrantTypeProperty];
        if (string.IsNullOrWhiteSpace(grantType)) return null;
        var info = handlers.TryGetValue(grantType, out var h) ? await h(q, cancellationToken) : grantType.ToLowerInvariant() switch
        {
            ClientTokenRequestBody.ClientCredentialsGrantType => await TokenInfoExtensions.ProcessAsync(q, ClientTokenRequestBody.Create, SignInAsync, cancellationToken),
            CodeTokenRequestBody.AuthorizationCodeGrantType => await TokenInfoExtensions.ProcessAsync(q, CodeTokenRequestBody.Create, SignInAsync, cancellationToken),
            RefreshTokenRequestBody.RefreshTokenGrantType => await TokenInfoExtensions.ProcessAsync(q, RefreshTokenRequestBody.Create, SignInAsync, cancellationToken),
            PasswordTokenRequestBody.PasswordGrantType => await TokenInfoExtensions.ProcessAsync(q, PasswordTokenRequestBody.Create, SignInAsync, cancellationToken),
            _ => null
        };
        if (info is not null) SignedIn?.Invoke(this, new DataEventArgs<SelectionRelationship<T, TokenInfo>>(info));
        return info;
    }

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="stream">The stream input.</param>
    /// <param name="encoding">The encoding of stream; or null, by default, to use UTF-8.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The login response.</returns>
    /// <exception cref="ArgumentException">stream does not support reading.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
    public async Task<SelectionRelationship<T, TokenInfo>> SignInAsync(Stream stream, Encoding encoding, CancellationToken cancellationToken)
    {
        if (stream == null) return null;
        string input;
        using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
        {
#if NETCOREAPP
            input = await reader.ReadToEndAsync(cancellationToken);
#else
            input = await reader.ReadToEndAsync();
#endif
        }

        return await SignInAsync(input, cancellationToken);
    }

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="utf8Stream">The UTF-8 stream input.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>A token information.</returns>
    public Task<SelectionRelationship<T, TokenInfo>> SignInAsync(Stream utf8Stream, CancellationToken cancellationToken = default)
        => SignInAsync(utf8Stream, Encoding.UTF8, cancellationToken);

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The login response.</returns>
    protected virtual Task<SelectionRelationship<T, TokenInfo>> SignInAsync(TokenRequest<ClientTokenRequestBody> request, CancellationToken cancellationToken)
        => EmptyAsync();

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The login response.</returns>
    protected virtual Task<SelectionRelationship<T, TokenInfo>> SignInAsync(TokenRequest<CodeTokenRequestBody> request, CancellationToken cancellationToken)
        => EmptyAsync();

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The login response.</returns>
    protected virtual Task<SelectionRelationship<T, TokenInfo>> SignInAsync(TokenRequest<RefreshTokenRequestBody> request, CancellationToken cancellationToken)
        => EmptyAsync();

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The login response.</returns>
    protected virtual Task<SelectionRelationship<T, TokenInfo>> SignInAsync(TokenRequest<PasswordTokenRequestBody> request, CancellationToken cancellationToken)
        => EmptyAsync();

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of token request body.</typeparam>
    /// <param name="grantType">The grant type.</param>
    /// <param name="factory">The factory of token request body.</param>
    /// <param name="h">A token request handler.</param>
    private void Register<TRequestBody>(string grantType, Func<QueryData, TRequestBody> factory, Func<TokenRequest<TRequestBody>, CancellationToken, Task<SelectionRelationship<T, TokenInfo>>> h) where TRequestBody : TokenRequestBody
    {
        if (h == null) return;
        Register(grantType, (q, cancellationToken) => TokenInfoExtensions.ProcessAsync(q, factory, h, cancellationToken));
    }

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of token request body.</typeparam>
    /// <param name="grantType">The grant type.</param>
    /// <param name="factory">The factory of token request body.</param>
    /// <param name="h">A token request handler.</param>
    private void Register<TRequestBody>(string grantType, Func<QueryData, TRequestBody> factory, Func<TokenRequest<TRequestBody>, CancellationToken, Task<BaseAccountTokenInfo<T>>> h) where TRequestBody : TokenRequestBody
    {
        if (h == null) return;
        Register(grantType, (q, cancellationToken) => TokenInfoExtensions.ProcessAsync(q, factory, h, cancellationToken));
    }

    /// <summary>
    /// Creates an empty login response.
    /// </summary>
    /// <returns>An empty login response.</returns>
    private static Task<SelectionRelationship<T, TokenInfo>> EmptyAsync()
        => Task.FromResult<SelectionRelationship<T, TokenInfo>>(null);
}

/// <summary>
/// The helper for token info and request message.
/// </summary>
public static class TokenInfoExtensions
{
    /// <summary>
    /// Gets the login URI.
    /// </summary>
    /// <param name="request">The source.</param>
    /// <param name="uri">The base URI.</param>
    /// <param name="responseType">The response type.</param>
    /// <param name="state">The state.</param>
    /// <returns>The login URI.</returns>
    public static Uri GetLoginUri(this TokenRequest<CodeTokenRequestBody> request, Uri uri, string responseType, string state)
    {
        if (request?.Body is null) return null;
        var data = new QueryData
        {
            { CodeTokenRequestBody.ResponseType, responseType },
            { TokenRequestBody.ClientIdProperty, request.ClientId },
            { CodeTokenRequestBody.RedirectUriProperty, request.Body.RedirectUri?.OriginalString },
            { TokenInfo.ScopeProperty, request.ScopeString },
            { CodeTokenRequestBody.StateProperty, state }
        };
        return new Uri(data.ToString(uri.OriginalString));
    }

    /// <summary>
    /// Creates a JSON web token instance.
    /// </summary>
    /// <typeparam name="T">The type of payload.</typeparam>
    /// <param name="header">The customized header; or null, to generate automatically by default policy.</param>
    /// <param name="payload">The payload of JSON web token.</param>
    /// <param name="sign">The signature provider.</param>
    /// <returns>An instance of the JSON web token.</returns>
    public static JsonWebToken<T> CreateJsonWebToken<T>(JsonWebTokenHeader header, T payload, ISignatureProvider sign)
        => new(header, payload, sign);

    /// <summary>
    /// Creates a JSON web token instance.
    /// </summary>
    /// <typeparam name="T">The type of payload.</typeparam>
    /// <param name="payload">The payload of JSON web token.</param>
    /// <param name="sign">The signature provider.</param>
    /// <returns>An instance of the JSON web token.</returns>
    public static JsonWebToken<T> CreateJsonWebToken<T>(T payload, ISignatureProvider sign)
        => new(payload, sign);

    /// <summary>
    /// Creates a JSON web token instance.
    /// </summary>
    /// <param name="payload">The payload of JSON web token.</param>
    /// <param name="fill">The handler to fill additional properties or update current ones to payload.</param>
    /// <param name="sign">The signature provider.</param>
    /// <returns>An instance of the JSON web token.</returns>
    public static JsonWebToken<JsonObjectNode> CreateJsonWebToken(JsonWebTokenPayload payload, Action<JsonObjectNode> fill, ISignatureProvider sign)
    {
        var obj = JsonObjectNode.ConvertFrom(payload) ?? new();
        fill?.Invoke(obj);
        return new(obj, sign);
    }

    /// <summary>
    /// Processes a token request handler into a route.
    /// </summary>
    /// <typeparam name="TUser">The type of entity.</typeparam>
    /// <typeparam name="TRequestBody">The type of token request body.</typeparam>
    /// <param name="q">The input query data.</param>
    /// <param name="factory">The factory of token request body.</param>
    /// <param name="h">A token request handler.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <exception cref="InvalidOperationException">The grant type is not expected.</exception>
    internal static Task<SelectionRelationship<TUser, TokenInfo>> ProcessAsync<TUser, TRequestBody>(QueryData q, Func<QueryData, TRequestBody> factory, Func<TokenRequest<TRequestBody>, CancellationToken, Task<SelectionRelationship<TUser, TokenInfo>>> h, CancellationToken cancellationToken) where TRequestBody : TokenRequestBody
    {
        if (h is null) return Task.FromResult<SelectionRelationship<TUser, TokenInfo>>(new());
        var body = factory(q);
        var req = new TokenRequest<TRequestBody>(body, q);
        return h(req, cancellationToken);
    }

    /// <summary>
    /// Processes a token request handler into a route.
    /// </summary>
    /// <typeparam name="TUser">The type of entity.</typeparam>
    /// <typeparam name="TRequestBody">The type of token request body.</typeparam>
    /// <param name="q">The input query data.</param>
    /// <param name="factory">The factory of token request body.</param>
    /// <param name="h">A token request handler.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <exception cref="InvalidOperationException">The grant type is not expected.</exception>
    internal static async Task<SelectionRelationship<TUser, TokenInfo>> ProcessAsync<TUser, TRequestBody>(QueryData q, Func<QueryData, TRequestBody> factory, Func<TokenRequest<TRequestBody>, CancellationToken, Task<BaseAccountTokenInfo<TUser>>> h, CancellationToken cancellationToken) where TRequestBody : TokenRequestBody
    {
        if (h is null) return new();
        var body = factory(q);
        var req = new TokenRequest<TRequestBody>(body, q);
        var resp = await h(req, cancellationToken);
        return new(resp.User, resp);
    }
}
