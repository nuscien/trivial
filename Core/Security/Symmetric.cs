// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Symmetric.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The helper and extension of symmetric functions.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Trivial.Security
{
    /// <summary>
    /// The helper of symmetric functions.
    /// </summary>
    public static class SymmetricExtension
    {
        /// <summary>
        /// Gets a encrypted string value of a specific string instance by symmetric algorithm.
        /// </summary>
        /// <param name="h">The hash algorithm object maker.</param>
        /// <param name="plainText">The original input value to encrypt.</param>
        /// <param name="key">The secret key for the symmetric algorithm.</param>
        /// <param name="iv">The initialization vector (System.Security.Cryptography.SymmetricAlgorithm.IV) for the symmetric algorithm. If iv is null, the value will be filled by the one of key,</param>
        /// <returns>A encrypted string value as Base64 format of the given string; or null, if h or input is null.</returns>
        public static string Encrypt<T>(this Func<T> h, string plainText, byte[] key, byte[] iv = null) where T : SymmetricAlgorithm
        {
            // Check if the arguments is not null.
            if (h == null || string.IsNullOrEmpty(plainText) || key == null || key.Length <= 0 || iv == null || iv.Length <= 0) return null;

            // Initialize encrypted data.
            byte[] data;

            // Create an Aes object with the specified key and IV.
            using (var alg = h())
            {
                if (alg == null) return null;
                alg.Key = key;
                alg.IV = iv ?? key;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = alg.CreateEncryptor(alg.Key, alg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }

                        // Convert the encrypted bytes from the memory stream.
                        data = msEncrypt.ToArray();
                    }
                }
            }

            // Return the Base64 string.
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// Gets a encrypted string value of a specific string instance by symmetric algorithm.
        /// </summary>
        /// <param name="h">The symmetric algorithm object maker.</param>
        /// <param name="plainText">The original input value to encrypt.</param>
        /// <param name="key">The secret key for the symmetric algorithm.</param>
        /// <param name="iv">The initialization vector (System.Security.Cryptography.SymmetricAlgorithm.IV) for the symmetric algorithm. If iv is null, the value will be filled by the one of key,</param>
        /// <returns>A encrypted string value as Base64 format of the given string; or null, if h or input is null.</returns>
        public static string Encrypt<T>(this Func<T> h, string plainText, string key, string iv = null) where T : SymmetricAlgorithm
        {
            // Check if the arguments is not null.
            if (h == null || string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(key)) return null;

            // Convert key and iv to byte array.
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(iv) ? key : iv);

            // Return encrypted string as Base64 format.
            return Encrypt(h, plainText, keyBytes, ivBytes);
        }

        /// <summary>
        /// Gets a decrypted string value from a specific cipher text by symmetric algorithm.
        /// </summary>
        /// <param name="h">The symmetric algorithm object maker.</param>
        /// <param name="cipherText">The input value to decrypt as Base64 format.</param>
        /// <param name="key">The secret key for the symmetric algorithm.</param>
        /// <param name="iv">The initialization vector (System.Security.Cryptography.SymmetricAlgorithm.IV) for the symmetric algorithm. If iv is null, the value will be filled by the one of key,</param>
        /// <returns>A decrypted string value of the given string; or null, if h or input is null.</returns>
        public static string Decrypt<T>(this Func<T> h, string cipherText, byte[] key, byte[] iv) where T : SymmetricAlgorithm
        {
            // Check if the arguments is not null.
            if (h == null || string.IsNullOrEmpty(cipherText) || key == null || key.Length <= 0 || iv == null || iv.Length <= 0) return null;

            // Convert cipher bytes from Base64 string.
            var cipherBytes = Convert.FromBase64String(cipherText);

            // Declare the string used to hold the decrypted text.
            string plaintext = null;

            // Create an Aes object with the specified key and IV.
            using (var alg = h())
            {
                if (alg == null) return null;
                alg.Key = key;
                alg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = alg.CreateDecryptor(alg.Key, alg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        /// <summary>
        /// Gets a decrypted string value from a specific cipher text by symmetric algorithm.
        /// </summary>
        /// <param name="h">The symmetric algorithm object maker.</param>
        /// <param name="cipherText">The input value to decrypt as Base64 format.</param>
        /// <param name="key">The secret key for the symmetric algorithm.</param>
        /// <param name="iv">The initialization vector (System.Security.Cryptography.SymmetricAlgorithm.IV) for the symmetric algorithm. If iv is null, the value will be filled by the one of key,</param>
        /// <returns>A decrypted string value of the given string; or null, if h or input is null.</returns>
        public static string Decrypt<T>(this Func<T> h, string cipherText, string key, string iv) where T : SymmetricAlgorithm
        {
            // Check if the arguments is not null.
            if (h == null || string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(key)) return null;

            // Convert key and iv to byte array.
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(iv) ? key : iv);

            // Return original plain text string.
            return Decrypt(h, cipherText, keyBytes, ivBytes);
        }
    }
}
