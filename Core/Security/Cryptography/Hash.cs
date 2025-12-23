using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Trivial.Reflection;
using Trivial.Web;

namespace Trivial.Security;

/// <summary>
/// The helper of hash functions.
/// </summary>
public static class HashUtility
{
    /// <summary>
    /// Computes a hash bytes of a specific string instance.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    public static byte[] ComputeHash(this HashAlgorithm alg, string plainText, Encoding encoding = null)
    {
        // Check if the parameter is not null.
        if (alg == null || plainText == null) return null;

        // Return the hash bytes.
        return alg.ComputeHash((encoding ?? Encoding.UTF8).GetBytes(plainText));
    }

    /// <summary>
    /// Computes a hash bytes of a specific string instance.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="secureString">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    public static byte[] ComputeHash(this HashAlgorithm alg, SecureString secureString, Encoding encoding = null)
        => ComputeHash(alg, secureString, encoding);

    /// <summary>
    /// Computes a hash bytes of a specific string instance.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="file">The file to hash.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    public static byte[] ComputeHash(this HashAlgorithm alg, FileInfo file)
    {
        if (alg == null || file == null) return null;
        using var stream = file.OpenRead();
        return alg.ComputeHash(stream);
    }

    /// <summary>
    /// Computes a hash string value of a specific string instance.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    public static string ComputeHashString(this HashAlgorithm alg, string plainText, Encoding encoding = null)
    {
        // Check if the parameter is not null.
        if (alg == null || plainText == null) return null;

        // Return the hexadecimal string.
        return ComputeHashString(alg, (encoding ?? Encoding.UTF8).GetBytes(plainText));
    }

    /// <summary>
    /// Computes a hash string value of a specific string instance.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="input">The original input value to get hash.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    public static string ComputeHashString(this HashAlgorithm alg, byte[] input)
    {
        // Check if the arguments is not null.
        if (alg == null || input == null) return null;

        // Convert the input string to a byte array and compute the hash.
        var data = alg.ComputeHash(input);

        // Return the hexadecimal string.
        return ObjectConvert.ToHexString(data);
    }

    /// <summary>
    /// Computes a hash string value of a specific string instance.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="secureString">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    public static string ComputeHashString(this HashAlgorithm alg, SecureString secureString, Encoding encoding = null)
        => ComputeHashString(alg, secureString.ToUnsecureString(), encoding);

    /// <summary>
    /// Gets the hash value of a file.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="file">The file to hash.</param>
    /// <returns>The hash value.</returns>
    /// <exception cref="UnauthorizedAccessException">Unauthorized to access the file.</exception>
    /// <exception cref="IOException">IO exception about the file.</exception>
    public static string ComputeHashString(this HashAlgorithm alg, FileInfo file)
    {
        if (alg == null || file == null) return null;
        byte[] arr;
        using (var stream = file.OpenRead())
        {
            arr = alg.ComputeHash(stream);
        }

        return ObjectConvert.ToHexString(arr);
    }

    /// <summary>
    /// Computes a hash string value of a specific string instance.
    /// </summary>
    /// <param name="h">The hash algorithm object maker.</param>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    public static string ComputeHashString<T>(Func<T> h, string plainText, Encoding encoding = null) where T : HashAlgorithm
    {
        // Check if the arguments is not null.
        if (h == null || plainText == null) return null;

        // Return the hash string computed.
        using var alg = h();
        return ComputeHashString(alg, plainText, encoding);
    }

    /// <summary>
    /// Computes a hash string value of a specific string instance.
    /// </summary>
    /// <param name="h">The hash algorithm object maker.</param>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    public static string ComputeHashString<T>(Func<T> h, byte[] plainText) where T : HashAlgorithm
    {
        // Check if the arguments is not null.
        if (h == null || plainText == null) return null;

        // Return the hash string computed.
        using var alg = h();
        return ComputeHashString(h(), plainText);
    }

