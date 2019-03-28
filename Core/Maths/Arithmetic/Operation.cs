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
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The addition capable interface.
    /// </summary>
    /// <typeparam name="T">The type of value for addition.</typeparam>
    public interface IAdditionCapable<T>
    {
        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">A given value to be added.</param>
        /// <returns>A result after addition.</returns>
        T Plus(T value);
    }

    /// <summary>
    /// The subtraction capable interface.
    /// </summary>
    /// <typeparam name="T">The type of value for subtraction.</typeparam>
    public interface ISubtractionCapable<T>
    {
        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">A given value to be added.</param>
        /// <returns>A result after subtraction.</returns>
        T Minus(T value);
    }

    /// <summary>
    /// The subtraction capable interface.
    /// </summary>
    /// <typeparam name="T">The type of value for subtraction.</typeparam>
    public interface INegationCapable<out T>
    {
        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        T Negate();
    }

    /// <summary>
    /// The addition and subtraction capable interface.
    /// </summary>
    /// <typeparam name="T">The type of value for addition and subtraction.</typeparam>
    public interface IAdvancedAdditionCapable<T> : IAdditionCapable<T>, ISubtractionCapable<T>, INegationCapable<T>
    {
        /// <summary>
        /// Gets a unit element for addition and subtraction.
        /// 0
        /// </summary>
        /// <returns>An element zero for the value.</returns>
        T GetElementZero();

        /// <summary>
        /// Gets a value indicating whether the current element is negative.
        /// true if it is positve or zero; otherwise, false.
        /// </summary>
        bool IsNegative { get; }

        /// <summary>
        /// Gets a value indicating whether the current element is a unit element of zero.
        /// </summary>
        bool IsZero { get; }
    }

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
    }
}
