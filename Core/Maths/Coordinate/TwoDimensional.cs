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
    /// The generic 2D (plane) coordinate point.
    /// </summary>
    /// <typeparam name="TUnit">The type of unit.</typeparam>
    public class TwoDimensionalPoint<TUnit> : TwoElements<TUnit> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
    {
        /// <summary>
        /// Initializes a new instance of the TwoDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public TwoDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TwoDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public TwoDimensionalPoint(TUnit x, TUnit y)
        {
            X = x;
            Y = y;
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
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static TwoDimensionalPoint<int> Add(TwoDimensionalPoint<int> leftValue, TwoDimensionalPoint<int> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new TwoDimensionalPoint<int>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static TwoDimensionalPoint<long> Add(TwoDimensionalPoint<long> leftValue, TwoDimensionalPoint<long> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new TwoDimensionalPoint<long>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static TwoDimensionalPoint<float> Add(TwoDimensionalPoint<float> leftValue, TwoDimensionalPoint<float> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new TwoDimensionalPoint<float>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static TwoDimensionalPoint<double> Add(TwoDimensionalPoint<double> leftValue, TwoDimensionalPoint<double> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new TwoDimensionalPoint<double>
            {
                X = leftValue.X + rightValue.X,
                Y = leftValue.Y + rightValue.Y
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static TwoDimensionalPoint<int> Minus(TwoDimensionalPoint<int> leftValue, TwoDimensionalPoint<int> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new TwoDimensionalPoint<int>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static TwoDimensionalPoint<long> Minus(TwoDimensionalPoint<long> leftValue, TwoDimensionalPoint<long> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new TwoDimensionalPoint<long>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static TwoDimensionalPoint<float> Minus(TwoDimensionalPoint<float> leftValue, TwoDimensionalPoint<float> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new TwoDimensionalPoint<float>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static TwoDimensionalPoint<double> Minus(TwoDimensionalPoint<double> leftValue, TwoDimensionalPoint<double> rightValue)
        {
            if (leftValue == null)
                throw new ArgumentNullException("leftValue");
            if (rightValue == null)
                throw new ArgumentNullException("rightValue");
            return new TwoDimensionalPoint<double>
            {
                X = leftValue.X - rightValue.X,
                Y = leftValue.Y - rightValue.Y
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static TwoDimensionalPoint<int> Negate(TwoDimensionalPoint<int> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new TwoDimensionalPoint<int>
            {
                X = -value.X,
                Y = -value.Y
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static TwoDimensionalPoint<long> Negate(TwoDimensionalPoint<long> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new TwoDimensionalPoint<long>
            {
                X = -value.X,
                Y = -value.Y
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static TwoDimensionalPoint<float> Minus(TwoDimensionalPoint<float> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new TwoDimensionalPoint<float>
            {
                X = -value.X,
                Y = -value.Y
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static TwoDimensionalPoint<double> Negate(TwoDimensionalPoint<double> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new TwoDimensionalPoint<double>
            {
                X = -value.X,
                Y = -value.Y
            };
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public double GetDistance(TwoDimensionalPoint<int> pointA, TwoDimensionalPoint<int> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            return Math.Sqrt((pointB.X - pointA.X) ^ 2 + (pointB.Y - pointA.Y) ^ 2);
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public double GetDistance(TwoDimensionalPoint<long> pointA, TwoDimensionalPoint<long> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            return Math.Sqrt((pointB.X - pointA.X) ^ 2 + (pointB.Y - pointA.Y) ^ 2);
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public double GetDistance(TwoDimensionalPoint<float> pointA, TwoDimensionalPoint<float> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            var numA = pointB.X - pointA.X;
            var numB = pointB.Y - pointA.Y;
            return Math.Sqrt(numA * numA + numB * numB);
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public double GetDistance(TwoDimensionalPoint<double> pointA, TwoDimensionalPoint<double> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            var numA = pointB.X - pointA.X;
            var numB = pointB.Y - pointA.Y;
            return Math.Sqrt(numA * numA + numB * numB);
        }

        /// <summary>
        /// Returns the point string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this point.</returns>
        public override string ToString()
        {
            var x = X.ToString();
            var y = Y.ToString();
            var longStr = string.Format("{0} - {1}", x, y);
            var sep = false;
            if (longStr.IndexOfAny(new[] { ',', ';' }) > -1) sep = true;
            if (!sep && longStr.IndexOf(';') > -1)
            {
                const string quoteStr = "\"{0}\"";
                x = string.Format(quoteStr, x.Replace("\"", "\\\""));
                y = string.Format(quoteStr, y.Replace("\"", "\\\""));
            }

            return string.Format("X = {0}{1} Y = {2}", x, sep ? ";" : ",", y);
        }
    }
}
