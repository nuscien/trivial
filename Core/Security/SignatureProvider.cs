using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Trivial.Collection;

namespace Trivial.Security
{
    /// <summary>
    /// The signature for string.
    /// </summary>
    public interface ISignatureProvider
    {
        /// <summary>
        /// Gets the signature name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether it can sign a specific data.
        /// </summary>
        bool CanSign { get; }

        /// <summary>
        /// Computes the signature for the specified hash value.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <returns>The signature for the specified hash value.</returns>
        byte[] Sign(byte[] data);

        /// <summary>
        /// Verifies that a digital signature is valid by calculating the hash value of the specified data
        /// using the specified hash algorithm and padding, and comparing it to the provided signature.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <returns>true if the signature is valid; otherwise, false.</returns>
        bool Verify(byte[] data, byte[] signature);
    }

    /// <summary>
    /// The hash signature for string.
    /// </summary>
    public class HashSignatureProvider : ISignatureProvider, IDisposable
    {
        private readonly bool needDispose;
        private readonly HashAlgorithm alg;

        /// <summary>
        /// Creates a hash signature provider using HMAC SHA-512 keyed hash algorithm.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateHS512(string secret)
        {
            var a = new HMACSHA512(Encoding.ASCII.GetBytes(secret ?? string.Empty));
            return new HashSignatureProvider(a, "HS512", true);
        }

        /// <summary>
        /// Creates a hash signature provider using HMAC SHA-512 keyed hash algorithm.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateHS512(byte[] secret)
        {
            var a = new HMACSHA512(secret);
            return new HashSignatureProvider(a, "HS512", true);
        }

        /// <summary>
        /// Creates a hash signature provider using HMAC SHA-384 keyed hash algorithm.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateHS384(string secret)
        {
            var a = new HMACSHA384(Encoding.ASCII.GetBytes(secret ?? string.Empty));
            return new HashSignatureProvider(a, "HS384", true);
        }

        /// <summary>
        /// Creates a hash signature provider using HMAC SHA-384 keyed hash algorithm.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateHS384(byte[] secret)
        {
            var a = new HMACSHA384(secret);
            return new HashSignatureProvider(a, "HS384", true);
        }

        /// <summary>
        /// Creates a hash signature provider using HMAC SHA-256 keyed hash algorithm.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateHS256(string secret)
        {
            var a = new HMACSHA256(Encoding.ASCII.GetBytes(secret ?? string.Empty));
            return new HashSignatureProvider(a, "HS256", true);
        }

        /// <summary>
        /// Creates a hash signature provider using HMAC SHA-256 keyed hash algorithm.
        /// </summary>
        /// <param name="secret">The secret.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateHS256(byte[] secret)
        {
            var a = new HMACSHA256(secret);
            return new HashSignatureProvider(a, "HS256", true);
        }

        /// <summary>
        /// Creates a hash signature provider using SHA-512 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="shortName">true if use short name; otherwise, false.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateSHA512(bool shortName = false)
        {
            return new HashSignatureProvider(SHA512.Create(), shortName ? "S512" : "SHA512", true);
        }

        /// <summary>
        /// Creates a hash signature provider using SHA-384 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="shortName">true if use short name; otherwise, false.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateSHA384(bool shortName = false)
        {
            return new HashSignatureProvider(SHA384.Create(), shortName ? "S384" : "SHA384", true);
        }

        /// <summary>
        /// Creates a hash signature provider using SHA-256 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="shortName">true if use short name; otherwise, false.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateSHA256(bool shortName = false)
        {
            return new HashSignatureProvider(SHA256.Create(), shortName ? "S256" : "SHA256", true);
        }

        /// <summary>
        /// Creates a hash signature provider using SHA-3-512 hash algorithm.
        /// </summary>
        /// <param name="shortName">true if use short name; otherwise, false.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateSHA3512(bool shortName = false)
        {
            return new HashSignatureProvider(SHA3Managed.Create512(), shortName ? "S3512" : "SHA3512", true);
        }

        /// <summary>
        /// Creates a hash signature provider using SHA-3-384 hash algorithm.
        /// </summary>
        /// <param name="shortName">true if use short name; otherwise, false.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateSHA3384(bool shortName = false)
        {
            return new HashSignatureProvider(SHA3Managed.Create384(), shortName ? "S3384" : "SHA3384", true);
        }

