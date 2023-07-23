using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;

public static partial class Arithmetic
{
    /// <summary>
    /// Rounds a floating-point value to the nearest integral value, and rounds midpoint values to the nearest even number.
    /// </summary>
    /// <param name="value">A floating-point number to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int Round(double value)
        => (int)Math.Round(value);

    /// <summary>
    /// Rounds a floating-point value to the nearest integral value, and rounds midpoint values to the nearest even number.
    /// </summary>
    /// <param name="value">A floating-point number to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int Round(float value)
        => (int)Math.Round(value);

    /// <summary>
    /// Rounds a floating-point value to the nearest integral value, and rounds midpoint values to the nearest even number.
    /// </summary>
    /// <param name="value">A floating-point number to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int Round(decimal value)
        => (int)Math.Round(value);

    /// <summary>
    /// Rounds a floating-point value to the nearest integral value, and rounds midpoint values to the nearest even number.
    /// </summary>
    /// <param name="value">A floating-point number to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long RoundLong(double value)
        => (long)Math.Round(value);

    /// <summary>
    /// Rounds a floating-point value to the nearest integral value, and rounds midpoint values to the nearest even number.
    /// </summary>
    /// <param name="value">A floating-point number to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long RoundLong(float value)
        => (long)Math.Round(value);

    /// <summary>
    /// Rounds a floating-point value to the nearest integral value, and rounds midpoint values to the nearest even number.
    /// </summary>
    /// <param name="value">A floating-point number to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long RoundLong(decimal value)
        => (long)Math.Round(value);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<int> Round(IEnumerable<double> value)
        => Round(value, Round);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<int> Round(IEnumerable<float> value)
        => Round(value, Round);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<int> Round(IEnumerable<decimal> value)
        => Round(value, Round);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int[] Round(double[] value)
        => Round(value, Round);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int[] Round(float[] value)
        => Round(value, Round);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int[] Round(decimal[] value)
        => Round(value, Round);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<long> RoundLong(IEnumerable<double> value)
        => Round(value, RoundLong);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<long> RoundLong(IEnumerable<float> value)
        => Round(value, RoundLong);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<long> RoundLong(IEnumerable<decimal> value)
        => Round(value, RoundLong);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long[] RoundLong(double[] value)
        => Round(value, RoundLong);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long[] RoundLong(float[] value)
        => Round(value, RoundLong);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long[] RoundLong(decimal[] value)
        => Round(value, RoundLong);

    /// <summary>
    /// Returns the largest integral value less than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int Floor(double value)
        => (int)Math.Floor(value);

    /// <summary>
    /// Returns the largest integral value less than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int Floor(float value)
        => (int)Math.Floor(value);

    /// <summary>
    /// Returns the largest integral value less than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int Floor(decimal value)
        => (int)Math.Floor(value);

    /// <summary>
    /// Returns the largest integral value less than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long FloorLong(double value)
        => (long)Math.Floor(value);

    /// <summary>
    /// Returns the largest integral value less than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number to be rounded.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long FloorLong(float value)
        => (long)Math.Floor(value);

    /// <summary>
    /// Returns the largest integral value less than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long FloorLong(decimal value)
        => (long)Math.Floor(value);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<int> Floor(IEnumerable<double> value)
        => Round(value, Floor);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<int> Floor(IEnumerable<float> value)
        => Round(value, Floor);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<int> Floor(IEnumerable<decimal> value)
        => Round(value, Floor);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int[] Floor(double[] value)
        => Round(value, Floor);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int[] Floor(float[] value)
        => Round(value, Floor);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int[] Floor(decimal[] value)
        => Round(value, Floor);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<long> FloorLong(IEnumerable<double> value)
        => Round(value, FloorLong);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<long> FloorLong(IEnumerable<float> value)
        => Round(value, FloorLong);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<long> FloorLong(IEnumerable<decimal> value)
        => Round(value, FloorLong);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long[] FloorLong(double[] value)
        => Round(value, FloorLong);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long[] FloorLong(float[] value)
        => Round(value, FloorLong);

    /// <summary>
    /// Floors a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers.</param>
    /// <returns>The largest integral value less than or equal to input value.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long[] FloorLong(decimal[] value)
        => Round(value, FloorLong);

    /// <summary>
    /// Returns the smallest integral value that is greater than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int Ceiling(double value)
        => (int)Math.Ceiling(value);

    /// <summary>
    /// Returns the smallest integral value that is greater than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int Ceiling(float value)
        => (int)Math.Ceiling(value);

    /// <summary>
    /// Returns the smallest integral value that is greater than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int Ceiling(decimal value)
        => (int)Math.Ceiling(value);

    /// <summary>
    /// Returns the smallest integral value that is greater than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long CeilingLong(double value)
        => (long)Math.Ceiling(value);

    /// <summary>
    /// Returns the smallest integral value that is greater than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long CeilingLong(float value)
        => (long)Math.Ceiling(value);

    /// <summary>
    /// Returns the smallest integral value that is greater than or equal to the specified floating-point number.
    /// </summary>
    /// <param name="value">A floating-point number.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">The value is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long CeilingLong(decimal value)
        => (long)Math.Ceiling(value);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<int> Ceiling(IEnumerable<double> value)
        => Round(value, Ceiling);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<int> Ceiling(IEnumerable<float> value)
        => Round(value, Ceiling);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<int> Ceiling(IEnumerable<decimal> value)
        => Round(value, Ceiling);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int[] Ceiling(double[] value)
        => Round(value, Ceiling);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int[] Ceiling(float[] value)
        => Round(value, Ceiling);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static int[] Ceiling(decimal[] value)
        => Round(value, Ceiling);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<long> CeilingLong(IEnumerable<double> value)
        => Round(value, CeilingLong);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<long> CeilingLong(IEnumerable<float> value)
        => Round(value, CeilingLong);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static IEnumerable<long> CeilingLong(IEnumerable<decimal> value)
        => Round(value, CeilingLong);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long[] CeilingLong(double[] value)
        => Round(value, CeilingLong);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long[] CeilingLong(float[] value)
        => Round(value, CeilingLong);

    /// <summary>
    /// Ceilings a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <returns>The smallest integral value that is greater than or equal to input value.en and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    public static long[] CeilingLong(decimal[] value)
        => Round(value, CeilingLong);

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <param name="round">The round function.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    private static IEnumerable<T2> Round<T1, T2>(IEnumerable<T1> value, Func<T1, T2> round)
    {
        if (value == null) yield break;
        foreach (var item in value)
        {
            yield return round(item);
        }
    }

    /// <summary>
    /// Rounds a collection of floating-point value to the nearest integral values, and rounds midpoint values to the nearest even numbers.
    /// </summary>
    /// <param name="value">A collection with floating-point numbers to be rounded.</param>
    /// <param name="round">The round function.</param>
    /// <returns>The integer nearest input value. If the fractional component of input value is halfway between two integers, one of which is even and the other odd, then the even number is returned.</returns>
    /// <exception cref="OverflowException">One of the value at least is less the minimum number supported or greater than the maximum number supported.</exception>
    private static T2[] Round<T1, T2>(T1[] value, Func<T1, T2> round)
    {
        if (value == null) return null;
        var arr = new T2[value.Length];
        for (var i = 0; i < value.Length; i++)
        {
            arr[i] = round(value[i]);
        }

        return arr;
    }
}
