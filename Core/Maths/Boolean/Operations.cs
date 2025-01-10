using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Trivial.Collection;

namespace Trivial.Maths;

/// <summary>
/// The calculators and operator symbols of boolean.
/// </summary>
public static class BooleanOperations
{
    /// <summary>
    /// The operator sign of not.
    /// </summary>
    public const string NotSign = "¬";

    /// <summary>
    /// The operator sign of equal.
    /// </summary>
    public const string EqualSign = "=";

    /// <summary>
    /// The operator sign of not equal.
    /// </summary>
    public const string NotEqualSign = "≠";

    /// <summary>
    /// The operator sign of similar.
    /// </summary>
    public const string SimilarSign = "≈";

    /// <summary>
    /// The operator sign of greater.
    /// </summary>
    public const string GreaterSign = ">";

    /// <summary>
    /// The operator sign of less.
    /// </summary>
    public const string LessSign = "<";

    /// <summary>
    /// The operator sign of greater or equal.
    /// </summary>
    public const string GreaterOrEqualSign = "≥";

    /// <summary>
    /// The operator sign of less or equal.
    /// </summary>
    public const string LessOrEqualSign = "≤";

    /// <summary>
    /// The operator sign of not greater.
    /// </summary>
    public const string NotGreaterSign = "≮";

    /// <summary>
    /// The operator sign of not less.
    /// </summary>
    public const string NotLessSign = "≯";

    /// <summary>
    /// The operator sign of and.
    /// </summary>
    public const string AndSign = "∧";

    /// <summary>
    /// The operator sign of or.
    /// </summary>
    public const string OrSign = "∨";

    /// <summary>
    /// The operator sign of exclusive or.
    /// </summary>
    public const string XorSign = "⊕";

    /// <summary>
    /// The operator sign of exclusive negated or.
    /// </summary>
    public const string XnorSign = "⊙";

    /// <summary>
    /// The operator sign of union.
    /// </summary>
    public const string UnionSign = "∪";

    /// <summary>
    /// The operator sign of intersection.
    /// </summary>
    public const string IntersectionSign = "∩";

    /// <summary>
    /// The operator sign of include.
    /// </summary>
    public const string IncludeSign = "∈";

    /// <summary>
    /// Converts operation string.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <returns>A string that represents the operation.</returns>
    public static string ToString(BinaryBooleanOperator op)
        => op switch
        {
            BinaryBooleanOperator.And => AndSign,
            BinaryBooleanOperator.Or => OrSign,
            BinaryBooleanOperator.Xor => XorSign,
            BinaryBooleanOperator.Nand => string.Concat(NotSign, AndSign),
            BinaryBooleanOperator.Nor => string.Concat(NotSign, OrSign),
            BinaryBooleanOperator.Xnor => XnorSign,
            _ => null
        };

    /// <summary>
    /// Converts operation string.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <returns>A string that represents the operation.</returns>
    public static string ToString(CollectionBooleanOperator op)
        => op switch
        {
            CollectionBooleanOperator.Union => UnionSign,
            CollectionBooleanOperator.Intersection => IntersectionSign,
            CollectionBooleanOperator.Subtranction => "-",
            _ => null
        };

    /// <summary>
    /// Converts operation string.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <returns>A string that represents the operation.</returns>
    public static string ToString(CriteriaBooleanOperator op)
        => op switch
        {
            CriteriaBooleanOperator.And => "&",
            CriteriaBooleanOperator.Or => "|",
            _ => null
        };

    /// <summary>
    /// Converts the formulars to string.
    /// </summary>
    /// <param name="col">The formulars.</param>
    /// <returns>A string that each formular is in a line.</returns>
    public static string ToString(IEnumerable<BooleanBinaryOperationFormula> col)
        => string.Join(Environment.NewLine, col.Select(ele => ele?.ToString()));

    /// <summary>
    /// Converts a boolean to string format customized.
    /// </summary>
    /// <param name="value">A boolean value.</param>
    /// <param name="trueString">true in string.</param>
    /// <param name="falseString">false in string.</param>
    /// <returns>The customized string converted from boolean.</returns>
    public static string ToString(bool value, string trueString, string falseString)
        => value ? trueString : falseString;

