using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The host for JSON object node.
/// </summary>
public interface IJsonObjectHost
{
    /// <summary>
    /// Converts to JSON object node.
    /// </summary>
    /// <returns>The JSON object node about current instance.</returns>
    JsonObjectNode ToJson();
}
