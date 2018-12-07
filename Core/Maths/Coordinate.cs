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
    /// The polar coordinate point.
    /// </summary>
    public class PolarPoint
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
        public PolarPoint(float r, Angle theta)
        {
            Radius = r;
            Theta = theta;
        }

        /// <summary>
        /// Gets or sets the length between center point and the specific point (r).
        /// </summary>
        public float Radius
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
        /// Returns a tuple that represents the values of current coordinate point object.
        /// </summary>
        /// <returns>The tuple representation of this coordinate point object.</returns>
        public Tuple<float, Angle> ToTuple()
        {
            return new Tuple<float, Angle>(Radius, Theta);
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
    }

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
