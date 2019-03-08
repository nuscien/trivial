using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    /// The token resolver.
    /// </summary>
    public abstract class TokenResolver
    {
        private readonly AppAccessingKey appInfo;
        private Task<TokenInfo> task;

        /// <summary>
        /// Initializes a new instance of the TokenResolver class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public TokenResolver(TokenInfo tokenCached)
        {
            Token = tokenCached;
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
        /// Gets the open id info cached.
        /// </summary>
        public TokenInfo Token { get; protected set; }

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
        /// Returns a System.String that represents the current TokenResolver.
        /// </summary>
        /// <returns>A System.String that represents the current TokenResolver.</returns>
        public override string ToString()
        {
            return Token?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenResolver.
        /// </summary>
        /// <param name="schemeCase">The scheme case.</param>
        /// <param name="parameterCase">The parameter case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenResolver.</returns>
        public AuthenticationHeaderValue ToAuthenticationHeaderValue(Cases schemeCase = Cases.Original, Cases parameterCase = Cases.Original)
        {
            return Token?.ToAuthenticationHeaderValue(schemeCase, parameterCase);
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenResolver.
        /// </summary>
        /// <param name="scheme">The scheme to use for authorization.</param>
        /// <param name="parameterCase">The parameter case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenResolver.</returns>
        public AuthenticationHeaderValue ToAuthenticationHeaderValue(string scheme, Cases parameterCase = Cases.Original)
        {
            return Token?.ToAuthenticationHeaderValue(scheme, parameterCase);
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
            if (AppAccessingKey.IsNullOrEmpty(appInfo)) return null;
            var wc = WebClient;
            var oldToken = Token;
            using (var request = CreateResolveMessage(appInfo.Secret))
            {
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
    public abstract class OpenIdTokenClient
    {
        private readonly AppAccessingKey appInfo;
        private Task<TokenInfo> task;

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(TokenInfo tokenCached)
        {
            Token = tokenCached;
        }

        /// <summary>
        /// Initializes a new instance of the OpenIdTokenClient class.
        /// </summary>
        /// <param name="appKey">The app accessing key.</param>
        /// <param name="tokenCached">The token information instance cached.</param>
        public OpenIdTokenClient(AppAccessingKey appKey, TokenInfo tokenCached = null)
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
        /// Gets the open id info.
        /// </summary>
        public TokenInfo Token { get; protected set; }

        /// <summary>
        /// Gets the latest visited date.
        /// </summary>
        public DateTime LatestVisitDate { get; private set; }

        /// <summary>
        /// Gets the latest refreshed date.
        /// </summary>
        public DateTime LatestRefreshDate { get; private set; }

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
            using (var request = CreateValidationMessage(appInfo.Secret, code))
            {
                return await ProcessAsync(request, cancellationToken);
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

            using (var request = CreateRefreshingMessage(appInfo.Secret))
            {
                task = t = ProcessAsync(request, cancellationToken);
                var result = await t;
                LatestRefreshDate = DateTime.Now;
                task = null;
                return result;
            }
        }

        /// <summary>
        /// Returns a System.String that represents the current OpenIdTokenClient.
        /// </summary>
        /// <returns>A System.String that represents the current OpenIdTokenClient.</returns>
        public override string ToString()
        {
            return Token?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current OpenIdTokenClient.
        /// </summary>
        /// <param name="schemeCase">The scheme case.</param>
        /// <param name="parameterCase">The parameter case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current OpenIdTokenClient.</returns>
        public AuthenticationHeaderValue ToAuthenticationHeaderValue(Cases schemeCase = Cases.Original, Cases parameterCase = Cases.Original)
        {
            return Token?.ToAuthenticationHeaderValue(schemeCase, parameterCase);
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current OpenIdTokenClient.
        /// </summary>
        /// <param name="scheme">The scheme to use for authorization.</param>
        /// <param name="parameterCase">The parameter case.</param>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current OpenIdTokenClient.</returns>
        public AuthenticationHeaderValue ToAuthenticationHeaderValue(string scheme, Cases parameterCase = Cases.Original)
        {
            return Token?.ToAuthenticationHeaderValue(scheme, parameterCase);
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
            if (AppAccessingKey.IsNullOrEmpty(appInfo) || request == null) return null;
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
    public class RsaTokenExchange
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
        /// Gets or sets the handler to set the token. For SetToken and DecryptToken methods.
        /// </summary>
        public Func<string, bool, string> TokenSet { get; set; }

        /// <summary>
        /// Gets or sets the handler to get the token. For EncryptToken method.
        /// </summary>
        public Func<string, bool, string> TokenGet { get; set; }

        /// <summary>
        /// Sets a specific crypto service provider.
        /// </summary>
        /// <param name="rsa">The new crypto service provider.</param>
        /// <param name="syncEncryptKey">true if set the token encryption key from the crypto service provider; otherwise, false.</param>
        public void SetCrypto(RSA rsa, bool syncEncryptKey = false)
        {
            crypto = rsa;
            if (rsa == null)
            {
                PublicKey = null;
                if (!syncEncryptKey) return;
                EncryptKey = null;
                EncryptKeyId = Id;
                return;
            }
            
            PublicKey = rsa.ExportParameters(false);
            if (!syncEncryptKey) return;
            EncryptKey = PublicKey;
            EncryptKeyId = Id;
        }

        /// <summary>
        /// Sets a specific crypto service provider.
        /// </summary>
        /// <param name="privateKey">The RSA private key.</param>
        /// <param name="syncEncryptKey">true if set the token encryption key from the crypto service provider; otherwise, false.</param>
        public void SetCrypto(string privateKey, bool syncEncryptKey = false)
        {
            var key = RSAUtility.ParseParameters(privateKey);
            if (!key.HasValue)
            {
                PublicKey = null;
                if (!syncEncryptKey) return;
                EncryptKey = null;
                EncryptKeyId = Id;
                return;
            }

            if (key.Value.D == null || key.Value.D.Length == 0) throw new ArgumentException("privateKey should be an OpenSSL RSA private key.");
            var rsa = RSA.Create();
            rsa.ImportParameters(key.Value);
            SetCrypto(rsa, syncEncryptKey);
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
        /// <param name="supportNoCrypto">true if the token is not encrypted if there is no crypto service provider; otherwise, false.</param>
        /// <param name="padding">The optional padding mode for decryption.</param>
        public void DecryptToken(string tokenEncrypted, bool supportNoCrypto = false, RSAEncryptionPadding padding = null)
        {
            if (string.IsNullOrWhiteSpace(tokenEncrypted))
            {
                AccessToken = null;
                return;
            }

            var rsa = crypto;
            if (rsa == null && supportNoCrypto)
            {
                if (TokenSet != null) tokenEncrypted = TokenSet(tokenEncrypted, false);
                AccessToken = tokenEncrypted.ToSecure();
                return;
            }

            var plain = rsa.Decrypt(Convert.FromBase64String(tokenEncrypted), padding ?? RSAEncryptionPadding.Pkcs1);
            AccessToken = TokenSet != null ? TokenSet(Encoding.UTF8.GetString(plain), true).ToSecure() : Encoding.UTF8.GetString(plain).ToSecure();
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
        /// Sets a new token.
        /// </summary>
        /// <param name="token">The new token.</param>
        public void SetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;
            if (TokenSet != null) token = TokenSet(token, false);
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
        /// Decrypts the token and fills into this token exchange instance.
        /// </summary>
        /// <param name="key">The optional token encryption key to use to override the original one set.</param>
        /// <param name="padding">The optional padding mode for decryption.</param>
        public string EncryptToken(RSAParameters? key = null, RSAEncryptionPadding padding = null)
        {
            if (!key.HasValue) key = EncryptKey;
            if (!key.HasValue) return TokenGet != null ? TokenGet(AccessToken.ToUnsecureString(), false) : AccessToken.ToUnsecureString();
            var rsa = RSA.Create();
            rsa.ImportParameters(key.Value);
            var cypher = rsa.Encrypt(
                Encoding.UTF8.GetBytes(TokenGet != null ? TokenGet(AccessToken.ToUnsecureString(), false) : AccessToken.ToUnsecureString()),
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
    }
}
