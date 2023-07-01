using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;

public static partial class StatisticalMethod
{
    /// <summary>
    /// Converts a set of number to another calculated by softmax that turns into a collection of the real values that sum to 1.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by softmax.</returns>
    public static List<double> Softmax(IEnumerable<double> numbers)
    {
        var result = new List<double>();
        var sum = 0d;
        foreach (var number in numbers)
        {
            var exp = Math.Exp(number);
            result.Add(exp);
            sum += exp;
        }

        for (var i = 0; i < result.Count; i++)
        {
            result[i] /= sum;
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by softmax that turns into a collection of the real values that sum to 1.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by softmax.</returns>
    public static List<float> Softmax(IEnumerable<float> numbers)
    {
        var result = new List<float>();
        var sum = 0f;
        foreach (var number in numbers)
        {
#if NET6_0_OR_GREATER
            var exp = MathF.Exp(number);
#else
            var exp = (float)Math.Exp(number);
#endif
            result.Add(exp);
            sum += exp;
        }

        for (var i = 0; i < result.Count; i++)
        {
            result[i] /= sum;
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by softmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by softmin.</returns>
    public static List<double> Softmin(IEnumerable<double> numbers)
    {
        var result = new List<double>();
        var sum = 0d;
        foreach (var number in numbers)
        {
            var exp = Math.Exp(-number);
            result.Add(exp);
            sum += exp;
        }

        for (var i = 0; i < result.Count; i++)
        {
            result[i] /= sum;
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by softmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by softmin.</returns>
    public static List<float> Softmin(IEnumerable<float> numbers)
    {
        var result = new List<float>();
        var sum = 0f;
        foreach (var number in numbers)
        {
#if NET6_0_OR_GREATER
            var exp = MathF.Exp(-number);
#else
            var exp = (float)Math.Exp(-number);
#endif
            result.Add(exp);
            sum += exp;
        }

        for (var i = 0; i < result.Count; i++)
        {
            result[i] /= sum;
        }

        return result;
    }
}
