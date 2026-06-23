using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Net;

/// <summary>
/// The generator of query data.
/// </summary>
public interface IQueryDataGenerator
{
    /// <summary>
    /// Converts to query from the current object.
    /// </summary>
    /// <returns>The query data.</returns>
    public QueryData ToQueryData();
}

/// <summary>
/// The base class to generate query data for URL.
/// </summary>
public abstract class BaseQueryRequestInfo : IQueryDataGenerator
{
    /// <summary>
    /// Occurs before filling data into query data instance.
    /// </summary>
    /// <param name="q">The query data instance.</param>
    protected virtual void OnQueryDataGenerating(QueryData q)
    {
    }

    /// <summary>
    /// Occurs after filling data into query data instance.
    /// </summary>
    /// <param name="q">The query data instance.</param>
    protected virtual void OnQueryDataGenerated(QueryData q)
    {
    }

    /// <summary>
    /// Converts to query from the current object.
    /// </summary>
    /// <param name="q">The query data instance used to fill data.</param>
    protected abstract void ToQueryData(QueryData q);

    /// <summary>
    /// Converts to query from the current object.
    /// </summary>
    /// <returns>The query data.</returns>
    public QueryData ToQueryData()
    {
        var q = new QueryData();
        OnQueryDataGenerating(q);
        ToQueryData(q);
        OnQueryDataGenerated(q);
        return q;
    }
}
