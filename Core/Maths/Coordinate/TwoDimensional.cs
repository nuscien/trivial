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
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

using Trivial.Text;

namespace Trivial.Maths;

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
public class Point2D<TUnit> : TwoElements<TUnit>, IEquatable<Point2D<TUnit>>, IEquatable<TwoElements<TUnit>> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
{
    /// <summary>
    /// The event arguments with the position.
    /// </summary>
    /// <param name="x">The value of X.</param>
    /// <param name="y">The value of Y.</param>
    public class DataEventArgs(TUnit x, TUnit y) : EventArgs
    {
        /// <summary>
        /// Gets the value of X.
        /// </summary>
        public TUnit X { get; } = x;

        /// <summary>
        /// Gets the value of Y.
        /// </summary>
        public TUnit Y { get; } = y;
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
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
    public virtual bool Equals(Point2D<TUnit> other)
        => other is not null && X.Equals(other.X) && Y.Equals(other.Y);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other is Point2D<TUnit> p) return Equals(p);
        if (other is TwoElements<TUnit> e) return Equals(e);
        return base.Equals(other);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
        => base.GetHashCode();

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
[JsonConverter(typeof(DoublePoint2DConverter))]
public sealed class DoublePoint2D : Point2D<double>, IAdditionCapable<DoublePoint2D>, ISubtractionCapable<DoublePoint2D>, INegationCapable<DoublePoint2D>, IEquatable<System.Drawing.PointF>, IEquatable<Vector2>, IEquatable<Point2D<double>>, IEquatable<Point2D<float>>, IEquatable<Point2D<int>>, IJsonObjectHost
#if NETCOREAPP
    , IAdditionOperators<DoublePoint2D, DoublePoint2D, DoublePoint2D>, ISubtractionOperators<DoublePoint2D, DoublePoint2D, DoublePoint2D>, IUnaryNegationOperators<DoublePoint2D, DoublePoint2D>
#endif
{
    /// <summary>
    /// Initializes a new instance of the DoublePoint2D class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DoublePoint2D()
    {
    }

    /// <summary>
    /// Initializes a new instance of the DoublePoint2D class.
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
        => value != null ? new(X + value.X, Y + value.Y) : new(X, Y);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public DoublePoint2D Plus(Point2D<float> value)
        => value != null ? new(X + value.X, Y + value.Y) : new(X, Y);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public DoublePoint2D Plus(Point2D<int> value)
        => value != null ? new(X + value.X, Y + value.Y) : new(X, Y);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public DoublePoint2D Plus(DoublePoint2D value)
        => value != null ? new(X + value.X, Y + value.Y) : new(X, Y);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public DoublePoint2D Minus(Point2D<double> value)
        => value != null ? new(X - value.X, Y - value.Y) : new(X, Y);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public DoublePoint2D Minus(Point2D<float> value)
        => value != null ? new(X - value.X, Y - value.Y) : new(X, Y);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public DoublePoint2D Minus(Point2D<int> value)
        => value != null ? new(X - value.X, Y - value.Y) : new(X, Y);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public DoublePoint2D Minus(DoublePoint2D value)
        => value != null ? new(X - value.X, Y - value.Y) : new(X, Y);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// -this
    /// </summary>
    /// <returns>A result after negation.</returns>
    public DoublePoint2D Negate()
        => new(-X, -Y);

    /// <summary>
    /// Converts to an instance of JSON.
    /// </summary>
    /// <returns>A JSON object instance.</returns>
    public JsonObjectNode ToJson()
    {
        var json = new JsonObjectNode();
        ToJson(json);
        return json;
    }

    /// <summary>
    /// Converts to an instance of JSON.
    /// </summary>
    /// <param name="obj">The optional JSON object instance to add properties.</param>
    /// <returns>A JSON object instance.</returns>
    public void ToJson(JsonObjectNode obj)
    {
        if (obj is null) return;
        obj.SetValue("x", X);
        obj.SetValue("y", Y);
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
    public bool Equals(System.Drawing.PointF other)
        => X == other.X && Y == other.Y;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(Vector2 other)
        => X == other.X && Y == other.Y;

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
    public bool Equals(Point2D<float> other)
        => other is not null && (Math.Abs(X - other.X) < Arithmetic.DoubleAccuracy) && (Math.Abs(Y - other.Y) < Arithmetic.DoubleAccuracy);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(Point2D<double> other)
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
        if (other is Point2D<float> p1) return Equals(p1);
        if (other is Point2D<int> p2) return Equals(p2);
        if (other is Vector2 p3) return Equals(p3);
        if (other is System.Drawing.PointF p4) return Equals(p4);
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
    public static explicit operator Vector2(DoublePoint2D value)
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

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// </summary>
    /// <param name="value">The value to negate.</param>
    /// <returns>A result after negation.</returns>
    public static DoublePoint2D operator -(DoublePoint2D value)
        => value?.Negate();
}

/// <summary>
/// The point of 2D (flat) integer coordinate.
/// </summary>
[JsonConverter(typeof(IntPoint2DConverter))]
public sealed class IntPoint2D : Point2D<int>, IAdditionCapable<IntPoint2D>, ISubtractionCapable<IntPoint2D>, INegationCapable<IntPoint2D>, IEquatable<System.Drawing.Point>, IEquatable<Point2D<int>>, IEquatable<Point2D<double>>, IJsonObjectHost
#if NETCOREAPP
    , IAdditionOperators<IntPoint2D, IntPoint2D, IntPoint2D>, ISubtractionOperators<IntPoint2D, IntPoint2D, IntPoint2D>, IUnaryNegationOperators<IntPoint2D, IntPoint2D>
#endif
{
    /// <summary>
    /// Initializes a new instance of the IntPoint2D class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public IntPoint2D()
    {
    }

    /// <summary>
    /// Initializes a new instance of the IntPoint2D class.
    /// </summary>
    /// <param name="x">The value of X.</param>
    /// <param name="y">The value of Y.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public IntPoint2D(int x, int y) : base(x, y)
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
    public IntPoint2D Plus(Point2D<int> value)
        => value != null ? new(X + value.X, Y + value.Y) : new(X, Y);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public IntPoint2D Plus(IntPoint2D value)
        => value != null ? new(X + value.X, Y + value.Y) : new(X, Y);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public IntPoint2D Minus(Point2D<int> value)
        => value != null ? new(X - value.X, Y - value.Y) : new(X, Y);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public IntPoint2D Minus(IntPoint2D value)
        => value != null ? new(X - value.X, Y - value.Y) : new(X, Y);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// -this
    /// </summary>
    /// <returns>A result after negation.</returns>
    public IntPoint2D Negate()
        => new(-X, -Y);

    /// <summary>
    /// Converts to an instance of JSON.
    /// </summary>
    /// <returns>A JSON object instance.</returns>
    public JsonObjectNode ToJson()
    {
        var json = new JsonObjectNode();
        ToJson(json);
        return json;
    }

    /// <summary>
    /// Converts to an instance of JSON.
    /// </summary>
    /// <param name="obj">The optional JSON object instance to add properties.</param>
    /// <returns>A JSON object instance.</returns>
    public void ToJson(JsonObjectNode obj)
    {
        if (obj is null) return;
        obj.SetValue("x", X);
        obj.SetValue("y", Y);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
        => new IntPoint2D(X, Y);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(System.Drawing.Point other)
        => X == other.X && Y == other.Y;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(Point2D<double> other)
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
        if (other is Point2D<double> p2) return Equals(p2);
        if (other is System.Drawing.Point p3) return Equals(p3);
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
    public static explicit operator System.Drawing.Point(IntPoint2D p)
        => new(p.X, p.Y);

    /// <summary>
    /// Converts from point.
    /// </summary>
    /// <param name="p">The point value.</param>
    /// <returns>An instance of the point class.</returns>
    public static implicit operator IntPoint2D(System.Drawing.Point p)
        => new(p.X, p.Y);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static IntPoint2D operator +(IntPoint2D leftValue, IntPoint2D rightValue)
        => (leftValue ?? new IntPoint2D()).Plus(rightValue);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static IntPoint2D operator -(IntPoint2D leftValue, IntPoint2D rightValue)
        => (leftValue ?? new IntPoint2D()).Minus(rightValue);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// </summary>
    /// <param name="value">The value to negate.</param>
    /// <returns>A result after negation.</returns>
    public static IntPoint2D operator -(IntPoint2D value)
        => value?.Negate();
}

/// <summary>
/// Json point converter.
/// </summary>
sealed class IntPoint2DConverter : JsonConverter<IntPoint2D>
{
    /// <summary>
    /// Gets or sets a value indicating whether need also write to a string.
    /// </summary>
    public bool NeedWriteAsString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether need throw exception for null value.
    /// </summary>
    public bool NeedThrowForNull { get; set; }

    /// <inheritdoc />
    public override IntPoint2D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.Number => reader.TryGetInt32(out var integer) ? new IntPoint2D(integer, integer) : new IntPoint2D((int)reader.GetDouble(), (int)reader.GetDouble()),
                JsonTokenType.False => null,
                JsonTokenType.StartObject => ReadJson(ref reader),
                JsonTokenType.StartArray => ReadJsonArray(ref reader),
                _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON.")
            };
        }
        catch (InvalidCastException ex)
        {
            throw new JsonException($"The token format is not valid or it is out of range supported.", ex);
        }
        catch (FormatException ex)
        {
            throw new JsonException($"The token format is not valid.", ex);
        }
        catch (ArgumentException ex)
        {
            throw new JsonException($"The token format is not valid or it is out of range supported.", ex);
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IntPoint2D value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteNumber("x", value.X);
        writer.WriteNumber("y", value.Y);
        writer.WriteEndObject();
    }

    private static IntPoint2D ReadJson(ref Utf8JsonReader reader)
    {
        var json = JsonObjectNode.ParseValue(ref reader) ?? throw new JsonException("Cannot parse the JSON.");
        if (!json.TryGetInt32Value("x", out var x) && !json.TryGetInt32Value("X", out x))
            throw new JsonException("Expect a property x in the JSON.");
        if (!json.TryGetInt32Value("y", out var y) && !json.TryGetInt32Value("Y", out y))
            throw new JsonException("Expect a property y in the JSON.");
        return new(x, y);
    }

    private static IntPoint2D ReadJsonArray(ref Utf8JsonReader reader)
    {
        var json = JsonArrayNode.ParseValue(ref reader) ?? throw new JsonException("Cannot parse the JSON.");
        if (json.Count != 2) throw new JsonException("The count of the JSON array is not expected.");
        if (!json.TryGetInt32Value(0, out var num1) || !json.TryGetInt32Value(1, out var num2))
            throw new JsonException("The type of the array item is not expected.");
        return new(num1, num2);
    }
}

/// <summary>
/// Json point converter.
/// </summary>
sealed class DoublePoint2DConverter : JsonConverter<DoublePoint2D>
{
    /// <summary>
    /// Gets or sets a value indicating whether need also write to a string.
    /// </summary>
    public bool NeedWriteAsString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether need throw exception for null value.
    /// </summary>
    public bool NeedThrowForNull { get; set; }

