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
using System.Collections.Generic;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The utility for arithmetic.
    /// </summary>
    public static partial class Arithmetic
    {
        /// <summary>
        /// Gets a result of factorial for a specific number.
        /// </summary>
        /// <param name="value">A number to calculate.</param>
        /// <returns>A number of result.</returns>
        /// <example>
        /// <code>
        /// var factorialNum = Arithmetic.Factorial(20); // => 2432902008176640000
        /// </code>
        /// </example>
        public static long Factorial(uint value)
        {
            if (value < 2) return 1;
            var resultNum = (long)value;
            for (uint step = 2; step < value; step++)
            {
                resultNum *= step;
            }

            return resultNum;
        }

        /// <summary>
        /// Gets a result of factorial for a specific number.
        /// </summary>
        /// <param name="value">A number to calculate.</param>
        /// <returns>A number of result.</returns>
        /// <example>
        /// <code>
        /// var factorialNum = Arithmetic.FactorialApproximate(100); // 9.33262154439442e+157
        /// </code>
        /// </example>
        public static double FactorialApproximate(uint value)
        {
            if (value < 2) return 1;
            var resultNum = (double)value;
            for (double step = 2; step < value; step++)
            {
                resultNum *= step;
            }

            return resultNum;
        }

        /// <summary>
        /// Calculates the value times 1024 of the specific power.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="e">The exponential.</param>
        /// <returns>The result calculated.</returns>
        /// <remarks>You can use this to calculate such as 80K or 4M.</remarks>
        public static int Times1024(int value, int e = 1)
        {
            return value * (1024 ^ e);
        }

        /// <summary>
        /// Calculates the value times 1024 of the specific power.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="e">The exponential.</param>
        /// <returns>The result calculated.</returns>
        /// <remarks>You can use this to calculate such as 80K or 4M.</remarks>
        public static long Times1024(long value, int e = 1)
        {
            return value * (1024L ^ e);
        }
    }
}
