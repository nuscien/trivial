using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.Net;
using Trivial.Text;

namespace Trivial.Security
{
    /// <summary>
    /// The token container.
    /// </summary>
    public class TokenContainer
    {
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
        public TokenInfo Token { get; protected set; }

        /// <summary>
        /// Gets the access token saved in this container.
        /// </summary>
        public string AccessToken => Token?.AccessToken;

        /// <summary>
        /// Gets a value indicating whether the access token is null, empty or consists only of white-space characters.
        /// </summary>
        public bool IsTokenNullOrEmpty => Token?.IsEmpty ?? false;

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
        /// <param name="schemeCase">The scheme case.</param>
        /// <param name="parameterCase">The parameter case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.</returns>
        public virtual AuthenticationHeaderValue ToAuthenticationHeaderValue(Cases schemeCase = Cases.Original, Cases parameterCase = Cases.Original)
        {
            return Token?.ToAuthenticationHeaderValue(schemeCase, parameterCase);
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.
        /// </summary>
        /// <param name="scheme">The scheme to use for authorization.</param>
        /// <param name="parameterCase">The parameter case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current token container instance.</returns>
        public virtual AuthenticationHeaderValue ToAuthenticationHeaderValue(string scheme, Cases parameterCase = Cases.Original)
        {
            return Token?.ToAuthenticationHeaderValue(scheme, parameterCase);
        }
    }

