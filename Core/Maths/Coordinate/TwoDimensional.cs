﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinate\TwoDimensional.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The 2D models.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text.Json.Serialization;

namespace Trivial.Maths
{
    /// <summary>
    /// Quadrants.
    /// </summary>
    public enum Quadrants : byte
    {
        /// <summary>
        /// Origin, X is 0 and Y is 0.
        /// </summary>
        Origin = 0,

        /// <summary>
        /// Quadrant I, X is positive and Y is postivie.
        /// </summary>
        I = 1,

        /// <summary>
        /// Quadrant II, X is positive and Y is negative.
        /// </summary>
        II = 2,

        /// <summary>
        /// Quadrant III, X is negative and Y is negative.
        /// </summary>
        III = 3,

        /// <summary>
        /// Quadrant IV, X is negative and Y is positive.
        /// </summary>
        IV = 4,

        /// <summary>
        /// The positive X-axis part.
        /// </summary>
        PositiveXAxis = 5,

        /// <summary>
        /// The negative X-axis part.
        /// </summary>
        NegativeXAxis = 6,

        /// <summary>
        /// The positive Y-axis part.
        /// </summary>
        PositiveYAxis = 7,

        /// <summary>
        /// The negative Y-axis part.
        /// </summary>
        NegativeYAxis = 8,
    }

    /// <summary>
    /// The generic 2D (flat) coordinate point.
    /// </summary>
    /// <typeparam name="TUnit">The type of unit.</typeparam>
    public class Point2D<TUnit> : TwoElements<TUnit>, IEquatable<TwoElements<TUnit>> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
    {
        /// <summary>
        /// The event arguments with the position.
        /// </summary>
        public class DataEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the DataEventArgs class.
            /// </summary>
            /// <param name="x">The value of X.</param>
            /// <param name="y">The value of Y.</param>
            public DataEventArgs(TUnit x, TUnit y)
            {
                X = x;
                Y = y;
            }

            /// <summary>
            /// Gets the value of X.
            /// </summary>
            public TUnit X { get; }

            /// <summary>
            /// Gets the value of Y.
            /// </summary>
            public TUnit Y { get; }
        }

        /// <summary>
        /// Initializes a new instance of the TwoDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Point2D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TwoDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Point2D(TUnit x, TUnit y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets or sets the value of X (horizontal position). The value is same as ItemA.
        /// </summary>
        [JsonPropertyName("x")]
        public TUnit X
        {
            get => ItemA;
            set => ItemA = value;
        }

