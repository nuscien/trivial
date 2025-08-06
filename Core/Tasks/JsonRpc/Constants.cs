using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Trivial.Tasks;

/// <summary>
/// The constants for JSON-RPC.
/// </summary>
public static class JsonRpcConstants
{
    /// <summary>
    /// The supported JSON-RPC version.
    /// </summary>
    internal const string Version = "2.0";

    /// <summary>
    /// Invalid JSON was received by the server. An error occurred on the server while parsing the JSON text.
    /// </summary>
    public const int ParseError = -32700;

    /// <summary>
    /// The JSON sent is not a valid request object.
    /// </summary>
    public const int InvalidRequest = -32600;

    /// <summary>
    /// The method does not exist or is not available.
    /// </summary>
    public const int MethodNotFound = -32601;

    /// <summary>
    /// Invalid method parameter(s).
    /// </summary>
    public const int InvalidParams = -32602;

    /// <summary>
    /// Internal JSON-RPC error.
    /// </summary>
    public const int InternalError = -32603;
}
