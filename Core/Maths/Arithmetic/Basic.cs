// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arithmetic\Basic.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The basic arithmetic functions.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trivial.Maths;

/// <summary>
/// The utility for arithmetic.
/// </summary>
public static partial class Arithmetic
{
    internal const double DoubleAccuracy = 1e-10;

    /// <summary>
    /// Gets a result of factorial for a specific number.
    /// </summary>
    /// <param name="value">A number to calculate.</param>
    /// <returns>A number of result.</returns>
    /// <example>
    /// <code>
    /// var factorialNum = Arithmetic.Factorial(20); // => 2432902008176640000
    /// </code>
    /// </example>
    public static long Factorial(uint value)
    {
        if (value < 2) return 1;
        var resultNum = (long)value;
        for (uint step = 2; step < value; step++)
        {
            resultNum *= step;
        }

        return resultNum;
    }

    /// <summary>
    /// Gets a result of factorial for a specific number.
    /// </summary>
    /// <param name="value">A number to calculate.</param>
    /// <returns>A number of result.</returns>
    /// <example>
    /// <code>
    /// var factorialNum = Arithmetic.FactorialApproximate(100); // 9.33262154439442e+157
    /// </code>
    /// </example>
    public static double FactorialApproximate(uint value)
    {
        if (value < 2) return 1;
        var resultNum = (double)value;
        for (double step = 2; step < value; step++)
        {
            resultNum *= step;
        }

        return resultNum;
    }

    /// <summary>
    /// Calculates the value times 1024 of the specific power.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="e">The exponential.</param>
    /// <returns>The result calculated.</returns>
    /// <remarks>You can use this to calculate such as 80K or 4M.</remarks>
    public static long Times1024(int value, int e = 1)
    {
        return value * (long)Math.Pow(1024, e);
    }

    /// <summary>
    /// Calculates the value times 1024 of the specific power.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="e">The exponential.</param>
    /// <returns>The result calculated.</returns>
    /// <remarks>You can use this to calculate such as 80K or 4M.</remarks>
    public static double Times1024(long value, int e = 1)
    {
        return value * Math.Pow(1024, e);
    }

    /// <summary>
    /// Gets the greatest common divisor of the given integers.
    /// </summary>
    /// <param name="a">Number 1.</param>
    /// <param name="b">Number 2.</param>
    /// <returns>The greatest common divisor.</returns>
    /// <example>
    /// <code>
    /// var gcdNum = Arithmetic.Gcd(192, 128); // => 64
    /// </code>
    /// </example>
    public static int Gcd(int a, int b)
    {
        if (a == 0 || b == 0) return 1;
        a = Math.Abs(a);
        b = Math.Abs(b);
        var big = Math.Max(a, b);
        var small = Math.Min(a, b);
        while (small > 0)
        {
            var remainder = big % small;
            big = small;
            small = remainder;
        }

        return big;
    }

    /// <summary>
    /// Gets the greatest common divisor of the given integers.
    /// </summary>
    /// <param name="a">Number 1.</param>
    /// <param name="b">Number 2.</param>
    /// <returns>The greatest common divisor.</returns>
    public static long Gcd(long a, long b)
    {
        if (a == 0 || b == 0) return 1;
        a = Math.Abs(a);
        b = Math.Abs(b);
        var big = Math.Max(a, b);
        var small = Math.Min(a, b);
        while (small > 0)
        {
            var remainder = big % small;
            big = small;
            small = remainder;
        }

        return big;
    }

    /// <summary>
    /// Gets the least common multiple of the given integers.
    /// </summary>
    /// <param name="a">Number 1.</param>
    /// <param name="b">Number 2.</param>
    /// <returns>The least common multiple.</returns>
    /// <example>
    /// <code>
    /// var lcmNum = Arithmetic.Lcm(192, 128); // => 384
    /// </code>
    /// </example>
    public static int Lcm(int a, int b)
    {
        return a / Gcd(a, b) * b;
    }

