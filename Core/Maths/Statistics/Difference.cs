using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trivial.Maths;

/// <summary>
/// The functions of probability theory and mathematical statistics.
/// </summary>
public static partial class StatisticalMethod
{
    /// <summary>
    /// Gets a sequence about each item is different than before one of a specific numbers.
    /// </summary>
    /// <param name="col">The input numbers.</param>
    /// <returns>The increasement of each item.</returns>
    public static IEnumerable<double> Difference(IEnumerable<double> col)
    {
        if (col == null) yield break;
        double? test = null;
        foreach (var item in col)
        {
            if (test.HasValue) yield return item - test.Value;
            test = item;
        }
    }

    /// <summary>
    /// Gets a sequence about each item is different than before one of a specific numbers.
    /// </summary>
    /// <param name="col">The input numbers.</param>
    /// <returns>The increasement of each item.</returns>
    public static IEnumerable<float> Difference(IEnumerable<float> col)
    {
        if (col == null) yield break;
        float? test = null;
        foreach (var item in col)
        {
            if (test.HasValue) yield return item - test.Value;
            test = item;
        }
    }

    /// <summary>
    /// Gets a sequence about each item is different than before one of a specific numbers.
    /// </summary>
    /// <param name="col">The input numbers.</param>
    /// <returns>The increasement of each item.</returns>
    public static IEnumerable<decimal> Difference(IEnumerable<decimal> col)
    {
        if (col == null) yield break;
        decimal? test = null;
        foreach (var item in col)
        {
            if (test.HasValue) yield return item - test.Value;
            test = item;
        }
    }

    /// <summary>
    /// Gets a sequence about each item is different than before one of a specific numbers.
    /// </summary>
    /// <param name="col">The input numbers.</param>
    /// <returns>The increasement of each item.</returns>
    public static IEnumerable<long> Difference(IEnumerable<long> col)
    {
        if (col == null) yield break;
        long? test = null;
        foreach (var item in col)
        {
            if (test.HasValue) yield return item - test.Value;
            test = item;
        }
    }

    /// <summary>
    /// Gets a sequence about each item is different than before one of a specific numbers.
    /// </summary>
    /// <param name="col">The input numbers.</param>
    /// <returns>The increasement of each item.</returns>
    public static IEnumerable<int> Difference(IEnumerable<int> col)
    {
        if (col == null) yield break;
        int? test = null;
        foreach (var item in col)
        {
            if (test.HasValue) yield return item - test.Value;
            test = item;
        }
    }

    /// <summary>
    /// Gets a sequence about each item is different than before one of a specific numbers.
    /// </summary>
    /// <param name="col">The input numbers.</param>
    /// <returns>The increasement of each item.</returns>
    public static IEnumerable<TimeSpan> Difference(IEnumerable<DateTime> col)
    {
        if (col == null) yield break;
        DateTime? test = null;
        foreach (var item in col)
        {
            if (test.HasValue) yield return item - test.Value;
            test = item;
        }
    }

    /// <summary>
    /// Gets a sequence about each item is different than before one of a specific numbers.
    /// </summary>
    /// <param name="col">The input numbers.</param>
    /// <returns>The increasement of each item.</returns>
    public static IEnumerable<TimeSpan> Difference(IEnumerable<TimeSpan> col)
    {
        if (col == null) yield break;
        TimeSpan? test = null;
        foreach (var item in col)
        {
            if (test.HasValue) yield return item - test.Value;
            test = item;
        }
    }
}
