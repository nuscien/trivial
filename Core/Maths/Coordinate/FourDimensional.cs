// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinate.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The classes of coordinates and points.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Trivial.Maths
{
    /// <summary>
    /// The generic 4D (time and space) coordinate point.
    /// </summary>
    /// <typeparam name="TUnit">The type of unit.</typeparam>
    public class FourDimensionalPoint<TUnit> : FourElements<TUnit> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
    {
        /// <summary>
        /// Initializes a new instance of the FourDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public FourDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FourDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <param name="z">The value of Z.</param>
        /// <param name="t">The value of T.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public FourDimensionalPoint(TUnit x, TUnit y, TUnit z, TUnit t)
        {
            X = x;
            Y = y;
            Z = z;
            T = t;
        }

        /// <summary>
        /// Gets or sets the value of X (horizontal position). The value is same as ItemA.
        /// </summary>
        public TUnit X
        {
            get => base.ItemA;
            set => base.ItemA = value;
        }

        /// <summary>
        /// Gets or sets the value of Y (vertical position). The value is same as ItemB.
        /// </summary>
        public TUnit Y
        {
            get => base.ItemB;
            set => base.ItemB = value;
        }

        /// <summary>
        /// Gets or sets the value of Z (depth). The value is same as ItemC.
        /// </summary>
        public TUnit Z
        {
            get => base.ItemC;
            set => base.ItemC = value;
        }

        /// <summary>
        /// Gets or sets the value of T (time). The value is same as ItemD.
        /// </summary>
        public TUnit T
        {
            get => base.ItemD;
            set => base.ItemD = value;
        }

        private new TUnit ItemA
        {
            get => base.ItemA;
            set => base.ItemA = value;
        }

        private new TUnit ItemB
        {
            get => base.ItemB;
            set => base.ItemB = value;
        }

        private new TUnit ItemC
        {
            get => base.ItemC;
            set => base.ItemC = value;
        }

        private new TUnit ItemD
        {
            get => base.ItemD;
            set => base.ItemD = value;
        }
 
        /// <summary>
        /// Returns the point string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this point.</returns>
        public override string ToString()
        {
            var x = X.ToString();
            var y = Y.ToString();
            var z = Z.ToString();
            var t = T.ToString();
            var longStr = string.Format("{0} - {1} - {2} - {3}", x, y, z, t);
            var sep = false;
            if (longStr.IndexOfAny(new[] {',', ';'}) > -1) sep = true;
            if (!sep && longStr.IndexOf(';') > -1)
            {
                const string quoteStr = "\"{0}\"";
                x = string.Format(quoteStr, x.Replace("\"", "\\\""));
                y = string.Format(quoteStr, y.Replace("\"", "\\\""));
                z = string.Format(quoteStr, z.Replace("\"", "\\\""));
                t = string.Format(quoteStr, t.Replace("\"", "\\\""));
            }

            return string.Format("X = {0}{1} Y = {2}{1} Z = {3} T = {4}", x, sep ? ";" : ",", y, z, t);
        }
    }

    /// <summary>
    /// The generic 4D (time and space) coordinate point.
    /// </summary>
    /// <typeparam name="TSpaceUnit">The type of space unit.</typeparam>
    /// <typeparam name="TTimeUnit">The type of time unit.</typeparam>
    public class FourDimensionalPoint<TSpaceUnit, TTimeUnit> : ThreeDimensionalPoint<TSpaceUnit>
        where TSpaceUnit : struct, IComparable<TSpaceUnit>, IEquatable<TSpaceUnit>
        where TTimeUnit : struct, IComparable<TTimeUnit>, IEquatable<TTimeUnit>
    {
        /// <summary>
        /// Initializes a new instance of the FourDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public FourDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FourDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <param name="z">The value of Z.</param>
        /// <param name="t">The value of T.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public FourDimensionalPoint(TSpaceUnit x, TSpaceUnit y, TSpaceUnit z, TTimeUnit t)
        {
            X = x;
            Y = y;
            Z = z;
            T = t;
        }

        /// <summary>
        /// Gets or sets the value of T (time). The value is same as ItemD.
        /// </summary>
        public TTimeUnit T
        {
            get; set;
        }

        /// <summary>
        /// Returns the point string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this point.</returns>
        public override string ToString()
        {
            var x = X.ToString();
            var y = Y.ToString();
            var z = Z.ToString();
            var t = T.ToString();
            var longStr = string.Format("{0} - {1} - {2} - {3}", x, y, z, t);
            var sep = false;
            if (longStr.IndexOfAny(new[] { ',', ';' }) > -1) sep = true;
            if (!sep && longStr.IndexOf(';') > -1)
            {
                const string quoteStr = "\"{0}\"";
                x = string.Format(quoteStr, x.Replace("\"", "\\\""));
                y = string.Format(quoteStr, y.Replace("\"", "\\\""));
                z = string.Format(quoteStr, z.Replace("\"", "\\\""));
                t = string.Format(quoteStr, t.Replace("\"", "\\\""));
            }

            return string.Format("X = {0}{1} Y = {2}{1} Z = {3} T = {4}", x, sep ? ";" : ",", y, z, t);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public virtual bool Equals(FourDimensionalPoint<TSpaceUnit, TTimeUnit> other)
        {
            return other != null && X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && T.Equals(other.T);
        }
    }

    /// <summary>
    /// The point of the 4D (time and space) mathematics coordinate and time span.
    /// </summary>
    public class SpacetimePoint : FourDimensionalPoint<double, TimeSpan>
    {
        /// <summary>
        /// Initializes a new instance of the SpaceTimePoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public SpacetimePoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SpaceTimePoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <param name="z">The value of Z.</param>
        /// <param name="t">The value of T.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public SpacetimePoint(double x, double y, double z, TimeSpan t) : base(x, y, z, t)
        {
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public SpacetimePoint Plus(FourDimensionalPoint<double, TimeSpan> value)
        {
            return value != null
                ? new SpacetimePoint(X + value.X, Y + value.Y, Z + value.Z, T + value.T)
                : new SpacetimePoint(X, Y, Z, T);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public SpacetimePoint Plus(FourDimensionalPoint<int, TimeSpan> value)
        {
            return value != null
                ? new SpacetimePoint(X + value.X, Y + value.Y, Z + value.Z, T + value.T)
                : new SpacetimePoint(X, Y, Z, T);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public SpacetimePoint Plus(SpacetimePoint value)
        {
            return value != null
                ? new SpacetimePoint(X + value.X, Y + value.Y, Z + value.Z, T + value.T)
                : new SpacetimePoint(X, Y, Z, T);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public SpacetimePoint Minus(FourDimensionalPoint<double, TimeSpan> value)
        {
            return value != null
                ? new SpacetimePoint(X - value.X, Y - value.Y, Z - value.Z, T - value.T)
                : new SpacetimePoint(X, Y, Z, T);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public SpacetimePoint Minus(FourDimensionalPoint<int, TimeSpan> value)
        {
            return value != null
                ? new SpacetimePoint(X - value.X, Y - value.Y, Z - value.Z, T - value.T)
                : new SpacetimePoint(X, Y, Z, T);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public SpacetimePoint Minus(SpacetimePoint value)
        {
            return value != null
                ? new SpacetimePoint(X - value.X, Y - value.Y, Z - value.Z, T - value.T)
                : new SpacetimePoint(X, Y, Z, T);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public SpacetimePoint Negate()
        {
            return new SpacetimePoint(-X, -Y, -Z, -T);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new SpacetimePoint(X, Y, Z, T);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static SpacetimePoint operator +(SpacetimePoint leftValue, SpacetimePoint rightValue)
        {
            return (leftValue ?? new SpacetimePoint()).Plus(rightValue);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static SpacetimePoint operator -(SpacetimePoint leftValue, SpacetimePoint rightValue)
        {
            return (leftValue ?? new SpacetimePoint()).Minus(rightValue);
        }
    }
}