    /// <summary>
    /// Converts a boolean to string format customized.
    /// </summary>
    /// <param name="value">A boolean value.</param>
    /// <param name="trueString">true in string.</param>
    /// <param name="falseString">false in string.</param>
    /// <param name="nullString">null in string.</param>
    /// <returns>The customized string converted from boolean.</returns>
    public static string ToString(bool? value, string trueString, string falseString, string nullString)
        => value.HasValue ? ToString(value.Value, trueString, falseString) : nullString;

    /// <summary>
    /// Calculates by boolean operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Calculate(bool leftValue, BinaryBooleanOperator op, bool rightValue)
    {
        var result = new BooleanBinaryOperationFormula(leftValue, op, rightValue);
        return result.IsValid ? result.Result : throw Arithmetic.NotSupport(op);
    }

    /// <summary>
    /// Calculates by boolean operation.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op1">The first operation.</param>
    /// <param name="middleValue">The right value.</param>
    /// <param name="op2">The second operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Calculate(bool leftValue, BinaryBooleanOperator op1, bool middleValue, BinaryBooleanOperator op2, bool rightValue)
        => Calculate(Calculate(leftValue, op1, middleValue), op2, rightValue);

    /// <summary>
    /// Calculates by boolean operation.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="leftValue">The left value.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Calculate(BinaryBooleanOperator op, bool leftValue, bool rightValue)
    {
        var result = new BooleanBinaryOperationFormula(op, leftValue, rightValue);
        return result.IsValid ? result.Result : throw Arithmetic.NotSupport(op);
    }

    /// <summary>
    /// Calculates by boolean operation.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="leftValue">The left value collection.</param>
    /// <param name="rightValue">The left value collection.</param>
    /// <param name="padding">The value to fill the missed.</param>
    /// <returns>A boolean sequence after calculating.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static IEnumerable<bool> Calculate(BinaryBooleanOperator op, IEnumerable<bool> leftValue, IEnumerable<bool> rightValue, bool padding)
        => ListExtensions.Select(leftValue, rightValue, padding, (l, hasL, r, hasR, i) => Calculate(op, l, r));

    /// <summary>
    /// Calculates by boolean operation.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="leftValue">The left value collection.</param>
    /// <param name="rightValue">The left value collection.</param>
    /// <returns>A boolean sequence after calculating.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static IEnumerable<bool> Calculate(BinaryBooleanOperator op, IEnumerable<bool> leftValue, IEnumerable<bool> rightValue)
        => ListExtensions.Select(leftValue, rightValue, false, (l, hasL, r, hasR, i) =>
        {
            if (hasL && hasR) return Calculate(op, l, r);
            return hasL ? l : r;
        });

    /// <summary>
    /// Calculates by boolean operation.
    /// </summary>
    /// <param name="col">The collection to formula.</param>
    /// <returns>A boolean sequence after calculating.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static IEnumerable<bool> Calculate(this IEnumerable<BooleanBinaryOperationFormula> col)
    {
        if (col is null) yield break;
        foreach (var item in col)
        {
            yield return item.Result;
        }
    }

    /// <summary>
    /// Calculates by boolean operation.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="value">The value to calculate in the unary boolean operation.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Calculate(UnaryBooleanOperator op, bool value)
        => op switch
        {
            UnaryBooleanOperator.Default => value,
            UnaryBooleanOperator.Not => !value,
            _ => throw Arithmetic.NotSupport(op)
        };

    /// <summary>
    /// Calculates by boolean operation.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="col">The collection to calculate each other in the unary boolean operation.</param>
    /// <returns>A boolean sequence after calculating.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static IEnumerable<bool> Calculate(UnaryBooleanOperator op, IEnumerable<bool> col)
    {
        if (col is null) yield break;
        foreach (var item in col)
        {
            yield return Calculate(op, item);
        }
    }
    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <param name="defaultValue">The default value for empty.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static bool Calculate(CriteriaBooleanOperator op, IEnumerable<bool> input, bool defaultValue)
        => Calculate(op, input) ?? defaultValue;

