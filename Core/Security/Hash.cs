// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hash.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The helper and extension of hash functions.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Trivial.Security
{
    /// <summary>
    /// The helper of hash functions.
    /// </summary>
    public static class HashUtility
    {
        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="alg">The hash algorithm instance.</param>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        public static string ToHashString(this HashAlgorithm alg, string plainText, Encoding encoding = null)
        {
            // Check if the parameter is not null.
            if (alg == null || plainText == null) return null;

            // Convert the input string to a byte array and compute the hash.
            var data = alg.ComputeHash((encoding ?? Encoding.UTF8).GetBytes(plainText));

            // Return the hexadecimal string.
            return ToHashString(alg, data);
        }

        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="alg">The hash algorithm instance.</param>
        /// <param name="data">The original input value to get hash.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        public static string ToHashString(this HashAlgorithm alg, byte[] data)
        {
            // Check if the arguments is not null.
            if (alg == null || data == null) return null;

            // Create a new Stringbuilder to collect the bytes and create a string.
            var str = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
            {
                str.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return str.ToString().ToUpper();
        }

        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="alg">The hash algorithm instance.</param>
        /// <param name="secureString">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        public static string ToHashString(this HashAlgorithm alg, SecureString secureString, Encoding encoding = null)
        {
            return ToHashString(alg, secureString.ToUnsecureString(), encoding);
        }

        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="h">The hash algorithm object maker.</param>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        public static string ToHashString<T>(Func<T> h, string plainText, Encoding encoding = null) where T : HashAlgorithm
        {
            // Check if the arguments is not null.
            if (h == null || plainText == null) return null;

            // Return the hash string computed.
            return ToHashString(h(), plainText, encoding);
        }

        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="h">The hash algorithm object maker.</param>
        /// <param name="secureString">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        public static string ToHashString<T>(Func<T> h, SecureString secureString, Encoding encoding = null) where T : HashAlgorithm
        {
            return ToHashString(h, secureString.ToUnsecureString(), encoding);
        }

        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="name">The hash algorithm name.</param>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        /// <exception cref="NotSupportedException">The hash algorithm name is not supported.</exception>
        public static string ToHashString(HashAlgorithmName name, string plainText, Encoding encoding = null)
        {
            Func<HashAlgorithm> h = null;
            try
            {
                h = GetHashAlgorithmFactory(name);
            }
            catch (NotSupportedException)
            {
                throw new NotSupportedException("The hash algorithm name is not supported. Please use the factory or instance of its hash algorithm as the first parameter to call.");
            }

            if (h == null) return null;
            return ToHashString(h, plainText, encoding);
        }

        /// <summary>
        /// Creates a hash algorithm instance.
        /// </summary>
        /// <param name="name">The hash algorithm name,</param>
        /// <returns>A hash algorithm instance.</returns>
        public static HashAlgorithm Create(HashAlgorithmName name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name), "name should not be null.");
            var h = GetHashAlgorithmFactory(name);
            if (h == null) throw new ArgumentException(nameof(name), "name.Name should not be null or empty.");
            return h();
        }

        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="h">The hash algorithm name.</param>
        /// <param name="secureString">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        /// <exception cref="NotSupportedException">The hash algorithm name is not supported.</exception>
        public static string ToHashString<T>(HashAlgorithmName h, SecureString secureString, Encoding encoding = null)
        {
            return ToHashString(h, secureString.ToUnsecureString(), encoding);
        }

        /// <summary>
        /// Computes a SHA-1 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        [Obsolete("SHA-1 is no longer considered secure. Please use SHA-512 or other better one instead.")]
        public static string ToSHA1String(string plainText, Encoding encoding = null)
        {
            // Create a new instance of the SHA512CryptoServiceProvider object.
            return ToHashString(SHA1.Create, plainText, encoding);
        }

        /// <summary>
        /// Computes a SHA-512 (of SHA-2 family) hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ToSHA512String(string plainText, Encoding encoding = null)
        {
            // Create a new instance of the SHA-512 crypto service provider object.
            return ToHashString(SHA512.Create, plainText, encoding);
        }

        /// <summary>
        /// Computes a SHA-3-512 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ToSHA3512String(string plainText, Encoding encoding = null)
        {
            // Create a new instance of the SHA512CryptoServiceProvider object.
            return ToHashString(SHA3Managed.Create512, plainText, encoding);
        }

        /// <summary>
        /// Verifies a hash against a string.
        /// </summary>
        /// <param name="h">The hash algorithm object maker.</param>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool Verify<T>(this Func<T> h, string plainText, string hash, Encoding encoding = null) where T : HashAlgorithm
        {
            // Return the result after StringComparer comparing.
            return h != null ?
                StringComparer.OrdinalIgnoreCase.Equals(ToHashString(h, plainText, encoding), hash) :
                StringComparer.OrdinalIgnoreCase.Equals(plainText, hash);
        }

        /// <summary>
        /// Verifies a hash against a string.
        /// </summary>
        /// <param name="alg">One of algorithms about hash.</param>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool Verify(Func<string, string> alg, string plainText, string hash)
        {
            // Return the result after StringComparer comparing.
            return alg != null ?
                StringComparer.OrdinalIgnoreCase.Equals(alg(plainText), hash) :
                StringComparer.OrdinalIgnoreCase.Equals(plainText, hash);
        }

        /// <summary>
        /// Verifies a SHA-1 hash against a string.
        /// </summary>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        [Obsolete("SHA-1 is no longer considered secure.")]
        public static bool VerifySHA1(string plainText, string hash, Encoding encoding = null)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ToSHA1String(plainText, encoding), hash);
        }

        /// <summary>
        /// Verifies a SHA-512 (of SHA-2 family) hash against a string.
        /// </summary>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifySHA512(string plainText, string hash, Encoding encoding = null)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ToSHA512String(plainText, encoding), hash);
        }

        /// <summary>
        /// Verifies a SHA-3-512 hash against a string.
        /// </summary>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifySHA3512(string plainText, string hash, Encoding encoding = null)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ToSHA3512String(plainText, encoding), hash);
        }

        /// <summary>
        /// Computes the signature for the specified hash value.
        /// </summary>
        /// <param name="sign">The signature provider.</param>
        /// <param name="value">The value to sign.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>The signature for the specified hash value.</returns>
        public static byte[] Sign(this ISignatureProvider sign, string value, Encoding encoding = null)
        {
            return sign.Sign((encoding ?? Encoding.UTF8).GetBytes(value));
        }

        private static Func<HashAlgorithm> GetHashAlgorithmFactory(HashAlgorithmName h)
        {
            if (h == null) return null;
            if (h == HashAlgorithmName.SHA512) return SHA512.Create;
            if (h == HashAlgorithmName.MD5) return MD5.Create;
            if (h == HashAlgorithmName.SHA256) return SHA256.Create;
            if (h == HashAlgorithmName.SHA1) return SHA1.Create;
            if (h == HashAlgorithmName.SHA384) return SHA384.Create;
            if (string.IsNullOrWhiteSpace(h.Name)) return null;
            switch (h.Name.ToUpperInvariant().Replace("-", string.Empty))
            {
                case "SHA3512":
                    return SHA3Managed.Create512;
                case "SHA3384":
                    return SHA3Managed.Create384;
                case "SHA3256":
                    return SHA3Managed.Create256;
                case "SHA3224":
                    return SHA3Managed.Create224;
                case "SHA2":
                    return SHA512.Create;
                case "SHA256":
                    return SHA256.Create;
                case "SHA384":
                    return SHA384.Create;
                case "SHA512":
                    return SHA512.Create;
                case "MD5":
                    return MD5.Create;
            }

            throw new NotSupportedException("The hash algorithm name is not supported.");
        }
    }
}