        /// <summary>
        /// Creates a hash signature provider using SHA-3-256 hash algorithm.
        /// </summary>
        /// <param name="shortName">true if use short name; otherwise, false.</param>
        /// <returns>A keyed hash signature provider.</returns>
        public static HashSignatureProvider CreateSHA3256(bool shortName = false)
        {
            return new HashSignatureProvider(SHA3Managed.Create256(), shortName ? "S3256" : "SHA3256", true);
        }

        /// <summary>
        /// Initializes a new instance of the HashSignatureProvider class.
        /// </summary>
        /// <param name="algorithm">The hash algorithm</param>
        /// <param name="name">The signature algorithm name.</param>
        /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
        public HashSignatureProvider(HashAlgorithm algorithm, string name, bool needDisposeAlgorithmAutomatically = false)
        {
            Name = name;
            alg = algorithm;
            needDispose = needDisposeAlgorithmAutomatically;
        }

        /// <summary>
        /// Gets the signature name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether it can sign a specific data.
        /// </summary>
        public bool CanSign => true;

        /// <summary>
        /// Computes the signature for the specified hash value.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <returns>The signature for the specified hash value.</returns>
        public byte[] Sign(byte[] data)
        {
            return alg.ComputeHash(data);
        }

        /// <summary>
        /// Verifies that a digital signature is valid by calculating the hash value of the specified data
        /// using the specified hash algorithm and padding, and comparing it to the provided signature.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <returns>true if the signature is valid; otherwise, false.</returns>
        public bool Verify(byte[] data, byte[] signature)
        {
            return ListExtension.Equals(Sign(data), signature);
        }