    /// <summary>
    /// Gets the hash value of a file.
    /// </summary>
    /// <param name="h">The hash algorithm object maker.</param>
    /// <param name="file">The file to hash.</param>
    /// <returns>The hash value.</returns>
    /// <exception cref="UnauthorizedAccessException">Unauthorized to access the file.</exception>
    /// <exception cref="IOException">IO exception about the file.</exception>
    public static string ComputeHashString<T>(Func<T> h, FileInfo file) where T : HashAlgorithm
    {
        // Check if the arguments is not null.
        if (h == null || file == null) return null;

        // Return the hash string computed.
        using var alg = h();
        return ComputeHashString(h(), file);
    }

    /// <summary>
    /// Computes a hash string value of a specific string instance.
    /// </summary>
    /// <param name="h">The hash algorithm object maker.</param>
    /// <param name="secureString">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    public static string ComputeHashString<T>(Func<T> h, SecureString secureString, Encoding encoding = null) where T : HashAlgorithm
        => ComputeHashString(h, secureString.ToUnsecureString(), encoding);

#if NETFRAMEWORK
    /// <summary>
    /// Computes a hash string value of a specific string instance.
    /// </summary>
    /// <param name="name">The hash algorithm name.</param>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    /// <exception cref="ArgumentNullException">name was null.</exception>
    /// <exception cref="ArgumentException">name.Name was null or empty.</exception>
    /// <exception cref="NotSupportedException">The hash algorithm name is not supported.</exception>
    public static string ComputeHashString(HashAlgorithmName name, string plainText, Encoding encoding = null)
    {
        using var alg = Create(name);
        return ComputeHashString(alg, plainText, encoding);
    }

    /// <summary>
    /// Computes a hash string value of a specific string instance.
    /// </summary>
    /// <param name="h">The hash algorithm name.</param>
    /// <param name="secureString">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
    /// <exception cref="NotSupportedException">The hash algorithm name is not supported.</exception>
    public static string ComputeHashString(HashAlgorithmName h, SecureString secureString, Encoding encoding = null)
        => ComputeHashString(h, secureString.ToUnsecureString(), encoding);
#endif

    /// <summary>
    /// Creates a hash algorithm instance.
    /// </summary>
    /// <param name="name">The hash algorithm name,</param>
    /// <returns>A new instance of the specified hash algorithm, or null if hash name is not a valid hash algorithm..</returns>
    /// <exception cref="ArgumentNullException">name was null.</exception>
    /// <exception cref="ArgumentException">name.Name was null or empty.</exception>
    /// <exception cref="NotSupportedException">The hash algorithm name is not supported.</exception>
#if NETFRAMEWORK
    public static HashAlgorithm Create(HashAlgorithmName name)
#else
    internal static HashAlgorithm Create(HashAlgorithmName name)
#endif
    {
        if (name == HashAlgorithmName.SHA512) return SHA512.Create();
        if (name == HashAlgorithmName.MD5) return MD5.Create();
        if (name == HashAlgorithmName.SHA256) return SHA256.Create();
        if (name == HashAlgorithmName.SHA1) return SHA1.Create();
        if (name == HashAlgorithmName.SHA384) return SHA384.Create();
        Text.StringExtensions.AssertNotWhiteSpace("name.Name", name.Name);
        return (name.Name.ToUpperInvariant().Replace("-", string.Empty)) switch
        {
            "SHA3512" or "KECCAK512" or "SHA3" => SHA3Managed.Create512(),
            "SHA3384" or "KECCAK384" => SHA3Managed.Create384(),
            "SHA3256" or "KECCAK256" => SHA3Managed.Create256(),
            "SHA3224" or "KECCAK224" => SHA3Managed.Create224(),
            "SHA256" => SHA256.Create(),
            "SHA384" => SHA384.Create(),
            "SHA512" or "SHA2" => SHA512.Create(),
            "MD5" => MD5.Create(),
            "SHA1" => SHA1.Create(),
#if NETFRAMEWORK
            _ => HashAlgorithm.Create(name.Name)
#else
            _ => null
#endif
        };
    }

