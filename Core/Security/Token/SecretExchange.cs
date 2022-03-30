using System;
using System.Collections.Generic;
using System.Globalization;
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

using Trivial.Data;
using Trivial.Net;
using Trivial.Text;

namespace Trivial.Security;

/// <summary>
///   <para>
///     The secret exchange based on RSA.
///   </para>
///   <para>
///     You can save the secret or other string in local and send it another side
///     after encryption by the public key required by that side.
///     And send a public key registered in current container to the other side
///     so that it can use the same mechanism to transfer the secret encrypted back
///     and you can decrypt it by your private key.
///   </para>
/// </summary>
/// <remarks>
///   <para>
///     So you can use this in both 2 sides.
///   </para>
///   <para>
///     Following is an example of flow, suppose one is server side and another is client side.
///   </para>
///   <list type="number">
///     <item>
///       Client side asks for a server encrypt key
///       to encrypt the authorization form filled in client.
///     </item>
///     <item>
///       Server side sends a server encrypt key and its identifier.
///       This key is a public key (marked as S-Key-Public here) for encryption.
///       Its private key (marked as S-Key-Private here) is used to decrypt.
///       Both keys and its identifier (marked as S-Key-Id here) should cache in server.
///     </item>
///     <item>
///       Client creates a pair of private key and public key.
///       The private key (marked as C-Key-Private here) is used to decrypt data
///       and the public key (marked as C-Key-Public here) is used to encrypt.
///       The key pair should cache in client with an identifier (marked as C-Key-Id here).
///     </item>
///     <item>
///       Client uses S-Key-Public to encrypt the form now
///       and send it back to server with the S-Key-Id, C-Key-Public and C-Key-Id.
///       Client also need cache S-Key-Id and S-Key-Public in local.
///     </item>
///     <item>
///       Server uses S-Key-Id to find the S-Key-Private to decrypt the form.
///       Then generates a secret and encrypt it by C-Key-Public.
///       Then send the secret encrypted back to client with C-Key-Id.
///       Server also need cache C-Key-Id and C-Key-Public with a mapping relationship with the token.
///     </item>
///     <item>
///       Client uses C-Key-Id to find C-Key-Private to decrypt the secret.At next time,
///       client sends the data and the secret encrypted by S-Key-Public with S-Key-Id.
///     </item>
///     <item>
///       Server uses S-Key-Id to find the S-Key-Private to decrypt the secret to authorize.
///       Then process the business logic of request.
///       Then return the result and the secret encrypted by C-Key-Public with C-Key-Id.
///     </item>
///   </list>
///   <para>
///     Here, in server side.
///   </para>
///   <list type="bullet">
///     <item>C-Key-Public is the <code>EncryptKey</code>.</item>
///     <item>C-Key-Id is the <code>EncryptKeyId</code>.</item>
///     <item>S-Key-Public is the <code>PublicKey</code>.</item>
///     <item>S-Key-Id is the <code>Id</code>.</item>
///   </list>
///   <para>
///     In client side.
///   </para>
///   <list type="bullet">
///     <item>C-Key-Public is the <code>PublicKey</code>.</item>
///     <item>C-Key-Id is the <code>Id</code>.</item>
///     <item>S-Key-Public is the <code>EncryptKey</code>.</item>
///     <item>S-Key-Id is the <code>EncryptKeyId</code>.</item>
///   </list>
///   <para>
///     and in both, secret is in `Secret` property,
///     the private key is not accessable directly.
///   </para>
/// </remarks>
/// <example>
///   <code>
///     // Create a secret exchange instance and create a pair of RSA key.
///     var exchange = new RSASecretExchange();
///     exchange.CreateCrypto();
///     
///     // Get the public key to send to the other side
///     // so that they can use this to encrypt the secret
///     // and send back to us.
///     var publicKey = exchange.PublicKey.ToPublicPEMString();
///     
///     // Save the secret from the other side.
///     // The secret is encrypted by the current public key
///     // and we can decrypt by the private key stored in this instance to save.
///     exchange.DecryptSecret(secretReceived);
///     
///     // Save the other side public key.
///     var otherSidePublicKey = ...; // An RSA public key from the other side.
///     exchange.EncryptKey = RSAUtility.Parse(otherSidePublicKey);
///     
///     // Get the Base64 of the secret encrypted by the other side public key.
///     var secretToSend = exchange.EncryptSecret();
///
///     // Get the authentication header value in JSON web secret format
///     // using HMAC SHA-512 keyed hash algorithm to signature for example.
///     var sign = HashSignatureProvider.CreateHS512("a secret hash key");
///     var jwt = exchange.ToJsonWebTokenAuthenticationHeaderValue(sign);
///   </code>
/// </example>
public class RSASecretExchange : ICloneable
{
    /// <summary>
    /// The RSA exchange JWT payload model.
    /// </summary>
    [DataContract]
    public class JsonWebTokenPayload : Security.JsonWebTokenPayload
    {
        /// <summary>
        /// Gets or sets the optional user identifer.
        /// </summary>
        [DataMember(Name = "uid", EmitDefaultValue = false)]
        [JsonPropertyName("uid")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the token or the token encrypted if supports.
        /// </summary>
        [DataMember(Name = "val", EmitDefaultValue = false)]
        [JsonPropertyName("val")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is encrypted.
        /// </summary>
        [DataMember(Name = "ien")]
        [JsonPropertyName("ien")]
        public bool IsEncrypted { get; set; }

        /// <summary>
        /// Gets or sets the optional current encrypt key identifier if has.
        /// </summary>
        [DataMember(Name = "cei", EmitDefaultValue = false)]
        [JsonPropertyName("cei")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string CurrentEncryptId { get; set; }

        /// <summary>
        /// Gets or sets the optional encrypt key identifier expected for next operation back.
        /// </summary>
        [DataMember(Name = "eei", EmitDefaultValue = false)]
        [JsonPropertyName("eei")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ExpectFutureEncryptId { get; set; }

        /// <summary>
        /// Gets or sets the optional creation identifier.
        /// </summary>
        [DataMember(Name = "jci", EmitDefaultValue = false)]
        [JsonPropertyName("jci")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string CreationId { get; set; } = Guid.NewGuid().ToString("n");

        /// <summary>
        /// Reads an RSA token exchange instance from the other side.
        /// </summary>
        /// <param name="rsa">The RSA token exchange instance from the other side.</param>
        /// <param name="encryptKeyResolver">A handler to resolve encrypt key.</param>
        /// <returns>true if read succeeded; otherwise, false.</returns>
        public bool Read(RSASecretExchange rsa, Func<string, RSAParameters?> encryptKeyResolver = null)
        {
            if (!IsEncrypted || string.IsNullOrWhiteSpace(CurrentEncryptId))
            {
                rsa.SetSecret(Value);
            }
            else
            {
                if (!rsa.IsSameId(CurrentEncryptId)) return false;
            }

            rsa.DecryptSecret(Value, true);
            if (!string.IsNullOrWhiteSpace(UserId)) rsa.EntityId = UserId;
            if (string.IsNullOrWhiteSpace(ExpectFutureEncryptId) || (!IsEncrypted && ExpectFutureEncryptId == rsa.Id))
            {
                rsa.EncryptKeyId = null;
                rsa.EncryptKey = null;
            }
            else if (ExpectFutureEncryptId != rsa.EncryptKeyId)
            {
                if (encryptKeyResolver != null)
                {
                    rsa.EncryptKey = encryptKeyResolver(ExpectFutureEncryptId);
                    rsa.EncryptKeyId = rsa.EncryptKey != null ? ExpectFutureEncryptId : null;
                }
                else
                {
                    rsa.EncryptKey = null;
                    rsa.EncryptKeyId = null;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// A value inidicating whether the RSA crypto service provider is from outside.
    /// </summary>
    private bool needDisposeCrypto;

    /// <summary>
    /// The RSA crypto service provider.
    /// </summary>
    private RSA crypto;

    /// <summary>
    /// Deconstructor.
    /// </summary>
    ~RSASecretExchange()
    {
        if (crypto != null && needDisposeCrypto) crypto.Dispose();
        if (Secret != null) Secret.Dispose();
        Secret = null;
        crypto = null;
    }

    /// <summary>
    /// Gets or sets the identifier for the secret exchange.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the related entity identifier.
    /// </summary>
    public string EntityId { get; set; }

    /// <summary>
    /// Gets or sets the secret.
    /// </summary>
    public SecureString Secret { get; set; }

    /// <summary>
    /// Gets the public key for secret.
    /// </summary>
    public RSAParameters? PublicKey { get; private set; }

    /// <summary>
    /// Gets or sets the encryption key for secret.
    /// </summary>
    public RSAParameters? EncryptKey { get; set; }

    /// <summary>
    /// Gets or sets the additional identifier of the encryption key for secret.
    /// </summary>
    public string EncryptKeyId { get; set; }

    /// <summary>
    /// Gets a value indicating whether there is an RSA crypto service provider.
    /// </summary>
    public bool IsSecure => crypto != null;

    /// <summary>
    /// Gets a value indicating whether there is a secret.
    /// </summary>
    public bool HasSecret => Secret != null && Secret.Length > 0;

    /// <summary>
    /// Gets or sets the optional handler of the secret setter for DecryptSecret methods.
    /// </summary>
    public Func<string, bool, string> SecretFormatBeforeDecrypt { get; set; }

    /// <summary>
    /// Gets or sets the optional handler of the access secret getter for EncryptSecret method.
    /// </summary>
    public Func<string, bool, string> SecretFormatBeforeEncrypt { get; set; }

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
    /// <param name="syncEncryptKey">true if set the secret encryption key from the crypto service provider; otherwise, false.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    public void SetCrypto(RSA rsa, bool syncEncryptKey = false, bool needDisposeAlgorithmAutomatically = false)
    {
        if (crypto != null && !needDisposeCrypto) crypto.Dispose();
        crypto = rsa;
        if (rsa == null)
        {
            ClearCrypto(syncEncryptKey);
            return;
        }

        needDisposeCrypto = needDisposeAlgorithmAutomatically;
        PublicKey = rsa.ExportParameters(false);
        if (!syncEncryptKey) return;
        EncryptKey = PublicKey;
        EncryptKeyId = Id;
    }

    /// <summary>
    /// Sets a specific crypto service provider instance.
    /// </summary>
    /// <param name="privateKey">The RSA private key.</param>
    /// <param name="syncEncryptKey">true if set the secret encryption key from the crypto service provider; otherwise, false.</param>
    public void SetCrypto(string privateKey, bool syncEncryptKey = false)
    {
        var key = RSAParametersConvert.Parse(privateKey);
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
    /// <param name="syncEncryptKey">true if set the secret encryption key from the crypto service provider; otherwise, false.</param>
    public void SetCrypto(RSAParameters privateKey, bool syncEncryptKey = false)
    {
        if (privateKey.D == null || privateKey.D.Length == 0) throw new ArgumentException("privateKey should be an RSA private key.");
        var rsa = RSA.Create();
        rsa.ImportParameters(privateKey);
        SetCrypto(rsa, syncEncryptKey, false);
    }

    /// <summary>
    /// Clears the crypto service provider instance.
    /// </summary>
    /// <param name="syncEncryptKey"></param>
    public void ClearCrypto(bool syncEncryptKey = false)
    {
        needDisposeCrypto = false;
        PublicKey = null;
        if (!syncEncryptKey) return;
        EncryptKey = null;
        EncryptKeyId = Id;
        return;
    }

    /// <summary>
    /// Creates a new crypto service provider.
    /// </summary>
    /// <param name="syncEncryptKey">true if set the secret encryption key from the crypto service provider; otherwise, false.</param>
    public void CreateCrypto(bool syncEncryptKey = false)
    {
        SetCrypto(RSA.Create(), syncEncryptKey, false);
    }

    /// <summary>
    /// Exports the System.Security.Cryptography.RSAParameters.
    /// </summary>
    /// <param name="includePrivateParameters">true to include private parameters; otherwise, false.</param>
    /// <returns>The parameters for System.Security.Cryptography.RSA.</returns>
    public RSAParameters ExportParameters(bool includePrivateParameters)
    {
        if (crypto == null) throw new InvalidOperationException("No RSA crypto service provider.");
        return crypto.ExportParameters(includePrivateParameters);
    }

    /// <summary>
    /// Decrypts the secret and fills into this secret exchange instance.
    /// </summary>
    /// <param name="secretEncrypted">The new secret encrypted.</param>
    /// <param name="ignoreFormatIfNoCrypto">true if ignore format when no crypto set; otherwise, false.</param>
    /// <param name="padding">The optional padding mode for decryption.</param>
    public void DecryptSecret(string secretEncrypted, bool ignoreFormatIfNoCrypto, RSAEncryptionPadding padding = null)
    {
        if (Secret != null) Secret.Dispose();
        if (string.IsNullOrWhiteSpace(secretEncrypted))
        {
            Secret = null;
            return;
        }

        var rsa = crypto;
        if (rsa == null)
        {
            if (!ignoreFormatIfNoCrypto && SecretFormatBeforeDecrypt != null) secretEncrypted = SecretFormatBeforeDecrypt(secretEncrypted, false);
            Secret = secretEncrypted.ToSecure();
            return;
        }

        var plain = rsa.Decrypt(Convert.FromBase64String(secretEncrypted), padding ?? RSAEncryptionPadding.Pkcs1);
        Secret = SecretFormatBeforeDecrypt != null ? SecretFormatBeforeDecrypt(Encoding.UTF8.GetString(plain), true).ToSecure() : Encoding.UTF8.GetString(plain).ToSecure();
    }

    /// <summary>
    /// Decrypts the secret and fills into this secret exchange instance.
    /// </summary>
    /// <param name="secretEncrypted">The new secret encrypted.</param>
    /// <param name="padding">The padding mode for decryption.</param>
    public void DecryptSecret(string secretEncrypted, RSAEncryptionPadding padding = null)
    {
        DecryptSecret(secretEncrypted, false, padding);
    }

    /// <summary>
    /// Decrypts the secret and fills into this secret exchange instance.
    /// </summary>
    /// <param name="text">The string to decrypt.</param>
    /// <param name="ignoreFormatIfNoCrypto">true if ignore format when no crypto set; otherwise, false.</param>
    /// <param name="padding">The optional padding mode for decryption.</param>
    public string DecryptText(string text, bool ignoreFormatIfNoCrypto = false, RSAEncryptionPadding padding = null)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        var rsa = crypto;
        if (rsa == null) return (!ignoreFormatIfNoCrypto && SecretFormatBeforeDecrypt != null) ? SecretFormatBeforeDecrypt(text, false) : text;
        var plain = rsa.Decrypt(Convert.FromBase64String(text), padding ?? RSAEncryptionPadding.Pkcs1);
        return SecretFormatBeforeDecrypt != null ? SecretFormatBeforeDecrypt(Encoding.UTF8.GetString(plain), true) : Encoding.UTF8.GetString(plain);
    }

    /// <summary>
    /// Clears the secret.
    /// </summary>
    public void ClearSecret()
    {
        if (Secret != null) Secret.Dispose();
        Secret = null;
    }

    /// <summary>
    /// Sets a new secret.
    /// </summary>
    /// <param name="secret">The new secret to set.</param>
    public void SetSecret(string secret)
    {
        if (Secret != null) Secret.Dispose();
        if (string.IsNullOrWhiteSpace(secret))
        {
            Secret = null;
            return;
        }

        Secret = secret.ToSecure();
    }

    /// <summary>
    /// Sets a new secret.
    /// </summary>
    /// <param name="secret">The new secret to set.</param>
    public void SetSecret(SecureString secret)
    {
        if (Secret != null) Secret.Dispose();
        Secret = secret?.Copy();
    }

    /// <summary>
    /// Sets a new secret from an access token.
    /// </summary>
    /// <param name="token">The new token.</param>
    public void SetSecret(TokenInfo token)
    {
        SetSecret(token?.AccessToken);
    }

    /// <summary>
    /// Sets a new token from an access token.
    /// </summary>
    /// <param name="token">The new token.</param>
    public void SetSecret(TokenContainer token)
    {
        SetSecret(token?.AccessToken);
    }

    /// <summary>
    /// Gets the secret encrypted.
    /// </summary>
    /// <param name="key">The optional secret encryption key to use to override the original one set.</param>
    /// <param name="padding">The optional padding mode for decryption.</param>
    /// <returns>The Base64 string with secret encrypted.</returns>
    public string EncryptSecret(RSAParameters? key = null, RSAEncryptionPadding padding = null)
    {
        if (!key.HasValue) key = EncryptKey;
        if (Secret == null) return null;
        if (!key.HasValue) return SecretFormatBeforeEncrypt != null ? SecretFormatBeforeEncrypt(Secret.ToUnsecureString(), false) : Secret.ToUnsecureString();
        using var rsa = RSA.Create();
        rsa.ImportParameters(key.Value);
        var cypher = rsa.Encrypt(
            Encoding.UTF8.GetBytes(SecretFormatBeforeEncrypt != null ? SecretFormatBeforeEncrypt(Secret.ToUnsecureString(), true) : Secret.ToUnsecureString()),
            padding ?? RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(cypher);
    }

    /// <summary>
    /// Gets the secret encrypted.
    /// </summary>
    /// <param name="key">The secret encryption key to use to override the original one set.</param>
    /// <param name="padding">The optional padding mode for decryption.</param>
    /// <returns>The Base64 string with secret encrypted.</returns>
    public string EncryptSecret(string key, RSAEncryptionPadding padding = null)
    {
        return EncryptSecret(RSAParametersConvert.Parse(key), padding);
    }

    /// <summary>
    /// Gets the secret encrypted.
    /// </summary>
    /// <param name="padding">The optional padding mode for decryption.</param>
    /// <returns>The Base64 string with secret encrypted.</returns>
    public string EncryptSecret(RSAEncryptionPadding padding)
    {
        return EncryptSecret(null as string, padding);
    }

    /// <summary>
    /// Gets the encrypted text by private key.
    /// </summary>
    /// <param name="text">The string to encrypt.</param>
    /// <param name="ignoreFormatIfNoCrypto">true if ignore format when no crypto set; otherwise, false.</param>
    /// <param name="padding">The optional padding mode for decryption.</param>
    /// <returns>The Base64 string with secret encrypted.</returns>
    public string EncryptText(string text, bool ignoreFormatIfNoCrypto = false, RSAEncryptionPadding padding = null)
    {
        if (text == null) return null;
        if (EncryptKey == null) return (!ignoreFormatIfNoCrypto && SecretFormatBeforeEncrypt != null) ? SecretFormatBeforeEncrypt(text, false) : text;
        using var rsa = RSA.Create();
        rsa.ImportParameters(EncryptKey.Value);
        var cypher = rsa.Encrypt(
            Encoding.UTF8.GetBytes(SecretFormatBeforeEncrypt != null ? SecretFormatBeforeEncrypt(text, true) : text),
            padding ?? RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(cypher);
    }

    /// <summary>
    /// Gets the encrypted text by private key.
    /// </summary>
    /// <param name="text">The string to encrypt.</param>
    /// <param name="ignoreFormatIfNoCrypto">true if ignore format when no crypto set; otherwise, false.</param>
    /// <param name="padding">The optional padding mode for decryption.</param>
    /// <returns>The Base64 string with secret encrypted.</returns>
    public string EncryptText(SecureString text, bool ignoreFormatIfNoCrypto = false, RSAEncryptionPadding padding = null)
    {
        return EncryptText(SecureStringExtensions.ToUnsecureString(text), ignoreFormatIfNoCrypto, padding);
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
    /// Creates the payload for JSON web token.
    /// </summary>
    /// <param name="thisSide">
    /// true if get the JSON web token payload without encryption for this side usage;
    /// otherwise, false, as default value, for the other side to send.
    /// </param>
    /// <returns>A JSON web token payload.</returns>
    public JsonWebTokenPayload ToJsonWebTokenPayload(bool thisSide = false)
    {
        if (thisSide)
        {
            return new JsonWebTokenPayload
            {
                UserId = EntityId,
                Value = Secret?.ToUnsecureString(),
                ExpectFutureEncryptId = EncryptKeyId
            };
        }

        if (string.IsNullOrWhiteSpace(EncryptKeyId))
        {
            return new JsonWebTokenPayload
            {
                UserId = EntityId,
                Value = Secret?.ToUnsecureString(),
                ExpectFutureEncryptId = Id,
                IssuedAt = DateTime.Now
            };
        }

        return new JsonWebTokenPayload
        {
            CurrentEncryptId = EncryptKeyId,
            UserId = EntityId,
            Value = EncryptSecret(),
            IsEncrypted = IsSecure,
            ExpectFutureEncryptId = Id,
            IssuedAt = DateTime.Now
        };
    }

    /// <summary>
    /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current RSA token exchange in JSON web token format.
    /// </summary>
    /// <param name="sign">The signature provider.</param>
    /// <param name="thisSide">
    /// true if get the JSON web token payload without encryption for this side usage;
    /// otherwise, false, as default value, for the other side to send.
    /// </param>
    /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current RSA token exchange in JSON web token format.</returns>
    public AuthenticationHeaderValue ToJsonWebTokenAuthenticationHeaderValue(ISignatureProvider sign, bool thisSide = false)
    {
        return ToJsonWebTokenAuthenticationHeaderValue(sign, null, thisSide);
    }

    /// <summary>
    /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current RSA token exchange in JSON web token format.
    /// </summary>
    /// <param name="sign">The signature provider.</param>
    /// <param name="serializer">An optional JSON serializer.</param>
    /// <param name="thisSide">
    /// true if get the JSON web token payload without encryption for this side usage;
    /// otherwise, false, as default value, for the other side to send.
    /// </param>
    /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current RSA token exchange in JSON web token format.</returns>
    public AuthenticationHeaderValue ToJsonWebTokenAuthenticationHeaderValue(ISignatureProvider sign, Func<object, string> serializer, bool thisSide = false)
    {
        var m = ToJsonWebTokenPayload(thisSide);
        return m.ToAuthenticationHeaderValue(sign, serializer);
    }

    /// <summary>
    /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current RSA token exchange in JSON web token format.
    /// </summary>
    /// <param name="sign">The signature provider.</param>
    /// <param name="converter">A converter for payload.</param>
    /// <param name="thisSide">
    /// true if get the JSON web token payload without encryption for this side usage;
    /// otherwise, false, as default value, for the other side to send.
    /// </param>
    /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current RSA token exchange in JSON web token format.</returns>
    public AuthenticationHeaderValue ToJsonWebTokenAuthenticationHeaderValue(ISignatureProvider sign, Func<JsonWebTokenPayload, object> converter, bool thisSide = false)
    {
        return ToJsonWebTokenAuthenticationHeaderValue(sign, converter, null, thisSide);
    }

    /// <summary>
    /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current RSA token exchange in JSON web token format.
    /// </summary>
    /// <param name="sign">The signature provider.</param>
    /// <param name="converter">A converter for payload.</param>
    /// <param name="serializer">An optional JSON serializer.</param>
    /// <param name="thisSide">
    /// true if get the JSON web token payload without encryption for this side usage;
    /// otherwise, false, as default value, for the other side to send.
    /// </param>
    /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current RSA token exchange in JSON web token format.</returns>
    public AuthenticationHeaderValue ToJsonWebTokenAuthenticationHeaderValue(ISignatureProvider sign, Func<JsonWebTokenPayload, object> converter, Func<object, string> serializer, bool thisSide = false)
    {
        var m = ToJsonWebTokenPayload(thisSide);
        var obj = converter != null ? converter(m) : m;
        var jwt = new JsonWebToken<object>(obj, sign)
        {
            Serializer = serializer
        };
        return jwt.ToAuthenticationHeaderValue();
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public RSASecretExchange Clone()
    {
        needDisposeCrypto = false;
        return new RSASecretExchange
        {
            Id = Id,
            EntityId = EntityId,
            EncryptKey = EncryptKey,
            EncryptKeyId = EncryptKeyId,
            Secret = Secret?.Copy(),
            SecretFormatBeforeDecrypt = SecretFormatBeforeDecrypt,
            SecretFormatBeforeEncrypt = SecretFormatBeforeEncrypt,
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
