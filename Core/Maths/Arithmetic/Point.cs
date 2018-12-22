using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The utility for arithmetic.
    /// </summary>
    public static partial class Arithmetic
    {
        #region 1D

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static OneDimensionalPoint<int> Plus(OneDimensionalPoint<int> leftValue, OneDimensionalPoint<int> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<int>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<int>();
            return new OneDimensionalPoint<int>
            {
                X = leftValue.X + rightValue.X
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static OneDimensionalPoint<long> Plus(OneDimensionalPoint<long> leftValue, OneDimensionalPoint<long> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<long>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<long>();
            return new OneDimensionalPoint<long>
            {
                X = leftValue.X + rightValue.X
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static OneDimensionalPoint<float> Plus(OneDimensionalPoint<float> leftValue, OneDimensionalPoint<float> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<float>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<float>();
            return new OneDimensionalPoint<float>
            {
                X = leftValue.X + rightValue.X
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static OneDimensionalPoint<double> Plus(OneDimensionalPoint<double> leftValue, OneDimensionalPoint<double> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<double>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<double>();
            return new OneDimensionalPoint<double>
            {
                X = leftValue.X + rightValue.X
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static OneDimensionalPoint<int> Minus(OneDimensionalPoint<int> leftValue, OneDimensionalPoint<int> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<int>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<int>();
            return new OneDimensionalPoint<int>
            {
                X = leftValue.X - rightValue.X
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static OneDimensionalPoint<long> Minus(OneDimensionalPoint<long> leftValue, OneDimensionalPoint<long> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<long>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<long>();
            return new OneDimensionalPoint<long>
            {
                X = leftValue.X - rightValue.X
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static OneDimensionalPoint<float> Minus(OneDimensionalPoint<float> leftValue, OneDimensionalPoint<float> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<float>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<float>();
            return new OneDimensionalPoint<float>
            {
                X = leftValue.X - rightValue.X
            };
        }

        /// <summary>
        /// Minuses two points in coordinate.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static OneDimensionalPoint<double> Minus(OneDimensionalPoint<double> leftValue, OneDimensionalPoint<double> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<double>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<double>();
            return new OneDimensionalPoint<double>
            {
                X = leftValue.X - rightValue.X
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static OneDimensionalPoint<int> Negate(OneDimensionalPoint<int> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new OneDimensionalPoint<int>
            {
                X = -value.X
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static OneDimensionalPoint<long> Negate(OneDimensionalPoint<long> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new OneDimensionalPoint<long>
            {
                X = -value.X
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static OneDimensionalPoint<float> Negate(OneDimensionalPoint<float> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new OneDimensionalPoint<float>
            {
                X = -value.X
            };
        }

        /// <summary>
        /// Negates a specific point in coordinate.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific point in coordinate.</returns>
        public static OneDimensionalPoint<double> Negate(OneDimensionalPoint<double> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new OneDimensionalPoint<double>
            {
                X = -value.X
            };
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public static int GetDistance(OneDimensionalPoint<int> pointA, OneDimensionalPoint<int> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            return pointB.X - pointA.X;
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public static long GetDistance(OneDimensionalPoint<long> pointA, OneDimensionalPoint<long> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            return pointB.X - pointA.X;
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public static float GetDistance(OneDimensionalPoint<float> pointA, OneDimensionalPoint<float> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            return pointB.X - pointA.X;
        }

        /// <summary>
        /// Gets the distance between specific two points.
        /// </summary>
        /// <param name="pointA">One of points to begin.</param>
        /// <param name="pointB">Another point to end.</param>
        /// <returns>A distance between two points.</returns>
        public static double GetDistance(OneDimensionalPoint<double> pointA, OneDimensionalPoint<double> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            return pointB.X - pointA.X;
        }

        #endregion

        #region 2D

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static TwoDimensionalPoint<int> Plus(TwoDimensionalPoint<int> leftValue, TwoDimensionalPoint<int> rightValue)
        {
            if (leftValue == null) leftValue = new TwoDimensionalPoint<int>();
            if (rightValue == null) rightValue = new TwoDimensionalPoint<int>();
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
        public static TwoDimensionalPoint<long> Plus(TwoDimensionalPoint<long> leftValue, TwoDimensionalPoint<long> rightValue)
        {
            if (leftValue == null) leftValue = new TwoDimensionalPoint<long>();
            if (rightValue == null) rightValue = new TwoDimensionalPoint<long>();
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
        public static TwoDimensionalPoint<float> Plus(TwoDimensionalPoint<float> leftValue, TwoDimensionalPoint<float> rightValue)
        {
            if (leftValue == null) leftValue = new TwoDimensionalPoint<float>();
            if (rightValue == null) rightValue = new TwoDimensionalPoint<float>();
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
        public static TwoDimensionalPoint<double> Plus(TwoDimensionalPoint<double> leftValue, TwoDimensionalPoint<double> rightValue)
        {
            if (leftValue == null) leftValue = new TwoDimensionalPoint<double>();
            if (rightValue == null) rightValue = new TwoDimensionalPoint<double>();
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
            if (leftValue == null) leftValue = new TwoDimensionalPoint<int>();
            if (rightValue == null) rightValue = new TwoDimensionalPoint<int>();
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
            if (leftValue == null) leftValue = new TwoDimensionalPoint<long>();
            if (rightValue == null) rightValue = new TwoDimensionalPoint<long>();
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
            if (leftValue == null) leftValue = new TwoDimensionalPoint<float>();
            if (rightValue == null) rightValue = new TwoDimensionalPoint<float>();
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
            if (leftValue == null) leftValue = new TwoDimensionalPoint<double>();
            if (rightValue == null) rightValue = new TwoDimensionalPoint<double>();
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
        public static TwoDimensionalPoint<float> Negate(TwoDimensionalPoint<float> value)
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
        public static double GetDistance(TwoDimensionalPoint<int> pointA, TwoDimensionalPoint<int> pointB)
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
        public static double GetDistance(TwoDimensionalPoint<long> pointA, TwoDimensionalPoint<long> pointB)
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
        public static double GetDistance(TwoDimensionalPoint<float> pointA, TwoDimensionalPoint<float> pointB)
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
        public static double GetDistance(TwoDimensionalPoint<double> pointA, TwoDimensionalPoint<double> pointB)
        {
            if (pointA == null)
                throw new ArgumentNullException("pointA");
            if (pointB == null)
                throw new ArgumentNullException("pointB");
            var numA = pointB.X - pointA.X;
            var numB = pointB.Y - pointA.Y;
            return Math.Sqrt(numA * numA + numB * numB);
        }

        #endregion

        #region 3D

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static ThreeDimensionalPoint<int> Plus(ThreeDimensionalPoint<int> leftValue, ThreeDimensionalPoint<int> rightValue)
        {
            if (leftValue == null) leftValue = new ThreeDimensionalPoint<int>();
            if (rightValue == null) rightValue = new ThreeDimensionalPoint<int>();
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
        public static ThreeDimensionalPoint<long> Plus(ThreeDimensionalPoint<long> leftValue, ThreeDimensionalPoint<long> rightValue)
        {
            if (leftValue == null) leftValue = new ThreeDimensionalPoint<long>();
            if (rightValue == null) rightValue = new ThreeDimensionalPoint<long>();
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
        public static ThreeDimensionalPoint<float> Plus(ThreeDimensionalPoint<float> leftValue, ThreeDimensionalPoint<float> rightValue)
        {
            if (leftValue == null) leftValue = new ThreeDimensionalPoint<float>();
            if (rightValue == null) rightValue = new ThreeDimensionalPoint<float>();
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
        public static ThreeDimensionalPoint<double> Plus(ThreeDimensionalPoint<double> leftValue, ThreeDimensionalPoint<double> rightValue)
        {
            if (leftValue == null) leftValue = new ThreeDimensionalPoint<double>();
            if (rightValue == null) rightValue = new ThreeDimensionalPoint<double>();
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
            if (leftValue == null) leftValue = new ThreeDimensionalPoint<int>();
            if (rightValue == null) rightValue = new ThreeDimensionalPoint<int>();
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
            if (leftValue == null) leftValue = new ThreeDimensionalPoint<long>();
            if (rightValue == null) rightValue = new ThreeDimensionalPoint<long>();
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
            if (leftValue == null) leftValue = new ThreeDimensionalPoint<float>();
            if (rightValue == null) rightValue = new ThreeDimensionalPoint<float>();
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
            if (leftValue == null) leftValue = new ThreeDimensionalPoint<double>();
            if (rightValue == null) rightValue = new ThreeDimensionalPoint<double>();
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
        public static double GetDistance(ThreeDimensionalPoint<int> pointA, ThreeDimensionalPoint<int> pointB)
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
        public static double GetDistance(ThreeDimensionalPoint<long> pointA, ThreeDimensionalPoint<long> pointB)
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
        public static double GetDistance(ThreeDimensionalPoint<float> pointA, ThreeDimensionalPoint<float> pointB)
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
        public static double GetDistance(ThreeDimensionalPoint<double> pointA, ThreeDimensionalPoint<double> pointB)
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

        #endregion

        #region 4D (T is number)

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static FourDimensionalPoint<int> Plus(FourDimensionalPoint<int> leftValue, FourDimensionalPoint<int> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<int>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<int>();
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
        public static FourDimensionalPoint<long> Plus(FourDimensionalPoint<long> leftValue, FourDimensionalPoint<long> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<long>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<long>();
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
        public static FourDimensionalPoint<float> Plus(FourDimensionalPoint<float> leftValue, FourDimensionalPoint<float> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<float>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<float>();
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
        public static FourDimensionalPoint<double> Plus(FourDimensionalPoint<double> leftValue, FourDimensionalPoint<double> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<double>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<double>();
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
            if (leftValue == null) leftValue = new FourDimensionalPoint<int>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<int>();
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
            if (leftValue == null) leftValue = new FourDimensionalPoint<long>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<long>();
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
            if (leftValue == null) leftValue = new FourDimensionalPoint<float>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<float>();
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
            if (leftValue == null) leftValue = new FourDimensionalPoint<double>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<double>();
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

        #endregion

        #region 4D (T is TimeSpan)

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static FourDimensionalPoint<int, TimeSpan> Plus(FourDimensionalPoint<int, TimeSpan> leftValue, FourDimensionalPoint<int, TimeSpan> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<int, TimeSpan>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<int, TimeSpan>();
            return new FourDimensionalPoint<int, TimeSpan>
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
        public static FourDimensionalPoint<long, TimeSpan> Plus(FourDimensionalPoint<long, TimeSpan> leftValue, FourDimensionalPoint<long, TimeSpan> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<long, TimeSpan>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<long, TimeSpan>();
            return new FourDimensionalPoint<long, TimeSpan>
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
        public static FourDimensionalPoint<float, TimeSpan> Plus(FourDimensionalPoint<float, TimeSpan> leftValue, FourDimensionalPoint<float, TimeSpan> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<float, TimeSpan>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<float, TimeSpan>();
            return new FourDimensionalPoint<float, TimeSpan>
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
        public static FourDimensionalPoint<double, TimeSpan> Plus(FourDimensionalPoint<double, TimeSpan> leftValue, FourDimensionalPoint<double, TimeSpan> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<double, TimeSpan>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<double, TimeSpan>();
            return new FourDimensionalPoint<double, TimeSpan>
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
        public static FourDimensionalPoint<int, TimeSpan> Minus(FourDimensionalPoint<int, TimeSpan> leftValue, FourDimensionalPoint<int, TimeSpan> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<int, TimeSpan>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<int, TimeSpan>();
            return new FourDimensionalPoint<int, TimeSpan>
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
        public static FourDimensionalPoint<long, TimeSpan> Minus(FourDimensionalPoint<long, TimeSpan> leftValue, FourDimensionalPoint<long, TimeSpan> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<long, TimeSpan>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<long, TimeSpan>();
            return new FourDimensionalPoint<long, TimeSpan>
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
        public static FourDimensionalPoint<float, TimeSpan> Minus(FourDimensionalPoint<float, TimeSpan> leftValue, FourDimensionalPoint<float, TimeSpan> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<float, TimeSpan>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<float, TimeSpan>();
            return new FourDimensionalPoint<float, TimeSpan>
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
        public static FourDimensionalPoint<double, TimeSpan> Minus(FourDimensionalPoint<double, TimeSpan> leftValue, FourDimensionalPoint<double, TimeSpan> rightValue)
        {
            if (leftValue == null) leftValue = new FourDimensionalPoint<double, TimeSpan>();
            if (rightValue == null) rightValue = new FourDimensionalPoint<double, TimeSpan>();
            return new FourDimensionalPoint<double, TimeSpan>
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
        public static FourDimensionalPoint<int, TimeSpan> Negate(FourDimensionalPoint<int, TimeSpan> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new FourDimensionalPoint<int, TimeSpan>
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
        public static FourDimensionalPoint<long, TimeSpan> Negate(FourDimensionalPoint<long, TimeSpan> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new FourDimensionalPoint<long, TimeSpan>
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
        public static FourDimensionalPoint<float, TimeSpan> Negate(FourDimensionalPoint<float, TimeSpan> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new FourDimensionalPoint<float, TimeSpan>
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
        public static FourDimensionalPoint<double, TimeSpan> Negate(FourDimensionalPoint<double, TimeSpan> value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return new FourDimensionalPoint<double, TimeSpan>
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z,
                T = -value.T
            };
        }

        #endregion
    }
}
