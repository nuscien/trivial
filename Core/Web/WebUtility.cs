using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Trivial.Security;

namespace Trivial.Web
{
    /// <summary>
    /// Web utilities.
    /// </summary>
    public static class WebUtility
    {
        /// <summary>
        /// Gets the MIME value of JSON format text.
        /// </summary>
        public const string JsonMIME = "application/json";

        /// <summary>
        /// Gets the MIME value of JavaScript format text.
        /// </summary>
        public const string JavaScriptMIME = "text/javascript";

        /// <summary>
        /// Gets the MIME value of CSS format text.
        /// </summary>
        public const string CssMIME = "text/css";

        /// <summary>
        /// Parses JavaScript date tick to date and time.
        /// </summary>
        /// <param name="tick">The JavaScript date tick.</param>
        /// <returns>A date and time.</returns>
        public static DateTime ParseDate(long tick)
        {
            var time = new DateTime(tick * 10000 + 621355968000000000, DateTimeKind.Utc);
            return time.ToLocalTime();
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time.
        /// </summary>
        /// <param name="tick">The JavaScript date tick.</param>
        /// <returns>A date and time.</returns>
        public static DateTime? ParseDate(long? tick)
        {
            if (!tick.HasValue) return null;
            return ParseDate(tick.Value);
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        public static long ParseDate(DateTime date)
        {
            return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// Parses JavaScript date tick to date and time back.
        /// </summary>
        /// <param name="date">A date and time.</param>
        /// <returns>The JavaScript date tick.</returns>
        public static long? ParseDate(DateTime? date)
        {
            if (!date.HasValue) return null;
            return ParseDate(date.Value);
        }

        /// <summary>
        /// Encodes a specific byte array into Base64Url format.
        /// </summary>
        /// <param name="bytes">The value to encode.</param>
        /// <returns>A Base64Url string.</returns>
        public static string Base64UrlEncode(byte[] bytes)
        {
            if (bytes == null) return null;
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", string.Empty);
        }

        /// <summary>
        /// Encodes a specific string into Base64Url format.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <param name="encoding">Optional text encoding.</param>
        /// <returns>A Base64Url string.</returns>
        public static string Base64UrlEncode(string value, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var bytes = (encoding ?? Encoding.UTF8).GetBytes(value);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", string.Empty);
        }

        /// <summary>
        /// Encodes a specific object into JSON Base64Url format.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>A Base64Url string.</returns>
        public static string Base64UrlEncode<T>(T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                stream.Position = 0;
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return Base64UrlEncode(bytes);
            }
        }

        /// <summary>
        /// Decodes the string from a Base64Url format.
        /// </summary>
        /// <param name="base64UrlStringEncoded">A Base64Url encoded string.</param>
        /// <returns>A plain text.</returns>
        public static byte[] Base64UrlDecode(string base64UrlStringEncoded)
        {
            if (base64UrlStringEncoded == null) return null;
            if (base64UrlStringEncoded == string.Empty) return new byte[0];
            base64UrlStringEncoded = base64UrlStringEncoded.Replace("-", "+").Replace("_", "/");
            base64UrlStringEncoded = base64UrlStringEncoded.PadRight(4 - base64UrlStringEncoded.Length % 4, '=');
            var bytes = Convert.FromBase64String(base64UrlStringEncoded);
            return bytes;
        }

        /// <summary>
        /// Decodes the string from a Base64Url format.
        /// </summary>
        /// <param name="base64UrlStringEncoded">A Base64Url encoded string.</param>
        /// <param name="encoding">Optional text encoding.</param>
        /// <returns>A plain text.</returns>
        public static string Base64UrlDecodeAsString(string base64UrlStringEncoded, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(base64UrlStringEncoded)) return base64UrlStringEncoded;
            var bytes = Base64UrlDecode(base64UrlStringEncoded);
            return (encoding ?? Encoding.ASCII).GetString(bytes);
        }

        /// <summary>
        /// Decodes and deserializes the object from a JSON Base64Url format.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="base64UrlStringEncoded">A Base64Url encoded string.</param>
        /// <returns>The object typed.</returns>
        public static T Base64UrlDecodeAs<T>(string base64UrlStringEncoded)
        {
            if (string.IsNullOrEmpty(base64UrlStringEncoded)) return default;
            var bytes = Base64UrlDecode(base64UrlStringEncoded);
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream(bytes))
            {
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