        /// <summary>
        /// Gets or sets the value of Y (vertical position). The value is same as ItemB.
        /// </summary>
        [JsonPropertyName("y")]
        public TUnit Y
        {
            get => ItemB;
            set => ItemB = value;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
            => new Point2D<TUnit>(X, Y);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(TwoElements<TUnit> other)
            => other is not null && X.Equals(other.ItemA) && Y.Equals(other.ItemB);

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
    /// The point of 2D (flat) mathematics coordinate.
    /// </summary>
    public class DoublePoint2D : Point2D<double>, IAdditionCapable<DoublePoint2D>, ISubtractionCapable<DoublePoint2D>, INegationCapable<DoublePoint2D>, IEquatable<Point2D<double>>
    {
        /// <summary>
        /// Initializes a new instance of the DoubleTwoDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DoublePoint2D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DoubleTwoDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DoublePoint2D(double x, double y) : base(x, y)
        {
        }

        /// <summary>
        /// Gets the quadrant.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Quadrants Quadrant
        {
            get
            {
                if (X == 0)
                {
                    if (Y == 0) return Quadrants.Origin;
                    return Y > 0 ? Quadrants.PositiveYAxis : Quadrants.NegativeYAxis;
                }

                if (X > 0)
                {
                    if (Y == 0) return Quadrants.PositiveXAxis;
                    return Y > 0 ? Quadrants.I : Quadrants.IV;
                }

                if (Y == 0) return Quadrants.NegativeXAxis;
                return Y > 0 ? Quadrants.II : Quadrants.III;
            }
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoublePoint2D Plus(Point2D<double> value)
        {
            return value != null
                ? new DoublePoint2D(X + value.X, Y + value.Y)
                : new DoublePoint2D(X, Y);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoublePoint2D Plus(Point2D<int> value)
        {
            return value != null
                ? new DoublePoint2D(X + value.X, Y + value.Y)
                : new DoublePoint2D(X, Y);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoublePoint2D Plus(DoublePoint2D value)
        {
            return value != null
                ? new DoublePoint2D(X + value.X, Y + value.Y)
                : new DoublePoint2D(X, Y);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoublePoint2D Minus(Point2D<double> value)
        {
            return value != null
                ? new DoublePoint2D(X - value.X, Y - value.Y)
                : new DoublePoint2D(X, Y);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoublePoint2D Minus(Point2D<int> value)
        {
            return value != null
                ? new DoublePoint2D(X - value.X, Y - value.Y)
                : new DoublePoint2D(X, Y);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoublePoint2D Minus(DoublePoint2D value)
        {
            return value != null
                ? new DoublePoint2D(X - value.X, Y - value.Y)
                : new DoublePoint2D(X, Y);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public DoublePoint2D Negate()
        {
            return new DoublePoint2D(-X, -Y);
        }

        /// <summary>
        /// Converts to an instance of JSON.
        /// </summary>
        /// <returns>A JSON object instance.</returns>
        public Text.JsonObjectNode ToJson()
        {
            return ToJson(new Text.JsonObjectNode());
        }

        /// <summary>
        /// Converts to an instance of JSON.
        /// </summary>
        /// <param name="obj">The optional JSON object instance to add properties.</param>
        /// <returns>A JSON object instance.</returns>
        public Text.JsonObjectNode ToJson(Text.JsonObjectNode obj)
        {
            if (obj is null) obj = new Text.JsonObjectNode();
            obj.SetValue("x", X);
            obj.SetValue("y", Y);
            return obj;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
            => new DoublePoint2D(X, Y);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(Point2D<double> other)
            => other is not null && (Math.Abs(X - other.X) < Arithmetic.DoubleAccuracy) && (Math.Abs(Y - other.Y) < Arithmetic.DoubleAccuracy);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other is Point2D<double> p) return Equals(p);
            return base.Equals(other);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => base.GetHashCode();

        /// <summary>
        /// Converts to a point.
        /// </summary>
        /// <param name="value">The point converted.</param>
        /// <returns>An instance of the point class.</returns>
        public static explicit operator System.Numerics.Vector2(DoublePoint2D value)
            => new((float)value.X, (float)value.Y);

        /// <summary>
        /// Converts to a point.
        /// </summary>
        /// <param name="value">The point converted.</param>
        /// <returns>An instance of the point class.</returns>
        public static explicit operator System.Drawing.PointF(DoublePoint2D value)
            => new((float)value.X, (float)value.Y);

        /// <summary>
        /// Converts a point.
        /// </summary>
        /// <param name="value">The point to convert.</param>
        public static implicit operator DoublePoint2D(Point2D<int> value)
            => new(value.X, value.Y);

        /// <summary>
        /// Converts a point.
        /// </summary>
        /// <param name="value">The point to convert.</param>
        public static implicit operator DoublePoint2D(Point2D<float> value)
            => new(value.X, value.Y);

        /// <summary>
        /// Converts a vector to the point.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        /// <returns>An instance of the point class.</returns>
        public static implicit operator DoublePoint2D(System.Numerics.Vector2 value)
            => new(value.X, value.Y);

        /// <summary>
        /// Converts from point.
        /// </summary>
        /// <param name="p">The point value.</param>
        /// <returns>An instance of the point class.</returns>
        public static implicit operator DoublePoint2D(System.Drawing.PointF p)
            => new(p.X, p.Y);

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static DoublePoint2D operator +(DoublePoint2D leftValue, DoublePoint2D rightValue)
            => (leftValue ?? new DoublePoint2D()).Plus(rightValue);

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static DoublePoint2D operator -(DoublePoint2D leftValue, DoublePoint2D rightValue)
            => (leftValue ?? new DoublePoint2D()).Minus(rightValue);
    }

    /// <summary>
    /// The point of 2D (flat) integer coordinate.
    /// </summary>
    public class Int32Point2D : Point2D<int>, IAdditionCapable<Int32Point2D>, ISubtractionCapable<Int32Point2D>, INegationCapable<Int32Point2D>, IEquatable<Point2D<int>>
    {
        /// <summary>
        /// Initializes a new instance of the Int32TwoDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32Point2D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Int32TwoDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32Point2D(int x, int y) : base(x, y)
        {
        }

        /// <summary>
        /// Gets the quadrant.
        /// </summary>
        public Quadrants Quadrant
        {
            get
            {
                if (X == 0)
                {
                    if (Y == 0) return Quadrants.Origin;
                    return Y > 0 ? Quadrants.PositiveYAxis : Quadrants.NegativeYAxis;
                }

                if (X > 0)
                {
                    if (Y == 0) return Quadrants.PositiveXAxis;
                    return Y > 0 ? Quadrants.I : Quadrants.IV;
                }

                if (Y == 0) return Quadrants.NegativeXAxis;
                return Y > 0 ? Quadrants.II : Quadrants.III;
            }
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public Int32Point2D Plus(Point2D<int> value)
        {
            return value != null
                ? new Int32Point2D(X + value.X, Y + value.Y)
                : new Int32Point2D(X, Y);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public Int32Point2D Plus(Int32Point2D value)
        {
            return value != null
                ? new Int32Point2D(X + value.X, Y + value.Y)
                : new Int32Point2D(X, Y);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public Int32Point2D Minus(Point2D<int> value)
        {
            return value != null
                ? new Int32Point2D(X - value.X, Y - value.Y)
                : new Int32Point2D(X, Y);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public Int32Point2D Minus(Int32Point2D value)
        {
            return value != null
                ? new Int32Point2D(X - value.X, Y - value.Y)
                : new Int32Point2D(X, Y);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public Int32Point2D Negate()
        {
            return new Int32Point2D(-X, -Y);
        }

        /// <summary>
        /// Converts to an instance of JSON.
        /// </summary>
        /// <returns>A JSON object instance.</returns>
        public Text.JsonObjectNode ToJson()
        {
            return ToJson(new Text.JsonObjectNode());
        }

        /// <summary>
        /// Converts to an instance of JSON.
        /// </summary>
        /// <param name="obj">The optional JSON object instance to add properties.</param>
        /// <returns>A JSON object instance.</returns>
        public Text.JsonObjectNode ToJson(Text.JsonObjectNode obj)
        {
            if (obj is null) obj = new Text.JsonObjectNode();
            obj.SetValue("x", X);
            obj.SetValue("y", Y);
            return obj;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
            => new Int32Point2D(X, Y);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(Point2D<int> other)
            => other is not null && X == other.X && Y == other.Y;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other is Point2D<int> p) return Equals(p);
            return base.Equals(other);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => base.GetHashCode();

        /// <summary>
        /// Converts to point.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>An instance of the point.</returns>
        public static explicit operator System.Drawing.Point(Int32Point2D p)
            => new(p.X, p.Y);

        /// <summary>
        /// Converts from point.
        /// </summary>
        /// <param name="p">The point value.</param>
        /// <returns>An instance of the point class.</returns>
        public static implicit operator Int32Point2D(System.Drawing.Point p)
            => new(p.X, p.Y);

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Int32Point2D operator +(Int32Point2D leftValue, Int32Point2D rightValue)
            => (leftValue ?? new Int32Point2D()).Plus(rightValue);

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Int32Point2D operator -(Int32Point2D leftValue, Int32Point2D rightValue)
            => (leftValue ?? new Int32Point2D()).Minus(rightValue);
    }
}
