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
            if (value >= number) continue;
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
            if (value >= number) continue;
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
            if (value >= number) continue;
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
            if (value <= number) continue;
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
            if (value <= number) continue;
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
            if (value <= number) continue;
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
