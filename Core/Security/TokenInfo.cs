// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessTokenResponse.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The access token response information.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Runtime.Serialization;

using Trivial.Text;
using Trivial.Data;

namespace Trivial.Security
{
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
            AdditionalData = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        [DataMember(Name = AccessTokenProperty)]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the token type.
        /// </summary>
        [DataMember(Name = TokenTypeProperty)]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the expiration seconds.
        /// </summary>
        [DataMember(Name = ExpiresInProperty)]
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
        /// Gets or sets the expiration seconds.
        /// </summary>
        public TimeSpan? ExpiredAfter { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        [DataMember(Name = RefreshTokenProperty)]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [DataMember(Name = ErrorCodeProperty)]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error description.
        /// Human-readable text providing additional information,
        /// used to assist the client developer in understanding the error that occurred.
        /// </summary>
        [DataMember(Name = ErrorDescriptionProperty)]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the error URI in string.
        /// A URI identifying a human-readable web page with information about the error,
        /// used to provide the client developer with additional information about the error.
        /// </summary>
        [DataMember(Name = ErrorUriProperty)]
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
                    ErrorUri = !string.IsNullOrWhiteSpace(value) ? new Uri(value) : null;
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
        public Uri ErrorUri { get; set; }

        /// <summary>
        /// Gets or sets the state sent by client authorization request.
        /// </summary>
        [DataMember(Name = StateProperty)]
        public virtual string State { get; set; }

        /// <summary>
        /// Gets or sets the resource identifier.
        /// </summary>
        [DataMember(Name = ResourceIdProperty)]
        public virtual string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        [DataMember(Name = UserIdProperty)]
        public virtual string UserId { get; set; }

        /// <summary>
        /// Gets or sets the scope string.
        /// </summary>
        [DataMember(Name = ScopeProperty)]
        public string ScopeString
        {
            get
            {
                return Scope.Count > 0 ? string.Join(" ", Scope) : null;
            }

            set
            {
                Scope.Clear();
                if (string.IsNullOrWhiteSpace(value)) return;
                foreach (var ele in value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Scope.Add(ele);
                }
            }
        }

        /// <summary>
        /// Gets the permission scope.
        /// </summary>
        public IList<string> Scope { get; } = new List<string>();

        /// <summary>
        /// Gets the additional data.
        /// </summary>
        public Dictionary<string, string> AdditionalData { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the access token is null, empty or consists only of white-space characters.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(AccessToken);

        /// <summary>
        /// Tries to get string value of additional data.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <returns>A string value of additional data; or null, if no such property.</returns>
        public string TryGetAdditionalValue(string key)
        {
            return AdditionalData.TryGetValue(key, out string resultStr) ? resultStr : null;
        }

        /// <summary>
        /// Returns a System.String that represents the current TokenInfo.
        /// </summary>
        /// <returns>A System.String that represents the current TokenInfo.</returns>
        public override string ToString()
        {
            return $"{TokenType} {AccessToken}".Trim();
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.
        /// </summary>
        /// <param name="schemeCase">The scheme case.</param>
        /// <param name="parameterCase">The parameter case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.</returns>
        public AuthenticationHeaderValue ToAuthenticationHeaderValue(Cases schemeCase = Cases.Original, Cases parameterCase = Cases.Original)
        {
            return AccessToken != null
                ? new AuthenticationHeaderValue(
                    StringUtility.ToSpecificCaseInvariant(TokenType, schemeCase),
                    StringUtility.ToSpecificCaseInvariant(AccessToken, parameterCase))
                : new AuthenticationHeaderValue(
                    StringUtility.ToSpecificCaseInvariant(TokenType, schemeCase));
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.
        /// </summary>
        /// <param name="scheme">The scheme to use for authorization.</param>
        /// <param name="parameterCase">The parameter case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.</returns>
        public AuthenticationHeaderValue ToAuthenticationHeaderValue(string scheme, Cases parameterCase = Cases.Original)
        {
            return AccessToken != null
                ? new AuthenticationHeaderValue(
                    scheme ?? TokenType,
                    StringUtility.ToSpecificCaseInvariant(AccessToken, parameterCase))
                : new AuthenticationHeaderValue(scheme ?? TokenType);
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
        /// Gets the token information instance saved in this container.
        /// </summary>
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
        /// Adds or removes the event raised after token changed.
        /// </summary>
        public event ChangeEventHandler<TokenInfo> TokenChanged;

        /// <summary>
        /// Gets the access token saved in this container.
        /// </summary>
        public string AccessToken => Token?.AccessToken;

        /// <summary>
        /// Gets a value indicating whether the access token is null, empty or consists only of white-space characters.
        /// </summary>
        public bool IsTokenNullOrEmpty => Token?.IsEmpty ?? false;

        /// <summary>
        /// Gets or sets the case of authenticiation scheme.
        /// </summary>
        public virtual Cases AuthenticationSchemeCase { get; set; } = Cases.Original;

        /// <summary>
        /// Gets or sets the converter of authentication header value.
        /// </summary>
        public virtual Func<TokenInfo, Cases, AuthenticationHeaderValue> ConvertToAuthenticationHeaderValue { get; set; }

        /// <summary>
        /// Returns a System.String that represents the current token container instance.
        /// </summary>
        /// <returns>A System.String that represents the current token container instance.</returns>
        public override string ToString()
        {
            return Token?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.
        /// </summary>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.</returns>
        public virtual AuthenticationHeaderValue ToAuthenticationHeaderValue()
        {
            return Token?.ToAuthenticationHeaderValue(AuthenticationSchemeCase);
        }

        /// <summary>
        /// Sets the headers with current authorization information.
        /// </summary>
        /// <param name="headers">The headers to fill.</param>
        /// <param name="forceToSet">true if force to set even if it has one; otherwise, false.</param>
        /// <returns>true if write succeeded; otherwise, false.</returns>
        public bool WriteAuthenticationHeaderValue(HttpRequestHeaders headers, bool forceToSet = false)
        {
            if (headers == null || Token == null || headers.Authorization != null) return false;
            headers.Authorization = ConvertToAuthenticationHeaderValue != null
                ? ConvertToAuthenticationHeaderValue(Token, AuthenticationSchemeCase)
                : Token.ToAuthenticationHeaderValue(AuthenticationSchemeCase);
            return true;
        }
    }
}
