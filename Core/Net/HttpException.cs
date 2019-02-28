using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
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
    }
}
