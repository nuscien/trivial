using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
        => Compute(a, b, c, d, 0, Plus);

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<int> Plus(IEnumerable<int> a, IEnumerable<int> b, int c)
        => Compute(a, b, c, 0, Plus);

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<int> Plus(IEnumerable<int> a, int b)
        => Compute(a, b, Plus);

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static int[] Plus(int[] a, int[] b)
        => Compute(a, b, 0, Plus, Plus);

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static int[] Plus(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
        => Compute(a, b, 0, Plus, Plus);

    private static int Plus(int a, int b)
        => a + b;

    private static Vector<int> Plus(Vector<int> a, Vector<int> b)
        => a + b;

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<int> Minus(IEnumerable<int> a, IEnumerable<int> b)
        => ListExtensions.Select(a, b, 0, Minus);

    private static int Minus(int itemA, bool hasA, int itemB, bool hasB, int i)
        => itemA - itemB;

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<int> Minus(IEnumerable<int> a, int b)
        => Compute(a, b, Minus);

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
        => Compute(a, b, 0, Minus, Minus);

    /// <summary>
    /// Minuses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items ,inused.</returns>
    public static int[] Minus(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
        => Compute(a, b, 0, Minus, Minus);

    private static int Minus(int a, int b)
        => a - b;

    private static Vector<int> Minus(Vector<int> a, Vector<int> b)
        => a - b;

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
        => ListExtensions.Select(a, b, 0L, Plus);

    private static long Plus(long itemA, bool hasA, long itemB, bool hasB, int i)
        => itemA + itemB;

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<long> Plus(IEnumerable<long> a, IEnumerable<long> b, IEnumerable<long> c, IEnumerable<long> d = null)
        => Compute(a, b, c, d, 0L, Plus);

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<long> Plus(IEnumerable<long> a, IEnumerable<long> b, long c)
        => Compute(a, b, c, 0L, Plus);

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<long> Plus(IEnumerable<long> a, long b)
        => Compute(a, b, Plus);

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static long[] Plus(long[] a, long[] b)
        => Compute(a, b, 0L, Plus, Plus);

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static long[] Plus(ReadOnlySpan<long> a, ReadOnlySpan<long> b)
        => Compute(a, b, 0L, Plus, Plus);

    private static long Plus(long a, long b)
        => a + b;

    private static Vector<long> Plus(Vector<long> a, Vector<long> b)
        => a + b;

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<long> Minus(IEnumerable<long> a, IEnumerable<long> b)
        => ListExtensions.Select(a, b, 0L, Minus);

    private static long Minus(long itemA, bool hasA, long itemB, bool hasB, int i)
        => itemA - itemB;

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<long> Minus(IEnumerable<long> a, long b)
        => Compute(a, b, Minus);

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
        => Compute(a, b, 0L, Minus, Minus);

    /// <summary>
    /// Minuses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items ,inused.</returns>
    public static long[] Minus(ReadOnlySpan<long> a, ReadOnlySpan<long> b)
        => Compute(a, b, 0L, Minus, Minus);

    private static long Minus(long a, long b)
        => a - b;

    private static Vector<long> Minus(Vector<long> a, Vector<long> b)
        => a - b;

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
        => ListExtensions.Select(a, b, 0f, Plus);

    private static float Plus(float itemA, bool hasA, float itemB, bool hasB, int i)
        => itemA + itemB;

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<float> Plus(IEnumerable<float> a, IEnumerable<float> b, IEnumerable<float> c, IEnumerable<float> d = null)
        => Compute(a, b, c, d, 0f, Plus);

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<float> Plus(IEnumerable<float> a, IEnumerable<float> b, float c)
        => Compute(a, b, float.IsNaN(c) ? 0f : c, 0f, Plus);

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<float> Plus(IEnumerable<float> a, float b)
        => Compute(a, float.IsNaN(b) ? 0f : b, Plus);

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static float[] Plus(float[] a, float[] b)
        => Compute(a, b, 0f, Plus, Plus);

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static float[] Plus(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
        => Compute(a, b, 0f, Plus, Plus);

    private static float Plus(float a, float b)
        => a + b;

    private static Vector<float> Plus(Vector<float> a, Vector<float> b)
        => a + b;

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<float> Minus(IEnumerable<float> a, IEnumerable<float> b)
        => ListExtensions.Select(a, b, 0f, Minus);

    private static float Minus(float itemA, bool hasA, float itemB, bool hasB, int i)
        => itemA - itemB;

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<float> Minus(IEnumerable<float> a, float b)
        => Compute(a, float.IsNaN(b) ? 0f : b, Minus);

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
        => Compute(a, b, 0f, Minus, Minus);

    /// <summary>
    /// Minuses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items ,inused.</returns>
    public static float[] Minus(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
        => Compute(a, b, 0f, Minus, Minus);

    private static float Minus(float a, float b)
        => a - b;

    private static Vector<float> Minus(Vector<float> a, Vector<float> b)
        => a - b;

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
        => ListExtensions.Select(a, b, 0d, Plus);

    private static double Plus(double itemA, bool hasA, double itemB, bool hasB, int i)
        => itemA + itemB;

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<double> Plus(IEnumerable<double> a, IEnumerable<double> b, IEnumerable<double> c, IEnumerable<double> d = null)
        => Compute(a, b, c, d, 0d, Plus);

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<double> Plus(IEnumerable<double> a, IEnumerable<double> b, double c)
        => Compute(a, b, double.IsNaN(c) ? 0d : c, 0d, Plus);

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<double> Plus(IEnumerable<double> a, double b)
        => Compute(a, double.IsNaN(b) ? 0d : b, Plus);

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static double[] Plus(double[] a, double[] b)
        => Compute(a, b, 0d, Plus, Plus);

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items plused.</returns>
    public static double[] Plus(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        => Compute(a, b, 0d, Plus, Plus);

    private static double Plus(double a, double b)
        => a + b;

    private static Vector<double> Plus(Vector<double> a, Vector<double> b)
        => a + b;

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<double> Minus(IEnumerable<double> a, IEnumerable<double> b)
        => ListExtensions.Select(a, b, 0d, Minus);

    private static double Minus(double itemA, bool hasA, double itemB, bool hasB, int i)
        => itemA - itemB;

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<double> Minus(IEnumerable<double> a, double b)
        => Compute(a, double.IsNaN(b) ? 0d : b, Minus);

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
        => Compute(a, b, 0d, Minus, Minus);

    /// <summary>
    /// Minuses each item in 2 arrays by index.
    /// </summary>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <returns>An array with items ,inused.</returns>
    public static double[] Minus(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        => Compute(a, b, 0d, Minus, Minus);

    private static double Minus(double a, double b)
        => a - b;

    private static Vector<double> Minus(Vector<double> a, Vector<double> b)
        => a - b;

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
        => ListExtensions.Select(a, b, decimal.Zero, Plus);

    private static decimal Plus(decimal itemA, bool hasA, decimal itemB, bool hasB, int i)
        => itemA + itemB;

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<decimal> Plus(IEnumerable<decimal> a, IEnumerable<decimal> b, IEnumerable<decimal> c, IEnumerable<decimal> d = null)
        => Compute(a, b, c, d, decimal.Zero, Plus);

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<decimal> Plus(IEnumerable<decimal> a, IEnumerable<decimal> b, decimal c)
        => Compute(a, b, c, decimal.Zero, Plus);

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items plused.</returns>
    public static IEnumerable<decimal> Plus(IEnumerable<decimal> a, decimal b)
        => Compute(a, b, Plus);

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
            var numA = i >= a.Length ? decimal.Zero : a[i];
            var numB = i >= b.Length ? decimal.Zero : b[i];
            c[i] = numA + numB;
        }

        return c;
    }

    private static decimal Plus(decimal a, decimal b)
        => a + b;

    /// <summary>
    /// Minuses each item in 2 collections by index.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<decimal> Minus(IEnumerable<decimal> a, IEnumerable<decimal> b)
        => ListExtensions.Select(a, b, decimal.Zero, Minus);

    private static decimal Minus(decimal itemA, bool hasA, decimal itemB, bool hasB, int i)
        => itemA - itemB;

    /// <summary>
    /// Minuses.
    /// </summary>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <returns>A collection with items minused.</returns>
    public static IEnumerable<decimal> Minus(IEnumerable<decimal> a, decimal b)
        => Compute(a, b, Minus);

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
            var numA = i >= a.Length ? decimal.Zero : a[i];
            var numB = i >= b.Length ? decimal.Zero : b[i];
            c[i] = numA - numB;
        }

        return c;
    }

    private static decimal Minus(decimal a, decimal b)
        => a - b;

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

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="compute">The handler to compute 2 values.</param>
    /// <param name="vectorCompute">The handler to compute 2 vectors.</param>
    /// <returns>An array with items plused.</returns>
    private static T[] Compute<T>(T[] a, T[] b, T defaultValue, Func<T, T, T> compute, Func<Vector<T>, Vector<T>, Vector<T>> vectorCompute) where T : struct
    {
        a ??= Array.Empty<T>();
        b ??= Array.Empty<T>();
        var c = new T[Math.Max(a.Length, b.Length)];
#if NET8_0_OR_GREATER
        if (!Vector<T>.IsSupported || (a.Length < 1000 && b.Length < 1000))
        {
            ComputeByArray(c, 0, a, b, defaultValue, compute);
            return c;
        }

        var step = Vector<T>.Count;
        if (step < 4)
        {
            ComputeByArray(c, 0, a, b, defaultValue, compute);
            return c;
        }

        var count = Math.Min(a.Length, b.Length);
        count -= count % step;
        for (var i = 0; i < count; i += step)
        {
            var vA = new Vector<T>(a, i);
            var vB = new Vector<T>(b, i);
            var result = vectorCompute(vA, vB);
            result.CopyTo(c, i);
        }

        ComputeByArray(c, count, a, b, defaultValue, compute);
        return c;
#else
        ComputeByArray(c, 0, a, b, defaultValue, compute);
        return c;
#endif
    }

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="compute">The handler to compute 2 values.</param>
    /// <param name="vectorCompute">The handler to compute 2 vectors.</param>
    /// <returns>An array with items plused.</returns>
    private static T[] Compute<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b, T defaultValue, Func<T, T, T> compute, Func<Vector<T>, Vector<T>, Vector<T>> vectorCompute) where T : struct
    {
        var c = new T[Math.Max(a.Length, b.Length)];
#if NET8_0_OR_GREATER
        if (!Vector<T>.IsSupported || (a.Length < 1000 && b.Length < 1000))
        {
            ComputeByArray(c, 0, a, b, defaultValue, compute);
            return c;
        }

        var step = Vector<T>.Count;
        if (step < 4)
        {
            ComputeByArray(c, 0, a, b, defaultValue, compute);
            return c;
        }

        var count = Math.Min(a.Length, b.Length);
        count -= count % step;
        for (var i = 0; i < count; i += step)
        {
            var vA = new Vector<T>(a.Slice(i));
            var vB = new Vector<T>(b.Slice(i));
            var result = vectorCompute(vA, vB);
            result.CopyTo(c, i);
        }

        ComputeByArray(c, count, a, b, defaultValue, compute);
        return c;
#else
        ComputeByArray(c, 0, a, b, defaultValue, compute);
        return c;
#endif
    }

    /// <summary>
    /// Pluses a number and each item in a collection.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right number.</param>
    /// <param name="compute">The handler to compute 2 values.</param>
    /// <returns>A collection with items plused.</returns>
    private static IEnumerable<T> Compute<T>(IEnumerable<T> a, T b, Func<T, T, T> compute) where T : struct
    {
        if (a == null) yield break;
        foreach (var item in a)
        {
            yield return compute(item, b);
        }
    }

    /// <summary>
    /// Pluses each item in 3-4 collections by index.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Middle collection.</param>
    /// <param name="c">Right collection.</param>
    /// <param name="d">Additional collection.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="compute">The handler to compute 2 values.</param>
    /// <returns>A collection with items plused.</returns>
    private static IEnumerable<T> Compute<T>(IEnumerable<T> a, IEnumerable<T> b, IEnumerable<T> c, IEnumerable<T> d, T defaultValue, Func<T, T, T> compute)
    {
        var colA = ListExtensions.ToList(a, true);
        var colB = ListExtensions.ToList(b, true);
        var colC = ListExtensions.ToList(c, true);
        var colD = ListExtensions.ToList(d, true);
        var count = Math.Max(Math.Max(colA.Count, colB.Count), Math.Max(colC.Count, colD.Count));
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? defaultValue : colA[i];
            var numB = i >= colB.Count ? defaultValue : colB[i];
            var numC = i >= colC.Count ? defaultValue : colC[i];
            var numD = i >= colD.Count ? defaultValue : colD[i];
            yield return compute(compute(compute(numA, numB), numC), numD);
        }
    }

    /// <summary>
    /// Pluses each item in 2 collections by index.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="a">Left collection.</param>
    /// <param name="b">Right collection.</param>
    /// <param name="c">Additional number to plus.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="compute">The handler to compute 2 values.</param>
    /// <returns>A collection with items plused.</returns>
    private static IEnumerable<T> Compute<T>(IEnumerable<T> a, IEnumerable<T> b, T c, T defaultValue, Func<T, T, T> compute)
    {
        var colA = ListExtensions.ToList(a, true);
        var colB = ListExtensions.ToList(b, true);
        var count = Math.Max(colA.Count, colB.Count);
        for (var i = 0; i < count; i++)
        {
            var numA = i >= colA.Count ? defaultValue : colA[i];
            var numB = i >= colB.Count ? defaultValue : colB[i];
            yield return compute(compute(numA, numB), c);
        }
    }

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="c">The output array.</param>
    /// <param name="start">The zero-based index to start enumerate in the input arrays.</param>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="compute">The handler to compute 2 values.</param>
    /// <returns>An array with items plused.</returns>
    private static void ComputeByArray<T>(T[] c, int start, T[] a, T[] b, T defaultValue, Func<T, T, T> compute)
    {
        var count = Math.Min(a.Length, b.Length);
        for (var i = start; i < count; i++)
        {
            c[i] = compute(a[i], b[i]);
        }

        if (a.Length > b.Length)
        {
            for (var i = b.Length; i < a.Length; i++)
            {
                c[i] = compute(a[i], defaultValue);
            }
        }
        else if (a.Length < b.Length)
        {
            for (var i = a.Length; i < b.Length; i++)
            {
                c[i] = compute(defaultValue, b[i]);
            }
        }
    }

    /// <summary>
    /// Pluses each item in 2 arrays by index.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="c">The output array.</param>
    /// <param name="start">The zero-based index to start enumerate in the input arrays.</param>
    /// <param name="a">Left array.</param>
    /// <param name="b">Right array.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="compute">The handler to compute 2 values.</param>
    /// <returns>An array with items plused.</returns>
    private static void ComputeByArray<T>(T[] c, int start, ReadOnlySpan<T> a, ReadOnlySpan<T> b, T defaultValue, Func<T, T, T> compute)
    {
        var count = Math.Min(a.Length, b.Length);
        for (var i = start; i < count; i++)
        {
            c[i] = compute(a[i], b[i]);
        }

        if (a.Length > b.Length)
        {
            for (var i = b.Length; i < a.Length; i++)
            {
                c[i] = compute(a[i], defaultValue);
            }
        }
        else if (a.Length < b.Length)
        {
            for (var i = a.Length; i < b.Length; i++)
            {
                c[i] = compute(defaultValue, b[i]);
            }
        }
    }
}
