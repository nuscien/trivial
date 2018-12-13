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
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Maths
{
    /// <summary>
    /// The utility for arithmetic.
    /// </summary>
    public static partial class Arithmetic
    {
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
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static bool IsPrime(int value)
        {
            return IsPrime((uint)Math.Abs(value));
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
        /// <param name="cancellationToken">The additional cancellation token.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static async Task<bool> IsPrimeAsync(ulong value, CancellationToken cancellationToken = default)
        {
            if (value < 2 || value % 2 == 0 || value % 3 == 0 || value % 5 == 0 || value % 7 == 0 || value % 11 == 0 || value % 13 == 0 || value % 17 == 0 || value % 19 == 0) return false;
            if (value <= 370) return true;
            var sq = (uint)Math.Floor(Math.Sqrt(value));
            for (uint i = 23; i < sq;)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var max = Math.Min(sq, i + 30000000L);
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
        public static bool IsPrime(long value)
        {
            return IsPrime((ulong)Math.Abs(value));
        }

        /// <summary>
        /// Gets a value indicating whether a number is a prime number.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="cancellationToken">The additional cancellation token.</param>
        /// <returns>true if it is a prime number; otherwise, false.</returns>
        public static Task<bool> IsPrimeAsync(long value, CancellationToken cancellationToken = default)
        {
            return IsPrimeAsync((ulong)Math.Abs(value), cancellationToken);
        }

        /// <summary>
        /// Gets the biggest prime number which is less than the specific one; or the smallest greater than, if the given one is negative.
        /// </summary>
        /// <param name="value">A number to start to find.</param>
        /// <param name="cancellationToken">The additional cancellation token.</param>
        /// <returns>The previous prime number.</returns>
        public static async Task<int> PreviousPrimeAsync(int value, CancellationToken cancellationToken = default)
        {
            var isNegative = value < 0;
            var prime = await PreviousPrimeAsync((ulong)Math.Abs(value), cancellationToken);
            return (int)prime * (isNegative ? -1 : 1);
        }

        /// <summary>
        /// Gets the biggest prime number which is less than the specific one; or the smallest greater than, if the given one is negative.
        /// </summary>
        /// <param name="value">A number to start to find.</param>
        /// <param name="cancellationToken">The additional cancellation token.</param>
        /// <returns>The previous prime number.</returns>
        public static async Task<long> PreviousPrimeAsync(long value, CancellationToken cancellationToken = default)
        {
            var isNegative = value < 0;
            var prime = await PreviousPrimeAsync((ulong)Math.Abs(value), cancellationToken);
            return (long)prime * (isNegative ? -1 : 1);
        }

        /// <summary>
        /// Gets the biggest prime number which is less than the specific one.
        /// </summary>
        /// <param name="value">A number to start to find.</param>
        /// <param name="cancellationToken">The additional cancellation token.</param>
        /// <returns>The previous prime number.</returns>
        public static async Task<ulong> PreviousPrimeAsync(ulong value, CancellationToken cancellationToken = default)
        {
            if (value < 24)
            {
                if (value <= 2) return 1;
                if (value <= 3) return 2;
                if (value <= 5) return 3;
                if (value <= 7) return 5;
                if (value <= 11) return 7;
                if (value <= 13) return 11;
                if (value <= 17) return 13;
                if (value <= 19) return 17;
                return 19;
            }

            var isEven = value % 2 == 0;
            if (value % 3 == 0)
            {
                if (isEven) value--;
                else value -= 2;
            }
            else if (isEven)
            {
                value--;
                if (value % 3 == 0) value -= 2;
            }
            else
            {
                value -= 6;
            }

            while (!await IsPrimeAsync(value, cancellationToken))
            {
                value -= 6;
            }

            return value;
        }

        /// <summary>
        /// Gets the smallest prime number which is greater than the specific one; or the biggest less, if the given one is negative.
        /// </summary>
        /// <param name="value">A number to start to find.</param>
        /// <param name="cancellationToken">The additional cancellation token.</param>
        /// <returns>The next prime number.</returns>
        public static async Task<int> NextPrimeAsync(int value, CancellationToken cancellationToken = default)
        {
            var isNegative = value < 0;
            var prime = await NextPrimeAsync((ulong)Math.Abs(value), cancellationToken);
            return (int)prime * (isNegative ? -1 : 1);
        }

        /// <summary>
        /// Gets the smallest prime number which is greater than the specific one; or the biggest less, if the given one is negative.
        /// </summary>
        /// <param name="value">A number to start to find.</param>
        /// <param name="cancellationToken">The additional cancellation token.</param>
        /// <returns>The next prime number.</returns>
        public static async Task<long> NextPrimeAsync(long value, CancellationToken cancellationToken = default)
        {
            var isNegative = value < 0;
            var prime = await NextPrimeAsync((ulong)Math.Abs(value), cancellationToken);
            return (long)prime * (isNegative ? -1 : 1);
        }

        /// <summary>
        /// Gets the smallest prime number which is greater than the specific one.
        /// </summary>
        /// <param name="value">A number to start to find.</param>
        /// <param name="cancellationToken">The additional cancellation token.</param>
        /// <returns>The next prime number.</returns>
        public static async Task<ulong> NextPrimeAsync(ulong value, CancellationToken cancellationToken = default)
        {
            var isEven = value % 2 == 0;
            if (value % 3 == 0)
            {
                if (isEven) value++;
                else value += 2;
            }
            else if (isEven)
            {
                value++;
                if (value % 3 == 0) value += 2;
            }
            else
            {
                value += 6;
            }

            while (!await IsPrimeAsync(value, cancellationToken))
            {
                value += 6;
            }

            return value;
        }
    }
}
