using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Trivial.Net
{
    /// <summary>
    /// The web exception for unexpected HTTP status.
    /// </summary>
    public class FailedHttpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the FailedHttpException class.
        /// </summary>
        /// <param name="response">The HTTP web response.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public FailedHttpException(HttpResponseMessage response, string message = null, Exception innerException = null) : base(message, innerException) => Response = response;

        /// <summary>
        /// Gets the HTTP response message.
        /// </summary>
        public HttpResponseMessage Response { get; }

        /// <summary>
        /// Gets the collection of HTTP response headers.
        /// </summary>
        public HttpResponseHeaders Header => Response?.Headers;

        /// <summary>
        /// Gets the reason phrase which typically is sent by servers together with the status code.
        /// </summary>
        public string ReasonPhrase => Response?.ReasonPhrase;

        /// <summary>
        /// Gets a value that indicates if the HTTP response was successful.
        /// </summary>
        public bool IsSuccessStatusCode => Response?.IsSuccessStatusCode ?? false;

        /// <summary>
        /// Gets the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode? StatusCode => Response?.StatusCode;

        /// <summary>
        /// Gets the content of a HTTP response message.
        /// </summary>
        public HttpContent Content => Response?.Content;

        /// <summary>
        /// Gets the request message which led to this response message.
        /// </summary>
        public HttpRequestMessage RequestMessage => Response?.RequestMessage;

        internal static FailedHttpException Create(HttpResponseMessage response, string message)
        {
            if (response == null || response.IsSuccessStatusCode) return null;
            Exception innerEx = null;
            var msg = $"Failed HTTP request with code ${(int)response.StatusCode}.";
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.ProxyAuthenticationRequired:
                case HttpStatusCode.Forbidden:
                    innerEx = new UnauthorizedAccessException(msg);
                    break;
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.UnsupportedMediaType:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.UpgradeRequired:
                case HttpStatusCode.HttpVersionNotSupported:
                    innerEx = new NotSupportedException(msg);
                    break;
                case HttpStatusCode.RequestTimeout:
                    innerEx = new TimeoutException("Request is timeout.");
                    break;
                case HttpStatusCode.GatewayTimeout:
                    innerEx = new TimeoutException("Gateway is timeout.");
                    break;
                case HttpStatusCode.NotImplemented:
                    innerEx = new NotImplementedException(msg);
                    break;
            }

            var ex = innerEx != null ? new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).", innerEx) : new HttpRequestException(response.ReasonPhrase);
            return new FailedHttpException(response, message, ex);
        }
    }
}
