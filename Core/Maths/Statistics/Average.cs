using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Trivial.Reflection;

namespace Trivial.Maths;

/// <summary>
/// The functions of probability theory and mathematical statistics.
/// </summary>
public static partial class StatisticalMethod
{
    /// <summary>
    /// Computes variance of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The variance; or double.NaN, if invalid input.</returns>
    public static double Variance(IEnumerable<int> col)
    {
        if (col == null) return double.NaN;
        if (col is int[] arr)
        {
            var count = arr.Length;
            if (count < 1) return double.NaN;
            var avg = arr.Average();
            return arr.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        if (col is not ICollection<int> list) list = col.ToList();
        {
            var count = list.Count;
            if (count < 1) return double.NaN;
            var avg = list.Average();
            return list.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }
    }

    /// <summary>
    /// Computes sample variance of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The sample variance; or double.NaN, if invalid input.</returns>
    public static double SampleVariance(IEnumerable<int> col)
    {
        if (col == null) return double.NaN;
        if (col is int[] arr)
        {
            var count = arr.Length - 1;
            if (count < 1) return double.NaN;
            var avg = arr.Average();
            return arr.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        if (col is not ICollection<int> list) list = col.ToList();
        {
            var count = list.Count - 1;
            if (count < 1) return double.NaN;
            var avg = list.Average();
            return list.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }
    }

    /// <summary>
    /// Gets standard deviation of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The standard deviation; or double.NaN, if invalid input.</returns>
    public static double StandardDeviation(IEnumerable<int> col)
    {
        var r = Variance(col);
        if (double.IsNaN(r)) return double.NaN;
        return Math.Sqrt(r);
    }

    /// <summary>
    /// Gets sample deviation of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The sample deviation; or double.NaN, if invalid input.</returns>
    public static double SampleDeviation(IEnumerable<int> col)
    {
        var r = SampleVariance(col);
        if (double.IsNaN(r)) return double.NaN;
        return Math.Sqrt(r);
    }

    /// <summary>
    /// Computes variance of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The variance; or double.NaN, if invalid input.</returns>
    public static double Variance(IEnumerable<long> col)
    {
        if (col == null) return double.NaN;
        if (col is long[] arr)
        {
            var count = arr.Length;
            if (count < 1) return double.NaN;
            var avg = arr.Average();
            return arr.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        if (col is not ICollection<long> list) list = col.ToList();
        {
            var count = list.Count;
            if (count < 1) return double.NaN;
            var avg = list.Average();
            return list.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }
    }

    /// <summary>
    /// Computes sample variance of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The sample variance; or double.NaN, if invalid input.</returns>
    public static double SampleVariance(IEnumerable<long> col)
    {
        if (col == null) return double.NaN;
        if (col is long[] arr)
        {
            var count = arr.Length - 1;
            if (count < 1) return double.NaN;
            var avg = arr.Average();
            return arr.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        if (col is not ICollection<long> list) list = col.ToList();
        {
            var count = list.Count - 1;
            if (count < 1) return double.NaN;
            var avg = list.Average();
            return list.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }
    }

    /// <summary>
    /// Gets standard deviation of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The standard deviation; or double.NaN, if invalid input.</returns>
    public static double StandardDeviation(IEnumerable<long> col)
    {
        var r = Variance(col);
        if (double.IsNaN(r)) return double.NaN;
        return Math.Sqrt(r);
    }

    /// <summary>
    /// Gets sample deviation of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The sample deviation; or double.NaN, if invalid input.</returns>
    public static double SampleDeviation(IEnumerable<long> col)
    {
        var r = SampleVariance(col);
        if (double.IsNaN(r)) return double.NaN;
        return Math.Sqrt(r);
    }

    /// <summary>
    /// Computes variance of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The variance; or double.NaN, if invalid input.</returns>
    public static double Variance(IEnumerable<float> col)
    {
        if (col == null) return double.NaN;
        if (col is float[] arr)
        {
            var count = arr.Length;
            if (count < 1) return double.NaN;
#if !NETFRAMEWORK
            if (Vector.IsHardwareAccelerated && Vector<float>.IsSupported && count >= 1000 && Vector<float>.Count >= 4)
            {
                var step = Vector<float>.Count;
                var vectorLength = count - count % step;
                var vectorAverage = arr.Average();
                var lowSquares = Vector<double>.Zero;
                var highSquares = Vector<double>.Zero;
                var i = 0;
                for (; i < vectorLength; i += step)
                {
                    var delta = new Vector<float>(arr, i) - new Vector<float>(vectorAverage);
                    Vector.Widen(delta, out var lowDelta, out var highDelta);
                    lowSquares += lowDelta * lowDelta;
                    highSquares += highDelta * highDelta;
                }

                var squareTotal = 0d;
                for (var j = 0; j < Vector<double>.Count; j++) squareTotal += lowSquares[j] + highSquares[j];
                for (; i < count; i++) squareTotal += Math.Pow(arr[i] - vectorAverage, 2);
                return squareTotal / count;
            }
#endif
            var avg = arr.Average();
            return arr.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        if (col is not ICollection<float> list) list = col.ToList();
        {
            var count = list.Count;
            if (count < 1) return double.NaN;
            var avg = list.Average();
            return list.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }
    }

    /// <summary>
    /// Computes sample variance of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The sample variance; or double.NaN, if invalid input.</returns>
    public static double SampleVariance(IEnumerable<float> col)
    {
        if (col == null) return double.NaN;
        if (col is float[] arr)
        {
            var count = arr.Length - 1;
            if (count < 1) return double.NaN;
#if !NETFRAMEWORK
            if (Vector.IsHardwareAccelerated && Vector<float>.IsSupported && arr.Length >= 1000 && Vector<float>.Count >= 4)
            {
                var step = Vector<float>.Count;
                var vectorLength = arr.Length - arr.Length % step;
                var vectorAverage = arr.Average();
                var lowSquares = Vector<double>.Zero;
                var highSquares = Vector<double>.Zero;
                var i = 0;
                for (; i < vectorLength; i += step)
                {
                    var delta = new Vector<float>(arr, i) - new Vector<float>(vectorAverage);
                    Vector.Widen(delta, out var lowDelta, out var highDelta);
                    lowSquares += lowDelta * lowDelta;
                    highSquares += highDelta * highDelta;
                }

                var squareTotal = 0d;
                for (var j = 0; j < Vector<double>.Count; j++) squareTotal += lowSquares[j] + highSquares[j];
                for (; i < arr.Length; i++) squareTotal += Math.Pow(arr[i] - vectorAverage, 2);
                return squareTotal / count;
            }
#endif
            var avg = arr.Average();
            return arr.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        if (col is not ICollection<float> list) list = col.ToList();
        {
            var count = list.Count - 1;
            if (count < 1) return double.NaN;
            var avg = list.Average();
            return list.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }
    }

    /// <summary>
    /// Gets standard deviation of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The standard deviation; or double.NaN, if invalid input.</returns>
    public static double StandardDeviation(IEnumerable<float> col)
    {
        var r = Variance(col);
        if (double.IsNaN(r)) return double.NaN;
        return Math.Sqrt(r);
    }

    /// <summary>
    /// Gets sample deviation of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The sample deviation; or double.NaN, if invalid input.</returns>
    public static double SampleDeviation(IEnumerable<float> col)
    {
        var r = SampleVariance(col);
        if (double.IsNaN(r)) return double.NaN;
        return Math.Sqrt(r);
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Computes variance of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The variance; or float.NaN, if invalid input.</returns>
    public static float VarianceF(IEnumerable<float> col)
    {
        if (col == null) return float.NaN;
        if (col is float[] arr)
        {
            var count = arr.Length;
            if (count < 1) return float.NaN;
#if !NETFRAMEWORK
            if (Vector.IsHardwareAccelerated && Vector<float>.IsSupported && count >= 1000 && Vector<float>.Count >= 4)
            {
                var step = Vector<float>.Count;
                var vectorLength = count - count % step;
                var vectorAverage = arr.Average();
                var average = new Vector<float>(vectorAverage);
                var sumSquares = Vector<float>.Zero;
                var i = 0;
                i = 0;
                for (; i < vectorLength; i += step)
                {
                    var delta = new Vector<float>(arr, i) - average;
                    sumSquares += delta * delta;
                }

                var squareTotal = 0f;
                for (var j = 0; j < step; j++) squareTotal += sumSquares[j];
                for (; i < count; i++) squareTotal += (arr[i] - vectorAverage) * (arr[i] - vectorAverage);
                return squareTotal / count;
            }
#endif
            var avg = arr.Average();
            return arr.Sum(ele => MathF.Pow(ele - avg, 2)) / count;
        }

        if (col is not ICollection<float> list) list = col.ToList();
        {
            var count = list.Count;
            if (count < 1) return float.NaN;
            var avg = list.Average();
            return list.Sum(ele => MathF.Pow(ele - avg, 2)) / count;
        }
    }

    /// <summary>
    /// Computes sample variance of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The sample variance; or float.NaN, if invalid input.</returns>
    public static float SampleVarianceF(IEnumerable<float> col)
    {
        if (col == null) return float.NaN;
        if (col is float[] arr)
        {
            var count = arr.Length - 1;
            if (count < 1) return float.NaN;
#if !NETFRAMEWORK
            if (Vector.IsHardwareAccelerated && Vector<float>.IsSupported && arr.Length >= 1000 && Vector<float>.Count >= 4)
            {
                var step = Vector<float>.Count;
                var vectorLength = arr.Length - arr.Length % step;
                var vectorAverage = arr.Average();
                var average = new Vector<float>(vectorAverage);
                var sumSquares = Vector<float>.Zero;
                var i = 0;
                i = 0;
                for (; i < vectorLength; i += step)
                {
                    var delta = new Vector<float>(arr, i) - average;
                    sumSquares += delta * delta;
                }

                var squareTotal = 0f;
                for (var j = 0; j < step; j++) squareTotal += sumSquares[j];
                for (; i < arr.Length; i++) squareTotal += (arr[i] - vectorAverage) * (arr[i] - vectorAverage);
                return squareTotal / count;
            }
#endif
            var avg = arr.Average();
            return arr.Sum(ele => MathF.Pow(ele - avg, 2)) / count;
        }

        if (col is not ICollection<float> list) list = col.ToList();
        {
            var count = list.Count - 1;
            if (count < 1) return float.NaN;
            var avg = list.Average();
            return list.Sum(ele => MathF.Pow(ele - avg, 2)) / count;
        }
    }

    /// <summary>
    /// Gets standard deviation of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The standard deviation; or float.NaN, if invalid input.</returns>
    public static double StandardDeviationF(IEnumerable<float> col)
    {
        var r = VarianceF(col);
        if (float.IsNaN(r)) return float.NaN;
        return MathF.Sqrt(r);
    }

    /// <summary>
    /// Gets sample deviation of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The sample deviation; or float.NaN, if invalid input.</returns>
    public static float SampleDeviationF(IEnumerable<float> col)
    {
        var r = SampleVarianceF(col);
        if (float.IsNaN(r)) return float.NaN;
        return MathF.Sqrt(r);
    }
#endif

    /// <summary>
    /// Computes variance of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The variance; or double.NaN, if invalid input.</returns>
    public static double Variance(IEnumerable<double> col)
    {
        if (col == null) return double.NaN;
        if (col is double[] arr)
        {
            var count = arr.Length;
            if (count < 1) return double.NaN;
#if !NETFRAMEWORK
            if (Vector.IsHardwareAccelerated && Vector<double>.IsSupported && count >= 1000 && Vector<double>.Count >= 4)
            {
                var step = Vector<double>.Count;
                var vectorLength = count - count % step;
                var sum = Vector<double>.Zero;
                var i = 0;
                for (; i < vectorLength; i += step)
                {
                    sum += new Vector<double>(arr, i);
                }

                var total = 0d;
                for (var j = 0; j < step; j++) total += sum[j];
                for (; i < count; i++) total += arr[i];
                var vectorAverage = total / count;
                var average = new Vector<double>(vectorAverage);
                var sumSquares = Vector<double>.Zero;
                i = 0;
                for (; i < vectorLength; i += step)
                {
                    var delta = new Vector<double>(arr, i) - average;
                    sumSquares += delta * delta;
                }

                var squareTotal = 0d;
                for (var j = 0; j < step; j++) squareTotal += sumSquares[j];
                for (; i < count; i++) squareTotal += Math.Pow(arr[i] - vectorAverage, 2);
                return squareTotal / count;
            }
#endif
            var avg = arr.Average();
            return arr.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        if (col is not ICollection<double> list) list = col.ToList();
        {
            var count = list.Count;
            if (count < 1) return double.NaN;
            var avg = list.Average();
            return list.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }
    }

    /// <summary>
    /// Computes sample variance of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The sample variance; or double.NaN, if invalid input.</returns>
    public static double SampleVariance(IEnumerable<double> col)
    {
        if (col == null) return double.NaN;
        if (col is double[] arr)
        {
            var count = arr.Length - 1;
            if (count < 1) return double.NaN;
#if !NETFRAMEWORK
            if (Vector.IsHardwareAccelerated && Vector<double>.IsSupported && arr.Length >= 1000 && Vector<double>.Count >= 4)
            {
                var step = Vector<double>.Count;
                var vectorLength = arr.Length - arr.Length % step;
                var sum = Vector<double>.Zero;
                var i = 0;
                for (; i < vectorLength; i += step)
                {
                    sum += new Vector<double>(arr, i);
                }

                var total = 0d;
                for (var j = 0; j < step; j++) total += sum[j];
                for (; i < arr.Length; i++) total += arr[i];
                var vectorAverage = total / arr.Length;
                var average = new Vector<double>(vectorAverage);
                var sumSquares = Vector<double>.Zero;
                i = 0;
                for (; i < vectorLength; i += step)
                {
                    var delta = new Vector<double>(arr, i) - average;
                    sumSquares += delta * delta;
                }

                var squareTotal = 0d;
                for (var j = 0; j < step; j++) squareTotal += sumSquares[j];
                for (; i < arr.Length; i++) squareTotal += Math.Pow(arr[i] - vectorAverage, 2);
                return squareTotal / count;
            }
#endif
            var avg = arr.Average();
            return arr.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        if (col is not ICollection<double> list) list = col.ToList();
        {
            var count = list.Count - 1;
            if (count < 1) return double.NaN;
            var avg = list.Average();
            return list.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }
    }

    /// <summary>
    /// Gets standard deviation of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The standard deviation; or double.NaN, if invalid input.</returns>
    public static double StandardDeviation(IEnumerable<double> col)
    {
        var r = Variance(col);
        if (double.IsNaN(r)) return double.NaN;
        return Math.Sqrt(r);
    }

    /// <summary>
    /// Gets sample deviation of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
    /// <returns>The sample deviation; or double.NaN, if invalid input.</returns>
    public static double SampleDeviation(IEnumerable<double> col)
    {
        var r = SampleVariance(col);
        if (double.IsNaN(r)) return double.NaN;
        return Math.Sqrt(r);
    }

    /// <summary>
    /// Computes the mean of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <returns>The mean of the sequence of number; or double.NaN, if input is empty.</returns>
    public static double Mean(IEnumerable<int> col)
    {
        try
        {
            if (col is not null) return col.Average();
        }
        catch (InvalidOperationException)
        {
        }

        return double.NaN;
    }

    /// <summary>
    /// Computes the mean of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <returns>The mean of the sequence of number; or double.NaN, if input is empty.</returns>
    public static double Mean(IEnumerable<long> col)
    {
        try
        {
            if (col is not null) return col.Average();
        }
        catch (InvalidOperationException)
        {
        }

        return double.NaN;
    }

    /// <summary>
    /// Computes the mean of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <returns>The mean of the sequence of number; or float.NaN, if input is empty.</returns>
    public static float Mean(IEnumerable<float> col)
    {
        try
        {
            if (col is not null) return col.Average();
        }
        catch (InvalidOperationException)
        {
        }

        return float.NaN;
    }

    /// <summary>
    /// Computes the mean of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <returns>The mean of the sequence of number; or double.NaN, if input is empty.</returns>
    public static double Mean(IEnumerable<double> col)
    {
        try
        {
            if (col is not null) return col.Average();
        }
        catch (InvalidOperationException)
        {
        }

        return double.NaN;
    }

    /// <summary>
    /// Computes the mean of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <returns>The mean of the sequence of number; or zero (0), if input is empty.</returns>
    public static decimal Mean(IEnumerable<decimal> col)
    {
        try
        {
            if (col is not null) return col.Average();
        }
        catch (InvalidOperationException)
        {
        }

        return decimal.Zero;
    }

    /// <summary>
    /// Computes the mode of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <returns>The mode of the sequence of number.</returns>
    public static IList<T> Mode<T>(IEnumerable<T> col)
    {
        if (col is null) return null;
        var dict = new Dictionary<T, int>();
        foreach (var item in col)
        {
            if (dict.ContainsKey(item)) dict[item]++;
            else dict[item] = 1;
        }

        var arr = new List<T>();
        if (dict.Count < 1) return arr;
        var count = dict.Max(ele => ele.Value);
        foreach (var item in dict)
        {
            if (item.Value == count) arr.Add(item.Key);
        }

        return arr;
    }

    /// <summary>
    /// Computes the median of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="defaultValue">The default value used when no element.</param>
    /// <param name="count">The count of the median numbers.</param>
    /// <returns>The median of the sequence of number; or the defaultValue, if no element, in this case count is zero (0).</returns>
    public static TSource Median<TSource, TKey>(IEnumerable<TSource> col, Func<TSource, TKey> keySelector, TSource defaultValue, out int count)
    {
        if (col is null)
        {
            count = 0;
            return defaultValue;
        }

        var list = col.Where(ele => ele is not null).OrderBy(keySelector).ToList();
        if (list.Count == 0)
        {
            count = 0;
            return defaultValue;
        }

        count = list.Count % 2 == 0 ? 2 : 1;
        return list[list.Count / 2];
    }

    /// <summary>
    /// Computes the median of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="count">The count of the median numbers.</param>
    /// <returns>The median of the sequence of number; or the default value, if no element, in this case count is zero (0).</returns>
    public static TSource Median<TSource, TKey>(IEnumerable<TSource> col, Func<TSource, TKey> keySelector, out int count)
        => Median(col, keySelector, default, out count);

    /// <summary>
    /// Computes the median of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>The median of the sequence of number.</returns>
    /// <exception cref="ArgumentNullException">col was null.</exception>
    public static TSource Median<TSource, TKey>(IEnumerable<TSource> col, Func<TSource, TKey> keySelector)
    {
        var v = Median(col, keySelector, default, out var count);
        if (count < 0) throw ObjectConvert.ArgumentNull(nameof(col));
        return v;
    }

    /// <summary>
    /// Computes the median of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <param name="count">The count of the median numbers.</param>
    /// <returns>The median of the sequence of number; or the default value, if no element, in this case count is zero (0).</returns>
    public static T Median<T>(IEnumerable<T> col, out int count)
        => Median(col, ele => ele, default, out count);

    /// <summary>
    /// Computes the median of a sequence of number.
    /// </summary>
    /// <param name="col">The input collection of number.</param>
    /// <returns>The median of the sequence of number.</returns>
    /// <exception cref="ArgumentNullException">col was null.</exception>
    public static T Median<T>(IEnumerable<T> col)
        => Median(col, ele => ele);
}