    /// <summary>
    /// Computes a SHA-1 hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    [Obsolete("SHA-1 is no longer considered secure. Please use SHA-512 or other better one instead.")]
    public static string ComputeSHA1String(string plainText, Encoding encoding = null)
        => ComputeHashString(SHA1.Create, plainText, encoding);

    /// <summary>
    /// Computes a SHA-256 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA256String(byte[] plainText)
        => ComputeHashString(SHA256.Create, plainText);

    /// <summary>
    /// Computes a SHA-256 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA256String(string plainText, Encoding encoding = null)
        => ComputeHashString(SHA256.Create, plainText, encoding);

    /// <summary>
    /// Computes a SHA-256 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="secureString">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA256String(SecureString secureString, Encoding encoding = null)
        => ComputeHashString(SHA256.Create, secureString, encoding);

    /// <summary>
    /// Computes a SHA-256 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="file">The file to hash.</param>
    /// <returns>The hash value.</returns>
    /// <exception cref="UnauthorizedAccessException">Unauthorized to access the file.</exception>
    /// <exception cref="IOException">IO exception about the file.</exception>
    public static string ComputeSHA256String(FileInfo file)
        => ComputeHashString(SHA256.Create, file);

    /// <summary>
    /// Computes a SHA-384 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA384String(byte[] plainText)
        => ComputeHashString(SHA384.Create, plainText);

    /// <summary>
    /// Computes a SHA-384 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA384String(string plainText, Encoding encoding = null)
        => ComputeHashString(SHA384.Create, plainText, encoding);

    /// <summary>
    /// Computes a SHA-384 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="secureString">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA384String(SecureString secureString, Encoding encoding = null)
        => ComputeHashString(SHA384.Create, secureString, encoding);

    /// <summary>
    /// Computes a SHA-384 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="file">The file to hash.</param>
    /// <returns>The hash value.</returns>
    /// <exception cref="UnauthorizedAccessException">Unauthorized to access the file.</exception>
    /// <exception cref="IOException">IO exception about the file.</exception>
    public static string ComputeSHA384String(FileInfo file)
        => ComputeHashString(SHA384.Create, file);

    /// <summary>
    /// Computes a SHA-512 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA512String(byte[] plainText)
        => ComputeHashString(SHA512.Create, plainText);

    /// <summary>
    /// Computes a SHA-512 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA512String(string plainText, Encoding encoding = null)
        => ComputeHashString(SHA512.Create, plainText, encoding);

    /// <summary>
    /// Computes a SHA-512 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="secureString">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA512String(SecureString secureString, Encoding encoding = null)
        => ComputeHashString(SHA512.Create, secureString, encoding);

    /// <summary>
    /// Computes a SHA-512 (of SHA-2 family) hash string value of a specific string instance.
    /// </summary>
    /// <param name="file">The file to hash.</param>
    /// <returns>The hash value.</returns>
    /// <exception cref="UnauthorizedAccessException">Unauthorized to access the file.</exception>
    /// <exception cref="IOException">IO exception about the file.</exception>
    public static string ComputeSHA512String(FileInfo file)
        => ComputeHashString(SHA512.Create, file);

    /// <summary>
    /// Computes a SHA-3-256 hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA3256String(string plainText, Encoding encoding = null)
        => ComputeHashString(SHA3Managed.Create256, plainText, encoding);

    /// <summary>
    /// Computes a SHA-3-512 hash string value of a specific string instance.
    /// </summary>
    /// <param name="secureString">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA3256String(SecureString secureString, Encoding encoding = null)
        => ComputeHashString(SHA3Managed.Create256, secureString, encoding);

    /// <summary>
    /// Computes a SHA-3-256 hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA3256String(byte[] plainText)
        => ComputeHashString(SHA3Managed.Create256, plainText);