    /// <summary>
    /// The token resolver.
    /// </summary>
    public abstract class TokenResolver : TokenContainer
    {
        private readonly AppAccessingKey appInfo;
        private Task<TokenInfo> task;

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(TokenInfo tokenCached) : base(tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(AppAccessingKey appKey, TokenInfo tokenCached = null) : this(tokenCached)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(string appId, string secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Gets the app id.
        /// </summary>
        public string AppId => !AppAccessingKey.IsNullOrEmpty(appInfo) ? appInfo.Id : null;

        /// <summary>
        /// Gets or sets a value indicating whether it requires an app identifier.
        /// </summary>
        public bool IsAppIdRequired { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a token cached.
        /// </summary>
        public bool HasCache => !string.IsNullOrWhiteSpace(Token?.AccessToken);

        /// <summary>
        /// Gets authorization value.
        /// </summary>
        public string Authorization => Token?.ToString();

        /// <summary>
        /// Gets the latest resolved date.
        /// </summary>
        public DateTime LatestResolveDate { get; private set; }

        /// <summary>
        /// Gets the JSON HTTP web client for resolving access token information instance.
        /// </summary>
        protected virtual JsonHttpClient<TokenInfo> WebClient { get; } = new JsonHttpClient<TokenInfo>();

        /// <summary>
        /// Adds or removes the event raised after token changed.
        /// </summary>
        public event ChangeEventHandler<TokenInfo> TokenChanged;

        /// <summary>
        /// Updates the access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token information instance updated.</returns>
        public async Task<TokenInfo> UpdateAsync(CancellationToken cancellationToken = default)
        {
            var t = task;
            if (t != null && t.Status == TaskStatus.Running)
            {
                try
                {
                    return await Task.Run(async () =>
                    {
                        return await t;
                    }, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested) throw;
                }
                catch (ObjectDisposedException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }

            task = t = UpdateUnlockedAsync(cancellationToken);
            var result = await t;
            task = null;
            return result;
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token information instance.</returns>
        public Task<TokenInfo> GetAsync(CancellationToken cancellationToken = default)
        {
            return HasCache ? Task.Run(() => Token) : UpdateAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the token resolve request message.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <returns>A URI for login.</returns>
        protected abstract HttpRequestMessage CreateResolveMessage(SecureString appSecretKey);

        /// <summary>
        /// Updates the access token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token information instance updated.</returns>
        private async Task<TokenInfo> UpdateUnlockedAsync(CancellationToken cancellationToken = default)
        {
            if (IsAppIdRequired && AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            var wc = WebClient;
            var oldToken = Token;
            using (var request = CreateResolveMessage(appInfo?.Secret))
            {
                if (request == null) return null;
                Token = await wc.SendAsync(request, cancellationToken);
                LatestResolveDate = DateTime.Now;
            }

            TokenChanged?.Invoke(this, new ChangeEventArgs<TokenInfo>(oldToken, Token, nameof(Token), true));
            return Token;
        }
    }

    /// <summary>
    /// The open id token client.
    /// </summary>
    public abstract class OpenIdTokenClient : TokenContainer
    {
        private readonly AppAccessingKey appInfo;
        private Task<TokenInfo> task;

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(TokenInfo tokenCached) : base(tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(AppAccessingKey appKey, TokenInfo tokenCached = null) : this(tokenCached)
        {
            appInfo = appKey;
        }

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(string appId, string secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(string appId, SecureString secretKey, TokenInfo tokenCached = null) : this(new AppAccessingKey(appId, secretKey), tokenCached)
        {
        }

        /// <summary>
        /// Gets the app id.
        /// </summary>
        public string AppId => !AppAccessingKey.IsNullOrEmpty(appInfo) ? appInfo.Id : null;

        /// <summary>
        /// Gets or sets a value indicating whether it requires an app identifier.
        /// </summary>
        public bool IsAppIdRequired { get; set; }

        /// <summary>
        /// Gets the latest visited date.
        /// </summary>
        public DateTime LatestVisitDate { get; private set; }

        /// <summary>
        /// Gets the latest refreshed date.
        /// </summary>
        public DateTime LatestRefreshDate { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether need refresh token before valdate the code when expired.
        /// </summary>
        public bool IsAutoRefresh { get; set; }

        /// <summary>
        /// Gets the JSON HTTP web client for resolving access token information instance.
        /// </summary>
        protected virtual JsonHttpClient<TokenInfo> WebClient { get; } = new JsonHttpClient<TokenInfo>();

        /// <summary>
        /// Adds or removes the event raised after token changed.
        /// </summary>
        public event ChangeEventHandler<TokenInfo> TokenChanged;

        /// <summary>
        /// Validates the code.
        /// </summary>
        /// <param name="code">The code to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A new open id; or null, if failed.</returns>
        public async Task<TokenInfo> ValidateCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(code)) return null;
            if (IsAppIdRequired && AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            using (var request = CreateValidationMessage(appInfo?.Secret, code))
            {
                if (request == null) return null;
                try
                {
                    return await ProcessAsync(request, cancellationToken);
                }
                catch (FailedHttpException ex)
                {
                    if (!IsAutoRefresh || ex.Response == null || ex.Response.StatusCode != HttpStatusCode.Unauthorized) throw;
                    var token = await RefreshAsync(cancellationToken);
                    if (token == null || token.IsEmpty) throw;
                    using (var request2 = CreateValidationMessage(appInfo?.Secret, code))
                    {
                        if (request2 == null) return null;
                        return await ProcessAsync(request2, cancellationToken);
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A new open id; or null, if failed.</returns>
        public async Task<TokenInfo> RefreshAsync(CancellationToken cancellationToken = default)
        {
            var t = task;
            if (t != null && t.Status == TaskStatus.Running)
            {
                try
                {
                    return await Task.Run(async () =>
                    {
                        return await t;
                    }, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested) throw;
                }
                catch (ObjectDisposedException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }

            if (IsAppIdRequired && AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            using (var request = CreateRefreshingMessage(appInfo?.Secret))
            {
                if (request == null) return null;
                task = t = ProcessAsync(request, cancellationToken);
                var result = await t;
                LatestRefreshDate = DateTime.Now;
                task = null;
                return result;
            }
        }

        /// <summary>
        /// Gets the login URI.
        /// </summary>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="scope">The permission scope to request.</param>
        /// <param name="state">A state code.</param>
        /// <returns>A URI for login.</returns>
        public abstract Uri GetLoginUri(Uri redirectUri, string scope, string state);

        /// <summary>
        /// Creates the validation request message.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <param name="code">The code to validate.</param>
        /// <returns>A URI for login.</returns>
        protected abstract HttpRequestMessage CreateValidationMessage(SecureString appSecretKey, string code);

        /// <summary>
        /// Creates the token refresh request message.
        /// </summary>
        /// <param name="appSecretKey">The app secret string.</param>
        /// <returns>A URI for login.</returns>
        protected abstract HttpRequestMessage CreateRefreshingMessage(SecureString appSecretKey);

        private async Task<TokenInfo> ProcessAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null) return null;
            if (IsAppIdRequired && AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            var wc = WebClient;
            var oldToken = Token;
            Token = await wc.SendAsync(request, cancellationToken);
            LatestVisitDate = DateTime.Now;
            TokenChanged?.Invoke(this, new ChangeEventArgs<TokenInfo>(oldToken, Token, nameof(Token), true));
            return Token;
        }
    }

    /// <summary>
    /// The token exchange based on RSA.
    /// </summary>
    public class RsaTokenExchange : ICloneable
    {
        /// <summary>
        /// The RSA crypto service provider.
        /// </summary>
        private RSA crypto;

        /// <summary>
        /// Gets or sets the identifier for the token exchange.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the related entity identifier.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public SecureString AccessToken { get; set; }

        /// <summary>
        /// Gets the public key for token.
        /// </summary>
        public RSAParameters? PublicKey { get; private set; }

        /// <summary>
        /// Gets or sets the encryption key for token.
        /// </summary>
        public RSAParameters? EncryptKey { get; set; }

        /// <summary>
        /// Gets or sets the additional identifier of the encryption key for token.
        /// </summary>
        public string EncryptKeyId { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is an RSA crypto service provider.
        /// </summary>
        public bool IsSecure => crypto != null;

        /// <summary>
        /// Gets a value indicating whether there is an access token.
        /// </summary>
        public bool HasToken => AccessToken != null && AccessToken.Length > 0;

        /// <summary>
        /// Gets or sets the optional handler of the access token setter for DecryptToken methods.
        /// </summary>
        public Func<string, bool, string> TokenFormatBeforeDecrypt { get; set; }

        /// <summary>
        /// Gets or sets the optional handler of the access token getter for EncryptToken method.
        /// </summary>
        public Func<string, bool, string> TokenFormatBeforeEncrypt { get; set; }

        /// <summary>
        /// Creates a random new identifier.
        /// </summary>
        public void RenewId()
        {
            Id = Guid.NewGuid().ToString("n");
        }

        /// <summary>
        /// Sets a specific crypto service provider instance.
        /// </summary>
        /// <param name="rsa">The new crypto service provider.</param>
        /// <param name="syncEncryptKey">true if set the token encryption key from the crypto service provider; otherwise, false.</param>
        public void SetCrypto(RSA rsa, bool syncEncryptKey = false)
        {
            crypto = rsa;
            if (rsa == null)
            {
                ClearCrypto(syncEncryptKey);
                return;
            }
            
            PublicKey = rsa.ExportParameters(false);
            if (!syncEncryptKey) return;
            EncryptKey = PublicKey;
            EncryptKeyId = Id;
        }

        /// <summary>
        /// Sets a specific crypto service provider instance.
        /// </summary>
        /// <param name="privateKey">The RSA private key.</param>
        /// <param name="syncEncryptKey">true if set the token encryption key from the crypto service provider; otherwise, false.</param>
        public void SetCrypto(string privateKey, bool syncEncryptKey = false)
        {
            var key = RSAUtility.ParseParameters(privateKey);
            if (!key.HasValue)
            {
                ClearCrypto(syncEncryptKey);
                return;
            }

            SetCrypto(key.Value, syncEncryptKey);
        }

        /// <summary>
        /// Sets a specific crypto service provider instance.
        /// </summary>
        /// <param name="privateKey">The RSA private key.</param>
        /// <param name="syncEncryptKey">true if set the token encryption key from the crypto service provider; otherwise, false.</param>
        public void SetCrypto(RSAParameters privateKey, bool syncEncryptKey = false)
        {
            if (privateKey.D == null || privateKey.D.Length == 0) throw new ArgumentException("privateKey should be an RSA private key.");
            var rsa = RSA.Create();
            rsa.ImportParameters(privateKey);
            SetCrypto(rsa, syncEncryptKey);
        }

        /// <summary>
        /// Clears the crypto service provider instance.
        /// </summary>
        /// <param name="syncEncryptKey"></param>
        public void ClearCrypto(bool syncEncryptKey = false)
        {
            PublicKey = null;
            if (!syncEncryptKey) return;
            EncryptKey = null;
            EncryptKeyId = Id;
            return;
        }

        /// <summary>
        /// Creates a new crypto service provider.
        /// </summary>
        /// <param name="syncEncryptKey">true if set the token encryption key from the crypto service provider; otherwise, false.</param>
        public void CreateCrypto(bool syncEncryptKey = false)
        {
            SetCrypto(RSA.Create(), syncEncryptKey);
        }

        /// <summary>
        /// Decrypts the token and fills into this token exchange instance.
        /// </summary>
        /// <param name="tokenEncrypted">The new token encrypted.</param>
        /// <param name="ignoreFormatIfNoCrypto">true if ignore format when no crypto set; otherwise, false.</param>
        /// <param name="padding">The optional padding mode for decryption.</param>
        public void DecryptToken(string tokenEncrypted, bool ignoreFormatIfNoCrypto, RSAEncryptionPadding padding = null)
        {
            if (string.IsNullOrWhiteSpace(tokenEncrypted))
            {
                AccessToken = null;
                return;
            }

            var rsa = crypto;
            if (rsa == null)
            {
                if (!ignoreFormatIfNoCrypto && TokenFormatBeforeDecrypt != null) tokenEncrypted = TokenFormatBeforeDecrypt(tokenEncrypted, false);
                AccessToken = tokenEncrypted.ToSecure();
                return;
            }

            var plain = rsa.Decrypt(Convert.FromBase64String(tokenEncrypted), padding ?? RSAEncryptionPadding.Pkcs1);
            AccessToken = TokenFormatBeforeDecrypt != null ? TokenFormatBeforeDecrypt(Encoding.UTF8.GetString(plain), true).ToSecure() : Encoding.UTF8.GetString(plain).ToSecure();
        }

        /// <summary>
        /// Decrypts the token and fills into this token exchange instance.
        /// </summary>
        /// <param name="tokenEncrypted">The new token encrypted.</param>
        /// <param name="padding">The padding mode for decryption.</param>
        public void DecryptToken(string tokenEncrypted, RSAEncryptionPadding padding)
        {
            DecryptToken(tokenEncrypted, false, padding);
        }

        /// <summary>
        /// Clears the token.
        /// </summary>
        public void ClearToken()
        {
            AccessToken = null;
        }

        /// <summary>
        /// Sets a new token.
        /// </summary>
        /// <param name="token">The new token.</param>
        public void SetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                AccessToken = null;
                return;
            }

            AccessToken = token.ToSecure();
        }

        /// <summary>
        /// Sets a new token.
        /// </summary>
        /// <param name="token">The new token.</param>
        public void SetToken(TokenInfo token)
        {
            SetToken(token?.AccessToken);
        }

        /// <summary>
        /// Sets a new token.
        /// </summary>
        /// <param name="token">The new token.</param>
        public void SetToken(TokenContainer token)
        {
            SetToken(token?.AccessToken);
        }

        /// <summary>
        /// Decrypts the token and fills into this token exchange instance.
        /// </summary>
        /// <param name="key">The optional token encryption key to use to override the original one set.</param>
        /// <param name="padding">The optional padding mode for decryption.</param>
        public string EncryptToken(RSAParameters? key = null, RSAEncryptionPadding padding = null)
        {
            if (!key.HasValue) key = EncryptKey;
            if (!key.HasValue) return TokenFormatBeforeEncrypt != null ? TokenFormatBeforeEncrypt(AccessToken.ToUnsecureString(), false) : AccessToken.ToUnsecureString();
            var rsa = RSA.Create();
            rsa.ImportParameters(key.Value);
            var cypher = rsa.Encrypt(
                Encoding.UTF8.GetBytes(TokenFormatBeforeEncrypt != null ? TokenFormatBeforeEncrypt(AccessToken.ToUnsecureString(), true) : AccessToken.ToUnsecureString()),
                padding ?? RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(cypher);
        }

        /// <summary>
        /// Decrypts the token and fills into this token exchange instance.
        /// </summary>
        /// <param name="key">The token encryption key to use to override the original one set.</param>
        /// <param name="padding">The optional padding mode for decryption.</param>
        public string EncryptToken(string key, RSAEncryptionPadding padding = null)
        {
            return EncryptToken(RSAUtility.ParseParameters(key), padding);
        }

        /// <summary>
        /// Decrypts the token and fills into this token exchange instance.
        /// </summary>
        /// <param name="padding">The padding mode for decryption.</param>
        public string EncryptToken(RSAEncryptionPadding padding)
        {
            return EncryptToken(null as string, padding);
        }

        /// <summary>
        /// Tests if the given identifier is as same as the identifier of this instance.
        /// </summary>
        /// <param name="id">The identifier to test.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public bool IsSameId(string id)
        {
            return id == Id || (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(Id));
        }

        /// <summary>
        /// Tests if the given identifier is as same as the related entity identifier of this instance.
        /// </summary>
        /// <param name="entity">The entity identifier to test.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public bool IsSameEntityId(string entity)
        {
            return entity == EntityId || (string.IsNullOrWhiteSpace(entity) && string.IsNullOrWhiteSpace(EntityId));
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public RsaTokenExchange Clone()
        {
            return new RsaTokenExchange
            {
                Id = Id,
                EntityId = EntityId,
                EncryptKey = EncryptKey,
                EncryptKeyId = EncryptKeyId,
                AccessToken = AccessToken,
                TokenFormatBeforeDecrypt = TokenFormatBeforeDecrypt,
                TokenFormatBeforeEncrypt = TokenFormatBeforeEncrypt,
                PublicKey = PublicKey,
                crypto = crypto
            };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
