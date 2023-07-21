using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;


/// <summary>
/// The operators for number collection.
/// </summary>
public enum ComparerCollectionOperators : byte
{
    /// <summary>
    /// Default value.
    /// </summary>
    None = 0,

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
}

/// <summary>
/// The operations for data collection.
/// </summary>
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
    public static T Merge<T>(ComparerCollectionOperators op, IEnumerable<T> input, T defaultValue = default) where T : IComparable<T>
    {
        if (input == null) return defaultValue;
        if (input is bool[] arr)
        {
            if (arr.Length < 1) return defaultValue;
        }
        else if (input is List<T> col)
        {
            if (col.Count < 1) return defaultValue;
        }
        else
        {
            col = input?.ToList();
            input = col;
            if (col.Count < 1) return defaultValue;
        }

        return op switch
        {
            ComparerCollectionOperators.None => defaultValue,
            ComparerCollectionOperators.Min => input.Min(),
            ComparerCollectionOperators.Max => input.Max(),
            ComparerCollectionOperators.First => input.First(),
            ComparerCollectionOperators.Last => input.Last(),
            _ => throw NotSupported(op),
        };
    }
    private static NotSupportedException NotSupported<T>(T op)
        => new($"The operation {op} is not supported.", new ArgumentException("op should be a valid one.", nameof(op)));
}
