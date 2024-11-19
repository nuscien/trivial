using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// Letter cases.
/// </summary>
public enum Cases : byte
{
    /// <summary>
    /// Keep original.
    /// </summary>
    Original = 0,

    /// <summary>
    /// Uppercase.
    /// </summary>
    Upper = 1,

    /// <summary>
    /// Lowercase.
    /// </summary>
    Lower = 2,

    /// <summary>
    /// First letter uppercase and rest keeping original.
    /// </summary>
    Capitalize = 3,

    /// <summary>
    /// First letter lowercase and rest keeping original.
    /// </summary>
    Uncapitalize = 4
}
