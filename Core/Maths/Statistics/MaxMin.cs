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
        if (numbers == null) return null;
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
        if (numbers == null) return null;
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
        if (numbers == null) return null;
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
        if (numbers == null) return null;
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

    /// <summary>
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static List<int> HardMax(IEnumerable<int> numbers)
    {
        var col = numbers?.ToList();
        if (col == null) return null;
        var result = new List<int>();
        if (col.Count < 1) return result;
        var value = col[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value < number) continue;
            if (value > number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? 1 : 0);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static List<double> HardMax(IEnumerable<double> numbers)
    {
        var col = numbers?.ToList();
        if (col == null) return null;
        var result = new List<double>();
        if (col.Count < 1) return result;
        var value = col[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value < number) continue;
            if (value > number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? 1 : 0);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static List<float> HardMax(IEnumerable<float> numbers)
    {
        var col = numbers?.ToList();
        if (col == null) return null;
        var result = new List<float>();
        if (col.Count < 1) return result;
        var value = col[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value < number) continue;
            if (value > number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? 1 : 0);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static List<int> HardMin(IEnumerable<int> numbers)
    {
        var col = numbers?.ToList();
        if (col == null) return null;
        var result = new List<int>();
        if (col.Count < 1) return result;
        var value = col[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value > number) continue;
            if (value < number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? 1 : 0);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static List<double> HardMin(IEnumerable<double> numbers)
    {
        var col = numbers?.ToList();
        if (col == null) return null;
        var result = new List<double>();
        if (col.Count < 1) return result;
        var value = col[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value > number) continue;
            if (value < number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? 1 : 0);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static List<float> HardMin(IEnumerable<float> numbers)
    {
        var col = numbers?.ToList();
        if (col == null) return null;
        var result = new List<float>();
        if (col.Count < 1) return result;
        var value = col[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value > number) continue;
            if (value < number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? 1 : 0);
        }

        return result;
    }

    /// <summary>
    /// Gets the first index of maximum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="value">The maximum number output.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMax(IEnumerable<int> numbers, out int value)
    {
        var col = numbers?.ToList();
        if (col == null || col.Count < 1)
        {
            value = default;
            return -1;
        }

        value = col[0];
        var i = 0;
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value <= number) continue;
            value = number;
            i = j;
        }

        return i;
    }

    /// <summary>
    /// Gets the first index of maximum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMax(IEnumerable<int> numbers)
        => IndexOfMax(numbers, out _);

    /// <summary>
    /// Gets the first index of maximum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="value">The maximum number output.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMax(IEnumerable<double> numbers, out double value)
    {
        var col = numbers?.ToList();
        if (col == null || col.Count < 1)
        {
            value = default;
            return -1;
        }

        value = col[0];
        var i = 0;
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value <= number) continue;
            value = number;
            i = j;
        }

        return i;
    }

    /// <summary>
    /// Gets the first index of maximum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMax(IEnumerable<double> numbers)
        => IndexOfMax(numbers, out _);

    /// <summary>
    /// Gets the first index of maximum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="value">The maximum number output.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMax(IEnumerable<float> numbers, out float value)
    {
        var col = numbers?.ToList();
        if (col == null || col.Count < 1)
        {
            value = default;
            return -1;
        }

        value = col[0];
        var i = 0;
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value <= number) continue;
            value = number;
            i = j;
        }

        return i;
    }

    /// <summary>
    /// Gets the first index of maximum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMax(IEnumerable<float> numbers)
        => IndexOfMax(numbers, out _);

    /// <summary>
    /// Gets the first index of minimum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="value">The minimum number output.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMin(IEnumerable<int> numbers, out int value)
    {
        var col = numbers?.ToList();
        if (col == null || col.Count < 1)
        {
            value = default;
            return -1;
        }

        value = col[0];
        var i = 0;
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value >= number) continue;
            value = number;
            i = j;
        }

        return i;
    }

    /// <summary>
    /// Gets the first index of minimum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMin(IEnumerable<int> numbers)
        => IndexOfMin(numbers, out _);

    /// <summary>
    /// Gets the first index of minimum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="value">The minimum number output.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMin(IEnumerable<double> numbers, out double value)
    {
        var col = numbers?.ToList();
        if (col == null || col.Count < 1)
        {
            value = default;
            return -1;
        }

        value = col[0];
        var i = 0;
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value >= number) continue;
            value = number;
            i = j;
        }

        return i;
    }

    /// <summary>
    /// Gets the first index of minimum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMin(IEnumerable<double> numbers)
        => IndexOfMin(numbers, out _);

    /// <summary>
    /// Gets the first index of minimum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="value">The minimum number output.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMin(IEnumerable<float> numbers, out float value)
    {
        var col = numbers?.ToList();
        if (col == null || col.Count < 1)
        {
            value = default;
            return -1;
        }

        value = col[0];
        var i = 0;
        for (var j = 1; j < col.Count; j++)
        {
            var number = col[j];
            if (value >= number) continue;
            value = number;
            i = j;
        }

        return i;
    }

    /// <summary>
    /// Gets the first index of minimum number.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The zero-based index.</returns>
    public static int IndexOfMin(IEnumerable<float> numbers)
        => IndexOfMin(numbers, out _);
}
