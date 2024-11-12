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
    public static List<double> Softmax(IEnumerable<int> numbers)
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

        if (sum == 0d)
        {
            for (var i = 0; i < result.Count; i++)
            {
                result[i] = 0d;
            }
        }
        else
        {
            for (var i = 0; i < result.Count; i++)
            {
                result[i] /= sum;
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by softmax that turns into a collection of the real values that sum to 1.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by softmax.</returns>
    public static double[] Softmax(int[] numbers)
    {
        if (numbers == null) return null;
        var result = new double[numbers.Length];
        var sum = 0d;
        for (var i = 0; i < numbers.Length; i++)
        {
            var exp = Math.Exp(numbers[i]);
            result[i] = exp;
            sum += exp;
        }

        if (sum == 0d)
        {
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = 0d;
            }
        }
        else
        {
            for (var i = 0; i < result.Length; i++)
            {
                result[i] /= sum;
            }
        }

        return result;
    }
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

        if (sum == 0d)
        {
            for (var i = 0; i < result.Count; i++)
            {
                result[i] = 0d;
            }
        }
        else
        {
            for (var i = 0; i < result.Count; i++)
            {
                result[i] /= sum;
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by softmax that turns into a collection of the real values that sum to 1.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by softmax.</returns>
    public static double[] Softmax(double[] numbers)
    {
        if (numbers == null) return null;
        var result = new double[numbers.Length];
        var sum = 0d;
        for (var i = 0; i < numbers.Length; i++)
        {
            var exp = Math.Exp(numbers[i]);
            result[i] = exp;
            sum += exp;
        }

        if (sum == 0d)
        {
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = 0d;
            }
        }
        else
        {
            for (var i = 0; i < result.Length; i++)
            {
                result[i] /= sum;
            }
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

        if (sum == 0f)
        {
            for (var i = 0; i < result.Count; i++)
            {
                result[i] = 0f;
            }
        }
        else
        {
            for (var i = 0; i < result.Count; i++)
            {
                result[i] /= sum;
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by softmax that turns into a collection of the real values that sum to 1.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by softmax.</returns>
    public static float[] Softmax(float[] numbers)
    {
        if (numbers == null) return null;
        var result = new float[numbers.Length];
        var sum = 0f;
        for (var i = 0; i < numbers.Length; i++)
        {
#if NET6_0_OR_GREATER
            var exp = MathF.Exp(numbers[i]);
#else
            var exp = (float)Math.Exp(numbers[i]);
#endif
            result[i] = exp;
            sum += exp;
        }

        if (sum == 0f)
        {
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = 0f;
            }
        }
        else
        {
            for (var i = 0; i < result.Length; i++)
            {
                result[i] /= sum;
            }
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

        if (sum == 0d)
        {
            for (var i = 0; i < result.Count; i++)
            {
                result[i] = 0d;
            }
        }
        else
        {
            for (var i = 0; i < result.Count; i++)
            {
                result[i] /= sum;
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by softmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by softmin.</returns>
    public static double[] Softmin(double[] numbers)
    {
        if (numbers == null) return null;
        var result = new double[numbers.Length];
        var sum = 0d;
        for (var i = 0; i < numbers.Length; i++)
        {
            var exp = Math.Exp(-numbers[i]);
            result[i] = exp;
            sum += exp;
        }

        if (sum == 0d)
        {
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = 0d;
            }
        }
        else
        {
            for (var i = 0; i < result.Length; i++)
            {
                result[i] /= sum;
            }
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

        if (sum == 0f)
        {
            for (var i = 0; i < result.Count; i++)
            {
                result[i] = 0f;
            }
        }
        else
        {
            for (var i = 0; i < result.Count; i++)
            {
                result[i] /= sum;
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by softmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by softmin.</returns>
    public static float[] Softmin(float[] numbers)
    {
        if (numbers == null) return null;
        var result = new float[numbers.Length];
        var sum = 0f;
        for (var i = 0; i < numbers.Length; i++)
        {
#if NET6_0_OR_GREATER
            var exp = MathF.Exp(-numbers[i]);
#else
            var exp = (float)Math.Exp(-numbers[i]);
#endif
            result[i] = exp;
            sum += exp;
        }

        if (sum == 0f)
        {
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = 0f;
            }
        }
        else
        {
            for (var i = 0; i < result.Length; i++)
            {
                result[i] /= sum;
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static List<int> Hardmax(IEnumerable<int> numbers)
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
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static int[] Hardmax(int[] numbers)
    {
        if (numbers == null) return null;
        var result = new int[numbers.Length];
        if (numbers.Length < 1) return result;
        var value = numbers[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < numbers.Length; j++)
        {
            var number = numbers[j];
            if (value > number) continue;
            if (value < number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        for (var j = 0; j < numbers.Length; j++)
        {
            result[j] = indexes.Contains(j) ? 1 : 0;
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static List<double> Hardmax(IEnumerable<int> numbers, bool equalDivisionMax)
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

        var max = (equalDivisionMax && indexes.Count > 0) ? (1d / indexes.Count) : 1d;
        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? max : 0d);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static double[] Hardmax(int[] numbers, bool equalDivisionMax)
    {
        if (numbers == null) return null;
        var result = new double[numbers.Length];
        if (numbers.Length < 1) return result;
        var value = numbers[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < numbers.Length; j++)
        {
            var number = numbers[j];
            if (value > number) continue;
            if (value < number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        var max = (equalDivisionMax && indexes.Count > 0) ? (1d / indexes.Count) : 1d;
        for (var j = 0; j < numbers.Length; j++)
        {
            result[j] = indexes.Contains(j) ? max : 0d;
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static List<double> Hardmax(IEnumerable<double> numbers, bool equalDivisionMax = false)
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

        var max = (equalDivisionMax && indexes.Count > 0) ? (1d / indexes.Count) : 1d;
        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? max : 0d);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static double[] Hardmax(double[] numbers, bool equalDivisionMax = false)
    {
        if (numbers == null) return null;
        var result = new double[numbers.Length];
        if (numbers.Length < 1) return result;
        var value = numbers[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < numbers.Length; j++)
        {
            var number = numbers[j];
            if (value > number) continue;
            if (value < number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        var max = (equalDivisionMax && indexes.Count > 0) ? (1d / indexes.Count) : 1d;
        for (var j = 0; j < numbers.Length; j++)
        {
            result[j] = indexes.Contains(j) ? max : 0d;
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static List<float> Hardmax(IEnumerable<float> numbers, bool equalDivisionMax = false)
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

        var max = (equalDivisionMax && indexes.Count > 0) ? (1f / indexes.Count) : 1f;
        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? max : 0f);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmax function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmax.</returns>
    public static float[] Hardmax(float[] numbers, bool equalDivisionMax = false)
    {
        if (numbers == null) return null;
        var result = new float[numbers.Length];
        if (numbers.Length < 1) return result;
        var value = numbers[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < numbers.Length; j++)
        {
            var number = numbers[j];
            if (value > number) continue;
            if (value < number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        var max = (equalDivisionMax && indexes.Count > 0) ? (1f / indexes.Count) : 1f;
        for (var j = 0; j < numbers.Length; j++)
        {
            result[j] = indexes.Contains(j) ? max : 0f;
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static List<int> Hardmin(IEnumerable<int> numbers)
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
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static int[] Hardmin(int[] numbers)
    {
        if (numbers == null) return null;
        var result = new int[numbers.Length];
        if (numbers.Length < 1) return result;
        var value = numbers[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < numbers.Length; j++)
        {
            var number = numbers[j];
            if (value < number) continue;
            if (value > number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        for (var j = 0; j < numbers.Length; j++)
        {
            result[j] = indexes.Contains(j) ? 1 : 0;
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static List<double> Hardmin(IEnumerable<int> numbers, bool equalDivisionMax)
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

        var max = (equalDivisionMax && indexes.Count > 0) ? (1d / indexes.Count) : 1d;
        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? max : 0d);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static double[] Hardmin(int[] numbers, bool equalDivisionMax)
    {
        if (numbers == null) return null;
        var result = new double[numbers.Length];
        if (numbers.Length < 1) return result;
        var value = numbers[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < numbers.Length; j++)
        {
            var number = numbers[j];
            if (value < number) continue;
            if (value > number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        var max = (equalDivisionMax && indexes.Count > 0) ? (1d / indexes.Count) : 1d;
        for (var j = 0; j < numbers.Length; j++)
        {
            result[j] = indexes.Contains(j) ? max : 0d;
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static List<double> Hardmin(IEnumerable<double> numbers, bool equalDivisionMax = false)
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

        var max = (equalDivisionMax && indexes.Count > 0) ? (1d / indexes.Count) : 1d;
        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? max : 0d);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static double[] Hardmin(double[] numbers, bool equalDivisionMax = false)
    {
        if (numbers == null) return null;
        var result = new double[numbers.Length];
        if (numbers.Length < 1) return result;
        var value = numbers[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < numbers.Length; j++)
        {
            var number = numbers[j];
            if (value < number) continue;
            if (value > number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        var max = (equalDivisionMax && indexes.Count > 0) ? (1d / indexes.Count) : 1d;
        for (var j = 0; j < numbers.Length; j++)
        {
            result[j] = indexes.Contains(j) ? max : 0d;
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static List<float> Hardmin(IEnumerable<float> numbers, bool equalDivisionMax = false)
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

        var max = (equalDivisionMax && indexes.Count > 0) ? (1f / indexes.Count) : 1f;
        for (var j = 0; j < col.Count; j++)
        {
            result.Add(indexes.Contains(j) ? max : 0f);
        }

        return result;
    }

    /// <summary>
    /// Converts a set of number to another calculated by hardmin function.
    /// </summary>
    /// <param name="numbers">The input numbers.</param>
    /// <param name="equalDivisionMax">true if the maximum value will be equal division by all maximum elements but not one (1); otherwise, false.</param>
    /// <returns>The output numbers calculated by hardmin.</returns>
    public static float[] Hardmin(float[] numbers, bool equalDivisionMax = false)
    {
        if (numbers == null) return null;
        var result = new float[numbers.Length];
        if (numbers.Length < 1) return result;
        var value = numbers[0];
        var indexes = new List<int>
        {
            0
        };
        for (var j = 1; j < numbers.Length; j++)
        {
            var number = numbers[j];
            if (value < number) continue;
            if (value > number) indexes.Clear();
            indexes.Add(j);
            value = number;
        }

        var max = (equalDivisionMax && indexes.Count > 0) ? (1f / indexes.Count) : 1f;
        for (var j = 0; j < numbers.Length; j++)
        {
            result[j] = indexes.Contains(j) ? max : 0f;
        }

        return result;
    }

    /// <summary>
    /// Gets the minimum number of the specific collection.
    /// </summary>
    /// <typeparam name="T">The type of element.</typeparam>
    /// <param name="col">The input numbers.</param>
    /// <returns>The minimum number found.</returns>
    /// <exception cref="ArgumentNullException">col is null.</exception>
    /// <exception cref="InvalidOperationException">col has no element.</exception>
    public static T Min<T>(IEnumerable<T> col) where T : IComparable
    {
        if (col is null) throw new ArgumentNullException(nameof(col), "col was null.");
        var result = Min(col, default, out var i);
        if (i < 0) throw new InvalidOperationException("col should contain one or more elements but not empty.", new ArgumentException("col was empty.", nameof(col)));
        return result;
    }

    /// <summary>
    /// Gets the minimum number of the specific collection.
    /// </summary>
    /// <typeparam name="T">The type of element.</typeparam>
    /// <param name="col">The input numbers.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The maximum number found; or returns defaultValue, if col is null or empty..</returns>
    public static T Min<T>(IEnumerable<T> col, T defaultValue) where T : struct, IComparable
        => Min(col, defaultValue, out _);

    /// <summary>
    /// Gets the minimum number of the specific collection.
    /// </summary>
    /// <typeparam name="T">The type of element.</typeparam>
    /// <param name="col">The input numbers.</param>
    /// <param name="index">The first index of the minimum number.</param>
    /// <returns>The minimum number found.</returns>
    public static T Min<T>(IEnumerable<T> col, out int index) where T : IComparable
        => Min(col, default, out index);

    /// <summary>
    /// Gets the minimum number of the specific collection.
    /// </summary>
    /// <typeparam name="T">The type of element.</typeparam>
    /// <param name="col">The input numbers.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="index">The first index of the minimum number.</param>
    /// <returns>The minimum number found.</returns>
    public static T Min<T>(IEnumerable<T> col, T defaultValue, out int index) where T : IComparable
    {
        index = -1;
        if (col is null) return defaultValue;
        T min = default;
        var has = false;
        var i = -1;
        foreach (var item in col)
        {
            i++;
            if (has && item.CompareTo(min) >= 0) continue;
            has = true;
            min = item;
            index = i;
        }

        return min ?? defaultValue;
    }

    /// <summary>
    /// Gets the maximum number of the specific collection.
    /// </summary>
    /// <typeparam name="T">The type of element.</typeparam>
    /// <param name="col">The input numbers.</param>
    /// <returns>The maximum number found.</returns>
    /// <exception cref="ArgumentNullException">col is null.</exception>
    /// <exception cref="InvalidOperationException">col has no element.</exception>
    public static T Max<T>(IEnumerable<T> col) where T : IComparable
    {
        if (col is null) throw new ArgumentNullException(nameof(col), "col was null.");
        var result = Max(col, default, out var i);
        if (i < 0) throw new InvalidOperationException("col should contain one or more elements but not empty.", new ArgumentException("col was empty.", nameof(col)));
        return result;
    }

    /// <summary>
    /// Gets the maximum number of the specific collection.
    /// </summary>
    /// <typeparam name="T">The type of element.</typeparam>
    /// <param name="col">The input numbers.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The maximum number found; or returns defaultValue, if col is null or empty..</returns>
    public static T Max<T>(IEnumerable<T> col, T defaultValue) where T : IComparable
        => Max(col, default, out _);

    /// <summary>
    /// Gets the maximum number of the specific collection.
    /// </summary>
    /// <typeparam name="T">The type of element.</typeparam>
    /// <param name="col">The input numbers.</param>
    /// <param name="index">The first index of the maximum number.</param>
    /// <returns>The maximum number found.</returns>
    public static T Max<T>(IEnumerable<T> col, out int index) where T : IComparable
        => Max(col, default, out index);

    /// <summary>
    /// Gets the maximum number of the specific collection.
    /// </summary>
    /// <typeparam name="T">The type of element.</typeparam>
    /// <param name="col">The input numbers.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="index">The first index of the maximum number.</param>
    /// <returns>The maximum number found.</returns>
    public static T Max<T>(IEnumerable<T> col, T defaultValue, out int index) where T : IComparable
    {
        index = -1;
        if (col is null) return defaultValue;
        T max = default;
        var has = false;
        var i = -1;
        foreach (var item in col)
        {
            i++;
            if (has && item.CompareTo(max) <= 0) continue;
            has = true;
            max = item;
            index = i;
        }

        return max ?? defaultValue;
    }
}