    /// <inheritdoc />
    public override DoublePoint2D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.Number => reader.TryGetInt32(out var integer) ? new DoublePoint2D(integer, integer) : new DoublePoint2D(reader.GetDouble(), reader.GetDouble()),
                JsonTokenType.False => null,
                JsonTokenType.StartObject => ReadJson(ref reader),
                JsonTokenType.StartArray => ReadJsonArray(ref reader),
                _ => throw new JsonException($"The token type is {reader.TokenType} but expect a JSON.")
            };
        }
        catch (InvalidCastException ex)
        {
            throw new JsonException($"The token format is not valid or it is out of range supported.", ex);
        }
        catch (FormatException ex)
        {
            throw new JsonException($"The token format is not valid.", ex);
        }
        catch (ArgumentException ex)
        {
            throw new JsonException($"The token format is not valid or it is out of range supported.", ex);
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DoublePoint2D value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteNumber("x", value.X);
        writer.WriteNumber("y", value.Y);
        writer.WriteEndObject();
    }

    private static DoublePoint2D ReadJson(ref Utf8JsonReader reader)
    {
        var json = JsonObjectNode.ParseValue(ref reader);
        if (json == null) throw new JsonException("Cannot parse the JSON.");
        if (!json.TryGetDoubleValue("x", out var x) && !json.TryGetDoubleValue("X", out x))
            throw new JsonException("Expect a property x in the JSON.");
        if (!json.TryGetDoubleValue("y", out var y) && !json.TryGetDoubleValue("Y", out y))
            throw new JsonException("Expect a property y in the JSON.");
        return new(x, y);
    }

    private static DoublePoint2D ReadJsonArray(ref Utf8JsonReader reader)
    {
        var json = JsonArrayNode.ParseValue(ref reader);
        if (json == null) throw new JsonException("Cannot parse the JSON.");
        if (json.Count != 2) throw new JsonException("The count of the JSON array is not expected.");
        if (!json.TryGetDoubleValue(0, out var num1) || !json.TryGetDoubleValue(1, out var num2))
            throw new JsonException("The type of the array item is not expected.");
        return new(num1, num2);
    }
}
