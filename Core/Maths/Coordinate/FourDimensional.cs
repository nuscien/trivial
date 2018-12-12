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
            get { return ItemA; }
            set { ItemA = value; }
        }

        /// <summary>
        /// Gets or sets the value of Y (vertical position). The value is same as ItemB.
        /// </summary>
        public TUnit Y
        {
            get { return ItemB; }
            set { ItemB = value; }
        }

        /// <summary>
        /// Gets or sets the value of Z (depth). The value is same as ItemC.
        /// </summary>
        public TUnit Z
        {
            get { return ItemC; }
            set { ItemC = value; }
        }

        /// <summary>
        /// Gets or sets the value of T (time). The value is same as ItemD.
        /// </summary>
        public TUnit T
        {
            get { return ItemD; }
            set { ItemD = value; }
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static FourDimensionalPoint<int> Add(FourDimensionalPoint<int> leftValue, FourDimensionalPoint<int> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new FourDimensionalPoint<int>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y,
                Z = leftValue.Z - rightValue.Z,
                T = leftValue.T + rightValue.T
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static FourDimensionalPoint<long> Add(FourDimensionalPoint<long> leftValue, FourDimensionalPoint<long> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new FourDimensionalPoint<long>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y,
                Z = leftValue.Z - rightValue.Z,
                T = leftValue.T + rightValue.T
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static FourDimensionalPoint<float> Add(FourDimensionalPoint<float> leftValue, FourDimensionalPoint<float> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new FourDimensionalPoint<float>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y,
                Z = leftValue.Z - rightValue.Z,
                T = leftValue.T + rightValue.T
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static FourDimensionalPoint<double> Add(FourDimensionalPoint<double> leftValue, FourDimensionalPoint<double> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new FourDimensionalPoint<double>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y,
                Z = leftValue.Z - rightValue.Z,
                T = leftValue.T + rightValue.T
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static FourDimensionalPoint<int> Minus(FourDimensionalPoint<int> leftValue, FourDimensionalPoint<int> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new FourDimensionalPoint<int>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y,
                Z = leftValue.Z - rightValue.Z,
                T = leftValue.T - rightValue.T
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static FourDimensionalPoint<long> Minus(FourDimensionalPoint<long> leftValue, FourDimensionalPoint<long> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new FourDimensionalPoint<long>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y,
                Z = leftValue.Z - rightValue.Z,
                T = leftValue.T - rightValue.T
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static FourDimensionalPoint<float> Minus(FourDimensionalPoint<float> leftValue, FourDimensionalPoint<float> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new FourDimensionalPoint<float>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y,
                Z = leftValue.Z - rightValue.Z,
                T = leftValue.T - rightValue.T
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static FourDimensionalPoint<double> Minus(FourDimensionalPoint<double> leftValue, FourDimensionalPoint<double> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new FourDimensionalPoint<double>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y,
                Z = leftValue.Z - rightValue.Z,
                T = leftValue.T - rightValue.T
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static FourDimensionalPoint<int> Negate(FourDimensionalPoint<int> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new FourDimensionalPoint<int>
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z,
                T = -value.T
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static FourDimensionalPoint<long> Negate(FourDimensionalPoint<long> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new FourDimensionalPoint<long>
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z,
                T = -value.T
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static FourDimensionalPoint<float> Negate(FourDimensionalPoint<float> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new FourDimensionalPoint<float>
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z,
                T = -value.T
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static FourDimensionalPoint<double> Negate(FourDimensionalPoint<double> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new FourDimensionalPoint<double>
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z,
                T = -value.T
            };
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

            return string.Format("X = {0}{1} Y = {2}{1} Z = {3}", x, sep ? ";" : ",", y, z, t);
        }
    }
}
