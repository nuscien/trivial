using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

using Trivial.Web;

namespace Trivial.Security
{
    /// <summary>
    /// Json web token model.
    /// </summary>
    /// <typeparam name="T">The type of payload.</typeparam>
    public class JsonWebToken<T>
    {
        /// <summary>
        /// Json web token header model.
        /// </summary>
        [DataContract]
        private class HeaderModel
        {
            /// <summary>
            /// Gets or sets the name of signature algorithm.
            /// </summary>
            [DataMember(Name = "alg")]
            public string AlgorithmName { get; set; }

            /// <summary>
            /// Gets the type.
            /// </summary>
            [DataMember(Name = "typ")]
            public string Type { get; set; } = "JWT";
        }

        /// <summary>
        /// Signature algorithm.
        /// </summary>
        /// <param name="payload">The payload string.</param>
        /// <param name="secret">The secret.</param>
        /// <returns>The signature.</returns>
        public delegate byte[] SignatureAlgorithm(string payload, string secret);

        /// <summary>
        /// Creates a JSON web token with HMAC SHA-512 hash algorithm.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="secret">The secret.</param>
        /// <returns>A JSON web token object.</returns>
        public static JsonWebToken<T> CreateHS512(T payload, string secret)
        {
            return new JsonWebToken<T>(payload, new HMACSHA512(Encoding.ASCII.GetBytes(secret ?? string.Empty)), "HS512");
        }

        /// <summary>
        /// Parses a JSON web token with HMAC SHA-512 hash algorithm.
        /// </summary>
        /// <param name="jwt">The string encoded.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="verify">true if verify the signature; otherwise, false.</param>
        /// <returns>A JSON web token object.</returns>
        public static JsonWebToken<T> CreateHS512(string jwt, string secret, bool verify = true)
        {
            return Parse(jwt, new HMACSHA512(Encoding.ASCII.GetBytes(secret ?? string.Empty)), verify);
        }

        /// <summary>
        /// Creates a JSON web token with HMAC SHA-384 hash algorithm.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="secret">The secret.</param>
        /// <returns>A JSON web token object.</returns>
        public static JsonWebToken<T> CreateHS384(T payload, string secret)
        {
            return new JsonWebToken<T>(payload, new HMACSHA384(Encoding.ASCII.GetBytes(secret ?? string.Empty)), "HS384");
        }

        /// <summary>
        /// Parses a JSON web token with HMAC SHA-384 hash algorithm.
        /// </summary>
        /// <param name="jwt">The string encoded.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="verify">true if verify the signature; otherwise, false.</param>
        /// <returns>A JSON web token object.</returns>
        public static JsonWebToken<T> CreateHS384(string jwt, string secret, bool verify = true)
        {
            return Parse(jwt, new HMACSHA384(Encoding.ASCII.GetBytes(secret ?? string.Empty)), verify);
        }

        /// <summary>
        /// Creates a JSON web token with HMAC SHA-256 hash algorithm.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="secret">The secret.</param>
        /// <returns>A JSON web token object.</returns>
        public static JsonWebToken<T> CreateHS256(T payload, string secret)
        {
            return new JsonWebToken<T>(payload, new HMACSHA256(Encoding.ASCII.GetBytes(secret ?? string.Empty)), "HS256");
        }

        /// <summary>
        /// Parses a JSON web token with HMAC SHA-256 hash algorithm.
        /// </summary>
        /// <param name="jwt">The string encoded.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="verify">true if verify the signature; otherwise, false.</param>
        /// <returns>A JSON web token object.</returns>
        public static JsonWebToken<T> CreateHS256(string jwt, string secret, bool verify = true)
        {
            return Parse(jwt, new HMACSHA256(Encoding.ASCII.GetBytes(secret ?? string.Empty)), verify);
        }

        /// <summary>
        /// Parses a JWT string encoded.
        /// </summary>
        /// <param name="jwt">The string encoded.</param>
        /// <param name="algorithm">The signature algorithm.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="verify">true if verify the signature; otherwise, false.</param>
        /// <returns>A JSON web token object.</returns>
        /// <exception cref="ArgumentNullException">jwt was null or empty.</exception>
        /// <exception cref="ArgumentException">jwt did not contain any information.</exception>
        /// <exception cref="FormatException">jwt was in incorrect format.</exception>
        /// <exception cref="InvalidOperationException">Verify failure.</exception>
        public static JsonWebToken<T> Parse(string jwt, SignatureAlgorithm algorithm, string secret, bool verify = true)
        {
            var (payload, algorithmName, sign) = ParseInternal(jwt);
            var obj = new JsonWebToken<T>(payload, algorithm, algorithmName, secret);
            if (verify && obj.ToSigntureBase64Url() != sign) throw new InvalidOperationException("jwt signature is incorrect.");
            return obj;
        }

        /// <summary>
        /// Parses a JWT string encoded.
        /// </summary>
        /// <param name="jwt">The string encoded.</param>
        /// <param name="algorithm">The signature algorithm.</param>
        /// <param name="verify">true if verify the signature; otherwise, false.</param>
        /// <returns>A JSON web token object.</returns>
        /// <exception cref="ArgumentNullException">jwt was null or empty.</exception>
        /// <exception cref="ArgumentException">jwt did not contain any information.</exception>
        /// <exception cref="FormatException">jwt was in incorrect format.</exception>
        /// <exception cref="InvalidOperationException">Verify failure.</exception>
        public static JsonWebToken<T> Parse(string jwt, KeyedHashAlgorithm algorithm, bool verify = true)
        {
            var (payload, algorithmName, sign) = ParseInternal(jwt);
            var obj = new JsonWebToken<T>(payload, algorithm, algorithmName);
            if (verify && obj.ToSigntureBase64Url() != sign) throw new InvalidOperationException("jwt signature is incorrect.");
            return obj;
        }