        /// <summary>
        /// Releases all resources used by the current signature provider object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (needDispose && alg != null) alg.Dispose();
        }
    }

    /// <summary>
    /// The RSA hash signature for string.
    /// </summary>
    public class RSASignatureProvider : ISignatureProvider, IDisposable
    {
        private readonly bool needDispose;
        private readonly RSA rsa;
        private readonly HashAlgorithmName hashName;

        /// <summary>
        /// Creates an RSA hash signature provider using SHA-512 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="secret">The RSA key.</param>
        /// <returns>An RSA hash signature provider instance.</returns>
        public static RSASignatureProvider CreateRS512(string secret)
        {
            return Create(secret, HashAlgorithmName.SHA512, "RS512");
        }

        /// <summary>
        /// Creates an RSA hash signature provider using SHA-512 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="secret">The RSA parameters.</param>
        /// <returns>An RSA hash signature provider instance.</returns>
        public static RSASignatureProvider CreateRS512(RSAParameters secret)
        {
            return new RSASignatureProvider(secret, HashAlgorithmName.SHA512, "RS512");
        }

        /// <summary>
        /// Creates an RSA hash signature provider using SHA-512 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="rsaInstance">The RSA instance.</param>
        /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
        /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
        /// <returns>An RSA hash signature provider instance.</returns>
        public static RSASignatureProvider CreateRS512(RSA rsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        {
            return new RSASignatureProvider(rsaInstance, hasPrivateKey, HashAlgorithmName.SHA512, "RS512", needDisposeAlgorithmAutomatically);
        }

        /// <summary>
        /// Creates an RSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="secret">The RSA key.</param>
        /// <returns>An RSA hash signature provider instance.</returns>
        public static RSASignatureProvider CreateRS384(string secret)
        {
            return Create(secret, HashAlgorithmName.SHA384, "RS384");
        }

        /// <summary>
        /// Creates an RSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="secret">The RSA parameters.</param>
        /// <returns>An RSA hash signature provider instance.</returns>
        public static RSASignatureProvider CreateRS384(RSAParameters secret)
        {
            return new RSASignatureProvider(secret, HashAlgorithmName.SHA384, "RS384");
        }

        /// <summary>
        /// Creates an RSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="rsaInstance">The RSA instance.</param>
        /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
        /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
        /// <returns>An RSA hash signature provider instance.</returns>
        public static RSASignatureProvider CreateRS384(RSA rsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        {
            return new RSASignatureProvider(rsaInstance, hasPrivateKey, HashAlgorithmName.SHA384, "RS384", needDisposeAlgorithmAutomatically);
        }

        /// <summary>
        /// Creates an RSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="secret">The RSA key.</param>
        /// <returns>An RSA hash signature provider instance.</returns>
        public static RSASignatureProvider CreateRS256(string secret)
        {
            return Create(secret, HashAlgorithmName.SHA256, "RS256");
        }

        /// <summary>
        /// Creates an RSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="secret">The RSA parameters.</param>
        /// <returns>An RSA hash signature provider instance.</returns>
        public static RSASignatureProvider CreateRS256(RSAParameters secret)
        {
            return new RSASignatureProvider(secret, HashAlgorithmName.SHA256, "RS256");
        }


        /// <summary>
        /// Creates an RSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
        /// </summary>
        /// <param name="rsaInstance">The RSA instance.</param>
        /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
        /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
        /// <returns>An RSA hash signature provider instance.</returns>
        public static RSASignatureProvider CreateRS256(RSA rsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        {
            return new RSASignatureProvider(rsaInstance, hasPrivateKey, HashAlgorithmName.SHA256, "RS256", needDisposeAlgorithmAutomatically);
        }

        /// <summary>
        /// Creates an RSA hash signature provider.
        /// </summary>
        /// <param name="secret">The RSA key.</param>
        /// <param name="hashAlgorithmName">The hash algorithm name.</param>
        /// <param name="signAlgorithmName">The signature algorithm name.</param>
        /// <returns>An RSA hash signature provider instance.</returns>
        private static RSASignatureProvider Create(string secret, HashAlgorithmName hashAlgorithmName, string signAlgorithmName)
        {
            if (string.IsNullOrWhiteSpace(secret))
                throw new ArgumentNullException(nameof(secret), "secret should not be null, empty or consists only of white-space characters.");
            var p = RSAParametersParse.Invoke(secret);
            if (p == null)
                throw new FormatException("secret is not a valid RSA key. A PEM string or XML string expected.");
            return new RSASignatureProvider(p.Value, hashAlgorithmName, signAlgorithmName);
        }

        /// <summary>
        /// Initializes a new instance of the RSASignatureProvider class.
        /// </summary>
        /// <param name="rsaParams">The RSA parameters.</param>
        /// <param name="hashAlgorithmName">The hash algorithm name.</param>
        /// <param name="signAlgorithmName">The signature algorithm name.</param>
        public RSASignatureProvider(RSAParameters rsaParams, HashAlgorithmName hashAlgorithmName, string signAlgorithmName)
        {
            Name = signAlgorithmName;
            rsa = RSA.Create();
            needDispose = true;
            CanSign = rsaParams.D!= null && rsaParams.D.Length > 0;
            rsa.ImportParameters(rsaParams);
            hashName = hashAlgorithmName;
        }

        /// <summary>
        /// Initializes a new instance of the RSASignatureProvider class.
        /// </summary>
        /// <param name="rsaInstance">The RSA instance.</param>
        /// <param name="hashAlgorithmName">The hash algorithm name.</param>
        /// <param name="signAlgorithmName">The signature algorithm name.</param>
        /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
        public RSASignatureProvider(RSA rsaInstance, HashAlgorithmName hashAlgorithmName, string signAlgorithmName, bool needDisposeAlgorithmAutomatically = false) : this(rsaInstance, true, hashAlgorithmName, signAlgorithmName, needDisposeAlgorithmAutomatically)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RSASignatureProvider class.
        /// </summary>
        /// <param name="rsaInstance">The RSA instance.</param>
        /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
        /// <param name="hashAlgorithmName">The hash algorithm name.</param>
        /// <param name="signAlgorithmName">The signature algorithm name.</param>
        /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
        public RSASignatureProvider(RSA rsaInstance, bool hasPrivateKey, HashAlgorithmName hashAlgorithmName, string signAlgorithmName, bool needDisposeAlgorithmAutomatically = false)
        {
            Name = signAlgorithmName;
            rsa = rsaInstance;
            hashName = hashAlgorithmName;
            if (rsa == null || !hasPrivateKey) return;
            needDispose = needDisposeAlgorithmAutomatically;
            try
            {
                var p = rsa.ExportParameters(true);
                CanSign = p.D != null && p.D.Length > 0;
            }
            catch (SystemException)
            {
            }
            catch (ApplicationException)
            {
            }
        }

        /// <summary>
        /// Gets the signature name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether it can sign a specific data.
        /// </summary>
        public bool CanSign { get; private set; }

        /// <summary>
        /// Computes the signature for the specified hash value.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <returns>The signature for the specified hash value.</returns>
        public byte[] Sign(byte[] data)
        {
            return rsa.SignData(data, hashName, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// Verifies that a digital signature is valid by calculating the hash value of the specified data
        /// using the specified hash algorithm and padding, and comparing it to the provided signature.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <returns>true if the signature is valid; otherwise, false.</returns>
        public bool Verify(byte[] data, byte[] signature)
        {
            return rsa.VerifyData(data, signature, hashName, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// Releases all resources used by the current signature provider object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (needDispose && rsa != null) rsa.Dispose();
        }
    }

    /// <summary>
    /// The customized keyed signature for string.
    /// </summary>
    public class KeyedSignatureProvider : ISignatureProvider
    {
        private readonly SignHandler sign;
        private readonly VerifyHandler verify;
        private readonly byte[] secretBytes;

        /// <summary>
        /// Signature algorithm.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="secret">The secret.</param>
        /// <returns>The signature.</returns>
        public delegate byte[] SignHandler(byte[] data, byte[] secret);

        /// <summary>
        /// Signature verify algorithm for JWT.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="signature">The signature.</param>
        /// <param name="secret">The secret.</param>
        /// <returns>The signature.</returns>
        public delegate bool VerifyHandler(byte[] data, byte[] signature, byte[] secret);

        /// <summary>
        /// Initializes a new instance of the KeyedSignatureProvider class.
        /// </summary>
        /// <param name="signHandler">The signature handler.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="name">The signature algorithm name.</param>
        public KeyedSignatureProvider(SignHandler signHandler, byte[] secret, string name)
        {
            secretBytes = secret;
            Name = name;
            sign = signHandler;
            CanSign = true;
        }

        /// <summary>
        /// Initializes a new instance of the KeyedSignatureProvider class.
        /// </summary>
        /// <param name="signHandler">The signature handler.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="name">The signature algorithm name.</param>
        public KeyedSignatureProvider(SignHandler signHandler, string secret, string name)
        {
            secretBytes = Encoding.ASCII.GetBytes(secret);
            Name = name;
            sign = signHandler;
            CanSign = true;
        }

        /// <summary>
        /// Initializes a new instance of the KeyedSignatureProvider class.
        /// </summary>
        /// <param name="verifyHandler">The verify handler.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="name">The signature algorithm name.</param>
        public KeyedSignatureProvider(VerifyHandler verifyHandler, byte[] secret, string name)
        {
            secretBytes = secret;
            Name = name;
            verify = verifyHandler;
            CanSign = true;
        }

        /// <summary>
        /// Initializes a new instance of the KeyedSignatureProvider class.
        /// </summary>
        /// <param name="verifyHandler">The verify handler.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="name">The signature algorithm name.</param>
        public KeyedSignatureProvider(VerifyHandler verifyHandler, string secret, string name)
        {
            secretBytes = Encoding.ASCII.GetBytes(secret);
            Name = name;
            verify = verifyHandler;
            CanSign = true;
        }

        /// <summary>
        /// Initializes a new instance of the KeyedSignatureProvider class.
        /// </summary>
        /// <param name="signHandler">The signature handler.</param>
        /// <param name="verifyHandler">The verify handler.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="name">The signature algorithm name.</param>
        public KeyedSignatureProvider(SignHandler signHandler, VerifyHandler verifyHandler, byte[] secret, string name) : this(signHandler, secret, name)
        {
            verify = verifyHandler;
        }

        /// <summary>
        /// Initializes a new instance of the KeyedSignatureProvider class.
        /// </summary>
        /// <param name="signHandler">The signature handler.</param>
        /// <param name="verifyHandler">The verify handler.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="name">The signature algorithm name.</param>
        public KeyedSignatureProvider(SignHandler signHandler, VerifyHandler verifyHandler, string secret, string name) : this(signHandler, secret, name)
        {
            verify = verifyHandler;
        }

        /// <summary>
        /// Gets the signature name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether it can sign a specific data.
        /// </summary>
        public bool CanSign { get; }

        /// <summary>
        /// Computes the signature for the specified hash value.
        /// </summary>
        /// <param name="value">The value to sign.</param>
        /// <returns>The signature for the specified hash value.</returns>
        public byte[] Sign(byte[] value)
        {
            return sign(value, secretBytes);
        }

        /// <summary>
        /// Verifies that a digital signature is valid by calculating the hash value of the specified data
        /// using the specified hash algorithm and padding, and comparing it to the provided signature.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="signature">The signature data to be verified.</param>
        /// <returns>true if the signature is valid; otherwise, false.</returns>
        public bool Verify(byte[] data, byte[] signature)
        {
            return verify != null
                ? verify(data, signature, secretBytes)
                : ListExtension.Equals(sign(data, secretBytes), signature);
        }
    }
}