    /// <summary>
    /// Gets one item by specific operator.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="input">The input collection to compute.</param>
    /// <returns>The result.</returns>
    /// <exception cref="NotSupportedException">The operator is not supported.</exception>
    public static bool? Calculate(CriteriaBooleanOperator op, IEnumerable<bool> input)
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
                throw Arithmetic.NotSupport(op);
        }
    }

    /// <summary>
    /// Calculates by boolean operation.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="col">The collection to calculate each other in the unary boolean operation.</param>
    /// <returns>A boolean sequence after calculating; or null, if the input is null, or the invalid to calculate.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool? Calculate(SequenceBooleanOperator op, IEnumerable<bool> col)
    {
        if (col is null) return null;
        switch (op)
        {
            case SequenceBooleanOperator.And:
                {
                    bool? r = null;
                    foreach (var b in col)
                    {
                        if (b) r = true;
                        else return false;
                    }

                    return r;
                }
            case SequenceBooleanOperator.Or:
                {
                    bool? r = null;
                    foreach (var b in col)
                    {
                        if (b) return true;
                        else r = false;
                    }

                    return r;
                }
            case SequenceBooleanOperator.Nand:
                {
                    bool? r = null;
                    foreach (var b in col)
                    {
                        if (b) r = false;
                        else return true;
                    }

                    return r;
                }
            case SequenceBooleanOperator.Nor:
                {
                    bool? r = null;
                    foreach (var b in col)
                    {
                        if (b) return false;
                        else r = true;
                    }

                    return r;
                }
            case SequenceBooleanOperator.Same:
                {
                    bool? r = null;
                    foreach (var b in col)
                    {
                        if (r.HasValue && r.Value != b) return false;
                        r = b;
                    }

                    return r;
                }
            case SequenceBooleanOperator.Various:
                {
                    bool? r = null;
                    foreach (var b in col)
                    {
                        if (r.HasValue && r.Value != b) return true;
                        r = b;
                    }

                    return r;
                }
            case SequenceBooleanOperator.First:
                return ListExtensions.FirstOrNull(col);
            case SequenceBooleanOperator.Last:
                return ListExtensions.LastOrNull(col);
            case SequenceBooleanOperator.Most:
                {
                    var trues = 0;
                    var falses = 0;
                    foreach (var b in col)
                    {
                        if (b) trues++;
                        else falses++;
                    }

                    if (trues == falses) return null;
                    return trues > falses;
                }
            case SequenceBooleanOperator.MostOrFirst:
                {
                    var trues = 0;
                    var falses = 0;
                    bool? first = null;
                    foreach (var b in col)
                    {
                        if (!first.HasValue) first = b;
                        if (b) trues++;
                        else falses++;
                    }

                    if (trues == falses) return first;
                    return trues > falses;
                }
            case SequenceBooleanOperator.MostOrLast:
                {
                    var trues = 0;
                    var falses = 0;
                    bool? first = null;
                    foreach (var b in col)
                    {
                        first = b;
                        if (b) trues++;
                        else falses++;
                    }

                    if (trues == falses) return first;
                    return trues > falses;
                }
            case SequenceBooleanOperator.Least:
                {
                    var trues = 0;
                    var falses = 0;
                    foreach (var b in col)
                    {
                        if (b) trues++;
                        else falses++;
                    }

                    if (trues == falses) return null;
                    return trues < falses;
                }
            case SequenceBooleanOperator.LeastOrFirst:
                {
                    var trues = 0;
                    var falses = 0;
                    bool? first = null;
                    foreach (var b in col)
                    {
                        if (!first.HasValue) first = b;
                        if (b) trues++;
                        else falses++;
                    }

                    if (trues == falses) return first;
                    return trues < falses;
                }
            case SequenceBooleanOperator.LeastOrLast:
                {
                    var trues = 0;
                    var falses = 0;
                    bool? first = null;
                    foreach (var b in col)
                    {
                        first = b;
                        if (b) trues++;
                        else falses++;
                    }

                    if (trues == falses) return first;
                    return trues < falses;
                }
            case SequenceBooleanOperator.Half:
                {
                    if (col is ICollection<bool> col2)
                    {
                        if (col2.Count % 2 == 1) return false;
                    }
                    else if (col is bool[] col3)
                    {
                        if (col3.Length % 2 == 1) return false;
                    }

                    var trues = 0;
                    var falses = 0;
                    foreach (var b in col)
                    {
                        if (b) trues++;
                        else falses++;
                    }

                    return trues == falses;
                }
            case SequenceBooleanOperator.Nhalf:
                {
                    if (col is ICollection<bool> col2)
                    {
                        if (col2.Count % 2 == 1) return true;
                    }
                    else if (col is bool[] col3)
                    {
                        if (col3.Length % 2 == 1) return true;
                    }

                    var trues = 0;
                    var falses = 0;
                    foreach (var b in col)
                    {
                        if (b) trues++;
                        else falses++;
                    }

                    return trues != falses;
                }
            case SequenceBooleanOperator.Positive:
                {
                    var trues = 0;
                    var falses = 0;
                    foreach (var b in col)
                    {
                        if (b) trues++;
                        else falses++;
                    }

                    return trues >= falses;
                }
            case SequenceBooleanOperator.Negative:
                {
                    var trues = 0;
                    var falses = 0;
                    foreach (var b in col)
                    {
                        if (b) trues++;
                        else falses++;
                    }

                    return trues <= falses;
                }
            case SequenceBooleanOperator.True:
                return true;
            case SequenceBooleanOperator.False:
                return false;
            default:
                throw Arithmetic.NotSupport(op);
        }
    }

    /// <summary>
    /// Calculates by boolean operation.
    /// </summary>
    /// <param name="op">The operation.</param>
    /// <param name="col">The collection to calculate each other in the unary boolean operation.</param>
    /// <param name="defaultValue">The default (or fallback) value.</param>
    /// <returns>A boolean sequence after calculating.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Calculate(SequenceBooleanOperator op, IEnumerable<bool> col, bool defaultValue)
        => Calculate(op, col) ?? defaultValue;
}

