// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arithmetic.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The arithmetic classes and interfaces.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Maths
{
    /// <summary>
    /// The utility for arithmetic.
    /// </summary>
    public static class Arithmetic
    {
        /// <summary>
        /// Arithmetic symbols.
        /// </summary>
        public static class Symbols
        {
            /// <summary>
            /// The sign of addition.
            /// </summary>
            public const string AdditionSign = "+";

            /// <summary>
            /// The sign of subtraction.
            /// </summary>
            public const string SubtractionSign = "-";

            /// <summary>
            /// The sign of multiplication.
            /// </summary>
            public const string MultiplicationSign = "×";

            /// <summary>
            /// The sign of division.
            /// </summary>
            public const string DivisionSign = "÷";

            /// <summary>
            /// The sign of factorial.
            /// </summary>
            public const string FactorialSign = "!";

            /// <summary>
            /// The sign of percent.
            /// </summary>
            public const string PercentSign = "%";

            /// <summary>
            /// The sign of per thousand.
            /// </summary>
            public const string PerThousandSign = "‰";

            /// <summary>
            /// The sign of N-Ary summation.
            /// </summary>
            public const string SummationSign = "∑";

            /// <summary>
            /// The sign of N-Ary product.
            /// </summary>
            public const string ProductSign = "∏";

            /// <summary>
            /// The sign of square root.
            /// </summary>
            public const string SquareRootSign = "√";
        }

        /// <summary>
        /// Gets a result of factorial for a specific number.
        /// </summary>
        /// <param name="value">A number to calculate.</param>
        /// <returns>A number of result.</returns>
        public static long Factorial(uint value)
        {
            if (value < 2) return 1;
            var resultNum = (long)value;
            for (long step = 2; step < value; step++)
            {
                resultNum *= step;
            }

            return resultNum;
        }

        /// <summary>
        /// Gets a result of factorial for a specific number.
        /// </summary>
        /// <param name="value">A number to calculate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A number of result.</returns>
        public static async Task<long> FactorialAsync(uint value, CancellationToken cancellationToken)
        {
            if (value < 2) return 1;
            var resultNum = (long)value;
            for (long i = 2; i < value;)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var max = Math.Min(value, i + 1000000);
                await Task.Run(() =>
                {
                    while (i < max)
                    {
                        resultNum *= i;
                        i++;
                    }
                });
            }

            return resultNum;
        }

        /// <summary>
        /// Gets a result of factorial for a specific number.
        /// </summary>
        /// <param name="value">A number to calculate.</param>
        /// <returns>A number of result.</returns>
        public static Task<long> FactorialAsync(uint value)
        {
            return FactorialAsync(value, CancellationToken.None);
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static bool IsPrime(uint value)
        {
            if (value < 2 || value % 2 == 0 || value % 3 == 0 || value % 5 == 0 || value % 7 == 0 || value % 11 == 0 || value % 13 == 0 || value % 17 == 0 || value % 19 == 0) return false;
            if (value <= 370) return true;
            var sq = (uint)Math.Floor(Math.Sqrt(value));
            for (var i = 23; i < sq;)
            {
                if (value % i == 0) return false;
                i += 2;
                if (value % i == 0) return false;
                i += 4;
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static async Task<bool> IsPrimeAsync(uint value, CancellationToken cancellationToken)
        {
            if (value < 2 || value % 2 == 0 || value % 3 == 0 || value % 5 == 0 || value % 7 == 0 || value % 11 == 0 || value % 13 == 0 || value % 17 == 0 || value % 19 == 0) return false;
            if (value <= 370) return true;
            var sq = (uint)Math.Floor(Math.Sqrt(value));
            for (uint i = 23; i < sq;)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var max = Math.Min(sq, i + 6000000L);
                var isPrime = await Task.Run(() =>
                {
                    while (i < max)
                    {
                        if (value % i == 0) return false;
                        i += 2;
                        if (value % i == 0) return false;
                        i += 4;
                    }

                    return true;
                });

                if (!isPrime) return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static Task<bool> IsPrimeAsync(uint value)
        {
            return IsPrimeAsync(value);
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static bool IsPrime(int value)
        {
            return IsPrime((uint)Math.Abs(value));
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static Task<bool> IsPrimeAsync(int value, CancellationToken cancellationToken)
        {
            return IsPrimeAsync((ulong)Math.Abs(value), cancellationToken);
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static Task<bool> IsPrimeAsync(int value)
        {
            return IsPrimeAsync((ulong)Math.Abs(value), CancellationToken.None);
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static bool IsPrime(ulong value)
        {
            if (value < 2 || value % 2 == 0 || value % 3 == 0 || value % 5 == 0 || value % 7 == 0 || value % 11 == 0 || value % 13 == 0 || value % 17 == 0 || value % 19 == 0) return false;
            if (value <= 370) return true;
            var sq = (uint)Math.Floor(Math.Sqrt(value));
            for (uint i = 23; i < sq;)
            {
                if (value % i == 0) return false;
                i += 2;
                if (value % i == 0) return false;
                i += 4;
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static async Task<bool> IsPrimeAsync(ulong value, CancellationToken cancellationToken)
        {
            if (value < 2 || value % 2 == 0 || value % 3 == 0 || value % 5 == 0 || value % 7 == 0 || value % 11 == 0 || value % 13 == 0 || value % 17 == 0 || value % 19 == 0) return false;
            if (value <= 370) return true;
            var sq = (uint)Math.Floor(Math.Sqrt(value));
            for (uint i = 23; i < sq;)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var max = Math.Min(sq, i + 6000000L);
                var isPrime = await Task.Run(() =>
                {
                    while (i < max)
                    {
                        if (value % i == 0) return false;
                        i += 2;
                        if (value % i == 0) return false;
                        i += 4;
                    }

                    return true;
                });

                if (!isPrime) return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static Task<bool> IsPrimeAsync(ulong value)
        {
            return IsPrimeAsync(value, CancellationToken.None);
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static bool IsPrime(long value)
        {
            return IsPrime((ulong)Math.Abs(value));
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static Task<bool> IsPrimeAsync(long value, CancellationToken cancellationToken)
        {
            return IsPrimeAsync((ulong)Math.Abs(value), cancellationToken);
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static Task<bool> IsPrimeAsync(long value)
        {
            return IsPrimeAsync((ulong)Math.Abs(value), CancellationToken.None);
        }

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
            if (leftValue == null) throw new ArgumentNullException(nameof(leftValue));
            if (rightValue == null) throw new ArgumentNullException(nameof(rightValue));
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
            if (leftValue == null) throw new ArgumentNullException(nameof(leftValue));
            if (rightValue == null) throw new ArgumentNullException(nameof(rightValue));
            return leftValue.Minus(rightValue);
        }

        /// <summary>
        /// Minuses from leftValue to rightValue.
        /// leftValue - rightValue
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="leftValue">The left value to minus.</param>
        /// <param name="rightValue">The right value to be minused.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public static T Plus<T>(IAdditionCapable<T> leftValue, T rightValue)
        {
            if (leftValue == null) throw new ArgumentNullException(nameof(leftValue));
            if (rightValue == null) throw new ArgumentNullException(nameof(rightValue));
            return leftValue.Plus(rightValue);
        }
    }

    /// <summary>
    /// The addition capable interface.
    /// </summary>
    /// <typeparam name="T">The type of value for addition.</typeparam>
    public interface IAdditionCapable<T>
    {
        /// <summary>
        /// Pluses another value.
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
        /// Minuses another value.
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
        /// Negates the current value.
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
}
