// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arithmetic\Operation.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The basic arithmetic operations.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Trivial.Maths;

/// <summary>
/// The utility for arithmetic.
/// </summary>
public static partial class Arithmetic
{
    /// <summary>
    /// Minuses from leftValue to rightValue.
    /// leftValue - rightValue
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="leftValue">The left value to minus.</param>
    /// <param name="rightValue">The right value to be minused.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public static T Minus<T>(IAdditionCapable<T> leftValue, INegationCapable<T> rightValue)
    {
        if (leftValue is null) throw new ArgumentNullException(nameof(leftValue));
        if (rightValue is null) throw new ArgumentNullException(nameof(rightValue));
        return leftValue.Plus(rightValue.Negate());
    }

    /// <summary>
    /// Minuses from leftValue to rightValue.
    /// leftValue - rightValue
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="leftValue">The left value to minus.</param>
    /// <param name="rightValue">The right value to be minused.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public static T Minus<T>(ISubtractionCapable<T> leftValue, T rightValue)
    {
        if (leftValue is null) throw new ArgumentNullException(nameof(leftValue));
        if (rightValue == null) throw new ArgumentNullException(nameof(rightValue));
        return leftValue.Minus(rightValue);
    }

    /// <summary>
    /// Pluses from leftValue to rightValue.
    /// leftValue + rightValue
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="leftValue">The left value to plus.</param>
    /// <param name="rightValue">The right value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public static T Plus<T>(IAdditionCapable<T> leftValue, T rightValue)
    {
        if (leftValue is null) throw new ArgumentNullException(nameof(leftValue));
        if (rightValue == null) throw new ArgumentNullException(nameof(rightValue));
        return leftValue.Plus(rightValue);
    }

    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Compare(short leftValue, BasicCompareOperator op, short rightValue)
        => op switch
        {
            BasicCompareOperator.Equal => leftValue == rightValue,
            BasicCompareOperator.NotEqual => leftValue != rightValue,
            BasicCompareOperator.Greater => leftValue > rightValue,
            BasicCompareOperator.Less => leftValue < rightValue,
            BasicCompareOperator.GreaterOrEqual => leftValue >= rightValue,
            BasicCompareOperator.LessOrEqual => leftValue <= rightValue,
            _ => throw NotSupport(op)
        };

    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Compare(int leftValue, BasicCompareOperator op, int rightValue)
        => op switch
        {
            BasicCompareOperator.Equal => leftValue == rightValue,
            BasicCompareOperator.NotEqual => leftValue != rightValue,
            BasicCompareOperator.Greater => leftValue > rightValue,
            BasicCompareOperator.Less => leftValue < rightValue,
            BasicCompareOperator.GreaterOrEqual => leftValue >= rightValue,
            BasicCompareOperator.LessOrEqual => leftValue <= rightValue,
            _ => throw NotSupport(op)
        };

    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Compare(long leftValue, BasicCompareOperator op, long rightValue)
        => op switch
        {
            BasicCompareOperator.Equal => leftValue == rightValue,
            BasicCompareOperator.NotEqual => leftValue != rightValue,
            BasicCompareOperator.Greater => leftValue > rightValue,
            BasicCompareOperator.Less => leftValue < rightValue,
            BasicCompareOperator.GreaterOrEqual => leftValue >= rightValue,
            BasicCompareOperator.LessOrEqual => leftValue <= rightValue,
            _ => throw NotSupport(op)
        };

    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Compare(float leftValue, BasicCompareOperator op, float rightValue)
        => op switch
        {
            BasicCompareOperator.Equal => leftValue == rightValue,
            BasicCompareOperator.NotEqual => leftValue != rightValue,
            BasicCompareOperator.Greater => leftValue > rightValue,
            BasicCompareOperator.Less => leftValue < rightValue,
            BasicCompareOperator.GreaterOrEqual => leftValue >= rightValue,
            BasicCompareOperator.LessOrEqual => leftValue <= rightValue,
            _ => throw NotSupport(op)
        };

    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Compare(double leftValue, BasicCompareOperator op, double rightValue)
        => op switch
        {
            BasicCompareOperator.Equal => leftValue == rightValue,
            BasicCompareOperator.NotEqual => leftValue != rightValue,
            BasicCompareOperator.Greater => leftValue > rightValue,
            BasicCompareOperator.Less => leftValue < rightValue,
            BasicCompareOperator.GreaterOrEqual => leftValue >= rightValue,
            BasicCompareOperator.LessOrEqual => leftValue <= rightValue,
            _ => throw NotSupport(op)
        };

    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Compare(decimal leftValue, BasicCompareOperator op, decimal rightValue)
        => op switch
        {
            BasicCompareOperator.Equal => leftValue == rightValue,
            BasicCompareOperator.NotEqual => leftValue != rightValue,
            BasicCompareOperator.Greater => leftValue > rightValue,
            BasicCompareOperator.Less => leftValue < rightValue,
            BasicCompareOperator.GreaterOrEqual => leftValue >= rightValue,
            BasicCompareOperator.LessOrEqual => leftValue <= rightValue,
            _ => throw NotSupport(op)
        };

    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Compare(DateTime leftValue, BasicCompareOperator op, DateTime rightValue)
        => op switch
        {
            BasicCompareOperator.Equal => leftValue == rightValue,
            BasicCompareOperator.NotEqual => leftValue != rightValue,
            BasicCompareOperator.Greater => leftValue > rightValue,
            BasicCompareOperator.Less => leftValue < rightValue,
            BasicCompareOperator.GreaterOrEqual => leftValue >= rightValue,
            BasicCompareOperator.LessOrEqual => leftValue <= rightValue,
            _ => throw NotSupport(op)
        };

    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Compare(TimeSpan leftValue, BasicCompareOperator op, TimeSpan rightValue)
        => op switch
        {
            BasicCompareOperator.Equal => leftValue == rightValue,
            BasicCompareOperator.NotEqual => leftValue != rightValue,
            BasicCompareOperator.Greater => leftValue > rightValue,
            BasicCompareOperator.Less => leftValue < rightValue,
            BasicCompareOperator.GreaterOrEqual => leftValue >= rightValue,
            BasicCompareOperator.LessOrEqual => leftValue <= rightValue,
            _ => throw NotSupport(op)
        };

    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Compare(Angle leftValue, BasicCompareOperator op, IAngle rightValue)
        => op switch
        {
            BasicCompareOperator.Equal => leftValue == rightValue,
            BasicCompareOperator.NotEqual => leftValue != rightValue,
            BasicCompareOperator.Greater => leftValue > rightValue,
            BasicCompareOperator.Less => leftValue < rightValue,
            BasicCompareOperator.GreaterOrEqual => leftValue >= rightValue,
            BasicCompareOperator.LessOrEqual => leftValue <= rightValue,
            _ => throw NotSupport(op)
        };

    /// <summary>
    /// Compares two values.
    /// </summary>
    /// <param name="leftValue">The left value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="rightValue">The right value.</param>
    /// <returns>true if the condition is tenable; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">op is not valid.</exception>
    public static bool Compare(Angle.Model leftValue, BasicCompareOperator op, IAngle rightValue)
        => op switch
        {
            BasicCompareOperator.Equal => leftValue == rightValue,
            BasicCompareOperator.NotEqual => leftValue != rightValue,
            BasicCompareOperator.Greater => leftValue > rightValue,
            BasicCompareOperator.Less => leftValue < rightValue,
            BasicCompareOperator.GreaterOrEqual => leftValue >= rightValue,
            BasicCompareOperator.LessOrEqual => leftValue <= rightValue,
            _ => throw NotSupport(op)
        };

    internal static NotSupportedException NotSupport<T>(T op) where T : struct, Enum
        => new($"The operation {op} is not supported.", new ArgumentOutOfRangeException(nameof(op), $"op has an invalid value {op}."));
}
