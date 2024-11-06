using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Trivial.Text;

/// <summary>
/// The interface of JSON switch context info bag.
/// </summary>
public interface IJsonSwitchContextInfo
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the creation date time.
    /// </summary>
    public DateTime CreationTime { get; }

    /// <summary>
    /// Gets the latest test time.
    /// </summary>
    public DateTime LatestTestTime { get; }

    /// <summary>
    /// Gets the count of cases run.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the args.
    /// </summary>
    public object Args { get; }

    /// <summary>
    /// Gets the JSON node source.
    /// </summary>
    public IJsonValueNode Source { get; }

    /// <summary>
    /// Gets or sets the additional tag.
    /// </summary>
    public object Tag { get; set; }

    /// <summary>
    /// Gets a value indicating whether the switch is passed all cases.
    /// </summary>
    public bool IsPassed { get; }
}

/// <summary>
/// The interface of JSON switch context info bag.
/// </summary>
public interface IJsonSwitchContextInfo<T> : IJsonSwitchContextInfo
{
    /// <summary>
    /// Gets or sets the args.
    /// </summary>
    public new T Args { get; set; }
}
