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
        public static string ComputeHashString<T>(this T alg, string plainText, Encoding encoding = null) where T : HashAlgorithm
        {
            // Check if the arguments is not null.
            if (alg == null || plainText == null) return null;

            // Check if the parameter is not null.
            if (alg == null) return null;

            // Convert the input string to a byte array and compute the hash.
            var data = alg.ComputeHash((encoding ?? Encoding.UTF8).GetBytes(plainText));

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
        /// <param name="h">The hash algorithm object maker.</param>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        public static string ComputeHashString<T>(Func<T> h, string plainText, Encoding encoding = null) where T : HashAlgorithm
        {
            // Check if the arguments is not null.
            if (h == null || plainText == null) return null;

            // Return the hash string computed.
            return ComputeHashString(h(), plainText, encoding);
        }

        /// <summary>
        /// Computes a hash string value of a specific string instance.
        /// </summary>
        /// <param name="h">The hash algorithm name.</param>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        /// <exception cref="NotImplementedException">The hash algorithm name is not supported.</exception>
        public static string ComputeHashString<T>(HashAlgorithmName h, string plainText, Encoding encoding = null) where T : HashAlgorithm
        {
            if (h == null) return null;
            if (h == HashAlgorithmName.SHA512) return ComputeHashString(SHA512.Create, plainText, encoding);
            if (h == HashAlgorithmName.MD5) return ComputeHashString(MD5.Create, plainText, encoding);
            if (h == HashAlgorithmName.SHA256) return ComputeHashString(SHA256.Create, plainText, encoding);
            if (h == HashAlgorithmName.SHA1) return ComputeHashString(SHA1.Create, plainText, encoding);
            if (h == HashAlgorithmName.SHA384) return ComputeHashString(SHA384.Create, plainText, encoding);
            throw new NotSupportedException("The hash algorithm name is not supported.");
        }

        /// <summary>
        /// Computes a MD5 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ComputeMd5String(string plainText, Encoding encoding = null)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            return ComputeHashString(MD5.Create, plainText, encoding);
        }

        /// <summary>
        /// Computes a SHA1 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ComputeSha1String(string plainText, Encoding encoding = null)
        {
            // Create a new instance of the SHA512CryptoServiceProvider object.
            return ComputeHashString(SHA1.Create, plainText, encoding);
        }

        /// <summary>
        /// Computes a SHA512 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string ComputeSha512String(this string plainText, Encoding encoding = null)
        {
            // Create a new instance of the SHA512CryptoServiceProvider object.
            return ComputeHashString(SHA512.Create, plainText, encoding);
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
        /// Verifies a MD5 hash against a string.
        /// </summary>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifyMd5(string plainText, string hash, Encoding encoding = null)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ComputeMd5String(plainText, encoding), hash);
        }

        /// <summary>
        /// Verifies a SHA1 hash against a string.
        /// </summary>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifySha1(string plainText, string hash, Encoding encoding = null)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ComputeSha1String(plainText, encoding), hash);
        }

        /// <summary>
        /// Verifies a SHA512 hash against a string.
        /// </summary>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifySha512(string plainText, string hash, Encoding encoding = null)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(ComputeSha512String(plainText, encoding), hash);
        }
    }
}
