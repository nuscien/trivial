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
