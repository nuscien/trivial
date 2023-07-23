using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Collection;

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
    /// Only for the ones above zero (0).
    /// </summary>
    AboveZero = 4,

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
    public static IEnumerable<float> ToSingleFloatingPointCollection(this IEnumerable<double> source)
    {
        if (source == null) yield break;
        foreach (var item in source)
        {
            yield return (float)item;
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
    public static float[] ToSingleFloatingPointArray(this double[] source)
    {
        if (source == null) return null;
        var arr = new float[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            arr[i] = (float)source[i];
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

    /// <summary>
    /// Computes ratios by given numbers.
    /// </summary>
    /// <param name="source">The source numbers.</param>
    /// <param name="options">The convertion options.</param>
    /// <returns>An array of all ratios.</returns>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static List<double> Ratios(IEnumerable<int> source, RatioConvertionOptions options = RatioConvertionOptions.Scale)
    {
        if (source == null) return null;
        var col = new List<int>(source);
        if (col.Count < 1) return new();
        switch (options)
        {
            case RatioConvertionOptions.Scale:
                return Ratios(col, ele => true);
            case RatioConvertionOptions.SoftMax:
                return StatisticalMethod.Softmax(col);
            case RatioConvertionOptions.HardMax:
                return StatisticalMethod.Hardmax(col, true);
            case RatioConvertionOptions.AboveAverage:
                {
                    var average = col.Average();
                    return Ratios(col, ele => ele > average);
                }
            case RatioConvertionOptions.AboveZero:
                return Ratios(col, ele => ele > 0);
            case RatioConvertionOptions.EqualDivision:
                return ListExtensions.Create(col.Count, 1d / col.Count).ToList();
            default:
                throw new NotSupportedException("The options was not supported.", new ArgumentException("options is invalid.", nameof(options)));
        }
    }

    /// <summary>
    /// Computes ratios by given numbers.
    /// </summary>
    /// <param name="source">The source numbers.</param>
    /// <param name="converter">The converter to get number value from source.</param>
    /// <param name="selector">The selector to generate result.</param>
    /// <param name="options">The convertion options.</param>
    /// <returns>An array of all ratios.</returns>
    /// <exception cref="ArgumentNullException">converter or selector is null.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static List<TResult> Ratios<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int> converter, Func<TSource, double, int, TResult> selector, RatioConvertionOptions options = RatioConvertionOptions.Scale)
        => Ratios(source, converter, Ratios, options, selector);

    /// <summary>
    /// Computes ratios by given numbers.
    /// </summary>
    /// <param name="source">The source numbers.</param>
    /// <param name="options">The convertion options.</param>
    /// <returns>An array of all ratios.</returns>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static double[] Ratios(int[] source, RatioConvertionOptions options = RatioConvertionOptions.Scale)
    {
        if (source == null) return null;
        if (source.Length < 1) return Array.Empty<double>();
        switch (options)
        {
            case RatioConvertionOptions.Scale:
                return Ratios(source, ele => true);
            case RatioConvertionOptions.SoftMax:
                return StatisticalMethod.Softmax(source);
            case RatioConvertionOptions.HardMax:
                return StatisticalMethod.Hardmax(source, true);
            case RatioConvertionOptions.AboveAverage:
                {
                    var average = source.Average();
                    return Ratios(source, ele => ele > average);
                }
            case RatioConvertionOptions.AboveZero:
                return Ratios(source, ele => ele > 0);
            case RatioConvertionOptions.EqualDivision:
                return ListExtensions.CreateArray(source.Length, 1d / source.Length);
            default:
                throw new NotSupportedException("The options was not supported.", new ArgumentException("options is invalid.", nameof(options)));
        }
    }

    /// <summary>
    /// Computes ratios by given numbers.
    /// </summary>
    /// <param name="source">The source numbers.</param>
    /// <param name="options">The convertion options.</param>
    /// <returns>An array of all ratios.</returns>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static List<double> Ratios(IEnumerable<double> source, RatioConvertionOptions options = RatioConvertionOptions.Scale)
    {
        if (source == null) return null;
        var col = new List<double>(source);
        if (col.Count < 1) return new();
        switch (options)
        {
            case RatioConvertionOptions.Scale:
                return Ratios(col, ele => true);
            case RatioConvertionOptions.SoftMax:
                return StatisticalMethod.Softmax(col);
            case RatioConvertionOptions.HardMax:
                return StatisticalMethod.Hardmax(col, true);
            case RatioConvertionOptions.AboveAverage:
                {
                    var average = col.Average();
                    return Ratios(col, ele => ele > average);
                }
            case RatioConvertionOptions.AboveZero:
                return Ratios(col, ele => ele > 0);
            case RatioConvertionOptions.EqualDivision:
                return ListExtensions.Create(col.Count, 1d / col.Count).ToList();
            default:
                throw new NotSupportedException("The options was not supported.", new ArgumentException("options is invalid.", nameof(options)));
        }
    }

    /// <summary>
    /// Computes ratios by given numbers.
    /// </summary>
    /// <param name="source">The source numbers.</param>
    /// <param name="converter">The converter to get number value from source.</param>
    /// <param name="selector">The selector to generate result.</param>
    /// <param name="options">The convertion options.</param>
    /// <returns>An array of all ratios.</returns>
    /// <exception cref="ArgumentNullException">converter or selector is null.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static List<TResult> Ratios<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, double> converter, Func<TSource, double, int, TResult> selector, RatioConvertionOptions options = RatioConvertionOptions.Scale)
        => Ratios(source, converter, Ratios, options, selector);

    /// <summary>
    /// Computes ratios by given numbers.
    /// </summary>
    /// <param name="source">The source numbers.</param>
    /// <param name="options">The convertion options.</param>
    /// <returns>An array of all ratios.</returns>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static double[] Ratios(double[] source, RatioConvertionOptions options = RatioConvertionOptions.Scale)
    {
        if (source == null) return null;
        if (source.Length < 1) return Array.Empty<double>();
        switch (options)
        {
            case RatioConvertionOptions.Scale:
                return Ratios(source, ele => true);
            case RatioConvertionOptions.SoftMax:
                return StatisticalMethod.Softmax(source);
            case RatioConvertionOptions.HardMax:
                return StatisticalMethod.Hardmax(source, true);
            case RatioConvertionOptions.AboveAverage:
                {
                    var average = source.Average();
                    return Ratios(source, ele => ele > average);
                }
            case RatioConvertionOptions.AboveZero:
                return Ratios(source, ele => ele > 0);
            case RatioConvertionOptions.EqualDivision:
                return ListExtensions.CreateArray(source.Length, 1d / source.Length);
            default:
                throw new NotSupportedException("The options was not supported.", new ArgumentException("options is invalid.", nameof(options)));
        }
    }

    /// <summary>
    /// Computes ratios by given numbers.
    /// </summary>
    /// <param name="source">The source numbers.</param>
    /// <param name="options">The convertion options.</param>
    /// <returns>An array of all ratios.</returns>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static List<float> Ratios(IEnumerable<float> source, RatioConvertionOptions options = RatioConvertionOptions.Scale)
    {
        if (source == null) return null;
        var col = new List<float>(source);
        if (col.Count < 1) return new();
        switch (options)
        {
            case RatioConvertionOptions.Scale:
                return Ratios(col, ele => true);
            case RatioConvertionOptions.SoftMax:
                return StatisticalMethod.Softmax(col);
            case RatioConvertionOptions.HardMax:
                return StatisticalMethod.Hardmax(col, true);
            case RatioConvertionOptions.AboveAverage:
                {
                    var average = col.Average();
                    return Ratios(col, ele => ele > average);
                }
            case RatioConvertionOptions.AboveZero:
                return Ratios(col, ele => ele > 0);
            case RatioConvertionOptions.EqualDivision:
                return ListExtensions.Create(col.Count, 1f / col.Count).ToList();
            default:
                throw new NotSupportedException("The options was not supported.", new ArgumentException("options is invalid.", nameof(options)));
        }
    }

    /// <summary>
    /// Computes ratios by given numbers.
    /// </summary>
    /// <param name="source">The source numbers.</param>
    /// <param name="options">The convertion options.</param>
    /// <returns>An array of all ratios.</returns>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static float[] Ratios(float[] source, RatioConvertionOptions options = RatioConvertionOptions.Scale)
    {
        if (source == null) return null;
        if (source.Length < 1) return Array.Empty<float>();
        switch (options)
        {
            case RatioConvertionOptions.Scale:
                return Ratios(source, ele => true);
            case RatioConvertionOptions.SoftMax:
                return StatisticalMethod.Softmax(source);
            case RatioConvertionOptions.HardMax:
                return StatisticalMethod.Hardmax(source, true);
            case RatioConvertionOptions.AboveAverage:
                {
                    var average = source.Average();
                    return Ratios(source, ele => ele > average);
                }
            case RatioConvertionOptions.AboveZero:
                return Ratios(source, ele => ele > 0);
            case RatioConvertionOptions.EqualDivision:
                return ListExtensions.CreateArray(source.Length, 1f / source.Length);
            default:
                throw new NotSupportedException("The options was not supported.", new ArgumentException("options is invalid.", nameof(options)));
        }
    }

    /// <summary>
    /// Computes ratios by given numbers.
    /// </summary>
    /// <param name="source">The source numbers.</param>
    /// <param name="converter">The converter to get number value from source.</param>
    /// <param name="options">The convertion options.</param>
    /// <param name="selector">The selector to generate result.</param>
    /// <returns>An array of all ratios.</returns>
    /// <exception cref="ArgumentNullException">converter or selector is null.</exception>
    /// <exception cref="NotSupportedException">The options is not supported.</exception>
    public static List<TResult> Ratios<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, float> converter, Func<TSource, float, int, TResult> selector, RatioConvertionOptions options = RatioConvertionOptions.Scale)
        => Ratios(source, converter, Ratios, options, selector);

    private static List<double> Ratios(IList<int> source, Func<int, bool> predicate)
    {
        var list = new List<double>();
        var min = source.Where(predicate).Min();
        var total = source.Where(predicate).Sum();
        if (min < 0)
        {
            total -= min * source.Count;
            if (total == 0) return list;
            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                list.Add(predicate(item) ? ((item * 1d - min) / total) : 0d);
            }
        }
        else
        {
            if (total == 0) return list;
            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                list.Add(predicate(item) ? (item * 1d / total) : 0d);
            }
        }

        return list;
    }

    private static double[] Ratios(int[] source, Func<int, bool> predicate)
    {
        var arr = new double[source.Length];
        var min = source.Where(predicate).Min();
        var total = source.Where(predicate).Sum();
        if (min < 0)
        {
            total -= min * source.Length;
            if (total == 0) return arr;
            for (var i = 0; i < source.Length; i++)
            {
                var item = source[i];
                arr[i] = predicate(item) ? ((item * 1d - min) / total) : 0d;
            }
        }
        else
        {
            if (total == 0) return arr;
            for (var i = 0; i < source.Length; i++)
            {
                var item = source[i];
                arr[i] = predicate(item) ? (item * 1d / total) : 0d;
            }
        }

        return arr;
    }

    private static List<double> Ratios(IList<double> source, Func<double, bool> predicate)
    {
        var list = new List<double>();
        var min = source.Where(predicate).Min();
        var total = source.Where(predicate).Sum();
        if (min < 0)
        {
            total -= min * source.Count;
            if (total == 0d) return list;
            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                list.Add(predicate(item) ? ((item - min) / total) : 0d);
            }
        }
        else
        {
            if (total == 0d) return list;
            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                list.Add(predicate(item) ? (item / total) : 0d);
            }
        }

        return list;
    }

    private static double[] Ratios(double[] source, Func<double, bool> predicate)
    {
        var arr = new double[source.Length];
        var min = source.Where(predicate).Min();
        var total = source.Where(predicate).Sum();
        if (min < 0)
        {
            total -= min * source.Length;
            if (total == 0d) return arr;
            for (var i = 0; i < source.Length; i++)
            {
                var item = source[i];
                arr[i] = predicate(item) ? ((item - min) / total) : 0d;
            }
        }
        else
        {
            if (total == 0d) return arr;
            for (var i = 0; i < source.Length; i++)
            {
                var item = source[i];
                arr[i] = predicate(item) ? (item / total) : 0d;
            }
        }

        return arr;
    }

    private static List<float> Ratios(IList<float> source, Func<float, bool> predicate)
    {
        var list = new List<float>();
        var min = source.Where(predicate).Min();
        var total = source.Where(predicate).Sum();
        if (min < 0)
        {
            total -= min * source.Count;
            if (total == 0f) return list;
            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                list.Add(predicate(item) ? ((item - min) / total) : 0f);
            }
        }
        else
        {
            if (total == 0f) return list;
            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                list.Add(predicate(item) ? (item / total) : 0f);
            }
        }

        return list;
    }

    private static float[] Ratios(float[] source, Func<float, bool> predicate)
    {
        var arr = new float[source.Length];
        var min = source.Where(predicate).Min();
        var total = source.Where(predicate).Sum();
        if (min < 0)
        {
            total -= min * source.Length;
            if (total == 0f) return arr;
            for (var i = 0; i < source.Length; i++)
            {
                var item = source[i];
                arr[i] = predicate(item) ? ((item - min) / total) : 0f;
            }
        }
        else
        {
            if (total == 0f) return arr;
            for (var i = 0; i < source.Length; i++)
            {
                var item = source[i];
                arr[i] = predicate(item) ? (item / total) : 0f;
            }
        }

        return arr;
    }

    private static List<TResult> Ratios<TSource, TNumber, TRatio, TResult>(IEnumerable<TSource> source, Func<TSource, TNumber> converter, Func<IEnumerable<TNumber>, RatioConvertionOptions, List<TRatio>> ratios, RatioConvertionOptions options, Func<TSource, TRatio, int, TResult> selector)
    {
        if (source == null) return null;
        if (converter == null) throw new ArgumentNullException(nameof(converter), "converter should not be null.");
        if (selector == null) throw new ArgumentNullException(nameof(selector), "selector should not be null.");
        var list = new List<TSource>(source);
        var col = list.Select(converter);
        var r = ratios(col, options);
        var result = new List<TResult>();
        for (var i = 0; i < list.Count; i++)
        {
            var ratio = i < r.Count ? r[i] : default;
            var value = selector(list[i], ratio, i);
            result.Add(value);
        }

        return result;
    }
}
