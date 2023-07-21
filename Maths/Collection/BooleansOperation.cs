using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths;

/// <summary>
/// The operators for boolean collection.
/// </summary>
public enum BooleanCollectionOperators : byte
{
    /// <summary>
    /// Always returns false.
    /// </summary>
    False = 0,

    /// <summary>
    /// Always returns true.
    /// </summary>
    True = 1,

    /// <summary>
    /// And operator.
    /// That means to return true if all are true; otherwise, false.
    /// </summary>
    And = 2,

    /// <summary>
    /// Or operator.
    /// That means to return true if one or more are true; otherwise, false. 
    /// </summary>
    Or = 3,

    /// <summary>
    /// And operator first, not operator then.
    /// </summary>
    Nand = 4,

    /// <summary>
    /// Or operator first, not operator then.
    /// </summary>
    Nor = 5,

    /// <summary>
    /// As the most ones, including all the same.
    /// </summary>
    Most = 6,

    /// <summary>
    /// As the most ones, including all the same; or as the first one, if half and half (including all are empty).
    /// </summary>
    MostOrFirst = 7,

    /// <summary>
    /// As the most ones, including all the same; or as the last one, if half and half (including all are empty).
    /// </summary>
    MostOrLast = 8,

    /// <summary>
    /// As the least one, or none in items.
    /// </summary>
    Least = 9,

    /// <summary>
    /// As the least ones (or none); or as the first one, if half and half (including all are empty).
    /// </summary>
    LeastOrFirst = 10,

    /// <summary>
    /// As the least ones (or none); or as the last one, if half and half (including all are empty).
    /// </summary>
    LeastOrLast = 11,

    /// <summary>
    /// All are same.
    /// </summary>
    Same = 12,

    /// <summary>
    /// Different.
    /// </summary>
    Different = 13,
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
    public static bool Merge(CriteriaBooleanOperator op, IEnumerable<bool> input, bool defaultValue)
    {
        var col = input?.ToList();
        return Merge(op, col) ?? defaultValue;
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static bool? Merge(CriteriaBooleanOperator op, IEnumerable<bool> input)
    {
        switch (op)
        {
            case CriteriaBooleanOperator.And:
                foreach (var item in input)
                {
                    if (!item) return false;
                }

                return true;
            case CriteriaBooleanOperator.Or:
                foreach (var item in input)
                {
                    if (item) return true;
                }

                return false;
            default:
                throw NotSupported(op);
        }
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static bool? Merge(CriteriaBooleanOperator op, params bool[] input)
        => Merge(op, input as IEnumerable<bool>);

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="defaultValue">The default value for empty.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static bool Merge(BooleanCollectionOperators op, IEnumerable<bool> input, bool defaultValue)
    {
        var col = input?.ToList();
        return Merge(op, col) ?? defaultValue;
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static bool? Merge(BooleanCollectionOperators op, IEnumerable<bool> input)
    {
        if (input == null) return Merge(op);
        if (input is bool[] arr)
        {
            if (arr.Length < 1) return Merge(op);
        }
        else if (input is List<bool> col)
        {
            if (col.Count < 1) return Merge(op);
        }
        else
        {
            col = input?.ToList();
            input = col;
            if (col.Count < 1) return Merge(op);
        }

        switch (op)
        {
            case BooleanCollectionOperators.False:
                return false;
            case BooleanCollectionOperators.True:
                return true;
            case BooleanCollectionOperators.And:
                foreach (var item in input)
                {
                    if (!item) return false;
                }

                return true;
            case BooleanCollectionOperators.Or:
                foreach (var item in input)
                {
                    if (item) return true;
                }

                return false;
            case BooleanCollectionOperators.Nand:
                foreach (var item in input)
                {
                    if (!item) return true;
                }

                return false;
            case BooleanCollectionOperators.Nor:
                foreach (var item in input)
                {
                    if (item) return false;
                }

                return true;
            case BooleanCollectionOperators.Most:
                {
                    var t = input.Count(IsTrue);
                    var f = input.Count(IsFalse);
                    return t == f ? null : t > f;
                }
            case BooleanCollectionOperators.MostOrFirst:
                {
                    var t = input.Count(IsTrue);
                    var f = input.Count(IsFalse);
                    return t == f ? input.First() : t > f;
                }
            case BooleanCollectionOperators.MostOrLast:
                {
                    var t = input.Count(IsTrue);
                    var f = input.Count(IsFalse);
                    return t == f ? input.Last() : t > f;
                }
            case BooleanCollectionOperators.Least:
                {
                    var t = input.Count(IsTrue);
                    var f = input.Count(IsFalse);
                    return t == f ? null : t < f;
                }
            case BooleanCollectionOperators.LeastOrFirst:
                {
                    var t = input.Count(IsTrue);
                    var f = input.Count(IsFalse);
                    return t == f ? input.First() : t < f;
                }
            case BooleanCollectionOperators.LeastOrLast:
                {
                    var t = input.Count(IsTrue);
                    var f = input.Count(IsFalse);
                    return t == f ? input.Last() : t < f;
                }
            case BooleanCollectionOperators.Same:
                {
                    var t = input.Count(IsTrue);
                    var f = input.Count(IsFalse);
                    return t == 0 || f == 0;
                }
            case BooleanCollectionOperators.Different:
                {
                    var t = input.Count(IsTrue);
                    var f = input.Count(IsFalse);
                    return t > 0 && f > 0;
                }
            default:
                throw NotSupported(op);
        }
    }

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static bool? Merge(BooleanCollectionOperators op, params bool[] input)
        => Merge(op, input as IEnumerable<bool>);

    private static bool? Merge(BooleanCollectionOperators op)
        => op switch
        {
            BooleanCollectionOperators.True => true,
            BooleanCollectionOperators.False => false,
            _ => null
        };

    private static bool IsFalse(bool b)
        => !b;

    private static bool IsTrue(bool b)
        => b;
}
