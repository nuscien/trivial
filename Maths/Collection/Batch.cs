using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;

public static partial class CollectionOperation
{
    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<int> Plus(IEnumerable<int> a, IEnumerable<int> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0 : colA[i];
            var numB = i >= colB.Count ? 0 : colB[i];
            yield return numA + numB;
        }
    }

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<int> Plus(IEnumerable<int> a, IEnumerable<int> b, IEnumerable<int> c, IEnumerable<int> d = null)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var colC = c?.ToList() ?? new();
        var colD = d?.ToList() ?? new();
        var count = Math.Max(Math.Max(colA.Count, colB.Count), Math.Max(colC.Count, colD.Count));
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0 : colA[i];
            var numB = i >= colB.Count ? 0 : colB[i];
            var numC = i >= colC.Count ? 0 : colC[i];
            var numD = i >= colD.Count ? 0 : colD[i];
            yield return numA + numB + numC + numD;
        }
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<int> Plus(IEnumerable<int> a, IEnumerable<int> b, int c)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0 : colA[i];
            var numB = i >= colB.Count ? 0 : colB[i];
            yield return numA + numB + c;
        }
    }

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<int> Plus(IEnumerable<int> a, int b)
    {
        foreach (var item in a)
        {
            yield return item + b;
        }
    }

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static int[] Plus(int[] a, int[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new int[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? 0 : a[i];
            var numB = i >= b.Length ? 0 : b[i];
            c[i] = numA + numB;
        }

        return c;
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<int> Minus(IEnumerable<int> a, IEnumerable<int> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0 : colA[i];
            var numB = i >= colB.Count ? 0 : colB[i];
            yield return numA - numB;
        }
    }

    /// <summary>
    /// Minuss a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<int> Minus(IEnumerable<int> a, int b)
    {
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items ,inused.</returns>
    public static int[] Minus(int[] a, int[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new int[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? 0 : a[i];
            var numB = i >= b.Length ? 0 : b[i];
            c[i] = numA - numB;
        }

        return c;
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<long> Plus(IEnumerable<long> a, IEnumerable<long> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0L : colA[i];
            var numB = i >= colB.Count ? 0L : colB[i];
            yield return numA + numB;
        }
    }

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<long> Plus(IEnumerable<long> a, IEnumerable<long> b, IEnumerable<long> c, IEnumerable<long> d = null)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var colC = c?.ToList() ?? new();
        var colD = d?.ToList() ?? new();
        var count = Math.Max(Math.Max(colA.Count, colB.Count), Math.Max(colC.Count, colD.Count));
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0L : colA[i];
            var numB = i >= colB.Count ? 0L : colB[i];
            var numC = i >= colC.Count ? 0L : colC[i];
            var numD = i >= colD.Count ? 0L : colD[i];
            yield return numA + numB + numC + numD;
        }
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<long> Plus(IEnumerable<long> a, IEnumerable<long> b, long c)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0L : colA[i];
            var numB = i >= colB.Count ? 0L : colB[i];
            yield return numA + numB + c;
        }
    }

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<long> Plus(IEnumerable<long> a, long b)
    {
        foreach (var item in a)
        {
            yield return item + b;
        }
    }

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static long[] Plus(long[] a, long[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new long[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? 0L : a[i];
            var numB = i >= b.Length ? 0L : b[i];
            c[i] = numA + numB;
        }

        return c;
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<long> Minus(IEnumerable<long> a, IEnumerable<long> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0 : colA[i];
            var numB = i >= colB.Count ? 0 : colB[i];
            yield return numA - numB;
        }
    }

    /// <summary>
    /// Minuss a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<long> Minus(IEnumerable<long> a, long b)
    {
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items ,inused.</returns>
    public static long[] Minus(long[] a, long[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new long[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? 0 : a[i];
            var numB = i >= b.Length ? 0 : b[i];
            c[i] = numA - numB;
        }

        return c;
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<float> Plus(IEnumerable<float> a, IEnumerable<float> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0f : colA[i];
            var numB = i >= colB.Count ? 0f : colB[i];
            yield return numA + numB;
        }
    }

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<float> Plus(IEnumerable<float> a, IEnumerable<float> b, IEnumerable<float> c, IEnumerable<float> d = null)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var colC = c?.ToList() ?? new();
        var colD = d?.ToList() ?? new();
        var count = Math.Max(Math.Max(colA.Count, colB.Count), Math.Max(colC.Count, colD.Count));
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0f : colA[i];
            var numB = i >= colB.Count ? 0f : colB[i];
            var numC = i >= colC.Count ? 0f : colC[i];
            var numD = i >= colD.Count ? 0f : colD[i];
            yield return numA + numB + numC + numD;
        }
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<float> Plus(IEnumerable<float> a, IEnumerable<float> b, float c)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0f : colA[i];
            var numB = i >= colB.Count ? 0f : colB[i];
            yield return numA + numB + c;
        }
    }

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<float> Plus(IEnumerable<float> a, float b)
    {
        foreach (var item in a)
        {
            yield return item + b;
        }
    }

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static float[] Plus(float[] a, float[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new float[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? 0f : a[i];
            var numB = i >= b.Length ? 0f : b[i];
            c[i] = numA + numB;
        }

        return c;
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<float> Minus(IEnumerable<float> a, IEnumerable<float> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0f : colA[i];
            var numB = i >= colB.Count ? 0f : colB[i];
            yield return numA - numB;
        }
    }

    /// <summary>
    /// Minuss a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<float> Minus(IEnumerable<float> a, float b)
    {
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items ,inused.</returns>
    public static float[] Minus(float[] a, float[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new float[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? 0f : a[i];
            var numB = i >= b.Length ? 0f : b[i];
            c[i] = numA - numB;
        }

        return c;
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<double> Plus(IEnumerable<double> a, IEnumerable<double> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0d : colA[i];
            var numB = i >= colB.Count ? 0d : colB[i];
            yield return numA + numB;
        }
    }

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<double> Plus(IEnumerable<double> a, IEnumerable<double> b, IEnumerable<double> c, IEnumerable<double> d = null)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var colC = c?.ToList() ?? new();
        var colD = d?.ToList() ?? new();
        var count = Math.Max(Math.Max(colA.Count, colB.Count), Math.Max(colC.Count, colD.Count));
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0d : colA[i];
            var numB = i >= colB.Count ? 0d : colB[i];
            var numC = i >= colC.Count ? 0d : colC[i];
            var numD = i >= colD.Count ? 0d : colD[i];
            yield return numA + numB + numC + numD;
        }
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<double> Plus(IEnumerable<double> a, IEnumerable<double> b, double c)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0d : colA[i];
            var numB = i >= colB.Count ? 0d : colB[i];
            yield return numA + numB + c;
        }
    }

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<double> Plus(IEnumerable<double> a, double b)
    {
        foreach (var item in a)
        {
            yield return item + b;
        }
    }

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static double[] Plus(double[] a, double[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new double[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? 0d : a[i];
            var numB = i >= b.Length ? 0d : b[i];
            c[i] = numA + numB;
        }

        return c;
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<double> Minus(IEnumerable<double> a, IEnumerable<double> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0d : colA[i];
            var numB = i >= colB.Count ? 0d : colB[i];
            yield return numA - numB;
        }
    }

    /// <summary>
    /// Minuss a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<double> Minus(IEnumerable<double> a, double b)
    {
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items ,inused.</returns>
    public static double[] Minus(double[] a, double[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new double[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? 0d : a[i];
            var numB = i >= b.Length ? 0d : b[i];
            c[i] = numA - numB;
        }

        return c;
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<decimal> Plus(IEnumerable<decimal> a, IEnumerable<decimal> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0 : colA[i];
            var numB = i >= colB.Count ? 0 : colB[i];
            yield return numA + numB;
        }
    }

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<decimal> Plus(IEnumerable<decimal> a, IEnumerable<decimal> b, IEnumerable<decimal> c, IEnumerable<decimal> d = null)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var colC = c?.ToList() ?? new();
        var colD = d?.ToList() ?? new();
        var count = Math.Max(Math.Max(colA.Count, colB.Count), Math.Max(colC.Count, colD.Count));
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0 : colA[i];
            var numB = i >= colB.Count ? 0 : colB[i];
            var numC = i >= colC.Count ? 0 : colC[i];
            var numD = i >= colD.Count ? 0 : colD[i];
            yield return numA + numB + numC + numD;
        }
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<decimal> Plus(IEnumerable<decimal> a, IEnumerable<decimal> b, decimal c)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0 : colA[i];
            var numB = i >= colB.Count ? 0 : colB[i];
            yield return numA + numB + c;
        }
    }

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<decimal> Plus(IEnumerable<decimal> a, decimal b)
    {
        foreach (var item in a)
        {
            yield return item + b;
        }
    }

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static decimal[] Plus(decimal[] a, decimal[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new decimal[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? 0 : a[i];
            var numB = i >= b.Length ? 0 : b[i];
            c[i] = numA + numB;
        }

        return c;
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<decimal> Minus(IEnumerable<decimal> a, IEnumerable<decimal> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? 0 : colA[i];
            var numB = i >= colB.Count ? 0 : colB[i];
            yield return numA - numB;
        }
    }

    /// <summary>
    /// Minuss a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<decimal> Minus(IEnumerable<decimal> a, decimal b)
    {
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items ,inused.</returns>
    public static decimal[] Minus(decimal[] a, decimal[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new decimal[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? 0 : a[i];
            var numB = i >= b.Length ? 0 : b[i];
            c[i] = numA - numB;
        }

        return c;
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<TimeSpan> Plus(IEnumerable<TimeSpan> a, IEnumerable<TimeSpan> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? TimeSpan.Zero : colA[i];
            var numB = i >= colB.Count ? TimeSpan.Zero : colB[i];
            yield return numA + numB;
        }
    }

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<TimeSpan> Plus(IEnumerable<TimeSpan> a, IEnumerable<TimeSpan> b, IEnumerable<TimeSpan> c, IEnumerable<TimeSpan> d = null)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var colC = c?.ToList() ?? new();
        var colD = d?.ToList() ?? new();
        var count = Math.Max(Math.Max(colA.Count, colB.Count), Math.Max(colC.Count, colD.Count));
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? TimeSpan.Zero : colA[i];
            var numB = i >= colB.Count ? TimeSpan.Zero : colB[i];
            var numC = i >= colC.Count ? TimeSpan.Zero : colC[i];
            var numD = i >= colD.Count ? TimeSpan.Zero : colD[i];
            yield return numA + numB + numC + numD;
        }
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<TimeSpan> Plus(IEnumerable<TimeSpan> a, IEnumerable<TimeSpan> b, TimeSpan c)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? TimeSpan.Zero : colA[i];
            var numB = i >= colB.Count ? TimeSpan.Zero : colB[i];
            yield return numA + numB + c;
        }
    }

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<TimeSpan> Plus(IEnumerable<TimeSpan> a, TimeSpan b)
    {
        foreach (var item in a)
        {
            yield return item + b;
        }
    }

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static TimeSpan[] Plus(TimeSpan[] a, TimeSpan[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new TimeSpan[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? TimeSpan.Zero : a[i];
            var numB = i >= b.Length ? TimeSpan.Zero : b[i];
            c[i] = numA + numB;
        }

        return c;
    }

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<DateTime> Plus(IEnumerable<DateTime> a, TimeSpan b)
    {
        foreach (var item in a)
        {
            yield return item + b;
        }
    }

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<DateTimeOffset> Plus(IEnumerable<DateTimeOffset> a, TimeSpan b)
    {
        foreach (var item in a)
        {
            yield return item + b;
        }
    }

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<TimeSpan> Minus(IEnumerable<TimeSpan> a, IEnumerable<TimeSpan> b)
    {
        var colA = a?.ToList() ?? new();
        var colB = b?.ToList() ?? new();
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? TimeSpan.Zero : colA[i];
            var numB = i >= colB.Count ? TimeSpan.Zero : colB[i];
            yield return numA - numB;
        }
    }

    /// <summary>
    /// Minuss a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<TimeSpan> Minus(IEnumerable<TimeSpan> a, TimeSpan b)
    {
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items ,inused.</returns>
    public static TimeSpan[] Minus(TimeSpan[] a, TimeSpan[] b)
    {
        var count = Math.Max(a.Length, b.Length);
        var c = new TimeSpan[count];
        for (var i = 0; i < count; i++)
        {
            var numA = i >= a.Length ? TimeSpan.Zero : a[i];
            var numB = i >= b.Length ? TimeSpan.Zero : b[i];
            c[i] = numA - numB;
        }

        return c;
    }

    /// <summary>
    /// Minuss a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<DateTime> Minus(IEnumerable<DateTime> a, TimeSpan b)
    {
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuss a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<DateTimeOffset> Minus(IEnumerable<DateTimeOffset> a, TimeSpan b)
    {
        foreach (var item in a)
        {
            yield return item - b;
        }
    }
}