        /// <summary>
        /// Parses a JWT string encoded.
        /// </summary>
        /// <param name="jwt">The string encoded.</param>
        /// <returns>A JSON web token tuple.</returns>
        /// <exception cref="ArgumentNullException">jwt was null or empty.</exception>
        /// <exception cref="ArgumentException">jwt did not contain any information.</exception>
        /// <exception cref="FormatException">jwt was in incorrect format.</exception>
        /// <exception cref="InvalidOperationException">Verify failure.</exception>
        private static (T, string, string) ParseInternal(string jwt)
        {
            if (string.IsNullOrWhiteSpace(jwt)) throw new ArgumentNullException(nameof(jwt), "jwt should not be null or empty.");
            var prefix = $"{TokenInfo.BearerTokenType} ";
            if (jwt.IndexOf(prefix) == 0)
            {
                if (jwt.Length == prefix.Length) throw new ArgumentException(nameof(jwt), "jwt should not contain a scheme only.");
                jwt = jwt.Substring(prefix.Length);
            }

            var arr = jwt.Split('.');
            if (arr.Length < 3) throw new FormatException("jwt is not in the correct format.");
            var header = WebUtility.Base64UrlDecodeTo<HeaderModel>(arr[0]);
            var payload = WebUtility.Base64UrlDecodeTo<T>(arr[1]);
            if (header == null) throw new ArgumentException(nameof(jwt), "jwt should contain header in Base64Url.");
            if (payload == null) throw new ArgumentException(nameof(jwt), "jwt should contain payload in Base64Url.");
            return (payload, header.AlgorithmName, arr[2]);
        }

        /// <summary>
        /// The signature algorithm function.
        /// </summary>
        private readonly Func<string, byte[]> signature;

        /// <summary>
        /// The Jwt header model.
        /// </summary>
        private readonly HeaderModel header;

        /// <summary>
        /// The header Base64Url encoded string.
        /// </summary>
        private string headerBase64Url;

        /// <summary>
        /// Initializes a new instance of the JwtModel class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="algorithm">The signature algorithm instance.</param>
        /// <param name="algorithmName">The signature algorithm name.</param>
        public JsonWebToken(T payload, KeyedHashAlgorithm algorithm, string algorithmName)
        {
            Payload = payload;
            if (algorithm == null)
            {
                if (algorithmName != null)
                {
                    switch (algorithmName.ToUpperInvariant())
                    {
                        case "HS256":
                            algorithm = new HMACSHA256(new byte[0]);
                            break;
                        case "HS384":
                            algorithm = new HMACSHA384(new byte[0]);
                            break;
                        case "HS512":
                            algorithm = new HMACSHA512(new byte[0]);
                            break;
                    }
                }

                if (algorithm == null) algorithm = new HMACSHA512(new byte[0]);
                if (string.IsNullOrWhiteSpace(algorithmName)) algorithmName = "HS512";
            }

            signature = value => algorithm.ComputeHash(Encoding.ASCII.GetBytes(value));
            header = new HeaderModel
            {
                AlgorithmName = algorithmName
            };
        }

        /// <summary>
        /// Initializes a new instance of the JwtModel class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="algorithm">The signature algorithm instance.</param>
        /// <param name="algorithmName">The signature algorithm name.</param>
        /// <param name="secret">The secret.</param>
        public JsonWebToken(T payload, SignatureAlgorithm algorithm, string algorithmName, string secret)
        {
            Payload = payload;
            signature = value => algorithm(value, secret);
            header = new HeaderModel
            {
                AlgorithmName = algorithmName
            };
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        public string AlgorithmName => header.AlgorithmName;

        /// <summary>
        /// Gets the payload.
        /// </summary>
        public T Payload { get; }

        /// <summary>
        /// Gets the Base64Url of payload.
        /// </summary>
        /// <returns>A string encoded of payload.</returns>
        public string ToPayloadBase64Url()
        {
            return WebUtility.Base64UrlEncode(Payload);
        }

        /// <summary>
        /// Gets the Base64Url of header.
        /// </summary>
        /// <returns>A string encoded of header.</returns>
        public string ToHeaderBase64Url()
        {
            if (headerBase64Url == null) headerBase64Url = WebUtility.Base64UrlEncode(header);
            return headerBase64Url;
        }

        /// <summary>
        /// Gets the Base64Url of signature.
        /// </summary>
        /// <returns>A string encoded of signature.</returns>
        public string ToSigntureBase64Url()
        {
            if (signature == null) return string.Empty;
            var bytes = signature($"{ToHeaderBase64Url()}.{ToPayloadBase64Url()}");
            return WebUtility.Base64UrlEncode(bytes);
        }

        /// <summary>
        /// Gets the encoded string.
        /// </summary>
        /// <returns>A string encoded.</returns>
        public string ToEncodedString()
        {
            return $"{ToHeaderBase64Url()}.{ToPayloadBase64Url()}.{ToSigntureBase64Url()}";
        }

        /// <summary>
        /// Returns a System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.
        /// </summary>
        /// <returns>A System.Net.Http.Headers.AuthenticationHeaderValue that represents the current TokenInfo.</returns>
        public string ToAuthenticationHeaderValue()
        {
            return $"{TokenInfo.BearerTokenType} {ToEncodedString()}";
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (Payload == null) return string.Empty;
            return Payload.ToString();
        }
    }
}
