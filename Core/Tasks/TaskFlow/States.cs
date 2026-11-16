using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks;

/// <summary>
/// The states of task flow.
/// </summary>
public enum TaskFlowStates : byte
{
    /// <summary>
    /// Unknown state.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The task is running.
    /// </summary>
    Running = 1,

    /// <summary>
    /// Completed successfully.
    /// </summary>
    Success = 2,

    /// <summary>
    /// Completed with a failure or cancellation, or no underlying task was supplied.
    /// </summary>
    Failure = 3
}