    /// <summary>
    /// Computes a SHA-3-256 hash string value of a specific string instance.
    /// </summary>
    /// <param name="file">The file to hash.</param>
    /// <returns>The hash value.</returns>
    /// <exception cref="UnauthorizedAccessException">Unauthorized to access the file.</exception>
    /// <exception cref="IOException">IO exception about the file.</exception>
    public static string ComputeSHA3256String(FileInfo file)
        => ComputeHashString(SHA3Managed.Create256, file);

    /// <summary>
    /// Computes a SHA-3-384 hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA3384String(string plainText, Encoding encoding = null)
        => ComputeHashString(SHA3Managed.Create384, plainText, encoding);

    /// <summary>
    /// Computes a SHA-3-384 hash string value of a specific string instance.
    /// </summary>
    /// <param name="secureString">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA3384String(SecureString secureString, Encoding encoding = null)
        => ComputeHashString(SHA3Managed.Create384, secureString, encoding);

    /// <summary>
    /// Computes a SHA-3-384 hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA3384String(byte[] plainText)
        => ComputeHashString(SHA3Managed.Create384, plainText);

    /// <summary>
    /// Computes a SHA-3-384 hash string value of a specific string instance.
    /// </summary>
    /// <param name="file">The file to hash.</param>
    /// <returns>The hash value.</returns>
    /// <exception cref="UnauthorizedAccessException">Unauthorized to access the file.</exception>
    /// <exception cref="IOException">IO exception about the file.</exception>
    public static string ComputeSHA3384String(FileInfo file)
        => ComputeHashString(SHA3Managed.Create384, file);

    /// <summary>
    /// Computes a SHA-3-512 hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA3512String(string plainText, Encoding encoding = null)
        => ComputeHashString(SHA3Managed.Create512, plainText, encoding);

    /// <summary>
    /// Computes a SHA-3-512 hash string value of a specific string instance.
    /// </summary>
    /// <param name="secureString">The original input value to get hash.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA3512String(SecureString secureString, Encoding encoding = null)
        => ComputeHashString(SHA3Managed.Create512, secureString, encoding);

    /// <summary>
    /// Computes a SHA-3-512 hash string value of a specific string instance.
    /// </summary>
    /// <param name="plainText">The original input value to get hash.</param>
    /// <returns>A hash string value of the given string.</returns>
    public static string ComputeSHA3512String(byte[] plainText)
        => ComputeHashString(SHA3Managed.Create512, plainText);

    /// <summary>
    /// Computes a SHA-3-512 hash string value of a specific string instance.
    /// </summary>
    /// <param name="file">The file to hash.</param>
    /// <returns>The hash value.</returns>
    /// <exception cref="UnauthorizedAccessException">Unauthorized to access the file.</exception>
    /// <exception cref="IOException">IO exception about the file.</exception>
    public static string ComputeSHA3512String(FileInfo file)
        => ComputeHashString(SHA3Managed.Create512, file);

    /// <summary>
    /// Computes the signature for the specified hash value.
    /// </summary>
    /// <param name="alg">The signature provider.</param>
    /// <param name="value">The value to sign.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>The signature for the specified hash value.</returns>
    public static byte[] Sign(this ISignatureProvider alg, string value, Encoding encoding = null)
        => alg.Sign((encoding ?? Encoding.UTF8).GetBytes(value));

    /// <summary>
    /// Computes the signature for the specified hash value.
    /// </summary>
    /// <param name="alg">The signature provider.</param>
    /// <param name="plainText">The data to sign.</param>
    /// <returns>The signature in hex format string for the specified hash value.</returns>
    public static string SignToHex(this ISignatureProvider alg, byte[] plainText)
        => ObjectConvert.ToHexString(alg.Sign(plainText));

