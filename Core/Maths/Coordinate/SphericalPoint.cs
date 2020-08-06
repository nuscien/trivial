// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinate\SphericalPoint.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The spherical point.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Trivial.Maths
{
    /// <summary>
    /// The spherical coordinate point.
    /// </summary>
    public class SphericalPoint : IEquatable<SphericalPoint>, ICloneable
    {
        /// <summary>
        /// Spherical coordinate point symbols.
        /// </summary>
        public static class Symbols
        {
            /// <summary>
            /// The parameter name of phi.
            /// </summary>
            public const string PhiSymbol = "φ";
        }

        /// <summary>
        /// Initializes a new instance of the SphericalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public SphericalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SphericalPoint class.
        /// </summary>
        /// <param name="r">The length between center point and the specific point.</param>
        /// <param name="theta">The value of angel theta.</param>
        /// <param name="phi">The value of angel phi.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public SphericalPoint(double r, Angle theta, Angle phi)
        {
            Radius = r;
            Theta = theta;
            Phi = phi;
        }

        /// <summary>
        /// Initializes a new instance of the SphericalPoint class.
        /// </summary>
        /// <param name="r">The length between center point and the specific point.</param>
        /// <param name="theta">The total degrees of angel theta.</param>
        /// <param name="phi">The total degrees of angel phi.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public SphericalPoint(double r, double theta, double phi)
        {
            Radius = r;
            Theta = new Angle(theta);
            Phi = new Angle(phi);
        }

        /// <summary>
        /// Gets or sets the length between center point and the specific point (r).
        /// </summary>
        [JsonPropertyName("r")]
        [DataMember(Name = "r")]
        public double Radius
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the angel (θ).
        /// </summary>
        [JsonPropertyName("theta")]
        [DataMember(Name = "theta")]
        public Angle Theta
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the angel (φ).
        /// </summary>
        [JsonPropertyName("phi")]
        [DataMember(Name = "phi")]
        public Angle Phi
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
            return new SphericalPoint(Radius, Theta, Phi);
        }

        /// <summary>
        /// Returns a tuple that represents the values of current coordinate point object.
        /// </summary>
        /// <returns>The tuple representation of this coordinate point object.</returns>
        public Tuple<double, Angle, Angle> ToTuple()
        {
            return new Tuple<double, Angle, Angle>(Radius, Theta, Phi);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Theta.Degrees, Phi.Degrees, Radius).GetHashCode();
        }

        /// <summary>
        /// Returns the point string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this point.</returns>
        public override string ToString()
        {
            var radius = Radius.ToString();
            var theta = Theta.ToString();
            var phi = Phi.ToString();
            var longStr = string.Format("{0} - {1}", radius, theta);
            var sep = false;
            if (longStr.IndexOfAny(new[] { ',', ';' }) > -1) sep = true;
            if (!sep && longStr.IndexOf(';') > -1)
            {
                const string quoteStr = "\"{0}\"";
                radius = string.Format(quoteStr, radius.Replace("\"", "\\\""));
                theta = string.Format(quoteStr, theta.Replace("\"", "\\\""));
                phi = string.Format(quoteStr, phi.Replace("\"", "\\\""));
            }

            return string.Format("{0} = {1}{2} {3} = {4}{2} {5} = {6}", PolarPoint.Symbols.RadiusSymbol, radius, sep ? ";" : ",", PolarPoint.Symbols.ThetaSymbol, theta, Symbols.PhiSymbol, phi);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(SphericalPoint other)
        {
            if (other is null) return false;
            return Radius == other.Radius && Theta == other.Theta && Phi == other.Phi;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is SphericalPoint p) return Equals(p);
            return false;
        }

        /// <summary>
        /// Compares two points to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(SphericalPoint leftValue, SphericalPoint rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (leftValue is null || rightValue is null) return false;
            return leftValue.Theta == rightValue.Theta && leftValue.Phi == rightValue.Phi && leftValue.Radius == rightValue.Radius;
        }

        /// <summary>
        /// Compares two points to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(SphericalPoint leftValue, SphericalPoint rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (leftValue is null || rightValue is null) return true;
            return leftValue.Theta != rightValue.Theta || leftValue.Phi != rightValue.Phi || leftValue.Radius != rightValue.Radius;
        }

        /// <summary>
        /// Converts a number to a spherical point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static implicit operator SphericalPoint(ThreeDimensionalPoint<double> value)
        {
            if (value == null) return null;
            var r = Math.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z);
            return new SphericalPoint(r, Math.Acos(value.Z / r), Math.Atan(value.Y / value.X));
        }

        /// <summary>
        /// Converts a number to a spherical point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static implicit operator SphericalPoint(ThreeDimensionalPoint<long> value)
        {
            if (value == null) return null;
            var r = Math.Sqrt(value.X * 1.0 * value.X + value.Y * 1.0 * value.Y + value.Z * 1.0 * value.Z);
            return new SphericalPoint(r, Math.Acos(value.Z / r), Math.Atan(value.Y * 1/0 / value.X));
        }

        /// <summary>
        /// Converts a number to a spherical point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static implicit operator SphericalPoint(ThreeDimensionalPoint<int> value)
        {
            if (value == null) return null;
            var r = Math.Sqrt(value.X * 1.0 * value.X + value.Y * 1.0 * value.Y + value.Z * 1.0 * value.Z);
            return new SphericalPoint(r, Math.Acos(value.Z / r), Math.Atan(value.Y * 1.0 / value.X));
        }

        /// <summary>
        /// Converts a number to a spherical point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static explicit operator DoubleThreeDimensionalPoint(SphericalPoint value)
        {
            if (value == null) return null;
            return new DoubleThreeDimensionalPoint(value.Theta.Sin() * value.Phi.Cos() * value.Radius, value.Theta.Sin() * value.Phi.Sin() * value.Radius, value.Theta.Cos() * value.Radius);
        }
    }
}