    /// <summary>
    /// Gets the least common multiple of the given integers.
    /// </summary>
    /// <param name="a">Number 1.</param>
    /// <param name="b">Number 2.</param>
    /// <returns>The least common multiple.</returns>
    public static long Lcm(long a, long b)
    {
        return a / Gcd(a, b) * b;
    }

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static byte Min(byte a, byte b)
        => Math.Min(a, b);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static byte Min(byte a, byte b, byte c)
        => Math.Min(Math.Min(a, b), c);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static sbyte Min(sbyte a, sbyte b)
        => Math.Min(a, b);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static sbyte Min(sbyte a, sbyte b, sbyte c)
        => Math.Min(Math.Min(a, b), c);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static short Min(short a, short b)
        => Math.Min(a, b);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static short Min(short a, short b, short c)
        => Math.Min(Math.Min(a, b), c);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static ushort Min(ushort a, ushort b)
        => Math.Min(a, b);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static ushort Min(ushort a, ushort b, ushort c)
        => Math.Min(Math.Min(a, b), c);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static int Min(int a, int b)
        => Math.Min(a, b);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static int Min(int a, int b, int c)
        => Math.Min(Math.Min(a, b), c);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static uint Min(uint a, uint b)
        => Math.Min(a, b);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static uint Min(uint a, uint b, uint c)
        => Math.Min(Math.Min(a, b), c);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <param name="d">Number 4 to compare.</param>
    /// <param name="e">Optional number 5 to compare.</param>
    /// <param name="f">Optional number 6 to compare.</param>
    /// <param name="g">Optional number 7 to compare.</param>
    /// <param name="h">Optional number 8 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static int Min(int a, int b, int c, int d, int? e = null, int? f = null, int? g = null, int? h = null)
    {
        var m = Math.Min(Math.Min(Math.Min(a, b), c), d);
        if (e.HasValue) m = Math.Min(m, e.Value);
        if (f.HasValue) m = Math.Min(m, f.Value);
        if (g.HasValue) m = Math.Min(m, g.Value);
        if (h.HasValue) m = Math.Min(m, h.Value);
        return m;
    }

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static long Min(long a, long b)
        => Math.Min(a, b);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static long Min(long a, long b, long c)
        => Math.Min(Math.Min(a, b), c);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <param name="d">Number 4 to compare.</param>
    /// <param name="e">Optional number 5 to compare.</param>
    /// <param name="f">Optional number 6 to compare.</param>
    /// <param name="g">Optional number 7 to compare.</param>
    /// <param name="h">Optional number 8 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static long Min(long a, long b, long c, long d, long? e = null, long? f = null, long? g = null, long? h = null)
    {
        var m = Math.Min(Math.Min(Math.Min(a, b), c), d);
        if (e.HasValue) m = Math.Min(m, e.Value);
        if (f.HasValue) m = Math.Min(m, f.Value);
        if (g.HasValue) m = Math.Min(m, g.Value);
        if (h.HasValue) m = Math.Min(m, h.Value);
        return m;
    }

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static ulong Min(ulong a, ulong b)
        => Math.Min(a, b);

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static ulong Min(ulong a, ulong b, ulong c)
        => Math.Min(Math.Min(a, b), c);

#if NET6_0_OR_GREATER
    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static Half Min(Half a, Half b)
        => a <= b ? a : b;

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static Half Min(Half a, Half b, Half c)
        => Min(Min(a, b), c);
#endif

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Optional number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static float Min(float a, float b, float? c = null)
    {
        var m = Math.Min(a, b);
        if (c.HasValue) m = Math.Min(m, c.Value);
        return m;
    }

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <param name="d">Number 4 to compare.</param>
    /// <param name="e">Optional number 5 to compare.</param>
    /// <param name="f">Optional number 6 to compare.</param>
    /// <param name="g">Optional number 7 to compare.</param>
    /// <param name="h">Optional number 8 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static float Min(float a, float b, float c, float d, float? e = null, float? f = null, float? g = null, float? h = null)
    {
        var m = Math.Min(Math.Min(Math.Min(a, b), c), d);
        if (e.HasValue) m = Math.Min(m, e.Value);
        if (f.HasValue) m = Math.Min(m, f.Value);
        if (g.HasValue) m = Math.Min(m, g.Value);
        if (h.HasValue) m = Math.Min(m, h.Value);
        return m;
    }

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Optional number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static decimal Min(decimal a, decimal b, decimal? c = null)
    {
        var m = Math.Min(a, b);
        if (c.HasValue) m = Math.Min(m, c.Value);
        return m;
    }

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <param name="d">Number 4 to compare.</param>
    /// <param name="e">Optional number 5 to compare.</param>
    /// <param name="f">Optional number 6 to compare.</param>
    /// <param name="g">Optional number 7 to compare.</param>
    /// <param name="h">Optional number 8 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static decimal Min(decimal a, decimal b, decimal c, decimal d, decimal? e = null, decimal? f = null, decimal? g = null, decimal? h = null)
    {
        var m = Math.Min(Math.Min(Math.Min(a, b), c), d);
        if (e.HasValue) m = Math.Min(m, e.Value);
        if (f.HasValue) m = Math.Min(m, f.Value);
        if (g.HasValue) m = Math.Min(m, g.Value);
        if (h.HasValue) m = Math.Min(m, h.Value);
        return m;
    }

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Optional number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static double Min(double a, double b, double? c = null)
    {
        var m = Math.Min(a, b);
        if (c.HasValue) m = Math.Min(m, c.Value);
        return m;
    }

