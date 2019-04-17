﻿// --------------------------------------------------------------------------------------------------------------------
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

            // Create a new Stringbuilder to collect the bytes and create a string.
            var str = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
            {
                str.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return str.ToString();
        }

        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="alg">The hash algorithm instance.</param>
        /// <param name="secureString">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        public static string ComputeHashString(this HashAlgorithm alg, SecureString secureString, Encoding encoding = null)
        {
            return ComputeHashString(alg, secureString.ToUnsecureString(), encoding);
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
            using (var alg = h())
            {
                return ComputeHashString(alg, plainText, encoding);
            }
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
            using (var alg = h())
            {
                return ComputeHashString(h(), plainText);
            }
        }

        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="h">The hash algorithm object maker.</param>
        /// <param name="secureString">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        public static string ComputeHashString<T>(Func<T> h, SecureString secureString, Encoding encoding = null) where T : HashAlgorithm
        {
            return ComputeHashString(h, secureString.ToUnsecureString(), encoding);
        }

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
            if (name == null) return ComputeHashString(null, plainText, encoding);
            using (var alg = Create(name))
            {
                return ComputeHashString(alg, plainText, encoding);
            }
        }

        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="h">The hash algorithm name.</param>
        /// <param name="secureString">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        /// <exception cref="NotSupportedException">The hash algorithm name is not supported.</exception>
        public static string ComputeHashString<T>(HashAlgorithmName h, SecureString secureString, Encoding encoding = null)
        {
            return ComputeHashString(h, secureString.ToUnsecureString(), encoding);
        }

        /// <summary>
        /// Creates a hash algorithm instance.
        /// </summary>
        /// <param name="name">The hash algorithm name,</param>
        /// <returns>A hash algorithm instance.</returns>
        /// <exception cref="ArgumentNullException">name was null.</exception>
        /// <exception cref="ArgumentException">name.Name was null or empty.</exception>
        /// <exception cref="NotSupportedException">The hash algorithm name is not supported.</exception>
        public static HashAlgorithm Create(HashAlgorithmName name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name), "name should not be null.");
            if (name == HashAlgorithmName.SHA512) return SHA512.Create();
            if (name == HashAlgorithmName.MD5) return MD5.Create();
            if (name == HashAlgorithmName.SHA256) return SHA256.Create();
            if (name == HashAlgorithmName.SHA1) return SHA1.Create();
            if (name == HashAlgorithmName.SHA384) return SHA384.Create();
            if (string.IsNullOrWhiteSpace(name.Name)) throw new ArgumentException("name.Name should not be null or empty.", nameof(name));
            switch (name.Name.ToUpperInvariant().Replace("-", string.Empty))
            {
                case "SHA3512":
                case "KECCAK512":
                case "SHA3":
                    return SHA3Managed.Create512();
                case "SHA3384":
                case "KECCAK384":
                    return SHA3Managed.Create384();
                case "SHA3256":
                case "KECCAK256":
                    return SHA3Managed.Create256();
                case "SHA3224":
                case "KECCAK224":
                    return SHA3Managed.Create224();
                case "SHA256":
                    return SHA256.Create();
                case "SHA384":
                    return SHA384.Create();
                case "SHA512":
                case "SHA2":
                    return SHA512.Create();
                case "MD5":
                    return MD5.Create();
                case "SHA1":
                    return SHA1.Create();
            }

            return HashAlgorithm.Create(name.Name);
        }

        /// <summary>
        /// Computes a SHA-1 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        [Obsolete("SHA-1 is no longer considered secure. Please use SHA-512 or other better one instead.")]
        public static string ComputeSHA1String(string plainText, Encoding encoding = null)
        {
            // Create a new instance of the SHA512CryptoServiceProvider object.
            return ComputeHashString(SHA1.Create, plainText, encoding);
        }

        /// <summary>
        /// Computes a SHA-512 (of SHA-2 family) hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ComputeSHA512String(byte[] plainText)
        {
            // Create a new instance of the SHA-512 crypto service provider object.
            return ComputeHashString(SHA512.Create, plainText);
        }

        /// <summary>
        /// Computes a SHA-512 (of SHA-2 family) hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ComputeSHA512String(string plainText, Encoding encoding = null)
        {
            // Create a new instance of the SHA-512 crypto service provider object.
            return ComputeHashString(SHA512.Create, plainText, encoding);
        }

        /// <summary>
        /// Computes a SHA-512 (of SHA-2 family) hash string value of a specific string instance.
        /// </summary>
        /// <param name="secureString">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ComputeSHA512String(SecureString secureString, Encoding encoding = null)
        {
            // Create a new instance of the SHA-512 crypto service provider object.
            return ComputeHashString(SHA512.Create, secureString, encoding);
        }

        /// <summary>
        /// Computes a SHA-3-512 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ComputeSHA3512String(string plainText, Encoding encoding = null)
        {
            // Create a new instance of the SHA-3-512 crypto service provider object.
            return ComputeHashString(SHA3Managed.Create512, plainText, encoding);
        }

        /// <summary>
        /// Computes a SHA-3-512 hash string value of a specific string instance.
        /// </summary>
        /// <param name="secureString">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ComputeSHA3512String(SecureString secureString, Encoding encoding = null)
        {
            // Create a new instance of the SHA-3-512 crypto service provider object.
            return ComputeHashString(SHA3Managed.Create512, secureString, encoding);
        }

        /// <summary>
        /// Computes a SHA-3-512 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ComputeSHA3512String(byte[] plainText)
        {
            // Create a new instance of the SHA-3-512 crypto service provider object.
            return ComputeHashString(SHA3Managed.Create512, plainText);
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
                StringComparer.OrdinalIgnoreCase.Equals(ComputeHashString(h, plainText, encoding), hash) :
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
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ComputeSHA1String(plainText, encoding), hash);
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
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ComputeSHA512String(plainText, encoding), hash);
        }

        /// <summary>
        /// Verifies a SHA-512 (of SHA-2 family) hash against a string.
        /// </summary>
        /// <param name="secureString">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifySHA512(SecureString secureString, string hash, Encoding encoding = null)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ComputeSHA512String(secureString, encoding), hash);
        }

        /// <summary>
        /// Verifies a SHA-512 (of SHA-2 family) hash against a string.
        /// </summary>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifySHA512(byte[] plainText, string hash)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ComputeSHA512String(plainText), hash);
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
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ComputeSHA3512String(plainText, encoding), hash);
        }

        /// <summary>
        /// Verifies a SHA-3-512 hash against a string.
        /// </summary>
        /// <param name="secureString">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifySHA3512(SecureString secureString, string hash, Encoding encoding = null)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ComputeSHA3512String(secureString, encoding), hash);
        }

        /// <summary>
        /// Verifies a SHA-3-512 hash against a string.
        /// </summary>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifySHA3512(byte[] plainText, string hash)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ComputeSHA3512String(plainText), hash);
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
    }
}
