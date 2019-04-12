using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
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
        public FailedHttpException(HttpResponseMessage response, string message = null, Exception innerException = null) : base(message, innerException)
        {
            if (response == null) return;
            Response = response;
            ReasonPhrase = response.ReasonPhrase;
            StatusCode = response.StatusCode;
            Header = response.Headers;
        }

        /// <summary>
        /// Initializes a new instance of the FailedHttpException class.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        protected FailedHttpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ReasonPhrase = info.GetString("ReasonPhrase");
            var statusCode = info.GetInt32("StatusCode");
            if (statusCode < 0) return;
            StatusCode = (HttpStatusCode)statusCode;
        }

        /// <summary>
        /// Gets the HTTP response message.
        /// </summary>
        public HttpResponseMessage Response { get; }

        /// <summary>
        /// Gets the collection of HTTP response headers.
        /// </summary>
        public HttpResponseHeaders Header { get; }

        /// <summary>
        /// Gets the reason phrase which typically is sent by servers together with the status code.
        /// </summary>
        public string ReasonPhrase { get; }

        /// <summary>
        /// Gets a value that indicates if the HTTP response was successful.
        /// </summary>
        public bool IsSuccessStatusCode => StatusCode.HasValue && (int)StatusCode.Value >= 200 && (int)StatusCode.Value < 300;

        /// <summary>
        /// Gets the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode? StatusCode { get; }

        /// <summary>
        /// Gets the content of a HTTP response message.
        /// </summary>
        public HttpContent Content => Response?.Content;

        /// <summary>
        /// Gets the request message which led to this response message.
        /// </summary>
        public HttpRequestMessage RequestMessage => Response?.RequestMessage;

        /// <summary>
        /// When overridden in a derived class, sets the System.Runtime.Serialization.SerializationInfo with information about the exception.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ReasonPhrase", ReasonPhrase, typeof(string));
            var statusCode = -1;
            if (StatusCode.HasValue) statusCode = (int)StatusCode.Value;
            info.AddValue("HttpStatusCode", statusCode, typeof(int));
        }

        internal static FailedHttpException Create(HttpResponseMessage response, string message)
        {
            if (response == null || response.IsSuccessStatusCode) return null;
            Exception innerEx = null;
            var msg = $"Failed HTTP request with status code {(int)response.StatusCode} {response.ReasonPhrase}.";
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
                case HttpStatusCode.RequestUriTooLong:
                    innerEx = new PathTooLongException("Request URI is too long.");
                    break;
                case HttpStatusCode.BadRequest:
                    innerEx = new InvalidOperationException(msg);
                    break;
            }

            var reqEx = $"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).";
            var ex = innerEx != null ? new HttpRequestException(reqEx, innerEx) : new HttpRequestException(reqEx);
            return new FailedHttpException(response, message, ex);
        }
    }
}
