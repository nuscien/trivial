// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arithmetic\Basic.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The basic arithmetic functions.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The utility for arithmetic.
    /// </summary>
    public static partial class Arithmetic
    {
        /// <summary>
        /// Computes variance of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The variance.</returns>
        public static double Variance(IEnumerable<int> col)
        {
            if (col == null) return double.NaN;
            var avg = col.Average();
            var count = col.Count();
            if (count < 1) return double.NaN;
            return col.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        /// <summary>
        /// Computes sample variance of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The sample variance.</returns>
        public static double SampleVariance(IEnumerable<int> col)
        {
            if (col == null) return double.NaN;
            var avg = col.Average();
            var count = col.Count() - 1;
            if (count < 1) return double.NaN;
            return col.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        /// <summary>
        /// Gets standard deviation of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The standard deviation.</returns>
        public static double StandardDeviation(IEnumerable<int> col)
        {
            var r = Variance(col);
            if (double.IsNaN(r)) return double.NaN;
            return Math.Sqrt(r);
        }

        /// <summary>
        /// Gets sample deviation of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The sample deviation.</returns>
        public static double SampleDeviation(IEnumerable<int> col)
        {
            var r = SampleVariance(col);
            if (double.IsNaN(r)) return double.NaN;
            return Math.Sqrt(r);
        }

        /// <summary>
        /// Computes variance of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The variance.</returns>
        public static double Variance(IEnumerable<long> col)
        {
            if (col == null) return double.NaN;
            var avg = col.Average();
            var count = col.Count();
            if (count < 1) return double.NaN;
            return col.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        /// <summary>
        /// Computes sample variance of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The sample variance.</returns>
        public static double SampleVariance(IEnumerable<long> col)
        {
            if (col == null) return double.NaN;
            var avg = col.Average();
            var count = col.Count() - 1;
            if (count < 1) return double.NaN;
            return col.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        /// <summary>
        /// Gets standard deviation of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The standard deviation.</returns>
        public static double StandardDeviation(IEnumerable<long> col)
        {
            var r = Variance(col);
            if (double.IsNaN(r)) return double.NaN;
            return Math.Sqrt(r);
        }

        /// <summary>
        /// Gets sample deviation of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The sample deviation.</returns>
        public static double SampleDeviation(IEnumerable<long> col)
        {
            var r = SampleVariance(col);
            if (double.IsNaN(r)) return double.NaN;
            return Math.Sqrt(r);
        }

        /// <summary>
        /// Computes variance of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The variance.</returns>
        public static double Variance(IEnumerable<float> col)
        {
            if (col == null) return double.NaN;
            var avg = col.Average();
            var count = col.Count();
            if (count < 1) return double.NaN;
            return col.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        /// <summary>
        /// Computes sample variance of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The sample variance.</returns>
        public static double SampleVariance(IEnumerable<float> col)
        {
            if (col == null) return double.NaN;
            var avg = col.Average();
            var count = col.Count() - 1;
            if (count < 1) return double.NaN;
            return col.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        /// <summary>
        /// Gets standard deviation of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The standard deviation.</returns>
        public static double StandardDeviation(IEnumerable<float> col)
        {
            var r = Variance(col);
            if (double.IsNaN(r)) return double.NaN;
            return Math.Sqrt(r);
        }

        /// <summary>
        /// Gets sample deviation of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The sample deviation.</returns>
        public static double SampleDeviation(IEnumerable<float> col)
        {
            var r = SampleVariance(col);
            if (double.IsNaN(r)) return double.NaN;
            return Math.Sqrt(r);
        }

        /// <summary>
        /// Computes variance of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The variance.</returns>
        public static double Variance(IEnumerable<double> col)
        {
            if (col == null) return double.NaN;
            var avg = col.Average();
            var count = col.Count();
            if (count < 1) return double.NaN;
            return col.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        /// <summary>
        /// Computes sample variance of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The sample variance.</returns>
        public static double SampleVariance(IEnumerable<double> col)
        {
            if (col == null) return double.NaN;
            var avg = col.Average();
            var count = col.Count() - 1;
            if (count < 1) return double.NaN;
            return col.Sum(ele => Math.Pow(ele - avg, 2)) / count;
        }

        /// <summary>
        /// Gets standard deviation of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The standard deviation.</returns>
        public static double StandardDeviation(IEnumerable<double> col)
        {
            var r = Variance(col);
            if (double.IsNaN(r)) return double.NaN;
            return Math.Sqrt(r);
        }

        /// <summary>
        /// Gets sample deviation of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number. Each number should not be less than 0.</param>
        /// <returns>The sample deviation.</returns>
        public static double SampleDeviation(IEnumerable<double> col)
        {
            var r = SampleVariance(col);
            if (double.IsNaN(r)) return double.NaN;
            return Math.Sqrt(r);
        }

        /// <summary>
        /// Computes the mean of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number.</param>
        /// <returns>The mean of the sequence of number.</returns>
        public static double Mean(IEnumerable<int> col)
            => col.Average();

        /// <summary>
        /// Computes the mean of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number.</param>
        /// <returns>The mean of the sequence of number.</returns>
        public static double Mean(IEnumerable<long> col)
            => col.Average();

        /// <summary>
        /// Computes the mean of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number.</param>
        /// <returns>The mean of the sequence of number.</returns>
        public static float Mean(IEnumerable<float> col)
            => col.Average();

        /// <summary>
        /// Computes the mean of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number.</param>
        /// <returns>The mean of the sequence of number.</returns>
        public static double Mean(IEnumerable<double> col)
            => col.Average();

        /// <summary>
        /// Computes the mode of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number.</param>
        /// <param name="count">The count of the mode.</param>
        /// <returns>The mode of the sequence of number.</returns>
        public static IEnumerable<T> Mode<T>(IEnumerable<T> col, out int count)
        {
            if (col == null)
            {
                count = 0;
                return null;
            }

            var dict = new Dictionary<T, int>();
            foreach (var item in col)
            {
                if (dict.ContainsKey(item)) dict[item]++;
                else dict[item] = 1;
            }

            var arr = new List<T>();
            if (dict.Count < 0)
            {
                count = 0;
                return arr;
            }

            count = dict.Max(ele => ele.Value);
            foreach (var item in dict)
            {
                if (item.Value == count) arr.Add(item.Key);
            }

            return arr;
        }

        /// <summary>
        /// Computes the mode of a sequence of number.
        /// </summary>
        /// <param name="col">The input collection of number.</param>
        /// <returns>The mode of the sequence of number.</returns>
        public static (IEnumerable<T> values, int count) Mode<T>(IEnumerable<T> col)
        {
            col = Mode(col, out var count);
            return (col, count);
        }
    }
}