    /// <summary>
    /// Returns the smaller of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <param name="d">Number 4 to compare.</param>
    /// <param name="e">Optional number 5 to compare.</param>
    /// <param name="f">Optional number 6 to compare.</param>
    /// <param name="g">Optional number 7 to compare.</param>
    /// <param name="h">Optional number 8 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static double Min(double a, double b, double c, double d, double? e = null, double? f = null, double? g = null, double? h = null)
    {
        var m = Math.Min(Math.Min(Math.Min(a, b), c), d);
        if (e.HasValue) m = Math.Min(m, e.Value);
        if (f.HasValue) m = Math.Min(m, f.Value);
        if (g.HasValue) m = Math.Min(m, g.Value);
        if (h.HasValue) m = Math.Min(m, h.Value);
        return m;
    }

    /// <summary>
    /// Returns the smaller of the specific time spans.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static TimeSpan Min(TimeSpan a, TimeSpan b)
        => a <= b ? a : b;

    /// <summary>
    /// Returns the smaller of the specific time spans.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static TimeSpan Min(TimeSpan a, TimeSpan b, TimeSpan c)
        => Min(Min(a, b), c);

    /// <summary>
    /// Returns the smaller of the specific dates and times.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static DateTime Min(DateTime a, DateTime b)
        => a <= b ? a : b;

    /// <summary>
    /// Returns the smaller of the specific dates and times.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static DateTime Min(DateTime a, DateTime b, DateTime c)
        => Min(Min(a, b), c);

    /// <summary>
    /// Returns the smaller of the specific dates and times.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static DateTimeOffset Min(DateTimeOffset a, DateTimeOffset b)
        => a <= b ? a : b;

    /// <summary>
    /// Returns the smaller of the specific dates and times.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is smaller.</returns>
    public static DateTimeOffset Min(DateTimeOffset a, DateTimeOffset b, DateTimeOffset c)
        => Min(Min(a, b), c);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static byte Max(byte a, byte b)
        => Math.Max(a, b);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static byte Max(byte a, byte b, byte c)
        => Math.Max(Math.Max(a, b), c);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static sbyte Max(sbyte a, sbyte b)
        => Math.Max(a, b);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static sbyte Max(sbyte a, sbyte b, sbyte c)
        => Math.Max(Math.Max(a, b), c);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static short Max(short a, short b)
        => Math.Max(a, b);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static short Max(short a, short b, short c)
        => Math.Max(Math.Max(a, b), c);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static ushort Max(ushort a, ushort b)
        => Math.Max(a, b);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static ushort Max(ushort a, ushort b, ushort c)
        => Math.Max(Math.Max(a, b), c);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static int Max(int a, int b)
        => Math.Max(a, b);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static int Max(int a, int b, int c)
        => Math.Max(Math.Max(a, b), c);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static uint Max(uint a, uint b)
        => Math.Max(a, b);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static uint Max(uint a, uint b, uint c)
        => Math.Max(Math.Max(a, b), c);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <param name="d">Number 4 to compare.</param>
    /// <param name="e">Optional number 5 to compare.</param>
    /// <param name="f">Optional number 6 to compare.</param>
    /// <param name="g">Optional number 7 to compare.</param>
    /// <param name="h">Optional number 8 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static int Max(int a, int b, int c, int d, int? e = null, int? f = null, int? g = null, int? h = null)
    {
        var m = Math.Max(Math.Max(Math.Max(a, b), c), d);
        if (e.HasValue) m = Math.Max(m, e.Value);
        if (f.HasValue) m = Math.Max(m, f.Value);
        if (g.HasValue) m = Math.Max(m, g.Value);
        if (h.HasValue) m = Math.Max(m, h.Value);
        return m;
    }

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static long Max(long a, long b)
        => Math.Max(a, b);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static long Max(long a, long b, long c)
        => Math.Max(Math.Max(a, b), c);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <param name="d">Number 4 to compare.</param>
    /// <param name="e">Optional number 5 to compare.</param>
    /// <param name="f">Optional number 6 to compare.</param>
    /// <param name="g">Optional number 7 to compare.</param>
    /// <param name="h">Optional number 8 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static long Max(long a, long b, long c, long d, long? e = null, long? f = null, long? g = null, long? h = null)
    {
        var m = Math.Max(Math.Max(Math.Max(a, b), c), d);
        if (e.HasValue) m = Math.Max(m, e.Value);
        if (f.HasValue) m = Math.Max(m, f.Value);
        if (g.HasValue) m = Math.Max(m, g.Value);
        if (h.HasValue) m = Math.Max(m, h.Value);
        return m;
    }

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static ulong Max(ulong a, ulong b)
        => Math.Max(a, b);

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static ulong Max(ulong a, ulong b, ulong c)
        => Math.Max(Math.Max(a, b), c);

#if NET6_0_OR_GREATER
    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static Half Max(Half a, Half b)
        => a >= b ? a : b;

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static Half Max(Half a, Half b, Half c)
        => Max(Max(a, b), c);
#endif

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Optional number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static float Max(float a, float b, float? c = null)
    {
        var m = Math.Max(a, b);
        if (c.HasValue) m = Math.Max(m, c.Value);
        return m;
    }

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <param name="d">Number 4 to compare.</param>
    /// <param name="e">Optional number 5 to compare.</param>
    /// <param name="f">Optional number 6 to compare.</param>
    /// <param name="g">Optional number 7 to compare.</param>
    /// <param name="h">Optional number 8 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static float Max(float a, float b, float c, float d, float? e = null, float? f = null, float? g = null, float? h = null)
    {
        var m = Math.Max(Math.Max(Math.Max(a, b), c), d);
        if (e.HasValue) m = Math.Max(m, e.Value);
        if (f.HasValue) m = Math.Max(m, f.Value);
        if (g.HasValue) m = Math.Max(m, g.Value);
        if (h.HasValue) m = Math.Max(m, h.Value);
        return m;
    }

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Optional number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static decimal Max(decimal a, decimal b, decimal? c = null)
    {
        var m = Math.Max(a, b);
        if (c.HasValue) m = Math.Max(m, c.Value);
        return m;
    }

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <param name="d">Number 4 to compare.</param>
    /// <param name="e">Optional number 5 to compare.</param>
    /// <param name="f">Optional number 6 to compare.</param>
    /// <param name="g">Optional number 7 to compare.</param>
    /// <param name="h">Optional number 8 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static decimal Max(decimal a, decimal b, decimal c, decimal d, decimal? e = null, decimal? f = null, decimal? g = null, decimal? h = null)
    {
        var m = Math.Max(Math.Max(Math.Max(a, b), c), d);
        if (e.HasValue) m = Math.Max(m, e.Value);
        if (f.HasValue) m = Math.Max(m, f.Value);
        if (g.HasValue) m = Math.Max(m, g.Value);
        if (h.HasValue) m = Math.Max(m, h.Value);
        return m;
    }

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Optional number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static double Max(double a, double b, double? c = null)
    {
        var m = Math.Max(a, b);
        if (c.HasValue) m = Math.Max(m, c.Value);
        return m;
    }

