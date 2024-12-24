// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinate\FourDimensional.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The 4D models.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Numerics;
using Trivial.Text;

namespace Trivial.Maths;

/// <summary>
/// The generic 4D (time and space) coordinate point.
/// </summary>
/// <typeparam name="TUnit">The type of unit.</typeparam>
public class Point4D<TUnit> : FourElements<TUnit>, IEquatable<Point4D<TUnit>>, IEquatable<FourElements<TUnit>> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
{
    /// <summary>
    /// Initializes a new instance of the FourDimensionalPoint class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Point4D()
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
    public Point4D(TUnit x, TUnit y, TUnit z, TUnit t)
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
    /// Gets or sets the value of T (time). The value is same as ItemD.
    /// </summary>
    public TUnit T
    {
        get => ItemD;
        set => ItemD = value;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(FourElements<TUnit> other)
        => other is not null && X.Equals(other.ItemA) && Y.Equals(other.ItemB) && Z.Equals(other.ItemC) && T.Equals(other.ItemD);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
    public virtual bool Equals(Point4D<TUnit> other)
        => other is not null && X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && T.Equals(other.T);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other is Point4D<TUnit> p) return Equals(p);
        if (other is FourElements<TUnit> e) return Equals(e);
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

        return string.Format("X = {0}{1} Y = {2}{1} Z = {3} T = {4}", x, sep ? ";" : ",", y, z, t);
    }
}

/// <summary>
/// The generic 4D (time and space) coordinate point.
/// </summary>
/// <typeparam name="TSpaceUnit">The type of space unit.</typeparam>
/// <typeparam name="TTimeUnit">The type of time unit.</typeparam>
public class Point4D<TSpaceUnit, TTimeUnit> : Point3D<TSpaceUnit>
    where TSpaceUnit : struct, IComparable<TSpaceUnit>, IEquatable<TSpaceUnit>
    where TTimeUnit : struct, IComparable<TTimeUnit>, IEquatable<TTimeUnit>
{
    /// <summary>
    /// Initializes a new instance of the Point4D class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Point4D()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Point4D class.
    /// </summary>
    /// <param name="x">The value of X.</param>
    /// <param name="y">The value of Y.</param>
    /// <param name="z">The value of Z.</param>
    /// <param name="t">The value of T.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Point4D(TSpaceUnit x, TSpaceUnit y, TSpaceUnit z, TTimeUnit t)
    {
        X = x;
        Y = y;
        Z = z;
        T = t;
    }

    /// <summary>
    /// Gets or sets the value of T (time). The value is same as ItemD.
    /// </summary>
    public TTimeUnit T { get; set; }

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
        if (longStr.IndexOfAny(new[] { ',', ';' }) > -1) sep = true;
        if (!sep && longStr.IndexOf(';') > -1)
        {
            const string quoteStr = "\"{0}\"";
            x = string.Format(quoteStr, x.Replace("\"", "\\\""));
            y = string.Format(quoteStr, y.Replace("\"", "\\\""));
            z = string.Format(quoteStr, z.Replace("\"", "\\\""));
            t = string.Format(quoteStr, t.Replace("\"", "\\\""));
        }

        return string.Format("X = {0}{1} Y = {2}{1} Z = {3} T = {4}", x, sep ? ";" : ",", y, z, t);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public virtual bool Equals(Point4D<TSpaceUnit, TTimeUnit> other)
        => other != null && X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && T.Equals(other.T);
}

/// <summary>
/// The point of the 4D (time and space) mathematics coordinate and date time.
/// </summary>
public sealed class SpacetimePoint : Point4D<double, DateTime>, IJsonObjectHost
#if NET8_0_OR_GREATER
    , IAdditionOperators<SpacetimePoint, RelativeSpacetimePoint, SpacetimePoint>, ISubtractionOperators<SpacetimePoint, RelativeSpacetimePoint, SpacetimePoint>, ISubtractionOperators<SpacetimePoint, SpacetimePoint, RelativeSpacetimePoint>, IUnaryNegationOperators<SpacetimePoint, SpacetimePoint>
