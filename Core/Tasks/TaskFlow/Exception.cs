using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// The exception thrown by previous task or node.
/// </summary>
public class PreviousTaskException : Exception
{
    /// <summary>
    /// Initializes a new instance of the PreviousTaskException class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public PreviousTaskException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the PreviousTaskException class.
    /// </summary>
    /// <param name="source">The source object that throws the inner exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public PreviousTaskException(object source, string message, Exception innerException) : base(message, innerException)
    {
        SourceObject = source;
    }

    /// <summary>
    /// Initializes a new instance of the PreviousTaskException class.
    /// </summary>
    /// <param name="info">The serialization information instance that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The serialization streaming context that contains contextual information about the source or destination.</param>
    protected PreviousTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        if (info == null) return;
    }

    /// <summary>
    /// Gets the source object that throws the inner exception.
    /// </summary>
    public object SourceObject { get; }
}
