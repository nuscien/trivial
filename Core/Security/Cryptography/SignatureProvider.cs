using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Trivial.Collection;
using Trivial.Text;

namespace Trivial.Security;

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
    /// Computes the signature for the specified hash value.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <returns>The signature for the specified hash value.</returns>
    byte[] Sign(Stream data);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="signature">The signature data to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    bool Verify(byte[] data, byte[] signature);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="signature">The signature data to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    bool Verify(Stream data, byte[] signature);
}

/// <summary>
/// The hash signature for string.
/// </summary>
[Guid("0476149D-9482-409B-9D75-427801ABC853")]
public class HashSignatureProvider : ISignatureProvider
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
        return new(a, "HS512", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-512 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS512(byte[] secret)
    {
        var a = new HMACSHA512(secret);
        return new(a, "HS512", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-512 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS512(SecureString secret)
        => CreateHS512(secret?.ToUnsecureString());

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-512 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS512(ReadOnlySpan<char> secret)
        => CreateHS512(secret.ToString());

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-384 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS384(string secret)
    {
        var a = new HMACSHA384(Encoding.ASCII.GetBytes(secret ?? string.Empty));
        return new(a, "HS384", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-384 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS384(byte[] secret)
    {
        var a = new HMACSHA384(secret);
        return new(a, "HS384", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-384 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS384(SecureString secret)
        => CreateHS384(secret?.ToUnsecureString());

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-384 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS384(ReadOnlySpan<char> secret)
        => CreateHS384(secret.ToString());

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-256 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS256(string secret)
    {
        var a = new HMACSHA256(Encoding.ASCII.GetBytes(secret ?? string.Empty));
        return new(a, "HS256", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-256 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS256(byte[] secret)
    {
        var a = new HMACSHA256(secret);
        return new(a, "HS256", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-256 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS256(SecureString secret)
        => CreateHS256(secret?.ToUnsecureString());

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-256 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateHS256(ReadOnlySpan<char> secret)
        => CreateHS256(secret.ToString());

    /// <summary>
    /// Creates a hash signature provider using SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="shortName">true if use short name; otherwise, false.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateSHA512(bool shortName = false)
        => new(SHA512.Create(), shortName ? "S512" : "SHA512", true);

    /// <summary>
    /// Creates a hash signature provider using SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="shortName">true if use short name; otherwise, false.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateSHA384(bool shortName = false)
        => new(SHA384.Create(), shortName ? "S384" : "SHA384", true);

    /// <summary>
    /// Creates a hash signature provider using SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="shortName">true if use short name; otherwise, false.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateSHA256(bool shortName = false)
        => new(SHA256.Create(), shortName ? "S256" : "SHA256", true);

#if NETCOREAPP
    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-512 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3512(string secret)
    {
        var a = new HMACSHA3_512(Encoding.ASCII.GetBytes(secret ?? string.Empty));
        return new(a, "HS3512", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-512 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3512(byte[] secret)
    {
        var a = new HMACSHA3_512(secret);
        return new(a, "HS3512", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-512 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3512(SecureString secret)
        => CreateHS3512(secret?.ToUnsecureString());

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-512 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3512(ReadOnlySpan<char> secret)
        => CreateHS3512(secret.ToString());

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-384 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3384(string secret)
    {
        var a = new HMACSHA3_384(Encoding.ASCII.GetBytes(secret ?? string.Empty));
        return new(a, "HS3384", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-384 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3384(byte[] secret)
    {
        var a = new HMACSHA3_384(secret);
        return new(a, "HS3384", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-384 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3384(SecureString secret)
        => CreateHS3384(secret?.ToUnsecureString());

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-384 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3384(ReadOnlySpan<char> secret)
        => CreateHS3384(secret.ToString());

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-256 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3256(string secret)
    {
        var a = new HMACSHA3_256(Encoding.ASCII.GetBytes(secret ?? string.Empty));
        return new(a, "HS3256", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-256 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3256(byte[] secret)
    {
        var a = new HMACSHA3_256(secret);
        return new(a, "HS3256", true);
    }

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-256 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3256(SecureString secret)
        => CreateHS3256(secret?.ToUnsecureString());

    /// <summary>
    /// Creates a hash signature provider using HMAC SHA-256 keyed hash algorithm.
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A keyed hash signature provider.</returns>
    [SupportedOSPlatform("windows10.0.25324")]
    public static HashSignatureProvider CreateHS3256(ReadOnlySpan<char> secret)
        => CreateHS3256(secret.ToString());
#endif

    /// <summary>
    /// Creates a hash signature provider using SHA-3-512 hash algorithm.
    /// </summary>
    /// <param name="shortName">true if use short name; otherwise, false.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateSHA3512(bool shortName = false)
        => new(SHA3Managed.Create512(), shortName ? "S3512" : "SHA3512", true);

    /// <summary>
    /// Creates a hash signature provider using SHA-3-384 hash algorithm.
    /// </summary>
    /// <param name="shortName">true if use short name; otherwise, false.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateSHA3384(bool shortName = false)
        => new(SHA3Managed.Create384(), shortName ? "S3384" : "SHA3384", true);

    /// <summary>
    /// Creates a hash signature provider using SHA-3-256 hash algorithm.
    /// </summary>
    /// <param name="shortName">true if use short name; otherwise, false.</param>
    /// <returns>A keyed hash signature provider.</returns>
    public static HashSignatureProvider CreateSHA3256(bool shortName = false)
        => new(SHA3Managed.Create256(), shortName ? "S3256" : "SHA3256", true);

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
    /// Deconstructor.
    /// </summary>
    ~HashSignatureProvider()
    {
        if (!needDispose || alg == null) return;
        alg.Dispose();
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
        => alg.ComputeHash(data);

    /// <summary>
    /// Computes the signature for the specified hash value.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <returns>The signature for the specified hash value.</returns>
    public byte[] Sign(Stream data)
        => alg.ComputeHash(data);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="signature">The signature data to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public bool Verify(byte[] data, byte[] signature)
        => ListExtensions.Equals(Sign(data), signature);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="signature">The signature data to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public bool Verify(Stream data, byte[] signature)
        => ListExtensions.Equals(Sign(data), signature);
}

/// <summary>
/// The hash signature for string, by Rivest-Shamir-Adleman algorithm.
/// </summary>
[Guid("0A09C949-B088-461E-A886-A72C06C296EB")]
public class RSASignatureProvider : ISignatureProvider
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
        => Create(secret, RSASignaturePadding.Pkcs1, HashAlgorithmName.SHA512, "RS512");

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA parameters.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS512(RSAParameters secret)
        => new(secret, HashAlgorithmName.SHA512, "RS512");

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA instance created.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS512(out RSA secret)
    {
        secret = RSA.Create();
        return new(secret, true, HashAlgorithmName.SHA512, "RS512");
    }

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="rsaInstance">The RSA instance.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS512(RSA rsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        => new(rsaInstance, hasPrivateKey, HashAlgorithmName.SHA512, "RS512", needDisposeAlgorithmAutomatically);

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA key.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS384(string secret)
        => Create(secret, RSASignaturePadding.Pkcs1, HashAlgorithmName.SHA384, "RS384");

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA parameters.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS384(RSAParameters secret)
        => new(secret, HashAlgorithmName.SHA384, "RS384");

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA instance created.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS384(out RSA secret)
    {
        secret = RSA.Create();
        return new(secret, true, HashAlgorithmName.SHA384, "RS384");
    }

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="rsaInstance">The RSA instance.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS384(RSA rsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        => new(rsaInstance, hasPrivateKey, HashAlgorithmName.SHA384, "RS384", needDisposeAlgorithmAutomatically);

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA key.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS256(string secret)
        => Create(secret, RSASignaturePadding.Pkcs1, HashAlgorithmName.SHA256, "RS256");

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA parameters.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS256(RSAParameters secret)
        => new(secret, HashAlgorithmName.SHA256, "RS256");

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA instance created.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS256(out RSA secret)
    {
        secret = RSA.Create();
        return new(secret, true, HashAlgorithmName.SHA256, "RS256");
    }

    /// <summary>
    /// Creates an RSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="rsaInstance">The RSA instance.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreateRS256(RSA rsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        => new(rsaInstance, hasPrivateKey, HashAlgorithmName.SHA256, "RS256", needDisposeAlgorithmAutomatically);

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding and SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA key.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS512(string secret)
        => Create(secret, RSASignaturePadding.Pss, HashAlgorithmName.SHA512, "PS512");

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA parameters.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS512(RSAParameters secret)
        => new(secret, RSASignaturePadding.Pss, HashAlgorithmName.SHA512, "PS512");

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA instance created.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS512(out RSA secret)
    {
        secret = RSA.Create();
        return new(secret, RSASignaturePadding.Pss, true, HashAlgorithmName.SHA512, "PS512");
    }

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="rsaInstance">The RSA instance.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS512(RSA rsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        => new(rsaInstance, RSASignaturePadding.Pss, hasPrivateKey, HashAlgorithmName.SHA512, "PS512", needDisposeAlgorithmAutomatically);

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA key.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS384(string secret)
        => Create(secret, RSASignaturePadding.Pss, HashAlgorithmName.SHA384, "PS384");

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA parameters.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS384(RSAParameters secret)
        => new(secret, RSASignaturePadding.Pss, HashAlgorithmName.SHA384, "PS384");

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA instance created.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS384(out RSA secret)
    {
        secret = RSA.Create();
        return new(secret, RSASignaturePadding.Pss, true, HashAlgorithmName.SHA384, "PS384");
    }

    /// <summary>
    /// Creates an RSA hash signature provider usin probabilistic signature scheme paddingg SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="rsaInstance">The RSA instance.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS384(RSA rsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        => new(rsaInstance, RSASignaturePadding.Pss, hasPrivateKey, HashAlgorithmName.SHA384, "PS384", needDisposeAlgorithmAutomatically);

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA key.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS256(string secret)
        => Create(secret, RSASignaturePadding.Pss, HashAlgorithmName.SHA256, "PS256");

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA parameters.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS256(RSAParameters secret)
        => new(secret, RSASignaturePadding.Pss, HashAlgorithmName.SHA256, "PS256");

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The RSA instance created.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS256(out RSA secret)
    {
        secret = RSA.Create();
        return new(secret, RSASignaturePadding.Pss, true, HashAlgorithmName.SHA256, "PS256");
    }

    /// <summary>
    /// Creates an RSA hash signature provider using probabilistic signature scheme padding SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="rsaInstance">The RSA instance.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    public static RSASignatureProvider CreatePS256(RSA rsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        => new(rsaInstance, RSASignaturePadding.Pss, hasPrivateKey, HashAlgorithmName.SHA256, "PS256", needDisposeAlgorithmAutomatically);

    /// <summary>
    /// Creates an RSA hash signature provider.
    /// </summary>
    /// <param name="secret">The RSA key.</param>
    /// <param name="padding">The signature padding.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <param name="signAlgorithmName">The signature algorithm name.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    /// <exception cref="ArgumentNullException">secret was null.</exception>
    /// <exception cref="ArgumentException">secret was empty or consists only of white-space characters.</exception>
    private static RSASignatureProvider Create(string secret, RSASignaturePadding padding, HashAlgorithmName hashAlgorithmName, string signAlgorithmName)
    {
        StringExtensions.AssertNotWhiteSpace(nameof(secret), secret);
        var p = RSAParametersConvert.Parse(secret);
        return p == null
            ? throw new FormatException("secret is not a valid RSA key. A PEM string or XML string expected.")
            : new(p.Value, padding, hashAlgorithmName, signAlgorithmName);
    }

    /// <summary>
    /// Initializes a new instance of the RSASignatureProvider class.
    /// </summary>
    /// <param name="rsaParams">The RSA parameters.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <param name="signAlgorithmName">The signature algorithm name.</param>
    public RSASignatureProvider(RSAParameters rsaParams, HashAlgorithmName hashAlgorithmName, string signAlgorithmName)
        : this(rsaParams, RSASignaturePadding.Pkcs1, hashAlgorithmName, signAlgorithmName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RSASignatureProvider class.
    /// </summary>
    /// <param name="rsaInstance">The RSA instance.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <param name="signAlgorithmName">The signature algorithm name.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    public RSASignatureProvider(RSA rsaInstance, HashAlgorithmName hashAlgorithmName, string signAlgorithmName, bool needDisposeAlgorithmAutomatically = false)
        : this(rsaInstance, true, hashAlgorithmName, signAlgorithmName, needDisposeAlgorithmAutomatically)
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
        : this(rsaInstance, RSASignaturePadding.Pkcs1, hasPrivateKey, hashAlgorithmName, signAlgorithmName, needDisposeAlgorithmAutomatically)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RSASignatureProvider class.
    /// </summary>
    /// <param name="rsaParams">The RSA parameters.</param>
    /// <param name="padding">The signature padding.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <param name="signAlgorithmName">The signature algorithm name.</param>
    public RSASignatureProvider(RSAParameters rsaParams, RSASignaturePadding padding, HashAlgorithmName hashAlgorithmName, string signAlgorithmName)
    {
        Name = signAlgorithmName;
        rsa = RSA.Create();
        Padding = padding;
        needDispose = true;
        CanSign = rsaParams.D != null && rsaParams.D.Length > 0;
        rsa.ImportParameters(rsaParams);
        hashName = hashAlgorithmName;
    }

    /// <summary>
    /// Initializes a new instance of the RSASignatureProvider class.
    /// </summary>
    /// <param name="rsaInstance">The RSA instance.</param>
    /// <param name="padding">The signature padding.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <param name="signAlgorithmName">The signature algorithm name.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    public RSASignatureProvider(RSA rsaInstance, RSASignaturePadding padding, bool hasPrivateKey, HashAlgorithmName hashAlgorithmName, string signAlgorithmName, bool needDisposeAlgorithmAutomatically = false)
    {
        Name = signAlgorithmName;
        rsa = rsaInstance;
        Padding = padding;
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
    /// Deconstructor.
    /// </summary>
    ~RSASignatureProvider()
    {
        if (!needDispose || rsa == null) return;
        rsa.Dispose();
    }

    /// <summary>
    /// Gets the signature name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the signature padding.
    /// </summary>
    public RSASignaturePadding Padding { get; }

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
        => rsa.SignData(data, hashName, RSASignaturePadding.Pkcs1);

    /// <summary>
    /// Computes the signature for the specified hash value.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <returns>The signature for the specified hash value.</returns>
    public byte[] Sign(Stream data)
        => rsa.SignData(data, hashName, RSASignaturePadding.Pkcs1);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="signature">The signature data to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public bool Verify(byte[] data, byte[] signature)
        => rsa.VerifyData(data, signature, hashName, RSASignaturePadding.Pkcs1);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="signature">The signature data to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public bool Verify(Stream data, byte[] signature)
        => rsa.VerifyData(data, signature, hashName, RSASignaturePadding.Pkcs1);
}

#if !NETFRAMEWORK
/// <summary>
/// The hash signature for string, by Elliptic Curve Digital Signature Algorithm.
/// </summary>
[Guid("5EB7E921-9CF8-41EA-96D1-9F7DA015FFEE")]
public class ECDsaSignatureProvider : ISignatureProvider
{
    private readonly bool needDispose;
    private readonly ECDsa ecdsa;
    private readonly HashAlgorithmName hashName;

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The ECC DSA PEM.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES512(string secret)
        => Create(secret, HashAlgorithmName.SHA512, "ES512");

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The ECC DSA parameters.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES512(ECParameters secret)
        => new(secret, HashAlgorithmName.SHA512, "ES512");

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The ECC DSA instance created.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES512(out ECDsa secret)
    {
        secret = ECDsa.Create();
        return new(secret, true, HashAlgorithmName.SHA512, "ES512");
    }

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-512 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="ecdsaInstance">The ECC DSA instance.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES512(ECDsa ecdsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        => new(ecdsaInstance, hasPrivateKey, HashAlgorithmName.SHA512, "ES512", needDisposeAlgorithmAutomatically);

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The ECC DSA PEM.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES384(string secret)
        => Create(secret, HashAlgorithmName.SHA384, "ES384");

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The ECC DSA parameters.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES384(ECParameters secret)
        => new(secret, HashAlgorithmName.SHA384, "ES384");

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The ECC DSA instance created.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES384(out ECDsa secret)
    {
        secret = ECDsa.Create();
        return new(secret, true, HashAlgorithmName.SHA384, "ES384");
    }

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-384 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="ecdsaInstance">The ECC DSA instance.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES384(ECDsa ecdsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        => new(ecdsaInstance, hasPrivateKey, HashAlgorithmName.SHA384, "ES384", needDisposeAlgorithmAutomatically);

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The ECC DSA PEM.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES256(string secret)
        => Create(secret, HashAlgorithmName.SHA256, "ES256");

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The ECC DSA parameters.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES256(ECParameters secret)
        => new(secret, HashAlgorithmName.SHA256, "ES256");

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="secret">The ECC DSA instance created.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES256(out ECDsa secret)
    {
        secret = ECDsa.Create();
        return new(secret, true, HashAlgorithmName.SHA256, "ES256");
    }

    /// <summary>
    /// Creates an ECC DSA hash signature provider using SHA-256 hash algorithm of SHA-2 family.
    /// </summary>
    /// <param name="ecdsaInstance">The ECC DSA instance.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    /// <returns>An ECC DSA hash signature provider instance.</returns>
    public static ECDsaSignatureProvider CreateES256(ECDsa ecdsaInstance, bool hasPrivateKey = true, bool needDisposeAlgorithmAutomatically = false)
        => new(ecdsaInstance, hasPrivateKey, HashAlgorithmName.SHA256, "ES256", needDisposeAlgorithmAutomatically);

    /// <summary>
    /// Creates an ECC DSA hash signature provider.
    /// </summary>
    /// <param name="secret">The ECC DSA key.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <param name="signAlgorithmName">The signature algorithm name.</param>
    /// <returns>An RSA hash signature provider instance.</returns>
    /// <exception cref="ArgumentNullException">secret was null.</exception>
    /// <exception cref="ArgumentException">secret was empty or consists only of white-space characters.</exception>
    private static ECDsaSignatureProvider Create(string secret, HashAlgorithmName hashAlgorithmName, string signAlgorithmName)
    {
        StringExtensions.AssertNotWhiteSpace(nameof(secret), secret);
        var cert = X509Certificate2.CreateFromPem(secret);
        var p = cert.GetECDsaPrivateKey();
        var hasPrivateKey = p is not null;
        if (!hasPrivateKey) p = cert.GetECDsaPublicKey();
        return p == null
            ? throw new FormatException("secret is not a valid ECDsa key. A PEM string expected.")
            : new(p, hasPrivateKey, hashAlgorithmName, signAlgorithmName);
    }

    /// <summary>
    /// Initializes a new instance of the ECDsaSignatureProvider class.
    /// </summary>
    /// <param name="ecdsaParams">The ECC DSA parameters.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <param name="signAlgorithmName">The signature algorithm name.</param>
    public ECDsaSignatureProvider(ECParameters ecdsaParams, HashAlgorithmName hashAlgorithmName, string signAlgorithmName)
    {
        Name = signAlgorithmName;
        ecdsa = ECDsa.Create(ecdsaParams);
        needDispose = true;
        CanSign = ecdsaParams.D != null && ecdsaParams.D.Length > 0;
        hashName = hashAlgorithmName;
    }

    /// <summary>
    /// Initializes a new instance of the ECDsaSignatureProvider class.
    /// </summary>
    /// <param name="ecdsaInstance">The ECC DSA instance.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <param name="signAlgorithmName">The signature algorithm name.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    public ECDsaSignatureProvider(ECDsa ecdsaInstance, HashAlgorithmName hashAlgorithmName, string signAlgorithmName, bool needDisposeAlgorithmAutomatically = false) : this(ecdsaInstance, true, hashAlgorithmName, signAlgorithmName, needDisposeAlgorithmAutomatically)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ECDsaSignatureProvider class.
    /// </summary>
    /// <param name="ecdsaInstance">The ECC DSA instance.</param>
    /// <param name="hasPrivateKey">true if has the private key; otherwise, false.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <param name="signAlgorithmName">The signature algorithm name.</param>
    /// <param name="needDisposeAlgorithmAutomatically">true if need dispose the given algorithm instance automatically when this object is disposed.</param>
    public ECDsaSignatureProvider(ECDsa ecdsaInstance, bool hasPrivateKey, HashAlgorithmName hashAlgorithmName, string signAlgorithmName, bool needDisposeAlgorithmAutomatically = false)
    {
        Name = signAlgorithmName;
        ecdsa = ecdsaInstance;
        hashName = hashAlgorithmName;
        if (ecdsa == null || !hasPrivateKey) return;
        needDispose = needDisposeAlgorithmAutomatically;
        try
        {
            var p = ecdsa.ExportParameters(true);
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
    /// Deconstructor.
    /// </summary>
    ~ECDsaSignatureProvider()
    {
        if (!needDispose || ecdsa == null) return;
        ecdsa.Dispose();
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
        => ecdsa.SignData(data, hashName);

    /// <summary>
    /// Computes the signature for the specified hash value.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <returns>The signature for the specified hash value.</returns>
    public byte[] Sign(Stream data)
        => ecdsa.SignData(data, hashName);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="signature">The signature data to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public bool Verify(byte[] data, byte[] signature)
        => ecdsa.VerifyData(data, signature, hashName);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="signature">The signature data to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public bool Verify(Stream data, byte[] signature)
        => ecdsa.VerifyData(data, signature, hashName);
}
#endif

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
        => sign(value, secretBytes);

    /// <summary>
    /// Computes the signature for the specified hash value.
    /// </summary>
    /// <param name="value">The value to sign.</param>
    /// <returns>The signature for the specified hash value.</returns>
    public byte[] Sign(Stream value)
    {
        var bytes = new byte[value.Length];
#if NETCOREAPP
        value.ReadExactly(bytes);
#else
        value.Read(bytes, 0, bytes.Length);
#endif
        value.Seek(0, SeekOrigin.Begin);
        return sign(bytes, secretBytes);
    }

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="signature">The signature data to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public bool Verify(byte[] data, byte[] signature)
        => verify != null
            ? verify(data, signature, secretBytes)
            : ListExtensions.Equals(sign(data, secretBytes), signature);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="signature">The signature data to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public bool Verify(Stream data, byte[] signature)
    {
        if (data == null || !data.CanRead) return false;
        var bytes = new byte[data.Length];
#if NETCOREAPP
        data.ReadExactly(bytes);
#else
        data.Read(bytes, 0, bytes.Length);
#endif
        data.Seek(0, SeekOrigin.Begin);
        return verify != null
            ? verify(bytes, signature, secretBytes)
            : ListExtensions.Equals(sign(bytes, secretBytes), signature);
    }
}