    /// <summary>
    /// Returns the larger of the specific numbers.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <param name="d">Number 4 to compare.</param>
    /// <param name="e">Optional number 5 to compare.</param>
    /// <param name="f">Optional number 6 to compare.</param>
    /// <param name="g">Optional number 7 to compare.</param>
    /// <param name="h">Optional number 8 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static double Max(double a, double b, double c, double d, double? e = null, double? f = null, double? g = null, double? h = null)
    {
        var m = Math.Max(Math.Max(Math.Max(a, b), c), d);
        if (e.HasValue) m = Math.Max(m, e.Value);
        if (f.HasValue) m = Math.Max(m, f.Value);
        if (g.HasValue) m = Math.Max(m, g.Value);
        if (h.HasValue) m = Math.Max(m, h.Value);
        return m;
    }

    /// <summary>
    /// Returns the larger of the specific time spans.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static TimeSpan Max(TimeSpan a, TimeSpan b)
        => a >= b ? a : b;

    /// <summary>
    /// Returns the larger of the specific time spans.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static TimeSpan Max(TimeSpan a, TimeSpan b, TimeSpan c)
        => Max(Max(a, b), c);

    /// <summary>
    /// Returns the larger of the specific dates and times.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static DateTime Max(DateTime a, DateTime b)
        => a >= b ? a : b;

    /// <summary>
    /// Returns the larger of the specific dates and times.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static DateTime Max(DateTime a, DateTime b, DateTime c)
        => Max(Max(a, b), c);

    /// <summary>
    /// Returns the larger of the specific dates and times.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static DateTimeOffset Max(DateTimeOffset a, DateTimeOffset b)
        => a >= b ? a : b;

    /// <summary>
    /// Returns the larger of the specific dates and times.
    /// </summary>
    /// <param name="a">Number 1 to compare.</param>
    /// <param name="b">Number 2 to compare.</param>
    /// <param name="c">Number 3 to compare.</param>
    /// <returns>One of the parameter, whichever is larger.</returns>
    public static DateTimeOffset Max(DateTimeOffset a, DateTimeOffset b, DateTimeOffset c)
        => Max(Max(a, b), c);
}
