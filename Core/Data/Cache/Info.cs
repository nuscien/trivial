using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Data;

/// <summary>
/// The cache item info.
/// </summary>
/// <typeparam name="T">The type of data model.</typeparam>
public class DataCacheItemInfo<T>
{
    /// <summary>
    /// Initializes a new instance of the DataCacheItemInfo class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="value">The value.</param>
    /// <param name="expiration">The optional expiration to override current policy.</param>
    public DataCacheItemInfo(string id, T value, TimeSpan? expiration = null)
    {
        Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
        Value = value;
        Expiration = expiration;
    }

    /// <summary>
    /// Initializes a new instance of the DataCacheItemInfo class.
    /// </summary>
    /// <param name="ns">The namespace of resource group; or null for no namespace ones.</param>
    /// <param name="id">The identifier in the resource group.</param>
    /// <param name="value">The value.</param>
    /// <param name="expiration">The optional expiration to override current policy.</param>
    public DataCacheItemInfo(string ns, string id, T value, TimeSpan? expiration = null) : this(id, value, expiration)
    {
        Namespace = string.IsNullOrWhiteSpace(ns) ? null : ns.Trim();
    }

    /// <summary>
    /// Initializes a new instance of the DataCacheItemInfo class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="value">The value.</param>
    /// <param name="update">The last update time.</param>
    /// <param name="expiration">The optional expiration to override current policy.</param>
    public DataCacheItemInfo(string id, T value, DateTime update, TimeSpan? expiration = null)
    {
        Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
        Value = value;
        Expiration = expiration;
        if (update > DateTime.Now) return;
        UpdateDate = update;
    }

    /// <summary>
    /// Initializes a new instance of the DataCacheItemInfo class.
    /// </summary>
    /// <param name="ns">The namespace of resource group; or null for no namespace ones.</param>
    /// <param name="id">The identifier in the resource group.</param>
    /// <param name="value">The value.</param>
    /// <param name="update">The last update time.</param>
    /// <param name="expiration">The optional expiration to override current policy.</param>
    public DataCacheItemInfo(string ns, string id, T value, DateTime update, TimeSpan? expiration = null) : this(id, value, update, expiration)
    {
        Namespace = string.IsNullOrWhiteSpace(ns) ? null : ns.Trim();
    }

    /// <summary>
    /// Gets or sets the namespace.
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets or sets the item creation date.
    /// </summary>
    public DateTime CreationDate { get; internal set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public DateTime UpdateDate { get; private set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Gets the optional expiration.
    /// </summary>
    public TimeSpan? Expiration { get; }

    /// <summary>
    /// Gets or sets the additional data.
    /// </summary>
    public object Tag { get; set; }

    /// <summary>
    /// Gets or sets the expired date.
    /// </summary>
    /// <param name="expiration">The expiration time span.</param>
    /// <returns>The expired date.</returns>
    public DateTime GetExpiredDate(TimeSpan expiration)
        => UpdateDate + (Expiration ?? expiration);

    /// <summary>
    /// Tests if the item is expired.
    /// </summary>
    /// <param name="expiration">The expiration time span.</param>
    /// <returns>true if expired; otherwise, false.</returns>
    public bool IsExpired(TimeSpan? expiration)
    {
        if (!expiration.HasValue && !Expiration.HasValue) return false;
        return DateTime.Now >= (UpdateDate + (Expiration ?? expiration).Value);
    }

    /// <summary>
    /// Forces update the update date time to now.
    /// </summary>
    public void ForceExtendToNow()
        => UpdateDate = DateTime.Now;
}

/// <summary>
/// The collection for data cache.
/// </summary>
/// <typeparam name="T">The type of data model.</typeparam>
internal class DataCacheFactoryInfo<T> : ConcurrentDictionary<string, SemaphoreSlim>
{
    /// <summary>
    /// Gets or sets the factory.
    /// </summary>
    public Func<string, Task<T>> Factory { get; set; }

    /// <summary>
    /// Gets or sets the optional time span that represents the number of milliseconds to wait.
    /// </summary>
    public TimeSpan? Timeout { get; set; }
}
