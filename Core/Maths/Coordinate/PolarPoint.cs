// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinate\PolarPoint.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The polar point.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;

namespace Trivial.Maths
{
    /// <summary>
    /// The polar coordinate point.
    /// </summary>
    public class PolarPoint : IEquatable<PolarPoint>, ICloneable
    {
        /// <summary>
        /// Polar coordinate point symbols.
        /// </summary>
        public static class Symbols
        {
            /// <summary>
            /// The parameter name of theta.
            /// </summary>
            public const string ThetaSymbol = "θ";

            /// <summary>
            /// The parameter name of radius.
            /// </summary>
            public const string RadiusSymbol = "r";
        }

        /// <summary>
        /// Initializes a new instance of the PolarPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public PolarPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PolarPoint class.
        /// </summary>
        /// <param name="r">The length between center point and the specific point.</param>
        /// <param name="theta">The value of angel.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public PolarPoint(double r, Angle theta)
        {
            Radius = r;
            Theta = theta;
        }

        /// <summary>
        /// Initializes a new instance of the PolarPoint class.
        /// </summary>
        /// <param name="r">The length between center point and the specific point.</param>
        /// <param name="degrees">The total degrees of angel.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public PolarPoint(double r, double degrees)
        {
            Radius = r;
            Theta = new Angle(degrees);
        }

        /// <summary>
        /// Gets or sets the length between center point and the specific point (r).
        /// </summary>
        public double Radius
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the angel (θ).
        /// </summary>
        public Angle Theta
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new PolarPoint(Radius, Theta);
        }

        /// <summary>
        /// Returns a tuple that represents the values of current coordinate point object.
        /// </summary>
        /// <returns>The tuple representation of this coordinate point object.</returns>
        public Tuple<double, Angle> ToTuple()
        {
            return new Tuple<double, Angle>(Radius, Theta);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", Theta.Degrees, Radius).GetHashCode();
        }

        /// <summary>
        /// Returns the point string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this point.</returns>
        public override string ToString()
        {
            var radius = Radius.ToString();
            var theta = Theta.ToString();
            var longStr = string.Format("{0} - {1}", radius, theta);
            var sep = false;
            if (longStr.IndexOfAny(new[] { ',', ';' }) > -1) sep = true;
            if (!sep && longStr.IndexOf(';') > -1)
            {
                const string quoteStr = "\"{0}\"";
                radius = string.Format(quoteStr, radius.Replace("\"", "\\\""));
                theta = string.Format(quoteStr, theta.Replace("\"", "\\\""));
            }

            return string.Format("{0} = {1}{2} {3} = {4}", Symbols.RadiusSymbol, radius, sep ? ";" : ",", Symbols.ThetaSymbol, theta);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(PolarPoint other)
        {
            if (other is null) return false;
            return Radius == other.Radius && Theta == other.Theta;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is PolarPoint p) return Equals(p);
            return false;
        }

        /// <summary>
        /// Compares two points to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(PolarPoint leftValue, PolarPoint rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (leftValue is null || rightValue is null) return false;
            return leftValue.Theta == rightValue.Theta && leftValue.Radius == rightValue.Radius;
        }

        /// <summary>
        /// Compares two points to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(PolarPoint leftValue, PolarPoint rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (leftValue is null || rightValue is null) return true;
            return leftValue.Theta != rightValue.Theta || leftValue.Radius != rightValue.Radius;
        }

        /// <summary>
        /// Converts a number to a polar point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static implicit operator PolarPoint(TwoDimensionalPoint<double> value)
        {
            if (value == null) return null;
            return new PolarPoint(Math.Sqrt(value.X * value.X + value.Y * value.Y), Math.Atan(value.Y / value.X));
        }

        /// <summary>
        /// Converts a number to a polar point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static implicit operator PolarPoint(TwoDimensionalPoint<long> value)
        {
            if (value == null) return null;
            return new PolarPoint(Math.Sqrt(value.X * 1.0 * value.X + value.Y * 1.0 * value.Y), Math.Atan(value.Y * 1.0 / value.X));
        }

        /// <summary>
        /// Converts a number to a polar point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static implicit operator PolarPoint(TwoDimensionalPoint<int> value)
        {
            if (value == null) return null;
            return new PolarPoint(Math.Sqrt(value.X * 1.0 * value.X + value.Y * 1.0 * value.Y), Math.Atan(value.Y * 1.0 / value.X));
        }

        /// <summary>
        /// Converts a number to a polar point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static explicit operator DoubleTwoDimensionalPoint(PolarPoint value)
        {
            if (value == null) return null;
            return new DoubleTwoDimensionalPoint(value.Theta.Cos() * value.Radius, value.Theta.Sin() * value.Radius);
        }
    }
}
