using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;

/// <summary>
/// The options for converting a set of number to ratio collection.
/// </summary>
public enum RatioConvertionOptions : byte
{
    /// <summary>
    /// Original.
    /// </summary>
    Scale = 0,

    /// <summary>
    /// Calculated by soft max.
    /// </summary>
    SoftMax = 1,

    /// <summary>
    /// Calculated by hard max. That means only the maximum ones only.
    /// </summary>
    HardMax = 2,

    /// <summary>
    /// Only for the ones above average.
    /// </summary>
    AboveAverage = 3,

    /// <summary>
    /// Equal division.
    /// </summary>
    EqualDivision = 7
}

public static partial class CollectionOperation
{
    /// <summary>
    /// Converts to double-precision floating-point number collection.
    /// </summary>
    /// <param name="source">The source sequence to convert.</param>
    /// <returns>A sequence with double-precision floating-point numbers.</returns>
    public static IEnumerable<double> ToDoubleFloatingPointCollection(this IEnumerable<int> source)
    {
        if (source == null) yield break;
        foreach (var item in source)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Converts to double-precision floating-point number collection.
    /// </summary>
    /// <param name="source">The source sequence to convert.</param>
    /// <returns>A sequence with double-precision floating-point numbers.</returns>
    public static IEnumerable<double> ToDoubleFloatingPointCollection(this IEnumerable<long> source)
    {
        if (source == null) yield break;
        foreach (var item in source)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Converts to double-precision floating-point number collection.
    /// </summary>
    /// <param name="source">The source sequence to convert.</param>
    /// <returns>A sequence with double-precision floating-point numbers.</returns>
    public static IEnumerable<double> ToDoubleFloatingPointCollection(this IEnumerable<float> source)
    {
        if (source == null) yield break;
        foreach (var item in source)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Converts to double-precision floating-point number collection.
    /// </summary>
    /// <param name="source">The source sequence to convert.</param>
    /// <returns>A sequence with double-precision floating-point numbers.</returns>
    public static IEnumerable<double> ToDoubleFloatingPointCollection(this IEnumerable<decimal> source)
    {
        if (source == null) yield break;
        foreach (var item in source)
        {
            yield return (double)item;
        }
    }

    /// <summary>
    /// Converts to double-precision floating-point number array.
    /// </summary>
    /// <param name="source">The source array to convert.</param>
    /// <returns>An array with double-precision floating-point numbers.</returns>
    public static double[] ToDoubleFloatingPointArray(this int[] source)
    {
        if (source == null) return null;
        var arr = new double[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            arr[i] = source[i];
        }

        return arr;
    }


    /// <summary>
    /// Converts to double-precision floating-point number array.
    /// </summary>
    /// <param name="source">The source array to convert.</param>
    /// <returns>An array with double-precision floating-point numbers.</returns>
    public static double[] ToDoubleFloatingPointArray(this long[] source)
    {
        if (source == null) return null;
        var arr = new double[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            arr[i] = source[i];
        }

        return arr;
    }

    /// <summary>
    /// Converts to double-precision floating-point number array.
    /// </summary>
    /// <param name="source">The source array to convert.</param>
    /// <returns>An array with double-precision floating-point numbers.</returns>
    public static double[] ToDoubleFloatingPointArray(this float[] source)
    {
        if (source == null) return null;
        var arr = new double[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            arr[i] = source[i];
        }

        return arr;
    }

    /// <summary>
    /// Converts to double-precision floating-point number array.
    /// </summary>
    /// <param name="source">The source array to convert.</param>
    /// <returns>An array with double-precision floating-point numbers.</returns>
    public static double[] ToDoubleFloatingPointArray(this decimal[] source)
    {
        if (source == null) return null;
        var arr = new double[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            arr[i] = (double)source[i];
        }

        return arr;
    }

    /// <summary>
    /// Converts to single-precision floating-point number collection.
    /// </summary>
    /// <param name="source">The source sequence to convert.</param>
    /// <returns>A sequence with single-precision floating-point numbers.</returns>
    public static IEnumerable<float> ToSingleFloatingPointCollection(this IEnumerable<int> source)
    {
        if (source == null) yield break;
        foreach (var item in source)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Converts to single-precision floating-point number collection.
    /// </summary>
    /// <param name="source">The source sequence to convert.</param>
    /// <returns>A sequence with single-precision floating-point numbers.</returns>
    public static IEnumerable<float> ToSingleFloatingPointCollection(this IEnumerable<long> source)
    {
        if (source == null) yield break;
        foreach (var item in source)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Converts to single-precision floating-point number collection.
    /// </summary>
    /// <param name="source">The source sequence to convert.</param>
    /// <returns>A sequence with single-precision floating-point numbers.</returns>
    public static IEnumerable<float> ToSingleFloatingPointCollection(this IEnumerable<decimal> source)
    {
        if (source == null) yield break;
        foreach (var item in source)
        {
            yield return (float)item;
        }
    }

    /// <summary>
    /// Converts to double-precision floating-point number array.
    /// </summary>
    /// <param name="source">The source array to convert.</param>
    /// <returns>An array with double-precision floating-point numbers.</returns>
    public static float[] ToSingleFloatingPointArray(this int[] source)
    {
        if (source == null) return null;
        var arr = new float[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            arr[i] = source[i];
        }

        return arr;
    }


    /// <summary>
    /// Converts to single-precision floating-point number array.
    /// </summary>
    /// <param name="source">The source array to convert.</param>
    /// <returns>An array with single-precision floating-point numbers.</returns>
    public static float[] ToSingleFloatingPointArray(this long[] source)
    {
        if (source == null) return null;
        var arr = new float[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            arr[i] = source[i];
        }

        return arr;
    }

    /// <summary>
    /// Converts to single-precision floating-point number array.
    /// </summary>
    /// <param name="source">The source array to convert.</param>
    /// <returns>An array with single-precision floating-point numbers.</returns>
    public static float[] ToSingleFloatingPointArray(this decimal[] source)
    {
        if (source == null) return null;
        var arr = new float[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            arr[i] = (float)source[i];
        }

        return arr;
    }

    //private static IEnumerable<double> Ratios<T>(IEnumerable<T> source, Func<IEnumerable<T>, T> count, Func<T, T, double> ratio, RatioConvertionOptions options = RatioConvertionOptions.Scale)
    //{

    //}
}