    /// <summary>
    /// Computes the signature for the specified hash value.
    /// </summary>
    /// <param name="alg">The signature provider.</param>
    /// <param name="plainText">The text to sign.</param>
    /// <param name="encoding">The encoding; or null, to use default, UTF-8.</param>
    /// <returns>The signature in hex format string for the specified hash value.</returns>
    public static string SignToHex(this ISignatureProvider alg, string plainText, Encoding encoding = null)
        => string.IsNullOrEmpty(plainText) ? plainText : SignToHex(alg, (encoding ?? Encoding.UTF8).GetBytes(plainText));

    /// <summary>
    /// Computes the signature for the specified hash value.
    /// </summary>
    /// <param name="sign">The signature provider.</param>
    /// <param name="plainText">The data to sign.</param>
    /// <returns>The signature in Base64Url format string for the specified hash value.</returns>
    public static string SignToBase64Url(this ISignatureProvider sign, byte[] plainText)
    {
        var data = sign.Sign(plainText);
        return WebFormat.Base64UrlEncode(data);
    }

    /// <summary>
    /// Computes the signature for the specified hash value.
    /// </summary>
    /// <param name="sign">The signature provider.</param>
    /// <param name="plainText">The text to sign.</param>
    /// <param name="encoding">The encoding; or null, to use default, UTF-8.</param>
    /// <returns>The signature in hex format string for the specified hash value.</returns>
    public static string SignToBase64Url(this ISignatureProvider sign, string plainText, Encoding encoding = null)
        => string.IsNullOrEmpty(plainText) ? plainText : SignToBase64Url(sign, (encoding ?? Encoding.UTF8).GetBytes(plainText));

    /// <summary>
    /// Verifies a hash against a string.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(this HashAlgorithm alg, string plainText, string hash, Encoding encoding = null)
        => alg != null ?
            StringComparer.OrdinalIgnoreCase.Equals(ComputeHashString(alg, plainText, encoding), hash) :
            (string.IsNullOrEmpty(hash) || StringComparer.OrdinalIgnoreCase.Equals(plainText, hash));

    /// <summary>
    /// Verifies a hash against a string.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="secureString">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(this HashAlgorithm alg, SecureString secureString, string hash, Encoding encoding = null)
        => Verify(alg, secureString.ToUnsecureString(), hash, encoding);

    /// <summary>
    /// Verifies a hash against a byte array.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash byte array for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(this HashAlgorithm alg, string plainText, byte[] hash, Encoding encoding = null)
        => alg != null ?
            StringComparer.OrdinalIgnoreCase.Equals(ComputeHash(alg, plainText, encoding), hash) :
            (hash == null || hash.Length == 0);

    /// <summary>
    /// Verifies a hash against a byte array.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="secureString">The original input value to test.</param>
    /// <param name="hash">A hash byte array for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(this HashAlgorithm alg, SecureString secureString, byte[] hash, Encoding encoding = null)
        => Verify(alg, secureString.ToUnsecureString(), hash, encoding);

    /// <summary>
    /// Verifies a hash against a bytes.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="input">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(this HashAlgorithm alg, byte[] input, string hash)
        => alg != null ?
            StringComparer.OrdinalIgnoreCase.Equals(ComputeHashString(alg, input), hash) :
            string.IsNullOrEmpty(hash);

    /// <summary>
    /// Verifies a hash against a bytes.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="input">The original input value to test.</param>
    /// <param name="hash">A hash bytes for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(this HashAlgorithm alg, byte[] input, byte[] hash)
        => alg != null ?
            Collection.ListExtensions.Equals(alg.ComputeHash(input), hash) :
            (hash == null || hash.Length == 0);

    /// <summary>
    /// Verifies a hash against a bytes.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="input">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(this HashAlgorithm alg, FileInfo input, string hash)
        => alg != null ?
            StringComparer.OrdinalIgnoreCase.Equals(ComputeHashString(alg, input), hash) :
            string.IsNullOrEmpty(hash);

