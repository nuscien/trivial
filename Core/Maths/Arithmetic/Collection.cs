using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Text;

namespace Trivial.Maths;

public static partial class Arithmetic
{
    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<int> Plus(IEnumerable<int> a, IEnumerable<int> b)
        => ListExtensions.Select(a, b, 0, Plus);

    private static int Plus(int itemA, bool hasA, int itemB, bool hasB, int i)
        => itemA + itemB;

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
        if (a == null) yield break;
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
        a ??= Array.Empty<int>();
        b ??= Array.Empty<int>();
        var c = new int[Math.Max(a.Length, b.Length)];
#if NET8_0_OR_GREATER
        if (!Vector<int>.IsSupported || (a.Length < 1000 && b.Length < 1000))
        {
            PlusByArray(c, 0, a, b);
            return c;
        }

        var step = Vector<int>.Count;
        if (step < 4)
        {
            PlusByArray(c, 0, a, b);
            return c;
        }

        var count = Math.Min(a.Length, b.Length);
        count -= count % step;
        for (int i = 0; i < count; i += step)
        {
            var vA = new Vector<int>(a, i);
            var vB = new Vector<int>(b, i);
            var result = vA + vB;
            result.CopyTo(c, i);
        }

        PlusByArray(c, count, a, b);
        return c;
#else
        PlusByArray(c, 0, a, b);
        return c;
#endif
    }

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="c">The output array.</param>
    /// <param name="start">The zero-based index to start enumerate in the input arrays.</param>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static void PlusByArray(int[] c, int start, int[] a, int[] b)
    {
        var count = Math.Min(a.Length, b.Length);
        for (var i = start; i < count; i++)
        {
            c[i] = a[i] + b[i];
        }

        if (a.Length > b.Length)
        {
            for (var i = b.Length; i < a.Length; i++)
            {
                c[i] = a[i];
            }
        }
        else if (a.Length < b.Length)
        {
            for (var i = a.Length; i < b.Length; i++)
            {
                c[i] = b[i];
            }
        }
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
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<int> Minus(IEnumerable<int> a, int b)
    {
        if (a == null) yield break;
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left number.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<int> Minus(int a, IEnumerable<int> b)
    {
        if (b == null) yield break;
        foreach (var item in b)
        {
            yield return a - item;
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
        a ??= Array.Empty<int>();
        b ??= Array.Empty<int>();
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
    /// Negates all items in the given collection.
    /// </summary>
    /// <param name="col">The number collection.</param>
    /// <returns>A collection.</returns>
    public static IEnumerable<int> Negate(IEnumerable<int> col)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            yield return -item;
        }
    }

    /// <summary>
    /// Negates all items in the given array.
    /// </summary>
    /// <param name="col">The number array.</param>
    /// <returns>An array.</returns>
    public static int[] Negate(int[] col)
    {
        if (col == null) return null;
        var c = new int[col.Length];
        for (var i = 0; i < col.Length; i++)
        {
            c[i] = -col[i];
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
        if (a == null) yield break;
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
        a ??= Array.Empty<long>();
        b ??= Array.Empty<long>();
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
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<long> Minus(IEnumerable<long> a, long b)
    {
        if (a == null) yield break;
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left number.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<long> Minus(long a, IEnumerable<long> b)
    {
        if (b == null) yield break;
        foreach (var item in b)
        {
            yield return a - item;
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
        a ??= Array.Empty<long>();
        b ??= Array.Empty<long>();
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
    /// Negates all items in the given collection.
    /// </summary>
    /// <param name="col">The number collection.</param>
    /// <returns>A collection.</returns>
    public static IEnumerable<long> Negate(IEnumerable<long> col)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            yield return -item;
        }
    }

    /// <summary>
    /// Negates all items in the given array.
    /// </summary>
    /// <param name="col">The number array.</param>
    /// <returns>An array.</returns>
    public static long[] Negate(long[] col)
    {
        if (col == null) return null;
        var c = new long[col.Length];
        for (var i = 0; i < col.Length; i++)
        {
            c[i] = -col[i];
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
        if (float.IsNaN(c)) c = 0f;
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
        if (a == null) yield break;
        if (float.IsNaN(b)) b = 0f;
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
        a ??= Array.Empty<float>();
        b ??= Array.Empty<float>();
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
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<float> Minus(IEnumerable<float> a, float b)
    {
        if (a == null) yield break;
        if (float.IsNaN(b)) b = 0f;
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left number.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<float> Minus(float a, IEnumerable<float> b)
    {
        if (b == null) yield break;
        if (float.IsNaN(a)) a = 0f;
        foreach (var item in b)
        {
            yield return a - item;
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
        a ??= Array.Empty<float>();
        b ??= Array.Empty<float>();
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
    /// Negates all items in the given collection.
    /// </summary>
    /// <param name="col">The number collection.</param>
    /// <returns>A collection.</returns>
    public static IEnumerable<float> Negate(IEnumerable<float> col)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            yield return -item;
        }
    }

    /// <summary>
    /// Negates all items in the given array.
    /// </summary>
    /// <param name="col">The number array.</param>
    /// <returns>An array.</returns>
    public static float[] Negate(float[] col)
    {
        if (col == null) return null;
        var c = new float[col.Length];
        for (var i = 0; i < col.Length; i++)
        {
            c[i] = -col[i];
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
        if (double.IsNaN(c)) c = 0d;
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
        if (a == null) yield break;
        if (double.IsNaN(b)) b = 0d;
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
        a ??= Array.Empty<double>();
        b ??= Array.Empty<double>();
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
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<double> Minus(IEnumerable<double> a, double b)
    {
        if (a == null) yield break;
        if (double.IsNaN(b)) b = 0d;
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left number.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<double> Minus(double a, IEnumerable<double> b)
    {
        if (b == null) yield break;
        if (double.IsNaN(a)) a = 0d;
        foreach (var item in b)
        {
            yield return a - item;
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
        a ??= Array.Empty<double>();
        b ??= Array.Empty<double>();
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
    /// Negates all items in the given collection.
    /// </summary>
    /// <param name="col">The number collection.</param>
    /// <returns>A collection.</returns>
    public static IEnumerable<double> Negate(IEnumerable<double> col)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            yield return -item;
        }
    }

    /// <summary>
    /// Negates all items in the given array.
    /// </summary>
    /// <param name="col">The number array.</param>
    /// <returns>An array.</returns>
    public static double[] Negate(double[] col)
    {
        if (col == null) return null;
        var c = new double[col.Length];
        for (var i = 0; i < col.Length; i++)
        {
            c[i] = -col[i];
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
        if (a == null) yield break;
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
        a ??= Array.Empty<decimal>();
        b ??= Array.Empty<decimal>();
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
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<decimal> Minus(IEnumerable<decimal> a, decimal b)
    {
        if (a == null) yield break;
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left number.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<decimal> Minus(decimal a, IEnumerable<decimal> b)
    {
        if (b == null) yield break;
        foreach (var item in b)
        {
            yield return a - item;
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
        a ??= Array.Empty<decimal>();
        b ??= Array.Empty<decimal>();
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
    /// Negates all items in the given collection.
    /// </summary>
    /// <param name="col">The number collection.</param>
    /// <returns>A collection.</returns>
    public static IEnumerable<decimal> Negate(IEnumerable<decimal> col)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            yield return -item;
        }
    }

    /// <summary>
    /// Negates all items in the given array.
    /// </summary>
    /// <param name="col">The number array.</param>
    /// <returns>An array.</returns>
    public static decimal[] Negate(decimal[] col)
    {
        if (col == null) return null;
        var c = new decimal[col.Length];
        for (var i = 0; i < col.Length; i++)
        {
            c[i] = -col[i];
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
        if (a == null) yield break;
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
        a ??= Array.Empty<TimeSpan>();
        b ??= Array.Empty<TimeSpan>();
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
        if (a == null) yield break;
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
        if (a == null) yield break;
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
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<TimeSpan> Minus(IEnumerable<TimeSpan> a, TimeSpan b)
    {
        if (a == null) yield break;
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left number.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<TimeSpan> Minus(TimeSpan a, IEnumerable<TimeSpan> b)
    {
        if (b == null) yield break;
        foreach (var item in b)
        {
            yield return a - item;
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
        a ??= Array.Empty<TimeSpan>();
        b ??= Array.Empty<TimeSpan>();
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
    /// Negates all items in the given collection.
    /// </summary>
    /// <param name="col">The number collection.</param>
    /// <returns>A collection.</returns>
    public static IEnumerable<TimeSpan> Negate(IEnumerable<TimeSpan> col)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            yield return -item;
        }
    }

    /// <summary>
    /// Negates all items in the given array.
    /// </summary>
    /// <param name="col">The number array.</param>
    /// <returns>An array.</returns>
    public static TimeSpan[] Negate(TimeSpan[] col)
    {
        if (col == null) return null;
        var c = new TimeSpan[col.Length];
        for (var i = 0; i < col.Length; i++)
        {
            c[i] = -col[i];
        }

        return c;
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<DateTime> Minus(IEnumerable<DateTime> a, TimeSpan b)
    {
        if (a == null) yield break;
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<DateTimeOffset> Minus(IEnumerable<DateTimeOffset> a, TimeSpan b)
    {
        if (a == null) yield break;
        foreach (var item in a)
        {
            yield return item - b;
        }
    }

    /// <summary>
    /// Negates all items in the given collection.
    /// </summary>
    /// <param name="col">The number collection.</param>
    /// <returns>A collection.</returns>
    public static IEnumerable<bool> Negate(IEnumerable<bool> col)
    {
        if (col == null) yield break;
        foreach (var item in col)
        {
            yield return !item;
        }
    }

    /// <summary>
    /// Negates all items in the given array.
    /// </summary>
    /// <param name="col">The number array.</param>
    /// <returns>An array.</returns>
    public static bool[] Negate(bool[] col)
    {
        if (col == null) return null;
        var c = new bool[col.Length];
        for (var i = 0; i < col.Length; i++)
        {
            c[i] = !col[i];
        }

        return c;
    }
}
