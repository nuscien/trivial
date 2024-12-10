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
    public static double[] Difference(ReadOnlySpan<double> col)
    {
        var result = new double[col.Length - 1];
        for (var i = 1; i < col.Length; i++)
        {
            var j = i - 1;
            result[j] = col[i] - col[j];
        }

        return result;
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
    public static float[] Difference(ReadOnlySpan<float> col)
    {
        var result = new float[col.Length - 1];
        for (var i = 1; i < col.Length; i++)
        {
            var j = i - 1;
            result[j] = col[i] - col[j];
        }

        return result;
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
    public static decimal[] Difference(ReadOnlySpan<decimal> col)
    {
        var result = new decimal[col.Length - 1];
        for (var i = 1; i < col.Length; i++)
        {
            var j = i - 1;
            result[j] = col[i] - col[j];
        }

        return result;
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
    public static long[] Difference(ReadOnlySpan<long> col)
    {
        var result = new long[col.Length - 1];
        for (var i = 1; i < col.Length; i++)
        {
            var j = i - 1;
            result[j] = col[i] - col[j];
        }

        return result;
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
    public static int[] Difference(ReadOnlySpan<int> col)
    {
        var result = new int[col.Length - 1];
        for (var i = 1; i < col.Length; i++)
        {
            var j = i - 1;
            result[j] = col[i] - col[j];
        }

        return result;
    }

    /// <summary>
    /// Gets a sequence about each item is different than before one of a specific numbers.
    /// </summary>
    /// <param name="col">The input numbers.</param>
    /// <returns>The increasement of each item.</returns>
    public static IEnumerable<short> Difference(IEnumerable<short> col)
    {
        if (col == null) yield break;
        short? test = null;
        foreach (var item in col)
        {
            if (test.HasValue) yield return (short)(item - test.Value);
            test = item;
        }
    }

    /// <summary>
    /// Gets a sequence about each item is different than before one of a specific numbers.
    /// </summary>
    /// <param name="col">The input numbers.</param>
    /// <returns>The increasement of each item.</returns>
    public static short[] Difference(ReadOnlySpan<short> col)
    {
        var result = new short[col.Length - 1];
        for (var i = 1; i < col.Length; i++)
        {
            var j = i - 1;
            result[j] = (short)(col[i] - col[j]);
        }

        return result;
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
    public static TimeSpan[] Difference(ReadOnlySpan<DateTime> col)
    {
        var result = new TimeSpan[col.Length - 1];
        for (var i = 1; i < col.Length; i++)
        {
            var j = i - 1;
            result[j] = col[i] - col[j];
        }

        return result;
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

    /// <summary>
    /// Gets a sequence about each item is different than before one of a specific numbers.
    /// </summary>
    /// <param name="col">The input numbers.</param>
    /// <returns>The increasement of each item.</returns>
    public static TimeSpan[] Difference(ReadOnlySpan<TimeSpan> col)
    {
        var result = new TimeSpan[col.Length - 1];
        for (var i = 1; i < col.Length; i++)
        {
            var j = i - 1;
            result[j] = col[i] - col[j];
        }

        return result;
    }
}
