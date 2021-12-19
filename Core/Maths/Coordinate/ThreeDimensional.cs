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
    public class Point3D<TUnit> : ThreeElements<TUnit>, IEquatable<ThreeElements<TUnit>> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
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
            /// <param name="z">The value of Z.</param>
            public DataEventArgs(TUnit x, TUnit y, TUnit z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            /// <summary>
            /// Gets the value of X.
            /// </summary>
            public TUnit X { get; }

            /// <summary>
            /// Gets the value of Y.
            /// </summary>
            public TUnit Y { get; }

            /// <summary>
            /// Gets the value of Y.
            /// </summary>
            public TUnit Z { get; }
        }

        /// <summary>
        /// Initializes a new instance of the ThreeDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Point3D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ThreeDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <param name="z">The value of Z.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Point3D(TUnit x, TUnit y, TUnit z)
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
        /// Gets or sets the value of Z (depth). The value is same as ItemC.
        /// </summary>
        public TUnit Z
        {
            get => ItemC;
            set => ItemC = value;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
            => new Point3D<TUnit>(X, Y, Z);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(ThreeElements<TUnit> other)
            => other is not null && X.Equals(other.ItemA) && Y.Equals(other.ItemB) && Z.Equals(other.ItemC);

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
    public class DoublePoint3D : Point3D<double>, IAdditionCapable<DoublePoint3D>, ISubtractionCapable<DoublePoint3D>, INegationCapable<DoublePoint3D>, IEquatable<Point3D<double>>
    {
        /// <summary>
        /// Initializes a new instance of the DoubleThreeDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DoublePoint3D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DoubleThreeDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <param name="z">The value of Z.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DoublePoint3D(double x, double y, double z) : base(x, y, z)
        {
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoublePoint3D Plus(Point3D<double> value)
        {
            return value != null
                ? new DoublePoint3D(X + value.X, Y + value.Y, Z + value.Z)
                : new DoublePoint3D(X, Y, Z);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoublePoint3D Plus(Point3D<int> value)
        {
            return value != null
                ? new DoublePoint3D(X + value.X, Y + value.Y, Z + value.Z)
                : new DoublePoint3D(X, Y, Z);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoublePoint3D Plus(DoublePoint3D value)
        {
            return value != null
                ? new DoublePoint3D(X + value.X, Y + value.Y, Z + value.Z)
                : new DoublePoint3D(X, Y, Z);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoublePoint3D Minus(Point3D<double> value)
        {
            return value != null
                ? new DoublePoint3D(X - value.X, Y - value.Y, Z - value.Z)
                : new DoublePoint3D(X, Y, Z);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoublePoint3D Minus(Point3D<int> value)
        {
            return value != null
                ? new DoublePoint3D(X - value.X, Y - value.Y, Z - value.Z)
                : new DoublePoint3D(X, Y, Z);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoublePoint3D Minus(DoublePoint3D value)
        {
            return value != null
                ? new DoublePoint3D(X - value.X, Y - value.Y, Z - value.Z)
                : new DoublePoint3D(X, Y, Z);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public DoublePoint3D Negate()
        {
            return new DoublePoint3D(-X, -Y, -Z);
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
            obj.SetValue("z", Z);
            return obj;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
            => new DoublePoint3D(X, Y, Z);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(Point3D<double> other)
            => other is not null && (Math.Abs(X - other.X) < Arithmetic.DoubleAccuracy) && (Math.Abs(Y - other.Y) < Arithmetic.DoubleAccuracy) && (Math.Abs(X - other.X) < Arithmetic.DoubleAccuracy);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other is Point3D<double> p) return Equals(p);
            return base.Equals(other);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => base.GetHashCode();

        /// <summary>
        /// Converts a vector to the point.
        /// </summary>
        /// <param name="value">The vector to convert.</param>
        public static implicit operator DoublePoint3D(System.Numerics.Vector3 value)
        {
            return new DoublePoint3D(value.X, value.Y, value.Y);
        }

        /// <summary>
        /// Converts a vectorpoint.
        /// </summary>
        /// <param name="value">The point to convert.</param>
        public static implicit operator DoublePoint3D(Point3D<int> value)
        {
            return new DoublePoint3D(value.X, value.Y, value.Y);
        }

        /// <summary>
        /// Converts a vectorpoint.
        /// </summary>
        /// <param name="value">The point to convert.</param>
        public static implicit operator DoublePoint3D(Point3D<float> value)
        {
            return new DoublePoint3D(value.X, value.Y, value.Y);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static DoublePoint3D operator +(DoublePoint3D leftValue, DoublePoint3D rightValue)
        {
            return (leftValue ?? new DoublePoint3D()).Plus(rightValue);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static DoublePoint3D operator -(DoublePoint3D leftValue, DoublePoint3D rightValue)
        {
            return (leftValue ?? new DoublePoint3D()).Minus(rightValue);
        }
    }

    /// <summary>
    /// The point of 3D (stereoscophic) integer coordinate.
    /// </summary>
    public class Int32Point3D : Point3D<int>, IAdditionCapable<Int32Point3D>, ISubtractionCapable<Int32Point3D>, INegationCapable<Int32Point3D>, IEquatable<Point3D<int>>
    {
        /// <summary>
        /// Initializes a new instance of the Int32ThreeDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32Point3D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Int32ThreeDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <param name="z">The value of Z.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32Point3D(int x, int y, int z) : base(x, y, z)
        {
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public Int32Point3D Plus(Point3D<int> value)
        {
            return value != null
                ? new Int32Point3D(X + value.X, Y + value.Y, Z + value.Z)
                : new Int32Point3D(X, Y, Z);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public Int32Point3D Plus(Int32Point3D value)
        {
            return value != null
                ? new Int32Point3D(X + value.X, Y + value.Y, Z + value.Z)
                : new Int32Point3D(X, Y, Z);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public Int32Point3D Minus(Point3D<int> value)
        {
            return value != null
                ? new Int32Point3D(X - value.X, Y - value.Y, Z - value.Z)
                : new Int32Point3D(X, Y, Z);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public Int32Point3D Minus(Int32Point3D value)
        {
            return value != null
                ? new Int32Point3D(X - value.X, Y - value.Y, Z - value.Z)
                : new Int32Point3D(X, Y, Z);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public Int32Point3D Negate()
        {
            return new Int32Point3D(-X, -Y, -Z);
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
            obj.SetValue("z", Z);
            return obj;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
            => new Int32Point3D(X, Y, Z);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(Point3D<int> other)
            => other is not null && X == other.X && Y == other.Y && Z == other.Z;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other is Point3D<int> p) return Equals(p);
            return base.Equals(other);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => base.GetHashCode();

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Int32Point3D operator +(Int32Point3D leftValue, Int32Point3D rightValue)
        {
            return (leftValue ?? new Int32Point3D()).Plus(rightValue);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Int32Point3D operator -(Int32Point3D leftValue, Int32Point3D rightValue)
        {
            return (leftValue ?? new Int32Point3D()).Minus(rightValue);
        }
    }
}