#endif
{
    /// <summary>
    /// Initializes a new instance of the SpaceTimePoint class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public SpacetimePoint()
    {
    }

    /// <summary>
    /// Initializes a new instance of the SpaceTimePoint class.
    /// </summary>
    /// <param name="x">The value of X.</param>
    /// <param name="y">The value of Y.</param>
    /// <param name="z">The value of Z.</param>
    /// <param name="t">The value of T.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public SpacetimePoint(double x, double y, double z, DateTime t) : base(x, y, z, t)
    {
    }

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public SpacetimePoint Plus(Point4D<double, TimeSpan> value)
        => value != null ? new(X + value.X, Y + value.Y, Z + value.Z, T + value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public SpacetimePoint Plus(Point4D<float, TimeSpan> value)
        => value != null ? new(X + value.X, Y + value.Y, Z + value.Z, T + value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public SpacetimePoint Plus(Point4D<int, TimeSpan> value)
        => value != null ? new(X + value.X, Y + value.Y, Z + value.Z, T + value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public SpacetimePoint Minus(Point4D<double, TimeSpan> value)
        => value != null ? new(X - value.X, Y - value.Y, Z - value.Z, T - value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public SpacetimePoint Minus(Point4D<float, TimeSpan> value)
        => value != null ? new(X - value.X, Y - value.Y, Z - value.Z, T - value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public SpacetimePoint Minus(Point4D<int, TimeSpan> value)
        => value != null ? new(X - value.X, Y - value.Y, Z - value.Z, T - value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public RelativeSpacetimePoint Minus(Point4D<double, DateTime> value)
        => value != null ? new(X - value.X, Y - value.Y, Z - value.Z, T - value.T) : new(X, Y, Z, TimeSpan.Zero);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public RelativeSpacetimePoint Minus(Point4D<float, DateTime> value)
        => value != null ? new(X - value.X, Y - value.Y, Z - value.Z, T - value.T) : new(X, Y, Z, TimeSpan.Zero);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public RelativeSpacetimePoint Minus(Point4D<int, DateTime> value)
        => value != null ? new(X - value.X, Y - value.Y, Z - value.Z, T - value.T) : new(X, Y, Z, TimeSpan.Zero);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// -this
    /// </summary>
    /// <returns>A result after negation.</returns>
    public SpacetimePoint Negate()
        => new(-X, -Y, -Z, T);

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
        obj.SetValue("z", Z);
        obj.SetValue("t", T);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
        => new SpacetimePoint(X, Y, Z, T);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static SpacetimePoint operator +(SpacetimePoint leftValue, RelativeSpacetimePoint rightValue)
        => (leftValue ?? new()).Plus(rightValue);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static SpacetimePoint operator +(RelativeSpacetimePoint leftValue, SpacetimePoint rightValue)
        => (rightValue ?? new()).Plus(leftValue);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static SpacetimePoint operator -(SpacetimePoint leftValue, RelativeSpacetimePoint rightValue)
        => (leftValue ?? new()).Minus(rightValue);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static RelativeSpacetimePoint operator -(SpacetimePoint leftValue, SpacetimePoint rightValue)
        => (leftValue ?? new()).Minus(rightValue);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// </summary>
    /// <param name="value">The value to negate.</param>
    /// <returns>A result after negation.</returns>
    public static SpacetimePoint operator -(SpacetimePoint value)
        => value?.Negate();
}

/// <summary>
/// The point of the 4D (time and space) mathematics coordinate and time span.
/// </summary>
public sealed class RelativeSpacetimePoint : Point4D<double, TimeSpan>, IJsonObjectHost
#if NET8_0_OR_GREATER
    , IAdditionOperators<RelativeSpacetimePoint, RelativeSpacetimePoint, RelativeSpacetimePoint>, ISubtractionOperators<RelativeSpacetimePoint, RelativeSpacetimePoint, RelativeSpacetimePoint>, IUnaryNegationOperators<RelativeSpacetimePoint, RelativeSpacetimePoint>
#endif
{
    /// <summary>
    /// Initializes a new instance of the RelativeSpacetimePoint class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public RelativeSpacetimePoint()
    {
    }

    /// <summary>
    /// Initializes a new instance of the RelativeSpacetimePoint class.
    /// </summary>
    /// <param name="x">The value of X.</param>
    /// <param name="y">The value of Y.</param>
    /// <param name="z">The value of Z.</param>
    /// <param name="t">The value of T.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public RelativeSpacetimePoint(double x, double y, double z, TimeSpan t) : base(x, y, z, t)
    {
    }

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public RelativeSpacetimePoint Plus(Point4D<double, TimeSpan> value)
        => value != null ? new(X + value.X, Y + value.Y, Z + value.Z, T + value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public RelativeSpacetimePoint Plus(Point4D<float, TimeSpan> value)
        => value != null ? new(X + value.X, Y + value.Y, Z + value.Z, T + value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public RelativeSpacetimePoint Plus(Point4D<int, TimeSpan> value)
        => value != null ? new(X + value.X, Y + value.Y, Z + value.Z, T + value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">The value to be plused.</param>
    /// <returns>A result after leftValue plus rightValue.</returns>
    public RelativeSpacetimePoint Plus(RelativeSpacetimePoint value)
        => value != null ? new(X + value.X, Y + value.Y, Z + value.Z, T + value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public RelativeSpacetimePoint Minus(Point4D<double, TimeSpan> value)
        => value != null ? new(X - value.X, Y - value.Y, Z - value.Z, T - value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public RelativeSpacetimePoint Minus(Point4D<float, TimeSpan> value)
        => value != null ? new(X - value.X, Y - value.Y, Z - value.Z, T - value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public RelativeSpacetimePoint Minus(Point4D<int, TimeSpan> value)
        => value != null ? new(X - value.X, Y - value.Y, Z - value.Z, T - value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">The value to be minuses.</param>
    /// <returns>A result after leftValue minus rightValue.</returns>
    public RelativeSpacetimePoint Minus(RelativeSpacetimePoint value)
        => value != null ? new(X - value.X, Y - value.Y, Z - value.Z, T - value.T) : new(X, Y, Z, T);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// -this
    /// </summary>
    /// <returns>A result after negation.</returns>
    public RelativeSpacetimePoint Negate()
        => new(-X, -Y, -Z, -T);

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
        obj.SetValue("z", Z);
        try
        {
            obj.SetValue("t", (long)T.TotalMilliseconds);
        }
        catch (OverflowException)
        {
        }
        catch (InvalidCastException)
        {
        }
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
        => new RelativeSpacetimePoint(X, Y, Z, T);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static RelativeSpacetimePoint operator +(RelativeSpacetimePoint leftValue, RelativeSpacetimePoint rightValue)
        => (leftValue ?? new()).Plus(rightValue);

    /// <summary>
    /// Pluses two points in coordinate.
    /// </summary>
    /// <param name="leftValue">The left value for addition operator.</param>
    /// <param name="rightValue">The right value for addition operator.</param>
    /// <returns>A result after addition.</returns>
    public static RelativeSpacetimePoint operator -(RelativeSpacetimePoint leftValue, RelativeSpacetimePoint rightValue)
        => (leftValue ?? new()).Minus(rightValue);

    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// </summary>
    /// <param name="value">The value to negate.</param>
    /// <returns>A result after negation.</returns>
    public static RelativeSpacetimePoint operator -(RelativeSpacetimePoint value)
        => value?.Negate();
}
