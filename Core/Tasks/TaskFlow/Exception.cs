using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// Represents a failure propagated from a previous task or node rather than from the current handler.
/// </summary>
/// <remarks>Task flows wrap predecessor failures with the predecessor as <see cref="SourceObject"/> and the
/// original exception as <see cref="Exception.InnerException"/>. Each subsequent failed step adds another wrapper;
/// cancellation of the underlying predecessor task is propagated without wrapping.</remarks>
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

#if !NETCOREAPP
    /// <summary>
    /// Initializes a new instance of the PreviousTaskException class.
    /// </summary>
    /// <param name="info">The serialization information instance that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The serialization streaming context that contains contextual information about the source or destination.</param>
    protected PreviousTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        if (info == null) return;
    }
#endif

    /// <summary>
    /// Gets the source of the preceding failure, or null if no source was specified.
    /// </summary>
    /// <remarks>For failures propagated by a task flow, this is the immediately preceding flow, not necessarily the root failing flow.</remarks>
    public object SourceObject { get; }
}