    /// <summary>
    /// Verifies a hash against a bytes.
    /// </summary>
    /// <param name="alg">The hash algorithm instance.</param>
    /// <param name="input">The original input value to test.</param>
    /// <param name="hash">A hash bytes for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(this HashAlgorithm alg, FileInfo input, byte[] hash)
        => alg != null ?
            Collection.ListExtensions.Equals(alg.ComputeHash(input), hash) :
            (hash == null || hash.Length == 0);

    /// <summary>
    /// Verifies a hash against a string.
    /// </summary>
    /// <param name="h">The hash algorithm object maker.</param>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify<T>(Func<T> h, string plainText, string hash, Encoding encoding = null) where T : HashAlgorithm
        => h != null ?
            StringComparer.OrdinalIgnoreCase.Equals(ComputeHashString(h, plainText, encoding), hash) :
            (string.IsNullOrEmpty(hash) || StringComparer.OrdinalIgnoreCase.Equals(plainText, hash));

    /// <summary>
    /// Verifies a hash against a string.
    /// </summary>
    /// <param name="alg">One of algorithms about hash.</param>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(Func<string, string> alg, string plainText, string hash)
        => alg != null ?
            StringComparer.OrdinalIgnoreCase.Equals(alg(plainText), hash) :
            (string.IsNullOrEmpty(hash) || StringComparer.OrdinalIgnoreCase.Equals(plainText, hash));

#if NETFRAMEWORK
    /// <summary>
    /// Verifies a hash against a string.
    /// </summary>
    /// <param name="name">The hash algorithm name.</param>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(HashAlgorithmName name, string plainText, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeHashString(name, plainText, encoding), hash);

    /// <summary>
    /// Verifies a hash against a string.
    /// </summary>
    /// <param name="name">The hash algorithm name.</param>
    /// <param name="secureString">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool Verify(HashAlgorithmName name, SecureString secureString, string hash, Encoding encoding = null)
        => Verify(name, secureString.ToUnsecureString(), hash, encoding);
#endif

    /// <summary>
    /// Verifies a SHA-1 hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    [Obsolete("SHA-1 is no longer considered secure.")]
    public static bool VerifySHA1(string plainText, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA1String(plainText, encoding), hash);

    /// <summary>
    /// Verifies a SHA-256 (of SHA-2 family) hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA256(string plainText, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA256String(plainText, encoding), hash);

    /// <summary>
    /// Verifies a SHA-256 (of SHA-2 family) hash against a string.
    /// </summary>
    /// <param name="secureString">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA256(SecureString secureString, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA256String(secureString, encoding), hash);

    /// <summary>
    /// Verifies a SHA-256 (of SHA-2 family) hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA256(byte[] plainText, string hash)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA256String(plainText), hash);

    /// <summary>
    /// Verifies a SHA-384 (of SHA-2 family) hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA384(string plainText, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA384String(plainText, encoding), hash);

    /// <summary>
    /// Verifies a SHA-384 (of SHA-2 family) hash against a string.
    /// </summary>
    /// <param name="secureString">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA384(SecureString secureString, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA384String(secureString, encoding), hash);

    /// <summary>
    /// Verifies a SHA-384 (of SHA-2 family) hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA384(byte[] plainText, string hash)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA384String(plainText), hash);

    /// <summary>
    /// Verifies a SHA-512 (of SHA-2 family) hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA512(string plainText, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA512String(plainText, encoding), hash);

    /// <summary>
    /// Verifies a SHA-512 (of SHA-2 family) hash against a string.
    /// </summary>
    /// <param name="secureString">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA512(SecureString secureString, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA512String(secureString, encoding), hash);

    /// <summary>
    /// Verifies a SHA-512 (of SHA-2 family) hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA512(byte[] plainText, string hash)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA512String(plainText), hash);

    /// <summary>
    /// Verifies a SHA-3-256 hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA3256(string plainText, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA3512String(plainText, encoding), hash);

    /// <summary>
    /// Verifies a SHA-3-256 hash against a string.
    /// </summary>
    /// <param name="secureString">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA3256(SecureString secureString, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA3256String(secureString, encoding), hash);

