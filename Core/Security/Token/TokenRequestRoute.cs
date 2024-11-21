using System;
using System.Collections.Generic;
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
using static System.Net.WebRequestMethods;

namespace Trivial.Security;

/// <summary>
/// The server route of token request handler.
/// </summary>
/// <typeparam name="T">The type of account information.</typeparam>
public class TokenRequestRoute<T>
{
    /// <summary>
    /// The handlers.
    /// </summary>
    private readonly Dictionary<string, Func<QueryData, Task<SelectionRelationship<T, TokenInfo>>>> handlers = new();

    /// <summary>
    /// Adds or removes the event handler after signing in.
    /// </summary>
    public event DataEventHandler<SelectionRelationship<T, TokenInfo>> SignedIn;

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="h">The handler.</param>
    public void Register(string grantType, Func<QueryData, Task<SelectionRelationship<T, TokenInfo>>> h)
        => handlers[grantType] = h;

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="h">The handler.</param>
    public void Register(string grantType, Func<TokenRequest, Task<SelectionRelationship<T, TokenInfo>>> h)
        => handlers[grantType] = q =>
        {
            var info = new TokenRequest(q);
            return h(info);
        };

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="tokenRequestFactory">The token request factory.</param>
    /// <param name="h">The handler.</param>
    public void Register<TTokenRequest>(string grantType, Func<QueryData, TTokenRequest> tokenRequestFactory, Func<TTokenRequest, Task<SelectionRelationship<T, TokenInfo>>> h) where TTokenRequest : TokenRequest
        => handlers[grantType] = q =>
        {
            var info = tokenRequestFactory(q);
            if (info == null) return Task.FromResult<SelectionRelationship<T, TokenInfo>>(new());
            return h(info);
        };

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<ClientTokenRequestBody>, Task<SelectionRelationship<T, TokenInfo>>> h)
        => Register(ClientTokenRequestBody.ClientCredentialsGrantType, ClientTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<CodeTokenRequestBody>, Task<SelectionRelationship<T, TokenInfo>>> h)
        => Register(CodeTokenRequestBody.AuthorizationCodeGrantType, CodeTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<RefreshTokenRequestBody>, Task<SelectionRelationship<T, TokenInfo>>> h)
        => Register(RefreshTokenRequestBody.RefreshTokenGrantType, RefreshTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<PasswordTokenRequestBody>, Task<BaseAccountTokenInfo<T>>> h)
        => Register(PasswordTokenRequestBody.PasswordGrantType, PasswordTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<ClientTokenRequestBody>, Task<BaseAccountTokenInfo<T>>> h)
        => Register(ClientTokenRequestBody.ClientCredentialsGrantType, ClientTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<CodeTokenRequestBody>, Task<BaseAccountTokenInfo<T>>> h)
        => Register(CodeTokenRequestBody.AuthorizationCodeGrantType, CodeTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<RefreshTokenRequestBody>, Task<BaseAccountTokenInfo<T>>> h)
        => Register(RefreshTokenRequestBody.RefreshTokenGrantType, RefreshTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <param name="h">A token request handler.</param>
    public void Register(Func<TokenRequest<PasswordTokenRequestBody>, Task<SelectionRelationship<T, TokenInfo>>> h)
        => Register(PasswordTokenRequestBody.PasswordGrantType, PasswordTokenRequestBody.Create, h);

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="h">The handler.</param>
    public void Register(string grantType, Func<QueryData, Task<BaseAccountTokenInfo<T>>> h)
        => handlers[grantType] = async q =>
        {
            var resp = await h(q);
            return new(resp.User, resp);
        };

    /// <summary>
    /// Registers a handler.
    /// </summary>
    /// <param name="grantType">The grant type.</param>
    /// <param name="h">The handler.</param>
    public void Register(string grantType, Func<TokenRequest, Task<BaseAccountTokenInfo<T>>> h)
        => handlers[grantType] = async q =>
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
        => handlers[grantType] = async q =>
        {
            var info = tokenRequestFactory(q);
            if (info == null) return new();
            var resp = await h(info);
            return new(resp.User, resp);
        };

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="tokenRequest">The token request information.</param>
    /// <returns>A token information.</returns>
    public Task<SelectionRelationship<T, TokenInfo>> SignInAsync(TokenRequest tokenRequest)
    {
        var q = tokenRequest?.ToQueryData();
        return SignInAsync(q);
    }

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="s">The token request information.</param>
    /// <returns>A token information.</returns>
    public Task<SelectionRelationship<T, TokenInfo>> SignInAsync(string s)
    {
        var q = QueryData.Parse(s);
        return SignInAsync(q);
    }

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="q">The token request information.</param>
    /// <returns>A token information.</returns>
    public async Task<SelectionRelationship<T, TokenInfo>> SignInAsync(QueryData q)
    {
        if (q == null) return null;
        var grantType = q[TokenRequestBody.GrantTypeProperty];
        if (string.IsNullOrWhiteSpace(grantType)) return null;
        if (!handlers.TryGetValue(grantType, out var h)) return grantType switch
        {
            ClientTokenRequestBody.ClientCredentialsGrantType => await TokenInfoExtensions.ProcessAsync(q, ClientTokenRequestBody.Create, SignInAsync),
            CodeTokenRequestBody.AuthorizationCodeGrantType => await TokenInfoExtensions.ProcessAsync(q, CodeTokenRequestBody.Create, SignInAsync),
            RefreshTokenRequestBody.RefreshTokenGrantType => await TokenInfoExtensions.ProcessAsync(q, RefreshTokenRequestBody.Create, SignInAsync),
            PasswordTokenRequestBody.PasswordGrantType => await TokenInfoExtensions.ProcessAsync(q, PasswordTokenRequestBody.Create, SignInAsync),
            _ => null
        };
        var info = await h(q);
        if (info is not null) SignedIn?.Invoke(this, new DataEventArgs<SelectionRelationship<T, TokenInfo>>(info));
        return info;
    }

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="utf8Stream">The UTF-8 stream input.</param>
    /// <returns>The login response.</returns>
    /// <exception cref="ArgumentException">stream does not support reading.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
    public async Task<SelectionRelationship<T, TokenInfo>> SignInAsync(Stream utf8Stream)
    {
        if (utf8Stream == null) return null;
        string input;
        using (var reader = new StreamReader(utf8Stream, Encoding.UTF8))
        {
            input = await reader.ReadToEndAsync();
        }

        return await SignInAsync(input);
    }

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The login response.</returns>
    protected virtual Task<SelectionRelationship<T, TokenInfo>> SignInAsync(TokenRequest<ClientTokenRequestBody> request)
        => EmptyAsync();

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The login response.</returns>
    protected virtual Task<SelectionRelationship<T, TokenInfo>> SignInAsync(TokenRequest<CodeTokenRequestBody> request)
        => EmptyAsync();

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The login response.</returns>
    protected virtual Task<SelectionRelationship<T, TokenInfo>> SignInAsync(TokenRequest<RefreshTokenRequestBody> request)
        => EmptyAsync();

    /// <summary>
    /// Signs in.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The login response.</returns>
    protected virtual Task<SelectionRelationship<T, TokenInfo>> SignInAsync(TokenRequest<PasswordTokenRequestBody> request)
        => EmptyAsync();

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of token request body.</typeparam>
    /// <param name="grantType">The grant type.</param>
    /// <param name="factory">The factory of token request body.</param>
    /// <param name="h">A token request handler.</param>
    private void Register<TRequestBody>(string grantType, Func<QueryData, TRequestBody> factory, Func<TokenRequest<TRequestBody>, Task<SelectionRelationship<T, TokenInfo>>> h) where TRequestBody : TokenRequestBody
    {
        if (h == null) return;
        Register(grantType, q => TokenInfoExtensions.ProcessAsync(q, factory, h));
    }

    /// <summary>
    /// Registers a token request handler into a route.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of token request body.</typeparam>
    /// <param name="grantType">The grant type.</param>
    /// <param name="factory">The factory of token request body.</param>
    /// <param name="h">A token request handler.</param>
    private void Register<TRequestBody>(string grantType, Func<QueryData, TRequestBody> factory, Func<TokenRequest<TRequestBody>, Task<BaseAccountTokenInfo<T>>> h) where TRequestBody : TokenRequestBody
    {
        if (h == null) return;
        Register(grantType, q => TokenInfoExtensions.ProcessAsync(q, factory, h));
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
    public static Uri GetLoginUri(TokenRequest<CodeTokenRequestBody> request, Uri uri, string responseType, string state)
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
    /// Processes a token request handler into a route.
    /// </summary>
    /// <typeparam name="TUser">The type of entity.</typeparam>
    /// <typeparam name="TRequestBody">The type of token request body.</typeparam>
    /// <param name="q">The input query data.</param>
    /// <param name="factory">The factory of token request body.</param>
    /// <param name="h">A token request handler.</param>
    /// <exception cref="InvalidOperationException">The grant type is not expected.</exception>
    internal static Task<SelectionRelationship<TUser, TokenInfo>> ProcessAsync<TUser, TRequestBody>(QueryData q, Func<QueryData, TRequestBody> factory, Func<TokenRequest<TRequestBody>, Task<SelectionRelationship<TUser, TokenInfo>>> h) where TRequestBody : TokenRequestBody
    {
        if (h is null) return Task.FromResult<SelectionRelationship<TUser, TokenInfo>>(new());
        var body = factory(q);
        var req = new TokenRequest<TRequestBody>(body, q);
        return h(req);
    }

    /// <summary>
    /// Processes a token request handler into a route.
    /// </summary>
    /// <typeparam name="TUser">The type of entity.</typeparam>
    /// <typeparam name="TRequestBody">The type of token request body.</typeparam>
    /// <param name="q">The input query data.</param>
    /// <param name="factory">The factory of token request body.</param>
    /// <param name="h">A token request handler.</param>
    /// <exception cref="InvalidOperationException">The grant type is not expected.</exception>
    internal static async Task<SelectionRelationship<TUser, TokenInfo>> ProcessAsync<TUser, TRequestBody>(QueryData q, Func<QueryData, TRequestBody> factory, Func<TokenRequest<TRequestBody>, Task<BaseAccountTokenInfo<TUser>>> h) where TRequestBody : TokenRequestBody
    {
        if (h is null) return new();
        var body = factory(q);
        var req = new TokenRequest<TRequestBody>(body, q);
        var resp = await h(req);
        return new(resp.User, resp);
    }
}
