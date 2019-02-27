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
using System.Security;
using System.Runtime.Serialization;

namespace Trivial.Security
{
    /// <summary>
    /// Gets the app secret key for accessing api.
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
        public AppAccessingKey(string id, SecureString secret)
        {
            Id = id;
            Secret = secret;
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
    }

    /// <summary>
    /// The access token response information.
    /// </summary>
    [DataContract]
    public class TokenInfo
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
        /// Gets the permission scope.
        /// </summary>
        public IList<string> Scope { get; } = new List<string>();

        /// <summary>
        /// Gets the additional data.
        /// </summary>
        public Dictionary<string, string> AdditionalData { get; private set; }

        /// <summary>
        /// Tests if the access token is set.
        /// </summary>
        /// <returns>true if it is null or empty; otherwise, false.</returns>
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
        /// Returns a System.String that represents the current RequestIdentifier.
        /// </summary>
        /// <returns>A System.String that represents the current RequestIdentifier.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", TokenType, AccessToken).Trim();
        }
    }
}
