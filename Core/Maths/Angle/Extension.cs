// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Angle.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The struct of angle.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Trivial.Maths
{
    /// <summary>
    /// The struct of degree (angle).
    /// </summary>
    public static class AngleExtension
    {
        /// <summary>
        /// Gets a value indicating whether the angle is an acute angle.
        /// </summary>
        public static bool IsAcute(this IAngle angle)
        {
            return angle.Degree < 90 && angle.Degree >= 0;
        }

        /// <summary>
        /// Gets a value indicating whether the angle is a right angle.
        /// </summary>
        public static bool IsRight(this IAngle angle)
        {
            return angle.Arcsecond.Equals(0) && angle.Arcminute == 0 && angle.Degree == 90;
        }

        /// <summary>
        /// Gets a value indicating whether the angle is an oblique angle.
        /// </summary>
        public static bool IsOblique(this IAngle angle)
        {
            return !angle.Arcsecond.Equals(0) || angle.Arcminute != 0 || angle.Degree % 90 != 0;
        }

        /// <summary>
        /// Gets a value indicating whether the angle is an obtuse angle.
        /// </summary>
        public static bool IsObtuse(this IAngle angle)
        {
            return angle.Degree < 180 && angle.Degrees > 90;
        }

        /// <summary>
        /// Gets a value indicating whether the angle is a reflex angle.
        /// </summary>
        public static bool IsReflex(this IAngle angle)
        {
            return angle.Degree < 360 && angle.Degrees > 180;
        }

        /// <summary>
        /// Gets a value indicating whether the angle is less than 360 degrees and not less than 0 degree.
        /// </summary>
        public static bool LessThanRound(this IAngle angle)
        {
            var degree = angle.Degree;
            return degree < 360 && degree >= 0;
        }

        /// <summary>
        /// Returns the sine of this angle.
        /// </summary>
        /// <returns>The sine of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public static double Sin(this IAngle angle)
        {
            return Math.Sin(angle.Degrees);
        }

        /// <summary>
        /// Returns the cosine of this angle.
        /// </summary>
        /// <returns>The cosine of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public static double Cos(this IAngle angle)
        {
            return Math.Cos(angle.Degrees);
        }

        /// <summary>
        /// Returns the tangent of this angle.
        /// </summary>
        /// <returns>The tangent of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public static double Tan(this IAngle angle)
        {
            return Math.Tan(angle.Degrees);
        }

        /// <summary>
        /// Returns the cotangent of this angle.
        /// </summary>
        /// <returns>The cotangent of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public static double Cot(this IAngle angle)
        {
            return 1 / Math.Tan(angle.Degrees);
        }

        /// <summary>
        /// Returns the secant of this angle.
        /// </summary>
        /// <returns>The secant of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public static double Sec(this IAngle angle)
        {
            return 1 / Math.Cos(angle.Degrees);
        }

        /// <summary>
        /// Returns the cosecant of this angle.
        /// </summary>
        /// <returns>The cosecant of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public static double Csc(this IAngle angle)
        {
            return 1 / Math.Sin(angle.Degrees);
        }

        /// <summary>
        /// Returns the angle string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this angle.</returns>
        public static string ToAbsAngleString(this IAngle angle)
        {
            var degree = angle.Degree;
            var minute = angle.Arcminute;
            var second = angle.Arcsecond;
            if (second == 0)
            {
                if (minute == 0)
                {
                    return string.Format("{0}{1}", degree, Angle.Symbols.DegreeUnit);
                }

                return string.Format("{0}{1}{2}{3}", degree, Angle.Symbols.DegreeUnit, minute, Angle.Symbols.ArcminuteUnit);
            }

            return string.Format("{0}{1}{2}{3}{4}{5}", degree, Angle.Symbols.DegreeUnit, minute, Angle.Symbols.ArcminuteUnit, second, Angle.Symbols.ArcsecondUnit);
        }
    }
}