    /// <summary>
    /// Verifies a SHA-3-256 hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA3256(byte[] plainText, string hash)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA3256String(plainText), hash);

    /// <summary>
    /// Verifies a SHA-3-384 hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA3384(string plainText, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA3384String(plainText, encoding), hash);

    /// <summary>
    /// Verifies a SHA-3-384 hash against a string.
    /// </summary>
    /// <param name="secureString">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA3384(SecureString secureString, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA3384String(secureString, encoding), hash);

    /// <summary>
    /// Verifies a SHA-3-384 hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA3384(byte[] plainText, string hash)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA3384String(plainText), hash);

    /// <summary>
    /// Verifies a SHA-3-512 hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA3512(string plainText, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA3512String(plainText, encoding), hash);

    /// <summary>
    /// Verifies a SHA-3-512 hash against a string.
    /// </summary>
    /// <param name="secureString">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA3512(SecureString secureString, string hash, Encoding encoding = null)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA3512String(secureString, encoding), hash);

    /// <summary>
    /// Verifies a SHA-3-512 hash against a string.
    /// </summary>
    /// <param name="plainText">The original input value to test.</param>
    /// <param name="hash">A hash string for comparing.</param>
    /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
    public static bool VerifySHA3512(byte[] plainText, string hash)
        => StringComparer.OrdinalIgnoreCase.Equals(ComputeSHA3512String(plainText), hash);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="alg">The signature provider.</param>
    /// <param name="plainText">The data to sign.</param>
    /// <param name="hash">The signature data in hex format string to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public static bool VerifyFromHex(this ISignatureProvider alg, byte[] plainText, string hash)
        => alg is not null && !string.IsNullOrEmpty(hash) && alg.Verify(plainText, ObjectConvert.FromHexString(hash));

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="alg">The signature provider.</param>
    /// <param name="plainText">The plain text to sign.</param>
    /// <param name="encoding">The encoding of the plain text.</param>
    /// <param name="hash">The signature data in hex format string to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public static bool VerifyFromHex(this ISignatureProvider alg, string plainText, Encoding encoding, string hash)
        => VerifyFromHex(alg, (encoding ?? Encoding.UTF8).GetBytes(plainText), hash);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="alg">The signature provider.</param>
    /// <param name="plainText">The data to sign.</param>
    /// <param name="hash">The signature data in hex format string to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public static bool VerifyFromHex(this ISignatureProvider alg, string plainText, string hash)
        => VerifyFromHex(alg, plainText, null, hash);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="alg">The signature provider.</param>
    /// <param name="plainText">The data to sign.</param>
    /// <param name="hash">The signature data in Base64Url format string to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public static bool VerifyFromBase64Url(this ISignatureProvider alg, byte[] plainText, string hash)
        => alg is not null && !string.IsNullOrEmpty(hash) && alg.Verify(plainText, WebFormat.Base64UrlDecode(hash));

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="alg">The signature provider.</param>
    /// <param name="plainText">The plain text to sign.</param>
    /// <param name="encoding">The encoding of the plain text.</param>
    /// <param name="hash">The signature data in Base64Url format string to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public static bool VerifyFromBase64Url(this ISignatureProvider alg, string plainText, Encoding encoding, string hash)
        => VerifyFromBase64Url(alg, (encoding ?? Encoding.UTF8).GetBytes(plainText), hash);

    /// <summary>
    /// Verifies that a digital signature is valid by calculating the hash value of the specified data
    /// using the specified hash algorithm and padding, and comparing it to the provided signature.
    /// </summary>
    /// <param name="alg">The signature provider.</param>
    /// <param name="plainText">The data to sign.</param>
    /// <param name="hash">The signature data in Base64Url format string to be verified.</param>
    /// <returns>true if the signature is valid; otherwise, false.</returns>
    public static bool VerifyFromBase64Url(this ISignatureProvider alg, string plainText, string hash)
        => VerifyFromBase64Url(alg, plainText, null, hash);
}
