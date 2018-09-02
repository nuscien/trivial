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
    public static class HashExtension
    {
        /// <summary>
        /// Gets a hash string value of a specific string instance.
        /// </summary>
        /// <param name="alg">The hash algorithm instance.</param>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        public static string GetHash<T>(T alg, string plainText) where T : HashAlgorithm
        {
            // Check if the arguments is not null.
            if (alg == null || plainText == null) return null;

            // Check if the parameter is not null.
            if (alg == null) return null;

            // Convert the input string to a byte array and compute the hash.
            var data = alg.ComputeHash(Encoding.UTF8.GetBytes(plainText));

            // Create a new Stringbuilder to collect the bytes and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString().ToUpper();
        }

        /// <summary>
        /// Gets a hash string value of a specific string instance.
        /// </summary>
        /// <param name="h">The hash algorithm object maker.</param>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <returns>A hash string value of the given string; or null, if h or input is null.</returns>
        public static string GetHash<T>(Func<T> h, string plainText) where T : HashAlgorithm
        {
            // Check if the arguments is not null.
            if (h == null || plainText == null) return null;

            // Create a new instance of the algorithm object.
            var alg = h();

            // Check if the parameter is not null.
            if (alg == null) return null;

            // Convert the input string to a byte array and compute the hash.
            var data = alg.ComputeHash(Encoding.UTF8.GetBytes(plainText));

            // Create a new Stringbuilder to collect the bytes and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString().ToUpper();
        }

        /// <summary>
        /// Gets a MD5 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string GetMd5Hash(string plainText)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            return GetHash(MD5.Create, plainText);
        }

        /// <summary>
        /// Gets a SHA1 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string GetSha1Hash(string plainText)
        {
            // Create a new instance of the SHA512CryptoServiceProvider object.
            return GetHash(SHA1.Create, plainText);
        }

        /// <summary>
        /// Gets a SHA512 hash string value of a specific string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string GetSha512Hash(string plainText)
        {
            // Create a new instance of the SHA512CryptoServiceProvider object.
            return GetHash(SHA512.Create, plainText);
        }

        /// <summary>
        /// Verifies a MD5 hash against a string.
        /// </summary>
        /// <param name="h">The hash algorithm object maker.</param>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifyHash<T>(Func<T> h, string plainText, string hash) where T : HashAlgorithm
        {
            // Return the result after StringComparer comparing.
            return h != null ?
                StringComparer.OrdinalIgnoreCase.Equals(GetHash(h, plainText), hash?.ToUpper()) :
                StringComparer.OrdinalIgnoreCase.Equals(plainText, hash);
        }

        /// <summary>
        /// Verifies a hash against a string.
        /// </summary>
        /// <param name="alg">One of algorithms about hash.</param>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifyHash(Func<string, string> alg, string plainText, string hash)
        {
            // Return the result after StringComparer comparing.
            return alg != null ?
                StringComparer.OrdinalIgnoreCase.Equals(alg(plainText), hash?.ToUpper()) :
                StringComparer.OrdinalIgnoreCase.Equals(plainText, hash);
        }

        /// <summary>
        /// Verifies a MD5 hash against a string.
        /// </summary>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifyMd5Hash(string plainText, string hash)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(GetMd5Hash(plainText), hash?.ToUpper());
        }

        /// <summary>
        /// Verifies a SHA512 hash against a string.
        /// </summary>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifySha512Hash(string plainText, string hash)
        {
            // Return the result after StringComparer comparing.
            return 0 == StringComparer.OrdinalIgnoreCase.Compare(GetMd5Hash(plainText), hash?.ToUpper());
        }

        /// <summary>
        /// Gets the SHA512 hash string value of the string instance.
        /// </summary>
        /// <param name="alg">One of algorithms about hash.</param>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string GetHashString<T>(this T alg, string plainText) where T : HashAlgorithm
        {
            // Get hash string.
            return GetHash(alg, plainText);
        }

        /// <summary>
        /// Verifies a specific hash against a string.
        /// </summary>
        /// <param name="alg">One of algorithms about hash.</param>
        /// <param name="plainText">The original input value to test.</param>
        /// <param name="hash">A hash string for comparing.</param>
        /// <returns>true if hash is a hash value of input; otherwise, false.</returns>
        public static bool VerifyHashString<T>(this T alg, string plainText, string hash) where T : HashAlgorithm
        {
            // Return the result after StringComparer comparing.
            return alg != null ?
                StringComparer.OrdinalIgnoreCase.Equals(GetHash(alg, plainText), hash?.ToUpper()) :
                StringComparer.OrdinalIgnoreCase.Equals(plainText, hash);
        }

        /// <summary>
        /// Gets the SHA512 hash string value of the string instance.
        /// </summary>
        /// <param name="plainText">The original input value to get hash.</param>
        /// <returns>A hash string value of the given string.</returns>
        public static string Sha512HashString(this string plainText)
        {
            // Get hash string.
            return GetHash(SHA512.Create, plainText);
        }
    }
}
