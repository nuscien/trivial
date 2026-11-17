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
/// The property key and value consts.
/// </summary>
public static class TokenPropertyKeys
{
    /// <summary>
    /// The error codes of token response.
    /// </summary>
    public static class ErrorCodes
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
        /// The operation is not valid to proceed.
        /// </summary>
        public const string InvalidOperation = "invalid_operation";

        /// <summary>
        /// The resource does not exist.
        /// </summary>
        public const string NotFound = "not_found";

        /// <summary>
        /// The client is not authorized to request an access token using this method.
        /// </summary>
        public const string UnauthorizedClientTraditional = "UnauthorizedClient";

        /// <summary>
        /// The client is not authorized to request an access token using this method.
        /// </summary>
        public const string UnauthorizedClient = "unauthorized_client";

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
        /// The requested format is unsupported.
        /// </summary>
        public const string UnsupportedFormat = "unsupported_format";

        /// <summary>
        /// The requested format is unsupported.
        /// </summary>
        public const string UnsupportedContentType = "unsupported_content_type";

        /// <summary>
        /// The requested scope is invalid, unknown, or malformed.
        /// </summary>
        public const string InvalidScope = "invalid_scope";

        /// <summary>
        /// The requested format is invalid, unknown, or malformed.
        /// </summary>
        public const string InvalidFormat = "invalid_format";

        /// <summary>
        /// The requested content type is invalid, unknown, or malformed.
        /// </summary>
        public const string InvalidContentType = "invalid_content_type";

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

        /// <summary>
        /// The operation is timeout.
        /// </summary>
        public const string Timeout = "timeout";

        /// <summary>
        /// The task is aborted or canceled.
        /// </summary>
        public const string Aborted = "aborted";
    }

    /// <summary>
    /// The user identifier property name.
    /// </summary>
    public const string Token = "token";

    /// <summary>
    /// The user identifier property name.
    /// </summary>
    public const string UserId = "user_id";

    /// <summary>
    /// The property name of resource.
    /// </summary>
    public const string Resource = "resource";

    /// <summary>
    /// The resource identifier property name.
    /// </summary>
    public const string ResourceId = "res_id";

    /// <summary>
    /// The resource name property name.
    /// </summary>
    public const string ResourceName = "resource_name";

    /// <summary>
    /// The user name property name.
    /// </summary>
    public const string UserName = "username";

    /// <summary>
    /// The grant type value of password.
    /// </summary>
    public const string Password = "password";

    /// <summary>
    /// The LDAP (Lightweight Directory Access Protocol) property name.
    /// </summary>
    public const string Ldap = "ldap";

    /// <summary>
    /// The token reference identifier property name.
    /// </summary>
    public const string TokenId = "token_id";

    /// <summary>
    /// The access token property name.
    /// </summary>
    public const string AccessToken = "access_token";

    /// <summary>
    /// The token type property name.
    /// </summary>
    public const string TokenType = "token_type";

    /// <summary>
    /// The refresh token property name.
    /// </summary>
    public const string RefreshToken = "refresh_token";

    /// <summary>
    /// The expires in property name.
    /// </summary>
    public const string ExpiresIn = "expires_in";

    /// <summary>
    /// The ID token property name.
    /// </summary>
    public const string IdToken = "id_token";

    /// <summary>
    /// The error code property name.
    /// </summary>
    public const string ErrorCode = "error";

    /// <summary>
    /// The error description property name.
    /// </summary>
    public const string ErrorDescription = "error_description";

    /// <summary>
    /// The error description property name.
    /// </summary>
    public const string ErrorUri = "error_uri";

    /// <summary>
    /// The state property name.
    /// </summary>
    public const string State = "state";

    /// <summary>
    /// The scope property name.
    /// </summary>
    public const string Scope = "scope";

    /// <summary>
    /// The property name of bearer methods supported.
    /// </summary>
    public const string BearerMethodsSupported = "bearer_methods_supported";

    /// <summary>
    /// The property name of scopes supported.
    /// </summary>
    public const string ScopesSupported = "scopes_supported";

    /// <summary>
    /// The token type.
    /// </summary>
    public const string BearerTokenType = "Bearer";

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
    /// The response type property name.
    /// </summary>
    public const string ResponseType = "response_type";
    
    /// <summary>
    /// The property name of authorization servers.
    /// </summary>
    public const string AuthorizationServers = "authorization_servers";

    /// <summary>
    /// The token type of basic.
    /// </summary>
    public const string Basic = "Basic";
}
