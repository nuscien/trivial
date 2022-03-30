using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Maths;

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
    public static Point1D<int> Plus(Point1D<int> leftValue, Point1D<int> rightValue)
    {
        if (leftValue is null) leftValue = new Point1D<int>();
        if (rightValue is null) rightValue = new Point1D<int>();
        return new Point1D<int>
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
    public static Point1D<long> Plus(Point1D<long> leftValue, Point1D<long> rightValue)
    {
        if (leftValue is null) leftValue = new Point1D<long>();
        if (rightValue is null) rightValue = new Point1D<long>();
        return new Point1D<long>
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
    public static Point1D<float> Plus(Point1D<float> leftValue, Point1D<float> rightValue)
    {
        if (leftValue is null) leftValue = new Point1D<float>();
        if (rightValue is null) rightValue = new Point1D<float>();
        return new Point1D<float>
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
    public static Point1D<double> Plus(Point1D<double> leftValue, Point1D<double> rightValue)
    {
        if (leftValue is null) leftValue = new Point1D<double>();
        if (rightValue is null) rightValue = new Point1D<double>();
        return new Point1D<double>
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
    public static Point1D<int> Minus(Point1D<int> leftValue, Point1D<int> rightValue)
    {
        if (leftValue is null) leftValue = new Point1D<int>();
        if (rightValue is null) rightValue = new Point1D<int>();
        return new Point1D<int>
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
    public static Point1D<long> Minus(Point1D<long> leftValue, Point1D<long> rightValue)
    {
        if (leftValue is null) leftValue = new Point1D<long>();
        if (rightValue is null) rightValue = new Point1D<long>();
        return new Point1D<long>
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
    public static Point1D<float> Minus(Point1D<float> leftValue, Point1D<float> rightValue)
    {
        if (leftValue is null) leftValue = new Point1D<float>();
        if (rightValue is null) rightValue = new Point1D<float>();
        return new Point1D<float>
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
    public static Point1D<double> Minus(Point1D<double> leftValue, Point1D<double> rightValue)
    {
        if (leftValue is null) leftValue = new Point1D<double>();
        if (rightValue is null) rightValue = new Point1D<double>();
        return new Point1D<double>
        {
            X = leftValue.X - rightValue.X
        };
    }

    /// <summary>
    /// Negates a specific point in coordinate.
    /// </summary>
    /// <param name="value">A value to create mirror.</param>
    /// <returns>A result mirrored with the specific point in coordinate.</returns>
    public static Point1D<int> Negate(Point1D<int> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point1D<int>
        {
            X = -value.X
        };
    }

    /// <summary>
    /// Negates a specific point in coordinate.
    /// </summary>
    /// <param name="value">A value to create mirror.</param>
    /// <returns>A result mirrored with the specific point in coordinate.</returns>
    public static Point1D<long> Negate(Point1D<long> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point1D<long>
        {
            X = -value.X
        };
    }

    /// <summary>
    /// Negates a specific point in coordinate.
    /// </summary>
    /// <param name="value">A value to create mirror.</param>
    /// <returns>A result mirrored with the specific point in coordinate.</returns>
    public static Point1D<float> Negate(Point1D<float> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point1D<float>
        {
            X = -value.X
        };
    }

    /// <summary>
    /// Negates a specific point in coordinate.
    /// </summary>
    /// <param name="value">A value to create mirror.</param>
    /// <returns>A result mirrored with the specific point in coordinate.</returns>
    public static Point1D<double> Negate(Point1D<double> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point1D<double>
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
    public static int GetDistance(Point1D<int> pointA, Point1D<int> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
            throw new ArgumentNullException("pointB");
        return pointB.X - pointA.X;
    }

    /// <summary>
    /// Gets the distance between specific two points.
    /// </summary>
    /// <param name="pointA">One of points to begin.</param>
    /// <param name="pointB">Another point to end.</param>
    /// <returns>A distance between two points.</returns>
    public static long GetDistance(Point1D<long> pointA, Point1D<long> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
            throw new ArgumentNullException("pointB");
        return pointB.X - pointA.X;
    }

    /// <summary>
    /// Gets the distance between specific two points.
    /// </summary>
    /// <param name="pointA">One of points to begin.</param>
    /// <param name="pointB">Another point to end.</param>
    /// <returns>A distance between two points.</returns>
    public static float GetDistance(Point1D<float> pointA, Point1D<float> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
            throw new ArgumentNullException("pointB");
        return pointB.X - pointA.X;
    }

    /// <summary>
    /// Gets the distance between specific two points.
    /// </summary>
    /// <param name="pointA">One of points to begin.</param>
    /// <param name="pointB">Another point to end.</param>
    /// <returns>A distance between two points.</returns>
    public static double GetDistance(Point1D<double> pointA, Point1D<double> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
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
    public static Point2D<int> Plus(Point2D<int> leftValue, Point2D<int> rightValue)
    {
        if (leftValue is null) leftValue = new Point2D<int>();
        if (rightValue is null) rightValue = new Point2D<int>();
        return new Point2D<int>
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
    public static Point2D<long> Plus(Point2D<long> leftValue, Point2D<long> rightValue)
    {
        if (leftValue is null) leftValue = new Point2D<long>();
        if (rightValue is null) rightValue = new Point2D<long>();
        return new Point2D<long>
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
    public static Point2D<float> Plus(Point2D<float> leftValue, Point2D<float> rightValue)
    {
        if (leftValue is null) leftValue = new Point2D<float>();
        if (rightValue is null) rightValue = new Point2D<float>();
        return new Point2D<float>
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
    public static Point2D<double> Plus(Point2D<double> leftValue, Point2D<double> rightValue)
    {
        if (leftValue is null) leftValue = new Point2D<double>();
        if (rightValue is null) rightValue = new Point2D<double>();
        return new Point2D<double>
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
    public static Point2D<int> Minus(Point2D<int> leftValue, Point2D<int> rightValue)
    {
        if (leftValue is null) leftValue = new Point2D<int>();
        if (rightValue is null) rightValue = new Point2D<int>();
        return new Point2D<int>
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
    public static Point2D<long> Minus(Point2D<long> leftValue, Point2D<long> rightValue)
    {
        if (leftValue is null) leftValue = new Point2D<long>();
        if (rightValue is null) rightValue = new Point2D<long>();
        return new Point2D<long>
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
    public static Point2D<float> Minus(Point2D<float> leftValue, Point2D<float> rightValue)
    {
        if (leftValue is null) leftValue = new Point2D<float>();
        if (rightValue is null) rightValue = new Point2D<float>();
        return new Point2D<float>
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
    public static Point2D<double> Minus(Point2D<double> leftValue, Point2D<double> rightValue)
    {
        if (leftValue is null) leftValue = new Point2D<double>();
        if (rightValue is null) rightValue = new Point2D<double>();
        return new Point2D<double>
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
    public static Point2D<int> Negate(Point2D<int> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point2D<int>
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
    public static Point2D<long> Negate(Point2D<long> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point2D<long>
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
    public static Point2D<float> Negate(Point2D<float> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point2D<float>
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
    public static Point2D<double> Negate(Point2D<double> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point2D<double>
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
    public static double GetDistance(Point2D<int> pointA, Point2D<int> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
            throw new ArgumentNullException("pointB");
        return Math.Sqrt((pointB.X - pointA.X) ^ 2 + (pointB.Y - pointA.Y) ^ 2);
    }

    /// <summary>
    /// Gets the distance between specific two points.
    /// </summary>
    /// <param name="pointA">One of points to begin.</param>
    /// <param name="pointB">Another point to end.</param>
    /// <returns>A distance between two points.</returns>
    public static double GetDistance(Point2D<long> pointA, Point2D<long> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
            throw new ArgumentNullException("pointB");
        return Math.Sqrt((pointB.X - pointA.X) ^ 2 + (pointB.Y - pointA.Y) ^ 2);
    }

    /// <summary>
    /// Gets the distance between specific two points.
    /// </summary>
    /// <param name="pointA">One of points to begin.</param>
    /// <param name="pointB">Another point to end.</param>
    /// <returns>A distance between two points.</returns>
    public static double GetDistance(Point2D<float> pointA, Point2D<float> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
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
    public static double GetDistance(Point2D<double> pointA, Point2D<double> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
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
    public static Point3D<int> Plus(Point3D<int> leftValue, Point3D<int> rightValue)
    {
        if (leftValue is null) leftValue = new Point3D<int>();
        if (rightValue is null) rightValue = new Point3D<int>();
        return new Point3D<int>
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
    public static Point3D<long> Plus(Point3D<long> leftValue, Point3D<long> rightValue)
    {
        if (leftValue is null) leftValue = new Point3D<long>();
        if (rightValue is null) rightValue = new Point3D<long>();
        return new Point3D<long>
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
    public static Point3D<float> Plus(Point3D<float> leftValue, Point3D<float> rightValue)
    {
        if (leftValue is null) leftValue = new Point3D<float>();
        if (rightValue is null) rightValue = new Point3D<float>();
        return new Point3D<float>
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
    public static Point3D<double> Plus(Point3D<double> leftValue, Point3D<double> rightValue)
    {
        if (leftValue is null) leftValue = new Point3D<double>();
        if (rightValue is null) rightValue = new Point3D<double>();
        return new Point3D<double>
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
    public static Point3D<int> Minus(Point3D<int> leftValue, Point3D<int> rightValue)
    {
        if (leftValue is null) leftValue = new Point3D<int>();
        if (rightValue is null) rightValue = new Point3D<int>();
        return new Point3D<int>
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
    public static Point3D<long> Minus(Point3D<long> leftValue, Point3D<long> rightValue)
    {
        if (leftValue is null) leftValue = new Point3D<long>();
        if (rightValue is null) rightValue = new Point3D<long>();
        return new Point3D<long>
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
    public static Point3D<float> Minus(Point3D<float> leftValue, Point3D<float> rightValue)
    {
        if (leftValue is null) leftValue = new Point3D<float>();
        if (rightValue is null) rightValue = new Point3D<float>();
        return new Point3D<float>
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
    public static Point3D<double> Minus(Point3D<double> leftValue, Point3D<double> rightValue)
    {
        if (leftValue is null) leftValue = new Point3D<double>();
        if (rightValue is null) rightValue = new Point3D<double>();
        return new Point3D<double>
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
    public static Point3D<int> Negate(Point3D<int> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point3D<int>
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
    public static Point3D<long> Negate(Point3D<long> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point3D<long>
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
    public static Point3D<float> Negate(Point3D<float> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point3D<float>
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
    public static Point3D<double> Negate(Point3D<double> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point3D<double>
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
    public static double GetDistance(Point3D<int> pointA, Point3D<int> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
            throw new ArgumentNullException("pointB");
        return Math.Sqrt((pointB.X - pointA.X) ^ 2 + (pointB.Y - pointA.Y) ^ 2 + (pointB.Z - pointA.Z) ^ 2);
    }

    /// <summary>
    /// Gets the distance between specific two points.
    /// </summary>
    /// <param name="pointA">One of points to begin.</param>
    /// <param name="pointB">Another point to end.</param>
    /// <returns>A distance between two points.</returns>
    public static double GetDistance(Point3D<long> pointA, Point3D<long> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
            throw new ArgumentNullException("pointB");
        return Math.Sqrt((pointB.X - pointA.X) ^ 2 + (pointB.Y - pointA.Y) ^ 2 + (pointB.Z - pointA.Z) ^ 2);
    }

    /// <summary>
    /// Gets the distance between specific two points.
    /// </summary>
    /// <param name="pointA">One of points to begin.</param>
    /// <param name="pointB">Another point to end.</param>
    /// <returns>A distance between two points.</returns>
    public static double GetDistance(Point3D<float> pointA, Point3D<float> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
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
    public static double GetDistance(Point3D<double> pointA, Point3D<double> pointB)
    {
        if (pointA is null)
            throw new ArgumentNullException("pointA");
        if (pointB is null)
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
    public static Point4D<int> Plus(Point4D<int> leftValue, Point4D<int> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<int>();
        if (rightValue is null) rightValue = new Point4D<int>();
        return new Point4D<int>
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
    public static Point4D<long> Plus(Point4D<long> leftValue, Point4D<long> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<long>();
        if (rightValue is null) rightValue = new Point4D<long>();
        return new Point4D<long>
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
    public static Point4D<float> Plus(Point4D<float> leftValue, Point4D<float> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<float>();
        if (rightValue is null) rightValue = new Point4D<float>();
        return new Point4D<float>
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
    public static Point4D<double> Plus(Point4D<double> leftValue, Point4D<double> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<double>();
        if (rightValue is null) rightValue = new Point4D<double>();
        return new Point4D<double>
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
    public static Point4D<int> Minus(Point4D<int> leftValue, Point4D<int> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<int>();
        if (rightValue is null) rightValue = new Point4D<int>();
        return new Point4D<int>
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
    public static Point4D<long> Minus(Point4D<long> leftValue, Point4D<long> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<long>();
        if (rightValue is null) rightValue = new Point4D<long>();
        return new Point4D<long>
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
    public static Point4D<float> Minus(Point4D<float> leftValue, Point4D<float> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<float>();
        if (rightValue is null) rightValue = new Point4D<float>();
        return new Point4D<float>
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
    public static Point4D<double> Minus(Point4D<double> leftValue, Point4D<double> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<double>();
        if (rightValue is null) rightValue = new Point4D<double>();
        return new Point4D<double>
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
    public static Point4D<int> Negate(Point4D<int> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point4D<int>
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
    public static Point4D<long> Negate(Point4D<long> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point4D<long>
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
    public static Point4D<float> Negate(Point4D<float> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point4D<float>
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
    public static Point4D<double> Negate(Point4D<double> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point4D<double>
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
    public static Point4D<int, TimeSpan> Plus(Point4D<int, TimeSpan> leftValue, Point4D<int, TimeSpan> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<int, TimeSpan>();
        if (rightValue is null) rightValue = new Point4D<int, TimeSpan>();
        return new Point4D<int, TimeSpan>
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
    public static Point4D<long, TimeSpan> Plus(Point4D<long, TimeSpan> leftValue, Point4D<long, TimeSpan> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<long, TimeSpan>();
        if (rightValue is null) rightValue = new Point4D<long, TimeSpan>();
        return new Point4D<long, TimeSpan>
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
    public static Point4D<float, TimeSpan> Plus(Point4D<float, TimeSpan> leftValue, Point4D<float, TimeSpan> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<float, TimeSpan>();
        if (rightValue is null) rightValue = new Point4D<float, TimeSpan>();
        return new Point4D<float, TimeSpan>
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
    public static Point4D<double, TimeSpan> Plus(Point4D<double, TimeSpan> leftValue, Point4D<double, TimeSpan> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<double, TimeSpan>();
        if (rightValue is null) rightValue = new Point4D<double, TimeSpan>();
        return new Point4D<double, TimeSpan>
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
    public static Point4D<int, TimeSpan> Minus(Point4D<int, TimeSpan> leftValue, Point4D<int, TimeSpan> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<int, TimeSpan>();
        if (rightValue is null) rightValue = new Point4D<int, TimeSpan>();
        return new Point4D<int, TimeSpan>
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
    public static Point4D<long, TimeSpan> Minus(Point4D<long, TimeSpan> leftValue, Point4D<long, TimeSpan> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<long, TimeSpan>();
        if (rightValue is null) rightValue = new Point4D<long, TimeSpan>();
        return new Point4D<long, TimeSpan>
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
    public static Point4D<float, TimeSpan> Minus(Point4D<float, TimeSpan> leftValue, Point4D<float, TimeSpan> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<float, TimeSpan>();
        if (rightValue is null) rightValue = new Point4D<float, TimeSpan>();
        return new Point4D<float, TimeSpan>
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
    public static Point4D<double, TimeSpan> Minus(Point4D<double, TimeSpan> leftValue, Point4D<double, TimeSpan> rightValue)
    {
        if (leftValue is null) leftValue = new Point4D<double, TimeSpan>();
        if (rightValue is null) rightValue = new Point4D<double, TimeSpan>();
        return new Point4D<double, TimeSpan>
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
    public static Point4D<int, TimeSpan> Negate(Point4D<int, TimeSpan> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point4D<int, TimeSpan>
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
    public static Point4D<long, TimeSpan> Negate(Point4D<long, TimeSpan> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point4D<long, TimeSpan>
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
    public static Point4D<float, TimeSpan> Negate(Point4D<float, TimeSpan> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point4D<float, TimeSpan>
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
    public static Point4D<double, TimeSpan> Negate(Point4D<double, TimeSpan> value)
    {
        if (value is null)
            throw new ArgumentNullException("value");
        return new Point4D<double, TimeSpan>
        {
            X = -value.X,
            Y = -value.Y,
            Z = -value.Z,
            T = -value.T
        };
    }

    #endregion

    #region JSON

    #endregion
}
