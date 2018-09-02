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
using System.Runtime.Serialization;

namespace Trivial.Security
{
    /// <summary>
    /// The access token response information.
    /// </summary>
    public class AccessTokenResponse
    {
        /// <summary>
        /// The user identifier property.
        /// </summary>
        public const string TokenKey = "token";

        /// <summary>
        /// The user identifier property.
        /// </summary>
        public const string UserIdProperty = "user_id";

        /// <summary>
        /// The resource identifier property.
        /// </summary>
        public const string ResourceIdProperty = "res_id";

        /// <summary>
        /// The user name property.
        /// </summary>
        public const string UserNameProperty = "username";

        /// <summary>
        /// The token reference identifier property.
        /// </summary>
        public const string TokenIdProperty = "token_id";

        /// <summary>
        /// The access token property.
        /// </summary>
        public const string AccessTokenProperty = "access_token";

        /// <summary>
        /// The token type property.
        /// </summary>
        public const string TokenTypeProperty = "token_type";

        /// <summary>
        /// The refresh token property.
        /// </summary>
        public const string RefreshTokenProperty = "refresh_token";

        /// <summary>
        /// The expires in property.
        /// </summary>
        public const string ExpiresInProperty = "expires_in";

        /// <summary>
        /// The token type.
        /// </summary>
        public const string BearerTokenType = "Bearer";

        /// <summary>
        /// Initializes a new instance of the AccessTokenResponse class.
        /// </summary>
        public AccessTokenResponse()
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
        /// Gets or sets the expiration date and time.
        /// </summary>
        [DataMember(Name = ExpiresInProperty)]
        public DateTime? ExpiredTime { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        [DataMember(Name = RefreshTokenProperty)]
        public string RefreshToken { get; set; }

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
        /// Gets the additional data.
        /// </summary>
        public Dictionary<string, string> AdditionalData { get; private set; }

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
        /// Returns a System.String that represents the current RequestIdentifier.
        /// </summary>
        /// <returns>A System.String that represents the current RequestIdentifier.</returns>
        public override string ToString()
        {
            return string.Format("{0}\\{1}", TokenType, AccessToken);
        }
    }
}
