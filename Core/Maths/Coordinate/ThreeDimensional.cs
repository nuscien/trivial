// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinate\ThreeDimensional.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The 3D models.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Trivial.Maths
{
    /// <summary>
    /// The generic 3D (stereoscophic) coordinate point.
    /// </summary>
    /// <typeparam name="TUnit">The type of unit.</typeparam>
    public class ThreeDimensionalPoint<TUnit> : ThreeElements<TUnit>, IEquatable<ThreeElements<TUnit>> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
    {
        /// <summary>
        /// Initializes a new instance of the ThreeDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public ThreeDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ThreeDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <param name="z">The value of Z.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public ThreeDimensionalPoint(TUnit x, TUnit y, TUnit z)
        {
            X = x;
            Y = y;
            Z = z;
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

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new ThreeDimensionalPoint<TUnit>(X, Y, Z);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(ThreeElements<TUnit> other)
        {
            return other != null && X.Equals(other.ItemA) && Y.Equals(other.ItemB) && Z.Equals(other.ItemC);
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
            var longStr = string.Format("{0} - {1} - {2}", x, y, z);
            var sep = false;
            if (longStr.IndexOfAny(new[] { ',', ';' }) > -1) sep = true;
            if (!sep && longStr.IndexOf(';') > -1)
            {
                const string quoteStr = "\"{0}\"";
                x = string.Format(quoteStr, x.Replace("\"", "\\\""));
                y = string.Format(quoteStr, y.Replace("\"", "\\\""));
                z = string.Format(quoteStr, z.Replace("\"", "\\\""));
            }

            return string.Format("X = {0}{1} Y = {2}{1} Z = {3}", x, sep ? ";" : ",", y, z);
        }
    }

    /// <summary>
    /// The point of 3D (stereoscophic) mathematics coordinate.
    /// </summary>
    public class DoubleThreeDimensionalPoint : ThreeDimensionalPoint<double>, IAdditionCapable<DoubleThreeDimensionalPoint>, ISubtractionCapable<DoubleThreeDimensionalPoint>, INegationCapable<DoubleThreeDimensionalPoint>
    {
        /// <summary>
        /// Initializes a new instance of the DoubleThreeDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DoubleThreeDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DoubleThreeDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <param name="z">The value of Z.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DoubleThreeDimensionalPoint(double x, double y, double z) : base(x, y, z)
        {
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoubleThreeDimensionalPoint Plus(ThreeDimensionalPoint<double> value)
        {
            return value != null
                ? new DoubleThreeDimensionalPoint(X + value.X, Y + value.Y, Z + value.Z)
                : new DoubleThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoubleThreeDimensionalPoint Plus(ThreeDimensionalPoint<int> value)
        {
            return value != null
                ? new DoubleThreeDimensionalPoint(X + value.X, Y + value.Y, Z + value.Z)
                : new DoubleThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoubleThreeDimensionalPoint Plus(DoubleThreeDimensionalPoint value)
        {
            return value != null
                ? new DoubleThreeDimensionalPoint(X + value.X, Y + value.Y, Z + value.Z)
                : new DoubleThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoubleThreeDimensionalPoint Minus(ThreeDimensionalPoint<double> value)
        {
            return value != null
                ? new DoubleThreeDimensionalPoint(X - value.X, Y - value.Y, Z - value.Z)
                : new DoubleThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoubleThreeDimensionalPoint Minus(ThreeDimensionalPoint<int> value)
        {
            return value != null
                ? new DoubleThreeDimensionalPoint(X - value.X, Y - value.Y, Z - value.Z)
                : new DoubleThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoubleThreeDimensionalPoint Minus(DoubleThreeDimensionalPoint value)
        {
            return value != null
                ? new DoubleThreeDimensionalPoint(X - value.X, Y - value.Y, Z - value.Z)
                : new DoubleThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public DoubleThreeDimensionalPoint Negate()
        {
            return new DoubleThreeDimensionalPoint(-X, -Y, -Z);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new DoubleThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static DoubleThreeDimensionalPoint operator +(DoubleThreeDimensionalPoint leftValue, DoubleThreeDimensionalPoint rightValue)
        {
            return (leftValue ?? new DoubleThreeDimensionalPoint()).Plus(rightValue);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static DoubleThreeDimensionalPoint operator -(DoubleThreeDimensionalPoint leftValue, DoubleThreeDimensionalPoint rightValue)
        {
            return (leftValue ?? new DoubleThreeDimensionalPoint()).Minus(rightValue);
        }
    }

    /// <summary>
    /// The point of 3D (stereoscophic) integer coordinate.
    /// </summary>
    public class Int32ThreeDimensionalPoint : ThreeDimensionalPoint<int>, IAdditionCapable<Int32ThreeDimensionalPoint>, ISubtractionCapable<Int32ThreeDimensionalPoint>, INegationCapable<Int32ThreeDimensionalPoint>
    {
        /// <summary>
        /// Initializes a new instance of the Int32ThreeDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32ThreeDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Int32ThreeDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <param name="z">The value of Z.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32ThreeDimensionalPoint(int x, int y, int z) : base(x, y, z)
        {
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public Int32ThreeDimensionalPoint Plus(ThreeDimensionalPoint<int> value)
        {
            return value != null
                ? new Int32ThreeDimensionalPoint(X + value.X, Y + value.Y, Z + value.Z)
                : new Int32ThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public Int32ThreeDimensionalPoint Plus(Int32ThreeDimensionalPoint value)
        {
            return value != null
                ? new Int32ThreeDimensionalPoint(X + value.X, Y + value.Y, Z + value.Z)
                : new Int32ThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public Int32ThreeDimensionalPoint Minus(ThreeDimensionalPoint<int> value)
        {
            return value != null
                ? new Int32ThreeDimensionalPoint(X - value.X, Y - value.Y, Z - value.Z)
                : new Int32ThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public Int32ThreeDimensionalPoint Minus(Int32ThreeDimensionalPoint value)
        {
            return value != null
                ? new Int32ThreeDimensionalPoint(X - value.X, Y - value.Y, Z - value.Z)
                : new Int32ThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public Int32ThreeDimensionalPoint Negate()
        {
            return new Int32ThreeDimensionalPoint(-X, -Y, -Z);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new Int32ThreeDimensionalPoint(X, Y, Z);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Int32ThreeDimensionalPoint operator +(Int32ThreeDimensionalPoint leftValue, Int32ThreeDimensionalPoint rightValue)
        {
            return (leftValue ?? new Int32ThreeDimensionalPoint()).Plus(rightValue);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Int32ThreeDimensionalPoint operator -(Int32ThreeDimensionalPoint leftValue, Int32ThreeDimensionalPoint rightValue)
        {
            return (leftValue ?? new Int32ThreeDimensionalPoint()).Minus(rightValue);
        }
    }
}
