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
    /// The generic 3D (stereoscophic) coordinate point.
    /// </summary>
    /// <typeparam name="TUnit">The type of unit.</typeparam>
    public class ThreeDimensionalPoint<TUnit> : ThreeElements<TUnit> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
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
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static ThreeDimensionalPoint<int> Add(ThreeDimensionalPoint<int> leftValue, ThreeDimensionalPoint<int> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new ThreeDimensionalPoint<int>
                       {
                           X = leftValue.X + rightValue.X,
                           Y = leftValue.Y + rightValue.Y,
                           Z = leftValue.Z + rightValue.Z
                       };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static ThreeDimensionalPoint<long> Add(ThreeDimensionalPoint<long> leftValue, ThreeDimensionalPoint<long> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new ThreeDimensionalPoint<long>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y,
                Z = leftValue.Z + rightValue.Z
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static ThreeDimensionalPoint<float> Add(ThreeDimensionalPoint<float> leftValue, ThreeDimensionalPoint<float> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new ThreeDimensionalPoint<float>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y,
                Z = leftValue.Z + rightValue.Z
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static ThreeDimensionalPoint<double> Add(ThreeDimensionalPoint<double> leftValue, ThreeDimensionalPoint<double> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new ThreeDimensionalPoint<double>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y,
                Z = leftValue.Z + rightValue.Z
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static ThreeDimensionalPoint<int> Minus(ThreeDimensionalPoint<int> leftValue, ThreeDimensionalPoint<int> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new ThreeDimensionalPoint<int>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y,
                Z = leftValue.Z - rightValue.Z
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static ThreeDimensionalPoint<long> Minus(ThreeDimensionalPoint<long> leftValue, ThreeDimensionalPoint<long> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new ThreeDimensionalPoint<long>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y,
                Z = leftValue.Z - rightValue.Z
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static ThreeDimensionalPoint<float> Minus(ThreeDimensionalPoint<float> leftValue, ThreeDimensionalPoint<float> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new ThreeDimensionalPoint<float>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y,
                Z = leftValue.Z - rightValue.Z
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static ThreeDimensionalPoint<double> Minus(ThreeDimensionalPoint<double> leftValue, ThreeDimensionalPoint<double> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new ThreeDimensionalPoint<double>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y,
                Z = leftValue.Z - rightValue.Z
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static ThreeDimensionalPoint<int> Negate(ThreeDimensionalPoint<int> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new ThreeDimensionalPoint<int>
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static ThreeDimensionalPoint<long> Negate(ThreeDimensionalPoint<long> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new ThreeDimensionalPoint<long>
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static ThreeDimensionalPoint<float> Negate(ThreeDimensionalPoint<float> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new ThreeDimensionalPoint<float>
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static ThreeDimensionalPoint<double> Negate(ThreeDimensionalPoint<double> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new ThreeDimensionalPoint<double>
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z
            };
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public double GetDistance(ThreeDimensionalPoint<int> pointA, ThreeDimensionalPoint<int> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            return Math.Sqrt((pointB.X - pointA.X) ^ 2 + (pointB.Y - pointA.Y) ^ 2 + (pointB.Z - pointA.Z) ^ 2);
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public double GetDistance(ThreeDimensionalPoint<long> pointA, ThreeDimensionalPoint<long> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            return Math.Sqrt((pointB.X - pointA.X) ^ 2 + (pointB.Y - pointA.Y) ^ 2 + (pointB.Z - pointA.Z) ^ 2);
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public double GetDistance(ThreeDimensionalPoint<float> pointA, ThreeDimensionalPoint<float> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            var numA = pointB.X - pointA.X;
            var numB = pointB.Y - pointA.Y;
            var numC = pointB.Z - pointA.Z;
            return Math.Sqrt(numA * numA + numB * numB + numC * numC);
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public double GetDistance(ThreeDimensionalPoint<double> pointA, ThreeDimensionalPoint<double> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            var numA = pointB.X - pointA.X;
            var numB = pointB.Y - pointA.Y;
            var numC = pointB.Z - pointA.Z;
            return Math.Sqrt(numA * numA + numB * numB + numC * numC);
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
}