/// <summary>
/// The boolean binary operation formula.
/// </summary>
[JsonConverter(typeof(JsonBinaryOperationFormulaConverter))]
public sealed class BooleanBinaryOperationFormula : BaseBinaryOperationFormula<bool, BinaryBooleanOperator>
{
    private string s;

    /// <summary>
    /// Initializes a new instance of the BooleanBinaryOperationFormula class.
    /// </summary>
    /// <param name="leftValue">The left value to calculate.</param>
    /// <param name="op">The operator.</param>
    /// <param name="rightValue">The right value to calculate.</param>
    public BooleanBinaryOperationFormula(bool leftValue, BinaryBooleanOperator op, bool rightValue)
        : base(leftValue, op, rightValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the BooleanBinaryOperationFormula class.
    /// </summary>
    /// <param name="op">The operator.</param>
    /// <param name="leftValue">The left value to calculate.</param>
    /// <param name="rightValue">The right value to calculate.</param>
    public BooleanBinaryOperationFormula(BinaryBooleanOperator op, bool leftValue, bool rightValue)
        : base(leftValue, op, rightValue)
    {
    }

    /// <inheritdoc />
    protected override bool TryGetResult(out bool result)
    {
        switch (Operator)
        {
            case BinaryBooleanOperator.And:
                result = LeftValue && RightValue;
                s = string.Concat(LeftValue, ' ', BooleanOperations.AndSign, ' ', RightValue, " = ", result);
                return true;
            case BinaryBooleanOperator.Or:
                result = LeftValue || RightValue;
                s = string.Concat(LeftValue, ' ', BooleanOperations.OrSign, ' ', RightValue, " = ", result);
                return true;
            case BinaryBooleanOperator.Xor:
                result = LeftValue != RightValue;
                s = string.Concat(LeftValue, ' ', BooleanOperations.XorSign, ' ', RightValue, " = ", result);
                return true;
            case BinaryBooleanOperator.Nand:
                result = !(LeftValue && RightValue);
                s = string.Concat(BooleanOperations.NotSign, '(', LeftValue, ' ', BooleanOperations.AndSign, ' ', RightValue, ") = ", result);
                return true;
            case BinaryBooleanOperator.Nor:
                result = !(LeftValue || RightValue);
                s = string.Concat(BooleanOperations.NotSign, '(', LeftValue, ' ', BooleanOperations.OrSign, ' ', RightValue, ") = ", result);
                return true;
            case BinaryBooleanOperator.Xnor:
                result = LeftValue == RightValue;
                s = string.Concat(LeftValue, ' ', BooleanOperations.XnorSign, ' ', RightValue, " = ", result);
                return true;
            default:
                result = default;
                s = string.Concat("Error! Invalid operator ", Operator, " to calculate ", LeftValue, " and ", RightValue, '.');
                return false;
        }
    }

    /// <summary>
    /// Returns a string that represents the current formula.
    /// </summary>
    /// <returns>A string that represents the current formula.</returns>
    public override string ToString()
        => s;
}
