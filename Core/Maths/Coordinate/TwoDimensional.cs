// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinate\TwoDimensional.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The 2D models.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Trivial.Maths
{
    /// <summary>
    /// Quadrants.
    /// </summary>
    public enum Quadrants
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
    public class TwoDimensionalPoint<TUnit> : TwoElements<TUnit>, IEquatable<TwoElements<TUnit>> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
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
            get => ItemA;
            set => ItemA = value;
        }

        /// <summary>
        /// Gets or sets the value of Y (vertical position). The value is same as ItemB.
        /// </summary>
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
        {
            return new TwoDimensionalPoint<TUnit>(X, Y);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(TwoElements<TUnit> other)
        {
            return other != null && X.Equals(other.ItemA) && Y.Equals(other.ItemB);
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
    /// The point of 2D (flat) mathematics coordinate.
    /// </summary>
    public class DoubleTwoDimensionalPoint : TwoDimensionalPoint<double>, IAdditionCapable<DoubleTwoDimensionalPoint>, ISubtractionCapable<DoubleTwoDimensionalPoint>, INegationCapable<DoubleTwoDimensionalPoint>
    {
        /// <summary>
        /// Initializes a new instance of the DoubleTwoDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DoubleTwoDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DoubleTwoDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DoubleTwoDimensionalPoint(double x, double y) : base(x, y)
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
        public DoubleTwoDimensionalPoint Plus(TwoDimensionalPoint<double> value)
        {
            return value != null
                ? new DoubleTwoDimensionalPoint(X + value.X, Y + value.Y)
                : new DoubleTwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoubleTwoDimensionalPoint Plus(TwoDimensionalPoint<int> value)
        {
            return value != null
                ? new DoubleTwoDimensionalPoint(X + value.X, Y + value.Y)
                : new DoubleTwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoubleTwoDimensionalPoint Plus(DoubleTwoDimensionalPoint value)
        {
            return value != null
                ? new DoubleTwoDimensionalPoint(X + value.X, Y + value.Y)
                : new DoubleTwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoubleTwoDimensionalPoint Minus(TwoDimensionalPoint<double> value)
        {
            return value != null
                ? new DoubleTwoDimensionalPoint(X - value.X, Y - value.Y)
                : new DoubleTwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoubleTwoDimensionalPoint Minus(TwoDimensionalPoint<int> value)
        {
            return value != null
                ? new DoubleTwoDimensionalPoint(X - value.X, Y - value.Y)
                : new DoubleTwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoubleTwoDimensionalPoint Minus(DoubleTwoDimensionalPoint value)
        {
            return value != null
                ? new DoubleTwoDimensionalPoint(X - value.X, Y - value.Y)
                : new DoubleTwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public DoubleTwoDimensionalPoint Negate()
        {
            return new DoubleTwoDimensionalPoint(-X, -Y);
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
        {
            return new DoubleTwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Converts a vector to the point.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        public static implicit operator DoubleTwoDimensionalPoint(System.Numerics.Vector2 value)
            => new(value.X, value.Y);

        /// <summary>
        /// Converts from point.
        /// </summary>
        /// <param name="p">The point value.</param>
        /// <returns>An instance of the point class.</returns>
        public static implicit operator DoubleTwoDimensionalPoint(System.Drawing.PointF p)
            => new(p.X, p.Y);

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static DoubleTwoDimensionalPoint operator +(DoubleTwoDimensionalPoint leftValue, DoubleTwoDimensionalPoint rightValue)
            => (leftValue ?? new DoubleTwoDimensionalPoint()).Plus(rightValue);

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static DoubleTwoDimensionalPoint operator -(DoubleTwoDimensionalPoint leftValue, DoubleTwoDimensionalPoint rightValue)
            => (leftValue ?? new DoubleTwoDimensionalPoint()).Minus(rightValue);
    }

    /// <summary>
    /// The point of 2D (flat) integer coordinate.
    /// </summary>
    public class Int32TwoDimensionalPoint : TwoDimensionalPoint<int>, IAdditionCapable<Int32TwoDimensionalPoint>, ISubtractionCapable<Int32TwoDimensionalPoint>, INegationCapable<Int32TwoDimensionalPoint>
    {
        /// <summary>
        /// Initializes a new instance of the Int32TwoDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32TwoDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Int32TwoDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32TwoDimensionalPoint(int x, int y) : base(x, y)
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
        public Int32TwoDimensionalPoint Plus(TwoDimensionalPoint<int> value)
        {
            return value != null
                ? new Int32TwoDimensionalPoint(X + value.X, Y + value.Y)
                : new Int32TwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public Int32TwoDimensionalPoint Plus(Int32TwoDimensionalPoint value)
        {
            return value != null
                ? new Int32TwoDimensionalPoint(X + value.X, Y + value.Y)
                : new Int32TwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public Int32TwoDimensionalPoint Minus(TwoDimensionalPoint<int> value)
        {
            return value != null
                ? new Int32TwoDimensionalPoint(X - value.X, Y - value.Y)
                : new Int32TwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public Int32TwoDimensionalPoint Minus(Int32TwoDimensionalPoint value)
        {
            return value != null
                ? new Int32TwoDimensionalPoint(X - value.X, Y - value.Y)
                : new Int32TwoDimensionalPoint(X, Y);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public Int32TwoDimensionalPoint Negate()
        {
            return new Int32TwoDimensionalPoint(-X, -Y);
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
            => new Int32TwoDimensionalPoint(X, Y);

        /// <summary>
        /// Converts to point.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>An instance of the point.</returns>
        public static explicit operator System.Drawing.Point(Int32TwoDimensionalPoint p)
            => new(p.X, p.Y);

        /// <summary>
        /// Converts from point.
        /// </summary>
        /// <param name="p">The point value.</param>
        /// <returns>An instance of the point class.</returns>
        public static implicit operator Int32TwoDimensionalPoint(System.Drawing.Point p)
            => new(p.X, p.Y);

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Int32TwoDimensionalPoint operator +(Int32TwoDimensionalPoint leftValue, Int32TwoDimensionalPoint rightValue)
            => (leftValue ?? new Int32TwoDimensionalPoint()).Plus(rightValue);

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Int32TwoDimensionalPoint operator -(Int32TwoDimensionalPoint leftValue, Int32TwoDimensionalPoint rightValue)
            => (leftValue ?? new Int32TwoDimensionalPoint()).Minus(rightValue);
    }
}
