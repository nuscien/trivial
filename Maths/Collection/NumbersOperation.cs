using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;

/// <summary>
/// The operators for number collection.
/// </summary>
public enum NumberCollectionOperators : byte
{
    /// <summary>
    /// Always returns zero (0).
    /// </summary>
    Zero = 0,

    /// <summary>
    /// The smallest number.
    /// </summary>
    Min = 1,

    /// <summary>
    /// The largest number.
    /// </summary>
    Max = 2,

    /// <summary>
    /// The first number.
    /// </summary>
    First = 3,

    /// <summary>
    /// The last number.
    /// </summary>
    Last = 4,

    /// <summary>
    /// Sum of the numbers.
    /// </summary>
    Sum = 5,

    /// <summary>
    /// Average of the numbers.
    /// </summary>
    Average = 6,
}

public static partial class CollectionOperation
{
    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="defaultValue">The default value for empty.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static int Merge(NumberCollectionOperators op, IEnumerable<int> input, int defaultValue)
    {
        var col = input?.ToList();
        return Merge(op, col, col == null || col.Count < 1) ?? defaultValue;
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="defaultValue">The default value for empty.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static double Merge(NumberCollectionOperators op, IEnumerable<int> input, double defaultValue)
    {
        var col = input?.ToList();
        var empty = col == null || col.Count < 1;
        return op switch
        {
            NumberCollectionOperators.Zero => 0,
            NumberCollectionOperators.Sum => empty ? defaultValue : col.Sum(),
            NumberCollectionOperators.Average => empty ? defaultValue : col.Average(),
            NumberCollectionOperators.Min => empty ? defaultValue : col.Min(),
            NumberCollectionOperators.Max => empty ? defaultValue : col.Max(),
            NumberCollectionOperators.First => empty ? defaultValue : col.First(),
            NumberCollectionOperators.Last => empty ? defaultValue : col.Last(),
            _ => throw NotSupported(op),
        };
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static int? Merge(NumberCollectionOperators op, IEnumerable<int> input)
    {
        var col = input?.ToList();
        return Merge(op, col, col == null || col.Count < 1);
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static int? Merge(NumberCollectionOperators op, params int[] input)
        => Merge(op, input, input == null || input.Length < 1);

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="defaultValue">The default value for empty.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static long Merge(NumberCollectionOperators op, IEnumerable<long> input, long defaultValue)
    {
        var col = input?.ToList();
        return Merge(op, col, col == null || col.Count < 1) ?? defaultValue;
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static long? Merge(NumberCollectionOperators op, IEnumerable<long> input)
    {
        var col = input?.ToList();
        return Merge(op, col, col == null || col.Count < 1);
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static long? Merge(NumberCollectionOperators op, params long[] input)
        => Merge(op, input, input == null || input.Length < 1);

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="defaultValue">The default value for empty.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static float Merge(NumberCollectionOperators op, IEnumerable<float> input, float defaultValue)
    {
        var col = input?.ToList();
        return Merge(op, col, col == null || col.Count < 1) ?? defaultValue;
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static float? Merge(NumberCollectionOperators op, IEnumerable<float> input)
    {
        var col = input?.ToList();
        return Merge(op, col, col == null || col.Count < 1);
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static float? Merge(NumberCollectionOperators op, params float[] input)
        => Merge(op, input, input == null || input.Length < 1);

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="defaultValue">The default value for empty.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static double Merge(NumberCollectionOperators op, IEnumerable<double> input, double defaultValue)
    {
        var col = input?.ToList();
        return Merge(op, col, col == null || col.Count < 1) ?? defaultValue;
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static double? Merge(NumberCollectionOperators op, IEnumerable<double> input)
    {
        var col = input?.ToList();
        return Merge(op, col, col == null || col.Count < 1);
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static double? Merge(NumberCollectionOperators op, params double[] input)
        => Merge(op, input, input == null || input.Length < 1);

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="defaultValue">The default value for empty.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static decimal Merge(NumberCollectionOperators op, IEnumerable<decimal> input, decimal defaultValue)
    {
        var col = input?.ToList();
        return Merge(op, col, col == null || col.Count < 1) ?? defaultValue;
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static decimal? Merge(NumberCollectionOperators op, IEnumerable<decimal> input)
    {
        var col = input?.ToList();
        return Merge(op, col, col == null || col.Count < 1);
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static decimal? Merge(NumberCollectionOperators op, params decimal[] input)
        => Merge(op, input, input == null || input.Length < 1);

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="empty">true if the input collection is empty or null; otherwise, false.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    private static int? Merge(NumberCollectionOperators op, IEnumerable<int> input, bool empty)
        => op switch
        {
            NumberCollectionOperators.Zero => 0,
            NumberCollectionOperators.Sum => empty ? null : input.Sum(),
            NumberCollectionOperators.Average => empty ? null : (int)Math.Round(input.Average()),
            NumberCollectionOperators.Min => empty ? null : input.Min(),
            NumberCollectionOperators.Max => empty ? null : input.Max(),
            NumberCollectionOperators.First => empty ? null : input.First(),
            NumberCollectionOperators.Last => empty ? null : input.Last(),
            _ => throw NotSupported(op),
        };

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="empty">true if the input collection is empty or null; otherwise, false.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    private static long? Merge(NumberCollectionOperators op, IEnumerable<long> input, bool empty)
        => op switch
        {
            NumberCollectionOperators.Zero => 0,
            NumberCollectionOperators.Sum => empty ? null : input.Sum(),
            NumberCollectionOperators.Average => empty ? null : (long)Math.Round(input.Average()),
            NumberCollectionOperators.Min => empty ? null : input.Min(),
            NumberCollectionOperators.Max => empty ? null : input.Max(),
            NumberCollectionOperators.First => empty ? null : input.First(),
            NumberCollectionOperators.Last => empty ? null : input.Last(),
            _ => throw NotSupported(op),
        };

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="empty">true if the input collection is empty or null; otherwise, false.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    private static float? Merge(NumberCollectionOperators op, IEnumerable<float> input, bool empty)
        => op switch
        {
            NumberCollectionOperators.Zero => 0,
            NumberCollectionOperators.Sum => empty ? null : input.Sum(),
            NumberCollectionOperators.Average => empty ? null : input.Average(),
            NumberCollectionOperators.Min => empty ? null : input.Min(),
            NumberCollectionOperators.Max => empty ? null : input.Max(),
            NumberCollectionOperators.First => empty ? null : input.First(),
            NumberCollectionOperators.Last => empty ? null : input.Last(),
            _ => throw NotSupported(op),
        };

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="empty">true if the input collection is empty or null; otherwise, false.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    private static double? Merge(NumberCollectionOperators op, IEnumerable<double> input, bool empty)
        => op switch
        {
            NumberCollectionOperators.Zero => 0,
            NumberCollectionOperators.Sum => empty ? null : input.Sum(),
            NumberCollectionOperators.Average => empty ? null : input.Average(),
            NumberCollectionOperators.Min => empty ? null : input.Min(),
            NumberCollectionOperators.Max => empty ? null : input.Max(),
            NumberCollectionOperators.First => empty ? null : input.First(),
            NumberCollectionOperators.Last => empty ? null : input.Last(),
            _ => throw NotSupported(op),
        };

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="empty">true if the input collection is empty or null; otherwise, false.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    private static decimal? Merge(NumberCollectionOperators op, IEnumerable<decimal> input, bool empty)
        => op switch
        {
            NumberCollectionOperators.Zero => 0,
            NumberCollectionOperators.Sum => empty ? null : input.Sum(),
            NumberCollectionOperators.Average => empty ? null : input.Average(),
            NumberCollectionOperators.Min => empty ? null : input.Min(),
            NumberCollectionOperators.Max => empty ? null : input.Max(),
            NumberCollectionOperators.First => empty ? null : input.First(),
            NumberCollectionOperators.Last => empty ? null : input.Last(),
            _ => throw NotSupported(op),
        };
}
